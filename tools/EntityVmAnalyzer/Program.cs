using EntityVmAnalyzer.Analyzers;
using EntityVmAnalyzer.Generators;
using EntityVmAnalyzer.Models;

namespace EntityVmAnalyzer;

class Program
{
    static async Task Main(string[] args)
    {
        Console.WriteLine("Entity-ViewModel Property Mapping Analyzer");
        Console.WriteLine("==========================================");
        Console.WriteLine();

        try
        {
            // Auto-detect paths or use provided CLI args
            (string entitiesDirectory, string viewModelsDirectory, string outputDirectory) = ResolvePaths(args);

            // Validate directories
            if (!Directory.Exists(entitiesDirectory))
            {
                Console.WriteLine($"Error: Entities directory not found: {entitiesDirectory}");
                Console.WriteLine("Please provide the correct path to the CollectionApp.Domain/Entities directory.");
                return;
            }

            if (!Directory.Exists(viewModelsDirectory))
            {
                Console.WriteLine($"Error: ViewModels directory not found: {viewModelsDirectory}");
                Console.WriteLine("Please provide the correct path to the CollectionApp.Application/ViewModels directory.");
                return;
            }

            Console.WriteLine($"Entities Directory: {Path.GetFullPath(entitiesDirectory)}");
            Console.WriteLine($"ViewModels Directory: {Path.GetFullPath(viewModelsDirectory)}");
            Console.WriteLine($"Output Directory: {Path.GetFullPath(outputDirectory)}");
            Console.WriteLine();

            // Step 1: Analyze Entities
            Console.WriteLine("Step 1: Analyzing Entities...");
            var entityAnalyzer = new EntityAnalyzer();
            var entities = await entityAnalyzer.AnalyzeEntitiesAsync(entitiesDirectory);
            Console.WriteLine($"Found {entities.Count} entities:");
            foreach (var entity in entities)
            {
                Console.WriteLine($"  - {entity.Name} ({entity.Properties.Count} properties)");
            }
            Console.WriteLine();

            // Step 2: Analyze ViewModels
            Console.WriteLine("Step 2: Analyzing ViewModels...");
            var viewModelAnalyzer = new ViewModelAnalyzer();
            
            // Pass entity type names for better navigation property detection
            var entityTypeNames = entities.Select(e => e.Name).ToHashSet();
            viewModelAnalyzer.SetEntityTypeNames(entityTypeNames);
            
            var viewModels = await viewModelAnalyzer.AnalyzeViewModelsAsync(viewModelsDirectory);
            Console.WriteLine($"Found {viewModels.Count} ViewModels:");
            
            var viewModelsByCategory = viewModels.GroupBy(vm => vm.Category);
            foreach (var category in viewModelsByCategory)
            {
                Console.WriteLine($"  - {category.Key}: {category.Count()} ViewModels");
                foreach (var vm in category)
                {
                    Console.WriteLine($"    * {vm.Name} ({vm.Properties.Count} properties)");
                }
            }
            Console.WriteLine();

            // Step 3: Analyze Mappings
            Console.WriteLine("Step 3: Analyzing Entity-ViewModel Mappings...");
            var mappingAnalyzer = new MappingAnalyzer();
            var mappingResults = mappingAnalyzer.AnalyzeMappings(entities, viewModels);
            Console.WriteLine($"Generated {mappingResults.Count} mapping results");
            Console.WriteLine();

            // Step 4: Generate Analysis Summary
            Console.WriteLine("Step 4: Generating Analysis Summary...");
            var analysisSummary = mappingAnalyzer.GenerateAnalysisSummary(entities, viewModels, mappingResults);
            Console.WriteLine("Analysis summary generated");
            Console.WriteLine();

            // Step 5: Generate Reports
            Console.WriteLine("Step 5: Generating Reports...");
            var reportGenerator = new ReportGenerator(outputDirectory);
            await reportGenerator.GenerateAllReportsAsync(analysisSummary);
            Console.WriteLine("All reports generated successfully!");
            Console.WriteLine();

            // Step 6: Display Summary Statistics
            Console.WriteLine("Analysis Complete!");
            Console.WriteLine("=================");
            Console.WriteLine();
            
            var totalMissingInEntity = analysisSummary.MissingPropertiesByEntity.Values.Sum(x => x.Count);
            var totalMissingInViewModel = mappingResults.Sum(m => m.MissingInViewModel.Count);
            var totalTypeMismatches = mappingResults.Sum(m => m.TypeMismatches.Count);
            
            Console.WriteLine("Summary Statistics:");
            Console.WriteLine($"  - Total Entities: {entities.Count}");
            Console.WriteLine($"  - Total ViewModels: {viewModels.Count}");
            Console.WriteLine($"  - Total Mappings: {mappingResults.Count}");
            Console.WriteLine($"  - Properties Missing in Entities: {totalMissingInEntity}");
            Console.WriteLine($"  - Properties Missing in ViewModels: {totalMissingInViewModel}");
            Console.WriteLine($"  - Type Mismatches: {totalTypeMismatches}");
            Console.WriteLine($"  - Value Object Flattening Patterns: {analysisSummary.ValueObjectFlatteningPatterns["All"].Count}");
            Console.WriteLine($"  - Computed Properties: {analysisSummary.ComputedPropertyPatterns["All"].Count}");
            Console.WriteLine();

            if (totalMissingInEntity > 0)
            {
                Console.WriteLine("Critical Issues Found:");
                Console.WriteLine("=====================");
                foreach (var entity in entities)
                {
                    if (analysisSummary.MissingPropertiesByEntity.ContainsKey(entity.Name) && 
                        analysisSummary.MissingPropertiesByEntity[entity.Name].Any())
                    {
                        Console.WriteLine($"  {entity.Name} Entity:");
                        foreach (var missingProp in analysisSummary.MissingPropertiesByEntity[entity.Name])
                        {
                            Console.WriteLine($"    - Missing: {missingProp}");
                        }
                    }
                }
                Console.WriteLine();
            }

            Console.WriteLine($"Reports generated in: {Path.GetFullPath(outputDirectory)}");
            Console.WriteLine("Files generated:");
            Console.WriteLine("  - property-comparison-report.md (Comprehensive analysis)");
            Console.WriteLine("  - missing-properties-summary.md (Focus on missing properties)");
            Console.WriteLine("  - mapping-matrix.json (Machine-readable data)");
            Console.WriteLine("  - property-details.csv (Spreadsheet-friendly format)");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error during analysis: {ex.Message}");
            Console.WriteLine(ex.StackTrace);
            Environment.Exit(1);
        }
    }

    private static (string entitiesDir, string viewModelsDir, string outputDir) ResolvePaths(string[] args)
    {
        // Override with command line arguments if provided
        if (args.Length >= 3)
        {
            return (args[0], args[1], args[2]);
        }

        // Auto-detect paths by walking up from current directory
        var currentDir = AppContext.BaseDirectory;
        var solutionRoot = FindSolutionRoot(currentDir);

        if (solutionRoot != null)
        {
            var entitiesDir = Path.Combine(solutionRoot, "CollectionApp.Domain", "Entities");
            var viewModelsDir = Path.Combine(solutionRoot, "CollectionApp.Application", "ViewModels");
            var outputDir = Path.Combine(solutionRoot, "docs", "analysis", "entity-vm-mapping");

            Console.WriteLine($"Auto-detected solution root: {solutionRoot}");
            Console.WriteLine($"Attempting to use paths:");
            Console.WriteLine($"  Entities: {entitiesDir}");
            Console.WriteLine($"  ViewModels: {viewModelsDir}");
            Console.WriteLine($"  Output: {outputDir}");

            return (entitiesDir, viewModelsDir, outputDir);
        }

        // Fallback to default relative paths
        Console.WriteLine("Warning: Could not auto-detect solution root, using fallback paths");
        var fallbackEntitiesDir = Path.Combine("..", "..", "..", "..", "CollectionApp.Domain", "Entities");
        var fallbackViewModelsDir = Path.Combine("..", "..", "..", "..", "CollectionApp.Application", "ViewModels");
        var fallbackOutputDir = Path.Combine("..", "..", "..", "..", "docs", "analysis", "entity-vm-mapping");

        return (fallbackEntitiesDir, fallbackViewModelsDir, fallbackOutputDir);
    }

    private static string? FindSolutionRoot(string startDirectory)
    {
        var currentDir = new DirectoryInfo(startDirectory);
        
        while (currentDir != null)
        {
            // Check if this directory contains a .sln file
            if (currentDir.GetFiles("*.sln").Any())
            {
                return currentDir.FullName;
            }

            // Check if this directory contains the expected project structure
            var entitiesDir = Path.Combine(currentDir.FullName, "CollectionApp.Domain", "Entities");
            var viewModelsDir = Path.Combine(currentDir.FullName, "CollectionApp.Application", "ViewModels");
            
            if (Directory.Exists(entitiesDir) && Directory.Exists(viewModelsDir))
            {
                return currentDir.FullName;
            }

            currentDir = currentDir.Parent;
        }

        return null;
    }
}
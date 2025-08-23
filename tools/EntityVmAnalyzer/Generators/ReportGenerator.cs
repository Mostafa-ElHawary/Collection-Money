using System.Text.Json;
using CsvHelper;
using EntityVmAnalyzer.Models;

namespace EntityVmAnalyzer.Generators;

public class ReportGenerator
{
    private readonly string _outputDirectory;

    public ReportGenerator(string outputDirectory)
    {
        _outputDirectory = outputDirectory;
        Directory.CreateDirectory(outputDirectory);
    }

    public async Task GenerateAllReportsAsync(AnalysisSummary summary)
    {
        await GenerateMarkdownReportAsync(summary);
        await GenerateJsonReportAsync(summary);
        await GenerateCsvReportAsync(summary);
        await GenerateMissingPropertiesReportAsync(summary);
    }

    public async Task GenerateMarkdownReportAsync(AnalysisSummary summary)
    {
        var reportPath = Path.Combine(_outputDirectory, "property-comparison-report.md");
        var content = GenerateMarkdownContent(summary);
        await File.WriteAllTextAsync(reportPath, content);
        Console.WriteLine($"Markdown report generated: {reportPath}");
    }

    public async Task GenerateJsonReportAsync(AnalysisSummary summary)
    {
        var reportPath = Path.Combine(_outputDirectory, "mapping-matrix.json");
        var options = new JsonSerializerOptions { WriteIndented = true };
        var json = JsonSerializer.Serialize(summary, options);
        await File.WriteAllTextAsync(reportPath, json);
        Console.WriteLine($"JSON report generated: {reportPath}");
    }

    public async Task GenerateCsvReportAsync(AnalysisSummary summary)
    {
        var reportPath = Path.Combine(_outputDirectory, "property-details.csv");
        
        using var writer = new StringWriter();
        using var csv = new CsvWriter(writer, System.Globalization.CultureInfo.InvariantCulture);
        
        // Write header
        csv.WriteField("EntityName");
        csv.WriteField("EntityProperty");
        csv.WriteField("EntityPropertyType");
        csv.WriteField("ViewModelType");
        csv.WriteField("ViewModelProperty");
        csv.WriteField("ViewModelPropertyType");
        csv.WriteField("MappingStatus");
        csv.WriteField("Notes");
        csv.NextRecord();

        // Write data
        foreach (var mappingResult in summary.MappingResults)
        {
            foreach (var mapping in mappingResult.Mappings)
            {
                csv.WriteField(mappingResult.EntityName);
                csv.WriteField(mapping.EntityProperty);
                csv.WriteField(mapping.EntityPropertyType);
                csv.WriteField($"{mappingResult.ViewModelName} ({mappingResult.ViewModelCategory})");
                csv.WriteField(mapping.ViewModelProperty);
                csv.WriteField(mapping.ViewModelPropertyType);
                csv.WriteField(mapping.Status.ToString());
                csv.WriteField(mapping.Notes);
                csv.NextRecord();
            }
        }

        await File.WriteAllTextAsync(reportPath, writer.ToString());
        Console.WriteLine($"CSV report generated: {reportPath}");
    }

    public async Task GenerateMissingPropertiesReportAsync(AnalysisSummary summary)
    {
        var reportPath = Path.Combine(_outputDirectory, "missing-properties-summary.md");
        var content = GenerateMissingPropertiesContent(summary);
        await File.WriteAllTextAsync(reportPath, content);
        Console.WriteLine($"Missing properties report generated: {reportPath}");
    }

    private string GenerateMarkdownContent(AnalysisSummary summary)
    {
        var content = new System.Text.StringBuilder();

        // Header
        content.AppendLine("# Entity-ViewModel Property Mapping Analysis Report");
        content.AppendLine();
        content.AppendLine($"**Analysis Date:** {summary.AnalysisDate:yyyy-MM-dd HH:mm:ss}");
        content.AppendLine($"**Total Entities:** {summary.Entities.Count}");
        content.AppendLine($"**Total ViewModels:** {summary.ViewModels.Count}");
        content.AppendLine($"**Total Mappings:** {summary.MappingResults.Count}");
        content.AppendLine();

        // Executive Summary
        content.AppendLine("## Executive Summary");
        content.AppendLine();
        
        var totalMissingInEntity = summary.MissingPropertiesByEntity.Values.Sum(x => x.Count);
        var totalMissingInViewModel = summary.MappingResults.Sum(m => m.MissingInViewModel.Count);
        var totalTypeMismatches = summary.MappingResults.Sum(m => m.TypeMismatches.Count);
        
        content.AppendLine($"- **Properties Missing in Entities:** {totalMissingInEntity}");
        content.AppendLine($"- **Properties Missing in ViewModels:** {totalMissingInViewModel}");
        content.AppendLine($"- **Type Mismatches:** {totalTypeMismatches}");
        content.AppendLine($"- **Value Object Flattening Patterns:** {summary.ValueObjectFlatteningPatterns["All"].Count}");
        content.AppendLine($"- **Computed Properties:** {summary.ComputedPropertyPatterns["All"].Count}");
        content.AppendLine();

        // Detailed Analysis by Entity
        foreach (var entity in summary.Entities)
        {
            content.AppendLine($"## {entity.Name} Entity");
            content.AppendLine();
            content.AppendLine($"**File:** `{entity.FilePath}`");
            content.AppendLine($"**Properties:** {entity.Properties.Count}");
            content.AppendLine();

            // Entity Properties Table
            content.AppendLine("### Entity Properties");
            content.AppendLine();
            content.AppendLine("| Property | Type | Nullable | Collection | Value Object | Navigation |");
            content.AppendLine("|----------|------|----------|------------|--------------|------------|");
            
            foreach (var prop in entity.Properties)
            {
                content.AppendLine($"| {prop.Name} | {prop.Type} | {prop.IsNullable} | {prop.IsCollection} | {prop.IsValueObject} | {prop.IsNavigation} |");
            }
            content.AppendLine();

            // ViewModel Mappings
            var entityMappings = summary.MappingResults.Where(m => m.EntityName == entity.Name).ToList();
            if (entityMappings.Any())
            {
                content.AppendLine("### ViewModel Mappings");
                content.AppendLine();
                
                foreach (var mapping in entityMappings)
                {
                    content.AppendLine($"#### {mapping.ViewModelName} ({mapping.ViewModelCategory})");
                    content.AppendLine();
                    
                    content.AppendLine("| Entity Property | ViewModel Property | Status | Notes |");
                    content.AppendLine("|----------------|-------------------|--------|-------|");
                    
                    foreach (var propMapping in mapping.Mappings)
                    {
                        var entityProp = string.IsNullOrEmpty(propMapping.EntityProperty) ? "-" : propMapping.EntityProperty;
                        var vmProp = string.IsNullOrEmpty(propMapping.ViewModelProperty) ? "-" : propMapping.ViewModelProperty;
                        content.AppendLine($"| {entityProp} | {vmProp} | {propMapping.Status} | {propMapping.Notes} |");
                    }
                    content.AppendLine();
                }
            }

            // Missing Properties Summary
            if (summary.MissingPropertiesByEntity.ContainsKey(entity.Name) && 
                summary.MissingPropertiesByEntity[entity.Name].Any())
            {
                content.AppendLine("### Missing Properties in Entity");
                content.AppendLine();
                content.AppendLine("The following properties are referenced in ViewModels but missing from the entity:");
                content.AppendLine();
                
                foreach (var missingProp in summary.MissingPropertiesByEntity[entity.Name])
                {
                    content.AppendLine($"- **{missingProp}** - Referenced in ViewModels but not defined in entity");
                }
                content.AppendLine();
            }
        }

        // Value Object Flattening Analysis
        content.AppendLine("## Value Object Flattening Analysis");
        content.AppendLine();
        content.AppendLine("The following properties represent flattened value objects:");
        content.AppendLine();
        
        foreach (var flattenedProp in summary.ValueObjectFlatteningPatterns["All"])
        {
            content.AppendLine($"- {flattenedProp}");
        }
        content.AppendLine();

        // Computed Properties Analysis
        content.AppendLine("## Computed Properties Analysis");
        content.AppendLine();
        content.AppendLine("The following properties are computed/derived and not expected to exist in entities:");
        content.AppendLine();
        
        foreach (var computedProp in summary.ComputedPropertyPatterns["All"])
        {
            content.AppendLine($"- {computedProp}");
        }
        content.AppendLine();

        // Recommendations
        content.AppendLine("## Recommendations");
        content.AppendLine();
        content.AppendLine("### Critical Issues to Address:");
        content.AppendLine();
        
        foreach (var entity in summary.Entities)
        {
            if (summary.MissingPropertiesByEntity.ContainsKey(entity.Name) && 
                summary.MissingPropertiesByEntity[entity.Name].Any())
            {
                content.AppendLine($"**{entity.Name} Entity:**");
                foreach (var missingProp in summary.MissingPropertiesByEntity[entity.Name])
                {
                    content.AppendLine($"- Add missing property `{missingProp}` to support ViewModel requirements");
                }
                content.AppendLine();
            }
        }

        content.AppendLine("### Design Patterns to Maintain:");
        content.AppendLine("- Value object flattening for UI presentation");
        content.AppendLine("- Computed properties for derived data");
        content.AppendLine("- Navigation property mappings for related data");

        return content.ToString();
    }

    private string GenerateMissingPropertiesContent(AnalysisSummary summary)
    {
        var content = new System.Text.StringBuilder();

        content.AppendLine("# Missing Properties Summary Report");
        content.AppendLine();
        content.AppendLine($"**Analysis Date:** {summary.AnalysisDate:yyyy-MM-dd HH:mm:ss}");
        content.AppendLine();

        content.AppendLine("## Critical Missing Properties by Entity");
        content.AppendLine();

        foreach (var entity in summary.Entities)
        {
            if (summary.MissingPropertiesByEntity.ContainsKey(entity.Name) && 
                summary.MissingPropertiesByEntity[entity.Name].Any())
            {
                content.AppendLine($"### {entity.Name}");
                content.AppendLine();
                content.AppendLine("**Missing Properties:**");
                content.AppendLine();
                
                foreach (var missingProp in summary.MissingPropertiesByEntity[entity.Name])
                {
                    content.AppendLine($"- **{missingProp}**");
                    
                    // Find which ViewModels reference this property
                    var referencingViewModels = summary.MappingResults
                        .Where(m => m.EntityName == entity.Name && m.MissingInEntity.Contains(missingProp))
                        .Select(m => m.ViewModelName)
                        .Distinct();
                    
                    if (referencingViewModels.Any())
                    {
                        content.AppendLine($"  - Referenced in: {string.Join(", ", referencingViewModels)}");
                    }
                    content.AppendLine();
                }
            }
        }

        content.AppendLine("## Priority Recommendations");
        content.AppendLine();
        content.AppendLine("### High Priority (Critical for Functionality)");
        content.AppendLine("- Properties referenced in Create/Update ViewModels");
        content.AppendLine("- Properties used in business logic or validation");
        content.AppendLine();
        content.AppendLine("### Medium Priority (UI Enhancement)");
        content.AppendLine("- Properties used in Detail/List ViewModels");
        content.AppendLine("- Properties for display formatting");
        content.AppendLine();
        content.AppendLine("### Low Priority (Nice to Have)");
        content.AppendLine("- Properties used only in analytics ViewModels");
        content.AppendLine("- Properties for advanced filtering");

        return content.ToString();
    }
} 
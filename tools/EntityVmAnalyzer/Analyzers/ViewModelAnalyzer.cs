using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EntityVmAnalyzer.Models;

namespace EntityVmAnalyzer.Analyzers;

public class ViewModelAnalyzer
{
    private readonly List<string> _computedPropertyPatterns = new()
    {
        "Display", "Name", "Full", "Total", "Count", "Summary", "Formatted"
    };

    private readonly List<string> _flattenedValueObjectPatterns = new()
    {
        "Phone", "Address", "Money", "CountryCode", "AreaCode", "Street", "City", "State", "ZipCode", "Amount", "Currency"
    };

    private HashSet<string> _entityTypeNames = new();

    public async Task<List<ViewModelInfo>> AnalyzeViewModelsAsync(string viewModelsDirectory)
    {
        var viewModels = new List<ViewModelInfo>();
        var viewModelFiles = Directory.GetFiles(viewModelsDirectory, "*.cs", SearchOption.AllDirectories);

        foreach (var file in viewModelFiles)
        {
            var viewModelInfos = await AnalyzeViewModelFileAsync(file);
            viewModels.AddRange(viewModelInfos);
        }

        return viewModels;
    }

    public void SetEntityTypeNames(HashSet<string> entityTypeNames)
    {
        _entityTypeNames = entityTypeNames;
    }

    private async Task<List<ViewModelInfo>> AnalyzeViewModelFileAsync(string filePath)
    {
        var viewModels = new List<ViewModelInfo>();
        
        try
        {
            var sourceCode = await File.ReadAllTextAsync(filePath);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = await syntaxTree.GetRootAsync();

            var classDeclarations = root.DescendantNodes().OfType<ClassDeclarationSyntax>();

            foreach (var classDeclaration in classDeclarations)
            {
                var className = classDeclaration.Identifier.ValueText;
                if (IsViewModelClass(className))
                {
                    var viewModelInfo = new ViewModelInfo
                    {
                        Name = className,
                        FilePath = filePath,
                        Category = DetermineViewModelCategory(className)
                    };

                    // Extract base classes and interfaces
                    if (classDeclaration.BaseList != null)
                    {
                        foreach (var baseType in classDeclaration.BaseList.Types)
                        {
                            var baseTypeName = baseType.Type.ToString();
                            
                            // Use pattern matching for base class vs interface classification
                            if (baseTypeName.Contains("BaseEntity") || 
                                baseTypeName.Contains("Entity") ||
                                baseTypeName.Contains("Model"))
                            {
                                viewModelInfo.BaseClasses.Add(baseTypeName);
                            }
                            else if (baseTypeName.StartsWith("I") && 
                                     baseTypeName.Length > 1 && 
                                     char.IsUpper(baseTypeName[1]))
                            {
                                // Convention: interfaces start with 'I' followed by uppercase
                                viewModelInfo.Interfaces.Add(baseTypeName);
                            }
                            else
                            {
                                // Default to base class if uncertain
                                viewModelInfo.BaseClasses.Add(baseTypeName);
                            }
                        }
                    }

                    // Extract properties
                    var properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>();
                    foreach (var property in properties)
                    {
                        if (property.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                        {
                            var propertyInfo = ExtractViewModelPropertyInfo(property, filePath);
                            viewModelInfo.Properties.Add(propertyInfo);
                        }
                    }

                    viewModels.Add(viewModelInfo);
                }
            }

            return viewModels;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error analyzing ViewModel file {filePath}: {ex.Message}");
            return new List<ViewModelInfo>();
        }
    }

    private bool IsViewModelClass(string className)
    {
        return className.EndsWith("VM") || 
               className.EndsWith("ViewModel") || 
               className.Contains("Create") || 
               className.Contains("Update") || 
               className.Contains("Detail") || 
               className.Contains("List") ||
               className.Contains("Analytics");
    }

    private string DetermineViewModelCategory(string className)
    {
        if (className.Contains("Create")) return "Create";
        if (className.Contains("Update")) return "Update";
        if (className.Contains("Detail")) return "Detail";
        if (className.Contains("List")) return "List";
        if (className.Contains("Analytics")) return "Analytics";
        if (className.Contains("Summary")) return "Summary";
        return "General";
    }

    private PropertyInfo ExtractViewModelPropertyInfo(PropertyDeclarationSyntax property, string filePath)
    {
        var propertyName = property.Identifier.ValueText;
        var type = property.Type?.ToString() ?? "unknown";
        var isNullable = type.EndsWith("?");
        var isCollection = type.Contains("List<") || type.Contains("ICollection<") || type.Contains("IEnumerable<");
        var isComputed = IsComputedProperty(propertyName);
        var isFlattened = IsFlattenedValueObjectProperty(propertyName);
        var isNavigation = IsNavigationProperty(property, type);

        // Get source location
        var location = property.GetLocation();
        var lineSpan = location.GetLineSpan();
        var sourceLocation = $"{Path.GetFileName(filePath)}:{lineSpan.StartLinePosition.Line + 1}";

        return new PropertyInfo
        {
            Name = propertyName,
            Type = type,
            IsNullable = isNullable,
            IsCollection = isCollection,
            IsValueObject = false, // ViewModels don't have value objects directly
            IsNavigation = isNavigation,
            IsComputed = isComputed,
            IsFlattened = isFlattened,
            SourceLocation = sourceLocation,
            Attributes = property.AttributeLists
                .SelectMany(al => al.Attributes)
                .Select(a => a.Name.ToString())
                .ToList()
        };
    }

    private bool IsComputedProperty(string propertyName)
    {
        return _computedPropertyPatterns.Any(pattern => 
            propertyName.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsFlattenedValueObjectProperty(string propertyName)
    {
        return _flattenedValueObjectPatterns.Any(pattern => 
            propertyName.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    private bool IsNavigationProperty(PropertyDeclarationSyntax property, string type)
    {
        // Check if it's a collection of entities
        if (type.Contains("List<") || type.Contains("ICollection<") || type.Contains("IEnumerable<"))
        {
            var elementType = ExtractElementType(type);
            return _entityTypeNames.Contains(elementType);
        }

        // Check if it's a single entity reference
        return _entityTypeNames.Contains(type.Replace("?", "").Trim());
    }

    private string ExtractElementType(string collectionType)
    {
        if (collectionType.Contains("<"))
        {
            var start = collectionType.IndexOf('<') + 1;
            var end = collectionType.LastIndexOf('>');
            return end > start ? collectionType[start..end] : collectionType;
        }
        return collectionType;
    }
} 
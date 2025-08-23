using Microsoft.CodeAnalysis;
using Microsoft.CodeAnalysis.CSharp;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using EntityVmAnalyzer.Models;

namespace EntityVmAnalyzer.Analyzers;

public class EntityAnalyzer
{
    private readonly List<string> _valueObjectTypes = new()
    {
        "Address", "Phone", "Money"
    };

    private readonly HashSet<string> _entityTypeNames = new();

    public async Task<List<EntityInfo>> AnalyzeEntitiesAsync(string entitiesDirectory)
    {
        var entities = new List<EntityInfo>();
        var entityFiles = Directory.GetFiles(entitiesDirectory, "*.cs", SearchOption.AllDirectories);

        foreach (var file in entityFiles)
        {
            var entityInfo = await AnalyzeEntityFileAsync(file);
            if (entityInfo != null)
            {
                entities.Add(entityInfo);
                _entityTypeNames.Add(entityInfo.Name);
            }
        }

        return entities;
    }

    private async Task<EntityInfo?> AnalyzeEntityFileAsync(string filePath)
    {
        try
        {
            var sourceCode = await File.ReadAllTextAsync(filePath);
            var syntaxTree = CSharpSyntaxTree.ParseText(sourceCode);
            var root = await syntaxTree.GetRootAsync();

            var classDeclaration = root.DescendantNodes()
                .OfType<ClassDeclarationSyntax>()
                .FirstOrDefault();

            if (classDeclaration == null) return null;

            var entityInfo = new EntityInfo
            {
                Name = classDeclaration.Identifier.ValueText,
                FilePath = filePath
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
                        entityInfo.BaseClasses.Add(baseTypeName);
                    }
                    else if (baseTypeName.StartsWith("I") && 
                             baseTypeName.Length > 1 && 
                             char.IsUpper(baseTypeName[1]))
                    {
                        // Convention: interfaces start with 'I' followed by uppercase
                        entityInfo.Interfaces.Add(baseTypeName);
                    }
                    else
                    {
                        // Default to base class if uncertain
                        entityInfo.BaseClasses.Add(baseTypeName);
                    }
                }
            }

            // Extract properties including inherited ones
            var properties = classDeclaration.Members.OfType<PropertyDeclarationSyntax>();
            foreach (var property in properties)
            {
                if (property.Modifiers.Any(m => m.IsKind(SyntaxKind.PublicKeyword)))
                {
                    var propertyInfo = ExtractPropertyInfo(property, filePath);
                    
                    // Include all properties but mark inherited ones
                    if (AnalysisConstants.BaseEntityPropertyNames.Contains(propertyInfo.Name))
                    {
                        propertyInfo.IsInherited = true;
                    }
                    
                    entityInfo.Properties.Add(propertyInfo);
                }
            }

            return entityInfo;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error analyzing entity file {filePath}: {ex.Message}");
            return null;
        }
    }

    private PropertyInfo ExtractPropertyInfo(PropertyDeclarationSyntax property, string filePath)
    {
        var type = property.Type?.ToString() ?? "unknown";
        var isNullable = type.EndsWith("?");
        var isCollection = type.Contains("List<") || type.Contains("ICollection<") || type.Contains("IEnumerable<");
        var isValueObject = _valueObjectTypes.Any(vo => type.Contains(vo));
        var isNavigation = IsNavigationProperty(property, type);

        // Get source location
        var location = property.GetLocation();
        var lineSpan = location.GetLineSpan();
        var sourceLocation = $"{Path.GetFileName(filePath)}:{lineSpan.StartLinePosition.Line + 1}";

        return new PropertyInfo
        {
            Name = property.Identifier.ValueText,
            Type = type,
            IsNullable = isNullable,
            IsCollection = isCollection,
            IsValueObject = isValueObject,
            IsNavigation = isNavigation,
            SourceLocation = sourceLocation,
            Attributes = property.AttributeLists
                .SelectMany(al => al.Attributes)
                .Select(a => a.Name.ToString())
                .ToList()
        };
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
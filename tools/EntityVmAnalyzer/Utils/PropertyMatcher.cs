using EntityVmAnalyzer.Models;

namespace EntityVmAnalyzer.Utils;

public static class PropertyMatcher
{
    private static readonly Dictionary<string, List<string>> ValueObjectPropertyMappings = new()
    {
        ["Address"] = new List<string> { "Street", "City", "State", "ZipCode", "Country" },
        ["Phone"] = new List<string> { "CountryCode", "AreaCode", "Number" },
        ["Money"] = new List<string> { "Amount", "Currency" }
    };

    public static PropertyMapping CreatePropertyMapping(
        PropertyInfo entityProperty, 
        PropertyInfo viewModelProperty,
        string entityName,
        string viewModelName)
    {
        var mapping = new PropertyMapping
        {
            EntityProperty = entityProperty.Name,
            ViewModelProperty = viewModelProperty.Name,
            EntityPropertyType = entityProperty.Type,
            ViewModelPropertyType = viewModelProperty.Type,
            Status = MappingStatus.Matched,
            Notes = string.Empty
        };

        // Check for value object flattening
        if (IsValueObjectFlattening(entityProperty, viewModelProperty))
        {
            mapping.Status = MappingStatus.Flattened;
            mapping.IsValueObjectFlattening = true;
            mapping.Notes = $"Value object {entityProperty.Type} flattened to {viewModelProperty.Name}";
        }
        // Check for computed properties
        else if (viewModelProperty.IsComputed)
        {
            mapping.Status = MappingStatus.Derived;
            mapping.IsComputedProperty = true;
            mapping.Notes = "Computed/derived property";
        }
        // Check for navigation properties
        else if (entityProperty.IsNavigation || viewModelProperty.IsNavigation)
        {
            mapping.Status = MappingStatus.Navigation;
            mapping.IsNavigationMapping = true;
            mapping.Notes = "Navigation property mapping";
        }
        // Check for type mismatches
        else if (!AreTypesCompatible(entityProperty.Type, viewModelProperty.Type))
        {
            mapping.Status = MappingStatus.TypeMismatch;
            mapping.Notes = $"Type mismatch: {entityProperty.Type} vs {viewModelProperty.Type}";
        }

        return mapping;
    }

    public static bool IsValueObjectFlattening(PropertyInfo entityProperty, PropertyInfo viewModelProperty)
    {
        if (!entityProperty.IsValueObject) return false;

        var valueObjectType = entityProperty.Type.Replace("?", "").Trim();
        if (!ValueObjectPropertyMappings.ContainsKey(valueObjectType)) return false;

        var expectedProperties = ValueObjectPropertyMappings[valueObjectType];
        return expectedProperties.Any(prop => viewModelProperty.Name.Contains(prop));
    }

    public static bool IsComputedProperty(string propertyName)
    {
        var computedPatterns = new[]
        {
            "Display", "Name", "Full", "Total", "Count", "Summary", 
            "Formatted", "Calculated", "Derived", "Computed"
        };

        return computedPatterns.Any(pattern => 
            propertyName.Contains(pattern, StringComparison.OrdinalIgnoreCase));
    }

    public static bool AreTypesCompatible(string entityType, string viewModelType)
    {
        // Normalize types for comparison
        var normalizedEntityType = NormalizeType(entityType);
        var normalizedViewModelType = NormalizeType(viewModelType);

        // Direct match
        if (normalizedEntityType == normalizedViewModelType) return true;

        // Handle nullable types
        if (normalizedEntityType.EndsWith("?") && normalizedViewModelType == normalizedEntityType.TrimEnd('?'))
            return true;

        if (normalizedViewModelType.EndsWith("?") && normalizedEntityType == normalizedViewModelType.TrimEnd('?'))
            return true;

        // Handle collection types
        if (IsCollectionType(normalizedEntityType) && IsCollectionType(normalizedViewModelType))
        {
            var entityElementType = ExtractElementType(normalizedEntityType);
            var viewModelElementType = ExtractElementType(normalizedViewModelType);
            return AreTypesCompatible(entityElementType, viewModelElementType);
        }

        // Handle primitive type conversions
        return ArePrimitiveTypesCompatible(normalizedEntityType, normalizedViewModelType);
    }

    private static string NormalizeType(string type)
    {
        return type.Replace("System.", "").Replace("Collections.Generic.", "").Trim();
    }

    private static bool IsCollectionType(string type)
    {
        return type.Contains("List<") || type.Contains("ICollection<") || 
               type.Contains("IEnumerable<") || type.Contains("[]");
    }

    private static string ExtractElementType(string collectionType)
    {
        if (collectionType.Contains("<"))
        {
            var start = collectionType.IndexOf('<') + 1;
            var end = collectionType.LastIndexOf('>');
            return end > start ? collectionType[start..end] : collectionType;
        }
        if (collectionType.EndsWith("[]"))
        {
            return collectionType[..^2];
        }
        return collectionType;
    }

    private static bool ArePrimitiveTypesCompatible(string type1, string type2)
    {
        var compatibleGroups = new[]
        {
            new[] { "int", "long", "decimal", "double", "float" },
            new[] { "string", "char" },
            new[] { "bool" },
            new[] { "DateTime", "DateOnly" },
            new[] { "Guid" }
        };

        foreach (var group in compatibleGroups)
        {
            if (group.Contains(type1) && group.Contains(type2))
                return true;
        }

        return false;
    }

    public static List<string> FindMissingPropertiesInEntity(
        List<PropertyInfo> entityProperties, 
        List<PropertyInfo> viewModelProperties)
    {
        var missing = new List<string>();
        var entityPropertyNames = entityProperties.Select(p => p.Name).ToHashSet();

        foreach (var vmProperty in viewModelProperties)
        {
            if (vmProperty.IsComputed) continue;

            // Check if this is a flattened value object property
            var isFlattened = false;
            foreach (var entityProperty in entityProperties.Where(p => p.IsValueObject))
            {
                if (IsValueObjectFlattening(entityProperty, vmProperty))
                {
                    isFlattened = true;
                    break;
                }
            }

            // Check for entity name aliasing
            var isAliased = CheckForEntityNameAliasing(vmProperty, entityProperties);
            if (isAliased) continue;

            if (!isFlattened && !entityPropertyNames.Contains(vmProperty.Name))
            {
                missing.Add(vmProperty.Name);
            }
        }

        return missing;
    }

    public static List<string> FindMissingPropertiesInViewModel(
        List<PropertyInfo> entityProperties, 
        List<PropertyInfo> viewModelProperties)
    {
        var missing = new List<string>();
        var viewModelPropertyNames = viewModelProperties.Select(p => p.Name).ToHashSet();

        foreach (var entityProperty in entityProperties)
        {
            // Skip inherited properties from BaseEntity
            if (entityProperty.IsInherited) continue;
            
            // Skip navigation properties as they are optional in ViewModels
            if (entityProperty.IsNavigation) continue;

            // Check for entity name aliasing
            var isAliased = CheckForEntityNameAliasing(entityProperty, viewModelProperties);
            if (isAliased) continue;

            if (!viewModelPropertyNames.Contains(entityProperty.Name))
            {
                missing.Add(entityProperty.Name);
            }
        }

        return missing;
    }

    private static bool CheckForEntityNameAliasing(PropertyInfo property, List<PropertyInfo> targetProperties)
    {
        // This is a simplified check - the actual entity name would need to be passed in
        // For now, we'll check common patterns
        var commonAliases = new[]
        {
            ("Id", new[] { "Id" }),
            ("Name", new[] { "Name" }),
            ("Number", new[] { "Number" }),
            ("Code", new[] { "Code" })
        };

        foreach (var (baseProperty, aliases) in commonAliases)
        {
            if (property.Name.Equals(baseProperty, StringComparison.OrdinalIgnoreCase))
            {
                foreach (var alias in aliases)
                {
                    if (targetProperties.Any(p => p.Name.Equals(alias, StringComparison.OrdinalIgnoreCase)))
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }
} 
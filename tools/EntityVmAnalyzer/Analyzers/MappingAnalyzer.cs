using EntityVmAnalyzer.Models;
using EntityVmAnalyzer.Utils;

namespace EntityVmAnalyzer.Analyzers;

public class MappingAnalyzer
{
    public List<MappingResult> AnalyzeMappings(List<EntityInfo> entities, List<ViewModelInfo> viewModels)
    {
        var mappingResults = new List<MappingResult>();

        foreach (var entity in entities)
        {
            var entityViewModels = viewModels.Where(vm => 
                IsViewModelForEntity(vm.Name, entity.Name)).ToList();

            foreach (var viewModel in entityViewModels)
            {
                var mappingResult = AnalyzeEntityViewModelMapping(entity, viewModel);
                mappingResults.Add(mappingResult);
            }
        }

        return mappingResults;
    }

    private bool IsViewModelForEntity(string viewModelName, string entityName)
    {
        // Remove common suffixes and prefixes to find the base entity name
        var cleanViewModelName = viewModelName
            .Replace("Create", "")
            .Replace("Update", "")
            .Replace("Detail", "")
            .Replace("List", "")
            .Replace("VM", "")
            .Replace("ViewModel", "");

        var cleanEntityName = entityName;

        return cleanViewModelName.Equals(cleanEntityName, StringComparison.OrdinalIgnoreCase) ||
               cleanViewModelName.Contains(cleanEntityName, StringComparison.OrdinalIgnoreCase) ||
               cleanEntityName.Contains(cleanViewModelName, StringComparison.OrdinalIgnoreCase);
    }

    private MappingResult AnalyzeEntityViewModelMapping(EntityInfo entity, ViewModelInfo viewModel)
    {
        var mappingResult = new MappingResult
        {
            EntityName = entity.Name,
            ViewModelName = viewModel.Name,
            ViewModelCategory = viewModel.Category
        };

        var mappings = new List<PropertyMapping>();

        // Use helper methods to find missing properties
        var missingInEntity = PropertyMatcher.FindMissingPropertiesInEntity(entity.Properties, viewModel.Properties);
        var missingInViewModel = PropertyMatcher.FindMissingPropertiesInViewModel(entity.Properties, viewModel.Properties);

        // Create mappings for all entity properties
        foreach (var entityProperty in entity.Properties)
        {
            // Skip inherited properties from BaseEntity when reporting as missing
            if (entityProperty.IsInherited && missingInViewModel.Contains(entityProperty.Name))
            {
                continue;
            }

            var viewModelProperty = viewModel.Properties.FirstOrDefault(p => 
                p.Name.Equals(entityProperty.Name, StringComparison.OrdinalIgnoreCase));

            if (viewModelProperty != null)
            {
                var mapping = PropertyMatcher.CreatePropertyMapping(entityProperty, viewModelProperty, entity.Name, viewModel.Name);
                mappings.Add(mapping);
            }
            else
            {
                // Check for entity name aliasing (e.g., CustomerId -> Id)
                var aliasedProperty = CheckForEntityNameAliasing(entityProperty, viewModel.Properties, entity.Name);
                if (aliasedProperty != null)
                {
                    var mapping = new PropertyMapping
                    {
                        EntityProperty = entityProperty.Name,
                        ViewModelProperty = aliasedProperty.Name,
                        EntityPropertyType = entityProperty.Type,
                        ViewModelPropertyType = aliasedProperty.Type,
                        Status = MappingStatus.Matched,
                        Notes = $"Entity name aliasing: {entityProperty.Name} -> {aliasedProperty.Name}"
                    };
                    mappings.Add(mapping);
                }
                else
                {
                    // Entity property not found in ViewModel
                    var mapping = new PropertyMapping
                    {
                        EntityProperty = entityProperty.Name,
                        ViewModelProperty = "",
                        EntityPropertyType = entityProperty.Type,
                        ViewModelPropertyType = "",
                        Status = MappingStatus.MissingInViewModel,
                        Notes = "Property exists in entity but not in ViewModel"
                    };
                    mappings.Add(mapping);
                }
            }
        }

        // Check for ViewModel properties not in entity
        foreach (var viewModelProperty in viewModel.Properties)
        {
            var entityProperty = entity.Properties.FirstOrDefault(p => 
                p.Name.Equals(viewModelProperty.Name, StringComparison.OrdinalIgnoreCase));

            if (entityProperty == null)
            {
                // Check if this is a flattened value object property
                var isFlattened = false;
                var flattenedEntityProperty = entity.Properties.FirstOrDefault(p => p.IsValueObject && 
                    PropertyMatcher.IsValueObjectFlattening(p, viewModelProperty));

                if (flattenedEntityProperty != null)
                {
                    isFlattened = true;
                    var mapping = new PropertyMapping
                    {
                        EntityProperty = flattenedEntityProperty.Name,
                        ViewModelProperty = viewModelProperty.Name,
                        EntityPropertyType = flattenedEntityProperty.Type,
                        ViewModelPropertyType = viewModelProperty.Type,
                        Status = MappingStatus.Flattened,
                        IsValueObjectFlattening = true,
                        Notes = $"Value object {flattenedEntityProperty.Type} flattened to {viewModelProperty.Name}"
                    };
                    mappings.Add(mapping);
                }

                // Check if this is a computed property
                if (viewModelProperty.IsComputed)
                {
                    var mapping = new PropertyMapping
                    {
                        EntityProperty = "",
                        ViewModelProperty = viewModelProperty.Name,
                        EntityPropertyType = "",
                        ViewModelPropertyType = viewModelProperty.Type,
                        Status = MappingStatus.Derived,
                        IsComputedProperty = true,
                        Notes = "Computed/derived property"
                    };
                    mappings.Add(mapping);
                }
                else if (!isFlattened)
                {
                    // Property exists in ViewModel but not in entity
                    var mapping = new PropertyMapping
                    {
                        EntityProperty = "",
                        ViewModelProperty = viewModelProperty.Name,
                        EntityPropertyType = "",
                        ViewModelPropertyType = viewModelProperty.Type,
                        Status = MappingStatus.MissingInEntity,
                        Notes = "Property exists in ViewModel but not in entity"
                    };
                    mappings.Add(mapping);
                }
            }
        }

        mappingResult.Mappings = mappings;

        // Categorize mappings
        mappingResult.MatchedProperties = mappings
            .Where(m => m.Status == MappingStatus.Matched)
            .Select(m => m.EntityProperty)
            .ToList();

        mappingResult.MissingInEntity = mappings
            .Where(m => m.Status == MappingStatus.MissingInEntity)
            .Select(m => m.ViewModelProperty)
            .ToList();

        mappingResult.MissingInViewModel = mappings
            .Where(m => m.Status == MappingStatus.MissingInViewModel)
            .Select(m => m.EntityProperty)
            .ToList();

        mappingResult.TypeMismatches = mappings
            .Where(m => m.Status == MappingStatus.TypeMismatch)
            .Select(m => $"{m.EntityProperty} ({m.EntityPropertyType}) vs {m.ViewModelProperty} ({m.ViewModelPropertyType})")
            .ToList();

        mappingResult.DerivedProperties = mappings
            .Where(m => m.Status == MappingStatus.Derived)
            .Select(m => m.ViewModelProperty)
            .ToList();

        mappingResult.FlattenedProperties = mappings
            .Where(m => m.Status == MappingStatus.Flattened)
            .Select(m => m.ViewModelProperty)
            .ToList();

        mappingResult.NavigationProperties = mappings
            .Where(m => m.Status == MappingStatus.Navigation)
            .Select(m => m.EntityProperty)
            .ToList();

        return mappingResult;
    }

    private PropertyInfo? CheckForEntityNameAliasing(PropertyInfo entityProperty, List<PropertyInfo> viewModelProperties, string entityName)
    {
        // Check for common aliasing patterns
        var aliasingPatterns = new[]
        {
            ($"{entityName}Id", "Id"),
            ($"{entityName}Name", "Name"),
            ($"{entityName}Number", "Number"),
            ($"{entityName}Code", "Code")
        };

        foreach (var (alias, baseProperty) in aliasingPatterns)
        {
            if (entityProperty.Name.Equals(baseProperty, StringComparison.OrdinalIgnoreCase))
            {
                var aliasedProperty = viewModelProperties.FirstOrDefault(p => 
                    p.Name.Equals(alias, StringComparison.OrdinalIgnoreCase));
                if (aliasedProperty != null)
                {
                    return aliasedProperty;
                }
            }
        }

        return null;
    }

    public AnalysisSummary GenerateAnalysisSummary(
        List<EntityInfo> entities, 
        List<ViewModelInfo> viewModels, 
        List<MappingResult> mappingResults)
    {
        var summary = new AnalysisSummary
        {
            Entities = entities,
            ViewModels = viewModels,
            MappingResults = mappingResults
        };

        // Analyze missing properties by entity (excluding inherited ones)
        foreach (var entity in entities)
        {
            var entityMappings = mappingResults.Where(m => m.EntityName == entity.Name);
            var missingProperties = entityMappings
                .SelectMany(m => m.MissingInEntity)
                .Distinct()
                .ToList();

            summary.MissingPropertiesByEntity[entity.Name] = missingProperties;
        }

        // Analyze value object flattening patterns
        var flattenedProperties = mappingResults
            .SelectMany(m => m.Mappings)
            .Where(m => m.IsValueObjectFlattening)
            .Select(m => m.ViewModelProperty)
            .Distinct()
            .ToList();

        summary.ValueObjectFlatteningPatterns["All"] = flattenedProperties;

        // Analyze computed property patterns
        var computedProperties = mappingResults
            .SelectMany(m => m.Mappings)
            .Where(m => m.IsComputedProperty)
            .Select(m => m.ViewModelProperty)
            .Distinct()
            .ToList();

        summary.ComputedPropertyPatterns["All"] = computedProperties;

        return summary;
    }
} 
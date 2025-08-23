namespace EntityVmAnalyzer.Models;

public class PropertyMapping
{
    public string EntityProperty { get; set; } = string.Empty;
    public string ViewModelProperty { get; set; } = string.Empty;
    public string EntityPropertyType { get; set; } = string.Empty;
    public string ViewModelPropertyType { get; set; } = string.Empty;
    public MappingStatus Status { get; set; }
    public string? Notes { get; set; }
    public bool IsValueObjectFlattening { get; set; }
    public bool IsComputedProperty { get; set; }
    public bool IsNavigationMapping { get; set; }
}

public enum MappingStatus
{
    Matched,
    MissingInEntity,
    MissingInViewModel,
    TypeMismatch,
    Derived,
    Flattened,
    Navigation
}

public class MappingResult
{
    public string EntityName { get; set; } = string.Empty;
    public string ViewModelName { get; set; } = string.Empty;
    public string ViewModelCategory { get; set; } = string.Empty;
    public List<PropertyMapping> Mappings { get; set; } = new();
    public List<string> MatchedProperties { get; set; } = new();
    public List<string> MissingInEntity { get; set; } = new();
    public List<string> MissingInViewModel { get; set; } = new();
    public List<string> TypeMismatches { get; set; } = new();
    public List<string> DerivedProperties { get; set; } = new();
    public List<string> FlattenedProperties { get; set; } = new();
    public List<string> NavigationProperties { get; set; } = new();
}

public class AnalysisSummary
{
    public DateTime AnalysisDate { get; set; } = DateTime.Now;
    public List<EntityInfo> Entities { get; set; } = new();
    public List<ViewModelInfo> ViewModels { get; set; } = new();
    public List<MappingResult> MappingResults { get; set; } = new();
    public Dictionary<string, List<string>> MissingPropertiesByEntity { get; set; } = new();
    public Dictionary<string, List<string>> ValueObjectFlatteningPatterns { get; set; } = new();
    public Dictionary<string, List<string>> ComputedPropertyPatterns { get; set; } = new();
} 
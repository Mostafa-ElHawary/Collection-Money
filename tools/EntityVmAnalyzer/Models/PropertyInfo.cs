namespace EntityVmAnalyzer.Models;

public class PropertyInfo
{
    public string Name { get; set; } = string.Empty;
    public string Type { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public bool IsCollection { get; set; }
    public bool IsNavigation { get; set; }
    public bool IsValueObject { get; set; }
    public bool IsComputed { get; set; }
    public bool IsFlattened { get; set; }
    public bool IsInherited { get; set; }
    public string? SourceLocation { get; set; }
    public List<string> Attributes { get; set; } = new();
}

public class EntityInfo
{
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public List<PropertyInfo> Properties { get; set; } = new();
    public List<string> BaseClasses { get; set; } = new();
    public List<string> Interfaces { get; set; } = new();
}

public class ViewModelInfo
{
    public string Name { get; set; } = string.Empty;
    public string FilePath { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty; // Create, Update, Detail, List, Analytics
    public List<PropertyInfo> Properties { get; set; } = new();
    public List<string> BaseClasses { get; set; } = new();
    public List<string> Interfaces { get; set; } = new();
}

public static class AnalysisConstants
{
    public static readonly List<string> BaseEntityPropertyNames = new()
    {
        "Id", "CreatedAt", "UpdatedAt"
    };
} 
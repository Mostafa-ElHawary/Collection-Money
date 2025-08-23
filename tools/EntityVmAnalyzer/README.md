# Entity-ViewModel Property Mapping Analyzer

A comprehensive analysis tool for .NET applications that systematically compares entity properties with their corresponding ViewModels to identify missing properties, type mismatches, and mapping patterns.

## Overview

This tool analyzes the relationship between domain entities and application ViewModels in a .NET solution, providing detailed insights into:

- **Property Mappings**: Direct property-to-property relationships
- **Missing Properties**: Properties referenced in ViewModels but not defined in entities
- **Value Object Flattening**: Detection of flattened value object patterns
- **Computed Properties**: Identification of derived/computed properties
- **Type Compatibility**: Analysis of type mismatches between entities and ViewModels
- **Navigation Properties**: Mapping of related entity collections

## Features

- **Automated Analysis**: Uses Roslyn (Microsoft.CodeAnalysis) to parse C# source code
- **Intelligent Pattern Recognition**: Detects value object flattening and computed properties
- **Multiple Output Formats**: Generates Markdown, JSON, and CSV reports
- **Comprehensive Coverage**: Analyzes all entities and ViewModels in the solution
- **Detailed Categorization**: Classifies properties by mapping status and type

## Prerequisites

- .NET 8.0 or later
- Access to source code directories (Entities and ViewModels)
- NuGet packages (automatically restored):
  - Microsoft.CodeAnalysis.CSharp
  - System.Text.Json
  - CsvHelper
  - System.CommandLine

## Installation

1. Navigate to the `tools/EntityVmAnalyzer` directory
2. Restore NuGet packages:
   ```bash
   dotnet restore
   ```
3. Build the project:
   ```bash
   dotnet build
   ```

## Usage

### Basic Usage

Run the analyzer with default paths:

```bash
dotnet run
```

The tool will automatically detect the following default paths:
- Entities: `../CollectionApp.Domain/Entities`
- ViewModels: `../CollectionApp.Application/ViewModels`
- Output: `../docs/analysis/entity-vm-mapping`

### Custom Paths

Specify custom directories as command-line arguments:

```bash
dotnet run "path/to/entities" "path/to/viewmodels" "path/to/output"
```

### Example Output

```
Entity-ViewModel Property Mapping Analyzer
==========================================

Entities Directory: C:\path\to\CollectionApp.Domain\Entities
ViewModels Directory: C:\path\to\CollectionApp.Application\ViewModels
Output Directory: C:\path\to\docs\analysis\entity-vm-mapping

Step 1: Analyzing Entities...
Found 9 entities:
  - Contract (8 properties)
  - Customer (7 properties)
  - Staff (6 properties)
  ...

Step 2: Analyzing ViewModels...
Found 27 ViewModels:
  - Create: 9 ViewModels
  - Detail: 9 ViewModels
  - List: 9 ViewModels
  ...

Step 3: Analyzing Entity-ViewModel Mappings...
Generated 27 mapping results

Step 4: Generating Analysis Summary...
Analysis summary generated

Step 5: Generating Reports...
All reports generated successfully!

Analysis Complete!
=================

Summary Statistics:
  - Total Entities: 9
  - Total ViewModels: 27
  - Total Mappings: 27
  - Properties Missing in Entities: 3
  - Properties Missing in ViewModels: 12
  - Type Mismatches: 0
  - Value Object Flattening Patterns: 15
  - Computed Properties: 8

Critical Issues Found:
=====================
  Contract Entity:
    - Missing: StaffId
  Customer Entity:
    - Missing: PhoneCountryCode
    - Missing: PhoneAreaCode
```

## Output Files

The tool generates four comprehensive reports:

### 1. property-comparison-report.md
Complete analysis report with:
- Executive summary and statistics
- Detailed entity analysis with property tables
- ViewModel mapping matrices
- Value object flattening analysis
- Computed properties analysis
- Recommendations for fixes

### 2. missing-properties-summary.md
Focused report highlighting:
- Critical missing properties by entity
- Priority recommendations
- Referencing ViewModels for each missing property

### 3. mapping-matrix.json
Machine-readable data containing:
- Complete analysis results
- Property mappings and statuses
- Type information and metadata
- Suitable for programmatic processing

### 4. property-details.csv
Spreadsheet-friendly format with:
- Entity-ViewModel property mappings
- Mapping status and notes
- Type information
- Easy filtering and analysis

## Analysis Methodology

### Property Categorization

The tool categorizes properties into several mapping statuses:

1. **Matched**: Direct property-to-property mapping with compatible types
2. **MissingInEntity**: Property exists in ViewModel but not in entity
3. **MissingInViewModel**: Property exists in entity but not in ViewModel
4. **TypeMismatch**: Properties exist in both but have incompatible types
5. **Derived**: Computed/calculated properties (e.g., FullName, TotalAmount)
6. **Flattened**: Flattened value object properties (e.g., AddressStreet, PhoneCountryCode)
7. **Navigation**: Navigation properties for related entities

### Value Object Flattening Detection

The tool automatically detects when value objects are flattened in ViewModels:

- **Address** → Street, City, State, ZipCode, Country
- **Phone** → CountryCode, AreaCode, Number
- **Money** → Amount, Currency

### Computed Property Recognition

Uses naming patterns to identify computed properties:
- Properties ending with: Display, Name, Full, Total, Count, Summary
- Properties containing: Formatted, Calculated, Derived, Computed

## Configuration

### Customizing Analysis Patterns

Modify the following files to customize analysis behavior:

- **EntityAnalyzer.cs**: Adjust base entity property filtering
- **ViewModelAnalyzer.cs**: Modify ViewModel detection patterns
- **PropertyMatcher.cs**: Customize value object flattening detection
- **ReportGenerator.cs**: Adjust report formatting and content

### Extending Value Object Support

Add new value object types in `PropertyMatcher.cs`:

```csharp
private static readonly Dictionary<string, List<string>> ValueObjectPropertyMappings = new()
{
    ["Address"] = new List<string> { "Street", "City", "State", "ZipCode", "Country" },
    ["Phone"] = new List<string> { "CountryCode", "AreaCode", "Number" },
    ["Money"] = new List<string> { "Amount", "Currency" },
    ["NewValueObject"] = new List<string> { "Property1", "Property2" } // Add here
};
```

## Troubleshooting

### Common Issues

1. **Directory Not Found Errors**
   - Verify the paths to Entities and ViewModels directories
   - Use absolute paths if relative paths fail

2. **No Entities/ViewModels Found**
   - Ensure the source files have `.cs` extensions
   - Check that classes are public and properly formatted

3. **Build Errors**
   - Ensure .NET 8.0 SDK is installed
   - Run `dotnet restore` to restore NuGet packages

### Debug Mode

Enable detailed logging by modifying the Program.cs file to include more verbose output.

## Extending the Tool

### Adding New Analysis Types

1. Create new analyzer classes in the `Analyzers` directory
2. Implement the analysis logic
3. Integrate with the main Program.cs workflow
4. Add corresponding report generation methods

### Custom Report Formats

1. Extend the `ReportGenerator` class
2. Implement new report generation methods
3. Add the new format to the `GenerateAllReportsAsync` method

## Contributing

When extending the tool:

1. Follow the existing code structure and patterns
2. Add comprehensive error handling
3. Include XML documentation for public methods
4. Test with various entity and ViewModel patterns
5. Update this README with new features

## License

This tool is part of the Tracker-Money project and follows the same licensing terms.

## Support

For issues or questions:
1. Check the troubleshooting section above
2. Review the generated reports for insights
3. Examine the source code for specific analysis logic
4. Create an issue in the project repository 
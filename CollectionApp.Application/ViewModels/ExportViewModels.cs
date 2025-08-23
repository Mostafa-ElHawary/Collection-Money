using System.ComponentModel.DataAnnotations;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Application.ViewModels
{
    /// <summary>
    /// Base export request model
    /// </summary>
    public class ExportRequestVM
    {
        /// <summary>
        /// Export format (Excel, CSV, PDF, JSON)
        /// </summary>
        [Required]
        [EnumDataType(typeof(ExportFormat))]
        public string Format { get; set; } = "Excel";

        /// <summary>
        /// Include headers in the export
        /// </summary>
        public bool IncludeHeaders { get; set; } = true;

        /// <summary>
        /// Start date for date range filtering
        /// </summary>
        public DateTime? FromDate { get; set; }

        /// <summary>
        /// End date for date range filtering
        /// </summary>
        public DateTime? ToDate { get; set; }

        /// <summary>
        /// Additional filters as key-value pairs
        /// </summary>
        public Dictionary<string, object> Filters { get; set; } = new Dictionary<string, object>();

        /// <summary>
        /// Field to sort by
        /// </summary>
        public string SortBy { get; set; }

        /// <summary>
        /// Sort direction (Ascending, Descending)
        /// </summary>
        public string SortDirection { get; set; } = "Ascending";
    }

    /// <summary>
    /// Customer-specific export request
    /// </summary>
    public class CustomerExportRequestVM : ExportRequestVM
    {
        /// <summary>
        /// Include contract information in export
        /// </summary>
        public bool IncludeContracts { get; set; } = false;

        /// <summary>
        /// Include payment information in export
        /// </summary>
        public bool IncludePayments { get; set; } = false;

        /// <summary>
        /// Specific customer IDs to export
        /// </summary>
        public List<Guid> CustomerIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Search criteria for filtering customers
        /// </summary>
        public AdvancedCustomerSearchCriteriaVM SearchCriteria { get; set; }
    }

    /// <summary>
    /// Contract-specific export request
    /// </summary>
    public class ContractExportRequestVM : ExportRequestVM
    {
        /// <summary>
        /// Include installment information in export
        /// </summary>
        public bool IncludeInstallments { get; set; } = false;

        /// <summary>
        /// Include payment information in export
        /// </summary>
        public bool IncludePayments { get; set; } = false;

        /// <summary>
        /// Specific contract IDs to export
        /// </summary>
        public List<Guid> ContractIds { get; set; } = new List<Guid>();

        /// <summary>
        /// Filter by contract status
        /// </summary>
        public List<ContractStatus> StatusFilter { get; set; } = new List<ContractStatus>();

        /// <summary>
        /// Filter by specific customer
        /// </summary>
        public Guid? CustomerId { get; set; }
    }

    /// <summary>
    /// Export operation result
    /// </summary>
    public class ExportResultVM
    {
        /// <summary>
        /// Indicates if export was successful
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Name of the exported file
        /// </summary>
        public string FileName { get; set; }

        /// <summary>
        /// MIME type of the exported file
        /// </summary>
        public string ContentType { get; set; }

        /// <summary>
        /// Size of the exported file in bytes
        /// </summary>
        public long FileSize { get; set; }

        /// <summary>
        /// Number of records exported
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// Timestamp when export was generated
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Unique identifier for the export operation
        /// </summary>
        public Guid ExportId { get; set; }

        /// <summary>
        /// URL for downloading the exported file
        /// </summary>
        public string DownloadUrl { get; set; }

        /// <summary>
        /// Error message if export failed
        /// </summary>
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Multiple export operations result
    /// </summary>
    public class BulkExportResultVM
    {
        /// <summary>
        /// Total number of export requests
        /// </summary>
        public int TotalRequested { get; set; }

        /// <summary>
        /// Number of completed exports
        /// </summary>
        public int TotalCompleted { get; set; }

        /// <summary>
        /// Number of failed exports
        /// </summary>
        public int TotalFailed { get; set; }

        /// <summary>
        /// Individual export results
        /// </summary>
        public List<ExportResultVM> Results { get; set; } = new List<ExportResultVM>();

        /// <summary>
        /// General messages about the bulk export
        /// </summary>
        public List<string> Messages { get; set; } = new List<string>();
    }

    /// <summary>
    /// Analytics data export request
    /// </summary>
    public class AnalyticsExportRequestVM : ExportRequestVM
    {
        /// <summary>
        /// Type of analytics to export
        /// </summary>
        [Required]
        [EnumDataType(typeof(AnalyticsType))]
        public string AnalyticsType { get; set; }

        /// <summary>
        /// Include charts and graphs in export
        /// </summary>
        public bool IncludeCharts { get; set; } = true;

        /// <summary>
        /// Include raw data in export
        /// </summary>
        public bool IncludeRawData { get; set; } = true;

        /// <summary>
        /// Field to group analytics by
        /// </summary>
        public string GroupBy { get; set; }

        /// <summary>
        /// Type of aggregation to perform
        /// </summary>
        [EnumDataType(typeof(AggregationType))]
        public string AggregationType { get; set; } = "Sum";
    }

    /// <summary>
    /// Financial report export request
    /// </summary>
    public class FinancialReportExportRequestVM : ExportRequestVM
    {
        /// <summary>
        /// Type of financial report
        /// </summary>
        [Required]
        [EnumDataType(typeof(ReportType))]
        public string ReportType { get; set; }

        /// <summary>
        /// Include graphs and charts
        /// </summary>
        public bool IncludeGraphs { get; set; } = true;

        /// <summary>
        /// Period for comparison
        /// </summary>
        [EnumDataType(typeof(ComparisonPeriod))]
        public string ComparisonPeriod { get; set; } = "Month";

        /// <summary>
        /// Currency for financial data
        /// </summary>
        public string Currency { get; set; } = "USD";
    }

    /// <summary>
    /// Export configuration settings
    /// </summary>
    public class ExportConfigurationVM
    {
        /// <summary>
        /// Maximum records per export
        /// </summary>
        public int MaxRecordsPerExport { get; set; } = 10000;

        /// <summary>
        /// Supported export formats
        /// </summary>
        public List<string> SupportedFormats { get; set; } = new List<string> { "Excel", "CSV", "PDF", "JSON" };

        /// <summary>
        /// Default export format
        /// </summary>
        public string DefaultFormat { get; set; } = "Excel";

        /// <summary>
        /// Enable compression for large exports
        /// </summary>
        public bool CompressionEnabled { get; set; } = true;

        /// <summary>
        /// Number of days to retain exported files
        /// </summary>
        public int RetentionDays { get; set; } = 30;
    }

    /// <summary>
    /// Export operation status tracking
    /// </summary>
    public class ExportStatusVM
    {
        /// <summary>
        /// Unique identifier for the export
        /// </summary>
        public Guid ExportId { get; set; }

        /// <summary>
        /// Current status of the export
        /// </summary>
        [EnumDataType(typeof(ExportStatus))]
        public string Status { get; set; }

        /// <summary>
        /// Progress percentage (0-100)
        /// </summary>
        [Range(0, 100)]
        public int Progress { get; set; }

        /// <summary>
        /// When the export started
        /// </summary>
        public DateTime StartedAt { get; set; }

        /// <summary>
        /// When the export completed (if finished)
        /// </summary>
        public DateTime? CompletedAt { get; set; }

        /// <summary>
        /// Error message if export failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Estimated time remaining
        /// </summary>
        public TimeSpan? EstimatedTimeRemaining { get; set; }
    }

    /// <summary>
    /// Supported export formats
    /// </summary>
    public enum ExportFormat
    {
        Excel,
        CSV,
        PDF,
        JSON
    }

    /// <summary>
    /// Analytics types for export
    /// </summary>
    public enum AnalyticsType
    {
        Customer,
        Contract,
        Financial,
        Collection,
        Overdue
    }

    /// <summary>
    /// Aggregation types for analytics
    /// </summary>
    public enum AggregationType
    {
        Sum,
        Average,
        Count,
        Min,
        Max
    }

    /// <summary>
    /// Financial report types
    /// </summary>
    public enum ReportType
    {
        Summary,
        Detailed,
        Comparative
    }

    /// <summary>
    /// Comparison periods for financial reports
    /// </summary>
    public enum ComparisonPeriod
    {
        Month,
        Quarter,
        Year
    }

    /// <summary>
    /// Export operation status
    /// </summary>
    public enum ExportStatus
    {
        Pending,
        InProgress,
        Completed,
        Failed,
        Cancelled
    }
} 
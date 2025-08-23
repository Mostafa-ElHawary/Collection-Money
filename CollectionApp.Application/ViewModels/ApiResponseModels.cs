using System.ComponentModel.DataAnnotations;
using CollectionApp.Application.Common;

namespace CollectionApp.Application.ViewModels
{
    /// <summary>
    /// Generic wrapper for successful API responses
    /// </summary>
    /// <typeparam name="T">The type of data being returned</typeparam>
    public class ApiResponse<T>
    {
        /// <summary>
        /// Indicates if the request was successful
        /// </summary>
        public bool Success { get; set; } = true;

        /// <summary>
        /// The actual data payload
        /// </summary>
        public T Data { get; set; }

        /// <summary>
        /// Optional message describing the response
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Timestamp when the response was generated
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Unique identifier for tracking the request
        /// </summary>
        public string RequestId { get; set; }

        public ApiError Error { get; set; }  // Add this property


    }

    /// <summary>
    /// Standardized error response for API endpoints
    /// </summary>
    public class ApiErrorResponse
    {
        /// <summary>
        /// Always false for error responses
        /// </summary>
        public bool Success { get; set; } = false;

        /// <summary>
        /// Detailed error information
        /// </summary>
        public ApiError Error { get; set; }

        /// <summary>
        /// Human-readable error message
        /// </summary>
        public string Message { get; set; }

        /// <summary>
        /// Timestamp when the error occurred
        /// </summary>
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Unique identifier for tracking the request
        /// </summary>
        public string RequestId { get; set; }
    }

    /// <summary>
    /// Detailed error information for API responses
    /// </summary>
    public class ApiError
    {
        /// <summary>
        /// Error code for programmatic handling
        /// </summary>
        public string Code { get; set; }

        /// <summary>
        /// Short error title
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// Detailed error description
        /// </summary>
        public string Detail { get; set; }

        /// <summary>
        /// Error type URI for categorization
        /// </summary>
        public string Type { get; set; }

        /// <summary>
        /// Instance URI for the specific error occurrence
        /// </summary>
        public string Instance { get; set; }

        /// <summary>
        /// Validation errors organized by field name
        /// </summary>
        public Dictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();

    
    }
    


    /// <summary>
    /// Wrapper for paginated API responses
    /// </summary>
    /// <typeparam name="T">The type of items in the page</typeparam>
    public class PagedApiResponse<T> : ApiResponse<IList<T>>
    {
        public int TotalCount { get; set; }
        public int PageNumber { get; set; }
        public int PageSize { get; set; }
        public int TotalPages { get; set; }
        public bool HasNextPage { get; set; }
        public bool HasPreviousPage { get; set; }
    }

    /// <summary>
    /// Response for bulk operations with detailed results
    /// </summary>
    public class BulkOperationResponse
    {
        /// <summary>
        /// Total number of items requested for processing
        /// </summary>
        public int TotalRequested { get; set; }

        /// <summary>
        /// Total number of items actually processed
        /// </summary>
        public int TotalProcessed { get; set; }

        /// <summary>
        /// Number of items successfully processed
        /// </summary>
        public int TotalSuccessful { get; set; }

        /// <summary>
        /// Number of items that failed processing
        /// </summary>
        public int TotalFailed { get; set; }

        /// <summary>
        /// Detailed results for each processed item
        /// </summary>
        public List<BulkOperationResult> Results { get; set; } = new List<BulkOperationResult>();

        /// <summary>
        /// General messages about the bulk operation
        /// </summary>
        public List<string> Messages { get; set; } = new List<string>();

        /// <summary>
        /// Timestamp when the bulk operation completed
        /// </summary>
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }

    /// <summary>
    /// Individual result for a bulk operation item
    /// </summary>
    public class BulkOperationResult
    {
        /// <summary>
        /// Unique identifier of the item
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Indicates if the operation was successful for this item
        /// </summary>
        public bool Success { get; set; }

        /// <summary>
        /// Error message if the operation failed
        /// </summary>
        public string ErrorMessage { get; set; }

        /// <summary>
        /// Additional data about the operation result
        /// </summary>
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Response for export operations
    /// </summary>
    public class ExportResponse
    {
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
        /// Format of the exported file (Excel, CSV, PDF, etc.)
        /// </summary>
        public string ExportFormat { get; set; }

        /// <summary>
        /// Number of records exported
        /// </summary>
        public int RecordCount { get; set; }

        /// <summary>
        /// Timestamp when the export was generated
        /// </summary>
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        /// <summary>
        /// Unique identifier for the export operation
        /// </summary>
        public string ExportId { get; set; }

        /// <summary>
        /// URL for downloading the exported file
        /// </summary>
        public string DownloadUrl { get; set; }
    }
}
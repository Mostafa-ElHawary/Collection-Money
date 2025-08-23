using CollectionApp.Application.ViewModels;

namespace CollectionApp.Application.Interfaces
{
    /// <summary>
    /// Service interface for handling data export functionality
    /// </summary>
    public interface IExportService
    {
        /// <summary>
        /// Export customer data with filtering and formatting
        /// </summary>
        /// <param name="request">Export request with filters and format</param>
        /// <returns>Export result with file information</returns>
        Task<ExportResultVM> ExportCustomersAsync(CustomerExportRequestVM request);

        /// <summary>
        /// Export customer analytics data
        /// </summary>
        /// <param name="customerId">Customer ID for analytics</param>
        /// <param name="format">Export format</param>
        /// <returns>Export result with analytics data</returns>
        Task<ExportResultVM> ExportCustomerAnalyticsAsync(Guid customerId, string format);

        /// <summary>
        /// Export contract data with filtering and formatting
        /// </summary>
        /// <param name="request">Export request with filters and format</param>
        /// <returns>Export result with file information</returns>
        Task<ExportResultVM> ExportContractsAsync(ContractExportRequestVM request);

        /// <summary>
        /// Export contract financial report
        /// </summary>
        /// <param name="contractId">Contract ID for financial report</param>
        /// <param name="format">Export format</param>
        /// <returns>Export result with financial report</returns>
        Task<ExportResultVM> ExportContractFinancialReportAsync(Guid contractId, string format);

        /// <summary>
        /// Export installment schedule for a contract
        /// </summary>
        /// <param name="contractId">Contract ID for installment schedule</param>
        /// <param name="format">Export format</param>
        /// <returns>Export result with installment schedule</returns>
        Task<ExportResultVM> ExportInstallmentScheduleAsync(Guid contractId, string format);

        /// <summary>
        /// Export payment history for a contract
        /// </summary>
        /// <param name="contractId">Contract ID for payment history</param>
        /// <param name="format">Export format</param>
        /// <returns>Export result with payment history</returns>
        Task<ExportResultVM> ExportPaymentHistoryAsync(Guid contractId, string format);

        /// <summary>
        /// Export collection report with date range
        /// </summary>
        /// <param name="fromDate">Start date for collection report</param>
        /// <param name="toDate">End date for collection report</param>
        /// <param name="format">Export format</param>
        /// <returns>Export result with collection report</returns>
        Task<ExportResultVM> ExportCollectionReportAsync(DateTime fromDate, DateTime toDate, string format);

        /// <summary>
        /// Export overdue analysis report
        /// </summary>
        /// <param name="format">Export format</param>
        /// <returns>Export result with overdue analysis</returns>
        Task<ExportResultVM> ExportOverdueAnalysisAsync(string format);

        /// <summary>
        /// Export analytics data based on request
        /// </summary>
        /// <param name="request">Analytics export request</param>
        /// <returns>Export result with analytics data</returns>
        Task<ExportResultVM> ExportAnalyticsAsync(AnalyticsExportRequestVM request);

        /// <summary>
        /// Export financial report based on request
        /// </summary>
        /// <param name="request">Financial report export request</param>
        /// <returns>Export result with financial report</returns>
        Task<ExportResultVM> ExportFinancialReportAsync(FinancialReportExportRequestVM request);

        /// <summary>
        /// Get export status by ID
        /// </summary>
        /// <param name="exportId">Export operation ID</param>
        /// <returns>Export status information</returns>
        Task<ExportStatusVM> GetExportStatusAsync(Guid exportId);

        /// <summary>
        /// Cancel an in-progress export operation
        /// </summary>
        /// <param name="exportId">Export operation ID</param>
        /// <returns>Success status of cancellation</returns>
        Task<bool> CancelExportAsync(Guid exportId);

        /// <summary>
        /// Get export configuration settings
        /// </summary>
        /// <returns>Export configuration</returns>
        Task<ExportConfigurationVM> GetExportConfigurationAsync();

        /// <summary>
        /// Update export configuration settings
        /// </summary>
        /// <param name="configuration">New configuration settings</param>
        /// <returns>Success status of update</returns>
        Task<bool> UpdateExportConfigurationAsync(ExportConfigurationVM configuration);
    }
} 
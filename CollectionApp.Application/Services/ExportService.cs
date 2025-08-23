using AutoMapper;
using CollectionApp.Application.Interfaces;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Enums;
using Microsoft.Extensions.Logging;
using System.Text;

namespace CollectionApp.Application.Services
{
    /// <summary>
    /// Service for handling data export functionality
    /// </summary>
    public class ExportService : IExportService
    {
        private readonly ICustomerService _customerService;
        private readonly IContractService _contractService;
        private readonly IPaymentService _paymentService;
        private readonly IInstallmentService _installmentService;
        private readonly IMapper _mapper;
        private readonly ILogger<ExportService> _logger;
        private readonly Dictionary<Guid, ExportStatusVM> _exportStatuses = new();

        public ExportService(
            ICustomerService customerService,
            IContractService contractService,
            IPaymentService paymentService,
            IInstallmentService installmentService,
            IMapper mapper,
            ILogger<ExportService> logger)
        {
            _customerService = customerService;
            _contractService = contractService;
            _paymentService = paymentService;
            _installmentService = installmentService;
            _mapper = mapper;
            _logger = logger;
        }

        /// <summary>
        /// Export customer data with filtering and formatting
        /// </summary>
        public async Task<ExportResultVM> ExportCustomersAsync(CustomerExportRequestVM request)
        {
            try
            {
                var exportId = Guid.NewGuid();
                var status = new ExportStatusVM
                {
                    ExportId = exportId,
                    Status = ExportStatus.InProgress.ToString(),
                    StartedAt = DateTime.UtcNow,
                    Progress = 0
                };
                _exportStatuses[exportId] = status;

                _logger.LogInformation("Starting customer export {ExportId}", exportId);

                // Fetch customer data based on request
                var customers = await GetCustomerDataForExport(request);
                status.Progress = 50;

                // Generate export file
                (byte[] fileBytes, string fileName, string contentType) = await GenerateCustomerExport(customers, request);
                status.Progress = 100;
                status.Status = ExportStatus.Completed.ToString();
                status.CompletedAt = DateTime.UtcNow;

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = customers.Count,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = exportId,
                    DownloadUrl = $"/api/exports/{exportId}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customers");
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export customer analytics data
        /// </summary>
        public async Task<ExportResultVM> ExportCustomerAnalyticsAsync(Guid customerId, string format)
        {
            try
            {
                var analytics = await _customerService.GetCustomerAnalyticsAsync(customerId);
                (byte[] fileBytes, string fileName, string contentType) = await GenerateAnalyticsExport(analytics, format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = 1,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting customer analytics for customer {CustomerId}", customerId);
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export contract data with filtering and formatting
        /// </summary>
        public async Task<ExportResultVM> ExportContractsAsync(ContractExportRequestVM request)
        {
            try
            {
                var contracts = await GetContractDataForExport(request);
                (byte[] fileBytes, string fileName, string contentType) = await GenerateContractExport(contracts, request);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = contracts.Count,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting contracts");
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export contract financial report
        /// </summary>
        public async Task<ExportResultVM> ExportContractFinancialReportAsync(Guid contractId, string format)
        {
            try
            {
                var financialSummary = await _contractService.GetContractFinancialSummaryAsync(contractId);
                (byte[] fileBytes, string fileName, string contentType) = await GenerateFinancialReportExport(financialSummary, format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = 1,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting contract financial report for contract {ContractId}", contractId);
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export installment schedule for a contract
        /// </summary>
        public async Task<ExportResultVM> ExportInstallmentScheduleAsync(Guid contractId, string format)
        {
            try
            {
                var installments = await _installmentService.GetInstallmentsByContractAsync(contractId);
                (byte[] fileBytes, string fileName, string contentType) = await GenerateInstallmentExport(installments, format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = installments.Count,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting installment schedule for contract {ContractId}", contractId);
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export payment history for a contract
        /// </summary>
        public async Task<ExportResultVM> ExportPaymentHistoryAsync(Guid contractId, string format)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByContractAsync(contractId);
                (byte[] fileBytes, string fileName, string contentType) = await GeneratePaymentExport(payments, format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = payments.Count,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting payment history for contract {ContractId}", contractId);
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export collection report with date range
        /// </summary>
        public async Task<ExportResultVM> ExportCollectionReportAsync(DateTime fromDate, DateTime toDate, string format)
        {
            try
            {
                var collectionData = await _paymentService.GetCollectionReportAsync(fromDate, toDate);
                var collectionDataList = new List<object> { collectionData };
                (byte[] fileBytes, string fileName, string contentType) = await GenerateCollectionReportExport(collectionDataList, format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = collectionData.TotalInstallments,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting collection report from {FromDate} to {ToDate}", fromDate, toDate);
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export overdue analysis report
        /// </summary>
        public async Task<ExportResultVM> ExportOverdueAnalysisAsync(string format)
        {
            try
            {
                var overdueData = await _installmentService.GetOverdueAnalysisAsync();
                var overdueDataList = new List<object> { overdueData };
                (byte[] fileBytes, string fileName, string contentType) = await GenerateOverdueAnalysisExport(overdueDataList, format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = overdueData.TotalOverdue,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting overdue analysis");
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export analytics data based on request
        /// </summary>
        public async Task<ExportResultVM> ExportAnalyticsAsync(AnalyticsExportRequestVM request)
        {
            try
            {
                var analyticsData = await GetAnalyticsDataForExport(request);
                (byte[] fileBytes, string fileName, string contentType) = await GenerateAnalyticsExport(analyticsData, request.Format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = analyticsData.Count,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting analytics data");
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Export financial report based on request
        /// </summary>
        public async Task<ExportResultVM> ExportFinancialReportAsync(FinancialReportExportRequestVM request)
        {
            try
            {
                var financialData = await GetFinancialDataForExport(request);
                (byte[] fileBytes, string fileName, string contentType) = await GenerateFinancialReportExport(financialData, request.Format);

                return new ExportResultVM
                {
                    Success = true,
                    FileName = fileName,
                    ContentType = contentType,
                    FileSize = fileBytes.Length,
                    RecordCount = financialData.Count,
                    GeneratedAt = DateTime.UtcNow,
                    ExportId = Guid.NewGuid(),
                    DownloadUrl = $"/api/exports/{Guid.NewGuid()}/download"
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting financial report");
                return new ExportResultVM
                {
                    Success = false,
                    ErrorMessage = ex.Message
                };
            }
        }

        /// <summary>
        /// Get export status by ID
        /// </summary>
        public async Task<ExportStatusVM> GetExportStatusAsync(Guid exportId)
        {
            return await Task.FromResult(_exportStatuses.GetValueOrDefault(exportId));
        }

        /// <summary>
        /// Cancel an in-progress export operation
        /// </summary>
        public async Task<bool> CancelExportAsync(Guid exportId)
        {
            if (_exportStatuses.TryGetValue(exportId, out var status))
            {
                status.Status = ExportStatus.Cancelled.ToString();
                status.CompletedAt = DateTime.UtcNow;
                return await Task.FromResult(true);
            }
            return await Task.FromResult(false);
        }

        /// <summary>
        /// Get export configuration settings
        /// </summary>
        public async Task<ExportConfigurationVM> GetExportConfigurationAsync()
        {
            return await Task.FromResult(new ExportConfigurationVM());
        }

        /// <summary>
        /// Update export configuration settings
        /// </summary>
        public async Task<bool> UpdateExportConfigurationAsync(ExportConfigurationVM configuration)
        {
            // Implementation would typically save to configuration store
            return await Task.FromResult(true);
        }

        #region Private Helper Methods

        private async Task<List<CustomerSearchResultVM>> GetCustomerDataForExport(CustomerExportRequestVM request)
        {
            if (request.CustomerIds.Any())
    {
        var customers = await _customerService.BulkGetCustomersAsync(request.CustomerIds);
        return _mapper.Map<List<CustomerSearchResultVM>>(customers);
    }

    if (request.SearchCriteria != null)
    {
        var result = await _customerService.AdvancedSearchAsync(request.SearchCriteria, 1, int.MaxValue);
        return _mapper.Map<List<CustomerSearchResultVM>>(result.Items);
    }

    // Default: get all customers
    var allCustomers = await _customerService.GetAllAsync();
    return _mapper.Map<List<CustomerSearchResultVM>>(allCustomers);
        }

        private async Task<List<ContractListVM>> GetContractDataForExport(ContractExportRequestVM request)
        {
            if (request.ContractIds.Any())
            {
                var contracts = new List<ContractListVM>();
                foreach (var id in request.ContractIds)
                {
                    var contract = await _contractService.GetByIdAsync(id);
                    if (contract != null)
                    {
                        contracts.Add(_mapper.Map<ContractListVM>(contract));
                    }
                }
                return contracts;
            }

            // Default: get all contracts
            var allContracts = await _contractService.GetAllAsync();
            return _mapper.Map<List<ContractListVM>>(allContracts);
        }

        private async Task<List<object>> GetAnalyticsDataForExport(AnalyticsExportRequestVM request)
        {
            switch (request.AnalyticsType)
            {
                case "Customer":
                    var customerAnalytics = await _customerService.GetCustomerAnalyticsAsync();
                    return new List<object> { customerAnalytics };
                case "Contract":
                    var contractAnalytics = await _contractService.GetContractAnalyticsAsync();
                    return new List<object> { contractAnalytics };
                case "Financial":
                    var financialAnalytics = await _paymentService.GetFinancialAnalyticsAsync();
                    return new List<object> { financialAnalytics };
                default:
                    return new List<object>();
            }
        }

        private async Task<List<object>> GetFinancialDataForExport(FinancialReportExportRequestVM request)
        {
            // Implementation would fetch financial data based on request type
            return new List<object>();
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateCustomerExport(List<CustomerSearchResultVM> customers, CustomerExportRequestVM request)
        {
            switch (request.Format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(customers, "customers", request.IncludeHeaders);
                case "json":
                    return await GenerateJsonExport(customers, "customers");
                default:
                    return await GenerateExcelExport(customers, "customers", request.IncludeHeaders);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateContractExport(List<ContractListVM> contracts, ContractExportRequestVM request)
        {
            switch (request.Format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(contracts, "contracts", request.IncludeHeaders);
                case "json":
                    return await GenerateJsonExport(contracts, "contracts");
                default:
                    return await GenerateExcelExport(contracts, "contracts", request.IncludeHeaders);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateAnalyticsExport(object analyticsData, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(new List<object> { analyticsData }, "analytics", true);
                case "json":
                    return await GenerateJsonExport(analyticsData, "analytics");
                default:
                    return await GenerateExcelExport(new List<object> { analyticsData }, "analytics", true);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateFinancialReportExport(object financialData, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(new List<object> { financialData }, "financial_report", true);
                case "json":
                    return await GenerateJsonExport(financialData, "financial_report");
                default:
                    return await GenerateExcelExport(new List<object> { financialData }, "financial_report", true);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateInstallmentExport(List<InstallmentListVM> installments, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(installments, "installments", true);
                case "json":
                    return await GenerateJsonExport(installments, "installments");
                default:
                    return await GenerateExcelExport(installments, "installments", true);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GeneratePaymentExport(List<PaymentListVM> payments, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(payments, "payments", true);
                case "json":
                    return await GenerateJsonExport(payments, "payments");
                default:
                    return await GenerateExcelExport(payments, "payments", true);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateCollectionReportExport(List<object> collectionData, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(collectionData, "collection_report", true);
                case "json":
                    return await GenerateJsonExport(collectionData, "collection_report");
                default:
                    return await GenerateExcelExport(collectionData, "collection_report", true);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateOverdueAnalysisExport(List<object> overdueData, string format)
        {
            switch (format.ToLower())
            {
                case "csv":
                    return await GenerateCsvExport(overdueData, "overdue_analysis", true);
                case "json":
                    return await GenerateJsonExport(overdueData, "overdue_analysis");
                default:
                    return await GenerateExcelExport(overdueData, "overdue_analysis", true);
            }
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateCsvExport<T>(List<T> data, string fileName, bool includeHeaders)
        {
            var csv = new StringBuilder();

            if (includeHeaders && data.Any())
            {
                var headers = typeof(T).GetProperties().Select(p => p.Name);
                csv.AppendLine(string.Join(",", headers));
            }

            foreach (var item in data)
            {
                var values = typeof(T).GetProperties()
                    .Select(p => p.GetValue(item)?.ToString() ?? "")
                    .Select(v => $"\"{v.Replace("\"", "\"\"")}\"");
                csv.AppendLine(string.Join(",", values));
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return await Task.FromResult((bytes, $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv", "text/csv"));
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateJsonExport<T>(T data, string fileName)
        {
            var json = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
            {
                WriteIndented = true
            });
            var bytes = Encoding.UTF8.GetBytes(json);
            return await Task.FromResult((bytes, $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json", "application/json"));
        }

        private async Task<(byte[] fileBytes, string fileName, string contentType)> GenerateExcelExport<T>(List<T> data, string fileName, bool includeHeaders)
        {
            // Simple CSV-like Excel format for now
            // In a real implementation, you would use EPPlus or similar library
            var csv = new StringBuilder();

            if (includeHeaders && data.Any())
            {
                var headers = typeof(T).GetProperties().Select(p => p.Name);
                csv.AppendLine(string.Join("\t", headers));
            }

            foreach (var item in data)
            {
                var values = typeof(T).GetProperties()
                    .Select(p => p.GetValue(item)?.ToString() ?? "");
                csv.AppendLine(string.Join("\t", values));
            }

            var bytes = Encoding.UTF8.GetBytes(csv.ToString());
            return await Task.FromResult((bytes, $"{fileName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}.xls", "application/vnd.ms-excel"));
        }

        #endregion
    }
}
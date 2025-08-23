using AutoMapper;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using System;
using System.ComponentModel.DataAnnotations;
using static CollectionApp.Application.ViewModels.InstallmentAnalyticsViewModels;

namespace CollectionApp.Application.Mappings
{
    /// <summary>
    /// AutoMapper profile for export-related mappings
    /// </summary>
    public class ExportMappingProfile : Profile
    {
        public ExportMappingProfile()
        {
            // Export Request Mappings
            CreateMap<ExportRequestVM, ExportParameters>()
                .ForMember(dest => dest.Format, opt => opt.MapFrom(src => src.Format))
                .ForMember(dest => dest.IncludeHeaders, opt => opt.MapFrom(src => src.IncludeHeaders))
                .ForMember(dest => dest.FromDate, opt => opt.MapFrom(src => src.FromDate))
                .ForMember(dest => dest.ToDate, opt => opt.MapFrom(src => src.ToDate))
                .ForMember(dest => dest.Filters, opt => opt.MapFrom(src => src.Filters))
                .ForMember(dest => dest.SortBy, opt => opt.MapFrom(src => src.SortBy))
                .ForMember(dest => dest.SortDirection, opt => opt.MapFrom(src => src.SortDirection));

            CreateMap<CustomerExportRequestVM, CustomerExportParameters>()
                .IncludeBase<ExportRequestVM, ExportParameters>()
                .ForMember(dest => dest.IncludeContracts, opt => opt.MapFrom(src => src.IncludeContracts))
                .ForMember(dest => dest.IncludePayments, opt => opt.MapFrom(src => src.IncludePayments))
                .ForMember(dest => dest.CustomerIds, opt => opt.MapFrom(src => src.CustomerIds))
                .ForMember(dest => dest.SearchCriteria, opt => opt.MapFrom(src => src.SearchCriteria));

            CreateMap<ContractExportRequestVM, ContractExportParameters>()
                .IncludeBase<ExportRequestVM, ExportParameters>()
                .ForMember(dest => dest.IncludeInstallments, opt => opt.MapFrom(src => src.IncludeInstallments))
                .ForMember(dest => dest.IncludePayments, opt => opt.MapFrom(src => src.IncludePayments))
                .ForMember(dest => dest.ContractIds, opt => opt.MapFrom(src => src.ContractIds))
                .ForMember(dest => dest.StatusFilter, opt => opt.MapFrom(src => src.StatusFilter))
                .ForMember(dest => dest.CustomerId, opt => opt.MapFrom(src => src.CustomerId));

            // Export Result Mappings
            CreateMap<ExportResult, ExportResultVM>()
                .ForMember(dest => dest.Success, opt => opt.MapFrom(src => src.Success))
                .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.FileName))
                .ForMember(dest => dest.ContentType, opt => opt.MapFrom(src => src.ContentType))
                .ForMember(dest => dest.FileSize, opt => opt.MapFrom(src => src.FileSize))
                .ForMember(dest => dest.RecordCount, opt => opt.MapFrom(src => src.RecordCount))
                .ForMember(dest => dest.GeneratedAt, opt => opt.MapFrom(src => src.GeneratedAt))
                .ForMember(dest => dest.ExportId, opt => opt.MapFrom(src => src.ExportId))
                .ForMember(dest => dest.DownloadUrl, opt => opt.MapFrom(src => src.DownloadUrl))
                .ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.ErrorMessage));

            // Analytics Export Mappings
            CreateMap<CustomerAnalyticsVM, CustomerAnalyticsExportVM>()
                .ForMember(dest => dest.TotalCustomers, opt => opt.MapFrom(src => src.TotalCustomers))
                .ForMember(dest => dest.ActiveCustomers, opt => opt.MapFrom(src => src.ActiveCustomers))
                .ForMember(dest => dest.InactiveCustomers, opt => opt.MapFrom(src => src.InactiveCustomers))
                .ForMember(dest => dest.TotalContracts, opt => opt.MapFrom(src => src.TotalContracts))
                .ForMember(dest => dest.AverageContractValue, opt => opt.MapFrom(src => src.AverageContractValue))
                .ForMember(dest => dest.TotalOutstandingAmount, opt => opt.MapFrom(src => src.TotalOutstandingAmount))
                .ForMember(dest => dest.CollectionRate, opt => opt.MapFrom(src => src.CollectionRate))
                .ForMember(dest => dest.ExportDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<ContractFinancialSummaryVM, ContractFinancialExportVM>()
                .ForMember(dest => dest.ContractId, opt => opt.MapFrom(src => src.ContractId))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.ContractNumber))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.CustomerName))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount))
                .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => src.PaidAmount))
                .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.OutstandingAmount))
                .ForMember(dest => dest.PaymentPercentage, opt => opt.MapFrom(src => src.PaymentPercentage))
                .ForMember(dest => dest.InstallmentCount, opt => opt.MapFrom(src => src.InstallmentCount))
                .ForMember(dest => dest.PaidInstallmentCount, opt => opt.MapFrom(src => src.PaidInstallmentCount))
                .ForMember(dest => dest.OverdueInstallmentCount, opt => opt.MapFrom(src => src.OverdueInstallmentCount))
                .ForMember(dest => dest.ExportDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            CreateMap<InstallmentStatusSummaryVM, InstallmentStatusExportVM>()
                .ForMember(dest => dest.TotalInstallments, opt => opt.MapFrom(src => src.TotalInstallments))
                .ForMember(dest => dest.PaidInstallments, opt => opt.MapFrom(src => src.PaidInstallments))
                .ForMember(dest => dest.OverdueInstallments, opt => opt.MapFrom(src => src.OverdueInstallments))
                .ForMember(dest => dest.UpcomingInstallments, opt => opt.MapFrom(src => src.UpcomingInstallments))
                .ForMember(dest => dest.WaivedInstallments, opt => opt.MapFrom(src => src.WaivedInstallments))
                .ForMember(dest => dest.ExportDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Entity to Export Format Mappings
            CreateMap<Customer, CustomerExportVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Phone, opt => opt.MapFrom(src => src.Phone.Number))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address.ToString()))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.UpdatedAt, opt => opt.MapFrom(src => src.UpdatedAt))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ForMember(dest => dest.ContractCount, opt => opt.MapFrom(src => src.Contracts.Count))
                .ForMember(dest => dest.TotalContractValue, opt => opt.MapFrom(src => src.Contracts.Sum(c => c.TotalAmount.Value)))
                .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.Contracts.Sum(c => c.RemainingBalance.Value)));

            CreateMap<Contract, ContractExportVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.ContractNumber))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Customer.Name))
                .ForMember(dest => dest.CustomerEmail, opt => opt.MapFrom(src => src.Customer.Email))
                .ForMember(dest => dest.TotalAmount, opt => opt.MapFrom(src => src.TotalAmount.Value))
                .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.RemainingBalance.Value))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.StartDate, opt => opt.MapFrom(src => src.StartDate))
                .ForMember(dest => dest.EndDate, opt => opt.MapFrom(src => src.EndDate))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt))
                .ForMember(dest => dest.InstallmentCount, opt => opt.MapFrom(src => src.Installments.Count))
                .ForMember(dest => dest.PaidInstallmentCount, opt => opt.MapFrom(src => src.Installments.Count(i => i.Status == InstallmentStatus.Paid)))
                .ForMember(dest => dest.OverdueInstallmentCount, opt => opt.MapFrom(src => src.Installments.Count(i => i.Status == InstallmentStatus.Overdue)));

            CreateMap<Payment, PaymentExportVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Installment.Contract.ContractNumber))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Installment.Contract.Customer.Name))
                .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.Installment.InstallmentNumber))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Value))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.PaymentMethod, opt => opt.MapFrom(src => src.PaymentMethod.ToString()))
                .ForMember(dest => dest.ReferenceNumber, opt => opt.MapFrom(src => src.ReferenceNumber))
                .ForMember(dest => dest.Notes, opt => opt.MapFrom(src => src.Notes))
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.CreatedAt));

            CreateMap<Installment, InstallmentExportVM>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.ContractNumber, opt => opt.MapFrom(src => src.Contract.ContractNumber))
                .ForMember(dest => dest.CustomerName, opt => opt.MapFrom(src => src.Contract.Customer.Name))
                .ForMember(dest => dest.InstallmentNumber, opt => opt.MapFrom(src => src.InstallmentNumber))
                .ForMember(dest => dest.Amount, opt => opt.MapFrom(src => src.Amount.Value))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.PaidAmount, opt => opt.MapFrom(src => src.PaidAmount.Value))
                .ForMember(dest => dest.OutstandingAmount, opt => opt.MapFrom(src => src.OutstandingAmount.Value))
                .ForMember(dest => dest.PaymentDate, opt => opt.MapFrom(src => src.PaymentDate))
                .ForMember(dest => dest.IsOverdue, opt => opt.MapFrom(src => src.IsOverdue(DateTime.UtcNow)))
                .ForMember(dest => dest.OverdueDays, opt => opt.MapFrom(src => src.OverdueDays));

            // Bulk Operation Mappings
            CreateMap<BulkUpdateResultVM, BulkUpdateExportVM>()
                .ForMember(dest => dest.TotalRequested, opt => opt.MapFrom(src => src.TotalRequested))
                .ForMember(dest => dest.TotalProcessed, opt => opt.MapFrom(src => src.TotalProcessed))
                .ForMember(dest => dest.TotalSuccessful, opt => opt.MapFrom(src => src.TotalSuccessful))
                .ForMember(dest => dest.TotalFailed, opt => opt.MapFrom(src => src.TotalFailed))
                .ForMember(dest => dest.Results, opt => opt.MapFrom(src => src.Results))
                .ForMember(dest => dest.Messages, opt => opt.MapFrom(src => src.Messages))
                .ForMember(dest => dest.CompletedAt, opt => opt.MapFrom(src => src.CompletedAt))
                .ForMember(dest => dest.ExportDate, opt => opt.MapFrom(src => DateTime.UtcNow));

            // Collection Mappings
            CreateMap<List<Customer>, List<CustomerExportVM>>();
            CreateMap<List<Contract>, List<ContractExportVM>>();
            CreateMap<List<Payment>, List<PaymentExportVM>>();
            CreateMap<List<Installment>, List<InstallmentExportVM>>();

            // Validation and Error Mappings
            CreateMap<ValidationResult, ExportValidationErrorVM>()
                .ForMember(dest => dest.FieldName, opt => opt.MapFrom(src => src.MemberNames.FirstOrDefault()))
                .ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.ErrorMessage))
                .ForMember(dest => dest.ErrorCode, opt => opt.MapFrom(src => src.ErrorMessage));

            CreateMap<Exception, ExportErrorVM>()
                .ForMember(dest => dest.ErrorType, opt => opt.MapFrom(src => src.GetType().Name))
                .ForMember(dest => dest.ErrorMessage, opt => opt.MapFrom(src => src.Message))
                .ForMember(dest => dest.StackTrace, opt => opt.MapFrom(src => src.StackTrace))
                .ForMember(dest => dest.InnerException, opt => opt.MapFrom(src => src.InnerException != null ? src.InnerException.Message : null));
        }
    }

    #region Internal Classes for Mapping

    public class ExportParameters
    {
        public string Format { get; set; }
        public bool IncludeHeaders { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public Dictionary<string, object> Filters { get; set; }
        public string SortBy { get; set; }
        public string SortDirection { get; set; }
    }

    public class CustomerExportParameters : ExportParameters
    {
        public bool IncludeContracts { get; set; }
        public bool IncludePayments { get; set; }
        public List<Guid> CustomerIds { get; set; }
        public AdvancedCustomerSearchCriteriaVM SearchCriteria { get; set; }
    }

    public class ContractExportParameters : ExportParameters
    {
        public bool IncludeInstallments { get; set; }
        public bool IncludePayments { get; set; }
        public List<Guid> ContractIds { get; set; }
        public List<ContractStatus> StatusFilter { get; set; }
        public Guid? CustomerId { get; set; }
    }

    public class ExportResult
    {
        public bool Success { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public long FileSize { get; set; }
        public int RecordCount { get; set; }
        public DateTime GeneratedAt { get; set; }
        public Guid ExportId { get; set; }
        public string DownloadUrl { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class CustomerAnalyticsExportVM
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int InactiveCustomers { get; set; }
        public int TotalContracts { get; set; }
        public decimal AverageContractValue { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal CollectionRate { get; set; }
        public DateTime ExportDate { get; set; }
    }

    public class ContractFinancialExportVM
    {
        public Guid ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string CustomerName { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal PaymentPercentage { get; set; }
        public int InstallmentCount { get; set; }
        public int PaidInstallmentCount { get; set; }
        public int OverdueInstallmentCount { get; set; }
        public DateTime ExportDate { get; set; }
    }

    public class InstallmentStatusExportVM
    {
        public int TotalInstallments { get; set; }
        public int PaidInstallments { get; set; }
        public int OverdueInstallments { get; set; }
        public int UpcomingInstallments { get; set; }
        public int WaivedInstallments { get; set; }
        public DateTime ExportDate { get; set; }
    }

    public class CustomerExportVM
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public bool IsActive { get; set; }
        public int ContractCount { get; set; }
        public decimal TotalContractValue { get; set; }
        public decimal OutstandingAmount { get; set; }
    }

    public class ContractExportVM
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; }
        public string CustomerName { get; set; }
        public string CustomerEmail { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public string Status { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public int InstallmentCount { get; set; }
        public int PaidInstallmentCount { get; set; }
        public int OverdueInstallmentCount { get; set; }
    }

    public class PaymentExportVM
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; }
        public string CustomerName { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public string PaymentMethod { get; set; }
        public string ReferenceNumber { get; set; }
        public string Notes { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class InstallmentExportVM
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; }
        public string CustomerName { get; set; }
        public int InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public DateTime DueDate { get; set; }
        public string Status { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
        public bool IsOverdue { get; set; }
        public int OverdueDays { get; set; }
    }

    public class BulkUpdateExportVM
    {
        public int TotalRequested { get; set; }
        public int TotalProcessed { get; set; }
        public int TotalSuccessful { get; set; }
        public int TotalFailed { get; set; }
        public List<BulkOperationResult> Results { get; set; }
        public List<string> Messages { get; set; }
        public DateTime CompletedAt { get; set; }
        public DateTime ExportDate { get; set; }
    }

    public class ExportValidationErrorVM
    {
        public string FieldName { get; set; }
        public string ErrorMessage { get; set; }
        public string ErrorCode { get; set; }
    }

    public class ExportErrorVM
    {
        public string ErrorType { get; set; }
        public string ErrorMessage { get; set; }
        public string StackTrace { get; set; }
        public string InnerException { get; set; }
    }

    #endregion
}
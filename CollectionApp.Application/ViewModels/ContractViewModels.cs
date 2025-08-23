using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Application.ViewModels
{
    public class OutstandingAmountVM
    {
        public decimal Amount { get; set; }
        public string Currency { get; set; }
        public decimal OutstandingAmount { get => Amount; }
    }

    public class ContractCreateVM : IValidatableObject
    {
        [Required]
        [MaxLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required]
        public Guid CustomerId { get; set; }

        public Guid? StaffId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal PrincipalAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContractType { get; set; } = string.Empty;

        [Required]
        [Range(1, 600)]
        public int TermInMonths { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = "";

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentFrequency { get; set; } = string.Empty;

        [Range(0, 365)]
        public int GracePeriodDays { get; set; }

        [Range(0, 1)]
        public decimal InterestRate { get; set; }

        [Range(0, 1)]
        public decimal LateFeePercentage { get; set; }

        [Range(0, 1)]
        public decimal PenaltyPercentage { get; set; }

        [Range(1, int.MaxValue)]
        public int NumberOfInstallments { get; set; }

        [MaxLength(500)]
        public string? CollateralDescription { get; set; }

        [MaxLength(200)]
        public string? GuarantorName { get; set; }

        [MaxLength(100)]
        public string? GuarantorContact { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
        
        [MaxLength(500)]
        public string? Purpose { get; set; }
        
        [MaxLength(50)]
        public string? SourceOfFunds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && EndDate.Value.Date < StartDate.Date)
            {
                yield return new ValidationResult(
                    "EndDate cannot be earlier than StartDate.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }

    public class ContractUpdateVM : IValidatableObject
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? StaffId { get; set; }
        
        [Required]
        public Guid CustomerId { get; set; }
        
        public string CustomerName { get; set; } = string.Empty;
        
        public ContractStatus Status { get; set; }
        
        [Required]
        [MaxLength(50)]
        public string ContractNumber { get; set; } = string.Empty;

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal TotalAmount { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal PrincipalAmount { get; set; }

        [Required]
        [MaxLength(50)]
        public string ContractType { get; set; } = string.Empty;

        [Required]
        [Range(1, 600)]
        public int TermInMonths { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? EndDate { get; set; }

        [Required]
        [MaxLength(20)]
        public string PaymentFrequency { get; set; } = string.Empty;

        [Range(0, 365)]
        public int GracePeriodDays { get; set; }

        [Range(0, 1)]
        public decimal InterestRate { get; set; }

        [Range(0, 1)]
        public decimal LateFeePercentage { get; set; }

        [Range(0, 1)]
        public decimal PenaltyPercentage { get; set; }

        [Range(1, int.MaxValue)]
        public int NumberOfInstallments { get; set; }

        [MaxLength(500)]
        public string? CollateralDescription { get; set; }

        [MaxLength(200)]
        public string? GuarantorName { get; set; }

        [MaxLength(100)]
        public string? GuarantorContact { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        [MaxLength(500)]
        public string? Purpose { get; set; }

        [MaxLength(50)]
        public string? SourceOfFunds { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (EndDate.HasValue && EndDate.Value.Date < StartDate.Date)
            {
                yield return new ValidationResult(
                    "EndDate cannot be earlier than StartDate.",
                    new[] { nameof(EndDate), nameof(StartDate) }
                );
            }
        }
    }

    public class ContractDetailVM
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public ContractStatus Status { get; set; }
        public decimal InterestRate { get; set; }
        public int NumberOfInstallments { get; set; }
        public decimal OutstandingAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        
        // Additional properties for expanded functionality
        public decimal? PrincipalAmount { get; set; }
        public string? ContractType { get; set; }
        public int? TermInMonths { get; set; }
        public string? PaymentFrequency { get; set; }
        public int? GracePeriodDays { get; set; }
        public decimal? LateFeePercentage { get; set; }
        public decimal? PenaltyPercentage { get; set; }
        public string? CollateralDescription { get; set; }
        public string? GuarantorName { get; set; }
        public string? GuarantorContact { get; set; }
        public string? Notes { get; set; }
        public Guid? StaffId { get; set; }
        public string? StaffName { get; set; }
        public string? Purpose { get; set; }
        public string? SourceOfFunds { get; set; }
        
        public IReadOnlyList<ContractInstallmentSummaryVM> Installments { get; set; } = Array.Empty<ContractInstallmentSummaryVM>();
        public IReadOnlyList<ContractPaymentSummaryVM> Payments { get; set; } = Array.Empty<ContractPaymentSummaryVM>();
    }

    public class ContractListVM
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public Guid CustomerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public ContractStatus Status { get; set; }
        public DateTime StartDate { get; set; }
        public decimal OutstandingAmount { get; set; }
        public DateTime CreatedAt { get; set; }
        
        // Additional properties for expanded functionality
        public decimal PrincipalAmount { get; set; }
        public string? ContractType { get; set; }
        public int? TermInMonths { get; set; }
        public Guid? StaffId { get; set; }
        public string? StaffName { get; set; }
    }

    public class ContractInstallmentSummaryVM
    {
        public Guid Id { get; set; }
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public InstallmentStatus Status { get; set; }
        public decimal PaidAmount { get; set; }
        public DateTime? PaymentDate { get; set; }
    }

    public class ContractPaymentSummaryVM
    {
        public Guid Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public PaymentMethod PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
    }

    public class ContractTermsPatch
    {
        public DateTime StartDate { get; set; }
        public decimal InterestRate { get; set; }
        public int NumberOfInstallments { get; set; }
    }

    // New view models for financial operations
    public class PaymentModalVM
    {
        [Required]
        public Guid ContractId { get; set; }

        [Required]
        public Guid InstallmentId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InstallmentNumber { get; set; }

        [Required]
        [Range(typeof(decimal), "0.00", "79228162514264337593543950335")]
        public decimal InstallmentAmount { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; } = DateTime.UtcNow.Date;

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public Guid? ProcessedByStaffId { get; set; }
    }

    public class WaiveInstallmentVM
    {
        [Required]
        public Guid InstallmentId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InstallmentNumber { get; set; }

        [Required]
        [Range(typeof(decimal), "0.00", "79228162514264337593543950335")]
        public decimal InstallmentAmount { get; set; }

        [Required]
        [MaxLength(1000)]
        public string Reason { get; set; } = string.Empty;

        public Guid? WaivedByStaffId { get; set; }
    }

    public class RescheduleInstallmentVM : IValidatableObject
    {
        [Required]
        public Guid InstallmentId { get; set; }

        [Required]
        [Range(1, int.MaxValue)]
        public int InstallmentNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime CurrentDueDate { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime NewDueDate { get; set; }

        [MaxLength(1000)]
        public string? Reason { get; set; }

        public Guid? RescheduledByStaffId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (NewDueDate.Date <= DateTime.UtcNow.Date)
            {
                yield return new ValidationResult(
                    "New due date must be a future date.",
                    new[] { nameof(NewDueDate) }
                );
            }
        }
    }

    public class ContractFinancialSummaryVM
    {
        public Guid ContractId { get; set; }
        public string ContractNumber { get; set; }
        public string CustomerName { get; set; }
        public int TotalInstallments { get; set; }
        public int PaidInstallments { get; set; }
        public int PendingInstallments { get; set; }
        public int OverdueInstallments { get; set; }
        public int InstallmentCount { get; set; }
        public int PaidInstallmentCount { get; set; }
        public int OverdueInstallmentCount { get; set; }
        public decimal TotalAmount { get; set; }
        public decimal PaidAmount { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal CollectionRate { get; set; }
        public decimal PaymentPercentage { get; set; }
        public decimal AveragePaymentAmount { get; set; }
        public DateTime? LastPaymentDate { get; set; }
        public DateTime? NextDueDate { get; set; }
        public string Currency { get; set; } = "USD";
    }

    public enum BulkInstallmentActionType
    {
        Waive,
        Reschedule,
        MarkPaid
    }

    public class BulkInstallmentActionVM : IValidatableObject
    {
        [Required]
        public List<Guid> InstallmentIds { get; set; } = new List<Guid>();

        [Required]
        public BulkInstallmentActionType ActionType { get; set; }

        [MaxLength(1000)]
        public string? CommonReason { get; set; }

        [DataType(DataType.Date)]
        public DateTime? CommonNewDueDate { get; set; }

        public Guid? ProcessedByStaffId { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (InstallmentIds == null || InstallmentIds.Count == 0)
            {
                yield return new ValidationResult(
                    "At least one installment must be selected.",
                    new[] { nameof(InstallmentIds) }
                );
            }

            if (ActionType == BulkInstallmentActionType.Waive && string.IsNullOrWhiteSpace(CommonReason))
            {
                yield return new ValidationResult(
                    "Reason is required for waive action.",
                    new[] { nameof(CommonReason) }
                );
            }

            if (ActionType == BulkInstallmentActionType.Reschedule)
            {
                if (!CommonNewDueDate.HasValue)
                {
                    yield return new ValidationResult(
                        "New due date is required for reschedule action.",
                        new[] { nameof(CommonNewDueDate) }
                    );
                }
                else if (CommonNewDueDate.Value.Date <= DateTime.UtcNow.Date)
                {
                    yield return new ValidationResult(
                        "New due date must be a future date.",
                        new[] { nameof(CommonNewDueDate) }
                    );
                }
            }
        }
    }
    
    public class ContractAnalyticsVM
    {
        public int TotalContracts { get; set; }
        public int ActiveContracts { get; set; }
        public int CompletedContracts { get; set; }
        public int DefaultedContracts { get; set; }
        public decimal TotalContractValue { get; set; }
        public decimal OutstandingAmount { get; set; }
        public decimal CollectedAmount { get; set; }
        public decimal CollectionRate { get; set; }
        public int OverdueContractsCount { get; set; }
        public decimal OverdueAmount { get; set; }
        public Dictionary<string, int> ContractsByType { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> AmountByContractType { get; set; } = new Dictionary<string, decimal>();
        public Dictionary<string, int> ContractsByMonth { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> CollectionByMonth { get; set; } = new Dictionary<string, decimal>();
    }
}


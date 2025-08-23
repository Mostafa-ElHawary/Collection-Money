using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Application.ViewModels
{
    public class PaymentCreateVM
    {
        [Required]
        public Guid ContractId { get; set; }

        [Required]
        public Guid InstallmentId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public Guid? StaffId { get; set; }

        public Guid? ProcessedByStaffId { get; set; }
    }

    public class PaymentUpdateVM
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class PaymentDetailVM
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public Guid? InstallmentId { get; set; }
        public int? InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string? Notes { get; set; }
        public Guid? StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public Guid? ReceiptId { get; set; }
        public string? ReceiptNumber { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class PaymentListVM
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public Guid? InstallmentId { get; set; }
        public int? InstallmentNumber { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime PaymentDate { get; set; }
        public PaymentMethod PaymentMethod { get; set; }
        public string? ReferenceNumber { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public Guid? ProcessedByStaffId { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    public class PartialPaymentVM
    {
        [Required]
        public Guid ContractId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime PaymentDate { get; set; }

        [Required]
        public PaymentMethod PaymentMethod { get; set; }

        [MaxLength(100)]
        public string? ReferenceNumber { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }

        public Guid? ProcessedByStaffId { get; set; }
    }

    public class PaymentSearchCriteriaVM
    {
        [MaxLength(50)]
        public string? ContractNumber { get; set; }

        [MaxLength(100)]
        public string? CustomerName { get; set; }

        public PaymentMethod? PaymentMethod { get; set; }

        [DataType(DataType.Date)]
        public DateTime? FromDate { get; set; }

        [DataType(DataType.Date)]
        public DateTime? ToDate { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal? MinAmount { get; set; }

        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal? MaxAmount { get; set; }

        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string? Currency { get; set; }

        public Guid? StaffId { get; set; }
    }

    public class PaymentHistoryVM
    {
        public Guid? ContractId { get; set; }
        public Guid? CustomerId { get; set; }
        public List<PaymentListVM> Payments { get; set; } = new List<PaymentListVM>();
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public int PaymentCount { get; set; }
    }

    public class FileDownloadResult
    {
        public byte[] Bytes { get; set; } = Array.Empty<byte>();
        public string FileName { get; set; } = string.Empty;
        public string ContentType { get; set; } = string.Empty;
    }

    public class ReversePaymentVM
    {
        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        [MaxLength(500)]
        public string Reason { get; set; } = string.Empty;
    }

    public class BulkPaymentResultVM
    {
        public List<PaymentDetailVM> SuccessfulPayments { get; set; } = new List<PaymentDetailVM>();
        public List<BulkPaymentErrorVM> FailedPayments { get; set; } = new List<BulkPaymentErrorVM>();
        public int TotalProcessed { get; set; }
        public int SuccessCount { get; set; }
        public int ErrorCount { get; set; }
        public bool HasErrors => ErrorCount > 0;
        public bool AllSuccessful => ErrorCount == 0;
    }

    public class BulkPaymentErrorVM
    {
        public Guid InstallmentId { get; set; }
        public Guid ContractId { get; set; }
        public string ErrorMessage { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
    }
}


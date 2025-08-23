using System;
using System.ComponentModel.DataAnnotations;

namespace CollectionApp.Application.ViewModels
{
    public class ReceiptCreateVM
    {
        [Required]
        [MaxLength(50)]
        public string ReceiptNumber { get; set; } = string.Empty;

        [Required]
        public Guid PaymentId { get; set; }

        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime IssueDate { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        [Required]
        public Guid StaffId { get; set; }
    }

    public class ReceiptUpdateVM
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }
    }

    public class ReceiptDetailVM
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public Guid PaymentId { get; set; }
        public DateTime PaymentDate { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string? Description { get; set; }
        public Guid StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class ReceiptListVM
    {
        public Guid Id { get; set; }
        public string ReceiptNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public DateTime IssueDate { get; set; }
        public string? Description { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
    }
}


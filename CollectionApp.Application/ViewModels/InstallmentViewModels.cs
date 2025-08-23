using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Application.ViewModels
{
    public class InstallmentCreateVM
    {
        [Required]
        public Guid ContractId { get; set; }

        [Range(1, int.MaxValue)]
        public int InstallmentNumber { get; set; }

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [StringLength(500)]
        public string? Notes { get; set; }
    }

    public class InstallmentUpdateVM
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [Range(typeof(decimal), "0.01", "79228162514264337593543950335")]
        public decimal Amount { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime DueDate { get; set; }

        [StringLength(500)]
        public string? Notes { get; set; }

        public InstallmentStatus Status { get; set; }

        [DataType(DataType.Date)]
        public DateTime? PaymentDate { get; set; }
    }

    public class InstallmentDetailVM
    {
        public Guid Id { get; set; }
        public Guid ContractId { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public InstallmentStatus Status { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaidCurrency { get; set; } = string.Empty;
        public DateTime? PaidDate { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public IReadOnlyList<InstallmentPaymentSummaryVM> Payments { get; set; } = Array.Empty<InstallmentPaymentSummaryVM>();
    }

    public class InstallmentListVM
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public string CustomerName { get; set; } = string.Empty;
        public int InstallmentNumber { get; set; }
        public DateTime DueDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public InstallmentStatus Status { get; set; }
        public decimal PaidAmount { get; set; }
        public string PaidCurrency { get; set; } = string.Empty;
        public decimal RemainingAmount { get; set; }
        public bool IsOverdue { get; set; }
        public DateTime? PaidDate { get; set; }
    }

    public class InstallmentPaymentSummaryVM
    {
        public Guid Id { get; set; }
        public DateTime PaymentDate { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? ReferenceNumber { get; set; }
    }

    /// <summary>
    /// Contains ViewModels for installment analytics and financial reporting functionality.
    /// These ViewModels support comprehensive installment analysis including status summaries,
    /// collection reports, overdue analysis, and payment history tracking.
    /// </summary>
    public class InstallmentAnalyticsViewModels
    {
        /// <summary>
        /// ViewModel for analyzing installment status and financial summaries by contract.
        /// Provides comprehensive overview of installment completion and outstanding amounts.
        /// </summary>
        public class InstallmentStatusSummaryVM
        {
            /// <summary>
            /// Unique identifier of the contract
            /// </summary>
            public Guid ContractId { get; set; }

            /// <summary>
            /// Total number of installments in the contract
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total installments must be non-negative")]
            [Display(Name = "Total Installments")]
            public int TotalInstallments { get; set; }

            /// <summary>
            /// Number of installments that have been paid
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Paid installments must be non-negative")]
            [Display(Name = "Paid Installments")]
            public int PaidInstallments { get; set; }

            /// <summary>
            /// Number of installments still pending payment
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Pending installments must be non-negative")]
            [Display(Name = "Pending Installments")]
            public int PendingInstallments { get; set; }

            /// <summary>
            /// Number of installments that are overdue
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Overdue installments must be non-negative")]
            [Display(Name = "Overdue Installments")]
            public int OverdueInstallments { get; set; }

            /// <summary>
            /// Number of installments that have been waived
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Waived installments must be non-negative")]
            [Display(Name = "Waived Installments")]
            public int WaivedInstallments { get; set; }

            /// <summary>
            /// Total amount of all installments in the contract
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount must be non-negative")]
            [Display(Name = "Total Amount")]
            public decimal TotalAmount { get; set; }

            /// <summary>
            /// Total amount that has been paid
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Paid amount must be non-negative")]
            [Display(Name = "Paid Amount")]
            public decimal PaidAmount { get; set; }

            /// <summary>
            /// Outstanding amount remaining to be paid
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Outstanding amount must be non-negative")]
            [Display(Name = "Outstanding Amount")]
            public decimal OutstandingAmount { get; set; }

            /// <summary>
            /// Number of upcoming installments (pending and not yet due)
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Upcoming installments must be non-negative")]
            [Display(Name = "Upcoming Installments")]
            public int UpcomingInstallments { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing collection performance over a specified time period.
        /// Provides insights into installment collection rates and financial performance.
        /// </summary>
        public class CollectionReportVM
        {
            /// <summary>
            /// Start date for the collection analysis period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the collection analysis period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total number of installments in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total installments must be non-negative")]
            [Display(Name = "Total Installments")]
            public int TotalInstallments { get; set; }


            /// <summary>
            /// Number of installments that were paid in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Paid installments must be non-negative")]
            [Display(Name = "Paid Installments")]
            public int PaidInstallments { get; set; }

            /// <summary>
            /// Number of installments that are overdue in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Overdue installments must be non-negative")]
            [Display(Name = "Overdue Installments")]
            public int OverdueInstallments { get; set; }

            /// <summary>
            /// Total amount of all installments in the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount must be non-negative")]
            [Display(Name = "Total Amount")]
            public decimal TotalAmount { get; set; }

            /// <summary>
            /// Total amount collected in the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Collected amount must be non-negative")]
            [Display(Name = "Collected Amount")]
            public decimal CollectedAmount { get; set; }

            /// <summary>
            /// Outstanding amount remaining to be collected
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Outstanding amount must be non-negative")]
            [Display(Name = "Outstanding Amount")]
            public decimal OutstandingAmount { get; set; }

            /// <summary>
            /// Collection rate as a percentage of total amount
            /// </summary>
            [Range(0, 100, ErrorMessage = "Collection rate must be between 0 and 100")]
            [Display(Name = "Collection Rate (%)")]
            public decimal CollectionRate { get; set; }


        }

        /// <summary>
        /// ViewModel for analyzing overdue installments and aging patterns.
        /// Provides insights into overdue amounts and helps prioritize collection efforts.
        /// </summary>
        public class OverdueAnalysisVM
        {
            /// <summary>
            /// Total number of overdue installments
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total overdue must be non-negative")]
            [Display(Name = "Total Overdue")]
            public int TotalOverdue { get; set; }

            /// <summary>
            /// Total amount of overdue installments
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total overdue amount must be non-negative")]
            [Display(Name = "Total Overdue Amount")]
            public decimal TotalOverdueAmount { get; set; }

            /// <summary>
            /// Average number of days installments are overdue
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Average overdue days must be non-negative")]
            [Display(Name = "Average Overdue Days")]
            public int AverageOverdueDays { get; set; }

            /// <summary>
            /// Breakdown of overdue installments by age categories
            /// </summary>
            [Display(Name = "Overdue by Age")]
            public Dictionary<string, int> OverdueByAge { get; set; } = new Dictionary<string, int>();
        }

        /// <summary>
        /// ViewModel for tracking payment history and transaction details.
        /// Provides comprehensive payment information for audit and reconciliation purposes.
        /// </summary>
        public class PaymentHistoryVM
        {
            /// <summary>
            /// Unique identifier of the payment record
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Unique identifier of the installment being paid
            /// </summary>
            [Display(Name = "Installment ID")]
            public Guid InstallmentId { get; set; }

            /// <summary>
            /// Amount of the payment
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Payment amount must be non-negative")]
            [Display(Name = "Payment Amount")]
            public decimal Amount { get; set; }

            /// <summary>
            /// Date when the payment was made
            /// </summary>
            [Required(ErrorMessage = "Payment date is required")]
            [Display(Name = "Payment Date")]
            public DateTime PaymentDate { get; set; }

            /// <summary>
            /// Method used for the payment
            /// </summary>
            [Required(ErrorMessage = "Payment method is required")]
            [Display(Name = "Payment Method")]
            public string PaymentMethod { get; set; } = string.Empty;

            /// <summary>
            /// Reference number for the payment transaction
            /// </summary>
            [Required(ErrorMessage = "Reference number is required")]
            [Display(Name = "Reference Number")]
            public string ReferenceNumber { get; set; } = string.Empty;

            /// <summary>
            /// Current status of the payment
            /// </summary>
            [Required(ErrorMessage = "Payment status is required")]
            [Display(Name = "Payment Status")]
            public string Status { get; set; } = string.Empty;
        }

        public class FinancialAnalyticsVM
        {
            public int TotalPayments { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal AveragePaymentAmount { get; set; }
            public Dictionary<string, decimal> PaymentTrendsByMonth { get; set; } = new();
            public Dictionary<PaymentMethod, decimal> PaymentMethodDistribution { get; set; } = new();
            public decimal CollectionEfficiencyRate { get; set; }
            public int OverduePaymentsCount { get; set; }
            public decimal OverdueAmount { get; set; }
            public Dictionary<string, decimal> RevenueByProduct { get; set; } = new();
            public Dictionary<string, decimal> RevenueByRegion { get; set; } = new();
            public List<TopCustomerAnalytics> TopCustomers { get; set; } = new();
        }
        
           public class TopCustomerAnalytics
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalPayments { get; set; }
            public int PaymentCount { get; set; }
        }
    }
}


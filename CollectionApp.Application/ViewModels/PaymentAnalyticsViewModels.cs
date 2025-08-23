using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Application.ViewModels
{
    /// <summary>
    /// Contains ViewModels for payment analytics and financial reporting functionality.
    /// These ViewModels support comprehensive payment analysis including collection reports,
    /// payment trends, and financial performance metrics.
    /// </summary>
    public class PaymentAnalyticsViewModels
    {
        /// <summary>
        /// ViewModel for analyzing collection performance over a specified time period.
        /// Provides insights into payment collection rates and financial performance.
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
            /// Total number of payments in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total payments must be non-negative")]
            [Display(Name = "Total Payments")]
            public int TotalPayments { get; set; }

            /// <summary>
            /// Total amount of all payments in the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount must be non-negative")]
            [Display(Name = "Total Amount")]
            public decimal TotalAmount { get; set; }

            /// <summary>
            /// Average payment amount in the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average payment amount must be non-negative")]
            [Display(Name = "Average Payment Amount")]
            public decimal AveragePaymentAmount { get; set; }

            /// <summary>
            /// Number of payments by payment method
            /// </summary>
            [Display(Name = "Payments by Method")]
            public Dictionary<string, int> PaymentsByMethod { get; set; } = new Dictionary<string, int>();

            /// <summary>
            /// Amount collected by payment method
            /// </summary>
            [Display(Name = "Amount by Method")]
            public Dictionary<string, decimal> AmountByMethod { get; set; } = new Dictionary<string, decimal>();

            /// <summary>
            /// Payments by day of week
            /// </summary>
            [Display(Name = "Payments by Day")]
            public Dictionary<string, int> PaymentsByDay { get; set; } = new Dictionary<string, int>();

            /// <summary>
            /// Payments by month
            /// </summary>
            [Display(Name = "Payments by Month")]
            public Dictionary<string, int> PaymentsByMonth { get; set; } = new Dictionary<string, int>();

            /// <summary>
            /// Collection amount by month
            /// </summary>
            [Display(Name = "Collection by Month")]
            public Dictionary<string, decimal> CollectionByMonth { get; set; } = new Dictionary<string, decimal>();

            public int TotalInstallments { get; set; }  // Add this property


        }

 public class FinancialAnalyticsVM
        {
            public int TotalPayments { get; set; }
            public decimal TotalAmount { get; set; }
            public decimal AveragePaymentAmount { get; set; }
            public Dictionary<string, decimal> PaymentTrendsByMonth { get; set; } = new();
            public Dictionary<PaymentMethod, decimal> PaymentMethodDistribution { get; set; } = new();
            public decimal CollectionEfficiencyRate { get; set; }
            public decimal OnTimePaymentRate { get; set; }
            public decimal LatePaymentRate { get; set; }
            public decimal DefaultRate { get; set; }
            public Dictionary<string, decimal> RevenueByProduct { get; set; } = new();
            public Dictionary<string, decimal> RevenueByRegion { get; set; } = new();
            public List<TopCustomerVM> TopCustomers { get; set; } = new();
        }

        public class TopCustomerVM
        {
            public Guid CustomerId { get; set; }
            public string CustomerName { get; set; }
            public decimal TotalPayments { get; set; }
            public int PaymentCount { get; set; }
            public decimal AveragePaymentAmount { get; set; }
        }

    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CollectionApp.Application.ViewModels
{
    public class LedgerEntryCreateVM : IValidatableObject
    {
        [Required]
        public DateTime TransactionDate { get; set; }

        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal DebitAmount { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal CreditAmount { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal Balance { get; set; }

        [Required]
        [StringLength(3, MinimumLength = 3)]
        [RegularExpression("^[A-Za-z]{3}$")]
        public string Currency { get; set; } = string.Empty;

        [MaxLength(100)]
        public string? ReferenceType { get; set; }

        public Guid? ReferenceId { get; set; }

        public Guid? ContractId { get; set; }

        public Guid? CustomerId { get; set; }

        public Guid? StaffId { get; set; }
        public string EntryType { get; internal set; }
        public decimal Amount { get; internal set; }

        public System.Collections.Generic.IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            var hasDebit = DebitAmount > 0;
            var hasCredit = CreditAmount > 0;
            if (hasDebit && hasCredit)
            {
                yield return new ValidationResult("Only one of DebitAmount or CreditAmount may be greater than zero.", new[] { nameof(DebitAmount), nameof(CreditAmount) });
            }
        }
    }

    public class LedgerEntryUpdateVM
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(500)]
        public string? Description { get; set; }
    }

    public class LedgerEntryDetailVM
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? ReferenceType { get; set; }
        public Guid? ReferenceId { get; set; }
        public Guid? ContractId { get; set; }
        public string? ContractNumber { get; set; }
        public Guid? CustomerId { get; set; }
        public string? CustomerName { get; set; }
        public Guid? StaffId { get; set; }
        public string? StaffName { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class LedgerEntryListVM
    {
        public Guid Id { get; set; }
        public DateTime TransactionDate { get; set; }
        public string Description { get; set; } = string.Empty;
        public decimal DebitAmount { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal Balance { get; set; }
        public string Currency { get; set; } = string.Empty;
        public string? ReferenceType { get; set; }
        public string? CustomerName { get; set; }
        public string? ContractNumber { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Contains ViewModels for ledger analytics and financial reporting functionality.
    /// These ViewModels support comprehensive financial analysis including trial balances,
    /// cash flow reports, accounts receivable, audit trails, and profitability analysis.
    /// </summary>
    public static class LedgerAnalyticsViewModels
    {
        /// <summary>
        /// ViewModel for individual accounts receivable item
        /// </summary>
        public class AccountReceivableItemVM
        {
            /// <summary>
            /// Unique identifier of the customer
            /// </summary>
            [Display(Name = "Customer ID")]
            public Guid CustomerId { get; set; }
            
            /// <summary>
            /// Name of the customer
            /// </summary>
            [Display(Name = "Customer Name")]
            public string CustomerName { get; set; } = string.Empty;
            
            /// <summary>
            /// Unique identifier of the contract
            /// </summary>
            [Display(Name = "Contract ID")]
            public Guid ContractId { get; set; }
            
            /// <summary>
            /// Contract number or reference
            /// </summary>
            [Display(Name = "Contract Number")]
            public string ContractNumber { get; set; } = string.Empty;
            
            /// <summary>
            /// Outstanding amount for this receivable item
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Amount must be non-negative")]
            [Display(Name = "Amount")]
            public decimal Amount { get; set; }
            
            /// <summary>
            /// Total outstanding amount for this receivable item
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount must be non-negative")]
            [Display(Name = "Total Amount")]
            public decimal TotalAmount { get; set; }
            
            /// <summary>
            /// Date of the last payment made for this receivable item
            /// </summary>
            [Display(Name = "Last Payment Date")]
            public DateTime LastPaymentDate { get; set; }
            
            /// <summary>
            /// Due date for this receivable item
            /// </summary>
            [Display(Name = "Due Date")]
            public DateTime DueDate { get; set; }
            
            /// <summary>
            /// Number of days this item is overdue
            /// </summary>
            [Display(Name = "Days Overdue")]
            public int DaysOverdue { get; set; }
        }
        /// <summary>
        /// ViewModel for trial balance analysis and financial statement preparation.
        /// Ensures accounting equation balance and supports financial reporting requirements.
        /// </summary>
        public class TrialBalanceVM
        {
            /// <summary>
            /// Date as of which the trial balance is prepared
            /// </summary>
            [Required(ErrorMessage = "As of date is required")]
            [Display(Name = "As of Date")]
            public DateTime AsOfDate { get; set; }

            /// <summary>
            /// Total of all debit amounts in the ledger
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total debits must be non-negative")]
            [Display(Name = "Total Debits")]
            public decimal TotalDebits { get; set; }

            /// <summary>
            /// Total of all credit amounts in the ledger
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total credits must be non-negative")]
            [Display(Name = "Total Credits")]
            public decimal TotalCredits { get; set; }

            /// <summary>
            /// Difference between total debits and total credits
            /// </summary>
            [Display(Name = "Difference")]
            public decimal Difference { get; set; }

            /// <summary>
            /// Indicates whether the trial balance is balanced (debits = credits)
            /// </summary>
            [Display(Name = "Is Balanced")]
            public bool IsBalanced { get; set; }
        }

        /// <summary>
        /// ViewModel for cash flow analysis and reporting.
        /// Tracks cash inflows, outflows, and net cash flow over specified periods.
        /// </summary>
        public class CashFlowReportVM
        {
            /// <summary>
            /// Start date for the cash flow analysis period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the cash flow analysis period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total cash inflows during the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Cash inflows must be non-negative")]
            [Display(Name = "Cash Inflows")]
            public decimal CashInflows { get; set; }

            /// <summary>
            /// Total cash outflows during the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Cash outflows must be non-negative")]
            [Display(Name = "Cash Outflows")]
            public decimal CashOutflows { get; set; }

            /// <summary>
            /// Net cash flow (inflows - outflows) for the period
            /// </summary>
            [Display(Name = "Net Cash Flow")]
            public decimal NetCashFlow { get; set; }

            /// <summary>
            /// Monthly breakdown of cash flow patterns
            /// </summary>
            [Display(Name = "Cash Flow by Month")]
            public Dictionary<string, decimal> CashFlowByMonth { get; set; } = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// ViewModel for accounts receivable analysis and aging.
        /// Provides insights into outstanding receivables and collection priorities.
        /// </summary>
        public class AccountsReceivableVM
        {
            /// <summary>
            /// List of accounts receivable items
            /// </summary>
            [Display(Name = "Accounts Receivable")]
            public List<AccountReceivableItemVM> AccountsReceivable { get; set; } = new List<AccountReceivableItemVM>();
            
            /// <summary>
            /// Date as of which the receivables analysis is prepared
            /// </summary>
            [Required(ErrorMessage = "As of date is required")]
            [Display(Name = "As of Date")]
            public DateTime AsOfDate { get; set; }

            /// <summary>
            /// Total amount of outstanding receivables
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total receivables must be non-negative")]
            [Display(Name = "Total Receivables")]
            public decimal TotalReceivables { get; set; }

            /// <summary>
            /// Breakdown of receivables by age categories
            /// </summary>
            [Display(Name = "Receivables by Age")]
            public Dictionary<string, decimal> ReceivablesByAge { get; set; } = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// ViewModel for audit trail and change tracking.
        /// Maintains comprehensive record of all financial transactions and modifications.
        /// </summary>
        public class AuditTrailVM
        {
            /// <summary>
            /// Unique identifier of the audit record
            /// </summary>
            public Guid Id { get; set; }

            /// <summary>
            /// Type of entity being audited (e.g., Customer, Contract, Payment)
            /// </summary>
            [Required(ErrorMessage = "Entity type is required")]
            [Display(Name = "Entity Type")]
            public string EntityType { get; set; } = string.Empty;

            /// <summary>
            /// Unique identifier of the entity being audited
            /// </summary>
            [Display(Name = "Entity ID")]
            public Guid EntityId { get; set; }

            /// <summary>
            /// Action performed on the entity (e.g., Create, Update, Delete)
            /// </summary>
            [Required(ErrorMessage = "Action is required")]
            [Display(Name = "Action")]
            public string Action { get; set; } = string.Empty;

            /// <summary>
            /// Description of changes made to the entity
            /// </summary>
            [Required(ErrorMessage = "Changes description is required")]
            [Display(Name = "Changes")]
            public string Changes { get; set; } = string.Empty;

            /// <summary>
            /// Unique identifier of the user who performed the action
            /// </summary>
            [Display(Name = "User ID")]
            public Guid UserId { get; set; }

            /// <summary>
            /// Timestamp when the action was performed
            /// </summary>
            [Required(ErrorMessage = "Timestamp is required")]
            [Display(Name = "Timestamp")]
            public DateTime Timestamp { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing collection efficiency and performance.
        /// Measures the effectiveness of collection efforts and receivables management.
        /// </summary>
        public class CollectionEfficiencyVM
        {
            /// <summary>
            /// Start date for the collection efficiency analysis period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the collection efficiency analysis period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total amount collected during the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total collections must be non-negative")]
            [Display(Name = "Total Collections")]
            public decimal TotalCollections { get; set; }

            /// <summary>
            /// Total amount of receivables at the beginning of the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total receivables must be non-negative")]
            [Display(Name = "Total Receivables")]
            public decimal TotalReceivables { get; set; }

            /// <summary>
            /// Collection rate as a percentage of total receivables
            /// </summary>
            [Range(0, 100, ErrorMessage = "Collection rate must be between 0 and 100")]
            [Display(Name = "Collection Rate (%)")]
            public decimal CollectionRate { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing payment trends and patterns.
        /// Provides insights into payment behavior and cash flow forecasting.
        /// </summary>
        public class PaymentTrendsVM
        {
            /// <summary>
            /// Start date for the payment trends analysis period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the payment trends analysis period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total amount of payments received during the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total payments must be non-negative")]
            [Display(Name = "Total Payments")]
            public decimal TotalPayments { get; set; }

            /// <summary>
            /// Average amount per payment during the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average payment amount must be non-negative")]
            [Display(Name = "Average Payment Amount")]
            public decimal AveragePaymentAmount { get; set; }

            /// <summary>
            /// Monthly breakdown of payment trends and patterns
            /// </summary>
            [Display(Name = "Payment Trends by Month")]
            public Dictionary<string, decimal> PaymentTrendsByMonth { get; set; } = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// ViewModel for analyzing outstanding amounts and customer balances.
        /// Helps prioritize collection efforts and manage credit risk.
        /// </summary>
        public class OutstandingAnalysisVM
        {
            /// <summary>
            /// Total amount of outstanding receivables
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total outstanding must be non-negative")]
            [Display(Name = "Total Outstanding")]
            public decimal TotalOutstanding { get; set; }

            /// <summary>
            /// Breakdown of outstanding amounts by customer
            /// </summary>
            [Display(Name = "Outstanding by Customer")]
            public Dictionary<Guid, decimal> OutstandingByCustomer { get; set; } = new Dictionary<Guid, decimal>();

            /// <summary>
            /// Breakdown of outstanding amounts by contract
            /// </summary>
            [Display(Name = "Outstanding by Contract")]
            public Dictionary<Guid, decimal> OutstandingByContract { get; set; } = new Dictionary<Guid, decimal>();

            /// <summary>
            /// Average outstanding amount per customer or contract
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average outstanding must be non-negative")]
            [Display(Name = "Average Outstanding")]
            public decimal AverageOutstanding { get; set; }
            
            /// <summary>
            /// Breakdown of outstanding amounts by age categories
            /// </summary>
            [Display(Name = "Ageing Buckets")]
            public Dictionary<string, decimal> AgeingBuckets { get; set; } = new Dictionary<string, decimal>();
        }

        /// <summary>
        /// ViewModel for analyzing profitability and financial performance.
        /// Provides insights into revenue, expenses, and profit margins.
        /// </summary>
        public class ProfitabilityAnalysisVM
        {
            /// <summary>
            /// Start date for the profitability analysis period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the profitability analysis period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total revenue generated during the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total revenue must be non-negative")]
            [Display(Name = "Total Revenue")]
            public decimal TotalRevenue { get; set; }

            /// <summary>
            /// Total expenses incurred during the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total expenses must be non-negative")]
            [Display(Name = "Total Expenses")]
            public decimal TotalExpenses { get; set; }

            /// <summary>
            /// Net profit (revenue - expenses) for the period
            /// </summary>
            [Display(Name = "Net Profit")]
            public decimal NetProfit { get; set; }

            /// <summary>
            /// Profit margin as a percentage of revenue
            /// </summary>
            [Range(0, 100, ErrorMessage = "Profit margin must be between 0 and 100")]
            [Display(Name = "Profit Margin (%)")]
            public decimal ProfitMargin { get; set; }
            
            /// <summary>
            /// Detailed breakdown of revenue sources
            /// </summary>
            [Display(Name = "Revenue Breakdown")]
            public Dictionary<string, decimal> Revenue { get; set; } = new Dictionary<string, decimal>();
            
            /// <summary>
            /// Detailed breakdown of expense categories
            /// </summary>
            [Display(Name = "Expense Breakdown")]
            public Dictionary<string, decimal> Expenses { get; set; } = new Dictionary<string, decimal>();
            
            /// <summary>
            /// Bad debt write-offs for the period
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Bad debt must be non-negative")]
            [Display(Name = "Bad Debt")]
            public decimal BadDebt { get; set; }
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CollectionApp.Application.ViewModels
{
    /// <summary>
    /// Enumeration of staff activity types for improved type safety
    /// </summary>
    public enum StaffActivityType
    {
        Visit,
        FollowUp,
        Payment,
        Contract,
        Customer,
        Meeting,
        Training,
        Other
    }

    /// <summary>
    /// Enumeration of related entity types for improved type safety
    /// </summary>
    public enum RelatedEntityType
    {
        Customer,
        Contract,
        Payment,
        Visit,
        FollowUp,
        Installment,
        Receipt,
        Other
    }

    /// <summary>
    /// ViewModel for creating new staff members.
    /// Contains all necessary information to register a new staff member in the system.
    /// </summary>
    public class StaffCreateVM
    {
        [Required]
        [MaxLength(50)]
        public string EmployeeId { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Position { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Department { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        public DateTime HireDate { get; set; }

        public bool IsActive { get; set; } = true;

        // Phone (flattened)
        [Required]
        [StringLength(3, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string PhoneCountryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string PhoneAreaCode { get; set; } = string.Empty;

        [Required]
        [StringLength(12, MinimumLength = 4)]
        [RegularExpression("^\\d+$")]
        public string PhoneNumber { get; set; } = string.Empty;

        [MaxLength(20)]
        public string PhoneType { get; set; } = string.Empty;

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Salary { get; set; }

        public List<string> Permissions { get; set; } = new();

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class StaffUpdateVM
    {
        [Required]
        public Guid Id { get; set; }

        [MaxLength(100)]
        public string? FirstName { get; set; }

        [MaxLength(100)]
        public string? LastName { get; set; }

        [MaxLength(100)]
        public string? Position { get; set; }

        [MaxLength(100)]
        public string? Department { get; set; }

        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        [DataType(DataType.Date)]
        public DateTime? HireDate { get; set; }

        public bool IsActive { get; set; }

        // Phone (flattened)
        [StringLength(3, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string? PhoneCountryCode { get; set; }

        [StringLength(5, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string? PhoneAreaCode { get; set; }

        [StringLength(12, MinimumLength = 4)]
        [RegularExpression("^\\d+$")]
        public string? PhoneNumber { get; set; }

        [MaxLength(20)]
        public string? PhoneType { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? Salary { get; set; }

        public List<string> Permissions { get; set; } = new();

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class StaffDetailVM
    {
        public Guid Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => string.Join(" ", StringHelpers.JoinNonEmpty(FirstName, LastName));
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public DateTime HireDate { get; set; }
        public bool IsActive { get; set; }

        // Phone (flattened)
        public string? PhoneCountryCode { get; set; }
        public string? PhoneAreaCode { get; set; }
        public string? PhoneNumber { get; set; }

        public string PhoneDisplay
        {
            get
            {
                var parts = StringHelpers.JoinNonEmpty(PhoneCountryCode, PhoneAreaCode, PhoneNumber);
                return string.Join("-", parts);
            }
        }

        public string? PhoneType { get; set; }
        public decimal? Salary { get; set; }
        public IReadOnlyList<string> Permissions { get; set; } = Array.Empty<string>();
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public IReadOnlyList<StaffRelatedSummaryVM> Payments { get; set; } = Array.Empty<StaffRelatedSummaryVM>();
        public IReadOnlyList<StaffRelatedSummaryVM> Receipts { get; set; } = Array.Empty<StaffRelatedSummaryVM>();
        public IReadOnlyList<StaffRelatedSummaryVM> Visits { get; set; } = Array.Empty<StaffRelatedSummaryVM>();
        public IReadOnlyList<StaffRelatedSummaryVM> FollowUps { get; set; } = Array.Empty<StaffRelatedSummaryVM>();
    }

    public class StaffListVM
    {
        public Guid Id { get; set; }
        public string EmployeeId { get; set; } = string.Empty;
        public string FirstName { get; set; } = string.Empty;
        public string LastName { get; set; } = string.Empty;
        public string FullName => string.Join(" ", StringHelpers.JoinNonEmpty(FirstName, LastName));
        public string? Position { get; set; }
        public string? Department { get; set; }
        public string? Email { get; set; }
        public bool IsActive { get; set; }
        public DateTime HireDate { get; set; }
        public string? PhoneCountryCode { get; set; }
        public string? PhoneAreaCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string PhoneDisplay
        {
            get
            {
                var parts = StringHelpers.JoinNonEmpty(PhoneCountryCode, PhoneAreaCode, PhoneNumber);
                return string.Join("-", parts);
            }
        }
        public string? PhoneType { get; set; }
    }

    public class StaffRelatedSummaryVM
    {
        public Guid Id { get; set; }
        public string Reference { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public string Description { get; set; } = string.Empty;
    }

    /// <summary>
    /// Contains ViewModels for staff analytics and performance reporting functionality.
    /// These ViewModels support comprehensive staff analysis including activity tracking,
    /// performance metrics, collection performance, and workload analysis.
    /// </summary>
    public class StaffAnalyticsViewModels
    {
        /// <summary>
        /// ViewModel for tracking individual staff activities and transactions.
        /// Provides detailed information about staff actions including payments, visits, and follow-ups.
        /// </summary>
        public class StaffActivityVM
        {
            /// <summary>
            /// Type of activity performed by the staff member
            /// </summary>
            [Required(ErrorMessage = "Activity type is required")]
            [Display(Name = "Activity Type")]
            public StaffActivityType ActivityType { get; set; }

            /// <summary>
            /// Unique identifier of the activity
            /// </summary>
            public Guid ActivityId { get; set; }

            /// <summary>
            /// Detailed description of the activity
            /// </summary>
            [Required(ErrorMessage = "Activity description is required")]
            [Display(Name = "Description")]
            public string Description { get; set; } = string.Empty;

            /// <summary>
            /// Financial amount associated with the activity
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Amount must be non-negative")]
            [Display(Name = "Amount")]
            public decimal Amount { get; set; }

            /// <summary>
            /// Date when the activity occurred
            /// </summary>
            [Required(ErrorMessage = "Activity date is required")]
            [Display(Name = "Activity Date")]
            public DateTime Date { get; set; }

            /// <summary>
            /// Unique identifier of the related entity (customer, contract, etc.)
            /// </summary>
            [Display(Name = "Related Entity ID")]
            public Guid RelatedEntityId { get; set; }

            /// <summary>
            /// Type of entity related to the activity
            /// </summary>
            [Required(ErrorMessage = "Related entity type is required")]
            [Display(Name = "Related Entity Type")]
            public RelatedEntityType RelatedEntityType { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing staff performance over a specified time period.
        /// Provides comprehensive metrics on staff effectiveness and productivity.
        /// </summary>
        public class StaffPerformanceVM
        {
            /// <summary>
            /// Unique identifier of the staff member
            /// </summary>
            public Guid StaffId { get; set; }

            /// <summary>
            /// Start date for the performance analysis period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the performance analysis period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total number of payments processed by the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total payments processed must be non-negative")]
            [Display(Name = "Total Payments Processed")]
            public int TotalPaymentsProcessed { get; set; }

            /// <summary>
            /// Total amount collected by the staff member
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount collected must be non-negative")]
            [Display(Name = "Total Amount Collected")]
            public decimal TotalAmountCollected { get; set; }

            /// <summary>
            /// Total number of visits conducted by the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits conducted must be non-negative")]
            [Display(Name = "Total Visits Conducted")]
            public int TotalVisitsConducted { get; set; }

            /// <summary>
            /// Total number of follow-ups completed by the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total follow-ups completed must be non-negative")]
            [Display(Name = "Total Follow-ups Completed")]
            public int TotalFollowUpsCompleted { get; set; }

            /// <summary>
            /// Average amount per payment processed
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average payment amount must be non-negative")]
            [Display(Name = "Average Payment Amount")]
            public decimal AveragePaymentAmount { get; set; }

            /// <summary>
            /// Ratio of visits that resulted in payments (percentage)
            /// </summary>
            [Range(0, 100, ErrorMessage = "Visit to payment ratio must be between 0 and 100")]
            [Display(Name = "Visit to Payment Ratio (%)")]
            public decimal VisitToPaymentRatio { get; set; }

            /// <summary>
            /// Rate of follow-up completion (percentage)
            /// </summary>
            [Range(0, 100, ErrorMessage = "Follow-up completion rate must be between 0 and 100")]
            [Display(Name = "Follow-up Completion Rate (%)")]
            public decimal FollowUpCompletionRate { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing staff collection performance and effectiveness.
        /// Tracks key metrics related to contract handling and payment processing.
        /// </summary>
        public class CollectionPerformanceVM
        {
            /// <summary>
            /// Unique identifier of the staff member
            /// </summary>
            public Guid StaffId { get; set; }

            /// <summary>
            /// Total number of contracts handled by the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total contracts handled must be non-negative")]
            [Display(Name = "Total Contracts Handled")]
            public int TotalContractsHandled { get; set; }

            /// <summary>
            /// Total number of payments processed by the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total payments processed must be non-negative")]
            [Display(Name = "Total Payments Processed")]
            public int TotalPaymentsProcessed { get; set; }

            /// <summary>
            /// Total amount collected by the staff member
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount collected must be non-negative")]
            [Display(Name = "Total Amount Collected")]
            public decimal TotalAmountCollected { get; set; }

            /// <summary>
            /// Average amount per payment processed
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average payment amount must be non-negative")]
            [Display(Name = "Average Payment Amount")]
            public decimal AveragePaymentAmount { get; set; }

            /// <summary>
            /// Frequency of payments processed (payments per time period)
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Payment frequency must be non-negative")]
            [Display(Name = "Payment Frequency")]
            public double PaymentFrequency { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing staff workload and current task assignments.
        /// Provides insights into staff capacity and pending work items.
        /// </summary>
        public class WorkloadAnalysisVM
        {
            /// <summary>
            /// Unique identifier of the staff member
            /// </summary>
            public Guid StaffId { get; set; }

            /// <summary>
            /// Current date for the workload analysis
            /// </summary>
            [Required(ErrorMessage = "Current date is required")]
            [Display(Name = "Current Date")]
            public DateTime CurrentDate { get; set; }

            /// <summary>
            /// Number of follow-ups pending completion
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Pending follow-ups must be non-negative")]
            [Display(Name = "Pending Follow-ups")]
            public int PendingFollowUps { get; set; }

            /// <summary>
            /// Number of follow-ups that are overdue
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Overdue follow-ups must be non-negative")]
            [Display(Name = "Overdue Follow-ups")]
            public int OverdueFollowUps { get; set; }

            /// <summary>
            /// Number of visits scheduled for the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Scheduled visits must be non-negative")]
            [Display(Name = "Scheduled Visits")]
            public int ScheduledVisits { get; set; }

            /// <summary>
            /// Number of visits completed today by the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Completed visits today must be non-negative")]
            [Display(Name = "Completed Visits Today")]
            public int CompletedVisitsToday { get; set; }

            /// <summary>
            /// Total number of assignments currently assigned to the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total assignments must be non-negative")]
            [Display(Name = "Total Assignments")]
            public int TotalAssignments { get; set; }
        }
    }
}


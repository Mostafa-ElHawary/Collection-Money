using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CollectionApp.Application.ViewModels
{
    public class VisitCreateVM : IValidatableObject
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        [Required]
        public DateTime VisitDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string VisitType { get; set; } = string.Empty;

        [MaxLength(200)]
        public string? Location { get; set; }

        [Required]
        [MaxLength(200)]
        public string Purpose { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [Range(0, 23)]
        public int DurationHours { get; set; }

        [Range(0, 59)]
        public int DurationMinutes { get; set; }

        public TimeSpan? Duration { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Ensure either Duration is provided or a positive combination of DurationHours/DurationMinutes
            if (Duration == null || Duration == TimeSpan.Zero)
            {
                if (DurationHours == 0 && DurationMinutes == 0)
                {
                    yield return new ValidationResult(
                        "Either Duration must be provided or a positive combination of DurationHours and DurationMinutes is required.",
                        new[] { nameof(Duration), nameof(DurationHours), nameof(DurationMinutes) });
                }
            }
        }
    }

    public class VisitUpdateVM : IValidatableObject
    {
        [Required]
        public Guid Id { get; set; }

        [DataType(DataType.DateTime)]
        public DateTime? VisitDate { get; set; }

        [MaxLength(50)]
        public string? VisitType { get; set; }

        [MaxLength(200)]
        public string? Location { get; set; }

        [MaxLength(200)]
        public string? Purpose { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(200)]
        public string? Outcome { get; set; }

        public DateTime? NextVisitDate { get; set; }

        [Range(0, 23)]
        public int? DurationHours { get; set; }

        [Range(0, 59)]
        public int? DurationMinutes { get; set; }

        public TimeSpan? Duration { get; set; }

        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            // Ensure either Duration is provided or a positive combination of DurationHours/DurationMinutes
            if (Duration == null || Duration == TimeSpan.Zero)
            {
                if ((DurationHours ?? 0) == 0 && (DurationMinutes ?? 0) == 0)
                {
                    yield return new ValidationResult(
                        "Either Duration must be provided or a positive combination of DurationHours and DurationMinutes is required.",
                        new[] { nameof(Duration), nameof(DurationHours), nameof(DurationMinutes) });
                }
            }
        }
    }

    public class VisitDetailVM
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string? VisitType { get; set; }
        public string? Location { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public string? Notes { get; set; }
        public string? Outcome { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }
        public TimeSpan? Duration { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class VisitListVM
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string StaffName { get; set; } = string.Empty;
        public DateTime VisitDate { get; set; }
        public string? VisitType { get; set; }
        public string? Location { get; set; }
        public string Purpose { get; set; } = string.Empty;
        public int? DurationHours { get; set; }
        public int? DurationMinutes { get; set; }
        public TimeSpan? Duration { get; set; }
        public string? Outcome { get; set; }
        public DateTime? NextVisitDate { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Contains ViewModels for visit analytics and reporting functionality.
    /// These ViewModels support comprehensive visit analysis including effectiveness metrics,
    /// customer patterns, staff performance, and territory reporting.
    /// </summary>
    public class VisitAnalyticsViewModels
    {
        /// <summary>
        /// ViewModel for analyzing visit effectiveness and conversion rates for staff members.
        /// Provides metrics on visit success, payment conversion, and overall effectiveness.
        /// </summary>
        public class VisitEffectivenessVM
        {
            /// <summary>
            /// Unique identifier of the staff member
            /// </summary>
            public Guid StaffId { get; set; }

            /// <summary>
            /// Total number of visits conducted by the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits must be non-negative")]
            public int TotalVisits { get; set; }

            /// <summary>
            /// Number of visits that resulted in payments
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Visits with payments must be non-negative")]
            public int VisitsWithPayments { get; set; }

            /// <summary>
            /// Total amount collected from visits
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount must be non-negative")]
            public decimal TotalAmountFromVisits { get; set; }

            /// <summary>
            /// Conversion rate of visits to payments (percentage)
            /// </summary>
            [Range(0, 100, ErrorMessage = "Conversion rate must be between 0 and 100")]
            [Display(Name = "Conversion Rate (%)")]
            public decimal ConversionRate { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing customer visit patterns and preferences.
        /// Helps identify customer behavior trends and optimal visit scheduling.
        /// </summary>
        public class CustomerVisitPatternVM
        {
            /// <summary>
            /// Unique identifier of the customer
            /// </summary>
            public Guid CustomerId { get; set; }

            /// <summary>
            /// Total number of visits for the customer
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits must be non-negative")]
            public int TotalVisits { get; set; }

            /// <summary>
            /// Average number of visits per month
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average visits per month must be non-negative")]
            [Display(Name = "Average Visits per Month")]
            public decimal AverageVisitsPerMonth { get; set; }

            /// <summary>
            /// Date of the most recent visit
            /// </summary>
            [Display(Name = "Last Visit Date")]
            public DateTime? LastVisitDate { get; set; }

            /// <summary>
            /// Visit frequency score indicating customer engagement level
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Visit frequency must be non-negative")]
            [Display(Name = "Visit Frequency Score")]
            public double VisitFrequency { get; set; }

            /// <summary>
            /// List of preferred days for visits
            /// </summary>
            [Display(Name = "Preferred Visit Days")]
            public List<string> PreferredVisitDays { get; set; } = new List<string>();
        }

        /// <summary>
        /// ViewModel for analyzing visit outcomes over a specified time period.
        /// Provides comprehensive breakdown of visit results and success metrics.
        /// </summary>
        public class VisitOutcomeAnalysisVM
        {
            /// <summary>
            /// Start date for the analysis period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the analysis period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total number of visits in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits must be non-negative")]
            public int TotalVisits { get; set; }

            /// <summary>
            /// Number of successfully completed visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Successful visits must be non-negative")]
            [Display(Name = "Successful Visits")]
            public int SuccessfulVisits { get; set; }

            /// <summary>
            /// Number of visits requiring follow-up actions
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Follow-up required visits must be non-negative")]
            [Display(Name = "Follow-up Required")]
            public int FollowUpRequired { get; set; }

            /// <summary>
            /// Number of no-show visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "No-show visits must be non-negative")]
            [Display(Name = "No-Show Visits")]
            public int NoShowVisits { get; set; }

            /// <summary>
            /// Detailed breakdown of visit outcomes by category
            /// </summary>
            [Display(Name = "Outcome Breakdown")]
            public Dictionary<string, int> OutcomeBreakdown { get; set; } = new Dictionary<string, int>();
        }

        /// <summary>
        /// ViewModel for analyzing staff visit performance and effectiveness.
        /// Tracks key performance indicators including completion rates and payment conversion.
        /// </summary>
        public class StaffVisitPerformanceVM
        {
            /// <summary>
            /// Unique identifier of the staff member
            /// </summary>
            public Guid StaffId { get; set; }

            /// <summary>
            /// Total number of visits assigned to the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits must be non-negative")]
            public int TotalVisits { get; set; }

            /// <summary>
            /// Number of successfully completed visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Completed visits must be non-negative")]
            [Display(Name = "Completed Visits")]
            public int CompletedVisits { get; set; }

            /// <summary>
            /// Number of cancelled visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Cancelled visits must be non-negative")]
            [Display(Name = "Cancelled Visits")]
            public int CancelledVisits { get; set; }

            /// <summary>
            /// Number of no-show visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "No-show visits must be non-negative")]
            [Display(Name = "No-Show Visits")]
            public int NoShowVisits { get; set; }

            /// <summary>
            /// Average duration of visits in hours
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average visit duration must be non-negative")]
            [Display(Name = "Average Visit Duration (Hours)")]
            public double AverageVisitDuration { get; set; }

            /// <summary>
            /// Number of visits that resulted in payments
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Visits with payments must be non-negative")]
            [Display(Name = "Visits with Payments")]
            public int VisitsWithPayments { get; set; }

            /// <summary>
            /// Total amount collected from visits
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount collected must be non-negative")]
            [Display(Name = "Total Amount Collected")]
            public decimal TotalAmountCollected { get; set; }

            /// <summary>
            /// Overall success rate of visits (percentage)
            /// </summary>
            [Range(0, 100, ErrorMessage = "Success rate must be between 0 and 100")]
            [Display(Name = "Success Rate (%)")]
            public decimal SuccessRate { get; set; }

            /// <summary>
            /// Rate of visits converting to payments (percentage)
            /// </summary>
            [Range(0, 100, ErrorMessage = "Payment conversion rate must be between 0 and 100")]
            [Display(Name = "Payment Conversion Rate (%)")]
            public decimal PaymentConversionRate { get; set; }
        }

        /// <summary>
        /// ViewModel for visit route planning and optimization.
        /// Supports efficient visit scheduling and travel time calculations.
        /// </summary>
        public class VisitRouteVM
        {
            /// <summary>
            /// Order of the visit in the route sequence
            /// </summary>
            [Range(1, int.MaxValue, ErrorMessage = "Order must be at least 1")]
            public int Order { get; set; }

            /// <summary>
            /// Unique identifier of the customer
            /// </summary>
            public Guid CustomerId { get; set; }

            /// <summary>
            /// Name of the customer
            /// </summary>
            [Required(ErrorMessage = "Customer name is required")]
            [Display(Name = "Customer Name")]
            public string CustomerName { get; set; } = string.Empty;

            /// <summary>
            /// Customer address for visit location
            /// </summary>
            [Display(Name = "Address")]
            public string? Address { get; set; }

            /// <summary>
            /// Estimated duration of the visit
            /// </summary>
            [Display(Name = "Estimated Visit Duration")]
            public TimeSpan EstimatedDuration { get; set; }

            /// <summary>
            /// Estimated travel time to reach the customer
            /// </summary>
            [Display(Name = "Estimated Travel Time")]
            public TimeSpan EstimatedTravelTime { get; set; }
        }

        /// <summary>
        /// ViewModel for staff scheduling and activity management.
        /// Tracks staff activities, time allocations, and schedule status.
        /// </summary>
        public class StaffScheduleVM
        {
            /// <summary>
            /// Type of activity (visit, follow-up, meeting, etc.)
            /// </summary>
            [Required(ErrorMessage = "Activity type is required")]
            [Display(Name = "Activity Type")]
            public StaffActivityType ActivityType { get; set; }

            /// <summary>
            /// Unique identifier of the activity
            /// </summary>
            public Guid ActivityId { get; set; }

            /// <summary>
            /// Description of the activity
            /// </summary>
            [Required(ErrorMessage = "Activity description is required")]
            [Display(Name = "Description")]
            public string Description { get; set; } = string.Empty;

            /// <summary>
            /// Scheduled start time of the activity
            /// </summary>
            [Required(ErrorMessage = "Start time is required")]
            [Display(Name = "Start Time")]
            public DateTime StartTime { get; set; }

            /// <summary>
            /// Scheduled end time of the activity
            /// </summary>
            [Required(ErrorMessage = "End time is required")]
            [Display(Name = "End Time")]
            public DateTime EndTime { get; set; }

            /// <summary>
            /// Location where the activity will take place
            /// </summary>
            [Display(Name = "Location")]
            public string? Location { get; set; }

            /// <summary>
            /// Current status of the scheduled activity
            /// </summary>
            [Required(ErrorMessage = "Status is required")]
            [Display(Name = "Status")]
            public string Status { get; set; } = string.Empty;
        }

        /// <summary>
        /// ViewModel for identifying and resolving visit scheduling conflicts.
        /// Helps prevent double-booking and optimize staff schedules.
        /// </summary>
        public class VisitConflictVM
        {
            /// <summary>
            /// Unique identifier of the existing conflicting visit
            /// </summary>
            [Display(Name = "Existing Visit ID")]
            public Guid ExistingVisitId { get; set; }

            /// <summary>
            /// Scheduled time of the existing visit
            /// </summary>
            [Display(Name = "Existing Visit Time")]
            public DateTime ExistingVisitTime { get; set; }

            /// <summary>
            /// Duration of the existing visit
            /// </summary>
            [Display(Name = "Existing Visit Duration")]
            public TimeSpan ExistingVisitDuration { get; set; }

            /// <summary>
            /// Proposed time for the new visit
            /// </summary>
            [Display(Name = "Proposed Visit Time")]
            public DateTime ProposedVisitTime { get; set; }

            /// <summary>
            /// Duration of the proposed visit
            /// </summary>
            [Display(Name = "Proposed Visit Duration")]
            public TimeSpan ProposedVisitDuration { get; set; }

            /// <summary>
            /// Type of conflict (overlap, travel time, etc.)
            /// </summary>
            [Required(ErrorMessage = "Conflict type is required")]
            [Display(Name = "Conflict Type")]
            public string ConflictType { get; set; } = string.Empty;
        }

        /// <summary>
        /// ViewModel for comprehensive visit summary reporting.
        /// Provides aggregated metrics and insights for management reporting.
        /// </summary>
        public class VisitSummaryReportVM
        {
            /// <summary>
            /// Start date for the reporting period
            /// </summary>
            [Required(ErrorMessage = "From date is required")]
            [Display(Name = "From Date")]
            public DateTime FromDate { get; set; }

            /// <summary>
            /// End date for the reporting period
            /// </summary>
            [Required(ErrorMessage = "To date is required")]
            [Display(Name = "To Date")]
            public DateTime ToDate { get; set; }

            /// <summary>
            /// Total number of visits in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits must be non-negative")]
            public int TotalVisits { get; set; }

            /// <summary>
            /// Number of successfully completed visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Completed visits must be non-negative")]
            [Display(Name = "Completed Visits")]
            public int CompletedVisits { get; set; }

            /// <summary>
            /// Number of cancelled visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Cancelled visits must be non-negative")]
            [Display(Name = "Cancelled Visits")]
            public int CancelledVisits { get; set; }

            /// <summary>
            /// Number of no-show visits
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "No-show visits must be non-negative")]
            [Display(Name = "No-Show Visits")]
            public int NoShowVisits { get; set; }

            /// <summary>
            /// Total hours spent on visits
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total visit hours must be non-negative")]
            [Display(Name = "Total Visit Hours")]
            public decimal TotalVisitHours { get; set; }

            /// <summary>
            /// Average duration of visits in hours
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average visit duration must be non-negative")]
            [Display(Name = "Average Visit Duration (Hours)")]
            public double AverageVisitDuration { get; set; }

            /// <summary>
            /// Breakdown of visits by type or category
            /// </summary>
            [Display(Name = "Visits by Type")]
            public Dictionary<string, int> VisitsByType { get; set; } = new Dictionary<string, int>();
        }

        /// <summary>
        /// ViewModel for customer engagement analysis and reporting.
        /// Tracks customer interaction patterns and payment history.
        /// </summary>
        public class CustomerEngagementReportVM
        {
            /// <summary>
            /// Unique identifier of the customer
            /// </summary>
            public Guid CustomerId { get; set; }

            /// <summary>
            /// Total number of visits for the customer
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits must be non-negative")]
            public int TotalVisits { get; set; }

            /// <summary>
            /// Total number of payments made by the customer
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total payments must be non-negative")]
            [Display(Name = "Total Payments")]
            public int TotalPayments { get; set; }

            /// <summary>
            /// Total amount paid by the customer
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Total amount paid must be non-negative")]
            [Display(Name = "Total Amount Paid")]
            public decimal TotalAmountPaid { get; set; }

            /// <summary>
            /// Date of the most recent visit
            /// </summary>
            [Display(Name = "Last Visit Date")]
            public DateTime? LastVisitDate { get; set; }

            /// <summary>
            /// Date of the most recent payment
            /// </summary>
            [Display(Name = "Last Payment Date")]
            public DateTime? LastPaymentDate { get; set; }

            /// <summary>
            /// Visit frequency score indicating customer engagement level
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Visit frequency must be non-negative")]
            [Display(Name = "Visit Frequency Score")]
            public double VisitFrequency { get; set; }
        }

        /// <summary>
        /// ViewModel for territory-based visit analysis and reporting.
        /// Provides insights into visit distribution and performance by geographic areas.
        /// </summary>
        public class TerritoryVisitReportVM
        {
            /// <summary>
            /// Territory or geographic area name
            /// </summary>
            [Required(ErrorMessage = "Territory is required")]
            [Display(Name = "Territory")]
            public string Territory { get; set; } = string.Empty;

            /// <summary>
            /// Total number of visits in the territory
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total visits must be non-negative")]
            public int TotalVisits { get; set; }

            /// <summary>
            /// Number of completed visits in the territory
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Completed visits must be non-negative")]
            [Display(Name = "Completed Visits")]
            public int CompletedVisits { get; set; }

            /// <summary>
            /// Number of pending visits in the territory
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Pending visits must be non-negative")]
            [Display(Name = "Pending Visits")]
            public int PendingVisits { get; set; }

            /// <summary>
            /// Number of cancelled visits in the territory
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Cancelled visits must be non-negative")]
            [Display(Name = "Cancelled Visits")]
            public int CancelledVisits { get; set; }

            /// <summary>
            /// Breakdown of visits by staff member in the territory
            /// </summary>
            [Display(Name = "Visits by Staff")]
            public Dictionary<Guid, int> VisitsByStaff { get; set; } = new Dictionary<Guid, int>();

            /// <summary>
            /// Average number of visits per day in the territory
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average visits per day must be non-negative")]
            [Display(Name = "Average Visits per Day")]
            public decimal AverageVisitsPerDay { get; set; }
        }
    }
}


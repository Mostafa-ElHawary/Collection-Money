using System;
using System.ComponentModel.DataAnnotations;
using System.Collections.Generic;

namespace CollectionApp.Application.ViewModels
{
    public class FollowUpCreateVM
    {
        [Required]
        public Guid CustomerId { get; set; }

        [Required]
        public Guid StaffId { get; set; }

        public Guid? AssignedToStaffId { get; set; }

        [Required]
        public DateTime ScheduledDate { get; set; }

        [Required]
        [MaxLength(50)]
        public string Type { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Priority { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string Status { get; set; } = string.Empty;

        public Guid? ContractId { get; set; }

        /// <summary>
        /// May not be persisted/returned until domain/mapping support is added
        /// </summary>
        [Required]
        [MaxLength(500)]
        public string Description { get; set; } = string.Empty;

        [MaxLength(2000)]
        public string? Notes { get; set; }
    }

    public class FollowUpUpdateVM
    {
        [Required]
        public Guid Id { get; set; }

        public Guid? AssignedToStaffId { get; set; }

        public DateTime? ScheduledDate { get; set; }

        [MaxLength(50)]
        public string? Type { get; set; }

        [MaxLength(50)]
        public string? Priority { get; set; }

        [MaxLength(50)]
        public string? Status { get; set; }

        /// <summary>
        /// May not be persisted/returned until domain/mapping support is added
        /// </summary>
        [MaxLength(500)]
        public string? Description { get; set; }

        [MaxLength(2000)]
        public string? Notes { get; set; }

        [MaxLength(1000)]
        public string? Outcome { get; set; }

        public DateTime? ActualDate { get; set; }
    }

    public class FollowUpDetailVM
    {
        public Guid Id { get; set; }
        public Guid CustomerId { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public Guid StaffId { get; set; }
        public string StaffName { get; set; } = string.Empty;
        /// <summary>
        /// May not be populated until mapping/domain updates are implemented
        /// </summary>
        public Guid? AssignedToStaffId { get; set; }
        /// <summary>
        /// May not be populated until mapping/domain updates are implemented
        /// </summary>
        public string AssignedStaffName { get; set; } = string.Empty;
        public Guid? ContractId { get; set; }
        public string? ContractNumber { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? Type { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        /// <summary>
        /// May not be populated until mapping/domain updates are implemented
        /// </summary>
        public string? Description { get; set; }
        public string? Notes { get; set; }
        public string? Outcome { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
    }

    public class FollowUpListVM
    {
        public Guid Id { get; set; }
        public string CustomerName { get; set; } = string.Empty;
        public string? ContractNumber { get; set; }
        public string StaffName { get; set; } = string.Empty;
        public Guid? AssignedToStaffId { get; set; }
        public DateTime ScheduledDate { get; set; }
        public DateTime? ActualDate { get; set; }
        public string? Type { get; set; }
        public string? Priority { get; set; }
        public string? Status { get; set; }
        public string? Description { get; set; }
        public DateTime CreatedAt { get; set; }
    }

    /// <summary>
    /// Contains ViewModels for follow-up analytics and performance reporting functionality.
    /// These ViewModels support comprehensive follow-up analysis including effectiveness metrics,
    /// performance tracking, customer patterns, and staff workload management.
    /// </summary>
    public class FollowUpAnalyticsViewModels
    {
        /// <summary>
        /// ViewModel for analyzing follow-up effectiveness and completion rates for staff members.
        /// Provides metrics on follow-up success, completion rates, and performance indicators.
        /// </summary>
        public class FollowUpEffectivenessVM
        {
            /// <summary>
            /// Unique identifier of the staff member
            /// </summary>
            public Guid StaffId { get; set; }

            /// <summary>
            /// Total number of follow-ups assigned to the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total follow-ups must be non-negative")]
            [Display(Name = "Total Follow-ups")]
            public int TotalFollowUps { get; set; }

            /// <summary>
            /// Number of follow-ups successfully completed
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Completed follow-ups must be non-negative")]
            [Display(Name = "Completed Follow-ups")]
            public int CompletedFollowUps { get; set; }

            /// <summary>
            /// Number of follow-ups still pending completion
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
            /// Completion rate of follow-ups (percentage)
            /// </summary>
            [Range(0, 100, ErrorMessage = "Completion rate must be between 0 and 100")]
            [Display(Name = "Completion Rate (%)")]
            public decimal CompletionRate { get; set; }

            /// <summary>
            /// Average time to complete follow-ups in days
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average completion time must be non-negative")]
            [Display(Name = "Average Completion Time (Days)")]
            public double AverageCompletionTime { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing follow-up performance over a specified time period.
        /// Provides comprehensive breakdown of follow-up metrics and trends.
        /// </summary>
        public class FollowUpPerformanceVM
        {
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
            /// Total number of follow-ups in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total follow-ups must be non-negative")]
            [Display(Name = "Total Follow-ups")]
            public int TotalFollowUps { get; set; }

            /// <summary>
            /// Number of completed follow-ups in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Completed follow-ups must be non-negative")]
            [Display(Name = "Completed Follow-ups")]
            public int CompletedFollowUps { get; set; }

            /// <summary>
            /// Number of pending follow-ups in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Pending follow-ups must be non-negative")]
            [Display(Name = "Pending Follow-ups")]
            public int PendingFollowUps { get; set; }

            /// <summary>
            /// Number of overdue follow-ups in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Overdue follow-ups must be non-negative")]
            [Display(Name = "Overdue Follow-ups")]
            public int OverdueFollowUps { get; set; }

            /// <summary>
            /// Number of high priority follow-ups in the period
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "High priority follow-ups must be non-negative")]
            [Display(Name = "High Priority Follow-ups")]
            public int HighPriorityFollowUps { get; set; }

            /// <summary>
            /// Breakdown of follow-ups by type
            /// </summary>
            [Display(Name = "Follow-ups by Type")]
            public Dictionary<string, int> FollowUpsByType { get; set; } = new Dictionary<string, int>();

            /// <summary>
            /// Breakdown of follow-ups by priority level
            /// </summary>
            [Display(Name = "Follow-ups by Priority")]
            public Dictionary<string, int> FollowUpsByPriority { get; set; } = new Dictionary<string, int>();
        }

        /// <summary>
        /// ViewModel for analyzing customer follow-up patterns and engagement.
        /// Helps identify customer behavior trends and optimal follow-up scheduling.
        /// </summary>
        public class CustomerFollowUpPatternVM
        {
            /// <summary>
            /// Unique identifier of the customer
            /// </summary>
            public Guid CustomerId { get; set; }

            /// <summary>
            /// Total number of follow-ups for the customer
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total follow-ups must be non-negative")]
            [Display(Name = "Total Follow-ups")]
            public int TotalFollowUps { get; set; }

            /// <summary>
            /// Number of completed follow-ups for the customer
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Completed follow-ups must be non-negative")]
            [Display(Name = "Completed Follow-ups")]
            public int CompletedFollowUps { get; set; }

            /// <summary>
            /// Number of pending follow-ups for the customer
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Pending follow-ups must be non-negative")]
            [Display(Name = "Pending Follow-ups")]
            public int PendingFollowUps { get; set; }

            /// <summary>
            /// Average number of follow-ups per month for the customer
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Average follow-ups per month must be non-negative")]
            [Display(Name = "Average Follow-ups per Month")]
            public decimal AverageFollowUpsPerMonth { get; set; }

            /// <summary>
            /// Date of the most recent follow-up
            /// </summary>
            [Display(Name = "Last Follow-up Date")]
            public DateTime? LastFollowUpDate { get; set; }

            /// <summary>
            /// Follow-up frequency score indicating customer engagement level
            /// </summary>
            [Range(typeof(decimal), "0", "79228162514264337593543950335", ErrorMessage = "Follow-up frequency must be non-negative")]
            [Display(Name = "Follow-up Frequency Score")]
            public double FollowUpFrequency { get; set; }
        }

        /// <summary>
        /// ViewModel for analyzing staff workload related to follow-up assignments.
        /// Provides insights into staff capacity and follow-up task distribution.
        /// </summary>
        public class StaffWorkloadVM
        {
            /// <summary>
            /// Unique identifier of the staff member
            /// </summary>
            public Guid StaffId { get; set; }

            /// <summary>
            /// Total number of follow-ups assigned to the staff member
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Total follow-ups must be non-negative")]
            [Display(Name = "Total Follow-ups")]
            public int TotalFollowUps { get; set; }

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
            /// Number of high priority follow-ups assigned
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "High priority follow-ups must be non-negative")]
            [Display(Name = "High Priority Follow-ups")]
            public int HighPriorityFollowUps { get; set; }

            /// <summary>
            /// Number of follow-ups scheduled for today
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "Today's follow-ups must be non-negative")]
            [Display(Name = "Today's Follow-ups")]
            public int TodayFollowUps { get; set; }

            /// <summary>
            /// Number of follow-ups scheduled for this week
            /// </summary>
            [Range(0, int.MaxValue, ErrorMessage = "This week's follow-ups must be non-negative")]
            [Display(Name = "This Week's Follow-ups")]
            public int ThisWeekFollowUps { get; set; }
        }
    }
}


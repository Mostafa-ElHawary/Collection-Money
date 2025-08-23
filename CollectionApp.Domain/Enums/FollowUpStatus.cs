namespace CollectionApp.Domain.Enums
{
    /// <summary>
    /// Represents the status of a follow-up.
    /// </summary>
    public enum FollowUpStatus
    {
        /// <summary>
        /// Follow-up is pending and not yet started.
        /// </summary>
        Pending = 0,

        /// <summary>
        /// Follow-up is currently in progress.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Follow-up has been completed successfully.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Follow-up was cancelled.
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Follow-up is overdue.
        /// </summary>
        Overdue = 4,

        /// <summary>
        /// Follow-up was rescheduled.
        /// </summary>
        Rescheduled = 5
    }
} 
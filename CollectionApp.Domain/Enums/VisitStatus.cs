namespace CollectionApp.Domain.Enums
{
    /// <summary>
    /// Represents the status of a visit.
    /// </summary>
    public enum VisitStatus
    {
        /// <summary>
        /// Visit is scheduled but not yet started.
        /// </summary>
        Scheduled = 0,

        /// <summary>
        /// Visit is currently in progress.
        /// </summary>
        InProgress = 1,

        /// <summary>
        /// Visit has been completed successfully.
        /// </summary>
        Completed = 2,

        /// <summary>
        /// Visit was cancelled.
        /// </summary>
        Cancelled = 3,

        /// <summary>
        /// Customer did not show up for the visit.
        /// </summary>
        NoShow = 4,

        /// <summary>
        /// Visit was rescheduled.
        /// </summary>
        Rescheduled = 5
    }
} 
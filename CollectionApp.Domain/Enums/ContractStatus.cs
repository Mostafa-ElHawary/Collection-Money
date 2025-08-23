namespace CollectionApp.Domain.Enums
{
    /// <summary>
    /// Represents the lifecycle state of a contract.
    /// </summary>
    public enum ContractStatus
    {
        /// <summary>
        /// Initial state. Not yet active; can be edited or cancelled.
        /// </summary>
        Draft = 0,

        /// <summary>
        /// Contract is active and obligations are in effect.
        /// </summary>
        Active = 1,

        /// <summary>
        /// Temporarily paused; accruals may be suspended.
        /// </summary>
        Suspended = 2,

        /// <summary>
        /// All obligations fulfilled; contract closed successfully.
        /// </summary>
        Completed = 3,

        /// <summary>
        /// Terminated before completion by either party.
        /// </summary>
        Cancelled = 4,

        /// <summary>
        /// Breach of obligations; contract in default.
        /// </summary>
        Defaulted = 5
    }
}


namespace CollectionApp.Domain.Enums
{
    /// <summary>
    /// Payment state of a single installment.
    /// </summary>
    public enum InstallmentStatus
    {
        /// <summary>Not yet due or unpaid.</summary>
        Pending = 0,
        /// <summary>Paid in full.</summary>
        Paid = 1,
        /// <summary>Partially paid.</summary>
        PartiallyPaid = 2,
        /// <summary>Due date passed and not fully paid.</summary>
        Overdue = 3,
        /// <summary>Waived by agreement.</summary>
        Waived = 4,
        /// <summary>Cancelled and no longer collectible.</summary>
        Cancelled = 5
    }
}


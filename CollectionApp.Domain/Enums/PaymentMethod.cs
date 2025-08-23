namespace CollectionApp.Domain.Enums
{
    /// <summary>
    /// Supported payment methods for receiving funds.
    /// </summary>
    public enum PaymentMethod
    {
        /// <summary>Cash payments made in person.</summary>
        Cash = 0,
        /// <summary>Paper check deposited to bank account.</summary>
        Check = 1,
        /// <summary>Electronic bank-to-bank transfer.</summary>
        BankTransfer = 2,
        /// <summary>Credit card payment via processor.</summary>
        CreditCard = 3,
        /// <summary>Debit card payment via processor.</summary>
        DebitCard = 4,
        /// <summary>Online payment channel (e.g., gateway).</summary>
        OnlinePayment = 5,
        /// <summary>Mobile wallet payment.</summary>
        MobilePayment = 6
    }
}


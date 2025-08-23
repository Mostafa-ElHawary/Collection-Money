using System;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Domain.Entities
{
    public class Receipt : BaseEntity
    {
        // Required by EF Core
        private Receipt() { }
        public string ReceiptNumber { get; private set; }
        public Guid PaymentId { get; private set; }
        public Guid CustomerId { get; private set; }
        public Money Amount { get; private set; }
        public DateTime IssueDate { get; private set; }
        public string? Description { get; private set; }
        public Guid StaffId { get; private set; }

        public Payment? Payment { get; private set; }
        public Customer? Customer { get; private set; }
        public Staff? Staff { get; private set; }

        public Receipt(string receiptNumber, Guid paymentId, Guid customerId, Money amount, DateTime issueDate, string? description, Guid staffId)
        {
            ReceiptNumber = ValidateReceiptNumber(receiptNumber);
            PaymentId = paymentId != Guid.Empty ? paymentId : throw new ArgumentException("PaymentId required", nameof(paymentId));
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException("CustomerId required", nameof(customerId));
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            IssueDate = issueDate;
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            StaffId = staffId != Guid.Empty ? staffId : throw new ArgumentException("StaffId required", nameof(staffId));
        }

        private static string ValidateReceiptNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("ReceiptNumber required", nameof(value));
            return value.Trim().ToUpperInvariant();
        }

        public void UpdateDescription(string? description)
        {
            Description = string.IsNullOrWhiteSpace(description) ? null : description.Trim();
            Touch();
        }
    }
}


using System;
using System.Collections.Generic;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Domain.Entities
{
    public class Installment : BaseEntity
    {
        // Required by EF Core
        private Installment() { }
        public Guid ContractId { get; private set; }
        public int InstallmentNumber { get; private set; }
        public DateTime DueDate { get; set; }
        public Money Amount { get; private set; }
        public InstallmentStatus Status { get; private set; } = InstallmentStatus.Pending;
        public Money PaidAmount { get; private set; }
        public DateTime? PaidDate { get; private set; }
        public string? Notes { get; private set; }
        
        // Property to get outstanding amount
        public Money OutstandingAmount => new Money(Math.Max(0, Amount.Amount - PaidAmount.Amount), Amount.Currency);

        public Contract? Contract { get; private set; }

        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        public Installment(Guid contractId, int installmentNumber, DateTime dueDate, Money amount, string? notes = null)
        {
            ContractId = contractId != Guid.Empty ? contractId : throw new ArgumentException("ContractId required", nameof(contractId));
            if (installmentNumber <= 0) throw new ArgumentOutOfRangeException(nameof(installmentNumber));
            InstallmentNumber = installmentNumber;
            DueDate = dueDate.Date;
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            PaidAmount = Money.Zero(amount.Currency);
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }

        public void AllocatePayment(Payment payment)
        {
            if (payment is null) throw new ArgumentNullException(nameof(payment));
            if (payment.InstallmentId != Id) throw new InvalidOperationException("Payment installment mismatch.");
            if (payment.ContractId != ContractId) throw new InvalidOperationException("Payment contract mismatch.");

            _payments.Add(payment);

            PaidAmount = PaidAmount + payment.Amount; // uses Money operator, enforces currency match

            if (PaidAmount >= Amount)
            {
                Status = InstallmentStatus.Paid;
                PaidDate = payment.PaymentDate.Date;
            }
            else if (PaidAmount.Amount > 0)
            {
                Status = InstallmentStatus.PartiallyPaid;
            }
            Touch();
        }

        public bool IsOverdue(DateTime asOfUtc)
        {
            return Status is InstallmentStatus.Pending or InstallmentStatus.PartiallyPaid && DueDate.Date < asOfUtc.Date;
        }

        public void UpdateNotes(string? notes)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        public void MarkAsPaid(DateTime paidDate)
        {
            Status = InstallmentStatus.Paid;
            PaidDate = paidDate.Date;
            Touch();
        }
        
        public void MarkAsPaid(Guid paymentId)
        {
            Status = InstallmentStatus.Paid;
            PaidDate = DateTime.UtcNow.Date;
            Touch();
        }
        
        public void UpdateDetails(Money amount, DateTime dueDate, string? notes)
        {
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            DueDate = dueDate.Date;
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }
        
        public void UpdateStatus(InstallmentStatus status)
        {
            Status = status;
            Touch();
        }
        
        public void UpdatePartialPayment(decimal amount)
        {
            // Update the status to PartiallyPaid if there's a payment but not fully paid
            if (amount > 0 && PaidAmount.Amount + amount < Amount.Amount)
            {
                Status = InstallmentStatus.PartiallyPaid;
            }
            else if (PaidAmount.Amount + amount >= Amount.Amount)
            {
                Status = InstallmentStatus.Paid;
                PaidDate = DateTime.UtcNow.Date;
            }
            
            // Update the paid amount
            PaidAmount = new Money(PaidAmount.Amount + amount, Amount.Currency);
            Touch();
        }
        
        public void UpdatePartialPayment(Money paymentAmount)
        {
            // Update the status to PartiallyPaid if there's a payment but not fully paid
            if (paymentAmount.Amount > 0 && PaidAmount + paymentAmount < Amount)
            {
                Status = InstallmentStatus.PartiallyPaid;
            }
            else if (PaidAmount + paymentAmount >= Amount)
            {
                Status = InstallmentStatus.Paid;
                PaidDate = DateTime.UtcNow.Date;
            }
            
            // Update the paid amount
            PaidAmount = PaidAmount + paymentAmount; // Uses Money operator
            Touch();
        }
        
        public void UpdateDueDate(DateTime dueDate)
        {
            DueDate = dueDate.Date;
            Touch();
        }
        
        public void RevertPayment()
        {
            // Reset the status based on the paid amount
            if (PaidAmount.Amount <= 0)
            {
                Status = InstallmentStatus.Pending;
                PaidDate = null;
            }
            else if (PaidAmount.Amount < Amount.Amount)
            {
                Status = InstallmentStatus.PartiallyPaid;
            }
            Touch();
        }
        
        public decimal RemainingAmount => Amount.Amount - PaidAmount.Amount;
        
        public void Waive(string? reason = null)
        {
            Status = InstallmentStatus.Waived;
            if (!string.IsNullOrWhiteSpace(reason))
            {
                Notes = reason.Trim();
            }
            Touch();
        }
        
        // Computed properties for export
        public DateTime? PaymentDate => PaidDate;
        
        // This property was causing a duplicate definition error
        // public bool IsOverdue => IsOverdue(DateTime.UtcNow);
        
        public int OverdueDays => IsOverdue(DateTime.UtcNow) ? (int)(DateTime.UtcNow.Date - DueDate.Date).TotalDays : 0;
        
        // Property is defined as a decimal above
    }
}


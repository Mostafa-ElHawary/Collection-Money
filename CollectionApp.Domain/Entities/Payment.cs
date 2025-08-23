using System;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Domain.Entities
{
    public class Payment : BaseEntity
    {
        // Required by EF Core
        private Payment() { }
        public Guid ContractId { get; private set; }
        public Guid InstallmentId { get; private set; }
        public Money Amount { get; private set; }
        public DateTime PaymentDate { get; private set; }
        public PaymentMethod PaymentMethod { get; private set; }
        public string? ReferenceNumber { get; private set; }
        public string? Notes { get; private set; }
        public Guid? StaffId { get; private set; }

        // New property for expanded functionality
        public Guid? ProcessedByStaffId { get; private set; }

        // Flag to identify reversal payments
        public bool IsReversal { get; private set; }

        public Contract? Contract { get; private set; }
        public Installment? Installment { get; private set; }
        public Staff? Staff { get; private set; }
        public Receipt? Receipt { get; private set; }

        // Original constructor for backward compatibility
        public Payment(Guid contractId, Guid installmentId, Money amount, DateTime paymentDate, PaymentMethod paymentMethod, string? referenceNumber = null, string? notes = null, Guid? staffId = null)
        {
            ContractId = contractId != Guid.Empty ? contractId : throw new ArgumentException("ContractId required", nameof(contractId));
            InstallmentId = installmentId != Guid.Empty ? installmentId : throw new ArgumentException("InstallmentId required", nameof(installmentId));
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            if (amount.Amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be positive.");
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
            ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim();
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            StaffId = staffId;
            ProcessedByStaffId = staffId; // Set both for backward compatibility
        }

        // New overloaded constructor for expanded functionality
        public Payment(Guid contractId, Guid installmentId, Money amount, DateTime paymentDate, PaymentMethod paymentMethod, Guid? processedByStaffId, string? referenceNumber = null, string? notes = null)
        {
            ContractId = contractId != Guid.Empty ? contractId : throw new ArgumentException("ContractId required", nameof(contractId));
            InstallmentId = installmentId != Guid.Empty ? installmentId : throw new ArgumentException("InstallmentId required", nameof(installmentId));
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            if (amount.Amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be positive.");
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
            ProcessedByStaffId = processedByStaffId;
            StaffId = processedByStaffId; // Maintain backward compatibility
            ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim();
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            IsReversal = false; // Default to regular payment
        }

        // Constructor for reversal payments
        public Payment(Guid contractId, Guid installmentId, Money amount, DateTime paymentDate, PaymentMethod paymentMethod, Guid? processedByStaffId, bool isReversal, string? referenceNumber = null, string? notes = null)
        {
            ContractId = contractId != Guid.Empty ? contractId : throw new ArgumentException("ContractId required", nameof(contractId));
            InstallmentId = installmentId != Guid.Empty ? installmentId : throw new ArgumentException("InstallmentId required", nameof(installmentId));
            Amount = amount ?? throw new ArgumentNullException(nameof(amount));
            if (amount.Amount <= 0m) throw new ArgumentOutOfRangeException(nameof(amount), "Payment amount must be positive.");
            PaymentDate = paymentDate;
            PaymentMethod = paymentMethod;
            ProcessedByStaffId = processedByStaffId;
            StaffId = processedByStaffId; // Maintain backward compatibility
            ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim();
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            IsReversal = isReversal;
        }

        public Receipt GenerateReceipt(string receiptNumber, Guid customerId, Guid staffId)
        {
            if (Receipt is not null) throw new InvalidOperationException("Receipt already generated.");
            var receipt = new Receipt(receiptNumber, Id, customerId, Amount, DateTime.UtcNow, "Payment receipt", staffId);
            Receipt = receipt;
            Touch();
            return receipt;
        }

        public void UpdateDetails(string? referenceNumber, string? notes)
        {
            ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim();
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        // New methods for expanded functionality
        public void UpdateReferenceNumber(string? referenceNumber)
        {
            ReferenceNumber = string.IsNullOrWhiteSpace(referenceNumber) ? null : referenceNumber.Trim();
            Touch();
        }

        public void UpdateNotes(string? notes)
        {
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }
    }
}


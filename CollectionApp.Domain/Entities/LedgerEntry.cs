using System;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Domain.Entities
{
    public class LedgerEntry : BaseEntity
    {
        // Required by EF Core
        private LedgerEntry() { }
        private Money amount;
        private string entryType;
        private bool isArchived;

        public Guid? ContractId { get; private set; }
        public Guid? CustomerId { get; private set; }
        public DateTime TransactionDate { get; private set; }
        public string Description { get; set; }
        public Money DebitAmount { get; private set; }
        public Money CreditAmount { get; private set; }
        public Money Balance { get; private set; }
        public string ReferenceType { get; private set; }
        public Guid? ReferenceId { get; private set; }
        public Guid? StaffId { get; private set; }
        public bool IsArchived { get => isArchived; private set => isArchived = value; }

        public Contract? Contract { get; private set; }
        public Customer? Customer { get; private set; }
        public Staff? Staff { get; private set; }

        public LedgerEntry(
            DateTime transactionDate,
            string description,
            Money debitAmount,
            Money creditAmount,
            Money balance,
            string referenceType,
            Guid? referenceId = null,
            Guid? contractId = null,
            Guid? customerId = null,
            Guid? staffId = null)
        {
            if (debitAmount is null) throw new ArgumentNullException(nameof(debitAmount));
            if (creditAmount is null) throw new ArgumentNullException(nameof(creditAmount));
            if (balance is null) throw new ArgumentNullException(nameof(balance));
            if (debitAmount.Currency != creditAmount.Currency || debitAmount.Currency != balance.Currency)
            {
                throw new InvalidOperationException("All money values must have the same currency.");
            }
            if (debitAmount.Amount > 0m && creditAmount.Amount > 0m)
            {
                throw new InvalidOperationException("Cannot have both debit and credit positive in the same entry.");
            }

            TransactionDate = transactionDate;
            Description = string.IsNullOrWhiteSpace(description) ? throw new ArgumentException("Description required", nameof(description)) : description.Trim();
            DebitAmount = debitAmount;
            CreditAmount = creditAmount;
            Balance = balance;
            ReferenceType = string.IsNullOrWhiteSpace(referenceType) ? throw new ArgumentException("ReferenceType required", nameof(referenceType)) : referenceType.Trim();
            ReferenceId = referenceId;
            ContractId = contractId;
            CustomerId = customerId;
            StaffId = staffId;
        }

        public LedgerEntry(Money amount, string entryType, string description, string? referenceType, Guid? referenceId)
        {
            this.amount = amount;
            this.entryType = entryType;
            Description = description;
            ReferenceType = referenceType;
            ReferenceId = referenceId;
        }
        
        /// <summary>
        /// Archives this ledger entry, marking it as no longer active but preserved for historical records
        /// </summary>
        public void Archive()
        {
            IsArchived = true;
        }
    }
}


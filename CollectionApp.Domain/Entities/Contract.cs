using System;
using System.Collections.Generic;
using System.Linq;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Domain.Entities
{
    public class Contract : BaseEntity
    {
        // Required by EF Core
        private Contract() { }
        
        // Property to get outstanding amount
        public Money RemainingBalance => OutstandingAmount();
        public string ContractNumber { get; private set; }
        public Guid CustomerId { get; private set; }
        public Money TotalAmount { get; private set; }
        public DateTime StartDate { get; private set; }
        public DateTime? EndDate { get; private set; }
        public ContractStatus Status { get; private set; } = ContractStatus.Draft;
        public decimal InterestRate { get; private set; }
        public int NumberOfInstallments { get; private set; }

        // New properties for expanded functionality
        public string? ContractType { get; private set; }
        public Money? PrincipalAmount { get; private set; }
        public int? TermInMonths { get; private set; }
        public string? PaymentFrequency { get; private set; }
        public int? GracePeriodDays { get; private set; }
        public decimal? LateFeePercentage { get; private set; }
        public decimal? PenaltyPercentage { get; private set; }
        public string? CollateralDescription { get; private set; }
        public string? GuarantorName { get; private set; }
        public string? GuarantorContact { get; private set; }
        public string? Notes { get; private set; }
        public Guid? StaffId { get; private set; }
        public string? Purpose { get; private set; }
        public string? SourceOfFunds { get; private set; }

        public Customer? Customer { get; private set; }
        public Staff? Staff { get; private set; }

        private readonly List<Installment> _installments = new();
        public IReadOnlyCollection<Installment> Installments => _installments.AsReadOnly();

        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        private readonly List<FollowUp> _followUps = new();
        public IReadOnlyCollection<FollowUp> FollowUps => _followUps.AsReadOnly();

        private readonly List<Visit> _visits = new();
        public IReadOnlyCollection<Visit> Visits => _visits.AsReadOnly();

        // Original constructor for backward compatibility
        public Contract(
            string contractNumber,
            Guid customerId,
            Money totalAmount,
            DateTime startDate,
            decimal interestRate,
            int numberOfInstallments,
            Guid? staffId = null)
        {
            ContractNumber = ValidateContractNumber(contractNumber);
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException("CustomerId required", nameof(customerId));
            TotalAmount = totalAmount ?? throw new ArgumentNullException(nameof(totalAmount));
            StartDate = startDate.Date;
            InterestRate = ValidateInterestRate(interestRate);
            NumberOfInstallments = ValidateInstallments(numberOfInstallments);
            StaffId = ValidateStaffId(staffId);
        }

        // New overloaded constructor for expanded functionality
        public Contract(
            string contractNumber,
            Guid customerId,
            string? contractType,
            Money? principalAmount,
            Money? interestRate,
            int? termInMonths,
            DateTime startDate,
            DateTime? endDate,
            string? paymentFrequency,
            int? gracePeriodDays,
            decimal? lateFeePercentage,
            decimal? penaltyPercentage,
            string? collateralDescription,
            string? guarantorName,
            string? guarantorContact,
            string? notes,
            Guid? staffId = null,
            string? purpose = null,
            string? sourceOfFunds = null)
        {
            ContractNumber = ValidateContractNumber(contractNumber);
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException("CustomerId required", nameof(customerId));
            ContractType = contractType?.Trim();
            PrincipalAmount = principalAmount;
            TotalAmount = principalAmount ?? Money.Zero(principalAmount?.Currency ?? Money.DefaultCurrency);
            StartDate = startDate.Date;
            EndDate = endDate?.Date;
            InterestRate = interestRate is null ? InterestRate : ValidateInterestRate(interestRate.Amount);
            TermInMonths = termInMonths;
            NumberOfInstallments = termInMonths ?? 1; // Default for backward compatibility
            Purpose = purpose?.Trim();
            SourceOfFunds = sourceOfFunds?.Trim();
            PaymentFrequency = paymentFrequency?.Trim();
            GracePeriodDays = gracePeriodDays;
            LateFeePercentage = lateFeePercentage;
            PenaltyPercentage = penaltyPercentage;
            CollateralDescription = collateralDescription?.Trim();
            GuarantorName = guarantorName?.Trim();
            GuarantorContact = guarantorContact?.Trim();
            Notes = notes?.Trim();
            StaffId = ValidateStaffId(staffId);
        }

        private static string ValidateContractNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("ContractNumber required", nameof(value));
            return value.Trim().ToUpperInvariant();
        }

        private static decimal ValidateInterestRate(decimal value)
        {
            if (value < 0m || value > 1m) throw new ArgumentOutOfRangeException(nameof(value), "InterestRate must be between 0 and 1 (0%-100%).");
            return value;
        }

        private static int ValidateInstallments(int value)
        {
            if (value <= 0) throw new ArgumentOutOfRangeException(nameof(value));
            return value;
        }

        private static Guid? ValidateStaffId(Guid? staffId)
        {
            if (staffId.HasValue && staffId.Value == Guid.Empty)
                throw new ArgumentException("StaffId cannot be empty Guid when provided", nameof(staffId));
            return staffId;
        }

        public void UpdateTerms(DateTime startDate, decimal interestRate, int numberOfInstallments, Guid? staffId = null)
        {
            if (Status != ContractStatus.Draft)
                throw new InvalidOperationException("Only draft contracts can be updated.");
            if (_installments.Any())
                throw new InvalidOperationException("Cannot change terms after installments have been generated.");

            StartDate = startDate.Date;
            InterestRate = ValidateInterestRate(interestRate);
            NumberOfInstallments = ValidateInstallments(numberOfInstallments);
            if (staffId.HasValue) StaffId = ValidateStaffId(staffId);
            Touch();
        }

        // New overloaded UpdateTerms method for expanded functionality
        public void UpdateTerms(
            Money? principalAmount,
            Money? interestRate,
            int? termInMonths,
            DateTime startDate,
            DateTime? endDate,
            string? paymentFrequency,
            int? gracePeriodDays,
            decimal? lateFeePercentage,
            decimal? penaltyPercentage,
            string? collateralDescription,
            string? guarantorName,
            string? guarantorContact,
            string? notes,
            Guid? staffId = null)
        {
            if (Status != ContractStatus.Draft)
                throw new InvalidOperationException("Only draft contracts can be updated.");
            if (_installments.Any())
                throw new InvalidOperationException("Cannot change terms after installments have been generated.");

            if (principalAmount != null) PrincipalAmount = principalAmount;
            if (principalAmount != null) TotalAmount = principalAmount; // Maintain backward compatibility
            StartDate = startDate.Date;
            if (endDate != null) EndDate = endDate.Value.Date;
            if (interestRate != null) InterestRate = ValidateInterestRate(interestRate.Amount);
            if (termInMonths != null) TermInMonths = termInMonths;
            if (termInMonths != null) NumberOfInstallments = termInMonths.Value; // Maintain backward compatibility
            if (paymentFrequency != null) PaymentFrequency = paymentFrequency.Trim();
            if (gracePeriodDays != null) GracePeriodDays = gracePeriodDays;
            if (lateFeePercentage != null) LateFeePercentage = lateFeePercentage;
            if (penaltyPercentage != null) PenaltyPercentage = penaltyPercentage;
            if (collateralDescription != null) CollateralDescription = collateralDescription.Trim();
            if (guarantorName != null) GuarantorName = guarantorName.Trim();
            if (guarantorContact != null) GuarantorContact = guarantorContact.Trim();
            if (notes != null) Notes = notes.Trim();
            if (staffId.HasValue) StaffId = ValidateStaffId(staffId);
            Touch();
        }

        public void Activate()
        {
            if (Status != ContractStatus.Draft) throw new InvalidOperationException("Only draft contracts can be activated.");
            Status = ContractStatus.Active;
            Touch();
        }

        public void Suspend()
        {
            if (Status != ContractStatus.Active) throw new InvalidOperationException("Only active contracts can be suspended.");
            Status = ContractStatus.Suspended;
            Touch();
        }

        public void Complete()
        {
            if (_installments.Any(i => i.Status != InstallmentStatus.Paid && i.Status != InstallmentStatus.Waived))
            {
                throw new InvalidOperationException("Cannot complete contract with unpaid installments.");
            }
            Status = ContractStatus.Completed;
            EndDate = DateTime.UtcNow.Date;
            Touch();
        }

        public void Cancel()
        {
            if (Status is ContractStatus.Completed or ContractStatus.Cancelled) throw new InvalidOperationException("Cannot cancel completed or already cancelled contract.");
            Status = ContractStatus.Cancelled;
            EndDate = DateTime.UtcNow.Date;
            Touch();
        }

        public void Default()
        {
            if (Status != ContractStatus.Active) throw new InvalidOperationException("Only active contracts can default.");
            Status = ContractStatus.Defaulted;
            Touch();
        }

        public IReadOnlyCollection<Installment> GenerateInstallments()
        {
            if (_installments.Any()) throw new InvalidOperationException("Installments already generated.");

            var per = new Money(TotalAmount.Amount / NumberOfInstallments, TotalAmount.Currency);
            var accumulated = Money.Zero(TotalAmount.Currency);

            for (int i = 1; i <= NumberOfInstallments; i++)
            {
                var due = StartDate.AddMonths(i);
                var amt = (i == NumberOfInstallments)
                    ? TotalAmount - accumulated
                    : per;
                _installments.Add(new Installment(Id, i, due, amt, "Auto-generated installment"));
                accumulated = accumulated + amt;
            }
            Touch();
            return _installments.AsReadOnly();
        }

        public Money OutstandingAmount()
        {
            if (!_installments.Any()) return TotalAmount;
            var paid = _installments.Aggregate(Money.Zero(TotalAmount.Currency), (acc, inst) => acc + inst.PaidAmount);
            var total = _installments.Aggregate(Money.Zero(TotalAmount.Currency), (acc, inst) => acc + inst.Amount);
            if (paid >= total) return Money.Zero(TotalAmount.Currency);
            return total - paid;
        }

        internal void RegisterPayment(Payment payment)
        {
            if (payment is null) throw new ArgumentNullException(nameof(payment));
            if (payment.ContractId != Id) throw new InvalidOperationException("Payment contract mismatch.");
            _payments.Add(payment);
            Touch();
        }
    }
}


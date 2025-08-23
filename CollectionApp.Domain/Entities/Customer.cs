using System;
using System.Collections.Generic;
using System.Linq;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Domain.Entities
{
    public class Customer : BaseEntity
    {
        // Required by EF Core
        private Customer() { }
        
        // Property to check if customer is active
        public bool IsActive { get; private set; } = true;
        
        // Property to get full name
        public string Name => FullName();
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string NationalId { get; private set; }
        public Address Address { get; private set; }
        public Phone Phone { get; private set; }
        public string Email { get; private set; }
        public DateTime? DateOfBirth { get; private set; }

        // New properties for expanded functionality
        public string? Gender { get; private set; }
        public string? Occupation { get; private set; }
        public string? EmployerName { get; private set; }
        public decimal? MonthlyIncome { get; private set; }
        public int? CreditScore { get; private set; }
        public string? SourceOfFunds { get; private set; }
        public string? PurposeOfLoan { get; private set; }
        public string? Notes { get; private set; }

        private readonly List<Contract> _contracts = new();
        public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();

        // Original constructor for backward compatibility
        public Customer(string firstName, string lastName, string nationalId, Address address, Phone phone, string email, DateTime? dateOfBirth = null)
        {
            FirstName = ValidateName(firstName, nameof(firstName));
            LastName = ValidateName(lastName, nameof(lastName));
            NationalId = ValidateNationalId(nationalId);
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = ValidateEmail(email);
            DateOfBirth = dateOfBirth;
        }

        // New overloaded constructor for expanded functionality
        public Customer(
            string firstName,
            string lastName,
            string nationalId,
            Address address,
            Phone phone,
            string email,
            DateTime? dateOfBirth,
            string? gender,
            string? occupation,
            string? employerName,
            decimal? monthlyIncome,
            int? creditScore,
            string? sourceOfFunds,
            string? purposeOfLoan,
            string? notes)
        {
            FirstName = ValidateName(firstName, nameof(firstName));
            LastName = ValidateName(lastName, nameof(lastName));
            NationalId = ValidateNationalId(nationalId);
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = ValidateEmail(email);
            DateOfBirth = dateOfBirth;
            Gender = gender?.Trim();
            Occupation = occupation?.Trim();
            EmployerName = employerName?.Trim();
            MonthlyIncome = monthlyIncome;
            CreditScore = creditScore;
            SourceOfFunds = sourceOfFunds?.Trim();
            PurposeOfLoan = purposeOfLoan?.Trim();
            Notes = notes?.Trim();
        }

        private static string ValidateName(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} is required.", name);
            return value.Trim();
        }

        private static string ValidateNationalId(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("NationalId is required", nameof(value));
            var v = value.Trim().ToUpperInvariant();
            if (v.Length < 5) throw new ArgumentException("NationalId too short", nameof(value));
            return v;
        }

        private static string ValidateEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email is required", nameof(value));
            var v = value.Trim();
            if (!v.Contains('@') || !v.Contains('.')) throw new ArgumentException("Email format invalid", nameof(value));
            return v;
        }

        public string FullName() => $"{FirstName} {LastName}";
        
        public void SetActive(bool isActive)
        {
            IsActive = isActive;
            Touch();
        }

        public void UpdateContact(Address address, Phone phone, string email)
        {
            Address = address ?? throw new ArgumentNullException(nameof(address));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = ValidateEmail(email);
            Touch();
        }

        public void UpdateIdentity(string firstName, string lastName, string nationalId)
        {
            FirstName = ValidateName(firstName, nameof(firstName));
            LastName = ValidateName(lastName, nameof(lastName));
            NationalId = ValidateNationalId(nationalId);
            Touch();
        }

        // New UpdatePersonalInfo method for expanded functionality
        public void UpdatePersonalInfo(
            DateTime? dateOfBirth,
            string? gender,
            string? occupation,
            string? employerName,
            decimal? monthlyIncome,
            int? creditScore,
            string? sourceOfFunds,
            string? purposeOfLoan,
            string? notes)
        {
            DateOfBirth = dateOfBirth;
            Gender = gender?.Trim();
            Occupation = occupation?.Trim();
            EmployerName = employerName?.Trim();
            MonthlyIncome = monthlyIncome;
            CreditScore = creditScore;
            SourceOfFunds = sourceOfFunds?.Trim();
            PurposeOfLoan = purposeOfLoan?.Trim();
            Notes = notes?.Trim();
            Touch();
        }

        public void AddContract(Contract contract)
        {
            if (contract is null) throw new ArgumentNullException(nameof(contract));
            if (_contracts.Any(c => c.Id == contract.Id)) return;
            _contracts.Add(contract);
            Touch();
        }
    }
}


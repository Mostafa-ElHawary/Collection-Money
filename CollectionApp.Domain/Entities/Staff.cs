using System;
using System.Collections.Generic;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Domain.Entities
{
    public class Staff : BaseEntity
    {
        // Required by EF Core
        private Staff() { }
        public string EmployeeId { get; private set; }
        public string FirstName { get; private set; }
        public string LastName { get; private set; }
        public string Position { get; private set; }
        public string Department { get; private set; }
        public Phone Phone { get; private set; }
        public string Email { get; private set; }
        public DateTime HireDate { get; private set; }
        public bool IsActive { get; private set; }

        // New properties for expanded functionality
        public decimal? Salary { get; private set; }
        public List<string> Permissions { get; private set; } = new();
        public string? Notes { get; private set; }

        private readonly List<Payment> _payments = new();
        public IReadOnlyCollection<Payment> Payments => _payments.AsReadOnly();

        private readonly List<Receipt> _receipts = new();
        public IReadOnlyCollection<Receipt> Receipts => _receipts.AsReadOnly();

        private readonly List<Visit> _visits = new();
        public IReadOnlyCollection<Visit> Visits => _visits.AsReadOnly();

        private readonly List<FollowUp> _followUps = new();
        public IReadOnlyCollection<FollowUp> FollowUps => _followUps.AsReadOnly();

        private readonly List<Contract> _contracts = new();
        public IReadOnlyCollection<Contract> Contracts => _contracts.AsReadOnly();

        // Original constructor for backward compatibility
        public Staff(string employeeId, string firstName, string lastName, string position, string department, Phone phone, string email, DateTime hireDate, bool isActive = true)
        {
            EmployeeId = ValidateRequired(employeeId, nameof(employeeId));
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            Position = ValidateRequired(position, nameof(position));
            Department = ValidateRequired(department, nameof(department));
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Email = ValidateEmail(email);
            HireDate = hireDate.Date;
            IsActive = isActive;
        }

        // New overloaded constructor for expanded functionality
        public Staff(
            string employeeId,
            string firstName,
            string lastName,
            string email,
            Phone phone,
            string position,
            string department,
            DateTime hireDate,
            decimal? salary,
            bool isActive,
            List<string>? permissions,
            string? notes)
        {
            EmployeeId = ValidateRequired(employeeId, nameof(employeeId));
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            Email = ValidateEmail(email);
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Position = ValidateRequired(position, nameof(position));
            Department = ValidateRequired(department, nameof(department));
            HireDate = hireDate.Date;
            Salary = salary;
            IsActive = isActive;
            Permissions = permissions ?? new List<string>();
            Notes = notes?.Trim();
        }

        private static string ValidateRequired(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} is required.", name);
            return value.Trim();
        }

        private static string ValidateEmail(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Email is required", nameof(value));
            var v = value.Trim();
            if (!v.Contains('@') || !v.Contains('.')) throw new ArgumentException("Email format invalid", nameof(value));
            return v;
        }

        public void Deactivate()
        {
            IsActive = false;
            Touch();
        }

        public void Activate()
        {
            IsActive = true;
            Touch();
        }

        public void UpdateContact(Phone phone, string? email, string? department, string? position)
        {
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            if (email != null) Email = ValidateEmail(email);
            if (department != null) Department = ValidateRequired(department, nameof(department));
            if (position != null) Position = ValidateRequired(position, nameof(position));
            Touch();
        }

        // New methods for expanded functionality
        public void UpdatePersonalInfo(string firstName, string lastName, DateTime? hireDate)
        {
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            if (hireDate.HasValue) HireDate = hireDate.Value.Date;
            Touch();
        }

        public void UpdatePersonalInfo(string firstName, string lastName, string position, string department)
        {
            FirstName = ValidateRequired(firstName, nameof(firstName));
            LastName = ValidateRequired(lastName, nameof(lastName));
            Position = ValidateRequired(position, nameof(position));
            Department = ValidateRequired(department, nameof(department));
            Touch();
        }

        public void UpdateContactInfo(string email, Phone phone)
        {
            Email = ValidateEmail(email);
            Phone = phone ?? throw new ArgumentNullException(nameof(phone));
            Touch();
        }

        public void UpdateEmploymentInfo(string position, string department, decimal? salary)
        {
            Position = ValidateRequired(position, nameof(position));
            Department = ValidateRequired(department, nameof(department));
            Salary = salary;
            Touch();
        }

        public void UpdateEmploymentInfo(DateTime hireDate, decimal? salary, bool isActive)
        {
            HireDate = hireDate.Date;
            Salary = salary;
            IsActive = isActive;
            Touch();
        }

        public void UpdatePermissions(List<string> permissions)
        {
            Permissions = permissions ?? new List<string>();
            Touch();
        }

        public void UpdateNotes(string? notes)
        {
            Notes = notes?.Trim();
            Touch();
        }

        public bool HasPermission(string permission)
        {
            return Permissions.Contains(permission, StringComparer.OrdinalIgnoreCase);
        }
    }
}


using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using CollectionApp.Domain.Enums;
//using CollectionApp.Application.Models;

namespace CollectionApp.Application.ViewModels
{
    public class CustomerCreateVM
    {
        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Address (flattened)
        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;
        
        // Alias for AddressLine1 to match view
        public string AddressLine1 { get => Street; set => Street = value; }
        
        // Alias for AddressLine2 (optional second line)
        public string? AddressLine2 { get; set; }

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 3)]
        [RegularExpression("^[A-Z0-9 -]+$")]
        public string ZipCode { get; set; } = string.Empty;
        
        // Alias for PostalCode to match view
        public string PostalCode { get => ZipCode; set => ZipCode = value; }

        // Phone (flattened)
        [Required]
        [StringLength(3, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string PhoneCountryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string PhoneAreaCode { get; set; } = string.Empty;

        [Required]
        [StringLength(12, MinimumLength = 4)]
        [RegularExpression("^\\d+$")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneType { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(100)]
        public string? Occupation { get; set; }

        [MaxLength(200)]
        public string? EmployerName { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MonthlyIncome { get; set; }

        [Range(300, 850)]
        public int? CreditScore { get; set; }

        [MaxLength(100)]
        public string? SourceOfFunds { get; set; }

        [MaxLength(200)]
        public string? PurposeOfLoan { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class CustomerUpdateVM
    {
        [Required]
        public Guid Id { get; set; }

        [Required]
        [MaxLength(100)]
        public string FirstName { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string LastName { get; set; } = string.Empty;

        [Required]
        [MaxLength(50)]
        public string NationalId { get; set; } = string.Empty;

        [Required]
        [EmailAddress]
        [MaxLength(200)]
        public string Email { get; set; } = string.Empty;

        [DataType(DataType.Date)]
        public DateTime? DateOfBirth { get; set; }

        // Address (flattened)
        [Required]
        [MaxLength(200)]
        public string Street { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string City { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string State { get; set; } = string.Empty;

        [Required]
        [MaxLength(100)]
        public string Country { get; set; } = string.Empty;

        [Required]
        [StringLength(10, MinimumLength = 3)]
        [RegularExpression("^[A-Z0-9 -]+$")]
        public string ZipCode { get; set; } = string.Empty;

        // Phone (flattened)
        [Required]
        [StringLength(3, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string PhoneCountryCode { get; set; } = string.Empty;

        [Required]
        [StringLength(5, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string PhoneAreaCode { get; set; } = string.Empty;

        [Required]
        [StringLength(12, MinimumLength = 4)]
        [RegularExpression("^\\d+$")]
        public string PhoneNumber { get; set; } = string.Empty;

        [Required]
        [MaxLength(20)]
        public string PhoneType { get; set; } = string.Empty;

        [MaxLength(10)]
        public string? Gender { get; set; }

        [MaxLength(100)]
        public string? Occupation { get; set; }

        [MaxLength(200)]
        public string? EmployerName { get; set; }

        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MonthlyIncome { get; set; }

        [Range(300, 850)]
        public int? CreditScore { get; set; }

        [MaxLength(100)]
        public string? SourceOfFunds { get; set; }

        [MaxLength(200)]
        public string? PurposeOfLoan { get; set; }

        [MaxLength(1000)]
        public string? Notes { get; set; }
    }

    public class CustomerDetailVM
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => string.Join(" ", StringHelpers.JoinNonEmpty(FirstName, LastName));

        public string NationalId { get; set; } = string.Empty;
        
        // Properties needed by Details.cshtml
        public int ActiveContractsCount { get; set; }
        public decimal TotalContractValue { get; set; }
        public decimal TotalPaidAmount { get; set; }
        public decimal TotalOutstandingAmount { get => TotalContractValue - TotalPaidAmount; }
        
        // Property for phone display
        public string PhoneDisplay
        {
            get
            {
                var parts = StringHelpers.JoinNonEmpty(PhoneCountryCode, PhoneAreaCode, PhoneNumber);
                return string.Join("-", parts);
            }
        }
        
        // Property for address display
        public string AddressDisplay
        {
            get
            {
                var parts = StringHelpers.JoinNonEmpty(Street, City, State, ZipCode, Country);
                return string.Join(", ", parts);
            }
        }

        public string? Email { get; set; }

        public DateTime? DateOfBirth { get; set; }

        // Address (flattened)
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }

        // Phone (flattened)
        public string? PhoneCountryCode { get; set; }
        public string? PhoneAreaCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneType { get; set; }

        // Additional properties
        public string? Gender { get; set; }
        public string? Occupation { get; set; }
        public string? EmployerName { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public int? CreditScore { get; set; }
        public string? SourceOfFunds { get; set; }
        public string? PurposeOfLoan { get; set; }
        public string? Notes { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public IReadOnlyList<CustomerContractSummaryVM> Contracts { get; set; } = Array.Empty<CustomerContractSummaryVM>();
    }

    public class CustomerContractSummaryVM
    {
        public Guid Id { get; set; }
        public string ContractNumber { get; set; } = string.Empty;
        public ContractStatus Status { get; set; }
        public decimal TotalAmount { get; set; }
        public string Currency { get; set; } = string.Empty;
        
        // Properties needed by _ContractSummary.cshtml
        public Guid ContractId { get => Id; set => Id = value; }
        public bool IsActive { get => Status == ContractStatus.Active; }
        public DateTime StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public decimal PaidAmount { get; set; } = 0; // Will be calculated from installments
        public decimal RemainingAmount { get => TotalAmount - PaidAmount; }
        public DateTime? NextPaymentDueDate { get; set; } = null; // Will be calculated from installments
    }

    public class CustomerListVM
    {
        public Guid Id { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string FullName => string.Join(" ", StringHelpers.JoinNonEmpty(FirstName, LastName));

        public string NationalId { get; set; } = string.Empty;

        public string? Email { get; set; }

        // Phone (flattened)
        public string? PhoneCountryCode { get; set; }
        public string? PhoneAreaCode { get; set; }
        public string? PhoneNumber { get; set; }
        public string? PhoneType { get; set; }

        public string PhoneDisplay
        {
            get
            {
                var parts = StringHelpers.JoinNonEmpty(PhoneCountryCode, PhoneAreaCode, PhoneNumber);
                return string.Join("-", parts);
            }
        }
        
        // Primary Address (flattened)
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Country { get; set; }
        public string? ZipCode { get; set; }

        public string AddressDisplay
        {
            get
            {
                var parts = StringHelpers.JoinNonEmpty(Street, City, State, ZipCode, Country);
                return string.Join(", ", parts);
            }
        }

        // Additional properties for list display
        public string? Gender { get; set; }
        public string? Occupation { get; set; }
        public decimal? MonthlyIncome { get; set; }
        public int? CreditScore { get; set; }

        public DateTime CreatedAt { get; set; }

        // Additional computed properties for list display
        public int ActiveContractsCount { get; set; }
        public decimal TotalContractValue { get; set; }
    }

    public class AdvancedCustomerSearchCriteriaVM
    {
        // Basic search
        [MaxLength(100)]
        public string? FirstName { get; set; }
        [MaxLength(100)]
        public string? LastName { get; set; }
        [MaxLength(50)]
        public string? NationalId { get; set; }
        [EmailAddress]
        [MaxLength(200)]
        public string? Email { get; set; }

        // Demographics
        [MaxLength(10)]
        public string? Gender { get; set; }
        public DateTime? DateOfBirthFrom { get; set; }
        public DateTime? DateOfBirthTo { get; set; }
        [MaxLength(100)]
        public string? Occupation { get; set; }

        // Financial
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MonthlyIncomeFrom { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? MonthlyIncomeTo { get; set; }
        [Range(300, 850)]
        public int? CreditScoreFrom { get; set; }
        [Range(300, 850)]
        public int? CreditScoreTo { get; set; }

        // Location
        [MaxLength(100)]
        public string? City { get; set; }
        [MaxLength(100)]
        public string? State { get; set; }
        [MaxLength(100)]
        public string? Country { get; set; }

        // Contract-related
        [Range(0, int.MaxValue)]
        public int? ActiveContractsCountFrom { get; set; }
        [Range(0, int.MaxValue)]
        public int? ActiveContractsCountTo { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? TotalContractValueFrom { get; set; }
        [Range(typeof(decimal), "0", "79228162514264337593543950335")]
        public decimal? TotalContractValueTo { get; set; }

        // Date filters
        public DateTime? CreatedAtFrom { get; set; }
        public DateTime? CreatedAtTo { get; set; }
        public DateTime? UpdatedAtFrom { get; set; }
        public DateTime? UpdatedAtTo { get; set; }

        // Paging and sorting
        [Range(1, int.MaxValue)]
        public int Page { get; set; } = 1;
        [Range(1, 200)]
        public int PageSize { get; set; } = 10;
        [MaxLength(100)]
        public string? OrderBy { get; set; }
        public bool OrderDescending { get; set; } = false;
    }

    public class CustomerAnalyticsVM
    {
        public int TotalCustomers { get; set; }
        public int ActiveCustomers { get; set; }
        public int InactiveCustomers { get; set; }
        public int MaleCustomers { get; set; }
        public int FemaleCustomers { get; set; }
        public int OtherGenderCustomers { get; set; }
        public int AgeUnder25 { get; set; }
        public int Age25To34 { get; set; }
        public int Age35To44 { get; set; }
        public int Age45To54 { get; set; }
        public int Age55Plus { get; set; }
        public decimal AverageMonthlyIncome { get; set; }
        public decimal TotalPortfolioValue { get; set; }
        public int CreditScoreLow { get; set; }
        public int CreditScoreMedium { get; set; }
        public int CreditScoreHigh { get; set; }
        public int ActiveContracts { get; set; }
        public int CompletedContracts { get; set; }
        public int DefaultedContracts { get; set; }
        public int TotalContracts { get; set; }
        public decimal AverageContractValue { get; set; }
        public decimal TotalOutstandingAmount { get; set; }
        public decimal CollectionRate { get; set; }
        public Dictionary<string, int> CustomersByCountry { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CustomersByState { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> CustomersByCity { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, int> NewCustomersOverTime { get; set; } = new Dictionary<string, int>();
        public Dictionary<string, decimal> PortfolioValueOverTime { get; set; } = new Dictionary<string, decimal>();
    }

    public class BulkCustomerUpdateVM
    {
        [Required]
        public List<Guid> CustomerIds { get; set; } = new List<Guid>();
        [MaxLength(200)]
        public string? Street { get; set; }
        [MaxLength(100)]
        public string? City { get; set; }
        [MaxLength(100)]
        public string? State { get; set; }
        [MaxLength(100)]
        public string? Country { get; set; }
        [StringLength(10, MinimumLength = 3)]
        [RegularExpression("^[A-Z0-9 -]+$")]
        public string? ZipCode { get; set; }
        [StringLength(3, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string? PhoneCountryCode { get; set; }
        [StringLength(5, MinimumLength = 1)]
        [RegularExpression("^\\d+$")]
        public string? PhoneAreaCode { get; set; }
        [StringLength(12, MinimumLength = 4)]
        [RegularExpression("^\\d+$")]
        public string? PhoneNumber { get; set; }
        [MaxLength(20)]
        public string? PhoneType { get; set; }
        [MaxLength(200)]
        public string? Email { get; set; }
        [MaxLength(1000)]
        public string? Notes { get; set; }
        public bool ValidateOnly { get; set; } = false;
        public bool OverwriteEmptyFields { get; set; } = false;
    }

    public class BulkUpdateResultVM
    {
        public int TotalRequested { get; set; }
        public int TotalUpdated { get; set; }
        public int TotalSkipped { get; set; }
        public int TotalProcessed => TotalUpdated + TotalSkipped;
        public int TotalSuccessful => TotalUpdated;
        public int TotalFailed => TotalSkipped;
        public List<BulkOperationResult> Results { get; set; } = new List<BulkOperationResult>();
        public List<string> Messages { get; set; } = new List<string>();
        public DateTime CompletedAt { get; set; } = DateTime.UtcNow;
    }

    public class CustomerContractActionVM
    {
        [Required]
        public Guid CustomerId { get; set; }
        [Required]
        public Guid ContractId { get; set; }
        [Required]
        [MaxLength(50)]
        public string Action { get; set; } = string.Empty;
        public Guid? ToCustomerId { get; set; }
    }

    public class CustomerSearchResultVM : CustomerListVM
    {
        public string? Highlight { get; set; }
        public decimal? MatchScore { get; set; }
    }
}


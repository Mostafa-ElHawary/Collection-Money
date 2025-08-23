# Entity-ViewModel Property Mapping Analysis Report

**Analysis Date:** 2025-08-17 22:11:47
**Total Entities:** 9
**Total ViewModels:** 77
**Total Mappings:** 71

## Executive Summary

- **Properties Missing in Entities:** 143
- **Properties Missing in ViewModels:** 677
- **Type Mismatches:** 13
- **Value Object Flattening Patterns:** 20
- **Computed Properties:** 24

## Contract Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\Contract.cs`
**Properties:** 22

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| ContractNumber | string | False | False | False | False |
| CustomerId | Guid | False | False | False | False |
| TotalAmount | Money | False | False | True | False |
| StartDate | DateTime | False | False | False | False |
| EndDate | DateTime? | True | False | False | False |
| Status | ContractStatus | False | False | False | False |
| InterestRate | decimal | False | False | False | False |
| NumberOfInstallments | int | False | False | False | False |
| ContractType | string? | True | False | False | False |
| PrincipalAmount | Money? | True | False | True | False |
| TermInMonths | int? | True | False | False | False |
| PaymentFrequency | string? | True | False | False | False |
| GracePeriodDays | int? | True | False | False | False |
| LateFeePercentage | decimal? | True | False | False | False |
| PenaltyPercentage | decimal? | True | False | False | False |
| CollateralDescription | string? | True | False | False | False |
| GuarantorName | string? | True | False | False | False |
| GuarantorContact | string? | True | False | False | False |
| Notes | string? | True | False | False | False |
| Customer | Customer? | True | False | False | False |
| Installments | IReadOnlyCollection<Installment> | False | False | False | False |
| Payments | IReadOnlyCollection<Payment> | False | False | False | False |

### ViewModel Mappings

#### ContractCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractNumber | ContractNumber | Matched |  |
| CustomerId | CustomerId | Matched |  |
| TotalAmount | TotalAmount | Flattened | Value object Money flattened to TotalAmount |
| StartDate | StartDate | Matched |  |
| EndDate | EndDate | Matched |  |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InterestRate | InterestRate | Matched |  |
| NumberOfInstallments | NumberOfInstallments | Matched |  |
| ContractType | ContractType | Matched |  |
| PrincipalAmount | PrincipalAmount | Flattened | Value object Money? flattened to PrincipalAmount |
| TermInMonths | TermInMonths | Matched |  |
| PaymentFrequency | PaymentFrequency | Matched |  |
| GracePeriodDays | GracePeriodDays | Matched |  |
| LateFeePercentage | LateFeePercentage | Matched |  |
| PenaltyPercentage | PenaltyPercentage | Matched |  |
| CollateralDescription | CollateralDescription | Matched |  |
| GuarantorName | GuarantorName | Derived | Computed/derived property |
| GuarantorContact | GuarantorContact | Matched |  |
| Notes | Notes | Matched |  |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TotalAmount | Currency | Flattened | Value object Money flattened to Currency |

#### ContractUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TotalAmount | TotalAmount | Flattened | Value object Money flattened to TotalAmount |
| StartDate | StartDate | Matched |  |
| EndDate | EndDate | Matched |  |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InterestRate | InterestRate | Matched |  |
| NumberOfInstallments | NumberOfInstallments | Matched |  |
| ContractType | ContractType | Matched |  |
| PrincipalAmount | PrincipalAmount | Flattened | Value object Money? flattened to PrincipalAmount |
| TermInMonths | TermInMonths | Matched |  |
| PaymentFrequency | PaymentFrequency | Matched |  |
| GracePeriodDays | GracePeriodDays | Matched |  |
| LateFeePercentage | LateFeePercentage | Matched |  |
| PenaltyPercentage | PenaltyPercentage | Matched |  |
| CollateralDescription | CollateralDescription | Matched |  |
| GuarantorName | GuarantorName | Derived | Computed/derived property |
| GuarantorContact | GuarantorContact | Matched |  |
| Notes | Notes | Matched |  |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| TotalAmount | Currency | Flattened | Value object Money flattened to Currency |

#### ContractDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractNumber | ContractNumber | Matched |  |
| CustomerId | CustomerId | Matched |  |
| TotalAmount | TotalAmount | Flattened | Value object Money flattened to TotalAmount |
| StartDate | StartDate | Matched |  |
| EndDate | EndDate | Matched |  |
| Status | Status | Matched |  |
| InterestRate | InterestRate | Matched |  |
| NumberOfInstallments | NumberOfInstallments | Matched |  |
| ContractType | ContractType | Matched |  |
| PrincipalAmount | PrincipalAmount | Flattened | Value object Money? flattened to PrincipalAmount |
| TermInMonths | TermInMonths | Matched |  |
| PaymentFrequency | PaymentFrequency | Matched |  |
| GracePeriodDays | GracePeriodDays | Matched |  |
| LateFeePercentage | LateFeePercentage | Matched |  |
| PenaltyPercentage | PenaltyPercentage | Matched |  |
| CollateralDescription | CollateralDescription | Matched |  |
| GuarantorName | GuarantorName | Derived | Computed/derived property |
| GuarantorContact | GuarantorContact | Matched |  |
| Notes | Notes | Matched |  |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installments | Installments | TypeMismatch | Type mismatch: IReadOnlyCollection<Installment> vs IReadOnlyList<ContractInstallmentSummaryVM> |
| Payments | Payments | TypeMismatch | Type mismatch: IReadOnlyCollection<Payment> vs IReadOnlyList<ContractPaymentSummaryVM> |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| TotalAmount | Currency | Flattened | Value object Money flattened to Currency |
| TotalAmount | OutstandingAmount | Flattened | Value object Money flattened to OutstandingAmount |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | StaffName | Derived | Computed/derived property |

#### ContractListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractNumber | ContractNumber | Matched |  |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TotalAmount | TotalAmount | Flattened | Value object Money flattened to TotalAmount |
| StartDate | StartDate | Matched |  |
| EndDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | Status | Matched |  |
| InterestRate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NumberOfInstallments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractType | ContractType | Matched |  |
| PrincipalAmount | PrincipalAmount | Flattened | Value object Money? flattened to PrincipalAmount |
| TermInMonths | TermInMonths | Matched |  |
| PaymentFrequency | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GracePeriodDays | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LateFeePercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PenaltyPercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CollateralDescription | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorContact | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| TotalAmount | Currency | Flattened | Value object Money flattened to Currency |
| TotalAmount | OutstandingAmount | Flattened | Value object Money flattened to OutstandingAmount |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | StaffName | Derived | Computed/derived property |

#### ContractInstallmentSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TotalAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StartDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| EndDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | Status | TypeMismatch | Type mismatch: ContractStatus vs InstallmentStatus |
| InterestRate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NumberOfInstallments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PrincipalAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TermInMonths | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentFrequency | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GracePeriodDays | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LateFeePercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PenaltyPercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CollateralDescription | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorContact | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | InstallmentNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | DueDate | MissingInEntity | Property exists in ViewModel but not in entity |
| TotalAmount | Amount | Flattened | Value object Money flattened to Amount |
| TotalAmount | Currency | Flattened | Value object Money flattened to Currency |

#### ContractPaymentSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TotalAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StartDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| EndDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InterestRate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NumberOfInstallments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PrincipalAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TermInMonths | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentFrequency | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GracePeriodDays | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LateFeePercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PenaltyPercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CollateralDescription | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorContact | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PaymentDate | MissingInEntity | Property exists in ViewModel but not in entity |
| TotalAmount | Amount | Flattened | Value object Money flattened to Amount |
| TotalAmount | Currency | Flattened | Value object Money flattened to Currency |
| - | PaymentMethod | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ReferenceNumber | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerContractSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractNumber | ContractNumber | Matched |  |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TotalAmount | TotalAmount | Flattened | Value object Money flattened to TotalAmount |
| StartDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| EndDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | Status | Matched |  |
| InterestRate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NumberOfInstallments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PrincipalAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TermInMonths | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentFrequency | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GracePeriodDays | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LateFeePercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PenaltyPercentage | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CollateralDescription | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| GuarantorContact | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| TotalAmount | Currency | Flattened | Value object Money flattened to Currency |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **Id** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity
- **InstallmentNumber** - Referenced in ViewModels but not defined in entity
- **DueDate** - Referenced in ViewModels but not defined in entity
- **PaymentDate** - Referenced in ViewModels but not defined in entity
- **PaymentMethod** - Referenced in ViewModels but not defined in entity
- **ReferenceNumber** - Referenced in ViewModels but not defined in entity

## Customer Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\Customer.cs`
**Properties:** 16

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| FirstName | string | False | False | False | False |
| LastName | string | False | False | False | False |
| NationalId | string | False | False | False | False |
| Address | Address | False | False | True | False |
| Phone | Phone | False | False | True | False |
| Email | string | False | False | False | False |
| DateOfBirth | DateTime? | True | False | False | False |
| Gender | string? | True | False | False | False |
| Occupation | string? | True | False | False | False |
| EmployerName | string? | True | False | False | False |
| MonthlyIncome | decimal? | True | False | False | False |
| CreditScore | int? | True | False | False | False |
| SourceOfFunds | string? | True | False | False | False |
| PurposeOfLoan | string? | True | False | False | False |
| Notes | string? | True | False | False | False |
| Contracts | IReadOnlyCollection<Contract> | False | False | False | False |

### ViewModel Mappings

#### CustomerCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | FirstName | Derived | Computed/derived property |
| LastName | LastName | Derived | Computed/derived property |
| NationalId | NationalId | Matched |  |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| DateOfBirth | DateOfBirth | Matched |  |
| Gender | Gender | Matched |  |
| Occupation | Occupation | Matched |  |
| EmployerName | EmployerName | Derived | Computed/derived property |
| MonthlyIncome | MonthlyIncome | Matched |  |
| CreditScore | CreditScore | Matched |  |
| SourceOfFunds | SourceOfFunds | Matched |  |
| PurposeOfLoan | PurposeOfLoan | Matched |  |
| Notes | Notes | Matched |  |
| Contracts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Address | Street | Flattened | Value object Address flattened to Street |
| Address | City | Flattened | Value object Address flattened to City |
| Address | State | Flattened | Value object Address flattened to State |
| Address | Country | Flattened | Value object Address flattened to Country |
| - | Country | Derived | Computed/derived property |
| Address | ZipCode | Flattened | Value object Address flattened to ZipCode |
| Address | PhoneCountryCode | Flattened | Value object Address flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | FirstName | Derived | Computed/derived property |
| LastName | LastName | Derived | Computed/derived property |
| NationalId | NationalId | Matched |  |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| DateOfBirth | DateOfBirth | Matched |  |
| Gender | Gender | Matched |  |
| Occupation | Occupation | Matched |  |
| EmployerName | EmployerName | Derived | Computed/derived property |
| MonthlyIncome | MonthlyIncome | Matched |  |
| CreditScore | CreditScore | Matched |  |
| SourceOfFunds | SourceOfFunds | Matched |  |
| PurposeOfLoan | PurposeOfLoan | Matched |  |
| Notes | Notes | Matched |  |
| Contracts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| Address | Street | Flattened | Value object Address flattened to Street |
| Address | City | Flattened | Value object Address flattened to City |
| Address | State | Flattened | Value object Address flattened to State |
| Address | Country | Flattened | Value object Address flattened to Country |
| - | Country | Derived | Computed/derived property |
| Address | ZipCode | Flattened | Value object Address flattened to ZipCode |
| Address | PhoneCountryCode | Flattened | Value object Address flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | FirstName | Derived | Computed/derived property |
| LastName | LastName | Derived | Computed/derived property |
| NationalId | NationalId | Matched |  |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| DateOfBirth | DateOfBirth | Matched |  |
| Gender | Gender | Matched |  |
| Occupation | Occupation | Matched |  |
| EmployerName | EmployerName | Derived | Computed/derived property |
| MonthlyIncome | MonthlyIncome | Matched |  |
| CreditScore | CreditScore | Matched |  |
| SourceOfFunds | SourceOfFunds | Matched |  |
| PurposeOfLoan | PurposeOfLoan | Matched |  |
| Notes | Notes | Matched |  |
| Contracts | Contracts | TypeMismatch | Type mismatch: IReadOnlyCollection<Contract> vs IReadOnlyList<CustomerContractSummaryVM> |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FullName | Derived | Computed/derived property |
| Address | Street | Flattened | Value object Address flattened to Street |
| Address | City | Flattened | Value object Address flattened to City |
| Address | State | Flattened | Value object Address flattened to State |
| Address | Country | Flattened | Value object Address flattened to Country |
| - | Country | Derived | Computed/derived property |
| Address | ZipCode | Flattened | Value object Address flattened to ZipCode |
| Address | PhoneCountryCode | Flattened | Value object Address flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerContractSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NationalId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DateOfBirth | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Gender | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Occupation | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| EmployerName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| MonthlyIncome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CreditScore | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| SourceOfFunds | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PurposeOfLoan | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contracts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| Phone | ContractNumber | Flattened | Value object Phone flattened to ContractNumber |
| - | Status | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalAmount | Derived | Computed/derived property |
| - | Currency | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | FirstName | Derived | Computed/derived property |
| LastName | LastName | Derived | Computed/derived property |
| NationalId | NationalId | Matched |  |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| DateOfBirth | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Gender | Gender | Matched |  |
| Occupation | Occupation | Matched |  |
| EmployerName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| MonthlyIncome | MonthlyIncome | Matched |  |
| CreditScore | CreditScore | Matched |  |
| SourceOfFunds | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PurposeOfLoan | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contracts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FullName | Derived | Computed/derived property |
| Address | PhoneCountryCode | Flattened | Value object Address flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PhoneDisplay | Derived | Computed/derived property |
| Address | Street | Flattened | Value object Address flattened to Street |
| Address | City | Flattened | Value object Address flattened to City |
| Address | State | Flattened | Value object Address flattened to State |
| Address | Country | Flattened | Value object Address flattened to Country |
| - | Country | Derived | Computed/derived property |
| Address | ZipCode | Flattened | Value object Address flattened to ZipCode |
| - | AddressDisplay | Derived | Computed/derived property |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerFollowUpPatternVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NationalId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DateOfBirth | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Gender | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Occupation | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| EmployerName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| MonthlyIncome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CreditScore | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| SourceOfFunds | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PurposeOfLoan | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contracts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | CustomerId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalFollowUps | Derived | Computed/derived property |
| - | CompletedFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PendingFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | AverageFollowUpsPerMonth | MissingInEntity | Property exists in ViewModel but not in entity |
| - | LastFollowUpDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FollowUpFrequency | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerVisitPatternVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NationalId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DateOfBirth | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Gender | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Occupation | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| EmployerName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| MonthlyIncome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CreditScore | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| SourceOfFunds | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PurposeOfLoan | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contracts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | CustomerId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalVisits | Derived | Computed/derived property |
| - | AverageVisitsPerMonth | MissingInEntity | Property exists in ViewModel but not in entity |
| - | LastVisitDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitFrequency | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PreferredVisitDays | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerEngagementReportVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NationalId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Address | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DateOfBirth | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Gender | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Occupation | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| EmployerName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| MonthlyIncome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CreditScore | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| SourceOfFunds | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PurposeOfLoan | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contracts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | CustomerId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalVisits | Derived | Computed/derived property |
| - | TotalPayments | Derived | Computed/derived property |
| - | TotalAmountPaid | Derived | Computed/derived property |
| - | LastVisitDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | LastPaymentDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitFrequency | MissingInEntity | Property exists in ViewModel but not in entity |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **PhoneType** - Referenced in ViewModels but not defined in entity
- **Id** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity
- **Status** - Referenced in ViewModels but not defined in entity
- **Currency** - Referenced in ViewModels but not defined in entity
- **CustomerId** - Referenced in ViewModels but not defined in entity
- **CompletedFollowUps** - Referenced in ViewModels but not defined in entity
- **PendingFollowUps** - Referenced in ViewModels but not defined in entity
- **AverageFollowUpsPerMonth** - Referenced in ViewModels but not defined in entity
- **LastFollowUpDate** - Referenced in ViewModels but not defined in entity
- **FollowUpFrequency** - Referenced in ViewModels but not defined in entity
- **AverageVisitsPerMonth** - Referenced in ViewModels but not defined in entity
- **LastVisitDate** - Referenced in ViewModels but not defined in entity
- **VisitFrequency** - Referenced in ViewModels but not defined in entity
- **PreferredVisitDays** - Referenced in ViewModels but not defined in entity
- **LastPaymentDate** - Referenced in ViewModels but not defined in entity

## FollowUp Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\FollowUp.cs`
**Properties:** 15

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| CustomerId | Guid | False | False | False | False |
| ContractId | Guid? | True | False | False | False |
| StaffId | Guid | False | False | False | False |
| ScheduledDate | DateTime | False | False | False | False |
| ActualDate | DateTime? | True | False | False | False |
| Type | string | False | False | False | False |
| Priority | string | False | False | False | False |
| Status | string | False | False | False | False |
| Notes | string? | True | False | False | False |
| Outcome | string? | True | False | False | False |
| AssignedToStaffId | Guid? | True | False | False | False |
| Description | string? | True | False | False | False |
| Customer | Customer? | True | False | False | True |
| Contract | Contract? | True | False | False | True |
| Staff | Staff? | True | False | False | False |

### ViewModel Mappings

#### FollowUpCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | CustomerId | Matched |  |
| ContractId | ContractId | Matched |  |
| StaffId | StaffId | Matched |  |
| ScheduledDate | ScheduledDate | Matched |  |
| ActualDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Type | Type | Matched |  |
| Priority | Priority | Matched |  |
| Status | Status | Matched |  |
| Notes | Notes | Matched |  |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| AssignedToStaffId | AssignedToStaffId | Matched |  |
| Description | Description | Matched |  |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |

#### FollowUpUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ScheduledDate | ScheduledDate | Matched |  |
| ActualDate | ActualDate | Matched |  |
| Type | Type | Matched |  |
| Priority | Priority | Matched |  |
| Status | Status | Matched |  |
| Notes | Notes | Matched |  |
| Outcome | Outcome | Matched |  |
| AssignedToStaffId | AssignedToStaffId | Matched |  |
| Description | Description | Matched |  |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |

#### FollowUpDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | CustomerId | Matched |  |
| ContractId | ContractId | Matched |  |
| StaffId | StaffId | Matched |  |
| ScheduledDate | ScheduledDate | Matched |  |
| ActualDate | ActualDate | Matched |  |
| Type | Type | Matched |  |
| Priority | Priority | Matched |  |
| Status | Status | Matched |  |
| Notes | Notes | Matched |  |
| Outcome | Outcome | Matched |  |
| AssignedToStaffId | AssignedToStaffId | Matched |  |
| Description | Description | Matched |  |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| - | StaffName | Derived | Computed/derived property |
| - | AssignedStaffName | Derived | Computed/derived property |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### FollowUpListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ScheduledDate | ScheduledDate | Matched |  |
| ActualDate | ActualDate | Matched |  |
| Type | Type | Matched |  |
| Priority | Priority | Matched |  |
| Status | Status | Matched |  |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| AssignedToStaffId | AssignedToStaffId | Matched |  |
| Description | Description | Matched |  |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | StaffName | Derived | Computed/derived property |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### FollowUpAnalyticsViewModels (Analytics)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ScheduledDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ActualDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Type | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Priority | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| AssignedToStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Description | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |

#### FollowUpEffectivenessVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | StaffId | Matched |  |
| ScheduledDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ActualDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Type | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Priority | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| AssignedToStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Description | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | TotalFollowUps | Derived | Computed/derived property |
| - | CompletedFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PendingFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | OverdueFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CompletionRate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | AverageCompletionTime | MissingInEntity | Property exists in ViewModel but not in entity |

#### FollowUpPerformanceVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ScheduledDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ActualDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Type | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Priority | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| AssignedToStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Description | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | FromDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ToDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalFollowUps | Derived | Computed/derived property |
| - | CompletedFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PendingFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | OverdueFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | HighPriorityFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FollowUpsByType | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FollowUpsByPriority | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerFollowUpPatternVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | CustomerId | Matched |  |
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ScheduledDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ActualDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Type | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Priority | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| AssignedToStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Description | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | TotalFollowUps | Derived | Computed/derived property |
| - | CompletedFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PendingFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | AverageFollowUpsPerMonth | MissingInEntity | Property exists in ViewModel but not in entity |
| - | LastFollowUpDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FollowUpFrequency | MissingInEntity | Property exists in ViewModel but not in entity |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **Id** - Referenced in ViewModels but not defined in entity
- **ContractNumber** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity
- **CompletedFollowUps** - Referenced in ViewModels but not defined in entity
- **PendingFollowUps** - Referenced in ViewModels but not defined in entity
- **OverdueFollowUps** - Referenced in ViewModels but not defined in entity
- **CompletionRate** - Referenced in ViewModels but not defined in entity
- **AverageCompletionTime** - Referenced in ViewModels but not defined in entity
- **FromDate** - Referenced in ViewModels but not defined in entity
- **ToDate** - Referenced in ViewModels but not defined in entity
- **HighPriorityFollowUps** - Referenced in ViewModels but not defined in entity
- **FollowUpsByType** - Referenced in ViewModels but not defined in entity
- **FollowUpsByPriority** - Referenced in ViewModels but not defined in entity
- **AverageFollowUpsPerMonth** - Referenced in ViewModels but not defined in entity
- **LastFollowUpDate** - Referenced in ViewModels but not defined in entity
- **FollowUpFrequency** - Referenced in ViewModels but not defined in entity

## Installment Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\Installment.cs`
**Properties:** 9

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| ContractId | Guid | False | False | False | False |
| InstallmentNumber | int | False | False | False | False |
| DueDate | DateTime | False | False | False | False |
| Amount | Money | False | False | True | False |
| Status | InstallmentStatus | False | False | False | False |
| PaidAmount | Money | False | False | True | False |
| PaidDate | DateTime? | True | False | False | False |
| Contract | Contract? | True | False | False | True |
| Payments | IReadOnlyCollection<Payment> | False | False | False | False |

### ViewModel Mappings

#### ContractInstallmentSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentNumber | InstallmentNumber | Matched |  |
| DueDate | DueDate | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| Status | Status | Matched |  |
| PaidAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| Amount | Currency | Flattened | Value object Money flattened to Currency |

#### InstallmentCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | ContractId | Matched |  |
| InstallmentNumber | InstallmentNumber | Matched |  |
| DueDate | DueDate | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Currency | Flattened | Value object Money flattened to Currency |

#### InstallmentUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DueDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |

#### InstallmentDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | ContractId | Matched |  |
| InstallmentNumber | InstallmentNumber | Matched |  |
| DueDate | DueDate | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| Status | Status | Matched |  |
| PaidAmount | PaidAmount | Flattened | Value object Money flattened to PaidAmount |
| PaidDate | PaidDate | Matched |  |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | Payments | TypeMismatch | Type mismatch: IReadOnlyCollection<Payment> vs IReadOnlyList<InstallmentPaymentSummaryVM> |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| Amount | Currency | Flattened | Value object Money flattened to Currency |
| Amount | PaidCurrency | Flattened | Value object Money flattened to PaidCurrency |
| - | IsOverdue | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### InstallmentListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentNumber | InstallmentNumber | Matched |  |
| DueDate | DueDate | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| Status | Status | Matched |  |
| PaidAmount | PaidAmount | Flattened | Value object Money flattened to PaidAmount |
| PaidDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| Amount | Currency | Flattened | Value object Money flattened to Currency |
| Amount | PaidCurrency | Flattened | Value object Money flattened to PaidCurrency |
| Amount | RemainingAmount | Flattened | Value object Money flattened to RemainingAmount |
| - | IsOverdue | MissingInEntity | Property exists in ViewModel but not in entity |

#### InstallmentPaymentSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DueDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PaymentDate | MissingInEntity | Property exists in ViewModel but not in entity |
| Amount | Currency | Flattened | Value object Money flattened to Currency |
| - | ReferenceNumber | MissingInEntity | Property exists in ViewModel but not in entity |

#### InstallmentAnalyticsViewModels (Analytics)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DueDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |

#### InstallmentStatusSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | ContractId | Matched |  |
| InstallmentNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DueDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaidAmount | PaidAmount | Flattened | Value object Money flattened to PaidAmount |
| PaidDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | TotalInstallments | Derived | Computed/derived property |
| - | PaidInstallments | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PendingInstallments | MissingInEntity | Property exists in ViewModel but not in entity |
| - | OverdueInstallments | MissingInEntity | Property exists in ViewModel but not in entity |
| - | WaivedInstallments | MissingInEntity | Property exists in ViewModel but not in entity |
| Amount | TotalAmount | Flattened | Value object Money flattened to TotalAmount |
| - | TotalAmount | Derived | Computed/derived property |
| Amount | OutstandingAmount | Flattened | Value object Money flattened to OutstandingAmount |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **Id** - Referenced in ViewModels but not defined in entity
- **ContractNumber** - Referenced in ViewModels but not defined in entity
- **IsOverdue** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity
- **PaymentDate** - Referenced in ViewModels but not defined in entity
- **ReferenceNumber** - Referenced in ViewModels but not defined in entity
- **PaidInstallments** - Referenced in ViewModels but not defined in entity
- **PendingInstallments** - Referenced in ViewModels but not defined in entity
- **OverdueInstallments** - Referenced in ViewModels but not defined in entity
- **WaivedInstallments** - Referenced in ViewModels but not defined in entity

## LedgerEntry Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\LedgerEntry.cs`
**Properties:** 13

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| ContractId | Guid? | True | False | False | False |
| CustomerId | Guid? | True | False | False | False |
| TransactionDate | DateTime | False | False | False | False |
| Description | string | False | False | False | False |
| DebitAmount | Money | False | False | True | False |
| CreditAmount | Money | False | False | True | False |
| Balance | Money | False | False | True | False |
| ReferenceType | string | False | False | False | False |
| ReferenceId | Guid? | True | False | False | False |
| StaffId | Guid? | True | False | False | False |
| Contract | Contract? | True | False | False | True |
| Customer | Customer? | True | False | False | True |
| Staff | Staff? | True | False | False | False |

### ViewModel Mappings

#### LedgerEntryCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | ContractId | Matched |  |
| CustomerId | CustomerId | Matched |  |
| TransactionDate | TransactionDate | Matched |  |
| Description | Description | Matched |  |
| DebitAmount | DebitAmount | Flattened | Value object Money flattened to DebitAmount |
| CreditAmount | CreditAmount | Flattened | Value object Money flattened to CreditAmount |
| Balance | Balance | TypeMismatch | Type mismatch: Money vs decimal |
| ReferenceType | ReferenceType | Matched |  |
| ReferenceId | ReferenceId | Matched |  |
| StaffId | StaffId | Matched |  |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| DebitAmount | Currency | Flattened | Value object Money flattened to Currency |

#### LedgerEntryUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TransactionDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Description | Description | Matched |  |
| DebitAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CreditAmount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Balance | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ReferenceType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ReferenceId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |

#### LedgerEntryDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | ContractId | Matched |  |
| CustomerId | CustomerId | Matched |  |
| TransactionDate | TransactionDate | Matched |  |
| Description | Description | Matched |  |
| DebitAmount | DebitAmount | Flattened | Value object Money flattened to DebitAmount |
| CreditAmount | CreditAmount | Flattened | Value object Money flattened to CreditAmount |
| Balance | Balance | TypeMismatch | Type mismatch: Money vs decimal |
| ReferenceType | ReferenceType | Matched |  |
| ReferenceId | ReferenceId | Matched |  |
| StaffId | StaffId | Matched |  |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| DebitAmount | Currency | Flattened | Value object Money flattened to Currency |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| - | StaffName | Derived | Computed/derived property |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### LedgerEntryListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| TransactionDate | TransactionDate | Matched |  |
| Description | Description | Matched |  |
| DebitAmount | DebitAmount | Flattened | Value object Money flattened to DebitAmount |
| CreditAmount | CreditAmount | Flattened | Value object Money flattened to CreditAmount |
| Balance | Balance | TypeMismatch | Type mismatch: Money vs decimal |
| ReferenceType | ReferenceType | Matched |  |
| ReferenceId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| DebitAmount | Currency | Flattened | Value object Money flattened to Currency |
| - | CustomerName | Derived | Computed/derived property |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **Id** - Referenced in ViewModels but not defined in entity
- **ContractNumber** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity

## Payment Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\Payment.cs`
**Properties:** 13

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| ContractId | Guid | False | False | False | False |
| InstallmentId | Guid | False | False | False | False |
| Amount | Money | False | False | True | False |
| PaymentDate | DateTime | False | False | False | False |
| PaymentMethod | PaymentMethod | False | False | False | False |
| ReferenceNumber | string? | True | False | False | False |
| Notes | string? | True | False | False | False |
| StaffId | Guid? | True | False | False | False |
| ProcessedByStaffId | Guid? | True | False | False | False |
| Contract | Contract? | True | False | False | True |
| Installment | Installment? | True | False | False | True |
| Staff | Staff? | True | False | False | False |
| Receipt | Receipt? | True | False | False | False |

### ViewModel Mappings

#### ContractPaymentSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| PaymentDate | PaymentDate | Matched |  |
| PaymentMethod | PaymentMethod | Matched |  |
| ReferenceNumber | ReferenceNumber | Matched |  |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ProcessedByStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| Amount | Currency | Flattened | Value object Money flattened to Currency |

#### InstallmentPaymentSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| PaymentDate | PaymentDate | Matched |  |
| PaymentMethod | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ReferenceNumber | ReferenceNumber | Matched |  |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ProcessedByStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| Amount | Currency | Flattened | Value object Money flattened to Currency |

#### PaymentHistoryVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentId | InstallmentId | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| PaymentDate | PaymentDate | Matched |  |
| PaymentMethod | PaymentMethod | TypeMismatch | Type mismatch: PaymentMethod vs string |
| ReferenceNumber | ReferenceNumber | Matched |  |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ProcessedByStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Status | MissingInEntity | Property exists in ViewModel but not in entity |

#### PaymentTrendsVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentMethod | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ReferenceNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ProcessedByStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | FromDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ToDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalPayments | Derived | Computed/derived property |
| Amount | AveragePaymentAmount | Flattened | Value object Money flattened to AveragePaymentAmount |
| - | PaymentTrendsByMonth | MissingInEntity | Property exists in ViewModel but not in entity |

#### PaymentCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | ContractId | Matched |  |
| InstallmentId | InstallmentId | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| PaymentDate | PaymentDate | Matched |  |
| PaymentMethod | PaymentMethod | Matched |  |
| ReferenceNumber | ReferenceNumber | Matched |  |
| Notes | Notes | Matched |  |
| StaffId | StaffId | Matched |  |
| ProcessedByStaffId | ProcessedByStaffId | Matched |  |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Currency | Flattened | Value object Money flattened to Currency |

#### PaymentUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentMethod | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ReferenceNumber | ReferenceNumber | Matched |  |
| Notes | Notes | Matched |  |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ProcessedByStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |

#### PaymentDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | ContractId | Matched |  |
| InstallmentId | InstallmentId | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| PaymentDate | PaymentDate | Matched |  |
| PaymentMethod | PaymentMethod | Matched |  |
| ReferenceNumber | ReferenceNumber | Matched |  |
| Notes | Notes | Matched |  |
| StaffId | StaffId | Matched |  |
| ProcessedByStaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | InstallmentNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| Amount | Currency | Flattened | Value object Money flattened to Currency |
| - | StaffName | Derived | Computed/derived property |
| - | ReceiptId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ReceiptNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### PaymentListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ContractId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| InstallmentId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| PaymentDate | PaymentDate | Matched |  |
| PaymentMethod | PaymentMethod | Matched |  |
| ReferenceNumber | ReferenceNumber | Matched |  |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| ProcessedByStaffId | ProcessedByStaffId | Matched |  |
| Contract | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Installment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipt | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ContractNumber | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| Amount | Currency | Flattened | Value object Money flattened to Currency |
| - | StaffName | Derived | Computed/derived property |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **Id** - Referenced in ViewModels but not defined in entity
- **Status** - Referenced in ViewModels but not defined in entity
- **FromDate** - Referenced in ViewModels but not defined in entity
- **ToDate** - Referenced in ViewModels but not defined in entity
- **PaymentTrendsByMonth** - Referenced in ViewModels but not defined in entity
- **ContractNumber** - Referenced in ViewModels but not defined in entity
- **InstallmentNumber** - Referenced in ViewModels but not defined in entity
- **ReceiptId** - Referenced in ViewModels but not defined in entity
- **ReceiptNumber** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity

## Receipt Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\Receipt.cs`
**Properties:** 10

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| ReceiptNumber | string | False | False | False | False |
| PaymentId | Guid | False | False | False | False |
| CustomerId | Guid | False | False | False | False |
| Amount | Money | False | False | True | False |
| IssueDate | DateTime | False | False | False | False |
| Description | string? | True | False | False | False |
| StaffId | Guid | False | False | False | False |
| Payment | Payment? | True | False | False | True |
| Customer | Customer? | True | False | False | True |
| Staff | Staff? | True | False | False | False |

### ViewModel Mappings

#### ReceiptCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ReceiptNumber | ReceiptNumber | Matched |  |
| PaymentId | PaymentId | Matched |  |
| CustomerId | CustomerId | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| IssueDate | IssueDate | Matched |  |
| Description | Description | Matched |  |
| StaffId | StaffId | Matched |  |
| Payment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Currency | Flattened | Value object Money flattened to Currency |

#### ReceiptUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ReceiptNumber | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| PaymentId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IssueDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Description | Description | Matched |  |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |

#### ReceiptDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ReceiptNumber | ReceiptNumber | Matched |  |
| PaymentId | PaymentId | Matched |  |
| CustomerId | CustomerId | Matched |  |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| IssueDate | IssueDate | Matched |  |
| Description | Description | Matched |  |
| StaffId | StaffId | Matched |  |
| Payment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PaymentDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| Amount | Currency | Flattened | Value object Money flattened to Currency |
| - | StaffName | Derived | Computed/derived property |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### ReceiptListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| ReceiptNumber | ReceiptNumber | Matched |  |
| PaymentId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Amount | Amount | Flattened | Value object Money flattened to Amount |
| IssueDate | IssueDate | Matched |  |
| Description | Description | Matched |  |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payment | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| Amount | Currency | Flattened | Value object Money flattened to Currency |
| - | StaffName | Derived | Computed/derived property |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **Id** - Referenced in ViewModels but not defined in entity
- **PaymentDate** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity

## Staff Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\Staff.cs`
**Properties:** 16

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| EmployeeId | string | False | False | False | False |
| FirstName | string | False | False | False | False |
| LastName | string | False | False | False | False |
| Position | string | False | False | False | False |
| Department | string | False | False | False | False |
| Phone | Phone | False | False | True | False |
| Email | string | False | False | False | False |
| HireDate | DateTime | False | False | False | False |
| IsActive | bool | False | False | False | False |
| Salary | decimal? | True | False | False | False |
| Permissions | List<string> | False | True | False | False |
| Notes | string? | True | False | False | False |
| Payments | IReadOnlyCollection<Payment> | False | False | False | False |
| Receipts | IReadOnlyCollection<Receipt> | False | False | False | False |
| Visits | IReadOnlyCollection<Visit> | False | False | False | False |
| FollowUps | IReadOnlyCollection<FollowUp> | False | False | False | False |

### ViewModel Mappings

#### StaffWorkloadVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Department | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | StaffId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalFollowUps | Derived | Computed/derived property |
| - | PendingFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | OverdueFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | HighPriorityFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TodayFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ThisWeekFollowUps | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | EmployeeId | Matched |  |
| FirstName | FirstName | Derived | Computed/derived property |
| LastName | LastName | Derived | Computed/derived property |
| Position | Position | Matched |  |
| Department | Department | Matched |  |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| HireDate | HireDate | Matched |  |
| IsActive | IsActive | Matched |  |
| Salary | Salary | Matched |  |
| Permissions | Permissions | Matched |  |
| Notes | Notes | Matched |  |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | PhoneCountryCode | Flattened | Value object Phone flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | Position | Matched |  |
| Department | Department | Matched |  |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | IsActive | Matched |  |
| Salary | Salary | Matched |  |
| Permissions | Permissions | Matched |  |
| Notes | Notes | Matched |  |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| Phone | PhoneCountryCode | Flattened | Value object Phone flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | EmployeeId | Matched |  |
| FirstName | FirstName | Derived | Computed/derived property |
| LastName | LastName | Derived | Computed/derived property |
| Position | Position | Matched |  |
| Department | Department | Matched |  |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| HireDate | HireDate | Matched |  |
| IsActive | IsActive | Matched |  |
| Salary | Salary | Matched |  |
| Permissions | Permissions | Matched |  |
| Notes | Notes | Matched |  |
| Payments | Payments | TypeMismatch | Type mismatch: IReadOnlyCollection<Payment> vs IReadOnlyList<StaffRelatedSummaryVM> |
| Receipts | Receipts | TypeMismatch | Type mismatch: IReadOnlyCollection<Receipt> vs IReadOnlyList<StaffRelatedSummaryVM> |
| Visits | Visits | TypeMismatch | Type mismatch: IReadOnlyCollection<Visit> vs IReadOnlyList<StaffRelatedSummaryVM> |
| FollowUps | FollowUps | TypeMismatch | Type mismatch: IReadOnlyCollection<FollowUp> vs IReadOnlyList<StaffRelatedSummaryVM> |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FullName | Derived | Computed/derived property |
| Phone | PhoneCountryCode | Flattened | Value object Phone flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneDisplay | Derived | Computed/derived property |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | EmployeeId | Matched |  |
| FirstName | FirstName | Derived | Computed/derived property |
| LastName | LastName | Derived | Computed/derived property |
| Position | Position | Matched |  |
| Department | Department | Matched |  |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | Email | Matched |  |
| HireDate | HireDate | Matched |  |
| IsActive | IsActive | Matched |  |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FullName | Derived | Computed/derived property |
| Phone | PhoneCountryCode | Flattened | Value object Phone flattened to PhoneCountryCode |
| - | PhoneCountryCode | Derived | Computed/derived property |
| Phone | PhoneAreaCode | Flattened | Value object Phone flattened to PhoneAreaCode |
| Phone | PhoneNumber | Flattened | Value object Phone flattened to PhoneNumber |
| - | PhoneDisplay | Derived | Computed/derived property |
| - | PhoneType | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffRelatedSummaryVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Department | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Reference | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Date | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Description | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffAnalyticsViewModels (Analytics)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Department | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |

#### StaffActivityVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Department | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | ActivityType | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ActivityId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Description | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Amount | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Date | MissingInEntity | Property exists in ViewModel but not in entity |
| - | RelatedEntityId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | RelatedEntityType | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffPerformanceVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Department | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | StaffId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FromDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ToDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalPaymentsProcessed | Derived | Computed/derived property |
| - | TotalAmountCollected | Derived | Computed/derived property |
| - | TotalVisitsConducted | Derived | Computed/derived property |
| - | TotalFollowUpsCompleted | Derived | Computed/derived property |
| - | AveragePaymentAmount | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitToPaymentRatio | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FollowUpCompletionRate | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffVisitPerformanceVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Department | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | StaffId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalVisits | Derived | Computed/derived property |
| - | CompletedVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CancelledVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | NoShowVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | AverageVisitDuration | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitsWithPayments | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalAmountCollected | Derived | Computed/derived property |
| - | SuccessRate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PaymentConversionRate | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffScheduleVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| EmployeeId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FirstName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| LastName | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Position | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Department | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Phone | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Email | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| HireDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| IsActive | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Salary | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Permissions | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Payments | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Receipts | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Visits | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| FollowUps | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | ActivityType | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ActivityId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Description | MissingInEntity | Property exists in ViewModel but not in entity |
| - | StartTime | MissingInEntity | Property exists in ViewModel but not in entity |
| - | EndTime | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Location | MissingInEntity | Property exists in ViewModel but not in entity |
| - | Status | MissingInEntity | Property exists in ViewModel but not in entity |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **StaffId** - Referenced in ViewModels but not defined in entity
- **PendingFollowUps** - Referenced in ViewModels but not defined in entity
- **OverdueFollowUps** - Referenced in ViewModels but not defined in entity
- **HighPriorityFollowUps** - Referenced in ViewModels but not defined in entity
- **TodayFollowUps** - Referenced in ViewModels but not defined in entity
- **ThisWeekFollowUps** - Referenced in ViewModels but not defined in entity
- **PhoneType** - Referenced in ViewModels but not defined in entity
- **Id** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity
- **Reference** - Referenced in ViewModels but not defined in entity
- **Date** - Referenced in ViewModels but not defined in entity
- **Description** - Referenced in ViewModels but not defined in entity
- **ActivityType** - Referenced in ViewModels but not defined in entity
- **ActivityId** - Referenced in ViewModels but not defined in entity
- **Amount** - Referenced in ViewModels but not defined in entity
- **RelatedEntityId** - Referenced in ViewModels but not defined in entity
- **RelatedEntityType** - Referenced in ViewModels but not defined in entity
- **FromDate** - Referenced in ViewModels but not defined in entity
- **ToDate** - Referenced in ViewModels but not defined in entity
- **AveragePaymentAmount** - Referenced in ViewModels but not defined in entity
- **VisitToPaymentRatio** - Referenced in ViewModels but not defined in entity
- **FollowUpCompletionRate** - Referenced in ViewModels but not defined in entity
- **CompletedVisits** - Referenced in ViewModels but not defined in entity
- **CancelledVisits** - Referenced in ViewModels but not defined in entity
- **NoShowVisits** - Referenced in ViewModels but not defined in entity
- **AverageVisitDuration** - Referenced in ViewModels but not defined in entity
- **VisitsWithPayments** - Referenced in ViewModels but not defined in entity
- **SuccessRate** - Referenced in ViewModels but not defined in entity
- **PaymentConversionRate** - Referenced in ViewModels but not defined in entity
- **StartTime** - Referenced in ViewModels but not defined in entity
- **EndTime** - Referenced in ViewModels but not defined in entity
- **Location** - Referenced in ViewModels but not defined in entity
- **Status** - Referenced in ViewModels but not defined in entity

## Visit Entity

**File:** `C:\Users\welcome\source\repos\Tracker-Money\CollectionApp.Domain\Entities\Visit.cs`
**Properties:** 13

### Entity Properties

| Property | Type | Nullable | Collection | Value Object | Navigation |
|----------|------|----------|------------|--------------|------------|
| CustomerId | Guid | False | False | False | False |
| StaffId | Guid | False | False | False | False |
| VisitDate | DateTime | False | False | False | False |
| Purpose | string | False | False | False | False |
| Notes | string? | True | False | False | False |
| Outcome | string? | True | False | False | False |
| NextVisitDate | DateTime? | True | False | False | False |
| Duration | TimeSpan? | True | False | False | False |
| VisitType | string? | True | False | False | False |
| Location | string? | True | False | False | False |
| Status | VisitStatus | False | False | False | False |
| Customer | Customer? | True | False | False | True |
| Staff | Staff? | True | False | False | True |

### ViewModel Mappings

#### VisitCreateVM (Create)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | CustomerId | Matched |  |
| StaffId | StaffId | Matched |  |
| VisitDate | VisitDate | Matched |  |
| Purpose | Purpose | Matched |  |
| Notes | Notes | Matched |  |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | Duration | Matched |  |
| VisitType | VisitType | Matched |  |
| Location | Location | Matched |  |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | DurationHours | MissingInEntity | Property exists in ViewModel but not in entity |
| - | DurationMinutes | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitUpdateVM (Update)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | VisitDate | Matched |  |
| Purpose | Purpose | Matched |  |
| Notes | Notes | Matched |  |
| Outcome | Outcome | Matched |  |
| NextVisitDate | NextVisitDate | Matched |  |
| Duration | Duration | Matched |  |
| VisitType | VisitType | Matched |  |
| Location | Location | Matched |  |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | DurationHours | MissingInEntity | Property exists in ViewModel but not in entity |
| - | DurationMinutes | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitDetailVM (Detail)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | CustomerId | Matched |  |
| StaffId | StaffId | Matched |  |
| VisitDate | VisitDate | Matched |  |
| Purpose | Purpose | Matched |  |
| Notes | Notes | Matched |  |
| Outcome | Outcome | Matched |  |
| NextVisitDate | NextVisitDate | Matched |  |
| Duration | Duration | Matched |  |
| VisitType | VisitType | Matched |  |
| Location | Location | Matched |  |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| - | StaffName | Derived | Computed/derived property |
| - | DurationHours | MissingInEntity | Property exists in ViewModel but not in entity |
| - | DurationMinutes | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |
| - | UpdatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitListVM (List)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | VisitDate | Matched |  |
| Purpose | Purpose | Matched |  |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | Outcome | Matched |  |
| NextVisitDate | NextVisitDate | Matched |  |
| Duration | Duration | Matched |  |
| VisitType | VisitType | Matched |  |
| Location | Location | Matched |  |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Id | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| - | StaffName | Derived | Computed/derived property |
| - | DurationHours | MissingInEntity | Property exists in ViewModel but not in entity |
| - | DurationMinutes | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CreatedAt | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitAnalyticsViewModels (Analytics)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |

#### VisitEffectivenessVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | StaffId | Matched |  |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | TotalVisits | Derived | Computed/derived property |
| - | VisitsWithPayments | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalAmountFromVisits | Derived | Computed/derived property |
| - | ConversionRate | MissingInEntity | Property exists in ViewModel but not in entity |

#### CustomerVisitPatternVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | CustomerId | Matched |  |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | TotalVisits | Derived | Computed/derived property |
| - | AverageVisitsPerMonth | MissingInEntity | Property exists in ViewModel but not in entity |
| - | LastVisitDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitFrequency | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PreferredVisitDays | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitOutcomeAnalysisVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | FromDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ToDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalVisits | Derived | Computed/derived property |
| - | SuccessfulVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | FollowUpRequired | MissingInEntity | Property exists in ViewModel but not in entity |
| - | NoShowVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | OutcomeBreakdown | MissingInEntity | Property exists in ViewModel but not in entity |

#### StaffVisitPerformanceVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | StaffId | Matched |  |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | TotalVisits | Derived | Computed/derived property |
| - | CompletedVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CancelledVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | NoShowVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | AverageVisitDuration | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitsWithPayments | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalAmountCollected | Derived | Computed/derived property |
| - | SuccessRate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PaymentConversionRate | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitRouteVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | CustomerId | Matched |  |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Order | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CustomerName | Derived | Computed/derived property |
| - | Address | MissingInEntity | Property exists in ViewModel but not in entity |
| - | EstimatedDuration | MissingInEntity | Property exists in ViewModel but not in entity |
| - | EstimatedTravelTime | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitConflictVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | ExistingVisitId | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ExistingVisitTime | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ExistingVisitDuration | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ProposedVisitTime | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ProposedVisitDuration | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ConflictType | MissingInEntity | Property exists in ViewModel but not in entity |

#### VisitSummaryReportVM (Summary)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | FromDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | ToDate | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalVisits | Derived | Computed/derived property |
| - | CompletedVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CancelledVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | NoShowVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalVisitHours | Derived | Computed/derived property |
| - | AverageVisitDuration | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitsByType | MissingInEntity | Property exists in ViewModel but not in entity |

#### TerritoryVisitReportVM (General)

| Entity Property | ViewModel Property | Status | Notes |
|----------------|-------------------|--------|-------|
| CustomerId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| StaffId | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Purpose | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Notes | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Outcome | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| NextVisitDate | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Duration | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| VisitType | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Location | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Status | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Customer | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| Staff | - | MissingInViewModel | Property exists in entity but not in ViewModel |
| - | Territory | MissingInEntity | Property exists in ViewModel but not in entity |
| - | TotalVisits | Derived | Computed/derived property |
| - | CompletedVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | PendingVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | CancelledVisits | MissingInEntity | Property exists in ViewModel but not in entity |
| - | VisitsByStaff | MissingInEntity | Property exists in ViewModel but not in entity |
| - | AverageVisitsPerDay | MissingInEntity | Property exists in ViewModel but not in entity |

### Missing Properties in Entity

The following properties are referenced in ViewModels but missing from the entity:

- **DurationHours** - Referenced in ViewModels but not defined in entity
- **DurationMinutes** - Referenced in ViewModels but not defined in entity
- **Id** - Referenced in ViewModels but not defined in entity
- **CreatedAt** - Referenced in ViewModels but not defined in entity
- **UpdatedAt** - Referenced in ViewModels but not defined in entity
- **VisitsWithPayments** - Referenced in ViewModels but not defined in entity
- **ConversionRate** - Referenced in ViewModels but not defined in entity
- **AverageVisitsPerMonth** - Referenced in ViewModels but not defined in entity
- **LastVisitDate** - Referenced in ViewModels but not defined in entity
- **VisitFrequency** - Referenced in ViewModels but not defined in entity
- **PreferredVisitDays** - Referenced in ViewModels but not defined in entity
- **FromDate** - Referenced in ViewModels but not defined in entity
- **ToDate** - Referenced in ViewModels but not defined in entity
- **SuccessfulVisits** - Referenced in ViewModels but not defined in entity
- **FollowUpRequired** - Referenced in ViewModels but not defined in entity
- **NoShowVisits** - Referenced in ViewModels but not defined in entity
- **OutcomeBreakdown** - Referenced in ViewModels but not defined in entity
- **CompletedVisits** - Referenced in ViewModels but not defined in entity
- **CancelledVisits** - Referenced in ViewModels but not defined in entity
- **AverageVisitDuration** - Referenced in ViewModels but not defined in entity
- **SuccessRate** - Referenced in ViewModels but not defined in entity
- **PaymentConversionRate** - Referenced in ViewModels but not defined in entity
- **Order** - Referenced in ViewModels but not defined in entity
- **Address** - Referenced in ViewModels but not defined in entity
- **EstimatedDuration** - Referenced in ViewModels but not defined in entity
- **EstimatedTravelTime** - Referenced in ViewModels but not defined in entity
- **ExistingVisitId** - Referenced in ViewModels but not defined in entity
- **ExistingVisitTime** - Referenced in ViewModels but not defined in entity
- **ExistingVisitDuration** - Referenced in ViewModels but not defined in entity
- **ProposedVisitTime** - Referenced in ViewModels but not defined in entity
- **ProposedVisitDuration** - Referenced in ViewModels but not defined in entity
- **ConflictType** - Referenced in ViewModels but not defined in entity
- **VisitsByType** - Referenced in ViewModels but not defined in entity
- **Territory** - Referenced in ViewModels but not defined in entity
- **PendingVisits** - Referenced in ViewModels but not defined in entity
- **VisitsByStaff** - Referenced in ViewModels but not defined in entity
- **AverageVisitsPerDay** - Referenced in ViewModels but not defined in entity

## Value Object Flattening Analysis

The following properties represent flattened value objects:

- TotalAmount
- PrincipalAmount
- Currency
- OutstandingAmount
- Amount
- Street
- City
- State
- Country
- ZipCode
- PhoneCountryCode
- PhoneAreaCode
- PhoneNumber
- ContractNumber
- PaidAmount
- PaidCurrency
- RemainingAmount
- DebitAmount
- CreditAmount
- AveragePaymentAmount

## Computed Properties Analysis

The following properties are computed/derived and not expected to exist in entities:

- GuarantorName
- CustomerName
- StaffName
- FirstName
- LastName
- EmployerName
- Country
- PhoneCountryCode
- FullName
- TotalAmount
- PhoneDisplay
- AddressDisplay
- TotalFollowUps
- TotalVisits
- TotalPayments
- TotalAmountPaid
- AssignedStaffName
- TotalInstallments
- TotalPaymentsProcessed
- TotalAmountCollected
- TotalVisitsConducted
- TotalFollowUpsCompleted
- TotalAmountFromVisits
- TotalVisitHours

## Recommendations

### Critical Issues to Address:

**Contract Entity:**
- Add missing property `Id` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements
- Add missing property `InstallmentNumber` to support ViewModel requirements
- Add missing property `DueDate` to support ViewModel requirements
- Add missing property `PaymentDate` to support ViewModel requirements
- Add missing property `PaymentMethod` to support ViewModel requirements
- Add missing property `ReferenceNumber` to support ViewModel requirements

**Customer Entity:**
- Add missing property `PhoneType` to support ViewModel requirements
- Add missing property `Id` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements
- Add missing property `Status` to support ViewModel requirements
- Add missing property `Currency` to support ViewModel requirements
- Add missing property `CustomerId` to support ViewModel requirements
- Add missing property `CompletedFollowUps` to support ViewModel requirements
- Add missing property `PendingFollowUps` to support ViewModel requirements
- Add missing property `AverageFollowUpsPerMonth` to support ViewModel requirements
- Add missing property `LastFollowUpDate` to support ViewModel requirements
- Add missing property `FollowUpFrequency` to support ViewModel requirements
- Add missing property `AverageVisitsPerMonth` to support ViewModel requirements
- Add missing property `LastVisitDate` to support ViewModel requirements
- Add missing property `VisitFrequency` to support ViewModel requirements
- Add missing property `PreferredVisitDays` to support ViewModel requirements
- Add missing property `LastPaymentDate` to support ViewModel requirements

**FollowUp Entity:**
- Add missing property `Id` to support ViewModel requirements
- Add missing property `ContractNumber` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements
- Add missing property `CompletedFollowUps` to support ViewModel requirements
- Add missing property `PendingFollowUps` to support ViewModel requirements
- Add missing property `OverdueFollowUps` to support ViewModel requirements
- Add missing property `CompletionRate` to support ViewModel requirements
- Add missing property `AverageCompletionTime` to support ViewModel requirements
- Add missing property `FromDate` to support ViewModel requirements
- Add missing property `ToDate` to support ViewModel requirements
- Add missing property `HighPriorityFollowUps` to support ViewModel requirements
- Add missing property `FollowUpsByType` to support ViewModel requirements
- Add missing property `FollowUpsByPriority` to support ViewModel requirements
- Add missing property `AverageFollowUpsPerMonth` to support ViewModel requirements
- Add missing property `LastFollowUpDate` to support ViewModel requirements
- Add missing property `FollowUpFrequency` to support ViewModel requirements

**Installment Entity:**
- Add missing property `Id` to support ViewModel requirements
- Add missing property `ContractNumber` to support ViewModel requirements
- Add missing property `IsOverdue` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements
- Add missing property `PaymentDate` to support ViewModel requirements
- Add missing property `ReferenceNumber` to support ViewModel requirements
- Add missing property `PaidInstallments` to support ViewModel requirements
- Add missing property `PendingInstallments` to support ViewModel requirements
- Add missing property `OverdueInstallments` to support ViewModel requirements
- Add missing property `WaivedInstallments` to support ViewModel requirements

**LedgerEntry Entity:**
- Add missing property `Id` to support ViewModel requirements
- Add missing property `ContractNumber` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements

**Payment Entity:**
- Add missing property `Id` to support ViewModel requirements
- Add missing property `Status` to support ViewModel requirements
- Add missing property `FromDate` to support ViewModel requirements
- Add missing property `ToDate` to support ViewModel requirements
- Add missing property `PaymentTrendsByMonth` to support ViewModel requirements
- Add missing property `ContractNumber` to support ViewModel requirements
- Add missing property `InstallmentNumber` to support ViewModel requirements
- Add missing property `ReceiptId` to support ViewModel requirements
- Add missing property `ReceiptNumber` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements

**Receipt Entity:**
- Add missing property `Id` to support ViewModel requirements
- Add missing property `PaymentDate` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements

**Staff Entity:**
- Add missing property `StaffId` to support ViewModel requirements
- Add missing property `PendingFollowUps` to support ViewModel requirements
- Add missing property `OverdueFollowUps` to support ViewModel requirements
- Add missing property `HighPriorityFollowUps` to support ViewModel requirements
- Add missing property `TodayFollowUps` to support ViewModel requirements
- Add missing property `ThisWeekFollowUps` to support ViewModel requirements
- Add missing property `PhoneType` to support ViewModel requirements
- Add missing property `Id` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements
- Add missing property `Reference` to support ViewModel requirements
- Add missing property `Date` to support ViewModel requirements
- Add missing property `Description` to support ViewModel requirements
- Add missing property `ActivityType` to support ViewModel requirements
- Add missing property `ActivityId` to support ViewModel requirements
- Add missing property `Amount` to support ViewModel requirements
- Add missing property `RelatedEntityId` to support ViewModel requirements
- Add missing property `RelatedEntityType` to support ViewModel requirements
- Add missing property `FromDate` to support ViewModel requirements
- Add missing property `ToDate` to support ViewModel requirements
- Add missing property `AveragePaymentAmount` to support ViewModel requirements
- Add missing property `VisitToPaymentRatio` to support ViewModel requirements
- Add missing property `FollowUpCompletionRate` to support ViewModel requirements
- Add missing property `CompletedVisits` to support ViewModel requirements
- Add missing property `CancelledVisits` to support ViewModel requirements
- Add missing property `NoShowVisits` to support ViewModel requirements
- Add missing property `AverageVisitDuration` to support ViewModel requirements
- Add missing property `VisitsWithPayments` to support ViewModel requirements
- Add missing property `SuccessRate` to support ViewModel requirements
- Add missing property `PaymentConversionRate` to support ViewModel requirements
- Add missing property `StartTime` to support ViewModel requirements
- Add missing property `EndTime` to support ViewModel requirements
- Add missing property `Location` to support ViewModel requirements
- Add missing property `Status` to support ViewModel requirements

**Visit Entity:**
- Add missing property `DurationHours` to support ViewModel requirements
- Add missing property `DurationMinutes` to support ViewModel requirements
- Add missing property `Id` to support ViewModel requirements
- Add missing property `CreatedAt` to support ViewModel requirements
- Add missing property `UpdatedAt` to support ViewModel requirements
- Add missing property `VisitsWithPayments` to support ViewModel requirements
- Add missing property `ConversionRate` to support ViewModel requirements
- Add missing property `AverageVisitsPerMonth` to support ViewModel requirements
- Add missing property `LastVisitDate` to support ViewModel requirements
- Add missing property `VisitFrequency` to support ViewModel requirements
- Add missing property `PreferredVisitDays` to support ViewModel requirements
- Add missing property `FromDate` to support ViewModel requirements
- Add missing property `ToDate` to support ViewModel requirements
- Add missing property `SuccessfulVisits` to support ViewModel requirements
- Add missing property `FollowUpRequired` to support ViewModel requirements
- Add missing property `NoShowVisits` to support ViewModel requirements
- Add missing property `OutcomeBreakdown` to support ViewModel requirements
- Add missing property `CompletedVisits` to support ViewModel requirements
- Add missing property `CancelledVisits` to support ViewModel requirements
- Add missing property `AverageVisitDuration` to support ViewModel requirements
- Add missing property `SuccessRate` to support ViewModel requirements
- Add missing property `PaymentConversionRate` to support ViewModel requirements
- Add missing property `Order` to support ViewModel requirements
- Add missing property `Address` to support ViewModel requirements
- Add missing property `EstimatedDuration` to support ViewModel requirements
- Add missing property `EstimatedTravelTime` to support ViewModel requirements
- Add missing property `ExistingVisitId` to support ViewModel requirements
- Add missing property `ExistingVisitTime` to support ViewModel requirements
- Add missing property `ExistingVisitDuration` to support ViewModel requirements
- Add missing property `ProposedVisitTime` to support ViewModel requirements
- Add missing property `ProposedVisitDuration` to support ViewModel requirements
- Add missing property `ConflictType` to support ViewModel requirements
- Add missing property `VisitsByType` to support ViewModel requirements
- Add missing property `Territory` to support ViewModel requirements
- Add missing property `PendingVisits` to support ViewModel requirements
- Add missing property `VisitsByStaff` to support ViewModel requirements
- Add missing property `AverageVisitsPerDay` to support ViewModel requirements

### Design Patterns to Maintain:
- Value object flattening for UI presentation
- Computed properties for derived data
- Navigation property mappings for related data

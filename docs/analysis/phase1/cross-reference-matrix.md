# Cross-Reference Matrix for Shared ViewModels

This document provides a comprehensive cross-reference matrix for ViewModels that are shared across multiple services, ensuring consistency in property expectations and usage patterns.

## Shared ViewModels Matrix

### 1. VisitAnalyticsViewModels.StaffScheduleVM

| Service | Method | Usage Context | Property Expectations | Consistency Requirements |
|---------|--------|---------------|----------------------|-------------------------|
| **VisitService.cs** | `GetStaffScheduleAsync` | Visit scheduling and route planning | All properties required, Location can be null | Must maintain visit-specific data structure |
| **StaffService.cs** | `GetStaffScheduleAsync` | Comprehensive staff scheduling | All properties required, ActivityType varies | Must support multiple activity types beyond visits |

**Shared Properties Analysis:**
- **ActivityType**: VisitService uses "Visit", StaffService supports multiple types
- **ActivityId**: Both services expect valid GUID for activity identification
- **Description**: Both services build human-readable descriptions
- **StartTime/EndTime**: Both services require valid DateTime ranges
- **Location**: Both services allow nullable location information
- **Status**: Both services require current activity status

**Consistency Requirements:**
1. **Time Validation**: EndTime must always be greater than StartTime
2. **Activity Identification**: ActivityId must be unique and valid
3. **Status Values**: Status must use consistent enumeration values
4. **Description Format**: Maintain consistent description formatting patterns

### 2. VisitAnalyticsViewModels.VisitEffectivenessVM

| Service | Method | Usage Context | Property Expectations | Consistency Requirements |
|---------|--------|---------------|----------------------|-------------------------|
| **VisitService.cs** | `GetVisitEffectivenessAsync` | Visit performance analysis | All properties required, calculations must be accurate | Must maintain visit-centric performance metrics |
| **StaffService.cs** | `GetVisitEffectivenessAsync` | Staff performance evaluation | All properties required, same calculation logic | Must produce identical results for same staff member |

**Shared Properties Analysis:**
- **StaffId**: Both services expect valid GUID for staff identification
- **TotalVisits**: Both services must calculate from same visit data source
- **VisitsWithPayments**: Both services must use consistent payment detection logic
- **TotalAmountFromVisits**: Both services must calculate from same payment data
- **ConversionRate**: Both services must use identical calculation formula

**Consistency Requirements:**
1. **Data Source**: Both services must query from same visit and payment repositories
2. **Calculation Logic**: Conversion rate = (VisitsWithPayments / TotalVisits) * 100
3. **Amount Precision**: TotalAmountFromVisits must use consistent decimal precision
4. **Performance**: Calculations must be efficient for real-time reporting

### 3. VisitAnalyticsViewModels.VisitRouteVM

| Service | Method | Usage Context | Property Expectations | Consistency Requirements |
|---------|--------|---------------|----------------------|-------------------------|
| **VisitService.cs** | `PlanVisitRouteAsync` | Route optimization for visits | All properties required, Order must be sequential | Must maintain route sequence integrity |

**Shared Properties Analysis:**
- **Order**: Must be sequential starting from 1
- **CustomerId**: Must reference valid customer entities
- **CustomerName**: Must be non-empty for display purposes
- **Address**: Can be null but should be populated when available
- **EstimatedDuration**: Must be positive TimeSpan
- **EstimatedTravelTime**: Must be non-negative TimeSpan

**Consistency Requirements:**
1. **Route Sequence**: Order property must be sequential and unique within route
2. **Customer Validation**: CustomerId must reference existing customer entities
3. **Time Calculations**: EstimatedDuration and EstimatedTravelTime must be realistic
4. **Address Handling**: Address should be consistent with customer entity format

### 4. VisitAnalyticsViewModels.VisitConflictVM

| Service | Method | Usage Context | Property Expectations | Consistency Requirements |
|---------|--------|---------------|----------------------|-------------------------|
| **VisitService.cs** | `GetVisitConflictsAsync` | Conflict detection for scheduling | All properties required, conflict analysis | Must provide accurate conflict detection |

**Shared Properties Analysis:**
- **ExistingVisitId**: Must reference valid existing visit
- **ExistingVisitTime**: Must be valid DateTime for existing visit
- **ExistingVisitDuration**: Must be positive TimeSpan
- **ProposedVisitTime**: Must be valid DateTime for new visit
- **ProposedVisitDuration**: Must be positive TimeSpan
- **ConflictType**: Must use consistent conflict classification

**Consistency Requirements:**
1. **Conflict Detection**: Must accurately identify overlapping time periods
2. **Conflict Classification**: ConflictType must use standardized values
3. **Time Validation**: All DateTime values must be in consistent timezone
4. **Duration Validation**: All TimeSpan values must be positive

## Cross-Service Property Consistency Rules

### 1. DateTime Handling
- **Rule**: All DateTime properties must use consistent timezone (preferably UTC)
- **Services Affected**: VisitService, StaffService
- **Implementation**: Use DateTime.UtcNow for new records, convert display times as needed

### 2. GUID Validation
- **Rule**: All Guid properties must be validated against existing entities
- **Services Affected**: All services using ViewModels
- **Implementation**: Validate GUIDs before creating ViewModel instances

### 3. String Properties
- **Rule**: Non-nullable string properties must be non-empty
- **Services Affected**: All services using ViewModels
- **Implementation**: Use string.IsNullOrWhiteSpace() validation

### 4. Numeric Constraints
- **Rule**: All numeric properties must respect business rule constraints
- **Services Affected**: All services using ViewModels
- **Implementation**: Implement validation attributes and business logic validation

### 5. Collection Properties
- **Rule**: Collection properties must be initialized to prevent null reference exceptions
- **Services Affected**: All services using ViewModels
- **Implementation**: Initialize collections in ViewModel constructors

## Service Integration Points

### VisitService ↔ StaffService
- **Shared ViewModels**: StaffScheduleVM, VisitEffectivenessVM
- **Data Consistency**: Both services must use same visit and staff data sources
- **Performance Impact**: Shared calculations should be cached or optimized

### VisitService ↔ LedgerService
- **Shared Data**: Visit payment information affects financial calculations
- **Consistency**: Payment amounts and dates must be synchronized
- **Audit Trail**: Financial transactions must maintain audit trail

### StaffService ↔ InstallmentService
- **Shared Data**: Staff performance affects installment collection success
- **Consistency**: Staff metrics must reflect actual collection performance
- **Reporting**: Performance reports must use consistent data sources

## Implementation Recommendations

### 1. Centralized ViewModel Factory
```csharp
public interface IViewModelFactory
{
    T CreateViewModel<T>(object source) where T : class;
    bool ValidateViewModel<T>(T viewModel) where T : class;
}
```

### 2. Shared Validation Attributes
```csharp
[AttributeUsage(AttributeTargets.Property)]
public class ViewModelValidationAttribute : ValidationAttribute
{
    // Common validation logic for ViewModels
}
```

### 3. Cross-Service Data Consistency
```csharp
public interface IDataConsistencyService
{
    Task<bool> ValidateCrossServiceConsistencyAsync();
    Task SynchronizeDataAsync();
}
```

### 4. Performance Optimization
```csharp
public interface IViewModelCacheService
{
    T GetOrCreate<T>(string key, Func<T> factory) where T : class;
    void Invalidate(string key);
}
```

## Conclusion

The cross-reference matrix reveals that several ViewModels are shared across multiple services, requiring careful attention to:

1. **Property Consistency**: Ensure identical property definitions across services
2. **Data Validation**: Implement consistent validation rules
3. **Performance Optimization**: Cache shared calculations and data
4. **Error Handling**: Provide consistent error messages and handling
5. **Documentation**: Maintain up-to-date property specifications

This analysis provides the foundation for implementing a robust ViewModel system that maintains consistency across the entire application architecture. 
# Missing ViewModels by Service Analysis

This document analyzes each service to identify missing ViewModels and their exact usage patterns in the codebase.

## VisitService.cs

### Missing ViewModels and Usage Patterns

#### 1. VisitRouteVM
- **Location**: Line 394, 413
- **Method**: `PlanVisitRouteAsync`
- **Usage**: Creating route planning data for staff visits
- **Code Excerpt**:
```csharp
route.Add(new VisitAnalyticsViewModels.VisitRouteVM
{
    Order = order,
    CustomerId = customer.Id,
    CustomerName = customer.FullName,
    Address = customer.Address,
    EstimatedDuration = estimatedDuration,
    EstimatedTravelTime = travelTime
});
```

#### 2. StaffScheduleVM
- **Location**: Line 430, 436
- **Method**: `GetStaffScheduleAsync`
- **Usage**: Building staff daily schedule with visit activities
- **Code Excerpt**:
```csharp
schedule.Add(new VisitAnalyticsViewModels.StaffScheduleVM
{
    ActivityType = "Visit",
    ActivityId = visit.Id,
    Description = $"Visit to {customer.FullName}",
    StartTime = visit.VisitDate,
    EndTime = visit.VisitDate.Add(visit.Duration),
    Location = visit.Location,
    Status = visit.Status.ToString()
});
```

#### 3. VisitConflictVM
- **Location**: Line 494, 504
- **Method**: `GetVisitConflictsAsync`
- **Usage**: Identifying scheduling conflicts for proposed visits
- **Code Excerpt**:
```csharp
conflicts.Add(new VisitAnalyticsViewModels.VisitConflictVM
{
    ExistingVisitId = existingVisit.Id,
    ExistingVisitTime = existingVisit.VisitDate,
    ExistingVisitDuration = existingVisit.Duration,
    ProposedVisitTime = proposedTime,
    ProposedVisitDuration = duration,
    ConflictType = conflictType
});
```

## StaffService.cs

### Missing ViewModels and Usage Patterns

#### 1. VisitEffectivenessVM
- **Location**: Line 306, 312, 507
- **Method**: `GetVisitEffectivenessAsync`
- **Usage**: Analyzing staff visit effectiveness metrics
- **Code Excerpt**:
```csharp
var visitEffectiveness = new VisitEffectivenessVM
{
    StaffId = staffId,
    TotalVisits = totalVisits,
    VisitsWithPayments = visitsWithPayments,
    TotalAmountFromVisits = totalAmount,
    ConversionRate = conversionRate
};
```

#### 2. StaffScheduleVM
- **Location**: Line 434, 443, 460, 514
- **Method**: `GetStaffScheduleAsync`
- **Usage**: Building comprehensive staff schedules including visits and other activities
- **Code Excerpt**:
```csharp
schedule.Add(new VisitAnalyticsViewModels.StaffScheduleVM
{
    ActivityType = "Visit",
    ActivityId = visit.Id,
    Description = $"Visit to {customer.FullName}",
    StartTime = visit.VisitDate,
    EndTime = visit.VisitDate.Add(visit.Duration),
    Location = visit.Location,
    Status = visit.Status.ToString()
});
```

## InstallmentService.cs

### Missing ViewModels and Usage Patterns

#### 1. PaymentHistoryVM
- **Location**: Line 318
- **Method**: `GetPaymentHistoryAsync`
- **Usage**: Retrieving payment history for installments
- **Code Excerpt**: Not visible in current code - requires implementation

## LedgerService.cs

### Missing ViewModels and Usage Patterns

#### 1. AccountsReceivableVM
- **Location**: Line 381
- **Method**: `GetAccountsReceivableAsync`
- **Usage**: Generating accounts receivable reports
- **Code Excerpt**: Not visible in current code - requires implementation

#### 2. AuditTrailVM
- **Location**: Line 394
- **Method**: `GetAuditTrailAsync`
- **Usage**: Retrieving audit trail information
- **Code Excerpt**: Not visible in current code - requires implementation

## FollowUpService.cs

### Missing ViewModels and Usage Patterns

No specific missing ViewModels identified in FollowUpService.cs - all referenced types appear to be properly defined.

## PaymentService.cs

### Missing ViewModels and Usage Patterns

No specific missing ViewModels identified in PaymentService.cs - all referenced types appear to be properly defined.

## Summary

The analysis reveals that most ViewModels are properly defined within the `VisitAnalyticsViewModels` nested class structure. However, there are several ViewModels referenced in services that need to be implemented:

1. **PaymentHistoryVM** - Referenced in InstallmentService
2. **AccountsReceivableVM** - Referenced in LedgerService  
3. **AuditTrailVM** - Referenced in LedgerService

The main issue appears to be that services are trying to access ViewModels that either don't exist or are not properly accessible due to namespace/accessibility issues. 
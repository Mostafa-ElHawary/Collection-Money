# ViewModel Property Specifications

This document provides detailed property specifications for each missing ViewModel, including types, nullability, constraints, source expressions, and rationale.

## VisitAnalyticsViewModels.VisitEffectivenessVM

### Property Specifications

| Property | Type | Nullability | Constraints | Source Expression | Rationale |
|----------|------|-------------|-------------|-------------------|-----------|
| StaffId | Guid | Non-nullable | Must be valid GUID | `staffId` parameter | Identifies the staff member being analyzed |
| TotalVisits | int | Non-nullable | ≥ 0 | `totalVisits` calculation | Total number of visits conducted by staff |
| VisitsWithPayments | int | Non-nullable | ≥ 0, ≤ TotalVisits | `visitsWithPayments` calculation | Count of visits that resulted in payments |
| TotalAmountFromVisits | decimal | Non-nullable | ≥ 0.00m | `totalAmount` calculation | Total monetary value collected from visits |
| ConversionRate | decimal | Non-nullable | 0.00m to 100.00m | `conversionRate` calculation | Percentage of visits that resulted in payments |

### Source Code Location
- **Service**: VisitService.cs, StaffService.cs
- **Method**: `GetVisitEffectivenessAsync`
- **Usage**: Performance analysis and reporting

## VisitAnalyticsViewModels.CustomerVisitPatternVM

### Property Specifications

| Property | Type | Nullability | Constraints | Source Expression | Rationale |
|----------|------|-------------|-------------|-------------------|-----------|
| CustomerId | Guid | Non-nullable | Must be valid GUID | `customerId` parameter | Identifies the customer being analyzed |
| TotalVisits | int | Non-nullable | ≥ 0 | `totalVisits` calculation | Total number of visits to this customer |
| AverageVisitsPerMonth | decimal | Non-nullable | ≥ 0.00m | `averageVisitsPerMonth` calculation | Monthly visit frequency average |
| LastVisitDate | DateTime? | Nullable | If null, no visits recorded | `lastVisitDate` from visit history | Most recent visit date for customer |
| VisitFrequency | double | Non-nullable | ≥ 0.0 | `visitFrequency` calculation | Days between average visits |
| PreferredVisitDays | List<string> | Non-nullable | Non-empty list of day names | `preferredVisitDays` analysis | Customer's preferred visit days |

### Source Code Location
- **Service**: VisitService.cs
- **Method**: `GetCustomerVisitPatternAsync`
- **Usage**: Customer behavior analysis and scheduling optimization

## VisitAnalyticsViewModels.VisitOutcomeAnalysisVM

### Property Specifications

| Property | Type | Nullability | Constraints | Source Expression | Rationale |
|----------|------|-------------|-------------|-------------------|-----------|
| FromDate | DateTime | Non-nullable | Valid date range | `fromDate` parameter | Start date for analysis period |
| ToDate | DateTime | Non-nullable | Valid date range, ≥ FromDate | `toDate` parameter | End date for analysis period |
| TotalVisits | int | Non-nullable | ≥ 0 | `totalVisits` calculation | Total visits in date range |
| SuccessfulVisits | int | Non-nullable | ≥ 0, ≤ TotalVisits | `successfulVisits` calculation | Visits that achieved objectives |
| FollowUpRequired | int | Non-nullable | ≥ 0, ≤ TotalVisits | `followUpRequired` calculation | Visits requiring follow-up action |
| NoShowVisits | int | Non-nullable | ≥ 0, ≤ TotalVisits | `noShowVisits` calculation | Visits where customer didn't appear |
| OutcomeBreakdown | Dictionary<string, int> | Non-nullable | Non-empty dictionary | `outcomeBreakdown` analysis | Detailed breakdown by outcome type |

### Source Code Location
- **Service**: VisitService.cs
- **Method**: `GetVisitOutcomeAnalysisAsync`
- **Usage**: Visit success rate analysis and trend reporting

## VisitAnalyticsViewModels.StaffVisitPerformanceVM

### Property Specifications

| Property | Type | Nullability | Constraints | Source Expression | Rationale |
|----------|------|-------------|-------------|-------------------|-----------|
| StaffId | Guid | Non-nullable | Must be valid GUID | `staffId` parameter | Identifies the staff member |
| TotalVisits | int | Non-nullable | ≥ 0 | `totalVisits` calculation | Total visits assigned to staff |
| CompletedVisits | int | Non-nullable | ≥ 0, ≤ TotalVisits | `completedVisits` calculation | Successfully completed visits |
| CancelledVisits | int | Non-nullable | ≥ 0, ≤ TotalVisits | `cancelledVisits` calculation | Visits that were cancelled |
| NoShowVisits | int | Non-nullable | ≥ 0, ≤ TotalVisits | `noShowVisits` calculation | Visits where customer didn't show |
| AverageVisitDuration | double | Non-nullable | ≥ 0.0 | `averageVisitDuration` calculation | Average time spent per visit |
| VisitsWithPayments | int | Non-nullable | ≥ 0, ≤ TotalVisits | `visitsWithPayments` calculation | Visits resulting in payments |
| TotalAmountCollected | decimal | Non-nullable | ≥ 0.00m | `totalAmountCollected` calculation | Total money collected by staff |
| SuccessRate | decimal | Non-nullable | 0.00m to 100.00m | `successRate` calculation | Percentage of successful visits |
| PaymentConversionRate | decimal | Non-nullable | 0.00m to 100.00m | `paymentConversionRate` calculation | Percentage of visits with payments |

### Source Code Location
- **Service**: VisitService.cs
- **Method**: `GetStaffVisitPerformanceAsync`
- **Usage**: Staff performance evaluation and KPI tracking

## VisitAnalyticsViewModels.VisitRouteVM

### Property Specifications

| Property | Type | Nullability | Constraints | Source Expression | Rationale |
|----------|------|-------------|-------------|-------------------|-----------|
| Order | int | Non-nullable | ≥ 1 | `order` parameter | Sequence order in route |
| CustomerId | Guid | Non-nullable | Must be valid GUID | `customer.Id` | Identifies the customer |
| CustomerName | string | Non-nullable | Non-empty string | `customer.FullName` | Customer's full name for display |
| Address | string? | Nullable | Can be null if not available | `customer.Address` | Customer's address for routing |
| EstimatedDuration | TimeSpan | Non-nullable | > TimeSpan.Zero | `estimatedDuration` calculation | Expected visit duration |
| EstimatedTravelTime | TimeSpan | Non-nullable | ≥ TimeSpan.Zero | `travelTime` calculation | Travel time to next location |

### Source Code Location
- **Service**: VisitService.cs
- **Method**: `PlanVisitRouteAsync`
- **Usage**: Route optimization and scheduling

## VisitAnalyticsViewModels.StaffScheduleVM

### Property Specifications

| Property | Type | Nullability | Constraints | Source Expression | Rationale |
|----------|------|-------------|-------------|-------------------|-----------|
| ActivityType | string | Non-nullable | Non-empty string | `"Visit"` or other activity | Type of scheduled activity |
| ActivityId | Guid | Non-nullable | Must be valid GUID | `visit.Id` or activity ID | Unique identifier for activity |
| Description | string | Non-nullable | Non-empty string | `$"Visit to {customer.FullName}"` | Human-readable activity description |
| StartTime | DateTime | Non-nullable | Valid date/time | `visit.VisitDate` | When activity begins |
| EndTime | DateTime | Non-nullable | Valid date/time, > StartTime | `visit.VisitDate.Add(visit.Duration)` | When activity ends |
| Location | string? | Nullable | Can be null if not applicable | `visit.Location` | Where activity takes place |
| Status | string | Non-nullable | Non-empty string | `visit.Status.ToString()` | Current status of activity |

### Source Code Location
- **Service**: VisitService.cs, StaffService.cs
- **Method**: `GetStaffScheduleAsync`
- **Usage**: Staff daily schedule management and display

## VisitAnalyticsViewModels.VisitConflictVM

### Property Specifications

| Property | Type | Nullability | Constraints | Source Expression | Rationale |
|----------|------|-------------|-------------|-------------------|-----------|
| ExistingVisitId | Guid | Non-nullable | Must be valid GUID | `existingVisit.Id` | ID of conflicting existing visit |
| ExistingVisitTime | DateTime | Non-nullable | Valid date/time | `existingVisit.VisitDate` | When existing visit is scheduled |
| ExistingVisitDuration | TimeSpan | Non-nullable | > TimeSpan.Zero | `existingVisit.Duration` | Duration of existing visit |
| ProposedVisitTime | DateTime | Non-nullable | Valid date/time | `proposedTime` parameter | When new visit is proposed |
| ProposedVisitDuration | TimeSpan | Non-nullable | > TimeSpan.Zero | `duration` parameter | Duration of proposed visit |
| ConflictType | string | Non-nullable | Non-empty string | `conflictType` analysis | Type of scheduling conflict |

### Source Code Location
- **Service**: VisitService.cs
- **Method**: `GetVisitConflictsAsync`
- **Usage**: Conflict detection and resolution

## Missing ViewModels Requiring Implementation

### PaymentHistoryVM (InstallmentService)
**Location**: Line 318 in InstallmentService.cs
**Purpose**: Track payment history for installments
**Required Properties**: InstallmentId, PaymentDate, Amount, PaymentMethod, Status, Notes

### AccountsReceivableVM (LedgerService)
**Location**: Line 381 in LedgerService.cs
**Purpose**: Generate accounts receivable reports
**Required Properties**: CustomerId, CustomerName, OutstandingAmount, DueDate, DaysOverdue, PaymentHistory

### AuditTrailVM (LedgerService)
**Location**: Line 394 in LedgerService.cs
**Purpose**: Track audit trail information
**Required Properties**: EntityId, EntityType, Action, Timestamp, UserId, OldValues, NewValues, Reason

## Implementation Notes

1. **Namespace Consistency**: All ViewModels should be accessible from their respective services
2. **Property Validation**: Implement proper validation for business rule constraints
3. **Nullability**: Use nullable reference types appropriately for optional properties
4. **Performance**: Consider lazy loading for large collections and complex calculations
5. **Documentation**: Each ViewModel should have XML documentation explaining its purpose 
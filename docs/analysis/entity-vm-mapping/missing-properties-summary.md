# Missing Properties Summary Report

**Analysis Date:** 2025-08-17 22:11:47

## Critical Missing Properties by Entity

### Contract

**Missing Properties:**

- **Id**
  - Referenced in: ContractUpdateVM, ContractDetailVM, ContractListVM, ContractInstallmentSummaryVM, ContractPaymentSummaryVM, CustomerContractSummaryVM

- **CreatedAt**
  - Referenced in: ContractDetailVM, ContractListVM

- **UpdatedAt**
  - Referenced in: ContractDetailVM

- **InstallmentNumber**
  - Referenced in: ContractInstallmentSummaryVM

- **DueDate**
  - Referenced in: ContractInstallmentSummaryVM

- **PaymentDate**
  - Referenced in: ContractPaymentSummaryVM

- **PaymentMethod**
  - Referenced in: ContractPaymentSummaryVM

- **ReferenceNumber**
  - Referenced in: ContractPaymentSummaryVM

### Customer

**Missing Properties:**

- **PhoneType**
  - Referenced in: CustomerCreateVM, CustomerUpdateVM, CustomerDetailVM, CustomerListVM

- **Id**
  - Referenced in: CustomerUpdateVM, CustomerDetailVM, CustomerContractSummaryVM, CustomerListVM

- **CreatedAt**
  - Referenced in: CustomerDetailVM, CustomerListVM

- **UpdatedAt**
  - Referenced in: CustomerDetailVM

- **Status**
  - Referenced in: CustomerContractSummaryVM

- **Currency**
  - Referenced in: CustomerContractSummaryVM

- **CustomerId**
  - Referenced in: CustomerFollowUpPatternVM, CustomerVisitPatternVM, CustomerEngagementReportVM

- **CompletedFollowUps**
  - Referenced in: CustomerFollowUpPatternVM

- **PendingFollowUps**
  - Referenced in: CustomerFollowUpPatternVM

- **AverageFollowUpsPerMonth**
  - Referenced in: CustomerFollowUpPatternVM

- **LastFollowUpDate**
  - Referenced in: CustomerFollowUpPatternVM

- **FollowUpFrequency**
  - Referenced in: CustomerFollowUpPatternVM

- **AverageVisitsPerMonth**
  - Referenced in: CustomerVisitPatternVM

- **LastVisitDate**
  - Referenced in: CustomerVisitPatternVM, CustomerEngagementReportVM

- **VisitFrequency**
  - Referenced in: CustomerVisitPatternVM, CustomerEngagementReportVM

- **PreferredVisitDays**
  - Referenced in: CustomerVisitPatternVM

- **LastPaymentDate**
  - Referenced in: CustomerEngagementReportVM

### FollowUp

**Missing Properties:**

- **Id**
  - Referenced in: FollowUpUpdateVM, FollowUpDetailVM, FollowUpListVM

- **ContractNumber**
  - Referenced in: FollowUpDetailVM, FollowUpListVM

- **CreatedAt**
  - Referenced in: FollowUpDetailVM, FollowUpListVM

- **UpdatedAt**
  - Referenced in: FollowUpDetailVM

- **CompletedFollowUps**
  - Referenced in: FollowUpEffectivenessVM, FollowUpPerformanceVM, CustomerFollowUpPatternVM

- **PendingFollowUps**
  - Referenced in: FollowUpEffectivenessVM, FollowUpPerformanceVM, CustomerFollowUpPatternVM

- **OverdueFollowUps**
  - Referenced in: FollowUpEffectivenessVM, FollowUpPerformanceVM

- **CompletionRate**
  - Referenced in: FollowUpEffectivenessVM

- **AverageCompletionTime**
  - Referenced in: FollowUpEffectivenessVM

- **FromDate**
  - Referenced in: FollowUpPerformanceVM

- **ToDate**
  - Referenced in: FollowUpPerformanceVM

- **HighPriorityFollowUps**
  - Referenced in: FollowUpPerformanceVM

- **FollowUpsByType**
  - Referenced in: FollowUpPerformanceVM

- **FollowUpsByPriority**
  - Referenced in: FollowUpPerformanceVM

- **AverageFollowUpsPerMonth**
  - Referenced in: CustomerFollowUpPatternVM

- **LastFollowUpDate**
  - Referenced in: CustomerFollowUpPatternVM

- **FollowUpFrequency**
  - Referenced in: CustomerFollowUpPatternVM

### Installment

**Missing Properties:**

- **Id**
  - Referenced in: ContractInstallmentSummaryVM, InstallmentUpdateVM, InstallmentDetailVM, InstallmentListVM, InstallmentPaymentSummaryVM

- **ContractNumber**
  - Referenced in: InstallmentDetailVM, InstallmentListVM

- **IsOverdue**
  - Referenced in: InstallmentDetailVM, InstallmentListVM

- **CreatedAt**
  - Referenced in: InstallmentDetailVM

- **UpdatedAt**
  - Referenced in: InstallmentDetailVM

- **PaymentDate**
  - Referenced in: InstallmentPaymentSummaryVM

- **ReferenceNumber**
  - Referenced in: InstallmentPaymentSummaryVM

- **PaidInstallments**
  - Referenced in: InstallmentStatusSummaryVM

- **PendingInstallments**
  - Referenced in: InstallmentStatusSummaryVM

- **OverdueInstallments**
  - Referenced in: InstallmentStatusSummaryVM

- **WaivedInstallments**
  - Referenced in: InstallmentStatusSummaryVM

### LedgerEntry

**Missing Properties:**

- **Id**
  - Referenced in: LedgerEntryUpdateVM, LedgerEntryDetailVM, LedgerEntryListVM

- **ContractNumber**
  - Referenced in: LedgerEntryDetailVM, LedgerEntryListVM

- **CreatedAt**
  - Referenced in: LedgerEntryDetailVM, LedgerEntryListVM

- **UpdatedAt**
  - Referenced in: LedgerEntryDetailVM

### Payment

**Missing Properties:**

- **Id**
  - Referenced in: ContractPaymentSummaryVM, InstallmentPaymentSummaryVM, PaymentHistoryVM, PaymentUpdateVM, PaymentDetailVM, PaymentListVM

- **Status**
  - Referenced in: PaymentHistoryVM

- **FromDate**
  - Referenced in: PaymentTrendsVM

- **ToDate**
  - Referenced in: PaymentTrendsVM

- **PaymentTrendsByMonth**
  - Referenced in: PaymentTrendsVM

- **ContractNumber**
  - Referenced in: PaymentDetailVM, PaymentListVM

- **InstallmentNumber**
  - Referenced in: PaymentDetailVM

- **ReceiptId**
  - Referenced in: PaymentDetailVM

- **ReceiptNumber**
  - Referenced in: PaymentDetailVM

- **CreatedAt**
  - Referenced in: PaymentDetailVM, PaymentListVM

- **UpdatedAt**
  - Referenced in: PaymentDetailVM

### Receipt

**Missing Properties:**

- **Id**
  - Referenced in: ReceiptUpdateVM, ReceiptDetailVM, ReceiptListVM

- **PaymentDate**
  - Referenced in: ReceiptDetailVM

- **CreatedAt**
  - Referenced in: ReceiptDetailVM, ReceiptListVM

- **UpdatedAt**
  - Referenced in: ReceiptDetailVM

### Staff

**Missing Properties:**

- **StaffId**
  - Referenced in: StaffWorkloadVM, StaffPerformanceVM, StaffVisitPerformanceVM

- **PendingFollowUps**
  - Referenced in: StaffWorkloadVM

- **OverdueFollowUps**
  - Referenced in: StaffWorkloadVM

- **HighPriorityFollowUps**
  - Referenced in: StaffWorkloadVM

- **TodayFollowUps**
  - Referenced in: StaffWorkloadVM

- **ThisWeekFollowUps**
  - Referenced in: StaffWorkloadVM

- **PhoneType**
  - Referenced in: StaffCreateVM, StaffUpdateVM, StaffDetailVM, StaffListVM

- **Id**
  - Referenced in: StaffUpdateVM, StaffDetailVM, StaffListVM, StaffRelatedSummaryVM

- **CreatedAt**
  - Referenced in: StaffDetailVM

- **UpdatedAt**
  - Referenced in: StaffDetailVM

- **Reference**
  - Referenced in: StaffRelatedSummaryVM

- **Date**
  - Referenced in: StaffRelatedSummaryVM, StaffActivityVM

- **Description**
  - Referenced in: StaffRelatedSummaryVM, StaffActivityVM, StaffScheduleVM

- **ActivityType**
  - Referenced in: StaffActivityVM, StaffScheduleVM

- **ActivityId**
  - Referenced in: StaffActivityVM, StaffScheduleVM

- **Amount**
  - Referenced in: StaffActivityVM

- **RelatedEntityId**
  - Referenced in: StaffActivityVM

- **RelatedEntityType**
  - Referenced in: StaffActivityVM

- **FromDate**
  - Referenced in: StaffPerformanceVM

- **ToDate**
  - Referenced in: StaffPerformanceVM

- **AveragePaymentAmount**
  - Referenced in: StaffPerformanceVM

- **VisitToPaymentRatio**
  - Referenced in: StaffPerformanceVM

- **FollowUpCompletionRate**
  - Referenced in: StaffPerformanceVM

- **CompletedVisits**
  - Referenced in: StaffVisitPerformanceVM

- **CancelledVisits**
  - Referenced in: StaffVisitPerformanceVM

- **NoShowVisits**
  - Referenced in: StaffVisitPerformanceVM

- **AverageVisitDuration**
  - Referenced in: StaffVisitPerformanceVM

- **VisitsWithPayments**
  - Referenced in: StaffVisitPerformanceVM

- **SuccessRate**
  - Referenced in: StaffVisitPerformanceVM

- **PaymentConversionRate**
  - Referenced in: StaffVisitPerformanceVM

- **StartTime**
  - Referenced in: StaffScheduleVM

- **EndTime**
  - Referenced in: StaffScheduleVM

- **Location**
  - Referenced in: StaffScheduleVM

- **Status**
  - Referenced in: StaffScheduleVM

### Visit

**Missing Properties:**

- **DurationHours**
  - Referenced in: VisitCreateVM, VisitUpdateVM, VisitDetailVM, VisitListVM

- **DurationMinutes**
  - Referenced in: VisitCreateVM, VisitUpdateVM, VisitDetailVM, VisitListVM

- **Id**
  - Referenced in: VisitUpdateVM, VisitDetailVM, VisitListVM

- **CreatedAt**
  - Referenced in: VisitDetailVM, VisitListVM

- **UpdatedAt**
  - Referenced in: VisitDetailVM

- **VisitsWithPayments**
  - Referenced in: VisitEffectivenessVM, StaffVisitPerformanceVM

- **ConversionRate**
  - Referenced in: VisitEffectivenessVM

- **AverageVisitsPerMonth**
  - Referenced in: CustomerVisitPatternVM

- **LastVisitDate**
  - Referenced in: CustomerVisitPatternVM

- **VisitFrequency**
  - Referenced in: CustomerVisitPatternVM

- **PreferredVisitDays**
  - Referenced in: CustomerVisitPatternVM

- **FromDate**
  - Referenced in: VisitOutcomeAnalysisVM, VisitSummaryReportVM

- **ToDate**
  - Referenced in: VisitOutcomeAnalysisVM, VisitSummaryReportVM

- **SuccessfulVisits**
  - Referenced in: VisitOutcomeAnalysisVM

- **FollowUpRequired**
  - Referenced in: VisitOutcomeAnalysisVM

- **NoShowVisits**
  - Referenced in: VisitOutcomeAnalysisVM, StaffVisitPerformanceVM, VisitSummaryReportVM

- **OutcomeBreakdown**
  - Referenced in: VisitOutcomeAnalysisVM

- **CompletedVisits**
  - Referenced in: StaffVisitPerformanceVM, VisitSummaryReportVM, TerritoryVisitReportVM

- **CancelledVisits**
  - Referenced in: StaffVisitPerformanceVM, VisitSummaryReportVM, TerritoryVisitReportVM

- **AverageVisitDuration**
  - Referenced in: StaffVisitPerformanceVM, VisitSummaryReportVM

- **SuccessRate**
  - Referenced in: StaffVisitPerformanceVM

- **PaymentConversionRate**
  - Referenced in: StaffVisitPerformanceVM

- **Order**
  - Referenced in: VisitRouteVM

- **Address**
  - Referenced in: VisitRouteVM

- **EstimatedDuration**
  - Referenced in: VisitRouteVM

- **EstimatedTravelTime**
  - Referenced in: VisitRouteVM

- **ExistingVisitId**
  - Referenced in: VisitConflictVM

- **ExistingVisitTime**
  - Referenced in: VisitConflictVM

- **ExistingVisitDuration**
  - Referenced in: VisitConflictVM

- **ProposedVisitTime**
  - Referenced in: VisitConflictVM

- **ProposedVisitDuration**
  - Referenced in: VisitConflictVM

- **ConflictType**
  - Referenced in: VisitConflictVM

- **VisitsByType**
  - Referenced in: VisitSummaryReportVM

- **Territory**
  - Referenced in: TerritoryVisitReportVM

- **PendingVisits**
  - Referenced in: TerritoryVisitReportVM

- **VisitsByStaff**
  - Referenced in: TerritoryVisitReportVM

- **AverageVisitsPerDay**
  - Referenced in: TerritoryVisitReportVM

## Priority Recommendations

### High Priority (Critical for Functionality)
- Properties referenced in Create/Update ViewModels
- Properties used in business logic or validation

### Medium Priority (UI Enhancement)
- Properties used in Detail/List ViewModels
- Properties for display formatting

### Low Priority (Nice to Have)
- Properties used only in analytics ViewModels
- Properties for advanced filtering

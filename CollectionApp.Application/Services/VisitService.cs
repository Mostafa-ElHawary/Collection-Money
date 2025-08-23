using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoMapper;
using CollectionApp.Application.Common;
using CollectionApp.Application.Interfaces;
using CollectionApp.Application.ViewModels;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Domain.ValueObjects;

namespace CollectionApp.Application.Services
{
    public class VisitService : IVisitService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IFollowUpService _followUpService;

        public VisitService(IUnitOfWork unitOfWork, IMapper mapper, IFollowUpService followUpService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _followUpService = followUpService ?? throw new ArgumentNullException(nameof(followUpService));
        }

        public async Task<VisitDetailVM> CreateAsync(VisitCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Validate customer exists
            var customer = await _unitOfWork.Customers.GetByIdAsync(model.CustomerId);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {model.CustomerId} not found.");

            // Validate staff exists
            var staff = await _unitOfWork.Staff.GetByIdAsync(model.StaffId);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {model.StaffId} not found.");

            // Validate visit date is reasonable
            if (model.VisitDate < DateTime.Today.AddDays(-30) || model.VisitDate > DateTime.Today.AddDays(90))
                throw new InvalidOperationException("Visit date must be within reasonable range (not more than 30 days in past or 90 days in future).");

            // Create TimeSpan value object
            var duration = model.Duration ?? new TimeSpan(model.DurationHours, model.DurationMinutes, 0);

            // Create visit entity
            var visit = new Visit(
                model.CustomerId,
                model.StaffId,
                model.VisitDate,
                duration,
                model.VisitType,
                model.Location,
                model.Purpose,
                model.Notes
            );

            await _unitOfWork.Visits.AddAsync(visit);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VisitDetailVM>(visit);
        }

        public async Task<VisitDetailVM> UpdateAsync(VisitUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var visit = await _unitOfWork.Visits.GetByIdAsync(model.Id);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {model.Id} not found.");

            // Check if visit can be updated (not completed)
            if (visit.Status == VisitStatus.Completed)
                throw new InvalidOperationException("Cannot update completed visit.");

            // Update visit details
            var hours = model.DurationHours ?? (visit.Duration.HasValue ? visit.Duration.Value.Hours : 0);
            var minutes = model.DurationMinutes ?? (visit.Duration.HasValue ? visit.Duration.Value.Minutes : 0);
            var duration = model.Duration ?? new TimeSpan(hours, minutes, 0);
            visit.UpdateDetails(model.VisitDate ?? visit.VisitDate, duration, model.VisitType, model.Location, model.Purpose, model.Notes);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VisitDetailVM>(visit);
        }

        public async Task<VisitDetailVM> GetByIdAsync(Guid id)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(id);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {id} not found.");

            return _mapper.Map<VisitDetailVM>(visit);
        }

        public async Task<PagedResult<VisitListVM>> GetPagedAsync(
            string filter = null,
            string orderBy = "VisitDate",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var visits = await _unitOfWork.Visits.GetPagedAsync(filter, orderBy, pageNumber, pageSize);
            
            var visitListVMs = _mapper.Map<List<VisitListVM>>(visits.Items);
            
            return new PagedResult<VisitListVM>(visitListVMs.AsReadOnly(), visits.TotalCount, visits.PageNumber, visits.PageSize);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(id);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {id} not found.");

            // Check if visit can be deleted (not completed)
            if (visit.Status == VisitStatus.Completed)
                throw new InvalidOperationException("Cannot delete completed visit.");

            await _unitOfWork.Visits.DeleteAsync(visit);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<VisitDetailVM> ScheduleVisitAsync(VisitCreateVM model)
        {
            // ScheduleVisitAsync is essentially the same as CreateAsync for scheduled visits
            return await CreateAsync(model);
        }

        public async Task<VisitDetailVM> RecordVisitAsync(VisitCreateVM model)
        {
            // For recorded visits, set the visit date to current time if not specified
            if (model.VisitDate == default)
            {
                model.VisitDate = DateTime.Now;
            }

            var visit = await CreateAsync(model);
            
            // Mark as completed immediately
            var visitEntity = await _unitOfWork.Visits.GetByIdAsync(visit.Id);
            visitEntity.Complete();
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VisitDetailVM>(visitEntity);
        }

        public async Task<VisitDetailVM> UpdateVisitOutcomeAsync(Guid visitId, string outcome, DateTime? nextVisitDate)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(visitId);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {visitId} not found.");

            visit.UpdateOutcome(outcome, nextVisitDate);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VisitDetailVM>(visit);
        }

        public async Task<VisitDetailVM> CancelVisitAsync(Guid visitId, string reason)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(visitId);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {visitId} not found.");

            visit.Cancel(reason);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VisitDetailVM>(visit);
        }

        public async Task<VisitDetailVM> RescheduleVisitAsync(Guid visitId, DateTime newDate)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(visitId);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {visitId} not found.");

            // Validate new date is reasonable
            if (newDate < DateTime.Today.AddDays(-30) || newDate > DateTime.Today.AddDays(90))
                throw new InvalidOperationException("Visit date must be within reasonable range (not more than 30 days in past or 90 days in future).");

            visit.Reschedule(newDate);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<VisitDetailVM>(visit);
        }

        public async Task<List<VisitListVM>> GetVisitsByCustomerAsync(Guid customerId)
        {
            var visits = await _unitOfWork.Visits.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<VisitListVM>>(visits);
        }

        public async Task<List<VisitListVM>> GetVisitsByStaffAsync(Guid staffId)
        {
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            return _mapper.Map<List<VisitListVM>>(visits);
        }

        public async Task<List<VisitListVM>> GetScheduledVisitsAsync(DateTime fromDate, DateTime toDate)
        {
            var visits = await _unitOfWork.Visits.GetScheduledAsync(fromDate, toDate);
            return _mapper.Map<List<VisitListVM>>(visits);
        }

        public async Task<List<VisitListVM>> GetCompletedVisitsAsync(DateTime fromDate, DateTime toDate)
        {
            var visits = await _unitOfWork.Visits.GetCompletedAsync(fromDate, toDate);
            return _mapper.Map<List<VisitListVM>>(visits);
        }

        public async Task<List<VisitListVM>> GetOverdueVisitsAsync()
        {
            var visits = await _unitOfWork.Visits.GetOverdueAsync();
            return _mapper.Map<List<VisitListVM>>(visits);
        }

        public async Task<VisitAnalyticsViewModels.VisitEffectivenessVM> GetVisitEffectivenessAsync(Guid staffId)
        {
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            var payments = await _unitOfWork.Payments.GetByStaffIdAsync(staffId);

            var effectiveness = new VisitAnalyticsViewModels.VisitEffectivenessVM
            {
                StaffId = staffId,
                TotalVisits = visits.Count,
                VisitsWithPayments = 0,
                TotalAmountFromVisits = 0
            };

            foreach (var visit in visits)
            {
                // Get contracts for the customer
                var contracts = await _unitOfWork.Contracts.GetByCustomerIdAsync(visit.CustomerId);
                var contractIds = contracts.Select(c => c.Id).ToHashSet();
                
                // Filter payments by contract IDs
                var customerPayments = payments.Where(p => contractIds.Contains(p.ContractId)).ToList();
                if (customerPayments.Any())
                {
                    effectiveness.VisitsWithPayments++;
                    effectiveness.TotalAmountFromVisits += customerPayments.Sum(p => p.Amount.Amount);
                }
            }

            effectiveness.ConversionRate = visits.Any() ? 
                (decimal)effectiveness.VisitsWithPayments / visits.Count * 100 : 0;

            return effectiveness;
        }

        public async Task<VisitAnalyticsViewModels.CustomerVisitPatternVM> GetCustomerVisitPatternAsync(Guid customerId)
        {
            var visits = await _unitOfWork.Visits.GetByCustomerIdAsync(customerId);
            
            var pattern = new VisitAnalyticsViewModels.CustomerVisitPatternVM
            {
                CustomerId = customerId,
                TotalVisits = visits.Count,
                AverageVisitsPerMonth = visits.Any() ? 
                    (decimal)visits.Count / Math.Max(1, (DateTime.Now - visits.Min(v => v.VisitDate)).Days / 30) : 0,
                LastVisitDate = visits.Any() ? visits.Max(v => v.VisitDate) : null,
                VisitFrequency = visits.Count > 1 ? 
                    visits.OrderBy(v => v.VisitDate)
                          .ToList()
                          .Zip(visits.OrderBy(v => v.VisitDate).Skip(1), (a, b) => (b.VisitDate - a.VisitDate).Days)
                          .Where(d => d > 0)
                          .Average() : 0,
                PreferredVisitDays = visits.GroupBy(v => v.VisitDate.DayOfWeek)
                                         .OrderByDescending(g => g.Count())
                                         .Select(g => g.Key.ToString())
                                         .ToList()
            };

            return pattern;
        }

        public async Task<VisitAnalyticsViewModels.VisitOutcomeAnalysisVM> GetVisitOutcomeAnalysisAsync(DateTime fromDate, DateTime toDate)
        {
            var visits = await _unitOfWork.Visits.GetCompletedAsync(fromDate, toDate);
            
            var analysis = new VisitAnalyticsViewModels.VisitOutcomeAnalysisVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalVisits = visits.Count,
                SuccessfulVisits = visits.Count(v => v.Outcome?.Contains("successful") == true || v.Outcome?.Contains("payment") == true),
                FollowUpRequired = visits.Count(v => v.Outcome?.Contains("follow-up") == true || v.Outcome?.Contains("reschedule") == true),
                NoShowVisits = visits.Count(v => v.Outcome?.Contains("no-show") == true || v.Outcome?.Contains("cancelled") == true),
                OutcomeBreakdown = visits.GroupBy(v => v.Outcome ?? "Unknown")
                                       .ToDictionary(g => g.Key, g => g.Count())
            };

            return analysis;
        }

        public async Task<VisitAnalyticsViewModels.StaffVisitPerformanceVM> GetStaffVisitPerformanceAsync(Guid staffId)
        {
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            var payments = await _unitOfWork.Payments.GetByStaffIdAsync(staffId);

            var performance = new VisitAnalyticsViewModels.StaffVisitPerformanceVM
            {
                StaffId = staffId,
                TotalVisits = visits.Count,
                CompletedVisits = visits.Count(v => v.Status == VisitStatus.Completed),
                CancelledVisits = visits.Count(v => v.Status == VisitStatus.Cancelled),
                NoShowVisits = visits.Count(v => v.Status == VisitStatus.NoShow),
                AverageVisitDuration = visits.Any() ? 
                    visits.Average(v => v.Duration?.TotalHours ?? 0) : 0,
                VisitsWithPayments = 0,
                TotalAmountCollected = 0
            };

            foreach (var visit in visits)
            {
                // Get contracts for the customer
                var contracts = await _unitOfWork.Contracts.GetByCustomerIdAsync(visit.CustomerId);
                var contractIds = contracts.Select(c => c.Id).ToHashSet();
                
                // Filter payments by contract IDs
                var customerPayments = payments.Where(p => contractIds.Contains(p.ContractId)).ToList();
                if (customerPayments.Any())
                {
                    performance.VisitsWithPayments++;
                    performance.TotalAmountCollected += customerPayments.Sum(p => p.Amount.Amount);
                }
            }

            performance.SuccessRate = visits.Any() ? 
                (decimal)performance.CompletedVisits / visits.Count * 100 : 0;
            performance.PaymentConversionRate = visits.Any() ? 
                (decimal)performance.VisitsWithPayments / visits.Count * 100 : 0;

            return performance;
        }

        public async Task<bool> CreateFollowUpFromVisitAsync(Guid visitId, FollowUpCreateVM followUp)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(visitId);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {visitId} not found.");

            // Create follow-up service and create follow-up
            await _followUpService.CreateAsync(followUp);

            return true;
        }

        public async Task<bool> LinkVisitToPaymentAsync(Guid visitId, Guid paymentId)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(visitId);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {visitId} not found.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {paymentId} not found.");

            visit.LinkToPayment(paymentId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<decimal> GetVisitROIAsync(Guid visitId)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(visitId);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {visitId} not found.");

            // Calculate ROI based on visit cost vs. payments received
            var visitCost = 50m; // Assume fixed cost per visit
            var payments = await _unitOfWork.Payments.GetByCustomerIdAsync(visit.CustomerId);
            var totalPayments = payments.Sum(p => p.Amount.Amount);

            return totalPayments - visitCost;
        }

        public async Task<List<VisitAnalyticsViewModels.VisitRouteVM>> PlanVisitRouteAsync(Guid staffId, DateTime date, List<Guid> customerIds)
        {
            var route = new List<VisitAnalyticsViewModels.VisitRouteVM>();
            var customers = new List<Customer>();

            // Get customer details
            foreach (var customerId in customerIds)
            {
                var customer = await _unitOfWork.Customers.GetByIdAsync(customerId);
                if (customer != null)
                {
                    customers.Add(customer);
                }
            }

            // Simple route planning - order by distance (assuming coordinates are available)
            // In a real implementation, you would use a routing algorithm
            var orderedCustomers = customers.OrderBy(c => c.Address?.City).ToList();

            for (int i = 0; i < orderedCustomers.Count; i++)
            {
                var customer = orderedCustomers[i];
                route.Add(new VisitAnalyticsViewModels.VisitRouteVM
                {
                    Order = i + 1,
                    CustomerId = customer.Id,
                    CustomerName = customer.FullName(),
                    Address = customer.Address?.ToString(),
                    EstimatedDuration = TimeSpan.FromHours(1),
                    EstimatedTravelTime = i > 0 ? TimeSpan.FromMinutes(30) : TimeSpan.Zero
                });
            }

            return route;
        }

        public async Task<List<VisitAnalyticsViewModels.StaffScheduleVM>> GetStaffScheduleAsync(Guid staffId, DateTime date)
        {
            var schedule = new List<VisitAnalyticsViewModels.StaffScheduleVM>();

            // Get scheduled visits
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            var dayVisits = visits.Where(v => v.VisitDate.Date == date.Date);
            foreach (var visit in dayVisits)
            {
                schedule.Add(new VisitAnalyticsViewModels.StaffScheduleVM
                {
                    ActivityType = StaffActivityType.Visit,
                    ActivityId = visit.Id,
                    Description = $"Visit to {(visit.Customer != null ? visit.Customer.FullName() : "Unknown Customer")}",
                    StartTime = visit.VisitDate,
                    EndTime = visit.VisitDate.Add(visit.Duration ?? TimeSpan.Zero),
                    Location = visit.Location,
                    Status = visit.Status.ToString()
                });
            }

            return schedule.OrderBy(s => s.StartTime).ToList();
        }

        public async Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(Guid staffId, DateTime date)
        {
            var availableSlots = new List<TimeSpan>();
            var workStart = new TimeSpan(9, 0, 0); // 9:00 AM
            var workEnd = new TimeSpan(17, 0, 0); // 5:00 PM
            var slotDuration = TimeSpan.FromHours(1);

            // Get existing visits for the day
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            var dayVisits = visits.Where(v => v.VisitDate.Date == date.Date).ToList();

            var currentTime = workStart;
            while (currentTime < workEnd)
            {
                var slotEnd = currentTime.Add(slotDuration);
                var isAvailable = !dayVisits.Any(v => 
                    v.VisitDate.TimeOfDay < slotEnd && 
                    v.VisitDate.Add(v.Duration ?? TimeSpan.Zero).TimeOfDay > currentTime);

                if (isAvailable)
                {
                    availableSlots.Add(currentTime);
                }

                currentTime = currentTime.Add(slotDuration);
            }

            return availableSlots;
        }

        public async Task<bool> BulkScheduleVisitsAsync(List<VisitCreateVM> visits)
        {
            foreach (var visit in visits)
            {
                await CreateAsync(visit);
            }

            return true;
        }

        public async Task<List<VisitAnalyticsViewModels.VisitConflictVM>> GetVisitConflictsAsync(Guid staffId, DateTime date, TimeSpan duration)
        {
            var conflicts = new List<VisitAnalyticsViewModels.VisitConflictVM>();
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            var dayVisits = visits.Where(v => v.VisitDate.Date == date.Date).ToList();

            foreach (var visit in dayVisits)
            {
                var visitEnd = visit.VisitDate.Add(visit.Duration ?? TimeSpan.Zero);
                var proposedEnd = date.Add(duration);

                if ((date < visitEnd && proposedEnd > visit.VisitDate))
                {
                    conflicts.Add(new VisitAnalyticsViewModels.VisitConflictVM
                    {
                        ExistingVisitId = visit.Id,
                        ExistingVisitTime = visit.VisitDate,
                        ExistingVisitDuration = visit.Duration ?? TimeSpan.Zero,
                        ProposedVisitTime = date,
                        ProposedVisitDuration = duration,
                        ConflictType = "Time Overlap"
                    });
                }
            }

            return conflicts;
        }

        public async Task<VisitAnalyticsViewModels.VisitSummaryReportVM> GetVisitSummaryReportAsync(DateTime fromDate, DateTime toDate)
        {
            var visits = await _unitOfWork.Visits.GetCompletedAsync(fromDate, toDate);
            
            var report = new VisitAnalyticsViewModels.VisitSummaryReportVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalVisits = visits.Count,
                CompletedVisits = visits.Count(v => v.Status == VisitStatus.Completed),
                CancelledVisits = visits.Count(v => v.Status == VisitStatus.Cancelled),
                NoShowVisits = visits.Count(v => v.Status == VisitStatus.NoShow),
                TotalVisitHours = (decimal)(visits.Sum(v => v.Duration?.TotalHours ?? 0)),
                AverageVisitDuration = visits.Any() ? visits.Average(v => v.Duration?.TotalHours ?? 0) : 0,
                VisitsByType = visits.GroupBy(v => v.VisitType ?? "Unknown")
                                   .ToDictionary(g => g.Key, g => g.Count())
            };

            return report;
        }

        public async Task<VisitAnalyticsViewModels.CustomerEngagementReportVM> GetCustomerEngagementReportAsync(Guid customerId)
        {
            var visits = await _unitOfWork.Visits.GetByCustomerIdAsync(customerId);
            var payments = await _unitOfWork.Payments.GetByCustomerIdAsync(customerId);
            
            var report = new VisitAnalyticsViewModels.CustomerEngagementReportVM
            {
                CustomerId = customerId,
                TotalVisits = visits.Count,
                TotalPayments = payments.Count,
                TotalAmountPaid = payments.Sum(p => p.Amount.Amount),
                LastVisitDate = visits.Any() ? visits.Max(v => v.VisitDate) : null,
                LastPaymentDate = payments.Any() ? payments.Max(p => p.PaymentDate) : null,
                VisitFrequency = visits.Count > 1 ? 
                    (visits.OrderBy(v => v.VisitDate).ToList().Zip(
                        visits.OrderBy(v => v.VisitDate).Skip(1).ToList(), 
                        (a, b) => (b.VisitDate - a.VisitDate).Days)
                        .Where(d => d > 0)
                        .DefaultIfEmpty(0)
                        .Average()) : 0
            };

            return report;
        }

        public async Task<VisitAnalyticsViewModels.TerritoryVisitReportVM> GetTerritoryVisitReportAsync(string territory)
        {
            var visits = await _unitOfWork.Visits.GetByTerritoryAsync(territory);
            
            var report = new VisitAnalyticsViewModels.TerritoryVisitReportVM
            {
                Territory = territory,
                TotalVisits = visits.Count,
                CompletedVisits = visits.Count(v => v.Status == VisitStatus.Completed),
                PendingVisits = visits.Count(v => v.Status == VisitStatus.Scheduled),
                CancelledVisits = visits.Count(v => v.Status == VisitStatus.Cancelled),
                VisitsByStaff = visits.GroupBy(v => v.StaffId)
                                    .ToDictionary(g => g.Key, g => g.Count()),
                AverageVisitsPerDay = visits.Any() ? 
                    (decimal)visits.Count / Math.Max(1m, ((DateTime.Now - visits.Min(v => v.VisitDate)).Days) / 30m) : 0
            };

            return report;
        }
    }

    public interface IVisitService
    {
        Task<VisitDetailVM> CreateAsync(VisitCreateVM model);
        Task<VisitDetailVM> UpdateAsync(VisitUpdateVM model);
        Task<VisitDetailVM> GetByIdAsync(Guid id);
        Task<PagedResult<VisitListVM>> GetPagedAsync(string filter = null, string orderBy = "VisitDate", int pageNumber = 1, int pageSize = 10);
        Task<bool> DeleteAsync(Guid id);
        Task<VisitDetailVM> ScheduleVisitAsync(VisitCreateVM model);
        Task<VisitDetailVM> RecordVisitAsync(VisitCreateVM model);
        Task<VisitDetailVM> UpdateVisitOutcomeAsync(Guid visitId, string outcome, DateTime? nextVisitDate);
        Task<VisitDetailVM> CancelVisitAsync(Guid visitId, string reason);
        Task<VisitDetailVM> RescheduleVisitAsync(Guid visitId, DateTime newDate);
        Task<List<VisitListVM>> GetVisitsByCustomerAsync(Guid customerId);
        Task<List<VisitListVM>> GetVisitsByStaffAsync(Guid staffId);
        Task<List<VisitListVM>> GetScheduledVisitsAsync(DateTime fromDate, DateTime toDate);
        Task<List<VisitListVM>> GetCompletedVisitsAsync(DateTime fromDate, DateTime toDate);
        Task<List<VisitListVM>> GetOverdueVisitsAsync();
        Task<VisitAnalyticsViewModels.VisitEffectivenessVM> GetVisitEffectivenessAsync(Guid staffId);
        Task<VisitAnalyticsViewModels.CustomerVisitPatternVM> GetCustomerVisitPatternAsync(Guid customerId);
        Task<VisitAnalyticsViewModels.VisitOutcomeAnalysisVM> GetVisitOutcomeAnalysisAsync(DateTime fromDate, DateTime toDate);
        Task<VisitAnalyticsViewModels.StaffVisitPerformanceVM> GetStaffVisitPerformanceAsync(Guid staffId);
        Task<bool> CreateFollowUpFromVisitAsync(Guid visitId, FollowUpCreateVM followUp);
        Task<bool> LinkVisitToPaymentAsync(Guid visitId, Guid paymentId);
        Task<decimal> GetVisitROIAsync(Guid visitId);
        Task<List<VisitAnalyticsViewModels.VisitRouteVM>> PlanVisitRouteAsync(Guid staffId, DateTime date, List<Guid> customerIds);
        Task<List<VisitAnalyticsViewModels.StaffScheduleVM>> GetStaffScheduleAsync(Guid staffId, DateTime date);
        Task<List<TimeSpan>> GetAvailableTimeSlotsAsync(Guid staffId, DateTime date);
        Task<bool> BulkScheduleVisitsAsync(List<VisitCreateVM> visits);
        Task<List<VisitAnalyticsViewModels.VisitConflictVM>> GetVisitConflictsAsync(Guid staffId, DateTime date, TimeSpan duration);
        Task<VisitAnalyticsViewModels.VisitSummaryReportVM> GetVisitSummaryReportAsync(DateTime fromDate, DateTime toDate);
        Task<VisitAnalyticsViewModels.CustomerEngagementReportVM> GetCustomerEngagementReportAsync(Guid customerId);
        Task<VisitAnalyticsViewModels.TerritoryVisitReportVM> GetTerritoryVisitReportAsync(string territory);
    }
}
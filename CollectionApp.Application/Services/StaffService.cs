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
    public class StaffService : IStaffService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public StaffService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<StaffDetailVM> CreateAsync(StaffCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Check for duplicate employee ID
            var existingStaff = await _unitOfWork.Staff.GetByEmployeeIdAsync(model.EmployeeId);
            if (existingStaff != null)
                throw new InvalidOperationException($"Staff with Employee ID {model.EmployeeId} already exists.");

            // Check for duplicate email
            var existingEmailStaff = await _unitOfWork.Staff.GetByEmailAsync(model.Email);
            if (existingEmailStaff != null)
                throw new InvalidOperationException($"Staff with email {model.Email} already exists.");

            // Create phone value object
            var phone = new Phone(model.PhoneNumber, model.PhoneType);

            // Create staff entity
            var staff = new Staff(
                model.EmployeeId,
                model.FirstName,
                model.LastName,
                model.Email,
                phone,
                model.Position,
                model.Department,
                model.HireDate,
                model.Salary,
                model.IsActive,
                model.Permissions?.ToList() ?? new List<string>(),
                model.Notes
            );

            await _unitOfWork.Staff.AddAsync(staff);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StaffDetailVM>(staff);
        }

        public async Task<StaffDetailVM> UpdateAsync(StaffUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var staff = await _unitOfWork.Staff.GetByIdAsync(model.Id);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {model.Id} not found.");

            // Employee ID cannot be changed in update operation

            // Check for duplicate email if changed
            if (staff.Email != model.Email)
            {
                var existingEmailStaff = await _unitOfWork.Staff.GetByEmailAsync(model.Email);
                if (existingEmailStaff != null)
                    throw new InvalidOperationException($"Staff with email {model.Email} already exists.");
            }

            // Update staff information
            staff.UpdatePersonalInfo(model.FirstName, model.LastName, model.Position, model.Department);
            staff.UpdateContactInfo(model.Email, new Phone(model.PhoneNumber, model.PhoneType));
            staff.UpdateEmploymentInfo(model.HireDate ?? DateTime.Now, model.Salary, model.IsActive);
            staff.UpdatePermissions(model.Permissions?.ToList() ?? new List<string>());
            staff.UpdateNotes(model.Notes);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StaffDetailVM>(staff);
        }

        public async Task<StaffDetailVM> GetByIdAsync(Guid id)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(id);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {id} not found.");

            return _mapper.Map<StaffDetailVM>(staff);
        }

        public async Task<PagedResult<StaffListVM>> GetPagedAsync(
            string filter = null,
            string orderBy = "LastName",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var staff = await _unitOfWork.Staff.GetPagedAsync(filter, orderBy, pageNumber, pageSize);
            
            var staffListVMs = _mapper.Map<List<StaffListVM>>(staff.Items);
            
            return new PagedResult<StaffListVM>(staffListVMs.AsReadOnly(), staff.TotalCount, staff.PageNumber, staff.PageSize);
        }

        public async Task<bool> DeactivateAsync(Guid id)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(id);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {id} not found.");

            staff.Deactivate();
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<StaffListVM>> GetActiveStaffAsync()
        {
            var staff = await _unitOfWork.Staff.GetActiveAsync();
            return _mapper.Map<List<StaffListVM>>(staff);
        }

        public async Task<List<StaffListVM>> GetStaffByDepartmentAsync(string department)
        {
            var staff = await _unitOfWork.Staff.GetByDepartmentAsync(department);
            return _mapper.Map<List<StaffListVM>>(staff);
        }

        public async Task<List<StaffListVM>> GetStaffByPositionAsync(string position)
        {
            var staff = await _unitOfWork.Staff.GetByPositionAsync(position);
            return _mapper.Map<List<StaffListVM>>(staff);
        }

        public async Task<StaffDetailVM> UpdateStaffStatusAsync(Guid id, bool isActive)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(id);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {id} not found.");

            if (isActive)
                staff.Activate();
            else
                staff.Deactivate();

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StaffDetailVM>(staff);
        }

        public async Task<List<StaffListVM>> SearchStaffAsync(string searchTerm)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<StaffListVM>();

            var staff = await _unitOfWork.Staff.SearchAsync(searchTerm);
            return _mapper.Map<List<StaffListVM>>(staff);
        }

        public async Task<List<StaffAnalyticsViewModels.StaffActivityVM>> GetStaffActivitiesAsync(Guid staffId, DateTime fromDate, DateTime toDate)
        {
            var activities = new List<StaffAnalyticsViewModels.StaffActivityVM>();

            // Get payments processed by staff
            var payments = await _unitOfWork.Payments.GetByStaffIdAsync(staffId, fromDate, toDate);
            foreach (var payment in payments)
            {
                activities.Add(new StaffAnalyticsViewModels.StaffActivityVM
                {
                    ActivityType = StaffActivityType.Payment,
                    ActivityId = payment.Id,
                    Description = $"Processed payment {payment.ReferenceNumber}",
                    Amount = payment.Amount.Amount,
                    Date = payment.PaymentDate,
                    RelatedEntityId = payment.ContractId,
                    RelatedEntityType = RelatedEntityType.Contract
                });
            }

            // Get visits conducted by staff
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId, fromDate, toDate);
            foreach (var visit in visits)
            {
                activities.Add(new StaffAnalyticsViewModels.StaffActivityVM
                {
                    ActivityType = StaffActivityType.Visit,
                    ActivityId = visit.Id,
                    Description = $"Conducted visit to {(visit.Customer != null ? visit.Customer.FullName() : "Unknown Customer")}",
                    Amount = 0,
                    Date = visit.VisitDate,
                    RelatedEntityId = visit.CustomerId,
                    RelatedEntityType = RelatedEntityType.Customer
                });
            }

            // Get follow-ups assigned to staff
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId, fromDate, toDate);
            foreach (var followUp in followUps)
            {
                activities.Add(new StaffAnalyticsViewModels.StaffActivityVM
                {
                    ActivityType = StaffActivityType.FollowUp,
                    ActivityId = followUp.Id,
                    Description = $"Follow-up: {followUp.Description}",
                    Amount = 0,
                    Date = followUp.ScheduledDate,
                    RelatedEntityId = followUp.CustomerId,
                    RelatedEntityType = RelatedEntityType.Customer
                });
            }

            return activities.OrderByDescending(a => a.Date).ToList();
        }

        public async Task<List<PaymentListVM>> GetStaffPaymentsAsync(Guid staffId)
        {
            var payments = await _unitOfWork.Payments.GetByStaffIdAsync(staffId);
            return _mapper.Map<List<PaymentListVM>>(payments);
        }

        public async Task<List<VisitListVM>> GetStaffVisitsAsync(Guid staffId)
        {
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            return _mapper.Map<List<VisitListVM>>(visits);
        }

        public async Task<List<FollowUpListVM>> GetStaffFollowUpsAsync(Guid staffId)
        {
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId);
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<List<ReceiptListVM>> GetStaffReceiptsAsync(Guid staffId)
        {
            var receipts = await _unitOfWork.Receipts.GetByStaffIdAsync(staffId);
            return _mapper.Map<List<ReceiptListVM>>(receipts);
        }

        public async Task<StaffAnalyticsViewModels.StaffPerformanceVM> GetStaffPerformanceAsync(Guid staffId, DateTime fromDate, DateTime toDate)
        {
            var payments = await _unitOfWork.Payments.GetByStaffIdAsync(staffId, fromDate, toDate);
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId, fromDate, toDate);
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId, fromDate, toDate);

            var performance = new StaffAnalyticsViewModels.StaffPerformanceVM
            {
                StaffId = staffId,
                FromDate = fromDate,
                ToDate = toDate,
                TotalPaymentsProcessed = payments.Count,
                TotalAmountCollected = payments.Sum(p => p.Amount.Amount),
                TotalVisitsConducted = visits.Count,
                TotalFollowUpsCompleted = followUps.Count(f => f.Status == "Completed"),
                AveragePaymentAmount = payments.Any() ? payments.Average(p => p.Amount.Amount) : 0,
                VisitToPaymentRatio = visits.Any() ? (decimal)payments.Count / visits.Count * 100 : 0,
                FollowUpCompletionRate = followUps.Any() ? (decimal)followUps.Count(f => f.Status == "Completed") / followUps.Count * 100 : 0
            };

            return performance;
        }

        public async Task<StaffAnalyticsViewModels.CollectionPerformanceVM> GetCollectionPerformanceAsync(Guid staffId)
        {
            var payments = await _unitOfWork.Payments.GetByStaffIdAsync(staffId);
            var contracts = new HashSet<Guid>(payments.Select(p => p.ContractId));

            var performance = new StaffAnalyticsViewModels.CollectionPerformanceVM
            {
                StaffId = staffId,
                TotalContractsHandled = contracts.Count,
                TotalPaymentsProcessed = payments.Count,
                TotalAmountCollected = payments.Sum(p => p.Amount.Amount),
                AveragePaymentAmount = payments.Any() ? payments.Average(p => p.Amount.Amount) : 0,
                PaymentFrequency = payments.Any() ? payments.GroupBy(p => p.ContractId).Average(g => g.Count()) : 0
            };

            return performance;
        }

        public async Task<VisitAnalyticsViewModels.VisitEffectivenessVM> GetVisitEffectivenessAsync(Guid staffId)
        {
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);
            var payments = await _unitOfWork.Payments.GetByStaffIdAsync(staffId);

            // Group visits by customer and check if payments followed
            var visitEffectiveness = new VisitAnalyticsViewModels.VisitEffectivenessVM
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
                    visitEffectiveness.VisitsWithPayments++;
                    visitEffectiveness.TotalAmountFromVisits += customerPayments.Sum(p => p.Amount.Amount);
                }
            }

            visitEffectiveness.ConversionRate = visits.Any() ? 
                (decimal)visitEffectiveness.VisitsWithPayments / visits.Count * 100 : 0;

            return visitEffectiveness;
        }

        public async Task<StaffAnalyticsViewModels.WorkloadAnalysisVM> GetWorkloadAnalysisAsync(Guid staffId)
        {
            var currentDate = DateTime.Today;
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId);
            var visits = await _unitOfWork.Visits.GetByStaffIdAsync(staffId);

            var workload = new StaffAnalyticsViewModels.WorkloadAnalysisVM
            {
                StaffId = staffId,
                CurrentDate = currentDate,
                PendingFollowUps = followUps.Count(f => f.Status == "Pending" && f.ScheduledDate >= currentDate),
                OverdueFollowUps = followUps.Count(f => f.Status == "Pending" && f.ScheduledDate < currentDate),
                ScheduledVisits = visits.Count(v => v.VisitDate >= currentDate),
                CompletedVisitsToday = visits.Count(v => v.VisitDate.Date == currentDate.Date && v.Status == VisitStatus.Completed),
                TotalAssignments = followUps.Count(f => f.Status == "Pending") + visits.Count(v => v.VisitDate >= currentDate)
            };

            return workload;
        }

        public async Task<bool> ValidateStaffPermissionsAsync(Guid staffId, string operation)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId);
            if (staff == null)
                return false;

            return staff.HasPermission(operation);
        }

        public async Task<List<string>> GetStaffRoleAsync(Guid staffId)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId} not found.");

            return staff.Permissions.ToList();
        }

        public async Task<StaffDetailVM> UpdateStaffPermissionsAsync(Guid staffId, List<string> permissions)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId} not found.");

            staff.UpdatePermissions(permissions);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StaffDetailVM>(staff);
        }

        public async Task<bool> AssignFollowUpAsync(Guid staffId, Guid followUpId)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId} not found.");

            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            followUp.AssignToStaff(staffId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> TransferAssignmentsAsync(Guid fromStaffId, Guid toStaffId)
        {
            var fromStaff = await _unitOfWork.Staff.GetByIdAsync(fromStaffId);
            var toStaff = await _unitOfWork.Staff.GetByIdAsync(toStaffId);

            if (fromStaff == null || toStaff == null)
                throw new InvalidOperationException("One or both staff members not found.");

            // Transfer pending follow-ups
            var pendingFollowUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(fromStaffId);
            foreach (var followUp in pendingFollowUps.Where(f => f.Status == "Pending"))
            {
                followUp.AssignToStaff(toStaffId);
            }

            // Transfer scheduled visits
            var scheduledVisits = await _unitOfWork.Visits.GetByStaffIdAsync(fromStaffId);
            foreach (var visit in scheduledVisits.Where(v => v.VisitDate >= DateTime.Today))
            {
                visit.AssignToStaff(toStaffId);
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
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

            // Get scheduled follow-ups
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId);
            var dayFollowUps = followUps.Where(f => f.ScheduledDate.Date == date.Date);
            foreach (var followUp in dayFollowUps)
            {
                schedule.Add(new VisitAnalyticsViewModels.StaffScheduleVM
                {
                    ActivityType = StaffActivityType.FollowUp,
                    ActivityId = followUp.Id,
                    Description = followUp.Description ?? string.Empty,
                    StartTime = followUp.ScheduledDate,
                    EndTime = followUp.ScheduledDate.AddHours(1), // Default 1 hour duration
                    Location = "Office",
                    Status = followUp.Status.ToString()
                });
            }

            return schedule.OrderBy(s => s.StartTime).ToList();
        }

        public async Task<StaffDetailVM> UpdateContactInfoAsync(Guid staffId, Phone phone, string email)
        {
            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId} not found.");

            staff.UpdateContactInfo(email, phone);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<StaffDetailVM>(staff);
        }
    }

    public interface IStaffService
    {
        Task<StaffDetailVM> CreateAsync(StaffCreateVM model);
        Task<StaffDetailVM> UpdateAsync(StaffUpdateVM model);
        Task<StaffDetailVM> GetByIdAsync(Guid id);
        Task<PagedResult<StaffListVM>> GetPagedAsync(string filter = null, string orderBy = "LastName", int pageNumber = 1, int pageSize = 10);
        Task<bool> DeactivateAsync(Guid id);
        Task<List<StaffListVM>> GetActiveStaffAsync();
        Task<List<StaffListVM>> GetStaffByDepartmentAsync(string department);
        Task<List<StaffListVM>> GetStaffByPositionAsync(string position);
        Task<StaffDetailVM> UpdateStaffStatusAsync(Guid id, bool isActive);
        Task<List<StaffListVM>> SearchStaffAsync(string searchTerm);
        Task<List<StaffAnalyticsViewModels.StaffActivityVM>> GetStaffActivitiesAsync(Guid staffId, DateTime fromDate, DateTime toDate);
        Task<List<PaymentListVM>> GetStaffPaymentsAsync(Guid staffId);
        Task<List<VisitListVM>> GetStaffVisitsAsync(Guid staffId);
        Task<List<FollowUpListVM>> GetStaffFollowUpsAsync(Guid staffId);
        Task<List<ReceiptListVM>> GetStaffReceiptsAsync(Guid staffId);
        Task<StaffAnalyticsViewModels.StaffPerformanceVM> GetStaffPerformanceAsync(Guid staffId, DateTime fromDate, DateTime toDate);
        Task<StaffAnalyticsViewModels.CollectionPerformanceVM> GetCollectionPerformanceAsync(Guid staffId);
        Task<VisitAnalyticsViewModels.VisitEffectivenessVM> GetVisitEffectivenessAsync(Guid staffId);
        Task<StaffAnalyticsViewModels.WorkloadAnalysisVM> GetWorkloadAnalysisAsync(Guid staffId);
        Task<bool> ValidateStaffPermissionsAsync(Guid staffId, string operation);
        Task<List<string>> GetStaffRoleAsync(Guid staffId);
        Task<StaffDetailVM> UpdateStaffPermissionsAsync(Guid staffId, List<string> permissions);
        Task<bool> AssignFollowUpAsync(Guid staffId, Guid followUpId);
        Task<bool> TransferAssignmentsAsync(Guid fromStaffId, Guid toStaffId);
        Task<List<VisitAnalyticsViewModels.StaffScheduleVM>> GetStaffScheduleAsync(Guid staffId, DateTime date);
        Task<StaffDetailVM> UpdateContactInfoAsync(Guid staffId, Phone phone, string email);
    }
}
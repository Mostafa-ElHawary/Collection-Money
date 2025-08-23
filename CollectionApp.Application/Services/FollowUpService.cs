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

namespace CollectionApp.Application.Services
{
    public class FollowUpService : IFollowUpService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public FollowUpService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<FollowUpDetailVM> CreateAsync(FollowUpCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Validate customer exists
            var customer = await _unitOfWork.Customers.GetByIdAsync(model.CustomerId);
            if (customer == null)
                throw new InvalidOperationException($"Customer with ID {model.CustomerId} not found.");

            // Validate staff exists if assigned
            if (model.AssignedToStaffId.HasValue)
            {
                var staff = await _unitOfWork.Staff.GetByIdAsync(model.AssignedToStaffId.Value);
                if (staff == null)
                    throw new InvalidOperationException($"Staff with ID {model.AssignedToStaffId.Value} not found.");
            }

            // Validate contract exists if specified
            if (model.ContractId.HasValue)
            {
                var contract = await _unitOfWork.Contracts.GetByIdAsync(model.ContractId.Value);
                if (contract == null)
                    throw new InvalidOperationException($"Contract with ID {model.ContractId.Value} not found.");
            }

            // Validate scheduled date is reasonable
            if (model.ScheduledDate < DateTime.Today.AddDays(-7) || model.ScheduledDate > DateTime.Today.AddDays(90))
                throw new InvalidOperationException("Scheduled date must be within reasonable range (not more than 7 days in past or 90 days in future).");

            // Create follow-up entity
            var followUp = new FollowUp(
                model.CustomerId,
                model.ContractId,
                model.AssignedToStaffId,
                model.Type,
                model.Priority,
                model.Description,
                model.ScheduledDate,
                model.Notes
            );

            await _unitOfWork.FollowUps.AddAsync(followUp);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<FollowUpDetailVM> UpdateAsync(FollowUpUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(model.Id);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {model.Id} not found.");

            // Check if follow-up can be updated (not completed)
            if (followUp.Status.ToString() == FollowUpStatus.Completed.ToString())
                throw new InvalidOperationException("Cannot update completed follow-up.");

            // Update follow-up details
            followUp.UpdateDetails(
                model.Type,
                model.Priority,
                model.Description,
                model.ScheduledDate,
                model.Notes
            );

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<FollowUpDetailVM> GetByIdAsync(Guid id)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(id);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {id} not found.");

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<PagedResult<FollowUpListVM>> GetPagedAsync(
            string filter = null,
            string orderBy = "ScheduledDate",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var followUps = await _unitOfWork.FollowUps.GetPagedAsync(filter, orderBy, pageNumber, pageSize);
            
            var followUpListVMs = _mapper.Map<List<FollowUpListVM>>(followUps.Items);
            
            return new PagedResult<FollowUpListVM>(followUpListVMs.AsReadOnly(), followUps.TotalCount, followUps.PageNumber, followUps.PageSize);
        }

        public async Task<bool> DeleteAsync(Guid id)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(id);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {id} not found.");

            // Check if follow-up can be deleted (not completed)
            if (followUp.Status.ToString() == FollowUpStatus.Completed.ToString())
                throw new InvalidOperationException("Cannot delete completed follow-up.");

            await _unitOfWork.FollowUps.DeleteAsync(followUp);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<FollowUpDetailVM> ScheduleFollowUpAsync(FollowUpCreateVM model)
        {
            // ScheduleFollowUpAsync is essentially the same as CreateAsync
            return await CreateAsync(model);
        }

        public async Task<FollowUpDetailVM> CompleteFollowUpAsync(Guid followUpId, string outcome, DateTime actualDate)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            followUp.Complete(outcome, actualDate);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<FollowUpDetailVM> RescheduleFollowUpAsync(Guid followUpId, DateTime newDate)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            // Validate new date is reasonable
            if (newDate < DateTime.Today.AddDays(-7) || newDate > DateTime.Today.AddDays(90))
                throw new InvalidOperationException("Scheduled date must be within reasonable range (not more than 7 days in past or 90 days in future).");

            followUp.Reschedule(newDate);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<FollowUpDetailVM> AssignFollowUpAsync(Guid followUpId, Guid staffId)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId} not found.");

            followUp.AssignToStaff(staffId);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<FollowUpDetailVM> EscalateFollowUpAsync(Guid followUpId, string reason)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            followUp.Escalate(reason);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<List<FollowUpListVM>> GetFollowUpsByCustomerAsync(Guid customerId)
        {
            var followUps = await _unitOfWork.FollowUps.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<List<FollowUpListVM>> GetFollowUpsByStaffAsync(Guid staffId)
        {
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId);
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<List<FollowUpListVM>> GetFollowUpsByContractAsync(Guid contractId)
        {
            var followUps = await _unitOfWork.FollowUps.GetByContractIdAsync(contractId);
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<List<FollowUpListVM>> GetOverdueFollowUpsAsync()
        {
            var followUps = await _unitOfWork.FollowUps.GetOverdueAsync();
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<List<FollowUpListVM>> GetUpcomingFollowUpsAsync(int days)
        {
            var fromDate = DateTime.Today;
            var toDate = DateTime.Today.AddDays(days);
            var followUps = await _unitOfWork.FollowUps.GetByDateRangeAsync(fromDate, toDate);
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<FollowUpDetailVM> UpdateFollowUpPriorityAsync(Guid followUpId, string priority)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            followUp.UpdatePriority(priority);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<FollowUpDetailVM> UpdateFollowUpStatusAsync(Guid followUpId, string status)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            followUp.UpdateStatus(status);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<FollowUpDetailVM>(followUp);
        }

        public async Task<List<FollowUpListVM>> GetHighPriorityFollowUpsAsync()
        {
            var followUps = await _unitOfWork.FollowUps.GetByPriorityAsync("High");
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<List<FollowUpListVM>> GetFollowUpsByStatusAsync(string status)
        {
            var followUps = await _unitOfWork.FollowUps.GetByStatusAsync(status);
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<List<FollowUpListVM>> GetFollowUpsByTypeAsync(string type)
        {
            var followUps = await _unitOfWork.FollowUps.GetByTypeAsync(type);
            return _mapper.Map<List<FollowUpListVM>>(followUps);
        }

        public async Task<bool> CreateFollowUpFromPaymentAsync(Guid paymentId, FollowUpCreateVM model)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {paymentId} not found.");

            // Set contract ID from payment
            model.ContractId = payment.ContractId;
            
            // Create follow-up
            await CreateAsync(model);

            return true;
        }

        public async Task<bool> CreateFollowUpFromVisitAsync(Guid visitId, FollowUpCreateVM model)
        {
            var visit = await _unitOfWork.Visits.GetByIdAsync(visitId);
            if (visit == null)
                throw new InvalidOperationException($"Visit with ID {visitId} not found.");

            // Set customer ID from visit
            model.CustomerId = visit.CustomerId;
            
            // Create follow-up
            await CreateAsync(model);

            return true;
        }

        public async Task<bool> BulkCreateFollowUpsAsync(List<FollowUpCreateVM> followUps)
        {
            foreach (var followUp in followUps)
            {
                await CreateAsync(followUp);
            }

            return true;
        }

        public async Task<bool> TransferFollowUpsAsync(Guid fromStaffId, Guid toStaffId)
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

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<FollowUpAnalyticsViewModels.FollowUpEffectivenessVM> GetFollowUpEffectivenessAsync(Guid staffId)
        {
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId);
            
            var effectiveness = new FollowUpAnalyticsViewModels.FollowUpEffectivenessVM
            {
                StaffId = staffId,
                TotalFollowUps = followUps.Count,
                CompletedFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Completed.ToString()),
                PendingFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Pending.ToString()),
                OverdueFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Pending.ToString() && f.ScheduledDate < DateTime.Today),
                CompletionRate = followUps.Any() ? 
                    (decimal)followUps.Count(f => f.Status.ToString() == FollowUpStatus.Completed.ToString()) / followUps.Count * 100 : 0,
                AverageCompletionTime = followUps.Where(f => f.Status.ToString() == FollowUpStatus.Completed.ToString() && f.ActualDate.HasValue)
                                               .Select(f => (f.ActualDate.Value - f.ScheduledDate).Days)
                                               .DefaultIfEmpty(0)
                                               .Average()
            };

            return effectiveness;
        }

        public async Task<FollowUpAnalyticsViewModels.FollowUpPerformanceVM> GetFollowUpPerformanceAsync(DateTime fromDate, DateTime toDate)
        {
            var followUps = await _unitOfWork.FollowUps.GetByDateRangeAsync(fromDate, toDate);
            
            var performance = new FollowUpAnalyticsViewModels.FollowUpPerformanceVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalFollowUps = followUps.Count,
                CompletedFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Completed.ToString()),
                PendingFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Pending.ToString()),
                OverdueFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Pending.ToString() && f.ScheduledDate < DateTime.Today),
                HighPriorityFollowUps = followUps.Count(f => f.Priority == "High"),
                FollowUpsByType = followUps.GroupBy(f => f.Type)
                                         .ToDictionary(g => g.Key, g => g.Count()),
                FollowUpsByPriority = followUps.GroupBy(f => f.Priority)
                                             .ToDictionary(g => g.Key, g => g.Count())
            };

            return performance;
        }

        public async Task<FollowUpAnalyticsViewModels.CustomerFollowUpPatternVM> GetCustomerFollowUpPatternAsync(Guid customerId)
        {
            var followUps = await _unitOfWork.FollowUps.GetByCustomerIdAsync(customerId);
            
            var pattern = new FollowUpAnalyticsViewModels.CustomerFollowUpPatternVM
            {
                CustomerId = customerId,
                TotalFollowUps = followUps.Count,
                CompletedFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Completed.ToString()),
                PendingFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Pending.ToString()),
                AverageFollowUpsPerMonth = followUps.Any() ? 
                    (decimal)followUps.Count / Math.Max(1m, ((DateTime.Now - followUps.Min(f => f.ScheduledDate)).Days) / 30m) : 0,
                LastFollowUpDate = followUps.Any() ? followUps.Max(f => f.ScheduledDate) : null,
                FollowUpFrequency = followUps.Count > 1 ? 
                    followUps.OrderBy(f => f.ScheduledDate)
                             .Select((f, i) => i > 0 ? (f.ScheduledDate - followUps.OrderBy(f2 => f2.ScheduledDate)
                             .ElementAt(i - 1).ScheduledDate).Days : 0)
                             .Where(d => d > 0)
                             .Average() : 0
            };

            return pattern;
        }

        public async Task<FollowUpAnalyticsViewModels.StaffWorkloadVM> GetStaffWorkloadAsync(Guid staffId)
        {
            var followUps = await _unitOfWork.FollowUps.GetByStaffIdAsync(staffId);
            
            var workload = new FollowUpAnalyticsViewModels.StaffWorkloadVM
            {
                StaffId = staffId,
                TotalFollowUps = followUps.Count,
                PendingFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Pending.ToString()),
                OverdueFollowUps = followUps.Count(f => f.Status.ToString() == FollowUpStatus.Pending.ToString() && f.ScheduledDate < DateTime.Today),
                HighPriorityFollowUps = followUps.Count(f => f.Priority == "High" && f.Status.ToString() == FollowUpStatus.Pending.ToString()),
                TodayFollowUps = followUps.Count(f => f.ScheduledDate.Date == DateTime.Today),
                ThisWeekFollowUps = followUps.Count(f => f.ScheduledDate >= DateTime.Today && f.ScheduledDate <= DateTime.Today.AddDays(7))
            };

            return workload;
        }

        public async Task<bool> CreateAutomaticFollowUpsAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            var followUps = new List<FollowUpCreateVM>();

            // Create follow-up for contract activation
            if (contract.Status == Domain.Enums.ContractStatus.Active)
            {
                followUps.Add(new FollowUpCreateVM
                {
                    CustomerId = contract.CustomerId,
                    ContractId = contractId,
                    Type = "Contract Activation",
                    Priority = "Medium",
                    Description = "Follow up on contract activation and first payment",
                    ScheduledDate = DateTime.Today.AddDays(7),
                    Notes = "Automatic follow-up for contract activation"
                });
            }

            // Create follow-up for overdue installments
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            var overdueInstallments = installments.Where(i => i.Status == Domain.Enums.InstallmentStatus.Overdue);
            
            foreach (var installment in overdueInstallments)
            {
                followUps.Add(new FollowUpCreateVM
                {
                    CustomerId = contract.CustomerId,
                    ContractId = contractId,
                    Type = "Payment Reminder",
                    Priority = "High",
                    Description = $"Follow up on overdue installment {installment.InstallmentNumber}",
                    ScheduledDate = DateTime.Today.AddDays(3),
                    Notes = $"Automatic follow-up for overdue installment {installment.InstallmentNumber}"
                });
            }

            // Create all follow-ups
            foreach (var followUp in followUps)
            {
                await CreateAsync(followUp);
            }

            return true;
        }

        public async Task<bool> ProcessOverdueFollowUpsAsync()
        {
            var overdueFollowUps = await _unitOfWork.FollowUps.GetOverdueAsync();
            
            foreach (var followUp in overdueFollowUps)
            {
                // Escalate overdue follow-ups
                followUp.Escalate("Automatically escalated due to overdue status");
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> GenerateFollowUpRemindersAsync(int daysBefore)
        {
            var reminderDate = DateTime.Today.AddDays(daysBefore);
            var upcomingFollowUps = await _unitOfWork.FollowUps.GetByDateRangeAsync(reminderDate, reminderDate);
            
            // In a real implementation, you would send notifications/emails here
            // For now, we'll just log that reminders would be generated
            
            return true;
        }

        public async Task<bool> ArchiveCompletedFollowUpsAsync(DateTime beforeDate)
        {
            var completedFollowUps = await _unitOfWork.FollowUps.GetCompletedAsync(DateTime.MinValue, beforeDate);
            
            foreach (var followUp in completedFollowUps)
            {
                followUp.Archive();
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> LinkFollowUpToPaymentAsync(Guid followUpId, Guid paymentId)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {paymentId} not found.");

            followUp.LinkToPayment(paymentId);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<bool> CreateContractFollowUpAsync(Guid contractId, string type)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            var followUp = new FollowUpCreateVM
            {
                CustomerId = contract.CustomerId,
                ContractId = contractId,
                Type = type,
                Priority = "Medium",
                Description = $"Contract follow-up: {type}",
                ScheduledDate = DateTime.Today.AddDays(3),
                Notes = $"Automatic contract follow-up for {type}"
            };

            await CreateAsync(followUp);

            return true;
        }

        public async Task<List<FollowUpListVM>> GetRelatedFollowUpsAsync(Guid followUpId)
        {
            var followUp = await _unitOfWork.FollowUps.GetByIdAsync(followUpId);
            if (followUp == null)
                throw new InvalidOperationException($"Follow-up with ID {followUpId} not found.");

            var relatedFollowUps = new List<FollowUpListVM>();

            // Get follow-ups for the same customer
            var customerFollowUps = await _unitOfWork.FollowUps.GetByCustomerIdAsync(followUp.CustomerId);
            relatedFollowUps.AddRange(_mapper.Map<List<FollowUpListVM>>(customerFollowUps.Where(f => f.Id != followUpId)));

            // Get follow-ups for the same contract if applicable
            if (followUp.ContractId.HasValue)
            {
                var contractFollowUps = await _unitOfWork.FollowUps.GetByContractIdAsync(followUp.ContractId.Value);
                relatedFollowUps.AddRange(_mapper.Map<List<FollowUpListVM>>(contractFollowUps.Where(f => f.Id != followUpId)));
            }

            return relatedFollowUps.Distinct().ToList();
        }
    }

    public interface IFollowUpService
    {
        Task<FollowUpDetailVM> CreateAsync(FollowUpCreateVM model);
        Task<FollowUpDetailVM> UpdateAsync(FollowUpUpdateVM model);
        Task<FollowUpDetailVM> GetByIdAsync(Guid id);
        Task<PagedResult<FollowUpListVM>> GetPagedAsync(string filter = null, string orderBy = "ScheduledDate", int pageNumber = 1, int pageSize = 10);
        Task<bool> DeleteAsync(Guid id);
        Task<FollowUpDetailVM> ScheduleFollowUpAsync(FollowUpCreateVM model);
        Task<FollowUpDetailVM> CompleteFollowUpAsync(Guid followUpId, string outcome, DateTime actualDate);
        Task<FollowUpDetailVM> RescheduleFollowUpAsync(Guid followUpId, DateTime newDate);
        Task<FollowUpDetailVM> AssignFollowUpAsync(Guid followUpId, Guid staffId);
        Task<FollowUpDetailVM> EscalateFollowUpAsync(Guid followUpId, string reason);
        Task<List<FollowUpListVM>> GetFollowUpsByCustomerAsync(Guid customerId);
        Task<List<FollowUpListVM>> GetFollowUpsByStaffAsync(Guid staffId);
        Task<List<FollowUpListVM>> GetFollowUpsByContractAsync(Guid contractId);
        Task<List<FollowUpListVM>> GetOverdueFollowUpsAsync();
        Task<List<FollowUpListVM>> GetUpcomingFollowUpsAsync(int days);
        Task<FollowUpDetailVM> UpdateFollowUpPriorityAsync(Guid followUpId, string priority);
        Task<FollowUpDetailVM> UpdateFollowUpStatusAsync(Guid followUpId, string status);
        Task<List<FollowUpListVM>> GetHighPriorityFollowUpsAsync();
        Task<List<FollowUpListVM>> GetFollowUpsByStatusAsync(string status);
        Task<List<FollowUpListVM>> GetFollowUpsByTypeAsync(string type);
        Task<bool> CreateFollowUpFromPaymentAsync(Guid paymentId, FollowUpCreateVM model);
        Task<bool> CreateFollowUpFromVisitAsync(Guid visitId, FollowUpCreateVM model);
        Task<bool> BulkCreateFollowUpsAsync(List<FollowUpCreateVM> followUps);
        Task<bool> TransferFollowUpsAsync(Guid fromStaffId, Guid toStaffId);
        Task<FollowUpAnalyticsViewModels.FollowUpEffectivenessVM> GetFollowUpEffectivenessAsync(Guid staffId);
        Task<FollowUpAnalyticsViewModels.FollowUpPerformanceVM> GetFollowUpPerformanceAsync(DateTime fromDate, DateTime toDate);
        Task<FollowUpAnalyticsViewModels.CustomerFollowUpPatternVM> GetCustomerFollowUpPatternAsync(Guid customerId);
        Task<FollowUpAnalyticsViewModels.StaffWorkloadVM> GetStaffWorkloadAsync(Guid staffId);
        Task<bool> CreateAutomaticFollowUpsAsync(Guid contractId);
        Task<bool> ProcessOverdueFollowUpsAsync();
        Task<bool> GenerateFollowUpRemindersAsync(int daysBefore);
        Task<bool> ArchiveCompletedFollowUpsAsync(DateTime beforeDate);
        Task<bool> LinkFollowUpToPaymentAsync(Guid followUpId, Guid paymentId);
        Task<bool> CreateContractFollowUpAsync(Guid contractId, string type);
        Task<List<FollowUpListVM>> GetRelatedFollowUpsAsync(Guid followUpId);
    }
} 
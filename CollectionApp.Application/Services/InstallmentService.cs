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
    public class InstallmentService : IInstallmentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public InstallmentService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<InstallmentDetailVM> CreateAsync(InstallmentCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Validate contract exists
            var contract = await _unitOfWork.Contracts.GetByIdAsync(model.ContractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {model.ContractId} not found.");

            // Create Money value object
            var amount = new Money(model.Amount, model.Currency);

            // Create installment entity
            var installment = new Installment(
                model.ContractId,
                model.InstallmentNumber,
                model.DueDate,
                amount,
                model.Notes
            );

            await _unitOfWork.Installments.AddAsync(installment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InstallmentDetailVM>(installment);
        }

        public async Task<InstallmentDetailVM> UpdateAsync(InstallmentUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var installment = await _unitOfWork.Installments.GetByIdAsync(model.Id);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {model.Id} not found.");

            // Check if installment can be updated (not paid)
            if (installment.Status == InstallmentStatus.Paid)
                throw new InvalidOperationException("Cannot update paid installment.");

            // Update installment
            installment.UpdateDetails(
                new Money(model.Amount, model.Currency),
                model.DueDate,
                model.Notes
            );
            
            if (model.Status != installment.Status)
            {
                installment.UpdateStatus(model.Status);
            }
            
            if (model.PaymentDate.HasValue)
            {
                installment.MarkAsPaid(model.PaymentDate.Value);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InstallmentDetailVM>(installment);
        }

        public async Task<InstallmentDetailVM> GetByIdAsync(Guid id)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(id);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {id} not found.");

            return _mapper.Map<InstallmentDetailVM>(installment);
        }

        public async Task<PagedResult<InstallmentListVM>> GetPagedAsync(
            string filter = null,
            string orderBy = "DueDate",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var installments = await _unitOfWork.Installments.GetPagedAsync(filter, orderBy, pageNumber, pageSize);
            
            var installmentListVMs = _mapper.Map<List<InstallmentListVM>>(installments.Items);
            
            return new PagedResult<InstallmentListVM>(installmentListVMs.AsReadOnly(), installments.TotalCount, installments.PageNumber, installments.PageSize);
        }

        public async Task<List<InstallmentListVM>> GetInstallmentsByContractAsync(Guid contractId)
        {
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            return _mapper.Map<List<InstallmentListVM>>(installments);
        }

        public async Task<List<InstallmentListVM>> GetOverdueInstallmentsAsync()
        {
            var installments = await _unitOfWork.Installments.GetOverdueAsync();
            return _mapper.Map<List<InstallmentListVM>>(installments);
        }

        public async Task<List<InstallmentListVM>> GetUpcomingInstallmentsAsync(int days)
        {
            var fromDate = DateTime.Today;
            var toDate = DateTime.Today.AddDays(days);
            var installments = await _unitOfWork.Installments.GetByDateRangeAsync(fromDate, toDate);
            return _mapper.Map<List<InstallmentListVM>>(installments);
        }

        public async Task<InstallmentDetailVM> MarkInstallmentPaidAsync(Guid installmentId, Guid paymentId)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {installmentId} not found.");

            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {paymentId} not found.");

            installment.MarkAsPaid(payment.PaymentDate);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InstallmentDetailVM>(installment);
        }

        public async Task<decimal> CalculateRemainingAmountAsync(Guid installmentId)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {installmentId} not found.");

            return installment.RemainingAmount;
        }

        public async Task<InstallmentDetailVM> WaiveInstallmentAsync(Guid installmentId, string reason)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {installmentId} not found.");

            // Check if installment can be waived (not paid)
            if (installment.Status == InstallmentStatus.Paid)
                throw new InvalidOperationException("Cannot waive paid installment.");

            installment.Waive(reason);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InstallmentDetailVM>(installment);
        }

        public async Task<InstallmentDetailVM> RescheduleInstallmentAsync(Guid installmentId, DateTime newDueDate)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {installmentId} not found.");

            // Check if installment can be rescheduled (not paid)
            if (installment.Status == InstallmentStatus.Paid)
                throw new InvalidOperationException("Cannot reschedule paid installment.");

            installment.UpdateDueDate(newDueDate);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InstallmentDetailVM>(installment);
        }

        public async Task<List<InstallmentListVM>> SplitInstallmentAsync(Guid installmentId, List<decimal> amounts)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {installmentId} not found.");

            // Check if installment can be split (not paid)
            if (installment.Status == InstallmentStatus.Paid)
                throw new InvalidOperationException("Cannot split paid installment.");

            // Validate total amount matches original
            var totalAmount = amounts.Sum();
            if (Math.Abs(totalAmount - installment.Amount.Amount) > 0.01m)
                throw new InvalidOperationException("Split amounts must equal original installment amount.");

            // Delete original installment
            await _unitOfWork.Installments.DeleteAsync(installment);

            // Create new installments
            var newInstallments = new List<Installment>();
            for (int i = 0; i < amounts.Count; i++)
            {
                var newInstallment = new Installment(
                    installment.ContractId,
                    installment.InstallmentNumber * 10 + (i + 1), // Create new installment numbers
                    installment.DueDate.AddDays(i * 7), // Spread due dates
                    new Money(amounts[i], installment.Amount.Currency),
                    $"Split from installment {installment.InstallmentNumber}"
                );
                newInstallments.Add(newInstallment);
            }

            // Add new installments
            foreach (var newInstallment in newInstallments)
            {
                await _unitOfWork.Installments.AddAsync(newInstallment);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<List<InstallmentListVM>>(newInstallments);
        }

        public async Task<InstallmentAnalyticsViewModels.InstallmentStatusSummaryVM> GetInstallmentStatusSummaryAsync(Guid contractId)
        {
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            
            var summary = new InstallmentAnalyticsViewModels.InstallmentStatusSummaryVM
            {
                ContractId = contractId,
                TotalInstallments = installments.Count,
                PaidInstallments = installments.Count(i => i.Status == InstallmentStatus.Paid),
                PendingInstallments = installments.Count(i => i.Status == InstallmentStatus.Pending),
                OverdueInstallments = installments.Count(i => i.Status == InstallmentStatus.Overdue),
                WaivedInstallments = installments.Count(i => i.Status == InstallmentStatus.Waived),
                TotalAmount = installments.Sum(i => i.Amount.Amount),
                PaidAmount = installments.Where(i => i.Status == InstallmentStatus.Paid).Sum(i => i.Amount.Amount),
                OutstandingAmount = installments.Where(i => i.Status != InstallmentStatus.Paid).Sum(i => i.Amount.Amount)
            };

            return summary;
        }

        public async Task<InstallmentAnalyticsViewModels.CollectionReportVM> GetCollectionReportAsync(DateTime fromDate, DateTime toDate)
        {
            var installments = await _unitOfWork.Installments.GetByDateRangeAsync(fromDate, toDate);
            
            var report = new InstallmentAnalyticsViewModels.CollectionReportVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalInstallments = installments.Count,
                PaidInstallments = installments.Count(i => i.Status == InstallmentStatus.Paid),
                OverdueInstallments = installments.Count(i => i.Status == InstallmentStatus.Overdue),
                TotalAmount = installments.Sum(i => i.Amount.Amount),
                CollectedAmount = installments.Where(i => i.Status == InstallmentStatus.Paid).Sum(i => i.Amount.Amount),
                OutstandingAmount = installments.Where(i => i.Status != InstallmentStatus.Paid).Sum(i => i.Amount.Amount),
                CollectionRate = installments.Any() ? 
                    (decimal)installments.Count(i => i.Status == InstallmentStatus.Paid) / installments.Count * 100 : 0
            };

            return report;
        }

        public async Task<InstallmentAnalyticsViewModels.OverdueAnalysisVM> GetOverdueAnalysisAsync()
        {
            var overdueInstallments = await _unitOfWork.Installments.GetOverdueAsync();
            
            var analysis = new InstallmentAnalyticsViewModels.OverdueAnalysisVM
            {
                TotalOverdue = overdueInstallments.Count,
                TotalOverdueAmount = overdueInstallments.Sum(i => i.Amount.Amount),
                AverageOverdueDays = overdueInstallments.Any() ? 
                    (int)overdueInstallments.Average(i => (DateTime.Today - i.DueDate).Days) : 0,
                OverdueByAge = new Dictionary<string, int>
                {
                    ["1-30 days"] = overdueInstallments.Count(i => (DateTime.Today - i.DueDate).Days <= 30),
                    ["31-60 days"] = overdueInstallments.Count(i => (DateTime.Today - i.DueDate).Days > 30 && (DateTime.Today - i.DueDate).Days <= 60),
                    ["61-90 days"] = overdueInstallments.Count(i => (DateTime.Today - i.DueDate).Days > 60 && (DateTime.Today - i.DueDate).Days <= 90),
                    ["90+ days"] = overdueInstallments.Count(i => (DateTime.Today - i.DueDate).Days > 90)
                }
            };

            return analysis;
        }

        public async Task<InstallmentDetailVM> UpdateInstallmentStatusAsync(Guid installmentId, InstallmentStatus status)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {installmentId} not found.");

            // Validate status transition
            if (!IsValidStatusTransition(installment.Status, status))
                throw new InvalidOperationException($"Invalid status transition from {installment.Status} to {status}.");

            installment.UpdateStatus(status);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<InstallmentDetailVM>(installment);
        }

        public async Task<decimal> CalculateOverdueAmountAsync(Guid customerId)
        {
            var contracts = await _unitOfWork.Contracts.GetByCustomerIdAsync(customerId);
            var totalOverdue = 0m;

            foreach (var contract in contracts)
            {
                var installments = await _unitOfWork.Installments.GetByContractIdAsync(contract.Id);
                var overdueInstallments = installments.Where(i => i.Status == InstallmentStatus.Overdue);
                totalOverdue += overdueInstallments.Sum(i => i.Amount.Amount);
            }

            return totalOverdue;
        }

        public async Task<List<InstallmentAnalyticsViewModels.PaymentHistoryVM>> GetInstallmentHistoryAsync(Guid installmentId)
        {
            var payments = await _unitOfWork.Payments.GetByInstallmentIdAsync(installmentId);
            return _mapper.Map<List<InstallmentAnalyticsViewModels.PaymentHistoryVM>>(payments);
        }

        private bool IsValidStatusTransition(InstallmentStatus currentStatus, InstallmentStatus newStatus)
        {
            // Define valid status transitions
            var validTransitions = new Dictionary<InstallmentStatus, InstallmentStatus[]>
            {
                [InstallmentStatus.Pending] = new[] { InstallmentStatus.Paid, InstallmentStatus.Overdue, InstallmentStatus.Waived },
                [InstallmentStatus.Overdue] = new[] { InstallmentStatus.Paid, InstallmentStatus.Waived },
                [InstallmentStatus.Paid] = new[] { InstallmentStatus.Pending }, // For reversals
                [InstallmentStatus.Waived] = new[] { InstallmentStatus.Pending } // For unwaiving
            };

            return validTransitions.ContainsKey(currentStatus) && 
                   validTransitions[currentStatus].Contains(newStatus);
        }
    }

    public interface IInstallmentService
    {
        Task<InstallmentDetailVM> CreateAsync(InstallmentCreateVM model);
        Task<InstallmentDetailVM> UpdateAsync(InstallmentUpdateVM model);
        Task<InstallmentDetailVM> GetByIdAsync(Guid id);
        Task<PagedResult<InstallmentListVM>> GetPagedAsync(string filter = null, string orderBy = "DueDate", int pageNumber = 1, int pageSize = 10);
        Task<List<InstallmentListVM>> GetInstallmentsByContractAsync(Guid contractId);
        Task<List<InstallmentListVM>> GetOverdueInstallmentsAsync();
        Task<List<InstallmentListVM>> GetUpcomingInstallmentsAsync(int days);
        Task<InstallmentDetailVM> MarkInstallmentPaidAsync(Guid installmentId, Guid paymentId);
        Task<decimal> CalculateRemainingAmountAsync(Guid installmentId);
        Task<InstallmentDetailVM> WaiveInstallmentAsync(Guid installmentId, string reason);
        Task<InstallmentDetailVM> RescheduleInstallmentAsync(Guid installmentId, DateTime newDueDate);
        Task<List<InstallmentListVM>> SplitInstallmentAsync(Guid installmentId, List<decimal> amounts);
        Task<InstallmentAnalyticsViewModels.InstallmentStatusSummaryVM> GetInstallmentStatusSummaryAsync(Guid contractId);
        Task<InstallmentAnalyticsViewModels.CollectionReportVM> GetCollectionReportAsync(DateTime fromDate, DateTime toDate);
        Task<InstallmentAnalyticsViewModels.OverdueAnalysisVM> GetOverdueAnalysisAsync();
        Task<InstallmentDetailVM> UpdateInstallmentStatusAsync(Guid installmentId, InstallmentStatus status);
        Task<decimal> CalculateOverdueAmountAsync(Guid customerId);
        Task<List<InstallmentAnalyticsViewModels.PaymentHistoryVM>> GetInstallmentHistoryAsync(Guid installmentId);
    }
}
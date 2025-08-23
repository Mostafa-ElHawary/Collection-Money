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
    public class ContractService : IContractService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly IInstallmentService _installmentService; // Add this field


        public ContractService(IUnitOfWork unitOfWork, IMapper mapper, IInstallmentService installmentService)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _installmentService = installmentService ?? throw new ArgumentNullException(nameof(installmentService));

        }

        public async Task<ContractDetailVM> CreateAsync(ContractCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            return await _unitOfWork.ExecuteInTransactionAsync(async (cancellationToken) =>
            {
                // Validate customer exists
                var customer = await _unitOfWork.Customers.GetByIdAsync(model.CustomerId);
                if (customer == null)
                    throw new InvalidOperationException($"Customer with ID {model.CustomerId} not found.");

                // Validate staff exists if assigned
                if (model.StaffId.HasValue)
                {
                    var staff = await _unitOfWork.Staff.GetByIdAsync(model.StaffId.Value);
                    if (staff == null)
                        throw new InvalidOperationException($"Staff with ID {model.StaffId.Value} not found.");
                }

                // Check for duplicate contract number
                var existingContract = await _unitOfWork.Contracts.GetByContractNumberAsync(model.ContractNumber);
                if (existingContract != null)
                    throw new InvalidOperationException($"Contract with number {model.ContractNumber} already exists.");

                // Create Money value objects
                var principalAmount = new Money(model.PrincipalAmount, model.Currency);
                var totalAmount = new Money(model.TotalAmount, model.Currency);
                var interestRate = new Money(model.InterestRate, model.Currency);

                // Create contract entity
                var contract = new Contract(
                    model.ContractNumber,
                    model.CustomerId,
                    model.ContractType,
                    principalAmount,
                    interestRate,
                    model.TermInMonths,
                    model.StartDate,
                    model.EndDate,
                    model.PaymentFrequency,
                    model.GracePeriodDays,
                    model.LateFeePercentage,
                    model.PenaltyPercentage,
                    model.CollateralDescription,
                    model.GuarantorName,
                    model.GuarantorContact,
                    model.Notes,
                    model.StaffId,
                    model.Purpose,
                    model.SourceOfFunds
                );

                await _unitOfWork.Contracts.AddAsync(contract);

                // Generate installments
                await GenerateInstallmentsAsync(contract.Id);

                // Save all changes at once
                await _unitOfWork.SaveChangesAsync(false);

                return _mapper.Map<ContractDetailVM>(contract);
            });
        }

        public async Task<ContractDetailVM> UpdateAsync(ContractUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(model.Id);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {model.Id} not found.");

            // Check if contract can be updated (not completed or cancelled)
            if (contract.Status == ContractStatus.Completed || contract.Status == ContractStatus.Cancelled)
                throw new InvalidOperationException($"Cannot update contract in {contract.Status} status.");

            // Validate staff exists if assigned
            if (model.StaffId.HasValue)
            {
                var staff = await _unitOfWork.Staff.GetByIdAsync(model.StaffId.Value);
                if (staff == null)
                    throw new InvalidOperationException($"Staff with ID {model.StaffId.Value} not found.");
            }

            // Update contract terms
            var principalAmount = new Money(model.PrincipalAmount, model.Currency);
            var interestRate = new Money(model.InterestRate, model.Currency);

            contract.UpdateTerms(
                principalAmount,
                interestRate,
                model.TermInMonths,
                model.StartDate,
                model.EndDate,
                model.PaymentFrequency,
                model.GracePeriodDays,
                model.LateFeePercentage,
                model.PenaltyPercentage,
                model.CollateralDescription,
                model.GuarantorName,
                model.GuarantorContact,
                model.Notes,
                model.StaffId
            );

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractDetailVM>(contract);
        }

        public async Task<ContractDetailVM> GetByIdAsync(Guid id)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(id);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {id} not found.");

            return _mapper.Map<ContractDetailVM>(contract);
        }

        public async Task<PagedResult<ContractListVM>> GetPagedAsync(
            string filter = null,
            string orderBy = "StartDate",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var contracts = await _unitOfWork.Contracts.GetPagedAsync(filter, orderBy, pageNumber, pageSize);

            var contractListVMs = _mapper.Map<List<ContractListVM>>(contracts.Items ?? new List<Contract>());

            return new PagedResult<ContractListVM>(contractListVMs, contracts.TotalCount, contracts.PageNumber, contracts.PageSize);
        }

        public async Task<ContractDetailVM> ActivateContractAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            contract.Activate();
            await _unitOfWork.SaveChangesAsync();

            // Generate installments if they don't exist
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            if (!installments.Any())
            {
                await GenerateInstallmentsAsync(contractId);
            }

            return _mapper.Map<ContractDetailVM>(contract);
        }

        public async Task<ContractDetailVM> SuspendContractAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            contract.Suspend();
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractDetailVM>(contract);
        }

        public async Task<ContractDetailVM> CompleteContractAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            // Validate all installments are paid
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            var unpaidInstallments = installments.Where(i => i.Status != InstallmentStatus.Paid);

            if (unpaidInstallments.Any())
                throw new InvalidOperationException("Cannot complete contract with unpaid installments.");

            contract.Complete();
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractDetailVM>(contract);
        }

        public async Task<ContractDetailVM> CancelContractAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            // Check if contract has payments
            var payments = await _unitOfWork.Payments.GetByContractIdAsync(contractId);
            if (payments.Any())
                throw new InvalidOperationException("Cannot cancel contract with existing payments.");

            contract.Cancel();
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractDetailVM>(contract);
        }

        public async Task<ContractDetailVM> DefaultContractAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            contract.Default();
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ContractDetailVM>(contract);
        }

        public async Task<bool> DeleteAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new KeyNotFoundException($"Contract with ID {contractId} not found.");

            // Only allow deletion of draft contracts
            if (contract.Status != ContractStatus.Draft)
                throw new InvalidOperationException("Only draft contracts can be permanently deleted.");

            // Check if contract has any payments or installments
            var payments = await _unitOfWork.Payments.GetByContractIdAsync(contractId);
            if (payments.Any())
                throw new InvalidOperationException("Cannot delete contract with existing payments.");

            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            if (installments.Any())
                throw new InvalidOperationException("Cannot delete contract with existing installments.");

            await _unitOfWork.Contracts.DeleteAsync(contract);
            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<List<InstallmentListVM>> GenerateInstallmentsAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            // Generate installments using domain method
            var installments = contract.GenerateInstallments();

            // Add installments to repository
            foreach (var installment in installments)
            {
                await _unitOfWork.Installments.AddAsync(installment);
            }

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<List<InstallmentListVM>>(installments);
        }

        public async Task<List<InstallmentListVM>> GetContractInstallmentsAsync(Guid contractId)
        {
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            return _mapper.Map<List<InstallmentListVM>>(installments);
        }

        public async Task<OutstandingAmountVM> RecalculateOutstandingAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            var outstandingAmount = contract.OutstandingAmount();
            return new OutstandingAmountVM
            {
                Amount = outstandingAmount.Amount,
                Currency = outstandingAmount.Currency
            };
        }

        public async Task<List<ContractListVM>> GetContractsByCustomerAsync(Guid customerId)
        {
            var contracts = await _unitOfWork.Contracts.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<ContractListVM>>(contracts);
        }

        public async Task<List<ContractListVM>> GetContractsByStatusAsync(ContractStatus status)
        {
            var contracts = await _unitOfWork.Contracts.GetByStatusAsync(status);
            return _mapper.Map<List<ContractListVM>>(contracts);
        }

        public async Task<PagedResult<ContractListVM>> GetContractsByStatusAsync(ContractStatus status, int pageNumber, int pageSize)
        {
            var contracts = await _unitOfWork.Contracts.GetByStatusPagedAsync(status, pageNumber, pageSize);
            return new PagedResult<ContractListVM>(
                _mapper.Map<List<ContractListVM>>(contracts.Items),
                contracts.TotalCount,
                pageNumber,
                pageSize
            );
        }

        public async Task<PagedResult<ContractListVM>> GetContractsByStatusAsync(ContractStatus status, int pageNumber, int pageSize, string filter, string orderBy)
        {
            var contracts = await _unitOfWork.Contracts.GetByStatusPagedAsync(status, pageNumber, pageSize, filter, orderBy);
            return new PagedResult<ContractListVM>(
                _mapper.Map<List<ContractListVM>>(contracts.Items),
                contracts.TotalCount,
                pageNumber,
                pageSize
            );
        }

        public async Task<List<ContractListVM>> GetOverdueContractsAsync()
        {
            var contracts = await _unitOfWork.Contracts.GetOverdueAsync();
            return _mapper.Map<List<ContractListVM>>(contracts);
        }

        public async Task<List<ContractListVM>> GetAllAsync()
        {
            var contracts = await _unitOfWork.Contracts.GetAllAsync();
            return _mapper.Map<List<ContractListVM>>(contracts);
        }

        public async Task<ContractFinancialSummaryVM> GetContractFinancialSummaryAsync(Guid contractId)
        {
            // Verify contract exists
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            // Get installment status summary from the installment service
            var installmentSummary = await _installmentService.GetInstallmentStatusSummaryAsync(contractId);

            // Map the installment summary to contract financial summary
            var financialSummary = _mapper.Map<ContractFinancialSummaryVM>(installmentSummary);

            // Add additional contract information
            financialSummary.ContractNumber = contract.ContractNumber;
            financialSummary.CustomerName = contract.Customer?.FirstName ?? "Unknown";
            financialSummary.Currency = contract.TotalAmount.Currency;

            // Calculate collection rate and payment percentage
            if (financialSummary.TotalAmount > 0)
            {
                financialSummary.CollectionRate = financialSummary.PaidAmount / financialSummary.TotalAmount;
                financialSummary.PaymentPercentage = financialSummary.PaidAmount / financialSummary.TotalAmount * 100;
            }

            // Calculate average payment amount
            if (financialSummary.PaidInstallments > 0)
            {
                financialSummary.AveragePaymentAmount = financialSummary.PaidAmount / financialSummary.PaidInstallments;
            }

            // Get the last payment date and next due date
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            var payments = await _unitOfWork.Payments.GetByContractIdAsync(contractId);

            if (payments.Any())
            {
                financialSummary.LastPaymentDate = payments.OrderByDescending(p => p.PaymentDate).FirstOrDefault()?.PaymentDate;
            }

            var nextDueInstallment = installments
                .Where(i => i.Status == InstallmentStatus.Pending)
                .OrderBy(i => i.DueDate)
                .FirstOrDefault();

            if (nextDueInstallment != null)
            {
                financialSummary.NextDueDate = nextDueInstallment.DueDate;
            }

            return financialSummary;
        }

        public async Task<ContractAnalyticsVM> GetContractAnalyticsAsync(Guid? contractId = null)
        {
            var analytics = new ContractAnalyticsVM();

            // Get all contracts or filter by contractId
            var contractsQuery = _unitOfWork.Contracts.Query();
            if (contractId.HasValue)
            {
                contractsQuery = contractsQuery.Where(c => c.Id == contractId.Value);
            }

            var contracts = contractsQuery.ToList();

            // Basic contract statistics
            analytics.TotalContracts = contracts.Count;
            analytics.ActiveContracts = contracts.Count(c => c.Status == ContractStatus.Active);
            analytics.CompletedContracts = contracts.Count(c => c.Status == ContractStatus.Completed);
            analytics.DefaultedContracts = contracts.Count(c => c.Status == ContractStatus.Defaulted);

            // Financial statistics
            analytics.TotalContractValue = contracts.Sum(c => c.TotalAmount.Amount);

            // Get all installments for these contracts
            var contractIds = contracts.Select(c => c.Id).ToList();
            var allInstallments = await _unitOfWork.Installments.GetByContractIdsAsync(contractIds);

            // Calculate collected and outstanding amounts
            analytics.CollectedAmount = allInstallments
                .Where(i => i.Status == InstallmentStatus.Paid)
                .Sum(i => i.Amount.Amount);

            analytics.OutstandingAmount = allInstallments
                .Where(i => i.Status != InstallmentStatus.Paid)
                .Sum(i => i.Amount.Amount);

            // Calculate collection rate
            if (analytics.TotalContractValue > 0)
            {
                analytics.CollectionRate = analytics.CollectedAmount / analytics.TotalContractValue;
            }

            // Overdue statistics
            var overdueInstallments = allInstallments.Where(i => i.Status == InstallmentStatus.Overdue).ToList();
            analytics.OverdueAmount = overdueInstallments.Sum(i => i.Amount.Amount);

            // Count unique contracts with overdue installments
            analytics.OverdueContractsCount = overdueInstallments
                .Select(i => i.ContractId)
                .Distinct()
                .Count();

            // Contracts by type
            analytics.ContractsByType = contracts
                .GroupBy(c => c.ContractType)
                .ToDictionary(g => g.Key, g => g.Count());

            // Amount by contract type
            analytics.AmountByContractType = contracts
                .GroupBy(c => c.ContractType)
                .ToDictionary(g => g.Key, g => g.Sum(c => c.TotalAmount.Amount));

            // Contracts by month (last 12 months)
            var last12Months = Enumerable.Range(0, 12)
                .Select(i => DateTime.UtcNow.AddMonths(-i).ToString("yyyy-MM"))
                .ToList();

            analytics.ContractsByMonth = last12Months
                .ToDictionary(
                    month => month,
                    month => contracts.Count(c => c.CreatedAt.ToString("yyyy-MM") == month)
                );

            // Collection by month (based on payment dates)
            var allPayments = await _unitOfWork.Payments.GetByContractIdsAsync(contractIds);
            analytics.CollectionByMonth = last12Months
                .ToDictionary(
                    month => month,
                    month => allPayments
                        .Where(p => p.PaymentDate.ToString("yyyy-MM") == month)
                        .Sum(p => p.Amount.Amount)
                );

            return analytics;
        }
    }

    public interface IContractService
    {
        Task<ContractDetailVM> CreateAsync(ContractCreateVM model);
        Task<ContractDetailVM> UpdateAsync(ContractUpdateVM model);
        Task<ContractDetailVM> GetByIdAsync(Guid id);
        Task<PagedResult<ContractListVM>> GetPagedAsync(string filter = null, string orderBy = "StartDate", int pageNumber = 1, int pageSize = 10);
        Task<ContractDetailVM> ActivateContractAsync(Guid contractId);
        Task<ContractDetailVM> SuspendContractAsync(Guid contractId);
        Task<ContractDetailVM> CompleteContractAsync(Guid contractId);
        Task<ContractDetailVM> CancelContractAsync(Guid contractId);
        Task<ContractDetailVM> DefaultContractAsync(Guid contractId);
        Task<bool> DeleteAsync(Guid contractId);
        Task<List<InstallmentListVM>> GenerateInstallmentsAsync(Guid contractId);
        Task<List<InstallmentListVM>> GetContractInstallmentsAsync(Guid contractId);
        Task<OutstandingAmountVM> RecalculateOutstandingAsync(Guid contractId);
        Task<List<ContractListVM>> GetContractsByCustomerAsync(Guid customerId);
        Task<List<ContractListVM>> GetContractsByStatusAsync(ContractStatus status);
        Task<PagedResult<ContractListVM>> GetContractsByStatusAsync(ContractStatus status, int pageNumber, int pageSize);
        Task<PagedResult<ContractListVM>> GetContractsByStatusAsync(ContractStatus status, int pageNumber, int pageSize, string filter, string orderBy);
        Task<List<ContractListVM>> GetOverdueContractsAsync();
        Task<List<ContractListVM>> GetAllAsync();
        Task<ContractFinancialSummaryVM> GetContractFinancialSummaryAsync(Guid contractId);
        Task<ContractAnalyticsVM> GetContractAnalyticsAsync(Guid? contractId = null);
    }

}
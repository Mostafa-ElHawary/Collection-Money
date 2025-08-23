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
// Remove unused EntityFrameworkCore using statement since it's not needed in this service layer

namespace CollectionApp.Application.Services
{
    public class LedgerService : ILedgerService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;

        public LedgerService(IUnitOfWork unitOfWork, IMapper mapper)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        public async Task<LedgerEntryDetailVM> CreateAsync(LedgerEntryCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Validate that an entry doesn't have both debit and credit amounts
            if (model.DebitAmount > 0 && model.CreditAmount > 0)
                throw new InvalidOperationException("Cannot have both debit and credit amounts in the same entry.");

            // Create Money value objects
            var debitAmount = new Money(model.DebitAmount, model.Currency);
            var creditAmount = new Money(model.CreditAmount, model.Currency);
            var balance = new Money(model.Balance, model.Currency);

            // Create ledger entry entity
            var ledgerEntry = new LedgerEntry(
                model.TransactionDate,
                model.Description,
                debitAmount,
                creditAmount,
                balance,
                model.ReferenceType ?? string.Empty,
                model.ReferenceId,
                model.ContractId,
                model.CustomerId,
                model.StaffId
            );

            await _unitOfWork.LedgerEntries.AddAsync(ledgerEntry);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LedgerEntryDetailVM>(ledgerEntry);
        }

        private static object GetEntryType(LedgerEntryCreateVM model)
        {
            return model.EntryType;
        }

        public async Task<LedgerEntryDetailVM> UpdateAsync(LedgerEntryUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var ledgerEntry = await _unitOfWork.LedgerEntries.GetByIdAsync(model.Id);
            if (ledgerEntry == null)
                throw new InvalidOperationException($"Ledger entry with ID {model.Id} not found.");
            
            // Only allow limited updates for audit compliance
            ledgerEntry.Description = model.Description;
            
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<LedgerEntryDetailVM>(ledgerEntry);
        }

        public async Task<LedgerEntryDetailVM> GetByIdAsync(Guid id)
        {
            var ledgerEntry = await _unitOfWork.LedgerEntries.GetByIdAsync(id);
            if (ledgerEntry == null)
                throw new InvalidOperationException($"Ledger entry with ID {id} not found.");

            return _mapper.Map<LedgerEntryDetailVM>(ledgerEntry);
        }

        public async Task<PagedResult<LedgerEntryListVM>> GetPagedAsync(
            string filter = null,
            string orderBy = "TransactionDate",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetPagedAsync(filter, orderBy, pageNumber, pageSize);
            
            var ledgerEntryListVMs = _mapper.Map<List<LedgerEntryListVM>>(ledgerEntries.Items);
            
            return new PagedResult<LedgerEntryListVM>(ledgerEntryListVMs.AsReadOnly(), ledgerEntries.TotalCount, ledgerEntries.PageNumber, ledgerEntries.PageSize);
        }

        public async Task<LedgerEntryDetailVM> CreateDebitEntryAsync(decimal amount, string currency, string description, string referenceType, Guid referenceId, Guid? contractId = null, Guid? customerId = null, Guid? staffId = null)
        {
            var debitEntry = new LedgerEntryCreateVM
            {
                TransactionDate = DateTime.Now,
                DebitAmount = amount,
                CreditAmount = 0,
                Balance = amount, // Initial balance is the debit amount
                Currency = currency,
                Description = description,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                ContractId = contractId,
                CustomerId = customerId,
                StaffId = staffId
            };

            return await CreateAsync(debitEntry);
        }

        public async Task<LedgerEntryDetailVM> CreateCreditEntryAsync(decimal amount, string currency, string description, string referenceType, Guid referenceId, Guid? contractId = null, Guid? customerId = null, Guid? staffId = null)
        {
            var creditEntry = new LedgerEntryCreateVM
            {
                TransactionDate = DateTime.Now,
                DebitAmount = 0,
                CreditAmount = amount,
                Balance = -amount, // Initial balance is negative of the credit amount
                Currency = currency,
                Description = description,
                ReferenceType = referenceType,
                ReferenceId = referenceId,
                ContractId = contractId,
                CustomerId = customerId,
                StaffId = staffId
            };

            return await CreateAsync(creditEntry);
        }

        public async Task<List<LedgerEntryDetailVM>> CreateJournalEntryAsync(decimal debitAmount, decimal creditAmount, string description, string currency = "USD", Guid? contractId = null, Guid? customerId = null, Guid? staffId = null)
        {
            if (Math.Abs(debitAmount - creditAmount) > 0.01m)
                throw new InvalidOperationException("Debit and credit amounts must be equal for journal entries.");

            var entries = new List<LedgerEntryDetailVM>();

            // Create debit entry
            var debitEntry = await CreateDebitEntryAsync(debitAmount, currency, $"{description} - Debit", "Journal", Guid.Empty, contractId, customerId, staffId);
            entries.Add(debitEntry);

            // Create credit entry
            var creditEntry = await CreateCreditEntryAsync(creditAmount, currency, $"{description} - Credit", "Journal", Guid.Empty, contractId, customerId, staffId);
            entries.Add(creditEntry);

            return entries;
        }

        public async Task<List<LedgerEntryDetailVM>> PostPaymentEntryAsync(Guid paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {paymentId} not found.");

            var entries = new List<LedgerEntryDetailVM>();

            // Create debit entry for cash/bank account
            var debitEntry = await CreateDebitEntryAsync(
                payment.Amount.Amount,
                payment.Amount.Currency,
                $"Payment received - {payment.ReferenceNumber}",
                "Payment",
                payment.Id,
                payment.ContractId,
                null,
                payment.StaffId
            );
            entries.Add(debitEntry);

            // Create credit entry for accounts receivable
            var creditEntry = await CreateCreditEntryAsync(
                payment.Amount.Amount,
                payment.Amount.Currency,
                $"Payment applied - {payment.ReferenceNumber}",
                "Contract",
                payment.ContractId,
                payment.ContractId,
                null,
                payment.StaffId
            );
            entries.Add(creditEntry);

            return entries;
        }

        public async Task<List<LedgerEntryDetailVM>> PostContractEntryAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            var entries = new List<LedgerEntryDetailVM>();

            // Create debit entry for accounts receivable
            var debitEntry = await CreateDebitEntryAsync(
                contract.PrincipalAmount.Amount,
                contract.PrincipalAmount.Currency,
                $"Contract created - {contract.ContractNumber}",
                "Contract",
                contract.Id,
                contract.Id,
                contract.CustomerId,
                contract.StaffId
            );
            entries.Add(debitEntry);

            // Create credit entry for contract liability
            var creditEntry = await CreateCreditEntryAsync(
                contract.PrincipalAmount.Amount,
                contract.PrincipalAmount.Currency,
                $"Contract liability - {contract.ContractNumber}",
                "Contract",
                contract.Id,
                contract.Id,
                contract.CustomerId,
                contract.StaffId
            );
            entries.Add(creditEntry);

            return entries;
        }

        public async Task<decimal> GetAccountBalanceAsync(Guid? customerId = null, Guid? contractId = null)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByReferenceAsync(customerId, contractId);
            
            var balance = 0m;
            foreach (var entry in ledgerEntries)
            {
                balance += entry.DebitAmount.Amount;
                balance -= entry.CreditAmount.Amount;
            }

            return balance;
        }

        public async Task<decimal> GetRunningBalanceAsync(DateTime asOfDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(DateTime.MinValue, asOfDate);
            
            var balance = 0m;
            foreach (var entry in ledgerEntries.OrderBy(e => e.TransactionDate))
            {
                balance += entry.DebitAmount.Amount;
                balance -= entry.CreditAmount.Amount;
            }

            return balance;
        }

        public async Task<bool> ReconcileLedgerAsync(DateTime fromDate, DateTime toDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(fromDate, toDate);
            
            var totalDebits = ledgerEntries.Sum(e => e.DebitAmount.Amount);
            var totalCredits = ledgerEntries.Sum(e => e.CreditAmount.Amount);
            
            var difference = Math.Abs(totalDebits - totalCredits);
            
            // If difference is significant, create balancing entry
            if (difference > 0.01m)
            {
                if (totalDebits > totalCredits)
                {
                    await CreateCreditEntryAsync(difference, "USD", "Reconciliation adjustment", "System", Guid.Empty);
                }
                else
                {
                    await CreateDebitEntryAsync(difference, "USD", "Reconciliation adjustment", "System", Guid.Empty);
                }
            }

            return true;
        }

        public async Task<bool> ValidateLedgerIntegrityAsync()
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetAllAsync();
            
            var totalDebits = ledgerEntries.Sum(e => e.DebitAmount.Amount);
            var totalCredits = ledgerEntries.Sum(e => e.CreditAmount.Amount);
            
            var difference = Math.Abs(totalDebits - totalCredits);
            
            return difference <= 0.01m; // Allow for small rounding differences
        }

        public async Task<LedgerAnalyticsViewModels.TrialBalanceVM> GetTrialBalanceAsync(DateTime asOfDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(DateTime.MinValue, asOfDate);
            
            var totalDebits = ledgerEntries.Sum(e => e.DebitAmount.Amount);
            var totalCredits = ledgerEntries.Sum(e => e.CreditAmount.Amount);
            var difference = totalDebits - totalCredits;
            
            var trialBalance = new LedgerAnalyticsViewModels.TrialBalanceVM
            {
                AsOfDate = asOfDate,
                TotalDebits = totalDebits,
                TotalCredits = totalCredits,
                Difference = difference,
                IsBalanced = Math.Abs(difference) <= 0.01m
            };

            return trialBalance;
        }

        public async Task<List<LedgerEntryListVM>> GetLedgerByCustomerAsync(Guid customerId)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByReferenceAsync(customerId, null);
            return _mapper.Map<List<LedgerEntryListVM>>(ledgerEntries);
        }

        public async Task<List<LedgerEntryListVM>> GetLedgerByContractAsync(Guid contractId)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByReferenceAsync(null, contractId);
            return _mapper.Map<List<LedgerEntryListVM>>(ledgerEntries);
        }

        public async Task<List<LedgerEntryListVM>> GetLedgerByDateRangeAsync(DateTime fromDate, DateTime toDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(fromDate, toDate);
            return _mapper.Map<List<LedgerEntryListVM>>(ledgerEntries);
        }

        public async Task<LedgerAnalyticsViewModels.CashFlowReportVM> GetCashFlowReportAsync(DateTime fromDate, DateTime toDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(fromDate, toDate);
            
            var cashFlow = new LedgerAnalyticsViewModels.CashFlowReportVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                CashInflows = ledgerEntries.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.DebitAmount.Amount),
                CashOutflows = ledgerEntries.Where(e => e.CreditAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.CreditAmount.Amount),
                NetCashFlow = ledgerEntries.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.DebitAmount.Amount) -
                             ledgerEntries.Where(e => e.CreditAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.CreditAmount.Amount),
                CashFlowByMonth = ledgerEntries.GroupBy(e => new { e.TransactionDate.Year, e.TransactionDate.Month })
                                             .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:D2}", g => 
                                                 g.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.DebitAmount.Amount) -
                                                 g.Where(e => e.CreditAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.CreditAmount.Amount))
            };

            return cashFlow;
        }

        public async Task<LedgerAnalyticsViewModels.AccountsReceivableVM> GetAccountsReceivableAsync(DateTime asOfDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(DateTime.MinValue, asOfDate);
            
            var accountsReceivable = ledgerEntries
                .Where(e => e.ReferenceType == "Contract" && e.ReferenceId.HasValue)
                .GroupBy(e => e.ReferenceId.Value)
                .Select(g => new LedgerAnalyticsViewModels.AccountReceivableItemVM
                {
                    ContractId = g.Key,
                    TotalAmount = g.Sum(e => e.DebitAmount.Amount) - g.Sum(e => e.CreditAmount.Amount),
                    LastPaymentDate = g.Where(e => e.CreditAmount.Amount > 0 && e.ReferenceType == "Payment")
                                      .OrderByDescending(e => e.TransactionDate)
                                      .Select(e => e.TransactionDate)
                                      .FirstOrDefault()
                })
                .Where(ar => ar.TotalAmount > 0)
                .ToList();
                
            var report = new LedgerAnalyticsViewModels.AccountsReceivableVM
            {
                AsOfDate = asOfDate,
                TotalReceivables = accountsReceivable.Sum(ar => ar.TotalAmount),
                AccountsReceivable = accountsReceivable
            };
            
            return report;
        }

        public async Task<List<LedgerAnalyticsViewModels.AuditTrailVM>> GetAuditTrailAsync(Guid referenceId, string referenceType)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByReferenceAsync(null, null, referenceType, referenceId);
            return _mapper.Map<List<LedgerAnalyticsViewModels.AuditTrailVM>>(ledgerEntries);
        }

        public async Task<bool> ValidateEntryIntegrityAsync(Guid entryId)
        {
            var ledgerEntry = await _unitOfWork.LedgerEntries.GetByIdAsync(entryId);
            if (ledgerEntry == null)
                return false;

            // Validate reference exists
            switch (ledgerEntry.ReferenceType)
            {
                case "Payment":
                    if (ledgerEntry.ReferenceId != Guid.Empty)
                    {
                        var payment = await _unitOfWork.Payments.GetByIdAsync(ledgerEntry.ReferenceId.Value);
                        return payment != null;
                    }
                    return false;
                case "Contract":
                    if (ledgerEntry.ReferenceId != Guid.Empty)
                    {
                        var contract = await _unitOfWork.Contracts.GetByIdAsync(ledgerEntry.ReferenceId.Value);
                        return contract != null;
                    }
                    return false;
                case "Customer":
                    if (ledgerEntry.ReferenceId != Guid.Empty)
                    {
                        var customer = await _unitOfWork.Customers.GetByIdAsync(ledgerEntry.ReferenceId.Value);
                        return customer != null;
                    }
                    return false;
                default:
                    return true; // System entries don't need validation
            }
        }

        public async Task<List<LedgerEntryListVM>> GetUnbalancedEntriesAsync()
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetAllAsync();
            var unbalancedEntries = new List<LedgerEntryListVM>();

            // Group entries by date and check for balance
            var entriesByDate = ledgerEntries.GroupBy(e => e.TransactionDate.Date);
            
            foreach (var dateGroup in entriesByDate)
            {
                var totalDebits = dateGroup.Sum(e => e.DebitAmount.Amount);
                var totalCredits = dateGroup.Sum(e => e.CreditAmount.Amount);
                
                if (Math.Abs(totalDebits - totalCredits) > 0.01m)
                {
                    var dateEntries = _mapper.Map<List<LedgerEntryListVM>>(dateGroup);
                    unbalancedEntries.AddRange(dateEntries);
                }
            }

            return unbalancedEntries;
        }

        public async Task<bool> ArchiveLedgerEntriesAsync(DateTime beforeDate)
        {
            var oldEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(DateTime.MinValue, beforeDate);
            
            foreach (var entry in oldEntries)
            {
                entry.Archive();
            }

            await _unitOfWork.SaveChangesAsync();

            return true;
        }

        public async Task<string> ExportLedgerAsync(DateTime fromDate, DateTime toDate, string format)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(fromDate, toDate);
            
            // In a real implementation, you would export to the specified format
            // For now, we'll return a simple CSV format
            var csvContent = "TransactionDate,DebitAmount,CreditAmount,Currency,Description,ReferenceType,ReferenceId\n";
            
            foreach (var entry in ledgerEntries.OrderBy(e => e.TransactionDate))
            {
                string currency = entry.DebitAmount.Amount > 0 ? entry.DebitAmount.Currency : entry.CreditAmount.Currency;
                csvContent += $"{entry.TransactionDate:yyyy-MM-dd},{entry.DebitAmount.Amount},{entry.CreditAmount.Amount},{currency}," +
                             $"\"{entry.Description}\",{entry.ReferenceType},{entry.ReferenceId}\n";
            }

            return csvContent;
        }

        public async Task<bool> ProcessPaymentLedgerAsync(Payment payment)
        {
            // Create all ledger entries for payment processing
            await PostPaymentEntryAsync(payment.Id);
            return true;
        }

        public async Task<bool> ProcessRefundLedgerAsync(Guid paymentId, decimal refundAmount, string reason)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {paymentId} not found.");

            // Create credit entry for refund
            await CreateCreditEntryAsync(
                refundAmount,
                payment.Amount.Currency,
                $"Refund: {reason}",
                "Payment",
                paymentId,
                payment.ContractId,
                null,
                payment.StaffId
            );

            // Create debit entry for accounts receivable
            await CreateDebitEntryAsync(
                refundAmount,
                payment.Amount.Currency,
                $"Refund applied: {reason}",
                "Contract",
                payment.ContractId,
                payment.ContractId,
                null,
                payment.StaffId
            );

            return true;
        }

        public async Task<bool> ProcessWriteOffAsync(Guid contractId, decimal amount, string reason)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            // Create credit entry for write-off
            await CreateCreditEntryAsync(
                amount,
                contract.PrincipalAmount.Currency,
                $"Write-off: {reason}",
                "Contract",
                contractId,
                contractId,
                contract.CustomerId,
                contract.StaffId
            );

            // Create debit entry for bad debt expense
            await CreateDebitEntryAsync(
                amount,
                "USD",
                $"Bad debt expense: {reason}",
                "System",
                Guid.Empty,
                contractId,
                contract.CustomerId,
                contract.StaffId
            );

            return true;
        }

        public async Task<bool> ProcessAdjustmentAsync(Guid contractId, decimal amount, string reason, string type)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {contractId} not found.");

            if (type == "Debit")
            {
                await CreateDebitEntryAsync(
                    amount, 
                    contract.PrincipalAmount.Currency, 
                    $"Adjustment: {reason}", 
                    "Contract", 
                    contractId,
                    contractId,
                    contract.CustomerId,
                    contract.StaffId
                );
            }
            else if (type == "Credit")
            {
                await CreateCreditEntryAsync(
                    amount, 
                    contract.PrincipalAmount.Currency, 
                    $"Adjustment: {reason}", 
                    "Contract", 
                    contractId,
                    contractId,
                    contract.CustomerId,
                    contract.StaffId
                );
            }

            return true;
        }

        public async Task<bool> SyncWithPaymentServiceAsync(Guid paymentId)
        {
            // Ensure ledger sync with payment records
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                return false;

            // Check if ledger entries already exist
            var existingEntries = await _unitOfWork.LedgerEntries.GetByReferenceAsync(null, null, "Payment", paymentId);
            if (!existingEntries.Any())
            {
                await PostPaymentEntryAsync(paymentId);
            }

            return true;
        }

        public async Task<bool> SyncWithContractServiceAsync(Guid contractId)
        {
            // Ensure ledger sync with contract records
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract == null)
                return false;

            // Check if ledger entries already exist
            var existingEntries = await _unitOfWork.LedgerEntries.GetByReferenceAsync(null, null, "Contract", contractId);
            if (!existingEntries.Any())
            {
                await PostContractEntryAsync(contractId);
            }

            return true;
        }

        public async Task<bool> ValidateAgainstExternalSystemAsync()
        {
            // In a real implementation, you would validate against external accounting system
            // For now, we'll just return true
            return true;
        }

        public async Task<LedgerAnalyticsViewModels.CollectionEfficiencyVM> GetCollectionEfficiencyAsync(DateTime fromDate, DateTime toDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(fromDate, toDate);
            
            var totalCollections = ledgerEntries.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.DebitAmount.Amount);
            var totalReceivables = ledgerEntries.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "Contract").Sum(e => e.DebitAmount.Amount);
            
            var efficiency = new LedgerAnalyticsViewModels.CollectionEfficiencyVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalCollections = totalCollections,
                TotalReceivables = totalReceivables,
                CollectionRate = totalReceivables > 0 ? (totalCollections / totalReceivables) * 100 : 0
            };

            return efficiency;
        }

        public async Task<LedgerAnalyticsViewModels.PaymentTrendsVM> GetPaymentTrendsAsync(DateTime fromDate, DateTime toDate)
        {
            var ledgerEntries = await _unitOfWork.LedgerEntries.GetByDateRangeAsync(fromDate, toDate);
            
            var paymentsQuery = ledgerEntries.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "Payment");
            var totalPayments = paymentsQuery.Sum(e => e.DebitAmount.Amount);
            var averagePayment = paymentsQuery.Any() ? paymentsQuery.Average(e => e.DebitAmount.Amount) : 0;
            
            var trends = new LedgerAnalyticsViewModels.PaymentTrendsVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalPayments = totalPayments,
                AveragePaymentAmount = averagePayment,
                PaymentTrendsByMonth = paymentsQuery
                                       .GroupBy(e => new { e.TransactionDate.Year, e.TransactionDate.Month })
                                       .ToDictionary(g => $"{g.Key.Year}-{g.Key.Month:D2}", g => g.Sum(e => e.DebitAmount.Amount))
            };

            return trends;
        }

        public async Task<LedgerAnalyticsViewModels.OutstandingAnalysisVM> GetOutstandingAnalysisAsync(DateTime? startDate = null, DateTime? endDate = null)
    {
        var query = _unitOfWork.LedgerEntries.Query();
        
        if (startDate.HasValue)
            query = query.Where(e => e.TransactionDate >= startDate.Value);
        
        if (endDate.HasValue)
            query = query.Where(e => e.TransactionDate <= endDate.Value);

var entries = await Task.FromResult(query.ToList());
        
        var result = new LedgerAnalyticsViewModels.OutstandingAnalysisVM
        {
            TotalOutstanding = entries.Sum(e => e.DebitAmount.Amount - e.CreditAmount.Amount),
                
            AgeingBuckets = new Dictionary<string, decimal>
            {
                { "0-30 days", 0 },
                { "31-60 days", 0 },
                { "61-90 days", 0 },
                { "91+ days", 0 }
            }
        };

        var today = DateTime.Today;
        
        foreach (var entry in entries)
        {
            var daysDifference = (today - entry.TransactionDate).Days;
            var amount = entry.DebitAmount.Amount - entry.CreditAmount.Amount;
            
            if (daysDifference <= 30)
                result.AgeingBuckets["0-30 days"] += amount;
            else if (daysDifference <= 60)
                result.AgeingBuckets["31-60 days"] += amount;
            else if (daysDifference <= 90)
                result.AgeingBuckets["61-90 days"] += amount;
            else
                result.AgeingBuckets["91+ days"] += amount;
        }

        return result;
    }

        public async Task<LedgerAnalyticsViewModels.ProfitabilityAnalysisVM> GetProfitabilityAnalysisAsync(DateTime fromDate, DateTime toDate)
        {
            var query = _unitOfWork.LedgerEntries.Query()
                .Where(e => e.TransactionDate >= fromDate && e.TransactionDate <= toDate);
            var ledgerEntries = query.ToList();
            
            var revenue = ledgerEntries.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "Payment").Sum(e => e.DebitAmount.Amount);
            var expenses = ledgerEntries.Where(e => e.CreditAmount.Amount > 0 && e.ReferenceType == "System").Sum(e => e.CreditAmount.Amount);
            var badDebt = ledgerEntries.Where(e => e.DebitAmount.Amount > 0 && e.ReferenceType == "System").Sum(e => e.DebitAmount.Amount);
            var netProfit = revenue - expenses - badDebt;
            var profitMargin = revenue > 0 ? (netProfit / revenue) * 100 : 0;
            
            var profitability = new LedgerAnalyticsViewModels.ProfitabilityAnalysisVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalRevenue = revenue,
                TotalExpenses = expenses,
                BadDebt = badDebt,
                NetProfit = netProfit,
                ProfitMargin = profitMargin,
                Revenue = new Dictionary<string, decimal>(),
                Expenses = new Dictionary<string, decimal>()
            };
            
            // Populate the Revenue and Expenses dictionaries
            profitability.Revenue.Add("Payments", revenue);
            profitability.Expenses.Add("Operating Expenses", expenses);

            return profitability;
        }
    }

    public interface ILedgerService
    {
        Task<LedgerEntryDetailVM> CreateAsync(LedgerEntryCreateVM model);
        Task<LedgerEntryDetailVM> UpdateAsync(LedgerEntryUpdateVM model);
        Task<LedgerEntryDetailVM> GetByIdAsync(Guid id);
        Task<PagedResult<LedgerEntryListVM>> GetPagedAsync(string filter = null, string orderBy = "TransactionDate", int pageNumber = 1, int pageSize = 10);
        Task<LedgerEntryDetailVM> CreateDebitEntryAsync(decimal amount, string currency, string description, string referenceType, Guid referenceId, Guid? contractId = null, Guid? customerId = null, Guid? staffId = null);
        Task<LedgerEntryDetailVM> CreateCreditEntryAsync(decimal amount, string currency, string description, string referenceType, Guid referenceId, Guid? contractId = null, Guid? customerId = null, Guid? staffId = null);
        Task<List<LedgerEntryDetailVM>> CreateJournalEntryAsync(decimal debitAmount, decimal creditAmount, string description, string currency = "USD", Guid? contractId = null, Guid? customerId = null, Guid? staffId = null);
        Task<List<LedgerEntryDetailVM>> PostPaymentEntryAsync(Guid paymentId);
        Task<List<LedgerEntryDetailVM>> PostContractEntryAsync(Guid contractId);
        Task<decimal> GetAccountBalanceAsync(Guid? customerId = null, Guid? contractId = null);
        Task<decimal> GetRunningBalanceAsync(DateTime asOfDate);
        Task<bool> ReconcileLedgerAsync(DateTime fromDate, DateTime toDate);
        Task<bool> ValidateLedgerIntegrityAsync();
        Task<LedgerAnalyticsViewModels.TrialBalanceVM> GetTrialBalanceAsync(DateTime asOfDate);
        Task<List<LedgerEntryListVM>> GetLedgerByCustomerAsync(Guid customerId);
        Task<List<LedgerEntryListVM>> GetLedgerByContractAsync(Guid contractId);
        Task<List<LedgerEntryListVM>> GetLedgerByDateRangeAsync(DateTime fromDate, DateTime toDate);
        Task<LedgerAnalyticsViewModels.CashFlowReportVM> GetCashFlowReportAsync(DateTime fromDate, DateTime toDate);
        Task<LedgerAnalyticsViewModels.AccountsReceivableVM> GetAccountsReceivableAsync(DateTime asOfDate);
        Task<List<LedgerAnalyticsViewModels.AuditTrailVM>> GetAuditTrailAsync(Guid referenceId, string referenceType);
        Task<bool> ValidateEntryIntegrityAsync(Guid entryId);
        Task<List<LedgerEntryListVM>> GetUnbalancedEntriesAsync();
        Task<bool> ArchiveLedgerEntriesAsync(DateTime beforeDate);
        Task<string> ExportLedgerAsync(DateTime fromDate, DateTime toDate, string format);
        Task<bool> ProcessPaymentLedgerAsync(Payment payment);
        Task<bool> ProcessRefundLedgerAsync(Guid paymentId, decimal refundAmount, string reason);
        Task<bool> ProcessWriteOffAsync(Guid contractId, decimal amount, string reason);
        Task<bool> ProcessAdjustmentAsync(Guid contractId, decimal amount, string reason, string type);
        Task<bool> SyncWithPaymentServiceAsync(Guid paymentId);
        Task<bool> SyncWithContractServiceAsync(Guid contractId);
        Task<bool> ValidateAgainstExternalSystemAsync();
        Task<LedgerAnalyticsViewModels.CollectionEfficiencyVM> GetCollectionEfficiencyAsync(DateTime fromDate, DateTime toDate);
        Task<LedgerAnalyticsViewModels.PaymentTrendsVM> GetPaymentTrendsAsync(DateTime fromDate, DateTime toDate);
        Task<LedgerAnalyticsViewModels.OutstandingAnalysisVM> GetOutstandingAnalysisAsync(DateTime? startDate = null, DateTime? endDate = null);
        Task<LedgerAnalyticsViewModels.ProfitabilityAnalysisVM> GetProfitabilityAnalysisAsync(DateTime fromDate, DateTime toDate);
    }
}
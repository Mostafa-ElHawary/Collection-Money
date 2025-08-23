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
using Microsoft.Extensions.Logging;
using System.Globalization;
using static CollectionApp.Application.ViewModels.InstallmentAnalyticsViewModels;

namespace CollectionApp.Application.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IUnitOfWork _unitOfWork;
        private readonly IMapper _mapper;
        private readonly ILogger<PaymentService> _logger;

        public PaymentService(IUnitOfWork unitOfWork, IMapper mapper, ILogger<PaymentService> logger)
        {
            _unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        public async Task<PaymentDetailVM> ProcessPaymentAsync(PaymentCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            _logger.LogInformation("Starting payment processing for contract {ContractId}, installment {InstallmentId}, amount {Amount} {Currency}",
                model.ContractId, model.InstallmentId, model.Amount, model.Currency);

            return await _unitOfWork.ExecuteInTransactionAsync(async (cancellationToken) =>
            {
                try
                {
                    // Validate contract exists and is active
                    var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(model.ContractId);
                    if (contract == null)
                    {
                        _logger.LogWarning("Contract not found during payment processing: {ContractId}", model.ContractId);
                        throw new InvalidOperationException($"Contract with ID {model.ContractId} not found.");
                    }

                    if (contract.Status != Domain.Enums.ContractStatus.Active)
                    {
                        _logger.LogWarning("Cannot process payment for contract in {Status} status: {ContractId}", contract.Status, model.ContractId);
                        throw new InvalidOperationException($"Cannot process payment for contract in {contract.Status} status.");
                    }

                    // Validate payment date
                    if (model.PaymentDate > DateTime.UtcNow.Date)
                    {
                        _logger.LogWarning("Payment date cannot be in the future: {PaymentDate}", model.PaymentDate);
                        throw new InvalidOperationException("Payment date cannot be in the future.");
                    }

                    // Validate staff exists
                    var staffId = model.ProcessedByStaffId ?? model.StaffId;
                    if (!staffId.HasValue)
                    {
                        _logger.LogWarning("Staff ID is required for payment processing");
                        throw new InvalidOperationException("ProcessedByStaffId or StaffId is required for payment processing.");
                    }

                    var staff = await _unitOfWork.Staff.GetByIdAsync(staffId.Value);
                    if (staff == null)
                    {
                        _logger.LogWarning("Staff not found during payment processing: {StaffId}", staffId.Value);
                        throw new InvalidOperationException($"Staff with ID {staffId.Value} not found.");
                    }

                    // Create Money value object
                    var amount = new Money(model.Amount, model.Currency);

                    // Create payment entity
                    var payment = new Payment(
                        model.ContractId,
                        model.InstallmentId,
                        amount,
                        model.PaymentDate,
                        model.PaymentMethod,
                        model.ReferenceNumber,
                        model.Notes,
                        staffId.Value
                    );

                    await _unitOfWork.Payments.AddAsync(payment);
                    _logger.LogInformation("Payment entity created with ID {PaymentId}", payment.Id);

                    // Update installment status
                    await UpdateInstallmentStatusAsync(model.InstallmentId, payment.Id, model.Amount, model.PaymentDate);

                    // Generate receipt
                    var receipt = await GenerateReceiptAsync(payment.Id);
                    _logger.LogInformation("Receipt generated with number {ReceiptNumber}", receipt.ReceiptNumber);

                    // Create ledger entries
                    await CreateLedgerEntriesAsync(payment);

                    // Update contract balance
                    await UpdateContractBalanceAsync(model.ContractId);

                    // Save all changes at once
                    await _unitOfWork.SaveChangesAsync(false);

                    _logger.LogInformation("Payment processing completed successfully for payment {PaymentId}", payment.Id);
                    return _mapper.Map<PaymentDetailVM>(payment);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred during payment processing for contract {ContractId}, installment {InstallmentId}",
                        model.ContractId, model.InstallmentId);
                    throw;
                }
            });
        }

        public async Task<PaymentDetailVM> UpdatePaymentAsync(PaymentUpdateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            var payment = await _unitOfWork.Payments.GetByIdAsync(model.Id);
            if (payment == null)
                throw new InvalidOperationException($"Payment with ID {model.Id} not found.");

            // Only allow limited updates for audit compliance
            payment.UpdateReferenceNumber(model.ReferenceNumber);
            payment.UpdateNotes(model.Notes);

            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentDetailVM>(payment);
        }

        public async Task<PaymentDetailVM> GetByIdAsync(Guid id)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(id);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {id} not found.");

            return _mapper.Map<PaymentDetailVM>(payment);
        }

        public async Task<PagedResult<PaymentListVM>> GetPagedAsync(
            string filter = null,
            string orderBy = "PaymentDate",
            int pageNumber = 1,
            int pageSize = 10)
        {
            var payments = await _unitOfWork.Payments.GetPagedAsync(filter, orderBy, pageNumber, pageSize);

            var paymentListVMs = _mapper.Map<List<PaymentListVM>>(payments.Items);

            return new PagedResult<PaymentListVM>(paymentListVMs.AsReadOnly(), payments.TotalCount, payments.PageNumber, payments.PageSize);
        }

        public async Task<PaymentDetailVM> ProcessInstallmentPaymentAsync(Guid installmentId, PaymentCreateVM model)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment == null)
                throw new InvalidOperationException($"Installment with ID {installmentId} not found.");

            // Validate staff exists
            var staffId = model.ProcessedByStaffId ?? model.StaffId;
            if (!staffId.HasValue)
                throw new InvalidOperationException("ProcessedByStaffId or StaffId is required for payment processing.");

            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId.Value);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId.Value} not found.");

            // Validate payment amount
            var installmentAmount = installment.Amount.Amount;
            var paymentAmount = model.Amount;

            if (paymentAmount > installmentAmount)
                throw new InvalidOperationException($"Payment amount {paymentAmount} exceeds installment amount {installmentAmount}.");

            // Create payment using the private method
            var payment = await CreatePaymentAsync(model);

            // Update installment status
            if (paymentAmount >= installmentAmount)
            {
                installment.MarkAsPaid(payment.PaymentDate);
            }
            else
            {
                installment.UpdatePartialPayment(paymentAmount);
            }

            await _unitOfWork.SaveChangesAsync();

            return payment;
        }

        public async Task<IReadOnlyList<PaymentDetailVM>> ProcessPartialPaymentAsync(Guid contractId, PaymentCreateVM model)
        {
            // Validate staff exists
            var staffId = model.ProcessedByStaffId ?? model.StaffId;
            if (!staffId.HasValue)
                throw new InvalidOperationException("ProcessedByStaffId or StaffId is required for payment processing.");

            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId.Value);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId.Value} not found.");

            // Get unpaid installments for the contract
            var installments = await _unitOfWork.Installments.GetByContractIdAsync(contractId);
            var unpaidInstallments = installments.Where(i => i.Status != InstallmentStatus.Paid).OrderBy(i => i.DueDate);

            if (!unpaidInstallments.Any())
                throw new InvalidOperationException("No unpaid installments found for this contract.");

            var remainingAmount = model.Amount;
            var payments = new List<PaymentDetailVM>();

            foreach (var installment in unpaidInstallments)
            {
                if (remainingAmount <= 0) break;

                var installmentRemainingAmount = installment.RemainingAmount;
                var paymentAmount = Math.Min(remainingAmount, installmentRemainingAmount);

                // Create payment for this installment
                var referenceNumber = $"{model.ReferenceNumber}-{installment.InstallmentNumber}";

                // Truncate reference number if it exceeds 100 characters
                if (referenceNumber.Length > 100)
                {
                    var baseRef = model.ReferenceNumber ?? "";
                    var maxBaseLength = 100 - installment.InstallmentNumber.ToString().Length - 1; // -1 for the dash
                    var truncatedBase = baseRef.Length > maxBaseLength ? baseRef.Substring(0, maxBaseLength) : baseRef;
                    referenceNumber = $"{truncatedBase}-{installment.InstallmentNumber}";
                }

                var paymentModel = new PaymentCreateVM
                {
                    ContractId = contractId,
                    InstallmentId = installment.Id,
                    Amount = paymentAmount,
                    Currency = model.Currency,
                    PaymentMethod = model.PaymentMethod,
                    ReferenceNumber = referenceNumber,
                    ProcessedByStaffId = model.ProcessedByStaffId,
                    Notes = $"Partial payment: {model.Notes}"
                };

                var payment = await CreatePaymentAsync(paymentModel);
                payments.Add(payment);

                remainingAmount -= paymentAmount;
            }

            return payments.AsReadOnly();
        }

        public async Task<bool> ReversePaymentAsync(Guid paymentId, string reason)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

            return await _unitOfWork.ExecuteInTransactionAsync(async (cancellationToken) =>
            {
                // Create reversal payment with positive amount but marked as reversal
                var reversalPayment = new Payment(
                    payment.ContractId,
                    payment.InstallmentId,
                    payment.Amount, // Use positive amount
                    DateTime.UtcNow,
                    payment.PaymentMethod,
                    payment.ProcessedByStaffId ?? payment.StaffId,
                    true, // Mark as reversal
                    $"REV-{payment.ReferenceNumber}",
                    $"Reversal: {reason}"
                );

                await _unitOfWork.Payments.AddAsync(reversalPayment);

                // Update installment status
                var installment = await _unitOfWork.Installments.GetByIdAsync(payment.InstallmentId);
                if (installment != null)
                {
                    installment.RevertPayment();
                }

                // Create ledger entries for reversal
                await CreateLedgerEntriesAsync(reversalPayment);

                // Update contract balance
                await UpdateContractBalanceAsync(payment.ContractId);

                // Save all changes at once
                await _unitOfWork.SaveChangesAsync(false);

                return true;
            });
        }

        public async Task<List<PaymentListVM>> GetPaymentsByContractAsync(Guid contractId)
        {
            var payments = await _unitOfWork.Payments.GetByContractIdAsync(contractId);
            return _mapper.Map<List<PaymentListVM>>(payments);
        }

        public async Task<List<PaymentListVM>> GetPaymentsByCustomerAsync(Guid customerId)
        {
            var payments = await _unitOfWork.Payments.GetByCustomerIdAsync(customerId);
            return _mapper.Map<List<PaymentListVM>>(payments);
        }

        public async Task<ReceiptDetailVM> GenerateReceiptAsync(Guid paymentId)
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

            // Don't generate receipts for reversal payments
            if (payment.IsReversal)
                throw new InvalidOperationException("Receipts cannot be generated for reversal payments.");

            // Check if receipt already exists
            var existingReceipt = await _unitOfWork.Receipts.GetByPaymentIdAsync(paymentId);
            if (existingReceipt != null)
                return _mapper.Map<ReceiptDetailVM>(existingReceipt);

            // Generate receipt number
            var receiptNumber = await GenerateReceiptNumberAsync();

            // Get contract to access CustomerId
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(payment.ContractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {payment.ContractId} not found.");

            // Get staffId
            var staffId = payment.ProcessedByStaffId ?? payment.StaffId;
            if (!staffId.HasValue)
                throw new InvalidOperationException("Staff ID is required for receipt generation.");

            // Create receipt using domain method
            var receipt = payment.GenerateReceipt(receiptNumber, contract.CustomerId, staffId.Value);

            await _unitOfWork.Receipts.AddAsync(receipt);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<ReceiptDetailVM>(receipt);
        }

        public async Task<ReceiptDetailVM> GetReceiptByPaymentAsync(Guid paymentId)
        {
            var receipt = await _unitOfWork.Receipts.GetByPaymentIdAsync(paymentId);
            if (receipt == null)
                throw new KeyNotFoundException($"Receipt for payment {paymentId} not found.");

            return _mapper.Map<ReceiptDetailVM>(receipt);
        }

        public async Task<ReceiptDetailVM> RegenerateReceiptAsync(Guid paymentId)
        {
            // Delete existing receipt
            var existingReceipt = await _unitOfWork.Receipts.GetByPaymentIdAsync(paymentId);
            if (existingReceipt != null)
            {
                await _unitOfWork.Receipts.DeleteAsync(existingReceipt);
            }

            // Generate new receipt
            return await GenerateReceiptAsync(paymentId);
        }

        private async Task<PaymentDetailVM> CreatePaymentAsync(PaymentCreateVM model)
        {
            // Create Money value object
            var amount = new Money(model.Amount, model.Currency);

            // Create payment entity
            var payment = new Payment(
                model.ContractId,
                model.InstallmentId,
                amount,
                model.PaymentDate,
                model.PaymentMethod,
                model.ReferenceNumber,
                model.Notes,
                model.ProcessedByStaffId ?? model.StaffId.Value
            );

            await _unitOfWork.Payments.AddAsync(payment);
            await _unitOfWork.SaveChangesAsync();

            return _mapper.Map<PaymentDetailVM>(payment);
        }

        private async Task UpdateInstallmentStatusAsync(Guid installmentId, Guid paymentId, decimal amount, DateTime? paymentDate = null)
        {
            var installment = await _unitOfWork.Installments.GetByIdAsync(installmentId);
            if (installment != null)
            {
                var installmentAmount = installment.Amount.Amount;
                var effectiveDate = paymentDate ?? DateTime.UtcNow;

                if (amount >= installmentAmount)
                {
                    installment.MarkAsPaid(effectiveDate);
                }
                else
                {
                    installment.UpdatePartialPayment(amount);
                }
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task CreateLedgerEntriesAsync(Payment payment)
        {
            // Get contract to access CustomerId
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(payment.ContractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {payment.ContractId} not found.");

            var currency = payment.Amount.Currency;
            var zero = Money.Zero(currency);
            var txnDate = DateTime.UtcNow;
            var staffId = payment.ProcessedByStaffId ?? payment.StaffId;

            if (payment.IsReversal)
            {
                // For reversals, flip the debit/credit entries to offset the original payment
                // Create credit entry for cash/bank account (reversing the debit)
                var creditEntry = new LedgerEntry(
                    txnDate,
                    $"Payment reversal - {payment.ReferenceNumber}",
                    debitAmount: zero,
                    creditAmount: payment.Amount,
                    balance: zero,
                    referenceType: "Payment",
                    referenceId: payment.Id,
                    contractId: payment.ContractId,
                    customerId: contract.CustomerId,
                    staffId: staffId
                );

                // Create debit entry for accounts receivable (reversing the credit)
                var debitEntry = new LedgerEntry(
                    txnDate,
                    $"Payment reversal applied - {payment.ReferenceNumber}",
                    debitAmount: payment.Amount,
                    creditAmount: zero,
                    balance: zero,
                    referenceType: "Contract",
                    referenceId: payment.ContractId,
                    contractId: payment.ContractId,
                    customerId: contract.CustomerId,
                    staffId: staffId
                );

                await _unitOfWork.LedgerEntries.AddAsync(creditEntry);
                await _unitOfWork.LedgerEntries.AddAsync(debitEntry);
            }
            else
            {
                // Regular payment entries
                // Create debit entry for cash/bank account
                var debitEntry = new LedgerEntry(
                    txnDate,
                    $"Payment received - {payment.ReferenceNumber}",
                    debitAmount: payment.Amount,
                    creditAmount: zero,
                    balance: zero,
                    referenceType: "Payment",
                    referenceId: payment.Id,
                    contractId: payment.ContractId,
                    customerId: contract.CustomerId,
                    staffId: staffId
                );

                // Create credit entry for accounts receivable
                var creditEntry = new LedgerEntry(
                    txnDate,
                    $"Payment applied - {payment.ReferenceNumber}",
                    debitAmount: zero,
                    creditAmount: payment.Amount,
                    balance: zero,
                    referenceType: "Contract",
                    referenceId: payment.ContractId,
                    contractId: payment.ContractId,
                    customerId: contract.CustomerId,
                    staffId: staffId
                );

                await _unitOfWork.LedgerEntries.AddAsync(debitEntry);
                await _unitOfWork.LedgerEntries.AddAsync(creditEntry);
            }

            await _unitOfWork.SaveChangesAsync();
        }

        private async Task UpdateContractBalanceAsync(Guid contractId)
        {
            var contract = await _unitOfWork.Contracts.GetByIdAsync(contractId);
            if (contract != null)
            {
                // The contract entity handles balance calculation internally
                // This method ensures the contract is updated after payment processing
                await _unitOfWork.SaveChangesAsync();
            }
        }

        private async Task<string> GenerateReceiptNumberAsync()
        {
            var today = DateTime.Today;
            var maxRetries = 3;

            for (int attempt = 1; attempt <= maxRetries; attempt++)
            {
                try
                {
                    // Use high-entropy ID with timestamp for uniqueness
                    var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
                    var randomSuffix = Guid.NewGuid().ToString("N").Substring(0, 8);
                    var receiptNumber = $"RCP-{today:yyyyMMdd}-{timestamp}-{randomSuffix}";

                    // Verify uniqueness by checking if receipt number exists
                    var existingReceipt = await _unitOfWork.Receipts.GetByReceiptNumberAsync(receiptNumber);
                    if (existingReceipt == null)
                    {
                        return receiptNumber;
                    }

                    // If duplicate, wait a bit and retry
                    if (attempt < maxRetries)
                    {
                        await Task.Delay(10 * attempt); // Exponential backoff
                    }
                }
                catch (Exception) when (attempt < maxRetries)
                {
                    // Log error and retry
                    await Task.Delay(10 * attempt);
                }
            }

            // Fallback to timestamp-based number if all retries fail
            var fallbackTimestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();
            return $"RCP-{today:yyyyMMdd}-{fallbackTimestamp}";
        }

        public async Task<FileDownloadResult> DownloadReceiptAsync(Guid paymentId, string format = "pdf")
        {
            var payment = await _unitOfWork.Payments.GetByIdAsync(paymentId);
            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {paymentId} not found.");

            var receipt = await _unitOfWork.Receipts.GetByPaymentIdAsync(paymentId);
            if (receipt == null)
                throw new KeyNotFoundException($"Receipt for payment {paymentId} not found.");

            // Generate receipt content based on format
            byte[] fileBytes;
            string contentType;
            string fileName;

            switch (format.ToLower())
            {
                case "pdf":
                    fileBytes = await GeneratePdfReceiptAsync(receipt, payment);
                    contentType = "application/pdf";
                    fileName = $"Receipt_{receipt.ReceiptNumber}.pdf";
                    break;
                case "zip":
                    fileBytes = await GenerateZipReceiptAsync(receipt, payment);
                    contentType = "application/zip";
                    fileName = $"Receipt_{receipt.ReceiptNumber}.zip";
                    break;
                default:
                    throw new ArgumentException($"Unsupported format: {format}");
            }

            return new FileDownloadResult
            {
                Bytes = fileBytes,
                FileName = fileName,
                ContentType = contentType
            };
        }

        public async Task<BulkPaymentResultVM> ProcessBulkPaymentsAsync(IEnumerable<PaymentModalVM> payments)
        {
            if (payments == null || !payments.Any())
                throw new ArgumentException("Payments collection cannot be null or empty.");

            var results = new List<PaymentDetailVM>();
            var errors = new List<BulkPaymentErrorVM>();

            return await _unitOfWork.ExecuteInTransactionAsync(async (cancellationToken) =>
            {
                foreach (var paymentModel in payments)
                {
                    try
                    {
                        // Convert PaymentModalVM to PaymentCreateVM
                        var createModel = new PaymentCreateVM
                        {
                            ContractId = paymentModel.ContractId,
                            InstallmentId = paymentModel.InstallmentId,
                            Amount = paymentModel.Amount,
                            Currency = paymentModel.Currency,
                            PaymentDate = paymentModel.PaymentDate,
                            PaymentMethod = paymentModel.PaymentMethod,
                            ReferenceNumber = paymentModel.ReferenceNumber,
                            Notes = paymentModel.Notes,
                            ProcessedByStaffId = paymentModel.ProcessedByStaffId
                        };

                        // Use the internal method to avoid nested transactions
                        var payment = await ProcessPaymentInternalAsync(createModel);
                        results.Add(payment);
                    }
                    catch (Exception ex)
                    {
                        // Log the error and collect it for reporting
                        _logger.LogError(ex, "Failed to process payment for installment {InstallmentId}", paymentModel.InstallmentId);

                        errors.Add(new BulkPaymentErrorVM
                        {
                            InstallmentId = paymentModel.InstallmentId,
                            ContractId = paymentModel.ContractId,
                            ErrorMessage = ex.Message,
                            Amount = paymentModel.Amount,
                            Currency = paymentModel.Currency
                        });
                    }
                }

                return new BulkPaymentResultVM
                {
                    SuccessfulPayments = results,
                    FailedPayments = errors,
                    TotalProcessed = payments.Count(),
                    SuccessCount = results.Count,
                    ErrorCount = errors.Count
                };
            });
        }

        // Internal method for processing payments without transaction wrapper
        private async Task<PaymentDetailVM> ProcessPaymentInternalAsync(PaymentCreateVM model)
        {
            if (model == null)
                throw new ArgumentNullException(nameof(model));

            // Validate contract exists and is active
            var contract = await _unitOfWork.Contracts.GetByIdWithDetailsAsync(model.ContractId);
            if (contract == null)
                throw new InvalidOperationException($"Contract with ID {model.ContractId} not found.");

            if (contract.Status != Domain.Enums.ContractStatus.Active)
                throw new InvalidOperationException($"Cannot process payment for contract in {contract.Status} status.");

            // Validate staff exists
            var staffId = model.ProcessedByStaffId ?? model.StaffId;
            if (!staffId.HasValue)
                throw new InvalidOperationException("ProcessedByStaffId or StaffId is required for payment processing.");

            var staff = await _unitOfWork.Staff.GetByIdAsync(staffId.Value);
            if (staff == null)
                throw new InvalidOperationException($"Staff with ID {staffId.Value} not found.");

            // Create Money value object
            var amount = new Money(model.Amount, model.Currency);

            // Create payment entity
            var payment = new Payment(
                model.ContractId,
                model.InstallmentId,
                amount,
                model.PaymentDate,
                model.PaymentMethod,
                model.ReferenceNumber,
                model.Notes,
                staffId.Value
            );

            await _unitOfWork.Payments.AddAsync(payment);

            // Update installment status
            await UpdateInstallmentStatusAsync(model.InstallmentId, payment.Id, model.Amount, model.PaymentDate);

            // Generate receipt
            var receipt = await GenerateReceiptAsync(payment.Id);

            // Create ledger entries
            await CreateLedgerEntriesAsync(payment);

            // Update contract balance
            await UpdateContractBalanceAsync(model.ContractId);

            return _mapper.Map<PaymentDetailVM>(payment);
        }

        public async Task<PagedResult<PaymentListVM>> SearchPaymentsAsync(string searchTerm, int page = 1, int pageSize = 10)
        {
            return await GetPagedAsync(searchTerm, "PaymentDate", page, pageSize);
        }

        public async Task<PagedResult<PaymentListVM>> SearchPaymentsAsync(PaymentSearchCriteriaVM criteria, int page = 1, int pageSize = 10)
        {
            // Get all payments first, then apply filters
            var allPayments = await _unitOfWork.Payments.GetAllAsync();
            var payments = _mapper.Map<List<PaymentListVM>>(allPayments);

            // Apply filters
            var filteredPayments = payments.AsEnumerable();

            if (!string.IsNullOrEmpty(criteria.ContractNumber))
            {
                filteredPayments = filteredPayments.Where(p =>
                    p.ContractNumber.Contains(criteria.ContractNumber, StringComparison.OrdinalIgnoreCase));
            }

            if (!string.IsNullOrEmpty(criteria.CustomerName))
            {
                filteredPayments = filteredPayments.Where(p =>
                    p.CustomerName.Contains(criteria.CustomerName, StringComparison.OrdinalIgnoreCase));
            }

            if (criteria.PaymentMethod.HasValue)
            {
                filteredPayments = filteredPayments.Where(p => p.PaymentMethod == criteria.PaymentMethod.Value);
            }

            if (criteria.FromDate.HasValue)
            {
                filteredPayments = filteredPayments.Where(p => p.PaymentDate >= criteria.FromDate.Value);
            }

            if (criteria.ToDate.HasValue)
            {
                filteredPayments = filteredPayments.Where(p => p.PaymentDate <= criteria.ToDate.Value);
            }

            if (criteria.MinAmount.HasValue)
            {
                filteredPayments = filteredPayments.Where(p => p.Amount >= criteria.MinAmount.Value);
            }

            if (criteria.MaxAmount.HasValue)
            {
                filteredPayments = filteredPayments.Where(p => p.Amount <= criteria.MaxAmount.Value);
            }

            if (!string.IsNullOrEmpty(criteria.Currency))
            {
                filteredPayments = filteredPayments.Where(p =>
                    p.Currency.Equals(criteria.Currency, StringComparison.OrdinalIgnoreCase));
            }

            if (criteria.StaffId.HasValue)
            {
                filteredPayments = filteredPayments.Where(p => p.ProcessedByStaffId == criteria.StaffId.Value);
            }

            // Apply pagination
            var totalCount = filteredPayments.Count();
            var pagedPayments = filteredPayments
                .OrderByDescending(p => p.PaymentDate)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<PaymentListVM>(pagedPayments.AsReadOnly(), totalCount, page, pageSize);
        }

        public async Task<List<PaymentListVM>> GetPaymentHistoryAsync(Guid? contractId = null, Guid? customerId = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            var payments = await _unitOfWork.Payments.GetPaymentHistoryAsync(contractId, customerId, fromDate, toDate);
            return _mapper.Map<List<PaymentListVM>>(payments);
        }

        private async Task<byte[]> GeneratePdfReceiptAsync(Receipt receipt, Payment payment)
        {
            // This is a placeholder implementation
            // In a real application, you would use a PDF generation library like iText7, PdfSharp, or similar
            var receiptContent = $"Receipt Number: {receipt.ReceiptNumber}\n" +
                                $"Payment Date: {payment.PaymentDate:yyyy-MM-dd}\n" +
                                $"Amount: {payment.Amount.Amount} {payment.Amount.Currency}\n" +
                                $"Payment Method: {payment.PaymentMethod}\n" +
                                $"Reference: {payment.ReferenceNumber}";

            // For now, return a simple text representation as bytes
            return System.Text.Encoding.UTF8.GetBytes(receiptContent);
        }

        private async Task<byte[]> GenerateZipReceiptAsync(Receipt receipt, Payment payment)
        {
            // This is a placeholder implementation
            // In a real application, you would create a ZIP file containing the receipt and related documents
            var receiptContent = $"Receipt Number: {receipt.ReceiptNumber}\n" +
                                $"Payment Date: {payment.PaymentDate:yyyy-MM-dd}\n" +
                                $"Amount: {payment.Amount.Amount} {payment.Amount.Currency}\n" +
                                $"Payment Method: {payment.PaymentMethod}\n" +
                                $"Reference: {payment.ReferenceNumber}";

            // For now, return a simple text representation as bytes
            return System.Text.Encoding.UTF8.GetBytes(receiptContent);
        }

        public async Task<PaymentAnalyticsViewModels.CollectionReportVM> GetCollectionReportAsync(DateTime fromDate, DateTime toDate)
        {
            _logger.LogInformation("Generating collection report from {FromDate} to {ToDate}", fromDate, toDate);

            // Validate date range
            if (fromDate > toDate)
            {
                throw new ArgumentException("From date cannot be later than to date");
            }

            // Get all payments in the date range
            var payments = await _unitOfWork.Payments.GetByDateRangeAsync(fromDate, toDate);

            if (payments == null || !payments.Any())
            {
                _logger.LogWarning("No payments found in date range {FromDate} to {ToDate}", fromDate, toDate);
                return new PaymentAnalyticsViewModels.CollectionReportVM
                {
                    FromDate = fromDate,
                    ToDate = toDate,
                    TotalPayments = 0,
                    TotalAmount = 0,
                    AveragePaymentAmount = 0,
                    PaymentsByMethod = new Dictionary<string, int>(),
                    AmountByMethod = new Dictionary<string, decimal>(),
                    PaymentsByDay = new Dictionary<string, int>(),
                    PaymentsByMonth = new Dictionary<string, int>(),
                    CollectionByMonth = new Dictionary<string, decimal>()
                };
            }

            // Calculate report metrics
            var totalAmount = payments.Sum(p => p.Amount.Amount);
            var averageAmount = payments.Any() ? totalAmount / payments.Count : 0;

            // Group payments by method
            var paymentsByMethod = payments
                .GroupBy(p => p.PaymentMethod.ToString())
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );

            // Calculate amount by payment method
            var amountByMethod = payments
                .GroupBy(p => p.PaymentMethod.ToString())
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(p => p.Amount.Amount)
                );

            // Group payments by day of week
            var paymentsByDay = payments
                .GroupBy(p => p.PaymentDate.DayOfWeek.ToString())
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );

            // Group payments by month
            var paymentsByMonth = payments
                .GroupBy(p => p.PaymentDate.ToString("MMMM", CultureInfo.InvariantCulture))
                .ToDictionary(
                    g => g.Key,
                    g => g.Count()
                );

            // Calculate collection amount by month
            var collectionByMonth = payments
                .GroupBy(p => p.PaymentDate.ToString("MMMM", CultureInfo.InvariantCulture))
                .ToDictionary(
                    g => g.Key,
                    g => g.Sum(p => p.Amount.Amount)
                );

            // Create and return the report
            var report = new PaymentAnalyticsViewModels.CollectionReportVM
            {
                FromDate = fromDate,
                ToDate = toDate,
                TotalPayments = payments.Count,
                TotalAmount = totalAmount,
                AveragePaymentAmount = averageAmount,
                PaymentsByMethod = paymentsByMethod,
                AmountByMethod = amountByMethod,
                PaymentsByDay = paymentsByDay,
                PaymentsByMonth = paymentsByMonth,
                CollectionByMonth = collectionByMonth
            };

            _logger.LogInformation("Collection report generated successfully with {TotalPayments} payments totaling {TotalAmount}",
                report.TotalPayments, report.TotalAmount);

            return report;
        }

        public async Task<FinancialAnalyticsVM> GetFinancialAnalyticsAsync()
        {
            var analytics = new FinancialAnalyticsVM();

            try
            {
                var payments = await _unitOfWork.Payments.GetAllAsync();

                analytics.TotalPayments = payments.Count(); // Add parentheses here
                analytics.TotalAmount = payments.Sum(p => p.Amount.Amount);
                analytics.AveragePaymentAmount = payments.Any() ?
                    analytics.TotalAmount / analytics.TotalPayments : 0;

                // Calculate payment trends by month
                analytics.PaymentTrendsByMonth = payments
                    .GroupBy(p => p.PaymentDate.ToString("yyyy-MM"))
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(p => p.Amount.Amount)
                    );

                // Calculate payment method distribution
                analytics.PaymentMethodDistribution = payments
                    .GroupBy(p => p.PaymentMethod)
                    .ToDictionary(
                        g => g.Key,
                        g => g.Sum(p => p.Amount.Amount)
                    );

                // Calculate collection efficiency
                var totalDue = await _unitOfWork.Installments.GetTotalDueAmount();
                analytics.CollectionEfficiencyRate = totalDue > 0 ?
                    (analytics.TotalAmount / totalDue) * 100 : 0;

                // Calculate overdue statistics
                var overdueInstallments = await _unitOfWork.Installments.GetOverdueInstallments();
                analytics.OverduePaymentsCount = overdueInstallments.Count(); // Add parentheses here
                analytics.OverdueAmount = overdueInstallments.Sum(i => i.Amount.Amount);

                return analytics;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating financial analytics");
                throw;
            }
        }

    }

    public interface IPaymentService
    {
        Task<PaymentDetailVM> ProcessPaymentAsync(PaymentCreateVM model);
        Task<PaymentDetailVM> UpdatePaymentAsync(PaymentUpdateVM model);
        Task<PaymentDetailVM> GetByIdAsync(Guid id);
        Task<PagedResult<PaymentListVM>> GetPagedAsync(string filter = null, string orderBy = "PaymentDate", int pageNumber = 1, int pageSize = 10);
        Task<PaymentDetailVM> ProcessInstallmentPaymentAsync(Guid installmentId, PaymentCreateVM model);
        Task<IReadOnlyList<PaymentDetailVM>> ProcessPartialPaymentAsync(Guid contractId, PaymentCreateVM model);
        Task<bool> ReversePaymentAsync(Guid paymentId, string reason);
        Task<List<PaymentListVM>> GetPaymentsByContractAsync(Guid contractId);
        Task<List<PaymentListVM>> GetPaymentsByCustomerAsync(Guid customerId);
        Task<ReceiptDetailVM> GenerateReceiptAsync(Guid paymentId);
        Task<ReceiptDetailVM> GetReceiptByPaymentAsync(Guid paymentId);
        Task<ReceiptDetailVM> RegenerateReceiptAsync(Guid paymentId);
        Task<FileDownloadResult> DownloadReceiptAsync(Guid paymentId, string format = "pdf");
        Task<BulkPaymentResultVM> ProcessBulkPaymentsAsync(IEnumerable<PaymentModalVM> payments);
        Task<PagedResult<PaymentListVM>> SearchPaymentsAsync(string searchTerm, int page = 1, int pageSize = 10);
        Task<PagedResult<PaymentListVM>> SearchPaymentsAsync(PaymentSearchCriteriaVM criteria, int page = 1, int pageSize = 10);
        Task<List<PaymentListVM>> GetPaymentHistoryAsync(Guid? contractId = null, Guid? customerId = null, DateTime? fromDate = null, DateTime? toDate = null);
        Task<PaymentAnalyticsViewModels.CollectionReportVM> GetCollectionReportAsync(DateTime fromDate, DateTime toDate);
        Task<FinancialAnalyticsVM> GetFinancialAnalyticsAsync();



    }


}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CollectionApp.Application.Services;
using CollectionApp.Application.ViewModels;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using Tracker.Money.Extensions;
using System.Collections.Generic;
using System.Linq;

namespace Tracker_Money.Controllers
{
    /// <summary>
    /// API controller for payment management operations
    /// </summary>
    [Route("api/payments")]
    [ApiController]
    [Authorize]
    [Produces("application/json")]
    public class PaymentApiController : ControllerBase
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentApiController> _logger;

        public PaymentApiController(IPaymentService paymentService, ILogger<PaymentApiController> logger)
        {
            _paymentService = paymentService;
            _logger = logger;
        }

        /// <summary>
        /// Get paged list of payments with optional search and filtering
        /// </summary>
        /// <param name="searchTerm">Search term for filtering payments</param>
        /// <param name="orderBy">Field to order by (default: PaymentDate)</param>
        /// <param name="page">Page number (default: 1)</param>
        /// <param name="pageSize">Page size (default: 10)</param>
        /// <returns>Paged list of payments</returns>
        [HttpGet]
        [ProducesResponseType(typeof(PagedApiResponse<PaymentListVM>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<PagedApiResponse<PaymentListVM>>> GetPayments(
            [FromQuery] string searchTerm = "",
            [FromQuery] string orderBy = "PaymentDate",
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResult = await _paymentService.GetPagedAsync(searchTerm, orderBy, page, pageSize);
                return this.OkPagedApiResponse(pagedResult, "Payments retrieved successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "GetPayments");
            }
        }

        /// <summary>
        /// Get payment by ID
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Payment details</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailVM>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<PaymentDetailVM>>> GetPayment(Guid id)
        {
            try
            {
                var payment = await _paymentService.GetByIdAsync(id);
                return this.OkApiResponse(payment, "Payment retrieved successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "GetPayment");
            }
        }

        /// <summary>
        /// Process a new payment
        /// </summary>
        /// <param name="model">Payment creation model</param>
        /// <returns>Created payment details</returns>
        [HttpPost]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailVM>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<PaymentDetailVM>>> CreatePayment([FromBody] PaymentCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payment = await _paymentService.ProcessPaymentAsync(model);
                var location = Url.Action(nameof(GetPayment), new { id = payment.Id });
                return this.CreatedApiResponse(payment, location, "Payment processed successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "CreatePayment");
            }
        }

        /// <summary>
        /// Update payment details (limited fields)
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="model">Payment update model</param>
        /// <returns>Updated payment details</returns>
        [HttpPut("{id}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailVM>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<PaymentDetailVM>>> UpdatePayment(Guid id, [FromBody] PaymentUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            if (id != model.Id)
            {
                return this.BadRequestApiResponse("Payment ID mismatch");
            }

            try
            {
                var payment = await _paymentService.UpdatePaymentAsync(model);
                return this.OkApiResponse(payment, "Payment updated successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "UpdatePayment");
            }
        }

        /// <summary>
        /// Reverse a payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="model">Reversal details</param>
        /// <returns>Success status</returns>
        [HttpPost("{id}/reverse")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> ReversePayment(Guid id, [FromBody] ReversePaymentVM model)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            if (id != model.PaymentId)
            {
                return this.BadRequestApiResponse("Payment ID mismatch");
            }

            try
            {
                var result = await _paymentService.ReversePaymentAsync(id, model.Reason);
                return this.OkApiResponse(result, "Payment reversed successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "ReversePayment");
            }
        }

        /// <summary>
        /// Process payment for specific installment
        /// </summary>
        /// <param name="installmentId">Installment ID</param>
        /// <param name="model">Payment details</param>
        /// <returns>Created payment details</returns>
        [HttpPost("installment/{installmentId}")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailVM>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<PaymentDetailVM>>> ProcessInstallmentPayment(Guid installmentId, [FromBody] PaymentCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payment = await _paymentService.ProcessInstallmentPaymentAsync(installmentId, model);
                var location = Url.Action(nameof(GetPayment), new { id = payment.Id });
                return this.CreatedApiResponse(payment, location, "Installment payment processed successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "ProcessInstallmentPayment");
            }
        }

        /// <summary>
        /// Process partial payment across multiple installments
        /// </summary>
        /// <param name="contractId">Contract ID</param>
        /// <param name="model">Payment details</param>
        /// <returns>List of created payments</returns>
        [HttpPost("partial")]
        [ProducesResponseType(typeof(ApiResponse<IReadOnlyList<PaymentDetailVM>>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<IReadOnlyList<PaymentDetailVM>>>> ProcessPartialPayment([FromQuery] Guid contractId, [FromBody] PaymentCreateVM model)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payments = await _paymentService.ProcessPartialPaymentAsync(contractId, model);
                var location = Url.Action(nameof(GetPayments));
                return this.CreatedApiResponse(payments, location, "Partial payment processed successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "ProcessPartialPayment");
            }
        }

        /// <summary>
        /// Process multiple payments in bulk
        /// </summary>
        /// <param name="payments">Collection of payments to process</param>
        /// <returns>Bulk payment results with success and error details</returns>
        [HttpPost("bulk")]
        [ProducesResponseType(typeof(ApiResponse<BulkPaymentResultVM>), 201)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<BulkPaymentResultVM>>> ProcessBulkPayments([FromBody] IEnumerable<PaymentModalVM> payments)
        {
            if (payments == null || !payments.Any())
            {
                return this.BadRequestApiResponse("Payments collection cannot be null or empty");
            }

            try
            {
                var results = await _paymentService.ProcessBulkPaymentsAsync(payments);
                var location = Url.Action(nameof(GetPayments));
                
                if (results.HasErrors)
                {
                    return this.OkBulkApiResponse(results, $"Bulk payments processed with {results.ErrorCount} errors");
                }
                else
                {
                    return this.CreatedApiResponse(results, location, "Bulk payments processed successfully");
                }
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "ProcessBulkPayments");
            }
        }

        /// <summary>
        /// Get receipt details for a payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Receipt details</returns>
        [HttpGet("{id}/receipt")]
        [ProducesResponseType(typeof(ApiResponse<ReceiptDetailVM>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<ReceiptDetailVM>>> GetReceipt(Guid id)
        {
            try
            {
                var receipt = await _paymentService.GetReceiptByPaymentAsync(id);
                return this.OkApiResponse(receipt, "Receipt retrieved successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "GetReceipt");
            }
        }

        /// <summary>
        /// Download receipt file
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <param name="format">File format (pdf, zip)</param>
        /// <returns>File download</returns>
        [HttpGet("{id}/receipt/download")]
        [ProducesResponseType(200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult> DownloadReceipt(Guid id, [FromQuery] string format = "pdf")
        {
            try
            {
                var fileResult = await _paymentService.DownloadReceiptAsync(id, format);
                return this.FileApiResponse(fileResult.Bytes, fileResult.ContentType, fileResult.FileName);
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "DownloadReceipt");
            }
        }

        /// <summary>
        /// Regenerate receipt for a payment
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Regenerated receipt details</returns>
        [HttpPost("{id}/receipt/regenerate")]
        [ProducesResponseType(typeof(ApiResponse<ReceiptDetailVM>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<ReceiptDetailVM>>> RegenerateReceipt(Guid id)
        {
            try
            {
                var receipt = await _paymentService.RegenerateReceiptAsync(id);
                return this.OkApiResponse(receipt, "Receipt regenerated successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "RegenerateReceipt");
            }
        }

        /// <summary>
        /// Search payments with basic criteria
        /// </summary>
        /// <param name="searchTerm">Search term</param>
        /// <param name="page">Page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Paged search results</returns>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedApiResponse<PaymentListVM>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<PagedApiResponse<PaymentListVM>>> SearchPayments(
            [FromQuery] string searchTerm,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 10)
        {
            try
            {
                var pagedResult = await _paymentService.SearchPaymentsAsync(searchTerm, page, pageSize);
                return this.OkPagedApiResponse(pagedResult, "Search completed successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "SearchPayments");
            }
        }

        /// <summary>
        /// Advanced search with multiple criteria
        /// </summary>
        /// <param name="criteria">Search criteria</param>
        /// <returns>Filtered payment results</returns>
        [HttpPost("search/advanced")]
        [ProducesResponseType(typeof(ApiResponse<List<PaymentListVM>>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<List<PaymentListVM>>>> AdvancedSearch([FromBody] PaymentSearchCriteriaVM criteria)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payments = await _paymentService.GetPaymentHistoryAsync(
                    null, null, criteria.FromDate, criteria.ToDate);

                // Apply additional filters
                var filteredPayments = payments.AsEnumerable();

                if (criteria.PaymentMethod.HasValue)
                {
                    filteredPayments = filteredPayments.Where(p => p.PaymentMethod == criteria.PaymentMethod.Value);
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
                    filteredPayments = filteredPayments.Where(p => p.Currency == criteria.Currency);
                }

                var results = filteredPayments.ToList();
                return this.OkApiResponse(results, "Advanced search completed successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "AdvancedSearch");
            }
        }

        /// <summary>
        /// Get payments for a specific contract
        /// </summary>
        /// <param name="contractId">Contract ID</param>
        /// <returns>List of payments for the contract</returns>
        [HttpGet("contract/{contractId}")]
        [ProducesResponseType(typeof(ApiResponse<List<PaymentListVM>>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<List<PaymentListVM>>>> GetPaymentsByContract(Guid contractId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByContractAsync(contractId);
                return this.OkApiResponse(payments, "Contract payments retrieved successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "GetPaymentsByContract");
            }
        }

        /// <summary>
        /// Get payments for a specific customer
        /// </summary>
        /// <param name="customerId">Customer ID</param>
        /// <returns>List of payments for the customer</returns>
        [HttpGet("customer/{customerId}")]
        [ProducesResponseType(typeof(ApiResponse<List<PaymentListVM>>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<List<PaymentListVM>>>> GetPaymentsByCustomer(Guid customerId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByCustomerAsync(customerId);
                return this.OkApiResponse(payments, "Customer payments retrieved successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "GetPaymentsByCustomer");
            }
        }

        /// <summary>
        /// Delete a payment (soft delete or mark as cancelled)
        /// </summary>
        /// <param name="id">Payment ID</param>
        /// <returns>Success status</returns>
        [HttpDelete("{id}")]
        [ProducesResponseType(typeof(ApiResponse<bool>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 404)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<bool>>> DeletePayment(Guid id)
        {
            try
            {
                // Note: In a real application, you might want to implement soft delete
                // or mark the payment as cancelled rather than hard delete
                // For now, we'll return a not implemented response
                return this.BadRequestApiResponse("Payment deletion is not supported. Use reverse payment instead.");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "DeletePayment");
            }
        }

        /// <summary>
        /// Get payment history with optional filters
        /// </summary>
        /// <param name="contractId">Optional contract ID filter</param>
        /// <param name="customerId">Optional customer ID filter</param>
        /// <param name="fromDate">Optional start date filter</param>
        /// <param name="toDate">Optional end date filter</param>
        /// <returns>Payment history with summary</returns>
        [HttpGet("history")]
        [ProducesResponseType(typeof(ApiResponse<PaymentHistoryVM>), 200)]
        [ProducesResponseType(typeof(ApiErrorResponse), 400)]
        [ProducesResponseType(typeof(ApiErrorResponse), 500)]
        public async Task<ActionResult<ApiResponse<PaymentHistoryVM>>> GetPaymentHistory(
            [FromQuery] Guid? contractId = null,
            [FromQuery] Guid? customerId = null,
            [FromQuery] DateTime? fromDate = null,
            [FromQuery] DateTime? toDate = null)
        {
            try
            {
                var payments = await _paymentService.GetPaymentHistoryAsync(contractId, customerId, fromDate, toDate);
                var historyVM = new PaymentHistoryVM
                {
                    ContractId = contractId,
                    CustomerId = customerId,
                    Payments = payments,
                    TotalAmount = payments.Sum(p => p.Amount),
                    Currency = payments.FirstOrDefault()?.Currency ?? "USD",
                    PaymentCount = payments.Count()
                };

                return this.OkApiResponse(historyVM, "Payment history retrieved successfully");
            }
            catch (Exception ex)
            {
                return this.HandleApiException(ex, _logger, "GetPaymentHistory");
            }
        }
    }
}
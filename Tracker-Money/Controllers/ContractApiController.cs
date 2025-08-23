using Microsoft.AspNetCore.Mvc;
using CollectionApp.Application.Interfaces;
using CollectionApp.Application.ViewModels;
using CollectionApp.Application.Common;
using Microsoft.Extensions.Logging;
using CollectionApp.Application.Services;
using CollectionApp.Domain.Enums;
using Tracker.Money.Extensions;

namespace Tracker_Money.Controllers
{
    [ApiController]
    [Route("api/contracts")]
    [Produces("application/json")]
    public class ContractApiController : ControllerBase
    {
        private readonly IContractService _contractService;
        private readonly IPaymentService _paymentService;
        private readonly IInstallmentService _installmentService;
        private readonly IExportService _exportService;
        private readonly ILogger<ContractApiController> _logger;

        public ContractApiController(
            IContractService contractService,
            IPaymentService paymentService,
            IInstallmentService installmentService,
            IExportService exportService,
            ILogger<ContractApiController> logger)
        {
            _contractService = contractService;
            _paymentService = paymentService;
            _installmentService = installmentService;
            _exportService = exportService;
            _logger = logger;
        }

        /// <summary>
        /// Get paginated list of contracts
        /// </summary>
        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<ContractListVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedResult<ContractListVM>>> GetContracts(
            [FromQuery] int pageNumber = 1,
            [FromQuery] int pageSize = 10,
            [FromQuery] string? filter = null,
            [FromQuery] string orderBy = "ContractNumber",
            [FromQuery] string? status = null)
        {
            try
            {
                if (pageNumber < 1 || pageSize < 1 || pageSize > 100)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "Invalid pagination parameters",
                        Detail = "Page number must be >= 1 and page size must be between 1 and 100."
                    });
                }

                PagedResult<ContractListVM> result;

                if (!string.IsNullOrEmpty(status))
                {
                    if (!Enum.TryParse<ContractStatus>(status, out var statusEnum))
                    {
                        return BadRequest(new ProblemDetails
                        {
                            Title = "Invalid status parameter",
                            Detail = $"Status '{status}' is not valid."
                        });
                    }

                    result = await _contractService.GetContractsByStatusAsync(statusEnum, pageNumber, pageSize, filter, orderBy);
                }
                else
                {
                    result = await _contractService.GetPagedAsync(filter, orderBy, pageNumber, pageSize);
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while retrieving contracts."
                });
            }
        }

        /// <summary>
        /// Get contract by ID
        /// </summary>
        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(ContractDetailVM), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContractDetailVM>> GetContract(Guid id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null)
                {
                    return NotFound(new ProblemDetails
                    {
                        Title = "Contract not found",
                        Detail = $"Contract with ID '{id}' was not found."
                    });
                }

                return Ok(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while retrieving the contract."
                });
            }
        }

        /// <summary>
        /// Create a new contract
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(ContractDetailVM), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContractDetailVM>> CreateContract([FromBody] ContractCreateVM model)
        {
            try
            {
                var result = await _contractService.CreateAsync(model);
                return CreatedAtAction(nameof(GetContract), new { id = result.Id }, result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract creation");
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract");
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while creating the contract."
                });
            }
        }

        /// <summary>
        /// Update an existing contract
        /// </summary>
        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateContract(Guid id, [FromBody] ContractUpdateVM model)
        {
            try
            {
                if (id != model.Id)
                {
                    return BadRequest(new ProblemDetails
                    {
                        Title = "ID mismatch",
                        Detail = "The ID in the URL does not match the ID in the request body."
                    });
                }

                await _contractService.UpdateAsync(model);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract update for ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Contract not found during update for ID {ContractId}", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Contract not found",
                    Detail = $"Contract with ID '{id}' was not found."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while updating the contract."
                });
            }
        }

        /// <summary>
        /// Cancel a contract
        /// </summary>
        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteContract(Guid id)
        {
            try
            {
                await _contractService.CancelContractAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract deletion for ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (KeyNotFoundException ex)
            {
                _logger.LogWarning(ex, "Contract not found during deletion for ID {ContractId}", id);
                return NotFound(new ProblemDetails
                {
                    Title = "Contract not found",
                    Detail = $"Contract with ID '{id}' was not found."
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while cancelling the contract."
                });
            }
        }

        /// <summary>
        /// Activate a contract
        /// </summary>
        [HttpPost("{id:guid}/activate")]
        [ProducesResponseType(typeof(ContractDetailVM), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContractDetailVM>> ActivateContract(Guid id)
        {
            try
            {
                var result = await _contractService.ActivateContractAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract activation for ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while activating the contract."
                });
            }
        }

        /// <summary>
        /// Suspend a contract
        /// </summary>
        [HttpPost("{id:guid}/suspend")]
        [ProducesResponseType(typeof(ContractDetailVM), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContractDetailVM>> SuspendContract(Guid id)
        {
            try
            {
                var result = await _contractService.SuspendContractAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract suspension for ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while suspending the contract."
                });
            }
        }

        /// <summary>
        /// Complete a contract
        /// </summary>
        [HttpPost("{id:guid}/complete")]
        [ProducesResponseType(typeof(ContractDetailVM), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContractDetailVM>> CompleteContract(Guid id)
        {
            try
            {
                var result = await _contractService.CompleteContractAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract completion for ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while completing the contract."
                });
            }
        }

        /// <summary>
        /// Cancel a contract
        /// </summary>
        [HttpPost("{id:guid}/cancel")]
        [ProducesResponseType(typeof(ContractDetailVM), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContractDetailVM>> CancelContract(Guid id)
        {
            try
            {
                var result = await _contractService.CancelContractAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract cancellation for ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while cancelling the contract."
                });
            }
        }

        /// <summary>
        /// Mark a contract as defaulted
        /// </summary>
        [HttpPost("{id:guid}/default")]
        [ProducesResponseType(typeof(ContractDetailVM), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ContractDetailVM>> DefaultContract(Guid id)
        {
            try
            {
                var result = await _contractService.DefaultContractAsync(id);
                return Ok(result);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during contract default for ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error defaulting contract with ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while marking the contract as defaulted."
                });
            }
        }

        /// <summary>
        /// Get contract installments
        /// </summary>
        [HttpGet("{id:guid}/installments")]
        [ProducesResponseType(typeof(List<InstallmentListVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<InstallmentListVM>>> GetContractInstallments(Guid id)
        {
            try
            {
                var installments = await _contractService.GetContractInstallmentsAsync(id);
                return Ok(installments);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving installments for contract ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while retrieving contract installments."
                });
            }
        }

        /// <summary>
        /// Generate installments for a contract
        /// </summary>
        [HttpPost("{id:guid}/installments/generate")]
        [ProducesResponseType(typeof(List<InstallmentListVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<InstallmentListVM>>> GenerateInstallments(Guid id)
        {
            try
            {
                var installments = await _contractService.GenerateInstallmentsAsync(id);
                return Ok(installments);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during installment generation for contract ID {ContractId}", id);
                return BadRequest(new ProblemDetails
                {
                    Title = "Business rule violation",
                    Detail = ex.Message
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating installments for contract ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while generating installments."
                });
            }
        }

        /// <summary>
        /// Get contract outstanding amount
        /// </summary>
        [HttpGet("{id:guid}/outstanding")]
        [ProducesResponseType(typeof(OutstandingAmountVM), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<OutstandingAmountVM>> GetOutstandingAmount(Guid id)
        {
            try
            {
                var result = await _contractService.RecalculateOutstandingAsync(id);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error calculating outstanding amount for contract ID {ContractId}", id);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while calculating the outstanding amount."
                });
            }
        }

        /// <summary>
        /// Get contracts by customer
        /// </summary>
        [HttpGet("customer/{customerId:guid}")]
        [ProducesResponseType(typeof(List<ContractListVM>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<List<ContractListVM>>> GetContractsByCustomer(Guid customerId)
        {
            try
            {
                var contracts = await _contractService.GetContractsByCustomerAsync(customerId);
                return Ok(contracts);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts for customer ID {CustomerId}", customerId);
                return StatusCode(500, new ProblemDetails
                {
                    Title = "Internal server error",
                    Detail = "An error occurred while retrieving customer contracts."
                });
            }
        }

        /// <summary>
        /// Get overdue contracts
        /// </summary>
        [HttpGet("overdue")]
        [ProducesResponseType(typeof(ApiResponse<List<ContractListVM>>), 200)]
        [ProducesResponseType(500)]
      public async Task<ActionResult<ApiResponse<List<ContractListVM>>>> GetOverdueContracts()
{
    try
    {
        var contracts = await _contractService.GetOverdueContractsAsync();
        return this.OkApiResponse(contracts, "Overdue contracts retrieved successfully");
    }
    catch (Exception ex)
    {
        _logger.LogError(ex, "Error retrieving overdue contracts");
        return this.HandleApiException<List<ContractListVM>>(ex, _logger, "get overdue contracts");
    }
}

        /// <summary>
        /// Process payment for contract
        /// </summary>
        [HttpPost("{id:guid}/payments")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<PaymentDetailVM>>> ProcessPayment(Guid id, [FromBody] PaymentModalVM paymentModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payment = await _paymentService.ProcessPaymentAsync(id, paymentModel);
                return this.OkApiResponse(payment, "Payment processed successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process payment for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "process payment");
            }
        }

        /// <summary>
        /// Process installment payment
        /// </summary>
        [HttpPost("{id:guid}/installments/{installmentId:guid}/payments")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<PaymentDetailVM>>> ProcessInstallmentPayment(Guid id, Guid installmentId, [FromBody] PaymentModalVM paymentModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payment = await _paymentService.ProcessInstallmentPaymentAsync(installmentId, paymentModel);
                return this.OkApiResponse(payment, "Installment payment processed successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract or installment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process installment payment for contract {ContractId}, installment {InstallmentId}", id, installmentId);
                return this.HandleApiException(ex, _logger, "process installment payment");
            }
        }

        /// <summary>
        /// Process partial payment across multiple installments
        /// </summary>
        [HttpPost("{id:guid}/payments/partial")]
        [ProducesResponseType(typeof(ApiResponse<List<PaymentDetailVM>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<List<PaymentDetailVM>>>> ProcessPartialPayment(Guid id, [FromBody] PartialPaymentVM partialPaymentModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payments = await _paymentService.ProcessPartialPaymentAsync(id, partialPaymentModel);
                return this.OkApiResponse(payments, "Partial payment processed successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process partial payment for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "process partial payment");
            }
        }

        /// <summary>
        /// Reverse payment
        /// </summary>
        [HttpPost("payments/{paymentId:guid}/reverse")]
        [ProducesResponseType(typeof(ApiResponse<PaymentDetailVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<PaymentDetailVM>>> ReversePayment(Guid paymentId, [FromBody] PaymentReversalVM reversalModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var payment = await _paymentService.ReversePaymentAsync(paymentId, reversalModel);
                return this.OkApiResponse(payment, "Payment reversed successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Payment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reverse payment {PaymentId}", paymentId);
                return this.HandleApiException(ex, _logger, "reverse payment");
            }
        }

        /// <summary>
        /// Waive installment
        /// </summary>
        [HttpPost("{id:guid}/installments/{installmentId:guid}/waive")]
        [ProducesResponseType(typeof(ApiResponse<InstallmentDetailVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<InstallmentDetailVM>>> WaiveInstallment(Guid id, Guid installmentId, [FromBody] InstallmentWaiverVM waiverModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var installment = await _installmentService.WaiveInstallmentAsync(installmentId, waiverModel);
                return this.OkApiResponse(installment, "Installment waived successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract or installment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to waive installment {InstallmentId} for contract {ContractId}", installmentId, id);
                return this.HandleApiException(ex, _logger, "waive installment");
            }
        }

        /// <summary>
        /// Reschedule installment
        /// </summary>
        [HttpPost("{id:guid}/installments/{installmentId:guid}/reschedule")]
        [ProducesResponseType(typeof(ApiResponse<InstallmentDetailVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<InstallmentDetailVM>>> RescheduleInstallment(Guid id, Guid installmentId, [FromBody] InstallmentRescheduleVM rescheduleModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var installment = await _installmentService.RescheduleInstallmentAsync(installmentId, rescheduleModel);
                return this.OkApiResponse(installment, "Installment rescheduled successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract or installment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to reschedule installment {InstallmentId} for contract {ContractId}", installmentId, id);
                return this.HandleApiException(ex, _logger, "reschedule installment");
            }
        }

        /// <summary>
        /// Split installment
        /// </summary>
        [HttpPost("{id:guid}/installments/{installmentId:guid}/split")]
        [ProducesResponseType(typeof(ApiResponse<List<InstallmentDetailVM>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<List<InstallmentDetailVM>>>> SplitInstallment(Guid id, Guid installmentId, [FromBody] InstallmentSplitVM splitModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var installments = await _installmentService.SplitInstallmentAsync(installmentId, splitModel);
                return this.OkApiResponse(installments, "Installment split successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract or installment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to split installment {InstallmentId} for contract {ContractId}", installmentId, id);
                return this.HandleApiException(ex, _logger, "split installment");
            }
        }

        /// <summary>
        /// Bulk installment operations
        /// </summary>
        [HttpPost("{id:guid}/installments/bulk")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<BulkOperationResponse>>> BulkInstallmentOperations(Guid id, [FromBody] BulkInstallmentActionVM bulkActionModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var result = await _installmentService.ProcessBulkActionsAsync(id, bulkActionModel);
                return this.OkBulkApiResponse(result, "Bulk installment operations completed successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process bulk installment operations for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "bulk installment operations");
            }
        }

        /// <summary>
        /// Get contract financial summary
        /// </summary>
        [HttpGet("{id:guid}/analytics/financial")]
        [ProducesResponseType(typeof(ApiResponse<ContractFinancialSummaryVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<ContractFinancialSummaryVM>>> GetContractFinancialSummary(Guid id)
        {
            try
            {
                var summary = await _contractService.GetContractFinancialSummaryAsync(id);
                return this.OkApiResponse(summary, "Contract financial summary retrieved successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get financial summary for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "get contract financial summary");
            }
        }

        /// <summary>
        /// Get installment status summary
        /// </summary>
        [HttpGet("{id:guid}/analytics/installments")]
        [ProducesResponseType(typeof(ApiResponse<InstallmentStatusSummaryVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<InstallmentStatusSummaryVM>>> GetInstallmentStatusSummary(Guid id)
        {
            try
            {
                var summary = await _installmentService.GetInstallmentStatusSummaryAsync(id);
                return this.OkApiResponse(summary, "Installment status summary retrieved successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get installment status summary for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "get installment status summary");
            }
        }

        /// <summary>
        /// Get payment history analytics
        /// </summary>
        [HttpGet("{id:guid}/analytics/payments")]
        [ProducesResponseType(typeof(ApiResponse<PaymentAnalyticsVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<PaymentAnalyticsVM>>> GetPaymentAnalytics(Guid id)
        {
            try
            {
                var analytics = await _paymentService.GetPaymentAnalyticsAsync(id);
                return this.OkApiResponse(analytics, "Payment analytics retrieved successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get payment analytics for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "get payment analytics");
            }
        }

        /// <summary>
        /// Get collection report
        /// </summary>
        [HttpGet("analytics/collection")]
        [ProducesResponseType(typeof(ApiResponse<CollectionReportVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<CollectionReportVM>>> GetCollectionReport([FromQuery] DateTime fromDate, [FromQuery] DateTime toDate)
        {
            if (fromDate > toDate)
            {
                return this.BadRequestApiResponse("From date cannot be after to date");
            }

            try
            {
                var report = await _paymentService.GetCollectionReportAsync(fromDate, toDate);
                return this.OkApiResponse(report, "Collection report retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get collection report from {FromDate} to {ToDate}", fromDate, toDate);
                return this.HandleApiException(ex, _logger, "get collection report");
            }
        }

        /// <summary>
        /// Get overdue analysis
        /// </summary>
        [HttpGet("analytics/overdue")]
        [ProducesResponseType(typeof(ApiResponse<OverdueAnalysisVM>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<OverdueAnalysisVM>>> GetOverdueAnalysis()
        {
            try
            {
                var analysis = await _installmentService.GetOverdueAnalysisAsync();
                return this.OkApiResponse(analysis, "Overdue analysis retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get overdue analysis");
                return this.HandleApiException(ex, _logger, "get overdue analysis");
            }
        }

        /// <summary>
        /// Get receipt details
        /// </summary>
        [HttpGet("payments/{paymentId:guid}/receipt")]
        [ProducesResponseType(typeof(ApiResponse<ReceiptDetailVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<ReceiptDetailVM>>> GetReceipt(Guid paymentId)
        {
            try
            {
                var receipt = await _paymentService.GetReceiptByPaymentAsync(paymentId);
                return this.OkApiResponse(receipt, "Receipt retrieved successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Payment or receipt not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get receipt for payment {PaymentId}", paymentId);
                return this.HandleApiException(ex, _logger, "get receipt");
            }
        }

        /// <summary>
        /// Download receipt file
        /// </summary>
        [HttpGet("payments/{paymentId:guid}/receipt/download")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DownloadReceipt(Guid paymentId)
        {
            try
            {
                var receipt = await _paymentService.GetReceiptByPaymentAsync(paymentId);
                if (receipt == null)
                {
                    return this.NotFoundApiResponse("Receipt not found");
                }

                // In a real implementation, you would return the actual file
                var fileBytes = System.Text.Encoding.UTF8.GetBytes("Receipt content placeholder");
                return this.FileApiResponse(fileBytes, "application/pdf", $"receipt_{paymentId}.pdf");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download receipt for payment {PaymentId}", paymentId);
                return this.HandleApiException(ex, _logger, "download receipt");
            }
        }

        /// <summary>
        /// Regenerate receipt
        /// </summary>
        [HttpPost("payments/{paymentId:guid}/receipt/regenerate")]
        [ProducesResponseType(typeof(ApiResponse<ReceiptDetailVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<ReceiptDetailVM>>> RegenerateReceipt(Guid paymentId)
        {
            try
            {
                var receipt = await _paymentService.RegenerateReceiptAsync(paymentId);
                return this.OkApiResponse(receipt, "Receipt regenerated successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Payment not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to regenerate receipt for payment {PaymentId}", paymentId);
                return this.HandleApiException(ex, _logger, "regenerate receipt");
            }
        }

        /// <summary>
        /// Export contract financial report
        /// </summary>
        [HttpGet("{id:guid}/export/financial-report")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportFinancialReport(Guid id, [FromQuery] string format = "Excel")
        {
            try
            {
                var result = await _exportService.ExportContractFinancialReportAsync(id, format);
                
                if (!result.Success)
                {
                    return this.BadRequestApiResponse(result.ErrorMessage);
                }

                return this.OkApiResponse(result, "Financial report export completed successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export financial report for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "export financial report");
            }
        }

        /// <summary>
        /// Export contract list with filtering
        /// </summary>
        [HttpGet("export")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportContracts([FromQuery] string format = "Excel", [FromQuery] bool includeInstallments = false, [FromQuery] bool includePayments = false)
        {
            try
            {
                var request = new ContractExportRequestVM
                {
                    Format = format,
                    IncludeInstallments = includeInstallments,
                    IncludePayments = includePayments
                };

                var result = await _exportService.ExportContractsAsync(request);
                
                if (!result.Success)
                {
                    return this.BadRequestApiResponse(result.ErrorMessage);
                }

                return this.OkApiResponse(result, "Contract export completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export contracts");
                return this.HandleApiException(ex, _logger, "export contracts");
            }
        }

        /// <summary>
        /// Export installment schedule
        /// </summary>
        [HttpGet("{id:guid}/export/installments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportInstallments(Guid id, [FromQuery] string format = "Excel")
        {
            try
            {
                var result = await _exportService.ExportInstallmentScheduleAsync(id, format);
                
                if (!result.Success)
                {
                    return this.BadRequestApiResponse(result.ErrorMessage);
                }

                return this.OkApiResponse(result, "Installment schedule export completed successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export installments for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "export installments");
            }
        }

        /// <summary>
        /// Export payment history
        /// </summary>
        [HttpGet("{id:guid}/export/payments")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportPayments(Guid id, [FromQuery] string format = "Excel")
        {
            try
            {
                var result = await _exportService.ExportPaymentHistoryAsync(id, format);
                
                if (!result.Success)
                {
                    return this.BadRequestApiResponse(result.ErrorMessage);
                }

                return this.OkApiResponse(result, "Payment history export completed successfully");
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export payments for contract {ContractId}", id);
                return this.HandleApiException(ex, _logger, "export payments");
            }
        }
    }
}
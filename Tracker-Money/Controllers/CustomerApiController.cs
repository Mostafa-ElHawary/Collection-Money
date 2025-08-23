using Microsoft.AspNetCore.Mvc;
using CollectionApp.Application.Services;
using CollectionApp.Application.ViewModels;
using CollectionApp.Application.Common;
using CollectionApp.Application.Interfaces;
using Tracker.Money.Extensions;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.Extensions.Logging;

namespace Tracker_Money.Controllers
{
    [Route("api/customers")]
    [ApiController]
    [Produces("application/json")]
    public class CustomerApiController : ControllerBase
    {
        private readonly ICustomerService _customerService;
        private readonly IExportService _exportService;
        private readonly ILogger<CustomerApiController> _logger;

        public CustomerApiController(
            ICustomerService customerService, 
            IExportService exportService,
            ILogger<CustomerApiController> logger)
        {
            _customerService = customerService;
            _exportService = exportService;
            _logger = logger;
        }

        [HttpGet]
        [ProducesResponseType(typeof(PagedResult<CustomerListVM>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedResult<CustomerListVM>>> GetCustomers([FromQuery] string? searchTerm = "", [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            try
            {
                var result = await _customerService.GetPagedAsync(searchTerm, page, pageSize);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get customers");
                return Problem("An error occurred while retrieving customers.");
            }
        }

        [HttpGet("{id:guid}")]
        [ProducesResponseType(typeof(CustomerDetailVM), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CustomerDetailVM>> GetCustomer(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                return Ok(customer);
            }
            catch (InvalidOperationException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get customer {CustomerId}", id);
                return Problem("An error occurred while retrieving the customer.");
            }
        }

        [HttpPost]
        [ProducesResponseType(typeof(CustomerDetailVM), 201)]
        [ProducesResponseType(400)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<CustomerDetailVM>> CreateCustomer([FromBody] CustomerCreateVM model)
        {
            try
            {
                var created = await _customerService.CreateAsync(model);
                return CreatedAtAction(nameof(GetCustomer), new { id = created.Id }, created);
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Validation error while creating customer");
                return Conflict(invEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to create customer");
                return Problem("An error occurred while creating the customer.");
            }
        }

        [HttpPut("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> UpdateCustomer(Guid id, [FromBody] CustomerUpdateVM model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            try
            {
                await _customerService.UpdateAsync(model);
                return NoContent();
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Validation error while updating customer {CustomerId}", id);
                return Conflict(invEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to update customer {CustomerId}", id);
                return Problem("An error occurred while updating the customer.");
            }
        }

        [HttpDelete("{id:guid}")]
        [ProducesResponseType(204)]
        [ProducesResponseType(404)]
        [ProducesResponseType(409)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> DeleteCustomer(Guid id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
                return NoContent();
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Business rule prevented deletion for {CustomerId}", id);
                return Conflict(invEx.Message);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to delete customer {CustomerId}", id);
                return Problem("An error occurred while deleting the customer.");
            }
        }

        /// <summary>
        /// Search customers with basic search term
        /// </summary>
        [HttpGet("search")]
        [ProducesResponseType(typeof(PagedApiResponse<CustomerListVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedApiResponse<CustomerListVM>>> SearchCustomers([FromQuery] string searchTerm, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
            {
                return this.BadRequestApiResponse("Search term cannot be empty");
            }

            try
            {
                var results = await _customerService.SearchCustomersAsync(searchTerm, page, pageSize);
                return this.OkPagedApiResponse(results, "Customers found successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search failed for term '{SearchTerm}'", searchTerm);
                return this.HandleApiException(ex, _logger, "customer search");
            }
        }

        /// <summary>
        /// Advanced search for customers with complex criteria
        /// </summary>
        [HttpPost("search/advanced")]
        [ProducesResponseType(typeof(PagedApiResponse<CustomerSearchResultVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<PagedApiResponse<CustomerSearchResultVM>>> AdvancedSearch([FromBody] AdvancedCustomerSearchCriteriaVM criteria, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var results = await _customerService.AdvancedSearchAsync(criteria, page, pageSize);
                return this.OkPagedApiResponse(results, "Advanced search completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Advanced search failed");
                return this.HandleApiException(ex, _logger, "advanced customer search");
            }
        }

        /// <summary>
        /// Get customer contracts
        /// </summary>
        [HttpGet("{id:guid}/contracts")]
        [ProducesResponseType(typeof(ApiResponse<IEnumerable<CustomerContractSummaryVM>>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<IEnumerable<CustomerContractSummaryVM>>>> GetCustomerContracts(Guid id)
        {
            try
            {
                var _ = await _customerService.GetByIdAsync(id);
                var contracts = await _customerService.GetCustomerContractsAsync(id);
                return this.OkApiResponse(contracts, "Customer contracts retrieved successfully");
            }
            catch (InvalidOperationException)
            {
                return this.NotFoundApiResponse("Customer not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get contracts for {CustomerId}", id);
                return this.HandleApiException(ex, _logger, "get customer contracts");
            }
        }

        /// <summary>
        /// Get global customer analytics
        /// </summary>
        [HttpGet("analytics")]
        [ProducesResponseType(typeof(ApiResponse<CustomerAnalyticsVM>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<CustomerAnalyticsVM>>> GetCustomerAnalytics()
        {
            try
            {
                var analytics = await _customerService.GetCustomerAnalyticsAsync();
                return this.OkApiResponse(analytics, "Customer analytics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get customer analytics");
                return this.HandleApiException(ex, _logger, "get customer analytics");
            }
        }

        /// <summary>
        /// Get individual customer analytics
        /// </summary>
        [HttpGet("{id:guid}/analytics")]
        [ProducesResponseType(typeof(ApiResponse<CustomerAnalyticsVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<CustomerAnalyticsVM>>> GetCustomerAnalytics(Guid id)
        {
            try
            {
                var analytics = await _customerService.GetCustomerAnalyticsAsync(id);
                return this.OkApiResponse(analytics, "Customer analytics retrieved successfully");
            }
            catch (InvalidOperationException)
            {
                return this.NotFoundApiResponse("Customer not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get customer analytics for {CustomerId}", id);
                return this.HandleApiException(ex, _logger, "get customer analytics");
            }
        }

        /// <summary>
        /// Get portfolio-wide customer analytics
        /// </summary>
        [HttpGet("analytics/portfolio")]
        [ProducesResponseType(typeof(ApiResponse<CustomerAnalyticsVM>), 200)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<CustomerAnalyticsVM>>> GetPortfolioAnalytics()
        {
            try
            {
                var analytics = await _customerService.GetPortfolioAnalyticsAsync();
                return this.OkApiResponse(analytics, "Portfolio analytics retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get portfolio analytics");
                return this.HandleApiException(ex, _logger, "get portfolio analytics");
            }
        }

        /// <summary>
        /// Bulk update customers
        /// </summary>
        [HttpPost("bulk/update")]
        [ProducesResponseType(typeof(ApiResponse<BulkOperationResponse>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<BulkOperationResponse>>> BulkUpdateCustomers([FromBody] BulkCustomerUpdateVM updateModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var result = await _customerService.BulkUpdateCustomersAsync(updateModel);
                return this.OkBulkApiResponse(result, "Bulk update completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update failed");
                return this.HandleApiException(ex, _logger, "bulk customer update");
            }
        }

        /// <summary>
        /// Get multiple customers by IDs
        /// </summary>
        [HttpPost("bulk/get")]
        [ProducesResponseType(typeof(ApiResponse<List<CustomerSearchResultVM>>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<List<CustomerSearchResultVM>>>> BulkGetCustomers([FromBody] List<Guid> customerIds)
        {
            if (customerIds == null || !customerIds.Any())
            {
                return this.BadRequestApiResponse("Customer IDs list cannot be empty");
            }

            if (customerIds.Count > 100)
            {
                return this.BadRequestApiResponse("Cannot retrieve more than 100 customers at once");
            }

            try
            {
                var customers = await _customerService.BulkGetCustomersAsync(customerIds);
                return this.OkApiResponse(customers, "Customers retrieved successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk get customers failed");
                return this.HandleApiException(ex, _logger, "bulk get customers");
            }
        }

        /// <summary>
        /// Validate bulk update without applying
        /// </summary>
        [HttpPost("bulk/validate")]
        [ProducesResponseType(typeof(ApiResponse<BulkUpdateResultVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<BulkUpdateResultVM>>> ValidateBulkUpdate([FromBody] BulkCustomerUpdateVM updateModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var result = await _customerService.ValidateBulkUpdateAsync(updateModel);
                return this.OkApiResponse(result, "Bulk update validation completed");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update validation failed");
                return this.HandleApiException(ex, _logger, "bulk update validation");
            }
        }

        /// <summary>
        /// Link contract to customer
        /// </summary>
        [HttpPost("{id:guid}/contracts/{contractId:guid}/link")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<CustomerDetailVM>>> LinkContractToCustomer(Guid id, Guid contractId)
        {
            try
            {
                var customer = await _customerService.LinkContractToCustomerAsync(id, contractId);
                return this.OkApiResponse(customer, "Contract linked to customer successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Customer or contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to link contract {ContractId} to customer {CustomerId}", contractId, id);
                return this.HandleApiException(ex, _logger, "link contract to customer");
            }
        }

        /// <summary>
        /// Transfer contract between customers
        /// </summary>
        [HttpPost("contracts/{contractId:guid}/transfer")]
        [ProducesResponseType(typeof(ApiResponse<CustomerDetailVM>), 200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<CustomerDetailVM>>> TransferContract([FromBody] ContractTransferVM transferModel)
        {
            if (!ModelState.IsValid)
            {
                return this.ValidationErrorApiResponse(ModelState);
            }

            try
            {
                var customer = await _customerService.TransferContractAsync(transferModel);
                return this.OkApiResponse(customer, "Contract transferred successfully");
            }
            catch (InvalidOperationException ex)
            {
                return this.BadRequestApiResponse(ex.Message);
            }
            catch (KeyNotFoundException)
            {
                return this.NotFoundApiResponse("Customer or contract not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to transfer contract {ContractId}", transferModel.ContractId);
                return this.HandleApiException(ex, _logger, "transfer contract");
            }
        }

        /// <summary>
        /// Get comprehensive contract history for customer
        /// </summary>
        [HttpGet("{id:guid}/contracts/history")]
        [ProducesResponseType(typeof(ApiResponse<CustomerContractHistoryVM>), 200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<ActionResult<ApiResponse<CustomerContractHistoryVM>>> GetCustomerContractHistory(Guid id)
        {
            try
            {
                var history = await _customerService.GetCustomerContractHistoryAsync(id);
                return this.OkApiResponse(history, "Customer contract history retrieved successfully");
            }
            catch (InvalidOperationException)
            {
                return this.NotFoundApiResponse("Customer not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to get contract history for customer {CustomerId}", id);
                return this.HandleApiException(ex, _logger, "get customer contract history");
            }
        }

        /// <summary>
        /// Export customer list with filtering
        /// </summary>
        [HttpGet("export")]
        [ProducesResponseType(200)]
        [ProducesResponseType(400)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportCustomers([FromQuery] string format = "Excel", [FromQuery] bool includeContracts = false, [FromQuery] bool includePayments = false)
        {
            try
            {
                var request = new CustomerExportRequestVM
                {
                    Format = format,
                    IncludeContracts = includeContracts,
                    IncludePayments = includePayments
                };

                var result = await _exportService.ExportCustomersAsync(request);
                
                if (!result.Success)
                {
                    return this.BadRequestApiResponse(result.ErrorMessage);
                }

                // For now, return a placeholder response
                // In a real implementation, you would return the actual file
                return this.OkApiResponse(result, "Export completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export customers");
                return this.HandleApiException(ex, _logger, "export customers");
            }
        }

        /// <summary>
        /// Export individual customer data
        /// </summary>
        [HttpGet("{id:guid}/export")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportCustomer(Guid id, [FromQuery] string format = "Excel")
        {
            try
            {
                var result = await _exportService.ExportCustomerAnalyticsAsync(id, format);
                
                if (!result.Success)
                {
                    return this.BadRequestApiResponse(result.ErrorMessage);
                }

                return this.OkApiResponse(result, "Customer export completed successfully");
            }
            catch (InvalidOperationException)
            {
                return this.NotFoundApiResponse("Customer not found");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export customer {CustomerId}", id);
                return this.HandleApiException(ex, _logger, "export customer");
            }
        }

        /// <summary>
        /// Export analytics data
        /// </summary>
        [HttpGet("analytics/export")]
        [ProducesResponseType(200)]
        [ProducesResponseType(500)]
        public async Task<IActionResult> ExportAnalytics([FromQuery] string format = "Excel")
        {
            try
            {
                var request = new AnalyticsExportRequestVM
                {
                    Format = format,
                    AnalyticsType = "Customer",
                    IncludeCharts = true,
                    IncludeRawData = true
                };

                var result = await _exportService.ExportAnalyticsAsync(request);
                
                if (!result.Success)
                {
                    return this.BadRequestApiResponse(result.ErrorMessage);
                }

                return this.OkApiResponse(result, "Analytics export completed successfully");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to export analytics");
                return this.HandleApiException(ex, _logger, "export analytics");
            }
        }
    }
}
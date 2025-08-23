using Microsoft.AspNetCore.Mvc;
using CollectionApp.Application.Services;
using CollectionApp.Application.ViewModels;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Tracker_Money.Models;

namespace Tracker_Money.Controllers
{
    public class CustomerController : Controller
    {
        private readonly ICustomerService _customerService;
        private readonly ILogger<CustomerController> _logger;
        private readonly IMapper _mapper;

        public CustomerController(ICustomerService customerService, ILogger<CustomerController> logger, IMapper mapper)
        {
            _customerService = customerService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10)
        {
            try
            {
                var pagedResult = await _customerService.GetPagedAsync(searchTerm, page, pageSize);
                var viewModel = new Tracker_Money.Models.PagedViewModel<CustomerListVM>(pagedResult)
                {
                    SearchTerm = searchTerm,
                    OrderBy = null,
                    RouteValues = new System.Collections.Generic.Dictionary<string, object?>
                    {
                        { "searchTerm", searchTerm },
                        { "pageSize", pageSize }
                    }
                };
                ViewBag.SearchTerm = searchTerm;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load customers page {Page} with size {PageSize} and search '{Search}'", page, pageSize, searchTerm);
                TempData["ErrorMessage"] = "An error occurred while loading customers.";
                return View(new Tracker_Money.Models.PagedViewModel<CustomerListVM>(new CollectionApp.Application.Common.PagedResult<CustomerListVM>(Array.Empty<CustomerListVM>(), 0, page, pageSize)));
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                var contracts = await _customerService.GetCustomerContractsAsync(id);
                customer.Contracts = contracts;
                try
                {
                    ViewBag.Analytics = await _customerService.GetCustomerAnalyticsAsync(id);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to load analytics summary for details {CustomerId}", id);
                }

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load customer details for {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading customer details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CustomerCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _customerService.CreateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException invEx)
                {
                    _logger.LogWarning(invEx, "Validation error while creating customer");
                    ModelState.AddModelError(string.Empty, invEx.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while creating customer");
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                var updateModel = _mapper.Map<CustomerUpdateVM>(customer);
                return View(updateModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load edit model for {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the edit form.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CustomerUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    await _customerService.UpdateAsync(model);
                    return RedirectToAction(nameof(Index));
                }
                catch (InvalidOperationException invEx)
                {
                    _logger.LogWarning(invEx, "Validation error while updating customer {CustomerId}", model.Id);
                    ModelState.AddModelError(string.Empty, invEx.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unexpected error while updating customer {CustomerId}", model.Id);
                    ModelState.AddModelError(string.Empty, "An unexpected error occurred.");
                }
            }
            return View(model);
        }

        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null)
                {
                    return NotFound();
                }

                return View(customer);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load delete confirmation for {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the delete page.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                await _customerService.DeleteAsync(id);
                TempData["SuccessMessage"] = "Customer deleted successfully.";
            }
            catch (InvalidOperationException invEx)
            {
                _logger.LogWarning(invEx, "Business rule prevented deletion for {CustomerId}", id);
                TempData["ErrorMessage"] = invEx.Message;
                return RedirectToAction(nameof(Details), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while deleting customer {CustomerId}", id);
                TempData["ErrorMessage"] = "An unexpected error occurred while deleting the customer.";
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Search(string searchTerm, int page = 1, int pageSize = 10)
        {
            try
            {
                var result = await _customerService.GetPagedAsync(searchTerm, page, pageSize);
                var viewModel = new Tracker_Money.Models.PagedViewModel<CustomerListVM>(result)
                {
                    SearchTerm = searchTerm,
                    RouteValues = new System.Collections.Generic.Dictionary<string, object?>
                    {
                        { "searchTerm", searchTerm },
                        { "pageSize", pageSize }
                    }
                };
                return PartialView("_CustomerSearchResults", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Search failed for term '{SearchTerm}'", searchTerm);
                return PartialView("_CustomerSearchResults", new Tracker_Money.Models.PagedViewModel<CustomerListVM>(new CollectionApp.Application.Common.PagedResult<CustomerListVM>(Array.Empty<CustomerListVM>(), 0, page, pageSize)));
            }
        }

        [HttpGet]
        public async Task<IActionResult> AdvancedSearch()
        {
            var criteria = new AdvancedCustomerSearchCriteriaVM();
            if (Request.HasFormContentType)
            {
                try
                {
                    var result = await _customerService.AdvancedSearchAsync(criteria, criteria.Page, criteria.PageSize);
                    ViewData["Results"] = new Tracker_Money.Models.PagedViewModel<CustomerListVM>(result);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Failed to preload results on GET AdvancedSearch");
                }
            }
            return View(criteria);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdvancedSearch(AdvancedCustomerSearchCriteriaVM criteria)
        {
            try
            {
                var result = await _customerService.AdvancedSearchAsync(criteria, criteria.Page, criteria.PageSize);
                ViewData["Results"] = new Tracker_Money.Models.PagedViewModel<CustomerListVM>(result)
                {
                    OrderBy = criteria.OrderBy
                };
                return View(criteria);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Advanced search failed");
                TempData["ErrorMessage"] = "An error occurred while performing the advanced search.";
                return View(criteria);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AdvancedSearchResults(AdvancedCustomerSearchCriteriaVM criteria)
        {
            try
            {
                var result = await _customerService.AdvancedSearchAsync(criteria, criteria.Page, criteria.PageSize);
                var viewModel = new Tracker_Money.Models.PagedViewModel<CustomerListVM>(result)
                {
                    OrderBy = criteria.OrderBy
                };
                return PartialView("_AdvancedSearchResults", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Advanced search (AJAX) failed");
                return PartialView("_AdvancedSearchResults", new Tracker_Money.Models.PagedViewModel<CustomerListVM>(new CollectionApp.Application.Common.PagedResult<CustomerListVM>(Array.Empty<CustomerListVM>(), 0, criteria.Page, criteria.PageSize)));
            }
        }

        [HttpGet]
        public async Task<IActionResult> Analytics()
        {
            try
            {
                var vm = await _customerService.GetPortfolioAnalyticsAsync();
                return View(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load analytics");
                TempData["ErrorMessage"] = "An error occurred while loading analytics.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet("Customer/{id}/Analytics")]
        public async Task<IActionResult> CustomerAnalytics(Guid id)
        {
            try
            {
                var vm = await _customerService.GetCustomerAnalyticsAsync(id);
                return View("Analytics", vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load customer analytics for {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading customer analytics.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAnalyticsData(Guid? id)
        {
            try
            {
                var vm = await _customerService.GetCustomerAnalyticsAsync(id);
                return Json(vm);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load analytics data");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> BulkUpdate()
        {
            return View(new BulkCustomerUpdateVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkUpdate(BulkCustomerUpdateVM model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }
            try
            {
                var result = await _customerService.BulkUpdateCustomersAsync(model);
                TempData["SuccessMessage"] = $"Updated {result.TotalUpdated} of {result.TotalRequested} customers.";
                return View(model);
            }
            catch (InvalidOperationException inv)
            {
                _logger.LogWarning(inv, "Bulk update validation error");
                ModelState.AddModelError(string.Empty, inv.Message);
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk update failed");
                TempData["ErrorMessage"] = "An error occurred while processing bulk update.";
                return View(model);
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkSelect([FromForm] System.Collections.Generic.List<System.Guid> ids)
        {
            try
            {
                var customers = await _customerService.BulkGetCustomersAsync(ids);
                return Json(customers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk select failed");
                return StatusCode(500);
            }
        }

        [HttpPost]
        public async Task<IActionResult> BulkPreview([FromForm] BulkCustomerUpdateVM model)
        {
            try
            {
                model.ValidateOnly = true;
                var result = await _customerService.BulkUpdateCustomersAsync(model);
                return Json(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Bulk preview failed");
                return StatusCode(500);
            }
        }

        [HttpGet]
        public async Task<IActionResult> ContractActions(Guid id)
        {
            try
            {
                var customer = await _customerService.GetByIdAsync(id);
                if (customer == null) return NotFound();
                ViewBag.Customer = customer;
                return View(new CustomerContractActionVM { CustomerId = id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load contract actions for {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading contract actions.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> LinkContract(CustomerContractActionVM model)
        {
            try
            {
                var updated = await _customerService.LinkContractToCustomerAsync(model.CustomerId, model.ContractId);
                TempData["SuccessMessage"] = "Contract linked successfully.";
                return RedirectToAction(nameof(Details), new { id = model.CustomerId });
            }
            catch (InvalidOperationException inv)
            {
                _logger.LogWarning(inv, "Link contract validation error");
                TempData["ErrorMessage"] = inv.Message;
                return RedirectToAction(nameof(ContractActions), new { id = model.CustomerId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to link contract");
                TempData["ErrorMessage"] = "An error occurred while linking contract.";
                return RedirectToAction(nameof(ContractActions), new { id = model.CustomerId });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> TransferContract(CustomerContractActionVM model)
        {
            try
            {
                if (!model.ToCustomerId.HasValue)
                    throw new InvalidOperationException("Target customer is required for transfer.");
                var updated = await _customerService.TransferContractAsync(model.ContractId, model.CustomerId, model.ToCustomerId.Value);
                TempData["SuccessMessage"] = "Contract transferred successfully.";
                return RedirectToAction(nameof(Details), new { id = model.ToCustomerId });
            }
            catch (InvalidOperationException inv)
            {
                _logger.LogWarning(inv, "Transfer contract validation error");
                TempData["ErrorMessage"] = inv.Message;
                return RedirectToAction(nameof(ContractActions), new { id = model.CustomerId });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to transfer contract");
                TempData["ErrorMessage"] = "An error occurred while transferring contract.";
                return RedirectToAction(nameof(ContractActions), new { id = model.CustomerId });
            }
        }

        [HttpGet]
        public async Task<IActionResult> ContractHistory(Guid id)
        {
            try
            {
                var history = await _customerService.GetCustomerContractHistoryAsync(id);
                return View(history);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load contract history for {CustomerId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading contract history.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using CollectionApp.Application.Services;
using CollectionApp.Application.ViewModels;
using System.Threading.Tasks;
using System;
using Microsoft.Extensions.Logging;
using AutoMapper;
using Tracker_Money.Models;
using System.Collections.Generic;
using System.Linq;

namespace Tracker_Money.Controllers
{
    [Authorize]
    public class PaymentController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ILogger<PaymentController> _logger;
        private readonly IMapper _mapper;

        public PaymentController(IPaymentService paymentService, ILogger<PaymentController> logger, IMapper mapper)
        {
            _paymentService = paymentService;
            _logger = logger;
            _mapper = mapper;
        }

        public async Task<IActionResult> Index(string searchTerm = "", int page = 1, int pageSize = 10, string orderBy = "PaymentDate")
        {
            try
            {
                var pagedResult = await _paymentService.GetPagedAsync(searchTerm, orderBy, page, pageSize);
                var viewModel = new Tracker_Money.Models.PagedViewModel<PaymentListVM>(pagedResult)
                {
                    SearchTerm = searchTerm,
                    OrderBy = orderBy,
                    RouteValues = new System.Collections.Generic.Dictionary<string, object?>
                    {
                        { "searchTerm", searchTerm },
                        { "pageSize", pageSize },
                        { "orderBy", orderBy }
                    }
                };
                ViewBag.SearchTerm = searchTerm;
                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load payments page {Page} with size {PageSize} and search '{Search}'", page, pageSize, searchTerm);
                TempData["ErrorMessage"] = "An error occurred while loading payments.";
                return View(new Tracker_Money.Models.PagedViewModel<PaymentListVM>(new CollectionApp.Application.Common.PagedResult<PaymentListVM>(Array.Empty<PaymentListVM>(), 0, page, pageSize)));
            }
        }

        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var payment = await _paymentService.GetByIdAsync(id);
                return View(payment);
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Payment not found: {PaymentId}", id);
                return NotFound();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load payment details for {PaymentId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading payment details.";
                return RedirectToAction(nameof(Index));
            }
        }

        public IActionResult Create()
        {
            return View(new PaymentCreateVM
            {
                PaymentDate = DateTime.UtcNow.Date
            });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PaymentCreateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var payment = await _paymentService.ProcessPaymentAsync(model);
                    TempData["SuccessMessage"] = "Payment processed successfully.";
                    return RedirectToAction(nameof(Details), new { id = payment.Id });
                }
                catch (InvalidOperationException invEx)
                {
                    _logger.LogWarning(invEx, "Validation error while processing payment");
                    ModelState.AddModelError("", invEx.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to process payment");
                    TempData["ErrorMessage"] = "An error occurred while processing the payment.";
                }
            }

            // Repopulate ViewBag with selected values for form persistence
            if (model.ContractId != Guid.Empty)
            {
                ViewBag.SelectedContractId = model.ContractId;
            }
            if (model.InstallmentId != Guid.Empty)
            {
                ViewBag.SelectedInstallmentId = model.InstallmentId;
            }

            return View(model);
        }

        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var payment = await _paymentService.GetByIdAsync(id);
                if (payment == null)
                {
                    return NotFound();
                }

                var updateModel = new PaymentUpdateVM
                {
                    Id = payment.Id,
                    ReferenceNumber = payment.ReferenceNumber,
                    Notes = payment.Notes
                };

                return View(updateModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load payment for editing {PaymentId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the payment for editing.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(PaymentUpdateVM model)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var payment = await _paymentService.UpdatePaymentAsync(model);
                    TempData["SuccessMessage"] = "Payment updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = payment.Id });
                }
                catch (InvalidOperationException invEx)
                {
                    _logger.LogWarning(invEx, "Validation error while updating payment");
                    ModelState.AddModelError("", invEx.Message);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to update payment {PaymentId}", model.Id);
                    TempData["ErrorMessage"] = "An error occurred while updating the payment.";
                }
            }

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReversePayment(ReversePaymentVM model)
        {
            // Check if this is an AJAX request
            bool isAjax = Request.Headers["X-Requested-With"] == "XMLHttpRequest";
            
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _paymentService.ReversePaymentAsync(model.PaymentId, model.Reason);
                    if (result)
                    {
                        if (isAjax)
                        {
                            return Json(new { success = true, message = "Payment reversed successfully." });
                        }
                        else
                        {
                            TempData["SuccessMessage"] = "Payment reversed successfully.";
                            return RedirectToAction(nameof(Details), new { id = model.PaymentId });
                        }
                    }
                    else
                    {
                        if (isAjax)
                        {
                            return Json(new { success = false, message = "Failed to reverse payment." });
                        }
                        else
                        {
                            TempData["ErrorMessage"] = "Failed to reverse payment.";
                        }
                    }
                }
                catch (InvalidOperationException invEx)
                {
                    _logger.LogWarning(invEx, "Business rule error while reversing payment");
                    if (isAjax)
                    {
                        return Json(new { success = false, message = invEx.Message });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = invEx.Message;
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Failed to reverse payment {PaymentId}", model.PaymentId);
                    if (isAjax)
                    {
                        return Json(new { success = false, message = "An error occurred while reversing the payment." });
                    }
                    else
                    {
                        TempData["ErrorMessage"] = "An error occurred while reversing the payment.";
                    }
                }
            }
            else
            {
                if (isAjax)
                {
                    return Json(new { success = false, message = "Invalid reversal request." });
                }
                else
                {
                    TempData["ErrorMessage"] = "Invalid reversal request.";
                }
            }

            if (isAjax)
            {
                return Json(new { success = false, message = "Invalid reversal request." });
            }
            else
            {
                return RedirectToAction(nameof(Details), new { id = model.PaymentId });
            }
        }

        public async Task<IActionResult> ViewReceipt(Guid id)
        {
            try
            {
                var receipt = await _paymentService.GetReceiptByPaymentAsync(id);
                return View(receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load receipt for payment {PaymentId}", id);
                TempData["ErrorMessage"] = "An error occurred while loading the receipt.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public async Task<IActionResult> DownloadReceipt(Guid id, string format = "pdf")
        {
            try
            {
                var fileResult = await _paymentService.DownloadReceiptAsync(id, format);
                return File(fileResult.Bytes, fileResult.ContentType, fileResult.FileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to download receipt for payment {PaymentId}", id);
                TempData["ErrorMessage"] = "An error occurred while downloading the receipt.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegenerateReceipt(Guid id)
        {
            try
            {
                var receipt = await _paymentService.RegenerateReceiptAsync(id);
                TempData["SuccessMessage"] = "Receipt regenerated successfully.";
                return RedirectToAction(nameof(ViewReceipt), new { id });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to regenerate receipt for payment {PaymentId}", id);
                TempData["ErrorMessage"] = "An error occurred while regenerating the receipt.";
                return RedirectToAction(nameof(Details), new { id });
            }
        }

        public async Task<IActionResult> History(Guid? contractId = null, Guid? customerId = null, DateTime? fromDate = null, DateTime? toDate = null)
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

                ViewBag.FromDate = fromDate;
                ViewBag.ToDate = toDate;
                ViewBag.ContractId = contractId;
                ViewBag.CustomerId = customerId;

                return View(historyVM);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load payment history");
                TempData["ErrorMessage"] = "An error occurred while loading payment history.";
                return RedirectToAction(nameof(Index));
            }
        }

        public async Task<IActionResult> GetPaymentsByContract(Guid contractId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByContractAsync(contractId);
                return PartialView("_PaymentSearchResults", new Tracker_Money.Models.PagedViewModel<PaymentListVM>(
                    new CollectionApp.Application.Common.PagedResult<PaymentListVM>(payments.AsReadOnly(), payments.Count(), 1, payments.Count())));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load payments for contract {ContractId}", contractId);
                return PartialView("_PaymentSearchResults", new Tracker_Money.Models.PagedViewModel<PaymentListVM>(
                    new CollectionApp.Application.Common.PagedResult<PaymentListVM>(Array.Empty<PaymentListVM>(), 0, 1, 10)));
            }
        }

        public async Task<IActionResult> GetPaymentsByCustomer(Guid customerId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByCustomerAsync(customerId);
                return PartialView("_PaymentSearchResults", new Tracker_Money.Models.PagedViewModel<PaymentListVM>(
                    new CollectionApp.Application.Common.PagedResult<PaymentListVM>(payments.AsReadOnly(), payments.Count(), 1, payments.Count())));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load payments for customer {CustomerId}", customerId);
                return PartialView("_PaymentSearchResults", new Tracker_Money.Models.PagedViewModel<PaymentListVM>(
                    new CollectionApp.Application.Common.PagedResult<PaymentListVM>(Array.Empty<PaymentListVM>(), 0, 1, 10)));
            }
        }

        public async Task<IActionResult> Search(string searchTerm, int page = 1, int pageSize = 10)
        {
            try
            {
                var pagedResult = await _paymentService.SearchPaymentsAsync(searchTerm, page, pageSize);
                var viewModel = new Tracker_Money.Models.PagedViewModel<PaymentListVM>(pagedResult)
                {
                    SearchTerm = searchTerm,
                    RouteValues = new System.Collections.Generic.Dictionary<string, object?>
                    {
                        { "searchTerm", searchTerm },
                        { "pageSize", pageSize }
                    }
                };

                return PartialView("_PaymentSearchResults", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to search payments with term '{SearchTerm}'", searchTerm);
                return PartialView("_PaymentSearchResults", new Tracker_Money.Models.PagedViewModel<PaymentListVM>(
                    new CollectionApp.Application.Common.PagedResult<PaymentListVM>(Array.Empty<PaymentListVM>(), 0, page, pageSize)));
            }
        }

        public IActionResult AdvancedSearch()
        {
            return View(new PaymentSearchCriteriaVM());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AdvancedSearch(PaymentSearchCriteriaVM criteria)
        {
            try
            {
                // Use the new service method with criteria
                var pagedResult = await _paymentService.SearchPaymentsAsync(criteria, 1, 50);
                
                var viewModel = new Tracker_Money.Models.PagedViewModel<PaymentListVM>(pagedResult);

                return View("SearchResults", viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to perform advanced search");
                TempData["ErrorMessage"] = "An error occurred while performing the search.";
                return View(criteria);
            }
        }

        public IActionResult SearchResults()
        {
            return View();
        }
    }
}
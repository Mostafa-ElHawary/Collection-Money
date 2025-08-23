using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using CollectionApp.Application.Interfaces;
using CollectionApp.Application.ViewModels;
using CollectionApp.Application.Common;
using AutoMapper;
using Microsoft.Extensions.Logging;
using CollectionApp.Application.Services;
using CollectionApp.Domain.Enums;
using Tracker_Money.Models;

namespace Tracker_Money.Controllers
{
    public class ContractController : Controller
    {
        private readonly IContractService _contractService;
        private readonly IPaymentService _paymentService;
        private readonly IInstallmentService _installmentService;
        private readonly ILogger<ContractController> _logger;
        private readonly IMapper _mapper;

        public ContractController(
            IContractService contractService,
            IPaymentService paymentService,
            IInstallmentService installmentService,
            ILogger<ContractController> logger,
            IMapper mapper)
        {
            _contractService = contractService;
            _paymentService = paymentService;
            _installmentService = installmentService;
            _logger = logger;
            _mapper = mapper;
        }

        // GET: Contract
        public async Task<IActionResult> Index(string searchTerm = "", int pageNumber = 1, int pageSize = 10, 
            string orderBy = "ContractNumber", string status = "")
        {
            try
            {
                // Validate page number and page size parameters
                if (pageNumber < 1)
                {
                    pageNumber = 1;
                    TempData["ErrorMessage"] = "Invalid page number. Showing first page instead.";
                }

                if (pageSize < 1 || pageSize > 100)
                {
                    pageSize = pageSize < 1 ? 10 : 100;
                    TempData["ErrorMessage"] = $"Invalid page size. Using {pageSize} items per page instead.";
                }

                ViewData["Title"] = "Contracts";
                ViewData["CurrentFilter"] = searchTerm;
                ViewData["CurrentStatus"] = status;
                ViewData["CurrentOrderBy"] = orderBy;

                PagedResult<ContractListVM> result;

                if (!string.IsNullOrEmpty(status))
                {
                    if (Enum.TryParse(status, true, out ContractStatus statusEnum))
                    {
                        result = await _contractService.GetContractsByStatusAsync(statusEnum, pageNumber, pageSize);
                    }
                    else
                    {
                        TempData["ErrorMessage"] = $"Invalid contract status: {status}";
                        result = await _contractService.GetPagedAsync(searchTerm, orderBy, pageNumber, pageSize);
                    }
                }
                else
                {
                    result = await _contractService.GetPagedAsync(searchTerm, orderBy, pageNumber, pageSize);
                }

                var viewModel = new Tracker_Money.Models.PagedViewModel<ContractListVM>
                {
                    Items = result.Items,
                    TotalCount = result.TotalCount,
                    PageNumber = pageNumber,
                    PageSize = pageSize,
                    TotalPages = (int)Math.Ceiling((double)result.TotalCount / pageSize)
                };

                return View(viewModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contracts for page {PageNumber}", pageNumber);
                TempData["ErrorMessage"] = "An error occurred while retrieving contracts.";
                return View(new Tracker_Money.Models.PagedViewModel<ContractListVM>());
            }
        }

        // GET: Contract/Details/5
        public async Task<IActionResult> Details(Guid id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contract not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["Title"] = $"Contract - {contract.ContractNumber}";
                return View(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract details for ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving contract details.";
                return RedirectToAction(nameof(Index));
            }
        }

        [HttpGet]
        public async Task<IActionResult> FinancialAnalytics(Guid id)
        {
            var summary = await _installmentService.GetInstallmentStatusSummaryAsync(id);
            var vm = _mapper.Map<ContractFinancialSummaryVM>(summary);
            vm.ContractId = id;
            ViewData["Title"] = "Financial Analytics";
            return View("FinancialAnalytics", vm);
        }

        // GET: Contract/Create
        public IActionResult Create()
        {
            ViewData["Title"] = "Create New Contract";
            return View(new ContractCreateVM());
        }

        // POST: Contract/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ContractCreateVM model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var result = await _contractService.CreateAsync(model);
                    TempData["SuccessMessage"] = $"Contract {result.ContractNumber} created successfully.";
                    return RedirectToAction(nameof(Details), new { id = result.Id });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Business rule violation during contract creation");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating contract");
                ModelState.AddModelError("", "An error occurred while creating the contract.");
            }

            ViewData["Title"] = "Create New Contract";
            return View(model);
        }

        // GET: Contract/Edit/5
        public async Task<IActionResult> Edit(Guid id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contract not found.";
                    return RedirectToAction(nameof(Index));
                }

                var updateModel = _mapper.Map<ContractUpdateVM>(contract);
                ViewData["Title"] = $"Edit Contract - {contract.ContractNumber}";
                return View(updateModel);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract for editing, ID: {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the contract.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contract/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [FromForm] ContractUpdateVM model)
        {
            try
            {
                if (id != model.Id)
                {
                    TempData["ErrorMessage"] = "Contract ID mismatch.";
                    return RedirectToAction(nameof(Index));
                }

                if (ModelState.IsValid)
                {
                    await _contractService.UpdateAsync(model);
                    TempData["SuccessMessage"] = $"Contract {model.ContractNumber} updated successfully.";
                    return RedirectToAction(nameof(Details), new { id = model.Id });
                }
            }
            catch (InvalidOperationException ex)
            {
                ModelState.AddModelError("", ex.Message);
                _logger.LogWarning(ex, "Business rule violation during contract update for ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error updating contract with ID {ContractId}", id);
                ModelState.AddModelError("", "An error occurred while updating the contract.");
            }

            ViewData["Title"] = $"Edit Contract - {model.ContractNumber}";
            return View(model);
        }

        // GET: Contract/Delete/5
        public async Task<IActionResult> Delete(Guid id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contract not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["Title"] = $"Delete Contract - {contract.ContractNumber}";
                return View(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract for deletion, ID: {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving the contract.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contract/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            try
            {
                // Get the contract to check its status
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contract not found.";
                    return RedirectToAction(nameof(Index));
                }

                // If contract is Draft, permanently delete it; otherwise, cancel it
                if (contract.Status == ContractStatus.Draft)
                {
                    await _contractService.DeleteAsync(id);
                    TempData["SuccessMessage"] = "Contract permanently deleted.";
                }
                else
                {
                    await _contractService.CancelContractAsync(id);
                    TempData["SuccessMessage"] = "Contract cancelled successfully.";
                }
                
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                TempData["ErrorMessage"] = "Contract not found.";
                _logger.LogWarning(ex, "Contract not found during deletion for ID {ContractId}", id);
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Business rule violation during contract deletion for ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing contract with ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while processing the contract.";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: Contract/Activate/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Activate(Guid id)
        {
            try
            {
                var result = await _contractService.ActivateContractAsync(id);
                TempData["SuccessMessage"] = $"Contract {result.ContractNumber} activated successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Business rule violation during contract activation for ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error activating contract with ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while activating the contract.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Contract/Suspend/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Suspend(Guid id)
        {
            try
            {
                var result = await _contractService.SuspendContractAsync(id);
                TempData["SuccessMessage"] = $"Contract {result.ContractNumber} suspended successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Business rule violation during contract suspension for ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error suspending contract with ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while suspending the contract.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Contract/Complete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Complete(Guid id)
        {
            try
            {
                var result = await _contractService.CompleteContractAsync(id);
                TempData["SuccessMessage"] = $"Contract {result.ContractNumber} completed successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Business rule violation during contract completion for ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error completing contract with ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while completing the contract.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Contract/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(Guid id)
        {
            try
            {
                var result = await _contractService.CancelContractAsync(id);
                TempData["SuccessMessage"] = $"Contract {result.ContractNumber} cancelled successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Business rule violation during contract cancellation for ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error cancelling contract with ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while cancelling the contract.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // POST: Contract/Default/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Default(Guid id)
        {
            try
            {
                var result = await _contractService.DefaultContractAsync(id);
                TempData["SuccessMessage"] = $"Contract {result.ContractNumber} marked as defaulted successfully.";
                return RedirectToAction(nameof(Details), new { id = result.Id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Business rule violation during contract default for ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error defaulting contract with ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while marking the contract as defaulted.";
            }

            return RedirectToAction(nameof(Details), new { id });
        }

        // GET: Contract/Installments/5
        public async Task<IActionResult> Installments(Guid id)
        {
            try
            {
                var contract = await _contractService.GetByIdAsync(id);
                if (contract == null)
                {
                    TempData["ErrorMessage"] = "Contract not found.";
                    return RedirectToAction(nameof(Index));
                }

                ViewData["Title"] = $"Installments - {contract.ContractNumber}";
                return View(contract);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving contract installments for ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while retrieving contract installments.";
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: Contract/GenerateInstallments/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateInstallments(Guid id)
        {
            try
            {
                var installments = await _contractService.GenerateInstallmentsAsync(id);
                TempData["SuccessMessage"] = $"{installments.Count} installments generated successfully.";
                return RedirectToAction(nameof(Installments), new { id });
            }
            catch (InvalidOperationException ex)
            {
                TempData["ErrorMessage"] = ex.Message;
                _logger.LogWarning(ex, "Business rule violation during installment generation for contract ID {ContractId}", id);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error generating installments for contract ID {ContractId}", id);
                TempData["ErrorMessage"] = "An error occurred while generating installments.";
            }

            return RedirectToAction(nameof(Installments), new { id });
        }

        // POST: Contract/RecalculateOutstanding/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RecalculateOutstanding(Guid id)
        {
            try
            {
                var result = await _contractService.RecalculateOutstandingAsync(id);
                return Json(new { success = true, outstandingAmount = result.OutstandingAmount, currency = result.Currency });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error recalculating outstanding amount for contract ID {ContractId}", id);
                return Json(new { success = false, message = "An error occurred while recalculating the outstanding amount." });
            }
        }

        // ========== Payment Processing Actions ==========

        // GET: Contract/PayInstallment/{installmentId}
        [HttpGet]
        public async Task<IActionResult> PayInstallment(Guid installmentId)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentSummaryAsync(installmentId);
                if (installment == null)
                {
                    return NotFound();
                }

                var model = new PaymentModalVM
                {
                    ContractId = Guid.Empty, // optional when called from installment context
                    InstallmentId = installment.Id,
                    InstallmentNumber = installment.InstallmentNumber,
                    InstallmentAmount = installment.Amount,
                    Amount = installment.Amount - installment.PaidAmount,
                    Currency = installment.Currency,
                    PaymentDate = DateTime.UtcNow.Date
                };

                return PartialView("_PaymentModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing payment modal for installment {InstallmentId}", installmentId);
                return StatusCode(500, "Failed to prepare payment form.");
            }
        }

        // POST: Contract/PayInstallment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PayInstallment([FromForm] PaymentModalVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var inst = await _installmentService.GetInstallmentSummaryAsync(model.InstallmentId);
                if (inst == null)
                {
                    return BadRequest(new { success = false, message = "Installment not found" });
                }
                var max = inst.Amount - inst.PaidAmount;
                if (model.Amount > max)
                {
                    return BadRequest(new { success = false, message = $"Amount exceeds outstanding ({max} {inst.Currency})." });
                }
                var result = await _paymentService.ProcessInstallmentPaymentAsync(model.InstallmentId, model);
                return Json(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while processing payment for installment {InstallmentId}", model.InstallmentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (ArgumentNullException ex)
            {
                _logger.LogWarning(ex, "Invalid payment request for installment {InstallmentId}", model.InstallmentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error processing payment for installment {InstallmentId}", model.InstallmentId);
                return StatusCode(500, new { success = false, message = "An error occurred while processing the payment." });
            }
        }

        [HttpGet]
        public async Task<IActionResult> InstallmentsPartial(Guid id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return PartialView("_ContractInstallments", contract.Installments);
        }

        [HttpGet]
        public async Task<IActionResult> PaymentsPartial(Guid id)
        {
            var contract = await _contractService.GetByIdAsync(id);
            if (contract == null) return NotFound();
            return PartialView("_ContractPayments", contract.Payments);
        }

        [HttpGet]
        public async Task<IActionResult> DownloadReceipt(Guid paymentId, string format = "pdf")
        {
            var file = await _paymentService.DownloadReceiptAsync(paymentId, format);
            var contentType = format?.ToLower() == "pdf" ? "application/pdf" : "application/zip";
            return File(file.Bytes, contentType, file.FileName);
        }

        // POST: Contract/ReversePayment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReversePayment(Guid paymentId, string reason)
        {
            if (paymentId == Guid.Empty)
            {
                return BadRequest(new { success = false, message = "Invalid payment id." });
            }

            try
            {
                var result = await _paymentService.ReversePaymentAsync(paymentId, reason);
                return Json(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while reversing payment {PaymentId}", paymentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error reversing payment {PaymentId}", paymentId);
                return StatusCode(500, new { success = false, message = "An error occurred while reversing the payment." });
            }
        }

        // GET: Contract/ViewReceipt/{paymentId}
        [HttpGet]
        public async Task<IActionResult> ViewReceipt(Guid paymentId)
        {
            try
            {
                var receipt = await _paymentService.GetReceiptByPaymentAsync(paymentId);
                if (receipt == null)
                {
                    return NotFound();
                }

                return View("Receipt", receipt);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving receipt for payment {PaymentId}", paymentId);
                return StatusCode(500, "Failed to retrieve receipt.");
            }
        }

        // POST: Contract/RegenerateReceipt
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegenerateReceipt(Guid paymentId)
        {
            try
            {
                var receipt = await _paymentService.RegenerateReceiptAsync(paymentId);
                return Json(new { success = true, data = receipt });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while regenerating receipt for payment {PaymentId}", paymentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error regenerating receipt for payment {PaymentId}", paymentId);
                return StatusCode(500, new { success = false, message = "An error occurred while regenerating the receipt." });
            }
        }

        // ========== Installment Management Actions ==========

        // GET: Contract/WaiveInstallment/{installmentId}
        [HttpGet]
        public async Task<IActionResult> WaiveInstallment(Guid installmentId)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentDetailAsync(installmentId);
                if (installment == null)
                {
                    return NotFound();
                }

                var model = new WaiveInstallmentVM
                {
                    InstallmentId = installment.Id,
                    InstallmentNumber = installment.InstallmentNumber,
                    InstallmentAmount = installment.Amount
                };

                return PartialView("_InstallmentActionsModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing waive installment modal for {InstallmentId}", installmentId);
                return StatusCode(500, "Failed to prepare waive installment form.");
            }
        }

        // POST: Contract/WaiveInstallment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> WaiveInstallment([FromForm] WaiveInstallmentVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _installmentService.WaiveInstallmentAsync(model.InstallmentId, model.Reason);
                return Json(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while waiving installment {InstallmentId}", model.InstallmentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error waiving installment {InstallmentId}", model.InstallmentId);
                return StatusCode(500, new { success = false, message = "An error occurred while waiving the installment." });
            }
        }

        // GET: Contract/RescheduleInstallment/{installmentId}
        [HttpGet]
        public async Task<IActionResult> RescheduleInstallment(Guid installmentId)
        {
            try
            {
                var installment = await _installmentService.GetInstallmentDetailAsync(installmentId);
                if (installment == null)
                {
                    return NotFound();
                }

                var model = new RescheduleInstallmentVM
                {
                    InstallmentId = installment.Id,
                    InstallmentNumber = installment.InstallmentNumber,
                    CurrentDueDate = installment.DueDate,
                    NewDueDate = installment.DueDate.AddDays(1)
                };

                return PartialView("_InstallmentActionsModal", model);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing reschedule installment modal for {InstallmentId}", installmentId);
                return StatusCode(500, "Failed to prepare reschedule form.");
            }
        }

        // POST: Contract/RescheduleInstallment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RescheduleInstallment([FromForm] RescheduleInstallmentVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                var result = await _installmentService.RescheduleInstallmentAsync(model.InstallmentId, model.NewDueDate, model.Reason);
                return Json(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while rescheduling installment {InstallmentId}", model.InstallmentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error rescheduling installment {InstallmentId}", model.InstallmentId);
                return StatusCode(500, new { success = false, message = "An error occurred while rescheduling the installment." });
            }
        }

        // GET: Contract/SplitInstallment/{installmentId}
        [HttpGet]
        public IActionResult SplitInstallment(Guid installmentId)
        {
            try
            {
                ViewData["InstallmentId"] = installmentId;
                return PartialView("_InstallmentActionsModal");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing split installment modal for {InstallmentId}", installmentId);
                return StatusCode(500, "Failed to prepare split form.");
            }
        }

        // POST: Contract/SplitInstallment
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SplitInstallment(Guid installmentId, [FromBody] decimal[] amounts)
        {
            if (installmentId == Guid.Empty || amounts == null || amounts.Length == 0)
            {
                return BadRequest(new { success = false, message = "Invalid split request." });
            }

            try
            {
                var result = await _installmentService.SplitInstallmentAsync(installmentId, amounts);
                return Json(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation while splitting installment {InstallmentId}", installmentId);
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error splitting installment {InstallmentId}", installmentId);
                return StatusCode(500, new { success = false, message = "An error occurred while splitting the installment." });
            }
        }

        // ========== Financial Analytics Actions ==========

        // GET: Contract/GetFinancialSummary/{contractId}
        [HttpGet]
        public async Task<IActionResult> GetFinancialSummary(Guid contractId)
        {
            try
            {
                var summary = await _installmentService.GetInstallmentStatusSummaryAsync(contractId);
                var vm = _mapper.Map<ContractFinancialSummaryVM>(summary);
                return Json(new { success = true, data = vm });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving financial summary for contract {ContractId}", contractId);
                return StatusCode(500, new { success = false, message = "Failed to load financial summary." });
            }
        }

        // GET: Contract/GetInstallmentAnalytics/{contractId}
        [HttpGet]
        public async Task<IActionResult> GetInstallmentAnalytics(Guid contractId)
        {
            try
            {
                var data = await _installmentService.GetInstallmentAnalyticsAsync(contractId);
                return Json(new { success = true, data });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving installment analytics for contract {ContractId}", contractId);
                return StatusCode(500, new { success = false, message = "Failed to load installment analytics." });
            }
        }

        // GET: Contract/GetPaymentHistory/{contractId}
        [HttpGet]
        public async Task<IActionResult> GetPaymentHistory(Guid contractId)
        {
            try
            {
                var payments = await _paymentService.GetPaymentsByContractAsync(contractId);
                return Json(new { success = true, data = payments });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving payment history for contract {ContractId}", contractId);
                return StatusCode(500, new { success = false, message = "Failed to load payment history." });
            }
        }

        // GET: Contract/ExportFinancialReport/{contractId}
        [HttpGet]
        public async Task<IActionResult> ExportFinancialReport(Guid contractId, string format = "excel")
        {
            try
            {
                var reportBytes = await _installmentService.ExportFinancialReportAsync(contractId, format);
                var fileName = $"contract-financial-report-{contractId}.{(format?.ToLower()=="pdf"?"pdf":"xlsx")}";
                var contentType = format?.ToLower() == "pdf" ? "application/pdf" : "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet";
                return File(reportBytes, contentType, fileName);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error exporting financial report for contract {ContractId}", contractId);
                return StatusCode(500, "Failed to export financial report.");
            }
        }

        // ========== Bulk Operations Actions ==========

        // GET: Contract/BulkInstallmentActions/{contractId}
        [HttpGet]
        public IActionResult BulkInstallmentActions(Guid contractId)
        {
            try
            {
                ViewData["ContractId"] = contractId;
                return View(new BulkInstallmentActionVM());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing bulk installment actions for contract {ContractId}", contractId);
                return StatusCode(500, "Failed to prepare bulk actions view.");
            }
        }

        // POST: Contract/BulkInstallmentActions
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkInstallmentActions([FromForm] BulkInstallmentActionVM model)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            try
            {
                object result;
                switch (model.ActionType)
                {
                    case BulkInstallmentActionType.Waive:
                        result = await _installmentService.WaiveInstallmentsAsync(model.InstallmentIds, model.CommonReason!);
                        break;
                    case BulkInstallmentActionType.Reschedule:
                        result = await _installmentService.RescheduleInstallmentsAsync(model.InstallmentIds, model.CommonNewDueDate!.Value, model.CommonReason);
                        break;
                    case BulkInstallmentActionType.MarkPaid:
                        result = await _installmentService.MarkInstallmentsAsPaidAsync(model.InstallmentIds);
                        break;
                    default:
                        return BadRequest(new { success = false, message = "Unsupported bulk action." });
                }

                return Json(new { success = true, data = result });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during bulk installment actions");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error executing bulk installment actions");
                return StatusCode(500, new { success = false, message = "An error occurred while executing bulk actions." });
            }
        }

        // GET: Contract/BulkPaymentProcessing/{contractId}
        [HttpGet]
        public IActionResult BulkPaymentProcessing(Guid contractId)
        {
            try
            {
                ViewData["ContractId"] = contractId;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error preparing bulk payment processing for contract {ContractId}", contractId);
                return StatusCode(500, "Failed to prepare bulk payment processing view.");
            }
        }

        // POST: Contract/BulkPaymentProcessing
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> BulkPaymentProcessing([FromBody] IEnumerable<PaymentModalVM> payments)
        {
            if (payments == null)
            {
                return BadRequest(new { success = false, message = "Invalid payment payload." });
            }

            try
            {
                var results = await _paymentService.ProcessBulkPaymentsAsync(payments);
                return Json(new { success = true, data = results });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogWarning(ex, "Business rule violation during bulk payment processing");
                return BadRequest(new { success = false, message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during bulk payment processing");
                return StatusCode(500, new { success = false, message = "An error occurred while processing payments." });
            }
        }
    }
}
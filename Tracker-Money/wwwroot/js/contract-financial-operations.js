/* global $, bootstrap */

/**
 * Initialize AJAX defaults and event bindings for financial operations
 */
function setupAjaxDefaults() {
	$.ajaxSetup({
		headers: {
			'RequestVerificationToken': $('input[name="__RequestVerificationToken"]').val() || ''
		}
	});
}

function showLoadingIndicator(element) {
	if (!element) return;
	const el = (element instanceof $) ? element : $(element);
	el.prop('disabled', true).addClass('disabled');
}

function hideLoadingIndicator(element) {
	if (!element) return;
	const el = (element instanceof $) ? element : $(element);
	el.prop('disabled', false).removeClass('disabled');
}

function showErrorMessage(message) {
	const text = message || 'Operation failed';
	console.error(text);
	toastr && toastr.error ? toastr.error(text) : alert(text);
}

function showSuccessMessage(message) {
	const text = message || 'Success';
	console.log(text);
	toastr && toastr.success ? toastr.success(text) : alert(text);
}

function formatCurrency(amount, currency) {
	try { return new Intl.NumberFormat(undefined, { style: 'currency', currency: currency || 'USD' }).format(amount || 0); } catch { return amount; }
}

function openPaymentModal(installmentId, installmentNumber, amount) {
	$.get('/Contract/PayInstallment', { installmentId: installmentId })
		.done(function (html) {
			$('#paymentModalContainer').html(html);
			$('#paymentModal').modal('show');
		})
		.fail(function () { showErrorMessage('Failed to load payment form'); });
}

function processPayment(formData) {
	return $.post('/Contract/PayInstallment', formData)
		.done(handlePaymentSuccess)
		.fail(handlePaymentError);
}

function handlePaymentSuccess(response) {
	if (response && response.success) {
		showSuccessMessage('Payment recorded');
		refreshInstallmentTable();
		refreshPaymentTable();
		updateFinancialSummary();
	} else {
		showErrorMessage(response && response.message);
	}
}

function handlePaymentError() { showErrorMessage('Payment failed'); }

function openInstallmentActionsModal(installmentId, actionType) {
	$.get('/Contract/WaiveInstallment', { installmentId: installmentId })
		.done(function (html) {
			$('#installmentActionsModalContainer').html(html);
			$('#installmentActionsModal').modal('show');
		})
		.fail(function () { showErrorMessage('Failed to load actions modal'); });
}

function waiveInstallment(installmentId, reason) {
	return $.post('/Contract/WaiveInstallment', { InstallmentId: installmentId, Reason: reason })
		.done(function () { showSuccessMessage('Installment waived'); refreshInstallmentTable(); updateFinancialSummary(); })
		.fail(function () { showErrorMessage('Waive failed'); });
}

function rescheduleInstallment(installmentId, newDate, reason) {
	return $.post('/Contract/RescheduleInstallment', { InstallmentId: installmentId, NewDueDate: newDate, Reason: reason })
		.done(function () { showSuccessMessage('Installment rescheduled'); refreshInstallmentTable(); updateFinancialSummary(); })
		.fail(function () { showErrorMessage('Reschedule failed'); });
}

function splitInstallment(installmentId, amounts) {
	return $.ajax({ url: '/Contract/SplitInstallment?installmentId=' + encodeURIComponent(installmentId), type: 'POST', data: JSON.stringify(amounts), contentType: 'application/json' })
		.done(function () { showSuccessMessage('Installment split'); refreshInstallmentTable(); updateFinancialSummary(); })
		.fail(function () { showErrorMessage('Split failed'); });
}

function confirmPaymentReversal(paymentId) {
	if (confirm('Reverse this payment?')) {
		const reason = prompt('Enter reversal reason (optional)') || '';
		reversePayment(paymentId, reason);
	}
}

function reversePayment(paymentId, reason) {
	return $.post('/Contract/ReversePayment', { paymentId: paymentId, reason: reason })
		.done(function (res) { if (res && res.success) { updatePaymentStatus(paymentId, 'Reversed'); refreshInstallmentTable(); refreshPaymentTable(); updateFinancialSummary(); showSuccessMessage('Payment reversed'); } else { showErrorMessage(res.message || 'Reverse failed'); } })
		.fail(function () { showErrorMessage('Reverse failed'); });
}

function updatePaymentStatus(paymentId, status) {
	$('[data-payment-id="' + paymentId + '"]').find('.payment-status').text(status);
}

function viewReceipt(paymentId) {
	window.open('/Contract/ViewReceipt?paymentId=' + encodeURIComponent(paymentId), '_blank');
}

function regenerateReceipt(paymentId) {
	return $.post('/Contract/RegenerateReceipt', { paymentId: paymentId })
		.done(function () { showSuccessMessage('Receipt regenerated'); })
		.fail(function () { showErrorMessage('Failed to regenerate receipt'); });
}

function printReceipt() { window.print(); }

function downloadReceipt(paymentId, format) {
	window.location.href = '/Contract/ExportFinancialReport?contractId=' + encodeURIComponent(paymentId) + '&format=' + encodeURIComponent(format || 'pdf');
}

function toggleBulkSelection(selectAll) {
	$('.installment-select').prop('checked', !!selectAll).trigger('change');
}

function processBulkActions(actionType, selectedIds) {
	return $.post('/Contract/BulkInstallmentActions', { ActionType: actionType, InstallmentIds: selectedIds })
		.done(function (res) { if (res && res.success) { handleBulkActionComplete(res.data); } else { showErrorMessage(res.message || 'Bulk action failed'); } })
		.fail(function () { showErrorMessage('Bulk action failed'); });
}

function showBulkActionProgress(actionType, total, completed) {
	console.log('Bulk ' + actionType + ': ' + completed + '/' + total);
}

function handleBulkActionComplete() {
	refreshInstallmentTable(); updateFinancialSummary(); showSuccessMessage('Bulk operation complete');
}

function refreshInstallmentTable() {
	const container = $('#installmentsTab');
	if (!container.length) return;
	const url = container.data('source');
	if (!url) return;
	container.load(url);
}

function refreshPaymentTable() {
	const container = $('#paymentsTab');
	if (!container.length) return;
	const url = container.data('source');
	if (!url) return;
	container.load(url);
}

function updateFinancialSummary() {
	const contractId = $('#contractDetailsRoot').data('contract-id');
	if (!contractId) return;
	$.get('/Contract/GetFinancialSummary', { contractId: contractId })
		.done(function (res) { if (res && res.success) { const d = res.data; $('[data-fin-outstanding]').text(formatCurrency(d.outstandingAmount, d.currency)); } });
}

function recalculateOutstanding() { updateFinancialSummary(); }

function bindEventHandlers() {
	$(document).on('click', '[data-action="record-payment"]', function(){
		const id = $(this).data('installment-id'); const num = $(this).data('installment-number'); const amt = $(this).data('installment-amount');
		openPaymentModal(id, num, amt);
	});
	$(document).on('click', '[data-action="waive-installment"]', function(){ openInstallmentActionsModal($(this).data('installment-id'), 'waive'); });
	$(document).on('click', '[data-action="reschedule-installment"]', function(){ openInstallmentActionsModal($(this).data('installment-id'), 'reschedule'); });
	$(document).on('click', '[data-action="split-installment"]', function(){ openInstallmentActionsModal($(this).data('installment-id'), 'split'); });
}

function initializeModals() {}
function resetModalForms() { $('#paymentForm, #waiveForm, #rescheduleForm, #splitForm').each(function(){ this && this.reset && this.reset(); }); }
function handleModalClose() { resetModalForms(); }

function initializeFinancialOperations() {
	setupAjaxDefaults();
	bindEventHandlers();
}

// Expose globally for views
window.openPaymentModal = openPaymentModal;
window.processPayment = processPayment;
window.handlePaymentSuccess = handlePaymentSuccess;
window.handlePaymentError = handlePaymentError;
window.openInstallmentActionsModal = openInstallmentActionsModal;
window.waiveInstallment = waiveInstallment;
window.rescheduleInstallment = rescheduleInstallment;
window.splitInstallment = splitInstallment;
window.confirmPaymentReversal = confirmPaymentReversal;
window.reversePayment = reversePayment;
window.updatePaymentStatus = updatePaymentStatus;
window.viewReceipt = viewReceipt;
window.regenerateReceipt = regenerateReceipt;
window.printReceipt = printReceipt;
window.downloadReceipt = downloadReceipt;
window.toggleBulkSelection = toggleBulkSelection;
window.processBulkActions = processBulkActions;
window.showBulkActionProgress = showBulkActionProgress;
window.handleBulkActionComplete = handleBulkActionComplete;
window.refreshInstallmentTable = refreshInstallmentTable;
window.refreshPaymentTable = refreshPaymentTable;
window.updateFinancialSummary = updateFinancialSummary;
window.recalculateOutstanding = recalculateOutstanding;
window.validatePaymentForm = function(){};
window.validateInstallmentAction = function(){};
window.showErrorMessage = showErrorMessage;
window.showSuccessMessage = showSuccessMessage;
window.formatCurrency = formatCurrency;
window.initializeModals = initializeModals;
window.resetModalForms = resetModalForms;
window.handleModalClose = handleModalClose;
window.bindEventHandlers = bindEventHandlers;
window.initializeFinancialOperations = initializeFinancialOperations;


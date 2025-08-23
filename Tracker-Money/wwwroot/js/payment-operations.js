/**
 * Payment Operations JavaScript
 * Handles payment processing, validation, and UI interactions
 */

// Global variables
let currentPaymentId = null;
let currentContractId = null;
let currentInstallmentId = null;

/**
 * Initialize payment operations
 */
$(document).ready(function() {
    initializePaymentOperations();
    setupEventListeners();
    setupValidation();
});

/**
 * Initialize payment operations
 */
function initializePaymentOperations() {
    console.log('Initializing payment operations...');
    
    // Set default payment date to today
    if ($('#paymentDate').length) {
        $('#paymentDate').val(new Date().toISOString().split('T')[0]);
    }
    
    // Initialize tooltips
    $('[data-bs-toggle="tooltip"]').tooltip();
    
    // Initialize currency formatting
    setupCurrencyFormatting();
}

/**
 * Setup event listeners
 */
function setupEventListeners() {
    // Contract selection change
    $(document).on('change', '#contractSelect', function() {
        const contractId = $(this).val();
        if (contractId) {
            loadInstallments(contractId);
        } else {
            $('#installmentSelect').html('<option value="">Select an installment...</option>').prop('disabled', true);
            $('#installmentDetails').hide();
        }
    });

    // Installment selection change
    $(document).on('change', '#installmentSelect', function() {
        const installmentId = $(this).val();
        if (installmentId) {
            loadInstallmentDetails(installmentId);
        } else {
            $('#installmentDetails').hide();
        }
    });

    // Amount validation
    $(document).on('input', '#paymentAmount, #modalAmount', function() {
        validatePaymentAmount();
    });

    // Form submission
    $(document).on('submit', '#paymentForm, #paymentModalForm', function(e) {
        if (!validatePaymentForm()) {
            e.preventDefault();
            return false;
        }
        
        // Disable submit button to prevent double submission
        const submitBtn = $(this).find('button[type="submit"]');
        submitBtn.prop('disabled', true).html('<i class="fas fa-spinner fa-spin me-1"></i>Processing...');
    });

    // Modal events
    $(document).on('show.bs.modal', '#paymentModal', function() {
        setupModalValidation();
    });

    $(document).on('hidden.bs.modal', '#paymentModal', function() {
        resetModalForm();
    });
}

/**
 * Setup validation
 */
function setupValidation() {
    // Custom validation for payment amount
    $.validator.addMethod('paymentAmount', function(value, element) {
        const amount = parseFloat(value);
        const installmentAmount = parseFloat($('#installmentSelect option:selected').data('amount') || 0);
        
        if (installmentAmount > 0 && amount > installmentAmount) {
            return false;
        }
        
        return amount > 0;
    }, 'Payment amount cannot exceed installment amount and must be greater than 0');

    // Custom validation for payment date
    $.validator.addMethod('paymentDate', function(value, element) {
        const paymentDate = new Date(value);
        const today = new Date();
        today.setHours(0, 0, 0, 0);
        
        return paymentDate <= today;
    }, 'Payment date cannot be in the future');
}

/**
 * Setup currency formatting
 */
function setupCurrencyFormatting() {
    // Format currency inputs
    $('.currency-input').on('input', function() {
        const value = $(this).val().replace(/[^\d.]/g, '');
        const formatted = parseFloat(value).toFixed(2);
        $(this).val(formatted);
    });
}

/**
 * Load contracts for dropdown
 */
function loadContracts() {
    showLoading('#contractSelect');
    
    $.get('/api/contracts', function(response) {
        const select = $('#contractSelect');
        select.find('option:not(:first)').remove();
        
        // Handle both paged and non-paged responses
        let contractsData;
        if (response.success && response.data) {
            // Wrapped API response
            contractsData = response.data.items || response.data;
        } else {
            // Direct response (non-wrapped)
            contractsData = response.items || response.data || response;
        }
        
        contractsData.forEach(function(contract) {
            select.append(`<option value="${contract.id}">${contract.contractNumber} - ${contract.customerName}</option>`);
        });
        
        hideLoading('#contractSelect');
    }).fail(function() {
        hideLoading('#contractSelect');
        showAlert('Error loading contracts', 'danger');
    });
}

/**
 * Load installments for selected contract
 */
function loadInstallments(contractId) {
    showLoading('#installmentSelect');
    
    $.get(`/api/contracts/${contractId}/installments`, function(response) {
        const select = $('#installmentSelect');
        select.html('<option value="">Select an installment...</option>').prop('disabled', false);
        
        // Handle both paged and non-paged responses
        let installmentsData;
        if (response.success && response.data) {
            // Wrapped API response
            installmentsData = response.data.items || response.data;
        } else {
            // Direct response (non-wrapped)
            installmentsData = response.items || response.data || response;
        }
        
        installmentsData.forEach(function(installment) {
            if (installment.status !== 'Paid') {
                select.append(`<option value="${installment.id}" 
                    data-amount="${installment.amount}" 
                    data-due-date="${installment.dueDate}"
                    data-currency="${installment.currency}">
                    Installment #${installment.installmentNumber} - ${installment.amount} ${installment.currency}
                </option>`);
            }
        });
        
        hideLoading('#installmentSelect');
    }).fail(function() {
        hideLoading('#installmentSelect');
        showAlert('Error loading installments', 'danger');
    });
}

/**
 * Load installment details
 */
function loadInstallmentDetails(installmentId) {
    const selectedOption = $(`#installmentSelect option[value="${installmentId}"]`);
    const amount = selectedOption.data('amount');
    const dueDate = selectedOption.data('due-date');
    const currency = selectedOption.data('currency');
    
    $('#amountDue').text(`${amount} ${currency}`);
    $('#dueDate').text(new Date(dueDate).toLocaleDateString());
    $('#installmentDetails').show();
    
    // Set payment amount to installment amount by default
    $('#paymentAmount, #modalAmount').val(amount);
    
    // Update currency if available
    if (currency && $('#currencySelect').length) {
        $('#currencySelect').val(currency);
    }
}

/**
 * Validate payment amount
 */
function validatePaymentAmount() {
    const amountInput = $('#paymentAmount, #modalAmount');
    const amount = parseFloat(amountInput.val());
    const installmentAmount = parseFloat($('#installmentSelect option:selected').data('amount') || 0);
    
    if (amount > installmentAmount) {
        amountInput.addClass('is-invalid');
        if (!amountInput.next('.invalid-feedback').length) {
            amountInput.after('<div class="invalid-feedback">Payment amount cannot exceed installment amount</div>');
        }
        return false;
    } else {
        amountInput.removeClass('is-invalid');
        amountInput.next('.invalid-feedback').remove();
        return true;
    }
}

/**
 * Validate payment form
 */
function validatePaymentForm() {
    let isValid = true;
    
    // Basic validation
    const requiredFields = ['contractSelect', 'installmentSelect', 'paymentAmount', 'paymentDate', 'paymentMethod'];
    
    requiredFields.forEach(function(fieldId) {
        const field = $(`#${fieldId}`);
        if (field.length && !field.val()) {
            field.addClass('is-invalid');
            isValid = false;
        } else {
            field.removeClass('is-invalid');
        }
    });
    
    // Amount validation
    if (!validatePaymentAmount()) {
        isValid = false;
    }
    
    if (!isValid) {
        showAlert('Please fill in all required fields correctly', 'warning');
    }
    
    return isValid;
}

/**
 * Process payment via AJAX
 */
function processPayment(formData) {
    return $.ajax({
        url: '/api/payments',
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify(formData),
        beforeSend: function() {
            showLoading('#paymentForm');
        }
    }).always(function() {
        hideLoading('#paymentForm');
    });
}

/**
 * Show payment modal
 */
function showPaymentModal(contractId, installmentId, installmentNumber, amount, dueDate, status) {
    // Populate modal fields
    $('#modalContractId').val(contractId);
    $('#modalInstallmentId').val(installmentId);
    $('#modalInstallmentAmount').val(amount);
    $('#modalAmount').val(amount);
    $('#modalInstallmentNumber').text('#' + installmentNumber);
    $('#modalAmountDue').text('$' + amount);
    $('#modalDueDate').text(new Date(dueDate).toLocaleDateString());
    
    // Set status badge
    const statusBadge = $('#modalStatus');
    statusBadge.removeClass().addClass('badge');
    switch (status) {
        case 'Pending':
            statusBadge.addClass('bg-warning').text('Pending');
            break;
        case 'Overdue':
            statusBadge.addClass('bg-danger').text('Overdue');
            break;
        case 'Paid':
            statusBadge.addClass('bg-success').text('Paid');
            break;
        default:
            statusBadge.addClass('bg-secondary').text(status);
    }

    // Clear optional fields
    $('#modalReferenceNumber').val('');
    $('#modalNotes').val('');

    // Show modal
    $('#paymentModal').modal('show');
}

/**
 * Show reverse payment modal
 */
function showReversePaymentModal(paymentId, paymentData) {
    // Populate modal fields
    $('#reversePaymentId').text(paymentId);
    $('#reversePaymentIdInput').val(paymentId);
    $('#reversePaymentAmount').text(paymentData.amount + ' ' + paymentData.currency);
    $('#reversePaymentDate').text(new Date(paymentData.paymentDate).toLocaleDateString());
    $('#reversePaymentMethod').text(paymentData.paymentMethod);

    // Clear form
    $('#reason').val('');
    $('#confirmReversal').prop('checked', false);
    $('#reverseSubmitBtn').prop('disabled', true);

    // Show modal
    $('#reversePaymentModal').modal('show');
}

/**
 * Reverse payment via AJAX
 */
function reversePayment(paymentId, reason) {
    if (!reason || reason.trim() === '') {
        showAlert('Please provide a reason for the reversal', 'warning');
        return;
    }

    return $.ajax({
        url: `/api/payments/${paymentId}/reverse`,
        method: 'POST',
        contentType: 'application/json',
        data: JSON.stringify({
            paymentId: paymentId,
            reason: reason
        })
    });
}

/**
 * Download receipt
 */
function downloadReceipt(paymentId, format = 'pdf') {
    window.open(`/Payment/DownloadReceipt/${paymentId}?format=${format}`, '_blank');
}

/**
 * Regenerate receipt
 */
function regenerateReceipt(paymentId) {
    if (confirm('Are you sure you want to regenerate the receipt? This will create a new receipt number.')) {
        // Create a form and submit it
        const form = document.createElement('form');
        form.method = 'POST';
        form.action = '/Payment/RegenerateReceipt';
        
        const csrfToken = document.querySelector('input[name="__RequestVerificationToken"]').value;
        const csrfInput = document.createElement('input');
        csrfInput.type = 'hidden';
        csrfInput.name = '__RequestVerificationToken';
        csrfInput.value = csrfToken;
        
        const idInput = document.createElement('input');
        idInput.type = 'hidden';
        idInput.name = 'id';
        idInput.value = paymentId;
        
        form.appendChild(csrfInput);
        form.appendChild(idInput);
        document.body.appendChild(form);
        form.submit();
    }
}

/**
 * Search payments
 */
function searchPayments(searchTerm, page = 1) {
    const url = `/Payment/Search?searchTerm=${encodeURIComponent(searchTerm)}&page=${page}`;
    
    $.get(url, function(data) {
        $('#paymentSearchResults').html(data);
    });
}

/**
 * Export payments
 */
function exportPayments(format = 'excel') {
    const searchTerm = $('#searchTerm').val() || '';
    const url = `/Payment/Export?format=${format}&searchTerm=${encodeURIComponent(searchTerm)}`;
    window.open(url, '_blank');
}

/**
 * Refresh payments
 */
function refreshPayments() {
    window.location.reload();
}

/**
 * Show loading state
 */
function showLoading(selector) {
    $(selector).addClass('loading');
    $(selector).prop('disabled', true);
}

/**
 * Hide loading state
 */
function hideLoading(selector) {
    $(selector).removeClass('loading');
    $(selector).prop('disabled', false);
}

/**
 * Show alert message
 */
function showAlert(message, type = 'info') {
    const alertHtml = `
        <div class="alert alert-${type} alert-dismissible fade show" role="alert">
            <i class="fas fa-exclamation-triangle me-2"></i>${message}
            <button type="button" class="btn-close" data-bs-dismiss="alert" aria-label="Close"></button>
        </div>
    `;
    
    $('.container-fluid').prepend(alertHtml);
    
    // Auto-hide after 5 seconds
    setTimeout(function() {
        $('.alert').alert('close');
    }, 5000);
}

/**
 * Setup modal validation
 */
function setupModalValidation() {
    // Set default payment date to today
    $('#modalPaymentDate').val(new Date().toISOString().split('T')[0]);
    
    // Setup amount validation
    $('#modalAmount').on('input', function() {
        validatePaymentAmount();
    });
}

/**
 * Reset modal form
 */
function resetModalForm() {
    $('#paymentModalForm')[0].reset();
    $('#modalAmount').removeClass('is-invalid');
    $('#modalAmount').next('.invalid-feedback').remove();
}

/**
 * Format currency
 */
function formatCurrency(amount, currency = 'USD') {
    return new Intl.NumberFormat('en-US', {
        style: 'currency',
        currency: currency
    }).format(amount);
}

/**
 * Parse currency string to number
 */
function parseCurrency(currencyString) {
    return parseFloat(currencyString.replace(/[^\d.-]/g, ''));
}

/**
 * Validate currency code
 */
function validateCurrencyCode(currencyCode) {
    const validCurrencies = ['USD', 'EUR', 'GBP', 'JPY', 'CAD', 'AUD', 'CHF', 'CNY'];
    return validCurrencies.includes(currencyCode.toUpperCase());
}

/**
 * Get payment method display name
 */
function getPaymentMethodDisplayName(method) {
    const methodNames = {
        'Cash': 'Cash',
        'BankTransfer': 'Bank Transfer',
        'CreditCard': 'Credit Card',
        'DebitCard': 'Debit Card',
        'Check': 'Check',
        'MobilePayment': 'Mobile Payment',
        'Other': 'Other'
    };
    
    return methodNames[method] || method;
}

/**
 * Get payment method badge class
 */
function getPaymentMethodBadgeClass(method) {
    const badgeClasses = {
        'Cash': 'payment-method-cash',
        'BankTransfer': 'payment-method-bank',
        'CreditCard': 'payment-method-card',
        'DebitCard': 'payment-method-card',
        'Check': 'payment-method-check',
        'MobilePayment': 'payment-method-mobile',
        'Other': 'payment-method-other'
    };
    
    return badgeClasses[method] || 'payment-method-other';
}

// Export functions for global access
window.PaymentOperations = {
    showPaymentModal,
    showReversePaymentModal,
    processPayment,
    reversePayment,
    downloadReceipt,
    regenerateReceipt,
    searchPayments,
    exportPayments,
    refreshPayments,
    formatCurrency,
    parseCurrency,
    validateCurrencyCode,
    getPaymentMethodDisplayName,
    getPaymentMethodBadgeClass
}; 
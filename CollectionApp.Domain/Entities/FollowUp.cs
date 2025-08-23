using System;
using CollectionApp.Domain.Common;

namespace CollectionApp.Domain.Entities
{
    public class FollowUp : BaseEntity
    {
        // Required by EF Core
        private FollowUp() { }
        public Guid CustomerId { get; private set; }
        public Guid? ContractId { get; private set; }
        public Guid StaffId { get; private set; }
        public DateTime ScheduledDate { get; private set; }
        public DateTime? ActualDate { get; private set; }
        public string Type { get; private set; }
        public string Priority { get; private set; }
        public string Status { get; private set; }
        public string? Notes { get; private set; }
        public string? Outcome { get; private set; }

        // New properties for expanded functionality
        public Guid? AssignedToStaffId { get; private set; }
        public string? Description { get; private set; }

        public Customer? Customer { get; private set; }
        public Contract? Contract { get; private set; }
        public Staff? Staff { get; private set; }

        // Original constructor for backward compatibility
        public FollowUp(Guid customerId, Guid staffId, DateTime scheduledDate, string type, string priority, string status, Guid? contractId = null, string? notes = null)
        {
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException("CustomerId required", nameof(customerId));
            StaffId = staffId != Guid.Empty ? staffId : throw new ArgumentException("StaffId required", nameof(staffId));
            ScheduledDate = scheduledDate;
            Type = ValidateRequired(type, nameof(type));
            Priority = ValidateRequired(priority, nameof(priority));
            Status = ValidateRequired(status, nameof(status));
            ContractId = contractId;
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }

        // New overloaded constructor for expanded functionality
        public FollowUp(
            Guid customerId,
            Guid? contractId,
            Guid? assignedToStaffId,
            string type,
            string priority,
            string? description,
            DateTime scheduledDate,
            string? notes)
        {
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException("CustomerId required", nameof(customerId));
            ContractId = contractId;
            AssignedToStaffId = assignedToStaffId;
            if (assignedToStaffId.HasValue) StaffId = assignedToStaffId.Value;
            Type = ValidateRequired(type, nameof(type));
            Priority = ValidateRequired(priority, nameof(priority));
            Status = "Pending"; // Default status
            Description = description?.Trim();
            ScheduledDate = scheduledDate;
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }

        private static string ValidateRequired(string value, string name)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException($"{name} required", name);
            return value.Trim();
        }

        public void MarkCompleted(string outcome)
        {
            ActualDate = DateTime.UtcNow;
            Outcome = string.IsNullOrWhiteSpace(outcome) ? null : outcome.Trim();
            Status = "Completed";
            Touch();
        }

        public void Escalate(string priority)
        {
            Priority = ValidateRequired(priority, nameof(priority));
            Status = "Escalated";
            Touch();
        }

        public void Escalate()
        {
            Status = "Escalated";
            Touch();
        }

        public void UpdateSchedule(DateTime? scheduledDate)
        {
            if (scheduledDate.HasValue)
            {
                ScheduledDate = scheduledDate.Value;
                Touch();
            }
        }

        public void UpdateDetails(string? type, string? priority, string? status, string? notes)
        {
            if (!string.IsNullOrWhiteSpace(type)) Type = ValidateRequired(type, nameof(type));
            if (!string.IsNullOrWhiteSpace(priority)) Priority = ValidateRequired(priority, nameof(priority));
            if (!string.IsNullOrWhiteSpace(status)) Status = ValidateRequired(status, nameof(status));
            if (notes != null) Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        public void UpdateDetails(string? type, string? priority, string? description, DateTime? scheduledDate, string? notes)
        {
            if (!string.IsNullOrWhiteSpace(type)) Type = ValidateRequired(type, nameof(type));
            if (!string.IsNullOrWhiteSpace(priority)) Priority = ValidateRequired(priority, nameof(priority));
            if (!string.IsNullOrWhiteSpace(description)) Description = description.Trim();
            if (scheduledDate.HasValue) ScheduledDate = scheduledDate.Value;
            if (notes != null) Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        public void Complete(string? outcome, DateTime? actualDate)
        {
            Outcome = string.IsNullOrWhiteSpace(outcome) ? null : outcome.Trim();
            ActualDate = actualDate ?? DateTime.UtcNow;
            Status = "Completed";
            Touch();
        }

        // New methods for expanded functionality
        public void Reschedule(DateTime newScheduledDate)
        {
            ScheduledDate = newScheduledDate;
            Touch();
        }

        public void AssignToStaff(Guid newStaffId)
        {
            if (newStaffId == Guid.Empty) throw new ArgumentException("StaffId required", nameof(newStaffId));
            AssignedToStaffId = newStaffId;
            StaffId = newStaffId; // Maintain backward compatibility
            Touch();
        }

        public void UpdatePriority(string newPriority)
        {
            Priority = ValidateRequired(newPriority, nameof(newPriority));
            Touch();
        }

        public void UpdateStatus(string newStatus)
        {
            Status = ValidateRequired(newStatus, nameof(newStatus));
            Touch();
        }

        public void Archive()
        {
            Status = "Archived";
            Touch();
        }

        public void LinkToPayment(Guid paymentId)
        {
            // This method would typically link the follow-up to a payment record
            // Implementation depends on the relationship between FollowUp and Payment
            Touch();
        }
    }
}


using System;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Domain.Entities
{
    public class Visit : BaseEntity
    {
        public Guid CustomerId { get; private set; }
        public Guid StaffId { get; private set; }
        public DateTime VisitDate { get; private set; }
        public string Purpose { get; private set; }
        public string? Notes { get; private set; }
        public string? Outcome { get; private set; }
        public DateTime? NextVisitDate { get; private set; }
        public TimeSpan? Duration { get; private set; }

        // New properties for expanded functionality
        public string? VisitType { get; private set; }
        public string? Location { get; private set; }
        public VisitStatus Status { get; private set; } = VisitStatus.Scheduled;

        public Customer? Customer { get; private set; }
        public Staff? Staff { get; private set; }

        // Original constructor for backward compatibility
        public Visit(Guid customerId, Guid staffId, DateTime visitDate, string purpose, string? notes = null, TimeSpan? duration = null)
        {
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException("CustomerId required", nameof(customerId));
            StaffId = staffId != Guid.Empty ? staffId : throw new ArgumentException("StaffId required", nameof(staffId));
            VisitDate = visitDate;
            Purpose = ValidatePurpose(purpose);
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Duration = duration;
        }

        // New overloaded constructor for expanded functionality
        public Visit(
            Guid customerId,
            Guid staffId,
            DateTime visitDate,
            TimeSpan? duration,
            string? visitType,
            string? location,
            string purpose,
            string? notes)
        {
            CustomerId = customerId != Guid.Empty ? customerId : throw new ArgumentException("CustomerId required", nameof(customerId));
            StaffId = staffId != Guid.Empty ? staffId : throw new ArgumentException("StaffId required", nameof(staffId));
            VisitDate = visitDate;
            Duration = duration;
            VisitType = visitType?.Trim();
            Location = location?.Trim();
            Purpose = ValidatePurpose(purpose);
            Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
        }

        private static string ValidatePurpose(string value)
        {
            if (string.IsNullOrWhiteSpace(value)) throw new ArgumentException("Purpose required", nameof(value));
            return value.Trim();
        }

        public void RecordOutcome(string outcome, DateTime? nextVisitDate = null)
        {
            Outcome = string.IsNullOrWhiteSpace(outcome) ? null : outcome.Trim();
            NextVisitDate = nextVisitDate;
            Touch();
        }

        // New methods for expanded functionality
        public void UpdateDetails(string? visitType, string? location, string? purpose, string? notes)
        {
            if (visitType != null) VisitType = visitType.Trim();
            if (location != null) Location = location.Trim();
            if (purpose != null) Purpose = ValidatePurpose(purpose);
            if (notes != null) Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        public void UpdateDetails(DateTime visitDate, TimeSpan? duration, string? visitType, string? location, string? purpose, string? notes)
        {
            VisitDate = visitDate;
            if (duration.HasValue) Duration = duration.Value;
            if (visitType != null) VisitType = visitType.Trim();
            if (location != null) Location = location.Trim();
            if (purpose != null) Purpose = ValidatePurpose(purpose);
            if (notes != null) Notes = string.IsNullOrWhiteSpace(notes) ? null : notes.Trim();
            Touch();
        }

        public void UpdateOutcome(string? outcome, DateTime? nextVisitDate)
        {
            Outcome = string.IsNullOrWhiteSpace(outcome) ? null : outcome.Trim();
            NextVisitDate = nextVisitDate;
            Touch();
        }

        public void Complete()
        {
            Status = VisitStatus.Completed;
            Touch();
        }

        public void Cancel()
        {
            Status = VisitStatus.Cancelled;
            Touch();
        }

        public void Cancel(string? reason)
        {
            Status = VisitStatus.Cancelled;
            Notes = string.IsNullOrWhiteSpace(reason) ? Notes : reason.Trim();
            Touch();
        }

        public void Reschedule(DateTime newVisitDate)
        {
            VisitDate = newVisitDate;
            Status = VisitStatus.Scheduled;
            Touch();
        }

        public void AssignToStaff(Guid newStaffId)
        {
            if (newStaffId == Guid.Empty) throw new ArgumentException("StaffId required", nameof(newStaffId));
            StaffId = newStaffId;
            Touch();
        }

        public void LinkToPayment(Guid paymentId)
        {
            // This method would typically link the visit to a payment record
            // Implementation depends on the relationship between Visit and Payment
            Touch();
        }
    }
}


using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface IPaymentRepository : IRepository<Payment>
    {
        Task<IReadOnlyList<Payment>> GetByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Payment>> GetByInstallmentIdAsync(Guid installmentId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Payment>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Payment>> GetByStaffIdAsync(Guid staffId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<IEnumerable<Payment>> GetByContractIdAsync(Guid contractId);
        Task<IEnumerable<Payment>> GetByContractIdsAsync(IEnumerable<Guid> contractIds);

        Task<IReadOnlyList<Payment>> GetPaymentHistoryAsync(
            Guid? contractId = null,
            Guid? customerId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null);

        Task<IReadOnlyList<Payment>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default);

    }
}
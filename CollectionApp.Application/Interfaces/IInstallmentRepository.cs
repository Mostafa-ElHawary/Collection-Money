using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface IInstallmentRepository : IRepository<Installment>
    {
        Task<IReadOnlyList<Installment>> GetByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Installment>> GetByContractIdsAsync(IEnumerable<Guid> contractIds, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Installment>> GetOverdueAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Installment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<decimal> GetTotalDueAmount();
        Task<IReadOnlyList<Installment>> GetOverdueInstallments();

    }
}
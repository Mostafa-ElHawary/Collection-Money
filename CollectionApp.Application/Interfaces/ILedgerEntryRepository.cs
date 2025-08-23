using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface ILedgerEntryRepository : IRepository<LedgerEntry>
    {
        Task<IReadOnlyList<LedgerEntry>> GetByReferenceAsync(Guid? customerId, Guid? contractId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<LedgerEntry>> GetByReferenceAsync(Guid? customerId, Guid? contractId, string? referenceType, Guid referenceId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<LedgerEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    }
} 
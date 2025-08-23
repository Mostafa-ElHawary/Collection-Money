using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface IVisitRepository : IRepository<Visit>
    {
        Task<IReadOnlyList<Visit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Visit>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Visit>> GetByStaffIdAsync(Guid staffId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Visit>> GetScheduledAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Visit>> GetCompletedAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Visit>> GetOverdueAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Visit>> GetByTerritoryAsync(string territory, CancellationToken cancellationToken = default);
    }
} 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface IFollowUpRepository : IRepository<FollowUp>
    {
        Task<IReadOnlyList<FollowUp>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetByStaffIdAsync(Guid staffId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetOverdueAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetByPriorityAsync(string priority, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetByStatusAsync(string status, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetByTypeAsync(string type, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<FollowUp>> GetCompletedAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default);
    }
} 
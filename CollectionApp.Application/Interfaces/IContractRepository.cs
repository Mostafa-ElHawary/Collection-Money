using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using System.Threading;
using CollectionApp.Application.Common;

namespace CollectionApp.Application.Interfaces
{
    public interface IContractRepository : IRepository<Contract>
    {
        Task<Contract?> GetByContractNumberAsync(string contractNumber, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Contract>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Contract>> GetByStatusAsync(ContractStatus status, CancellationToken cancellationToken = default);
        Task<PagedResult<Contract>> GetByStatusPagedAsync(ContractStatus status, int pageNumber, int pageSize, CancellationToken cancellationToken = default);
        Task<PagedResult<Contract>> GetByStatusPagedAsync(ContractStatus status, int pageNumber, int pageSize, string filter, string orderBy, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Contract>> GetOverdueAsync(CancellationToken cancellationToken = default);
        Task<bool> HasActiveContractsAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Contract>> GetAllAsync(CancellationToken cancellationToken = default);
    }
}
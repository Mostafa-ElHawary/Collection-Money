using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface ICustomerRepository : IRepository<Customer>
    {
        Task<Customer?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default);
        Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);
    }
} 
using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface IStaffRepository : IRepository<Staff>
    {
        Task<Staff?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default);
        Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Staff>> GetActiveAsync(CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Staff>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Staff>> GetByPositionAsync(string position, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Staff>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    }
} 
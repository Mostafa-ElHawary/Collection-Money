using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using CollectionApp.Infrastructure.Repositories;
using CollectionApp.Application.Common;

namespace CollectionApp.Infrastructure.Repositories
{
    public class StaffRepository : Repository<Staff>, IStaffRepository
    {
        public StaffRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Staff?> GetByEmployeeIdAsync(string employeeId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(employeeId))
                return null;

            var normalizedEmployeeId = employeeId.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefaultAsync(s => EF.Functions.Like(s.EmployeeId, normalizedEmployeeId), cancellationToken);
        }

        public async Task<Staff?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalizedEmail = email.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .FirstOrDefaultAsync(s => EF.Functions.Like(s.Email, normalizedEmail), cancellationToken);
        }

        public async Task<IReadOnlyList<Staff>> GetActiveAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Where(s => s.IsActive)
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Staff>> GetByDepartmentAsync(string department, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(department))
                return new List<Staff>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Where(s => EF.Functions.Like(s.Department, department))
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Staff>> GetByPositionAsync(string position, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(position))
                return new List<Staff>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Where(s => EF.Functions.Like(s.Position, position))
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Staff>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Staff>();

            var normalizedSearchTerm = searchTerm.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Where(s => EF.Functions.Like(s.FirstName, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(s.LastName, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(s.EmployeeId, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(s.Email, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(s.Department, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(s.Position, $"%{normalizedSearchTerm}%"))
                .OrderBy(s => s.FirstName)
                .ThenBy(s => s.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedResult<Staff>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                filter, 
                orderBy, 
                pageNumber, 
                pageSize,
                q => q, // No includes needed for Staff
                q => q.OrderBy(s => s.FirstName).ThenBy(s => s.LastName).ThenBy(s => s.Id),
                cancellationToken);
        }
    }
} 
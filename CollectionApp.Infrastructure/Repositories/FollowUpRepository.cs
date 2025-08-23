using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using CollectionApp.Infrastructure.Repositories;
using CollectionApp.Application.Common;
using CollectionApp.Domain.Enums;

namespace CollectionApp.Infrastructure.Repositories
{
    public class FollowUpRepository : Repository<FollowUp>, IFollowUpRepository
    {
        public FollowUpRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<FollowUp?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<FollowUp?> GetByIdWithContractAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .FirstOrDefaultAsync(f => f.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByContractIdAsync(Guid? contractId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .Where(f => f.ContractId == contractId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByCustomerIdAsync(Guid? customerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .Where(f => f.CustomerId == customerId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .Where(f => f.StaffId == staffId)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByStatusAsync(FollowUpStatus status, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .Where(f => f.Status.Equals(status))
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByPriorityAsync(string priority, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .Where(f => f.Priority == priority)
                .OrderByDescending(f => f.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByDueDateAsync(DateTime dueDate, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .Where(f => f.ScheduledDate.Date == dueDate.Date)
                .OrderBy(f => f.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetOverdueAsync(CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.UtcNow.Date;
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Contract)
                .ThenInclude(c => c.Staff)
                .Include(f => f.Customer)
                .Include(f => f.Staff)
                .Where(f => f.ScheduledDate < currentDate && f.Status.Equals(FollowUpStatus.Completed) == false)


                .OrderBy(f => f.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<FollowUp>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Customer)
                .Include(f => f.Contract)
                .Include(f => f.Staff)
                .Where(f => f.ScheduledDate >= fromDate && f.ScheduledDate <= toDate)
                .OrderBy(f => f.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByPriorityFilterAsync(string priority, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(priority))
                return new List<FollowUp>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Customer)
                .Include(f => f.Contract)
                .Include(f => f.Staff)
                .Where(f => EF.Functions.Like(f.Priority, $"%{priority}%"))
                .OrderBy(f => f.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetByStatusAsync(string status, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(status))
                return new List<FollowUp>();

            // Try to parse the status string to enum
            if (Enum.TryParse<FollowUpStatus>(status, true, out var statusEnum))
            {
                return await DbSet
                    .AsNoTrackingWithIdentityResolution()
                    .Include(f => f.Customer)
                    .Include(f => f.Contract)
                    .Include(f => f.Staff)
                    .Where(f => f.Status.Equals(statusEnum))
                    .OrderBy(f => f.ScheduledDate)
                    .ToListAsync(cancellationToken);
            }
            else
            {
                return await DbSet
                    .AsNoTrackingWithIdentityResolution()
                    .Include(f => f.Customer)
                    .Include(f => f.Contract)
                    .Include(f => f.Staff)
                    .Where(f => f.Status.ToString() == status)
                    .OrderBy(f => f.ScheduledDate)
                    .ToListAsync(cancellationToken);
            }
        }

        public async Task<IReadOnlyList<FollowUp>> GetByTypeAsync(string type, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(type))
                return new List<FollowUp>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Customer)
                .Include(f => f.Contract)
                .Include(f => f.Staff)
                .Where(f => EF.Functions.Like(f.Type, $"%{type}%"))
                .OrderBy(f => f.ScheduledDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<FollowUp>> GetCompletedAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<FollowUp>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(f => f.Customer)
                .Include(f => f.Contract)
                .Include(f => f.Staff)
                .Where(f => string.Equals(f.Status, "Completed", StringComparison.OrdinalIgnoreCase) && 
                           f.ActualDate >= fromDate && 
                           f.ActualDate <= toDate)
                .OrderByDescending(f => f.ActualDate)
                .ToListAsync(cancellationToken);
        }

        public new async Task<IPagedResult<FollowUp>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                filter, 
                orderBy, 
                pageNumber, 
                pageSize,
                q => q.Include(f => f.Customer).Include(f => f.Contract).ThenInclude(c => c.Staff).Include(f => f.Staff),
                q => q.OrderByDescending(f => f.CreatedAt).ThenBy(f => f.Id),
                cancellationToken);
        }

        
        /// Builds domain-specific filter expressions for FollowUp entity
       
        private string? BuildDomainSpecificFilter(string? filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return filter;

            var filters = new List<string>();

            // Parse status parameter for exact match
            if (filter.Contains("status="))
            {
                var statusMatch = System.Text.RegularExpressions.Regex.Match(filter, @"status=([^&\s]+)");
                if (statusMatch.Success)
                {
                    var statusTerm = statusMatch.Groups[1].Value;
                    filters.Add($"Status == \"{statusTerm}\"");
                }
            }

            // Parse priority parameter for Like match
            if (filter.Contains("priority="))
            {
                var priorityMatch = System.Text.RegularExpressions.Regex.Match(filter, @"priority=([^&\s]+)");
                if (priorityMatch.Success)
                {
                    var priorityTerm = priorityMatch.Groups[1].Value;
                    filters.Add($"Priority.Contains(\"{priorityTerm}\")");
                }
            }

            // Parse type parameter for Like match
            if (filter.Contains("type="))
            {
                var typeMatch = System.Text.RegularExpressions.Regex.Match(filter, @"type=([^&\s]+)");
                if (typeMatch.Success)
                {
                    var typeTerm = typeMatch.Groups[1].Value;
                    filters.Add($"Type.Contains(\"{typeTerm}\")");
                }
            }

            // If we have domain-specific filters, combine them with AND
            if (filters.Any())
            {
                var domainFilter = string.Join(" AND ", filters);
                
                // Remove domain-specific parameters from the original filter
                var cleanedFilter = System.Text.RegularExpressions.Regex.Replace(filter, @"(status|priority|type)=[^&\s]+&?", "");
                cleanedFilter = cleanedFilter.TrimEnd('&');
                
                // Combine domain-specific filter with remaining generic filter
                if (!string.IsNullOrWhiteSpace(cleanedFilter))
                {
                    return $"({domainFilter}) AND ({cleanedFilter})";
                }
                
                return domainFilter;
            }

            return filter;
        }

        public Task<IReadOnlyList<FollowUp>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<FollowUp>> GetByStaffIdAsync(Guid staffId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }

        public Task<IReadOnlyList<FollowUp>> GetByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default)
        {
            throw new NotImplementedException();
        }
    }
}
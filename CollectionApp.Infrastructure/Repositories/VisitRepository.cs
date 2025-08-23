using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using CollectionApp.Infrastructure.Repositories;
using CollectionApp.Application.Common;

namespace CollectionApp.Infrastructure.Repositories
{
    public class VisitRepository : Repository<Visit>, IVisitRepository
    {
        public VisitRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<IReadOnlyList<Visit>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(v => v.Customer)
                .Include(v => v.Staff)
                .Where(v => v.CustomerId == customerId)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Visit>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(v => v.Staff)
                .Include(v => v.Customer)
                .Where(v => v.StaffId == staffId)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Visit>> GetByStaffIdAsync(Guid staffId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<Visit>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(v => v.Staff)
                .Include(v => v.Customer)
                .Where(v => v.StaffId == staffId && 
                           v.VisitDate >= fromDate && 
                           v.VisitDate <= toDate)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Visit>> GetScheduledAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<Visit>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(v => v.Customer)
                .Include(v => v.Staff)
                .Where(v => v.VisitDate >= fromDate && 
                           v.VisitDate <= toDate && 
                           v.Outcome == null)
                .OrderBy(v => v.VisitDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Visit>> GetCompletedAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<Visit>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(v => v.Customer)
                .Include(v => v.Staff)
                .Where(v => v.VisitDate >= fromDate && 
                           v.VisitDate <= toDate && 
                           v.Outcome != null)
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Visit>> GetOverdueAsync(CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.UtcNow.Date;
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(v => v.Customer)
                .Include(v => v.Staff)
                .Where(v => v.VisitDate < currentDate && v.Outcome == null)
                .OrderBy(v => v.VisitDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Visit>> GetByTerritoryAsync(string territory, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(territory))
                return new List<Visit>();

            var normalizedTerritory = territory.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(v => v.Customer)
                .Include(v => v.Staff)
                .Where(v => v.Customer != null && v.Customer.Address != null && (
                    EF.Functions.Like(v.Customer.Address.City ?? string.Empty, $"%{normalizedTerritory}%") ||
                    EF.Functions.Like(v.Customer.Address.State ?? string.Empty, $"%{normalizedTerritory}%")))
                .OrderByDescending(v => v.VisitDate)
                .ToListAsync(cancellationToken);
        }

        public new async Task<IPagedResult<Visit>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Build domain-specific filter expression
            var domainFilter = BuildDomainSpecificFilter(filter);
            
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                domainFilter, 
                orderBy, 
                pageNumber, 
                pageSize,
                q => q.Include(v => v.Customer).Include(v => v.Staff),
                q => q.OrderByDescending(v => v.VisitDate).ThenBy(v => v.Id),
                cancellationToken);
        }

        /// <summary>
        /// Builds domain-specific filter expressions for Visit entity
        /// </summary>
        private string? BuildDomainSpecificFilter(string? filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return filter;

            var filters = new List<string>();

            // Parse territory parameter for Customer.Address.City/State Like
            if (filter.Contains("territory="))
            {
                var territoryMatch = System.Text.RegularExpressions.Regex.Match(filter, @"territory=([^&\s]+)");
                if (territoryMatch.Success)
                {
                    var territoryTerm = territoryMatch.Groups[1].Value;
                    // Note: This will need to be handled in the QueryExpressionHelper or we'll need to use a different approach
                    // For now, we'll add it as a comment and handle it in the generic filter
                    filters.Add($"Customer.Address.City.Contains(\"{territoryTerm}\") OR Customer.Address.State.Contains(\"{territoryTerm}\")");
                }
            }

            // If we have domain-specific filters, combine them with AND
            if (filters.Any())
            {
                var domainFilter = string.Join(" AND ", filters);
                
                // Remove domain-specific parameters from the original filter
                var cleanedFilter = System.Text.RegularExpressions.Regex.Replace(filter, @"(territory)=[^&\s]+&?", "");
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
    }
} 
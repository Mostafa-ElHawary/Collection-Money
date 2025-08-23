using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using CollectionApp.Infrastructure.Repositories;
using CollectionApp.Application.Common;
using System.Linq.Expressions;

namespace CollectionApp.Infrastructure.Repositories
{
    public class CustomerRepository : Repository<Customer>, ICustomerRepository
    {
        public CustomerRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Customer?> GetByNationalIdAsync(string nationalId, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(nationalId))
                return null;

            var normalizedNationalId = nationalId.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => EF.Functions.Like(c.NationalId, normalizedNationalId), cancellationToken);
        }

        public async Task<Customer?> GetByEmailAsync(string email, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(email))
                return null;

            var normalizedEmail = email.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Contracts)
                .FirstOrDefaultAsync(c => EF.Functions.Like(c.Email, normalizedEmail), cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return new List<Customer>();

            var normalizedSearchTerm = searchTerm.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Contracts)
                .Where(c => EF.Functions.Like(c.FirstName, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(c.LastName, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(c.NationalId, $"%{normalizedSearchTerm}%") ||
                           EF.Functions.Like(c.Email, $"%{normalizedSearchTerm}%"))
                .OrderBy(c => c.FirstName)
                .ThenBy(c => c.LastName)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Customer>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default)
        {
            if (ids is null) return new List<Customer>();
            var idList = ids.Distinct().ToList();
            if (idList.Count == 0) return new List<Customer>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Contracts)
                .Where(c => idList.Contains(c.Id))
                .ToListAsync(cancellationToken);
        }

        public new async Task<IPagedResult<Customer>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Build domain-specific filter expression
            var domainFilter = BuildDomainSpecificFilter(filter);
            
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                domainFilter, 
                orderBy, 
                pageNumber, 
                pageSize,
                q => q.Include(c => c.Contracts),
                q => q.OrderBy(c => c.FirstName).ThenBy(c => c.LastName).ThenBy(c => c.Id),
                cancellationToken);
        }

       
        /// Builds domain-specific filter expressions for Customer entity
    
        private string? BuildDomainSpecificFilter(string? filter)
        {
            if (string.IsNullOrWhiteSpace(filter))
                return filter;

            var filters = new List<string>();

            // Parse search parameter for FirstName/LastName/NationalId/Email Like
            if (filter.Contains("search="))
            {
                var searchMatch = System.Text.RegularExpressions.Regex.Match(filter, @"search=([^&\s]+)");
                if (searchMatch.Success)
                {
                    var searchTerm = searchMatch.Groups[1].Value;
                    filters.Add($"(FirstName.Contains(\"{searchTerm}\") OR LastName.Contains(\"{searchTerm}\") OR NationalId.Contains(\"{searchTerm}\") OR Email.Contains(\"{searchTerm}\"))");
                }
            }

            // Parse email parameter for exact or Like match
            if (filter.Contains("email="))
            {
                var emailMatch = System.Text.RegularExpressions.Regex.Match(filter, @"email=([^&\s]+)");
                if (emailMatch.Success)
                {
                    var emailTerm = emailMatch.Groups[1].Value;
                    filters.Add($"Email.Contains(\"{emailTerm}\")");
                }
            }

            // Parse nationalId parameter for exact or Like match
            if (filter.Contains("nationalId="))
            {
                var nationalIdMatch = System.Text.RegularExpressions.Regex.Match(filter, @"nationalId=([^&\s]+)");
                if (nationalIdMatch.Success)
                {
                    var nationalIdTerm = nationalIdMatch.Groups[1].Value;
                    filters.Add($"NationalId.Contains(\"{nationalIdTerm}\")");
                }
            }

            // If we have domain-specific filters, combine them with AND
            if (filters.Any())
            {
                var domainFilter = string.Join(" AND ", filters);
                
                // Remove domain-specific parameters from the original filter
                var cleanedFilter = System.Text.RegularExpressions.Regex.Replace(filter, @"(search|email|nationalId)=[^&\s]+&?", "");
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
using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using CollectionApp.Infrastructure.Repositories;
using CollectionApp.Application.Common;

namespace CollectionApp.Infrastructure.Repositories
{
    public class LedgerEntryRepository : Repository<LedgerEntry>, ILedgerEntryRepository
    {
        public LedgerEntryRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<LedgerEntry?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(l => l.Contract)
                .ThenInclude(c => c.Staff)
                .Include(l => l.Customer)
                .Include(l => l.Staff)
                .FirstOrDefaultAsync(l => l.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<LedgerEntry>> GetByContractIdAsync(Guid? contractId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(l => l.Contract)
                .ThenInclude(c => c.Staff)
                .Include(l => l.Customer)
                .Include(l => l.Staff)
                .Where(l => l.ContractId == contractId)
                .OrderByDescending(l => l.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<LedgerEntry>> GetByCustomerIdAsync(Guid? customerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(l => l.Contract)
                .ThenInclude(c => c.Staff)
                .Include(l => l.Customer)
                .Include(l => l.Staff)
                .Where(l => l.CustomerId == customerId)
                .OrderByDescending(l => l.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<LedgerEntry>> GetByReferenceAsync(Guid? customerId, Guid? contractId, CancellationToken cancellationToken = default)
        {
            var query = DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(l => l.Customer)
                .Include(l => l.Contract)
                .Include(l => l.Staff)
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(l => l.CustomerId == customerId.Value);

            if (contractId.HasValue)
                query = query.Where(l => l.ContractId == contractId.Value);

            return await query
                .OrderByDescending(l => l.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<LedgerEntry>> GetByReferenceAsync(Guid? customerId, Guid? contractId, string? referenceType, Guid referenceId, CancellationToken cancellationToken = default)
        {
            var query = DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(l => l.Customer)
                .Include(l => l.Contract)
                .Include(l => l.Staff)
                .AsQueryable();

            if (customerId.HasValue)
                query = query.Where(l => l.CustomerId == customerId.Value);

            if (contractId.HasValue)
                query = query.Where(l => l.ContractId == contractId.Value);

            if (!string.IsNullOrWhiteSpace(referenceType))
                query = query.Where(l => EF.Functions.Like(l.ReferenceType, referenceType));

            query = query.Where(l => l.ReferenceId == referenceId);

            return await query
                .OrderByDescending(l => l.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<LedgerEntry>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<LedgerEntry>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(l => l.Customer)
                .Include(l => l.Contract)
                .Include(l => l.Staff)
                .Where(l => l.TransactionDate >= fromDate && l.TransactionDate <= toDate)
                .OrderByDescending(l => l.TransactionDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedResult<LedgerEntry>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                filter, 
                orderBy, 
                pageNumber, 
                pageSize,
                q => q.Include(l => l.Customer).Include(l => l.Contract).ThenInclude(c => c.Staff).Include(l => l.Staff),
                q => q.OrderByDescending(l => l.TransactionDate).ThenBy(l => l.Id),
                cancellationToken);
        }
    }
} 
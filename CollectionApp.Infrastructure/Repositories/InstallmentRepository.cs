using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Domain.Enums;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using CollectionApp.Infrastructure.Repositories;
using CollectionApp.Application.Common;

namespace CollectionApp.Infrastructure.Repositories
{
    public class InstallmentRepository : Repository<Installment>, IInstallmentRepository
    {
        public InstallmentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Installment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(i => i.Contract)
                .ThenInclude(c => c.Staff)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<Installment?> GetByIdWithContractAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(i => i.Contract)
                .ThenInclude(c => c.Staff)
                .Include(i => i.Payments)
                .FirstOrDefaultAsync(i => i.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Installment>> GetByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(i => i.Contract)
                .ThenInclude(c => c.Staff)
                .Include(i => i.Payments)
                .Where(i => i.ContractId == contractId)
                .OrderBy(i => i.InstallmentNumber)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Installment>> GetOverdueAsync(CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.UtcNow.Date;

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(i => i.Contract)
                .ThenInclude(c => c.Customer)
                .Where(i => i.DueDate < currentDate &&
                    (i.Status == InstallmentStatus.Pending || i.Status == InstallmentStatus.PartiallyPaid))
                .OrderBy(i => i.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Installment>> GetByDateRangeAsync(DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<Installment>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(i => i.Contract)
                .Where(i => i.DueDate >= fromDate && i.DueDate <= toDate)
                .OrderBy(i => i.DueDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IPagedResult<Installment>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                filter,
                orderBy,
                pageNumber,
                pageSize,
                q => q.Include(i => i.Contract)
                    .ThenInclude(c => c.Customer)
                    .Include(i => i.Contract)
                    .ThenInclude(c => c.Staff)
                    .Include(i => i.Payments),
                q => q.OrderByDescending(i => i.CreatedAt).ThenBy(i => i.Id),
                cancellationToken);
        }

        public async Task<decimal> GetTotalDueAmount()
        {
            return await DbSet
                .Where(i => i.Status != InstallmentStatus.Paid)
                .SumAsync(i => i.Amount.Amount);
        }

        public async Task<IReadOnlyList<Installment>> GetOverdueInstallments()
        {
            return await DbSet
                .Where(i => i.Status == InstallmentStatus.Overdue)
                .OrderBy(i => i.DueDate)
                .ToListAsync();
        }

          public async Task<IReadOnlyList<Installment>> GetByContractIdsAsync(IEnumerable<Guid> contractIds, CancellationToken cancellationToken = default)
    {
        return await DbSet
            .AsNoTrackingWithIdentityResolution()
            .Include(i => i.Contract)
            .ThenInclude(c => c.Staff)
            .Include(i => i.Payments)
            .Where(i => contractIds.Contains(i.ContractId))
            .OrderBy(i => i.ContractId)
            .ThenBy(i => i.InstallmentNumber)
            .ToListAsync(cancellationToken);
    }
    }
}
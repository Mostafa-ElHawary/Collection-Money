using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;
using CollectionApp.Infrastructure.Repositories;
using CollectionApp.Application.Common;

namespace CollectionApp.Infrastructure.Repositories
{
    public class PaymentRepository : Repository<Payment>, IPaymentRepository
    {
        public PaymentRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Payment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Contract)
                .ThenInclude(c => c.Staff)
                .Include(p => p.Installment)
                .Include(p => p.Staff)
                .Include(p => p.Receipt)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<Payment?> GetByIdWithContractAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Contract)
                .ThenInclude(c => c.Staff)
                .Include(p => p.Installment)
                .Include(p => p.Staff)
                .Include(p => p.Receipt)
                .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByContractIdAsync(Guid contractId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Contract)
                .ThenInclude(c => c.Staff)
                .Include(p => p.Installment)
                .Include(p => p.Staff)
                .Include(p => p.Receipt)
                .Where(p => p.ContractId == contractId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Contract)
                .ThenInclude(c => c.Customer)
                .Include(p => p.Installment)
                .Include(p => p.Staff)
                .Where(p => p.Contract.CustomerId == customerId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByInstallmentIdAsync(Guid installmentId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Contract)
                .ThenInclude(c => c.Staff)
                .Include(p => p.Installment)
                .Include(p => p.Staff)
                .Include(p => p.Receipt)
                .Where(p => p.InstallmentId == installmentId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Staff)
                .Include(p => p.Contract)
                .Include(p => p.Installment)
                .Where(p => p.StaffId == staffId)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Payment>> GetByStaffIdAsync(Guid staffId, DateTime fromDate, DateTime toDate, CancellationToken cancellationToken = default)
        {
            // Validate date range
            if (fromDate > toDate)
                return new List<Payment>();

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Staff)
                .Include(p => p.Contract)
                .Include(p => p.Installment)
                .Where(p => p.StaffId == staffId &&
                           p.PaymentDate >= fromDate &&
                           p.PaymentDate <= toDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(cancellationToken);
        }


        public async Task<IPagedResult<Payment>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                filter,
                orderBy,
                pageNumber,
                pageSize,
                q => q.Include(p => p.Contract)
                    .ThenInclude(c => c.Customer)
                    .Include(p => p.Contract)
                    .ThenInclude(c => c.Staff)
                    .Include(p => p.Installment)
                    .Include(p => p.Staff)
                    .Include(p => p.Receipt),
                q => q.OrderByDescending(p => p.PaymentDate).ThenBy(p => p.Id),
                cancellationToken);
        }

        public async Task<IEnumerable<Payment>> GetByContractIdAsync(Guid contractId)
        {
            return await _dbContext.Payments
                .Where(p => p.ContractId == contractId)
                .ToListAsync();
        }

        public async Task<IEnumerable<Payment>> GetByContractIdsAsync(IEnumerable<Guid> contractIds)
        {
            return await _dbContext.Payments
                .Where(p => contractIds.Contains(p.ContractId))
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Payment>> GetPaymentHistoryAsync(
            Guid? contractId = null,
            Guid? customerId = null,
            DateTime? fromDate = null,
            DateTime? toDate = null)
        {
            var query = DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Contract)
                .ThenInclude(c => c.Customer)
                .Include(p => p.Contract)
                .ThenInclude(c => c.Staff)
                .Include(p => p.Installment)
                .Include(p => p.Staff)
                .Include(p => p.Receipt)
                .AsQueryable();

            if (contractId.HasValue)
            {
                query = query.Where(p => p.ContractId == contractId.Value);
            }

            if (customerId.HasValue)
            {
                query = query.Where(p => p.Contract.CustomerId == customerId.Value);
            }

            if (fromDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate >= fromDate.Value);
            }

            if (toDate.HasValue)
            {
                query = query.Where(p => p.PaymentDate <= toDate.Value);
            }

            return await query
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Payment>> GetByDateRangeAsync(
            DateTime startDate,
            DateTime endDate,
            CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(p => p.Contract)
                .ThenInclude(c => c.Staff)
                .Include(p => p.Installment)
                .Include(p => p.Staff)
                .Include(p => p.Receipt)
                .Where(p => p.PaymentDate >= startDate && p.PaymentDate <= endDate)
                .OrderByDescending(p => p.PaymentDate)
                .ToListAsync(cancellationToken);
        }




    }
}
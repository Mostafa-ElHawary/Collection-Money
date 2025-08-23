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
    public class ContractRepository : Repository<Contract>, IContractRepository
    {
        public ContractRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Contract?> GetByContractNumberAsync(string contractNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(contractNumber))
                return null;

            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Include(c => c.Installments)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.ContractNumber == contractNumber, cancellationToken);
        }

        public async Task<IReadOnlyList<Contract>> GetByCustomerIdAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Include(c => c.Installments)
                .Where(c => c.CustomerId == customerId)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<IReadOnlyList<Contract>> GetByStatusAsync(ContractStatus status, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Where(c => c.Status == status)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }
        
        public async Task<PagedResult<Contract>> GetByStatusPagedAsync(ContractStatus status, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            var query = DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Include(c => c.Installments)
                .Include(c => c.Payments)
                .Where(c => c.Status == status);
                
            var totalCount = await query.CountAsync(cancellationToken);
            
            var items = await query
                .OrderByDescending(c => c.CreatedAt)
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
                
            return new PagedResult<Contract>(items, totalCount, pageNumber, pageSize);
        }
        
        public async Task<PagedResult<Contract>> GetByStatusPagedAsync(ContractStatus status, int pageNumber, int pageSize, string filter, string orderBy, CancellationToken cancellationToken = default)
        {
            var query = DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Include(c => c.Installments)
                .Include(c => c.Payments)
                .Where(c => c.Status == status);
                
            // Apply filter if provided
            if (!string.IsNullOrWhiteSpace(filter))
            {
                filter = filter.Trim().ToLower();
                query = query.Where(c => 
                    c.ContractNumber.ToLower().Contains(filter) ||
                    c.Customer.FirstName.ToLower().Contains(filter) ||
                    c.Notes.ToLower().Contains(filter));
            }
            
            var totalCount = await query.CountAsync(cancellationToken);
            
            // Apply ordering
            query = orderBy?.ToLower() switch
            {
                "contractnumber" => query.OrderBy(c => c.ContractNumber),
                "contractnumber_desc" => query.OrderByDescending(c => c.ContractNumber),
                "customername" => query.OrderBy(c => c.Customer.FirstName),
                "customername_desc" => query.OrderByDescending(c => c.Customer.FirstName),
                "startdate" => query.OrderBy(c => c.StartDate),
                "startdate_desc" => query.OrderByDescending(c => c.StartDate),
                "totalamount" => query.OrderBy(c => c.TotalAmount.Amount),
                "totalamount_desc" => query.OrderByDescending(c => c.TotalAmount.Amount),
                _ => query.OrderByDescending(c => c.CreatedAt)
            };
            
            var items = await query
                .Skip((pageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);
                
            return new PagedResult<Contract>(items, totalCount, pageNumber, pageSize);
        }

        public async Task<IReadOnlyList<Contract>> GetOverdueAsync(CancellationToken cancellationToken = default)
        {
            var currentDate = DateTime.UtcNow.Date;
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Installments.Where(i => i.DueDate < currentDate && 
                    (i.Status == InstallmentStatus.Pending || i.Status == InstallmentStatus.PartiallyPaid)))
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Where(c => c.Installments.Any(i => i.DueDate < currentDate && 
                    (i.Status == InstallmentStatus.Pending || i.Status == InstallmentStatus.PartiallyPaid)))
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }

        public async Task<bool> HasActiveContractsAsync(Guid customerId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTracking()
                .AnyAsync(c => c.CustomerId == customerId && c.Status == ContractStatus.Active, cancellationToken);
        }

        public new async Task<IPagedResult<Contract>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            // Use the shared helper method
            return await GetPagedWithIncludesAsync(
                filter, 
                orderBy, 
                pageNumber, 
                pageSize,
                q => q.Include(c => c.Customer).Include(c => c.Staff).Include(c => c.Installments).Include(c => c.Payments),
                q => q.OrderByDescending(c => c.CreatedAt).ThenBy(c => c.Id),
                cancellationToken);
        }

        public new async Task<Contract?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Include(c => c.Installments)
                .Include(c => c.Payments)
                .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
        }
        
        public async Task<IReadOnlyList<Contract>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(c => c.Customer)
                .Include(c => c.Staff)
                .Include(c => c.Installments)
                .OrderByDescending(c => c.CreatedAt)
                .ToListAsync(cancellationToken);
        }
    }
}
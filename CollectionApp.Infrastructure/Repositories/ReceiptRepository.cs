using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System.Threading;

namespace CollectionApp.Infrastructure.Repositories
{
    public class ReceiptRepository : Repository<Receipt>, IReceiptRepository
    {
        public ReceiptRepository(AppDbContext context) : base(context)
        {
        }

        public async Task<Receipt?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(r => r.Payment)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .FirstOrDefaultAsync(r => r.PaymentId == paymentId, cancellationToken);
        }

        public async Task<Receipt?> GetByReceiptNumberAsync(string receiptNumber, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(receiptNumber))
                return null;

            var normalizedReceiptNumber = receiptNumber.Trim();
            
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(r => r.Payment)
                .Include(r => r.Customer)
                .Include(r => r.Staff)
                .FirstOrDefaultAsync(r => EF.Functions.Like(r.ReceiptNumber, normalizedReceiptNumber), cancellationToken);
        }

        public async Task<IReadOnlyList<Receipt>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default)
        {
            return await DbSet
                .AsNoTrackingWithIdentityResolution()
                .Include(r => r.Staff)
                .Include(r => r.Payment)
                .Include(r => r.Customer)
                .Where(r => r.StaffId == staffId)
                .OrderByDescending(r => r.IssueDate)
                .ToListAsync(cancellationToken);
        }
    }
} 
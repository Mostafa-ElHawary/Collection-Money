using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CollectionApp.Domain.Entities;
using System.Threading;

namespace CollectionApp.Application.Interfaces
{
    public interface IReceiptRepository : IRepository<Receipt>
    {
        Task<Receipt?> GetByPaymentIdAsync(Guid paymentId, CancellationToken cancellationToken = default);
        Task<Receipt?> GetByReceiptNumberAsync(string receiptNumber, CancellationToken cancellationToken = default);
        Task<IReadOnlyList<Receipt>> GetByStaffIdAsync(Guid staffId, CancellationToken cancellationToken = default);
    }
} 
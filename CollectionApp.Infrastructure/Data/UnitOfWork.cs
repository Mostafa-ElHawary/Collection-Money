using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Entities;
using CollectionApp.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace CollectionApp.Infrastructure.Data
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _dbContext;
        private readonly ConcurrentDictionary<Type, object> _repositories = new();
        private bool _disposed;

        // Single source of truth for repository factories
        private static readonly Dictionary<Type, Func<AppDbContext, object>> RepoFactories = new()
        {
            { typeof(Customer), context => new CustomerRepository(context) },
            { typeof(Staff), context => new StaffRepository(context) },
            { typeof(Contract), context => new ContractRepository(context) },
            { typeof(Installment), context => new InstallmentRepository(context) },
            { typeof(Payment), context => new PaymentRepository(context) },
            { typeof(Receipt), context => new ReceiptRepository(context) },
            { typeof(Visit), context => new VisitRepository(context) },
            { typeof(FollowUp), context => new FollowUpRepository(context) },
            { typeof(LedgerEntry), context => new LedgerEntryRepository(context) }
        };

        public UnitOfWork(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        // Repository accessors using specific interfaces
        public ICustomerRepository Customers => GetSpecificRepository<Customer, ICustomerRepository>();
        public IStaffRepository Staff => GetSpecificRepository<Staff, IStaffRepository>();
        public IContractRepository Contracts => GetSpecificRepository<Contract, IContractRepository>();
        public IInstallmentRepository Installments => GetSpecificRepository<Installment, IInstallmentRepository>();
        public IPaymentRepository Payments => GetSpecificRepository<Payment, IPaymentRepository>();
        public IReceiptRepository Receipts => GetSpecificRepository<Receipt, IReceiptRepository>();
        public IVisitRepository Visits => GetSpecificRepository<Visit, IVisitRepository>();
        public IFollowUpRepository FollowUps => GetSpecificRepository<FollowUp, IFollowUpRepository>();
        public ILedgerEntryRepository LedgerEntries => GetSpecificRepository<LedgerEntry, ILedgerEntryRepository>();

        public IRepository<TEntity> GetRepository<TEntity>() where TEntity : CollectionApp.Domain.Common.BaseEntity
        {
            EnsureNotDisposed();
            var repository = (IRepository<TEntity>)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => CreateConcreteRepository<TEntity>());
            return repository;
        }

        private IRepository<TEntity> CreateConcreteRepository<TEntity>() where TEntity : CollectionApp.Domain.Common.BaseEntity
        {
            var entityType = typeof(TEntity);
            
            if (RepoFactories.TryGetValue(entityType, out var factory))
            {
                return (IRepository<TEntity>)factory(_dbContext);
            }
            
            return new Repository<TEntity>(_dbContext);
        }

        private TRepository GetSpecificRepository<TEntity, TRepository>() where TEntity : CollectionApp.Domain.Common.BaseEntity
        {
            EnsureNotDisposed();
            var repository = (TRepository)_repositories.GetOrAdd(
                typeof(TEntity),
                _ => CreateConcreteRepository<TEntity>());
            return repository;
        }

        public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();
            return _dbContext.SaveChangesAsync(cancellationToken);
        }

        public Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();
            return _dbContext.SaveChangesAsync(acceptAllChangesOnSuccess, cancellationToken);
        }

        public async Task BeginTransactionAsync(CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();
            if (_dbContext.Database.CurrentTransaction is not null)
            {
                return;
            }

            await _dbContext.Database.BeginTransactionAsync(cancellationToken);
        }

        public async Task CommitTransactionAsync(CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();
            if (_dbContext.Database.CurrentTransaction is null)
            {
                throw new InvalidOperationException("No active transaction to commit.");
            }

            // Use facade commit if available; otherwise commit the current transaction
            var currentTransaction = _dbContext.Database.CurrentTransaction;
            if (currentTransaction is not null)
            {
                await currentTransaction.CommitAsync(cancellationToken);
                await currentTransaction.DisposeAsync();
            }
        }

        public async Task RollbackTransactionAsync(CancellationToken cancellationToken = default)
        {
            EnsureNotDisposed();
            if (_dbContext.Database.CurrentTransaction is null)
            {
                throw new InvalidOperationException("No active transaction to roll back.");
            }

            var currentTransaction = _dbContext.Database.CurrentTransaction;
            if (currentTransaction is not null)
            {
                await currentTransaction.RollbackAsync(cancellationToken);
                await currentTransaction.DisposeAsync();
            }
        }

        public bool HasActiveTransaction
        {
            get
            {
                EnsureNotDisposed();
                return _dbContext.Database.CurrentTransaction is not null;
            }
        }

        public async Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default)
        {
            if (operation is null) throw new ArgumentNullException(nameof(operation));
            EnsureNotDisposed();
            var ownsTransaction = !HasActiveTransaction;
            if (ownsTransaction)
            {
                await BeginTransactionAsync(cancellationToken);
            }
            try
            {
                await operation(cancellationToken);
                if (ownsTransaction)
                {
                    await CommitTransactionAsync(cancellationToken);
                }
            }
            catch
            {
                if (ownsTransaction)
                {
                    try { await RollbackTransactionAsync(cancellationToken); } catch { }
                }
                throw;
            }
        }

        public async Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default)
        {
            if (operation is null) throw new ArgumentNullException(nameof(operation));
            EnsureNotDisposed();
            var ownsTransaction = !HasActiveTransaction;
            if (ownsTransaction)
            {
                await BeginTransactionAsync(cancellationToken);
            }
            try
            {
                var result = await operation(cancellationToken);
                if (ownsTransaction)
                {
                    await CommitTransactionAsync(cancellationToken);
                }
                return result;
            }
            catch
            {
                if (ownsTransaction)
                {
                    try { await RollbackTransactionAsync(cancellationToken); } catch { }
                }
                throw;
            }
        }

        public void Dispose()
        {
            if (_disposed) return;
            _repositories.Clear();
            _dbContext.Dispose();
            _disposed = true;
        }

        public async ValueTask DisposeAsync()
        {
            if (_disposed) return;
            _repositories.Clear();
            await _dbContext.DisposeAsync();
            _disposed = true;
        }

        private void EnsureNotDisposed()
        {
            if (_disposed)
            {
                throw new ObjectDisposedException(nameof(UnitOfWork));
            }
        }
    }
}


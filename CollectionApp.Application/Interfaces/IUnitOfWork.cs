using System;
using System.Threading;
using System.Threading.Tasks;
using CollectionApp.Domain.Common;
using CollectionApp.Domain.Entities;

namespace CollectionApp.Application.Interfaces
{
    public interface IUnitOfWork : IDisposable, IAsyncDisposable
    {
        // Repository accessors
        ICustomerRepository Customers { get; }
        IStaffRepository Staff { get; }
        IContractRepository Contracts { get; }
        IInstallmentRepository Installments { get; }
        IPaymentRepository Payments { get; }
        IReceiptRepository Receipts { get; }
        IVisitRepository Visits { get; }
        IFollowUpRepository FollowUps { get; }
        ILedgerEntryRepository LedgerEntries { get; }


        /// Provides a generic repository for a given entity type. This is a scalable alternative
        /// to adding new explicit repository properties each time a new entity is introduced.

        /// <typeparam name="TEntity">The entity type.</typeparam>
        /// <returns>An <see cref="IRepository{TEntity}"/> instance for the requested entity type.</returns>
        IRepository<TEntity> GetRepository<TEntity>() where TEntity : BaseEntity;


        /// Persists pending changes to the underlying store.
        /// Use this overload for the common case where EF Core should automatically
        /// call AcceptAllChanges after a successful save and no explicit transaction management is required.

        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Persists pending changes to the underlying store.
        /// When <paramref name="acceptAllChangesOnSuccess"/> is <c>true</c>, EF Core will call
        /// AcceptAllChanges after a successful save. When <c>false</c>, AcceptAllChanges is not called,
        /// which can be useful when coordinating with an explicit transaction: defer AcceptAllChanges until
        /// after the transaction is committed to avoid accepting changes for a transaction that might roll back.
        /// Consumers managing explicit transactions should typically pass <c>false</c>, call Commit on the transaction,
        /// and then invoke AcceptAllChanges (or another save with <c>true</c>) once the transaction has safely committed.
        /// </summary>
        /// <param name="acceptAllChangesOnSuccess">Whether EF Core should call AcceptAllChanges automatically on success.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>The number of state entries written to the underlying database.</returns>
        Task<int> SaveChangesAsync(bool acceptAllChangesOnSuccess, CancellationToken cancellationToken = default);

        /// <summary>
        /// Begins a new database transaction. If a transaction is already active, implementations may
        /// choose to no-op, throw, or create a nested/ambient transaction depending on the underlying provider.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task BeginTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Commits the current transaction. Implementations should throw if no transaction is active.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task CommitTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Rolls back the current transaction. Implementations should throw if no transaction is active.
        /// </summary>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task RollbackTransactionAsync(CancellationToken cancellationToken = default);

        /// <summary>
        /// Indicates whether a database transaction is currently active.
        /// This can be used by consumers to detect ambient transaction state and avoid misuse.
        /// </summary>
        bool HasActiveTransaction { get; }

        /// <summary>
        /// Executes the provided delegate within a transaction scope, handling begin/commit/rollback internally.
        /// Implementations should begin a transaction, invoke <paramref name="operation"/>, commit on success,
        /// and roll back on exception.
        /// </summary>
        /// <param name="operation">The operation to execute within the transaction.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        Task ExecuteInTransactionAsync(Func<CancellationToken, Task> operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Executes the provided delegate within a transaction scope, handling begin/commit/rollback internally,
        /// and returns a result from the delegate.
        /// </summary>
        /// <typeparam name="TResult">The result type returned by the operation.</typeparam>
        /// <param name="operation">The operation to execute within the transaction.</param>
        /// <param name="cancellationToken">Token to observe while waiting for the task to complete.</param>
        /// <returns>The result produced by <paramref name="operation"/>.</returns>
        // Task<TResult> ExecuteInTransactionAsync<TResult>(Func<CancellationToken, Task<TResult>> operation, CancellationToken cancellationToken = default);
       Task<T> ExecuteInTransactionAsync<T>(Func<CancellationToken, Task<T>> operation, CancellationToken cancellationToken = default);

        /// <summary>
        /// Asynchronously releases any resources held by the unit of work (e.g., DbContext, active transactions).
        /// </summary>
        /// <returns>A task representing the asynchronous dispose operation.</returns>
        new ValueTask DisposeAsync();
    }
}


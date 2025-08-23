using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using CollectionApp.Application.Common;
using CollectionApp.Application.Interfaces;
using CollectionApp.Domain.Common;
using CollectionApp.Infrastructure.Data;
using CollectionApp.Infrastructure.Extensions;
using Microsoft.EntityFrameworkCore;
using CollectionApp.Infrastructure.Repositories;

namespace CollectionApp.Infrastructure.Repositories
{
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        protected readonly AppDbContext _dbContext;
        protected readonly DbSet<TEntity> _dbSet;

        protected DbSet<TEntity> DbSet => _dbSet;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext ?? throw new ArgumentNullException(nameof(dbContext));
            _dbSet = _dbContext.Set<TEntity>();
        }

        public async Task<TEntity?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // EF Core FindAsync uses the primary key and can leverage the change tracker cache
            var result = await _dbSet.FindAsync(new object[] { id }, cancellationToken);
            return result;
        }

        public async Task<TEntity?> GetByIdWithDetailsAsync(Guid id, CancellationToken cancellationToken = default)
        {
            // For now, this is a placeholder implementation
            // In a real implementation, you would use Include() to load related entities
            // This method should be overridden in specific repositories that need to load related data
            return await GetByIdAsync(id, cancellationToken);
        }

        public async Task<IReadOnlyList<TEntity>> GetAllAsync(CancellationToken cancellationToken = default)
        {
            var list = await _dbSet.AsNoTracking().ToListAsync(cancellationToken);
            return list as IReadOnlyList<TEntity> ?? list;
        }

        public async Task AddAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            entity.Touch();
            await _dbSet.AddAsync(entity, cancellationToken);
        }

        public Task UpdateAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            entity.Touch();

            var localEntity = _dbSet.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (localEntity is not null)
            {
                _dbContext.Entry(localEntity).CurrentValues.SetValues(entity);
                _dbContext.Entry(localEntity).State = EntityState.Modified;
            }
            else
            {
                _dbSet.Attach(entity);
                _dbContext.Entry(entity).State = EntityState.Modified;
            }

            return Task.CompletedTask;
        }

        public Task DeleteAsync(TEntity entity, CancellationToken cancellationToken = default)
        {
            if (entity is null) throw new ArgumentNullException(nameof(entity));

            var localEntity = _dbSet.Local.FirstOrDefault(e => e.Id == entity.Id);
            if (localEntity is not null)
            {
                _dbSet.Remove(localEntity);
            }
            else
            {
                _dbContext.Entry(entity).State = EntityState.Deleted;
            }
            return Task.CompletedTask;
        }

        public IQueryable<TEntity> Query()
        {
            return _dbSet;
        }

        public async Task<IReadOnlyList<TEntity>> FindAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return await _dbSet.AsNoTracking().Where(predicate).ToListAsync(cancellationToken);
        }

        public Task<TEntity?> FirstOrDefaultAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate, cancellationToken);
        }

        public Task<bool> AnyAsync(Expression<Func<TEntity, bool>> predicate, CancellationToken cancellationToken = default)
        {
            if (predicate is null) throw new ArgumentNullException(nameof(predicate));

            return _dbSet.AsNoTracking().AnyAsync(predicate, cancellationToken);
        }

        public Task<int> CountAsync(Expression<Func<TEntity, bool>>? predicate = null, CancellationToken cancellationToken = default)
        {
            if (predicate is null)
            {
                return _dbSet.AsNoTracking().CountAsync(cancellationToken);
            }

            return _dbSet.AsNoTracking().CountAsync(predicate, cancellationToken);
        }

        public async Task<IPagedResult<TEntity>> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            var baseQuery = _dbSet.AsNoTracking();

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
            var normalizedPageNumber = totalPages == 0 ? 1 : Math.Min(Math.Max(pageNumber, 1), totalPages);

            var orderedQuery = baseQuery.OrderBy(e => e.Id);

            var items = await orderedQuery
                .Skip((normalizedPageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TEntity>(items, totalCount, normalizedPageNumber, pageSize);
        }

        public async Task<IPagedResult<TEntity>> GetPagedAsync(Expression<Func<TEntity, bool>>? filter, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            IQueryable<TEntity> baseQuery = _dbSet.AsNoTracking();
            if (filter is not null)
            {
                baseQuery = baseQuery.Where(filter);
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
            var normalizedPageNumber = totalPages == 0 ? 1 : Math.Min(Math.Max(pageNumber, 1), totalPages);

            var orderedQuery = baseQuery.OrderBy(e => e.Id);

            var items = await orderedQuery
                .Skip((normalizedPageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TEntity>(items, totalCount, normalizedPageNumber, pageSize);
        }

        public async Task<IPagedResult<TEntity>> GetPagedAsync(
            Expression<Func<TEntity, bool>>? filter,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy,
            int pageNumber,
            int pageSize,
            CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            IQueryable<TEntity> baseQuery = _dbSet.AsNoTracking();

            if (filter is not null)
            {
                baseQuery = baseQuery.Where(filter);
            }

            var totalCount = await baseQuery.CountAsync(cancellationToken);

            var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
            var normalizedPageNumber = totalPages == 0 ? 1 : Math.Min(Math.Max(pageNumber, 1), totalPages);

            IQueryable<TEntity> orderedQuery = orderBy is not null
                ? orderBy(baseQuery)
                : baseQuery.OrderBy(e => e.Id);

            var items = await orderedQuery
                .Skip((normalizedPageNumber - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(cancellationToken);

            return new PagedResult<TEntity>(items, totalCount, normalizedPageNumber, pageSize);
        }

        public async Task<IPagedResult<TEntity>> GetPagedAsync(string? filter, string? orderBy, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
        {
            if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            // Parse the filter string to an expression
            var filterExpression = QueryExpressionHelper.BuildFilterExpression<TEntity>(filter);

            // Build the orderBy function from the orderBy string
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByFunc = null;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                try
                {
                    orderByFunc = q => QueryExpressionHelper.BuildOrderByExpression(q, orderBy);
                }
                catch
                {
                    orderByFunc = null; /* optional: log */
                }
            }

            // Delegate to the existing expression-based overload
            return await GetPagedAsync(filterExpression, orderByFunc, pageNumber, pageSize, cancellationToken);
        }

        /// Shared helper for paged querying with includes and default ordering

        protected async Task<IPagedResult<TEntity>> GetPagedWithIncludesAsync(
            string? filter, string? orderBy, int pageNumber, int pageSize,
            Func<IQueryable<TEntity>, IQueryable<TEntity>> applyIncludes,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> defaultOrder,
            CancellationToken ct)
        {
            if (pageNumber < 1) throw new ArgumentOutOfRangeException(nameof(pageNumber));
            if (pageSize < 1) throw new ArgumentOutOfRangeException(nameof(pageSize));

            // Parse the filter string to an expression
            var filterExpression = QueryExpressionHelper.BuildFilterExpression<TEntity>(filter);

            // Build the orderBy function from the orderBy string with graceful fallback
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderByFunc = null;
            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                try
                {
                    orderByFunc = q => QueryExpressionHelper.BuildOrderByExpression(q, orderBy);
                }
                catch
                {
                    orderByFunc = null; /* optional: log */
                }
            }

            // 1) Build filtered, ordered base (no Include) for count
            var countQuery = DbSet.AsNoTracking();
            if (filterExpression != null)
                countQuery = countQuery.Where(filterExpression);
            var totalCount = await countQuery.CountAsync(ct);
            var totalPages = pageSize > 0 ? (int)Math.Ceiling(totalCount / (double)pageSize) : 0;
            var normalized = totalPages == 0 ? 1 : Math.Min(Math.Max(pageNumber, 1), totalPages);

            // 2) Get page keys without includes
            var keyPage = await (orderByFunc != null ? orderByFunc(countQuery) : defaultOrder(countQuery))
                .Select(e => e.Id)
                .Skip((normalized - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            // 3) Load page with Includes
            var page = await DbSet.AsNoTrackingWithIdentityResolution()
                .AsSplitQuery()
                .Where(e => keyPage.Contains(e.Id))
                .Apply(applyIncludes)
                .ToListAsync(ct);

            return new PagedResult<TEntity>(OrderByKeyPage(page, keyPage), totalCount, normalized, pageSize);
        }

        /// Orders the result page according to the key page order

        private static List<TEntity> OrderByKeyPage(List<TEntity> page, List<Guid> keyPage)
        {
            var orderedPage = new List<TEntity>(page.Count);
            foreach (var id in keyPage)
            {
                var entity = page.FirstOrDefault(e => e.Id == id);
                if (entity != null)
                    orderedPage.Add(entity);
            }
            return orderedPage;
        }
    }
}


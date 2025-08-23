using System;
using System.Linq;
using System.Linq.Expressions;
using CollectionApp.Domain.Common;
using System.Linq.Dynamic.Core;

namespace CollectionApp.Infrastructure.Extensions
{
    public static class QueryableExtensions
    {
        /// <summary>
        /// Applies a filter expression to an IQueryable if the expression is not null
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="query">The query to apply the filter to</param>
        /// <param name="filter">The filter expression</param>
        /// <returns>Filtered query or original query if filter is null</returns>
        public static IQueryable<TEntity> Apply<TEntity>(this IQueryable<TEntity> query, Expression<Func<TEntity, bool>>? filter) 
            where TEntity : BaseEntity
        {
            return filter != null ? query.Where(filter) : query;
        }

        /// <summary>
        /// Applies a filter string to an IQueryable using dynamic LINQ
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="query">The query to apply the filter to</param>
        /// <param name="filter">The filter string</param>
        /// <returns>Filtered query or original query if filter is null or empty</returns>
        public static IQueryable<TEntity> Apply<TEntity>(this IQueryable<TEntity> query, string? filter) 
            where TEntity : BaseEntity
        {
            if (string.IsNullOrWhiteSpace(filter))
                return query;

            return query.Where(filter);
        }
        
        /// <summary>
        /// Applies a transformation function to an IQueryable
        /// </summary>
        /// <typeparam name="TEntity">The entity type</typeparam>
        /// <param name="query">The query to transform</param>
        /// <param name="transform">The transformation function</param>
        /// <returns>Transformed query or original query if transform is null</returns>
        public static IQueryable<TEntity> Apply<TEntity>(this IQueryable<TEntity> query, Func<IQueryable<TEntity>, IQueryable<TEntity>>? transform) 
            where TEntity : BaseEntity
        {
            return transform != null ? transform(query) : query;
        }
    }
}
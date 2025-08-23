using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Linq.Expressions;
using System.Reflection;
using CollectionApp.Domain.Common;

namespace CollectionApp.Infrastructure.Repositories
{
    internal static class QueryExpressionHelper
    {
        // Comment 6: Cache valid property names to reduce reflection overhead on hot paths
        private static readonly ConcurrentDictionary<Type, string[]> _propertyNamesCache = new();

        /// <summary>
        /// Converts a filter string to an Expression<Func<T, bool>> using System.Linq.Dynamic.Core
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="filter">The filter string (e.g., "Name.Contains(\"John\") AND Age > 25")</param>
        /// <returns>Parsed expression or null if filter is empty</returns>
        public static Expression<Func<T, bool>>? BuildFilterExpression<T>(string? filter) where T : BaseEntity
        {
            if (string.IsNullOrWhiteSpace(filter))
                return null;

            try
            {
                // Comment 4: Remove the pre-check IsValidFilterString<T>(filter) - do single ParseLambda
                // Use System.Linq.Dynamic.Core to parse the filter string
                var config = new ParsingConfig { AllowNewToEvaluateAnyType = false };
                var expression = DynamicExpressionParser.ParseLambda(config, typeof(T), typeof(bool), filter);
                
                // Comment 4: Run SafeMemberAccessVisitor<T> on expression.Body
                var visitor = new SafeMemberAccessVisitor<T>();
                visitor.Visit(expression.Body);

                // Comment 5: Change from 'as' cast to explicit cast
                return (Expression<Func<T, bool>>)expression;
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse filter string '{filter}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Applies ordering to an IQueryable based on an orderBy string
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="query">The base query</param>
        /// <param name="orderBy">The orderBy string (e.g., "Name ASC, Age DESC")</param>
        /// <returns>Ordered query or original query if orderBy is empty</returns>
        public static IOrderedQueryable<T> BuildOrderByExpression<T>(IQueryable<T> query, string? orderBy) where T : BaseEntity
        {
            // Comment 2: If orderBy is empty, return query.OrderBy(e => e.Id) instead of invalid cast
            if (string.IsNullOrWhiteSpace(orderBy))
                return query.OrderBy(e => e.Id);

            try
            {
                // Validate the orderBy string contains only valid property names
                if (!IsValidOrderByString<T>(orderBy))
                {
                    throw new ArgumentException($"OrderBy string contains invalid property names: {orderBy}");
                }

                // Use System.Linq.Dynamic.Core to apply ordering
                return query.OrderBy(orderBy);
            }
            catch (Exception ex)
            {
                throw new ArgumentException($"Failed to parse orderBy string '{orderBy}': {ex.Message}", ex);
            }
        }

        /// <summary>
        /// Validates that a property name exists on the entity type
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="propertyName">The property name to validate</param>
        /// <returns>True if the property exists, false otherwise</returns>
        public static bool ValidatePropertyName<T>(string propertyName) where T : BaseEntity
        {
            if (string.IsNullOrWhiteSpace(propertyName))
                return false;

            var validProperties = GetValidPropertyNames<T>();
            return validProperties.Contains(propertyName, StringComparer.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Gets all valid property names for the entity type
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <returns>List of valid property names</returns>
        public static List<string> GetValidPropertyNames<T>() where T : BaseEntity
        {
            // Comment 6: Use cache to reduce reflection overhead
            return _propertyNamesCache.GetOrAdd(typeof(T), type =>
                type.GetProperties(BindingFlags.Public | BindingFlags.Instance)
                    .Where(p => p.CanRead && !p.GetCustomAttributes<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>().Any())
                    .Select(p => p.Name)
                    .ToArray()
            ).ToList();
        }

        // Comment 1: Extract new outer helper that walks segments and ensures each property exists
        private static bool IsValidPropertyPath(Type type, string propertyPath)
        {
            var segments = propertyPath.Split('.');

            foreach (var segment in segments)
            {
                var property = type.GetProperty(segment, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (property == null || property.GetCustomAttributes<System.ComponentModel.DataAnnotations.Schema.NotMappedAttribute>().Any())
                {
                    return false;
                }
                type = property.PropertyType; // Update type for nested properties
            }
            return true;
        }

        private class SafeMemberAccessVisitor<T> : ExpressionVisitor where T : BaseEntity
        {
            private readonly List<string> _allowedStringMethods = new List<string> { "Contains", "StartsWith", "EndsWith", "Equals" };

            protected override Expression VisitMember(MemberExpression node)
            {
                if (node.Expression != null && node.Expression.NodeType == ExpressionType.Parameter)
                {
                    // This is a direct property access on the root entity
                    // Comment 1: Use outer helper IsValidPropertyPath(typeof(T), node.Member.Name)
                    if (!IsValidPropertyPath(typeof(T), node.Member.Name))
                    {
                        throw new ArgumentException($"Invalid property access: {node.Member.Name}");
                    }
                }
                else if (node.Expression != null && node.Expression.NodeType == ExpressionType.MemberAccess)
                {
                    // This is a nested property access. We need to validate the full path.
                    var path = GetMemberPath(node);
                    // Comment 1: Use outer helper IsValidPropertyPath(typeof(T), path)
                    if (!IsValidPropertyPath(typeof(T), path))
                    {
                        throw new ArgumentException($"Invalid nested property access: {path}");
                    }
                }
                else
                {
                    // Disallow other types of member access (e.g., static fields, methods on literals)
                    throw new ArgumentException($"Unsupported member access expression: {node}");
                }
                return base.VisitMember(node);
            }

            protected override Expression VisitMethodCall(MethodCallExpression node)
            {
                // Only allow specific methods on string properties
                if (node.Object != null && node.Object.Type == typeof(string) && _allowedStringMethods.Contains(node.Method.Name))
                {
                    // Ensure the method is called on a valid property or nested property
                    if (node.Object is MemberExpression memberExpression)
                    {
                        var path = GetMemberPath(memberExpression);
                        // Comment 1: Use outer helper IsValidPropertyPath(typeof(T), path)
                        if (!IsValidPropertyPath(typeof(T), path))
                        {
                            throw new ArgumentException($"Invalid method call on unsupported property: {path}.{node.Method.Name}");
                        }
                    }
                    else if (node.Object.NodeType == ExpressionType.Parameter)
                    {
                         // Direct method call on a parameter, not allowed
                         throw new ArgumentException($"Method call directly on parameter is not allowed: {node}");
                    }
                }
                else
                {
                    throw new ArgumentException($"Unsupported method call: {node.Method.Name}");
                }
                return base.VisitMethodCall(node);
            }

            private string GetMemberPath(Expression expression)
            {
                var parts = new Stack<string>();
                var current = expression;

                while (current is MemberExpression memberExpression)
                {
                    parts.Push(memberExpression.Member.Name);
                    current = memberExpression.Expression;
                }

                if (current.NodeType != ExpressionType.Parameter)
                {
                    throw new ArgumentException($"Unsupported expression type in member path: {current.NodeType}");
                }

                return string.Join(".", parts);
            }

            // Comment 1: Remove or rename the inner IsValidPropertyPath<TEntityType> to avoid confusion
        }

        // Comment 4: Delete IsValidFilterString<T> since it's not used elsewhere after refactoring

        /// <summary>
        /// Validates that an orderBy string only contains valid property names
        /// </summary>
        /// <typeparam name="T">The entity type</typeparam>
        /// <param name="orderBy">The orderBy string to validate</param>
        /// <returns>True if the orderBy string is valid, false otherwise</returns>
        private static bool IsValidOrderByString<T>(string orderBy) where T : BaseEntity
        {
            var validProperties = GetValidPropertyNames<T>();
            
            // Split by comma and validate each ordering clause
            var clauses = orderBy.Split(',', StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var clause in clauses)
            {
                var trimmedClause = clause.Trim();
                var parts = trimmedClause.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                
                if (parts.Length == 0)
                    continue;
                
                var propertyName = parts[0];
                
                // Comment 1: Use outer helper IsValidPropertyPath(typeof(T), propertyName)
                if (!IsValidPropertyPath(typeof(T), propertyName))
                {
                    return false;
                }
                
                // Comment 3: Check if the direction is valid (optional) - accept ASC, DESC, ASCENDING, DESCENDING
                if (parts.Length > 1)
                {
                    var dir = parts[1].Trim();
                    var allowed = new[] {"ASC", "DESC", "ASCENDING", "DESCENDING"};
                    if (!allowed.Contains(dir, StringComparer.OrdinalIgnoreCase))
                    {
                        return false;
                    }
                }
            }
            
            return true;
        }
    }
} 
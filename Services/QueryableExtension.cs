
using StellarPageable.Models;
using System.Linq.Expressions;
namespace StellarPageable.Services
{
    namespace StellarPageable
    {
        public static class QueryableExtensions
        {
            public static async Task<PaginatedResponse<T>> GetPaginatedAsync<T>(
                this IQueryable<T> query,
                PaginatedRequest request
            ) where T : class
            {
                // Apply filtering
                if (!string.IsNullOrEmpty(request.Filter))
                {
                    query = ApplyFilters(query, request.Filter);
                }

                // Apply ordering
                if (!string.IsNullOrEmpty(request.OrderBy))
                {
                    query = ApplyOrderBy(query, request.OrderBy);
                }

                // Total record count
                var totalRecords = query.Count();

                // Apply pagination
                var data = query
                    .Skip((request.PageNumber - 1) * request.PageSize)
                    .Take(request.PageSize)
                    .ToList();

                return new PaginatedResponse<T>
                {
                    Data = data,
                    TotalRecords = totalRecords,
                    TotalPages = (int)Math.Ceiling(totalRecords / (double)request.PageSize),
                    PageNumber = request.PageNumber,
                    PageSize = request.PageSize
                };
            }

            private static IQueryable<T> ApplyFilters<T>(IQueryable<T> query, string filter) where T : class
            {
                var filters = filter.Split(';', StringSplitOptions.RemoveEmptyEntries);
                foreach (var f in filters)
                {
                    var parts = f.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                    if (parts.Length >= 3)
                    {
                        var property = parts[0].Trim();
                        var operation = parts[1].Trim();
                        var value = string.Join(" ", parts.Skip(2)).Trim('\'', ' ');

                        query = ApplyFilterOperation(query, property, operation, value);
                    }
                }
                return query;
            }

            private static IQueryable<T> ApplyFilterOperation<T>(IQueryable<T> query, string property, string operation, string value) where T : class
            {
                var parameter = Expression.Parameter(typeof(T), "e");
                var propertyAccess = Expression.Property(parameter, property);
                var constant = Expression.Constant(Convert.ChangeType(value, propertyAccess.Type));

                Expression comparison = operation.ToLower() switch
                {
                    "eq" => Expression.Equal(propertyAccess, constant),
                    "ne" => Expression.NotEqual(propertyAccess, constant),
                    "gt" => Expression.GreaterThan(propertyAccess, constant),
                    "ge" => Expression.GreaterThanOrEqual(propertyAccess, constant),
                    "lt" => Expression.LessThan(propertyAccess, constant),
                    "le" => Expression.LessThanOrEqual(propertyAccess, constant),
                    _ => throw new ArgumentException($"Unsupported filter operation '{operation}'")
                };

                var lambda = Expression.Lambda<Func<T, bool>>(comparison, parameter);
                return query.Where(lambda);
            }

            private static IQueryable<T> ApplyOrderBy<T>(IQueryable<T> query, string orderBy) where T : class
            {
                var orderParts = orderBy.Split(' ', StringSplitOptions.RemoveEmptyEntries);
                var property = orderParts[0];
                var direction = orderParts.Length > 1 && orderParts[1].ToLower() == "desc"
                    ? "OrderByDescending"
                    : "OrderBy";

                var entityType = typeof(T);
                var propertyInfo = entityType.GetProperty(property);
                if (propertyInfo == null)
                {
                    throw new ArgumentException($"Property '{property}' not found on type '{entityType.Name}'");
                }

                var parameter = Expression.Parameter(entityType, "e");
                var propertyAccess = Expression.Property(parameter, property);
                var orderByLambda = Expression.Lambda(propertyAccess, parameter);

                var method = typeof(Queryable).GetMethods()
                    .First(m => m.Name == direction && m.GetParameters().Length == 2)
                    .MakeGenericMethod(entityType, propertyInfo.PropertyType);

                return (IQueryable<T>)method.Invoke(null, new object[] { query, orderByLambda });
            }
        }
    }

}


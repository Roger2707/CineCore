using System.Collections;
using System.Linq.Expressions;
using System.Reflection;

namespace Shared.Extensions.DynamicQueries
{
    public static class QueryableExtensions
    {

        #region Search
        public static IQueryable<T> DynamicSearch<T>(this IQueryable<T> query, string searchTerm, params string[] searchFields)
        {
            if (string.IsNullOrWhiteSpace(searchTerm))
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression searchExpression = null;

            // If no specific fields provided, search all string properties
            if (!searchFields.Any())
            {
                searchFields = typeof(T).GetProperties()
                    .Where(p => p.PropertyType == typeof(string))
                    .Select(p => p.Name)
                    .ToArray();
            }

            foreach (var field in searchFields)
            {
                var property = GetNestedProperty(typeof(T), field);
                if (property?.PropertyType == typeof(string))
                {
                    var propertyAccess = GetNestedPropertyExpression(parameter, field);
                    var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));

                    var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                    var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                    var propertyLower = Expression.Call(propertyAccess, toLowerMethod);
                    var searchTermLower = Expression.Constant(searchTerm.ToLower());
                    var containsCall = Expression.Call(propertyLower, containsMethod, searchTermLower);

                    var fieldExpression = Expression.AndAlso(nullCheck, containsCall);

                    searchExpression = searchExpression == null
                        ? fieldExpression
                        : Expression.OrElse(searchExpression, fieldExpression);
                }
            }

            if (searchExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(searchExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        #endregion

        #region Sorts

        public static IQueryable<T> DynamicSort<T>(this IQueryable<T> query, List<SortField> sortFields)
        {
            if (sortFields == null || !sortFields.Any())
                return query;

            IOrderedQueryable<T> orderedQuery = null;

            foreach (var sortField in sortFields)
            {
                var property = GetNestedProperty(typeof(T), sortField.Field);
                if (property == null) continue;

                var parameter = Expression.Parameter(typeof(T), "x");
                var propertyAccess = GetNestedPropertyExpression(parameter, sortField.Field);
                var lambda = Expression.Lambda(propertyAccess, parameter);

                string methodName;
                if (orderedQuery == null)
                {
                    methodName = sortField.Ascending ? "OrderBy" : "OrderByDescending";
                    orderedQuery = (IOrderedQueryable<T>)typeof(Queryable)
                        .GetMethods()
                        .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(T), property.PropertyType)
                        .Invoke(null, new object[] { query, lambda });
                }
                else
                {
                    methodName = sortField.Ascending ? "ThenBy" : "ThenByDescending";
                    orderedQuery = (IOrderedQueryable<T>)typeof(Queryable)
                        .GetMethods()
                        .Where(m => m.Name == methodName && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(typeof(T), property.PropertyType)
                        .Invoke(null, new object[] { orderedQuery, lambda });
                }
            }

            return orderedQuery ?? query;
        }

        #endregion

        #region Filters
        public class FilterConfiguration
        {
            private readonly Dictionary<string, FilterRule> _rules = new();

            public FilterConfiguration AddRule(string propertyName, ComparisonOperator operatorType)
            {
                _rules[propertyName.ToLower()] = new FilterRule
                {
                    PropertyName = propertyName,
                    Operator = operatorType
                };
                return this;
            }

            public FilterRule GetRule(string propertyName)
            {
                return _rules.TryGetValue(propertyName.ToLower(), out var rule) ? rule : null;
            }
        }

        public static IQueryable<T> DynamicFilterWithConfig<T>(this IQueryable<T> query, Dictionary<string, object> filters, FilterConfiguration config = null)
        {
            if (filters == null || !filters.Any())
                return query;

            var parameter = Expression.Parameter(typeof(T), "x");
            Expression combinedExpression = null;

            foreach (var filter in filters)
            {
                if (filter.Value == null) continue;

                var property = GetNestedProperty(typeof(T), filter.Key);
                if (property == null) continue;

                var propertyAccess = GetNestedPropertyExpression(parameter, filter.Key);

                // Get operator from configuration or use default
                var operatorType = config?.GetRule(filter.Key)?.Operator ?? ComparisonOperator.Equal;

                var filterExpression = CreateFilterExpressionWithOperator(
                    propertyAccess,
                    filter.Value,
                    property.PropertyType,
                    operatorType);

                if (filterExpression != null)
                {
                    combinedExpression = combinedExpression == null
                        ? filterExpression
                        : Expression.AndAlso(combinedExpression, filterExpression);
                }
            }

            if (combinedExpression != null)
            {
                var lambda = Expression.Lambda<Func<T, bool>>(combinedExpression, parameter);
                query = query.Where(lambda);
            }

            return query;
        }

        private static Expression CreateFilterExpressionWithOperator(
            Expression propertyAccess,
            object value,
            Type propertyType,
            ComparisonOperator operatorType)
        {
            var valueConstant = Expression.Constant(value);

            // Handle nullable types
            if (propertyType.IsGenericType && propertyType.GetGenericTypeDefinition() == typeof(Nullable<>))
            {
                var hasValueProperty = Expression.Property(propertyAccess, "HasValue");
                var valueProperty = Expression.Property(propertyAccess, "Value");
                var hasValue = Expression.Equal(hasValueProperty, Expression.Constant(true));
                var underlyingType = Nullable.GetUnderlyingType(propertyType);

                var comparison = CreateComparisonExpression(valueProperty, value, underlyingType, operatorType);
                return comparison != null ? Expression.AndAlso(hasValue, comparison) : null;
            }

            return CreateComparisonExpression(propertyAccess, value, propertyType, operatorType);
        }

        private static Expression CreateComparisonExpression(
            Expression propertyAccess,
            object value,
            Type propertyType,
            ComparisonOperator operatorType)
        {
            var valueConstant = Expression.Constant(value);

            switch (operatorType)
            {
                case ComparisonOperator.Equal:
                    return Expression.Equal(propertyAccess, Expression.Convert(valueConstant, propertyType));

                case ComparisonOperator.NotEqual:
                    return Expression.NotEqual(propertyAccess, Expression.Convert(valueConstant, propertyType));

                case ComparisonOperator.GreaterThan:
                    return Expression.GreaterThan(propertyAccess, Expression.Convert(valueConstant, propertyType));

                case ComparisonOperator.GreaterThanOrEqual:
                    return Expression.GreaterThanOrEqual(propertyAccess, Expression.Convert(valueConstant, propertyType));

                case ComparisonOperator.LessThan:
                    return Expression.LessThan(propertyAccess, Expression.Convert(valueConstant, propertyType));

                case ComparisonOperator.LessThanOrEqual:
                    return Expression.LessThanOrEqual(propertyAccess, Expression.Convert(valueConstant, propertyType));

                case ComparisonOperator.Contains:
                    if (propertyType == typeof(string) && value is string)
                    {
                        var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                        var containsMethod = typeof(string).GetMethod("Contains", new[] { typeof(string) });
                        var toLowerMethod = typeof(string).GetMethod("ToLower", Type.EmptyTypes);

                        var propertyLower = Expression.Call(propertyAccess, toLowerMethod);
                        var valueLower = Expression.Constant(value.ToString().ToLower());
                        var containsCall = Expression.Call(propertyLower, containsMethod, valueLower);

                        return Expression.AndAlso(nullCheck, containsCall);
                    }
                    break;

                case ComparisonOperator.StartsWith:
                    if (propertyType == typeof(string) && value is string)
                    {
                        var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                        var startsWithMethod = typeof(string).GetMethod("StartsWith", new[] { typeof(string) });
                        var startsWithCall = Expression.Call(propertyAccess, startsWithMethod, valueConstant);
                        return Expression.AndAlso(nullCheck, startsWithCall);
                    }
                    break;

                case ComparisonOperator.EndsWith:
                    if (propertyType == typeof(string) && value is string)
                    {
                        var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                        var endsWithMethod = typeof(string).GetMethod("EndsWith", new[] { typeof(string) });
                        var endsWithCall = Expression.Call(propertyAccess, endsWithMethod, valueConstant);
                        return Expression.AndAlso(nullCheck, endsWithCall);
                    }
                    break;

                case ComparisonOperator.In:
                    return CreateInExpression(propertyAccess, value, propertyType);

                case ComparisonOperator.NotIn:
                    var inExpression = CreateInExpression(propertyAccess, value, propertyType);
                    return inExpression != null ? Expression.Not(inExpression) : null;

                case ComparisonOperator.ContainsAny:
                    return CreateContainsAnyExpression(propertyAccess, value, propertyType);

                case ComparisonOperator.ContainsAll:
                    return CreateContainsAllExpression(propertyAccess, value, propertyType);

                case ComparisonOperator.ArrayContains:
                    return CreateArrayContainsExpression(propertyAccess, value, propertyType);
            }

            return null;
        }

        #endregion

        #region Helpers

        private static PropertyInfo GetNestedProperty(Type type, string propertyPath)
        {
            var properties = propertyPath.Split('.');
            PropertyInfo property = null;
            var currentType = type;

            foreach (var prop in properties)
            {
                property = currentType.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null) return null;
                currentType = property.PropertyType;
            }

            return property;
        }

        private static Expression GetNestedPropertyExpression(Expression parameter, string propertyPath)
        {
            var properties = propertyPath.Split('.');
            Expression expression = parameter;

            foreach (var prop in properties)
            {
                var property = expression.Type.GetProperty(prop, BindingFlags.IgnoreCase | BindingFlags.Public | BindingFlags.Instance);
                if (property == null) throw new ArgumentException($"Property '{prop}' not found");
                expression = Expression.Property(expression, property);
            }

            return expression;
        }

        private static Expression CreateInExpression(Expression propertyAccess, object value, Type propertyType)
        {
            if (value is IEnumerable enumerable && !(value is string))
            {
                var valueList = enumerable.Cast<object>().ToList();
                if (!valueList.Any()) return Expression.Constant(false);

                // Create: valueList.Contains(property)
                var listType = typeof(List<>).MakeGenericType(propertyType);
                var convertedList = Activator.CreateInstance(listType);
                var addMethod = listType.GetMethod("Add");

                foreach (var item in valueList)
                {
                    var convertedItem = Convert.ChangeType(item, propertyType);
                    addMethod.Invoke(convertedList, new[] { convertedItem });
                }

                var containsMethod = listType.GetMethod("Contains");
                var listConstant = Expression.Constant(convertedList);

                return Expression.Call(listConstant, containsMethod, propertyAccess);
            }
            return null;
        }

        private static Expression CreateContainsAnyExpression(Expression propertyAccess, object value, Type propertyType)
        {
            if (value is IEnumerable enumerable && !(value is string))
            {
                var valueList = enumerable.Cast<object>().ToList();
                if (!valueList.Any()) return Expression.Constant(false);

                // For collections like List<Genre>, check if property collection intersects with value list
                if (IsCollectionType(propertyType))
                {
                    var elementType = GetCollectionElementType(propertyType);
                    if (elementType != null)
                    {
                        // Create: property.Any(p => valueList.Contains(p))
                        var listType = typeof(List<>).MakeGenericType(elementType);
                        var convertedList = Activator.CreateInstance(listType);
                        var addMethod = listType.GetMethod("Add");

                        foreach (var item in valueList)
                        {
                            var convertedItem = Convert.ChangeType(item, elementType);
                            addMethod.Invoke(convertedList, new[] { convertedItem });
                        }

                        var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                        var anyMethod = typeof(Enumerable).GetMethods()
                            .Where(m => m.Name == "Any" && m.GetParameters().Length == 2)
                            .Single()
                            .MakeGenericMethod(elementType);

                        var parameter = Expression.Parameter(elementType, "p");
                        var containsMethod = listType.GetMethod("Contains");
                        var listConstant = Expression.Constant(convertedList);
                        var containsCall = Expression.Call(listConstant, containsMethod, parameter);
                        var lambda = Expression.Lambda(containsCall, parameter);

                        var anyCall = Expression.Call(anyMethod, propertyAccess, lambda);
                        return Expression.AndAlso(nullCheck, anyCall);
                    }
                }
            }
            return null;
        }

        private static Expression CreateContainsAllExpression(Expression propertyAccess, object value, Type propertyType)
        {
            if (value is IEnumerable enumerable && !(value is string))
            {
                var valueList = enumerable.Cast<object>().ToList();
                if (!valueList.Any()) return Expression.Constant(true);

                if (IsCollectionType(propertyType))
                {
                    var elementType = GetCollectionElementType(propertyType);
                    if (elementType != null)
                    {
                        // Create: valueList.All(v => property.Contains(v))
                        var listType = typeof(List<>).MakeGenericType(elementType);
                        var convertedList = Activator.CreateInstance(listType);
                        var addMethod = listType.GetMethod("Add");

                        foreach (var item in valueList)
                        {
                            var convertedItem = Convert.ChangeType(item, elementType);
                            addMethod.Invoke(convertedList, new[] { convertedItem });
                        }

                        var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                        var allMethod = typeof(Enumerable).GetMethods()
                            .Where(m => m.Name == "All" && m.GetParameters().Length == 2)
                            .Single()
                            .MakeGenericMethod(elementType);

                        var containsMethod = typeof(Enumerable).GetMethods()
                            .Where(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                            .Single()
                            .MakeGenericMethod(elementType);

                        var parameter = Expression.Parameter(elementType, "v");
                        var containsCall = Expression.Call(containsMethod, propertyAccess, parameter);
                        var lambda = Expression.Lambda(containsCall, parameter);

                        var listConstant = Expression.Constant(convertedList);
                        var allCall = Expression.Call(allMethod, listConstant, lambda);
                        return Expression.AndAlso(nullCheck, allCall);
                    }
                }
            }
            return null;
        }

        private static Expression CreateArrayContainsExpression(Expression propertyAccess, object value, Type propertyType)
        {
            // For single value check against array/list property
            if (IsCollectionType(propertyType))
            {
                var elementType = GetCollectionElementType(propertyType);
                if (elementType != null)
                {
                    var nullCheck = Expression.NotEqual(propertyAccess, Expression.Constant(null));
                    var containsMethod = typeof(Enumerable).GetMethods()
                        .Where(m => m.Name == "Contains" && m.GetParameters().Length == 2)
                        .Single()
                        .MakeGenericMethod(elementType);

                    var convertedValue = Convert.ChangeType(value, elementType);
                    var valueConstant = Expression.Constant(convertedValue);
                    var containsCall = Expression.Call(containsMethod, propertyAccess, valueConstant);

                    return Expression.AndAlso(nullCheck, containsCall);
                }
            }
            return null;
        }

        private static bool IsCollectionType(Type type)
        {
            return type != typeof(string) &&
                   (type.IsArray ||
                    (type.IsGenericType &&
                     (type.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                      type.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                      type.GetGenericTypeDefinition() == typeof(IList<>) ||
                      type.GetGenericTypeDefinition() == typeof(List<>) ||
                      type.GetInterfaces().Any(i => i.IsGenericType &&
                                                  (i.GetGenericTypeDefinition() == typeof(IEnumerable<>) ||
                                                   i.GetGenericTypeDefinition() == typeof(ICollection<>) ||
                                                   i.GetGenericTypeDefinition() == typeof(IList<>))))));
        }

        private static Type GetCollectionElementType(Type type)
        {
            if (type.IsArray)
                return type.GetElementType();

            if (type.IsGenericType)
            {
                var genericArgs = type.GetGenericArguments();
                if (genericArgs.Length == 1)
                    return genericArgs[0];
            }

            var enumerableInterface = type.GetInterfaces()
                .FirstOrDefault(i => i.IsGenericType && i.GetGenericTypeDefinition() == typeof(IEnumerable<>));

            return enumerableInterface?.GetGenericArguments()[0];
        }

        #endregion

        #region Support Classes
        public class SortField
        {
            public string Field { get; set; }
            public bool Ascending { get; set; } = true;
        }

        public class DynamicQueryRequest
        {
            public Dictionary<string, object> Filters { get; set; } = new();
            public string SearchTerm { get; set; }
            public List<string> SearchFields { get; set; } = new();
            public List<SortField> SortFields { get; set; } = new();
            public int Skip { get; set; } = 0;
            public int Take { get; set; } = 0;
        }

        public enum ComparisonOperator
        {
            Equal,
            NotEqual,
            GreaterThan,
            GreaterThanOrEqual,
            LessThan,
            LessThanOrEqual,
            Contains,
            StartsWith,
            EndsWith,
            In,                    // Property value is IN the provided list
            NotIn,                 // Property value is NOT IN the provided list
            ContainsAny,          // Property collection contains ANY of the provided values
            ContainsAll,          // Property collection contains ALL of the provided values
            ArrayContains
        }

        public class FilterRule
        {
            public string PropertyName { get; set; }
            public ComparisonOperator Operator { get; set; } = ComparisonOperator.Equal;
        }

        #endregion

        #region Dynamic Query

        public static IQueryable<T> DynamicQuery<T>(this IQueryable<T> query, DynamicQueryRequest request, FilterConfiguration config = null)
        {
            // Apply filters
            if (request.Filters?.Any() == true)
            {
                query = query.DynamicFilterWithConfig(request.Filters, config);
            }

            // Apply search
            if (!string.IsNullOrWhiteSpace(request.SearchTerm))
            {
                query = query.DynamicSearch(request.SearchTerm, request.SearchFields?.ToArray() ?? new string[0]);
            }

            // Apply sorting
            if (request.SortFields?.Any() == true)
            {
                query = query.DynamicSort(request.SortFields);
            }

            // Apply pagination
            if (request.Skip > 0)
            {
                query = query.Skip(request.Skip);
            }

            if (request.Take > 0)
            {
                query = query.Take(request.Take);
            }

            return query;
        }

        #endregion

    }
}


namespace Shared.Extensions.DynamicQueries
{
    public static class QueryablePagedExtensions
    {
        public static PagedResult<T> ToPagedResultAsync<T>(this IQueryable<T> query, int page = 1, int pageSize = 10)
        {
            var totalCount = query.Count();
            var items = query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return new PagedResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                Page = page,
                PageSize = pageSize
            };
        }

        public class PagedResult<T>
        {
            public List<T> Items { get; set; } = new();
            public int TotalCount { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
            public int TotalPages => (int)Math.Ceiling((double)TotalCount / PageSize);
        }
    }
}

namespace GameStore.API.Models.Responses
{
    public class PageRequest
    {
        public int Page { get; set; }
        public int PageSize { get; set; }

        public PageRequest(int page, int pageSize)
        {
            Page = page < 1 ? 1 : page;
            PageSize = pageSize < 1 ? 5 : pageSize;
        }
    }

    public class PageResult<T>
    {
        public IEnumerable<T> Items { get; set; } = new List<T>();
        public int Page { get; set; }
        public int PageSize { get; set; }
        public int TotalCount { get; set; }
        public int TotalPages { get; set; }

        public PageResult(IEnumerable<T> items, int count, int page, int pageSize)
        {
            Items = items;
            TotalCount = count;
            Page = page;
            PageSize = pageSize;
            TotalPages = (int)Math.Ceiling(count / (double)pageSize);
        }
    }

    public static class PaginationExtensions
    {
        public static async Task<PageResult<T>> ToPageAsync<T>(
            this IQueryable<T> query,
            PageRequest request)
        {
            var count = await Task.Run(() => query.Count());
            var items = await Task.Run(() =>
                query.Skip((request.Page - 1) * request.PageSize)
                     .Take(request.PageSize)
                     .ToList()
            );

            return new PageResult<T>(items, count, request.Page, request.PageSize);
        }
    }
}


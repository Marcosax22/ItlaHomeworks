using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Models.Responses
{
    public class PageRequest
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
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
            PageRequest request,
            CancellationToken ct = default)
        {
            var page = request.Page < 1 ? 1 : request.Page;
            var pageSize = (request.PageSize < 1 || request.PageSize > 200) ? 10 : request.PageSize;

            var total = await query.CountAsync(ct);

            var ordered = query; 

            var items = await ordered
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync(ct);

            return new PageResult<T>(items, total, page, pageSize);
        }
    }
}
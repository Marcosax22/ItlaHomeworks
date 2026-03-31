using Microsoft.EntityFrameworkCore;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.Application.Models
{
    public class Pagination
    {
        public class PageRequest
        {
            public int Page { get; set; } = 1;
            public int PageSize { get; set; } = 5;
        }

        public class PageResult<T>
        {
            public List<T> Items { get; set; } = new();
            public int TotalCount { get; set; }
            public int TotalPages { get; set; }
            public int Page { get; set; }
            public int PageSize { get; set; }
        }

    }

    public static class QueryableExtensions
    {
        public static async Task<PageResult<T>> ToPageAsync<T>(
            this IQueryable<T> query,
            PageRequest request)
        {
            var totalCount = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            var items = await query
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResult<T>
            {
                Items = items,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }
    }

}
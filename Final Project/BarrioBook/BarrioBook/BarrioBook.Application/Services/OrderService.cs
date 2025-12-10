using BarrioBook.Application.DTOs;
using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.Application.Services
{
    public class OrderService
    {
        private readonly UnitOfWork _uow;

        public OrderService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PageResult<OrderDto>> GetPagedAsync(PageRequest request)
        {
            var query = _uow.Orders
                .Query()
                .Include(o => o.Customer)
                .Include(o => o.Items);

            var totalCount = await query.CountAsync();

            var orders = await query
                .OrderByDescending(o => o.OrderDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var bookIds = orders
                .SelectMany(o => o.Items)
                .Select(i => i.BookId)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            var ordersDto = new List<OrderDto>();

            foreach (var order in orders)
            {
                var itemDtos = order.Items.Select(i =>
                {
                    var book = books.Single(b => b.Id == i.BookId);
                    return new OrderItemDto
                    {
                        Id = i.Id,
                        BookId = i.BookId,
                        BookTitle = book.Title,
                        Quantity = i.Quantity
                    };
                }).ToList();

                ordersDto.Add(new OrderDto
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = order.Customer?.Name,
                    OrderDate = order.OrderDate,
                    Status = order.Status,
                    Items = itemDtos
                });
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PageResult<OrderDto>
            {
                Items = ordersDto,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _uow.Orders
                .Query()
                .Include(o => o.Customer)
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return null;

            var bookIds = order.Items
                .Select(i => i.BookId)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            var itemDtos = order.Items.Select(i =>
            {
                var book = books.Single(b => b.Id == i.BookId);
                return new OrderItemDto
                {
                    Id = i.Id,
                    BookId = i.BookId,
                    BookTitle = book.Title,
                    Quantity = i.Quantity
                };
            }).ToList();

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Items = itemDtos
            };
        }

        public async Task<int> CreateAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("Order must contain at least one item.");

            var bookIds = dto.Items
                .Select(i => i.BookId)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            if (books.Count != bookIds.Count)
                throw new KeyNotFoundException("One or more books were not found.");

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    throw new InvalidOperationException("Quantity must be greater than zero.");

                var book = books.Single(b => b.Id == item.BookId);

                if (item.Quantity > book.CurrentStock)
                    throw new InvalidOperationException($"Not enough stock for book {book.Title}.");
            }

            foreach (var item in dto.Items)
            {
                var book = books.Single(b => b.Id == item.BookId);
                book.CurrentStock -= item.Quantity;
            }

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                OrderDate = DateTime.Now,
                Status = OrderStatusNames.Pending,
                Items = new List<OrderItem>()
            };

            foreach (var item in dto.Items)
            {
                order.Items.Add(new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity
                });
            }

            await _uow.Orders.AddAsync(order);
            await _uow.SaveChangesAsync();

            return order.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateOrderDto dto)
        {
            var order = await _uow.Orders
                .Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            if (order.Status != OrderStatusNames.Pending)
                throw new InvalidOperationException("Only pending orders can be updated.");

            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("Order must contain at least one item.");

            var oldItems = order.Items
                .GroupBy(i => i.BookId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var newItems = dto.Items
                .GroupBy(i => i.BookId)
                .ToDictionary(g => g.Key, g => g.Sum(x => x.Quantity));

            var allBookIds = oldItems.Keys
                .Union(newItems.Keys)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => allBookIds.Contains(b.Id))
                .ToListAsync();

            foreach (var bookId in allBookIds)
            {
                var oldQty = oldItems.ContainsKey(bookId) ? oldItems[bookId] : 0;
                var newQty = newItems.ContainsKey(bookId) ? newItems[bookId] : 0;
                var diff = newQty - oldQty;

                if (diff > 0)
                {
                    var book = books.Single(b => b.Id == bookId);
                    if (diff > book.CurrentStock)
                        throw new InvalidOperationException($"Not enough stock for book {book.Title}.");
                }
            }

            foreach (var bookId in allBookIds)
            {
                var oldQty = oldItems.ContainsKey(bookId) ? oldItems[bookId] : 0;
                var newQty = newItems.ContainsKey(bookId) ? newItems[bookId] : 0;
                var diff = newQty - oldQty;

                if (diff != 0)
                {
                    var book = books.Single(b => b.Id == bookId);
                    book.CurrentStock -= diff;
                }
            }

            order.CustomerId = dto.CustomerId;
            order.Items.Clear();

            foreach (var item in dto.Items)
            {
                order.Items.Add(new OrderItem
                {
                    BookId = item.BookId,
                    Quantity = item.Quantity
                });
            }

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> UpdateStatusAsync(int id, string newStatus)
        {
            var order = await _uow.Orders
                .Query()
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            var allowed = new[]
            {
                OrderStatusNames.Pending,
                OrderStatusNames.Cancelled,
                OrderStatusNames.Paid
            };

            if (!allowed.Contains(newStatus))
                throw new InvalidOperationException("Invalid status value.");

            order.Status = newStatus;
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var order = await _uow.Orders
                .Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == id);

            if (order == null) return false;

            if (order.Status == OrderStatusNames.Cancelled ||
                order.Status == OrderStatusNames.Paid)
            {
                return false;
            }

            var bookIds = order.Items
                .Select(i => i.BookId)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            foreach (var item in order.Items)
            {
                var book = books.Single(b => b.Id == item.BookId);
                book.CurrentStock += item.Quantity;
            }

            order.Status = OrderStatusNames.Cancelled;

            await _uow.SaveChangesAsync();
            return true;
        }
    }
}

using BarrioBook.Application.DTOs;
using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.Application.Services
{
    public class SaleService
    {
        private readonly UnitOfWork _uow;

        public SaleService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PageResult<SaleDto>> GetPagedAsync(PageRequest request)
        {
            var query = _uow.Sales
                .Query()
                .Include(s => s.Customer)
                .Include(s => s.Items);

            var totalCount = await query.CountAsync();

            var sales = await query
                .OrderByDescending(s => s.SaleDate)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var bookIds = sales
                .SelectMany(s => s.Items)
                .Select(i => i.BookId)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            var salesDto = new List<SaleDto>();

            foreach (var sale in sales)
            {
                var itemDtos = sale.Items.Select(i =>
                {
                    var book = books.Single(b => b.Id == i.BookId);
                    return new SaleItemDto
                    {
                        Id = i.Id,
                        BookId = i.BookId,
                        BookTitle = book.Title,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Subtotal = i.Subtotal
                    };
                }).ToList();

                salesDto.Add(new SaleDto
                {
                    Id = sale.Id,
                    CustomerId = sale.CustomerId,
                    CustomerName = sale.Customer?.Name,
                    OrderId = sale.OrderId,
                    SaleDate = sale.SaleDate,
                    TotalAmount = sale.TotalAmount,
                    Items = itemDtos
                });
            }

            var totalPages = (int)Math.Ceiling(totalCount / (double)request.PageSize);

            return new PageResult<SaleDto>
            {
                Items = salesDto,
                TotalCount = totalCount,
                TotalPages = totalPages,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<SaleDto?> GetByIdAsync(int id)
        {
            var sale = await _uow.Sales
                .Query()
                .Include(s => s.Customer)
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return null;

            var bookIds = sale.Items
                .Select(i => i.BookId)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            var itemDtos = sale.Items.Select(i =>
            {
                var book = books.Single(b => b.Id == i.BookId);
                return new SaleItemDto
                {
                    Id = i.Id,
                    BookId = i.BookId,
                    BookTitle = book.Title,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                };
            }).ToList();

            return new SaleDto
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer?.Name,
                OrderId = sale.OrderId,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                Items = itemDtos
            };
        }

        public async Task<int> CreateAsync(CreateSaleDto dto)
        {
            if (dto.OrderId.HasValue)
            {
                return await CreateFromOrderAsync(dto.OrderId.Value);
            }

            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("Sale must contain at least one item.");

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

            decimal total = 0m;

            foreach (var item in dto.Items)
            {
                if (item.Quantity <= 0)
                    throw new InvalidOperationException("Quantity must be greater than zero.");

                var book = books.Single(b => b.Id == item.BookId);

                if (item.Quantity > book.CurrentStock)
                    throw new InvalidOperationException($"Not enough stock for book {book.Title}.");

                total += book.SalePrice * item.Quantity;
            }

            var sale = new Sale
            {
                CustomerId = dto.CustomerId,
                OrderId = null,
                SaleDate = DateTime.Now,
                TotalAmount = total,
                Items = new List<SaleItem>()
            };

            foreach (var item in dto.Items)
            {
                var book = books.Single(b => b.Id == item.BookId);
                var subtotal = book.SalePrice * item.Quantity;

                sale.Items.Add(new SaleItem
                {
                    BookId = book.Id,
                    Quantity = item.Quantity,
                    UnitPrice = book.SalePrice,
                    Subtotal = subtotal
                });

                book.CurrentStock -= item.Quantity;
            }

            await _uow.Sales.AddAsync(sale);
            await _uow.SaveChangesAsync();

            return sale.Id;
        }

        private async Task<int> CreateFromOrderAsync(int orderId)
        {
            var order = await _uow.Orders
                .Query()
                .Include(o => o.Items)
                .FirstOrDefaultAsync(o => o.Id == orderId);

            if (order == null)
                throw new KeyNotFoundException("Order not found.");

            if (order.Status == OrderStatusNames.Cancelled)
                throw new InvalidOperationException("Cannot create a sale from a cancelled order.");

            if (order.Status == OrderStatusNames.Paid)
                throw new InvalidOperationException("This order is already paid.");

            if (order.Items == null || order.Items.Count == 0)
                throw new InvalidOperationException("Order has no items.");

            var bookIds = order.Items
                .Select(i => i.BookId)
                .Distinct()
                .ToList();

            var books = await _uow.Books
                .Query()
                .Where(b => bookIds.Contains(b.Id))
                .ToListAsync();

            decimal total = 0m;

            foreach (var item in order.Items)
            {
                var book = books.Single(b => b.Id == item.BookId);
                total += book.SalePrice * item.Quantity;
            }

            var sale = new Sale
            {
                CustomerId = order.CustomerId,
                OrderId = order.Id,
                SaleDate = DateTime.Now,
                TotalAmount = total,
                Items = new List<SaleItem>()
            };

            foreach (var item in order.Items)
            {
                var book = books.Single(b => b.Id == item.BookId);
                var subtotal = book.SalePrice * item.Quantity;

                sale.Items.Add(new SaleItem
                {
                    BookId = book.Id,
                    Quantity = item.Quantity,
                    UnitPrice = book.SalePrice,
                    Subtotal = subtotal
                });

            }

            await _uow.Sales.AddAsync(sale);

            order.Status = OrderStatusNames.Paid;

            await _uow.SaveChangesAsync();

            return sale.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateSaleDto dto)
        {
            var sale = await _uow.Sales
                .Query()
                .Include(s => s.Items)
                .FirstOrDefaultAsync(s => s.Id == id);

            if (sale == null) return false;

            if (sale.OrderId.HasValue)
            {
                sale.CustomerId = dto.CustomerId ?? sale.CustomerId;
                if (dto.SaleDate.HasValue)
                    sale.SaleDate = dto.SaleDate.Value;

                await _uow.SaveChangesAsync();
                return true;
            }

            if (dto.Items == null || dto.Items.Count == 0)
                throw new InvalidOperationException("Sale must contain at least one item.");

            var oldItems = sale.Items
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

            sale.CustomerId = dto.CustomerId ?? sale.CustomerId;
            if (dto.SaleDate.HasValue)
                sale.SaleDate = dto.SaleDate.Value;

            sale.Items.Clear();
            decimal total = 0m;

            foreach (var item in dto.Items)
            {
                var book = books.Single(b => b.Id == item.BookId);
                var subtotal = book.SalePrice * item.Quantity;

                sale.Items.Add(new SaleItem
                {
                    BookId = book.Id,
                    Quantity = item.Quantity,
                    UnitPrice = book.SalePrice,
                    Subtotal = subtotal
                });

                total += subtotal;
            }

            sale.TotalAmount = total;

            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var sale = await _uow.Sales.GetByIdAsync(id);
            if (sale == null) return false;

            _uow.Sales.Remove(sale);

            await _uow.SaveChangesAsync();
            return true;
        }
    }
}

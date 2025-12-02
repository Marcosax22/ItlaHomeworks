using BarrioBook.Application.DTOs;
using BarrioBook.Application.Models;
using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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

        public async Task<OrderDto?> GetByIdAsync(int id)
        {
            var order = await _uow.Orders.GetByIdAsync(id);
            if (order == null) return null;

            return new OrderDto
            {
                Id = order.Id,
                CustomerId = order.CustomerId,
                CustomerName = order.Customer?.Name,
                OrderDate = order.OrderDate,
                Status = order.Status,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    Id = i.Id,
                    BookId = i.BookId,
                    BookTitle = i.Book.Title,
                    Quantity = i.Quantity
                }).ToList()
            };
        }

        public async Task<PageResult<OrderDto>> GetPagedAsync(PageRequest request)
        {
            var query = _uow.Orders.Query()
                .Select(o => new OrderDto
                {
                    Id = o.Id,
                    CustomerId = o.CustomerId,
                    CustomerName = o.Customer != null ? o.Customer.Name : null,
                    OrderDate = o.OrderDate,
                    Status = o.Status,
                    Items = o.Items.Select(i => new OrderItemDto
                    {
                        Id = i.Id,
                        BookId = i.BookId,
                        BookTitle = i.Book.Title,
                        Quantity = i.Quantity
                    }).ToList()
                });

            return await query.ToPageAsync(request);
        }

        public async Task<int> CreateAsync(CreateOrderDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new System.InvalidOperationException("Order must contain at least one item.");

            var bookIds = dto.Items.Select(i => i.BookId).Distinct().ToList();
            var books = await _uow.Books.GetByIdsAsync(bookIds);

            foreach (var item in dto.Items)
            {
                var book = books.SingleOrDefault(b => b.Id == item.BookId);
                if (book == null)
                    throw new KeyNotFoundException($"Book {item.BookId} not found.");

                if (item.Quantity <= 0)
                    throw new System.InvalidOperationException("Quantity must be greater than zero.");

                if (item.Quantity > book.CurrentStock)
                    throw new System.InvalidOperationException($"Not enough stock for book {book.Title}.");
            }

            var order = new Order
            {
                CustomerId = dto.CustomerId,
                Status = dto.Status,
                Items = new List<OrderItem>()
            };

            foreach (var item in dto.Items)
            {
                var book = books.Single(b => b.Id == item.BookId);

                order.Items.Add(new OrderItem
                {
                    BookId = book.Id,
                    Quantity = item.Quantity
                });

                book.CurrentStock -= item.Quantity;
                _uow.Books.Update(book);
            }

            await _uow.Orders.AddAsync(order);
            await _uow.SaveChangesAsync();
            return order.Id;
        }

        public async Task<bool> CancelAsync(int id)
        {
            var order = await _uow.Orders.GetByIdAsync(id);
            if (order == null) return false;

            order.Status = "Cancelled";
            _uow.Orders.Update(order);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}

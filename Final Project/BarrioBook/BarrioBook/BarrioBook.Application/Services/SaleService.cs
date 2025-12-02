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
    public class SaleService
    {
        private readonly UnitOfWork _uow;

        public SaleService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<SaleDto?> GetByIdAsync(int id)
        {
            var sale = await _uow.Sales.GetByIdAsync(id);
            if (sale == null) return null;

            return new SaleDto
            {
                Id = sale.Id,
                CustomerId = sale.CustomerId,
                CustomerName = sale.Customer?.Name,
                SaleDate = sale.SaleDate,
                TotalAmount = sale.TotalAmount,
                Items = sale.Items.Select(i => new SaleItemDto
                {
                    Id = i.Id,
                    BookId = i.BookId,
                    BookTitle = i.Book.Title,
                    Quantity = i.Quantity,
                    UnitPrice = i.UnitPrice,
                    Subtotal = i.Subtotal
                }).ToList()
            };
        }

        public async Task<PageResult<SaleDto>> GetPagedAsync(PageRequest request)
        {
            var query = _uow.Sales.Query()
                .Select(s => new SaleDto
                {
                    Id = s.Id,
                    CustomerId = s.CustomerId,
                    CustomerName = s.Customer != null ? s.Customer.Name : null,
                    SaleDate = s.SaleDate,
                    TotalAmount = s.TotalAmount,
                    Items = s.Items.Select(i => new SaleItemDto
                    {
                        Id = i.Id,
                        BookId = i.BookId,
                        BookTitle = i.Book.Title,
                        Quantity = i.Quantity,
                        UnitPrice = i.UnitPrice,
                        Subtotal = i.Subtotal
                    }).ToList()
                });

            return await query.ToPageAsync(request);
        }

        public async Task<int> CreateAsync(CreateSaleDto dto)
        {
            if (dto.Items == null || dto.Items.Count == 0)
                throw new System.InvalidOperationException("Sale must contain at least one item.");

            var bookIds = dto.Items.Select(i => i.BookId).Distinct().ToList();
            var books = await _uow.Books.GetByIdsAsync(bookIds);

            decimal total = 0m;

            foreach (var item in dto.Items)
            {
                var book = books.SingleOrDefault(b => b.Id == item.BookId);
                if (book == null)
                    throw new KeyNotFoundException($"Book {item.BookId} not found.");

                if (item.Quantity <= 0)
                    throw new System.InvalidOperationException("Quantity must be greater than zero.");

                if (item.Quantity > book.CurrentStock)
                    throw new System.InvalidOperationException($"Not enough stock for book {book.Title}.");

                total += book.SalePrice * item.Quantity;
            }

            var sale = new Sale
            {
                CustomerId = dto.CustomerId,
                SaleDate = System.DateTime.Now,
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
                _uow.Books.Update(book);
            }

            await _uow.Sales.AddAsync(sale);
            await _uow.SaveChangesAsync();
            return sale.Id;
        }
    }
}

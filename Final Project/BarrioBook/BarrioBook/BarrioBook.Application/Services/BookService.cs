using BarrioBook.Application.DTOs;
using BarrioBook.Application.Models;
using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Repositories;
using static BarrioBook.Application.Models.Pagination;


namespace BarrioBook.Application.Services
{
    public class BookService
    {
        private readonly UnitOfWork _uow;

        public BookService(UnitOfWork uow)
        {
            _uow = uow;
        }

        public async Task<PageResult<BookDto>> GetPagedAsync(PageRequest request)
        {
            var query = _uow.Books.Query()
                .Select(b => new BookDto
                {
                    Id = b.Id,
                    Title = b.Title,
                    Author = b.Author,
                    SalePrice = b.SalePrice,
                    CurrentStock = b.CurrentStock,
                    SupplierId = b.SupplierId,
                    SupplierName = b.Supplier != null ? b.Supplier.Name : null,
                    ImageUrl = b.ImageUrl
                });

            return await query.ToPageAsync(request);
        }

        public async Task<BookDto?> GetByIdAsync(int id)
        {
            var book = await _uow.Books.GetByIdAsync(id);
            if (book == null) return null;

            return new BookDto
            {
                Id = book.Id,
                Title = book.Title,
                Author = book.Author,
                SalePrice = book.SalePrice,
                CurrentStock = book.CurrentStock,
                SupplierId = book.SupplierId,
                SupplierName = book.Supplier?.Name,
                ImageUrl = book.ImageUrl
            };
        }

        public async Task<int> CreateAsync(CreateBookDto dto)
        {
            var book = new Book
            {
                Title = dto.Title,
                Author = dto.Author,
                SalePrice = dto.SalePrice,
                CurrentStock = dto.CurrentStock,
                SupplierId = dto.SupplierId,
                ImageUrl = dto.ImageUrl
            };

            await _uow.Books.AddAsync(book);
            await _uow.SaveChangesAsync();
            return book.Id;
        }

        public async Task<bool> UpdateAsync(int id, UpdateBookDto dto)
        {
            var book = await _uow.Books.GetByIdAsync(id);
            if (book == null) return false;

            book.Title = dto.Title;
            book.Author = dto.Author;
            book.SalePrice = dto.SalePrice;
            book.CurrentStock = dto.CurrentStock;
            book.SupplierId = dto.SupplierId;
            book.ImageUrl = dto.ImageUrl;

            _uow.Books.Update(book);
            await _uow.SaveChangesAsync();
            return true;
        }

        public async Task<bool> DeleteAsync(int id)
        {
            var book = await _uow.Books.GetByIdAsync(id);
            if (book == null) return false;

            _uow.Books.Remove(book);
            await _uow.SaveChangesAsync();
            return true;
        }
    }
}

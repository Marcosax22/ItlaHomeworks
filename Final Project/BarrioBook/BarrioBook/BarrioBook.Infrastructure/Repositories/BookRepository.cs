using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Infrastructure.Repositories
{
    public class BookRepository
    {
        private readonly BarrioBookDbContext _context;

        public BookRepository(BarrioBookDbContext context)
        {
            _context = context;
        }

        public IQueryable<Book> Query()
        {
            return _context.Books
                .Include(b => b.Supplier)
                .AsQueryable();
        }

        public async Task<Book?> GetByIdAsync(int id)
        {
            return await _context.Books
                .Include(b => b.Supplier)
                .FirstOrDefaultAsync(b => b.Id == id);
        }

        public async Task<List<Book>> ListAsync()
        {
            return await _context.Books
                .Include(b => b.Supplier)
                .ToListAsync();
        }

        public async Task<List<Book>> GetByIdsAsync(IEnumerable<int> ids)
        {
            return await _context.Books
                .Where(b => ids.Contains(b.Id))
                .ToListAsync();
        }

        public async Task AddAsync(Book book)
        {
            await _context.Books.AddAsync(book);
        }

        public void Update(Book book)
        {
            _context.Books.Update(book);
        }

        public void Remove(Book book)
        {
            _context.Books.Remove(book);
        }
    }
}

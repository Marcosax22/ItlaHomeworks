using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Infrastructure.Repositories
{
    public class SaleRepository
    {
        private readonly BarrioBookDbContext _context;

        public SaleRepository(BarrioBookDbContext context)
        {
            _context = context;
        }

        public IQueryable<Sale> Query()
        {
            return _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Book)
                .AsQueryable();
        }

        public async Task<Sale?> GetByIdAsync(int id)
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Sale>> ListAsync()
        {
            return await _context.Sales
                .Include(s => s.Customer)
                .Include(s => s.Items)
                    .ThenInclude(i => i.Book)
                .ToListAsync();
        }

        public async Task AddAsync(Sale sale)
        {
            await _context.Sales.AddAsync(sale);
        }

        public void Update(Sale sale)
        {
            _context.Sales.Update(sale);
        }

        public void Remove(Sale sale)
        {
            _context.Sales.Remove(sale);
        }
    }
}

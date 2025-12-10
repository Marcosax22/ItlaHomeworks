using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Infrastructure.Repositories
{
    public class SupplierRepository
    {
        private readonly BarrioBookDbContext _context;

        public SupplierRepository(BarrioBookDbContext context)
        {
            _context = context;
        }

        public IQueryable<Supplier> Query()
        {
            return _context.Suppliers.AsQueryable();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers.FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task<List<Supplier>> ListAsync()
        {
            return await _context.Suppliers.ToListAsync();
        }

        public async Task AddAsync(Supplier supplier)
        {
            await _context.Suppliers.AddAsync(supplier);
        }

        public void Update(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
        }

        public void Remove(Supplier supplier)
        {
            _context.Suppliers.Remove(supplier);
        }
    }
}

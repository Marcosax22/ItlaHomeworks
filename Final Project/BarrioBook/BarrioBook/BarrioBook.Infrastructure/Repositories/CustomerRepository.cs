using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Infrastructure.Repositories
{
    public class CustomerRepository
    {
        private readonly BarrioBookDbContext _context;

        public CustomerRepository(BarrioBookDbContext context)
        {
            _context = context;
        }

        public IQueryable<Customer> Query()
        {
            return _context.Customers.AsQueryable();
        }

        public async Task<Customer?> GetByIdAsync(int id)
        {
            return await _context.Customers.FirstOrDefaultAsync(c => c.Id == id);
        }

        public async Task<List<Customer>> ListAsync()
        {
            return await _context.Customers.ToListAsync();
        }

        public async Task AddAsync(Customer customer)
        {
            await _context.Customers.AddAsync(customer);
        }

        public void Update(Customer customer)
        {
            _context.Customers.Update(customer);
        }

        public void Remove(Customer customer)
        {
            _context.Customers.Remove(customer);
        }
    }
}

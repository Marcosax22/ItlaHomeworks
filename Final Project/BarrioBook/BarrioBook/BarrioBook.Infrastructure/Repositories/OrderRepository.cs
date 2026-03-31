using BarrioBook.Domain.Entities;
using BarrioBook.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Infrastructure.Repositories
{
    public class OrderRepository
    {
        private readonly BarrioBookDbContext _context;

        public OrderRepository(BarrioBookDbContext context)
        {
            _context = context;
        }

        public IQueryable<Order> Query()
        {
            return _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Book)
                .AsQueryable();
        }

        public async Task<Order?> GetByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Book)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        public async Task<List<Order>> ListAsync()
        {
            return await _context.Orders
                .Include(o => o.Customer)
                .Include(o => o.Items)
                    .ThenInclude(i => i.Book)
                .ToListAsync();
        }

        public async Task AddAsync(Order order)
        {
            await _context.Orders.AddAsync(order);
        }

        public void Update(Order order)
        {
            _context.Orders.Update(order);
        }

        public void Remove(Order order)
        {
            _context.Orders.Remove(order);
        }
    }
}

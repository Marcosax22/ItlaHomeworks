using BarrioBook.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace BarrioBook.Infrastructure.Data
{
    public class BarrioBookDbContext : DbContext
    {
        public BarrioBookDbContext(DbContextOptions<BarrioBookDbContext> options)
            : base(options)
        {
        }

        public DbSet<Book> Books { get; set; } = null!;
        public DbSet<Customer> Customers { get; set; } = null!;
        public DbSet<Supplier> Suppliers { get; set; } = null!;
        public DbSet<Order> Orders { get; set; } = null!;
        public DbSet<OrderItem> OrderItems { get; set; } = null!;
        public DbSet<Sale> Sales { get; set; } = null!;
        public DbSet<SaleItem> SaleItems { get; set; } = null!;
    }
}

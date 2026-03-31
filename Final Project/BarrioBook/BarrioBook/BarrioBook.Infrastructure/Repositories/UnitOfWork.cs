using BarrioBook.Infrastructure.Data;

namespace BarrioBook.Infrastructure.Repositories
{
    public class UnitOfWork
    {
        private readonly BarrioBookDbContext _context;

        public BookRepository Books { get; }
        public CustomerRepository Customers { get; }
        public SupplierRepository Suppliers { get; }
        public OrderRepository Orders { get; }
        public SaleRepository Sales { get; }

        public UnitOfWork(BarrioBookDbContext context)
        {
            _context = context;

            Books = new BookRepository(context);
            Customers = new CustomerRepository(context);
            Suppliers = new SupplierRepository(context);
            Orders = new OrderRepository(context);
            Sales = new SaleRepository(context);
        }

        public Task<int> SaveChangesAsync()
        {
            return _context.SaveChangesAsync();
        }
    }
}

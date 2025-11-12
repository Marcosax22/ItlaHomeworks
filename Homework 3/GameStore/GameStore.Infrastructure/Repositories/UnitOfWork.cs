using GameStore.Infrastructure.Data;

namespace GameStore.Infrastructure.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly GameStoreDbContext _context;

        public GameRepository Games { get; }

        public UnitOfWork(GameStoreDbContext context, GameRepository gameRepository)
        {
            _context = context;
            Games = gameRepository;
        }

        public void Complete()
        {
            _context.SaveChanges();
        }

        public Task SaveAsync()
        {
            return _context.SaveChangesAsync();
        }

        public void BeginTransaction() => _context.Database.BeginTransaction();
        public Task BeginTransactionAsync() => _context.Database.BeginTransactionAsync();

        public void CommitTransaction() => _context.Database.CommitTransaction();
        public Task CommitTransactionAsync() => _context.Database.CommitTransactionAsync();

        public void RollbackTransaction() => _context.Database.RollbackTransaction();

        public void Dispose() => _context.Dispose();
    }
}

using GameStore.Infrastructure.Data;

namespace GameStore.Infrastructure.Repositories
{
    public class UnitOfWork : IDisposable
    {
        private readonly GameStoreDbContext _context;
        public GameRepository Games { get; }

        public UnitOfWork(GameStoreDbContext context, GameRepository games)
        {
            _context = context;
            Games = games;
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}
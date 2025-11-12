using GameStore.Domain.Entities;
using GameStore.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace GameStore.Infrastructure.Repositories
{
    public class GameRepository
    {
        private readonly GameStoreDbContext _context;

        public GameRepository(GameStoreDbContext context)
        {
            _context = context;
        }

        public List<Games> GetAll()
        {
            return _context.Games.AsNoTracking().ToList();
        }

        public Games? GetById(int id)
        {

            return _context.Games.Find(id);
        }

        public void Create(Games game)
        {
            _context.Games.Add(game);
        }

        public IQueryable<Games> Query()
        {
            return _context.Games.AsNoTracking();
        }

        public void Update(int id, Games updatedGame)
        {
            var existing = _context.Games.Find(id);
            if (existing == null)
                throw new KeyNotFoundException("Game not found");

            existing.Name = updatedGame.Name;
            existing.Description = updatedGame.Description;
            existing.Price = updatedGame.Price;

            _context.Games.Update(existing);
        }

        public void Delete(int id)
        {
            var game = _context.Games.Find(id);
            if (game == null)
                throw new KeyNotFoundException("Game not found");

            _context.Games.Remove(game);
        }
    }
}

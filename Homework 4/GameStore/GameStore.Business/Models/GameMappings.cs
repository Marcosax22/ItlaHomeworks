using GameStore.Business.Dtos;
using GameStore.Domain.Entities;

namespace GameStore.Business.Models
{
    public static class GameMappings
    {
        public static GameDto ToDto(this Games g) => new GameDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            Price = g.Price
        };

        public static Games ToEntity(this GameCreateDto d) => new Games
        {
            Name = d.Name,
            Description = d.Description,
            Price = d.Price
        };

        public static void MapToEntity(this GameUpdateDto d, Games e)
        {

            e.Name = d.Name;
            e.Description = d.Description;
            e.Price = d.Price;
        }
    }
}

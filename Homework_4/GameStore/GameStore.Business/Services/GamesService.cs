using GameStore.Business.Dtos;
using GameStore.Business.Models;
using GameStore.Business.Responses;
using GameStore.Business.Responses.Models;
using GameStore.Infrastructure.Repositories;

namespace GameStore.Business.Services
{
    public class GameService
    {
        private readonly UnitOfWork _unit;

        public GameService(UnitOfWork unit)
        {
            _unit = unit;
        }

        public ApiResponse<List<GameDto>> GetAll()
        {
            var games = _unit.Games.GetAll();
            var dtos = games.Select(g => g.ToDto()).ToList();

            return ApiResponse<List<GameDto>>.Success(
                dtos,
                200,
                "Games loaded successfully."
            );
        }

        public ApiResponse<GameDto> GetById(int id)
        {
            var game = _unit.Games.GetById(id);

            if (game == null)
                return ApiResponse<GameDto>.Fail("Game not found", 404);

            return ApiResponse<GameDto>.Success(
                game.ToDto(),
                200,
                "Game found."
            );
        }

        public ApiResponse<GameDto> Create(GameCreateDto dto)
        {
            var entity = dto.ToEntity();

            _unit.Games.Create(entity);
            _unit.Save();

            return ApiResponse<GameDto>.Success(
                entity.ToDto(),
                201,
                "Game created successfully."
            );
        }

        public ApiResponse<string> Update(int id, GameUpdateDto dto)
        {
            var entity = _unit.Games.GetById(id);

            if (entity == null)
                return ApiResponse<string>.Fail("Game not found", 404);

            dto.MapToEntity(entity);

            _unit.Games.Update(entity);
            _unit.Save();

            return ApiResponse<string>.Success(
                "Game updated successfully.",
                200
            );
        }

        public ApiResponse<string> Delete(int id)
        {
            var entity = _unit.Games.GetById(id);

            if (entity == null)
                return ApiResponse<string>.Fail("Game not found", 404);

            _unit.Games.Delete(id);
            _unit.Save();

            return ApiResponse<string>.Success(
                "Game deleted successfully.",
                200
            );
        }

        public async Task<ApiResponse<PageResult<GameDto>>> GetPaginatedAsync(PageRequest request)
        {
            var query = _unit.Games.Query();

            var paged = await query
                .Select(g => g.ToDto())
                .ToPageAsync(request);

            return ApiResponse<PageResult<GameDto>>.Success(
                paged,
                200,
                "Paged games loaded successfully."
            );
        }
    }
}

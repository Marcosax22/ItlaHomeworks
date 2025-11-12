using GameStore.API.Models.Dtos;
using GameStore.API.Models.Responses;
using GameStore.Domain.Entities;
using GameStore.Infrastructure.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly UnitOfWork _unitOfWork;

        public GamesController(UnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        [HttpGet("List")]
        public async Task<ActionResult<ApiResponse<PageResult<GameDto>>>> List([FromQuery] PageRequest request)
        {
            var query = _unitOfWork.Games
                .Query()
                .OrderBy(g => g.Id)
                .Select(g => new GameDto
                {
                    Id = g.Id,
                    Name = g.Name,
                    Description = g.Description,
                    Price = g.Price
                });

            var paged = await query.ToPageAsync(request);
            return Ok(ApiResponse<PageResult<GameDto>>.Success(paged, 200, "Games loaded successfully."));
        }

        [HttpGet("Details/{id:int}")]
        public IActionResult GetById(int id)
        {
            var game = _unitOfWork.Games.GetById(id);
            if (game == null)
                return NotFound(new { message = "Game not found" });

            return Ok(game);
        }

        [HttpPost("Create")]
        public async Task<IActionResult> Create([FromBody] Games game)
        {
            _unitOfWork.Games.Create(game);
            await _unitOfWork.SaveAsync();
            return CreatedAtAction(nameof(GetById), new { id = game.Id }, game);
        }

        [HttpPut("Update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] Games updatedGame)
        {
            try
            {
                _unitOfWork.Games.Update(id, updatedGame);
                await _unitOfWork.SaveAsync();
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Game not found" });
            }
        }

        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            try
            {
                _unitOfWork.Games.Delete(id);
                await _unitOfWork.SaveAsync();
                return NoContent();
            }
            catch (KeyNotFoundException)
            {
                return NotFound(new { message = "Game not found" });
            }
        }
    }
}

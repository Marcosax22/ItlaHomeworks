using GameStore.Business.Dtos;
using GameStore.Business.Responses.Models;
using GameStore.Business.Services;
using Microsoft.AspNetCore.Mvc;

namespace GameStore.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GamesController : ControllerBase
    {
        private readonly GameService _service;

        public GamesController(GameService service)
        {
            _service = service;
        }

        [HttpGet("Paged")]
        public async Task<IActionResult> GetPagedGames([FromQuery] PageRequest request)
        {
            var result = await _service.GetPaginatedAsync(request);
            return Ok(result);
        }

        [HttpGet("All/List")]
        public IActionResult GetAllGames()
        {
            var result = _service.GetAll();
            return Ok(result);
        }

        [HttpGet("Details/{id:int}")]
        public IActionResult GetGameById(int id)
        {
            var result = _service.GetById(id);
            return Ok(result);
        }

        [HttpPost("Create")]
        public IActionResult CreateGame([FromBody] GameCreateDto dto)
        {
            var result = _service.Create(dto);
            return Ok(result);
        }

        [HttpPut("Update/{id:int}")]
        public IActionResult UpdateGame(int id, [FromBody] GameUpdateDto dto)
        {
            var result = _service.Update(id, dto);
            return Ok(result);
        }

        [HttpDelete("Delete/{id:int}")]
        public IActionResult DeleteGame(int id)
        {
            var result = _service.Delete(id);
            return Ok(result);
        }
    }
}


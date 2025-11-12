//Marcos Ariel 2024-1785
using GameStore.API.Data;
using GameStore.API.Models;
using GameStore.API.Models.Dtos;
using GameStore.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace GameStore.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GamesController : ControllerBase
    {
        private readonly GameStoreDbContext _context;
        private readonly ILogger<GamesController> _logger;

        public GamesController(GameStoreDbContext context, ILogger<GamesController> logger)
        {
            _context = context;
            _logger = logger;
        }

        [HttpGet("List")]
        public async Task<ActionResult<ApiResponse<PageResult<GameDto>>>> GetAll([FromQuery] int page = 1, [FromQuery] int pageSize = 5)
        {
            var query = _context.Games
                .AsNoTracking()
                .Select(g => g.ToDto());

            var request = new PageRequest(page, pageSize);
            var pagedResult = await query.ToPageAsync(request);

            var response = ApiResponse<PageResult<GameDto>>.Success(
                pagedResult,
                200,
                "Games loaded successfully."
            );

            return Ok(response);
        }

        //[HttpGet("List")]
        //public async Task<ActionResult<Pagination<GameDto>>> GetAll(int pageNumber = 1, int pageSize = 5)
        //{
        //    if (pageNumber < 1 || pageSize < 1)
        //        return BadRequest(new { message = "PageNumber and PageSize must be greater than 0." });

        //    var query = _context.Games.AsQueryable();
        //    var totalCount = await query.CountAsync();

        //    var entities = await query
        //        .OrderBy(g => g.id) 
        //        .Skip((pageNumber - 1) * pageSize)
        //        .Take(pageSize)
        //        .ToListAsync();

        //    var dtos = entities.Select(e => e.ToDto());

        //    var result = new Pagination<GameDto>
        //    {
        //        PageNumber = pageNumber,
        //        PageSize = pageSize,
        //        TotalCount = totalCount,
        //        Data = dtos
        //    };

        //    return Ok(result);
        //}

        [HttpGet("Details/{id:int}")]
        public async Task<ActionResult<GameDto>> GetById(int id)
        {
            var entity = await _context.Games.FindAsync(id);
            if (entity == null) return NotFound();
            return Ok(entity.ToDto());
        }

        [HttpPost("Create")]
        public async Task<ActionResult<GameDto>> Create([FromBody] GameCreateDto input)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = input.ToEntity();

            if (await _context.Games.AnyAsync(g => g.Name == entity.Name))
                return Conflict(new { message = "A game with the same name already exists." });

            _context.Games.Add(entity);
            await _context.SaveChangesAsync();

            var dto = entity.ToDto();
            return CreatedAtAction(nameof(GetById), new { id = dto.Id }, dto);
        }

        [HttpPut("Update/{id:int}")]
        public async Task<IActionResult> Update(int id, [FromBody] GameUpdateDto update)
        {
            if (id != update.Id) return BadRequest(new { message = "Id in route and body do not match." });

            if (!ModelState.IsValid) return BadRequest(ModelState);

            var entity = await _context.Games.FindAsync(id);
            if (entity == null) return NotFound();

            update.MapToEntity(entity);

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, "Concurrency error updating game {GameId}", id);
                if (!await _context.Games.AnyAsync(g => g.id == id))
                    return NotFound();
                throw;
            }

            return NoContent();
        }

        [HttpDelete("Delete/{id:int}")]
        public async Task<IActionResult> Delete(int id)
        {
            var entity = await _context.Games.FindAsync(id);
            if (entity == null) return NotFound();

            _context.Games.Remove(entity);
            await _context.SaveChangesAsync();
            return NoContent();
        }
    }
}
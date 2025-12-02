using BarrioBook.Application.DTOs;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/books")]
    public class BooksController : ControllerBase
    {
        private readonly BookService _service;

        public BooksController(BookService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PageResult<BookDto>>> GetPaged([FromQuery] PageRequest request)
        {
            var result = await _service.GetPagedAsync(request);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<BookDto>> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);
            if (book == null) return NotFound();
            return Ok(book);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateBookDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateBookDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok) return NotFound();
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<ActionResult> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

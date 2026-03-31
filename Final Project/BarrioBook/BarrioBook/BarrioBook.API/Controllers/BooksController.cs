using BarrioBook.Application.DTOs;
using BarrioBook.Application.Responses;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Authorization;
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

        [HttpGet("List")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<PageResult<BookDto>>>> GetPaged([FromQuery] PageRequest request)
        {
            var result = await _service.GetPagedAsync(request);
            return Ok(ApiResponse<PageResult<BookDto>>.Success(result));
        }

        [HttpGet("/{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ApiResponse<BookDto>>> GetById(int id)
        {
            var book = await _service.GetByIdAsync(id);
            if (book == null)
                return NotFound(ApiResponse<BookDto>.Fail("Book not found", 404));

            return Ok(ApiResponse<BookDto>.Success(book));
        }

        [HttpPost("Create")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateBookDto dto)
        {
            var id = await _service.CreateAsync(dto);
            var response = ApiResponse<int>.Success(id, 201, "Book created successfully");
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpPut("{id:int}/Update")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateBookDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok)
                return NotFound(ApiResponse<object>.Fail("Book not found", 404));

            return Ok(ApiResponse<object>.Success(null!, 200, "Book updated successfully"));
        }

        [HttpDelete("{id:int}/Delete")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound(ApiResponse<object>.Fail("Book not found", 404));

            return Ok(ApiResponse<object>.Success(null!, 200, "Book deleted successfully"));
        }
    }
}
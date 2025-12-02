using BarrioBook.Application.DTOs;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/sales")]
    public class SalesController : ControllerBase
    {
        private readonly SaleService _service;

        public SalesController(SaleService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PageResult<SaleDto>>> GetPaged([FromQuery] PageRequest request)
        {
            var result = await _service.GetPagedAsync(request);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SaleDto>> GetById(int id)
        {
            var sale = await _service.GetByIdAsync(id);
            if (sale == null) return NotFound();
            return Ok(sale);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateSaleDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }
    }
}

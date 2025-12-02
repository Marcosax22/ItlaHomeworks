using BarrioBook.Application.DTOs;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Mvc;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _service;

        public OrdersController(OrderService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<PageResult<OrderDto>>> GetPaged([FromQuery] PageRequest request)
        {
            var result = await _service.GetPagedAsync(request);
            return Ok(result);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<OrderDto>> GetById(int id)
        {
            var order = await _service.GetByIdAsync(id);
            if (order == null) return NotFound();
            return Ok(order);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateOrderDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult> Cancel(int id)
        {
            var ok = await _service.CancelAsync(id);
            if (!ok) return NotFound();
            return NoContent();
        }
    }
}

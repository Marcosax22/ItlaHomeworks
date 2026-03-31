using BarrioBook.Application.DTOs;
using BarrioBook.Application.Responses;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/orders")]
    [Authorize] 
    public class OrdersController : ControllerBase
    {
        private readonly OrderService _orderService;

        public OrdersController(OrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<ApiResponse<PageResult<OrderDto>>>> GetPaged([FromQuery] PageRequest request)
        {
            var result = await _orderService.GetPagedAsync(request);
            return Ok(ApiResponse<PageResult<OrderDto>>.Success(result));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<OrderDto>>> GetById(int id)
        {
            var order = await _orderService.GetByIdAsync(id);
            if (order == null)
                return NotFound(ApiResponse<OrderDto>.Fail("Order not found.", 404));

            return Ok(ApiResponse<OrderDto>.Success(order));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateOrderDto dto)
        {
            var id = await _orderService.CreateAsync(dto);
            var response = ApiResponse<int>.Success(id, 201, "Order created successfully.");
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpPut("{id:int}/Update")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateOrderDto dto)
        {
            var ok = await _orderService.UpdateAsync(id, dto);
            if (!ok)
                return NotFound(ApiResponse<object>.Fail("Order not found.", 404));

            return Ok(ApiResponse<object>.Success(null!, 200, "Order updated successfully."));
        }

        [HttpPost("{id:int}/cancel")]
        public async Task<ActionResult<ApiResponse<object>>> Cancel(int id)
        {
            var ok = await _orderService.CancelAsync(id);
            if (!ok)
                return BadRequest(ApiResponse<object>.Fail("Order cannot be cancelled."));

            return Ok(ApiResponse<object>.Success(null!, 200, "Order cancelled and stock restored."));
        }
    }
}
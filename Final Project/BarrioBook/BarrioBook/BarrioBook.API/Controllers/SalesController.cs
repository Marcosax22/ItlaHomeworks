using BarrioBook.Application.DTOs;
using BarrioBook.Application.Responses;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using static BarrioBook.Application.Models.Pagination;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/sales")]
    [Authorize] 
    public class SalesController : ControllerBase
    {
        private readonly SaleService _saleService;

        public SalesController(SaleService saleService)
        {
            _saleService = saleService;
        }

        [HttpGet("List")]
        public async Task<ActionResult<ApiResponse<PageResult<SaleDto>>>> GetPaged([FromQuery] PageRequest request)
        {
            var result = await _saleService.GetPagedAsync(request);
            return Ok(ApiResponse<PageResult<SaleDto>>.Success(result));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SaleDto>>> GetById(int id)
        {
            var sale = await _saleService.GetByIdAsync(id);
            if (sale == null)
                return NotFound(ApiResponse<SaleDto>.Fail("Sale not found.", 404));

            return Ok(ApiResponse<SaleDto>.Success(sale));
        }

        [HttpPost("/Create")]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateSaleDto dto)
        {
            var id = await _saleService.CreateAsync(dto);
            var response = ApiResponse<int>.Success(id, 201, "Sale created successfully.");
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpPut("{id:int}/Update")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateSaleDto dto)
        {
            var ok = await _saleService.UpdateAsync(id, dto);
            if (!ok)
                return NotFound(ApiResponse<object>.Fail("Sale not found.", 404));

            return Ok(ApiResponse<object>.Success(null!, 200, "Sale updated successfully."));
        }

        [HttpDelete("{id:int}/Delete")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var ok = await _saleService.DeleteAsync(id);
            if (!ok)
                return NotFound(ApiResponse<object>.Fail("Sale not found.", 404));

            return Ok(ApiResponse<object>.Success(null!, 200, "Sale deleted successfully."));
        }
    }
}
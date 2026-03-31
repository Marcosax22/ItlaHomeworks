using BarrioBook.Application.DTOs;
using BarrioBook.Application.Responses;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    [Authorize(Roles = "Admin")]
    public class SuppliersController : ControllerBase
    {
        private readonly SupplierService _service;

        public SuppliersController(SupplierService service)
        {
            _service = service;
        }

        [HttpGet("List")]
        public async Task<ActionResult<ApiResponse<List<SupplierDto>>>> GetAll()
        {
            var suppliers = await _service.GetAllAsync();
            return Ok(ApiResponse<List<SupplierDto>>.Success(suppliers));
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<ApiResponse<SupplierDto>>> GetById(int id)
        {
            var supplier = await _service.GetByIdAsync(id);
            if (supplier == null)
                return NotFound(ApiResponse<SupplierDto>.Fail("Supplier not found", 404));

            return Ok(ApiResponse<SupplierDto>.Success(supplier));
        }

        [HttpPost("Create")]
        public async Task<ActionResult<ApiResponse<int>>> Create([FromBody] CreateSupplierDto dto)
        {
            var id = await _service.CreateAsync(dto);
            var response = ApiResponse<int>.Success(id, 201, "Supplier created successfully");
            return CreatedAtAction(nameof(GetById), new { id }, response);
        }

        [HttpPut("{id:int}/Update")]
        public async Task<ActionResult<ApiResponse<object>>> Update(int id, [FromBody] UpdateSupplierDto dto)
        {
            var ok = await _service.UpdateAsync(id, dto);
            if (!ok)
                return NotFound(ApiResponse<object>.Fail("Supplier not found", 404));

            return Ok(ApiResponse<object>.Success(null!, 200, "Supplier updated successfully"));
        }

        [HttpDelete("{id:int}/Delete")]
        public async Task<ActionResult<ApiResponse<object>>> Delete(int id)
        {
            var ok = await _service.DeleteAsync(id);
            if (!ok)
                return NotFound(ApiResponse<object>.Fail("Supplier not found", 404));

            return Ok(ApiResponse<object>.Success(null!, 200, "Supplier deleted successfully"));
        }
    }
}
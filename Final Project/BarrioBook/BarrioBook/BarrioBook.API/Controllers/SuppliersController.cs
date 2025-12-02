using BarrioBook.Application.DTOs;
using BarrioBook.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace BarrioBook.API.Controllers
{
    [ApiController]
    [Route("api/suppliers")]
    public class SuppliersController : ControllerBase
    {
        private readonly SupplierService _service;

        public SuppliersController(SupplierService service)
        {
            _service = service;
        }

        [HttpGet]
        public async Task<ActionResult<List<SupplierDto>>> GetAll()
        {
            var suppliers = await _service.GetAllAsync();
            return Ok(suppliers);
        }

        [HttpGet("{id:int}")]
        public async Task<ActionResult<SupplierDto>> GetById(int id)
        {
            var supplier = await _service.GetByIdAsync(id);
            if (supplier == null) return NotFound();
            return Ok(supplier);
        }

        [HttpPost]
        public async Task<ActionResult> Create([FromBody] CreateSupplierDto dto)
        {
            var id = await _service.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<ActionResult> Update(int id, [FromBody] UpdateSupplierDto dto)
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

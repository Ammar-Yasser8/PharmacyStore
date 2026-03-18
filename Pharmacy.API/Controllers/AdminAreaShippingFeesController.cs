using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Services;
using Pharmacy.Services.Dtos.ShippingDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.API.Controllers
{
    [Route("api/admin/area-shipping-fees")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminAreaShippingFeesController : ControllerBase
    {
        private readonly IAreaShippingFeeService _shippingService;

        public AdminAreaShippingFeesController(IAreaShippingFeeService shippingService)
        {
            _shippingService = shippingService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<AreaShippingFeeToReturnDto>>> GetAll()
        {
            return Ok(await _shippingService.GetAllAsync());
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AreaShippingFeeToReturnDto>> GetById(int id)
        {
            var fee = await _shippingService.GetByIdAsync(id);
            if (fee == null) return NotFound();

            return Ok(fee);
        }

        [HttpPost]
        public async Task<ActionResult<AreaShippingFeeToReturnDto>> Create(AreaShippingFeeDto dto)
        {
            var newFee = await _shippingService.CreateAsync(dto);
            return CreatedAtAction(nameof(GetById), new { id = newFee.Id }, newFee);
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<AreaShippingFeeToReturnDto>> Update(int id, AreaShippingFeeDto dto)
        {
            var updated = await _shippingService.UpdateAsync(id, dto);
            if (updated == null) return NotFound();

            return Ok(updated);
        }

        [HttpDelete("{id}")]
        public async Task<ActionResult> Delete(int id)
        {
            var success = await _shippingService.DeleteAsync(id);
            if (!success) return NotFound();

            return NoContent();
        }
    }
}

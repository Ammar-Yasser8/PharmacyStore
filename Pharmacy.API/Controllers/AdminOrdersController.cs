using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Services;
using Pharmacy.Services.Dtos.OrderDtos;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Pharmacy.API.Controllers
{
    [Route("api/admin/orders")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class AdminOrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public AdminOrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetAllOrders()
        {
            var orders = await _orderService.GetAllOrdersAsync();
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderById(int id)
        {
            var order = await _orderService.GetOrderByIdAsync(id);
            
            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpPut("{id}/status")]
        public async Task<ActionResult<OrderToReturnDto>> UpdateOrderStatus(int id, [FromQuery] string status)
        {
            try
            {
                var order = await _orderService.UpdateOrderStatusAsync(id, status);
                if (order == null) return NotFound();

                return Ok(order);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

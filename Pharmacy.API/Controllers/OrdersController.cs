using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pharmacy.Services;
using Pharmacy.Services.Dtos.OrderDtos;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class OrdersController : ControllerBase
    {
        private readonly IOrderService _orderService;

        public OrdersController(IOrderService orderService)
        {
            _orderService = orderService;
        }

        [HttpPost("checkout")]
        public async Task<ActionResult<OrderToReturnDto>> CreateOrder(CreateOrderDto dto)
        {
            var email = User.FindFirstValue(ClaimTypes.Email);
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var buyerName = User.FindFirstValue(ClaimTypes.GivenName) ?? string.Empty;

            if (buyerId == null) return Unauthorized();

            var order = await _orderService.CreateOrderAsync(buyerId, buyerName, dto);
            if (order == null) return BadRequest("Problem creating order");

            return Ok(order);
        }

        [HttpGet]
        public async Task<ActionResult<IReadOnlyList<OrderToReturnDto>>> GetOrdersForUser()
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (buyerId == null) return Unauthorized();

            var orders = await _orderService.GetOrdersForUserAsync(buyerId);
            return Ok(orders);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<OrderToReturnDto>> GetOrderByIdForUser(int id)
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (buyerId == null) return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id, buyerId);
            if (order == null) return NotFound();

            return Ok(order);
        }

        [HttpPut("{id}/cancel")]
        public async Task<ActionResult<OrderToReturnDto>> CancelOrder(int id)
        {
            var buyerId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (buyerId == null) return Unauthorized();

            var order = await _orderService.GetOrderByIdAsync(id, buyerId);
            if (order == null) return NotFound();

            if (order.Status != "Pending")
            {
                return BadRequest("Only Pending orders can be cancelled automatically. If you need to cancel a confirmed order, please contact support.");
            }

            try
            {
                var cancelledOrder = await _orderService.UpdateOrderStatusAsync(id, "Cancelled");
                return Ok(cancelledOrder);
            }
            catch (System.Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
    }
}

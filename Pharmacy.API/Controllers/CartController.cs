using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos.CartDtos;
using Pharmacy.Services;
using Pharmacy.Services.Dtos.CartDtos;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Pharmacy.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CartController : ControllerBase
    {
        private readonly ICartService _cartService;

        public CartController(ICartService cartService)
        {
            _cartService = cartService;
        }

        [HttpGet("{cartId}")]
        public async Task<ActionResult<CartToReturnDto>> GetCart(string cartId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.GetCartAsync(cartId, userId);
            if (cart == null) return NotFound("Cart not found");

            return Ok(cart);
        }

        [HttpPost("{cartId}/items")]
        public async Task<ActionResult<CartToReturnDto>> AddItemToCart(string cartId, [FromBody] AddCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.AddItemAsync(cartId, dto.ProductId, dto.Quantity, userId);
            if (cart == null) return BadRequest("Could not add item to cart (invalid product or insufficient stock)");

            return Ok(cart);
        }

        [HttpPut("{cartId}/items/{productId}")]
        public async Task<ActionResult<CartToReturnDto>> UpdateItemQuantity(string cartId, int productId, [FromBody] UpdateCartItemDto dto)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.UpdateItemQuantityAsync(cartId, productId, dto.Quantity, userId);
            if (cart == null) return BadRequest("Could not update item quantity");

            return Ok(cart);
        }

        [HttpDelete("{cartId}/items/{productId}")]
        public async Task<ActionResult<CartToReturnDto>> RemoveItemFromCart(string cartId, int productId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.RemoveItemAsync(cartId, productId, userId);
            if (cart == null) return NotFound("Item or cart not found");

            return Ok(cart);
        }

        [HttpDelete("{cartId}")]
        public async Task<ActionResult<CartToReturnDto>> ClearCart(string cartId)
        {
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            var cart = await _cartService.ClearCartAsync(cartId, userId);
            if (cart == null) return NotFound("Cart not found");

            return Ok(cart);
        }
    }
}

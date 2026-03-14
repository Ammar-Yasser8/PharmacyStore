using Microsoft.AspNetCore.Mvc;
using Pharmacy.API.Dtos.CartDtos;
using Pharmacy.API.Helpers;
using Pharmacy.Services;
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
            var cart = await _cartService.GetCartAsync(cartId);
            if (cart == null) return NotFound("Cart not found");

            return Ok(cart.MapToDto());
        }

        [HttpPost("{cartId}/items")]
        public async Task<ActionResult<CartToReturnDto>> AddItemToCart(string cartId, [FromBody] AddCartItemDto dto)
        {
            var cart = await _cartService.AddItemAsync(cartId, dto.ProductId, dto.Quantity);
            if (cart == null) return BadRequest("Could not add item to cart (invalid product or insufficient stock)");

            return Ok(cart.MapToDto());
        }

        [HttpPut("{cartId}/items/{productId}")]
        public async Task<ActionResult<CartToReturnDto>> UpdateItemQuantity(string cartId, int productId, [FromBody] UpdateCartItemDto dto)
        {
            var cart = await _cartService.UpdateItemQuantityAsync(cartId, productId, dto.Quantity);
            if (cart == null) return BadRequest("Could not update item quantity");

            return Ok(cart.MapToDto());
        }

        [HttpDelete("{cartId}/items/{productId}")]
        public async Task<ActionResult<CartToReturnDto>> RemoveItemFromCart(string cartId, int productId)
        {
            var cart = await _cartService.RemoveItemAsync(cartId, productId);
            if (cart == null) return NotFound("Item or cart not found");

            return Ok(cart.MapToDto());
        }

        [HttpDelete("{cartId}")]
        public async Task<ActionResult<CartToReturnDto>> ClearCart(string cartId)
        {
            var cart = await _cartService.ClearCartAsync(cartId);
            if (cart == null) return NotFound("Cart not found");

            return Ok(cart.MapToDto());
        }
    }
}

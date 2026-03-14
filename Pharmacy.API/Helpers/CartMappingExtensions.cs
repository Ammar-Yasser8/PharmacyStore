using Pharmacy.API.Dtos.CartDtos;
using Pharmacy.Domain.Entities;
using System.Linq;

namespace Pharmacy.API.Helpers
{
    public static class CartMappingExtensions
    {
        public static CartToReturnDto MapToDto(this Cart cart)
        {
            var cartDto = new CartToReturnDto
            {
                Id = cart.Id,
                Items = cart.Items.Select(item => new CartItemToReturnDto
                {
                    ProductId = item.ProductId,
                    ProductName = item.Product.Name,
                    ImageUrl = item.Product.ImageUrl ?? string.Empty,
                    UnitPrice = item.Product.Price,
                    Quantity = item.Quantity,
                    Total = item.Quantity * item.Product.Price
                }).ToList()
            };

            cartDto.Subtotal = cartDto.Items.Sum(i => i.Total);
            return cartDto;
        }
    }
}

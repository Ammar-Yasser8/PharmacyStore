using System.Collections.Generic;

namespace Pharmacy.API.Dtos.CartDtos
{
    public class CartToReturnDto
    {
        public string Id { get; set; } = string.Empty;
        public IReadOnlyList<CartItemToReturnDto> Items { get; set; } = new List<CartItemToReturnDto>();
        public decimal Subtotal { get; set; }
    }
}

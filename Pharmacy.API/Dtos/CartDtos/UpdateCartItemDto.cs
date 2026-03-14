using System.ComponentModel.DataAnnotations;

namespace Pharmacy.API.Dtos.CartDtos
{
    public class UpdateCartItemDto
    {
        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Quantity must be at least 0")]
        public int Quantity { get; set; }
    }
}

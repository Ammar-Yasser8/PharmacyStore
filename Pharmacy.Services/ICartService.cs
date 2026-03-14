using Pharmacy.Services.Dtos.CartDtos;
using System.Threading.Tasks;

namespace Pharmacy.Services
{
    public interface ICartService
    {
        Task<CartToReturnDto?> GetCartAsync(string cartId, string? userId = null);
        Task<CartToReturnDto?> GetCartByUserIdAsync(string userId);
        Task<CartToReturnDto?> AddItemAsync(string cartId, int productId, int quantity, string? userId = null);
        Task<CartToReturnDto?> UpdateItemQuantityAsync(string cartId, int productId, int quantity, string? userId = null);
        Task<CartToReturnDto?> RemoveItemAsync(string cartId, int productId, string? userId = null);
        Task<CartToReturnDto?> ClearCartAsync(string cartId, string? userId = null);
        Task AssignCartToUserAsync(string cartId, string userId);
    }
}

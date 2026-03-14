using Pharmacy.Services.Dtos.CartDtos;
using System.Threading.Tasks;

namespace Pharmacy.Services
{
    public interface ICartService
    {
        Task<CartToReturnDto?> GetCartAsync(string cartId);
        Task<CartToReturnDto?> AddItemAsync(string cartId, int productId, int quantity);
        Task<CartToReturnDto?> UpdateItemQuantityAsync(string cartId, int productId, int quantity);
        Task<CartToReturnDto?> RemoveItemAsync(string cartId, int productId);
        Task<CartToReturnDto?> ClearCartAsync(string cartId);
    }
}

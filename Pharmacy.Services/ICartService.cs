using Pharmacy.Domain.Entities;
using System.Threading.Tasks;

namespace Pharmacy.Services
{
    public interface ICartService
    {
        Task<Cart?> GetCartAsync(string cartId);
        Task<Cart?> AddItemAsync(string cartId, int productId, int quantity);
        Task<Cart?> UpdateItemQuantityAsync(string cartId, int productId, int quantity);
        Task<Cart?> RemoveItemAsync(string cartId, int productId);
        Task<Cart?> ClearCartAsync(string cartId);
    }
}

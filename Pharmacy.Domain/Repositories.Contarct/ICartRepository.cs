using Pharmacy.Domain.Entities;
using System.Threading.Tasks;

namespace Pharmacy.Domain.Repositories.Contarct
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(string cartId);
        Task<Cart?> GetCartByUserIdAsync(string userId);
        Task AddCartAsync(Cart cart);
        Task<int> SaveChangesAsync();
    }
}

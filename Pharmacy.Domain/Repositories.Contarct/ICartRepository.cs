using Pharmacy.Domain.Entities;
using System.Threading.Tasks;

namespace Pharmacy.Domain.Repositories.Contarct
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(string cartId);
        Task AddCartAsync(Cart cart);
        Task<int> SaveChangesAsync();
    }
}

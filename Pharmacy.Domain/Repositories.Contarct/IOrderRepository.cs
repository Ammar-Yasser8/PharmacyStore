using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy.Domain.Entities.OrderAggregate;

namespace Pharmacy.Domain.Repositories.Contarct
{
    public interface IOrderRepository
    {
        Task AddAsync(Order order);
        Task<IReadOnlyList<Order>> GetOrdersForUserAsync(string buyerId);
        Task<Order?> GetOrderByIdAsync(int orderId, string buyerId);
        
        // Admin Methods
        Task<IReadOnlyList<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int orderId);

        Task<int> SaveChangesAsync();
    }
}

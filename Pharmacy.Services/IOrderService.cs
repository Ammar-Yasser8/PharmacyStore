using System.Collections.Generic;
using System.Threading.Tasks;
using Pharmacy.Services.Dtos.OrderDtos;

namespace Pharmacy.Services
{
    public interface IOrderService
    {
        Task<OrderToReturnDto> CreateOrderAsync(string buyerId, string buyerName, CreateOrderDto dto);
        Task<IReadOnlyList<OrderToReturnDto>> GetOrdersForUserAsync(string buyerId);
        Task<OrderToReturnDto?> GetOrderByIdAsync(int orderId, string buyerId);
        
        // Admin Methods
        Task<IReadOnlyList<OrderToReturnDto>> GetAllOrdersAsync();
        Task<OrderToReturnDto?> GetOrderByIdAsync(int orderId);
        
        // Status Update
        Task<OrderToReturnDto?> UpdateOrderStatusAsync(int orderId, string status);
    }
}

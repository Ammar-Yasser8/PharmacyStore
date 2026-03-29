using Pharmacy.Services.Dtos.OrderDtos;

namespace Pharmacy.Services
{
    public interface IEmailService
    {
        Task SendOrderCreatedToAdminAsync(OrderToReturnDto order);
        Task SendOrderCreatedToUserAsync(OrderToReturnDto order);
        Task SendOrderConfirmedToUserAsync(OrderToReturnDto order);
        Task SendOrderShippedToUserAsync(OrderToReturnDto order);
        Task SendOrderDeliveredToUserAsync(OrderToReturnDto order);
        Task SendOrderCancelledToUserAsync(OrderToReturnDto order);
    }
}

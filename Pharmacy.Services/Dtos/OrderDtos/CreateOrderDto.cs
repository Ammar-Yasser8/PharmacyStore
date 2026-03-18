namespace Pharmacy.Services.Dtos.OrderDtos
{
    public class CreateOrderDto
    {
        public string CartId { get; set; } = string.Empty;
        public OrderAddressDto ShippingAddress { get; set; } = null!;
        public string BuyerEmail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
    }
}

using System;
using System.Collections.Generic;

namespace Pharmacy.Services.Dtos.OrderDtos
{
    public class OrderToReturnDto
    {
        public int Id { get; set; }
        public string BuyerId { get; set; } = string.Empty;
        public string BuyerName { get; set; } = string.Empty;
        public string BuyerEmail { get; set; } = string.Empty;
        public string PhoneNumber { get; set; } = string.Empty;
        public DateTimeOffset OrderDate { get; set; }
        public string Status { get; set; } = string.Empty;
        public OrderAddressDto ShipToAddress { get; set; } = null!;
        public ICollection<OrderItemDto> OrderItems { get; set; } = new List<OrderItemDto>();
        public decimal Subtotal { get; set; }
        public decimal ShippingFee { get; set; }
        public decimal Total { get; set; }
    }
}

namespace Pharmacy.Services.Dtos.OrderDtos
{
    public class OrderAddressDto
    {
        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}

namespace Pharmacy.Domain.Entities.OrderAggregate
{
    public class OrderAddress
    {
        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = string.Empty;
        public string Country { get; set; } = string.Empty;
    }
}

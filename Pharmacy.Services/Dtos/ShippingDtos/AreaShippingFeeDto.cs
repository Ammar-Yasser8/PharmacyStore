namespace Pharmacy.Services.Dtos.ShippingDtos
{
    public class AreaShippingFeeDto
    {
        public string Area { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

namespace Pharmacy.Domain.Entities
{
    public class AreaShippingFee
    {
        public int Id { get; set; }
        public string Area { get; set; } = string.Empty;
        public decimal Fee { get; set; }
        public bool IsActive { get; set; } = true;
    }
}

namespace Pharmacy.Domain.Entities
{
    public class Address
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = "Hurghada";
        public string Country { get; set; } = "Egypt";
        public string AppUserId { get; set; } = string.Empty;
        public AppUser AppUser { get; set; } = null!;

    }
}
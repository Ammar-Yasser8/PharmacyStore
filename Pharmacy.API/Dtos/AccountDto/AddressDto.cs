namespace Pharmacy.API.Dtos.AccountDto
{
    public class AddressDto
    {
        public int Id { get; set; }
        public string Street { get; set; } = string.Empty;
        public string Area { get; set; } = string.Empty;
        public string City { get; set; } = "Hurghada";
        public string Country { get; set; } = "Egypt";
    }
}

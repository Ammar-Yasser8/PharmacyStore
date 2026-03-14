namespace Pharmacy.API.Dtos.AccountDto
{
    public class LoginDto
    {
        public string Email { get; set; }
        public string Password { get; set; }
        public string? CartId { get; set; }
    }
}

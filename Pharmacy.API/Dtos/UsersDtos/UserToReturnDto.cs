namespace Pharmacy.API.Dtos.UsersDtos
{
    public class UserToReturnDto
    {
        public string Id { get; set; } = null!;
        public string DisplayName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string? PhoneNumber { get; set; }
        public IList<string> Roles { get; set; } = new List<string>();
    }
}

namespace Pharmacy.API.Dtos
{
    public class CategoryToReturnDto
    {
        public int Id { get; set; }
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameRu { get; set; } = string.Empty;
        public string ImageUrl { get; set; } = string.Empty;
    }
}

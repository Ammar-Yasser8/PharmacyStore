using Microsoft.AspNetCore.Http;

namespace Pharmacy.API.Dtos
{
    public class CategoryUpdateDto
    {
        public string NameAr { get; set; } = string.Empty;
        public string NameEn { get; set; } = string.Empty;
        public string NameRu { get; set; } = string.Empty;
        public IFormFile? Image { get; set; }
    }
}

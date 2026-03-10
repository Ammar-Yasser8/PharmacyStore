using Microsoft.AspNetCore.Http;

namespace Pharmacy.Services
{
    public interface IImageService
    {
        Task<string> UploadImageAsync(IFormFile image, string folderName);
        void DeleteImage(string imageUrl);
    }
}

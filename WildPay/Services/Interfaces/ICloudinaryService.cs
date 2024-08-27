using CloudinaryDotNet.Actions;

namespace WildPay.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<List<string>> UploadImageCloudinaryAsync(IFormFile image);
        Task DeleteImageCloudinaryAsync(string cloudinaryPublicID);
        ImageUploadParams CreateParams(string fileName, IFormFile image, int width, int height, string folder);
    }
}

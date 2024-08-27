using CloudinaryDotNet.Actions;

namespace WildPay.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<List<string>> UploadImageCloudinaryAsync(IFormFile image, bool isProfilePic = true);
        Task DeleteImageCloudinaryAsync(string cloudinaryPublicID);
    }
}
namespace WildPay.Services.Interfaces
{
    public interface ICloudinaryService
    {
        Task<List<string>> UploadImageCloudinaryAsync(IFormFile image);
        Task DeleteImageCloudinaryAsync(string cloudinaryPublicID);
    }
}

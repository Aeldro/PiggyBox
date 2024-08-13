using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using WildPay.Exceptions;
using WildPay.Services.Interfaces;

namespace WildPay.Services
{
    public class CloudinaryService : ICloudinaryService
    {
        private readonly Cloudinary _cloudinary;

        public CloudinaryService(Cloudinary cloudinary)
        {
            _cloudinary = cloudinary;
        }

        public async Task<List<string>> UploadImageCloudinaryAsync(IFormFile image)
        {
            // unique file name
            // otherwise it can be overwrite by a file of the same name
            string fileName = Guid.NewGuid().ToString() + "_" + image.FileName;

            // Transformation: resizes the image in 150x150
            // crop = image fills all the dimensions, respecting the aspect ratio
            // gravity = centers the image on the face if it is found
            var uploadParams = new ImageUploadParams()
            {
                File = new FileDescription(fileName, image.OpenReadStream()),
                Transformation = new Transformation().Width(150).Height(150).Crop("fill").Gravity("face"),
                UseFilename = true,
                UniqueFilename = false,
                Overwrite = true,
                Folder = "PiggyBox_profile_pic"
            };

            // upload(file, options)
            var uploadResult = await _cloudinary.UploadAsync(uploadParams);

            if (uploadResult.StatusCode == System.Net.HttpStatusCode.OK)
            {
                return new List<string> { uploadResult.SecureUrl.ToString(), uploadResult.PublicId };
            }
            else
            {
                throw new CloudinaryResponseNotOkException("Fail to upload image on Cloudinary.");
            }
        }

        public async Task DeleteImageCloudinaryAsync(string cloudinaryPublicID)
        {
            var deletionParams = new DeletionParams(cloudinaryPublicID);

            var deletionResult = await _cloudinary.DestroyAsync(deletionParams);

            if (!(deletionResult.StatusCode == System.Net.HttpStatusCode.OK))
            {
                throw new CloudinaryResponseNotOkException("Fail to delete image of Cloudinary");
            }
        }
    }
}

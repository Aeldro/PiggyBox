using Microsoft.Data.SqlClient;
using WildPay.Exceptions;
using WildPay.Models.ViewModels;
using WildPay.Repositories.Interfaces;
using WildPay.Services.Interfaces;

namespace WildPay.Services
{
    public class GroupService : IGroupService
    {
        public readonly IGroupRepository _groupRepository;
        private readonly ICloudinaryService _cloudinaryService;

        public GroupService(IGroupRepository groupRepository, ICloudinaryService cloudinaryService)
        {
            _groupRepository = groupRepository;
            _cloudinaryService = cloudinaryService;
        }

        public async Task AddGroupAsync(AddGroup groupToAdd, string userId)
        {
            try
            {
                List<string> imageInfos = [null, null];

                if (groupToAdd.Image != null && groupToAdd.Image.Length > 0)
                {
                    imageInfos = await _cloudinaryService.UploadImageCloudinaryAsync(groupToAdd.Image);
                }

                await _groupRepository.AddGroupAsync(groupToAdd.Name, imageInfos[0], imageInfos[1], userId);
            }
            catch (SqlException)
            {
                throw new DatabaseException();
            }
            catch (CloudinaryResponseNotOkException)
            {
                throw new Exception("Fail to upload image on cloudinary");
            }
        }

        public async Task EditGroupAsync()
        {

        }
    }
}

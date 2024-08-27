using Microsoft.Data.SqlClient;
using WildPay.Exceptions;
using WildPay.Models.Entities;
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
                    imageInfos = await _cloudinaryService.UploadImageCloudinaryAsync(groupToAdd.Image, false);
                }

                await _groupRepository.AddGroupAsync(groupToAdd.Name, imageInfos[0], imageInfos[1], userId);
            }
            catch (SqlException)
            {
                throw new DatabaseException();
            }
            catch (CloudinaryResponseNotOkException)
            {
                throw new CloudinaryResponseNotOkException("Fail to upload image on cloudinary");
            }
        }

        public async Task EditGroupAsync(AddGroup model)
        {
            try
            {
                Group? actualGroup = await _groupRepository.GetGroupByIdAsync(model.GroupId);

                if (actualGroup == null) throw new NullException();

                actualGroup.Name = model.Name;

                if (model.Image != null && model.Image.Length > 0)
                {
                    if (!string.IsNullOrEmpty(actualGroup.GroupImageUrl))
                    {
                        await _cloudinaryService.DeleteImageCloudinaryAsync(actualGroup.GroupImagePublicId);
                    }

                    List<string> ImageInfos = await _cloudinaryService.UploadImageCloudinaryAsync(model.Image, false);

                    actualGroup.GroupImageUrl = ImageInfos[0];
                    actualGroup.GroupImagePublicId = ImageInfos[1];
                }

                _groupRepository.EditGroupAsync(actualGroup);
            }
            catch (SqlException)
            {
                throw new DatabaseException();
            }
            catch (NullException)
            {
                throw new NullException();
            }
            catch (CloudinaryResponseNotOkException)
            {
                throw new CloudinaryResponseNotOkException("Fail to upload image on cloudinary");
            }
        }
    }
}

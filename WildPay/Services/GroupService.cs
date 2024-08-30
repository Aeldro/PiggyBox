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
        private readonly ILogger<IGroupService> _logger;

        public GroupService(IGroupRepository groupRepository, ICloudinaryService cloudinaryService, ILogger<IGroupService> logger)
        {
            _groupRepository = groupRepository;
            _cloudinaryService = cloudinaryService;
            _logger = logger;
        }

        public async Task AddGroupAsync(AddGroup groupToAdd, string userId)
        {
            try
            {
                List<string> imageInfos = [null, null];

                if (groupToAdd.Image != null && groupToAdd.Image.Length > 0)
                {
                    try
                    {
                        imageInfos = await _cloudinaryService.UploadImageCloudinaryAsync(groupToAdd.Image, false);
                    }
                    catch (CloudinaryResponseNotOkException ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                }

                await _groupRepository.AddGroupAsync(groupToAdd.Name, imageInfos[0], imageInfos[1], userId);
            }
            catch (SqlException)
            {
                throw new DatabaseException();
            }
        }

        public async Task EditGroupAsync(AddGroup model)
        {
            try
            {
                Group? actualGroup = await _groupRepository.GetGroupByIdAsync(model.GroupId);

                if (actualGroup == null) throw new NullException();

                Group groupToUpdate = new Group();

                groupToUpdate.Id = model.GroupId;
                groupToUpdate.Name = model.Name;

                if (model.Image != null && model.Image.Length > 0)
                {
                    if (!string.IsNullOrEmpty(actualGroup.GroupImageUrl))
                    {
                        try
                        {
                            await _cloudinaryService.DeleteImageCloudinaryAsync(actualGroup.GroupImagePublicId);
                        }
                        catch (CloudinaryResponseNotOkException ex)
                        {
                            _logger.LogInformation(ex.Message);
                        }
                    }

                    try
                    {
                        List<string> ImageInfos = await _cloudinaryService.UploadImageCloudinaryAsync(model.Image, false);
                        groupToUpdate.GroupImageUrl = ImageInfos[0];
                        groupToUpdate.GroupImagePublicId = ImageInfos[1];
                    }
                    catch (CloudinaryResponseNotOkException ex)
                    {
                        _logger.LogInformation(ex.Message);
                    }
                }

                if (model.Image == null && actualGroup.GroupImageUrl != null)
                {
                    groupToUpdate.GroupImageUrl = actualGroup.GroupImageUrl;
                    groupToUpdate.GroupImagePublicId = actualGroup.GroupImageUrl;
                }

                await _groupRepository.EditGroupAsync(groupToUpdate);
            }
            catch (SqlException)
            {
                throw new DatabaseException();
            }
            catch (NullException)
            {
                throw new NullException();
            }
        }

        public async Task DeleteGroupImageAsync(Group group)
        {
            if (!string.IsNullOrEmpty(group.GroupImageUrl))
            {
                try
                {
                    await _cloudinaryService.DeleteImageCloudinaryAsync(group.GroupImagePublicId);
                }
                catch (CloudinaryResponseNotOkException ex)
                {
                    _logger.LogInformation(ex.Message);
                }

                Group groupToUpdate = new Group();

                groupToUpdate.Id = group.Id;
                groupToUpdate.Name = group.Name;
                groupToUpdate.GroupImagePublicId = null;
                groupToUpdate.GroupImageUrl = null;

                await _groupRepository.EditGroupAsync(groupToUpdate);
            }
        }
    }
}
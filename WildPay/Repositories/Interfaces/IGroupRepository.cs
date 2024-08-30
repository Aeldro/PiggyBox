using WildPay.Models.Entities;

namespace WildPay.Repositories.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetGroupsAsync(string userId);
        Task<Group?> GetGroupByIdAsync(int groupId);
        Task EditGroupAsync(Group group);
        Task AddGroupAsync(string name, string Image, string userId);
        Task<bool> AddMemberToGroupAsync(Group group, string email);
        Task DeleteMemberFromGroupAsync(Group group, string userId);
        Task<bool> DeleteGroupAsync(int groupId);
    }
}

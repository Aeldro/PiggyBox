using WildPay.Models.Entities;

namespace WildPay.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetGroupsAsync(string userId);
        Task<Group?> GetGroupByIdAsync(int groupId);
        Task EditGroupAsync(Group group);
        Task AddGroupAsync(string name, string Image, string userId);
        Task AddMemberToGroupAsync(Group group, string userId);
        Task DeleteMemberFromGroupAsync(Group group, ApplicationUser member);
        Task<bool> DeleteGroupAsync(int groupId);
    }
}

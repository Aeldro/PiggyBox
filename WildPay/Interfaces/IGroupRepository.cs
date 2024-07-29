using WildPay.Models.Entities;

namespace WildPay.Interfaces
{
    public interface IGroupRepository
    {
        Task<List<Group>> GetGroupsAsync();
        Task<Group?> GetGroupByIdAsync(int groupId);
        Task EditGroupAsync(Group group);
        Task AddGroupAsync(string name, string Image);
        Task<bool> DeleteGroupAsync(int groupId);
    }
}

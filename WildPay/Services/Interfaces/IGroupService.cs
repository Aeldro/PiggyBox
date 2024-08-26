using WildPay.Models.ViewModels;

namespace WildPay.Services.Interfaces
{
    public interface IGroupService
    {
        Task AddGroupAsync(AddGroup groupToAdd, string userId);
    }
}

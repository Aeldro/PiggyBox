using Microsoft.AspNetCore.Mvc.Rendering;
using WildPay.Models.Entities;

namespace WildPay.Services.Interfaces
{
    public interface IDropDownService
    {
        Task<List<SelectListItem>> GetDropDownGroupMembers(Group group);
        Task<List<SelectListItem>> GetDropDownGroupCategories(Group group);
    }
}

using Microsoft.AspNetCore.Mvc.Rendering;
using WildPay.Models.Entities;
using WildPay.Services.Interfaces;

namespace WildPay.Services
{
    public class DropDownService : IDropDownService
    {
        public async Task<List<SelectListItem>> GetDropDownGroupMembers(Group group)
        {
            List<SelectListItem> groupUsers = new List<SelectListItem>();
            foreach (ApplicationUser member in group.ApplicationUsers)
            {
                groupUsers.Add(new SelectListItem { Value = member.Id, Text = $"{member.Firstname} {member.Lastname}" });
            }
            return groupUsers;
        }

        public async Task<List<SelectListItem>> GetDropDownGroupCategories(Group group)
        {
            List<SelectListItem> groupCategories = new List<SelectListItem>();
            foreach (Category category in group.Categories)
            {
                groupCategories.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Name });
            }
            return groupCategories;
        }
    }
}

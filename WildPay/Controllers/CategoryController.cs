using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WildPay.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Controllers
{
    public class CategoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupRepository _groupRepository;

        public CategoryController(UserManager<ApplicationUser> userManager, IGroupRepository groupRepository)
        {
            _userManager = userManager;
            _groupRepository = groupRepository;
        }

        [HttpGet]
        public async Task<IActionResult> ListGroupCategories(int Id)
        {
            //Get the group
            Group? group = await _groupRepository.GetGroupByIdAsync(Id);

            //Return not found if no group is found
            if (group == null) { return NotFound(); }

            //Verify if the User belongs to the group, else we block the access
            if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

            return View(group);
        }
        
        [HttpPost]
        public async Task<IActionResult> AddCategoryToGroup(Category category)
        {
            string ? userId = _userManager.GetUserId(User);
            if (category == null)  return NotFound(); 

            await _groupRepository.AddCate
            

        }
    }
}

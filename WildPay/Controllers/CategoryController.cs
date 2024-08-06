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
        private readonly ICategoryRepository _categoryRepository;
        public CategoryController(UserManager<ApplicationUser> userManager, IGroupRepository groupRepository, ICategoryRepository categoryRepository)
        {
            _userManager = userManager;
            _groupRepository = groupRepository;
            _categoryRepository = categoryRepository;
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
        [HttpGet]
        
        public IActionResult AddCategory(int Id)
        {
            Category category = new Category
            {
                GroupId = Id
            };

            return View(category);
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            string? userId = _userManager.GetUserId(User);
            if (category == null) return NotFound();

            Group? group = await _groupRepository.GetGroupByIdAsync(category.GroupId);

            category.Group = group;

            await _categoryRepository.AddCategoryAsync(category);

            return RedirectToAction(actionName: "ListGroupCategories", controllerName: "Category", new { Id = category.GroupId });
        }
        [HttpGet]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
                return View(category);
            
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int id, Category category)
        {

            await _categoryRepository.DeleteCategoryAsync(id);

            return RedirectToAction(actionName:"ListGroupCategories", controllerName:"Category");
        }
    }
}

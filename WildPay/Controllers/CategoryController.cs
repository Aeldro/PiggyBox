using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WildPay.Interfaces;
using WildPay.Models.Entities;
using System.Threading.Tasks;
using WildPay.Models.ViewModels;
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
            var existingCategory = await _categoryRepository.GetCategoryByIdAsync(id);
            if (existingCategory == null)
            {
                return NotFound();
            }
            await _categoryRepository.DeleteCategoryAsync(id);
            return RedirectToAction("ListGroupCategories", new { Id = existingCategory.GroupId });
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int id)
        {
            var category = await _categoryRepository.GetCategoryByIdAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        //[HttpPost]

        //public async Task<IActionResult> UpdateCategory(UpdateCategoryModel modelUpdated)
        //{
        //    if (ModelState.IsValid) return View(modelUpdated);
        //    Category? categoryUpdated = modelUpdated.CategoryToUpdate;
        //    if (categoryUpdated == null) return NotFound();
        //    await _categoryRepository.UpdateCategoryAsync(categoryUpdated);
        //    return RedirectToAction(actionName: "GetCategory", controllerName: "Category", new { categoryUpdated.Id });

        //}
        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            if (!ModelState.IsValid) return View(category);

            await _categoryRepository.UpdateCategoryAsync(category);

            return RedirectToAction("ListGroupCategories", new { Id = category.GroupId });
        }
    }
}

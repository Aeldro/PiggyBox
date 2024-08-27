using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WildPay.Exceptions;
using WildPay.Models.Entities;
using WildPay.Services;
using WildPay.Services.Interfaces;
using WildPay.Repositories.Interfaces;

namespace WildPay.Controllers
{

    // methods are only accessible if the user is connected
    [Authorize]
    public class CategoryController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly IGroupRepository _groupRepository;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IVerificationService _verificationService;
        public CategoryController(UserManager<ApplicationUser> userManager, IGroupRepository groupRepository, ICategoryRepository categoryRepository, IVerificationService verificationService)
        {
            _userManager = userManager;
            _groupRepository = groupRepository;
            _categoryRepository = categoryRepository;
            _verificationService = verificationService;
        }

        [HttpGet]
        public async Task<IActionResult> ListGroupCategories(int Id)
        {
            try
            {
                //Get the group
                Group? group = await _groupRepository.GetGroupByIdAsync(Id);

                //Verify if the User belongs to the group, else we block the access
                bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
                if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

                return View(group);
            }
            catch (DatabaseException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
            catch (NullException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> AddCategory(int Id)
        {
            try
            {
                ViewBag.Group = await _groupRepository.GetGroupByIdAsync(Id);

                //Verify if the User belongs to the group, else we block the access
                bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), ViewBag.Group);
                if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

                Category category = new Category
                {
                    GroupId = Id
                };


                return View(category);
            }
            catch (DatabaseException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
            catch (NullException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddCategory(Category category)
        {
            try
            {
                //string? userId = _userManager.GetUserId(User);
                if (!ModelState.IsValid)
                {
                    ViewBag.Group = await _groupRepository.GetGroupByIdAsync(category.GroupId);
                    return View(category);
                }
                if (category == null) return NotFound();

                Group? group = await _groupRepository.GetGroupByIdAsync(category.GroupId);

                //Verify if the User belongs to the group, else we block the access
                bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
                if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

                category.Group = group;

                await _categoryRepository.AddCategoryAsync(category);

                return RedirectToAction(actionName: "ListGroupCategories", controllerName: "Category", new { Id = category.GroupId });
            }
            catch (DatabaseException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
            catch (NullException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
        }
        
        [HttpPost]
        public async Task<IActionResult> DeleteCategory(int Id, Category category)
        {
            try
            {
                var existingCategory = await _categoryRepository.GetCategoryByIdAsync(Id);

                Group? group = await _groupRepository.GetGroupByIdAsync(existingCategory.GroupId);

                //Verify if the User belongs to the group, else we block the access
                bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
                if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

                if (existingCategory == null)
                {
                    return NotFound();
                }
                await _categoryRepository.DeleteCategoryAsync(Id);
                return RedirectToAction("ListGroupCategories", new { Id = existingCategory.GroupId });
            }
            catch (DatabaseException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
            catch (NullException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> UpdateCategory(int GroupId, int CategoryId)
        {
            try
            {
                ViewBag.Group = await _groupRepository.GetGroupByIdAsync(GroupId);

                //Verify if the User belongs to the group, else we block the access
                bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), ViewBag.Group);
                if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

                var category = await _categoryRepository.GetCategoryByIdAsync(CategoryId);
                if (category == null)
                {
                    return NotFound();
                }

                return View(category);
            }
            catch (DatabaseException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
            catch (NullException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCategory(Category category)
        {
            try
            {
                Group? group = await _groupRepository.GetGroupByIdAsync(category.GroupId);

                //Verify if the User belongs to the group, else we block the access
                bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
                if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

                if (!ModelState.IsValid)
                {
                    ViewBag.Group = await _groupRepository.GetGroupByIdAsync(category.GroupId);
                    return View(category);
                }

                await _categoryRepository.UpdateCategoryAsync(category);

                return RedirectToAction("ListGroupCategories", new { Id = category.GroupId });
            }
            catch (DatabaseException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
            catch (NullException ex)
            {
                return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
            }
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using WildPay.Exceptions;
using WildPay.Interfaces;
using WildPay.Models;
using WildPay.Models.Entities;
using WildPay.Models.ViewModels;
using WildPay.Repositories.Interfaces;
using WildPay.Services.Interfaces;

namespace WildPay.Controllers;

// methods are only accessible if the user is connected
[Authorize]
public class ExpenditureController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExpenditureRepository _expenditureRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBalanceService _balanceService;
    private readonly IExpenditureService _expenditureService;
    private readonly IDropDownService _dropDownService;
    private readonly IVerificationService _verificationService;

    public ExpenditureController(UserManager<ApplicationUser> userManager, IExpenditureRepository expenditureRepository, IGroupRepository groupRepository, IBalanceService balanceService, ICategoryRepository categoryRepository, IExpenditureService expenditureService, IDropDownService dropDownService, IVerificationService verificationService)
    {
        _userManager = userManager;
        _expenditureRepository = expenditureRepository;
        _groupRepository = groupRepository;
        _categoryRepository = categoryRepository;
        _balanceService = balanceService;
        _expenditureService = expenditureService;
        _dropDownService = dropDownService;
        _verificationService = verificationService;
    }

    // READ
    async public Task<IActionResult> ListGroupExpenditures(int Id)
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

    async public Task<IActionResult> ListGroupBalances(int Id)
    {
        try
        {
            //Get the group
            Group? group = await _groupRepository.GetGroupByIdAsync(Id);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            //Init GroupBalance instance
            GroupBalance groupBalance = new GroupBalance();

            groupBalance.Group = group;
            groupBalance.TotalAmount = group.Expenditures.Sum(el => el.Amount);

            // Calculate the amount paied or due by each members of the group
            Dictionary<ApplicationUser, double> membersBalance = await _balanceService.CalculateMembersBalance(group);

            membersBalance = membersBalance.OrderByDescending(el => el.Value).ToDictionary(el => el.Key, el => el.Value);
            groupBalance.UsersBalance = membersBalance;

            // Calculate the debts: creditor-debitor for each due amount
            groupBalance = _balanceService.CalculateDebtsList(groupBalance, group);

            // if no payer or no contributors are found in one expenditure or more
            if (group.Expenditures.Any(el => el.PayerId is null) || group.Expenditures.Any(el => el.Payer is null) ||
                group.Expenditures.Any(el => el.RefundContributors == null) ||
                group.Expenditures.Any(el => el.RefundContributors.Count() == 0))
            {
                groupBalance.Message = "Attention ! Les dépenses qui n'ont pas de payeur ou de contributeurs n'ont pas été prises compte.";
            }

            if (groupBalance.Debts.Count > 0 && groupBalance.Message == "")
            {
                groupBalance.Message = "Toutes les dépenses ont été prises en compte.";
            }
            else if (groupBalance.Debts.Count == 0 && groupBalance.Message == "")
            {
                groupBalance.Message = "Aucun remboursement à effectuer.";
            }

            return View(groupBalance);
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
    public async Task<IActionResult> UpdateExpenditure(int groupId, int expenditureId)
    {
        try
        {
            //Get the group
            Group? group = await _groupRepository.GetGroupByIdAsync(groupId);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            //Get the expenditure
            Expenditure? expenditure = await _expenditureRepository.GetExpenditureByIdAsync(expenditureId);

            //Return not found if no expenditure is found
            if (expenditure == null) { return NotFound(); }

            var groupUsers = new List<SelectListItem>();
            foreach (ApplicationUser member in group.ApplicationUsers)
            {
                groupUsers.Add(new SelectListItem { Value = member.Id, Text = $"{member.Firstname} {member.Lastname}" });
            }

            var groupCategories = new List<SelectListItem>();
            foreach (Category category in group.Categories)
            {
                groupCategories.Add(new SelectListItem { Value = category.Id.ToString(), Text = category.Name });
            }

            ViewBag.Group = group;
            ViewBag.GroupCategoriesSelects = await _dropDownService.GetDropDownGroupCategories(group);
            ViewBag.GroupUsersSelects = await _dropDownService.GetDropDownGroupMembers(group);

            return View(expenditure);
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
    public async Task<IActionResult> UpdateExpenditure(Expenditure expenditure, int groupId, string[] RefundContributors)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                //Get the group
                Group? group = await _groupRepository.GetGroupByIdAsync(groupId);

                //Verify if the User belongs to the group, else we block the access
                bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
                if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

                ViewBag.Group = group;
                ViewBag.GroupCategoriesSelects = await _dropDownService.GetDropDownGroupCategories(group);
                ViewBag.GroupUsersSelects = await _dropDownService.GetDropDownGroupMembers(group);

                //Rebuild the expenditure contributors
                Expenditure initialExpenditure = await _expenditureRepository.GetExpenditureByIdAsync(expenditure.Id);
                expenditure.RefundContributors = initialExpenditure.RefundContributors;

                return View(expenditure);
            }

            if (expenditure.PayerId is not null) expenditure.Payer = await _userManager.FindByIdAsync(expenditure.PayerId);
            else expenditure.Payer = null;
            if (expenditure.CategoryId is not null) expenditure.Category = await _categoryRepository.GetCategoryByIdAsync(Convert.ToInt32(expenditure.CategoryId));
            else expenditure.Category = null;

            expenditure.RefundContributors.Clear();
            foreach (string memberId in RefundContributors)
            {
                expenditure.RefundContributors.Add(await _userManager.FindByIdAsync(memberId));
            }

            await _expenditureRepository.EditExpenditureAsync(expenditure);
            return RedirectToAction(actionName: "ListGroupExpenditures", controllerName: "Expenditure", new { id = groupId });
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

    // CREATE
    [HttpGet]
    public async Task<IActionResult> AddExpenditure(int Id)
    {
        try
        {
            Group? group = await _groupRepository.GetGroupByIdAsync(Id);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            AddExpenditureInGroup model = await _expenditureService.AddExpenditure(Id); // returns a model to fetch in the View
            ViewBag.Group = await _groupRepository.GetGroupByIdAsync(model.GroupId);
            return View(model);
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

    // CREATE
    [HttpPost]
    public async Task<IActionResult> AddExpenditure(AddExpenditureInGroup model)
    {
        try
        {
            Group? group = await _groupRepository.GetGroupByIdAsync(model.GroupId);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            if (ModelState.IsValid)
            {
                await _expenditureService.AddExpenditure(model); // add the new Expenditure calling service
                return RedirectToAction(actionName: "ListGroupExpenditures", controllerName: "Expenditure", new { id = model.GroupId });
            }
            Group initialGroup = await _groupRepository.GetGroupByIdAsync(model.GroupId);
            model.UsersInGroup = initialGroup.ApplicationUsers;
            model.CategoriesInGroup = initialGroup.Categories;
            ViewBag.Group = initialGroup;
            return View(model);
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

    // DELETE
    [HttpGet]
    public async Task<IActionResult> DeleteExpenditure(int Id)
    {
        try
        {
            Expenditure expenditureToRemove = await _expenditureService.GetExpenditureById(Id);
            Group? group = await _groupRepository.GetGroupByIdAsync(expenditureToRemove.GroupId);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            return View(expenditureToRemove);
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

    // DELETE
    [HttpPost]
    public async Task<IActionResult> DeleteExpenditure(Expenditure expenditure)
    {
        try
        {
            Group? group = await _groupRepository.GetGroupByIdAsync(expenditure.GroupId);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            await _expenditureService.DeleteExpenditure(expenditure);
            return RedirectToAction(actionName: "ListGroupExpenditures", controllerName: "Expenditure", new { id = expenditure.GroupId }); ;
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
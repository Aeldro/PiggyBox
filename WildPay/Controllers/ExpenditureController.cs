using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.CodeAnalysis.Elfie.Serialization;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using NuGet.Protocol.Core.Types;
using WildPay.Interfaces;
using WildPay.Models;
using WildPay.Models.Entities;
using WildPay.Models.ViewModels;
using WildPay.Services.Interfaces;

namespace WildPay.Controllers;

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

    public ExpenditureController(UserManager<ApplicationUser> userManager, IExpenditureRepository expenditureRepository, IGroupRepository groupRepository, IBalanceService balanceService, ICategoryRepository categoryRepository, IExpenditureService expenditureService, IDropDownService dropDownService)
    {
        _userManager = userManager;
        _expenditureRepository = expenditureRepository;
        _groupRepository = groupRepository;
        _categoryRepository = categoryRepository;
        _balanceService = balanceService;
        _expenditureService = expenditureService;
        _dropDownService = dropDownService;
    }

    // READ
    async public Task<IActionResult> ListGroupExpenditures(int Id)
    {
        //Get the group
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        //Return not found if no group is found
        if (group == null) { return NotFound(); }

        //Verify if the User belongs to the group, else we block the access
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

        return View(group);
    }

    async public Task<IActionResult> ListGroupBalances(int Id)
    {
        //Get the group
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        //Return not found if no group is found
        if (group == null) { return NotFound(); }

        //Verify if the User belongs to the group, else we block the access
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

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

        if (group.Expenditures.Any(el => el.PayerId is null) || group.Expenditures.Any(el => el.Payer is null))
        {
            groupBalance.Message = "Attention ! Les dépenses qui n'ont pas de payeur n'ont pas été prises en compte.\nVérifiez les dépenses du groupe et ajoutez-y un payeur si vous voulez les inclure au calcul.\n";
        }

        // n'arrive pas à retrouver les refundcontributors de expenditures
        if (group.Expenditures.Any(el => el.RefundContributors == null) || group.Expenditures.Any(el => el.RefundContributors.Count() == 0))
        {
            groupBalance.Message = groupBalance.Message + "Attention ! Les dépenses qui n'ont pas de contributeurs n'ont pas été prises en compte.\nVérifiez les dépenses du groupe et ajoutez-y des contributeurs si vous voulez les inclure au calcul.";
        }

        if (groupBalance.Debts.Count > 0 && groupBalance.Message == "") groupBalance.Message = "Toutes les dépenses ont été prises en compte.";
        else if (groupBalance.Debts.Count == 0 && groupBalance.Message == "") groupBalance.Message = "Aucun remboursement à effectuer.";

        return View(groupBalance);
    }

    [HttpGet]
    public async Task<IActionResult> UpdateExpenditure(int groupId, int expenditureId)
    {
        //Get the group
        Group? group = await _groupRepository.GetGroupByIdAsync(groupId);

        //Return not found if no group is found
        if (group == null) { return NotFound(); }

        //Verify if the User belongs to the group, else we block the access
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

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

    [HttpPost]
    public async Task<IActionResult> UpdateExpenditure(Expenditure expenditure, int groupId, string[] RefundContributors)
    {
        if (!ModelState.IsValid)
        {
            //Get the group
            Group? group = await _groupRepository.GetGroupByIdAsync(groupId);

            //Return not found if no group is found
            if (group == null) { return NotFound(); }

            //Verify if the User belongs to the group, else we block the access
            if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

            ViewBag.Group = group;
            ViewBag.GroupCategoriesSelects = await _dropDownService.GetDropDownGroupCategories(group);
            ViewBag.GroupUsersSelects = await _dropDownService.GetDropDownGroupMembers(group);
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
        return RedirectToAction(actionName: "UpdateExpenditure", controllerName: "Expenditure", new { groupId = groupId, expenditureId = expenditure.Id });
    }
    
    // CREATE
    [HttpGet]
    public async Task<IActionResult> AddExpenditure(int Id)
    {
        AddExpenditureInGroup model = await _expenditureService.AddExpenditure(Id); // returns a model to fetch in the View
        return View(model);
    }
    
    // CREATE
    [HttpPost]
    public async Task<IActionResult> AddExpenditure(AddExpenditureInGroup model)
    {
        if (ModelState.IsValid)
        {
            await _expenditureService.AddExpenditure(model); // add the new Expenditure calling service
            return RedirectToAction(actionName: "ListGroupExpenditures", controllerName: "Expenditure", new {id = model.GroupId});
        }
        return RedirectToAction(actionName: "ListGroupExpenditures", controllerName: "Expenditure", new {id = model.GroupId});
    }
    
    // DELETE
    [HttpGet]
    public async Task<IActionResult> DeleteExpenditure(int Id)
    {
        Expenditure expenditureToRemove = await _expenditureRepository.GetExpenditureByIdAsync(Id);
        return View(expenditureToRemove);
    }
    
    // DELETE
    [HttpPost]
    public async Task<IActionResult> DeleteExpenditure(Expenditure expenditure)
    {
        int groupId = expenditure.GroupId;
        await _expenditureService.DeleteExpenditure(expenditure);
        return RedirectToAction(actionName: "ListGroupExpenditures", controllerName: "Expenditure", new { id = groupId });;
    }
}
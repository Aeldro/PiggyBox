using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
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
    private readonly IBalanceService _balanceService;
    private readonly IExpenditureService _expenditureService;
    

    public ExpenditureController(UserManager<ApplicationUser> userManager, IExpenditureRepository expenditureRepository, IGroupRepository groupRepository, IBalanceService balanceService, IExpenditureService expenditureService)
    {
        _userManager = userManager;
        _expenditureRepository = expenditureRepository;
        _groupRepository = groupRepository;
        _balanceService = balanceService;
        _expenditureService = expenditureService;
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

        Dictionary<ApplicationUser, double> membersBalance = await _balanceService.CalculateMembersBalance(group); //Calculate the balance of each member
        membersBalance = membersBalance.OrderByDescending(el => el.Value).ToDictionary(el => el.Key, el => el.Value);
        groupBalance.UsersBalance = membersBalance;
        groupBalance = await _balanceService.CalculateDebtsList(groupBalance, group); //Calculate who must pay who

        if (group.Expenditures.Any(el => el.PayerId is null) || group.Expenditures.Any(el => el.Payer is null)) groupBalance.Message = "Attention ! Les d�penses qui n'ont pas de payeur n'ont pas �t� prises en compte. V�rifiez les d�penses du groupe et ajoutez-y un payeur si vous voulez les inclure au calcul.";
        else if (groupBalance.Debts.Count > 0 && groupBalance.Message == "") groupBalance.Message = "Calcul effectu� avec succ�s.";
        else if (groupBalance.Debts.Count == 0 && groupBalance.Message == "") groupBalance.Message = "Aucun remboursement � effectuer.";

        return View(groupBalance);
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
        return View(model);
    }
}
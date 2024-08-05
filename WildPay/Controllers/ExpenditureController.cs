using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WildPay.Interfaces;
using WildPay.Models;
using WildPay.Models.Entities;
using WildPay.Services;

namespace WildPay.Controllers;

[Authorize]
public class ExpenditureController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExpenditureRepository _expenditureRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IBalanceService _balanceService;

    public ExpenditureController(UserManager<ApplicationUser> userManager, IExpenditureRepository expenditureRepository, IGroupRepository groupRepository, IBalanceService balanceService)
    {
        _userManager = userManager;
        _expenditureRepository = expenditureRepository;
        _groupRepository = groupRepository;
        _balanceService = balanceService;
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

        if (group.Expenditures.Any(el => el.PayerId is null) || group.Expenditures.Any(el => el.Payer is null)) groupBalance.Message = "Attention ! Les dépenses qui n'ont pas de payeur n'ont pas été prises en compte. Vérifiez les dépenses du groupe et ajoutez-y un payeur si vous voulez les inclure au calcul.";
        else if (groupBalance.Debts.Count > 0 && groupBalance.Message == "") groupBalance.Message = "Calcul effectué avec succès.";
        else if (groupBalance.Debts.Count == 0 && groupBalance.Message == "") groupBalance.Message = "Aucun remboursement à effectuer.";

        return View(groupBalance);
    }


}
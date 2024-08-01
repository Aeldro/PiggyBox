using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WildPay.Interfaces;
using WildPay.Models;
using WildPay.Models.Entities;

namespace WildPay.Controllers;

[Authorize]
public class ExpenditureController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExpenditureRepository _expenditureRepository;
    private readonly IGroupRepository _groupRepository;

    public ExpenditureController(UserManager<ApplicationUser> userManager, IExpenditureRepository expenditureRepository, IGroupRepository groupRepository)
    {
        _userManager = userManager;
        _expenditureRepository = expenditureRepository;
        _groupRepository = groupRepository;
    }

    // READ
    async public Task<IActionResult> GroupBalances(int Id)
    {
        //Get the group
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        //Return not found if no group is found
        if (group == null) { return NotFound(); }

        //Verify if the User belongs to the group, else we block the access
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

        GroupBalance groupBalance = new GroupBalance();
        groupBalance.Group = group;
        groupBalance.TotalAmount = group.Expenditures.Sum(el => el.Amount);
        groupBalance.Message = "Calcul effecuté avec succès.";

        Dictionary<string, double> membersBalance = new Dictionary<string, double>();
        foreach (ApplicationUser member in group.ApplicationUsers) membersBalance.Add(member.Id, 0);

        if (group.Expenditures is not null && group.Expenditures.Count > 0)
        {
            foreach (Expenditure expenditure in group.Expenditures)
            {
                if (expenditure.PayerId is null || expenditure.Payer is null) groupBalance.Message = "Attention ! Les dépenses qui n'ont pas de payeur n'ont pas été prises en compte. Vérifiez les dépenses du groupe et ajoutez-y un payeur si vous voulez les inclure au calcul.";
                else
                {
                    double expenditureContributionPerPerson = expenditure.Amount / expenditure.RefundContributors.Count;
                    membersBalance[expenditure.PayerId] += expenditure.Amount;
                    foreach (ApplicationUser contributor in expenditure.RefundContributors)
                    {
                        membersBalance[contributor.Id] -= expenditureContributionPerPerson;
                    }
                }
            }
        }
        groupBalance.UsersBalance = membersBalance;

        Dictionary<string, double> positiveBalanceMembers = new Dictionary<string, double>();
        Dictionary<string, double> negativeBalanceMembers = new Dictionary<string, double>();
        foreach (KeyValuePair<string, double> member in membersBalance)
        {
            if (member.Value < 0) negativeBalanceMembers.Add(member.Key, member.Value);
            else if (member.Value > 0) positiveBalanceMembers.Add(member.Key, member.Value);
        }

        while (positiveBalanceMembers.Any(el => el.Value != 0) && negativeBalanceMembers.Any(el => el.Value != 0))
        {
            KeyValuePair<string, double> positiveBalanceMember = positiveBalanceMembers.OrderByDescending(el => el.Value).First();
            ApplicationUser positiveBalanceUser = group.ApplicationUsers.Find(el => el.Id == positiveBalanceMember.Key);
            KeyValuePair<string, double> negativeBalanceMember = negativeBalanceMembers.OrderBy(el => el.Value).First();
            ApplicationUser negativeBalanceUser = group.ApplicationUsers.Find(el => el.Id == negativeBalanceMember.Key);
            double amount = 0;

            if (positiveBalanceMember.Value >= negativeBalanceMember.Value)
            {
                amount = -negativeBalanceMember.Value;
                positiveBalanceMembers[positiveBalanceMember.Key] = positiveBalanceMember.Value + negativeBalanceMember.Value;
                negativeBalanceMembers[negativeBalanceMember.Key] = negativeBalanceMember.Value + negativeBalanceMember.Value;
            }
            else if (positiveBalanceMember.Value < negativeBalanceMember.Value)
            {
                amount = positiveBalanceMember.Value;
                negativeBalanceMembers[negativeBalanceMember.Key] = negativeBalanceMember.Value + positiveBalanceMember.Value;
                positiveBalanceMembers[positiveBalanceMember.Key] = positiveBalanceMember.Value - positiveBalanceMember.Value;
            }

            Debt debt = new Debt(negativeBalanceUser, positiveBalanceUser, amount);
            groupBalance.Debts.Add(debt);
        }

        return View(groupBalance);
    }

    async public Task<IActionResult> GroupExpenditures(int Id)
    {
        //Get the group
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        //Return not found if no group is found
        if (group == null) { return NotFound(); }

        //Verify if the User belongs to the group, else we block the access
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

        return View(group);
    }

    // UPDATE
    [HttpGet]
    public IActionResult Edit()
    {
        return View();
    }

    // UPDATE
    [HttpPost]
    public IActionResult Edit(Expenditure expenditure)
    {
        return RedirectToAction(actionName: "List", controllerName: "Expenditure");
    }

    // CREATE
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // CREATE
    [HttpPost]
    public IActionResult Create(Expenditure expenditure)
    {
        return RedirectToAction(actionName: "List", controllerName: "Expenditure");
    }

    // DELETE
    [HttpGet]
    public IActionResult Delete()
    {
        return View();
    }

    // DELETE
    [HttpPost]
    public IActionResult Delete(Expenditure expenditure)
    {
        return RedirectToAction(actionName: "List", controllerName: "Expenditure");
    }
}
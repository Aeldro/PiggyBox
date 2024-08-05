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

        Dictionary<ApplicationUser, double> membersBalance = await CalculateMembersBalance(group); //Calculate the balance of each member
        membersBalance = membersBalance.OrderByDescending(el => el.Value).ToDictionary(el => el.Key, el => el.Value);
        groupBalance.UsersBalance = membersBalance;
        groupBalance = await CalculateDebtsList(groupBalance, group); //Calculate who must pay who

        if (group.Expenditures.Any(el => el.PayerId is null) || group.Expenditures.Any(el => el.Payer is null)) groupBalance.Message = "Attention ! Les d�penses qui n'ont pas de payeur n'ont pas �t� prises en compte. V�rifiez les d�penses du groupe et ajoutez-y un payeur si vous voulez les inclure au calcul.";
        else if (groupBalance.Debts.Count > 0 && groupBalance.Message == "") groupBalance.Message = "Calcul effectu� avec succ�s.";
        else if (groupBalance.Debts.Count == 0 && groupBalance.Message == "") groupBalance.Message = "Aucun remboursement � effectuer.";

        return View(groupBalance);
    }

    //Used in the GroupBalances method to calculate the balance of each member
    public async Task<Dictionary<ApplicationUser, double>> CalculateMembersBalance(Group group)
    {
        Dictionary<ApplicationUser, double> membersBalance = new Dictionary<ApplicationUser, double>();

        foreach (ApplicationUser member in group.ApplicationUsers) membersBalance.Add(member, 0);

        foreach (Expenditure expenditure in group.Expenditures)
        {
            if (expenditure.PayerId is not null && expenditure.Payer is not null)
            {
                double expenditureContributionPerPerson = expenditure.Amount / await _expenditureRepository.GetContributorsCount(expenditure.Id);
                membersBalance[expenditure.Payer] = membersBalance[expenditure.Payer] + expenditure.Amount;
                foreach (ApplicationUser contributor in expenditure.RefundContributors)
                {
                    membersBalance[contributor] = membersBalance[contributor] - expenditureContributionPerPerson;
                }
            }
        }

        return membersBalance;
    }

    //Used in the GroupBalances method to calculate who must pay who
    public async Task<GroupBalance> CalculateDebtsList(GroupBalance groupBalance, Group group)
    {
        //Split the users balances into two parts : positives balances and negative balances. This aims to make the further calculation easier
        Dictionary<ApplicationUser, double> positiveBalanceMembers = new Dictionary<ApplicationUser, double>();
        Dictionary<ApplicationUser, double> negativeBalanceMembers = new Dictionary<ApplicationUser, double>();
        foreach (KeyValuePair<ApplicationUser, double> member in groupBalance.UsersBalance)
        {
            if (member.Value < 0) negativeBalanceMembers.Add(member.Key, member.Value);
            else if (member.Value > 0) positiveBalanceMembers.Add(member.Key, member.Value);
        }

    // CREATE
    [HttpGet]
    public async Task<IActionResult> Add(int Id)
    {
        ViewBag.idGroup = Id;
        return View();
    }

    // CREATE
    [HttpPost]
    public async Task<IActionResult> Add(Expenditure expenditure)
    {
        if (ModelState.IsValid)
        {
            string applicationUserId = _userManager.GetUserId(User); // get the id of the connected user
            await _expenditureRepository.AddExpenditureAsync( // add expediture to the repository
                expenditure.Name, 
                (double)expenditure.Amount, 
                expenditure.Date, 
                applicationUserId, 
                expenditure.CategoryId, 
                expenditure.GroupId);
            return RedirectToAction(actionName: "GroupExpenditures", controllerName: "Expenditure", new {id = expenditure.GroupId});
        }
        return View();
    }

        //Looping until everyone gets refunded
        while (positiveBalanceMembers.Any(el => el.Value > 0.01) && negativeBalanceMembers.Any(el => el.Value < 0.01))
        {
            //Match the member who has the highest balance to the member who has the lowest balance
            KeyValuePair<ApplicationUser, double> positiveBalanceMember = positiveBalanceMembers.OrderByDescending(el => el.Value).First();
            ApplicationUser positiveBalanceUser = group.ApplicationUsers.Find(el => el.Id == positiveBalanceMember.Key.Id);
            KeyValuePair<ApplicationUser, double> negativeBalanceMember = negativeBalanceMembers.OrderBy(el => el.Value).First();
            ApplicationUser negativeBalanceUser = group.ApplicationUsers.Find(el => el.Id == negativeBalanceMember.Key.Id);

            double amount = 0;

            //Update the balance of both members
            if (positiveBalanceMember.Value >= negativeBalanceMember.Value)
            {
                amount = -negativeBalanceMember.Value;
                positiveBalanceMembers[positiveBalanceMember.Key] = positiveBalanceMember.Value + negativeBalanceMember.Value;
                negativeBalanceMembers[negativeBalanceMember.Key] = negativeBalanceMember.Value - negativeBalanceMember.Value;
            }
            else if (positiveBalanceMember.Value < negativeBalanceMember.Value)
            {
                amount = positiveBalanceMember.Value;
                negativeBalanceMembers[negativeBalanceMember.Key] = negativeBalanceMember.Value + positiveBalanceMember.Value;
                positiveBalanceMembers[positiveBalanceMember.Key] = positiveBalanceMember.Value - positiveBalanceMember.Value;
            }

            //Make the debt
            Debt debt = new Debt(negativeBalanceUser, positiveBalanceUser, amount);
            groupBalance.Debts.Add(debt);
        }

        return groupBalance;
    }
}
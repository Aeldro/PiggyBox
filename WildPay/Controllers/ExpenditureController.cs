using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using NuGet.Protocol.Core.Types;
using WildPay.Interfaces;
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

    async public Task<IActionResult> GroupBalances(int Id)
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
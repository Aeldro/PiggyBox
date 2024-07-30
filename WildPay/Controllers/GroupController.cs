using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using WildPay.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Controllers;

[Authorize]
public class GroupController : Controller
{
    private readonly IGroupRepository _repository;
    public GroupController(IGroupRepository repository)
    {
        _repository = repository;
    }

    // READ
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.UserId = userId;
        var groups = await _repository.GetGroupsAsync(userId);
        return View(groups);
    }

    [HttpGet]
    public async Task<IActionResult> GetGroup(int id)
    {
        Group group = await _repository.GetGroupByIdAsync(id);
        return View(group);
    }

    // CREATE view
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    // CREATE action
    [HttpPost]
    public async Task<IActionResult> Add(Group group)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        ViewBag.UserId = userId;
        await _repository.AddGroupAsync(group.Name, group.Image, userId);
        
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
    
    // UPDATE view
    [HttpGet]
    public IActionResult Edit()
    {
        return View();
    }
    
    // UPDATE action
    [HttpPost]
    public async Task<IActionResult> Update(Group group)
    {
        await _repository.EditGroupAsync(group);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }

    // DELETE view 
    [HttpGet]
    public IActionResult Delete()
    {
        return View();
    }
    
    // DELETE action 
    [HttpPost]
    public async Task<IActionResult> Delete(int groupId)
    {
        await _repository.DeleteGroupAsync(groupId);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
}
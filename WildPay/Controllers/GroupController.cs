using Microsoft.AspNetCore.Mvc;
using WildPay.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Controllers;

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
        var groups = await _repository.GetGroupsAsync();
        return View(groups);
    }

    // CREATE view
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }

    // CREATE action
    [HttpPost]
    public async Task<IActionResult> Create(Group group)
    {
        await _repository.AddGroupAsync(group.Name, group.Image);
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
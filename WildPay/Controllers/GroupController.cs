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

    // CREATE group view
    [HttpGet]
    public IActionResult Add()
    {
        return View();
    }

    // CREATE group action
    [HttpPost]
    public async Task<IActionResult> Add(Group group)
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        await _repository.AddGroupAsync(group.Name, group.Image, userId);
        
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
    
    // UPDATE group view
    [HttpGet]
    public async Task<IActionResult> Update(int id)
    {
        var group = await _repository.GetGroupByIdAsync(id);
        return View(group);
    }
    
    // UPDATE group action
    [HttpPost]
    public async Task<IActionResult> Update(Group group)
    {
        await _repository.EditGroupAsync(group);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }

    // Add a member to a group using a form
    // Make sure to add a hidden field for the group ID
    [HttpPost]
    public async Task<IActionResult> AddMember(int groupId, string email)
    {
        Group? group = await _repository.GetGroupByIdAsync(groupId);
       
        if (group != null)
        {
            // Returns false if no match is found;
            // think about a way to handle the case the email doesn't match a user
            await _repository.AddMemberToGroupAsync(group, email);
        }
        return RedirectToAction(actionName: "Edit", controllerName: "Group");    
    }

    // Delete a member from a group
    [HttpGet]
    public async Task<IActionResult> DeleteMember(int groupId, string userId)
    {
        Group? group = await _repository.GetGroupByIdAsync(groupId);

        if (group != null)
        {
            // Returns false if no match is found;
            // think about a way to handle the case the email doesn't match a user
            await _repository.DeleteMemberFromGroupAsync(group, userId);
        }
        return RedirectToAction(actionName: "Edit", controllerName: "Group");
    }

    // DELETE group view 
    [HttpGet]
    public IActionResult Delete()
    {
        
        return View();
    }
    
    // DELETE group action 
    [HttpPost]
    public async Task<IActionResult> Delete(int groupId)
    {
        await _repository.DeleteGroupAsync(groupId);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
}
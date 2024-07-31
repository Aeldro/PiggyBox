using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WildPay.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Controllers;

// only accessible if the user is connected
[Authorize]
public class GroupController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGroupRepository _repository;
    public GroupController(UserManager<ApplicationUser> userManager, IGroupRepository repository)
    {
        _userManager = userManager;
        _repository = repository;
    }

    // READ: get all the groups for the connected user
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var groups = await _repository.GetGroupsAsync(userId);
        return View(groups);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int Id)
    {
        //Get the group
        Group ?group = await _repository.GetGroupByIdAsync(Id);

        //Return not found if no group is found
        if (group == null)
        {
            return NotFound();
        }

        //Verify if the User belongs to the group, else we block the access
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return NotFound();

        return View(group);
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

        // check if the user is connected + is part of the group
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return NotFound();

        return View(group);
    }
    
    // UPDATE group action
    [HttpPost]
    public async Task<IActionResult> Update(Group group)
    {
        if (group.Image is null)
        {
            group.Image = string.Empty;
        }

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

        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return NotFound();

        if (group != null)
        {
            // Returns false if no match is found;
            // think about a way to handle the case the email doesn't match a user
            await _repository.DeleteMemberFromGroupAsync(group, userId);
        }
        return RedirectToAction(actionName: "Edit", controllerName: "Group");
    }

    // DELETE group action 
    [HttpGet]
    public async Task<IActionResult> Delete(int id)
    {
        await _repository.DeleteGroupAsync(id);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
}
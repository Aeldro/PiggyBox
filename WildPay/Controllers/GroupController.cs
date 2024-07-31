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
    private readonly IGroupRepository _groupRepository;

    public GroupController(UserManager<ApplicationUser> userManager, IGroupRepository groupRepository)
    {
        _userManager = userManager;
        _groupRepository = groupRepository;
    }

    // READ: get all the groups for the connected user
    [HttpGet]
    public async Task<IActionResult> List()
    {
        var userId = _userManager.GetUserId(User);
        var groups = await _groupRepository.GetGroupsAsync(userId);
        return View(groups);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int Id)
    {
        //Get the group
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        //Return not found if no group is found
        if (group == null) { return NotFound(); }

        //Verify if the User belongs to the group, else we block the access
        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) { return NotFound(); }

        return View(group);
    }

    // CREATE view
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
        await _groupRepository.AddGroupAsync(group.Name, group.Image, userId);

        return RedirectToAction(actionName: "List", controllerName: "Group");
    }

    // UPDATE group view
    [HttpGet]
    public async Task<IActionResult> Update(int Id)
    {
        var group = await _groupRepository.GetGroupByIdAsync(id);
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

        await _groupRepository.EditGroupAsync(group);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }

    // Add a member to a group using a form
    // Make sure to add a hidden field for the group ID
    [HttpPost]
    public async Task<IActionResult> AddMember(int groupId, string email)
    {
        Group? group = await _groupRepository.GetGroupByIdAsync(groupId);

        if (group != null)
        {
            // Returns false if no match is found;
            // think about a way to handle the case the email doesn't match a user
            await _groupRepository.AddMemberToGroupAsync(group, email);
        }
        return RedirectToAction(actionName: "Edit", controllerName: "Group");
    }

    // Delete a member from a group
    [HttpGet]
    public async Task<IActionResult> DeleteMember(int groupId, string userId)
    {
        Group? group = await _groupRepository.GetGroupByIdAsync(groupId);

        if (group != null)
        {
            // Returns false if no match is found;
            // think about a way to handle the case the email doesn't match a user
            await _groupRepository.DeleteMemberFromGroupAsync(group, userId);
        }
        return RedirectToAction(actionName: "Edit", controllerName: "Group");
    }

    // DELETE group view
    [HttpGet]
    public async Task<IActionResult> Delete(int Id)
    {
        var group = await _repository.GetGroupByIdAsync(Id);
        return View(group);
    }

    // DELETE group action 
    [HttpPost]
    public async Task<IActionResult> Delete(int Id, Group group)
    {
        await _groupRepository.DeleteGroupAsync(id);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
}
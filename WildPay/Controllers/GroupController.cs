using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WildPay.Interfaces;
using WildPay.Models.Entities;
using WildPay.Models.ViewModels;

namespace WildPay.Controllers;

// methods are only accessible if the user is connected
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
        if (userId is null) return NotFound();

        var groups = await _groupRepository.GetGroupsAsync(userId);
        return View(groups);
    }

    [HttpGet]
    public async Task<IActionResult> Index(int Id)
    {
        //Get the group
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        //Return not found if no group is found
        if (group is null) return NotFound();

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
        if (!ModelState.IsValid) return View(group);

        string? userId = _userManager.GetUserId(User);
        if (userId is null) return NotFound();

        await _groupRepository.AddGroupAsync(group.Name, group.Image, userId);
        
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }

    // UPDATE group view
    [HttpGet]
    public async Task<IActionResult> Update(int Id)
    {
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        if (group is null) return NotFound();

        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return RedirectToAction("List", "Group");

        UpdateGroupModel updateGroupModel = new UpdateGroupModel
        {
            GroupToUpdate = group,
            NewMember = new MemberAdded()
            {
                GroupId = Id,
                Email = ""
            }
        };

        return View(updateGroupModel);
    }

    // UPDATE group action
    [HttpPost]
    public async Task<IActionResult> Update(UpdateGroupModel modelUpdated)
    {
        if (ModelState.IsValid) return View(modelUpdated);

        Group? groupUpdated = modelUpdated.GroupToUpdate;

        if (groupUpdated is null) return NotFound();

        if (groupUpdated.Image is null)
        {
            groupUpdated.Image = string.Empty;
        }

        await _groupRepository.EditGroupAsync(groupUpdated);
        return RedirectToAction(actionName: "Index", controllerName: "Group", new { groupUpdated.Id });
    }

    // Add a member to a group using a form
    // Make sure to add a hidden field for the group ID
    [HttpPost]
    public async Task<IActionResult> AddMember(UpdateGroupModel modelUpdated)
    {
        if (modelUpdated.NewMember is null) return NotFound();

        MemberAdded newMember = modelUpdated.NewMember;
        if (newMember is null) return NotFound();

        if (newMember.Email is null) return NotFound();

        Group? group = await _groupRepository.GetGroupByIdAsync(newMember.GroupId);

        if (group is null) return NotFound();

        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return NotFound();

        // Returns false if no match is found;
        // think about a way to handle the case the email doesn't match a user
        await _groupRepository.AddMemberToGroupAsync(group, newMember.Email);

        return RedirectToAction(actionName: "Update", controllerName: "Group", new { Id = newMember.GroupId });    
    }

    // Delete a member from a group view
    [HttpGet]
    public async Task<IActionResult> DeleteMember(string userId, int groupId)
    {
        Group? group = await _groupRepository.GetGroupByIdAsync(groupId);
        if (group is null) return NotFound();

        ApplicationUser? userToRemove = group.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

        if (group is null) return NotFound();

        if (userToRemove is null) return NotFound();

        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return NotFound();

        ViewBag.user = userToRemove;
        return View("DeleteMember", group);
    }

    // Delete a member from a group action
    [HttpPost]
    public async Task<IActionResult> DeleteMember(string userId, int groupId, Group group)
    {
        Group? userGroup = await _groupRepository.GetGroupByIdAsync(groupId);

        if (userGroup is null) return NotFound();

        if (_userManager.GetUserId(User) is null || userGroup.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return NotFound();

        // Returns false if no match is found;
        // think about a way to handle the case the email doesn't match a user
        await _groupRepository.DeleteMemberFromGroupAsync(userGroup, userId);

        return RedirectToAction(actionName: "Update", controllerName: "Group", new { Id = groupId });
    }

    // DELETE group view
    [HttpGet]
    public async Task<IActionResult> Delete(int Id)
    {
        Group? group = await _groupRepository.GetGroupByIdAsync(Id);

        if (group is null) return NotFound();

        if (_userManager.GetUserId(User) is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == _userManager.GetUserId(User)) is null) return NotFound();

        return View(group);
    }

    // DELETE group action 
    [HttpPost]
    public async Task<IActionResult> Delete(int Id, Group group)
    {
        await _groupRepository.DeleteGroupAsync(Id);
        return RedirectToAction(actionName: "List", controllerName: "Group");
    }
}
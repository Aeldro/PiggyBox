using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using WildPay.Exceptions;
using WildPay.Interfaces;
using WildPay.Models.Entities;
using WildPay.Models.ViewModels;
using WildPay.Services.Interfaces;

namespace WildPay.Controllers;

// methods are only accessible if the user is connected
[Authorize]
public class GroupController : Controller
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IGroupRepository _groupRepository;
    private readonly IVerificationService _verificationService;

    public GroupController(UserManager<ApplicationUser> userManager, IGroupRepository groupRepository, IVerificationService verificationService)
    {
        _userManager = userManager;
        _groupRepository = groupRepository;
        _verificationService = verificationService;
    }

    // READ: get all the groups for the connected user
    [HttpGet]
    public async Task<IActionResult> ListMyGroups()
    {
        try
        {
            var userId = _userManager.GetUserId(User);
            if (userId is null) return NotFound();

            var groups = await _groupRepository.GetGroupsAsync(userId);
            return View(groups);
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    [HttpGet]
    public async Task<IActionResult> GetGroup(int Id)
    {
        try
        {
            //Get the group
            Group? group = await _groupRepository.GetGroupByIdAsync(Id);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            return View(group);
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // CREATE group view
    [HttpGet]
    public IActionResult AddGroup()
    {
        return View();
    }

    // CREATE group action
    [HttpPost]
    public async Task<IActionResult> AddGroup(Group group)
    {
        try
        {
            if (!ModelState.IsValid) return View(group);

            string? userId = _userManager.GetUserId(User);
            if (userId is null) return NotFound();

            await _groupRepository.AddGroupAsync(group.Name, group.Image, userId);

            return RedirectToAction(actionName: "ListMyGroups", controllerName: "Group");
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // UPDATE group view
    [HttpGet]
    public async Task<IActionResult> UpdateGroup(int Id, bool IsMemberAdded = true, bool IsMemberAlreadyExisting = false, bool FirstDisplay = true)
    {
        try
        {
            Group? group = await _groupRepository.GetGroupByIdAsync(Id);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            UpdateGroupModel updateGroupModel = new UpdateGroupModel
            {
                GroupToUpdate = group,
                NewMember = new MemberAdded()
                {
                    GroupId = Id,
                    Email = ""
                }
            };

            if (!IsMemberAdded)
            {
                ViewBag.Message = "Pas d'utilisateur trouvé avec cette adresse mail.";
            }
            else if (IsMemberAlreadyExisting)
            {
                ViewBag.Message = "Cet utilisateur appartient déjà au groupe.";
            }
            else if (!FirstDisplay)
            {
                ViewBag.Message = "Utilisateur ajouté au groupe avec succès.";
            }

            return View(updateGroupModel);
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // UPDATE group action
    [HttpPost]
    public async Task<IActionResult> UpdateGroup(Group group)
    {
        try
        {
            Group? currentGroup = await _groupRepository.GetGroupByIdAsync(group.Id);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), currentGroup);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            if (!ModelState.IsValid)
            {
                Group? invalidGroup = await _groupRepository.GetGroupByIdAsync(group.Id);

                UpdateGroupModel updateGroupModel = new UpdateGroupModel
                {
                    GroupToUpdate = invalidGroup,
                    NewMember = new MemberAdded()
                    {
                        GroupId = invalidGroup.Id,
                        Email = ""
                    }
                };

                return View(updateGroupModel);
            }

            if (group is null) return NotFound();

            if (group.Image is null)
            {
                group.Image = string.Empty;
            }

            await _groupRepository.EditGroupAsync(group);

            return RedirectToAction(actionName: "GetGroup", controllerName: "Group", new { group.Id });
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // Add a member to a group using a form
    // Make sure to add a hidden field for the group ID
    [HttpPost]
    public async Task<IActionResult> AddMemberToGroup(MemberAdded newMember)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                Group? groupUpdated = await _groupRepository.GetGroupByIdAsync(newMember.GroupId);

                if (groupUpdated is null) return NotFound();

                UpdateGroupModel updateGroupModel = new UpdateGroupModel
                {
                    GroupToUpdate = groupUpdated,
                    NewMember = newMember
                };

                return View("UpdateGroup", updateGroupModel);
            }

            Group? group = await _groupRepository.GetGroupByIdAsync(newMember.GroupId);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            bool OldMember = group.ApplicationUsers.Any(u => u.NormalizedEmail == newMember.Email.ToUpper());

            if (!OldMember)
            {
                // Returns false if no match is found;
                // think about a way to handle the case the email doesn't match a user
                bool IsFound = await _groupRepository.AddMemberToGroupAsync(group, newMember.Email);

                return RedirectToAction(actionName: "UpdateGroup", controllerName: "Group", new { Id = newMember.GroupId, IsMemberAdded = IsFound, FirstDisplay = false });
            }

            return RedirectToAction(actionName: "UpdateGroup", controllerName: "Group", new { Id = newMember.GroupId, IsMemberAlreadyExisting = OldMember, FirstDisplay = false });
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // Delete a member from a group view
    [HttpGet]
    public async Task<IActionResult> DeleteMemberFromGroup(string userId, int groupId)
    {
        try
        {
            Group? group = await _groupRepository.GetGroupByIdAsync(groupId);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            ApplicationUser? userToRemove = group.ApplicationUsers.FirstOrDefault(u => u.Id == userId);

            if (group is null) return NotFound();

            if (userToRemove is null) return NotFound();

            ViewBag.user = userToRemove;
            return View("DeleteMemberFromGroup", group);
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // Delete a member from a group action
    [HttpPost]
    public async Task<IActionResult> DeleteMemberFromGroup(string userId, int groupId, Group group)
    {
        try
        {
            Group? userGroup = await _groupRepository.GetGroupByIdAsync(groupId);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), userGroup);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            // Returns false if no match is found;
            // think about a way to handle the case the email doesn't match a user
            await _groupRepository.DeleteMemberFromGroupAsync(userGroup, userId);

            if (_userManager.GetUserId(User) == userId)
            {
                return RedirectToAction(actionName: "ListMyGroups", controllerName: "Group");
            }

            return RedirectToAction(actionName: "UpdateGroup", controllerName: "Group", new { Id = groupId });
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // DELETE group view
    [HttpGet]
    public async Task<IActionResult> DeleteGroup(int Id, string viewSender)
    {
        try
        {
            Group? group = await _groupRepository.GetGroupByIdAsync(Id);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            ViewBag.Action = viewSender;

            return View(group);
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }

    // DELETE group action 
    [HttpPost]
    public async Task<IActionResult> DeleteGroup(int Id)
    {
        try
        {
            Group? group = await _groupRepository.GetGroupByIdAsync(Id);

            //Verify if the User belongs to the group, else we block the access
            bool isUserFromGroup = _verificationService.IsUserBelongsToGroup(_userManager.GetUserId(User), group);
            if (!isUserFromGroup) return RedirectToAction(actionName: "Index", controllerName: "Home");

            await _groupRepository.DeleteGroupAsync(Id);
            return RedirectToAction(actionName: "ListMyGroups", controllerName: "Group");
        }
        catch (DatabaseException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
        catch (NullException ex)
        {
            return RedirectToAction(actionName: "Exception", controllerName: "Home", new { message = ex.Message });
        }
    }
}
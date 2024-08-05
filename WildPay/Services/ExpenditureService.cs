using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WildPay.Interfaces;
using WildPay.Models.Entities;
using WildPay.Models.ViewModels;
using WildPay.Services.Interfaces;

namespace WildPay.Services;

public class ExpenditureService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExpenditureRepository _expenditureRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IBalanceService _balanceService;

    public ExpenditureService(UserManager<ApplicationUser> userManager, IExpenditureRepository expenditureRepository, IGroupRepository groupRepository, ICategoryRepository categoryRepository, IBalanceService balanceService)
    {
        _userManager = userManager;
        _expenditureRepository = expenditureRepository;
        _groupRepository = groupRepository;
        _categoryRepository = categoryRepository;
        _balanceService = balanceService;
    }
    public async Task<IActionResult> AddExpenditure(AddExpenditureInGroup model)
    {
        Group group = await _groupRepository.GetGroupByIdAsync(model.GroupId);
        model.ExpenditureToCreate.Group = group;
        
        ApplicationUser payer = group.ApplicationUsers.FirstOrDefault(u => u.Id == model.ExpenditureToCreate.PayerId);
        model.ExpenditureToCreate.Payer = payer;

        var selectedUsers = await _userManager.Users
            .Where(u => model.SelectedUsersIds.Contains(u.Id))
            .ToListAsync();
        
        model.ExpenditureToCreate.RefundContributors = selectedUsers;

        await _expenditureRepository.AddExpenditureAsync(model.ExpenditureToCreate);
        return null;
    }
}
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using WildPay.Interfaces;
using WildPay.Models.Entities;
using WildPay.Models.ViewModels;
using WildPay.Services.Interfaces;

namespace WildPay.Services;

public class ExpenditureService : IExpenditureService
{
    private readonly UserManager<ApplicationUser> _userManager;
    private readonly IExpenditureRepository _expenditureRepository;
    private readonly ICategoryRepository _categoryRepository;
    private readonly IGroupRepository _groupRepository;

    public ExpenditureService(UserManager<ApplicationUser> userManager, IExpenditureRepository expenditureRepository, ICategoryRepository categoryRepository, IGroupRepository groupRepository)
    {
        _userManager = userManager;
        _expenditureRepository = expenditureRepository;
        _categoryRepository = categoryRepository;
        _groupRepository = groupRepository;
    }

    public async Task<AddExpenditureInGroup> AddExpenditure(int Id)
    {
        Group? group = await _groupRepository.GetGroupByIdAsync(Id); // Get the Id of the group associated to the new expenditure
        List<ApplicationUser> usersInGroup = group.ApplicationUsers.ToList();
        List<Category> categoriesInGroup = group.Categories.ToList();
        AddExpenditureInGroup model = new AddExpenditureInGroup // creates a new instance of modelView 
        {
            GroupId = group.Id,
            UsersInGroup = usersInGroup,
            CategoriesInGroup = categoriesInGroup
        };
        return model;
    }
    
    // method to create a new Expenditure that calls ExpenditureRepository
    public async Task<bool> AddExpenditure(AddExpenditureInGroup model)
    {
        Group? group = await _groupRepository.GetGroupByIdAsync(model.GroupId); // find the group of the model.Expenditure
        model.ExpenditureToCreate.Group = group; // set Group to the new Expenditure
       
        Category? category = await _categoryRepository.GetCategoryByIdAsync(model.CategoryId); // find the correspondent category 
        model.ExpenditureToCreate.Category = category; // set the category to the new expenditure
        
        ApplicationUser payer = group.ApplicationUsers.FirstOrDefault(u => u.Id == model.ExpenditureToCreate.PayerId);
        model.ExpenditureToCreate.Payer = payer; // set Payer to the new Expenditure

        var selectedUsers = await _userManager.Users // find the selected users in the view and convert it to a list
            .Where(u => model.SelectedUsersIds.Contains(u.Id))
            .ToListAsync();
        
        model.ExpenditureToCreate.RefundContributors = selectedUsers; // set RefundContributors to the new Expenditure

        await _expenditureRepository.AddExpenditureAsync(model.ExpenditureToCreate); // add expenditure to databse
        return true;
    }
}
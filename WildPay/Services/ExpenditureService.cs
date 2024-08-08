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
            CategoriesInGroup = categoriesInGroup,
            ExpenditureToCreate = new Expenditure()
            {
                Date = DateTime.Now
            }
        };
        return model;
    }
    
    // method to create a new Expenditure that calls ExpenditureRepository
    public async Task<bool> AddExpenditure(AddExpenditureInGroup model)
    {
        model.ExpenditureToCreate.Group = await _groupRepository.GetGroupByIdAsync(model.GroupId); ; // set Group to the new Expenditure

        if (model.CategoryId is not null)
        {
            model.ExpenditureToCreate.Category = await _categoryRepository.GetCategoryByIdAsync((int)model.CategoryId); // set the category to the new expenditure
        }
        else
        {
            model.CategoryId = null;
        }
        
        model.ExpenditureToCreate.Payer = model.ExpenditureToCreate.Group.ApplicationUsers.FirstOrDefault(u => u.Id == model.ExpenditureToCreate.PayerId); // set Payer to the new Expenditure

        // set RefundContributors to the new Expenditure
        model.ExpenditureToCreate.RefundContributors = await _userManager.Users // find the selected users in the view and convert it to a list
            .Where(u => model.SelectedUsersIds.Contains(u.Id))
            .ToListAsync();

        await _expenditureRepository.AddExpenditureAsync(model.ExpenditureToCreate); // add expenditure to databse
        return true;
    }

    public async Task<bool> DeleteExpenditure(Expenditure expenditure)
    {
        Expenditure expenditureToRemove = await _expenditureRepository.GetExpenditureByIdAsync(expenditure.Id);
        _expenditureRepository.DeleteExpenditureAsync(expenditureToRemove);
        return false;
    }
}
using WildPay.Models.Entities;

namespace WildPay.Models.ViewModels;

public class AddExpenditureInGroup
{
    public int GroupId { get; set; }
    public Expenditure ExpenditureToCreate { get; set; }
    public List<ApplicationUser> UsersInGroup { get; set; } = new List<ApplicationUser>();
    public List<string> SelectedUsersIds { get; set; } = new List<string>();
    public int CategoryId { get; set; }
    public List<Category> CategoriesInGroup { get; set; } = new List<Category>();
}
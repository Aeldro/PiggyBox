using WildPay.Models.Entities;

namespace WildPay.Models.ViewModels;

public class AddExpenditureInGroup
{
    public int GroupId { get; set; }
    public Expenditure ExpenditureToCreate { get; set; }
    public List<ApplicationUser> Users { get; set; } = new List<ApplicationUser>();
}
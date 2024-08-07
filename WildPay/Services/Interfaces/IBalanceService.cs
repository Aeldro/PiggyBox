using WildPay.Models;
using WildPay.Models.Entities;

namespace WildPay.Services.Interfaces
{
    public interface IBalanceService
    {
        Task<Dictionary<ApplicationUser, double>> CalculateMembersBalance(Group group);
        GroupBalance CalculateDebtsList(GroupBalance groupBalance, Group group);
    }
}

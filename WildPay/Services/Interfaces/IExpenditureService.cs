using WildPay.Models.ViewModels;

namespace WildPay.Services.Interfaces;

public interface IExpenditureService
{
    public Task<bool> AddExpenditure(AddExpenditureInGroup model);
    public Task<AddExpenditureInGroup> AddExpenditure(int Id);
}
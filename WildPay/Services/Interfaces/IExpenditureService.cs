using WildPay.Models.Entities;
using WildPay.Models.ViewModels;

namespace WildPay.Services.Interfaces;

public interface IExpenditureService
{
    public Task<bool> AddExpenditure(AddExpenditureInGroup model);
    public Task<AddExpenditureInGroup> AddExpenditure(int Id);
    public Task<bool> DeleteExpenditure(Expenditure expenditure);
    public Task<Expenditure> GetExpenditureById(int expenditureId);
}
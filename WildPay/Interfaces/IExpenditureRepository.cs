using WildPay.Models.Entities;

namespace WildPay.Interfaces
{
    public interface IExpenditureRepository
    {
        Task<Expenditure?> GetExpenditureByIdAsync(int expenditureId);
        Task<int> GetContributorsCount(int expenditureId);
        Task<List<Expenditure>> GetExpendituresAsync(string groupId);
        Task EditExpenditureAsync(Expenditure expenditure);
        Task AddExpenditureAsync(string name, double amount, DateTime date, string? payerId, int? categoryId, int? groupId);
        Task<bool> DeleteExpenditureAsync(int expenditureId);
    }
}

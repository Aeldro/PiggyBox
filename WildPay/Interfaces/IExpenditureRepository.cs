using WildPay.Models.Entities;

namespace WildPay.Interfaces
{
    public interface IExpenditureRepository
    {
        Task<List<Expenditure>> GetExpendituresAsync(string groupId);
        Task<Expenditure?> GetExpenditureByIdAsync(int expenditureId);
        Task EditExpenditureAsync(Expenditure expenditure);
        Task AddExpenditureAsync(string name, double amount, DateTime date, string? payerId, int? categoryId, int? groupId);
        Task<bool> DeleteExpenditureAsync(int expenditureId);
    }
}

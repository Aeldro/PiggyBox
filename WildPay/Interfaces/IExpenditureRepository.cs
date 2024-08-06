using WildPay.Models.Entities;

namespace WildPay.Interfaces
{
    public interface IExpenditureRepository
    {
        Task<Expenditure?> GetExpenditureByIdAsync(int expenditureId);
        Task<int> GetContributorsCount(int expenditureId);
        Task<List<Expenditure>> GetExpendituresAsync(string groupId);
        Task EditExpenditureAsync(Expenditure expenditure);
        Task AddExpenditureAsync(Expenditure expenditure);
        Task<bool> DeleteExpenditureAsync(int expenditureId);
    }
}

using WildPay.Models.Entities;

namespace WildPay.Interfaces
{
    public interface IExpenditureRepository
    {
        Task<Expenditure?> GetExpenditureByIdAsync(int expenditureId);
        Task<int> GetContributorsCount(int expenditureId);
        Task EditExpenditureAsync(Expenditure expenditure);

    }
}

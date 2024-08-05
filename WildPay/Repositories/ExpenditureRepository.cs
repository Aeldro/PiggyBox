using Microsoft.EntityFrameworkCore;
using WildPay.Data;
using WildPay.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Repositories
{
    public class ExpenditureRepository : IExpenditureRepository
    {
        private readonly WildPayDbContext _context;

        public ExpenditureRepository(WildPayDbContext context)
        {
            _context = context;
        }

        public async Task<Expenditure?> GetExpenditureByIdAsync(int expenditureId)
        {
            Expenditure? expenditure = await _context.Expenditures
                .Include(g => g.RefundContributors)
                .FirstOrDefaultAsync(g => g.Id == expenditureId);
            return expenditure;
        }

        public async Task<int> GetContributorsCount(int expenditureId)
        {
            Expenditure expenditure = await GetExpenditureByIdAsync(expenditureId);
            int contributorsCount = expenditure.RefundContributors.Count;
            return contributorsCount;
        }

        public async Task EditExpenditureAsync(Expenditure expenditure)
        {
            Expenditure expenditureToUpdate = await GetExpenditureByIdAsync(expenditure.Id);
            expenditureToUpdate.Name = expenditure.Name;
            expenditureToUpdate.Amount = expenditure.Amount;
            expenditureToUpdate.Date = expenditure.Date;
            expenditureToUpdate.Payer = expenditure.Payer;
            expenditureToUpdate.Category = expenditure.Category;
            expenditureToUpdate.RefundContributors = expenditure.RefundContributors;
            await _context.SaveChangesAsync();

        }

    }
}

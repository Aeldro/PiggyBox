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

        public async Task<List<Expenditure>> GetExpendituresAsync(string groupId)
        {
            throw new NotImplementedException();
        }

        public async Task<Expenditure?> GetExpenditureByIdAsync(int expenditureId)
        {
            throw new NotImplementedException();
        }

        public async Task EditExpenditureAsync(Expenditure expenditure)
        {
            throw new NotImplementedException();
        }

        public async Task AddExpenditureAsync(string name, double amount, DateTime date, string? payerId, int? categoryId, int? groupId)
        {
            Expenditure expenditure = new Expenditure()
            {
                Name = name,
                Amount = amount,
                Date = date,
                PayerId = payerId,
                CategoryId = categoryId,
                GroupId = groupId
            };
            await _context.Expenditures.AddAsync(expenditure);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> DeleteExpenditureAsync(int expenditureId)
        {
            throw new NotImplementedException();
        }
    }
}

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

        // method to create a new expenditure in a given group
        public async Task AddExpenditureAsync(string name, double amount, DateTime date, string? payerId, int? categoryId, int? groupId)
        {
            Expenditure expenditure = new Expenditure() // create the new object expenditure to save in database
            {
                Name = name,
                Amount = amount,
                Date = date,
                PayerId = payerId,
                CategoryId = categoryId,
                GroupId = groupId
            };
            await _context.Expenditures.AddAsync(expenditure); // calls the method from Entity Framework to add the new expenditure to database
            await _context.SaveChangesAsync(); // commit the new change to the database
        }
        
    }
}

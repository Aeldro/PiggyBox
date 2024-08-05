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

        public async Task<List<Expenditure>> GetExpendituresAsync(string groupId)
        {
            throw new NotImplementedException();
        }

        public async Task EditExpenditureAsync(Expenditure expenditure)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> DeleteExpenditureAsync(int expenditureId)
        {
            throw new NotImplementedException();
        }

        public async Task<int> GetContributorsCount(int expenditureId)
        {
            Expenditure expenditure = await GetExpenditureByIdAsync(expenditureId);
            int contributorsCount = expenditure.RefundContributors.Count;
            return contributorsCount;
        }

        // method to create a new expenditure in a given group
        public async Task AddExpenditureAsync(Expenditure expenditure)
        {
            Expenditure newExpenditure = new Expenditure() { // create the new object expenditure to save in database
            
                Name = expenditure.Name,
                Amount = expenditure.Amount,
                Date = expenditure.Date,
                Payer = expenditure.Payer,
                PayerId = expenditure.PayerId,
                Category = expenditure.Category,
                CategoryId = expenditure.CategoryId,
                Group = expenditure.Group,
                GroupId = expenditure.GroupId,
                RefundContributors = expenditure.RefundContributors
            };
            await _context.Expenditures.AddAsync(expenditure); // calls the method from Entity Framework to add the new expenditure to database
            await _context.SaveChangesAsync(); // commit the new change to the database
        }
        
    }
}

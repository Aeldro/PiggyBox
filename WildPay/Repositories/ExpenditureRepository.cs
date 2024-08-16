using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WildPay.Data;
using WildPay.Exceptions;
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
            try
            {
                Expenditure? expenditure = await _context.Expenditures
                .Include(g => g.RefundContributors)
                .FirstOrDefaultAsync(g => g.Id == expenditureId);

                if (expenditure == null) throw new NullException();

                return expenditure;
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        public async Task<int> GetContributorsCountAsync(int expenditureId)
        {
            try
            {
                Expenditure? expenditure = await GetExpenditureByIdAsync(expenditureId);

                if (expenditure == null) throw new NullException();

                int contributorsCount = expenditure.RefundContributors.Count;
                return contributorsCount;
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        // method to create a new expenditure in a given group
        public async Task AddExpenditureAsync(Expenditure expenditure)
        {
            try
            {
                Expenditure newExpenditure = new Expenditure()
                { // creates the new object expenditure to save in database

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
                await _context.Expenditures.AddAsync(newExpenditure); // calls the method from Entity Framework to add the new expenditure to database
                await _context.SaveChangesAsync(); // commit the new change to the database
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        public async Task EditExpenditureAsync(Expenditure expenditure)
        {
            try
            {
                Expenditure? expenditureToUpdate = await GetExpenditureByIdAsync(expenditure.Id);

                if (expenditureToUpdate == null) throw new NullException();

                expenditureToUpdate.Name = expenditure.Name;
                expenditureToUpdate.Amount = expenditure.Amount;
                expenditureToUpdate.Date = expenditure.Date;
                expenditureToUpdate.Payer = expenditure.Payer;
                expenditureToUpdate.PayerId = expenditure.PayerId;
                expenditureToUpdate.Category = expenditure.Category;
                expenditureToUpdate.CategoryId = expenditure.CategoryId;

                expenditureToUpdate.RefundContributors = expenditure.RefundContributors;
                await _context.SaveChangesAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        public async Task<bool> DeleteExpenditureAsync(Expenditure expenditure)
        {
            try
            {
                _context.Expenditures.Remove(expenditure);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

    }
}

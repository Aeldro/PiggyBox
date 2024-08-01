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
        
        public async Task<AddExpenditureAsync>()

    }
}

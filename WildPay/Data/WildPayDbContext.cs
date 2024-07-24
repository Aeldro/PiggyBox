using Microsoft.EntityFrameworkCore;
using WildPay.Models.Entities;

namespace WildPay.Data
{
    public class WildPayDbContext : DbContext
    {
        public WildPayDbContext(DbContextOptions<WildPayDbContext> options) : base(options) { }

        public DbSet<User> Users { get; set; }
        public DbSet<Group> Groups { get; set; }
        public DbSet<Expenditure> Expenditures { get; set; }
        public DbSet<Category> Categories { get; set; }
    }
}

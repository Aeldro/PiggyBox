using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System.Reflection.Emit;
using WildPay.Models.Entities;

namespace WildPay.Data;

public class WildPayDbContext : IdentityDbContext<ApplicationUser>
{
    public WildPayDbContext(DbContextOptions<WildPayDbContext> options)
        : base(options)
    {
    }
    public DbSet<Group> Groups { get; set; }
    public DbSet<Expenditure> Expenditures { get; set; }
    public DbSet<Category> Categories { get; set; }


    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);

    }
}

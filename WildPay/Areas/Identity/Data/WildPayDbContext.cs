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

        // if the group is delete,
        // all the categories related to this group will be deleted.
        builder.Entity<Category>()
            .HasOne(c => c.Group)
            .WithMany(g => g.Categories)
            .HasForeignKey(c => c.GroupId)
            .OnDelete(DeleteBehavior.Cascade);

        // Cascade doesn't work so we will have to manage in our code
        // the deletion of all the expenditures before the delete of the group
        builder.Entity<Expenditure>()
            .HasOne(e => e.Group)
            .WithMany(g => g.Expenditures)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        // if a user is delete,
        // the expenditure will not be deleted.
        // ApplicationUserId will only be set to the default value (empty string).
        builder.Entity<Expenditure>()
            .HasOne(e => e.ApplicationUser)
            .WithMany(g => g.Expenditures)
            .HasForeignKey(e => e.ApplicationUserId)
            .OnDelete(DeleteBehavior.SetNull);

        // same, but it is CategoryId that will be set to null.
        builder.Entity<Expenditure>()
            .HasOne(e => e.Category)
            .WithMany(g => g.Expenditures)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        var hasher = new PasswordHasher<IdentityUser>();

        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "1",
                Firstname = "Helena",
                Lastname = "Yamasaki",
                UserName = "Lele",
                NormalizedUserName = "LELE",
                Email = "helena@gmail.com",
                NormalizedEmail = "HELENA@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Helena.123")
            },
            new ApplicationUser
            {
                Id = "2",
                Firstname = "Pauline",
                Lastname = "Bouyssou",
                UserName = "popo",
                NormalizedUserName = "POPO",
                Email = "pauline@gmail.com",
                NormalizedEmail = "PAULINE@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Pauline.123")
            },
            new ApplicationUser
            {
                Id = "3",
                Firstname = "Nolan",
                Lastname = "De Puydt",
                UserName = "Nono",
                NormalizedUserName = "NONO",
                Email = "nolan@gmail.com",
                NormalizedEmail = "NOLAN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Nolan.123")
            },
            new ApplicationUser
            {
                Id = "4",
                Firstname = "Kevin",
                Lastname = "Osei Yaw",
                UserName = "Kev",
                NormalizedUserName = "KEV",
                Email = "kevin@gmail.com",
                NormalizedEmail = "KEVIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Kevin.123")
            });

        builder.Entity<Group>().HasData(
            new Group()
            {
                Id = 1,
                Name = "Coloc"
            },
            new Group()
            {
                Id = 2,
                Name = "Vacances d'été"
            },
            new Group()
            {
                Id = 3,
                Name = "Voyage scolaire"
            });

        builder.Entity<Category>().HasData(
            new Category()
            {
                Id = 1,
                Name = "Courses",
                GroupId = 1
            },
            new Category()
            {
                Id = 2,
                Name = "Plaisirs coupables",
                GroupId = 1
            },
            new Category()
            {
                Id = 3,
                Name = "Restaurants",
                GroupId = 1
            });

        builder.Entity<Expenditure>().HasData(
            new Expenditure()
            {
                Id = 1,
                Name = "Courses Auchan",
                Amount = 10.5,
                Date = new DateTime(2024, 04, 17),
                ApplicationUserId = "1",
                GroupId = 1
            },
            new Expenditure()
            {
                Id = 2,
                Name = "Restaurant Toulouse",
                Amount = 60,
                Date = new DateTime(2024, 05, 01),
                ApplicationUserId = "1",
                CategoryId = 3,
                GroupId = 1
            },
            new Expenditure()
            {
                Id = 3,
                Name = "Café Capitole",
                Amount = 16.5,
                Date = new DateTime(2024, 04, 29),
                ApplicationUserId = "2",
                GroupId = 2
            },
            new Expenditure()
            {
                Id = 4,
                Name = "Bonbons",
                Amount = 6.9,
                Date = new DateTime(2024, 05, 10),
                ApplicationUserId = "3",
                CategoryId = 2,
                GroupId = 1
            });
    }
}

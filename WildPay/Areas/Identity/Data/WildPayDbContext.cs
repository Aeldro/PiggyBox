using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
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

        // if a user is delete,
        // the expenditure will not be deleted.
        // ApplicationUserId will only be set to null.
        builder.Entity<Expenditure>()
            .HasOne(e => e.Payer)
            .WithMany(g => g.ExpendituresPayer)
            .HasForeignKey(e => e.PayerId)
            .OnDelete(DeleteBehavior.SetNull);

        // same, but it is CategoryId that will be set to null.
        builder.Entity<Expenditure>()
            .HasOne(e => e.Category)
            .WithMany(g => g.Expenditures)
            .HasForeignKey(e => e.CategoryId)
            .OnDelete(DeleteBehavior.SetNull);

        // Cascade doesn't work so we will have to manage in our code
        // the deletion of all the expenditures before the delete of the group
        builder.Entity<Expenditure>()
            .HasOne(e => e.Group)
            .WithMany(g => g.Expenditures)
            .HasForeignKey(e => e.GroupId)
            .OnDelete(DeleteBehavior.Restrict);

        var hasher = new PasswordHasher<ApplicationUser>();

        builder.Entity<ApplicationUser>().HasData(
            new ApplicationUser
            {
                Id = "1",
                Firstname = "Helena",
                Lastname = "Yamasaki",
                UserName = "helena@gmail.com",
                NormalizedUserName = "HELENA@GMAIL.COM",
                Email = "helena@gmail.com",
                NormalizedEmail = "HELENA@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Helena.123"),
                SecurityStamp = Guid.NewGuid().ToString("D")
            },
            new ApplicationUser
            {
                Id = "2",
                Firstname = "Pauline",
                Lastname = "Bouyssou",
                UserName = "pauline@gmail.com",
                NormalizedUserName = "PAULINE@GMAIL.COM",
                Email = "pauline@gmail.com",
                NormalizedEmail = "PAULINE@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Pauline.123"),
                SecurityStamp = Guid.NewGuid().ToString("D")
            },
            new ApplicationUser
            {
                Id = "3",
                Firstname = "Nolan",
                Lastname = "De Puydt",
                UserName = "nolan@gmail.com",
                NormalizedUserName = "NOLAN@GMAIL.COM",
                Email = "nolan@gmail.com",
                NormalizedEmail = "NOLAN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Nolan.123"),
                SecurityStamp = Guid.NewGuid().ToString("D")
            },
            new ApplicationUser
            {
                Id = "4",
                Firstname = "Kevin",
                Lastname = "Osei Yaw",
                UserName = "kevin@gmail.com",
                NormalizedUserName = "KEVIN@GMAIL.COM",
                Email = "kevin@gmail.com",
                NormalizedEmail = "KEVIN@GMAIL.COM",
                EmailConfirmed = true,
                PasswordHash = hasher.HashPassword(null, "Kevin.123"),
                SecurityStamp = Guid.NewGuid().ToString("D")
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
                PayerId = "1",
                GroupId = 1
            },
            new Expenditure()
            {
                Id = 2,
                Name = "Restaurant Toulouse",
                Amount = 60,
                Date = new DateTime(2024, 05, 01),
                PayerId = "1",
                CategoryId = 3,
                GroupId = 1
            },
            new Expenditure()
            {
                Id = 3,
                Name = "Café Capitole",
                Amount = 16.5,
                Date = new DateTime(2024, 04, 29),
                PayerId = "2",
                GroupId = 2
            },
            new Expenditure()
            {
                Id = 4,
                Name = "Bonbons",
                Amount = 6.9,
                Date = new DateTime(2024, 05, 10),
                PayerId = "3",
                CategoryId = 2,
                GroupId = 1
            });
    }

    // manage the deletion of the expenditures when a group is deleted
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        // select all the groups that are to be deleted
        List<Group> deletedGroups = ChangeTracker.Entries<Group>()
            .Where(e => e.State == EntityState.Deleted)
            .Select(e => e.Entity)
            .ToList();

        // Delete all the records in Expenditures
        // that depends on the deleted groups
        foreach (Group group in deletedGroups)
        {
            List<Expenditure> expenditures = await Expenditures
            .Where(e => e.GroupId == group.Id)
            .ToListAsync(cancellationToken);

            Expenditures.RemoveRange(expenditures);
        }

        // call the SaveChanges() function from DbContext
        return await base.SaveChangesAsync(cancellationToken);
    }
}
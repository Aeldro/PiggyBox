using Microsoft.EntityFrameworkCore;
using WildPay.Models.Entities;
using WildPay.Data;
using WildPay.Interfaces;
using WildPay.Repositories;
using WildPay.Services;
using WildPay.Services.Interfaces;

namespace WildPay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            var connectionString = builder.Configuration.GetConnectionString("WildPayDbContextConnection") ?? throw new InvalidOperationException("Connection string 'WildPayDbContextConnection' not found.");

            builder.Services.AddDbContext<WildPayDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<WildPayDbContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            builder.Services.AddScoped<IExpenditureRepository, ExpenditureRepository>();
            builder.Services.AddScoped<IBalanceService, BalanceService>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
            builder.Services.AddScoped<IExpenditureService, ExpenditureService>();
            builder.Services.AddScoped<IDropDownService, DropDownService>();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();

            app.UseRouting();

            app.UseAuthentication();
            app.UseAuthorization();

            app.MapRazorPages();

            app.MapControllerRoute(
                name: "default",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.MapControllerRoute(
                name: "defaultWithQueryString",
                pattern: "{controller=Home}/{action=Index}/{id?}");

            app.Run();
        }
    }
}

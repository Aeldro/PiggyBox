using Microsoft.EntityFrameworkCore;
using WildPay.Models.Entities;
using WildPay.Data;
using WildPay.Interfaces;
using WildPay.Repositories;

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
            // to add Expenditure Repository

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (!app.Environment.IsDevelopment())
            {
                app.UseExceptionHandler("/Home/Error");
                // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
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

            app.Run();
        }
    }
}

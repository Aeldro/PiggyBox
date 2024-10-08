using dotenv.net;
using CloudinaryDotNet;
using Microsoft.EntityFrameworkCore;
using WildPay.Models.Entities;
using WildPay.Data;
using WildPay.Interfaces;
using WildPay.Repositories;
using WildPay.Services;
using WildPay.Services.Interfaces;
using Microsoft.AspNetCore.Identity.UI.Services;
using WildPay.Repositories.Interfaces;

namespace WildPay
{
    public class Program
    {
        public static void Main(string[] args)
        {
            // Set the cloudinary credentials
            DotEnv.Load(options: new DotEnvOptions(probeForEnv: true));
            Cloudinary cloudinary = new Cloudinary(Environment.GetEnvironmentVariable("CLOUDINARY_URL"));
            cloudinary.Api.Secure = true;

            var builder = WebApplication.CreateBuilder(args);

            var connectionString = builder.Configuration.GetConnectionString("WildPayDbContextConnection") ?? throw new InvalidOperationException("Connection string 'WildPayDbContextConnection' not found.");

            builder.Services.AddDbContext<WildPayDbContext>(options => options.UseSqlServer(connectionString));

            builder.Services.AddDefaultIdentity<ApplicationUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<WildPayDbContext>();

            // Add services to the container.
            builder.Services.AddControllersWithViews();

            builder.Services.AddRazorPages();

            builder.Services.AddSingleton(cloudinary);

            builder.Services.AddScoped<IGroupRepository, GroupRepository>();
            builder.Services.AddScoped<IExpenditureRepository, ExpenditureRepository>();
            builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();

            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<IBalanceService, BalanceService>();
            builder.Services.AddScoped<IExpenditureService, ExpenditureService>();
            builder.Services.AddScoped<IDropDownService, DropDownService>();
            builder.Services.AddScoped<IVerificationService, VerificationService>();
            builder.Services.AddScoped<IEmailSender, EmailSender>();
            builder.Services.AddScoped<ICloudinaryService, CloudinaryService>();

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

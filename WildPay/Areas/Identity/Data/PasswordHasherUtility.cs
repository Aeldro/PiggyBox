using Microsoft.AspNetCore.Identity;
using WildPay.Models.Entities;

namespace WildPay.Areas.Identity.Data
{
    public static class PasswordHasherUtility
    {
        public static string HashPassword(string password)
        {
            var passwordHasher = new PasswordHasher<ApplicationUser>();
            var dummyUser = new ApplicationUser();

            return passwordHasher.HashPassword(dummyUser, password);
        }
    }
}

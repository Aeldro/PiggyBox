using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using WildPay.Services.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Services
{
    public class VerificationService : IVerificationService
    {
        public bool IsUserBelongsToGroup(string userId, Group group)
        {
            //Verify if the User belongs to the group, else we block the access
            if (userId is null || group.ApplicationUsers.FirstOrDefault(el => el.Id == userId) is null) return false;
            else return true;
        }
    }
}

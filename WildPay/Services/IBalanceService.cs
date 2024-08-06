﻿using WildPay.Models;
using WildPay.Models.Entities;

namespace WildPay.Services
{
    public interface IBalanceService
    {
        Task<Dictionary<ApplicationUser, double>> CalculateMembersBalance(Group group);
        Task<GroupBalance> CalculateDebtsList(GroupBalance groupBalance, Group group);
    }
}
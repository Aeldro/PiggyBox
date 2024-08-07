using WildPay.Models.Entities;
using WildPay.Models;
using WildPay.Interfaces;
using WildPay.Services.Interfaces;

namespace WildPay.Services
{
    public class BalanceService : IBalanceService
    {
        private readonly IExpenditureRepository _expenditureRepository;

        public BalanceService (IExpenditureRepository expenditureRepository)
        {
            _expenditureRepository = expenditureRepository;
        }

        /// <summary>
        /// Calculate the balance of each member (positive, negative or 0)
        /// </summary>
        /// <param name="group">the group in which the user is</param>
        /// <returns>A dictionary containing the users in the group as keys and their balance as values</returns>
        public async Task<Dictionary<ApplicationUser, double>> CalculateMembersBalance(Group group)
        {
            Dictionary<ApplicationUser, double> membersBalance = new Dictionary<ApplicationUser, double>();

            foreach (ApplicationUser member in group.ApplicationUsers) membersBalance.Add(member, 0);

            foreach (Expenditure expenditure in group.Expenditures)
            {
                if (expenditure.PayerId is not null && expenditure.Payer is not null)
                {
                    int numberOfContributors = await _expenditureRepository.GetContributorsCountAsync(expenditure.Id);

                    // we don't take into account in our balances the expenditures that has no contributors
                    if (numberOfContributors == 0) continue;

                    double expenditureContributionPerPerson = expenditure.Amount / numberOfContributors;

                    membersBalance[expenditure.Payer] += expenditure.Amount;

                    foreach (ApplicationUser contributor in expenditure.RefundContributors)
                    {
                        membersBalance[contributor] -= expenditureContributionPerPerson;
                    }
                }
            }

            return membersBalance;
        }

        /// <summary>
        /// Calculate the debts from the balance dictionary
        /// </summary>
        /// <param name="groupBalance">customed object with the informations for the View ListGroupBalances</param>
        /// <param name="group">the group in which the user is</param>
        /// <returns></returns>
        public GroupBalance CalculateDebtsList(GroupBalance groupBalance, Group group)
        {
            // must create a new dictionary to modify the dictionary without modifying groupBalance.UsersBalance (values useful into the View)
            // otherwise it makes a reference copy, not a value copy
            var membersBalances = new Dictionary<ApplicationUser, double>(groupBalance.UsersBalance);

            // Looping until everyone gets refunded
            while (membersBalances.Any(user => user.Value > 0.01))
            {
                // Find the user with the highest balance
                KeyValuePair<ApplicationUser, double> highestCreditor = membersBalances.OrderByDescending(user => user.Value).First();
                ApplicationUser Creditor = group.ApplicationUsers.Find(user => user.Id == highestCreditor.Key.Id);

                // Find the user with the lowest balance
                KeyValuePair<ApplicationUser, double> highestDebitor = membersBalances.OrderBy(user => user.Value).First();
                ApplicationUser Debitor = group.ApplicationUsers.Find(user => user.Id == highestDebitor.Key.Id);

                // will be used in Debt
                double amount = 0;

                if (highestCreditor.Value >= Math.Abs(highestDebitor.Value))
                {
                    // Update the balance of both members
                    amount = Math.Abs(highestDebitor.Value);

                    // the new balance of the highest creditor:
                    // its actual value - the value of the highest debitor
                    membersBalances[highestCreditor.Key] = highestCreditor.Value - Math.Abs(highestDebitor.Value);

                    // the new balance of the highest debitor: 0
                    // he pays the totality of its debt to the current creditor
                    membersBalances[highestDebitor.Key] = 0;
                }
                else
                {
                    // the amount for the creditor to pay back is the total
                    // amount due to the current highest creditor
                    amount = highestCreditor.Value;

                    membersBalances[highestCreditor.Key] = 0;
                    membersBalances[highestDebitor.Key] = highestDebitor.Value + Math.Abs(amount);
                }

                // Debt is used to store informations about every
                // creditor-debitor-amount for the View
                Debt debt = new Debt
                {
                    Amount = amount,
                    Debtor = Debitor,
                    Creditor = Creditor
                };

                groupBalance.Debts.Add(debt);
            }

            return groupBalance;
        }
    }
}

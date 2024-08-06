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

                    // we don't take into account in our balances the expenditures
                    // that has no contributors
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
        /// 
        /// </summary>
        /// <param name="groupBalance">customed object with the informations for the View Balance</param>
        /// <param name="group"></param>
        /// <returns></returns>
        public async Task<GroupBalance> CalculateDebtsList(GroupBalance groupBalance, Group group)
        {
            //Looping until everyone gets refunded
            while (groupBalance.UsersBalance.Any(user => user.Value > 0.01) && groupBalance.UsersBalance.Any(user => user.Value < 0.01))
            {
                //Match the member who has the highest balance to the member who has the lowest balance
                KeyValuePair<ApplicationUser, double> highestCreditor = groupBalance.UsersBalance.OrderByDescending(el => el.Value).First();
                ApplicationUser positiveBalanceUser = group.ApplicationUsers.Find(el => el.Id == highestCreditor.Key.Id);

                KeyValuePair<ApplicationUser, double> highestDebitor = negativeBalanceMembers.OrderBy(el => el.Value).First();
                ApplicationUser negativeBalanceUser = group.ApplicationUsers.Find(el => el.Id == highestDebitor.Key.Id);

                double amount = 0;

                //Update the balance of both members
                if (highestCreditor.Value >= highestDebitor.Value)
                {
                    amount = -highestDebitor.Value;

                    positiveBalanceMembers[highestCreditor.Key] = highestCreditor.Value + highestDebitor.Value;

                    negativeBalanceMembers[highestDebitor.Key] = highestDebitor.Value - highestDebitor.Value;
                }
                else if (highestCreditor.Value < highestDebitor.Value)
                {
                    amount = highestCreditor.Value;

                    negativeBalanceMembers[highestDebitor.Key] = highestDebitor.Value + highestCreditor.Value;

                    positiveBalanceMembers[highestCreditor.Key] = highestCreditor.Value - highestCreditor.Value;
                }

                //Make the debt
                Debt debt = new Debt(negativeBalanceUser, positiveBalanceUser, amount);
                groupBalance.Debts.Add(debt);
            }

            return groupBalance;
        }
    }
}

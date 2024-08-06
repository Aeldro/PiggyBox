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

        //Used in the GroupBalances method to calculate the balance of each member
        public async Task<Dictionary<ApplicationUser, double>> CalculateMembersBalance(Group group)
        {
            Dictionary<ApplicationUser, double> membersBalance = new Dictionary<ApplicationUser, double>();

            foreach (ApplicationUser member in group.ApplicationUsers) membersBalance.Add(member, 0);

            foreach (Expenditure expenditure in group.Expenditures)
            {
                if (expenditure.PayerId is not null && expenditure.Payer is not null)
                {
                    double expenditureContributionPerPerson = expenditure.Amount / await _expenditureRepository.GetContributorsCount(expenditure.Id);
                    membersBalance[expenditure.Payer] = membersBalance[expenditure.Payer] + expenditure.Amount;
                    foreach (ApplicationUser contributor in expenditure.RefundContributors)
                    {
                        membersBalance[contributor] = membersBalance[contributor] - expenditureContributionPerPerson;
                    }
                }
            }

            return membersBalance;
        }

        //Used in the GroupBalances method to calculate who must pay who
        public async Task<GroupBalance> CalculateDebtsList(GroupBalance groupBalance, Group group)
        {
            //Split the users balances into two parts : positives balances and negative balances. This aims to make the further calculation easier
            Dictionary<ApplicationUser, double> positiveBalanceMembers = new Dictionary<ApplicationUser, double>();
            Dictionary<ApplicationUser, double> negativeBalanceMembers = new Dictionary<ApplicationUser, double>();
            foreach (KeyValuePair<ApplicationUser, double> member in groupBalance.UsersBalance)
            {
                if (member.Value < 0) negativeBalanceMembers.Add(member.Key, member.Value);
                else if (member.Value > 0) positiveBalanceMembers.Add(member.Key, member.Value);
            }

            //Looping until everyone gets refunded
            while (positiveBalanceMembers.Any(el => el.Value > 0.01) && negativeBalanceMembers.Any(el => el.Value < 0.01))
            {
                //Match the member who has the highest balance to the member who has the lowest balance
                KeyValuePair<ApplicationUser, double> positiveBalanceMember = positiveBalanceMembers.OrderByDescending(el => el.Value).First();
                ApplicationUser positiveBalanceUser = group.ApplicationUsers.Find(el => el.Id == positiveBalanceMember.Key.Id);
                KeyValuePair<ApplicationUser, double> negativeBalanceMember = negativeBalanceMembers.OrderBy(el => el.Value).First();
                ApplicationUser negativeBalanceUser = group.ApplicationUsers.Find(el => el.Id == negativeBalanceMember.Key.Id);

                double amount = 0;

                //Update the balance of both members
                if (positiveBalanceMember.Value >= negativeBalanceMember.Value)
                {
                    amount = -negativeBalanceMember.Value;
                    positiveBalanceMembers[positiveBalanceMember.Key] = positiveBalanceMember.Value + negativeBalanceMember.Value;
                    negativeBalanceMembers[negativeBalanceMember.Key] = negativeBalanceMember.Value - negativeBalanceMember.Value;
                }
                else if (positiveBalanceMember.Value < negativeBalanceMember.Value)
                {
                    amount = positiveBalanceMember.Value;
                    negativeBalanceMembers[negativeBalanceMember.Key] = negativeBalanceMember.Value + positiveBalanceMember.Value;
                    positiveBalanceMembers[positiveBalanceMember.Key] = positiveBalanceMember.Value - positiveBalanceMember.Value;
                }

                //Make the debt
                Debt debt = new Debt(negativeBalanceUser, positiveBalanceUser, amount);
                groupBalance.Debts.Add(debt);
            }

            return groupBalance;
        }
    }
}

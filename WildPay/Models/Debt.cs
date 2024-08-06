using System.ComponentModel.DataAnnotations;
using WildPay.Models.Entities;

namespace WildPay.Models
{
    public class Debt
    {
        public double Amount { get; set; }
        public ApplicationUser? Debtor { get; set; }
        public ApplicationUser? Creditor { get; set; }

        public Debt(ApplicationUser debtor, ApplicationUser creditor, double amount)
        {
            Debtor = debtor;
            Creditor = creditor;
            Amount = amount;
        }
    }
}

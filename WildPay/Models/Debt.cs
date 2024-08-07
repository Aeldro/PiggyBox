using WildPay.Models.Entities;

namespace WildPay.Models
{
    public class Debt
    {
        public double Amount { get; set; }
        public ApplicationUser? Debtor { get; set; }
        public ApplicationUser? Creditor { get; set; }
    }
}

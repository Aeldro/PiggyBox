using WildPay.Models.Entities;

namespace WildPay.Models
{
    public class GroupBalance
    {
        public Group? Group { get; set; }
        public double TotalAmount { get; set; }
        public List<Debt> Debts { get; set; } = new List<Debt>();
        public string Message { get; set; } = string.Empty;
        public Dictionary<ApplicationUser, double> UsersBalance { get; set; } = new Dictionary<ApplicationUser, double>();
    }
}

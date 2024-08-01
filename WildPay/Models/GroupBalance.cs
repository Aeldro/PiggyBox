using System.ComponentModel.DataAnnotations;
using WildPay.Models.Entities;

namespace WildPay.Models
{
    public class GroupBalance
    {
        public Group ?Group { get; set; }
        public double TotalAmount { get; set; }
        public List<Debt> Debts { get; set; } = new List<Debt>();
        public string Message { get; set; } = string.Empty;
        public Dictionary<string, double> UsersBalance { get; set; } = new Dictionary<string, double>();
    }
}

using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        [MaxLength(25)]
        public string Firstname { get; set; } = string.Empty;

        [Required]
        [MaxLength(25)]
        public string Lastname { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public List<Group> Groups { get; set; } = new List<Group>();
        public List<Expenditure> ExpendituresPayer { get; set; } = new List<Expenditure>();
        public List<Expenditure> RefundContributions { get; set; } = new List<Expenditure>();
    }
}

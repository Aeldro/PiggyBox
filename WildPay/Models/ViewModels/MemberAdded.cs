using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.ViewModels
{
    public class MemberAdded
    {
        public int GroupId { get; set; }

        [EmailAddress]
        public string Email { get; set; } = string.Empty;
    }
}

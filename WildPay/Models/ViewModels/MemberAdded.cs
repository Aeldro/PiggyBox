using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.ViewModels
{
    public class MemberAdded
    {
        public int GroupId { get; set; }

        [EmailAddress(ErrorMessage = "L'adresse mail saisie n'est pas valide.")]
        public string Email { get; set; } = string.Empty;
    }
}

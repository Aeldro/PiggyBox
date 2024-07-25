using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.Entities
{
    public class ApplicationUser : IdentityUser
    {
        [Required]
        public string Firstname { get; set; } = string.Empty;
        public string Lastname { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public List<Group> Groups { get; set; } = new List<Group>();
    }
}

using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.Entities
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public List<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
        public List<Expenditure> Expenditures { get; set; } = new List<Expenditure>();
        public List<Category> Categories { get; set; } = new List<Category>();
    }
}

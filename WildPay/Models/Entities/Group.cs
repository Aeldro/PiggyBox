using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.Entities
{
    public class Group
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public string Image { get; set; } = string.Empty;
        public List<ApplicationUser> ApplicationUsers { get; set; } = new List<ApplicationUser>();
        public ICollection<Expenditure> Expenditures { get; set; } = new List<Expenditure>();
        public ICollection<Category> Categories { get; set; } = new List<Category>();
    }
}

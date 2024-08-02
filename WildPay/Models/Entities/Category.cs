using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.Entities
{
    public class Category
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(30)]
        public string Name { get; set; } = string.Empty;
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public List<Expenditure> Expenditures { get; set; } = new List<Expenditure>();
    }
}

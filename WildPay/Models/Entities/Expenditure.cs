using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.Entities
{
    public class Expenditure
    {
        public int Id { get; set; }

        [Required]
        public string Name { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DateTime Date { get; set; }
        public string? ApplicationUserId { get; set; } = null;
        public ApplicationUser? ApplicationUser { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }
        public int? GroupId { get; set; }
        public Group? Group { get; set; }
    }
}

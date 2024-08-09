using System.ComponentModel.DataAnnotations;

namespace WildPay.Models.Entities
{
    public class Expenditure
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Le nom est obligatoire.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Le montant est obligatoire.")]
        [Range(0.01, int.MaxValue, ErrorMessage = "Le montant de la dépense doit être supérieur à 0.01.")]
        [DisplayFormat(DataFormatString = "{0:F2}", ApplyFormatInEditMode = true)]
        public double Amount { get; set; }

        [Required(ErrorMessage = "La date est obligatoire.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        public string? PayerId { get; set; } = null;
        public ApplicationUser? Payer { get; set; }
        public int? CategoryId { get; set; } = null;
        public Category? Category { get; set; }
        public int GroupId { get; set; }
        public Group? Group { get; set; }
        public List<ApplicationUser> RefundContributors { get; set; } = new List<ApplicationUser>();
    }
}

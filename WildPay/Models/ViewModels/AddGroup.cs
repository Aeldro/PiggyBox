using System.ComponentModel.DataAnnotations;
using WildPay.Helpers;

namespace WildPay.Models.ViewModels
{
    public class AddGroup
    {
        [Required(ErrorMessage = "Le champ nom doit contenir au moins un caractère.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [Display(Name = "Photo de profil"),]
        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile? Image { get; set; } = null;

        public string? ImageUrl { get; set; }
        public string? ImagePublicId { get; set; }
    }
}

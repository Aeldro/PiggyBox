using System.ComponentModel.DataAnnotations;
using WildPay.Helpers;

namespace WildPay.Models.ViewModels
{
    public class GroupInfos
    {
        [Required]
        public int GroupId { get; set; }

        [Required(ErrorMessage = "Le champ nom doit contenir au moins un caractère.")]
        [MaxLength(50)]
        public string Name { get; set; } = string.Empty;

        [AllowedExtensions(new string[] { ".jpg", ".jpeg", ".png" })]
        [MaxFileSize(2 * 1024 * 1024)]
        public IFormFile? Image { get; set; }
    }
}

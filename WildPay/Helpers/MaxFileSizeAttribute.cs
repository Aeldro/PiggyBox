using System.ComponentModel.DataAnnotations;

namespace WildPay.Helpers
{
    public class MaxFileSizeAttribute : ValidationAttribute
    {
        private readonly int _maxFileSize;

        public MaxFileSizeAttribute(int maxFileSize)
        {
            _maxFileSize = maxFileSize;
        }

        protected override ValidationResult IsValid(object? value, ValidationContext validationContext)
        {
            IFormFile? file = value as IFormFile;

            if (file != null)
            {
                if (file.Length > _maxFileSize)
                {
                    return new ValidationResult(GetErrorMessage());
                }
            }

            return ValidationResult.Success;
        }

        public string GetErrorMessage()
        {
            return $"La taille maximale de l'image est de 2Mo";
        }
    }
}

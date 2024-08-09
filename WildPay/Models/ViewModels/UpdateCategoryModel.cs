using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WildPay.Models.Entities;

namespace WildPay.Models.ViewModels
{
    public class UpdateCategoryModel
    {
        public Category? CategoryToUpdate { get; set; }

    }
}

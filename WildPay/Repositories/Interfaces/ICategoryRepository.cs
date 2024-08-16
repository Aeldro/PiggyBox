using WildPay.Models.Entities;

namespace WildPay.Repositories.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync(int groupId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task UpdateCategoryAsync(Category category);
        Task AddCategoryAsync(Category category);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}
using WildPay.Models.Entities;

namespace WildPay.Interfaces
{
    public interface ICategoryRepository
    {
        Task<List<Category>> GetCategoriesAsync(int groupId);
        Task<Category?> GetCategoryByIdAsync(int categoryId);
        Task EditCategoryAsync(Category category);
        Task AddCategoryAsync(string name, int groupId);
        Task<bool> DeleteCategoryAsync(int categoryId);
    }
}

using Microsoft.EntityFrameworkCore;
using WildPay.Data;
using WildPay.Interfaces;
using WildPay.Models.Entities;

namespace WildPay.Repositories
{
    public class CategoryRepository : ICategoryRepository
    {
        private readonly WildPayDbContext _context;

        public CategoryRepository(WildPayDbContext context)
        {
            _context = context;
        }

        public async Task<List<Category>> GetCategoriesAsync(int groupId)
        {
            List<Category> categories = await _context.Categories
                .Where(c => c.GroupId == groupId)
                .Select(category => category)
                .ToListAsync();

            return categories;
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            Category? category = await _context.Categories.FindAsync(categoryId);
            return category;
        }

        public async Task UpdateCategoryAsync(Category category)
        {
            if (await _context.Categories.FindAsync(category.Id) is Category found && found != null)
            {
                found.Name = category.Name;

                await _context.SaveChangesAsync();
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            Category newCategory = new Category
            {
                Name = category.Name,
                GroupId = category.GroupId,
                Group = category.Group,
            };

            await _context.Categories.AddAsync(newCategory);
            await _context.SaveChangesAsync();
        }
        public async Task EditCategoryAsync(Category category)
        {
            if (await _context.Categories.FindAsync(category.Id) is Category found && found != null)
            {
                found.Name = category.Name;
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            if (await _context.Categories.FindAsync(categoryId) is Category found)
            {
                _context.Categories.Remove(found);

                // This method is override in WildPayDbContext
                // and delete all the expenditures linked to the removed group
                // before deleting the group
                await _context.SaveChangesAsync();
                return true;
            }

            return false;
        }
      
    }
}
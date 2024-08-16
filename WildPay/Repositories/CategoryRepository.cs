using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using WildPay.Data;
using WildPay.Exceptions;
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
            try
            {
                List<Category> categories = await _context.Categories
                .Where(c => c.GroupId == groupId)
                .Select(category => category)
                .ToListAsync();

                return categories;
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        public async Task<Category?> GetCategoryByIdAsync(int categoryId)
        {
            try
            {
                Category? category = await _context.Categories.FindAsync(categoryId);

                if (category == null) throw new NullException();

                return category;
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        public async Task UpdateCategoryAsync(Category updatedCategory)
        {
            try
            {
                Category? categoryToUpdate = await GetCategoryByIdAsync(updatedCategory.Id);

                if (categoryToUpdate == null) throw new NullException();

                categoryToUpdate.Name = updatedCategory.Name;

                await _context.SaveChangesAsync();
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        public async Task AddCategoryAsync(Category category)
        {
            try
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
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

        public async Task<bool> DeleteCategoryAsync(int categoryId)
        {
            try
            {
                Category? categoryToDelete = await GetCategoryByIdAsync(categoryId);

                if (categoryToDelete == null) throw new NullException();

                _context.Categories.Remove(categoryToDelete);

                // This method is override in WildPayDbContext
                // and delete all the expenditures linked to the removed group
                // before deleting the group
                await _context.SaveChangesAsync();
                return true;
            }
            catch (SqlException sqlEx)
            {
                throw new DatabaseException();
            }
        }

    }
}
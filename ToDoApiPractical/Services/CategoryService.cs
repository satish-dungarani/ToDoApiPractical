using Microsoft.EntityFrameworkCore;
using ToDoApiPractical.DataContext;
using ToDoApiPractical.Domains;
using ToDoApiPractical.Models;
using ToDoApiPractical.Services;

namespace ToDoApiPractical.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly AppDbContext _context;

        public CategoryService(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<CategoryModel>> GetAllAsync()
        {
            return await _context.Categories
                .Select(c => new CategoryModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    ParentCategoryId = c.ParentCategoryId
                })
                .ToListAsync();
        }

        public async Task<CategoryModel?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Where(c => c.Id == id)
                .Select(c => new CategoryModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    ParentCategoryId = c.ParentCategoryId
                })
                .FirstOrDefaultAsync();
        }

        public async Task<CategoryModel> SaveAsync(CategoryModel model)
        {
            var categoryEntity = await _context.Categories.FindAsync(model.Id);

            if (categoryEntity != null)
            {
                categoryEntity.Title = model.Title;
                categoryEntity.ParentCategoryId = model.ParentCategoryId;
                _context.Categories.Update(categoryEntity);
            }
            else
            {
                categoryEntity = new Category
                {
                    Title = model.Title,
                    ParentCategoryId = model.ParentCategoryId
                };
                await _context.Categories.AddAsync(categoryEntity);
            }

            await _context.SaveChangesAsync();
            model.Id = categoryEntity.Id;
            return model;
        }

        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
                throw new KeyNotFoundException("Category not found.");

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}
using ToDoApiPractical.Models;

namespace ToDoApiPractical.Services
{
    public interface ICategoryService
    {
        Task<IEnumerable<CategoryModel>> GetAllAsync();
        Task<CategoryModel?> GetByIdAsync(int id);
        Task<CategoryModel> SaveAsync(CategoryModel model);
        Task DeleteAsync(int id);
    }
}

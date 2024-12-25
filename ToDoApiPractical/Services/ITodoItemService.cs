using ToDoApiPractical.Models;

namespace ToDoApiPractical.Services
{
    public interface ITodoItemService
    {
        Task<IEnumerable<TodoItemModel>> GetAllAsync();
        Task<TodoItemModel?> GetByIdAsync(int id);
        Task<TodoItemModel> SaveAsync(TodoItemModel model);
        Task DeleteAsync(int id);
        Task<IEnumerable<TodoItemModel>> SearchAsync(string? todo, int? priority, DateTime? dueDate);
        Task<IEnumerable<dynamic>> GetCombinedTodoListWeatherAsync();
    }
}

using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using ToDoApiPractical.DataContext;
using ToDoApiPractical.Domains;
using ToDoApiPractical.Models;

namespace ToDoApiPractical.Services
{
    public class TodoItemService : ITodoItemService
    {
        private readonly AppDbContext _context;
        private readonly HttpClient _httpClient;
        private readonly string _weatherApiBaseUrl;
        private readonly string _weatherApiKey;

        public TodoItemService(AppDbContext context, HttpClient httpClient, IConfiguration configuration)
        {
            _context = context;
            _httpClient = httpClient;
            _weatherApiBaseUrl = configuration["WeatherApi:BaseUrl"];
            _weatherApiKey = configuration["WeatherApi:ApiKey"];
        }

        public async Task<IEnumerable<TodoItemModel>> GetAllAsync()
        {
            return await _context.TodoItems
                .Include(td => td.Category)
                .Select(td => new TodoItemModel
                {
                    Id = td.Id,
                    ToDo = td.ToDo,
                    Priority = td.Priority,
                    Lat = td.Lat,
                    Lon = td.Lon,
                    DueDate = td.DueDate,
                    CategoryId = td.CategoryId,
                    Category = td.Category != null ? td.Category.Title : null
                }).ToListAsync();
        }

        public async Task<TodoItemModel?> GetByIdAsync(int id)
        {
            return await _context.TodoItems
                .Include(td => td.Category)
                .Where(td => td.Id == id)
                .Select(td => new TodoItemModel
                {
                    Id = td.Id,
                    ToDo = td.ToDo,
                    Priority = td.Priority,
                    Lat = td.Lat,
                    Lon = td.Lon,
                    DueDate = td.DueDate,
                    CategoryId = td.CategoryId,
                    Category = td.Category != null ? td.Category.Title : null
                }).FirstOrDefaultAsync();
        }

        public async Task<TodoItemModel> SaveAsync(TodoItemModel model)
        {
            string category = "";

            if (model.CategoryId.HasValue)
            {
                var cat = await _context.Categories.Where(c => c.Id == model.CategoryId.Value).FirstOrDefaultAsync();
                if (cat == null)
                    throw new KeyNotFoundException("The specified category does not exist.");

                category = cat.Title;
            }

            if (model.Id > 0)
            {
                var existingTodo = await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == model.Id);
                if (existingTodo == null)
                    throw new KeyNotFoundException("Todo item not found.");

                _context.TodoItems.Update(new TodoItem
                {
                    Id = existingTodo.Id,
                    ToDo = model.ToDo,
                    Priority = model.Priority,
                    Lat = model.Lat,
                    Lon = model.Lon,
                    DueDate = model.DueDate,
                    CategoryId = model.CategoryId,
                });
            }
            else
            {
                var newTodo = new TodoItem
                {
                    ToDo = model.ToDo,
                    Priority = model.Priority,
                    Lat = model.Lat,
                    Lon = model.Lon,
                    DueDate = model.DueDate,
                    CategoryId = model.CategoryId,
                };
                await _context.TodoItems.AddAsync(newTodo);
                model.Id = newTodo.Id;
            }

            await _context.SaveChangesAsync();
            model.Category = category;
            return model;
        }


        public async Task DeleteAsync(int id)
        {
            var todo = await _context.TodoItems.FindAsync(id);
            if (todo == null)
                throw new KeyNotFoundException("Todo item not found.");

            _context.TodoItems.Remove(todo);
            await _context.SaveChangesAsync();
        }

        public async Task<IEnumerable<TodoItemModel>> SearchAsync(string? todo, int? priority, DateTime? dueDate)
        {
            var query = _context.TodoItems.Include(td => td.Category).AsQueryable();

            if (!string.IsNullOrEmpty(todo))
                query = query.Where(t => t.ToDo.Contains(todo));
            if (priority.HasValue)
                query = query.Where(t => t.Priority == priority.Value);
            if (dueDate.HasValue)
                query = query.Where(t => t.DueDate.HasValue && t.DueDate.Value.Date == dueDate.Value.Date);

            return await query.Select(td => new TodoItemModel
            {
                Id = td.Id,
                ToDo = td.ToDo,
                Priority = td.Priority,
                Lat = td.Lat,
                Lon = td.Lon,
                DueDate = td.DueDate,
                CategoryId = td.CategoryId,
                Category = td.Category != null ? td.Category.Title : null
            }).ToListAsync();
        }

        private async Task<(string Location, double Temperature, string Condition)?> FetchWeatherDataAsync(double lat, double lon)
        {
            var url = $"{_weatherApiBaseUrl}?key={_weatherApiKey}&q={lat},{lon}";
            try
            {
                var response = await _httpClient.GetFromJsonAsync<JsonDocument>(url);
                if (response == null || response.RootElement.ValueKind == JsonValueKind.Undefined)
                    return null;

                var root = response.RootElement;
                var location = root.GetProperty("location").GetProperty("name").GetString();
                var temperature = root.GetProperty("current").GetProperty("temp_c").GetDouble();
                var condition = root.GetProperty("current").GetProperty("condition").GetProperty("text").GetString();

                return (location, temperature, condition);
            }
            catch
            {
                return null;
            }
        }

        public async Task<IEnumerable<dynamic>> GetCombinedTodoListWeatherAsync()
        {
            var todos = await GetAllAsync();
            var weatherTasks = todos
                .Where(todo => todo.Lat.HasValue && todo.Lon.HasValue)
                .Select(async todo => new
                {
                    Todo = todo,
                    Weather = await FetchWeatherDataAsync(todo.Lat.Value, todo.Lon.Value)
                });

            var results = await Task.WhenAll(weatherTasks);

            return results.Select(result => new
            {
                Id = result.Todo.Id,
                ToDo = result.Todo.ToDo,
                Priority = result.Todo.Priority,
                Lat = result.Todo.Lat,
                Lon = result.Todo.Lon,
                DueDate = result.Todo.DueDate,
                CategoryId = result.Todo.CategoryId,
                Category = result.Todo.Category,
                Location = result.Weather?.Location,
                CurrentTemperature = result.Weather != null ? $"{result.Weather.Value.Temperature} C" : null,
                CurrentCondition = result.Weather?.Condition
            });
        }
    }
}
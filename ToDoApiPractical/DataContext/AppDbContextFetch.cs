using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Net.Http;
using System.Threading.Tasks;
using ToDoApiPractical.Domains;

namespace ToDoApiPractical.DataContext
{
    public static class AppDbContextFetch
    {
        public static async Task SeedTodoItemsAsync(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            if (!await context.TodoItems.AnyAsync())
            {
                using var client = httpClientFactory.CreateClient();
                var response = await client.GetStringAsync("https://dummyjson.com/todos");
                var data = JsonConvert.DeserializeObject<RootTodoResponse>(response);

                if (data?.Todos != null)
                {
                    var todoItems = data.Todos.Select(todo => new TodoItem
                    {
                        //Id = todo.Id,
                        ToDo = todo.ToDo,
                        Completed = todo.Completed,
                        Priority = 3,
                        UserId = todo.UserId
                    }).ToList();

                    await context.TodoItems.AddRangeAsync(todoItems);
                    await context.SaveChangesAsync();
                }
            }
        }
    }

    public class RootTodoResponse
    {
        public List<TodoResponse> Todos { get; set; }
    }

    public class TodoResponse
    {
        public int Id { get; set; }
        public required string ToDo { get; set; }
        public bool Completed { get; set; }
        public int UserId { get; set; }
    }
}

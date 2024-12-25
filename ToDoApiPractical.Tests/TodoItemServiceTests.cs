using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Moq;
using Moq.Protected;
using System.Net;
using ToDoApiPractical.DataContext;
using ToDoApiPractical.Domains;
using ToDoApiPractical.Models;
using ToDoApiPractical.Services;

namespace ToDoApiPractical.Tests
{
    public class TodoItemServiceTests
    {
        private readonly DbContextOptions<AppDbContext> _dbContextOptions;
        private readonly Mock<IConfiguration> _mockConfiguration;
        private readonly Mock<HttpMessageHandler> _mockHttpMessageHandler;

        public TodoItemServiceTests()
        {
            _dbContextOptions = new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;

            _mockConfiguration = new Mock<IConfiguration>();
            _mockHttpMessageHandler = new Mock<HttpMessageHandler>();
        }

        private HttpClient GetMockHttpClient(string jsonResponse, HttpStatusCode statusCode = HttpStatusCode.OK)
        {
            _mockHttpMessageHandler
                .Protected()
                .Setup<Task<HttpResponseMessage>>("SendAsync",
                    ItExpr.IsAny<HttpRequestMessage>(),
                    ItExpr.IsAny<CancellationToken>())
                .ReturnsAsync(new HttpResponseMessage
                {
                    StatusCode = statusCode,
                    Content = new StringContent(jsonResponse)
                });

            return new HttpClient(_mockHttpMessageHandler.Object);
        }

        [Fact]
        public async Task GetAllAsync_ReturnsAllTodos()
        {
            using (var context = new AppDbContext(_dbContextOptions))
            {
                context.TodoItems.Add(new TodoItem { ToDo = "Task 1", Priority = 1 });
                context.TodoItems.Add(new TodoItem { ToDo = "Task 2", Priority = 2 });
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_dbContextOptions))
            {
                var service = new TodoItemService(context, GetMockHttpClient(""), _mockConfiguration.Object);
                var result = await service.GetAllAsync();
                Assert.NotNull(result);
                Assert.Equal(2, result.Count());
            }
        }

        [Fact]
        public async Task GetByIdAsync_ReturnsCorrectTodo()
        {
            using (var context = new AppDbContext(_dbContextOptions))
            {
                context.TodoItems.Add(new TodoItem { Id = 1, ToDo = "Task 1", Priority = 1 });
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_dbContextOptions))
            {
                var service = new TodoItemService(context, GetMockHttpClient(""), _mockConfiguration.Object);
                var result = await service.GetByIdAsync(1);
                Assert.NotNull(result);
                Assert.Equal(1, result.Id);
                Assert.Equal("Task 1", result.ToDo);
            }
        }

        [Fact]
        public async Task SaveAsync_AddsNewTodoItem()
        {
            using (var context = new AppDbContext(_dbContextOptions))
            {
                var service = new TodoItemService(context, GetMockHttpClient(""), _mockConfiguration.Object);
                var newTodo = new TodoItemModel { ToDo = "Task 1", Priority = 1, Completed = true, CategoryId = null };
                var result = await service.SaveAsync(newTodo);
                Assert.NotNull(result);
                Assert.Equal("Task 1", result.ToDo);
                Assert.Single(await context.TodoItems.ToListAsync());
            }
        }

        [Fact]
        public async Task DeleteAsync_RemovesTodoItem()
        {
            using (var context = new AppDbContext(_dbContextOptions))
            {
                context.TodoItems.Add(new TodoItem { Id = 1, ToDo = "Task 1" });
                await context.SaveChangesAsync();
            }

            using (var context = new AppDbContext(_dbContextOptions))
            {
                var service = new TodoItemService(context, GetMockHttpClient(""), _mockConfiguration.Object);
                await service.DeleteAsync(1);
                Assert.Empty(await context.TodoItems.ToListAsync());
            }
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using ToDoApiPractical.Helpers;
using ToDoApiPractical.Models;
using ToDoApiPractical.Services;

namespace ToDoApiPractical.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class TodoController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;
        public TodoController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }

        [HttpPost("Save")]
        public async Task<ActionResult<TodoItemModel>> SaveTodoItem(TodoItemModel model)
        {
            try
            {
                var savedTodo = await _todoItemService.SaveAsync(model);
                return Ok(savedTodo);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(TodoController)} - {nameof(SaveTodoItem)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TodoItemModel>> GetTodoById(int id)
        {
            try
            {
                var todo = await _todoItemService.GetByIdAsync(id);
                if (todo == null) return NotFound();
                return Ok(todo);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(TodoController)} - {nameof(GetTodoById)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTodoById(int id)
        {
            try
            {
                await _todoItemService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(TodoController)} - {nameof(DeleteTodoById)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("Search")]
        public async Task<ActionResult<IEnumerable<TodoItemModel>>> SearchTodoItems([FromQuery] string? title,[FromQuery] int? priority,[FromQuery] DateTime? dueDate)
        {
            try
            {
                var result = await _todoItemService.SearchAsync(title, priority, dueDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(TodoController)} - {nameof(SearchTodoItems)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
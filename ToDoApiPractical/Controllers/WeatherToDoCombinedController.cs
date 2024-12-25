using Microsoft.AspNetCore.Mvc;
using Microsoft.VisualBasic;
using ToDoApiPractical.Helpers;
using ToDoApiPractical.Services;

namespace ToDoApiPractical.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class WeatherToDoCombinedController : ControllerBase
    {
        private readonly ITodoItemService _todoItemService;

        public WeatherToDoCombinedController(ITodoItemService todoItemService)
        {
            _todoItemService = todoItemService;
        }


        [HttpGet]
        public async Task<ActionResult<IEnumerable<object>>> GetCombinedTodoListWeather()
        {
            try
            {
                var combinedList = await _todoItemService.GetCombinedTodoListWeatherAsync();
                return Ok(combinedList);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(WeatherToDoCombinedController)} - {nameof(GetCombinedTodoListWeather)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}

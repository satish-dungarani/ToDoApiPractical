using Microsoft.AspNetCore.Mvc;
using ToDoApiPractical.Helpers;
using ToDoApiPractical.Models;
using ToDoApiPractical.Services;

namespace ToDoApiPractical.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CategoryController : ControllerBase
    {
        private readonly ICategoryService _categoryService;

        public CategoryController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
        }

        [HttpPost("Save")]
        public async Task<ActionResult> SaveCategory(CategoryModel model)
        {
            try
            {
                var savedCategory = await _categoryService.SaveAsync(model);
                return Ok(savedCategory);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(CategoryController)} - {nameof(SaveCategory)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<CategoryModel>>> GetAllCategories()
        {
            try
            {
                var categories = await _categoryService.GetAllAsync();
                return Ok(categories);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(CategoryController)} - {nameof(GetAllCategories)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<CategoryModel>> GetCategoryById(int id)
        {
            try
            {
                var category = await _categoryService.GetByIdAsync(id);
                if (category == null)
                {
                    return NotFound($"Category with ID {id} not found.");
                }
                return Ok(category);
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(CategoryController)} - {nameof(GetCategoryById)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCategory(int id)
        {
            try
            {
                await _categoryService.DeleteAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                LogHelper.logger.Error($"{nameof(CategoryController)} - {nameof(DeleteCategory)} - ERROR - {ex}");
                return StatusCode(StatusCodes.Status500InternalServerError, "An error occurred while processing your request.");
            }
        }
    }
}
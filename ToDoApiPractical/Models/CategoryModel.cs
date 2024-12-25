
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using static ToDoApiPractical.Helpers.Helpers;

namespace ToDoApiPractical.Models
{
    public class CategoryModel
    {
        public int Id { get; set; }
        [Required]
        public string Title { get; set; }
        public int? ParentCategoryId { get; set; }

        public string? ParentCategory
        {
            get
            {
                if (ParentCategoryId.HasValue && Enum.IsDefined(typeof(ParentCategories), ParentCategoryId.Value))
                {
                    return Enum.GetName(typeof(ParentCategories), ParentCategoryId.Value);
                }
                return  ParentCategories.None.ToString();
            }
        }
    }
}

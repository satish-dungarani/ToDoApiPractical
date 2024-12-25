using static ToDoApiPractical.Helpers.Helpers;

namespace ToDoApiPractical.Domains
{
    public class Category
    {
        public int Id { get; set; }
        public required string Title { get; set; }
        public int? ParentCategoryId { get; set; }
    }
}

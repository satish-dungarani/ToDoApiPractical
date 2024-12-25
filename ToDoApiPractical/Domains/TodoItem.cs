using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations.Schema;

namespace ToDoApiPractical.Domains
{
    public class TodoItem
    {
        public int Id { get; set; }
        public required string ToDo { get; set; }
        public bool Completed { get; set; }
        public int UserId { get; set; }
        public int Priority { get; set; } = 3;
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public DateTime? DueDate { get; set; }
        public int? CategoryId { get; set; }
        public Category? Category { get; set; }

    }
}

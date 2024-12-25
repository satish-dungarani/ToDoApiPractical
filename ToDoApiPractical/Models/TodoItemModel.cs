

using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace ToDoApiPractical.Models
{
    public class TodoItemModel
    {
        public int Id { get; set; }
        [Required]
        public string ToDo { get; set; }
        public bool Completed { get; set; }
        public int? UserId { get; set; }
        [Range(1, 5, ErrorMessage = "Priority must be between 1 (top priority) and 5 (low priority).")]
        public int Priority { get; set; } = 3;
        //public string? Location { get; set; }
        public double? Lat { get; set; }
        public double? Lon { get; set; }
        public DateTime? DueDate { get; set; }
        public int? CategoryId { get; set; }
        public string? Category { get; set; }
        //public string? CurrentTemperature { get; set; }
        //public string? CurrentCondition { get; set; }
    }
}

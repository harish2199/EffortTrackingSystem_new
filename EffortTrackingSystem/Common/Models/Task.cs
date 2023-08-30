using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Task
    {
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Task Name is required.")]
        [StringLength(50, ErrorMessage = "Task Name cannot exceed 50 characters.")]
        public string TaskName { get; set; }
    }
}

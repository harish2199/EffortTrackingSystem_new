using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Project
    {
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Project Name is required.")]
        [StringLength(100, ErrorMessage = "Project Name cannot exceed 100 characters.")]
        public string ProjectName { get; set; }
    }
}

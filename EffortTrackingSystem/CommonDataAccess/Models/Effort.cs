using System;
using System.ComponentModel.DataAnnotations;

namespace CommonDataAccess.Models
{
    public class Effort
    {
        public int EffortId { get; set; }

        [Required(ErrorMessage = "Assign Task ID is required.")]
        public int AssignTaskId { get; set; }

        [Required(ErrorMessage = "Shift ID is required.")]
        public int ShiftId { get; set; }

        [Required(ErrorMessage = "Hours Worked is required.")]
        [Range(1, int.MaxValue, ErrorMessage = "Hours Worked must be greater than 0.")]
        public int HoursWorked { get; set; }

        public DateTime SubmittedDate { get; set; }

        public string Status { get; set; }

        public AssignTask AssignTask { get; set; }
        public Shift Shift { get; set; }
    }
}

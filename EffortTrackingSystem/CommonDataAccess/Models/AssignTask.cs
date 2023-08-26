using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace CommonDataAccess.Models
{
    public class AssignTask
    {
        public int AssignTaskId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Project ID is required.")]
        public int ProjectId { get; set; }

        [Required(ErrorMessage = "Task ID is required.")]
        public int TaskId { get; set; }

        [Required(ErrorMessage = "Shift ID is required.")]
        public int ShiftId { get; set; }

        [Required(ErrorMessage = "Start Date is required.")]
        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; }

        [Required(ErrorMessage = "End Date is required.")]
        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; }

        public string Status { get; set; } = "Pending";

        public User User { get; set; }
        public Project Project { get; set; }
        public Task Task { get; set; }
        public Shift Shift { get; set; }
        public ICollection<Effort> Efforts { get; set; }
    }
}

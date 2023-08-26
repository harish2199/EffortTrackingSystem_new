using System;
using System.ComponentModel.DataAnnotations;

namespace CommonDataAccess.Models
{
    public class ShiftChange
    {
        public int ShiftChangeId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Assigned Shift ID is required.")]
        public int AssignedShiftId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "New Shift ID is required.")]
        public int NewShiftId { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }

        public string Status { get; set; } = "Pending";

        public User User { get; set; }
        public Shift AssignedShift { get; set; }
        public Shift NewShift { get; set; }
    }
}

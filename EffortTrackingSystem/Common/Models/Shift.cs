using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Shift
    {
        public int ShiftId { get; set; }

        [Required(ErrorMessage = "Shift Name is required.")]
        [StringLength(50, ErrorMessage = "Shift Name cannot exceed 50 characters.")]
        public string ShiftName { get; set; }

        [Required(ErrorMessage = "Start Time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan StartTime { get; set; }

        [Required(ErrorMessage = "End Time is required.")]
        [DataType(DataType.Time)]
        public TimeSpan EndTime { get; set; }
    }
}

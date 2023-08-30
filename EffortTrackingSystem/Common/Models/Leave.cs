using System;
using System.ComponentModel.DataAnnotations;

namespace Common.Models
{
    public class Leave
    {
        public int LeaveId { get; set; }

        [Required(ErrorMessage = "User ID is required.")]
        public int UserId { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; }

        [Required(ErrorMessage = "Reason is required.")]
        [StringLength(500, ErrorMessage = "Reason cannot exceed 500 characters.")]
        public string Reason { get; set; }

        public string Status { get; set; } = "Pending";

        public User User { get; set; }
    }
}

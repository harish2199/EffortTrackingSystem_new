using System;
using System.ComponentModel.DataAnnotations;

namespace CommonDataAccess.Models
{
    public class Admin
    {
        public int AdminId { get; set; }

        [Required(ErrorMessage = "Admin Name is required.")]
        [StringLength(50, ErrorMessage = "Admin Name cannot exceed 50 characters.")]
        public string AdminName { get; set; }

        [Required(ErrorMessage = "Admin Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "Admin Email cannot exceed 100 characters.")]
        public string AdminEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string HashedPassword { get; set; }

        public string Role { get; set; } = "Admin";
    }
}

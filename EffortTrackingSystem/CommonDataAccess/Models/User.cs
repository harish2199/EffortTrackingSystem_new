using System;
using System.ComponentModel.DataAnnotations;

namespace CommonDataAccess.Models
{
    public class User
    {
        public int UserId { get; set; }

        [Required(ErrorMessage = "User Name is required.")]
        [StringLength(50, ErrorMessage = "User Name cannot exceed 50 characters.")]
        public string UserName { get; set; }

        [StringLength(50, ErrorMessage = "Designation cannot exceed 50 characters.")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "User Email is required.")]
        [EmailAddress(ErrorMessage = "Invalid email address.")]
        [StringLength(100, ErrorMessage = "User Email cannot exceed 100 characters.")]
        public string UserEmail { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters.")]
        public string HashedPassword { get; set; }

        public string Role { get; set; } = "User";
    }
}


using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace smart_corporate_cafeteria_management.ViewModels
{
    public class StaffAccountViewModel
    {
        [Required(ErrorMessage = "Please select a staff member.")]
        public int StaffID { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Enter a valid email address.")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm the password.")]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "Password and confirmation password do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<SelectListItem> AvailableStaff { get; set; }
            = new List<SelectListItem>();
    }
}

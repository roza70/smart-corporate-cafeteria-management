
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace smart_corporate_cafeteria_management.ViewModels
{
    public class EmployeeAccountViewModel
    {
        [Required(ErrorMessage = "Please select an employee.")]
        public int EmployeeId { get; set; }

        [Required(ErrorMessage = "Password is required.")]
        [StringLength(100, MinimumLength = 8,
            ErrorMessage = "Password must be at least 8 characters.")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please confirm the password.")]
        [DataType(DataType.Password)]
        [Compare("Password",
            ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; } = string.Empty;

        public List<SelectListItem> AvailableEmployees { get; set; }
            = new List<SelectListItem>();
    }
}


using System.ComponentModel.DataAnnotations;

namespace smart_corporate_cafeteria_management.ViewModels
{
    public class CafeteriaStaffViewModel
    {
        [Required(ErrorMessage = "Staff name is required.")]
        [StringLength(100)]
        public string StaffName { get; set; } = string.Empty;

        [Required(ErrorMessage = "NID number is required.")]
        [StringLength(30)]
        public string NIDNumber { get; set; } = string.Empty;

        [Required(ErrorMessage = "Please select gender.")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "Date of birth is required.")]
        [DataType(DataType.Date)]
        public DateTime DateOfBirth { get; set; }

        [Phone(ErrorMessage = "Enter a valid phone number.")]
        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        [Required(ErrorMessage = "Joining date is required.")]
        [DataType(DataType.Date)]
        public DateTime JoiningDate { get; set; }

        [Range(0, 10000000, ErrorMessage = "Enter a valid salary.")]
        public decimal? Salary { get; set; }

        public bool Status { get; set; } = true;

        [Required(ErrorMessage = "Please select a staff role.")]
        public string StaffRole { get; set; } = string.Empty;
    }
}

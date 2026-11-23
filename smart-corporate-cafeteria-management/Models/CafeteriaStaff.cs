
namespace smart_corporate_cafeteria_management.Models
{
    public class CafeteriaStaff
    {
        public int StaffID { get; set; }

        public string StaffName { get; set; } = string.Empty;

        public string NIDNumber { get; set; } = string.Empty;

        public string Gender { get; set; } = string.Empty;

        public DateTime DateOfBirth { get; set; }

        public string? PhoneNumber { get; set; }

        public string? Address { get; set; }

        public DateTime JoiningDate { get; set; }

        public decimal? Salary { get; set; }

        public bool Status { get; set; } = true;

        // One staff member can have multiple role assignments
        public ICollection<StaffRoleAssignment> StaffRoleAssignments
        {
            get;
            set;
        } = new List<StaffRoleAssignment>();
    }
}


using Microsoft.AspNetCore.Identity;

namespace smart_corporate_cafeteria_management.Models
{
    public class ApplicationUser : IdentityUser
    {
        // Link to Employee record (for Employee login)
        public int? EmployeeId { get; set; }

        // Link to CafeteriaStaff record (for Admin,
        // Kitchen Staff and Procurement Staff)
        public int? StaffID { get; set; }
    }
}

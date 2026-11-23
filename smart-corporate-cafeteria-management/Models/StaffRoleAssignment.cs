
namespace smart_corporate_cafeteria_management.Models
{
    public class StaffRoleAssignment
    {
        public int AssignmentID { get; set; }

        public int StaffID { get; set; }
        public CafeteriaStaff? CafeteriaStaff { get; set; }

        public int RoleID { get; set; }
        public StaffRole? StaffRole { get; set; }
    }
}

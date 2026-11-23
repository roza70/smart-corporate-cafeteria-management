
namespace smart_corporate_cafeteria_management.Models
{
    public class StaffRole
    {
        public int RoleID { get; set; }

        public string RoleName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<StaffRoleAssignment> StaffRoleAssignments
        { get; set; } = new List<StaffRoleAssignment>();
    }
}

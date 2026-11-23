
namespace smart_corporate_cafeteria_management.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string EmployeeName { get; set; } = string.Empty;

        public string? Email { get; set; }

        public string? Phone { get; set; }

        public int DepartmentId { get; set; }

        public Department? Department { get; set; }
    }
}

namespace smart_corporate_cafeteria_management.Models
{
    public class Employee
    {
        public int EmployeeId { get; set; }

        public string Name { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Phone { get; set; } = string.Empty;

        public int DepartmentId { get; set; }

        public Department? Department { get; set; }
    }
}
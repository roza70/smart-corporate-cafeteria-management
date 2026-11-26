
namespace smart_corporate_cafeteria_management.Models
{
    public class MealToken
    {
        public int TokenID { get; set; }

        public int EmployeeID { get; set; }

        public Employee? Employee { get; set; }

        public string TokenNumber { get; set; } = string.Empty;

        public DateTime TokenDate { get; set; }

        public string Status { get; set; } = "Active";

        public DateTime GeneratedAt { get; set; }

        public DateTime? UsedAt { get; set; }
    }
}

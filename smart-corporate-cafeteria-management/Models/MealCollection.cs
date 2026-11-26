
namespace smart_corporate_cafeteria_management.Models
{
    public class MealCollection
    {
        public int CollectionID { get; set; }

        public int EmployeeID { get; set; }

        public Employee? Employee { get; set; }

        public DateTime CollectionDate { get; set; }

        public TimeSpan CollectionTime { get; set; }

        public string Status { get; set; } = "Approved";
    }
}

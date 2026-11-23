
namespace smart_corporate_cafeteria_management.Models
{
    public class DailyMenu
    {
        public int MenuID { get; set; }

        public DateTime MenuDate { get; set; }

        public string? Description { get; set; }

        public ICollection<DailyMenuItem> DailyMenuItems { get; set; }
            = new List<DailyMenuItem>();
    }
}

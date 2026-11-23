
namespace smart_corporate_cafeteria_management.Models
{
    public class DailyMenuItem
    {
        public int MenuItemID { get; set; }

        public int MenuID { get; set; }

        public DailyMenu? DailyMenu { get; set; }

        public int FoodItemID { get; set; }

        public FoodItem? FoodItem { get; set; }
    }
}

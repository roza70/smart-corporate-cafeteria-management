
namespace smart_corporate_cafeteria_management.Models
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }

        public string FoodName { get; set; } = string.Empty;

        public int CategoryID { get; set; }

        public string? Description { get; set; }

        // Relationship with FoodCategory
        public FoodCategory? FoodCategory { get; set; }
    }
}

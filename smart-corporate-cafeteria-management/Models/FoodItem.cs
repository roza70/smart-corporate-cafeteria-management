namespace smart_corporate_cafeteria_management.Models
{
    public class FoodItem
    {
        public int FoodItemId { get; set; }

        public string FoodName { get; set; } = string.Empty;

        public string Category { get; set; } = string.Empty;

        public string Unit { get; set; } = string.Empty;

        public decimal UnitPrice { get; set; }

        public bool IsAvailable { get; set; } = true;
    }
}
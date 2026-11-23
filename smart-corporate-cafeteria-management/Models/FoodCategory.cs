namespace smart_corporate_cafeteria_management.Models
{
    public class FoodCategory
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public string? Description { get; set; }

        public ICollection<FoodItem> FoodItems { get; set; }
            = new List<FoodItem>();
    }
}
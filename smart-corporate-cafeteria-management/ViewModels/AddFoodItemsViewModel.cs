
using smart_corporate_cafeteria_management.Models;

namespace smart_corporate_cafeteria_management.ViewModels
{
    public class AddFoodItemsViewModel
    {
        public int MenuID { get; set; }

        public DateTime MenuDate { get; set; }

        public List<FoodCategorySelectionViewModel> CategorySelections
        { get; set; } = new List<FoodCategorySelectionViewModel>();
    }

    public class FoodCategorySelectionViewModel
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; } = string.Empty;

        public int? SelectedFoodItemId { get; set; }

        public List<FoodItem> AvailableFoodItems { get; set; }
            = new List<FoodItem>();
    }
}

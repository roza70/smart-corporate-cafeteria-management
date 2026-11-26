
using Microsoft.AspNetCore.Mvc.Rendering;

namespace smart_corporate_cafeteria_management.ViewModels
{
    public class AddFoodItemsViewModel
    {
        public int MenuID { get; set; }

        public DateTime MenuDate { get; set; }

        public List<int> SelectedFoodItemIds { get; set; }
            = new List<int>();

        public List<SelectListItem> AvailableFoodItems { get; set; }
            = new List<SelectListItem>();
    }
}

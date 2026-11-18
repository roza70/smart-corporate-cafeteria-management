using Microsoft.AspNetCore.Mvc;
using smart_corporate_cafeteria_management.Services;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class MealCalculationTestController : Controller
    {
        public IActionResult Index()
        {
            var service = new MealCalculationService();

            decimal rice = service.CalculateIngredient(150, 0.15m);
            decimal chicken = service.CalculateIngredient(150, 0.10m);

            string output = $@"
                Meal Ingredient Calculation

                Total Employees: 150

                Required Rice: {rice} kg
                Required Chicken: {chicken} kg
            ";

            return Content(output);
        }
    }
}
using Microsoft.AspNetCore.Mvc;
using smart_corporate_cafeteria_management.Services;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class MealEligibilityTestController : Controller
    {
        public IActionResult Index()
        {
            var service = new MealEligibilityService();

            bool result1 = service.CanCollectMeal(true, false);
            bool result2 = service.CanCollectMeal(true, true);
            bool result3 = service.CanCollectMeal(false, false);
            bool result4 = service.CanCollectMeal(false, true);

            string output = $@"
                Meal Eligibility Test Results

                Test 1: Active, Not Collected = {result1}
                Test 2: Active, Already Collected = {result2}
                Test 3: Inactive, Not Collected = {result3}
                Test 4: Inactive, Already Collected = {result4}
            ";

            return Content(output);
        }
    }
}

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Kitchen Staff")]
    public class KitchenStaffController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public KitchenStaffController(CafeteriaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // Get today's date
            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Today's Menu with Food Items
            var todayMenu = await _context.DailyMenus
                .AsNoTracking()
                .Include(m => m.DailyMenuItems)
                    .ThenInclude(mi => mi.FoodItem)
                .FirstOrDefaultAsync(m => m.MenuDate == today);

            // Today's Menu Food Names
            var todayMenuItems = todayMenu?.DailyMenuItems
                .Where(mi => mi.FoodItem != null)
                .Select(mi => mi.FoodItem!.FoodName)
                .ToList() ?? new List<string>();

            // Total Recipes
            var totalRecipes = await _context.Recipes
                .CountAsync();

            // Actual Meal Collections Today
            var todayCollections = await _context.MealCollections
                .CountAsync(mc =>
                    mc.CollectionDate >= today &&
                    mc.CollectionDate < tomorrow);

            // Send Data to Dashboard View
            ViewBag.TodayMenu = todayMenu;
            ViewBag.TodayMenuItems = todayMenuItems;
            ViewBag.TodayMenuItemsCount = todayMenuItems.Count;
            ViewBag.TotalRecipes = totalRecipes;
            ViewBag.TodayCollections = todayCollections;

            ViewBag.MenuAvailable = todayMenu != null;

            return View();
        }
    }
}

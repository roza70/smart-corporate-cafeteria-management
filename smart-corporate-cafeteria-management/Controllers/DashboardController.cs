
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class DashboardController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public DashboardController(CafeteriaDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            ViewBag.TotalEmployees =
                await _context.Employees.CountAsync();

            ViewBag.Departments =
                await _context.Departments.CountAsync();

            ViewBag.DailyMenus =
                await _context.DailyMenus.CountAsync();

            // Meal Collection module is not implemented yet.
            ViewBag.MealCollections = 0;

            return View();
        }
    }
}

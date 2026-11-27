
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Employee")]
    public class EmployeeDashboardController : Controller
    {
        private readonly CafeteriaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeDashboardController(
            CafeteriaDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Employee Dashboard
        public async Task<IActionResult> Index()
        {
            // Get currently logged-in employee account
            var user = await _userManager.GetUserAsync(User);

            if (user == null || user.EmployeeId == null)
            {
                return Forbid();
            }

            var employeeId = user.EmployeeId.Value;

            var today = DateTime.Today;
            var tomorrow = today.AddDays(1);

            // Employee Information
            var employee = await _context.Employees
                .AsNoTracking()
                .Include(e => e.Department)
                .FirstOrDefaultAsync(e => e.EmployeeId == employeeId);

            if (employee == null)
            {
                return Forbid();
            }

            // Today's Published Menu Only
            var todayMenu = await _context.DailyMenus
                .AsNoTracking()
                .Include(m => m.DailyMenuItems)
                    .ThenInclude(mi => mi.FoodItem)
                        .ThenInclude(f => f!.FoodCategory)
                .Where(m =>
                    m.IsPublished &&
                    m.MenuDate >= today &&
                    m.MenuDate < tomorrow)
                .FirstOrDefaultAsync();

            // Today's Published Food Items with Category Names
            var todayMenuItems = todayMenu?.DailyMenuItems
                .Where(mi =>
                    mi.FoodItem != null &&
                    mi.FoodItem.IsActive)
                .Select(mi => new
                {
                    CategoryName = mi.FoodItem!.FoodCategory != null
                        ? mi.FoodItem.FoodCategory.CategoryName
                        : "Uncategorized",

                    FoodName = mi.FoodItem!.FoodName
                })
                .ToList();

            // This Employee's Meal Collections Today
            var todayCollections = await _context.MealCollections
                .AsNoTracking()
                .CountAsync(mc =>
                    mc.EmployeeID == employeeId &&
                    mc.CollectionDate >= today &&
                    mc.CollectionDate < tomorrow);

            // This Employee's Meal Collection History
            var collectionHistory = await _context.MealCollections
                .AsNoTracking()
                .Where(mc => mc.EmployeeID == employeeId)
                .OrderByDescending(mc => mc.CollectionDate)
                .ThenByDescending(mc => mc.CollectionTime)
                .Take(10)
                .ToListAsync();

            // Send Information to Employee Dashboard View
            ViewBag.Employee = employee;
            ViewBag.TodayMenuItems = todayMenuItems;
            ViewBag.TodayCollections = todayCollections;
            ViewBag.CollectionHistory = collectionHistory;

            // Only Published Menu is considered available
            ViewBag.MenuAvailable = todayMenu != null;

            return View();
        }

        // Employee Daily Food Menu
        // Shows today's and previous Published Menus
        public async Task<IActionResult> DailyFoodMenu()
        {
            var today = DateTime.Today;

            var publishedMenus = await _context.DailyMenus
                .AsNoTracking()
                .Include(m => m.DailyMenuItems)
                    .ThenInclude(mi => mi.FoodItem)
                        .ThenInclude(f => f!.FoodCategory)
                .Where(m =>
                    m.IsPublished &&
                    m.MenuDate < today.AddDays(1))
                .OrderByDescending(m => m.MenuDate)
                .ToListAsync();

            ViewBag.PublishedMenus = publishedMenus;

            return View();
        }
    }
}

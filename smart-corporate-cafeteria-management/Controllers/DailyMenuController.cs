
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.Rendering;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;
using smart_corporate_cafeteria_management.ViewModels;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class DailyMenuController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public DailyMenuController(CafeteriaDbContext context)
        {
            _context = context;
        }

        // Display all daily menus
        public async Task<IActionResult> Index()
        {
            var menus = await _context.DailyMenus.ToListAsync();

            return View(menus);
        }

        // Display Create Menu form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Save new daily menu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailyMenu menu)
        {
            if (ModelState.IsValid)
            {
                _context.DailyMenus.Add(menu);

                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(menu);
        }

        // Display Add Food Items form
        [HttpGet]
        public async Task<IActionResult> AddFoodItems(int id)
        {
            var menu = await _context.DailyMenus
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            var viewModel = new AddFoodItemsViewModel
            {
                MenuID = menu.MenuID,
                MenuDate = menu.MenuDate,

                AvailableFoodItems = await _context.FoodItems
                    .Where(f => f.IsActive)
                    .OrderBy(f => f.FoodName)
                    .Select(f => new SelectListItem
                    {
                        Value = f.FoodItemId.ToString(),
                        Text = f.FoodName
                    })
                    .ToListAsync()
            };

            return View(viewModel);
        }

        // Save selected food items
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddFoodItems(
            AddFoodItemsViewModel viewModel)
        {
            var menu = await _context.DailyMenus
                .FirstOrDefaultAsync(m => m.MenuID == viewModel.MenuID);

            if (menu == null)
            {
                return NotFound();
            }

            var selectedIds = viewModel.SelectedFoodItemIds
                .Distinct()
                .ToList();

            if (!selectedIds.Any())
            {
                return RedirectToAction(
                    nameof(AddFoodItems),
                    new { id = viewModel.MenuID }
                );
            }

            var validFoodItemIds = await _context.FoodItems
                .Where(f => selectedIds.Contains(f.FoodItemId) && f.IsActive)
                .Select(f => f.FoodItemId)
                .ToListAsync();

            if (validFoodItemIds.Count != selectedIds.Count)
            {
                return BadRequest(
                    "One or more selected food items are invalid."
                );
            }

            var existingFoodItemIds = await _context.DailyMenuItems
                .Where(d => d.MenuID == viewModel.MenuID)
                .Select(d => d.FoodItemID)
                .ToListAsync();

            var newFoodItemIds = validFoodItemIds
                .Except(existingFoodItemIds)
                .ToList();

            foreach (var foodItemId in newFoodItemIds)
            {
                var menuItem = new DailyMenuItem
                {
                    MenuID = viewModel.MenuID,
                    FoodItemID = foodItemId
                };

                _context.DailyMenuItems.Add(menuItem);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Display daily menu details with food items
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var menu = await _context.DailyMenus
                .Include(m => m.DailyMenuItems)
                    .ThenInclude(mi => mi.FoodItem)
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // Display Edit Menu form
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var menu = await _context.DailyMenus
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // Update Daily Menu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DailyMenu menu)
        {
            if (id != menu.MenuID)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(menu);
            }

            var existingMenu = await _context.DailyMenus
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (existingMenu == null)
            {
                return NotFound();
            }

            existingMenu.MenuDate = menu.MenuDate;
            existingMenu.Description = menu.Description;

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        // Display Delete Confirmation Page
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var menu = await _context.DailyMenus
                .Include(m => m.DailyMenuItems)
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            return View(menu);
        }

        // Delete Daily Menu
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var menu = await _context.DailyMenus
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            // Find related menu items
            var menuItems = await _context.DailyMenuItems
                .Where(mi => mi.MenuID == id)
                .ToListAsync();

            // Delete related menu items first
            _context.DailyMenuItems.RemoveRange(menuItems);

            // Delete the daily menu
            _context.DailyMenus.Remove(menu);

            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}

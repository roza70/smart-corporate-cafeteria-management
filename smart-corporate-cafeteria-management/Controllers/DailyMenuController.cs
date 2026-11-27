
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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
            var menus = await _context.DailyMenus
                .OrderByDescending(m => m.MenuDate)
                .ToListAsync();

            return View(menus);
        }

        // Display Create Menu form
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        // Save new daily menu with duplicate date validation
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DailyMenu menu)
        {
            var selectedDate = menu.MenuDate.Date;

            bool menuExists = await _context.DailyMenus
                .AnyAsync(m => m.MenuDate >= selectedDate &&
                               m.MenuDate < selectedDate.AddDays(1));

            if (menuExists)
            {
                ModelState.AddModelError(
                    "MenuDate",
                    "A menu already exists for this date. Only one daily menu is allowed per day."
                );

                return View(menu);
            }

            if (!ModelState.IsValid)
            {
                return View(menu);
            }

            menu.MenuDate = selectedDate;
            menu.IsPublished = false;

            _context.DailyMenus.Add(menu);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Daily menu created successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Publish Daily Menu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Publish(int id)
        {
            var menu = await _context.DailyMenus
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            var hasFoodItems = await _context.DailyMenuItems
                .AnyAsync(mi => mi.MenuID == id);

            if (!hasFoodItems)
            {
                TempData["ErrorMessage"] =
                    "Cannot publish an empty menu. Please add food items first.";

                return RedirectToAction(nameof(Index));
            }

            menu.IsPublished = true;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Daily menu published successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Unpublish Daily Menu
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Unpublish(int id)
        {
            var menu = await _context.DailyMenus
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            menu.IsPublished = false;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Daily menu unpublished successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Display Menu History
        [HttpGet]
        public async Task<IActionResult> History(DateTime? date)
        {
            var today = DateTime.Today;

            var query = _context.DailyMenus
                .AsNoTracking()
                .Include(m => m.DailyMenuItems)
                    .ThenInclude(mi => mi.FoodItem)
                .Where(m => m.MenuDate < today);

            if (date.HasValue)
            {
                var selectedDate = date.Value.Date;

                query = query.Where(m =>
                    m.MenuDate >= selectedDate &&
                    m.MenuDate < selectedDate.AddDays(1));
            }

            var menuHistory = await query
                .OrderByDescending(m => m.MenuDate)
                .ToListAsync();

            ViewBag.SelectedDate = date?.ToString("yyyy-MM-dd");

            return View(menuHistory);
        }

        // Display Category-wise Add Food Items form
        [HttpGet]
        public async Task<IActionResult> AddFoodItems(int id)
        {
            var menu = await _context.DailyMenus
                .Include(m => m.DailyMenuItems)
                .FirstOrDefaultAsync(m => m.MenuID == id);

            if (menu == null)
            {
                return NotFound();
            }

            var existingSelections = menu.DailyMenuItems
                .ToDictionary(
                    item => item.FoodItemID,
                    item => item.FoodItemID
                );

            var categories = await _context.FoodCategories
                .Include(c => c.FoodItems)
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            var viewModel = new AddFoodItemsViewModel
            {
                MenuID = menu.MenuID,
                MenuDate = menu.MenuDate,

                CategorySelections = categories.Select(category =>
                {
                    var selectedFoodItemId = category.FoodItems
                        .Where(food => existingSelections.ContainsKey(food.FoodItemId))
                        .Select(food => (int?)food.FoodItemId)
                        .FirstOrDefault();

                    return new FoodCategorySelectionViewModel
                    {
                        CategoryID = category.CategoryID,
                        CategoryName = category.CategoryName,
                        SelectedFoodItemId = selectedFoodItemId,

                        AvailableFoodItems = category.FoodItems
                            .Where(food => food.IsActive)
                            .OrderBy(food => food.FoodName)
                            .ToList()
                    };
                }).ToList()
            };

            return View(viewModel);
        }

        // Save one selected food item from each category
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

            // Load actual categories and active food items from database
            var categories = await _context.FoodCategories
                .Include(c => c.FoodItems)
                .ToListAsync();

            var categoryIds = categories
                .Select(c => c.CategoryID)
                .ToHashSet();

            // Prevent invalid or altered category submissions
            if (viewModel.CategorySelections == null ||
                viewModel.CategorySelections.Count != categories.Count ||
                viewModel.CategorySelections.Any(
                    s => !categoryIds.Contains(s.CategoryID)) ||
                viewModel.CategorySelections
                    .Select(s => s.CategoryID)
                    .Distinct()
                    .Count() != categories.Count)
            {
                return BadRequest(
                    "Invalid food category selection."
                );
            }

            var selectedFoodItemIds = new List<int>();

            foreach (var category in categories)
            {
                var selection = viewModel.CategorySelections
                    .First(s => s.CategoryID == category.CategoryID);

                if (!selection.SelectedFoodItemId.HasValue)
                {
                    ModelState.AddModelError(
                        $"CategorySelections[{categories.IndexOf(category)}].SelectedFoodItemId",
                        $"Please select one food item for {category.CategoryName}."
                    );

                    continue;
                }

                int selectedId = selection.SelectedFoodItemId.Value;

                bool validSelection = category.FoodItems.Any(
                    food => food.FoodItemId == selectedId &&
                            food.IsActive
                );

                if (!validSelection)
                {
                    ModelState.AddModelError(
                        $"CategorySelections[{categories.IndexOf(category)}].SelectedFoodItemId",
                        $"Invalid food item selected for {category.CategoryName}."
                    );

                    continue;
                }

                selectedFoodItemIds.Add(selectedId);
            }

            if (!ModelState.IsValid ||
                selectedFoodItemIds.Count != categories.Count)
            {
                // Rebuild the ViewModel so validation errors can be displayed
                viewModel.CategorySelections = categories
                    .OrderBy(c => c.CategoryName)
                    .Select(category =>
                    {
                        var postedSelection = viewModel.CategorySelections
                            .FirstOrDefault(s => s.CategoryID == category.CategoryID);

                        return new FoodCategorySelectionViewModel
                        {
                            CategoryID = category.CategoryID,
                            CategoryName = category.CategoryName,
                            SelectedFoodItemId = postedSelection?.SelectedFoodItemId,

                            AvailableFoodItems = category.FoodItems
                                .Where(f => f.IsActive)
                                .OrderBy(f => f.FoodName)
                                .ToList()
                        };
                    }).ToList();

                return View(viewModel);
            }

            // Replace existing menu items with the new selections
            var existingMenuItems = await _context.DailyMenuItems
                .Where(item => item.MenuID == viewModel.MenuID)
                .ToListAsync();

            _context.DailyMenuItems.RemoveRange(existingMenuItems);

            foreach (var foodItemId in selectedFoodItemIds)
            {
                _context.DailyMenuItems.Add(new DailyMenuItem
                {
                    MenuID = viewModel.MenuID,
                    FoodItemID = foodItemId
                });
            }

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Daily menu food items updated successfully.";

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

        // Update Daily Menu with duplicate date validation
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

            var selectedDate = menu.MenuDate.Date;

            bool duplicateDateExists = await _context.DailyMenus
                .AnyAsync(m =>
                    m.MenuID != id &&
                    m.MenuDate >= selectedDate &&
                    m.MenuDate < selectedDate.AddDays(1));

            if (duplicateDateExists)
            {
                ModelState.AddModelError(
                    "MenuDate",
                    "Another menu already exists for this date. Please select a different date."
                );

                return View(menu);
            }

            existingMenu.MenuDate = selectedDate;
            existingMenu.Description = menu.Description;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Daily menu updated successfully.";

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

            var menuItems = await _context.DailyMenuItems
                .Where(mi => mi.MenuID == id)
                .ToListAsync();

            _context.DailyMenuItems.RemoveRange(menuItems);
            _context.DailyMenus.Remove(menu);

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Daily menu deleted successfully.";

            return RedirectToAction(nameof(Index));
        }
    }
}

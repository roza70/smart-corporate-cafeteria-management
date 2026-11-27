
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Admin,Kitchen Staff")]
    public class FoodItemController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public FoodItemController(CafeteriaDbContext context)
        {
            _context = context;
        }

        // GET: FoodItem
        public async Task<IActionResult> Index(
            string? searchString,
            int? categoryId,
            string? status)
        {
            var foodItems = _context.FoodItems
                .Include(f => f.FoodCategory)
                .AsQueryable();

            // Search by Food Name OR Category Name
            if (!string.IsNullOrWhiteSpace(searchString))
            {
                foodItems = foodItems.Where(f =>
                    f.FoodName.Contains(searchString) ||
                    (f.FoodCategory != null &&
                     f.FoodCategory.CategoryName.Contains(searchString)));
            }

            // Filter by Category
            if (categoryId.HasValue)
            {
                foodItems = foodItems.Where(f =>
                    f.CategoryID == categoryId.Value);
            }

            // Filter by Status
            if (status == "Active")
            {
                foodItems = foodItems.Where(f => f.IsActive);
            }
            else if (status == "Inactive")
            {
                foodItems = foodItems.Where(f => !f.IsActive);
            }

            ViewBag.Categories = new SelectList(
                await _context.FoodCategories.ToListAsync(),
                "CategoryID",
                "CategoryName",
                categoryId);

            ViewBag.SearchString = searchString;
            ViewBag.SelectedStatus = status;

            var result = await foodItems
                .OrderBy(f =>
                    f.FoodCategory != null &&
                    f.FoodCategory.CategoryName == "Rice" ? 0 : 1)
                .ThenBy(f => f.FoodCategory!.CategoryName)
                .ThenBy(f => f.FoodName)
                .ToListAsync();

            return View(result);
        }

        // GET: FoodItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
                return NotFound();

            var foodItem = await _context.FoodItems
                .Include(f => f.FoodCategory)
                .FirstOrDefaultAsync(f => f.FoodItemId == id);

            if (foodItem == null)
                return NotFound();

            return View(foodItem);
        }

        // GET: FoodItem/Create
        // Supports category selection and return URL
        public async Task<IActionResult> Create(
            int? categoryId,
            string? returnTo)
        {
            await LoadCategoriesAsync(categoryId);

            ViewBag.ReturnTo = returnTo;

            return View();
        }

        // POST: FoodItem/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("FoodName,CategoryID,Description,IsActive")] FoodItem foodItem,
            string? returnTo)
        {
            // Validate Category
            if (!await _context.FoodCategories
                .AnyAsync(c => c.CategoryID == foodItem.CategoryID))
            {
                ModelState.AddModelError(
                    "CategoryID",
                    "Please select a valid category.");
            }

            // Prevent duplicate food item in the same category
            if (!string.IsNullOrWhiteSpace(foodItem.FoodName) &&
                await _context.FoodItems.AnyAsync(f =>
                    f.FoodName == foodItem.FoodName &&
                    f.CategoryID == foodItem.CategoryID))
            {
                ModelState.AddModelError(
                    "FoodName",
                    "This food item already exists in this category.");
            }

            if (ModelState.IsValid)
            {
                _context.FoodItems.Add(foodItem);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "New food item added successfully!";

                // Return to Daily Menu page
                if (!string.IsNullOrWhiteSpace(returnTo) &&
                    Url.IsLocalUrl(returnTo))
                {
                    return Redirect(returnTo);
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadCategoriesAsync(foodItem.CategoryID);

            ViewBag.ReturnTo = returnTo;

            return View(foodItem);
        }

        // GET: FoodItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
                return NotFound();

            var foodItem = await _context.FoodItems.FindAsync(id);

            if (foodItem == null)
                return NotFound();

            await LoadCategoriesAsync(foodItem.CategoryID);

            return View(foodItem);
        }

        // POST: FoodItem/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("FoodItemId,FoodName,CategoryID,Description,IsActive")] FoodItem foodItem)
        {
            if (id != foodItem.FoodItemId)
                return NotFound();

            if (!await _context.FoodCategories
                .AnyAsync(c => c.CategoryID == foodItem.CategoryID))
            {
                ModelState.AddModelError(
                    "CategoryID",
                    "Please select a valid category.");
            }

            // Prevent duplicate food item in the same category
            if (!string.IsNullOrWhiteSpace(foodItem.FoodName) &&
                await _context.FoodItems.AnyAsync(f =>
                    f.FoodName == foodItem.FoodName &&
                    f.CategoryID == foodItem.CategoryID &&
                    f.FoodItemId != id))
            {
                ModelState.AddModelError(
                    "FoodName",
                    "This food item already exists in this category.");
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.FoodItems.Update(foodItem);

                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] =
                        "Food item updated successfully!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.FoodItems
                        .AnyAsync(f => f.FoodItemId == id))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            await LoadCategoriesAsync(foodItem.CategoryID);

            return View(foodItem);
        }

        // GET: FoodItem/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
                return NotFound();

            var foodItem = await _context.FoodItems
                .Include(f => f.FoodCategory)
                .FirstOrDefaultAsync(f => f.FoodItemId == id);

            if (foodItem == null)
                return NotFound();

            return View(foodItem);
        }

        // POST: FoodItem/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);

            if (foodItem != null)
            {
                bool isUsedInMenu = await _context.DailyMenuItems
                    .AnyAsync(m => m.FoodItemID == id);

                if (isUsedInMenu)
                {
                    TempData["ErrorMessage"] =
                        "This food item is already used in a daily menu. Deactivate it instead.";

                    return RedirectToAction(nameof(Index));
                }

                _context.FoodItems.Remove(foodItem);

                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Food item deleted successfully!";
            }

            return RedirectToAction(nameof(Index));
        }

        // POST: FoodItem/ToggleStatus/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleStatus(int id)
        {
            var foodItem = await _context.FoodItems.FindAsync(id);

            if (foodItem == null)
                return NotFound();

            foodItem.IsActive = !foodItem.IsActive;

            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = foodItem.IsActive
                ? "Food item activated successfully!"
                : "Food item deactivated successfully!";

            return RedirectToAction(nameof(Index));
        }

        // Load Food Categories
        private async Task LoadCategoriesAsync(
            int? selectedCategory = null)
        {
            var categories = await _context.FoodCategories
                .OrderBy(c => c.CategoryName)
                .ToListAsync();

            ViewBag.CategoryID = new SelectList(
                categories,
                "CategoryID",
                "CategoryName",
                selectedCategory);
        }
    }
}

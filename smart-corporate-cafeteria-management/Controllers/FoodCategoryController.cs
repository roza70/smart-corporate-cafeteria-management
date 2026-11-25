
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class FoodCategoryController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public FoodCategoryController(CafeteriaDbContext context)
        {
            _context = context;
        }

        // 1. Display all food categories
        public async Task<IActionResult> Index()
        {
            var categories = await _context.FoodCategories
                .Include(c => c.FoodItems)
                .AsNoTracking()
                .ToListAsync();

            return View(categories);
        }

        // 2. Display category details
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.FoodCategories
                .Include(c => c.FoodItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // 3. Open create form
        public IActionResult Create()
        {
            return View();
        }

        // Save new category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            [Bind("CategoryName,Description")] FoodCategory category)
        {
            if (ModelState.IsValid)
            {
                _context.FoodCategories.Add(category);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Food category created successfully.";

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // 4. Open edit form
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.FoodCategories
                .FindAsync(id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // Save updated category
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(
            int id,
            [Bind("CategoryID,CategoryName,Description")]
            FoodCategory category)
        {
            if (id != category.CategoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.FoodCategories.Update(category);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] =
                        "Food category updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FoodCategoryExists(category.CategoryID))
                    {
                        return NotFound();
                    }

                    throw;
                }

                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }

        // 5. Display delete confirmation
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.FoodCategories
                .Include(c => c.FoodItems)
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // Delete category
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.FoodCategories
                .Include(c => c.FoodItems)
                .FirstOrDefaultAsync(c => c.CategoryID == id);

            if (category == null)
            {
                return NotFound();
            }

            // Prevent deletion if food items are assigned
            if (category.FoodItems.Any())
            {
                TempData["ErrorMessage"] =
                    "This category cannot be deleted because food items are assigned to it.";

                return RedirectToAction(nameof(Index));
            }

            _context.FoodCategories.Remove(category);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Food category deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // Check whether category exists
        private bool FoodCategoryExists(int id)
        {
            return _context.FoodCategories
                .Any(e => e.CategoryID == id);
        }
    }
}

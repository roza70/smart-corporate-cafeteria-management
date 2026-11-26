
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;
using System.Data;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class MealCollectionController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public MealCollectionController(CafeteriaDbContext context)
        {
            _context = context;
        }

        // =====================================
        // DISPLAY MEAL COLLECTION PAGE
        // =====================================

        public async Task<IActionResult> Index()
        {
            var today = DateTime.Today;

            var tokens = await _context.MealTokens
                .Include(t => t.Employee)
                .Where(t => t.TokenDate == today)
                .OrderBy(t => t.TokenNumber)
                .ToListAsync();

            ViewBag.Tokens = tokens;

            return View();
        }


        // =====================================
        // GENERATE DAILY UNIQUE TOKENS
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> GenerateDailyTokens()
        {
            var today = DateTime.Today;

            // Expire previous unused tokens
            var expiredTokens = await _context.MealTokens
                .Where(t => t.TokenDate < today && t.Status == "Active")
                .ToListAsync();

            foreach (var token in expiredTokens)
            {
                token.Status = "Expired";
            }

            // Get all employees
            var employees = await _context.Employees
                .OrderBy(e => e.EmployeeId)
                .ToListAsync();

            if (!employees.Any())
            {
                TempData["Message"] = "No employees found!";
                TempData["Status"] = "Rejected";

                return RedirectToAction("Index");
            }

            // Find employees who already have today's token
            var existingEmployeeIds = await _context.MealTokens
                .Where(t => t.TokenDate == today)
                .Select(t => t.EmployeeID)
                .ToListAsync();

            // Generate missing tokens
            var newTokens = new List<MealToken>();

            for (int i = 0; i < employees.Count; i++)
            {
                var employee = employees[i];

                if (existingEmployeeIds.Contains(employee.EmployeeId))
                {
                    continue;
                }

                string tokenNumber =
                    $"TK-{today:yyyyMMdd}-{i + 1:D3}";

                var token = new MealToken
                {
                    EmployeeID = employee.EmployeeId,
                    TokenNumber = tokenNumber,
                    TokenDate = today,
                    Status = "Active",
                    GeneratedAt = DateTime.Now
                };

                newTokens.Add(token);
            }

            if (!newTokens.Any())
            {
                TempData["Message"] =
                    "Today's tokens have already been generated!";

                TempData["Status"] = "Approved";

                return RedirectToAction("Index");
            }

            _context.MealTokens.AddRange(newTokens);

            try
            {
                await _context.SaveChangesAsync();

                TempData["Message"] =
                    $"{newTokens.Count} daily tokens generated successfully!";

                TempData["Status"] = "Approved";
            }
            catch (DbUpdateException)
            {
                TempData["Message"] =
                    "Token generation failed. Please check for duplicate tokens and try again.";

                TempData["Status"] = "Rejected";
            }

            return RedirectToAction("Index");
        }


        // =====================================
        // PROCESS LUNCH COLLECTION USING TOKEN
        // =====================================

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Collect(string tokenNumber)
        {
            if (string.IsNullOrWhiteSpace(tokenNumber))
            {
                TempData["Message"] = "Please enter a token number!";
                TempData["Status"] = "Rejected";

                return RedirectToAction("Index");
            }

            var today = DateTime.Today;

            // Use transaction to protect collection processing
            await using var transaction =
                await _context.Database.BeginTransactionAsync(
                    IsolationLevel.Serializable);

            // Find today's active token
            var token = await _context.MealTokens
                .FirstOrDefaultAsync(t =>
                    t.TokenNumber == tokenNumber.Trim() &&
                    t.TokenDate == today &&
                    t.Status == "Active");

            if (token == null)
            {
                await transaction.RollbackAsync();

                TempData["Message"] =
                    "Invalid, expired or already used token!";

                TempData["Status"] = "Rejected";

                return RedirectToAction("Index");
            }

            // Check whether employee already collected lunch
            var alreadyCollected = await _context.MealCollections
                .AnyAsync(m =>
                    m.EmployeeID == token.EmployeeID &&
                    m.CollectionDate == today);

            if (alreadyCollected)
            {
                await transaction.RollbackAsync();

                TempData["Message"] =
                    "This employee has already collected lunch today!";

                TempData["Status"] = "Rejected";

                return RedirectToAction("Index");
            }

            // Mark token as used
            token.Status = "Used";
            token.UsedAt = DateTime.Now;

            // Create collection record
            var collection = new MealCollection
            {
                EmployeeID = token.EmployeeID,
                CollectionDate = today,
                CollectionTime = DateTime.Now.TimeOfDay,
                Status = "Approved"
            };

            _context.MealCollections.Add(collection);

            await _context.SaveChangesAsync();

            await transaction.CommitAsync();

            TempData["Message"] =
                $"Lunch collection successful! Token: {token.TokenNumber}";

            TempData["Status"] = "Approved";

            return RedirectToAction("Index");
        }
    }
}


using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;
using smart_corporate_cafeteria_management.ViewModels;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeAccountController : Controller
    {
        private readonly CafeteriaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public EmployeeAccountController(
            CafeteriaDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: EmployeeAccount/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new EmployeeAccountViewModel();

            await LoadAvailableEmployees(model);

            return View(model);
        }

        // POST: EmployeeAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            EmployeeAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableEmployees(model);
                return View(model);
            }

            // Find selected employee
            var employee = await _context.Employees
                .AsNoTracking()
                .FirstOrDefaultAsync(e =>
                    e.EmployeeId == model.EmployeeId);

            if (employee == null)
            {
                ModelState.AddModelError(
                    "EmployeeId",
                    "Selected employee was not found.");

                await LoadAvailableEmployees(model);
                return View(model);
            }

            // Check employee email
            if (string.IsNullOrWhiteSpace(employee.Email))
            {
                ModelState.AddModelError(
                    string.Empty,
                    "This employee does not have an email address.");

                await LoadAvailableEmployees(model);
                return View(model);
            }

            var email = employee.Email.Trim();

            // Prevent duplicate Employee account
            var existingEmployeeAccount = await _context.Users
                .AnyAsync(u => u.EmployeeId == model.EmployeeId);

            if (existingEmployeeAccount)
            {
                ModelState.AddModelError(
                    "EmployeeId",
                    "This employee already has a login account.");

                await LoadAvailableEmployees(model);
                return View(model);
            }

            // Prevent duplicate email
            var existingEmail = await _userManager
                .FindByEmailAsync(email);

            if (existingEmail != null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "An account with this email already exists.");

                await LoadAvailableEmployees(model);
                return View(model);
            }

            // Create Identity account
            var user = new ApplicationUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                EmployeeId = model.EmployeeId
            };

            var result = await _userManager.CreateAsync(
                user,
                model.Password);

            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                await LoadAvailableEmployees(model);
                return View(model);
            }

            // Assign Employee role
            var roleResult = await _userManager.AddToRoleAsync(
                user,
                "Employee");

            if (!roleResult.Succeeded)
            {
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                await LoadAvailableEmployees(model);
                return View(model);
            }

            TempData["SuccessMessage"] =
                $"Employee login account created successfully for {employee.EmployeeName}.";

            return RedirectToAction(nameof(Create));
        }

        // POST: Reset Rahim's Password
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ResetRahimPassword()
        {
            var user = await _userManager.FindByEmailAsync(
                "rahim@example.com");

            if (user == null)
            {
                TempData["ErrorMessage"] =
                    "Rahim's account was not found.";

                return RedirectToAction(nameof(Create));
            }

            var token = await _userManager
                .GeneratePasswordResetTokenAsync(user);

            var result = await _userManager.ResetPasswordAsync(
                user,
                token,
                "Rahim@12345");

            if (result.Succeeded)
            {
                await _userManager.ResetAccessFailedCountAsync(user);

                TempData["SuccessMessage"] =
                    "Rahim's password has been reset successfully.";
            }
            else
            {
                TempData["ErrorMessage"] =
                    string.Join(
                        " | ",
                        result.Errors.Select(e => e.Description));
            }

            return RedirectToAction(nameof(Create));
        }

        // Load employees without login accounts
        private async Task LoadAvailableEmployees(
            EmployeeAccountViewModel model)
        {
            model.AvailableEmployees = await _context.Employees
                .AsNoTracking()
                .Where(e =>
                    !_context.Users.Any(u =>
                        u.EmployeeId == e.EmployeeId))
                .OrderBy(e => e.EmployeeName)
                .Select(e => new SelectListItem
                {
                    Value = e.EmployeeId.ToString(),
                    Text = e.EmployeeName +
                           " (ID: " + e.EmployeeId + ") - " +
                           e.Email
                })
                .ToListAsync();
        }
    }
}

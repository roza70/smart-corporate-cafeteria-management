
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
    public class StaffAccountController : Controller
    {
        private readonly CafeteriaDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public StaffAccountController(
            CafeteriaDbContext context,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: StaffAccount/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new StaffAccountViewModel();

            await LoadAvailableStaff(model);

            return View(model);
        }

        // POST: StaffAccount/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(
            StaffAccountViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadAvailableStaff(model);
                return View(model);
            }

            // Find selected active staff member
            var staff = await _context.CafeteriaStaff
                .AsNoTracking()
                .FirstOrDefaultAsync(s =>
                    s.StaffID == model.StaffID &&
                    s.Status);

            if (staff == null)
            {
                ModelState.AddModelError(
                    "StaffID",
                    "Selected staff member is not available.");

                await LoadAvailableStaff(model);
                return View(model);
            }

            // Prevent duplicate staff login account
            var existingAccount = await _context.Users
                .AnyAsync(u => u.StaffID == model.StaffID);

            if (existingAccount)
            {
                ModelState.AddModelError(
                    "StaffID",
                    "This staff member already has a login account.");

                await LoadAvailableStaff(model);
                return View(model);
            }

            // Find the staff member's designation
            var staffAssignment = await _context.StaffRoleAssignments
                .AsNoTracking()
                .Include(a => a.StaffRole)
                .FirstOrDefaultAsync(a =>
                    a.StaffID == model.StaffID);

            if (staffAssignment?.StaffRole == null)
            {
                ModelState.AddModelError(
                    "StaffID",
                    "No staff designation is assigned to this staff member.");

                await LoadAvailableStaff(model);
                return View(model);
            }

            // Determine Identity Login Role
            var designation = staffAssignment.StaffRole.RoleName;

            string? loginRole = designation switch
            {
                "Chef" => "Kitchen Staff",
                "Cook" => "Kitchen Staff",
                "Kitchen Assistant" => "Kitchen Staff",
                "Meal Collection Operator" => "Kitchen Staff",
                "Cleaner" => "Kitchen Staff",
                "Procurement Officer" => "Procurement Staff",
                _ => null
            };

            if (loginRole == null)
            {
                ModelState.AddModelError(
                    "StaffID",
                    "This staff designation is not eligible for a login account yet.");

                await LoadAvailableStaff(model);
                return View(model);
            }

            // Prevent duplicate email account
            var existingEmail = await _userManager.FindByEmailAsync(
                model.Email);

            if (existingEmail != null)
            {
                ModelState.AddModelError(
                    "Email",
                    "An account with this email already exists.");

                await LoadAvailableStaff(model);
                return View(model);
            }

            // Create Identity account
            var user = new ApplicationUser
            {
                UserName = model.Email,
                Email = model.Email,
                EmailConfirmed = true,
                StaffID = model.StaffID
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

                await LoadAvailableStaff(model);
                return View(model);
            }

            // Assign role according to staff designation
            var roleResult = await _userManager.AddToRoleAsync(
                user,
                loginRole);

            if (!roleResult.Succeeded)
            {
                // Delete account if role assignment fails
                await _userManager.DeleteAsync(user);

                foreach (var error in roleResult.Errors)
                {
                    ModelState.AddModelError(
                        string.Empty,
                        error.Description);
                }

                await LoadAvailableStaff(model);
                return View(model);
            }

            TempData["SuccessMessage"] =
                $"{loginRole} login account created successfully.";

            return RedirectToAction(nameof(Create));
        }

        // Load active staff members without login accounts
        private async Task LoadAvailableStaff(
            StaffAccountViewModel model)
        {
            model.AvailableStaff = await _context.CafeteriaStaff
                .AsNoTracking()
                .Where(s =>
                    s.Status &&
                    !_context.Users.Any(u =>
                        u.StaffID == s.StaffID))
                .OrderBy(s => s.StaffName)
                .Select(s => new SelectListItem
                {
                    Value = s.StaffID.ToString(),
                    Text = s.StaffName +
                           " (ID: " + s.StaffID + ")"
                })
                .ToListAsync();
        }
    }
}

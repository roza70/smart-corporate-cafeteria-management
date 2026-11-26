
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;
using smart_corporate_cafeteria_management.ViewModels;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class CafeteriaStaffController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public CafeteriaStaffController(CafeteriaDbContext context)
        {
            _context = context;
        }

        // Display All Staff Members
        public async Task<IActionResult> Index()
        {
            var staffMembers = await _context.CafeteriaStaff
                .AsNoTracking()
                .OrderBy(s => s.StaffID)
                .ToListAsync();

            return View(staffMembers);
        }

        // GET: Create Staff
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            await LoadStaffRoles();

            var model = new CafeteriaStaffViewModel
            {
                JoiningDate = DateTime.Today,
                Status = true
            };

            return View(model);
        }

        // POST: Create Staff
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CafeteriaStaffViewModel model)
        {
            if (!ModelState.IsValid)
            {
                await LoadStaffRoles();
                return View(model);
            }

            var selectedRole = await _context.StaffRoles
                .FirstOrDefaultAsync(r => r.RoleName == model.StaffRole);

            if (selectedRole == null)
            {
                ModelState.AddModelError(
                    nameof(model.StaffRole),
                    "Please select a valid staff designation.");

                await LoadStaffRoles();
                return View(model);
            }

            var existingNID = await _context.CafeteriaStaff
                .AnyAsync(s => s.NIDNumber == model.NIDNumber);

            if (existingNID)
            {
                ModelState.AddModelError(
                    nameof(model.NIDNumber),
                    "This NID number already exists.");

                await LoadStaffRoles();
                return View(model);
            }

            var staff = new CafeteriaStaff
            {
                StaffName = model.StaffName,
                NIDNumber = model.NIDNumber,
                Gender = model.Gender,
                DateOfBirth = model.DateOfBirth,
                PhoneNumber = model.PhoneNumber,
                Address = model.Address,
                JoiningDate = model.JoiningDate,
                Salary = model.Salary,
                Status = model.Status
            };

            await using var transaction =
                await _context.Database.BeginTransactionAsync();

            try
            {
                _context.CafeteriaStaff.Add(staff);
                await _context.SaveChangesAsync();

                var assignment = new StaffRoleAssignment
                {
                    StaffID = staff.StaffID,
                    RoleID = selectedRole.RoleID
                };

                _context.StaffRoleAssignments.Add(assignment);
                await _context.SaveChangesAsync();

                await transaction.CommitAsync();

                TempData["SuccessMessage"] =
                    "Staff member created successfully.";

                return RedirectToAction(nameof(Index));
            }
            catch
            {
                await transaction.RollbackAsync();

                ModelState.AddModelError(
                    string.Empty,
                    "Unable to save staff information. Please try again.");

                await LoadStaffRoles();
                return View(model);
            }
        }

        // Load Staff Designations
        private async Task LoadStaffRoles()
        {
            var roles = await _context.StaffRoles
                .AsNoTracking()
                .OrderBy(r => r.RoleName)
                .ToListAsync();

            ViewBag.AvailableRoles = new SelectList(
                roles,
                "RoleName",
                "RoleName");
        }
    }
}

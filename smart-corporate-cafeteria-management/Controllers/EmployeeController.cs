
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;

namespace smart_corporate_cafeteria_management.Controllers
{
    [Authorize(Roles = "Admin")]
    public class EmployeeController : Controller
    {
        private readonly CafeteriaDbContext _context;

        public EmployeeController(CafeteriaDbContext context)
        {
            _context = context;
        }

        // 1. Employee List
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .OrderBy(e => e.EmployeeId)
                .ToListAsync();

            return View(employees);
        }

        // 2. Employee Details
        public async Task<IActionResult> Details(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // 3. Create Employee - GET
        public async Task<IActionResult> Create()
        {
            await LoadDepartments();
            return View();
        }

        // 4. Create Employee - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Employee employee)
        {
            // Check duplicate email
            if (!string.IsNullOrWhiteSpace(employee.Email))
            {
                var email = employee.Email.Trim();

                var emailExists = await _context.Employees
                    .AnyAsync(e => e.Email != null &&
                        e.Email.ToLower() == email.ToLower());

                if (emailExists)
                {
                    ModelState.AddModelError(
                        "Email",
                        "An employee with this email already exists."
                    );
                }

                employee.Email = email;
            }

            // Validate department
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.DepartmentId == employee.DepartmentId);

            if (!departmentExists)
            {
                ModelState.AddModelError(
                    "DepartmentId",
                    "Please select a valid department."
                );
            }

            if (ModelState.IsValid)
            {
                _context.Employees.Add(employee);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] =
                    "Employee registered successfully.";

                return RedirectToAction(nameof(Index));
            }

            await LoadDepartments(employee.DepartmentId);
            return View(employee);
        }

        // 5. Edit Employee - GET
        public async Task<IActionResult> Edit(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            await LoadDepartments(employee.DepartmentId);
            return View(employee);
        }

        // 6. Edit Employee - POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeId)
            {
                return NotFound();
            }

            // Check duplicate email excluding current employee
            if (!string.IsNullOrWhiteSpace(employee.Email))
            {
                employee.Email = employee.Email.Trim();

                var email = employee.Email.ToLower();

                var emailExists = await _context.Employees
                    .AnyAsync(e =>
                        e.EmployeeId != id &&
                        e.Email != null &&
                        e.Email.ToLower() == email
                    );

                if (emailExists)
                {
                    ModelState.AddModelError(
                        "Email",
                        "Another employee is already using this email."
                    );
                }
            }

            // Validate department
            var departmentExists = await _context.Departments
                .AnyAsync(d => d.DepartmentId == employee.DepartmentId);

            if (!departmentExists)
            {
                ModelState.AddModelError(
                    "DepartmentId",
                    "Please select a valid department."
                );
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Employees.Update(employee);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] =
                        "Employee information updated successfully.";

                    return RedirectToAction(nameof(Index));
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!await _context.Employees.AnyAsync(
                        e => e.EmployeeId == id))
                    {
                        return NotFound();
                    }

                    throw;
                }
            }

            await LoadDepartments(employee.DepartmentId);
            return View(employee);
        }

        // 7. Delete Employee - GET
        public async Task<IActionResult> Delete(int id)
        {
            var employee = await _context.Employees
                .Include(e => e.Department)
                .AsNoTracking()
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        // 8. Delete Employee - POST
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employees
                .FirstOrDefaultAsync(e => e.EmployeeId == id);

            if (employee == null)
            {
                return NotFound();
            }

            // Prevent deletion if an Identity account is linked
            var linkedAccountExists = await _context.Users
                .AnyAsync(u => u.EmployeeId == id);

            if (linkedAccountExists)
            {
                TempData["ErrorMessage"] =
                    "This employee has a login account. Remove or unlink the account before deleting the employee.";

                return RedirectToAction(nameof(Index));
            }

            // Prevent deletion if meal collection history exists
            var hasMealHistory = await _context.MealCollections
                .AnyAsync(m => m.EmployeeID == id);

            if (hasMealHistory)
            {
                TempData["ErrorMessage"] =
                    "This employee has meal collection history and cannot be deleted.";

                return RedirectToAction(nameof(Index));
            }

            _context.Employees.Remove(employee);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] =
                "Employee deleted successfully.";

            return RedirectToAction(nameof(Index));
        }

        // 9. Department Dropdown
        private async Task LoadDepartments(int? selectedDepartmentId = null)
        {
            var departments = await _context.Departments
                .AsNoTracking()
                .OrderBy(d => d.DepartmentName)
                .ToListAsync();

            ViewBag.Departments = new SelectList(
                departments,
                "DepartmentId",
                "DepartmentName",
                selectedDepartmentId
            );
        }
    }
}

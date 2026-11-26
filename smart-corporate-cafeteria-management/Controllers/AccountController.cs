
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using smart_corporate_cafeteria_management.Models;
using smart_corporate_cafeteria_management.ViewModels;

namespace smart_corporate_cafeteria_management.Controllers
{
    public class AccountController : Controller
    {
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly UserManager<ApplicationUser> _userManager;

        public AccountController(
            SignInManager<ApplicationUser> signInManager,
            UserManager<ApplicationUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }

        // GET: Account/Login
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(
            LoginViewModel model,
            string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var result = await _signInManager.PasswordSignInAsync(
                model.Email,
                model.Password,
                model.RememberMe,
                lockoutOnFailure: true);

            if (result.Succeeded)
            {
                // Get the currently logged-in user
                var user = await _userManager.FindByEmailAsync(model.Email);

                // Respect a valid local return URL
                if (!string.IsNullOrEmpty(returnUrl) &&
                    Url.IsLocalUrl(returnUrl))
                {
                    return Redirect(returnUrl);
                }

                if (user != null)
                {
                    // Admin Dashboard
                    if (await _userManager.IsInRoleAsync(user, "Admin"))
                    {
                        return RedirectToAction("Index", "Dashboard");
                    }

                    // Kitchen Staff Dashboard
                    if (await _userManager.IsInRoleAsync(user, "Kitchen Staff"))
                    {
                        return RedirectToAction("Index", "KitchenStaff");
                    }

                    // Employee Dashboard
                    if (await _userManager.IsInRoleAsync(user, "Employee"))
                    {
                        return RedirectToAction("Index", "EmployeeDashboard");
                    }

                    // Procurement Staff Dashboard
                    if (await _userManager.IsInRoleAsync(user, "Procurement Staff"))
                    {
                        return RedirectToAction("Index", "ProcurementStaff");
                    }
                }

                // User has no recognized role
                await _signInManager.SignOutAsync();

                return RedirectToAction("AccessDenied", "Account");
            }

            if (result.IsLockedOut)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Your account is temporarily locked. Please try again later.");
            }
            else
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password.");
            }

            return View(model);
        }

        // POST: Account/Logout
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();

            return RedirectToAction("Login", "Account");
        }

        // GET: Account/AccessDenied
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied()
        {
            return View();
        }
    }
}

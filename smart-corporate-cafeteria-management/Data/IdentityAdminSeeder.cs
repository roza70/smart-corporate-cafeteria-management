
using Microsoft.AspNetCore.Identity;
using smart_corporate_cafeteria_management.Models;

namespace smart_corporate_cafeteria_management.Data
{
    public static class IdentityAdminSeeder
    {
        public static async Task SeedAdminAsync(
            IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var configuration = scope.ServiceProvider
                .GetRequiredService<IConfiguration>();

            var userManager = scope.ServiceProvider
                .GetRequiredService<UserManager<ApplicationUser>>();

            var email = configuration["AdminSeed:Email"];
            var password = configuration["AdminSeed:Password"];

            // Do not create an account without configured credentials.
            if (string.IsNullOrWhiteSpace(email) ||
                string.IsNullOrWhiteSpace(password))
            {
                return;
            }

            var admin = await userManager.FindByEmailAsync(email);

            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(
                    admin, password);

                if (!createResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        createResult.Errors.Select(e => e.Description));

                    throw new Exception(
                        $"Admin account creation failed: {errors}");
                }
            }

            if (!await userManager.IsInRoleAsync(admin, "Admin"))
            {
                var roleResult = await userManager.AddToRoleAsync(
                    admin, "Admin");

                if (!roleResult.Succeeded)
                {
                    var errors = string.Join(
                        "; ",
                        roleResult.Errors.Select(e => e.Description));

                    throw new Exception(
                        $"Admin role assignment failed: {errors}");
                }
            }
        }
    }
}

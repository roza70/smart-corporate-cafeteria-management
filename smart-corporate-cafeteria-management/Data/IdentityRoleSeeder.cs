
using Microsoft.AspNetCore.Identity;

namespace smart_corporate_cafeteria_management.Data
{
    public static class IdentityRoleSeeder
    {
        public static async Task SeedRolesAsync(
            IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();

            var roleManager = scope.ServiceProvider
                .GetRequiredService<RoleManager<IdentityRole>>();

            string[] roles =
            {
                "Admin",
                "Kitchen Staff",
                "Employee",
                "Procurement Staff"
            };

            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    var result = await roleManager.CreateAsync(
                        new IdentityRole(role));

                    if (!result.Succeeded)
                    {
                        var errors = string.Join(
                            "; ",
                            result.Errors.Select(e => e.Description));

                        throw new Exception(
                            $"Failed to create role '{role}': {errors}");
                    }
                }
            }
        }
    }
}

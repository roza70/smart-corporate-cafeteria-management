
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Data;
using smart_corporate_cafeteria_management.Models;
using smart_corporate_cafeteria_management.Data;
var builder = WebApplication.CreateBuilder(args);

// MVC Services
builder.Services.AddControllersWithViews();

// Database Connection
builder.Services.AddDbContext<CafeteriaDbContext>(options =>
    options.UseSqlServer(
        builder.Configuration.GetConnectionString("DefaultConnection")));

// ASP.NET Core Identity Configuration
builder.Services
    .AddIdentity<ApplicationUser, IdentityRole>(options =>
    {
        // Password requirements
        options.Password.RequiredLength = 8;
        options.Password.RequireDigit = true;
        options.Password.RequireUppercase = true;
        options.Password.RequireLowercase = true;
        options.Password.RequireNonAlphanumeric = true;

        // User settings
        options.User.RequireUniqueEmail = true;

        // Account lockout settings
        options.Lockout.MaxFailedAccessAttempts = 5;
        options.Lockout.DefaultLockoutTimeSpan =
            TimeSpan.FromMinutes(15);
    })
    .AddEntityFrameworkStores<CafeteriaDbContext>()
    .AddDefaultTokenProviders();

// Login and Access Denied Paths
builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.AccessDeniedPath = "/Account/AccessDenied";
    options.ExpireTimeSpan = TimeSpan.FromHours(8);
    options.SlidingExpiration = true;
});

var app = builder.Build();
await IdentityRoleSeeder.SeedRolesAsync(app.Services);
await IdentityAdminSeeder.SeedAdminAsync(app.Services);
// HTTP Request Pipeline
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapStaticAssets();
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Account}/{action=Login}/{id?}");

app.Run();


using Microsoft.EntityFrameworkCore;
using smart_corporate_cafeteria_management.Models;

namespace smart_corporate_cafeteria_management.Data
{
    public class CafeteriaDbContext : DbContext
    {
        public CafeteriaDbContext(
            DbContextOptions<CafeteriaDbContext> options)
            : base(options)
        {
        }

        // Employee and Department
        public DbSet<Department> Departments { get; set; }
        public DbSet<Employee> Employees { get; set; }

        // Food and Menu
        public DbSet<FoodCategory> FoodCategories { get; set; }
        public DbSet<FoodItem> FoodItems { get; set; }
        public DbSet<DailyMenu> DailyMenus { get; set; }
        public DbSet<DailyMenuItem> DailyMenuItems { get; set; }

        // Kitchen and Supplier
        public DbSet<Recipe> Recipes { get; set; }
        public DbSet<Supplier> Suppliers { get; set; }

        // Cafeteria Staff and Roles
        public DbSet<CafeteriaStaff> CafeteriaStaff { get; set; }
        public DbSet<StaffRole> StaffRoles { get; set; }
        public DbSet<StaffRoleAssignment> StaffRoleAssignments { get; set; }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);


            // =====================================
            // DEPARTMENT CONFIGURATION
            // =====================================

            // Department
            modelBuilder.Entity<Department>(entity =>
            {
                entity.ToTable("Department");

                entity.HasKey(e => e.DepartmentId);

                entity.Property(e => e.DepartmentId)
                    .HasColumnName("DepartmentID");

                entity.Property(e => e.DepartmentName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });


            // =====================================
            // EMPLOYEE CONFIGURATION
            // =====================================

            // Employee
            modelBuilder.Entity<Employee>(entity =>
            {
                entity.ToTable("Employee");

                entity.HasKey(e => e.EmployeeId);

                entity.Property(e => e.EmployeeId)
                    .HasColumnName("EmployeeID");

                entity.Property(e => e.EmployeeName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false);

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.HasOne(e => e.Department)
                    .WithMany(e => e.Employees)
                    .HasForeignKey(e => e.DepartmentId)
                    .HasConstraintName("FK_Employee_Department");
            });


            // =====================================
            // FOOD CATEGORY CONFIGURATION
            // =====================================

            // FoodCategory
            modelBuilder.Entity<FoodCategory>(entity =>
            {
                entity.ToTable("FoodCategory");

                entity.HasKey(e => e.CategoryID);

                entity.Property(e => e.CategoryName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });


            // =====================================
            // FoodItem
            modelBuilder.Entity<FoodItem>(entity =>
            {
                entity.ToTable("FoodItem");

                entity.HasKey(e => e.FoodItemId);

                entity.Property(e => e.FoodItemId)
                    .HasColumnName("FoodItemID");

                entity.Property(e => e.FoodName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.CategoryID)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.HasOne(e => e.FoodCategory)
                    .WithMany(e => e.FoodItems)
                    .HasForeignKey(e => e.CategoryID);
            });


            // =====================================
            // DAILY MENU CONFIGURATION
            // =====================================


            // DailyMenu
            modelBuilder.Entity<DailyMenu>(entity =>
            {
                entity.ToTable("DailyMenu");

                entity.HasKey(e => e.MenuID);

                entity.Property(e => e.MenuDate)
                    .HasColumnType("date")
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });

            // =====================================
            // DAILY MENU ITEM CONFIGURATION
            // =====================================


            // DailyMenuItem
            modelBuilder.Entity<DailyMenuItem>(entity =>
            {
                entity.ToTable("DailyMenuItem");

                entity.HasKey(e => e.MenuItemID);

                entity.HasOne(e => e.DailyMenu)
                    .WithMany(e => e.DailyMenuItems)
                    .HasForeignKey(e => e.MenuID);

                entity.HasOne(e => e.FoodItem)
                    .WithMany()
                    .HasForeignKey(e => e.FoodItemID);
                entity.HasIndex(e => new { e.MenuID, e.FoodItemID })
    .IsUnique()
    .HasDatabaseName("UQ_DailyMenuItem_Menu_Food");
            });


            // =====================================
            // CAFETERIA STAFF CONFIGURATION
            // =====================================
            // CafeteriaStaff
            modelBuilder.Entity<CafeteriaStaff>(entity =>
            {
                entity.ToTable("CafeteriaStaff");

                entity.HasKey(e => e.StaffID);

                entity.Property(e => e.StaffName)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.NIDNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasIndex(e => e.NIDNumber)
                    .IsUnique();

                entity.Property(e => e.Gender)
                    .HasMaxLength(10)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.DateOfBirth)
                    .HasColumnType("date");

                entity.Property(e => e.PhoneNumber)
                    .HasMaxLength(20)
                    .IsUnicode(false);

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false);

                entity.Property(e => e.JoiningDate)
                    .HasColumnType("date");

                entity.Property(e => e.Salary)
                    .HasPrecision(10, 2);

                entity.Property(e => e.Status)
                    .HasDefaultValue(true);
            });

            // =====================================
            // STAFF ROLE CONFIGURATION
            // =====================================

            // StaffRole
            modelBuilder.Entity<StaffRole>(entity =>
            {
                entity.ToTable("StaffRole");

                entity.HasKey(e => e.RoleID);

                entity.Property(e => e.RoleName)
                    .HasMaxLength(50)
                    .IsUnicode(false)
                    .IsRequired();

                entity.HasIndex(e => e.RoleName)
                    .IsUnique();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false);
            });


            // =====================================
            // STAFF ROLE ASSIGNMENT CONFIGURATION
            // =====================================

            // StaffRoleAssignment
            modelBuilder.Entity<StaffRoleAssignment>(entity =>
            {
                entity.ToTable("StaffRoleAssignment");

                entity.HasKey(e => e.AssignmentID);

                entity.HasOne(e => e.CafeteriaStaff)
                    .WithMany(e => e.StaffRoleAssignments)
                    .HasForeignKey(e => e.StaffID)
                    .HasConstraintName(
                        "FK_StaffRoleAssignment_CafeteriaStaff");

                entity.HasOne(e => e.StaffRole)
                    .WithMany(e => e.StaffRoleAssignments)
                    .HasForeignKey(e => e.RoleID)
                    .HasConstraintName(
                        "FK_StaffRoleAssignment_StaffRole");

                entity.HasIndex(e => new { e.StaffID, e.RoleID })
                    .IsUnique();
            });

            // Recipe
            modelBuilder.Entity<Recipe>(entity =>
            {
                entity.ToTable("Recipe");

                entity.HasKey(e => e.RecipeId);

                entity.Property(e => e.RecipeId)
                    .HasColumnName("RecipeID");

                entity.Property(e => e.RecipeName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Description)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.ServingSize)
                    .IsRequired();

                entity.Property(e => e.PreparationInstructions)
                    .HasColumnType("varchar(max)")
                    .IsUnicode(false)
                    .IsRequired();
            });

            // Supplier
            modelBuilder.Entity<Supplier>(entity =>
            {
                entity.ToTable("Supplier");

                entity.HasKey(e => e.SupplierId);

                entity.Property(e => e.SupplierId)
                    .HasColumnName("SupplierID");

                entity.Property(e => e.SupplierName)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.ContactPerson)
                    .HasMaxLength(100)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Phone)
                    .HasMaxLength(20)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Email)
                    .HasMaxLength(150)
                    .IsUnicode(false)
                    .IsRequired();

                entity.Property(e => e.Address)
                    .HasMaxLength(255)
                    .IsUnicode(false)
                    .IsRequired();
            });


        }
    }
}

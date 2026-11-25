
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart_corporate_cafeteria_management.Migrations
{
    public partial class AddFoodItemIsActive : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "FoodItem",
                type: "bit",
                nullable: false,
                defaultValue: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "FoodItem");
        }
    }
}

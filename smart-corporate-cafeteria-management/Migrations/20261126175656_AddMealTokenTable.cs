using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace smart_corporate_cafeteria_management.Migrations
{
    /// <inheritdoc />
    public partial class AddMealTokenTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MealToken",
                columns: table => new
                {
                    TokenID = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    EmployeeID = table.Column<int>(type: "int", nullable: false),
                    TokenNumber = table.Column<string>(type: "varchar(30)", unicode: false, maxLength: 30, nullable: false),
                    TokenDate = table.Column<DateTime>(type: "date", nullable: false),
                    Status = table.Column<string>(type: "varchar(20)", unicode: false, maxLength: 20, nullable: false),
                    GeneratedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UsedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MealToken", x => x.TokenID);
                    table.ForeignKey(
                        name: "FK_MealToken_Employee",
                        column: x => x.EmployeeID,
                        principalTable: "Employee",
                        principalColumn: "EmployeeID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MealToken_EmployeeID_TokenDate",
                table: "MealToken",
                columns: new[] { "EmployeeID", "TokenDate" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_MealToken_TokenNumber",
                table: "MealToken",
                column: "TokenNumber",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MealToken");
        }
    }
}

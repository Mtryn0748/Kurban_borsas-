using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kurban.Migrations
{
    /// <inheritdoc />
    public partial class AddAgeInAnimalCs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AnimalPrice",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "PricePerKg",
                table: "Animals");

            migrationBuilder.AddColumn<string>(
                name: "Age",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "InventoryStatus",
                table: "Animals",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Age",
                table: "Animals");

            migrationBuilder.DropColumn(
                name: "InventoryStatus",
                table: "Animals");

            migrationBuilder.AddColumn<decimal>(
                name: "AnimalPrice",
                table: "Animals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<decimal>(
                name: "PricePerKg",
                table: "Animals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);
        }
    }
}

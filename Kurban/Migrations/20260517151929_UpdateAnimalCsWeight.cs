using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kurban.Migrations
{
    /// <inheritdoc />
    public partial class UpdateAnimalCsWeight : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<decimal>(
                name: "AnimalWeight",
                table: "Animals",
                type: "decimal(18,3)",
                nullable: false,
                oldClrType: typeof(double),
                oldType: "float");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<double>(
                name: "AnimalWeight",
                table: "Animals",
                type: "float",
                nullable: false,
                oldClrType: typeof(decimal),
                oldType: "decimal(18,3)");
        }
    }
}

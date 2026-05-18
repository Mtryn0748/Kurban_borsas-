using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Kurban.Migrations
{
    /// <inheritdoc />
    public partial class AddIsSoldToAnimal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsSold",
                table: "Animals",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsSold",
                table: "Animals");
        }
    }
}

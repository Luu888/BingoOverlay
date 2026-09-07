using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingoOverlay.Migrations
{
    /// <inheritdoc />
    public partial class AddVipAllowed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "AllowVips",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AllowVips",
                table: "Settings");
        }
    }
}

using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingoOverlay.Migrations
{
    /// <inheritdoc />
    public partial class AddColumnsToSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "HideOverlayAfterTime",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "HideOverlaySeconds",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOverlayVisible",
                table: "Settings",
                type: "INTEGER",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<DateTime>(
                name: "LastOverlayActivity",
                table: "Settings",
                type: "TEXT",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HideOverlayAfterTime",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "HideOverlaySeconds",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "IsOverlayVisible",
                table: "Settings");

            migrationBuilder.DropColumn(
                name: "LastOverlayActivity",
                table: "Settings");
        }
    }
}

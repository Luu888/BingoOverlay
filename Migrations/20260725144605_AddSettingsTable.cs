using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BingoOverlay.Migrations
{
    /// <inheritdoc />
    public partial class AddSettingsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BingoAppearance",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    TileColor = table.Column<string>(type: "TEXT", nullable: false),
                    TileColorEnd = table.Column<string>(type: "TEXT", nullable: false),
                    CompletedColor = table.Column<string>(type: "TEXT", nullable: false),
                    CompletedColorEnd = table.Column<string>(type: "TEXT", nullable: false),
                    TextColor = table.Column<string>(type: "TEXT", nullable: false),
                    BorderRadius = table.Column<int>(type: "INTEGER", nullable: false),
                    TileSize = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BingoAppearance", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Settings",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AllowModerators = table.Column<bool>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Settings", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TwitchAuth",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    AccessToken = table.Column<string>(type: "TEXT", nullable: false),
                    RefreshToken = table.Column<string>(type: "TEXT", nullable: false),
                    BroadcasterId = table.Column<string>(type: "TEXT", nullable: false),
                    DisplayName = table.Column<string>(type: "TEXT", nullable: false),
                    ExpiresAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TwitchAuth", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "BingoAppearance");

            migrationBuilder.DropTable(
                name: "Settings");

            migrationBuilder.DropTable(
                name: "TwitchAuth");
        }
    }
}

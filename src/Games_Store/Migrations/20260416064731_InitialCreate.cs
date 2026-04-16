using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Games_Store.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Games",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 1000, nullable: false),
                    Price = table.Column<decimal>(type: "TEXT", nullable: false),
                    Genre = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    ImageUrl = table.Column<string>(type: "TEXT", maxLength: 500, nullable: false),
                    IsFeatured = table.Column<bool>(type: "INTEGER", nullable: false),
                    Rating = table.Column<double>(type: "REAL", nullable: false),
                    ReleaseDate = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Games", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Username = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    PasswordHash = table.Column<string>(type: "TEXT", nullable: false),
                    Role = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.Id);
                });

            migrationBuilder.InsertData(
                table: "Games",
                columns: new[] { "Id", "Description", "Genre", "ImageUrl", "IsFeatured", "Price", "Rating", "ReleaseDate", "Title" },
                values: new object[,]
                {
                    { 1, "An open-world action RPG set in a dystopian future city.", "RPG", "https://placehold.co/300x400/1a1a2e/e94560?text=Cyber+Warriors", true, 59.99m, 4.7999999999999998, new DateTime(2024, 3, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), "Cyber Warriors 2077" },
                    { 2, "A dark fantasy adventure with intense combat mechanics.", "Action", "https://placehold.co/300x400/16213e/0f3460?text=Shadow+Legends", true, 49.99m, 4.5, new DateTime(2024, 6, 20, 0, 0, 0, 0, DateTimeKind.Unspecified), "Shadow Legends" },
                    { 3, "High-speed racing with realistic physics and stunning tracks.", "Racing", "https://placehold.co/300x400/1b1b2f/e43f5a?text=Racing+Thunder", false, 39.99m, 4.2000000000000002, new DateTime(2024, 1, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), "Racing Thunder" },
                    { 4, "Lead your fleet through space in this epic strategy game.", "Strategy", "https://placehold.co/300x400/0a1931/185adb?text=Galaxy+Commander", true, 44.99m, 4.7000000000000002, new DateTime(2024, 9, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), "Galaxy Commander" },
                    { 5, "Explore procedurally generated dungeons full of traps and treasures.", "RPG", "https://placehold.co/300x400/2d132c/c72c41?text=Dungeon+Crawler", false, 29.99m, 4.0, new DateTime(2023, 11, 25, 0, 0, 0, 0, DateTimeKind.Unspecified), "Dungeon Crawler" },
                    { 6, "Build and manage the ultimate football team to glory.", "Sports", "https://placehold.co/300x400/1a1a40/4834d4?text=Football+Manager", false, 34.99m, 3.8999999999999999, new DateTime(2024, 8, 12, 0, 0, 0, 0, DateTimeKind.Unspecified), "Football Manager Pro" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_Username",
                table: "Users",
                column: "Username",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Games");

            migrationBuilder.DropTable(
                name: "Users");
        }
    }
}

using System.IO;
using Games_Store.Models;
using Microsoft.EntityFrameworkCore;

namespace Games_Store.Data
{
    public class AppDbContext : DbContext
    {
        public DbSet<Game> Games { get; set; }
        public DbSet<User> Users { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var dbPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "GamesStore.db");
            optionsBuilder.UseSqlite($"Data Source={dbPath}");
        }

        public bool AdminExists() => Users.Any(u => u.Role == "Admin");

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.Id);
                entity.HasIndex(u => u.Username).IsUnique();
            });

            modelBuilder.Entity<Game>(entity =>
            {
                entity.HasKey(g => g.Id);
            });

            // Seed some initial games
            modelBuilder.Entity<Game>().HasData(
                new Game
                {
                    Id = 1,
                    Title = "Cyber Warriors 2077",
                    Description = "An open-world action RPG set in a dystopian future city.",
                    Price = 59.99m,
                    Genre = "RPG",
                    ImageUrl = "https://placehold.co/300x400/1a1a2e/e94560?text=Cyber+Warriors",
                    IsFeatured = true,
                    Rating = 4.8,
                    ReleaseDate = new DateTime(2024, 3, 15)
                },
                new Game
                {
                    Id = 2,
                    Title = "Shadow Legends",
                    Description = "A dark fantasy adventure with intense combat mechanics.",
                    Price = 49.99m,
                    Genre = "Action",
                    ImageUrl = "https://placehold.co/300x400/16213e/0f3460?text=Shadow+Legends",
                    IsFeatured = true,
                    Rating = 4.5,
                    ReleaseDate = new DateTime(2024, 6, 20)
                },
                new Game
                {
                    Id = 3,
                    Title = "Racing Thunder",
                    Description = "High-speed racing with realistic physics and stunning tracks.",
                    Price = 39.99m,
                    Genre = "Racing",
                    ImageUrl = "https://placehold.co/300x400/1b1b2f/e43f5a?text=Racing+Thunder",
                    IsFeatured = false,
                    Rating = 4.2,
                    ReleaseDate = new DateTime(2024, 1, 10)
                },
                new Game
                {
                    Id = 4,
                    Title = "Galaxy Commander",
                    Description = "Lead your fleet through space in this epic strategy game.",
                    Price = 44.99m,
                    Genre = "Strategy",
                    ImageUrl = "https://placehold.co/300x400/0a1931/185adb?text=Galaxy+Commander",
                    IsFeatured = true,
                    Rating = 4.7,
                    ReleaseDate = new DateTime(2024, 9, 5)
                },
                new Game
                {
                    Id = 5,
                    Title = "Dungeon Crawler",
                    Description = "Explore procedurally generated dungeons full of traps and treasures.",
                    Price = 29.99m,
                    Genre = "RPG",
                    ImageUrl = "https://placehold.co/300x400/2d132c/c72c41?text=Dungeon+Crawler",
                    IsFeatured = false,
                    Rating = 4.0,
                    ReleaseDate = new DateTime(2023, 11, 25)
                },
                new Game
                {
                    Id = 6,
                    Title = "Football Manager Pro",
                    Description = "Build and manage the ultimate football team to glory.",
                    Price = 34.99m,
                    Genre = "Sports",
                    ImageUrl = "https://placehold.co/300x400/1a1a40/4834d4?text=Football+Manager",
                    IsFeatured = false,
                    Rating = 3.9,
                    ReleaseDate = new DateTime(2024, 8, 12)
                }
            );
        }
    }
}

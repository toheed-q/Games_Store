using System.ComponentModel.DataAnnotations;

namespace Games_Store.Models
{
    public class Game
    {
        public int Id { get; set; }

        [Required]
        [MaxLength(200)]
        public string Title { get; set; } = string.Empty;

        [MaxLength(1000)]
        public string Description { get; set; } = string.Empty;

        [Required]
        public decimal Price { get; set; }

        [MaxLength(100)]
        public string Genre { get; set; } = string.Empty;

        [MaxLength(500)]
        public string ImageUrl { get; set; } = string.Empty;

        public bool IsFeatured { get; set; }

        public double Rating { get; set; }

        public DateTime ReleaseDate { get; set; } = DateTime.Now;
    }
}

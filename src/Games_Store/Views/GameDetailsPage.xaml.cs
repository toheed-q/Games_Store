using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using Games_Store.Models;

namespace Games_Store.Views
{
    public partial class GameDetailsPage : UserControl
    {
        public GameDetailsPage(Game game)
        {
            InitializeComponent();
            LoadGameDetails(game);
        }

        private void LoadGameDetails(Game game)
        {
            TxtTitle.Text = game.Title;
            TxtGenre.Text = game.Genre;
            TxtReleaseDate.Text = $"Released: {game.ReleaseDate:MMM dd, yyyy}";
            TxtDescription.Text = game.Description;
            TxtPrice.Text = $"${game.Price:F2}";
            TxtRating.Text = $"{game.Rating:F1}";
            TxtStars.Text = GenerateStarText(game.Rating);

            if (game.IsFeatured)
                FeaturedBadge.Visibility = Visibility.Visible;

            // Load image if file exists
            if (!string.IsNullOrEmpty(game.ImageUrl) && File.Exists(game.ImageUrl))
            {
                var bitmap = new BitmapImage();
                bitmap.BeginInit();
                bitmap.UriSource = new Uri(game.ImageUrl, UriKind.Absolute);
                bitmap.DecodePixelWidth = 900;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.EndInit();
                bitmap.Freeze();
                HeroImage.Source = bitmap;
            }
        }

        private static string GenerateStarText(double rating)
        {
            int full = (int)rating;
            bool half = (rating - full) >= 0.3;
            int empty = 5 - full - (half ? 1 : 0);
            return new string('\u2605', full) + (half ? "\u00BD" : "") + new string('\u2606', empty);
        }

        private void OnBackClick(object sender, RoutedEventArgs e)
        {
            // Navigate back by finding the MainWindow and setting content
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateToStore();
            }
        }
    }
}

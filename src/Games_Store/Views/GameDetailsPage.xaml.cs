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

            // Load image from URL or local path
            if (!string.IsNullOrEmpty(game.ImageUrl))
            {
                try
                {
                    var bitmap = new BitmapImage();
                    bitmap.BeginInit();
                    bitmap.UriSource = new Uri(game.ImageUrl, UriKind.Absolute);
                    bitmap.CacheOption = BitmapCacheOption.OnLoad;
                    bitmap.CreateOptions = BitmapCreateOptions.IgnoreColorProfile;
                    bitmap.EndInit();
                    HeroImage.Source = bitmap;
                    HeroIcon.Visibility = Visibility.Collapsed;
                }
                catch
                {
                    HeroIcon.Visibility = Visibility.Visible;
                }
            }
            else
            {
                HeroIcon.Visibility = Visibility.Visible;
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

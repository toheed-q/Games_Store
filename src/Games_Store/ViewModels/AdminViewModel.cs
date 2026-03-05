using System.Collections.ObjectModel;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Games_Store.Data;
using Games_Store.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;

namespace Games_Store.ViewModels
{
    public class AdminViewModel : BaseViewModel
    {
        private ObservableCollection<Game> _games = new();
        private Game? _selectedGame;
        private string _searchText = string.Empty;

        // Form fields for add/edit
        private string _formTitle = string.Empty;
        private string _formDescription = string.Empty;
        private string _formPrice = string.Empty;
        private string _formGenre = string.Empty;
        private string _formImageUrl = string.Empty;
        private bool _formIsFeatured;
        private bool _isEditing;

        public ObservableCollection<Game> Games
        {
            get => _games;
            set => SetProperty(ref _games, value);
        }

        public Game? SelectedGame
        {
            get => _selectedGame;
            set
            {
                SetProperty(ref _selectedGame, value);
                OnPropertyChanged(nameof(HasSelection));
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                SetProperty(ref _searchText, value);
                LoadGames();
            }
        }

        public bool HasSelection => SelectedGame != null;

        // Form properties
        public string FormTitle
        {
            get => _formTitle;
            set => SetProperty(ref _formTitle, value);
        }

        public string FormDescription
        {
            get => _formDescription;
            set => SetProperty(ref _formDescription, value);
        }

        public string FormPrice
        {
            get => _formPrice;
            set => SetProperty(ref _formPrice, value);
        }

        public string FormGenre
        {
            get => _formGenre;
            set => SetProperty(ref _formGenre, value);
        }

        public string FormImageUrl
        {
            get => _formImageUrl;
            set => SetProperty(ref _formImageUrl, value);
        }

        public bool FormIsFeatured
        {
            get => _formIsFeatured;
            set => SetProperty(ref _formIsFeatured, value);
        }

        public bool IsEditing
        {
            get => _isEditing;
            set => SetProperty(ref _isEditing, value);
        }

        // Commands
        public ICommand AddGameCommand { get; }
        public ICommand EditGameCommand { get; }
        public ICommand DeleteGameCommand { get; }
        public ICommand SaveGameCommand { get; }
        public ICommand CancelCommand { get; }
        public ICommand ToggleFeaturedCommand { get; }
        public ICommand BrowseImageCommand { get; }

        public AdminViewModel()
        {
            AddGameCommand = new RelayCommand(_ => StartAdd());
            EditGameCommand = new RelayCommand(_ => StartEdit(), _ => HasSelection);
            DeleteGameCommand = new RelayCommand(_ => DeleteGame(), _ => HasSelection);
            SaveGameCommand = new RelayCommand(_ => SaveGame());
            CancelCommand = new RelayCommand(_ => CancelEdit());
            ToggleFeaturedCommand = new RelayCommand(_ => ToggleFeatured(), _ => HasSelection);
            BrowseImageCommand = new RelayCommand(_ => BrowseImage());

            LoadGames();
        }

        public void LoadGames()
        {
            using var context = new AppDbContext();
            var query = context.Games.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(g => g.Title.Contains(SearchText));
            }

            Games = new ObservableCollection<Game>(query.OrderBy(g => g.Title).ToList());
        }

        private void StartAdd()
        {
            SelectedGame = null;
            ClearForm();
            IsEditing = true;
        }

        private void StartEdit()
        {
            if (SelectedGame == null) return;

            FormTitle = SelectedGame.Title;
            FormDescription = SelectedGame.Description;
            FormPrice = SelectedGame.Price.ToString("F2");
            FormGenre = SelectedGame.Genre;
            FormImageUrl = SelectedGame.ImageUrl;
            FormIsFeatured = SelectedGame.IsFeatured;
            IsEditing = true;
        }

        private void SaveGame()
        {
            if (string.IsNullOrWhiteSpace(FormTitle))
            {
                MessageBox.Show("Title is required.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            if (!decimal.TryParse(FormPrice, out var price) || price < 0)
            {
                MessageBox.Show("Please enter a valid price.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            using var context = new AppDbContext();

            if (SelectedGame != null)
            {
                // Edit existing
                var game = context.Games.Find(SelectedGame.Id);
                if (game != null)
                {
                    game.Title = FormTitle;
                    game.Description = FormDescription;
                    game.Price = price;
                    game.Genre = FormGenre;
                    game.ImageUrl = FormImageUrl;
                    game.IsFeatured = FormIsFeatured;
                    context.SaveChanges();
                }
            }
            else
            {
                // Add new
                var game = new Game
                {
                    Title = FormTitle,
                    Description = FormDescription,
                    Price = price,
                    Genre = FormGenre,
                    ImageUrl = FormImageUrl,
                    IsFeatured = FormIsFeatured,
                    ReleaseDate = DateTime.Now
                };
                context.Games.Add(game);
                context.SaveChanges();
            }

            IsEditing = false;
            ClearForm();
            LoadGames();
        }

        private void DeleteGame()
        {
            if (SelectedGame == null) return;

            var result = MessageBox.Show(
                $"Are you sure you want to delete '{SelectedGame.Title}'?",
                "Confirm Delete",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result != MessageBoxResult.Yes) return;

            using var context = new AppDbContext();
            var game = context.Games.Find(SelectedGame.Id);
            if (game != null)
            {
                context.Games.Remove(game);
                context.SaveChanges();
            }

            SelectedGame = null;
            LoadGames();
        }

        private void ToggleFeatured()
        {
            if (SelectedGame == null) return;

            using var context = new AppDbContext();
            var game = context.Games.Find(SelectedGame.Id);
            if (game != null)
            {
                game.IsFeatured = !game.IsFeatured;
                context.SaveChanges();
            }

            LoadGames();
        }

        private void BrowseImage()
        {
            var dialog = new OpenFileDialog
            {
                Title = "Select Game Cover Image",
                Filter = "Image Files|*.jpg;*.jpeg;*.png;*.bmp;*.gif|All Files|*.*"
            };

            if (dialog.ShowDialog() == true)
            {
                // Copy image to app's Images folder so path is portable
                var imagesDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Images");
                Directory.CreateDirectory(imagesDir);

                var fileName = $"{Guid.NewGuid()}{Path.GetExtension(dialog.FileName)}";
                var destPath = Path.Combine(imagesDir, fileName);
                File.Copy(dialog.FileName, destPath, true);

                FormImageUrl = destPath;
            }
        }

        private void CancelEdit()
        {
            IsEditing = false;
            ClearForm();
        }

        private void ClearForm()
        {
            FormTitle = string.Empty;
            FormDescription = string.Empty;
            FormPrice = string.Empty;
            FormGenre = string.Empty;
            FormImageUrl = string.Empty;
            FormIsFeatured = false;
        }
    }
}

using System.Collections.ObjectModel;
using Games_Store.Data;
using Games_Store.Models;

namespace Games_Store.ViewModels
{
    public class CategoryItem : BaseViewModel
    {
        private bool _isSelected;

        public string Name { get; }

        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        public CategoryItem(string name, bool isSelected = false)
        {
            Name = name;
            _isSelected = isSelected;
        }
    }

    public class StoreViewModel : BaseViewModel
    {
        private ObservableCollection<Game> _featuredGames = new();
        private ObservableCollection<Game> _allGames = new();
        private ObservableCollection<Game> _carouselGames = new();
        private ObservableCollection<CategoryItem> _categories = new();
        private Game? _selectedGame;
        private string _searchText = string.Empty;
        private string _selectedCategory = "All";

        public ObservableCollection<Game> FeaturedGames
        {
            get => _featuredGames;
            set => SetProperty(ref _featuredGames, value);
        }

        public ObservableCollection<Game> AllGames
        {
            get => _allGames;
            set => SetProperty(ref _allGames, value);
        }

        public ObservableCollection<Game> CarouselGames
        {
            get => _carouselGames;
            set => SetProperty(ref _carouselGames, value);
        }

        public ObservableCollection<CategoryItem> Categories
        {
            get => _categories;
            set => SetProperty(ref _categories, value);
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

        public bool HasSelection => SelectedGame != null;

        public string SelectedCategory
        {
            get => _selectedCategory;
            set
            {
                if (SetProperty(ref _selectedCategory, value))
                {
                    UpdateCategorySelection();
                    LoadGames();
                }
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

        public RelayCommand SelectCategoryCommand { get; }

        public StoreViewModel()
        {
            SelectCategoryCommand = new RelayCommand(
                param => SelectedCategory = param as string ?? "All");

            LoadCategories();
            LoadGames();
        }

        private void LoadCategories()
        {
            var categoryNames = new[] { "All", "Action", "Adventure", "RPG", "Strategy", "Racing", "Sports" };
            Categories = new ObservableCollection<CategoryItem>(
                categoryNames.Select(c => new CategoryItem(c, c == _selectedCategory)));
        }

        private void UpdateCategorySelection()
        {
            foreach (var cat in Categories)
                cat.IsSelected = cat.Name == _selectedCategory;
        }

        public void LoadGames()
        {
            using var context = new AppDbContext();

            var query = context.Games.AsQueryable();

            if (!string.IsNullOrWhiteSpace(SearchText))
            {
                query = query.Where(g => g.Title.Contains(SearchText));
            }

            if (_selectedCategory != "All")
            {
                query = query.Where(g => g.Genre == _selectedCategory);
            }

            var games = query.OrderBy(g => g.Title).ToList();

            FeaturedGames = new ObservableCollection<Game>(games.Where(g => g.IsFeatured));
            AllGames = new ObservableCollection<Game>(games);

            // Carousel shows only featured games (unfiltered by category)
            using var ctx2 = new AppDbContext();
            var featured = ctx2.Games.Where(g => g.IsFeatured).OrderBy(g => g.Title).ToList();
            CarouselGames = new ObservableCollection<Game>(featured);
        }
    }
}

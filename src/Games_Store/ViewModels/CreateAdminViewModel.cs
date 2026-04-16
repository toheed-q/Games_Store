using System.Windows.Input;
using Games_Store.Data;
using Games_Store.Helpers;
using Games_Store.Models;

namespace Games_Store.ViewModels
{
    public class CreateAdminViewModel : BaseViewModel
    {
        private string _username = string.Empty;
        private string _errorMessage = string.Empty;
        private bool _isLoading;

        public string Username
        {
            get => _username;
            set => SetProperty(ref _username, value);
        }

        public string ErrorMessage
        {
            get => _errorMessage;
            set
            {
                SetProperty(ref _errorMessage, value);
                OnPropertyChanged(nameof(HasError));
            }
        }

        public bool HasError => !string.IsNullOrEmpty(_errorMessage);

        public bool IsLoading
        {
            get => _isLoading;
            set => SetProperty(ref _isLoading, value);
        }

        public ICommand CreateCommand { get; }

        public Action? OnAdminCreated { get; set; }

        public CreateAdminViewModel()
        {
            CreateCommand = new RelayCommand(_ => Create(), _ => !IsLoading);
        }

        public void Create(string password = "", string confirmPassword = "")
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "All fields are required.";
                return;
            }

            if (Username.Length < 3)
            {
                ErrorMessage = "Username must be at least 3 characters.";
                return;
            }

            if (password.Length < 6)
            {
                ErrorMessage = "Password must be at least 6 characters.";
                return;
            }

            if (password != confirmPassword)
            {
                ErrorMessage = "Passwords do not match.";
                return;
            }

            IsLoading = true;
            try
            {
                using var context = new AppDbContext();

                if (context.Users.Any(u => u.Username == Username))
                {
                    ErrorMessage = "Username already taken.";
                    return;
                }

                context.Users.Add(new User
                {
                    Username = Username,
                    PasswordHash = PasswordHelper.HashPassword(password),
                    Role = "Admin"
                });
                context.SaveChanges();

                OnAdminCreated?.Invoke();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

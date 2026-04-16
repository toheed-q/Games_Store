using System.Windows.Input;
using Games_Store.Data;
using Games_Store.Helpers;

namespace Games_Store.ViewModels
{
    public class LoginViewModel : BaseViewModel
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

        public ICommand LoginCommand { get; }
        public ICommand GoToSignupCommand { get; }

        // Actions set by the View
        public Action? OnLoginSuccess { get; set; }
        public Action? OnGoToSignup { get; set; }

        public LoginViewModel()
        {
            LoginCommand = new RelayCommand(_ => Login(), _ => !IsLoading);
            GoToSignupCommand = new RelayCommand(_ => OnGoToSignup?.Invoke());
        }

        public void Login(string password = "")
        {
            ErrorMessage = string.Empty;

            if (string.IsNullOrWhiteSpace(Username) || string.IsNullOrWhiteSpace(password))
            {
                ErrorMessage = "Please enter username and password.";
                return;
            }

            IsLoading = true;
            try
            {
                using var context = new AppDbContext();
                var user = context.Users.FirstOrDefault(u => u.Username == Username);

                if (user == null || !PasswordHelper.VerifyPassword(password, user.PasswordHash))
                {
                    ErrorMessage = "Invalid username or password.";
                    return;
                }

                SessionManager.CurrentUser = user;
                OnLoginSuccess?.Invoke();
            }
            finally
            {
                IsLoading = false;
            }
        }
    }
}

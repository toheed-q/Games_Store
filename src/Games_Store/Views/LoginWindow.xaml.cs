using System.Windows;
using Games_Store.ViewModels;

namespace Games_Store.Views
{
    public partial class LoginWindow : Window
    {
        private readonly LoginViewModel _vm;

        public LoginWindow()
        {
            InitializeComponent();
            _vm = new LoginViewModel();
            DataContext = _vm;

            _vm.OnLoginSuccess = () => { DialogResult = true; Close(); };
            _vm.OnGoToSignup = () =>
            {
                var signup = new SignupWindow();
                signup.Owner = this;
                if (signup.ShowDialog() == true)
                    TxtUsername.Text = signup.RegisteredUsername;
            };
        }

        private void BtnLogin_Click(object sender, RoutedEventArgs e)
            => _vm.Login(PwdPassword.Password);

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
            Close();
        }

        private void OnTitleBarDrag(object sender, System.Windows.Input.MouseButtonEventArgs e)
            => DragMove();
    }
}

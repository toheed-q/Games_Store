using System.Windows;
using Games_Store.ViewModels;

namespace Games_Store.Views
{
    public partial class SignupWindow : Window
    {
        private readonly SignupViewModel _vm;

        public string RegisteredUsername { get; private set; } = string.Empty;

        public SignupWindow()
        {
            InitializeComponent();
            _vm = new SignupViewModel();
            DataContext = _vm;

            _vm.OnSignupSuccess = () =>
            {
                RegisteredUsername = _vm.Username;
                Task.Delay(1200).ContinueWith(_ =>
                    Dispatcher.Invoke(() => { DialogResult = true; Close(); }));
            };

            _vm.OnGoToLogin = () => { DialogResult = false; Close(); };
        }

        private void BtnSignup_Click(object sender, RoutedEventArgs e)
            => _vm.Signup(PwdPassword.Password, PwdConfirm.Password);

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

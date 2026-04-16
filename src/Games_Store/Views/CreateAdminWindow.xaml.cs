using System.Windows;
using Games_Store.ViewModels;

namespace Games_Store.Views
{
    public partial class CreateAdminWindow : Window
    {
        private readonly CreateAdminViewModel _vm;

        public CreateAdminWindow()
        {
            InitializeComponent();
            _vm = new CreateAdminViewModel();
            DataContext = _vm;

            _vm.OnAdminCreated = () => { DialogResult = true; Close(); };
        }

        private void BtnCreate_Click(object sender, RoutedEventArgs e)
            => _vm.Create(PwdPassword.Password, PwdConfirm.Password);

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

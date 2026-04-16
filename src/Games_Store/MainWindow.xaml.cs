using System.Windows;
using System.Windows.Input;
using Games_Store.Helpers;
using Games_Store.Models;
using Games_Store.Views;

namespace Games_Store
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();

            BtnAdmin.Visibility = SessionManager.IsAdmin
                ? Visibility.Visible
                : Visibility.Collapsed;

            TxtUsername.Text = SessionManager.CurrentUser?.Username;
            ContentArea.Content = new PublicStore();
        }

        private void OnTitleBarDrag(object sender, MouseButtonEventArgs e)
        {
            if (e.ButtonState == MouseButtonState.Pressed)
                DragMove();
        }

        private void BtnMinimize_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState.Minimized;

        private void BtnMaxRestore_Click(object sender, RoutedEventArgs e)
            => WindowState = WindowState == WindowState.Maximized
                ? WindowState.Normal
                : WindowState.Maximized;

        private void BtnClose_Click(object sender, RoutedEventArgs e)
            => Close();

        private void BtnStore_Click(object sender, RoutedEventArgs e)
            => ContentArea.Content = new PublicStore();

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
            => ContentArea.Content = new AdminPanel();

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Clear();
            Close();

            var login = new LoginWindow();
            if (login.ShowDialog() == true)
            {
                var main = new MainWindow();
                main.Closed += (_, _) => Application.Current.Shutdown();
                main.Show();
            }
            else
            {
                Application.Current.Shutdown();
            }
        }

        public void NavigateToStore()
            => ContentArea.Content = new PublicStore();

        public void NavigateToGameDetails(Game game)
            => ContentArea.Content = new GameDetailsPage(game);
    }
}

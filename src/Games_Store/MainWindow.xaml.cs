using System.Windows;
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

            // Show/hide Admin Panel based on role
            BtnAdmin.Visibility = SessionManager.IsAdmin
                ? Visibility.Visible
                : Visibility.Collapsed;

            // Show logged-in username
            TxtUsername.Text = $"👤 {SessionManager.CurrentUser?.Username}";

            ContentArea.Content = new PublicStore();
        }

        private void BtnStore_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new PublicStore();
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AdminPanel();
        }

        private void BtnLogout_Click(object sender, RoutedEventArgs e)
        {
            SessionManager.Clear();
            Close(); // close MainWindow first

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
        {
            ContentArea.Content = new PublicStore();
        }

        public void NavigateToGameDetails(Game game)
        {
            ContentArea.Content = new GameDetailsPage(game);
        }
    }
}

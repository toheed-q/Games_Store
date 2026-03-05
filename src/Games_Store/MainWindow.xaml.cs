using System.Windows;
using Games_Store.Models;
using Games_Store.Views;

namespace Games_Store
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
            ContentArea.Content = new PublicStore();
        }

        private void BtnStore_Click(object sender, RoutedEventArgs e)
        {
            NavigateToStore();
        }

        private void BtnAdmin_Click(object sender, RoutedEventArgs e)
        {
            ContentArea.Content = new AdminPanel();
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

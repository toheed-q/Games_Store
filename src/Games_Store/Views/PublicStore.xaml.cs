using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Games_Store.Models;

namespace Games_Store.Views
{
    public partial class PublicStore : UserControl
    {
        public PublicStore()
        {
            InitializeComponent();
        }

        private void OnGameCardClick(object sender, MouseButtonEventArgs e)
        {
            if (sender is FrameworkElement element && element.DataContext is Game game)
            {
                NavigateToDetails(game);
            }
        }

        private void OnCarouselGameSelected(object sender, GameSelectedEventArgs e)
        {
            NavigateToDetails(e.Game);
        }

        private void NavigateToDetails(Game game)
        {
            if (Window.GetWindow(this) is MainWindow mainWindow)
            {
                mainWindow.NavigateToGameDetails(game);
            }
        }
    }
}

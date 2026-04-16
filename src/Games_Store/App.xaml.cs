using System.Windows;
using Games_Store.Data;
using Games_Store.Views;
using Microsoft.EntityFrameworkCore;

namespace Games_Store
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            ShutdownMode = ShutdownMode.OnExplicitShutdown;

            using var context = new AppDbContext();
            context.Database.Migrate();

            // First time: no admin exists → force admin creation
            if (!context.AdminExists())
            {
                var setup = new CreateAdminWindow();
                if (setup.ShowDialog() != true)
                {
                    Shutdown();
                    return;
                }
            }

            // Normal flow: show login
            var login = new LoginWindow();
            if (login.ShowDialog() == true)
            {
                var main = new MainWindow();
                main.Closed += (_, _) => Shutdown();
                main.Show();
            }
            else
            {
                Shutdown();
            }
        }
    }
}

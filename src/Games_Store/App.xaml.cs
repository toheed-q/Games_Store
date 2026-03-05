using System.Windows;
using Games_Store.Data;
using Microsoft.EntityFrameworkCore;

namespace Games_Store
{
    public partial class App : Application
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Create database if it doesn't exist, with seed data
            using var context = new AppDbContext();
            context.Database.EnsureCreated();
        }
    }
}

using System.Windows;

namespace Ast
{
    public partial class App
    {
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            MainWindow = new MainWindow();
            MainWindow.Show();
        }
    }
}
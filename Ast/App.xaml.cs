using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using Ast.Properties;
using NLog;
using Services;

namespace Ast
{
    public partial class App
    {
        private MainWindowViewModel _vm;
        private SynchronizationContext _context;
        private Logger _logger = LogManager.GetLogger("App");

        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            _vm = new MainWindowViewModel
            {
                Pri1 = CreatePriTestViewModel("i1"),
                Pri2 = CreatePriTestViewModel("i2"),
                Pri3 = CreatePriTestViewModel("i3"),
                Pri4 = CreatePriTestViewModel("i4"),
                Connect = ConnectAsterisk
            };
            LoadAsteriskData();

            _context = SynchronizationContext.Current;

            MainWindow = new MainWindow();
            MainWindow.Show();
        }

        private void RunAsyncOnBusy(Func<Task> func)
        {
            _vm.IsBusy = true;
            Task
                .Run(func)
                .ContinueWith(t => { _vm.IsBusy = false; });
        }

        private AsteriskService GetAst()
        {
            return new AsteriskService(_vm.Sever, _vm.Port, _vm.User, _vm.Password);
        }

        private void ConnectAsterisk(MainWindowViewModel model)
        {
            RunAsyncOnBusy(async () =>
            {
                try
                {
                    _logger.Info("ConnectAsterisk");
                    await GetAst().PriGetChannels();
                    _vm.AstStatus = "OK";
                }
                catch (Exception ex)
                {
                    _logger.Error(ex);
                    _vm.AstStatus = $"Error: {ex.Message}";
                }
            });
        }

        private void LoadAsteriskData()
        {
            _vm.Sever = Settings.Default.Server;
            _vm.Port = Settings.Default.Port;
            _vm.User = Settings.Default.User;
            _vm.Password = Settings.Default.Password;
        }

        private PriTestViewModel CreatePriTestViewModel(string id)
        {
            return new PriTestViewModel
            {
                Id = id,
                Start = StartTest,
                Stop = StopTest
            };
        }

        private void StopTest(PriTestViewModel obj)
        {
        }

        private void StartTest(PriTestViewModel obj)
        {
        }
    }
}
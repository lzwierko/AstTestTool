using System;
using System.Collections.Generic;
using System.Linq;
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
        private SynchronizationContext _sc;
        private readonly Logger _logger = LogManager.GetLogger("App");
        private readonly Dictionary<string, TestRunner> _tests = new Dictionary<string, TestRunner>();
        private readonly Dictionary<string, PriMonitor> _pris = new Dictionary<string, PriMonitor>();
        private void App_OnStartup(object sender, StartupEventArgs e)
        {
            _vm = new MainWindowViewModel
            {
                Pri1 = CreatePriTestViewModel("i1", "1"),
                Pri2 = CreatePriTestViewModel("i2", "2"),
                Pri3 = CreatePriTestViewModel("i3", "3"),
                Pri4 = CreatePriTestViewModel("i4", "4"),
                Connect = ConnectAsterisk
            };
            LoadAsteriskData();

            _sc = SynchronizationContext.Current;

            MainWindow = new MainWindow
            {
                DataContext = _vm
            };
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
                    Settings.Default.Server = _vm.Sever;
                    Settings.Default.User = _vm.User;
                    Settings.Default.Password = _vm.Password;
                    Settings.Default.Port = _vm.Port;
                    Settings.Default.Save();

                    _vm.GetPris().ToList().ForEach(StartMonitor);
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

        private PriTestViewModel CreatePriTestViewModel(string id, string spanId)
        {
            return new PriTestViewModel
            {
                Id = id,
                SpanId = spanId,
                Start = StartTest,
                Stop = StopTest
            };
        }

        private void StartTest(PriTestViewModel obj)
        {
            if (_tests.TryGetValue(obj.Id, out _))
            {
                return;
            }
            var runner = new TestRunner(_sc, obj, GetAst());
            runner.Start();
            _tests[obj.Id] = runner;
            obj.IsTestStarted = true;
        }

        private void StopTest(PriTestViewModel obj)
        {
            if (!_tests.TryGetValue(obj.Id, out var runner))
            {
                return;
            }
            runner.Stop();
            _tests.Remove(obj.Id);
            obj.IsTestStarted = false;
        }

        private void StartMonitor(PriTestViewModel obj)
        {
            if (_pris.TryGetValue(obj.Id, out var pri))
            {
                pri.Stop();
                _pris.Remove(obj.Id);
            }
            pri = new PriMonitor(_sc, obj, GetAst());
            pri.Start();
            _pris[obj.Id] = pri;
        }
    }
}
using System.IO;
using System.Windows;
using BatreConservation.Services;
using BatreConservation.ViewModels;
using BatreConservation.Views;
using AppBase = System.Windows.Application;

namespace BatreConservation
{
    public partial class App : AppBase
    {
        protected override void OnStartup(StartupEventArgs e)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, $"[{DateTime.Now:O}] Application Starting...{Environment.NewLine}");

            var configService = new ConfigService();
            var batteryService = new BatteryService();
            var thresholdService = new LenovoThresholdService();
            var notificationService = new NotificationService();
            var startupService = new StartupService();
            var viewModel = new DashboardViewModel(batteryService, thresholdService, configService, notificationService, startupService);
            var mainWindow = new MainWindow(viewModel);

            if (e.Args.Contains("--background"))
            {
                mainWindow.Hide();
            }
            else
            {
                mainWindow.Show();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, $"[{DateTime.Now:O}] Application Exiting...{Environment.NewLine}");
            base.OnExit(e);
        }
    }
}
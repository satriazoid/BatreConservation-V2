using System.IO;
using System.Windows;

namespace BatteryGuardian.Services
{
    public class NotificationService
    {
        public void ShowNotification(string title, string message)
        {
            string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
            Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
            File.AppendAllText(logPath, $"[{DateTime.Now:O}] {title}: {message}{Environment.NewLine}");

            try
            {
                System.Windows.MessageBox.Show(message, title, System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            }
            catch
            {
            }
        }
    }
}
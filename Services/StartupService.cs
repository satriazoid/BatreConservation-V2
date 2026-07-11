using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace BatteryGuardian.Services
{
    public class StartupService
    {
        private const string AppName = "BatteryGuardian";

        public void SetStartup(bool enable)
        {
            string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(runKey, true))
            {
                if (key != null)
                {
                    if (enable)
                    {
                        string appPath = Process.GetCurrentProcess().MainModule?.FileName ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BatteryGuardian.exe");
                        key.SetValue(AppName, $"\"{appPath}\" --background");
                    }
                    else
                    {
                        key.DeleteValue(AppName, false);
                    }
                }
            }
        }
    }
}
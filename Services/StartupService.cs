using Microsoft.Win32;
using System.Diagnostics;
using System.IO;

namespace BatreConservation.Services
{
    public class StartupService
    {
        private const string AppName = "BatreConservation";

        public void SetStartup(bool enable)
        {
            string runKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
            using (RegistryKey? key = Registry.CurrentUser.OpenSubKey(runKey, true))
            {
                if (key != null)
                {
                    if (enable)
                    {
                        string appPath = Process.GetCurrentProcess().MainModule?.FileName ?? Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "BatreConservation.exe");
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
using System.IO;

namespace BatteryGuardian.Services
{
    public class LenovoThresholdService
    {
        public bool SetConservationMode(bool enable)
        {
            try
            {
                string logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");
                Directory.CreateDirectory(Path.GetDirectoryName(logPath)!);
                File.AppendAllText(logPath, $"[{DateTime.Now:O}] Lenovo conservation mode {(enable ? "enabled" : "disabled")} (placeholder).{Environment.NewLine}");
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
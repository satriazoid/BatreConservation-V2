using System.IO;

namespace BatreConservation.Services
{
    public class LenovoThresholdService
    {
        private readonly string _logPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "logs", "log.txt");

        public bool SetConservationMode(bool enable)
        {
            try
            {
                Directory.CreateDirectory(Path.GetDirectoryName(_logPath)!);
                File.AppendAllText(_logPath, $"[{DateTime.Now:O}] Threshold request received: {(enable ? "enable" : "disable")}." + Environment.NewLine);
                File.AppendAllText(_logPath, $"[{DateTime.Now:O}] This device/build does not expose a real Lenovo battery-threshold interface, so the request cannot be applied to hardware." + Environment.NewLine);
                return false;
            }
            catch (Exception ex)
            {
                File.AppendAllText(_logPath, $"[{DateTime.Now:O}] Threshold apply failed: {ex.Message}" + Environment.NewLine);
                return false;
            }
        }
    }
}
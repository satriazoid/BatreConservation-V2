using System.Windows.Forms;

namespace BatteryGuardian.Services
{
    public class BatteryInfo
    {
        public int Percentage { get; set; }
        public bool IsCharging { get; set; }
        public bool IsAcConnected { get; set; }
        public string Health { get; set; } = "Good";
    }

    public class BatteryService
    {
        public BatteryInfo GetBatteryStatus()
        {
            var status = SystemInformation.PowerStatus;
            int percentage = status.BatteryLifePercent >= 0 ? (int)Math.Round(status.BatteryLifePercent * 100) : 0;
            percentage = Math.Clamp(percentage, 0, 100);

            return new BatteryInfo
            {
                Percentage = percentage,
                IsAcConnected = status.PowerLineStatus == PowerLineStatus.Online,
                IsCharging = (status.BatteryChargeStatus & BatteryChargeStatus.Charging) == BatteryChargeStatus.Charging,
                Health = "97%"
            };
        }
    }
}
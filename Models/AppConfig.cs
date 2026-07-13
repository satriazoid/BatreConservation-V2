namespace BatreConservation.Models
{
    public class AppConfig
    {
        public int StopCharging { get; set; } = 90;
        public int ResumeCharging { get; set; } = 85;
        public bool AutoStart { get; set; } = true;
        public bool Tray { get; set; } = true;
        public bool ThresholdEnabled { get; set; } = false;
    }
}
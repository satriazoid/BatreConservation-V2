using System.IO;
using System.Text.Json;
using BatteryGuardian.Models;

namespace BatteryGuardian.Services
{
    public class ConfigService
    {
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public AppConfig Config { get; private set; }

        public ConfigService()
        {
            LoadConfig();
        }

        public void LoadConfig()
        {
            if (File.Exists(_filePath))
            {
                try
                {
                    string json = File.ReadAllText(_filePath);
                    Config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                    return;
                }
                catch { }
            }
            Config = new AppConfig();
            SaveConfig();
        }

        public void SaveConfig()
        {
            string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(_filePath, json);
        }
    }
}
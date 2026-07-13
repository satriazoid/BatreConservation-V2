using System.IO;
using System.Text.Json;
using BatreConservation.Models;

namespace BatreConservation.Services
{
    public class ConfigService
    {
        private readonly string _filePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "config.json");
        public AppConfig Config { get; private set; }

        public ConfigService()
        {
            Config = new AppConfig();
            LoadConfig();
        }

        public void LoadConfig()
        {
            string fullPath = Path.GetFullPath(_filePath);
            if (File.Exists(fullPath))
            {
                try
                {
                    string json = File.ReadAllText(fullPath);
                    if (!string.IsNullOrWhiteSpace(json))
                    {
                        Config = JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
                        return;
                    }
                }
                catch { }
            }

            Config = new AppConfig();
            SaveConfig();
        }

        public void SaveConfig()
        {
            string fullPath = Path.GetFullPath(_filePath);
            string directory = Path.GetDirectoryName(fullPath)!;
            Directory.CreateDirectory(directory);
            string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(fullPath, json);
        }
    }
}
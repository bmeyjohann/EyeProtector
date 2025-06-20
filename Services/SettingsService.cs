using EyeProtector.Models;
using Newtonsoft.Json;
using System.IO;

namespace EyeProtector.Services
{
    public class SettingsService
    {
        private readonly string _settingsFilePath;
        private AppSettings _currentSettings;

        public SettingsService()
        {
            var appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var appFolder = Path.Combine(appDataFolder, "EyeProtector");
            
            if (!Directory.Exists(appFolder))
            {
                Directory.CreateDirectory(appFolder);
            }
            
            _settingsFilePath = Path.Combine(appFolder, "settings.json");
            _currentSettings = LoadSettings();
        }

        public AppSettings CurrentSettings => _currentSettings;

        public AppSettings LoadSettings()
        {
            try
            {
                if (File.Exists(_settingsFilePath))
                {
                    var json = File.ReadAllText(_settingsFilePath);
                    var settings = JsonConvert.DeserializeObject<AppSettings>(json);
                    if (settings != null)
                    {
                        // Handle migration from old BlinkIntervalMinutes to BlinkIntervalSeconds
                        if (settings.BlinkIntervalSeconds == 0 && settings.BlinkIntervalMinutes > 0)
                        {
                            settings.BlinkIntervalSeconds = settings.BlinkIntervalMinutes * 60;
                        }
                        // Ensure minimum values
                        if (settings.BlinkIntervalSeconds < 10)
                        {
                            settings.BlinkIntervalSeconds = 300; // Default 5 minutes
                        }
                        return settings;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log error in production - for now, just fall back to defaults
                System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
            }

            return new AppSettings();
        }

        public void SaveSettings(AppSettings settings)
        {
            try
            {
                _currentSettings = settings;
                var json = JsonConvert.SerializeObject(settings, Formatting.Indented);
                File.WriteAllText(_settingsFilePath, json);
            }
            catch (Exception ex)
            {
                // Log error in production
                System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
                throw; // Re-throw to notify the UI
            }
        }

        public void UpdateSettings(Action<AppSettings> updateAction)
        {
            updateAction(_currentSettings);
            SaveSettings(_currentSettings);
        }

        public string GetSettingsFilePath() => _settingsFilePath;
    }
} 
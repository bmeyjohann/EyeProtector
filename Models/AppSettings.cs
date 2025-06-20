using System.Drawing;

namespace EyeProtector.Models
{
    public class AppSettings
    {
        // Blink Reminder Settings
        public bool BlinkReminderEnabled { get; set; } = true;
        public int BlinkIntervalSeconds { get; set; } = 300; // 5 minutes default
        public int BlinkFlashDurationMs { get; set; } = 1000;
        public string BlinkFlashColor { get; set; } = "White";

        // Legacy property for backward compatibility
        public int BlinkIntervalMinutes 
        { 
            get => BlinkIntervalSeconds / 60; 
            set => BlinkIntervalSeconds = value * 60; 
        }

        // Break Reminder Settings
        public bool BreakReminderEnabled { get; set; } = true;
        public int BreakIntervalMinutes { get; set; } = 20;
        public int BreakDurationSeconds { get; set; } = 20;
        public string BreakColor { get; set; } = "Black";

        // Enforcement Settings
        public bool BlockInputDuringBreak { get; set; } = false;
        public bool AllowSnooze { get; set; } = true;
        public int SnoozeDelayMinutes { get; set; } = 5;

        // General Settings
        public bool AutoStartWithWindows { get; set; } = false;
        public bool ShowNotifications { get; set; } = true;
        public bool SoundEnabled { get; set; } = false;
        
        // Multi-monitor settings
        public bool CoverAllMonitors { get; set; } = true;

        public Color GetBlinkColor()
        {
            return BlinkFlashColor.ToLower() switch
            {
                "white" => Color.White,
                "black" => Color.Black,
                "red" => Color.Red,
                "green" => Color.Green,
                "blue" => Color.Blue,
                _ => Color.White
            };
        }

        public Color GetBreakColor()
        {
            return BreakColor.ToLower() switch
            {
                "white" => Color.White,
                "black" => Color.Black,
                "red" => Color.Red,
                "green" => Color.Green,
                "blue" => Color.Blue,
                _ => Color.Black
            };
        }
    }
} 
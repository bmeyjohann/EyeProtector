using EyeBreakEnforcer.Models;
using EyeBreakEnforcer.Windows;
using System.Windows;
using System.Windows.Controls;

namespace EyeBreakEnforcer.Windows
{
    public partial class SettingsWindow : Window
    {
        private AppSettings _originalSettings;
        private AppSettings _currentSettings;

        public event Action<AppSettings>? SettingsChanged;

        public SettingsWindow(AppSettings currentSettings)
        {
            InitializeComponent();
            _originalSettings = currentSettings;
            _currentSettings = CloneSettings(currentSettings);
            
            InitializeEventHandlers();
            LoadSettingsToUI();
        }

        private void InitializeEventHandlers()
        {
            // Slider value change handlers for updating text displays
            BlinkIntervalSlider.ValueChanged += (s, e) => 
                BlinkIntervalText.Text = $"{(int)e.NewValue} sec";
            
            BlinkDurationSlider.ValueChanged += (s, e) => 
                BlinkDurationText.Text = $"{(int)e.NewValue} ms";
            
            BreakIntervalSlider.ValueChanged += (s, e) => 
                BreakIntervalText.Text = $"{(int)e.NewValue} min";
            
            BreakDurationSlider.ValueChanged += (s, e) => 
                BreakDurationText.Text = $"{(int)e.NewValue} sec";
            
            SnoozeDelaySlider.ValueChanged += (s, e) => 
                SnoozeDelayText.Text = $"{(int)e.NewValue} min";
        }

        private void LoadSettingsToUI()
        {
            // Blink settings
            BlinkEnabledCheckBox.IsChecked = _currentSettings.BlinkReminderEnabled;
            BlinkIntervalSlider.Value = _currentSettings.BlinkIntervalSeconds;
            BlinkDurationSlider.Value = _currentSettings.BlinkFlashDurationMs;
            BlinkColorComboBox.SelectedItem = BlinkColorComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _currentSettings.BlinkFlashColor);

            // Break settings
            BreakEnabledCheckBox.IsChecked = _currentSettings.BreakReminderEnabled;
            BreakIntervalSlider.Value = _currentSettings.BreakIntervalMinutes;
            BreakDurationSlider.Value = _currentSettings.BreakDurationSeconds;
            BreakColorComboBox.SelectedItem = BreakColorComboBox.Items
                .Cast<ComboBoxItem>()
                .FirstOrDefault(item => item.Content.ToString() == _currentSettings.BreakColor);

            // Enforcement settings
            AllowSnoozeCheckBox.IsChecked = _currentSettings.AllowSnooze;
            SnoozeDelaySlider.Value = _currentSettings.SnoozeDelayMinutes;
            BlockInputCheckBox.IsChecked = _currentSettings.BlockInputDuringBreak;

            // General settings
            // Check actual Windows startup status and update settings if needed
            var actualAutoStart = EyeBreakEnforcer.Services.StartupManager.IsAutoStartEnabled();
            if (_currentSettings.AutoStartWithWindows != actualAutoStart)
            {
                _currentSettings.AutoStartWithWindows = actualAutoStart;
            }
            
            AutoStartCheckBox.IsChecked = _currentSettings.AutoStartWithWindows;
            CoverAllMonitorsCheckBox.IsChecked = _currentSettings.CoverAllMonitors;
            ShowNotificationsCheckBox.IsChecked = _currentSettings.ShowNotifications;

            // Set default selections if none are set
            if (BlinkColorComboBox.SelectedItem == null)
                BlinkColorComboBox.SelectedIndex = 0; // White
            
            if (BreakColorComboBox.SelectedItem == null)
                BreakColorComboBox.SelectedIndex = 0; // Black
        }

        private void SaveButton_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (ValidateSettings())
                {
                    SaveSettingsFromUI();
                    SettingsChanged?.Invoke(_currentSettings);
                    Close();
                }
            }
            catch (Exception ex)
            {
                System.Windows.MessageBox.Show($"Failed to save settings: {ex.Message}", 
                               "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CancelButton_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void TestBlinkButton_Click(object sender, RoutedEventArgs e)
        {
            // Create a temporary settings object with current UI values
            var testSettings = new AppSettings
            {
                BlinkFlashDurationMs = (int)BlinkDurationSlider.Value,
                BlinkFlashColor = (BlinkColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "White"
            };

            // Create and show test overlay
            var testOverlay = new OverlayWindow();
            testOverlay.ShowBlinkFlash(testSettings);
        }

        private void DurationPreset_Click(object sender, RoutedEventArgs e)
        {
            if (sender is System.Windows.Controls.Button button && button.Tag is string tagValue)
            {
                if (int.TryParse(tagValue, out int duration))
                {
                    BlinkDurationSlider.Value = duration;
                }
            }
        }

        private bool ValidateSettings()
        {
            // Basic validation
            if (BlinkIntervalSlider.Value < 10 || BlinkIntervalSlider.Value > 600)
            {
                System.Windows.MessageBox.Show("Blink interval must be between 10 and 600 seconds.", 
                               "Invalid Setting", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (BreakIntervalSlider.Value < 5 || BreakIntervalSlider.Value > 60)
            {
                System.Windows.MessageBox.Show("Break interval must be between 5 and 60 minutes.", 
                               "Invalid Setting", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (BlinkDurationSlider.Value < 10 || BlinkDurationSlider.Value > 3000)
            {
                System.Windows.MessageBox.Show("Blink duration must be between 10 and 3000 milliseconds.", 
                               "Invalid Setting", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            if (BreakDurationSlider.Value < 10 || BreakDurationSlider.Value > 120)
            {
                System.Windows.MessageBox.Show("Break duration must be between 10 and 120 seconds.", 
                               "Invalid Setting", MessageBoxButton.OK, MessageBoxImage.Warning);
                return false;
            }

            return true;
        }

        private void SaveSettingsFromUI()
        {
            // Blink settings
            _currentSettings.BlinkReminderEnabled = BlinkEnabledCheckBox.IsChecked ?? false;
            _currentSettings.BlinkIntervalSeconds = (int)BlinkIntervalSlider.Value;
            _currentSettings.BlinkFlashDurationMs = (int)BlinkDurationSlider.Value;
            _currentSettings.BlinkFlashColor = (BlinkColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "White";

            // Break settings
            _currentSettings.BreakReminderEnabled = BreakEnabledCheckBox.IsChecked ?? false;
            _currentSettings.BreakIntervalMinutes = (int)BreakIntervalSlider.Value;
            _currentSettings.BreakDurationSeconds = (int)BreakDurationSlider.Value;
            _currentSettings.BreakColor = (BreakColorComboBox.SelectedItem as ComboBoxItem)?.Content.ToString() ?? "Black";

            // Enforcement settings
            _currentSettings.AllowSnooze = AllowSnoozeCheckBox.IsChecked ?? true;
            _currentSettings.SnoozeDelayMinutes = (int)SnoozeDelaySlider.Value;
            _currentSettings.BlockInputDuringBreak = BlockInputCheckBox.IsChecked ?? false;

            // General settings
            var newAutoStart = AutoStartCheckBox.IsChecked ?? false;
            var autoStartChanged = _currentSettings.AutoStartWithWindows != newAutoStart;
            
            _currentSettings.AutoStartWithWindows = newAutoStart;
            _currentSettings.CoverAllMonitors = CoverAllMonitorsCheckBox.IsChecked ?? true;
            _currentSettings.ShowNotifications = ShowNotificationsCheckBox.IsChecked ?? true;

            // Handle Windows startup registration
            if (autoStartChanged)
            {
                try
                {
                    var success = EyeBreakEnforcer.Services.StartupManager.SetAutoStart(newAutoStart);
                    if (!success)
                    {
                        System.Windows.MessageBox.Show("Failed to update Windows startup setting. You may need to run as administrator.", 
                                       "Startup Setting Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                        // Revert the checkbox state
                        AutoStartCheckBox.IsChecked = !newAutoStart;
                        _currentSettings.AutoStartWithWindows = !newAutoStart;
                    }
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Failed to update Windows startup setting: {ex.Message}", 
                                   "Startup Setting Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    // Revert the checkbox state
                    AutoStartCheckBox.IsChecked = !newAutoStart;
                    _currentSettings.AutoStartWithWindows = !newAutoStart;
                }
            }
        }

        private AppSettings CloneSettings(AppSettings original)
        {
            return new AppSettings
            {
                BlinkReminderEnabled = original.BlinkReminderEnabled,
                BlinkIntervalSeconds = original.BlinkIntervalSeconds,
                BlinkFlashDurationMs = original.BlinkFlashDurationMs,
                BlinkFlashColor = original.BlinkFlashColor,
                BreakReminderEnabled = original.BreakReminderEnabled,
                BreakIntervalMinutes = original.BreakIntervalMinutes,
                BreakDurationSeconds = original.BreakDurationSeconds,
                BreakColor = original.BreakColor,
                BlockInputDuringBreak = original.BlockInputDuringBreak,
                AllowSnooze = original.AllowSnooze,
                SnoozeDelayMinutes = original.SnoozeDelayMinutes,
                AutoStartWithWindows = original.AutoStartWithWindows,
                ShowNotifications = original.ShowNotifications,
                SoundEnabled = original.SoundEnabled,
                CoverAllMonitors = original.CoverAllMonitors
            };
        }
    }
} 
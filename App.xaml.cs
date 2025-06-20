using EyeProtector.Models;
using EyeProtector.Services;
using EyeProtector.Windows;
using System.Windows;
using System.Windows.Threading;
using Microsoft.Win32;

namespace EyeProtector
{
    public partial class App : System.Windows.Application
    {
        private SettingsService? _settingsService;
        private TimerService? _timerService;
        private TrayIconManager? _trayIconManager;
        private OverlayWindow? _overlayWindow;
        private SettingsWindow? _settingsWindow;
        private DispatcherTimer? _statusUpdateTimer;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            
            // Ensure single instance
            if (IsAnotherInstanceRunning())
            {
                System.Windows.MessageBox.Show("EyeProtector is already running.", "Already Running", 
                               MessageBoxButton.OK, MessageBoxImage.Information);
                Shutdown();
                return;
            }

            InitializeServices();
            SetupStatusUpdateTimer();
            StartApplication();

            SystemEvents.SessionSwitch += OnSessionSwitch;
            SystemEvents.PowerModeChanged += OnPowerModeChanged;
        }

        private bool IsAnotherInstanceRunning()
        {
            var processes = System.Diagnostics.Process.GetProcessesByName("EyeProtector");
            return processes.Length > 1;
        }

        private void InitializeServices()
        {
            // Initialize settings service
            _settingsService = new SettingsService();
            
            // Initialize timer service with current settings
            _timerService = new TimerService(_settingsService.CurrentSettings);
            _timerService.BlinkReminderTriggered += OnBlinkReminderTriggered;
            _timerService.BreakReminderTriggered += OnBreakReminderTriggered;
            
            // Initialize overlay window
            _overlayWindow = new OverlayWindow();
            
            // Initialize tray icon
            _trayIconManager = new TrayIconManager();
            _trayIconManager.SettingsRequested += OnSettingsRequested;
            _trayIconManager.PauseRequested += OnPauseRequested;
            _trayIconManager.ResumeRequested += OnResumeRequested;
            _trayIconManager.PauseForRequested += OnPauseForRequested;
            _trayIconManager.ExitRequested += OnExitRequested;
            _trayIconManager.Initialize();
        }

        private void SetupStatusUpdateTimer()
        {
            _statusUpdateTimer = new DispatcherTimer();
            _statusUpdateTimer.Interval = TimeSpan.FromSeconds(1);
            _statusUpdateTimer.Tick += UpdateTrayStatus;
            _statusUpdateTimer.Start();
        }

        private void StartApplication()
        {
            _timerService?.Start();
            
            if (_settingsService?.CurrentSettings.ShowNotifications == true)
            {
                _trayIconManager?.ShowBalloonTip("EyeProtector", 
                    "Application started. Eye protection is now active.", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void OnBlinkReminderTriggered()
        {
            Dispatcher.Invoke(() =>
            {
                if (_settingsService?.CurrentSettings != null && _overlayWindow != null)
                {
                    _overlayWindow.ShowBlinkFlash(_settingsService.CurrentSettings);
                }
            });
        }

        private void OnBreakReminderTriggered()
        {
            Dispatcher.Invoke(() =>
            {
                if (_settingsService?.CurrentSettings != null && _overlayWindow != null)
                {
                    _overlayWindow.ShowBreakEnforcement(
                        _settingsService.CurrentSettings,
                        OnBreakCompleted,
                        OnBreakSkipped,
                        OnBreakSnoozed);
                }
            });
        }

        private void OnBreakCompleted()
        {
            _timerService?.ResetBreakTimer();
            
            if (_settingsService?.CurrentSettings.ShowNotifications == true)
            {
                _trayIconManager?.ShowBalloonTip("Break Complete", 
                    "Good job! Your eyes are refreshed.", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void OnBreakSkipped()
        {
            _timerService?.ResetBreakTimer();
            
            if (_settingsService?.CurrentSettings.ShowNotifications == true)
            {
                _trayIconManager?.ShowBalloonTip("Break Skipped", 
                    "Break was skipped. Try not to skip too many breaks!", 
                    System.Windows.Forms.ToolTipIcon.Warning);
            }
        }

        private void OnBreakSnoozed()
        {
            _timerService?.SnoozeBreak();
            
            if (_settingsService?.CurrentSettings.ShowNotifications == true)
            {
                var delay = _settingsService.CurrentSettings.SnoozeDelayMinutes;
                _trayIconManager?.ShowBalloonTip("Break Snoozed", 
                    $"Break reminder snoozed for {delay} minutes.", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void OnSettingsRequested()
        {
            if (_settingsWindow != null)
            {
                _settingsWindow.Activate();
                return;
            }

            _settingsWindow = new SettingsWindow(_settingsService!.CurrentSettings);
            _settingsWindow.SettingsChanged += OnSettingsChanged;
            _settingsWindow.Closed += (s, e) => _settingsWindow = null;
            _settingsWindow.Show();
        }

        private void OnSettingsChanged(AppSettings newSettings)
        {
            _settingsService?.SaveSettings(newSettings);
            _timerService?.UpdateSettings(newSettings);
            
            if (newSettings.ShowNotifications)
            {
                _trayIconManager?.ShowBalloonTip("Settings Updated", 
                    "Your new settings have been saved and applied.", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void OnPauseRequested()
        {
            _timerService?.Pause();
            _trayIconManager?.SetPausedState(true);
            
            if (_settingsService?.CurrentSettings.ShowNotifications == true)
            {
                _trayIconManager?.ShowBalloonTip("Paused", 
                    "Eye protection reminders are paused.", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void OnResumeRequested()
        {
            _timerService?.Resume();
            _trayIconManager?.SetPausedState(false);
            
            if (_settingsService?.CurrentSettings.ShowNotifications == true)
            {
                _trayIconManager?.ShowBalloonTip("Resumed", 
                    "Eye protection reminders are now active.", 
                    System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void OnPauseForRequested(TimeSpan duration)
        {
            _timerService?.PauseFor(duration);
            _trayIconManager?.SetPausedState(true);

            if (_settingsService?.CurrentSettings.ShowNotifications == true)
            {
                var minutes = (int)duration.TotalMinutes;
                string text = $"Eye protection reminders paused for {minutes} minute" + (minutes == 1 ? "" : "s") + ".";
                _trayIconManager?.ShowBalloonTip("Paused", text, System.Windows.Forms.ToolTipIcon.Info);
            }
        }

        private void OnExitRequested()
        {
            var result = System.Windows.MessageBox.Show("Are you sure you want to exit EyeProtector?", 
                                       "Confirm Exit", 
                                       MessageBoxButton.YesNo, 
                                       MessageBoxImage.Question);
            
            if (result == MessageBoxResult.Yes)
            {
                Shutdown();
            }
        }

        private void UpdateTrayStatus(object? sender, EventArgs e)
        {
            if (_timerService != null && _trayIconManager != null)
            {
                var nextBlink = _timerService.GetTimeUntilNextBlink();
                var nextBreak = _timerService.GetTimeUntilNextBreak();
                var resumeIn = _timerService.GetTimeUntilResume();

                _trayIconManager.UpdateStatus(_timerService.IsRunning, nextBlink, nextBreak, resumeIn);
            }
        }

        private void OnSessionSwitch(object? sender, SessionSwitchEventArgs e)
        {
            if (e.Reason == SessionSwitchReason.SessionLock)
            {
                _timerService?.Stop();
            }
            else if (e.Reason == SessionSwitchReason.SessionUnlock)
            {
                _timerService?.Start();
            }
        }

        private void OnPowerModeChanged(object? sender, PowerModeChangedEventArgs e)
        {
            if (e.Mode == PowerModes.Suspend)
            {
                _timerService?.Stop();
            }
            else if (e.Mode == PowerModes.Resume)
            {
                _timerService?.Start();
            }
        }

        protected override void OnExit(ExitEventArgs e)
        {
            // Clean up resources
            // Save current settings before disposing services
            _settingsService?.SaveSettings(_settingsService.CurrentSettings);

            _statusUpdateTimer?.Stop();
            _timerService?.Stop();
            _timerService?.Dispose();
            _trayIconManager?.Dispose();
            _overlayWindow?.ForceClose();
            _settingsWindow?.Close();

            SystemEvents.SessionSwitch -= OnSessionSwitch;
            SystemEvents.PowerModeChanged -= OnPowerModeChanged;

            base.OnExit(e);
        }
    }
} 
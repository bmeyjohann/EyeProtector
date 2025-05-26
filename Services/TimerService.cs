using EyeBreakEnforcer.Models;
using System.Timers;

namespace EyeBreakEnforcer.Services
{
    public class TimerService : IDisposable
    {
        private readonly System.Timers.Timer _blinkTimer;
        private readonly System.Timers.Timer _breakTimer;
        private AppSettings _settings;
        private bool _isPaused;

        public event Action? BlinkReminderTriggered;
        public event Action? BreakReminderTriggered;

        public bool IsRunning { get; private set; }
        public DateTime? NextBlinkTime { get; private set; }
        public DateTime? NextBreakTime { get; private set; }

        public TimerService(AppSettings settings)
        {
            _settings = settings;
            
            _blinkTimer = new System.Timers.Timer();
            _blinkTimer.Elapsed += OnBlinkTimerElapsed;
            _blinkTimer.AutoReset = true;
            
            _breakTimer = new System.Timers.Timer();
            _breakTimer.Elapsed += OnBreakTimerElapsed;
            _breakTimer.AutoReset = true;
            
            UpdateTimerIntervals();
        }

        public void Start()
        {
            if (_isPaused)
            {
                Resume();
                return;
            }

            if (_settings.BlinkReminderEnabled)
            {
                _blinkTimer.Start();
                NextBlinkTime = DateTime.Now.AddMilliseconds(_blinkTimer.Interval);
            }

            if (_settings.BreakReminderEnabled)
            {
                _breakTimer.Start();
                NextBreakTime = DateTime.Now.AddMilliseconds(_breakTimer.Interval);
            }

            IsRunning = true;
        }

        public void Stop()
        {
            _blinkTimer.Stop();
            _breakTimer.Stop();
            IsRunning = false;
            _isPaused = false;
            NextBlinkTime = null;
            NextBreakTime = null;
        }

        public void Pause()
        {
            if (IsRunning)
            {
                _blinkTimer.Stop();
                _breakTimer.Stop();
                _isPaused = true;
            }
        }

        public void Resume()
        {
            if (_isPaused)
            {
                if (_settings.BlinkReminderEnabled)
                {
                    _blinkTimer.Start();
                    NextBlinkTime = DateTime.Now.AddMilliseconds(_blinkTimer.Interval);
                }

                if (_settings.BreakReminderEnabled)
                {
                    _breakTimer.Start();
                    NextBreakTime = DateTime.Now.AddMilliseconds(_breakTimer.Interval);
                }

                _isPaused = false;
            }
        }

        public void SnoozeBreak()
        {
            if (_settings.BreakReminderEnabled)
            {
                _breakTimer.Stop();
                _breakTimer.Interval = _settings.SnoozeDelayMinutes * 60 * 1000; // Convert to milliseconds
                _breakTimer.Start();
                NextBreakTime = DateTime.Now.AddMilliseconds(_breakTimer.Interval);
            }
        }

        public void ResetBreakTimer()
        {
            if (_settings.BreakReminderEnabled)
            {
                _breakTimer.Stop();
                _breakTimer.Interval = _settings.BreakIntervalMinutes * 60 * 1000;
                _breakTimer.Start();
                NextBreakTime = DateTime.Now.AddMilliseconds(_breakTimer.Interval);
            }
        }

        public void ResetBlinkTimer()
        {
            if (_settings.BlinkReminderEnabled)
            {
                _blinkTimer.Stop();
                _blinkTimer.Interval = _settings.BlinkIntervalSeconds * 1000;
                _blinkTimer.Start();
                NextBlinkTime = DateTime.Now.AddMilliseconds(_blinkTimer.Interval);
            }
        }

        public void UpdateTimerIntervals()
        {
            var wasRunning = IsRunning;
            
            if (wasRunning)
            {
                Stop();
            }

            // Update intervals (convert to milliseconds)
            _blinkTimer.Interval = _settings.BlinkIntervalSeconds * 1000;
            _breakTimer.Interval = _settings.BreakIntervalMinutes * 60 * 1000;

            if (wasRunning)
            {
                Start();
            }
        }

        public void UpdateSettings(AppSettings newSettings)
        {
            _settings = newSettings;
            UpdateTimerIntervals();
        }

        private void OnBlinkTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (_settings.BlinkReminderEnabled && !_isPaused)
            {
                BlinkReminderTriggered?.Invoke();
                NextBlinkTime = DateTime.Now.AddMilliseconds(_blinkTimer.Interval);
            }
        }

        private void OnBreakTimerElapsed(object? sender, ElapsedEventArgs e)
        {
            if (_settings.BreakReminderEnabled && !_isPaused)
            {
                BreakReminderTriggered?.Invoke();
                // Don't reset NextBreakTime here - let the break completion handle it
            }
        }

        public TimeSpan GetTimeUntilNextBlink()
        {
            if (NextBlinkTime.HasValue && IsRunning)
            {
                var timeLeft = NextBlinkTime.Value - DateTime.Now;
                return timeLeft > TimeSpan.Zero ? timeLeft : TimeSpan.Zero;
            }
            return TimeSpan.Zero;
        }

        public TimeSpan GetTimeUntilNextBreak()
        {
            if (NextBreakTime.HasValue && IsRunning)
            {
                var timeLeft = NextBreakTime.Value - DateTime.Now;
                return timeLeft > TimeSpan.Zero ? timeLeft : TimeSpan.Zero;
            }
            return TimeSpan.Zero;
        }

        public void Dispose()
        {
            _blinkTimer?.Dispose();
            _breakTimer?.Dispose();
        }
    }
} 
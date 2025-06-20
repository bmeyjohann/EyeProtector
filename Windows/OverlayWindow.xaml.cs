using EyeProtector.Models;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using System.Runtime.InteropServices;
using System.Windows.Interop;

namespace EyeProtector.Windows
{
    public partial class OverlayWindow : Window
    {
        private DispatcherTimer? _countdownTimer;
        private int _remainingSeconds;
        private bool _isBreakMode;
        private AppSettings _settings;
        private Action? _onBreakComplete;
        private Action? _onBreakSkipped;
        private Action? _onBreakSnoozed;

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x00000020;

        [DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        private void EnableClickThrough()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int styles = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, styles | WS_EX_TRANSPARENT);
        }

        private void DisableClickThrough()
        {
            var hwnd = new WindowInteropHelper(this).Handle;
            int styles = GetWindowLong(hwnd, GWL_EXSTYLE);
            SetWindowLong(hwnd, GWL_EXSTYLE, styles & ~WS_EX_TRANSPARENT);
        }

        public OverlayWindow()
        {
            InitializeComponent();
            _settings = new AppSettings();
            SetupWindow();
        }

        private void SetupWindow()
        {
            // Ensure window covers all monitors if configured
            if (_settings.CoverAllMonitors)
            {
                var totalBounds = GetTotalScreenBounds();
                WindowState = WindowState.Normal;
                Left = totalBounds.Left;
                Top = totalBounds.Top;
                Width = totalBounds.Width;
                Height = totalBounds.Height;
            }
            else
            {
                // Default to maximizing on the current monitor
                WindowState = WindowState.Maximized;
            }
        }

        private Rect GetTotalScreenBounds()
        {
            // Use SystemParameters to get the virtual screen size in
            // device-independent units so that DPI scaling is handled
            return new Rect(
                SystemParameters.VirtualScreenLeft,
                SystemParameters.VirtualScreenTop,
                SystemParameters.VirtualScreenWidth,
                SystemParameters.VirtualScreenHeight);
        }

        public void ShowBlinkFlash(AppSettings settings)
        {
            _settings = settings;
            _isBreakMode = false;
            
            // Hide controls for blink mode
            BreakControls.Visibility = Visibility.Collapsed;
            
            // Set background color
            var color = settings.GetBlinkColor();
            Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            
            SetupWindow();
            IsHitTestVisible = false;
            Focusable = false;
            Topmost = true;
            Show();
            EnableClickThrough();
            
            // Auto-hide after specified duration
            var timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromMilliseconds(settings.BlinkFlashDurationMs);
            timer.Tick += (s, e) =>
            {
                timer.Stop();
                Hide();
                DisableClickThrough();
                IsHitTestVisible = true;
                Focusable = true;
            };
            timer.Start();
        }

        public void ShowBreakEnforcement(AppSettings settings, Action onComplete, Action onSkipped, Action onSnoozed)
        {
            _settings = settings;
            _isBreakMode = true;
            _onBreakComplete = onComplete;
            _onBreakSkipped = onSkipped;
            _onBreakSnoozed = onSnoozed;
            _remainingSeconds = settings.BreakDurationSeconds;

            DisableClickThrough();
            IsHitTestVisible = true;
            Focusable = true;
            
            // Set background color
            var color = settings.GetBreakColor();
            Background = new SolidColorBrush(System.Windows.Media.Color.FromRgb(color.R, color.G, color.B));
            
            // Update UI for break mode
            BreakControls.Visibility = Visibility.Visible;
            BreakMessage.Text = $"Time for a {settings.BreakDurationSeconds}-second break!";
            CountdownText.Text = _remainingSeconds.ToString();
            
            // Show/hide controls based on settings
            SkipButton.Visibility = settings.AllowSnooze ? Visibility.Visible : Visibility.Collapsed;
            SnoozeButton.Visibility = settings.AllowSnooze ? Visibility.Visible : Visibility.Collapsed;
            
            // Update snooze button text
            SnoozeButton.Content = $"Snooze ({settings.SnoozeDelayMinutes} min)";
            
            SetupWindow();
            Show();
            Activate();
            Focus();
            
            StartCountdown();
        }

        private void StartCountdown()
        {
            _countdownTimer = new DispatcherTimer();
            _countdownTimer.Interval = TimeSpan.FromSeconds(1);
            _countdownTimer.Tick += CountdownTimer_Tick;
            _countdownTimer.Start();
        }

        private void CountdownTimer_Tick(object? sender, EventArgs e)
        {
            _remainingSeconds--;
            CountdownText.Text = _remainingSeconds.ToString();
            
            if (_remainingSeconds <= 0)
            {
                _countdownTimer?.Stop();
                CompleteBreak();
            }
        }

        private void CompleteBreak()
        {
            Hide();
            _onBreakComplete?.Invoke();
        }

        private void SkipButton_Click(object sender, RoutedEventArgs e)
        {
            _countdownTimer?.Stop();
            Hide();
            _onBreakSkipped?.Invoke();
        }

        private void SnoozeButton_Click(object sender, RoutedEventArgs e)
        {
            _countdownTimer?.Stop();
            Hide();
            _onBreakSnoozed?.Invoke();
        }

        private void Window_KeyDown(object sender, System.Windows.Input.KeyEventArgs e)
        {
            // Allow ESC to close in break mode if controls are shown
            if (_isBreakMode && _settings.AllowSnooze && e.Key == Key.Escape)
            {
                SkipButton_Click(sender, new RoutedEventArgs());
            }
            
            // Block other input during strict enforcement
            if (_isBreakMode && _settings.BlockInputDuringBreak && !_settings.AllowSnooze)
            {
                e.Handled = true;
            }
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            base.OnSourceInitialized(e);
            
            // If input blocking is enabled, we could add low-level hooks here
            // For now, we rely on the Topmost property and focus capture
        }

        public void ForceClose()
        {
            _countdownTimer?.Stop();
            Hide();
        }
    }
} 
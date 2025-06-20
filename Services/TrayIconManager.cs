using System.Drawing;
using System.Windows.Forms;

namespace EyeProtector.Services
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenu;
        private bool _isPaused;

        public event Action? SettingsRequested;
        public event Action? PauseRequested;
        public event Action? ResumeRequested;
        public event Action<TimeSpan>? PauseForRequested;
        public event Action? ExitRequested;

        public void Initialize()
        {
            CreateContextMenu();
            CreateNotifyIcon();
        }

        private void CreateContextMenu()
        {
            _contextMenu = new ContextMenuStrip();

            var settingsItem = new ToolStripMenuItem("Settings", null, OnSettingsClick);
            var pauseResumeItem = new ToolStripMenuItem("Pause", null, OnPauseResumeClick) { Name = "PauseResume" };
            var pause5Item = new ToolStripMenuItem("Pause 5 min", null, (s, e) => OnTimedPauseClick(TimeSpan.FromMinutes(5)));
            var pause1hItem = new ToolStripMenuItem("Pause 1 h", null, (s, e) => OnTimedPauseClick(TimeSpan.FromHours(1)));
            var separatorItem = new ToolStripSeparator();
            var exitItem = new ToolStripMenuItem("Exit", null, OnExitClick);

            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                settingsItem,
                pauseResumeItem,
                pause5Item,
                pause1hItem,
                separatorItem,
                exitItem
            });
        }

        private void CreateNotifyIcon()
        {
            _notifyIcon = new NotifyIcon
            {
                Icon = CreateIcon(),
                ContextMenuStrip = _contextMenu,
                Text = "EyeProtector - Running",
                Visible = true
            };

            _notifyIcon.DoubleClick += OnDoubleClick;
        }

        private Icon CreateIcon()
        {
            // Create a simple icon - in a real app, you'd load from resources
            var bitmap = new Bitmap(16, 16);
            using (var graphics = Graphics.FromImage(bitmap))
            {
                graphics.Clear(Color.Blue);
                graphics.FillEllipse(Brushes.White, 2, 2, 12, 12);
                graphics.FillEllipse(Brushes.Blue, 6, 6, 4, 4);
            }

            return Icon.FromHandle(bitmap.GetHicon());
        }

        private static string FormatTime(TimeSpan time)
        {
            var totalSeconds = (int)time.TotalSeconds;
            if (totalSeconds >= 3600)
            {
                var hours = totalSeconds / 3600;
                var minutes = (totalSeconds % 3600) / 60;
                return $"{hours}h{minutes}m";
            }
            else if (totalSeconds >= 60)
            {
                var minutes = totalSeconds / 60;
                var seconds = totalSeconds % 60;
                return $"{minutes}m{seconds}s";
            }

            return $"{totalSeconds}s";
        }

        public void UpdateStatus(bool isRunning, TimeSpan? nextBlink = null, TimeSpan? nextBreak = null, TimeSpan? resumeIn = null)
        {
            if (_notifyIcon == null) return;

            string status;
            if (!isRunning)
            {
                status = "EyeProtector - Stopped";
            }
            else if (_isPaused)
            {
                var pauseInfo = string.Empty;
                if (resumeIn.HasValue && resumeIn.Value > TimeSpan.Zero)
                {
                    pauseInfo = $" ({FormatTime(resumeIn.Value)})";
                }
                status = $"EyeProtector - Paused{pauseInfo}";

                var pauseResumeItem = _contextMenu?.Items["PauseResume"] as ToolStripMenuItem;
                if (pauseResumeItem != null)
                {
                    pauseResumeItem.Text = resumeIn.HasValue && resumeIn.Value > TimeSpan.Zero
                        ? $"Resume ({FormatTime(resumeIn.Value)})"
                        : "Resume";
                }
            }
            else
            {
                var pauseResumeItem = _contextMenu?.Items["PauseResume"] as ToolStripMenuItem;
                if (pauseResumeItem != null)
                {
                    pauseResumeItem.Text = "Pause";
                }

                var nextEvent = "";
                if (nextBlink.HasValue)
                {
                    var totalSeconds = (int)nextBlink.Value.TotalSeconds;
                    if (totalSeconds < 60)
                    {
                        nextEvent = $" (Blink: {totalSeconds}s)";
                    }
                    else
                    {
                        var minutes = totalSeconds / 60;
                        var seconds = totalSeconds % 60;
                        nextEvent = $" (Blink: {minutes}m{seconds}s)";
                    }
                }
                status = $"EyeProtector - Running{nextEvent}";
            }

            _notifyIcon.Text = status.Length > 63 ? status.Substring(0, 63) : status;
        }

        public void ShowBalloonTip(string title, string text, ToolTipIcon icon = ToolTipIcon.Info)
        {
            _notifyIcon?.ShowBalloonTip(3000, title, text, icon);
        }

        public void SetPausedState(bool isPaused)
        {
            _isPaused = isPaused;
            
            var pauseResumeItem = _contextMenu?.Items["PauseResume"] as ToolStripMenuItem;
            if (pauseResumeItem != null)
            {
                pauseResumeItem.Text = isPaused ? "Resume" : "Pause";
            }
        }

        private void OnSettingsClick(object? sender, EventArgs e)
        {
            SettingsRequested?.Invoke();
        }

        private void OnPauseResumeClick(object? sender, EventArgs e)
        {
            if (_isPaused)
            {
                ResumeRequested?.Invoke();
            }
            else
            {
                PauseRequested?.Invoke();
            }
        }

        private void OnTimedPauseClick(TimeSpan duration)
        {
            PauseForRequested?.Invoke(duration);
        }

        private void OnExitClick(object? sender, EventArgs e)
        {
            ExitRequested?.Invoke();
        }

        private void OnDoubleClick(object? sender, EventArgs e)
        {
            SettingsRequested?.Invoke();
        }

        public void Dispose()
        {
            _notifyIcon?.Dispose();
            _contextMenu?.Dispose();
        }
    }
} 
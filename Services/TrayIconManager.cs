using System.Drawing;
using System.Windows.Forms;

namespace EyeBreakEnforcer.Services
{
    public class TrayIconManager : IDisposable
    {
        private NotifyIcon? _notifyIcon;
        private ContextMenuStrip? _contextMenu;
        private bool _isPaused;

        public event Action? SettingsRequested;
        public event Action? PauseRequested;
        public event Action? ResumeRequested;
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
            var separatorItem = new ToolStripSeparator();
            var exitItem = new ToolStripMenuItem("Exit", null, OnExitClick);

            _contextMenu.Items.AddRange(new ToolStripItem[]
            {
                settingsItem,
                pauseResumeItem,
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
                Text = "EyeBreakEnforcer - Running",
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

        public void UpdateStatus(bool isRunning, TimeSpan? nextBlink = null, TimeSpan? nextBreak = null)
        {
            if (_notifyIcon == null) return;

            string status;
            if (!isRunning)
            {
                status = "EyeBreakEnforcer - Stopped";
            }
            else if (_isPaused)
            {
                status = "EyeBreakEnforcer - Paused";
            }
            else
            {
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
                status = $"EyeBreakEnforcer - Running{nextEvent}";
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
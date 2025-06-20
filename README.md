# EyeBreakEnforcer

A Windows application that enforces the **20-20-20 rule** for eye health: every 20 minutes, look at something 20 feet away for at least 20 seconds.

## Features

### Core Functionality
- **Blink Reminders**: Quick screen flashes every few minutes to remind you to blink
- **Break Enforcement**: Full-screen overlays that force you to take 20-second breaks
- **Customizable Intervals**: Configure timing for both blink and break reminders
- **Multi-Monitor Support**: Covers all monitors when active

### User Experience
- **System Tray Operation**: Runs quietly in the background
- **Flexible Controls**: Pause, snooze, or skip breaks when needed
- **Visual Customization**: Choose different colors for blink flashes and break screens
- **Notifications**: Optional balloon tip notifications for status updates

### Advanced Features
- **Auto-Start**: Option to start automatically with Windows
- **Settings Persistence**: All preferences saved between sessions
- **Single Instance**: Prevents multiple copies from running
- **Graceful Shutdown**: Proper cleanup and confirmation on exit

## Installation

### Prerequisites
- Windows 10 or Windows 11
- .NET 9 Runtime

### Building from Source

1. **Clone the repository**:
   ```bash
   git clone <repository-url>
   cd EyeBreakEnforcer
   ```

2. **Build the application**:
   ```bash
   dotnet build --configuration Release
   ```

3. **Run the application**:
   ```bash
   dotnet run
   ```

### Creating a Standalone Executable

To create a self-contained executable:

```bash
dotnet publish -c Release -r win-x64 --self-contained
```

The executable will be in `bin/Release/net9.0-windows/win-x64/publish/`

## Usage

### First Run
1. Launch `EyeBreakEnforcer.exe`
2. The application will start in the system tray (look for the eye icon)
3. Right-click the tray icon to access settings

### Configuration
Right-click the tray icon and select "Settings" to configure:

#### Blink Reminder Settings
- **Enable/Disable**: Toggle blink reminders on/off
- **Interval**: Set time between blink reminders (1-30 minutes)
- **Duration**: How long the flash appears (200-3000 ms)
- **Color**: Choose flash color (White, Black, Red, Green, Blue)

#### Break Enforcement Settings
- **Enable/Disable**: Toggle break enforcement on/off
- **Interval**: Time between break reminders (5-60 minutes)
- **Duration**: Length of enforced break (10-120 seconds)
- **Color**: Background color during breaks

#### Enforcement Options
- **Allow Snooze**: Enable skip/snooze buttons during breaks
- **Snooze Delay**: How long to delay after snoozing (1-15 minutes)
- **Block Input**: (Advanced) Block keyboard/mouse during breaks

#### General Settings
- **Start with Windows**: Auto-start when Windows boots
- **Cover All Monitors**: Extend overlays across all screens
- **Show Notifications**: Enable balloon tip notifications

### Daily Use
- **Normal Operation**: The app works silently in the background
- **During Blinks**: Quick screen flash - just continue working
- **During Breaks**: Full screen overlay with countdown
  - Look at something 20 feet away
  - Use Skip button if absolutely necessary
  - Use Snooze to delay by configured amount
- **Pausing**: Right-click tray icon → Pause (useful for presentations)

## Default Settings

The application starts with these recommended defaults:
- **Blink Reminder**: Every 5 minutes, 1-second white flash
- **Break Enforcement**: Every 20 minutes, 20-second black screen
- **Snooze Enabled**: 5-minute snooze delay
- **Notifications**: Enabled
- **Multi-Monitor**: Enabled

## Technical Details

### Architecture
- **WPF Application**: Modern Windows UI framework
- **System Tray Integration**: Uses Windows Forms NotifyIcon
- **JSON Settings**: Configuration stored in AppData
- **Timer-Based**: Precise interval management
- **Multi-Monitor**: Automatic screen detection and coverage

### File Locations
- **Settings**: `%APPDATA%\EyeBreakEnforcer\settings.json`
- **Application**: Wherever you place the executable

### Performance
- **Low CPU Usage**: Timer-based with minimal background processing
- **Low Memory**: ~10-20MB typical usage
- **Non-Intrusive**: Only activates during reminders

## Development

### Project Structure
```
EyeBreakEnforcer/
├── Models/
│   └── AppSettings.cs          # Configuration data model
├── Services/
│   ├── SettingsService.cs      # Settings persistence
│   ├── TimerService.cs         # Timer management
│   └── TrayIconManager.cs      # System tray integration
├── Windows/
│   ├── OverlayWindow.xaml      # Full-screen overlay UI
│   ├── OverlayWindow.xaml.cs   # Overlay functionality
│   ├── SettingsWindow.xaml     # Settings UI
│   └── SettingsWindow.xaml.cs  # Settings functionality
├── App.xaml                    # Application definition
├── App.xaml.cs                 # Main application logic
└── EyeBreakEnforcer.csproj     # Project file
```

### Key Components
- **App.xaml.cs**: Main application coordinator
- **TimerService**: Manages blink and break intervals
- **OverlayWindow**: Handles both blink flashes and break enforcement
- **SettingsService**: JSON-based configuration persistence
- **TrayIconManager**: System tray integration and context menu

### Dependencies
- **Newtonsoft.Json**: Settings serialization
- **System.Drawing.Common**: Icon creation
- **System.Windows.Forms**: System tray integration

## Troubleshooting

### Common Issues

**Application doesn't start**
- Ensure .NET 9 Runtime is installed
- Check Windows compatibility (Win10/11 required)

**Settings not saved**
- Verify write permissions to `%APPDATA%` folder
- Check disk space availability

**Overlays don't appear**
- Verify application isn't paused
- Check if other applications are blocking overlays
- Try disabling and re-enabling features

**Multiple instances**
- Only one instance can run at a time
- Use Task Manager to end existing processes if stuck

### Debug Mode
For development, run with:
```bash
dotnet run --configuration Debug
```

This provides additional console output for troubleshooting.

## Additional Documentation

- [Setup Guide](docs/SETUP.md)
- [Distribution Guide](docs/DISTRIBUTION.md)
- [Product Specs](docs/specs.md)

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly on Windows 10/11
5. Submit a pull request

## License

This project is licensed under the [MIT License](LICENSE).

## Health Disclaimer

This application is designed to help remind users to take eye breaks but is not a medical device. Consult with eye care professionals for persistent eye strain or vision problems. 
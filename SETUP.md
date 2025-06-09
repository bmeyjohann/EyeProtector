# EyeBreakEnforcer - Setup and Build Guide

## Prerequisites

- **Windows 10/11** (64-bit)
- **.NET 9 SDK** or later
  - Download from: https://dotnet.microsoft.com/download/dotnet/9.0
  - Choose "SDK" version for building the application

## Quick Start

### Option 1: Use Pre-built Standalone Executable ⭐ **RECOMMENDED**

The simplest way to distribute and run the application:

1. Navigate to `bin/Release/net9.0-windows/win-x64/publish/`
2. Copy `EyeBreakEnforcer.exe` (163 MB)
3. **That's it!** This single file contains everything needed to run the app
4. No .NET installation required on target machines
5. Just double-click to run or distribute to others

### Option 2: Build from Source

```bash
# Clone or download the source code
cd EyeBreakEnforcer

# Build standalone executable (recommended for distribution)
dotnet publish -c Release

# OR build framework-dependent version (smaller, requires .NET 9 Runtime)
dotnet build -c Release
```

## Distribution Options

### 🎯 **Standalone Distribution** (Best for sharing with others)
- **File**: `bin/Release/net9.0-windows/win-x64/publish/EyeBreakEnforcer.exe`
- **Size**: ~163 MB (single file)
- **Requirements**: None (includes .NET runtime)
- **Distribution**: Just share this one `.exe` file

### 📦 **Framework-Dependent Distribution** (Smaller size)
- **Files**: Everything in `bin/Release/net9.0-windows/`
- **Size**: ~3 MB (multiple files)
- **Requirements**: Target machine needs .NET 9 Desktop Runtime
- **Distribution**: Share the entire folder contents

## Building Different Versions

```bash
# Standalone executable (default with updated project file)
dotnet publish -c Release

# Framework-dependent build
dotnet build -c Release

# Create installer-ready version with optimizations
dotnet publish -c Release --verbosity minimal

# Debug version for development
dotnet build -c Debug
```

## Installation

### For End Users (Standalone)
1. Download `EyeBreakEnforcer.exe`
2. Place it anywhere you want (e.g., `C:\Program Files\EyeBreakEnforcer\`)
3. Create a desktop shortcut (optional)
4. Run the application - it will add itself to the system tray

### For Developers
1. Install .NET 9 SDK
2. Clone this repository
3. Run `dotnet restore` to install dependencies
4. Run `dotnet run` for development or `dotnet publish -c Release` for distribution

## Project Structure

```
EyeBreakEnforcer/
├── Models/           # Configuration data models
├── Services/         # Core application services
├── Windows/          # WPF windows (Settings, Overlay)
├── App.xaml/.cs      # Main application entry point
├── *.csproj          # Project configuration
└── bin/Release/      # Build outputs
    └── net9.0-windows/
        ├── [framework-dependent files]
        └── win-x64/publish/
            └── EyeBreakEnforcer.exe  # ← Standalone version
```

## Features Included

✅ **20-20-20 Rule Enforcement**
- Configurable blink reminders (default: every 5 minutes)
- Configurable break enforcement (default: every 20 minutes for 20 seconds)

✅ **System Integration**
- System tray operation
- Windows startup integration
- Multi-monitor support
- Settings persistence

✅ **User Experience**
- Intuitive settings interface
- Pause/resume functionality
- Snooze options
- Visual and behavioral customization

## Troubleshooting

### Common Issues

**"Application won't start"**
- Ensure you're using the 64-bit Windows version
- For framework-dependent: Install .NET 9 Desktop Runtime

**"Settings not saving"**
- Check write permissions to `%APPDATA%\EyeBreakEnforcer\`
- Run as administrator if needed

**"Overlay not showing on all monitors"**
- This is expected behavior - overlays cover all connected displays

**"Auto-start not working"**
- Check Windows Startup settings in Task Manager > Startup tab
- Verify registry permissions (HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run)

### Build Issues

**"SDK not found"**
```bash
# Verify .NET installation
dotnet --version

# Should show 9.0.x or later
```

**"Package restore failed"**
```bash
# Clear NuGet cache and restore
dotnet nuget locals all --clear
dotnet restore
```

## Advanced Configuration

### Custom Build Options

```bash
# Build for specific Windows version
dotnet publish -c Release -r win-x64 --os win

# Create optimized release build
dotnet publish -c Release -p:PublishReadyToRun=true

# Debug symbols in release build
dotnet publish -c Release -p:DebugType=embedded
```

### Registry Settings (Advanced Users)

The application stores startup configuration in:
`HKEY_CURRENT_USER\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`

Settings are stored in JSON format at:
`%APPDATA%\EyeBreakEnforcer\settings.json`

## Security Notes

- The application requires no administrator privileges for normal operation
- Auto-start functionality modifies user registry (HKCU only, not HKLM)
- No network connectivity required
- All data stored locally in user's AppData folder

## Support

For issues, feature requests, or contributions:
1. Check this documentation first
2. Verify you have the latest version
3. Check Windows Event Viewer for any error details
4. Review the settings.json file for configuration issues 
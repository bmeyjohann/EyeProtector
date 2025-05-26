# EyeBreakEnforcer Setup Guide

## Prerequisites Installation

### Step 1: Install .NET 9 SDK

1. **Download .NET 9 SDK**:
   - Go to: https://dotnet.microsoft.com/download/dotnet/9.0
   - Click "Download .NET 9.0 SDK" for Windows
   - Choose the appropriate version (x64 for most modern Windows systems)

2. **Install the SDK**:
   - Run the downloaded installer
   - Follow the installation wizard
   - Restart your command prompt/PowerShell after installation

3. **Verify Installation**:
   ```bash
   dotnet --version
   ```
   Should display something like `9.0.xxx`

### Step 2: Verify System Requirements

- **Operating System**: Windows 10 (version 1607+) or Windows 11
- **Architecture**: x64, x86, or ARM64
- **Memory**: At least 512 MB RAM
- **Disk Space**: At least 200 MB available

## Building the Application

### Option 1: Command Line Build

1. **Open PowerShell or Command Prompt**
2. **Navigate to the project directory**:
   ```bash
   cd C:\Data\EyeProtector
   ```

3. **Restore dependencies**:
   ```bash
   dotnet restore
   ```

4. **Build the application**:
   ```bash
   dotnet build --configuration Release
   ```

5. **Run the application**:
   ```bash
   dotnet run --configuration Release
   ```

### Option 2: Create Standalone Executable

For a self-contained executable that doesn't require .NET runtime on target machines:

```bash
dotnet publish -c Release -r win-x64 --self-contained true -p:PublishSingleFile=true
```

The executable will be located at:
`bin\Release\net9.0-windows\win-x64\publish\EyeBreakEnforcer.exe`

### Option 3: Framework-Dependent Deployment

For smaller file size (requires .NET 9 runtime on target machine):

```bash
dotnet publish -c Release -r win-x64 --self-contained false
```

## Development Environment Setup

### Visual Studio 2022 (Recommended)

1. **Download Visual Studio 2022**:
   - Community Edition (free): https://visualstudio.microsoft.com/vs/community/
   - Include ".NET desktop development" workload during installation

2. **Open the project**:
   - File → Open → Project/Solution
   - Select `EyeBreakEnforcer.csproj`

3. **Build and run**:
   - Press F5 to build and run
   - Or Build → Build Solution (Ctrl+Shift+B)

### Visual Studio Code

1. **Install VS Code**: https://code.visualstudio.com/
2. **Install C# extension**: 
   - Open Extensions (Ctrl+Shift+X)
   - Search for "C#" by Microsoft
   - Install the extension

3. **Open project folder**:
   - File → Open Folder
   - Select the EyeProtector directory

4. **Build and run**:
   - Terminal → New Terminal
   - Run: `dotnet build` and `dotnet run`

## Common Build Issues

### Issue: "dotnet" not recognized

**Solution**: 
- Restart your terminal/command prompt after installing .NET 9 SDK
- If still not working, add .NET to your PATH:
  1. Open System Properties → Advanced → Environment Variables
  2. Add to PATH: `C:\Program Files\dotnet\`

### Issue: Missing Windows Forms reference

**Solution**: The project file should automatically handle this, but if you get errors:
```bash
dotnet add package Microsoft.WindowsDesktop.App
```

### Issue: Missing dependencies

**Solution**: Restore NuGet packages:
```bash
dotnet restore
```

### Issue: WPF/Windows Forms compatibility

**Solution**: Ensure you're targeting `net9.0-windows`:
- Check that `<TargetFramework>net9.0-windows</TargetFramework>` is in the .csproj file
- Check that `<UseWPF>true</UseWPF>` is present

## Testing the Build

1. **Quick test**:
   ```bash
   dotnet run
   ```
   - Application should start and appear in system tray
   - Right-click tray icon to test context menu

2. **Test features**:
   - Open Settings (right-click tray → Settings)
   - Set blink interval to 1 minute for quick testing
   - Verify overlay appears after 1 minute

3. **Test break functionality**:
   - Set break interval to 2 minutes
   - Wait for break overlay to appear
   - Test Skip and Snooze buttons

## Distribution

### Creating an Installer

For professional distribution, consider creating an MSI installer:

1. **Install WiX Toolset**: https://wixtoolset.org/
2. **Create installer project** using WiX templates
3. **Include .NET 9 Runtime** as prerequisite

### Simple Distribution

For personal use:
1. Create self-contained executable (see Option 2 above)
2. Copy the executable to desired location
3. Create desktop shortcut if needed
4. Add to Windows startup folder for auto-start:
   - Copy shortcut to: `%APPDATA%\Microsoft\Windows\Start Menu\Programs\Startup`

## Troubleshooting

### Build Errors

**Error: SDK not found**
- Reinstall .NET 9 SDK
- Verify installation with `dotnet --version`

**Error: Project doesn't load**
- Check .csproj file format
- Ensure all files are in correct directories

**Error: Reference issues**
- Run `dotnet restore` to download packages
- Check internet connection

### Runtime Errors

**Application doesn't start**
- Check Windows Event Viewer for detailed errors
- Run from command line to see error messages
- Verify .NET 9 runtime is installed

**System tray icon doesn't appear**
- Check Windows notification area settings
- Run as administrator if needed
- Verify Windows Forms is working

### Performance Issues

**High CPU usage**
- Check timer intervals in settings
- Look for infinite loops in logs
- Monitor with Task Manager

**Memory leaks**
- Ensure proper disposal of resources
- Check for event handler leaks
- Monitor memory usage over time

## Next Steps

After successful build:
1. Review the main README.md for usage instructions
2. Test all features thoroughly
3. Configure settings for your preferences
4. Set up auto-start if desired
5. Create desktop shortcuts for easy access 
# EyeBreakEnforcer - Distribution Guide

## Two Distribution Options

### 🎯 **Option 1: Standalone Executable** (RECOMMENDED)

**Best for**: Sharing with friends, colleagues, or anyone without technical expertise

✅ **What you get:**
- Single file: `EyeBreakEnforcer.exe` (163 MB)
- Location: `bin/Release/net9.0-windows/win-x64/publish/`

✅ **Advantages:**
- **No dependencies** - Works on any Windows 10/11 machine
- **Easy distribution** - Just share one file
- **No installation** - Double-click to run

✅ **How to distribute:**
1. Copy `EyeBreakEnforcer.exe` from the publish folder
2. Share via email, USB drive, cloud storage, etc.
3. Recipients just double-click to run
4. That's it!

---

### 📦 **Option 2: Framework-Dependent** 

**Best for**: Environments where .NET 9 is already installed

✅ **What you get:**
- Multiple files in `bin/Release/net9.0-windows/`
- Total size: ~3 MB

✅ **Advantages:**
- **Much smaller** file size
- **Faster startup** (runtime already optimized)

⚠️ **Requirements:**
- Target machine must have .NET 9 Desktop Runtime installed
- Download from: https://dotnet.microsoft.com/download/dotnet/9.0

✅ **How to distribute:**
1. Zip the entire `bin/Release/net9.0-windows/` folder
2. Share the zip file
3. Recipients extract and run `EyeBreakEnforcer.exe`
4. If it doesn't work, they need to install .NET 9 Desktop Runtime

---

## Quick Distribution Commands

### For Standalone (Recommended)
```bash
# Build standalone version
dotnet publish -c Release

# The result is in: bin/Release/net9.0-windows/win-x64/publish/EyeBreakEnforcer.exe
# Just share this single file!
```

### For Framework-Dependent
```bash
# Build framework-dependent version
dotnet build -c Release

# Share everything in: bin/Release/net9.0-windows/
```

---

## What Users Need to Know

### For Standalone Version
**Tell recipients:**
> "Just download EyeBreakEnforcer.exe and double-click it. No installation needed!"

### For Framework-Dependent Version
**Tell recipients:**
> "Download and extract the zip file, then run EyeBreakEnforcer.exe. If you get an error, install .NET 9 Desktop Runtime from Microsoft first."

---

## Professional Distribution

### Creating an Installer (Advanced)

For professional deployment, consider:

1. **MSI Installer** using WiX Toolset
2. **ClickOnce deployment** for automatic updates
3. **Microsoft Store** distribution
4. **Chocolatey package** for developers

### Code Signing (Recommended)

To avoid "Unknown Publisher" warnings:
1. Get a code signing certificate
2. Sign the executable using `signtool.exe`
3. This prevents Windows Defender false positives

---

## Troubleshooting Distribution

### "Windows protected your PC" warning
- This appears for unsigned executables
- Click "More info" → "Run anyway"
- Or get the executable code signed

### "This app can't run on your PC" 
- Usually means wrong architecture (x86 vs x64)
- Rebuild for correct target platform

### Missing DLL errors (Framework-dependent only)
- Install .NET 9 Desktop Runtime
- Ensure all files from the build folder are included

---

## File Size Comparison

| Version | Size | Dependencies | Best For |
|---------|------|--------------|----------|
| Standalone | 163 MB | None | General distribution |
| Framework-dependent | 3 MB | .NET 9 Runtime | Development teams |

**Recommendation**: Use standalone for maximum compatibility and ease of distribution. 
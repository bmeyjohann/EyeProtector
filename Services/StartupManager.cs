using Microsoft.Win32;
using System.IO;
using System.Reflection;

namespace EyeBreakEnforcer.Services
{
    public static class StartupManager
    {
        private const string AppName = "EyeBreakEnforcer";
        private const string RegistryKey = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

        /// <summary>
        /// Enables or disables auto-start with Windows
        /// </summary>
        /// <param name="enable">True to enable auto-start, false to disable</param>
        /// <returns>True if operation was successful</returns>
        public static bool SetAutoStart(bool enable)
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, true);
                if (key == null) return false;

                if (enable)
                {
                    var executablePath = GetExecutablePath();
                    if (!string.IsNullOrEmpty(executablePath))
                    {
                        key.SetValue(AppName, $"\"{executablePath}\"");
                        return true;
                    }
                }
                else
                {
                    key.DeleteValue(AppName, false);
                    return true;
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error setting auto-start: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Checks if auto-start is currently enabled
        /// </summary>
        /// <returns>True if auto-start is enabled</returns>
        public static bool IsAutoStartEnabled()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, false);
                if (key == null) return false;

                var value = key.GetValue(AppName);
                return value != null && !string.IsNullOrEmpty(value.ToString());
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error checking auto-start status: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets the current executable path
        /// </summary>
        /// <returns>Path to the current executable</returns>
        private static string GetExecutablePath()
        {
            try
            {
                // For single-file apps, use ProcessPath
                var processPath = Environment.ProcessPath;
                if (!string.IsNullOrEmpty(processPath))
                {
                    return Path.GetFullPath(processPath);
                }

                // Fallback to assembly location for non-single-file apps
                var assembly = Assembly.GetExecutingAssembly();
                var location = assembly.Location;
                if (!string.IsNullOrEmpty(location) && !location.EndsWith(".dll"))
                {
                    return Path.GetFullPath(location);
                }

                // Last resort: use base directory
                return Path.GetFullPath(AppContext.BaseDirectory);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error getting executable path: {ex.Message}");
                return "";
            }
        }

        /// <summary>
        /// Alternative method using startup folder (less reliable but doesn't require registry)
        /// </summary>
        /// <param name="enable">True to enable, false to disable</param>
        /// <returns>True if successful</returns>
        public static bool SetAutoStartViaStartupFolder(bool enable)
        {
            try
            {
                var startupFolder = Environment.GetFolderPath(Environment.SpecialFolder.Startup);
                var shortcutPath = Path.Combine(startupFolder, $"{AppName}.lnk");

                if (enable)
                {
                    var executablePath = GetExecutablePath();
                    if (!string.IsNullOrEmpty(executablePath))
                    {
                        return CreateShortcut(shortcutPath, executablePath);
                    }
                }
                else
                {
                    if (File.Exists(shortcutPath))
                    {
                        File.Delete(shortcutPath);
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error with startup folder method: {ex.Message}");
            }

            return false;
        }

        /// <summary>
        /// Creates a Windows shortcut file
        /// </summary>
        /// <param name="shortcutPath">Path where to create the shortcut</param>
        /// <param name="targetPath">Path to the target executable</param>
        /// <returns>True if successful</returns>
        private static bool CreateShortcut(string shortcutPath, string targetPath)
        {
            try
            {
                // This requires COM interop which is complex
                // For now, we'll use the registry method primarily
                // In a full implementation, you'd use IWshShortcut COM interface
                
                System.Diagnostics.Debug.WriteLine($"Shortcut creation not implemented. Use registry method instead.");
                return false;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error creating shortcut: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Gets registry-based auto-start status with error handling
        /// </summary>
        /// <returns>Current auto-start configuration</returns>
        public static (bool Enabled, string? Path, string? Error) GetAutoStartStatus()
        {
            try
            {
                using var key = Registry.CurrentUser.OpenSubKey(RegistryKey, false);
                if (key == null) 
                    return (false, null, "Registry key not accessible");

                var value = key.GetValue(AppName);
                if (value == null)
                    return (false, null, null);

                var path = value.ToString();
                var enabled = !string.IsNullOrEmpty(path);
                
                return (enabled, path, null);
            }
            catch (Exception ex)
            {
                return (false, null, ex.Message);
            }
        }

        /// <summary>
        /// Validates that the registered startup path still exists and is current
        /// </summary>
        /// <returns>True if the startup registration is valid</returns>
        public static bool ValidateAutoStartRegistration()
        {
            var (enabled, registeredPath, error) = GetAutoStartStatus();
            
            if (!enabled || !string.IsNullOrEmpty(error))
                return false;

            if (string.IsNullOrEmpty(registeredPath))
                return false;

            // Clean the path (remove quotes)
            var cleanPath = registeredPath.Trim('"');
            var currentPath = GetExecutablePath();

            return File.Exists(cleanPath) && 
                   string.Equals(cleanPath, currentPath, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Updates the auto-start registration if the executable path has changed
        /// </summary>
        /// <returns>True if update was successful or not needed</returns>
        public static bool UpdateAutoStartIfNeeded()
        {
            if (!IsAutoStartEnabled())
                return true; // Nothing to update

            if (ValidateAutoStartRegistration())
                return true; // Already valid

            // Re-register with current path
            return SetAutoStart(true);
        }
    }
} 
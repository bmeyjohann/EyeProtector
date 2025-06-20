Product Specification for the App
Here’s a structured product spec you can pass to a developer to create this:

1️⃣ App Overview
Name: “EyeBreakEnforcer” (or pick your own)

Platform: Windows 10/11

Language: C# using .NET 9

UI Framework: WPF (preferred over WinForms for better styling and layout)

Purpose: To enforce the 20-20-20 rule by forcing screen overlays for blinking and breaks.

2️⃣ Core Features
🔹 Blink Reminder
Trigger every 20 seconds (configurable).

Flash the entire screen white or black for 0.5–1 second.

Overlay covers the whole screen (like a fullscreen window) but doesn’t block input (to keep it minimally disruptive).

🔹 Enforced 20-minute Break
Trigger every 20 minutes.

Full-screen black overlay that covers everything.

Duration: 20–60 seconds (configurable).

During this time:

Overlay is “Topmost” so no other app can go over it.

Optionally block input (keyboard/mouse) to enforce the break fully.

Provide an exit button or a “snooze” option for flexibility (configurable if you want to make it fully enforced).

3️⃣ Settings and Configurability
✅ Blink Reminder:

On/off toggle

Interval in minutes (default 5 min)

Flash duration in milliseconds (default 1000 ms)

Color choice (white, black, custom color)

✅ Break Reminder:

On/off toggle

Interval in minutes (default 20 min)

Break duration in seconds (default 20 sec)

Color choice (default black)

✅ Enforcement Strictness:

Option to fully block input during breaks (requires low-level hooks; optional and advanced).

Option to snooze (skip the next break reminder) — only if you want a flexible mode.

✅ Tray Icon / Minimal UI:

App runs in the system tray.

Right-click on icon to open settings.

No other windows or distractions.

✅ Startup Behavior:

Option to auto-start with Windows (registry or startup folder).

Should remember settings across reboots (store in config file or registry).

4️⃣ Technical Implementation Details
✅ Fullscreen Overlay

Create a borderless, always-on-top WPF Window (WindowStyle=None, Topmost=True, WindowState=Maximized, transparent background).

To cover the screen: fill with white/black rectangle.

For blinking, show the window briefly and then close it.

For breaks, show the window for the full duration.

✅ Timers

Use a System.Timers.Timer to track the blink and break intervals precisely.

✅ Input Blocking (optional)

For breaks, if you want to block keyboard/mouse:

Use Windows API hooks (SetWindowsHookEx) to capture and discard input events.

This requires admin privileges and should be done carefully to avoid system freezes.

✅ Settings Storage

Use JSON or XML in the user’s AppData folder for settings persistence.

✅ Signing and Deployment

Sign the executable to avoid false-positive virus flags.

Provide as a standard MSI installer for user-friendly installation.

5️⃣ User Experience
App launches silently and sits in the tray.

Tray icon shows current status: “Idle / Waiting” or “Break in progress.”

User can open settings to customize intervals and strictness.

Blinking reminder is quick and less disruptive.

Break enforcement is more intrusive, by design, to ensure you rest.

App should never crash or hang the system — stability is key.

6️⃣ Potential Extensions (Optional)
Stats page: show how many breaks/blinks were enforced today.

Customizable sounds (e.g., chime during break start).

⚠️ Important Considerations
✅ This is not standard and requires careful handling to avoid locking the user out of their PC entirely.
✅ Testing on multiple monitor setups — overlays should cover all screens, not just the main display.
✅ User trust and control — provide a way to exit or disable if needed. Don’t make it too aggressive or it’ll just get uninstalled.
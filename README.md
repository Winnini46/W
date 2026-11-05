# Focus Lock

A small Windows 10/11 notification-area app. Press **Ctrl + Shift + F12** to lock your Windows session without closing your apps. It uses Windows' secure lock screen, so Alt+F4, Ctrl+T and other shortcuts cannot reach your open apps while locked.

## Run

1. Set a Windows account password and verify that **Win + L** requires your normal sign-in to resume. Focus Lock uses your Windows password/PIN/Windows Hello; it never collects or stores a separate password.
2. Keep all three app files in one folder and double-click **Start Focus Lock.cmd**. Windows PowerShell 5.1 and .NET Framework, included with Windows 10/11, compile the included C# source locally. No downloads, administrator rights, or third-party packages are needed.
3. Press **Ctrl + Shift + F12**, or click **Lock now**. Sign back in normally to resume.

Closing the app window leaves the shield icon and shortcut running. Double-click the shield to reopen it; right-click it to exit. If the shortcut is already registered by another app, Focus Lock reports the conflict and exits. **Win + L** always remains a built-in alternative.

If downloaded files are blocked, inspect the source and use Windows file Properties → Unblock on the downloaded archive before extracting it. Organizational PowerShell policies may require an administrator-approved deployment; the launcher does not override organizational policy.

## Optional: start at sign-in

Press **Win + R**, enter `shell:startup`, and create a shortcut to **Start Focus Lock.cmd** there. Remove that shortcut to disable automatic startup. The app does not change startup settings itself.

## Limits

This protects a signed-in session using Windows security. It does not disable the physical power button, force a password instead of configured Windows Hello/PIN, prevent system updates or sleep, or remove any shutdown controls Windows exposes on its lock screen. Configure those separately in Windows if desired. A person holding the power button can still force a shutdown. The app only requests a lock; Windows controls sign-in and the secure desktop.

## Windows acceptance check

- Launch the app and close its window; the tray icon must remain.
- With a disposable document open, press Ctrl + Shift + F12. Windows must show its lock screen. Try Alt+F4 and Ctrl+T, then sign back in and verify the document and tabs are unchanged.
- Check that incorrect Windows credentials do not unlock the session.
- Reopen the panel from the tray and test Lock now.
- Exit through the tray and confirm the custom shortcut is released; relaunch to confirm it registers again.
- Launch a second copy and confirm it reports that the first is already running.

The Windows UI, global hotkey, and secure-lock behavior require validation on a real Windows desktop. They cannot be exercised on a Linux build host.

using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Threading;
using System.Windows.Forms;

namespace FocusLock
{
    public static class Program
    {
        [STAThread]
        public static void Run()
        {
            bool created;
            using (var mutex = new Mutex(true, "Local\\FocusLock.Tray", out created))
            {
                if (!created)
                {
                    MessageBox.Show("Focus Lock is already running. Look for the shield in your notification area.", "Focus Lock");
                    return;
                }
                Application.EnableVisualStyles();
                Application.SetCompatibleTextRenderingDefault(false);
                using (var app = new LockContext()) Application.Run(app);
            }
        }
    }

    internal sealed class HotkeyWindow : NativeWindow, IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool RegisterHotKey(IntPtr window, int id, uint modifiers, uint key);
        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr window, int id);
        public event EventHandler Pressed;
        public HotkeyWindow()
        {
            CreateHandle(new CreateParams { Caption = "Focus Lock hotkey" });
            // Ctrl + Shift + F12; MOD_NOREPEAT prevents key repeat.
            if (!RegisterHotKey(Handle, 1, 0x4000 | 0x0002 | 0x0004, (uint)Keys.F12))
            {
                DestroyHandle();
                throw new InvalidOperationException("Ctrl + Shift + F12 is already in use. Close the other shortcut app and start Focus Lock again. You can always lock Windows with Win + L.");
            }
        }
        protected override void WndProc(ref Message message)
        {
            if (message.Msg == 0x0312 && message.WParam.ToInt32() == 1)
            {
                var handler = Pressed;
                if (handler != null) handler(this, EventArgs.Empty);
            }
            base.WndProc(ref message);
        }
        public void Dispose()
        {
            UnregisterHotKey(Handle, 1);
            DestroyHandle();
        }
    }

    internal sealed class LockContext : ApplicationContext
    {
        [DllImport("user32.dll", SetLastError = true)]
        private static extern bool LockWorkStation();
        private readonly HotkeyWindow hotkey;
        private readonly NotifyIcon tray;
        private readonly ContextMenuStrip menu;
        private Form panel;

        public LockContext()
        {
            hotkey = new HotkeyWindow();
            hotkey.Pressed += delegate { Lock(); };
            menu = new ContextMenuStrip();
            menu.Items.Add("Lock now   Ctrl + Shift + F12", null, delegate { Lock(); });
            menu.Items.Add("Open Focus Lock", null, delegate { ShowPanel(); });
            menu.Items.Add(new ToolStripSeparator());
            menu.Items.Add("Exit Focus Lock", null, delegate { ExitThread(); });
            tray = new NotifyIcon {
                Icon = SystemIcons.Shield,
                Text = "Focus Lock | Ctrl + Shift + F12",
                ContextMenuStrip = menu,
                Visible = true
            };
            tray.DoubleClick += delegate { ShowPanel(); };
            ShowPanel();
        }

        private void Lock()
        {
            if (!LockWorkStation())
                MessageBox.Show("Windows could not lock this session. Try Win + L. Error: " + Marshal.GetLastWin32Error(), "Focus Lock", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private void ShowPanel()
        {
            if (panel != null && !panel.IsDisposed) { panel.Show(); panel.Activate(); return; }
            panel = new Form {
                Text = "Focus Lock", ClientSize = new Size(440, 345),
                StartPosition = FormStartPosition.CenterScreen,
                FormBorderStyle = FormBorderStyle.FixedDialog,
                MaximizeBox = false, BackColor = Color.FromArgb(20, 25, 36),
                ForeColor = Color.White, Font = new Font("Segoe UI", 10),
                Icon = SystemIcons.Shield, AutoScaleMode = AutoScaleMode.Dpi
            };
            panel.Controls.Add(new Label { Text = "FOCUS LOCK", Location = new Point(28, 25), AutoSize = true, ForeColor = Color.FromArgb(119, 211, 193) });
            panel.Controls.Add(new Label { Text = "Step away. Stay protected.", Location = new Point(25, 58), Size = new Size(395, 38), Font = new Font("Segoe UI", 19, FontStyle.Bold) });
            panel.Controls.Add(new Label { Text = "Lock your session instantly. Your tabs and apps\nstay right where you left them.", Location = new Point(28, 108), Size = new Size(390, 48), ForeColor = Color.LightGray });
            panel.Controls.Add(new Label { Text = "Ctrl  +  Shift  +  F12", TextAlign = ContentAlignment.MiddleCenter, Location = new Point(28, 172), Size = new Size(384, 44), BackColor = Color.FromArgb(36, 44, 60), Font = new Font("Segoe UI", 13, FontStyle.Bold) });
            var button = new Button { Text = "Lock now", Location = new Point(28, 234), Size = new Size(384, 42), FlatStyle = FlatStyle.Flat, BackColor = Color.FromArgb(119, 211, 193), ForeColor = Color.FromArgb(20, 25, 36) };
            button.Click += delegate { Lock(); };
            panel.Controls.Add(button);
            panel.Controls.Add(new Label { Text = "Closing this window keeps the tray shortcut active.\nUnlock using your normal Windows sign-in.", Location = new Point(28, 291), Size = new Size(390, 44), Font = new Font("Segoe UI", 9), ForeColor = Color.LightGray });
            panel.Show();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                tray.Visible = false;
                tray.Dispose();
                hotkey.Dispose();
                menu.Dispose();
                if (panel != null) panel.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}

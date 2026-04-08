using System;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Interop;

namespace CopyAlert
{
    public class ClipboardMonitor : IDisposable
    {
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool AddClipboardFormatListener(IntPtr hwnd);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        private const int WM_CLIPBOARDUPDATE = 0x031D;
        private Window _window;
        private HwndSource? _hwndSource;

        public event EventHandler? ClipboardUpdate;

        public ClipboardMonitor(Window window)
        {
            _window = window;
            _window.SourceInitialized += Window_SourceInitialized;
            
            // If the window is already initialized, hook it now
            if (_window.IsLoaded)
            {
                InitializeHook();
            }
        }

        private void Window_SourceInitialized(object? sender, EventArgs e)
        {
            InitializeHook();
        }

        private void InitializeHook()
        {
            if (_hwndSource == null)
            {
                _hwndSource = PresentationSource.FromVisual(_window) as HwndSource;
                if (_hwndSource != null)
                {
                    _hwndSource.AddHook(HwndHook);
                    AddClipboardFormatListener(_hwndSource.Handle);
                }
            }
        }

        private IntPtr HwndHook(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (msg == WM_CLIPBOARDUPDATE)
            {
                ClipboardUpdate?.Invoke(this, EventArgs.Empty);
            }
            return IntPtr.Zero;
        }

        public void Dispose()
        {
            if (_hwndSource != null)
            {
                RemoveClipboardFormatListener(_hwndSource.Handle);
                _hwndSource.RemoveHook(HwndHook);
                _hwndSource = null;
            }
            _window.SourceInitialized -= Window_SourceInitialized;
        }
    }
}

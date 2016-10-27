namespace Egs
{
    using System;
    using System.Diagnostics;
    using Egs.Win32;

    internal sealed class Win32SetWindowPosition : IDisposable
    {
        IntPtr hWnd;
        IntPtr hWndHiddenOwner;
        IntPtr HWND_TOPMOST = new IntPtr(-1);
        int zeroWindowPostLeftTopValue = 0;

        public Win32SetWindowPosition(IntPtr newHWnd)
        {
            Trace.Assert(newHWnd != IntPtr.Zero);
            hWnd = newHWnd;
            hWndHiddenOwner = NativeMethods.GetWindow(hWnd, NativeMethods.GetWindowCmd.GW_OWNER);
            if (hWndHiddenOwner != IntPtr.Zero)
            {
                NativeMethods.SetWindowPos(hWnd, HWND_TOPMOST, zeroWindowPostLeftTopValue, zeroWindowPostLeftTopValue, 0, 0,
                    NativeMethods.SetWindowPosFlags.SWP_NOMOVE
                    | NativeMethods.SetWindowPosFlags.SWP_NOSIZE
                    | NativeMethods.SetWindowPosFlags.SWP_SHOWWINDOW
                    | NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);
            }
        }

        public void SetWindowPosition(int left, int top)
        {
            NativeMethods.SetWindowPos(hWnd, HWND_TOPMOST, left, top, 0, 0,
                NativeMethods.SetWindowPosFlags.SWP_NOMOVE
                | NativeMethods.SetWindowPosFlags.SWP_NOSIZE
                | NativeMethods.SetWindowPosFlags.SWP_SHOWWINDOW
                | NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);
        }

        public void BringToTop()
        {
            NativeMethods.SetWindowPos(hWnd, HWND_TOPMOST, zeroWindowPostLeftTopValue, zeroWindowPostLeftTopValue, 0, 0,
                NativeMethods.SetWindowPosFlags.SWP_NOMOVE
                | NativeMethods.SetWindowPosFlags.SWP_NOSIZE
                | NativeMethods.SetWindowPosFlags.SWP_SHOWWINDOW
                | NativeMethods.SetWindowPosFlags.SWP_NOACTIVATE);
        }

        private bool disposed = false;
        public void Dispose()
        {
            if (disposed) { return; }
            hWnd = IntPtr.Zero;
            hWndHiddenOwner = IntPtr.Zero;
            HWND_TOPMOST = IntPtr.Zero;
            disposed = true;
            GC.SuppressFinalize(this);
        }
        ~Win32SetWindowPosition() { Dispose(); }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>Information about DPI of a display</summary>
    [EditorBrowsable(EditorBrowsableState.Never)]
    public struct Dpi
    {
        /// <summary>new Dpi(96, 96)</summary>
        public static readonly Dpi Default = new Dpi(96, 96);
        public double X { get; set; }
        public double Y { get; set; }
        public static bool operator ==(Dpi dpi1, Dpi dpi2) { return dpi1.X == dpi2.X && dpi1.Y == dpi2.Y; }
        public static bool operator !=(Dpi dpi1, Dpi dpi2) { return !(dpi1 == dpi2); }
        public bool Equals(Dpi other) { return X == other.X && Y == other.Y; }
        public override bool Equals(object obj) { if (ReferenceEquals(null, obj)) { return false; } else { return obj is Dpi && Equals((Dpi)obj); } }
        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + X.GetHashCode();
                hash = hash * 23 + Y.GetHashCode();
                return hash;
            }
        }
        public override string ToString() { return string.Format(System.Globalization.CultureInfo.CurrentCulture, "({0},{1})", X, Y); }

        public Dpi(double x, double y)
            : this()
        {
            X = x;
            Y = y;
        }

        public static Dpi DpiFromHdcForTheEntireScreen
        {
            get
            {
                var ret = new Dpi();
                var hdcForTheEntireScreen = Win32.NativeMethods.GetDC(IntPtr.Zero);
                using (var g = System.Drawing.Graphics.FromHdc(hdcForTheEntireScreen))
                {
                    ret.X = g.DpiX;
                    ret.Y = g.DpiY;
                }
                Win32.NativeMethods.ReleaseDC(IntPtr.Zero, hdcForTheEntireScreen);
                return ret;
            }
        }

        static Dpi GetDpiFromGetDpiForMonitor(IntPtr hMonitor, Win32.MonitorDpiType dpiType)
        {
            try
            {
                var ret = new Dpi();
                uint dpiX = 0, dpiY = 0;
                Win32.NativeMethods.GetDpiForMonitor(hMonitor, dpiType, ref dpiX, ref dpiY);
                ret.X = dpiX;
                ret.Y = dpiY;
                return ret;
            }
            catch (Exception ex)
            {
#if DEBUG
                Debugger.Break();
#endif
                Debug.WriteLine(ex.Message);
                return Dpi.Default;
            }
        }

        public static Dpi DpiFromGetDpiForMonitorOfPrimaryMonitorWithEffectiveDpi
        {
            get
            {
                var hMonitor = Win32.NativeMethods.MonitorFromWindow(IntPtr.Zero, Win32.NativeMethods.MonitorDefaultTo.MONITOR_DEFAULTTOPRIMARY);
                return GetDpiFromGetDpiForMonitor(hMonitor, Win32.MonitorDpiType.EffectiveDpi);
            }
        }

        public static Dpi DpiFromGetDpiForMonitorOfNearestMonitorWithAngularDpi
        {
            get
            {
                var hMonitor = Win32.NativeMethods.MonitorFromWindow(IntPtr.Zero, Win32.NativeMethods.MonitorDefaultTo.MONITOR_DEFAULTTOPRIMARY);
                return GetDpiFromGetDpiForMonitor(hMonitor, Win32.MonitorDpiType.AngularDpi);
            }
        }

        public static Dpi DpiFromGetDpiForMonitorOfNearestMonitorWithRawDpi
        {
            get
            {
                var hMonitor = Win32.NativeMethods.MonitorFromWindow(IntPtr.Zero, Win32.NativeMethods.MonitorDefaultTo.MONITOR_DEFAULTTOPRIMARY);
                return GetDpiFromGetDpiForMonitor(hMonitor, Win32.MonitorDpiType.RawDpi);
            }
        }

        public static System.Drawing.Size GetPrimaryScreenPhysicalPixelResolution()
        {
            var ret = new System.Drawing.Size();
            var hdcForTheEntireScreen = Win32.NativeMethods.GetDC(IntPtr.Zero);
            const int DESKTOPVERTRES = 117;
            const int DESKTOPHORZRES = 118;
            ret.Width = Win32.NativeMethods.GetDeviceCaps(hdcForTheEntireScreen, DESKTOPHORZRES);
            ret.Height = Win32.NativeMethods.GetDeviceCaps(hdcForTheEntireScreen, DESKTOPVERTRES);
            Win32.NativeMethods.ReleaseDC(IntPtr.Zero, hdcForTheEntireScreen);
            return ret;
        }

        public System.Drawing.Point ScaledCursorPosition
        {
            get
            {
                var winFormsCursorPosition = System.Windows.Forms.Cursor.Position;
                var ret = new System.Drawing.Point((int)(winFormsCursorPosition.X * Default.X / X), (int)(winFormsCursorPosition.Y * Default.Y / Y));
                return ret;
            }
        }

        public System.Drawing.Rectangle ScaledPrimaryScreenBounds
        {
            get
            {
                var winFormsScreenPrimaryScreenBounds = System.Windows.Forms.Screen.PrimaryScreen.Bounds;
                var ret = new System.Drawing.Rectangle(
                    (int)(winFormsScreenPrimaryScreenBounds.X * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenBounds.Y * Default.Y / Y),
                    (int)(winFormsScreenPrimaryScreenBounds.Width * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenBounds.Height * Default.Y / Y));
                return ret;
            }
        }

        public System.Drawing.Rectangle ScaledPrimaryScreenWorkindArea
        {
            get
            {
                var winFormsScreenPrimaryScreenWorkingArea = System.Windows.Forms.Screen.PrimaryScreen.WorkingArea;
                var ret = new System.Drawing.Rectangle(
                    (int)(winFormsScreenPrimaryScreenWorkingArea.X * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenWorkingArea.Y * Default.Y / Y),
                    (int)(winFormsScreenPrimaryScreenWorkingArea.Width * Default.X / X),
                    (int)(winFormsScreenPrimaryScreenWorkingArea.Height * Default.Y / Y));
                return ret;
            }
        }
    }
}

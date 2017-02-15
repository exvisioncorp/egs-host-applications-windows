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

        public System.Drawing.Point GetScaledPosition(System.Drawing.Point position)
        {
            var ret = new System.Drawing.Point(
                (int)(position.X * Dpi.Default.X / X),
                (int)(position.Y * Dpi.Default.Y / Y));
            return ret;
        }

        public System.Drawing.Rectangle GetScaledRectangle(System.Drawing.Rectangle rectangle)
        {
            var ret = new System.Drawing.Rectangle(
                (int)(rectangle.X * Dpi.Default.X / X),
                (int)(rectangle.Y * Dpi.Default.Y / Y),
                (int)(rectangle.Width * Dpi.Default.X / X),
                (int)(rectangle.Height * Dpi.Default.Y / Y));
            return ret;
        }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    [EditorBrowsable(EditorBrowsableState.Never)]
    public static class DpiExtensions
    {
        public static Dpi DpiFromSystemParameters
        {
            get
            {
                var dpiXProperty = typeof(SystemParameters).GetProperty("DpiX", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var dpiYProperty = typeof(SystemParameters).GetProperty("Dpi", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);
                var ret = new Dpi();
                ret.X = (int)dpiXProperty.GetValue(null, null);
                ret.Y = (int)dpiYProperty.GetValue(null, null);
                return ret;
            }
        }

        /// <summary>
        /// Get primary screen's DPI from PresentationSource.FromVisual(visual).CompositionTarget.TransformToDevice
        /// </summary>
        public static Dpi GetDpiFromCompositionTargetTransformToDevice(System.Windows.Media.Visual visual)
        {
            Debug.Assert(visual != null);
            if (visual == null) { return Dpi.Default; }
            var hwndSource = PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            if (hwndSource == null) { return Dpi.Default; }
            if (hwndSource.CompositionTarget == null) { return Dpi.Default; }
            return new Dpi((int)(Dpi.Default.X * hwndSource.CompositionTarget.TransformToDevice.M11), (int)(Dpi.Default.Y * hwndSource.CompositionTarget.TransformToDevice.M22));
        }

        /// <summary>
        /// You can get current screen's DPi (screen which has mouse cursor currently), when you set dpiType to RawDpi or AngularDpi.
        /// Refer to https://msdn.microsoft.com/en-us/library/windows/desktop/dn280510(v=vs.85).aspx
        /// </summary>
        public static Dpi GetDpiFromGetDpiForMonitor(System.Windows.Media.Visual visual, Win32.MonitorDpiType dpiType = Win32.MonitorDpiType.Default)
        {
            Debug.Assert(visual != null);
            if (visual == null) { return Dpi.Default; }
            // NOTE: app.manifestに、Windows8.1対応を明記しなければならないため、このような判定は行わないことにする。
            //if (IsPerMonitorDpiServiceSupported == false) { return GetDpiFromCompositionTargetTransformToDevice(visual); }
            var hwndSource = PresentationSource.FromVisual(visual) as System.Windows.Interop.HwndSource;
            if (hwndSource == null) { return Dpi.Default; }
            var hmonitor = Win32.NativeMethods.MonitorFromWindow(hwndSource.Handle, Win32.NativeMethods.MonitorDefaultTo.MONITOR_DEFAULTTONEAREST);
            uint dpiX = 96, dpiY = 96;
            Win32.NativeMethods.GetDpiForMonitor(hmonitor, (Win32.MonitorDpiType)dpiType, ref dpiX, ref dpiY);
            return new Dpi((int)dpiX, (int)dpiY);
        }

        /// <summary>
        /// Just return the result of "Environment.OSVersion.Version >= new Version(6, 3)".  Before you use this, you have to activate "Application is designed for Windows 8.1" in app.manifest.
        /// Refer to https://github.com/Grabacr07/XamClaudia (Japanese)
        /// </summary>
        public static bool IsPerMonitorDpiServiceSupported { get { return Environment.OSVersion.Version >= new Version(6, 3); } }
    }
}

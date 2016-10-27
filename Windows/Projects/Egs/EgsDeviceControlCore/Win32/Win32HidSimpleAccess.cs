namespace Egs
{
    using System;
    using System.IO;
    using System.Diagnostics;
    using Egs.Win32;

    /// <summary>
    /// Exception when HID access causes some problems
    /// </summary>
    [Serializable]
    public sealed class HidSimpleAccessException : Exception
    {
        /// <summary>
        /// Error message when CreateFile() failed
        /// </summary>
        public static string CreateFileFailedErrorMessage { get { return "Failed to CreateFile to HID by the current DevicePath.  If the device is connected, make sure the application updates the DevicePath when the device is reconnected."; } }
        public HidSimpleAccessException(string message) : base(message) { }
    }

    internal static class Win32HidSimpleAccess
    {
        static object lockForHidSimpleAccess = new object();

        internal static void WriteOutputReport(string devicePath, byte[] outputReport)
        {
            if (string.IsNullOrEmpty(devicePath)) { throw new ArgumentNullException("devicePath"); }
            if (outputReport == null) { throw new ArgumentNullException("outputReport"); }
            lock (lockForHidSimpleAccess)
            {
                using (var handle = NativeMethods.CreateFile(devicePath, FileAccess.ReadWrite, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, NativeMethods.EFileAttributes.Overlapped, IntPtr.Zero))
                {
                    if (handle.IsInvalid)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new HidSimpleAccessException(HidSimpleAccessException.CreateFileFailedErrorMessage);
                    }
                    // Don't attempt to send an Output report if the Win32HidSimpleAccess has no Output report.  (It ignores this because we assume that device is not re-connected.）
                    using (var deviceDataFileStream = new FileStream(handle, FileAccess.Read | FileAccess.Write, outputReport.Length, true))
                    {
                        // TODO: Define the correct specification.
                        Debug.Assert(deviceDataFileStream.CanWrite);
                        deviceDataFileStream.Write(outputReport, 0, outputReport.Length);
                    }
                }
            }
        }

        internal static void SetFeatureReport(string devicePath, byte[] featureReport)
        {
            if (false) { Debug.WriteLine("Win32HidSimpleAccess.SetFeatureReport() called."); }
            if (string.IsNullOrEmpty(devicePath)) { throw new ArgumentNullException("devicePath"); }
            if (featureReport == null) { throw new ArgumentNullException("featureReport"); }
            lock (lockForHidSimpleAccess)
            {
                using (var handle = NativeMethods.CreateFile(devicePath, FileAccess.Write, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, NativeMethods.EFileAttributes.Overlapped, IntPtr.Zero))
                {
                    if (handle.IsInvalid)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new HidSimpleAccessException(HidSimpleAccessException.CreateFileFailedErrorMessage);
                    }
                    var ret = NativeMethods.HidD_SetFeature(handle, featureReport, featureReport.Length);
                    if (ret == false)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new HidSimpleAccessException("Win32HidSimpleAccess.SetFeatureReport failed!");
                    }
                }
            }
        }

        internal static void GetFeatureReport(string devicePath, byte[] featureReport)
        {
            if (false) { Debug.WriteLine("Win32HidSimpleAccess.GetFeatureReport() called."); }
            if (string.IsNullOrEmpty(devicePath)) { throw new ArgumentNullException("devicePath"); }
            if (featureReport == null) { throw new ArgumentNullException("featureReport"); }
            lock (lockForHidSimpleAccess)
            {
                using (var handle = NativeMethods.CreateFile(devicePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, NativeMethods.EFileAttributes.Overlapped, IntPtr.Zero))
                {
                    if (handle.IsInvalid)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new HidSimpleAccessException(HidSimpleAccessException.CreateFileFailedErrorMessage);
                    }
                    var ret = NativeMethods.HidD_GetFeature(handle, featureReport, featureReport.Length);
                    if (ret == false)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new HidSimpleAccessException("Win32HidSimpleAccess.GetFeatureReport failed!");
                    }
                }
            }
        }
    }
}

namespace Egs
{
    using System;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.Diagnostics;
    using Egs.PropertyTypes;
    using Egs.Win32;

    internal sealed class EgsDeviceHidReportsUpdateByWin32RawInput : IDisposable
    {
        internal EgsDevice Device { get; set; }

        NativeMethods.RAWINPUT rawInput;
        NativeMethods.RawInputHeaderType latestRawInputKind;
        int rawInputDataBufferSize;
        byte[] bRawData;

        // TODO: check memory leak
        IntPtr rawInputPtr = IntPtr.Zero;

        internal byte[] reportAsByteArray;
        internal int reportAsByteArrayActualLength;

        internal EgsDeviceHidReportsUpdateByWin32RawInput()
        {
            latestRawInputKind = NativeMethods.RawInputHeaderType.None;

            rawInputDataBufferSize = 0;
            reportAsByteArrayActualLength = 0;
            int bRawDataOfMouseMaxLength = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER)) + Marshal.SizeOf(typeof(NativeMethods.RAWMOUSE));
            int offsetForHid = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER)) + Marshal.SizeOf(typeof(NativeMethods.RAWHID));
            int bRawDataOfMaxHid64BytesLength = offsetForHid + 64;
            int bRawDataMaxLength = Math.Max(bRawDataOfMouseMaxLength, bRawDataOfMaxHid64BytesLength);
            bRawData = new byte[bRawDataMaxLength];
            int touchReportMaxLength = Math.Max(Marshal.SizeOf(typeof(NativeMethods.RAWMOUSE)), 64);
            reportAsByteArray = new byte[64];
        }

        bool CopyRawInputToDataByteArray(IntPtr lParam)
        {
            int dwSize = 0;
            int rawInputHeaderSize = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER));
            //If pData is NULL, the required size of the buffer is returned in *pcbSize.
            NativeMethods.GetRawInputData(lParam, (uint)NativeMethods.RID.INPUT, IntPtr.Zero, ref dwSize, rawInputHeaderSize);

            if (dwSize > rawInputDataBufferSize)
            {
                rawInputDataBufferSize = dwSize;
                ReleaseRawInputPtr();
            }
            if (rawInputPtr == IntPtr.Zero)
            {
                // allocate unmanaged memory, must be released by Marshal.FreeHGlobal in ReleaseRawInputDataBuffer()
                rawInputPtr = Marshal.AllocHGlobal((int)rawInputDataBufferSize);
                if (rawInputPtr == IntPtr.Zero)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    return false;
                }
            }
            try
            {
                if (NativeMethods.GetRawInputData(lParam, (uint)NativeMethods.RID.INPUT, rawInputPtr, ref dwSize, rawInputHeaderSize) != dwSize)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    return false;
                }
                rawInput = (NativeMethods.RAWINPUT)Marshal.PtrToStructure(rawInputPtr, typeof(NativeMethods.RAWINPUT));
                //Debug.WriteLine("rawInput.header.hDevice: " + rawInput.header.hDevice);

                latestRawInputKind = rawInput.header.dwType;

                int offset = 0;
                reportAsByteArrayActualLength = 0;
                switch (latestRawInputKind)
                {
                    case NativeMethods.RawInputHeaderType.Hid:
                        offset = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER)) + Marshal.SizeOf(typeof(NativeMethods.RAWHID));
                        reportAsByteArrayActualLength = rawInput.hid.dwSizeHid;
                        break;
                    case NativeMethods.RawInputHeaderType.Mouse:
                        offset = Marshal.SizeOf(typeof(NativeMethods.RAWINPUTHEADER));
                        reportAsByteArrayActualLength = Marshal.SizeOf(typeof(NativeMethods.RAWMOUSE));
                        break;
                    case NativeMethods.RawInputHeaderType.Keyboard:
                        return false;
                        break;
                    default:
                        return false;
                        break;
                }
                if (reportAsByteArrayActualLength > 64) { return false; }
                Marshal.Copy(rawInputPtr, bRawData, 0, offset + reportAsByteArrayActualLength);
                Array.Copy(bRawData, offset, reportAsByteArray, 0, reportAsByteArrayActualLength);
                return true;
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Debug.WriteLine(ex.Message);
                return false;
            }
        }

        void UpdateHidReport()
        {
            switch ((HidReportIds)reportAsByteArray[0])
            {
                case HidReportIds.TouchScreen:
                    Device.TouchScreenHidReport.UpdateByHidReportAsByteArray(reportAsByteArray);
                    break;
                case HidReportIds.EgsGesture:
                    Device.EgsGestureHidReport.UpdateByHidReportAsByteArray(reportAsByteArray);
                    break;
                default:
                    // TODO: MUSTDO: NOTE: Must check.  Because this value is "ReportId", so it should be correct.
                    var bufStr = BitConverter.ToString(reportAsByteArray, 0, reportAsByteArray.Length);
                    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "(HidReportIds)reportAsByteArray[0]: {0,3}  {1}  {2:HH:mm:ss.fff}", (HidReportIds)reportAsByteArray[0], bufStr, DateTime.Now));
                    if (false && ApplicationCommonSettings.IsDebuggingInternal) { Debugger.Break(); }
                    break;
            }
        }

        void UpdateTouchScreenHidReport()
        {
            if (ApplicationCommonSettings.IsToEmulateReportByActualMouseRawInputToDebugViews)
            {
                Device.EgsGestureHidReport.UpdateByRawMouse(ref rawInput.mouse);
                // NOTE: If "emulated input" and "actual HID input" are inconsistent, it makes no sense.  So it returns here.
                // Before I forgot that I was using mouse emulation mode, I could not understand why CameraView and GestureCursors are not displayed.  I was troubled.
                return;
            }
            switch (latestRawInputKind)
            {
                case NativeMethods.RawInputHeaderType.Hid:
                    UpdateHidReport();
                    break;
                case NativeMethods.RawInputHeaderType.Mouse:
                    Device.TouchScreenHidReport.UpdateByRawMouse(ref rawInput.mouse);
                    break;
                case NativeMethods.RawInputHeaderType.Keyboard:
                    break;
                default:
                    break;
            }
        }

        internal void ProcessRawInputData(IntPtr lParam)
        {
            var hr = CopyRawInputToDataByteArray(lParam);
            if (hr == false) { return; }
            UpdateTouchScreenHidReport();
            Device.LastUpdateTime = DateTime.Now;
        }

        private bool disposed = false;
        void ReleaseRawInputPtr()
        {
            if (rawInputPtr != IntPtr.Zero) { Marshal.FreeHGlobal(rawInputPtr); rawInputPtr = IntPtr.Zero; }
        }
        public void Dispose()
        {
            if (disposed) { return; }
            ReleaseRawInputPtr();
            disposed = true;
            GC.SuppressFinalize(this);
        }
        ~EgsDeviceHidReportsUpdateByWin32RawInput() { Dispose(); }
    }
}

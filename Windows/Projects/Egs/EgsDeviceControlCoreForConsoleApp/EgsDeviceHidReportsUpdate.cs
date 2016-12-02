namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.IO;
    using Egs;
    using Egs.Win32;
    using Egs.PropertyTypes;

    internal sealed class EgsDeviceHidReportsUpdate
    {
        EgsDevice owner { get; set; }
        internal System.ComponentModel.BackgroundWorker ReportMonitoringThread { get; private set; }
        internal string DevicePath { get; private set; }
        internal bool IsInvalidHandle { get; private set; }
        internal bool HasStoppedReportMonitoringThread { get; private set; }
        internal byte[] reportAsByteArray { get; private set; }
        Microsoft.Win32.SafeHandles.SafeFileHandle readHandle { get; set; }

        public EgsDeviceHidReportsUpdate()
        {
        }

        public void InitializeOnceAtStartup(EgsDevice device)
        {
            Trace.Assert(device != null);
            owner = device;
        }

        public void Start(string devicePath)
        {
            Trace.Assert(string.IsNullOrEmpty(devicePath) == false);
            DevicePath = devicePath;
            IsInvalidHandle = false;
            HasStoppedReportMonitoringThread = false;
            ReportMonitoringThread = new System.ComponentModel.BackgroundWorker() { WorkerSupportsCancellation = true };
            ReportMonitoringThread.DoWork += ReportMonitoringThread_DoWork;
            ReportMonitoringThread.RunWorkerCompleted += ReportMonitoringThread_RunWorkerCompleted;
            reportAsByteArray = new byte[64];
            ReportMonitoringThread.RunWorkerAsync();
        }

        void ReportMonitoringThread_DoWork(object sender, System.ComponentModel.DoWorkEventArgs e)
        {
#if false
            while (ReportMonitoringThread.CancellationPending == false)
            {
                owner.EgsGestureHidReport.UpdateByHidReportAsByteArray(reportAsByteArray);
            }
#else
            // If fileShare is not FileShare.ReadWrite, it causes errors in the other CreateFile().
            // "NativeMethods.EFileAttributes.Overlapped" and "NativeMethods.EFileAttributes.Overlapped | NativeMethods.EFileAttributes.Device" does not work!!
            using (var readHandle = NativeMethods.CreateFile(DevicePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, NativeMethods.EFileAttributes.Device, IntPtr.Zero))
            {
#if false
                if (true)
                {
                    // It works on Console Apps, but it may not work on Unity Apps.
                    IsInvalidHandle = readHandle.IsInvalid;
                    if (IsInvalidHandle)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new HidSimpleAccessException(HidSimpleAccessException.CreateFileFailedErrorMessage);
                    }
                    using (var deviceDataFileStream = new FileStream(readHandle.DangerousGetHandle(), FileAccess.Read, false, reportAsByteArray.Length, true))
                    {
                        while (ReportMonitoringThread.CancellationPending == false)
                        {
                            if (deviceDataFileStream.CanRead == false)
                            {
                                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                                throw new HidSimpleAccessException("deviceDataFileStream.CanRead == false");
                            }
                            deviceDataFileStream.Read(reportAsByteArray, 0, reportAsByteArray.Length);
                            if ((HidReportIds)reportAsByteArray[0] == HidReportIds.EgsGesture)
                            {
                                owner.EgsGestureHidReport.UpdateByHidReportAsByteArray(reportAsByteArray);
                            }
                        }
                    }
                }
#else
                if (true)
                {
                    // It does not work on Console App.
                    int numberOfBytesRead = 0;
                    //int overlappedBuffer = 0;
                    //System.Threading.NativeOverlapped overlappedBuffer = new System.Threading.NativeOverlapped();

                    while (ReportMonitoringThread.CancellationPending == false)
                    {
                        //NativeMethods.ReadFile(readHandle, reportAsByteArray, reportAsByteArray.Length, ref numberOfBytesRead, ref overlappedBuffer);
                        var hr = NativeMethods.ReadFile(readHandle, reportAsByteArray, reportAsByteArray.Length, out numberOfBytesRead, IntPtr.Zero);
                        if (false && hr) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } }
                        if (numberOfBytesRead != 0)
                        {
                            if ((HidReportIds)reportAsByteArray[0] == HidReportIds.EgsGesture)
                            {
                                owner.EgsGestureHidReport.UpdateByHidReportAsByteArray(reportAsByteArray);
                            }
                        }
                        System.Threading.Thread.Sleep(1);
                    }
                }
#endif
            }
#endif
        }

        void ReportMonitoringThread_RunWorkerCompleted(object sender, System.ComponentModel.RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (e.Error is HidSimpleAccessException)
                {
                    if (ApplicationCommonSettings.IsDebugging)
                    {
                        Debugger.Break();
                        Console.WriteLine(e.Error.Message);
                    }
                }
                else
                {
                    Console.WriteLine(e.Error.Message);
                }
            }
            if (e.Cancelled) { HasStoppedReportMonitoringThread = true; }
        }

        public void OnDisable()
        {
            if (ReportMonitoringThread == null) { return; }
            ReportMonitoringThread.CancelAsync();
            var sw = Stopwatch.StartNew();
            while (true)
            {
                if (HasStoppedReportMonitoringThread == false) { break; }
                if (sw.ElapsedMilliseconds > 1000)
                {
                    if (ApplicationCommonSettings.IsDebugging)
                    {
                        Debugger.Break();
                        Console.WriteLine("sw.ElapsedMilliseconds > 1000");
                    }
                    break;
                }
                System.Threading.Thread.Sleep(100);
            }
        }
    }
}

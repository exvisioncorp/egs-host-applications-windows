namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.IO;
    using System.ComponentModel;
    using Egs;
    using Egs.Win32;
    using Egs.PropertyTypes;

    internal sealed class EgsDeviceHidReportsUpdate
    {
        EgsDevice owner { get; set; }
        internal BackgroundWorker ReportMonitoringThread { get; private set; }
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
            ReportMonitoringThread = new BackgroundWorker() { WorkerSupportsCancellation = true };
            ReportMonitoringThread.DoWork += ReportMonitoringThread_DoWork;
            ReportMonitoringThread.RunWorkerCompleted += ReportMonitoringThread_RunWorkerCompleted;
            reportAsByteArray = new byte[64];
            ReportMonitoringThread.RunWorkerAsync();
        }

        void ReportMonitoringThread_DoWork(object sender, DoWorkEventArgs e)
        {
            // If fileShare is not FileShare.ReadWrite, it causes errors in the other CreateFile().
            // "NativeMethods.EFileAttributes.Overlapped" and "NativeMethods.EFileAttributes.Overlapped | NativeMethods.EFileAttributes.Device" does not work!!
            using (var readHandle = NativeMethods.CreateFile(DevicePath, FileAccess.Read, FileShare.ReadWrite, IntPtr.Zero, FileMode.Open, NativeMethods.EFileAttributes.Device, IntPtr.Zero))
            {
                IsInvalidHandle = readHandle.IsInvalid;
                if (IsInvalidHandle)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new HidSimpleAccessException(HidSimpleAccessException.CreateFileFailedErrorMessage);
                }

                // "FileStream and FileStream.Read" works on Console Apps, but it may not work on Unity Apps.
                //using (var deviceDataFileStream = new FileStream(readHandle.DangerousGetHandle(), FileAccess.Read, false, reportAsByteArray.Length, true))
                // (In loop)
                //deviceDataFileStream.Read(reportAsByteArray, 0, reportAsByteArray.Length);

                int numberOfBytesRead = 0;
                while (ReportMonitoringThread.CancellationPending == false)
                {
                    // I think that this method is synchronous.
                    var hr = NativeMethods.ReadFile(readHandle, reportAsByteArray, reportAsByteArray.Length, out numberOfBytesRead, IntPtr.Zero);
                    // So the next line is unnecessary.
                    // System.Threading.Thread.Sleep(1);
                    
                    if (false && hr) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } }
                    if (numberOfBytesRead != 0)
                    {
                        if ((HidReportIds)reportAsByteArray[0] == HidReportIds.EgsGesture)
                        {
                            owner.EgsGestureHidReport.UpdateByHidReportAsByteArray(reportAsByteArray);
                            owner.UpdateIsDetectingFaces();
                            owner.UpdateIsDetectingHands();
                        }
                    }
                }
                if (ReportMonitoringThread.CancellationPending) { e.Cancel = true; }
            }
        }

        void ReportMonitoringThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
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

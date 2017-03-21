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
        BackgroundWorker ReportMonitoringThread { get; set; }
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

            // NOTE: When the device is disconnected, the ReportMonitoringThread must be completed.
            if (ReportMonitoringThread != null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                OnDisable();
            }

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
                while (true)
                {
                    if (ReportMonitoringThread == null)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        break;
                    }
                    if (ReportMonitoringThread.CancellationPending)
                    {
                        e.Cancel = true;
                        break;
                    }

                    // I think that this method is synchronous.
                    var hr = NativeMethods.ReadFile(readHandle, reportAsByteArray, reportAsByteArray.Length, out numberOfBytesRead, IntPtr.Zero);
                    // So the next line is unnecessary, and the next line causes too much wait.
                    // System.Threading.Thread.Sleep(1);

                    // TODO: MUSTDO: NOTE: In some C++ Win32 application and .NET Stream classes, the callback function can be registered, so it can raise events only when it gets report from device.
                    // But in this app, it seems to be impossible to register the callback function.  So it calls ReadFile() too many times.  I want to fix it.
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
            }
        }

        void ReportMonitoringThread_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            if (e.Error != null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Console.WriteLine(e.Error.Message);
            }
            HasStoppedReportMonitoringThread = true;
        }

        public void OnDisable()
        {
            if (ReportMonitoringThread == null) { return; }
            ReportMonitoringThread.CancelAsync();
            var sw = Stopwatch.StartNew();
            while (true)
            {
                if (HasStoppedReportMonitoringThread == false)
                {
                    ReportMonitoringThread.DoWork -= ReportMonitoringThread_DoWork;
                    ReportMonitoringThread.RunWorkerCompleted -= ReportMonitoringThread_RunWorkerCompleted;
                    ReportMonitoringThread.Dispose();
                    ReportMonitoringThread = null;
                    break;
                }
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

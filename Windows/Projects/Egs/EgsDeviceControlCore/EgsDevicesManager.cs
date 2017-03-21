namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Globalization;
    using Egs.EgsDeviceControlCore.Properties;

    /// <summary>
    /// Exception thrown by some process (e.g. Firmware Update) in the application, to close the host application
    /// </summary>
    [Serializable]
    public sealed class EgsHostApplicationIsClosingException : Exception
    {
        public EgsHostApplicationIsClosingException(string reasonMessage) : base(reasonMessage) { }
    }

    internal class EgsDevicesManager : IDisposable, INotifyPropertyChanged
    {
        class CameraInterfaceInformation
        {
            public uint Index { get; set; }
            public string Description { get; set; }
            public string DevicePath { get; set; }
            public string RootDeviceDevicePath { get; set; }
            public string RootDeviceSerialNumber { get; set; }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        internal AForge.Video.DirectShow.FilterInfoCollection AForgeVideoCaptureDeviceInformationCollection { get; private set; }

        public IList<EgsDevice> DeviceList { get; private set; }
        internal EgsDevicesWindowMessageReceivingForm MessageReceivingForm { get; private set; }

        System.Windows.Forms.Timer EachDeviceStatusMonitoringTimer { get; set; }
        public double TemperatureMonitoringTimerIntervalTotalSeconds
        {
            get { return (double)EachDeviceStatusMonitoringTimer.Interval / 1000.0; }
            set
            {
                if (value < 1.0 || 60.0 < value)
                {
                    throw new ArgumentOutOfRangeException("TemperatureMonitoringTimerInterval", "Interval must be from 1[sec] to 60[sec]");
                }
                EachDeviceStatusMonitoringTimer.Interval = (int)(value * 1000.0);
                OnPropertyChanged(nameof(TemperatureMonitoringTimerIntervalTotalSeconds));
            }
        }

        System.Windows.Forms.Timer OnDeviceConnectedDelayTimer { get; set; }
        System.Windows.Forms.Timer OnDeviceDisconnectedDelayTimer { get; set; }

        internal EgsDevicesManager()
        {
            DeviceList = new List<EgsDevice>();
            MessageReceivingForm = new EgsDevicesWindowMessageReceivingForm();

            EachDeviceStatusMonitoringTimer = new System.Windows.Forms.Timer() { Interval = ApplicationCommonSettings.IsDebugging ? 1000 : 5000 };
            EachDeviceStatusMonitoringTimer.Tick += delegate
            {
                foreach (var device in DeviceList) { device.UpdateTemperatureProperties(); }
            };
            EachDeviceStatusMonitoringTimer.Start();

            // MUSTDO: Need investigatino.  "Connecting a device after running a host application" can cause a problem that the host application cannot catch the mouse event from the device!
            // NOTE: Immediately after the source event raises, AForge.NET (DirectShow wrapper) cannot enumerate all devices!!  So I added delay reluctantly.
            OnDeviceConnectedDelayTimer = new System.Windows.Forms.Timer();
            OnDeviceConnectedDelayTimer.Interval = 2000;
            OnDeviceConnectedDelayTimer.Tick += delegate
            {
                OnDeviceConnectedDelayTimer.Stop();
                UpdateNotInitializedFirstEgsDeviceOnSomeDeviceConnected();
            };
            MessageReceivingForm.DeviceConnected += delegate { OnDeviceConnectedDelayTimer.Start(); };

            OnDeviceDisconnectedDelayTimer = new System.Windows.Forms.Timer();
            OnDeviceDisconnectedDelayTimer.Interval = 2000;
            OnDeviceDisconnectedDelayTimer.Tick += delegate
            {
                OnDeviceDisconnectedDelayTimer.Stop();
                InitializeDisconnectedEgsDeviceOnSomeDeviceDisconnected();
            };
            MessageReceivingForm.DeviceDisconnected += delegate { OnDeviceDisconnectedDelayTimer.Start(); };
        }

        IList<CameraInterfaceInformation> GetCameraInterfaceInformationList()
        {
            var ret = new List<CameraInterfaceInformation>();
            try
            {
                var vidKeyStr = "vid_" + Win32SetupDiForEgsDevice.VendorId.ToString("x", CultureInfo.InvariantCulture);
                var pidKeyStr = "pid_" + Win32SetupDiForEgsDevice.ProductId.ToString("x", CultureInfo.InvariantCulture);

                AForgeVideoCaptureDeviceInformationCollection = new AForge.Video.DirectShow.FilterInfoCollection(AForge.Video.DirectShow.FilterCategory.VideoInputDevice);
                int variousMakersCamerasCount = AForgeVideoCaptureDeviceInformationCollection.Count;
                for (int i = 0; i < variousMakersCamerasCount; i++)
                {
                    string description = AForgeVideoCaptureDeviceInformationCollection[i].Name;
                    string devicePath = AForgeVideoCaptureDeviceInformationCollection[i].MonikerString;
                    if (devicePath.Contains(vidKeyStr) && devicePath.Contains(pidKeyStr))
                    {
                        var matchedItem = new CameraInterfaceInformation()
                        {
                            Index = (uint)i,
                            Description = description,
                            DevicePath = devicePath
                        };
                        // TODO: MUSTDO: get serial number from device path
                        var devicePathSubStrings = devicePath.Split('#').SelectMany(e => e.Split('&'));
                        if (false)
                        {
                            foreach (var item in devicePathSubStrings) { Debug.WriteLine(item); };
                        }
                        matchedItem.RootDeviceDevicePath = devicePathSubStrings.Where(e => e.ToLowerInvariant().Contains("exva")).SingleOrDefault();
                        ret.Add(matchedItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw;
            }
            return ret;
        }

        internal void UpdateNotInitializedFirstEgsDeviceOnSomeDeviceConnected()
        {
            // TODO: MUSTDO: Test with connecting and re-connecting USB connector, because various problems can occur here.
            try
            {
                // At first it checks HID connection.
                MessageReceivingForm.SetupDi.Update();
                foreach (var device in DeviceList) { device.UpdateHidDeviceConnectionStatus(); }

                // And then it checks camera connection.
                var cameras = GetCameraInterfaceInformationList();
                Debug.WriteLine("cameras.Count: " + cameras.Count);
                foreach (var camera in cameras)
                {
                    var matchedDeviceExists = DeviceList.Any(e => e.CameraViewImageSourceBitmapCapture.VideoCaptureDeviceIndex == camera.Index);
                    if (matchedDeviceExists) { continue; }
                    var notInitializedFirstEgsDevice = DeviceList.FirstOrDefault(e => (e.CameraViewImageSourceBitmapCapture.VideoCaptureDeviceIndex.HasValue == false));
                    if (notInitializedFirstEgsDevice == null) { return; }

                    Trace.Assert(notInitializedFirstEgsDevice.Settings != null);

                    // TODO: MUSTDO: get root device's serial number;
                    notInitializedFirstEgsDevice.CameraViewImageSourceBitmapCapture.VideoCaptureDeviceIndex = camera.Index;
                    notInitializedFirstEgsDevice.CameraViewImageSourceBitmapCapture.VideoCaptureDeviceName = camera.Description;
                    notInitializedFirstEgsDevice.CameraViewImageSourceBitmapCapture.VideoCaptureDeviceDevicePath = camera.DevicePath;
                    notInitializedFirstEgsDevice.TrySetIsCameraDeviceConnectedToTrue();
                    Debug.WriteLine("notInitializedFirstEgsDevice.IsConnected: " + notInitializedFirstEgsDevice.IsConnected);
                }
            }
            catch (EgsHostApplicationIsClosingException)
            {
                // NOTE: The exception occurs from inside of Timer, so the application thread cannot catch it.  EgsDevicesManager.Dispose() is called and then the application exit in the event handler of EgsDevicesManager.Disposing.
                System.Windows.Forms.MessageBox.Show(Resources.CommonStrings_ApplicationWillExit, "ZKOO", System.Windows.Forms.MessageBoxButtons.OK);
                this.Dispose();
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Debug.WriteLine(ex.Message);
            }
        }

        void InitializeDisconnectedEgsDeviceOnSomeDeviceDisconnected()
        {
            try
            {
                // At first it checks HID connection.
                MessageReceivingForm.SetupDi.Update();
                foreach (var device in DeviceList) { device.UpdateHidDeviceConnectionStatus(); }

                // And then it checks camera connection.
                var cameras = GetCameraInterfaceInformationList();
                Debug.WriteLine("cameras.Count: " + cameras.Count);
                foreach (var device in DeviceList)
                {
                    var matchedCameraExists = cameras.Any(e => e.Index == device.CameraViewImageSourceBitmapCapture.VideoCaptureDeviceIndex);
                    if (matchedCameraExists) { continue; }
                    device.SetIsCameraDeviceConnectedToFalse();
                }
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Debug.WriteLine(ex.Message);
            }
        }

        internal EgsDevice CreateNewEgsDeviceAndAddToDeviceList()
        {
            var device = EgsDevice.GetEgsDeviceForEgsDevicesManager(DeviceList.Count);
            DeviceList.Add(device);

            // TODO: MUSTDO: This is ad-hoc implementation.  This way cannot recognize multiple device's connection and disconnection.  This is very bad way, it should be fixed.
            MessageReceivingForm.CurrentDevice = device;

            return device;
        }

        internal void RemoveDeviceFromDeviceList(EgsDevice device)
        {
            if (DeviceList.Contains(device) == false) { return; }
            device.Close();
            DeviceList.Remove(device);
            // TODO: MUSTDO: delete MessageReceivingForm.CurrentDevice itself
            MessageReceivingForm.CurrentDevice = null;
        }

        #region IDisposable
        public event EventHandler Disposing;
        public event EventHandler Disposed;
        protected virtual void OnDisposing(EventArgs e) { var t = Disposing; if (t != null) { t(this, e); } }
        protected virtual void OnDisposed(EventArgs e) { var t = Disposed; if (t != null) { t(this, e); } }
        private bool disposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                // dispose managed objects, and dispose objects that implement IDisposable
                OnDisposing(EventArgs.Empty);

                if (EachDeviceStatusMonitoringTimer != null) { EachDeviceStatusMonitoringTimer.Dispose(); EachDeviceStatusMonitoringTimer = null; }
                if (OnDeviceConnectedDelayTimer != null) { OnDeviceConnectedDelayTimer.Stop(); OnDeviceConnectedDelayTimer.Dispose(); OnDeviceConnectedDelayTimer = null; }
                if (OnDeviceDisconnectedDelayTimer != null) { OnDeviceDisconnectedDelayTimer.Stop(); OnDeviceDisconnectedDelayTimer.Dispose(); OnDeviceDisconnectedDelayTimer = null; }
                if (MessageReceivingForm != null) { MessageReceivingForm.CurrentDevice = null; }
                if (DeviceList != null)
                {
                    foreach (var device in DeviceList) { device.Close(); }
                    DeviceList.Clear();
                    DeviceList = null;
                }
                if (MessageReceivingForm != null) { MessageReceivingForm.Dispose(); MessageReceivingForm = null; }
            }
            // release any unmanaged objects and set the object references to null
            disposed = true;
            OnDisposed(EventArgs.Empty);
        }
        ~EgsDevicesManager() { Dispose(false); }
        #endregion
    }
}

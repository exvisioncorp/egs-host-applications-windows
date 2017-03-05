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
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        internal Win32SetupDiForEgsDevice SetupDi { get; private set; }

        public IList<EgsDevice> DeviceList { get; private set; }

#if false
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
                OnPropertyChanged("TemperatureMonitoringTimerIntervalTotalSeconds");
            }
        }
#endif

        internal EgsDevicesManager()
        {
            DeviceList = new List<EgsDevice>();

            SetupDi = new Win32SetupDiForEgsDevice();

#if false
            EachDeviceStatusMonitoringTimer = new System.Windows.Forms.Timer() { Interval = ApplicationCommonSettings.IsDebugging ? 1000 : 60000 };
            EachDeviceStatusMonitoringTimer.Tick += delegate
            {
                foreach (var device in DeviceList)
                {
                    var isMonitoringTemperature = device.Settings.IsToMonitorTemperature.Value && device.IsHidDeviceConnected;
                    if (isMonitoringTemperature == false) { continue; }
                    device.UpdateTemperatureProperties();
                }
            };
#endif
        }

        internal void UpdateNotInitializedFirstEgsDeviceOnSomeDeviceConnected()
        {
            // TODO: MUSTDO: Test with connecting and re-connecting USB connector, because various problems can occur here.
            try
            {
                // At first it checks HID connection.
                SetupDi.Update();
                foreach (var device in DeviceList) { device.UpdateHidDeviceConnectionStatus(); }

                foreach (var device in DeviceList)
                {
                    device.TrySetIsCameraDeviceConnectedToTrue();
                }
            }
            catch (EgsHostApplicationIsClosingException)
            {
                // NOTE: The exception occurs from inside of Timer, so the application thread cannot catch it.  EgsDevicesManager.Dispose() is called and then the application exit in the event handler of EgsDevicesManager.Disposing.
                Debug.WriteLine(Resources.CommonStrings_ApplicationWillExit);
                this.Dispose();
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

            return device;
        }

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

#if false
                if (EachDeviceStatusMonitoringTimer != null) { EachDeviceStatusMonitoringTimer.Dispose(); EachDeviceStatusMonitoringTimer = null; }
#endif
                if (DeviceList != null)
                {
                    foreach (var device in DeviceList) { device.Close(); }
                    DeviceList.Clear();
                    DeviceList = null;
                }
            }
            // release any unmanaged objects and set the object references to null
            disposed = true;
            OnDisposed(EventArgs.Empty);
        }
        ~EgsDevicesManager() { Dispose(false); }
    }
}

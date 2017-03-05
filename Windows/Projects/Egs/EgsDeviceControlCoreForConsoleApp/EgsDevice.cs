namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.Serialization;
    using System.Runtime.InteropServices;
    using System.Collections.ObjectModel;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    /// <summary>
    /// The instance of EgsDevice class receives information from EGS device and reconstruct current state of the device.  Main roles are 
    /// (1) Monitoring of connection state of the device, and open HID and UVC (camera) devices while it is connected
    /// (2) Querying bitmap images for "Camera View"
    /// (3) Receiving HID gesture information and notifies the updates to other components
    /// (4) Changing settings of the device through "EgsDeviceSettings Settings" object as a member of EgsDevice
    /// </summary>
    [DataContract]
    public partial class EgsDevice : INotifyPropertyChanged
    {
        /// <summary>
        /// Currently the maximum number of objects which EGS device can detect and track is 2.
        /// </summary>
        public int TrackableHandsCountMaximum { get { return 2; } }
        /// <summary>
        /// EGS device can detect 5 faces in one image, and the device selects only one face as a device user.
        /// </summary>
        public int DetectableFacesCountMaximum { get { return 5; } }

        static object lockForNewEgsDevicesManager = new object();
        static EgsDevicesManager _DefaultEgsDevicesManager = null;
        internal static EgsDevicesManager DefaultEgsDevicesManager
        {
            get
            {
                if (_DefaultEgsDevicesManager != null) { return _DefaultEgsDevicesManager; }
                try
                {
                    lock (lockForNewEgsDevicesManager) { _DefaultEgsDevicesManager = new EgsDevicesManager(); }
                    return _DefaultEgsDevicesManager;
                }
                catch (Exception ex)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    Debug.WriteLine(ex);
                    throw;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        internal IList<HidAccessPropertyBase> HidAccessPropertyList { get; private set; }
        internal event EventHandler<HidAccessPropertyUpdatedEventArgs> HidAccessPropertyUpdated;
        internal virtual void OnHidAccessPropertyUpdated(HidAccessPropertyUpdatedEventArgs e)
        {
            var t = HidAccessPropertyUpdated; if (t != null) { t(this, e); }
        }

        /// <summary>
        /// This number depends on "TouchInterfaceKind".  MultiTouch mode returns 0.  Mouse mode and SingleTouch mode return 1.  If settings is not set, it returns 0.
        /// </summary>
        public int TrackableHandsCount
        {
            get
            {
                var ret = (Settings == null) ? 0 : (int)Settings.TrackableHandsCount.Value;
                return ret;
            }
        }

        internal EgsDeviceHidReportsUpdate HidReportsUpdate { get; private set; }

        /// <summary>
        /// HID report for OS.  Just only touch information is contained.  EgsGestureHidReport has more information for applications by this SDK users.
        /// </summary>
        public EgsDeviceTouchScreenHidReport TouchScreenHidReport { get; private set; }

        /// <summary>
        /// HID report for applications by this SDK users.
        /// </summary>
        public EgsDeviceEgsGestureHidReport EgsGestureHidReport { get; private set; }

        Stopwatch HidStopwatch = Stopwatch.StartNew();
        internal int WaitTimeInMillisecondsBeforeSetFeatureReport { get; set; }
        internal void SetHidFeatureReport(byte[] featureReport)
        {
            // NOTE: Keep this method internal for a while.
            if (featureReport == null || featureReport.Length >= 256) { throw new ArgumentException("featureReport is null or too large.", "featureReport"); }
            // TODO (en): I'm not sure this waiting is unnecessary or not.  If this function fails once, it continues to fail after waiting 100[ms], 10[sec].
            // TODO (ja): Setの前の待機は関係ないかも？！一度この関数が失敗すると、100ミリ秒とか10秒とか待機しても失敗している。
            if (WaitTimeInMillisecondsBeforeSetFeatureReport > 0) { while (HidStopwatch.ElapsedMilliseconds <= WaitTimeInMillisecondsBeforeSetFeatureReport) { System.Threading.Thread.Sleep(1); } }
            // NOTE: Exceptions can occur.  But this method does NOT catch them here.
            Win32HidSimpleAccess.SetFeatureReport(HidDeviceDevicePath, featureReport);
            if (WaitTimeInMillisecondsBeforeSetFeatureReport > 0) { HidStopwatch.Reset(); HidStopwatch.Start(); }
        }
        internal int WaitTimeInMillisecondsBeforeGetFeatureReport { get; set; }
        internal void GetHidFeatureReport(byte[] featureReport)
        {
            // NOTE: Keep this method internal for a while.
            if (featureReport == null || featureReport.Length >= 256) { throw new ArgumentException("featureReport is null or too large.", "featureReport"); }
            // TODO: It can be important to wait before Get.
            if (WaitTimeInMillisecondsBeforeGetFeatureReport > 0) { while (HidStopwatch.ElapsedMilliseconds <= WaitTimeInMillisecondsBeforeGetFeatureReport) { System.Threading.Thread.Sleep(1); } }
            // NOTE: Exceptions can occur.  But this method does NOT catch them here.
            Win32HidSimpleAccess.GetFeatureReport(HidDeviceDevicePath, featureReport);
            if (WaitTimeInMillisecondsBeforeGetFeatureReport > 0) { HidStopwatch.Reset(); HidStopwatch.Start(); }
        }

        internal bool CheckHidPropertyVersionAndCurrentFirmwareVersion(HidAccessPropertyBase item)
        {
            if (item.AvailableFirmwareVersion > FirmwareVersionAsVersion)
            {
                var msg = item.Description + " is available >= (" + item.AvailableFirmwareVersion + ").  Current firmware version is (" + FirmwareVersionString + ")";
                Console.WriteLine(msg);
                return false;
            }
            return true;
        }

        readonly int indexOfMessageIdInByteArray = 1;
        readonly byte flagValueWhenStartGetHidReport = 0x01;
        readonly byte flagValueWhenErrorOccurs = 0x80;
        internal bool SetHidAccessPropertyBySetHidFeatureReport(HidAccessPropertyBase item)
        {
            try
            {
                if (FirmwareVersionAsVersion != new Version(0, 0, 0, 0))
                {
                    if (CheckHidPropertyVersionAndCurrentFirmwareVersion(item) == false) { return false; }
                }
                SetHidFeatureReport(item.ByteArrayData);
                if ((item.ByteArrayData[indexOfMessageIdInByteArray] & flagValueWhenErrorOccurs) != 0) { return false; }
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return false;
            }
        }
        internal bool GetReadonlyHidAccessPropertyByGetHidFeatureReport(HidAccessPropertyBase item)
        {
            try
            {
                if (FirmwareVersionAsVersion != new Version(0, 0, 0, 0))
                {
                    if (CheckHidPropertyVersionAndCurrentFirmwareVersion(item) == false) { return false; }
                }
                // NOTE: About "Readonly" properties, the host reads the settings saved in a device.
                // NOTE: TODO: The next line is important!  And have to check if the place of such code should be written here or not.
                item.ByteArrayData[indexOfMessageIdInByteArray] = flagValueWhenStartGetHidReport;
                SetHidFeatureReport(item.ByteArrayData);
                if ((item.ByteArrayData[indexOfMessageIdInByteArray] & flagValueWhenErrorOccurs) != 0) { return false; }
                // TODO: Check necessity.
                if (true) { System.Threading.Thread.Sleep(20); }
                GetHidFeatureReport(item.ByteArrayData);
                if ((item.ByteArrayData[indexOfMessageIdInByteArray] & flagValueWhenErrorOccurs) != 0) { return false; }
                item.RaiseValueUpdatedOnGetHidFeatureReport();
                return true;
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return false;
            }
        }

        #region Temperature
        System.IO.StreamWriter TemperatureStreamWriter { get; set; }
        DateTime StartTime { get; set; }
        void CloseTemperatureStreamWriter()
        {
            if (TemperatureStreamWriter != null)
            {
                TemperatureStreamWriter.Flush();
                TemperatureStreamWriter.Close();
                TemperatureStreamWriter = null;
            }
        }
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToWriteLogOfTemperature = false;
        [EditorBrowsable(EditorBrowsableState.Never)]
        public bool IsToWriteLogOfTemperature
        {
            get { return _IsToWriteLogOfTemperature; }
            set
            {
                _IsToWriteLogOfTemperature = value;
                CloseTemperatureStreamWriter();

                if (_IsToWriteLogOfTemperature)
                {
                    var desktopPath = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
                    var zkooTestResultFolderPath = System.IO.Path.Combine(desktopPath, @"ZkooTestResults");
                    if (System.IO.Directory.Exists(zkooTestResultFolderPath) == false)
                    {
                        System.IO.Directory.CreateDirectory(zkooTestResultFolderPath);
                    }
                    var fileName = @"ZkooDeviceTemperature_";
                    fileName += DateTime.Now.ToString("yyMMdd-HHmmss", CultureInfo.InvariantCulture);
                    fileName += ".csv";
                    var fullPath = System.IO.Path.Combine(zkooTestResultFolderPath, fileName);
                    TemperatureStreamWriter = new System.IO.StreamWriter(fullPath);
                    StartTime = DateTime.Now;
                    TemperatureStreamWriter.WriteLine("DateTime.Now, Elapsed[sec], Temperature[C], Temperature[F}");
                }
                OnPropertyChanged("IsToWriteLogOfTemperature");
            }
        }
        #endregion

        /// <summary>
        /// Please use this method insted of "new EgsDevice()".
        /// </summary>
        public static EgsDevice GetDefaultEgsDevice()
        {
            var ret = DefaultEgsDevicesManager.CreateNewEgsDeviceAndAddToDeviceList();
            DefaultEgsDevicesManager.UpdateNotInitializedFirstEgsDeviceOnSomeDeviceConnected();
            return ret;
        }
        [Obsolete]
        public static EgsDevice GetDefaultEgsDevice(EgsDeviceSettings settings)
        {
            var ret = GetDefaultEgsDevice();
            ret.SetSettings(settings);
            return ret;
        }
        /// <summary>
        /// Device should be closed by this method, when applications exit.
        /// </summary>
        public static void CloseDefaultEgsDevice()
        {
            DefaultEgsDevicesManager.Dispose();
        }
        internal static EgsDevice GetEgsDeviceForEgsDevicesManager(int indexInHidDevicePathList)
        {
            var ret = new EgsDevice();
            ret.InitializeOnceAtStartup();
            ret.IndexInHidDevicePathList = indexInHidDevicePathList;
            return ret;
        }
        /// <summary>
        /// This class object is not deserializable, and should be created by EgsDevicesManager.
        /// </summary>
        internal EgsDevice()
        {
            _IndexInHidDevicePathList = -1;
            _HidDeviceDevicePath = "";
            _IsHidDeviceConnected = false;

            _IsConnected = false;

            _IsUpdatingFirmware = false;

            _IsDetectingFaces = false;
            _IsDetectingHands = false;
            _IsSendingTouchScreenHidReport = false;
            _IsSendingHoveringStateOnTouchScreenHidReport = false;
            _IsSendingEgsGestureHidReport = false;

            WaitTimeInMillisecondsBeforeSetFeatureReport = 2;
            // TODO: MUSTDO: tune !!  For stability of 960x540, it should be larger, but for firmware update, it should be smaller.  (2016/3/3)
            //WaitTimeInMillisecondsBeforeSetFeatureReport = 20;
            WaitTimeInMillisecondsBeforeGetFeatureReport = 10;

            CreateProperties();

            HidReportsUpdate = new EgsDeviceHidReportsUpdate();
            TouchScreenHidReport = new EgsDeviceTouchScreenHidReport();
            EgsGestureHidReport = new EgsDeviceEgsGestureHidReport();

            IsHidDeviceConnectedChanged += UpdateIsHidDeviceConnectedRelatedProperties;

            _Settings = new EgsDeviceSettings();
            _Settings.InitializeOnceAtStartup();
            _Settings.CurrentConnectedEgsDevice = this;
            _Settings.HidAccessPropertyUpdated += EgsDeviceSettings_HidAccessPropertyUpdated;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        EgsDeviceSettings _Settings;
        /// <summary>
        /// The current settings of this device.
        /// </summary>
        [DataMember]
        public EgsDeviceSettings Settings
        {
            get { return _Settings; }
            private set { SetSettings(value); }
        }

        [Obsolete]
        public void SetSettings(EgsDeviceSettings value)
        {
            Trace.Assert(value != null);

            if (_Settings != null)
            {
                _Settings.HidAccessPropertyUpdated -= EgsDeviceSettings_HidAccessPropertyUpdated;
                _Settings.CurrentConnectedEgsDevice = null;
                _Settings = null;
            }
            value.CurrentConnectedEgsDevice = this;
            value.HidAccessPropertyUpdated += EgsDeviceSettings_HidAccessPropertyUpdated;
            _Settings = value;

            // NOTE: When device is connected and then settings is updated, the app sets the settings from host to device.
            if (IsHidDeviceConnected)
            {
                SetAllSettingsToDeviceAndReadStatusFromDevice();
            }
            TouchScreenHidReport.Reset();
            EgsGestureHidReport.Reset();
        }

        internal void InitializeOnceAtStartup()
        {
            if (HidReportsUpdate != null) { HidReportsUpdate.InitializeOnceAtStartup(this); }
            TouchScreenHidReport.InitializeOnceAtStartup(this);
            EgsGestureHidReport.InitializeOnceAtStartup(this);

            AddPropertiesToHidAccessPropertyList();
            InitializePropertiesByDefaultValue();

            TemperatureInCelsius.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => TemperatureInCelsiusString)); };
            TemperatureInFahrenheit.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => TemperatureInFahrenheitString)); };
        }

        public event EventHandler HidReportObjectsReset;
        protected virtual void OnHidReportObjectsReset(EventArgs e)
        {
            var t = HidReportObjectsReset; if (t != null) { t(null, e); }
        }

        /// <summary>
        /// In some Android device or some applications, host application is not allowed to run on the device.  
        /// But the device can send mouse or touch report only by the device (without host), in that case, the default settings are load from the flash in the device.
        /// With this method, host application can send a command to save the current settings (saved and managed in host) into the flash in the device.
        /// </summary>
        public void SaveSettingsToFlash()
        {
            if (IsHidDeviceConnected == false)
            {
                Console.WriteLine(Resources.CommonStrings_PleaseConnectTheZkooCamera);
                return;
            }
            SetHidFeatureReport(HostToDeviceCommandFeatureReport.SaveSettingsToFlashCommandFeatureReport.ByteArrayData);
        }

        /// <summary>
        /// Send a reset command to connected Device.
        /// </summary>
        public void ResetDevice()
        {
            if (IsHidDeviceConnected == false)
            {
                Console.WriteLine(Resources.CommonStrings_PleaseConnectTheZkooCamera);
                return;
            }
            SetHidFeatureReport(HostToDeviceCommandFeatureReport.ResetDeviceCommandFeatureReport.ByteArrayData);
        }

        /// <summary>
        /// Reset TouchScreenHidReport and EgsGestureHidReport properties.
        /// </summary>
        public void ResetHidReportObjects()
        {
            if (TouchScreenHidReport == null) { Debugger.Break(); }
            if (EgsGestureHidReport == null) { Debugger.Break(); }
            TouchScreenHidReport.Reset();
            EgsGestureHidReport.Reset();
            OnHidReportObjectsReset(EventArgs.Empty);
        }

        public void ResetSettings()
        {
            Settings.Reset();
            //FaceDetectionOnHost.Reset();
            ResetHidReportObjects();
        }

        /// <summary>
        /// Sorry but this is internal.  But for DataBinding, it is public.  Only in newer device hardware can measure temperature.
        /// </summary>
        [EditorBrowsable(EditorBrowsableState.Never)]
        public void UpdateTemperatureProperties()
        {
            if (IsHidDeviceConnected == false) { Debugger.Break(); throw new EgsDeviceOperationException("IsHidDeviceConnected == false"); }
            GetReadonlyHidAccessPropertyByGetHidFeatureReport(TemperatureInCelsius);
            GetReadonlyHidAccessPropertyByGetHidFeatureReport(TemperatureInFahrenheit);
            if (IsToWriteLogOfTemperature)
            {
                Trace.Assert(TemperatureStreamWriter != null);
                TemperatureStreamWriter.WriteLine("{0}, {1}, {2}, {3}",
                    DateTime.Now,
                    (DateTime.Now - StartTime).TotalSeconds,
                    TemperatureInCelsius.Value,
                    TemperatureInFahrenheit.Value);
                TemperatureStreamWriter.Flush();
            }
        }

        void SetAllSettingsToDeviceAndReadStatusFromDevice()
        {
            // NOTE: It does not get or set the value of properties, in updating Firmware.
            if (IsUpdatingFirmware) { return; }

            if (Settings == null) { Debugger.Break(); throw new EgsDeviceOperationException("Settings == null"); }
            if (IsHidDeviceConnected == false) { Debugger.Break(); throw new EgsDeviceOperationException("IsHidDeviceConnected == false"); }

            // NOTE: At first, it sets USB protocol revision.
            if (SetHidAccessPropertyBySetHidFeatureReport(Settings.UsbProtocolRevision) == false)
            {
                if (ApplicationCommonSettings.IsDebuggingInternal) { Debugger.Break(); }
                throw new EgsDeviceOperationException("[EgsDevice.SetUsbProtocolVersionAndGetFirmwareVersion()] Failed to set UsbProtocolRevision");
            }
            // NOTE: And then, it gets firmware version.
            if (GetReadonlyHidAccessPropertyByGetHidFeatureReport(FirmwareVersion) == false)
            {
                if (ApplicationCommonSettings.IsDebuggingInternal) { Debugger.Break(); }
                Console.WriteLine("[EgsDevice.SetUsbProtocolVersionAndGetFirmwareVersion()] Failed to get " + FirmwareVersion.Description);
            }
            // NOTE: It sets TouchInterfaceKind, which can cause various different behaviors.
            if (SetHidAccessPropertyBySetHidFeatureReport(Settings.TouchInterfaceKind) == false)
            {
                if (ApplicationCommonSettings.IsDebuggingInternal) { Debugger.Break(); }
                throw new EgsDeviceOperationException("[EgsDevice.SetUsbProtocolVersionAndGetFirmwareVersion()] Failed to set UsbProtocolRevision");
            }

            // NOTE: And then it sets or gets properties, based on the current firmware version in the device.
            var version = FirmwareVersionAsVersion;
            foreach (var item in Settings.HidAccessPropertyList)
            {
                if (CheckHidPropertyVersionAndCurrentFirmwareVersion(item) == false) { continue; }
                // NOTE: About settable properties, the app sets the value from host to device (not from device to host).
                if (item.IsReadOnly)
                {
                    var hr = GetReadonlyHidAccessPropertyByGetHidFeatureReport(item);
                    if (hr == false)
                    {
                        Console.WriteLine("[Failed! to Get] " + item.Description);
                    }
                }
                else
                {
                    var hr = SetHidAccessPropertyBySetHidFeatureReport(item);
                    if (hr == false)
                    {
                        Console.WriteLine("[Failed! to Set] " + item.Description);
                    }
                }
            }

            foreach (var item in HidAccessPropertyList)
            {
                if (item.IsReadOnly != true) { Debugger.Break(); }
                if (CheckHidPropertyVersionAndCurrentFirmwareVersion(item) == false) { continue; }
                var hr = GetReadonlyHidAccessPropertyByGetHidFeatureReport(item);
                if (hr == false) { Console.WriteLine("[Failed! to Get] " + item.Description); }
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string _HidDeviceDevicePath;
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsHidDeviceConnected;
        public event EventHandler HidDeviceDevicePathChanged;
        public event EventHandler IsHidDeviceConnectedChanged;
        protected virtual void OnHidDeviceDevicePathChanged(EventArgs e)
        {
            var t = HidDeviceDevicePathChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("HidDeviceDevicePath");
        }
        protected virtual void OnIsHidDeviceConnectedChanged(EventArgs e)
        {
            var t = IsHidDeviceConnectedChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsHidDeviceConnected");
        }
        /// <summary>
        /// The "DevicePath" of the HID device of the connected device.
        /// </summary>
        public string HidDeviceDevicePath { get { return _HidDeviceDevicePath; } }
        public bool IsHidDeviceConnected { get { return _IsHidDeviceConnected; } }

        /// <summary>
        /// Call this method about all registered devices, after calling MessageReceivingForm.SetupDi.Update(); in EgsDevicesManagers
        /// </summary>
        internal void UpdateHidDeviceConnectionStatus()
        {
            // TODO: It should be able to use multiple devices, even if "ProductId" is changed in future.  If the app checks them, the place may be here.
            var newDevicePath = DefaultEgsDevicesManager.SetupDi.GetHidDevicePath(IndexInHidDevicePathList);
            if (string.IsNullOrEmpty(newDevicePath))
            {
                _HidDeviceDevicePath = "";
                _IsHidDeviceConnected = false;
            }
            else
            {
                _HidDeviceDevicePath = newDevicePath;
                HidReportsUpdate.Start(_HidDeviceDevicePath);
                _IsHidDeviceConnected = true;
                SetAllSettingsToDeviceAndReadStatusFromDevice();
            }

            OnHidDeviceDevicePathChanged(EventArgs.Empty);
            OnIsHidDeviceConnectedChanged(EventArgs.Empty);
        }

        /// <summary>
        /// This method should be called only from EgsDevicesManager.  This updates only connection state of "Camera".
        /// </summary>
        internal void TrySetIsCameraDeviceConnectedToTrue()
        {
            Debug.WriteLine("[EgsDevice] TrySetIsCameraDeviceConnectedToTrue() called.  DateTime.Now = " + DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss.fff", CultureInfo.InvariantCulture));

            // NOTE (en): We confirmed there is time lag, from setting "CameraViewImageSourceBitmapSize" to becoming that the app can get correct values related to the "CameraViewImageSourceBitmapSize".
            // NOTE (ja): PCの機種によっては？？カメラの解像度設定から、表示領域の内部状態が更新されてHIDで取得できるようになるまで、タイムラグがあることを確認した！！
            System.Threading.Thread.Sleep(300);

            // MUSTDO: Check if the application works correctly or not, if the next block is activated.  Currently it works correctly.
            if (true)
            {
                System.Threading.Thread.Sleep(300);
                // NOTE: Just in case, I leave the code to get the value again.
                GetReadonlyHidAccessPropertyByGetHidFeatureReport(Settings.CaptureImageSize);
                System.Threading.Thread.Sleep(100);
                GetReadonlyHidAccessPropertyByGetHidFeatureReport(Settings.CameraViewImageSourceRectInCaptureImage);
            }
        }

        internal void Close()
        {
            // NOTE: Stop gesture recognition, if the host application is not running.
            if (Settings != null)
            {
                Settings.IsToDetectFacesOnDevice.Value = false;
                // NOTE: If the firmware version is larger than 1.1, stopping hand detection changes the LED color from blue to red.
                Settings.IsToDetectHandsOnDevice.Value = false;
            }
            if (HidReportsUpdate != null)
            {
                HidReportsUpdate.OnDisable();
                HidReportsUpdate = null;
            }
        }

        static internal EgsDevice CreateEgsDeviceForXamlDesign()
        {
            var ret = new EgsDevice();
            ret.InitializeOnceAtStartup();
            ret.IndexInHidDevicePathList = 0;
            ret.DeviceSerialNumber.Value = "EXV000000005";
            ret._HidDeviceDevicePath = "THISISSOMEHIDDEVICEPATH";
            ret._IsHidDeviceConnected = true;
            ret.IsConnected = true;
            return ret;
        }
    }

    [Serializable]
    public sealed class EgsDeviceOperationException : Exception
    {
        public EgsDeviceOperationException(string message) : base(message) { }
    }
}

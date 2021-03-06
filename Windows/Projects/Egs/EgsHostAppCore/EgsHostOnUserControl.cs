﻿namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Windows;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.Views;
    using Egs.PropertyTypes;
    using Egs.EgsDeviceControlCore.Properties;

    /// <summary>
    /// The minimum set of EGS host application, which can run on some UserControl or Window.
    /// </summary>
    [DataContract]
    public partial class EgsHostOnUserControl : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        #region Host Settings
        // NOTE: Currently disabled
        //[DataMember]
        public CultureInfoAndDescriptionOptions CultureInfoAndDescription { get; private set; }
        [DataMember]
        public MouseCursorPositionUpdatedByGestureCursorMethodOptions MouseCursorPositionUpdatedByGestureCursorMethod { get; private set; }
        [DataMember]
        public CursorDrawingTimingMethodOptions CursorDrawingTimingMethod { get; private set; }
        [DataMember]
        public CameraViewBordersAndPointersAreDrawnByOptions CameraViewBordersAndPointersAreDrawnBy { get; private set; }
        #endregion

        [DataMember]
        public EgsDevice Device { get; private set; }
        // TODO: better implementation
        [DataMember]
        public CameraViewUserControlModel CameraViewUserControlModel { get; private set; }
        [DataMember]
        public OnePersonBothHandsViewModel OnePersonBothHandsViewModel { get; private set; }
        [DataMember]
        public long DrawingCursorsMinimumIntervalInMilliseconds { get; set; }
        [DataMember]
        public TimeSpan WaitTimeTillMouseCursorHideOnMouseMode { get; set; }

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
                    var zkooTestResultFolderPath = System.IO.Path.Combine(desktopPath, ApplicationCommonSettings.HostApplicationName + "TestResults");
                    if (System.IO.Directory.Exists(zkooTestResultFolderPath) == false)
                    {
                        System.IO.Directory.CreateDirectory(zkooTestResultFolderPath);
                    }
                    var fileName = ApplicationCommonSettings.HostApplicationName + "DeviceTemperature_";
                    fileName += DateTime.Now.ToString("yyMMdd-HHmmss", CultureInfo.InvariantCulture);
                    fileName += ".csv";
                    var fullPath = System.IO.Path.Combine(zkooTestResultFolderPath, fileName);
                    TemperatureStreamWriter = new System.IO.StreamWriter(fullPath);
                    StartTime = DateTime.Now;
                    TemperatureStreamWriter.WriteLine("DateTime.Now, Elapsed[sec], Temperature[C], Temperature[F}");
                }
                OnPropertyChanged(nameof(IsToWriteLogOfTemperature));
            }
        }
        #endregion

        Stopwatch drawingCursorsStopwatch { get; set; }
        public IList<CursorForm> CursorViews { get; protected set; }
        internal TimerPrecisionLogger PrecisionLogger { get; private set; }

        // NOTE: These commands are sent to device.  So they can be put in "EgsDeviceControlCore".  But the namespace of ICommand is System.Windows.Input of WPF, so they are written here.
        public SimpleDelegateCommand ResetSettingsCommand { get; private set; }
        public SimpleDelegateCommand SaveSettingsToFlashCommand { get; private set; }
        public SimpleDelegateCommand ResetDeviceCommand { get; private set; }
        public SimpleDelegateCommand SendManySettingsPacketsCommand { get; private set; }

        public bool CanSaveSettingsJsonFileSafely
        {
            get
            {
                if (Device == null
                    || Device.FaceDetectionOnHost == null
                    || Device.Settings == null
                    || CameraViewUserControlModel == null
                    || OnePersonBothHandsViewModel == null)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    return false;
                }
                return true;
            }
        }

        internal virtual void RaiseMultipleObjectsPropertyChanged()
        {
            OnPropertyChanged(nameof(Device));
            OnPropertyChanged(nameof(CameraViewUserControlModel));
            CameraViewUserControlModel.RaiseMultipleObjectsPropertyChanged();
            OnPropertyChanged(nameof(OnePersonBothHandsViewModel));
        }

        public EgsHostOnUserControl()
        {
            Device = EgsDevice.GetDefaultEgsDevice();

            CultureInfoAndDescription = new CultureInfoAndDescriptionOptions();
            MouseCursorPositionUpdatedByGestureCursorMethod = new MouseCursorPositionUpdatedByGestureCursorMethodOptions();
            CursorDrawingTimingMethod = new CursorDrawingTimingMethodOptions();
            CameraViewBordersAndPointersAreDrawnBy = new CameraViewBordersAndPointersAreDrawnByOptions();

            CameraViewUserControlModel = new CameraViewUserControlModel();
            OnePersonBothHandsViewModel = new OnePersonBothHandsViewModel();
            CursorViews = new List<CursorForm>();
            DrawingCursorsMinimumIntervalInMilliseconds = 8;
            drawingCursorsStopwatch = Stopwatch.StartNew();
            PrecisionLogger = new TimerPrecisionLogger();


            //CultureInfoAndDescription.SelectSingleItemByPredicate(e => e.CultureInfoString == ApplicationCommonSettings.DefaultCultureInfoName);
            MouseCursorPositionUpdatedByGestureCursorMethod.Value = MouseCursorPositionUpdatedByGestureCursorMethods.None;
            CursorDrawingTimingMethod.Value = CursorDrawingTimingMethods.ByHidReportUpdatedEvent;
            CameraViewBordersAndPointersAreDrawnBy.Value = CameraViewBordersAndPointersAreDrawnByKind.HostApplication;
            WaitTimeTillMouseCursorHideOnMouseMode = TimeSpan.FromSeconds(10);

            ResetSettingsCommand = new SimpleDelegateCommand();
            SaveSettingsToFlashCommand = new SimpleDelegateCommand();
            ResetDeviceCommand = new SimpleDelegateCommand();
            SendManySettingsPacketsCommand = new SimpleDelegateCommand();

            ResetSettingsCommand.CanPerform = true;
            SaveSettingsToFlashCommand.CanPerform = false;
            ResetDeviceCommand.CanPerform = false;
            SendManySettingsPacketsCommand.CanPerform = false;

            ResetSettingsCommand.PerformEventHandler += delegate
            {
                if (MessageBox.Show(Resources.CommonStrings_ResetSettingsConfirmation, Resources.CommonStrings_Confirmation, MessageBoxButton.OKCancel) != MessageBoxResult.OK) { return; }
                Reset();
            };
            SaveSettingsToFlashCommand.PerformEventHandler += delegate
            {
                if (MessageBox.Show(Resources.CommonStrings_SaveSettingsToFlashConfirmation, Resources.CommonStrings_Confirmation, MessageBoxButton.OKCancel) != MessageBoxResult.OK) { return; }
                Device.SaveSettingsToFlash();
            };
            ResetDeviceCommand.PerformEventHandler += delegate
            {
                if (MessageBox.Show(Resources.CommonStrings_ResetDeviceConfirmation, Resources.CommonStrings_Confirmation, MessageBoxButton.OKCancel) != MessageBoxResult.OK) { return; }
                Device.ResetDevice();
            };
            SendManySettingsPacketsCommand.PerformEventHandler += delegate
            {
                SendManySettingsPacketsAsync();
            };

            Device.IsHidDeviceConnectedChanged += Device_IsHidDeviceConnectedChanged;

            // NOTE: The next 2 lines are necessary.
            SaveSettingsToFlashCommand.CanPerform = Device.IsHidDeviceConnected;
            ResetDeviceCommand.CanPerform = Device.IsHidDeviceConnected;
            SendManySettingsPacketsCommand.CanPerform = Device.IsHidDeviceConnected;

            CursorDrawingTimingMethod.ValueUpdated += delegate { OnCursorDrawingTimingMethod_SelectedItemChanged(); };
            CameraViewBordersAndPointersAreDrawnBy.ValueUpdated += CameraViewBordersAndPointersAreDrawnBy_SelectedItemChanged;
            CultureInfoAndDescription.ValueUpdated += delegate { BindableResources.Current.ChangeCulture(CultureInfoAndDescription.Value); };

            // TODO: MUSTDO: When users cancel DFU, exceptions occur in Timer thread basically,
            // and the app cannot deal with them.
            // So it is necessary to attach them to event handlers.
            // The design of EgsDevicesManager is not completed.  So I don't think this is good way.
            EgsDevice.DefaultEgsDevicesManager.Disposing += delegate
            {
                if (false && ApplicationCommonSettings.IsDebuggingInternal) { Debugger.Break(); }
                this.Dispose();
            };
        }

        bool isSendingManySettingsPackets = false;
        async void SendManySettingsPacketsAsync()
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                if (isSendingManySettingsPackets)
                {
                    MessageBox.Show("The application is still doing previous task (sending many settings packets).");
                    return;
                }
                isSendingManySettingsPackets = true;
                var elapsed = Stopwatch.StartNew();
                int i = 0;
                for (i = 0; i < 10000; i++)
                {
                    if (disposed) { return; }
                    if (elapsed.ElapsedMilliseconds > 1000)
                    {
                        MessageBox.Show("Elapsed > 1000[ms].  The application will stop this test.");
                        break;
                    }
                    if (Device == null)
                    {
                        MessageBox.Show("Device == null.  The application will stop this test.");
                        break;
                    }
                    if (Device.CameraViewImageSourceBitmapCapture == null)
                    {
                        MessageBox.Show("Device.CameraViewImageSourceBitmapCapture == null.  The application will stop this test.");
                        break;
                    }
                    if (Device.CameraViewImageSourceBitmapCapture.IsRestartingUvc)
                    {
                        MessageBox.Show("UVC is restarting.  The application will stop this test.");
                        break;
                    }
                    Device.Settings.StatusLedBrightnessMaximum.Value = (byte)((i * 5) % 256);
                    elapsed.Restart();
                }
                if (i == 10000)
                {
                    MessageBox.Show("Completed sending many settings packets!");
                }
                isSendingManySettingsPackets = false;
            });
        }

        public virtual void InitializeOnceAtStartup()
        {
            Device.EgsGestureHidReport.ReportUpdated += Device_EgsGestureHidReport_ReportUpdated;

            if (false)
            {
                // NOTE: Even if it is Mouse mode, the application should draw not the OS protocol (TouchScreenHidRpoert) but the vendor-specific protocol (EgsGestureHidReport)!
                Device.TouchScreenHidReport.ReportUpdated += delegate
                {
                    if (Device.Settings.TouchInterfaceKind.Value == TouchInterfaceKinds.Mouse) { OnDeviceTouchScreenHidReportReportUpdated(); }
                };
            }

            Device.HidReportObjectsReset += Device_HidReportObjectsReset;
            Device.TemperaturePropertiesUpdated += TemperatureInCelsius_TemperaturePropertiesUpdated;

            InitializeCursorModelsAndCursorViews();
        }

        void TemperatureInCelsius_TemperaturePropertiesUpdated(object sender, EventArgs e)
        {
            if (IsToWriteLogOfTemperature)
            {
                Trace.Assert(TemperatureStreamWriter != null);
                TemperatureStreamWriter.WriteLine("{0}, {1}, {2}, {3}",
                    DateTime.Now,
                    (DateTime.Now - StartTime).TotalSeconds,
                    Device.TemperatureInCelsius.Value,
                    Device.TemperatureInFahrenheit.Value);
                TemperatureStreamWriter.Flush();
            }
        }

        void CameraViewBordersAndPointersAreDrawnBy_SelectedItemChanged(object sender, EventArgs e)
        {
            switch (CameraViewBordersAndPointersAreDrawnBy.Value)
            {
                case CameraViewBordersAndPointersAreDrawnByKind.HostApplication:
                    CameraViewUserControlModel.IsToDrawImageSet = true;
                    if (Device.Settings != null)
                    {
                        Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
                    }
                    break;
                case CameraViewBordersAndPointersAreDrawnByKind.Device:
                    CameraViewUserControlModel.IsToDrawImageSet = false;
                    if (Device.Settings != null)
                    {
                        Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = true;
                    }
                    break;
                default:
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
            }
        }

        void Device_IsHidDeviceConnectedChanged(object sender, EventArgs e)
        {
            SaveSettingsToFlashCommand.CanPerform = Device.IsHidDeviceConnected;
            ResetDeviceCommand.CanPerform = Device.IsHidDeviceConnected;
            SendManySettingsPacketsCommand.CanPerform = Device.IsHidDeviceConnected;
        }
        void Device_EgsGestureHidReport_ReportUpdated(object sender, EventArgs e)
        {
            OnDeviceEgsGestureHidReportReportUpdated();
        }
        void Device_HidReportObjectsReset(object sender, EventArgs e)
        {
            OnDeviceEgsGestureHidReportReportUpdated();
            // NOTE: Some people can use OS protocol, so I enable the next line.
            OnDeviceTouchScreenHidReportReportUpdated();
            // NOTE: The next 2 lines are necessary.  When a device is disconnected, they clear Borders and Pointers and so on in the Camera View.
            CameraViewUserControlModel.Reset();
            RaiseMultipleObjectsPropertyChanged();
        }

        void OnCursorDrawingTimingMethod_SelectedItemChanged()
        {
            var value = CursorDrawingTimingMethod.Value;
            switch (value)
            {
                case CursorDrawingTimingMethods.ByHidReportUpdatedEvent:
                    DrawingCursorsMinimumIntervalInMilliseconds = 1000 / 120;
                    break;
                case CursorDrawingTimingMethods.ByTimer60Fps:
                    DrawingCursorsMinimumIntervalInMilliseconds = 1000 / 60;
                    break;
                case CursorDrawingTimingMethods.ByTimer30Fps:
                    DrawingCursorsMinimumIntervalInMilliseconds = 1000 / 30;
                    break;
                default:
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
            }
            drawingCursorsStopwatch.Reset(); drawingCursorsStopwatch.Start();
        }

        public event EventHandler HasResetSettings;
        protected virtual void OnResetSettings(EventArgs e)
        {
            var t = HasResetSettings; if (t != null) { t(this, e); }
        }

        /// <summary>
        /// Call ResetSettings() and then call OnResetSettings()
        /// </summary>
        public void Reset()
        {
            ResetSettings();
            OnResetSettings(EventArgs.Empty);
        }

        protected virtual void ResetSettings()
        {
            // TODO: This is very old memo and it says "Do it at first", but I forgot the reason.
            Device.ResetSettings();
            // TODO: Very old memo says "Do not change!", should check the reason and test it again.
            //CultureInfoAndDescription.SelectedIndex = 0;
            CameraViewBordersAndPointersAreDrawnBy.Value = CameraViewBordersAndPointersAreDrawnByKind.HostApplication;
            MouseCursorPositionUpdatedByGestureCursorMethod.Value = MouseCursorPositionUpdatedByGestureCursorMethods.None;
            CursorDrawingTimingMethod.Value = CursorDrawingTimingMethods.ByHidReportUpdatedEvent;
            OnePersonBothHandsViewModel.CursorImageSetInformationOptionalValue.SelectedIndex = 0;
        }

        protected virtual void InitializeCursorModelsAndCursorViews()
        {
            for (int i = 0; i < Device.TrackableHandsCountMaximum; i++) { CursorViews.Add(new CursorForm()); }
            CameraViewUserControlModel.InitializeOnceAtStartup(Device);
            OnePersonBothHandsViewModel.InitializeOnceAtStartup(Device);
            CursorViews[0].InitializeOnceAtStartup(OnePersonBothHandsViewModel.RightHand, ImageInformationSet.CreateDefaultRightCursorImageInformationSetList());
            CursorViews[1].InitializeOnceAtStartup(OnePersonBothHandsViewModel.LeftHand, ImageInformationSet.CreateDefaultLeftCursorImageInformationSetList());
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        [Obsolete("Please use OnDeviceEgsGestureHidReportReportUpdated() instead.  This method will be deleted in next SDK.", true)]
        protected virtual void OnDeviceTouchReportUpdate()
        {
            // call from an obsolete method "OnDeviceTouchReportUpdate"
            OnDeviceEgsGestureHidReportReportUpdated();
        }

        protected virtual void OnDeviceEgsGestureHidReportReportUpdated()
        {
            if (PrecisionLogger.IsToUseTimerPrecisionMonitor) { PrecisionLogger.CallBeforeUpdating(OnePersonBothHandsViewModel.RightHand.IsTouching ? "1" : "0"); }

            // NOTE: In multi-touch mode, when a hand "touches" once, the mouse cursor disappear, and the position of the mouse cursor becomes not to change!
            // Some applications can use MouseMove event, so mouse cursor position is important.
            // So the application can let the mouse cursor track (first found / right / left) gesture cursor. 
            // MUSTDO: In some PCs, the position is not updated to the correct value.   Should be fixed.
            if (MouseCursorPositionUpdatedByGestureCursorMethod.Value != MouseCursorPositionUpdatedByGestureCursorMethods.None
                && Device.Settings.TouchInterfaceKind.Value != TouchInterfaceKinds.Mouse
                && OnePersonBothHandsViewModel != null)
            {
                CursorViewModel hand = null;
                switch (MouseCursorPositionUpdatedByGestureCursorMethod.Value)
                {
                    case MouseCursorPositionUpdatedByGestureCursorMethods.FirstFoundHand:
                        hand = OnePersonBothHandsViewModel.FirstFoundHand;
                        break;
                    case MouseCursorPositionUpdatedByGestureCursorMethods.RightHand:
                        hand = OnePersonBothHandsViewModel.RightHand;
                        break;
                    case MouseCursorPositionUpdatedByGestureCursorMethods.LeftHand:
                        hand = OnePersonBothHandsViewModel.LeftHand;
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException();
                }
                // NOTE: In face recognition mode, OnePersonBothHandsViewModel.FirstFoundHand is null.
                if (hand != null && hand.IsTracking)
                {
                    // NOte: In this code, position adjustment by DPI is NOT necessary.
                    System.Windows.Forms.Cursor.Position = new System.Drawing.Point((int)hand.PositionX, (int)hand.PositionY);
                }
            }

            // NOTE: In mouse mode, it must use not TrackableHandsCount but TrackableHandsCountMaximum to draw it in the correct position.
            for (int i = 0; i < Device.TrackableHandsCountMaximum; i++)
            {
                OnePersonBothHandsViewModel.Hands[i].UpdateByEgsGestureHidReportHand(Device.EgsGestureHidReport.Hands[i]);
            }
            if (drawingCursorsStopwatch.ElapsedMilliseconds > DrawingCursorsMinimumIntervalInMilliseconds)
            {
                // NOTE: It also must use Maximum.
                for (int i = 0; i < Device.TrackableHandsCountMaximum; i++)
                {
                    CursorViews[i].UpdatePosition();
                }
                drawingCursorsStopwatch.Reset(); drawingCursorsStopwatch.Start();
            }
            if (false && ApplicationCommonSettings.IsToEmulateReportByActualMouseRawInputToDebugViews)
            {
                var m = Device.EgsGestureHidReport.Hands[0];
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Model: {0} {1} {2} {3}", m.IsTracking, m.X, m.Y, m.IsTouching));
                var vm = OnePersonBothHandsViewModel.Hands[0];
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "ViewModel: {0} {1} {2} {3}", vm.IsTracking, vm.PositionX, vm.PositionY, vm.IsTouching));
            }

            // NOTE: Before, the application hides cursor after a while to display desktop without large gesture cursor.
            // But now, in mouse mode, a device sends information about tracking by EgsGestureHidReport, so the application just needs to draw the report.
            // NOTE: Even if the device is disconnected, the report object is also reset, so the cursor disappears correctly.
            if (PrecisionLogger.IsToUseTimerPrecisionMonitor) { PrecisionLogger.CallAfterUpdated(); }
        }

        protected virtual void OnDeviceTouchScreenHidReportReportUpdated()
        {
            // NOTE: It does not need to use TouchScreenHidReport (report used by OS).  Just draw images by EgsGestureHidRepoert (report for applications).
            // But some people can want to use TouchScreenHidReport, so I don't delete this method.
            for (int i = 0; i < Device.TrackableHandsCount; i++)
            {
                OnePersonBothHandsViewModel.Hands[i].UpdateByTouchScreenHidReportContact(Device.TouchScreenHidReport.Contacts[i]);
            }
            if (drawingCursorsStopwatch.ElapsedMilliseconds > DrawingCursorsMinimumIntervalInMilliseconds)
            {
                for (int i = 0; i < Device.TrackableHandsCount; i++) { CursorViews[i].UpdatePosition(); }
                drawingCursorsStopwatch.Reset(); drawingCursorsStopwatch.Start();
            }
            if (Device.Settings.TouchInterfaceKind.Value == TouchInterfaceKinds.Mouse)
            {
                if (DateTime.Now - Device.LastUpdateTime > WaitTimeTillMouseCursorHideOnMouseMode)
                {
                    OnePersonBothHandsViewModel.Hands[0].IsVisible = false;
                    CursorViews[0].UpdatePosition();
                }
            }
        }

        protected virtual void CloseCursorViews()
        {
            PrecisionLogger.ExportLog();
            foreach (var cursorView in CursorViews)
            {
                if (cursorView != null) { cursorView.Close(); }
            }
        }

        #region IDisposable
        public event EventHandler Disposing;
        public event EventHandler Disposed;
        protected virtual void OnDisposing(EventArgs e) { var t = Disposing; if (t != null) { t(this, e); } }
        protected virtual void OnDisposed(EventArgs e) { var t = Disposed; if (t != null) { t(this, e); } }
        protected bool hasOnDisposingCalled = false;
        private bool disposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                if (hasOnDisposingCalled == false)
                {
                    hasOnDisposingCalled = true;
                    OnDisposing(EventArgs.Empty);
                    if (disposed) { return; }
                }

                // dispose managed objects, and dispose objects that implement IDisposable
                isSendingManySettingsPackets = false;

                Device.IsHidDeviceConnectedChanged -= Device_IsHidDeviceConnectedChanged;
                Device.EgsGestureHidReport.ReportUpdated -= Device_EgsGestureHidReport_ReportUpdated;
                Device.HidReportObjectsReset -= Device_HidReportObjectsReset;

                CameraViewUserControlModel = null;
                OnePersonBothHandsViewModel = null;

                EgsDevice.DefaultEgsDevicesManager.Dispose();
                CloseCursorViews();
            }
            disposed = true;
            OnDisposed(EventArgs.Empty);
        }
        ~EgsHostOnUserControl() { Dispose(false); }
        #endregion
    }
}

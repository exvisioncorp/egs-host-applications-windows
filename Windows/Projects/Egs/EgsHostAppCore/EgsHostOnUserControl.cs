namespace Egs
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
        // TODO: NOTE: DataMember is not described.  But have to think again.
        public OptionalValue<CultureInfoAndDescriptionDetail> CultureInfoAndDescription { get; private set; }
        [DataMember]
        public OptionalValue<MouseCursorPositionUpdatedByGestureCursorMethodDetail> MouseCursorPositionUpdatedByGestureCursorMethod { get; private set; }
        [DataMember]
        public OptionalValue<CursorDrawingTimingMethodDetail> CursorDrawingTimingMethod { get; private set; }
        [DataMember]
        public OptionalValue<CameraViewBordersAndPointersAreDrawnByDetail> CameraViewBordersAndPointersAreDrawnBy { get; private set; }
        #endregion

        [DataMember]
        public EgsDevice Device { get; private set; }

        public EgsDeviceSettings DeviceSettings { get; private set; }


        // TODO: better implementation
        [DataMember]
        public CameraViewUserControlModel CameraViewUserControlModel { get; private set; }
        [DataMember]
        public OnePersonBothHandsViewModel OnePersonBothHandsViewModel { get; private set; }
        [DataMember]
        public long DrawingCursorsMinimumIntervalInMilliseconds { get; set; }
        [DataMember]
        public TimeSpan WaitTimeTillMouseCursorHideOnMouseMode { get; set; }

        Stopwatch drawingCursorsStopwatch { get; set; }
        public IList<CursorForm> CursorViews { get; protected set; }
        internal TimerPrecisionLogger PrecisionLogger { get; private set; }

        // NOTE: These commands are sent to device.  So they can be put in "EgsDeviceControlCore".  But the namespace of ICommand is System.Windows.Input of WPF, so they are written here.
        public SimpleDelegateCommand ResetSettingsCommand { get; private set; }
        public SimpleDelegateCommand SaveSettingsToFlashCommand { get; private set; }
        public SimpleDelegateCommand ResetDeviceCommand { get; private set; }


        internal virtual void RaiseMultipleObjectsPropertyChanged()
        {
            OnPropertyChanged("Device");
            OnPropertyChanged("CameraViewUserControlModel");
            CameraViewUserControlModel.RaiseMultipleObjectsPropertyChanged();
            OnPropertyChanged("OnePersonBothHandsViewModel");
        }

        public EgsHostOnUserControl()
        {
            CultureInfoAndDescription = new OptionalValue<CultureInfoAndDescriptionDetail>();
            MouseCursorPositionUpdatedByGestureCursorMethod = new OptionalValue<MouseCursorPositionUpdatedByGestureCursorMethodDetail>();
            CursorDrawingTimingMethod = new OptionalValue<CursorDrawingTimingMethodDetail>();
            CameraViewBordersAndPointersAreDrawnBy = new OptionalValue<CameraViewBordersAndPointersAreDrawnByDetail>();

            DeviceSettings = new EgsDeviceSettings();
            CameraViewUserControlModel = new CameraViewUserControlModel();
            OnePersonBothHandsViewModel = new OnePersonBothHandsViewModel();
            CursorViews = new List<CursorForm>();
            DrawingCursorsMinimumIntervalInMilliseconds = 8;
            drawingCursorsStopwatch = Stopwatch.StartNew();
            PrecisionLogger = new TimerPrecisionLogger();


            CultureInfoAndDescription.Options = CultureInfoAndDescriptionDetail.GetDefaultList();
            MouseCursorPositionUpdatedByGestureCursorMethod.Options = MouseCursorPositionUpdatedByGestureCursorMethodDetail.GetDefaultList();
            CursorDrawingTimingMethod.Options = CursorDrawingTimingMethodDetail.GetDefaultList();
            CameraViewBordersAndPointersAreDrawnBy.Options = CameraViewBordersAndPointersAreDrawnByDetail.GetDefaultList();

            //CultureInfoAndDescription.SelectSingleItemByPredicate(e => e.CultureInfoString == ApplicationCommonSettings.DefaultCultureInfoName);
            MouseCursorPositionUpdatedByGestureCursorMethod.SelectedIndex = 0;
            CursorDrawingTimingMethod.SelectedIndex = 0;
            CameraViewBordersAndPointersAreDrawnBy.SelectedIndex = 0;
            WaitTimeTillMouseCursorHideOnMouseMode = TimeSpan.FromSeconds(10);

            ResetSettingsCommand = new SimpleDelegateCommand();
            SaveSettingsToFlashCommand = new SimpleDelegateCommand();
            ResetDeviceCommand = new SimpleDelegateCommand();

            ResetSettingsCommand.CanPerform = true;
            SaveSettingsToFlashCommand.CanPerform = false;
            ResetDeviceCommand.CanPerform = false;

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

            // TODO: MUSTDO: When users cancel DFU, exceptions occur in Timer thread basically, and the app cannot deal with them.  So it is necessary to attach them to event handlers.
            // The design of EgsDevicesManager is not completed.  So I don't think this is good way.
            EgsDevice.DefaultEgsDevicesManager.Disposed += delegate
            {
                if (false && ApplicationCommonSettings.IsDebuggingInternal) { Debugger.Break(); }
                this.Dispose();
            };
        }

        public virtual void InitializeOnceAtStartup()
        {
            DeviceSettings.InitializeOnceAtStartup();
            Device = EgsDevice.GetDefaultEgsDevice(DeviceSettings);

            InitializeCursorModelsAndCursorViews();

            Device.IsHidDeviceConnectedChanged += Device_IsHidDeviceConnectedChanged;

            // NOTE: The next 2 lines are necessary.
            SaveSettingsToFlashCommand.CanPerform = Device.IsHidDeviceConnected;
            ResetDeviceCommand.CanPerform = Device.IsHidDeviceConnected;

            Device.EgsGestureHidReport.ReportUpdated += Device_EgsGestureHidReport_ReportUpdated;

#if false
            // NOTE: Even if it is Mouse mode, the application should draw not the OS protocol (TouchScreenHidRpoert) but the vendor-specific protocol (EgsGestureHidReport)!
            Device.TouchScreenHidReport.ReportUpdated += delegate
            {
                if (DeviceSettings.TouchInterfaceKind.OptionalValue.SelectedItem.EnumValue == EgsDeviceTouchInterfaceKind.Mouse) { OnDeviceTouchScreenHidReportReportUpdated(); }
            };
#endif

            Device.HidReportObjectsReset += Device_HidReportObjectsReset;

            CursorDrawingTimingMethod.SelectedItemChanged += delegate { OnCursorDrawingTimingMethod_SelectedItemChanged(); };

            CameraViewBordersAndPointersAreDrawnBy.SelectedItemChanged += delegate
            {
                switch (CameraViewBordersAndPointersAreDrawnBy.SelectedItem.EnumValue)
                {
                    case CameraViewBordersAndPointersAreDrawnByKind.HostApplication:
                        CameraViewUserControlModel.IsToDrawImageSet = true;
                        if (DeviceSettings != null)
                        {
                            DeviceSettings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
                        }
                        break;
                    case CameraViewBordersAndPointersAreDrawnByKind.Device:
                        CameraViewUserControlModel.IsToDrawImageSet = false;
                        if (DeviceSettings != null)
                        {
                            DeviceSettings.IsToDrawBordersOnCameraViewImageByDevice.Value = true;
                        }
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException();
                }
            };

            CultureInfoAndDescription.SelectedItemChanged += delegate
            {
                BindableResources.Current.ChangeCulture(CultureInfoAndDescription.SelectedItem.CultureInfoString);
            };
        }

        void Device_IsHidDeviceConnectedChanged(object sender, EventArgs e)
        {
            SaveSettingsToFlashCommand.CanPerform = Device.IsHidDeviceConnected;
            ResetDeviceCommand.CanPerform = Device.IsHidDeviceConnected;
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
            var value = CursorDrawingTimingMethod.SelectedItem.EnumValue;
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

        public virtual void Reset()
        {
            // TODO: This is very old memo and it says "Do it at first", but I forgot the reason.
            Device.ResetSettings();
            // TODO: Very old memo says "Do not change!", should check the reason and test it again.
            //CultureInfoAndDescription.SelectedIndex = 0;
            CameraViewBordersAndPointersAreDrawnBy.SelectedIndex = 0;
            MouseCursorPositionUpdatedByGestureCursorMethod.SelectedIndex = 0;
            CursorDrawingTimingMethod.SelectedIndex = 0;
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
            if (MouseCursorPositionUpdatedByGestureCursorMethod.SelectedItem.EnumValue != MouseCursorPositionUpdatedByGestureCursorMethods.None
                && DeviceSettings.TouchInterfaceKind.OptionalValue.SelectedItem.EnumValue != EgsDeviceTouchInterfaceKind.Mouse
                && OnePersonBothHandsViewModel != null)
            {
                CursorViewModel hand = null;
                switch (MouseCursorPositionUpdatedByGestureCursorMethod.SelectedItem.EnumValue)
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
            if (Device.Settings.TouchInterfaceKind.OptionalValue.SelectedItem.EnumValue == EgsDeviceTouchInterfaceKind.Mouse)
            {
                if (DateTime.Now - Device.LastUpdateTime > WaitTimeTillMouseCursorHideOnMouseMode)
                {
                    CursorViews[0].Hide();
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
                if (hasOnDisposingCalled == false) { OnDisposing(EventArgs.Empty); hasOnDisposingCalled = true; }
                Device.IsHidDeviceConnectedChanged -= Device_IsHidDeviceConnectedChanged;
                Device.EgsGestureHidReport.ReportUpdated -= Device_EgsGestureHidReport_ReportUpdated;
                Device.HidReportObjectsReset -= Device_HidReportObjectsReset;
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

namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Windows.Media.Animation;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;

    /// <summary>
    /// ViewModel of CameraViewWindow.  When you use CameraViewWindow,
    /// (1) Create an object of this ViewModel
    /// (2) Initialize it by (EgsDevice device)
    /// (3) Send it to CameraViewWindow.InitializeOnceAtStartup() as an argument
    /// </summary>
    [DataContract]
    public partial class CameraViewWindowModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        /// <summary>Reference to an EgsDevice object.  Users have to set this as an argument of InitializeOnceAtStartup() method.</summary>
        public EgsDevice Device { get; private set; }

        [EditorBrowsable(EditorBrowsableState.Never)]
        WindowState _WindowState;
        public event EventHandler WindowStateChanged;
        protected virtual void OnWindowStateChanged(EventArgs e)
        {
            var t = WindowStateChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(WindowState));
            OnPropertyChanged(nameof(IsNormalOrElseMinimized));
        }
        public WindowState WindowState
        {
            get { return _WindowState; }
            set
            {
                if (value == WindowState.Maximized)
                {
                    // TODO: The next line can cause exception?
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    _WindowState = WindowState.Normal;
                    OnWindowStateChanged(EventArgs.Empty);
                    SetWindowStateToMinimizedDelayTimer.Stop();
                    return;
                }
                WindowState newWindowState;
                switch (WindowStateHostApplicationsControlMethod.Value)
                {
                    case CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods:
                        newWindowState = value;
                        break;
                    case CameraViewWindowStateHostApplicationsControlMethods.KeepNormal:
                        newWindowState = WindowState.Normal;
                        break;
                    case CameraViewWindowStateHostApplicationsControlMethods.KeepMinimized:
                        newWindowState = WindowState.Minimized;
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException("WindowStateHostApplicationsControlMethod is invalid.");
                }
                if (_WindowState != newWindowState)
                {
                    _WindowState = newWindowState;
                    OnWindowStateChanged(EventArgs.Empty);
                    SetWindowStateToMinimizedDelayTimer.Stop();
                }
            }
        }
        public bool IsNormalOrElseMinimized
        {
            get
            {
                switch (_WindowState)
                {
                    case WindowState.Normal:
                        return true;
                        break;
                    case WindowState.Minimized:
                        return false;
                        break;
                    case WindowState.Maximized:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new InvalidOperationException("_WindowState == WindowState.Maximized");
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new InvalidOperationException("_WindowState == unknown");
                        break;
                }
            }
            set
            {
                WindowState = value ? WindowState.Normal : WindowState.Minimized;
            }
        }
        public void ToggleWindowStateControlMethodOnAutoOrOff()
        {
            IsNormalOrElseMinimized = !IsNormalOrElseMinimized;
        }

        System.Windows.Threading.DispatcherTimer SetWindowStateToMinimizedDelayTimer { get; set; }
        [DataMember]
        public TimeSpan SetWindowStateToMinimizedDelayTimerInterval
        {
            get { return SetWindowStateToMinimizedDelayTimer.Interval; }
            set
            {
                SetWindowStateToMinimizedDelayTimer.Interval = value;
                OnPropertyChanged(nameof(SetWindowStateToMinimizedDelayTimerInterval));
            }
        }

        public bool IsDragging { get; internal set; }
        public bool IsMouseHovered { get; internal set; }

        public CameraViewWindowStateHostApplicationsControlMethodOptions WindowStateHostApplicationsControlMethod { get; private set; }
        [DataMember]
        public CameraViewWindowStateUsersControlMethodOptions WindowStateUsersControlMethod { get; private set; }
        public SimpleDelegateCommand MinimizeCommand { get; private set; }
        public SimpleDelegateCommand SettingsCommand { get; private set; }
        public SimpleDelegateCommand ExitCommand { get; private set; }

        public CameraViewWindowModel()
        {
            _WindowState = WindowState.Normal;
            SetWindowStateToMinimizedDelayTimer = new System.Windows.Threading.DispatcherTimer();
            SetWindowStateToMinimizedDelayTimer.Interval = TimeSpan.FromMilliseconds(3000);
            SetWindowStateToMinimizedDelayTimer.Tick += (sender, e) =>
            {
                if (IsMouseHovered)
                {
                    // restart
                    SetWindowStateToMinimizedDelayTimer.Stop();
                    SetWindowStateToMinimizedDelayTimer.Start();
                }
                else
                {
                    IsNormalOrElseMinimized = false;
                }
            };

            IsDragging = false;
            IsMouseHovered = false;
            WindowStateHostApplicationsControlMethod = new CameraViewWindowStateHostApplicationsControlMethodOptions();
            WindowStateUsersControlMethod = new CameraViewWindowStateUsersControlMethodOptions();

            MinimizeCommand = new SimpleDelegateCommand();
            SettingsCommand = new SimpleDelegateCommand();
            ExitCommand = new SimpleDelegateCommand();

            _CanDragMove = ApplicationCommonSettings.IsDebugging;
            _CanResize = ApplicationCommonSettings.IsDebugging;
            _Topmost = !(ApplicationCommonSettings.IsDebugging);
            _CanShowMenu = true;

            WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods;
            WindowStateUsersControlMethod.Value = CameraViewWindowStateUsersControlMethods.ManualOnOff;

            WindowStateHostApplicationsControlMethod.ValueUpdated += delegate { StartCheckingIsShowingCameraViewWindow(); };
            WindowStateUsersControlMethod.ValueUpdated += delegate { StartCheckingIsShowingCameraViewWindow(); };

            MinimizeCommand.PerformEventHandler += delegate { ToggleWindowStateControlMethodOnAutoOrOff(); };
        }

        public void InitializeOnceAtStartup(EgsDevice device)
        {
            Trace.Assert(device != null);
            Device = device;

            LocationAndSize = GetDefaultLocationAndSize();

            // NOTE: Thie is CameraView Window's Visibility.  This does not mean if the app captures images from device or not.
            Device.EgsGestureHidReport.RecognitionStateChanged += EgsGestureHidReport_RecognitionStateChanged;
        }

        public void Reset()
        {
            WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods;
            WindowStateUsersControlMethod.Value = CameraViewWindowStateUsersControlMethods.ManualOnOff;

            CanDragMove = ApplicationCommonSettings.IsDebugging;
            CanResize = ApplicationCommonSettings.IsDebugging;
            Topmost = !(ApplicationCommonSettings.IsDebugging);
            CanShowMenu = true;

            WindowState = WindowState.Normal;

            LocationAndSize = GetDefaultLocationAndSize();
        }

        [DataMember]
        public System.Windows.Rect LocationAndSize
        {
            get { return new Rect(Left, Top, Width, Height); }
            set
            {
                var dpi = Dpi.DpiFromHdcForTheEntireScreen;
                var screens = System.Windows.Forms.Screen.AllScreens;
                bool isInsideAnyScreen = screens.Any(e => e.Bounds.ToWpfRect().Contains(value));
                if (isInsideAnyScreen == false)
                {
                    value = GetDefaultLocationAndSize();
                }

                _Left = value.X;
                _Top = value.Y;
                _Width = value.Width;
                _Height = value.Height;
                OnLeftChanged(EventArgs.Empty);
                OnTopChanged(EventArgs.Empty);
                OnWidthChanged(EventArgs.Empty);
                OnHeightChanged(EventArgs.Empty);
            }
        }

        public void ResetLocationAndSizeIfNotInsideAnyScreen()
        {
            LocationAndSize = LocationAndSize;
        }

        public Rect GetDefaultLocationAndSize()
        {
            // MUSTDO: test with changing DPI, because it can change the position of Camera View.
            // System.Window.Rectangle is struct, so it is difficult to use it in Binding.
            if (Device.Settings == null) { Debugger.Break(); throw new EgsDeviceOperationException("Device.Settings == null"); }

            var wVal = Device.Settings.CameraViewImageSourceBitmapSize.SelectedItem.Width + 10;
            var hVal = Device.Settings.CameraViewImageSourceBitmapSize.SelectedItem.Height + 12;

            var dpi = Dpi.DpiFromHdcForTheEntireScreen;
            var primaryScreen = dpi.GetScaledRectangle(System.Windows.Forms.Screen.PrimaryScreen.Bounds);
            var xVal = (int)(primaryScreen.Width - wVal - primaryScreen.Width / 40);
            var yVal = primaryScreen.Height / 10;

            return new Rect(xVal, yVal, wVal, hVal);
        }

        public void SetWindowStateToNormal()
        {
            if (IsNormalOrElseMinimized != true) { IsNormalOrElseMinimized = true; }
        }
        public void SetWindowStateToMinimizedWithDelay()
        {
            SetWindowStateToMinimizedDelayTimer.Start();
        }

        public void StartCheckingIsShowingCameraViewWindow()
        {
            SetWindowStateToMinimizedDelayTimer.Stop();

            switch (WindowStateHostApplicationsControlMethod.Value)
            {
                case CameraViewWindowStateHostApplicationsControlMethods.KeepMinimized:
                    WindowState = WindowState.Minimized;
                    return;
                case CameraViewWindowStateHostApplicationsControlMethods.KeepNormal:
                    WindowState = WindowState.Normal;
                    return;
                case CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods:
                    break;
                default:
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
            }

            // NOTE: "WindowState = WindowState.Normal" means that the app shows Camera View.   The app shows Camera View to notify that users can change the state manually.
            if (WindowStateUsersControlMethod.Value == CameraViewWindowStateUsersControlMethods.ManualOnOff)
            {
                WindowState = WindowState.Normal;
                return;
            }

            if (WindowStateUsersControlMethod.Value == CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart
                || WindowStateUsersControlMethod.Value == CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideWhenHandTrackingEnd)
            {
                SetWindowStateToMinimizedWithDelay();
            }

            // NOTE: Device state is not changed, just host application settings is changed by users or applications.  So it assumes typical state transition.
            if (Device.IsDetectingFaces) { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces)); }
            else if (Device.IsDetectingHands) { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands)); }
            else if (Device.IsTrackingOneOrMoreHands) { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands)); }
            else { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.AnyStates_StandingBy)); }
        }

        void EgsGestureHidReport_RecognitionStateChanged(object sender, EgsGestureHidReportRecognitionStateChangedEventArgs e)
        {
            if (Device.EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.Unknown)
            {
                Debug.WriteLine("EgsGestureHidReportMessageIds.Unknown");
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            if (Device.EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.StandingBy)
            {
                Debug.WriteLine("EgsGestureHidReportMessageIds.StandingBy");
                if (WindowStateUsersControlMethod.Value != CameraViewWindowStateUsersControlMethods.ManualOnOff)
                {
                    SetWindowStateToMinimizedWithDelay();
                }
                return;
            }

            // NOTE: When users are watching some contents (ex. video) and they does not move their body, Camera View should be turned off.
            // NOTE: If the Camera View blinks, doubt that the state can change backwards and forwards between StandingBy mode and DetectingFaces mode.
            // NOTE: Not only the current state but the latest transition is used, because "When X end" needs the previous state.
            // NOTE: "DetectingFaces_DetectingHands" means "check if user is starting gesture or not" AND "Face is detected, but user is not doing gesture".
            // NOTE: "WhenHandDetectionEnd" means "face detection restarts or tracking is started".


            Console.Write(DateTime.Now.ToString("[yyyy/MM/dd HH:mm:ss.fff] "));
            Console.WriteLine(e.TransitionType);

            switch (WindowStateUsersControlMethod.Value)
            {
                case CameraViewWindowStateUsersControlMethods.ManualOnOff:
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); throw new NotImplementedException(); }
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            break;
                        default:
                            if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); throw new NotImplementedException(); }
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); throw new NotImplementedException(); }
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideWhenHandTrackingEnd:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); throw new NotImplementedException(); }
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); throw new NotImplementedException(); }
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideWhenHandTrackingEnd:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            break;
                        default:
                            if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); throw new NotImplementedException(); }
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingByOrChangedSettingsByDevice_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); throw new NotImplementedException(); }
                            break;
                    }
                    break;
                default:
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
            }
        }
    }
}

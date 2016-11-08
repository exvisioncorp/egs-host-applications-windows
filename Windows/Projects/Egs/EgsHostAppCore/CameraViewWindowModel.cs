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
            OnPropertyChanged("WindowState");
            OnPropertyChanged("IsNormalOrElseMinimized");
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
                switch (WindowStateHostApplicationsControlMethod.SelectedItem.EnumValue)
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
                OnPropertyChanged("SetWindowStateToMinimizedDelayTimerInterval");
            }
        }

        public bool IsDragging { get; internal set; }
        public bool IsMouseHovered { get; internal set; }
        public OptionalValue<CameraViewWindowStateHostApplicationsControlMethodDetail> WindowStateHostApplicationsControlMethod { get; private set; }
        [DataMember]
        public OptionalValue<CameraViewWindowStateUsersControlMethodDetail> WindowStateUsersControlMethod { get; private set; }
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
            WindowStateHostApplicationsControlMethod = new OptionalValue<CameraViewWindowStateHostApplicationsControlMethodDetail>();
            WindowStateUsersControlMethod = new OptionalValue<CameraViewWindowStateUsersControlMethodDetail>();

            MinimizeCommand = new SimpleDelegateCommand();
            SettingsCommand = new SimpleDelegateCommand();
            ExitCommand = new SimpleDelegateCommand();

            _CanDragMove = ApplicationCommonSettings.IsDebugging;
            _CanResize = ApplicationCommonSettings.IsDebugging;
            _Topmost = !(ApplicationCommonSettings.IsDebugging);
            _CanShowMenu = true;


            WindowStateHostApplicationsControlMethod.Options = CameraViewWindowStateHostApplicationsControlMethodDetail.GetDefaultList();
            WindowStateUsersControlMethod.Options = CameraViewWindowStateUsersControlMethodDetail.GetDefaultList();
            WindowStateHostApplicationsControlMethod.SelectSingleItemByPredicate(e => e.EnumValue == CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods);
            WindowStateUsersControlMethod.SelectSingleItemByPredicate(e => e.EnumValue == CameraViewWindowStateUsersControlMethods.ManualOnOff);

            WindowStateHostApplicationsControlMethod.SelectedItemChanged += delegate
            {
                SetWindowStateToMinimizedDelayTimer.Stop();
                // NOTE: "WindowState = WindowState.Normal" means that the app shows Camera View.   The app shows Camera View to notify that users can change the state manually.
                if (WindowStateUsersControlMethod.SelectedItem.EnumValue == CameraViewWindowStateUsersControlMethods.ManualOnOff) { WindowState = WindowState.Normal; }
            };
            WindowStateUsersControlMethod.SelectedItemChanged += delegate
            {
                SetWindowStateToMinimizedDelayTimer.Stop();
                if (WindowStateUsersControlMethod.SelectedItem.EnumValue == CameraViewWindowStateUsersControlMethods.ManualOnOff) { WindowState = WindowState.Normal; }
            };
            MinimizeCommand.PerformEventHandler += delegate { ToggleWindowStateControlMethodOnAutoOrOff(); };
        }

        public void InitializeOnceAtStartup(EgsDevice device)
        {
            Trace.Assert(device != null);
            Device = device;

            UpdateWindowLocationAndSize();

            // NOTE: Thie is CameraView Window's Visibility.  This does not mean if the app captures images from device or not.
            Device.EgsGestureHidReport.RecognitionStateChanged += EgsGestureHidReport_RecognitionStateChanged;
        }

        public void Reset()
        {
            WindowStateHostApplicationsControlMethod.SelectSingleItemByPredicate(e => e.EnumValue == CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods);
            WindowStateUsersControlMethod.SelectSingleItemByPredicate(e => e.EnumValue == CameraViewWindowStateUsersControlMethods.ManualOnOff);

            CanDragMove = ApplicationCommonSettings.IsDebugging;
            CanResize = ApplicationCommonSettings.IsDebugging;
            Topmost = !(ApplicationCommonSettings.IsDebugging);
            CanShowMenu = true;

            WindowStateHostApplicationsControlMethod.SelectSingleItemByPredicate(e => e.EnumValue == CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods);
            WindowStateUsersControlMethod.SelectSingleItemByPredicate(e => e.EnumValue == CameraViewWindowStateUsersControlMethods.ManualOnOff);
            WindowState = WindowState.Normal;

            UpdateWindowLocationAndSize();
        }

        void UpdateWindowLocationAndSize()
        {
            var rect = Device.Settings.GetCameraViewWindowRectByDefaultValue();
            Left = rect.X;
            Top = rect.Y;
            Width = rect.Width;
            Height = rect.Height;
        }

        public void SetWindowStateToNormal()
        {
            if (IsNormalOrElseMinimized != true) { IsNormalOrElseMinimized = true; }
        }
        public void SetWindowStateToMinimizedWithDelay()
        {
            SetWindowStateToMinimizedDelayTimer.Start();
        }

        bool IsDetectingFaces { get { return Device.EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.DetectingFaces; } }
        bool IsDetectingHands { get { return (Device.EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.DetectingOrTrackingHands) && (IsTrackingOneOrMoreHands == false); } }
        bool IsTrackingOneOrMoreHands { get { return (Device.EgsGestureHidReport.Hands[0].IsTracking || Device.EgsGestureHidReport.Hands[1].IsTracking); } }

        public void StartCheckingIsShowingCameraViewWindow()
        {
            if (WindowStateUsersControlMethod.SelectedItem.EnumValue == CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart
                || WindowStateUsersControlMethod.SelectedItem.EnumValue == CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideWhenHandTrackingEnd)
            {
                SetWindowStateToMinimizedWithDelay();
            }

            if (IsDetectingFaces) { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.StandingBy_DetectingFaces)); }
            else if (IsDetectingHands) { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands)); }
            else if (IsTrackingOneOrMoreHands) { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands)); }
            else { EgsGestureHidReport_RecognitionStateChanged(null, new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_StandingBy)); }
        }
        void EgsGestureHidReport_RecognitionStateChanged(object sender, EgsGestureHidReportRecognitionStateChangedEventArgs e)
        {
            if (Device.EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.Unknown)
            {
                Debug.WriteLine("EgsGestureHidReportMessageIds.Unknown");
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            // TODO: When users are watching some videos and they does not move their body, Camera View should be turned off.
            // NOTE: If the Camera View blinks, doubt that the state can change backwards and forwards between StandingBy mode and DetectingFaces mode.
            if (Device.EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.StandingBy)
            {
                Debug.WriteLine("EgsGestureHidReportMessageIds.StandingBy");
                if (false) { SetWindowStateToMinimizedWithDelay(); }
                return;
            }

            if (WindowStateUsersControlMethod.SelectedItem.EnumValue == CameraViewWindowStateUsersControlMethods.ManualOnOff)
            {
                return;
            }

            switch (WindowStateUsersControlMethod.SelectedItem.EnumValue)
            {
                case CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingBy_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            Debug.WriteLine("SetWindowStateToNormal");
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingBy_DetectingFaces:
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
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.StandingBy_DetectingFaces:
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
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideWhenHandTrackingEnd:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideWhenHandTrackingEnd:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            break;
                    }
                    break;
                case CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart:
                    switch (e.TransitionType)
                    {
                        case EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands:
                            SetWindowStateToNormal();
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        case EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces:
                            SetWindowStateToMinimizedWithDelay();
                            break;
                        default:
                            break;
                    }
                    break;
                default:
                    throw new NotImplementedException();
            }
        }
    }
}

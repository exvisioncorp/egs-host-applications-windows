namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;
    using Egs.Views;

    /// <summary>
    /// State of touch, or kind of tap (Unknown, Touching, Tap, LongTap)
    /// </summary>
    public enum CursorTapKind
    {
        /// <summary>Touch state or tap kind is unknown.</summary>
        Unknown,
        /// <summary>Users are touching now, so Tap Gesture is not occurred yet.</summary>
        Touching,
        /// <summary>Users bended and opened their hands, so Tap Gesture is recognized, not LongTap.</summary>
        Tap,
        /// <summary>Users bended their hands for a while, and then opened the hand.  So LongTap Gesture is recognized.</summary>
        LongTap,
    }

    /// <summary>
    /// Index of kind of a cursor image
    /// </summary>
    public enum CursorImageIndexLabels
    {
        /// <summary>Index is None (-1).</summary>
        None = -1,
        /// <summary>The image of gesture cursor should be opened hand.  The index value in some image array is 0.</summary>
        OpenHand = 0,
        /// <summary>The image of gesture cursor should be closed hand.  The index value in some image array is 1.</summary>
        CloseHand = 1,
    }

    /// <summary>
    /// This class is used to draw a "Gesture Cursor".  Initialize method must be called.
    /// In the event handler of "EgsDevice.EgsGestureHidReport.ReportUpdated" event,
    /// the UpdateByTouchScreenHidReportContact(EgsDeviceTouchScreenHidReportContact contact) method should be called.
    /// </summary>
    public partial class CursorViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        EgsDevice Device { get; set; }

        internal Stopwatch ElapsedFromLastTouched { get; set; }
        public double ElapsedFromLastTouchedInMilliseconds { get { return ElapsedFromLastTouched.ElapsedMilliseconds; } }

        public event EventHandler StateUpdated;
        public event EventHandler IsTrackingChanged;
        public event EventHandler IsTouchingChanged;
        public event EventHandler IsVisibleChanged;
        protected virtual void OnStateUpdated(EventArgs e) { var t = StateUpdated; if (t != null) { t(this, e); } }
        protected virtual void OnIsTrackingChanged(EventArgs e) { var t = IsTrackingChanged; if (t != null) { t(this, e); } }
        protected virtual void OnIsTouchingChanged(EventArgs e) { var t = IsTouchingChanged; if (t != null) { t(this, e); } }
        protected virtual void OnIsVisibleChanged(EventArgs e) { var t = IsVisibleChanged; if (t != null) { t(this, e); } }

        internal VelocityFilter[] Velocities { get; private set; }

        public CursorViewModel()
        {
            _RightOrLeft = RightOrLeftKind.Unknown;
            _IsToShowCursor = true;
            _IsToDetectLongTouch = true;
            _IsToUpdateVelocities = false;
            _LongTapElapsedThresholdInMilliseconds = 800.0f;

            _IsTracking = false;
            _IsVisible = false;
            // NOTE: Maybe this is ok.  If these values are initialized by 0, gesture cursors appears on the left top corner of the primary screen unexpectedly.  It may cause various problems.
            _PositionX = short.MinValue;
            _PositionY = short.MinValue;
            _Rotation = 0.0;
            _RelativeZ = 1.0;
            _FingerPitch = 0.0;
            _IsTouching = false;
            _IsLongTouching = false;
            _LastTapKind = CursorTapKind.Unknown;
            _CurrentImageIndex = (int)CursorImageIndexLabels.OpenHand;

            ElapsedFromLastTouched = new Stopwatch();
            Velocities = new VelocityFilter[(int)CursorViewModelVelocityIndexes.Count];
            for (int i = 0; i < Velocities.Length; i++)
            {
                Velocities[i] = new VelocityFilter() { FramesPerSecond = 100.0 };
            }
            Velocities[(int)CursorViewModelVelocityIndexes.PositionX].XDotEmaCoefficient = 0.7;
            Velocities[(int)CursorViewModelVelocityIndexes.PositionY].XDotEmaCoefficient = 0.7;
            Velocities[(int)CursorViewModelVelocityIndexes.Rotation].XDotEmaCoefficient = 0.5;
            Velocities[(int)CursorViewModelVelocityIndexes.RelativeZ].XDotEmaCoefficient = 0.5;
            Velocities[(int)CursorViewModelVelocityIndexes.FingerPitch].XDotEmaCoefficient = 0.3;
            Velocities[(int)CursorViewModelVelocityIndexes.XYSpeed].XDotEmaCoefficient = 0.3;
            SetFalseToAllHasToCallPeropertyChangedFields();
        }

        /// <summary>
        /// This method must be called after an object of EgsDevice is constructed.
        /// </summary>
        public virtual void InitializeOnceAtStartup(EgsDevice device)
        {
            Trace.Assert(device != null);
            Device = device;
            IsToShowCursorChanged += delegate
            {
                if (IsToShowCursor == false) { InitializePropertiesDependsOnHidReport(); }
            };
            CallPropertyChangedOfAllPropertiesAtOnce();
            OnStateUpdated(EventArgs.Empty);
        }

        void InitializePropertiesDependsOnHidReport()
        {
            IsTracking = false;
            IsVisible = false;
            PositionX = short.MinValue;
            PositionY = short.MinValue;
            Rotation = 0.0;
            RelativeZ = 1.0;
            FingerPitch = 0.0;
            IsTouching = false;
            IsLongTouching = false;
            LastTapKind = CursorTapKind.Unknown;

            // NOTE(en): Check.  If the next line is "CurrentImageIndex = (int)CursorImageKind.None;", the state is "None" until the first touch happens, even if it is hovering.
            // NOTE(ja): 要確認。次の行が"CurrentImageIndex = (int)CursorImageKind.None;"という記述だと、最初のTouchが行われるまで、HoverしていてもNoneのままになってしまう。
            CurrentImageIndex = (int)CursorImageIndexLabels.OpenHand;

            ElapsedFromLastTouched.Reset();

            CallPropertyChangedOfAllPropertiesAtOnce();
            OnStateUpdated(EventArgs.Empty);
        }


        void OnIsTouchingChangedInternal()
        {
            if (IsTouching)
            {
                ElapsedFromLastTouched.Reset();
                ElapsedFromLastTouched.Start();
                LastTapKind = CursorTapKind.Touching;
            }
            else
            {
                ElapsedFromLastTouched.Stop();
                // NOTE: LastTapKind is "Last" TapKind, so it keeps the state, till IsTouching becomes true or tracking ends.
                if (IsToDetectLongTouch && IsLongTouching)
                {
                    LastTapKind = CursorTapKind.LongTap;
                }
                else
                {
                    LastTapKind = CursorTapKind.Tap;
                }
                IsLongTouching = false;
            }
            // NOTE: Left CursorView and right CursorView has each Bitmap, so ViewModels need not to have information whether it is left or right.
            CurrentImageIndex = (int)(IsTouching ? CursorImageIndexLabels.CloseHand : CursorImageIndexLabels.OpenHand);
        }

        void UpdateByIHidReportForCursorViewModel(IHidReportForCursorViewModel contact)
        {
            // NOTE: It calls PropertyChanged, after all inside states are updated.
            isTrackingPrevious = IsTracking;
            if (contact.IsTracking == false)
            {
                InitializePropertiesDependsOnHidReport();
                return;
            }
            IsTracking = contact.IsTracking;
            IsVisible = IsToShowCursor && contact.IsTracking;
            PositionX = contact.X;
            PositionY = contact.Y;
            IsTouching = contact.IsTouching;
            if (IsTouching && IsToDetectLongTouch)
            {
                bool newIsLongTouching = ElapsedFromLastTouchedInMilliseconds > LongTapElapsedThresholdInMilliseconds;
                if (newIsLongTouching)
                {
                    if (IsLongTouching != newIsLongTouching) { IsLongTouching = newIsLongTouching; }
                }
            }
        }

        public virtual void UpdateByTouchScreenHidReportContact(EgsDeviceTouchScreenHidReportContact contact)
        {
            Trace.Assert(contact != null);
            UpdateByIHidReportForCursorViewModel(contact);
            Rotation = 0;
            RelativeZ = 1.0;
            FingerPitch = IsTouching ? 10.0 : 0.0;
            if (hasToCallPropertyChangedOfIsTouching) { OnIsTouchingChangedInternal(); }
            CallPropertyChangedOfOnlyUpdatedPropertiesAtOnce();
            if (hasToCallPropertyChangedOfIsTracking) { OnIsTrackingChanged(EventArgs.Empty); }
            if (hasToCallPropertyChangedOfIsTouching) { OnIsTouchingChanged(EventArgs.Empty); }
            if (hasToCallPropertyChangedOfIsVisible) { OnIsVisibleChanged(EventArgs.Empty); }
            SetFalseToAllHasToCallPeropertyChangedFields();
            OnStateUpdated(EventArgs.Empty);
        }

        bool isTrackingPrevious { get; set; }
        public virtual void UpdateByEgsGestureHidReportHand(EgsDeviceEgsGestureHidReportHand hand)
        {
            Trace.Assert(hand != null);
            UpdateByIHidReportForCursorViewModel(hand);
            Rotation = hand.Rotation;
            RelativeZ = hand.Z;
            FingerPitch = hand.FingerPitch;
            if (IsToUpdateVelocities) { UpdateVelocities(); }
            if (hasToCallPropertyChangedOfIsTouching) { OnIsTouchingChangedInternal(); }
            CallPropertyChangedOfOnlyUpdatedPropertiesAtOnce();
            if (hasToCallPropertyChangedOfIsTracking) { OnIsTrackingChanged(EventArgs.Empty); }
            if (hasToCallPropertyChangedOfIsTouching) { OnIsTouchingChanged(EventArgs.Empty); }
            if (hasToCallPropertyChangedOfIsVisible) { OnIsVisibleChanged(EventArgs.Empty); }
            SetFalseToAllHasToCallPeropertyChangedFields();
            OnStateUpdated(EventArgs.Empty);
        }

        public void UpdateVelocities()
        {
            if (isTrackingPrevious == false && IsTracking == true)
            {
                foreach (var item in Velocities)
                {
                    item.FramesPerSecond = Device.EgsGestureHidReport.FramesPerSecond;
                }
                Velocities[(int)CursorViewModelVelocityIndexes.PositionX].Initialize(PositionX);
                Velocities[(int)CursorViewModelVelocityIndexes.PositionY].Initialize(PositionY);
                Velocities[(int)CursorViewModelVelocityIndexes.Rotation].Initialize(Rotation);
                Velocities[(int)CursorViewModelVelocityIndexes.RelativeZ].Initialize(RelativeZ);
                Velocities[(int)CursorViewModelVelocityIndexes.FingerPitch].Initialize(FingerPitch);
                Velocities[(int)CursorViewModelVelocityIndexes.XYSpeed].Initialize(0.0);
            }
            else
            {
                Velocities[(int)CursorViewModelVelocityIndexes.PositionX].Update(PositionX);
                Velocities[(int)CursorViewModelVelocityIndexes.PositionY].Update(PositionY);
                Velocities[(int)CursorViewModelVelocityIndexes.Rotation].Update(Rotation);
                Velocities[(int)CursorViewModelVelocityIndexes.RelativeZ].Update(RelativeZ);
                Velocities[(int)CursorViewModelVelocityIndexes.FingerPitch].Update(FingerPitch);
                var speed = Math.Sqrt(Velocities[0].XDot * Velocities[0].XDot + Velocities[1].XDot * Velocities[1].XDot);
                Velocities[(int)CursorViewModelVelocityIndexes.XYSpeed].Update(speed);
            }
        }
    }

    public enum CursorViewModelVelocityIndexes : int
    {
        PositionX,
        PositionY,
        Rotation,
        RelativeZ,
        FingerPitch,
        XYSpeed,
        Count
    }
}

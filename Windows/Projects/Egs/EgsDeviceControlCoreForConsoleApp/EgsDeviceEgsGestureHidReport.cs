namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;
    using Egs.Win32;

    public enum EgsDeviceRecognitionStateTransitionTypes
    {
        Unknown,
        NotChanged,
        StandingBy_DetectingFaces,
        DetectingFaces_StandingBy,
        DetectingFaces_DetectingHands,
        DetectingHands_DetectingFaces,
        DetectingHands_TrackingHands,
        TrackingHands_DetectingFaces,
    }

    public class EgsGestureHidReportRecognitionStateChangedEventArgs : EventArgs
    {
        public EgsDeviceRecognitionStateTransitionTypes TransitionType { get; private set; }
        public EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes newTransitionType)
        {
            TransitionType = newTransitionType;
        }
    }

    /// <summary>
    /// Vendor specific HID report for applications of EgsSDK users.  This class does not implement INotifyProperty.  Just this reconstructs the recognition state from byte array reports.
    /// </summary>
    public class EgsDeviceEgsGestureHidReport
    {
        internal EgsDevice Device { get; private set; }
        internal HidReportIds ReportId { get; private set; }

        public event EventHandler<EgsGestureHidReportRecognitionStateChangedEventArgs> RecognitionStateChanged;
        protected virtual void OnRecognitionStateChanged(EgsGestureHidReportRecognitionStateChangedEventArgs e)
        {
            var t = RecognitionStateChanged; if (t != null) { t(this, e); }
        }
        internal EgsGestureHidReportMessageIds MessageId { get; set; }
        public bool IsStandingBy { get { return MessageId == EgsGestureHidReportMessageIds.StandingBy; } }
        public bool IsFaceDetecting { get { return MessageId == EgsGestureHidReportMessageIds.DetectingFaces; } }
        public ushort FrameNumber { get; internal set; }
        public double FramesPerSecond { get; internal set; }

        public int[] FaceDetectionArea { get; internal set; }
        public IList<EgsDeviceEgsGestureHidReportFace> Faces { get; internal set; }
        public int DetectedFacesCount { get; internal set; }
        public int SelectedFaceIndex { get; internal set; }
        public IList<EgsDeviceEgsGestureHidReportHand> Hands { get; internal set; }
        public int TrackingHandsCount { get; internal set; }

        internal double EgsDeviceScreenMappedAreaResolutionSizeWidth { get { return 7540.0; } }
        internal double EgsDeviceScreenMappedAreaResolutionSizeHeight { get { return 7540.0; } }
        /// <summary>X_host = sx * (X_device - tx).  Translation Offset X of Conversion from "Captured Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double tx { get; set; }
        /// <summary>X_host = sx * (X_device - tx).  Scaling X of Conversion from "Captured Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double sx { get; set; }
        /// <summary>Y_host = sy * (Y_device - ty).  Translation Offset Y of Conversion from "Captured Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double ty { get; set; }
        /// <summary>Y_host = sy * (Y_device - ty).  Scaling Y of Conversion from "Captured Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double sy { get; set; }

        public event EventHandler ReportUpdated;
        protected virtual void OnReportUpdated(EventArgs e) { var t = ReportUpdated; if (t != null) { t(this, e); } }

        internal EgsDeviceEgsGestureHidReport()
        {
        }

        internal void InitializeOnceAtStartup(EgsDevice device)
        {
            if (device == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new ArgumentNullException("device");
            }
            Device = device;
            Faces = Enumerable.Range(0, Device.DetectableFacesCountMaximum).Select(e => new EgsDeviceEgsGestureHidReportFace()).ToList();
            Hands = Enumerable.Range(0, Device.TrackableHandsCountMaximum).Select(e => new EgsDeviceEgsGestureHidReportHand()).ToList();
            ResetInternal();
        }

        void ResetInternal()
        {
            // NOTE: Virtual methods should not be called in any constructors.  So I divided this method from the constuctor.  Reset() method should do the same thing, so it is called from Reset().
            MessageId = EgsGestureHidReportMessageIds.StandingBy;
            ReportId = HidReportIds.EgsGesture;
            FrameNumber = 0;
            FramesPerSecond = 100.0;
            FaceDetectionArea = new int[4];
            foreach (var face in Faces) { face.Reset(); }
            DetectedFacesCount = 0;
            SelectedFaceIndex = -1;
            foreach (var hand in Hands) { hand.Reset(); }
            TrackingHandsCount = 0;
        }

        public void Reset()
        {
            ResetInternal();
            OnRecognitionStateChanged(new EgsGestureHidReportRecognitionStateChangedEventArgs(EgsDeviceRecognitionStateTransitionTypes.StandingBy_DetectingFaces));
            OnReportUpdated(EventArgs.Empty);
        }

        internal virtual void UpdateByHidReportAsByteArray(byte[] hidReport)
        {
            Trace.Assert(hidReport[0] == (byte)HidReportIds.EgsGesture);
            var previousMessageId = MessageId;
            var previousHand0RecognitionState = Hands[0].RecognitionState;
            var previousHand1RecognitionState = Hands[1].RecognitionState;

            ReportId = (HidReportIds)hidReport[0];
            MessageId = (EgsGestureHidReportMessageIds)hidReport[1];
            FrameNumber = BitConverter.ToUInt16(hidReport, 2);

            switch (MessageId)
            {
                case EgsGestureHidReportMessageIds.StandingBy:
                    Reset();
                    break;
                case EgsGestureHidReportMessageIds.DetectingFaces:
                    // NOTE: In Kickstarter 1st released version, when MessageId is DetectingFaces, the app needed to reset this object by Timer.
                    if (MessageId != previousMessageId)
                    {
                        foreach (var face in Faces) { face.Reset(); }
                        DetectedFacesCount = 0;
                        SelectedFaceIndex = -1;
                        foreach (var hand in Hands) { hand.Reset(); }
                        TrackingHandsCount = 0;
                    }
                    UpdateOnDetectingFaces(hidReport);
                    break;
                case EgsGestureHidReportMessageIds.DetectingOrTrackingHands:
                    UpdateOnDetectingOrTrackingHands(hidReport);
                    break;
                default:
                    Reset();
                    // NOTE: Is Kickstarter version, the next line can be called.  After updating firmware, the next line should not be called.
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    break;
            }

            OnReportUpdated(EventArgs.Empty);
            if (previousMessageId != MessageId
                || previousHand0RecognitionState != Hands[0].RecognitionState
                || previousHand1RecognitionState != Hands[1].RecognitionState)
            {
                bool isTrackingPrevious = ((previousHand0RecognitionState == EgsGestureHidReportRecognitionState.Tracking)
                    || (previousHand1RecognitionState == EgsGestureHidReportRecognitionState.Tracking));
                bool isTracking = ((Hands[0].RecognitionState == EgsGestureHidReportRecognitionState.Tracking)
                    || (Hands[1].RecognitionState == EgsGestureHidReportRecognitionState.Tracking));

                EgsDeviceRecognitionStateTransitionTypes eventInfo;
                switch (previousMessageId)
                {
                    case EgsGestureHidReportMessageIds.StandingBy:
                        switch (MessageId)
                        {
                            case EgsGestureHidReportMessageIds.DetectingFaces:
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.StandingBy_DetectingFaces;
                                break;
                            case EgsGestureHidReportMessageIds.DetectingOrTrackingHands:
                                // MUSTDO: check.  it seems strange that device sends reports by this order.
                                // NOTE (en): If this is correct specification, and if it does not return the value "this is transition to DetectinfFaces", later event handlers may not work well.
                                // NOTE (ja): もしこういう仕様なら、DetectingFacesへの遷移だと返さないと、この後段のイベントハンドラで期待しない動作になってしまう。
                                Debug.WriteLine("[Strange Transition] StandingBy to DetectingOrTrackingHands");
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.StandingBy_DetectingFaces;
                                // NOTE: So it returns this value.  It seems to work without any problems.
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands;
                                break;
                            case EgsGestureHidReportMessageIds.Unknown:
                            default:
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.Unknown;
                                break;
                        }
                        break;
                    case EgsGestureHidReportMessageIds.DetectingFaces:
                        switch (MessageId)
                        {
                            case EgsGestureHidReportMessageIds.StandingBy:
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_StandingBy;
                                break;
                            case EgsGestureHidReportMessageIds.DetectingOrTrackingHands:
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.DetectingFaces_DetectingHands;
                                break;
                            case EgsGestureHidReportMessageIds.Unknown:
                            default:
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.Unknown;
                                break;
                        }
                        break;
                    case EgsGestureHidReportMessageIds.DetectingOrTrackingHands:
                        if (isTrackingPrevious == false)
                        {
                            if (isTracking)
                            {
                                eventInfo = EgsDeviceRecognitionStateTransitionTypes.DetectingHands_TrackingHands;
                            }
                            else
                            {
                                // Was Detecting
                                if (MessageId == EgsGestureHidReportMessageIds.DetectingFaces) { eventInfo = EgsDeviceRecognitionStateTransitionTypes.DetectingHands_DetectingFaces; }
                                else { eventInfo = EgsDeviceRecognitionStateTransitionTypes.Unknown; }
                            }
                        }
                        else
                        {
                            // was tracking
                            if (isTracking) { eventInfo = EgsDeviceRecognitionStateTransitionTypes.NotChanged; }
                            else if (MessageId == EgsGestureHidReportMessageIds.DetectingFaces) { eventInfo = EgsDeviceRecognitionStateTransitionTypes.TrackingHands_DetectingFaces; }
                            else { eventInfo = EgsDeviceRecognitionStateTransitionTypes.Unknown; }
                        }
                        break;
                    case EgsGestureHidReportMessageIds.Unknown:
                    default:
                        eventInfo = EgsDeviceRecognitionStateTransitionTypes.Unknown;
                        break;
                }
                OnRecognitionStateChanged(new EgsGestureHidReportRecognitionStateChangedEventArgs(eventInfo));
            }
        }

        void UpdateOnDetectingFaces(byte[] hidReport)
        {
            {
                int offset = 4;
                var x = (int)(sx * (BitConverter.ToInt16(hidReport, offset + 0) - tx));
                var y = (int)(sy * (BitConverter.ToInt16(hidReport, offset + 2) - ty));
                var w = (int)(sx * (BitConverter.ToInt16(hidReport, offset + 4)));
                var h = (int)(sy * (BitConverter.ToInt16(hidReport, offset + 6)));
                FaceDetectionArea = new int[4] { x, y, w, h };
            }
            DetectedFacesCount = hidReport[12];
            for (int i = 0; i < DetectedFacesCount; i++)
            {
                int offset = 10 * (i + 1) + 4;
                var x = (int)(sx * (BitConverter.ToInt16(hidReport, offset + 0) - tx));
                var y = (int)(sy * (BitConverter.ToInt16(hidReport, offset + 2) - ty));
                var w = (int)(sx * (BitConverter.ToInt16(hidReport, offset + 4)));
                var h = (int)(sy * (BitConverter.ToInt16(hidReport, offset + 6)));
                Faces[i].IsDetected = true;
                Faces[i].IsSelected = false;
                Faces[i].Area = new int[4] { x, y, w, h };
                Faces[i].Score = hidReport[offset + 8];
            }
            SelectedFaceIndex = (int)((sbyte)hidReport[13]);
            if (SelectedFaceIndex >= DetectedFacesCount)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            if (SelectedFaceIndex >= 0)
            {
                Faces[SelectedFaceIndex].IsSelected = true;
            }
        }

        void UpdateOnDetectingOrTrackingHands(byte[] hidReport)
        {
            var newTrackingHandsCount = 0;
            // MUSTDO: Check if the next line (handIndex < 2) is correct or not.
            for (int handIndex = 0; handIndex < 2; handIndex++)
            {
                int byteOffset = handIndex * 30;
                var hand = Hands[handIndex];
                hand.RecognitionState = (EgsGestureHidReportRecognitionState)hidReport[byteOffset + 4];
                hand.ObjectKind = hidReport[byteOffset + 5];
                hand.IsTouching = (hidReport[byteOffset + 6] & 0x01) == 0x01;
                hand.X = (int)(BitConverter.ToInt16(hidReport, byteOffset + 7) * hand.XScaleFactor);
                hand.Y = (int)(BitConverter.ToInt16(hidReport, byteOffset + 9) * hand.YScaleFactor);
                hand.Rotation = (sbyte)hidReport[byteOffset + 11];
                hand.Z = hidReport[byteOffset + 12];
                hand.FingerPitch = hidReport[byteOffset + 16];
                switch (hand.RecognitionState)
                {
                    case EgsGestureHidReportRecognitionState.NotDetecting:
                        {
                            break;
                        }
                    case EgsGestureHidReportRecognitionState.Detecting:
                        {
                            var x = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 18) - tx));
                            var y = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 20) - ty));
                            var w = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 22)));
                            var h = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 24)));
                            hand.DetectionArea = new int[4] { x, y, w, h };
                            break;
                        }
                    case EgsGestureHidReportRecognitionState.OnScreenMappedAreaUpdated:
                        {
                            var x1 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 18) - tx));
                            var y1 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 20) - ty));
                            var w1 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 22)));
                            var h1 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 24)));
                            hand.TrackingArea = new int[4] { x1, y1, w1, h1 };
                            var x2 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 26) - tx));
                            var y2 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 28) - ty));
                            var w2 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 30)));
                            var h2 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 32)));
                            hand.ScreenMappedArea = new int[4] { x2, y2, w2, h2 };
                            break;
                        }
                    case EgsGestureHidReportRecognitionState.Tracking:
                        {
                            var x = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 18) - tx));
                            var y = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 20) - ty));
                            var w = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 22)));
                            var h = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 24)));
                            hand.TrackingArea = new int[4] { x, y, w, h };
                            newTrackingHandsCount++;
                            break;
                        }
                    case EgsGestureHidReportRecognitionState.OnTrackingEnded:
                        {
                            hand.Reset();
                            break;
                        }
                    default:
                        Debug.WriteLine("[WARNING] EgsGestureHidReportRecognitionState default label:");
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        break;
                }
            }
            TrackingHandsCount = newTrackingHandsCount;
        }

        enum EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick
        {
            StandingBy,
            DetectingFace,
            DetectedFace,
            SelectedFaceAndDetectingHands,
            DetectedAndScreenMappedRightHand,
            TrackingRightHand,
            StoppedTrackingRightHand,
            Count,
        }
        EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick _StatusForMouseEmulation = 0;
        private EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick StatusForMouseEmulation
        {
            get { return _StatusForMouseEmulation; }
            set
            {
                _StatusForMouseEmulation = value;
                Debug.WriteLine("Emulated Status Changed: " + StatusForMouseEmulation);
            }
        }

#if USE_OLD_HID
        internal void UpdateByRawMouse(ref NativeMethods.RAWMOUSE mouse)
        {
            FrameNumber++;
            switch (StatusForMouseEmulation)
            {
                case EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.StandingBy:
                    Reset();
                    break;
                case EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.DetectingFace:
                    Reset();
                    FaceDetectionArea = new System.Drawing.Rectangle(10, 10, 320, 240);
                    break;
                case EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.DetectedFace:
                case EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.SelectedFaceAndDetectingHands:
                    {
                        Reset();
                        FaceDetectionArea = new System.Drawing.Rectangle(10, 10, 320, 240);
                        DetectedFacesCount = 1;
                        Faces[0].IsDetected = true;
                        Faces[0].Area = new System.Drawing.Rectangle(100, 100, 50, 50);
                        Faces[0].Score = 100;
                        SelectedFaceIndex = -1;
                        Faces[0].IsSelected = false;
                        if (StatusForMouseEmulation == EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.SelectedFaceAndDetectingHands)
                        {
                            SelectedFaceIndex = 0;
                            Faces[0].IsSelected = true;
                            Hands[0].RecognitionState = EgsGestureHidReportRecognitionState.Detecting;
                            Hands[0].DetectionArea = new System.Drawing.Rectangle(160, 50, 100, 100);
                        }
                    }
                    break;
                case EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.DetectedAndScreenMappedRightHand:
                    UpdateByRawMouseWhileDetectedAndScreenMappedRightHand(ref mouse);
                    StatusForMouseEmulation = EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.TrackingRightHand;
                    break;
                case EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.TrackingRightHand:
                    UpdateByRawMouseWhileTrackingRightHand(ref mouse);
                    break;
                case EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.StoppedTrackingRightHand:
                    Hands[0].Reset();
                    StatusForMouseEmulation = EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.StandingBy;
                    break;
                default:
                    break;
            }
            if (mouse.buttons.usButtonFlags == NativeMethods.RawMouseButtonFlags.RI_MOUSE_RIGHT_BUTTON_DOWN)
            {
                var nextStatusAsInt = (((int)StatusForMouseEmulation) + 1) % (int)EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick.Count;
                StatusForMouseEmulation = (EgsDeviceEgsGestureHidReportStatusEmulatedByMouseAndChangedByRightClick)nextStatusAsInt;
            }
            OnReportUpdated(EventArgs.Empty);
        }

        void UpdateByRawMouseWhileDetectedAndScreenMappedRightHand(ref NativeMethods.RAWMOUSE mouse)
        {
            // MUSTDO: check this usage.
            DetectedFacesCount = 1;
            SelectedFaceIndex = 0;
            TrackingHandsCount = 1;
            Hands[0].RecognitionState = EgsGestureHidReportRecognitionState.OnScreenMappedAreaUpdated;
            Hands[0].ObjectKind = 0;
            Hands[0].TrackingArea = new System.Drawing.Rectangle(170, 60, 80, 60);
            Hands[0].ScreenMappedArea = new System.Drawing.Rectangle(190, 80, 40, 30);
            Hands[0].X = System.Windows.Forms.Cursor.Position.X;
            Hands[0].Y = System.Windows.Forms.Cursor.Position.Y;
            Hands[0].IsTouching = false;
            Hands[1].Reset();
        }

        void UpdateByRawMouseWhileTrackingRightHand(ref NativeMethods.RAWMOUSE mouse)
        {
            DetectedFacesCount = 1;
            SelectedFaceIndex = 0;
            TrackingHandsCount = 1;

            Hands[0].RecognitionState = EgsGestureHidReportRecognitionState.Tracking;
            Hands[0].ObjectKind = 0;

            if (ApplicationCommonSettings.IsToEmulateReportByActualMouseRawInputToDebugViews)
            {
                Hands[0].X = System.Windows.Forms.Cursor.Position.X;
                Hands[0].Y = System.Windows.Forms.Cursor.Position.Y;
                // NOTE: The value of the next line is not correct!  So It's no use.
                Hands[0].IsTouching = (System.Windows.Forms.Control.MouseButtons & System.Windows.Forms.MouseButtons.Left) == System.Windows.Forms.MouseButtons.Left;
                Hands[1].Reset();
                return;
            }
            else if (mouse.usFlags == NativeMethods.RawMouseFlags.MOUSE_MOVE_ABSOLUTE)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                // Absolute coordinate.  EGS devices send the value in the absolute coordinates.
                Hands[0].X = (int)(mouse.lLastX * Hands[0].XScaleFactor);
                Hands[0].Y = (int)(mouse.lLastY * Hands[0].YScaleFactor);
            }
            else if (mouse.usFlags == NativeMethods.RawMouseFlags.MOUSE_MOVE_RELATIVE)
            {
                // Relative coordinate from previous position.  Normal mouses send the value in the relative coordinates.
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Hands[0].X += (int)(mouse.lLastX * Hands[0].XScaleFactor);
                Hands[0].Y += (int)(mouse.lLastY * Hands[0].YScaleFactor);
            }
            else
            {
                Hands[0].Reset();
                Hands[1].Reset();
                OnReportUpdated(EventArgs.Empty);
                return;
            }
            // NOTE: Must save the previous ON/OFF state.  In dragging, LEFT_BUTTON_DOWN is false, so it cannot recognize touch state in dragging.
            var isHand0TouchingPrevious = Hands[0].IsTouching;
            var isMouseLeftButtonDown = (mouse.buttons.usButtonFlags == NativeMethods.RawMouseButtonFlags.RI_MOUSE_LEFT_BUTTON_DOWN);
            var isMouseLeftButtonUp = (mouse.buttons.usButtonFlags == NativeMethods.RawMouseButtonFlags.RI_MOUSE_LEFT_BUTTON_UP);
            if (isMouseLeftButtonDown && (isHand0TouchingPrevious == false))
            {
                Hands[0].IsTouching = true;
            }
            else if (isMouseLeftButtonUp && (isHand0TouchingPrevious == true))
            {
                Hands[0].IsTouching = false;
            }

            if (false)
            {
                Console.Write("mouse.lLastX={0}  ", mouse.lLastX);
                Console.Write("mouse.lLastY={0}  ", mouse.lLastY);
                //Console.Write("isHand0TouchingPrevious={0}  ", isHand0TouchingPrevious);
                Console.Write("isMouseLeftButtonDown={0}  ", isMouseLeftButtonDown);
                Console.Write("isMouseLeftButtonUp={0}  ", isMouseLeftButtonUp);
                //Console.Write("Hands[0].X={0}  ", Hands[0].X);
                //Console.Write("Hands[0].Y={0}  ", Hands[0].Y);
                Console.WriteLine();
            }

            // It does not use 2nd point.
            Hands[1].Reset();
        }
#endif

    }
}

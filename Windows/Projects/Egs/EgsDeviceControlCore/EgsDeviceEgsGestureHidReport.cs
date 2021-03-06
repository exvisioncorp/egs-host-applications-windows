﻿namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;
    using Egs.Win32;

    /// <summary>
    /// Vendor specific HID report for applications of EgsSDK users.  This class does not implement INotifyProperty.  Just this reconstructs the recognition state from byte array reports.
    /// </summary>
    public partial class EgsDeviceEgsGestureHidReport
    {
        internal EgsDevice Device { get; private set; }
        internal HidReportIds ReportId { get; private set; }

        public event EventHandler RecognitionStateChanged;
        protected virtual void OnRecognitionStateChanged(EventArgs e)
        {
            var t = RecognitionStateChanged; if (t != null) { t(this, e); }
        }
        internal EgsGestureHidReportMessageIds MessageId { get; set; }
        internal EgsGestureHidReportMessageIds MessageIdPrevious { get; private set; }
        internal IList<EgsGestureHidReportRecognitionState> HandRecognitionStatePreviousList { get; private set; }

        public bool IsStandingBy { get { return MessageId == EgsGestureHidReportMessageIds.StandingBy; } }
        public bool IsFaceDetecting { get { return MessageId == EgsGestureHidReportMessageIds.DetectingFaces; } }
        public ushort FrameNumber { get; internal set; }
        public double FramesPerSecond { get; internal set; }

        public System.Drawing.Rectangle FaceDetectionArea { get; internal set; }
        public IList<EgsDeviceEgsGestureHidReportFace> Faces { get; internal set; }
        public int DetectedFacesCount { get; internal set; }
        public int SelectedFaceIndex { get; internal set; }
        public IList<EgsDeviceEgsGestureHidReportHand> Hands { get; internal set; }
        public int TrackingHandsCount { get; internal set; }

        internal double EgsDeviceScreenMappedAreaResolutionSizeWidth { get { return 7540.0; } }
        internal double EgsDeviceScreenMappedAreaResolutionSizeHeight { get { return 7540.0; } }
        /// <summary>X_host = sx * (X_device - tx).  Translation Offset X of Conversion from "Capture Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double tx { get; set; }
        /// <summary>X_host = sx * (X_device - tx).  Scaling X of Conversion from "Capture Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double sx { get; set; }
        /// <summary>Y_host = sy * (Y_device - ty).  Translation Offset Y of Conversion from "Capture Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double ty { get; set; }
        /// <summary>Y_host = sy * (Y_device - ty).  Scaling Y of Conversion from "Capture Image Coordinate on Sensor in Device" to "Camera View Image Coordinate on Host"</summary>
        double sy { get; set; }
        internal void UpdateImageSizeRelatedProperties()
        {
            if (Device != null && Device.Settings != null && Device.Settings.CameraViewImageSourceRectInCaptureImage != null && Device.Settings.CameraViewImageSourceBitmapSize != null)
            {
                var cameraViewImageSourceRectInCaptureImage = Device.Settings.CameraViewImageSourceRectInCaptureImage.Value;
                var cameraViewImageSourceBitmapSize = Device.Settings.CameraViewImageSourceBitmapSize.SelectedItem.Size;
                // MUSTDO: This is tested by WinForm example code.  WPF code is not correct.
                Debug.WriteLine("cameraViewImageSourceRectInCaptureImage: " + cameraViewImageSourceRectInCaptureImage + ", cameraViewImageSourceBitmapSize: " + cameraViewImageSourceBitmapSize);
                tx = (double)cameraViewImageSourceRectInCaptureImage.X;
                ty = (double)cameraViewImageSourceRectInCaptureImage.Y;
                sx = (double)cameraViewImageSourceBitmapSize.Width / (double)cameraViewImageSourceRectInCaptureImage.Width;
                sy = (double)cameraViewImageSourceBitmapSize.Height / (double)cameraViewImageSourceRectInCaptureImage.Height;
            }
            else
            {
                Debug.WriteLine("cameraViewImageSourceBitmapSize or cameraViewImageSourceRectInCaptureImage is unavailable");
                tx = 0;
                ty = 0;
                sx = 1;
                sy = 1;
            }
            Debug.WriteLine("tx: " + tx + ", ty: " + ty + ", sx: " + sx + ", sy: " + sy);
        }

        public event EventHandler ReportUpdated;
        protected virtual void OnReportUpdated(EventArgs e) { var t = ReportUpdated; if (t != null) { t(this, e); } }

        void UpdateHandXYScaleFactors()
        {
            // MUSTDO: Important!!  It makes effects to the position of gesture cursors.  Should test with changing DPI.
            var sizeByDesktopResolutionInPhysicalPixels = Dpi.GetPrimaryScreenPhysicalPixelResolution();
            // NOTE: By this way, it does not work well if the DPI is not 100[%].
            //var dpi = Dpi.GetDpiFromHdcForTheEntireScreen();
            // NOTE: When DPI is or is not 100[%], it works well by using DPI in 100[%].
            // MUSTDO: But by this way, if "aspect ratio of screen resolution" is different from "aspect ratio of default (maximum?) screen resolution", the gesture cursor position is wrong in Windows 10.
            var dpi = Dpi.Default;
            var scaledPrimaryScreenBounds = Dpi.GetPrimaryScreenPhysicalPixelResolution();
            if (ApplicationCommonSettings.IsDebugging)
            {
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "sizeByDesktopResolutionInPhysicalPixels: {0}, {1}", sizeByDesktopResolutionInPhysicalPixels.Width, sizeByDesktopResolutionInPhysicalPixels.Height));
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "Dpi: {0}, {1}", dpi.X, dpi.Y));
#if false
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "System.Windows.Forms.Screen.PrimaryScreen.Bounds: {0}", System.Windows.Forms.Screen.PrimaryScreen.Bounds.ToString()));
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "System.Windows.Forms.Screen.PrimaryScreen.Bounds: {0}", System.Windows.Forms.Screen.PrimaryScreen.Bounds.ToString()));
#endif
            }

            foreach (var hand in Hands)
            {
                hand.ScreenWidthInPhysicalPixels = scaledPrimaryScreenBounds.Width;
                hand.ScreenHeightInPhysicalPixels = scaledPrimaryScreenBounds.Height;
                hand.XScaleFactor = hand.ScreenWidthInPhysicalPixels / EgsDeviceScreenMappedAreaResolutionSizeWidth;
                hand.YScaleFactor = hand.ScreenHeightInPhysicalPixels / EgsDeviceScreenMappedAreaResolutionSizeHeight;
            }
        }
        internal void OnDisplaySettingsChanged(object sender, EventArgs e)
        {
            UpdateHandXYScaleFactors();
        }

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
            HandRecognitionStatePreviousList = Enumerable.Range(0, Device.TrackableHandsCountMaximum).Select(e => EgsGestureHidReportRecognitionState.NotDetecting).ToList();
            UpdateHandXYScaleFactors();
            ResetInternal();
        }

        void ResetInternal()
        {
            // NOTE: Virtual methods should not be called in any constructors.  So I divided this method from the constuctor.  Reset() method should do the same thing, so it is called from Reset().
            ReportId = HidReportIds.EgsGesture;
            MessageId = EgsGestureHidReportMessageIds.StandingBy;
            MessageIdPrevious = EgsGestureHidReportMessageIds.StandingBy;
            HandRecognitionStatePreviousList = Enumerable.Range(0, Device.TrackableHandsCountMaximum).Select(e => EgsGestureHidReportRecognitionState.NotDetecting).ToList();
            FrameNumber = 0;
            FramesPerSecond = 100.0;
            FaceDetectionArea = new System.Drawing.Rectangle();
            foreach (var face in Faces) { face.Reset(); }
            DetectedFacesCount = 0;
            SelectedFaceIndex = -1;
            foreach (var hand in Hands) { hand.Reset(); }
            TrackingHandsCount = 0;
            UpdateImageSizeRelatedProperties();
        }

        public void Reset()
        {
            ResetInternal();
            OnRecognitionStateChanged(EventArgs.Empty);
            OnReportUpdated(EventArgs.Empty);
        }

        internal virtual void UpdateByHidReportAsByteArray(byte[] hidReport)
        {
            Trace.Assert(hidReport[0] == (byte)HidReportIds.EgsGesture);

            if (MessageId != EgsGestureHidReportMessageIds.Unknown) { MessageIdPrevious = MessageId; }
            HandRecognitionStatePreviousList[0] = Hands[0].RecognitionState;
            HandRecognitionStatePreviousList[1] = Hands[1].RecognitionState;

            ReportId = (HidReportIds)hidReport[0];
            MessageId = (EgsGestureHidReportMessageIds)hidReport[1];
            FrameNumber = BitConverter.ToUInt16(hidReport, 2);

            switch (MessageId)
            {
                case EgsGestureHidReportMessageIds.StandingBy:
                    Reset();
                    break;
                case EgsGestureHidReportMessageIds.ChangedSettingsByDevice:
                    UpdateOnChangedSettingsByDevice(hidReport);
                    MessageId = EgsGestureHidReportMessageIds.StandingBy;
                    Reset();
                    break;
                case EgsGestureHidReportMessageIds.DetectingFaces:
                    if (Device.Settings.FaceDetectionMethod.Value == FaceDetectionMethods.DefaultProcessOnEgsDevice)
                    {
                        // NOTE: In Kickstarter 1st released version, when MessageId is DetectingFaces, the app needed to reset this object by Timer.
                        if (MessageId != MessageIdPrevious)
                        {
                            if (false) { Debug.WriteLine("DetectingFaces && MessageId has changed."); }
                            foreach (var face in Faces) { face.Reset(); }
                            DetectedFacesCount = 0;
                            SelectedFaceIndex = -1;
                            foreach (var hand in Hands) { hand.Reset(); }
                            TrackingHandsCount = 0;
                        }
                        // TODO: MUSTDO: When application detects faces on host, report from device is wrong.
                        UpdateOnDetectingFaces(hidReport);
                    }
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
            CheckRecognitionStateChanged();
        }

        void CheckRecognitionStateChanged()
        {
            if (MessageId == EgsGestureHidReportMessageIds.Unknown) { return; }
            //if (MessageIdPrevious == EgsGestureHidReportMessageIds.Unknown) { return; }
            if (MessageIdPrevious != MessageId
                || HandRecognitionStatePreviousList[0] != Hands[0].RecognitionState
                || HandRecognitionStatePreviousList[1] != Hands[1].RecognitionState)
            {
                OnRecognitionStateChanged(EventArgs.Empty);
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
                FaceDetectionArea = new System.Drawing.Rectangle(x, y, w, h);
            }
            DetectedFacesCount = hidReport[12];
            if (false) { Debug.WriteLine("detectedFacesCount: " + DetectedFacesCount); }
            for (int i = 0; i < DetectedFacesCount; i++)
            {
                int offset = 10 * (i + 1) + 4;
                var x = (int)(sx * (BitConverter.ToInt16(hidReport, offset + 0) - tx));
                var y = (int)(sy * (BitConverter.ToInt16(hidReport, offset + 2) - ty));
                var w = (int)(sx * (BitConverter.ToInt16(hidReport, offset + 4)));
                var h = (int)(sy * (BitConverter.ToInt16(hidReport, offset + 6)));
                Faces[i].IsDetected = true;
                Faces[i].IsSelected = false;
                Faces[i].Area = new System.Drawing.Rectangle(x, y, w, h);
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

        void UpdateOnChangedSettingsByDevice(byte[] hidReport)
        {
            // TODO: MUSTDO: test and debug
            byte categoryId = hidReport[2];
            byte propertyId = hidReport[3];
            var targetProperty = Device.Settings.HidAccessPropertyList.SingleOrDefault(e => e.CategoryId == categoryId && e.PropertyId == propertyId);
            if (targetProperty == null)
            {
                Debug.WriteLine($"[WARNING] Could not find the target HID property.  categoryId: {categoryId}  propertyId: {propertyId}");
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            if (hidReport.Length != targetProperty.ByteArrayData.Length)
            {
                Debug.WriteLine($"[WARNING] hidReport.Length != targetProperty.ByteArrayData.Length.  hidReport.Length: {hidReport.Length}  targetProperty.ByteArrayData.Length: {targetProperty.ByteArrayData.Length}");
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            if (false)
            {
                // This is general way but it cannot change the Device.Settings in the host appliaction
                for (int i = 4; i < hidReport.Length; i++) { targetProperty.ByteArrayData[i] = hidReport[i]; }
                targetProperty.RaiseValueUpdatedOnGetHidFeatureReport();
            }

#if ApplicationCommonSettings_CanChangeDeviceUsage
            if (targetProperty == Device.Settings.DeviceUsage && hidReport[16] == (byte)DeviceUsages.RemoteTouch)
            {
                Device.Settings.DeviceUsage.Value = DeviceUsages.RemoteTouch;
            }
#endif
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
                            hand.DetectionArea = new System.Drawing.Rectangle(x, y, w, h);
                            break;
                        }
                    case EgsGestureHidReportRecognitionState.OnScreenMappedAreaUpdated:
                        {
                            var x1 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 18) - tx));
                            var y1 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 20) - ty));
                            var w1 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 22)));
                            var h1 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 24)));
                            hand.TrackingArea = new System.Drawing.Rectangle(x1, y1, w1, h1);
                            var x2 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 26) - tx));
                            var y2 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 28) - ty));
                            var w2 = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 30)));
                            var h2 = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 32)));
                            hand.ScreenMappedArea = new System.Drawing.Rectangle(x2, y2, w2, h2);
                            break;
                        }
                    case EgsGestureHidReportRecognitionState.Tracking:
                        {
                            var x = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 18) - tx));
                            var y = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 20) - ty));
                            var w = (int)(sx * (BitConverter.ToInt16(hidReport, byteOffset + 22)));
                            var h = (int)(sy * (BitConverter.ToInt16(hidReport, byteOffset + 24)));
                            hand.TrackingArea = new System.Drawing.Rectangle(x, y, w, h);
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

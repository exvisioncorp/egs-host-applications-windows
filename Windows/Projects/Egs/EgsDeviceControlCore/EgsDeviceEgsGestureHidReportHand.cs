namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Diagnostics;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;

    /// <summary>
    /// Information about one hand made from Vendor Specific HID Report
    /// </summary>
    public class EgsDeviceEgsGestureHidReportHand : IHidReportForCursorViewModel
    {
        EgsGestureHidReportRecognitionState _RecognitionState;
        public EgsGestureHidReportRecognitionState RecognitionState
        {
            get { return _RecognitionState; }
            internal set
            {
                _RecognitionState = value;
                IsDetecting = (_RecognitionState == EgsGestureHidReportRecognitionState.Detecting);
                IsTracking = (_RecognitionState == EgsGestureHidReportRecognitionState.OnScreenMappedAreaUpdated)
                    || (_RecognitionState == EgsGestureHidReportRecognitionState.Tracking);
            }
        }
        public byte ObjectKind { get; internal set; }
        public RightOrLeftKind RightOrLeft
        {
            get { return (RightOrLeftKind)(ObjectKind & 0x03); }
        }
        public EgsGestureHidReportTargetObjectCategories TargetObjectCategory
        {
            get { return (EgsGestureHidReportTargetObjectCategories)(ObjectKind >> 2); }
        }
        public bool IsTouching { get; internal set; }
        /// <summary>X position on primary screen</summary>
        public int X { get; internal set; }
        /// <summary>Y position on primary screen</summary>
        public int Y { get; internal set; }
        public sbyte Rotation { get; internal set; }
        public byte Z { get; internal set; }
        public byte FingerPitch { get; internal set; }
        public bool IsDetecting { get; internal set; }
        public System.Drawing.Rectangle DetectionArea { get; internal set; }
        public bool IsTracking { get; internal set; }
        public System.Drawing.Rectangle TrackingArea { get; internal set; }
        public System.Drawing.Rectangle ScreenMappedArea { get; internal set; }

        // NOTE: This is very old specification.  Should be fixed.
        // TODO: implement in ViewModel and so on.
        //26, Bit 0   public  bool                                  Hands[0].XYSpeedIsFast    true(1) or false(0)        Low priority.  Confirm necessity.
        //26, Bit 1-3 public  byte (3bits)                          Hands[0].XYVelocityDirection    0 to 7    atan2(vy/vx), scaled to 0-7    Low priority.  Confirm necessity.
        //26, Bit 4-7 public  byte (4bits)                          Hands[0].XYSpeed    0 to 63    XY speed in pixels per one frame, clipped to 63    Low priority.  Confirm necessity.
        //27, Bit 0   public  bool                                  Hands[0].RotationSpeedIsFast    true(1) or false(0)        Low priority.  Confirm necessity.
        //27, Bit 1   public  EgsGestureHidReportRotationDirection  Hands[0].RotationDirection    Outer or Inner    TopMovesOuterSide=0.  TopMovesInnerSide=1.      Low priority.  Confirm necessity.
        //27, Bit 2-7 public  byte (6bits)                          Hands[0].RotationSpeed    0 to 63    Rotation speed in degrees per one frame,    Low priority.  Confirm necessity.
        //28, Bit 0   public  bool                                  Hands[0].ZSpeedIsFast    true(1) or false(0)        Low priority.  Confirm necessity.
        //28, Bit 1   public  EgsGestureHidReportScaleDirection     Hands[0].ZDirection    Closer or Further    atan2(vy/vx), scaled to 0-7    Low priority.  Confirm necessity.
        //28, Bit 2-7 public  byte (6bits)                          Hands[0].ZSpeed    0 to 63    XY speed in pixels per one frame    Low priority.  Confirm necessity.

        public double ScreenWidthInPhysicalPixels { get; internal set; }
        public double ScreenHeightInPhysicalPixels { get; internal set; }
        public double XScaleFactor { get; internal set; }
        public double YScaleFactor { get; internal set; }
        public double CursorXInScreenMappedArea { get { return (double)X / ScreenWidthInPhysicalPixels; } }
        public double CursorYInScreenMappedArea { get { return (double)Y / ScreenHeightInPhysicalPixels; } }
        public double XInCameraViewImage { get { return ScreenMappedArea.X + CursorXInScreenMappedArea * ScreenMappedArea.Width; } }
        public double YInCameraViewImage { get { return ScreenMappedArea.Y + CursorYInScreenMappedArea * ScreenMappedArea.Height; } }

        internal EgsDeviceEgsGestureHidReportHand()
        {
            Reset();
        }

        internal void Reset()
        {
            RecognitionState = EgsGestureHidReportRecognitionState.NotDetecting;
            ObjectKind = 0;
            IsTouching = false;
            X = short.MinValue;
            Y = short.MinValue;
            Rotation = 0;
            Z = 0;
            FingerPitch = 0;
            DetectionArea = new System.Drawing.Rectangle();
            TrackingArea = new System.Drawing.Rectangle();
            ScreenMappedArea = new System.Drawing.Rectangle();
        }
    }
}

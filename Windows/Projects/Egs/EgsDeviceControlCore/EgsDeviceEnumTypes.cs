using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Egs
{
    public enum EgsGestureHidReportRecognitionState : byte
    {
        /// <summary>0x00</summary>
        NotDetecting = 0x00,
        /// <summary>0x02</summary>
        Detecting = 0x02,
        /// <summary>0x08</summary>
        OnScreenMappedAreaUpdated = 0x08,
        /// <summary>0x20</summary>
        Tracking = 0x20,
        /// <summary>0x80</summary>
        OnTrackingEnded = 0x80,
    }

    internal enum EgsGestureHidReportMessageIds : byte
    {
        /// <summary>0x00</summary>
        Unknown = 0x00,
        /// <summary>0x01</summary>
        StandingBy = 0x01,
        /// <summary>0x08</summary>
        DetectingFaces = 0x08,
        /// <summary>0x10</summary>
        DetectingOrTrackingHands = 0x10,
    }

    public enum RightOrLeftKind : byte
    {
        Right = 0,
        Left = 1,
        None = 2,
        Unknown = 3,
    }

    public enum EgsGestureHidReportRotationDirection : byte
    {
        Unknown = 0,
        TopMovesToOuterSide = 1,
        TopMovesToInnerSide = 2,
    }

    public enum EgsGestureHidReportScaleDirection : byte
    {
        Unknown = 0,
        MovesCloserToCamera = 1,
        MovesFurtherFromCamera = 2,
    }
}

namespace Egs.PropertyTypes
{
    internal enum HidReportIds : byte
    {
        /// <summary>0x01</summary>
        TouchScreen = 0x01,
        /// <summary>0x02</summary>
        Keyboard = 0x02,
        /// <summary>0x03</summary>
        Mouse = 0x03,
        /// <summary>0x04</summary>
        Gamepad = 0x04,
        /// <summary>0x05</summary>
        Joystick = 0x05,
        /// <summary>0x0A</summary>
        EgsGesture = 0x0A,
        /// <summary>0x0B</summary>
        EgsDeviceSettings = 0x0B,
        /// <summary>0x0C.  (EgsDeviceQualityAssurance is also 0x0C.)</summary>
        EgsDeviceFirmwareUpdate = 0x0C,
        /// <summary>0x0C.  (EgsDeviceFirmwareUpdate is also 0x0C.)</summary>
        EgsDeviceQualityAssurance = 0x0C,
    }

    public enum EgsGestureHidReportTargetObjectCategories : byte
    {
        FiveFingersHandForTv = 0,
        Unknown = 63,
        Count = 64,
    }
}

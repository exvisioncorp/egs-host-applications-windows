using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// NOTE: Properties used in device and host.
namespace Egs.PropertyTypes
{
    public enum CameraViewBordersAndPointersAreDrawnByKind
    {
        HostApplication,
        Device
    }

    public enum FaceDetectionIsProcessedByKind
    {
        HostApplication,
        Device,
    }
}

// NOTE: Properties used only in host.
namespace Egs.PropertyTypes
{
    public enum MouseCursorPositionUpdatedByGestureCursorMethods
    {
        None,
        FirstFoundHand,
        RightHand,
        LeftHand,
    }

    public enum CursorDrawingTimingMethods
    {
        ByHidReportUpdatedEvent,
        ByTimer60Fps,
        ByTimer30Fps,
    }

    public enum CameraViewWindowStateHostApplicationsControlMethods
    {
        UseUsersControlMethods,
        KeepNormal,
        KeepMinimized,
    }

    public enum CameraViewWindowStateUsersControlMethods
    {
        /// <summary>0: Manual Show / Hide by Icons on Task Bar and System Tray.  Hide by the Minimize Button on the Menu</summary>
        ManualOnOff,
        /// <summary>1: Show When Face Recognition Starts.  Hide Soon After Hand Tracking Starts</summary>
        ShowWhenFaceDetectionStart_HideSoonAfterHandTrackingStart,
        /// <summary>2: Show When Face Recognition Starts, and Hide if Recognized.  Show in Hand Tracking</summary>
        ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart,
        /// <summary>3: Show When Face Recognition Starts, and Hide if Recognized.  Show for a While After Hand Tracking Starts</summary>
        ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart,
        /// <summary>4: Show if Faces are Recognized.</summary>
        ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideWhenHandTrackingEnd,
        /// <summary>5: Show if Faces are Recognized.  Hide Soon After Hand Tracking Starts</summary>
        ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideSoonAfterHandTrackingStart,
        /// <summary>6: Show in Hand Tracking</summary>
        ShowWhenHandTrackingStart_HideWhenHandTrackingEnd,
        /// <summary>7: Show for a While After Hand Tracking Starts</summary>
        ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart,
    }
}

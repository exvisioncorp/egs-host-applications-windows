namespace Egs.PropertyTypes
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using System.Windows;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;

    public enum CameraViewBordersAndPointersAreDrawnByKind
    {
        HostApplication,
        Device
    }
    public class CameraViewBordersAndPointersAreDrawnByOptions : EnumValueWithDescriptionOptions<CameraViewBordersAndPointersAreDrawnByKind>
    {
        public CameraViewBordersAndPointersAreDrawnByOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CameraViewBordersAndPointersAreDrawnByKind>()
            {
                Value = CameraViewBordersAndPointersAreDrawnByKind.HostApplication,
                DescriptionKey = nameof(Resources.CameraViewBordersAndPointersAreDrawnByDetail_ByHostApplication_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<CameraViewBordersAndPointersAreDrawnByKind>()
            {
                Value = CameraViewBordersAndPointersAreDrawnByKind.Device,
                DescriptionKey = nameof(Resources.CameraViewBordersAndPointersAreDrawnByDetail_ByDevice_Description)
            });
        }
    }

    public class CultureInfoAndDescriptionOptions : EnumValueWithDescriptionOptions<string>
    {
        public CultureInfoAndDescriptionOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<string>()
            {
                Value = "",
                DescriptionKey = nameof(Resources.CultureInfoAndDescriptionDetail_UseOSCulture_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<string>()
            {
                Value = "en",
                DescriptionKey = nameof(Resources.CultureInfoAndDescriptionDetail_English_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<string>()
            {
                Value = "ja",
                DescriptionKey = nameof(Resources.CultureInfoAndDescriptionDetail_Japanese_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<string>()
            {
                Value = "zh-Hans",
                DescriptionKey = nameof(Resources.CultureInfoAndDescriptionDetail_Chinese_Description)
            });
        }
    }

    public enum MouseCursorPositionUpdatedByGestureCursorMethods
    {
        None,
        FirstFoundHand,
        RightHand,
        LeftHand,
    }
    public class MouseCursorPositionUpdatedByGestureCursorMethodOptions : EnumValueWithDescriptionOptions<MouseCursorPositionUpdatedByGestureCursorMethods>
    {
        public MouseCursorPositionUpdatedByGestureCursorMethodOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<MouseCursorPositionUpdatedByGestureCursorMethods>()
            {
                Value = MouseCursorPositionUpdatedByGestureCursorMethods.None,
                DescriptionKey = nameof(Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_None_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<MouseCursorPositionUpdatedByGestureCursorMethods>()
            {
                Value = MouseCursorPositionUpdatedByGestureCursorMethods.FirstFoundHand,
                DescriptionKey = nameof(Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_FirstFoundHand_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<MouseCursorPositionUpdatedByGestureCursorMethods>()
            {
                Value = MouseCursorPositionUpdatedByGestureCursorMethods.RightHand,
                DescriptionKey = nameof(Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_RightHand_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<MouseCursorPositionUpdatedByGestureCursorMethods>()
            {
                Value = MouseCursorPositionUpdatedByGestureCursorMethods.LeftHand,
                DescriptionKey = nameof(Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_LeftHand_Description)
            });
        }
    }

    public enum CursorDrawingTimingMethods
    {
        ByHidReportUpdatedEvent,
        ByTimer60Fps,
        ByTimer30Fps,
    }
    public class CursorDrawingTimingMethodOptions : EnumValueWithDescriptionOptions<CursorDrawingTimingMethods>
    {
        public CursorDrawingTimingMethodOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CursorDrawingTimingMethods>()
            {
                Value = CursorDrawingTimingMethods.ByHidReportUpdatedEvent,
                DescriptionKey = nameof(Resources.CursorDrawingTimingMethodDetail_ByHidReportUpdatedEvent_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<CursorDrawingTimingMethods>()
            {
                Value = CursorDrawingTimingMethods.ByTimer60Fps,
                DescriptionKey = nameof(Resources.CursorDrawingTimingMethodDetail_ByTimer60Fps_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<CursorDrawingTimingMethods>()
            {
                Value = CursorDrawingTimingMethods.ByTimer30Fps,
                DescriptionKey = nameof(Resources.CursorDrawingTimingMethodDetail_ByTimer30Fps_Description)
            });
        }
    }

    public enum CameraViewWindowStateHostApplicationsControlMethods
    {
        UseUsersControlMethods,
        KeepNormal,
        KeepMinimized,
    }
    public class CameraViewWindowStateHostApplicationsControlMethodOptions : EnumValueWithDescriptionOptions<CameraViewWindowStateHostApplicationsControlMethods>
    {
        public CameraViewWindowStateHostApplicationsControlMethodOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateHostApplicationsControlMethods>()
            {
                Value = CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods,
                DescriptionKey = nameof(Resources.CameraViewWindowStateHostApplicationsControlMethodDetail_UseUsersControlMethods_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateHostApplicationsControlMethods>()
            {
                Value = CameraViewWindowStateHostApplicationsControlMethods.KeepNormal,
                DescriptionKey = nameof(Resources.CameraViewWindowStateHostApplicationsControlMethodDetail_KeepNormal_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateHostApplicationsControlMethods>()
            {
                Value = CameraViewWindowStateHostApplicationsControlMethods.KeepMinimized,
                DescriptionKey = nameof(Resources.CameraViewWindowStateHostApplicationsControlMethodDetail_KeepMinimized_Description)
            });
        }
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
    public class CameraViewWindowStateUsersControlMethodOptions : EnumValueWithDescriptionOptions<CameraViewWindowStateUsersControlMethods>
    {
        public CameraViewWindowStateUsersControlMethodOptions()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
            {
                Value = CameraViewWindowStateUsersControlMethods.ManualOnOff,
                DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ManualOnOff_Description)
            });
            OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
            {
                Value = CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandTrackingStart,
                DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenFaceDetectionStarts_HideSoonAfterHandTrackingStarts_Description)
            });

            if (ApplicationCommonSettings.IsDeveloperRelease)
            {
                OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
                {
                    Value = CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart,
                    DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenFaceDetectionStarts_HideSoonAfterHandDetectionStarts_ShowWhenHandTrackingStarts_Description)
                });
                OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
                {
                    Value = CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart,
                    DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenFaceDetectionStarts_HideSoonAfterHandDetectionStarts_ShowWhenHandTrackingStarts_HideSoonAfterHandTrackingStarts_Description)
                });
                OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
                {
                    Value = CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideWhenHandTrackingEnd,
                    DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandDetectionStarts_HideWhenHandDetectionEnds_HideWhenHandTrackingEnds_Description)
                });
                OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
                {
                    Value = CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideSoonAfterHandTrackingStart,
                    DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandDetectionStarts_HideWhenHandDetectionEnds_HideSoonAfterHandTrackingStarts_Description)
                });
                OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
                {
                    Value = CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideWhenHandTrackingEnd,
                    DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandTrackingStarts_HideWhenHandTrackingEnds_Description)
                });
            }

            OptionalValue.Options.Add(new ValueWithDescription<CameraViewWindowStateUsersControlMethods>()
            {
                Value = CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart,
                DescriptionKey = nameof(Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandTrackingStarts_HideSoonAfterHandTrackingStarts_Description)
            });
        }
    }

    public class CursorImageSetInformation
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public System.Windows.Media.Imaging.BitmapSource SampleImageSource { get; set; }
    }

    internal class CameraViewImageSetInformation
    {
        public int Index { get; set; }
        public string Description { get; set; }
        public System.Windows.Media.Imaging.BitmapSource SampleImageSource { get; set; }
    }
}

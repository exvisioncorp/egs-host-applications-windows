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

    public class EgsHostOptionalPropertyDetailBase
    {
        // NOTE: Do not implement IPropertyChanged here.  Use only for Optional<T>.  All strings are written in Resources.  Before I used "[DataMember]", but it is obsoleted.
        public string DescriptionKey { get; internal set; }
        public string Description { get { return string.IsNullOrEmpty(DescriptionKey) ? "" : Resources.ResourceManager.GetString(DescriptionKey, Resources.Culture); } }
        public override string ToString() { return Description; }
    }

    public class CameraViewBordersAndPointersAreDrawnByDetail : EgsHostOptionalPropertyDetailBase
    {
        public CameraViewBordersAndPointersAreDrawnByKind EnumValue { get; internal set; }

        public static List<CameraViewBordersAndPointersAreDrawnByDetail> GetDefaultList()
        {
            var ret = new List<CameraViewBordersAndPointersAreDrawnByDetail>();
            ret.Add(new CameraViewBordersAndPointersAreDrawnByDetail()
            {
                EnumValue = CameraViewBordersAndPointersAreDrawnByKind.HostApplication,
                DescriptionKey = Name.Of(() => Resources.CameraViewBordersAndPointersAreDrawnByDetail_ByHostApplication_Description)
            });
            ret.Add(new CameraViewBordersAndPointersAreDrawnByDetail()
            {
                EnumValue = CameraViewBordersAndPointersAreDrawnByKind.Device,
                DescriptionKey = Name.Of(() => Resources.CameraViewBordersAndPointersAreDrawnByDetail_ByDevice_Description)
            });
            return ret;
        }
    }

    public class CultureInfoAndDescriptionDetail : EgsHostOptionalPropertyDetailBase
    {
        public string CultureInfoString { get; internal set; }

        public static List<CultureInfoAndDescriptionDetail> GetDefaultList()
        {
            var ret = new List<CultureInfoAndDescriptionDetail>();
            ret.Add(new CultureInfoAndDescriptionDetail()
            {
                CultureInfoString = "",
                DescriptionKey = Name.Of(() => Resources.CultureInfoAndDescriptionDetail_UseOSCulture_Description)
            });
            ret.Add(new CultureInfoAndDescriptionDetail()
            {
                CultureInfoString = "en",
                DescriptionKey = Name.Of(() => Resources.CultureInfoAndDescriptionDetail_English_Description)
            });
            ret.Add(new CultureInfoAndDescriptionDetail()
            {
                CultureInfoString = "ja",
                DescriptionKey = Name.Of(() => Resources.CultureInfoAndDescriptionDetail_Japanese_Description)
            });
            ret.Add(new CultureInfoAndDescriptionDetail()
            {
                CultureInfoString = "zh-Hans",
                DescriptionKey = Name.Of(() => Resources.CultureInfoAndDescriptionDetail_Chinese_Description)
            });
            return ret;
        }
    }

    public class MouseCursorPositionUpdatedByGestureCursorMethodDetail : EgsHostOptionalPropertyDetailBase
    {
        public MouseCursorPositionUpdatedByGestureCursorMethods EnumValue { get; set; }
        public static List<MouseCursorPositionUpdatedByGestureCursorMethodDetail> GetDefaultList()
        {
            var ret = new List<MouseCursorPositionUpdatedByGestureCursorMethodDetail>();
            ret.Add(new MouseCursorPositionUpdatedByGestureCursorMethodDetail()
            {
                EnumValue = MouseCursorPositionUpdatedByGestureCursorMethods.None,
                DescriptionKey = Name.Of(() => Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_None_Description)
            });
            ret.Add(new MouseCursorPositionUpdatedByGestureCursorMethodDetail()
            {
                EnumValue = MouseCursorPositionUpdatedByGestureCursorMethods.FirstFoundHand,
                DescriptionKey = Name.Of(() => Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_FirstFoundHand_Description)
            });
            ret.Add(new MouseCursorPositionUpdatedByGestureCursorMethodDetail()
            {
                EnumValue = MouseCursorPositionUpdatedByGestureCursorMethods.RightHand,
                DescriptionKey = Name.Of(() => Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_RightHand_Description)
            });
            ret.Add(new MouseCursorPositionUpdatedByGestureCursorMethodDetail()
            {
                EnumValue = MouseCursorPositionUpdatedByGestureCursorMethods.LeftHand,
                DescriptionKey = Name.Of(() => Resources.MouseCursorPositionUpdatedByGestureCursorMethodDetail_LeftHand_Description)
            });
            return ret;
        }
    }

    public class CursorDrawingTimingMethodDetail : EgsHostOptionalPropertyDetailBase
    {
        public CursorDrawingTimingMethods EnumValue { get; set; }
        public static List<CursorDrawingTimingMethodDetail> GetDefaultList()
        {
            var ret = new List<CursorDrawingTimingMethodDetail>();
            ret.Add(new CursorDrawingTimingMethodDetail()
            {
                EnumValue = CursorDrawingTimingMethods.ByHidReportUpdatedEvent,
                DescriptionKey = Name.Of(() => Resources.CursorDrawingTimingMethodDetail_ByHidReportUpdatedEvent_Description)
            });
            ret.Add(new CursorDrawingTimingMethodDetail()
            {
                EnumValue = CursorDrawingTimingMethods.ByTimer60Fps,
                DescriptionKey = Name.Of(() => Resources.CursorDrawingTimingMethodDetail_ByTimer60Fps_Description)
            });
            ret.Add(new CursorDrawingTimingMethodDetail()
            {
                EnumValue = CursorDrawingTimingMethods.ByTimer30Fps,
                DescriptionKey = Name.Of(() => Resources.CursorDrawingTimingMethodDetail_ByTimer30Fps_Description)
            });
            return ret;
        }
    }

    public class CameraViewWindowStateHostApplicationsControlMethodDetail : EgsHostOptionalPropertyDetailBase
    {
        public CameraViewWindowStateHostApplicationsControlMethods EnumValue { get; internal set; }

        public static List<CameraViewWindowStateHostApplicationsControlMethodDetail> GetDefaultList()
        {
            var ret = new List<CameraViewWindowStateHostApplicationsControlMethodDetail>();
            ret.Add(new CameraViewWindowStateHostApplicationsControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateHostApplicationsControlMethodDetail_UseUsersControlMethods_Description)
            });
            ret.Add(new CameraViewWindowStateHostApplicationsControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateHostApplicationsControlMethods.KeepNormal,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateHostApplicationsControlMethodDetail_KeepNormal_Description)
            });
            ret.Add(new CameraViewWindowStateHostApplicationsControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateHostApplicationsControlMethods.KeepMinimized,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateHostApplicationsControlMethodDetail_KeepMinimized_Description)
            });
            return ret;
        }
    }

    public class CameraViewWindowStateUsersControlMethodDetail : EgsHostOptionalPropertyDetailBase
    {
        public CameraViewWindowStateUsersControlMethods EnumValue { get; internal set; }

        public static List<CameraViewWindowStateUsersControlMethodDetail> GetDefaultList()
        {
            var ret = new List<CameraViewWindowStateUsersControlMethodDetail>();
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ManualOnOff,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ManualOnOff_Description)
            });
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandTrackingStart,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenFaceDetectionStarts_HideSoonAfterHandTrackingStarts_Description)
            });
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenFaceDetectionStarts_HideSoonAfterHandDetectionStarts_ShowWhenHandTrackingStarts_Description)
            });
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ShowWhenFaceDetectionStart_HideSoonAfterHandDetectionStart_ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenFaceDetectionStarts_HideSoonAfterHandDetectionStarts_ShowWhenHandTrackingStarts_HideSoonAfterHandTrackingStarts_Description)
            });
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideWhenHandTrackingEnd,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandDetectionStarts_HideWhenHandDetectionEnds_HideWhenHandTrackingEnds_Description)
            });
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ShowWhenHandDetectionStart_HideWhenHandDetectionEnd_HideSoonAfterHandTrackingStart,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandDetectionStarts_HideWhenHandDetectionEnds_HideSoonAfterHandTrackingStarts_Description)
            });
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideWhenHandTrackingEnd,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandTrackingStarts_HideWhenHandTrackingEnds_Description)
            });
            ret.Add(new CameraViewWindowStateUsersControlMethodDetail()
            {
                EnumValue = CameraViewWindowStateUsersControlMethods.ShowWhenHandTrackingStart_HideSoonAfterHandTrackingStart,
                DescriptionKey = Name.Of(() => Resources.CameraViewWindowStateUsersControlMethodDetail_ShowWhenHandTrackingStarts_HideSoonAfterHandTrackingStarts_Description)
            });
            return ret;
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

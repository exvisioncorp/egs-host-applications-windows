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
    using System.Globalization;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;

    public enum HardwareTypes : byte
    {
        NoInformation = 0,
        ZkooForKickstarterBackers = 1,
        MA2150_IMX208 = 2,
        MA2150_IMX208_PIC = 3,
    }
    public partial class HardwareTypeOptions : HidAccessPropertyEnumValue<HardwareTypes>
    {
        public HardwareTypeOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<HardwareTypes>() { Value = (HardwareTypes)0, DescriptionKey = nameof(Resources.EgsDevice_HardwareType_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<HardwareTypes>() { Value = (HardwareTypes)1, DescriptionKey = nameof(Resources.EgsDevice_HardwareType_Options_1_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<HardwareTypes>() { Value = (HardwareTypes)2, DescriptionKey = nameof(Resources.EgsDevice_HardwareType_Options_2_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<HardwareTypes>() { Value = (HardwareTypes)3, DescriptionKey = nameof(Resources.EgsDevice_HardwareType_Options_3_DescriptionKey) });
        }
    }

    public enum PowerModes : byte
    {
        Active = 0,
        StandingBy = 1,
    }
    public partial class PowerModeOptions : HidAccessPropertyEnumValue<PowerModes>
    {
        public PowerModeOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<PowerModes>() { Value = (PowerModes)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_PowerMode_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<PowerModes>() { Value = (PowerModes)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_PowerMode_Options_1_DescriptionKey) });
        }
    }

    public enum HostMachineOperatingSystems : byte
    {
        Windows = 0,
        Android = 1,
    }
    public partial class HostMachineOperatingSystemOptions : HidAccessPropertyEnumValue<HostMachineOperatingSystems>
    {
        public HostMachineOperatingSystemOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<HostMachineOperatingSystems>() { Value = (HostMachineOperatingSystems)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_HostMachineOperatingSystem_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<HostMachineOperatingSystems>() { Value = (HostMachineOperatingSystems)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_HostMachineOperatingSystem_Options_1_DescriptionKey) });
        }
    }

    public enum TouchInterfaceKinds : byte
    {
        MultiTouch = 0,
        Mouse = 2,
        SingleTouch = 1,
    }
    public partial class TouchInterfaceKindOptions : HidAccessPropertyEnumValue<TouchInterfaceKinds>
    {
        public TouchInterfaceKindOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<TouchInterfaceKinds>() { Value = (TouchInterfaceKinds)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_TouchInterfaceKind_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<TouchInterfaceKinds>() { Value = (TouchInterfaceKinds)2, DescriptionKey = nameof(Resources.EgsDeviceSettings_TouchInterfaceKind_Options_2_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<TouchInterfaceKinds>() { Value = (TouchInterfaceKinds)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_TouchInterfaceKind_Options_1_DescriptionKey) });
        }
    }

    public enum DeviceRotationAngleInClockwises : byte
    {
        Landscape_0 = 0,
        PortraitFlipped_90 = 1,
        LandscapeFlipped_180 = 2,
        Portrait_270 = 3,
    }
    public partial class DeviceRotationAngleInClockwiseOptions : HidAccessPropertyEnumValue<DeviceRotationAngleInClockwises>
    {
        public DeviceRotationAngleInClockwiseOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<DeviceRotationAngleInClockwises>() { Value = (DeviceRotationAngleInClockwises)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceRotationAngleInClockwise_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<DeviceRotationAngleInClockwises>() { Value = (DeviceRotationAngleInClockwises)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceRotationAngleInClockwise_Options_1_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<DeviceRotationAngleInClockwises>() { Value = (DeviceRotationAngleInClockwises)2, DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceRotationAngleInClockwise_Options_2_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<DeviceRotationAngleInClockwises>() { Value = (DeviceRotationAngleInClockwises)3, DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceRotationAngleInClockwise_Options_3_DescriptionKey) });
        }
    }

    public enum DeviceUsages : byte
    {
        RemoteTouch = 0,
        MotionControl = 2,
        WebCamera = 1,
    }
    public partial class DeviceUsageOptions : HidAccessPropertyEnumValue<DeviceUsages>
    {
        public DeviceUsageOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<DeviceUsages>() { Value = (DeviceUsages)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceUsage_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<DeviceUsages>() { Value = (DeviceUsages)2, DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceUsage_Options_2_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<DeviceUsages>() { Value = (DeviceUsages)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceUsage_Options_1_DescriptionKey) });
        }
    }

    public enum ModelSetIds : byte
    {
        FiveFingersForTv = 0,
    }
    public partial class ModelSetIdOptions : HidAccessPropertyEnumValue<ModelSetIds>
    {
        public ModelSetIdOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<ModelSetIds>() { Value = 0, DescriptionKey = nameof(Resources.EgsDeviceSettings_ModelSetId_Options_0_DescriptionKey) });
        }
    }

    public enum CaptureBinnings : byte
    {
        NoBinning = 1,
        Binning2x2 = 2,
        Binning3x3 = 3,
        Binning4x4 = 4,
    }
    public partial class CaptureBinningOptions : HidAccessPropertyEnumValue<CaptureBinnings>
    {
        public CaptureBinningOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CaptureBinnings>() { Value = (CaptureBinnings)2, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureBinning_Options_2_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<CaptureBinnings>() { Value = (CaptureBinnings)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureBinning_Options_1_DescriptionKey) });
            if (ApplicationCommonSettings.IsInternalRelease)
            {
                OptionalValue.Options.Add(new ValueWithDescription<CaptureBinnings>() { Value = (CaptureBinnings)3, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureBinning_Options_3_DescriptionKey) });
                OptionalValue.Options.Add(new ValueWithDescription<CaptureBinnings>() { Value = (CaptureBinnings)4, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureBinning_Options_4_DescriptionKey) });
            }
        }
    }

    public enum CaptureFpsKind : byte
    {
        Auto = 0,
        Fps120 = 120,
        Fps100 = 100,
    }
    public partial class CaptureFpsOptions : HidAccessPropertyEnumValue<CaptureFpsKind>
    {
        public CaptureFpsOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CaptureFpsKind>() { Value = (CaptureFpsKind)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureFps_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<CaptureFpsKind>() { Value = (CaptureFpsKind)120, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureFps_Options_120_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<CaptureFpsKind>() { Value = (CaptureFpsKind)100, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureFps_Options_100_DescriptionKey) });
        }
    }

    public enum CaptureExposureModes : byte
    {
        Auto = 0,
        Manual = 1,
    }
    public partial class CaptureExposureModeOptions : HidAccessPropertyEnumValue<CaptureExposureModes>
    {
        public CaptureExposureModeOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CaptureExposureModes>() { Value = (CaptureExposureModes)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureExposureMode_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<CaptureExposureModes>() { Value = (CaptureExposureModes)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureExposureMode_Options_1_DescriptionKey) });
        }
    }

    public enum CameraViewImageSourceBitmapSizes : byte
    {
        Size_320x240 = 0,
        Size_384x240 = 1,
        Size_640x480 = 2,
    }
    public class HidAccessPropertyOptionalTypeParameterSize<T> : HidAccessPropertyOptionalTypeParameterBase<T>
        where T : IComparable
    {
        public int Width { get; set; }
        public int Height { get; set; }
        public System.Drawing.Size Size { get { return new System.Drawing.Size(Width, Height); } }
        public override string ToString() { return Width + "x" + Height; }
    }
    public partial class CameraViewImageSourceBitmapSizeDetail : HidAccessPropertyOptionalTypeParameterSize<CameraViewImageSourceBitmapSizes>
    {
        public override string Description { get { return string.Format(CultureInfo.InvariantCulture, "{0}: {1} x {2}", Value, Width, Height); } }
    }
    public partial class CameraViewImageSourceBitmapSizeOptions : HidAccessPropertyOptional<CameraViewImageSourceBitmapSizeDetail, CameraViewImageSourceBitmapSizes>
    {
        public CameraViewImageSourceBitmapSizeOptions()
            : base()
        {
            OptionalValue.Options.Add(new CameraViewImageSourceBitmapSizeDetail() { Value = (CameraViewImageSourceBitmapSizes)0, Width = 320, Height = 240, DescriptionKey = nameof(Resources.EgsDeviceSettings_CameraViewImageSourceBitmapSize_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new CameraViewImageSourceBitmapSizeDetail() { Value = (CameraViewImageSourceBitmapSizes)1, Width = 384, Height = 240, DescriptionKey = nameof(Resources.EgsDeviceSettings_CameraViewImageSourceBitmapSize_Options_1_DescriptionKey) });
            OptionalValue.Options.Add(new CameraViewImageSourceBitmapSizeDetail() { Value = (CameraViewImageSourceBitmapSizes)2, Width = 640, Height = 480, DescriptionKey = nameof(Resources.EgsDeviceSettings_CameraViewImageSourceBitmapSize_Options_2_DescriptionKey) });
        }
    }

    public enum FaceDetectionMethods
    {
        DefaultProcessOnEgsDevice = 0,
        DefaultProcessOnEgsHostApplication = 1,
        SdkUserProcess = 2,
    }
    public partial class FaceDetectionMethodOptions : EnumValueWithDescriptionOptions<FaceDetectionMethods>
    {
        public FaceDetectionMethodOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<FaceDetectionMethods>() { Value = (FaceDetectionMethods)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceDetectionMethod_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<FaceDetectionMethods>() { Value = (FaceDetectionMethods)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceDetectionMethod_Options_1_DescriptionKey) });
            if (ApplicationCommonSettings.IsDeveloperRelease)
            {
                OptionalValue.Options.Add(new ValueWithDescription<FaceDetectionMethods>() { Value = (FaceDetectionMethods)2, DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceDetectionMethod_Options_2_DescriptionKey) });
            }
        }
    }

    public enum FaceSelectionOnDeviceMethods
    {
        Auto = 0,
        OneByOne = 1,
        MostCenter = 2,
    }
    public partial class FaceSelectionOnDeviceMethodOptions : HidAccessPropertyEnumValue<FaceSelectionOnDeviceMethods>
    {
        public FaceSelectionOnDeviceMethodOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<FaceSelectionOnDeviceMethods>() { Value = (FaceSelectionOnDeviceMethods)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceSelectionOnDeviceMethod_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<FaceSelectionOnDeviceMethods>() { Value = (FaceSelectionOnDeviceMethods)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceSelectionOnDeviceMethod_Options_1_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<FaceSelectionOnDeviceMethods>() { Value = (FaceSelectionOnDeviceMethods)2, DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceSelectionOnDeviceMethod_Options_2_DescriptionKey) });
        }
    }

    public enum CursorSpeedAndPrecisionModes
    {
        Beginner = 0,
        Standard = 1,
        FruitNinja = 2,
        Reserved3 = 3,
        Reserved4 = 4,
        Reserved5 = 5,
        Reserved6 = 6,
        Reserved7 = 7,
    }
    public partial class CursorSpeedAndPrecisionModeOptions : HidAccessPropertyEnumValue<CursorSpeedAndPrecisionModes>
    {
        public CursorSpeedAndPrecisionModeOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_CursorSpeedAndPrecisionMode_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_CursorSpeedAndPrecisionMode_Options_1_DescriptionKey) });
            if (ApplicationCommonSettings.HostApplicationName == "ZKOO" || ApplicationCommonSettings.IsDeveloperRelease)
            {
                OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)2, DescriptionKey = nameof(Resources.EgsDeviceSettings_CursorSpeedAndPrecisionMode_Options_2_DescriptionKey) });
            }
            if (ApplicationCommonSettings.IsInternalRelease)
            {
                OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)3, DescriptionKey = "" });
                OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)4, DescriptionKey = "" });
                OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)5, DescriptionKey = "" });
                OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)6, DescriptionKey = "" });
                OptionalValue.Options.Add(new ValueWithDescription<CursorSpeedAndPrecisionModes>() { Value = (CursorSpeedAndPrecisionModes)7, DescriptionKey = "" });
            }
        }
    }

    public enum FastMovingHandsGestureModes
    {
        None = 0,
        Touch = 1,
    }
    public partial class FastMovingHandsGestureModeOptions : HidAccessPropertyEnumValue<FastMovingHandsGestureModes>
    {
        public FastMovingHandsGestureModeOptions()
            : base()
        {
            OptionalValue.Options.Add(new ValueWithDescription<FastMovingHandsGestureModes>() { Value = (FastMovingHandsGestureModes)0, DescriptionKey = nameof(Resources.EgsDeviceSettings_FastMovingHandsGestureMode_Options_0_DescriptionKey) });
            OptionalValue.Options.Add(new ValueWithDescription<FastMovingHandsGestureModes>() { Value = (FastMovingHandsGestureModes)1, DescriptionKey = nameof(Resources.EgsDeviceSettings_FastMovingHandsGestureMode_Options_1_DescriptionKey) });
        }
    }
}

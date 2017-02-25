namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Linq;
    using System.Runtime.Serialization;
    using System.Windows;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDevice
    {
        public HidAccessPropertyString DeviceSerialNumber { get; private set; }
        public HidAccessPropertyOptional<HardwareTypeDetail> HardwareType { get; private set; }
        public HidAccessPropertyInt32Array FirmwareVersion { get; private set; }
        internal HidAccessPropertySingle TemperatureInCelsius { get; private set; }
        internal HidAccessPropertySingle TemperatureInFahrenheit { get; private set; }

        void CreateProperties()
        {
            DeviceSerialNumber = new HidAccessPropertyString() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x10, ValueTypeOnDevice = "byte", DataLength = 12, IsReadOnly = true, NameOfProperty = "DeviceSerialNumber", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDevice_DeviceSerialNumber_Description) };
            HardwareType = new HidAccessPropertyOptional<HardwareTypeDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x1A, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = true, NameOfProperty = "HardwareType", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDevice_HardwareType_Description) }; HardwareType.OptionalValue.Options = HardwareTypeDetail.GetDefaultList(); HardwareType.InitializeOnceAtStartup();
            FirmwareVersion = new HidAccessPropertyInt32Array() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x20, ValueTypeOnDevice = "int", DataLength = 4, IsReadOnly = true, NameOfProperty = "FirmwareVersion", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDevice_FirmwareVersion_Description) };
            TemperatureInCelsius = new HidAccessPropertySingle() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x25, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "TemperatureInCelsius", AvailableFirmwareVersion = new Version("1.1"), DescriptionKey = Name.Of(() => Resources.EgsDevice_TemperatureInCelsius_Description) };
            TemperatureInFahrenheit = new HidAccessPropertySingle() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x27, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "TemperatureInFahrenheit", AvailableFirmwareVersion = new Version("1.1"), DescriptionKey = Name.Of(() => Resources.EgsDevice_TemperatureInFahrenheit_Description) };
        }

        void AddPropertiesToHidAccessPropertyList()
        {
            HidAccessPropertyList = new List<HidAccessPropertyBase>();
            HidAccessPropertyList.Add(DeviceSerialNumber);
            HidAccessPropertyList.Add(HardwareType);
            HidAccessPropertyList.Add(FirmwareVersion);
            HidAccessPropertyList.Add(TemperatureInCelsius);
            HidAccessPropertyList.Add(TemperatureInFahrenheit);
        }

        internal void InitializePropertiesByDefaultValue()
        {
        }
    }

    public partial class EgsDeviceSettings
    {
        [DataMember]
        internal HidAccessPropertyInt32 UsbProtocolRevision { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<PowerModeDetail> PowerMode { get; private set; }
        public HidAccessPropertyOptional<HostMachineOperatingSystemDetail> HostMachineOperatingSystem { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<TouchInterfaceKindDetail> TouchInterfaceKind { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<DeviceRotationAngleInClockwiseDetail> DeviceRotationAngleInClockwise { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToDetectFaces { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToDetectHands { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToSendTouchScreenHidReport { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToSendHoveringStateOnTouchScreenHidReport { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToSendEgsGestureHidReport { get; private set; }
        public HidAccessPropertyByte TrackableHandsCount { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<CaptureBinningDetail> CaptureBinning { get; private set; }
        public HidAccessPropertySize CaptureImageSize { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<CaptureFpsDetail> CaptureFps { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<CaptureExposureModeDetail> CaptureExposureMode { get; private set; }
        [DataMember]
        public HidAccessPropertySingleArray CaptureManualExposureParameters { get; private set; }
        public HidAccessPropertySingle LensEquivalentFocalLengthInMillimeters { get; private set; }
        public HidAccessPropertySingle SensorOnePixelSideLengthInMillimeters { get; private set; }
        public HidAccessPropertySingle LensFNumber { get; private set; }
        public HidAccessPropertySingle SensorExposureTimeInMilliseconds { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<CameraViewImageSourceBitmapSizeDetail> CameraViewImageSourceBitmapSize { get; private set; }
        public HidAccessPropertyRect CameraViewImageSourceRectInCapturedImage { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToDrawBordersOnCameraViewImageByDevice { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<FaceSelectionMethodKindDetail> FaceSelectionMethodKind { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToFixHandDetectionRegions { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect RightHandDetectionAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyRangedInt RightHandDetectionScaleOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect LeftHandDetectionAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyRangedInt LeftHandDetectionScaleOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToFixScreenMappedAreas { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect RightHandScreenMappedAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect LeftHandScreenMappedAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<CursorSpeedAndPrecisionModeDetail> CursorSpeedAndPrecisionMode { get; private set; }
        [DataMember]
        public HidAccessPropertyOptional<FastMovingHandsGestureModeDetail> FastMovingHandsGestureMode { get; private set; }
        public HidAccessPropertySize ScreenMappedAreaResolutionSize { get; private set; }
        public HidAccessPropertySize TouchTargetScreenSize { get; private set; }
        [DataMember]
        public HidAccessPropertyByte StatusLedBrightnessMaximum { get; private set; }

        void CreateProperties()
        {
            UsbProtocolRevision = new HidAccessPropertyInt32() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x00, ValueTypeOnDevice = "int", DataLength = 1, IsReadOnly = false, NameOfProperty = "UsbProtocolRevision", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_UsbProtocolRevision_Description) };
            PowerMode = new HidAccessPropertyOptional<PowerModeDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x2A, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "PowerMode", AvailableFirmwareVersion = new Version("1.1.7907"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_PowerMode_Description) }; PowerMode.OptionalValue.Options = PowerModeDetail.GetDefaultList(); PowerMode.InitializeOnceAtStartup();
            HostMachineOperatingSystem = new HidAccessPropertyOptional<HostMachineOperatingSystemDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x30, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "HostMachineOperatingSystem", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_HostMachineOperatingSystem_Description) }; HostMachineOperatingSystem.OptionalValue.Options = HostMachineOperatingSystemDetail.GetDefaultList(); HostMachineOperatingSystem.InitializeOnceAtStartup();
            TouchInterfaceKind = new HidAccessPropertyOptional<TouchInterfaceKindDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x40, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "TouchInterfaceKind", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_TouchInterfaceKind_Description) }; TouchInterfaceKind.OptionalValue.Options = TouchInterfaceKindDetail.GetDefaultList(); TouchInterfaceKind.InitializeOnceAtStartup();
            DeviceRotationAngleInClockwise = new HidAccessPropertyOptional<DeviceRotationAngleInClockwiseDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x50, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "DeviceRotationAngleInClockwise", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_DeviceRotationAngleInClockwise_Description) }; DeviceRotationAngleInClockwise.OptionalValue.Options = DeviceRotationAngleInClockwiseDetail.GetDefaultList(); DeviceRotationAngleInClockwise.InitializeOnceAtStartup();
            IsToDetectFaces = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xA0, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToDetectFaces", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToDetectFaces_Description) };
            IsToDetectHands = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xA8, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToDetectHands", AvailableFirmwareVersion = new Version("1.1"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToDetectHands_Description) };
            IsToSendTouchScreenHidReport = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xB0, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToSendTouchScreenHidReport", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToSendTouchScreenHidReport_Description) };
            IsToSendHoveringStateOnTouchScreenHidReport = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xB1, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToSendHoveringStateOnTouchScreenHidReport", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToSendHoveringStateOnTouchScreenHidReport_Description) };
            IsToSendEgsGestureHidReport = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xC0, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToSendEgsGestureHidReport", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToSendEgsGestureHidReport_Description) };
            TrackableHandsCount = new HidAccessPropertyByte() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x10, PropertyId = 0x60, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = true, NameOfProperty = "TrackableHandsCount", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_TrackableHandsCount_Description) };
            CaptureBinning = new HidAccessPropertyOptional<CaptureBinningDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x10, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureBinning", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CaptureBinning_Description) }; CaptureBinning.OptionalValue.Options = CaptureBinningDetail.GetDefaultList(); CaptureBinning.InitializeOnceAtStartup();
            CaptureImageSize = new HidAccessPropertySize() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x20, ValueTypeOnDevice = "int", DataLength = 2, IsReadOnly = true, NameOfProperty = "CaptureImageSize", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CaptureImageSize_Description) };
            CaptureFps = new HidAccessPropertyOptional<CaptureFpsDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x30, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureFps", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CaptureFps_Description) }; CaptureFps.OptionalValue.Options = CaptureFpsDetail.GetDefaultList(); CaptureFps.InitializeOnceAtStartup();
            CaptureExposureMode = new HidAccessPropertyOptional<CaptureExposureModeDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x60, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureExposureMode", AvailableFirmwareVersion = new Version("1.1"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CaptureExposureMode_Description) }; CaptureExposureMode.OptionalValue.Options = CaptureExposureModeDetail.GetDefaultList(); CaptureExposureMode.InitializeOnceAtStartup();
            CaptureManualExposureParameters = new HidAccessPropertySingleArray() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x61, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureManualExposureParameters", AvailableFirmwareVersion = new Version("1.1"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CaptureManualExposureParameters_Description) };
            LensEquivalentFocalLengthInMillimeters = new HidAccessPropertySingle() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA0, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "LensEquivalentFocalLengthInMillimeters", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_LensEquivalentFocalLengthInMillimeters_Description) };
            SensorOnePixelSideLengthInMillimeters = new HidAccessPropertySingle() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA1, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "SensorOnePixelSideLengthInMillimeters", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_SensorOnePixelSideLengthInMillimeters_Description) };
            LensFNumber = new HidAccessPropertySingle() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA2, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "LensFNumber", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_LensFNumber_Description) };
            SensorExposureTimeInMilliseconds = new HidAccessPropertySingle() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA3, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "SensorExposureTimeInMilliseconds", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_SensorExposureTimeInMilliseconds_Description) };
            CameraViewImageSourceBitmapSize = new HidAccessPropertyOptional<CameraViewImageSourceBitmapSizeDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x30, PropertyId = 0x00, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CameraViewImageSourceBitmapSize", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CameraViewImageSourceBitmapSize_Description) }; CameraViewImageSourceBitmapSize.OptionalValue.Options = CameraViewImageSourceBitmapSizeDetail.GetDefaultList(); CameraViewImageSourceBitmapSize.InitializeOnceAtStartup();
            CameraViewImageSourceRectInCapturedImage = new HidAccessPropertyRect() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x30, PropertyId = 0x01, ValueTypeOnDevice = "int", DataLength = 4, IsReadOnly = true, NameOfProperty = "CameraViewImageSourceRectInCapturedImage", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CameraViewImageSourceRectInCapturedImage_Description) };
            IsToDrawBordersOnCameraViewImageByDevice = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x30, PropertyId = 0x02, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToDrawBordersOnCameraViewImageByDevice", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToDrawBordersOnCameraViewImageByDevice_Description) };
            FaceSelectionMethodKind = new HidAccessPropertyOptional<FaceSelectionMethodKindDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x40, PropertyId = 0xA0, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "FaceSelectionMethodKind", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_FaceSelectionMethodKind_Description) }; FaceSelectionMethodKind.OptionalValue.Options = FaceSelectionMethodKindDetail.GetDefaultList(); FaceSelectionMethodKind.InitializeOnceAtStartup();
            IsToFixHandDetectionRegions = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x10, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToFixHandDetectionRegions", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToFixHandDetectionRegions_Description) };
            RightHandDetectionAreaOnFixed = new HidAccessPropertyRatioRect() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x20, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "RightHandDetectionAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_RightHandDetectionAreaOnFixed_Description) }; RightHandDetectionAreaOnFixed.InitializeOnceAtStartup();
            RightHandDetectionScaleOnFixed = new HidAccessPropertyRangedInt() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x21, ValueTypeOnDevice = "int", DataLength = 1, IsReadOnly = false, NameOfProperty = "RightHandDetectionScaleOnFixed", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_RightHandDetectionScaleOnFixed_Description) }; RightHandDetectionScaleOnFixed.InitializeOnceAtStartup();
            LeftHandDetectionAreaOnFixed = new HidAccessPropertyRatioRect() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x30, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "LeftHandDetectionAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_LeftHandDetectionAreaOnFixed_Description) }; LeftHandDetectionAreaOnFixed.InitializeOnceAtStartup();
            LeftHandDetectionScaleOnFixed = new HidAccessPropertyRangedInt() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x31, ValueTypeOnDevice = "int", DataLength = 1, IsReadOnly = false, NameOfProperty = "LeftHandDetectionScaleOnFixed", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_LeftHandDetectionScaleOnFixed_Description) }; LeftHandDetectionScaleOnFixed.InitializeOnceAtStartup();
            IsToFixScreenMappedAreas = new HidAccessPropertyBoolean() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x80, PropertyId = 0x10, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToFixScreenMappedAreas", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_IsToFixScreenMappedAreas_Description) };
            RightHandScreenMappedAreaOnFixed = new HidAccessPropertyRatioRect() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x80, PropertyId = 0x20, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "RightHandScreenMappedAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_RightHandScreenMappedAreaOnFixed_Description) }; RightHandScreenMappedAreaOnFixed.InitializeOnceAtStartup();
            LeftHandScreenMappedAreaOnFixed = new HidAccessPropertyRatioRect() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x80, PropertyId = 0x30, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "LeftHandScreenMappedAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_LeftHandScreenMappedAreaOnFixed_Description) }; LeftHandScreenMappedAreaOnFixed.InitializeOnceAtStartup();
            CursorSpeedAndPrecisionMode = new HidAccessPropertyOptional<CursorSpeedAndPrecisionModeDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xD0, PropertyId = 0x00, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CursorSpeedAndPrecisionMode", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_CursorSpeedAndPrecisionMode_Description) }; CursorSpeedAndPrecisionMode.OptionalValue.Options = CursorSpeedAndPrecisionModeDetail.GetDefaultList(); CursorSpeedAndPrecisionMode.InitializeOnceAtStartup();
            FastMovingHandsGestureMode = new HidAccessPropertyOptional<FastMovingHandsGestureModeDetail>() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xD0, PropertyId = 0x10, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "FastMovingHandsGestureMode", AvailableFirmwareVersion = new Version("1.1.8005"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_FastMovingHandsGestureMode_Description) }; FastMovingHandsGestureMode.OptionalValue.Options = FastMovingHandsGestureModeDetail.GetDefaultList(); FastMovingHandsGestureMode.InitializeOnceAtStartup();
            ScreenMappedAreaResolutionSize = new HidAccessPropertySize() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xE0, PropertyId = 0x10, ValueTypeOnDevice = "int", DataLength = 2, IsReadOnly = true, NameOfProperty = "ScreenMappedAreaResolutionSize", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_ScreenMappedAreaResolutionSize_Description) };
            TouchTargetScreenSize = new HidAccessPropertySize() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xE0, PropertyId = 0x25, ValueTypeOnDevice = "int", DataLength = 2, IsReadOnly = false, NameOfProperty = "TouchTargetScreenSize", AvailableFirmwareVersion = new Version("1.0"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_TouchTargetScreenSize_Description) };
            StatusLedBrightnessMaximum = new HidAccessPropertyByte() { ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xE0, PropertyId = 0x50, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "StatusLedBrightnessMaximum", AvailableFirmwareVersion = new Version("1.1.7916"), DescriptionKey = Name.Of(() => Resources.EgsDeviceSettings_StatusLedBrightnessMaximum_Description) };
        }

        void AddPropertiesToHidAccessPropertyList()
        {
            HidAccessPropertyList = new List<HidAccessPropertyBase>();
            HidAccessPropertyList.Add(UsbProtocolRevision);
            HidAccessPropertyList.Add(PowerMode);
            HidAccessPropertyList.Add(HostMachineOperatingSystem);
            HidAccessPropertyList.Add(TouchInterfaceKind);
            HidAccessPropertyList.Add(DeviceRotationAngleInClockwise);
            HidAccessPropertyList.Add(IsToDetectFaces);
            HidAccessPropertyList.Add(IsToDetectHands);
            HidAccessPropertyList.Add(IsToSendTouchScreenHidReport);
            HidAccessPropertyList.Add(IsToSendHoveringStateOnTouchScreenHidReport);
            HidAccessPropertyList.Add(IsToSendEgsGestureHidReport);
            HidAccessPropertyList.Add(TrackableHandsCount);
            HidAccessPropertyList.Add(CaptureBinning);
            HidAccessPropertyList.Add(CaptureImageSize);
            HidAccessPropertyList.Add(CaptureFps);
            HidAccessPropertyList.Add(CaptureExposureMode);
            HidAccessPropertyList.Add(CaptureManualExposureParameters);
            HidAccessPropertyList.Add(LensEquivalentFocalLengthInMillimeters);
            HidAccessPropertyList.Add(SensorOnePixelSideLengthInMillimeters);
            HidAccessPropertyList.Add(LensFNumber);
            HidAccessPropertyList.Add(SensorExposureTimeInMilliseconds);
            HidAccessPropertyList.Add(CameraViewImageSourceBitmapSize);
            HidAccessPropertyList.Add(CameraViewImageSourceRectInCapturedImage);
            HidAccessPropertyList.Add(IsToDrawBordersOnCameraViewImageByDevice);
            HidAccessPropertyList.Add(FaceSelectionMethodKind);
            HidAccessPropertyList.Add(IsToFixHandDetectionRegions);
            HidAccessPropertyList.Add(RightHandDetectionAreaOnFixed);
            HidAccessPropertyList.Add(RightHandDetectionScaleOnFixed);
            HidAccessPropertyList.Add(LeftHandDetectionAreaOnFixed);
            HidAccessPropertyList.Add(LeftHandDetectionScaleOnFixed);
            HidAccessPropertyList.Add(IsToFixScreenMappedAreas);
            HidAccessPropertyList.Add(RightHandScreenMappedAreaOnFixed);
            HidAccessPropertyList.Add(LeftHandScreenMappedAreaOnFixed);
            HidAccessPropertyList.Add(CursorSpeedAndPrecisionMode);
            HidAccessPropertyList.Add(FastMovingHandsGestureMode);
            HidAccessPropertyList.Add(ScreenMappedAreaResolutionSize);
            HidAccessPropertyList.Add(TouchTargetScreenSize);
            HidAccessPropertyList.Add(StatusLedBrightnessMaximum);
        }

        internal void InitializePropertiesByDefaultValue()
        {
            UsbProtocolRevision.Value = 1;
            PowerMode.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            HostMachineOperatingSystem.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            TouchInterfaceKind.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            DeviceRotationAngleInClockwise.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            IsToDetectFaces.Value = true;
            IsToDetectHands.Value = true;
            IsToSendTouchScreenHidReport.Value = true;
            IsToSendHoveringStateOnTouchScreenHidReport.Value = false;
            IsToSendEgsGestureHidReport.Value = true;
            TrackableHandsCount.Value = 2;
            CaptureBinning.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 2);
            CaptureFps.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            CaptureExposureMode.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            LensEquivalentFocalLengthInMillimeters.Value = 2.92f;
            SensorOnePixelSideLengthInMillimeters.Value = 0.0028f;
            LensFNumber.Value = 2.2f;
            SensorExposureTimeInMilliseconds.Value = 8.0f;
            CameraViewImageSourceBitmapSize.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 1);
            IsToDrawBordersOnCameraViewImageByDevice.Value = false;
            FaceSelectionMethodKind.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            IsToFixHandDetectionRegions.Value = false;
            RightHandDetectionAreaOnFixed.ValueAsString = "0.6, 0.3, 0.9, 0.9";
            RightHandDetectionScaleOnFixed.RangedValue.Minimum = 4;
            RightHandDetectionScaleOnFixed.RangedValue.Maximum = 25;
            RightHandDetectionScaleOnFixed.RangedValue.Value = 15;
            LeftHandDetectionAreaOnFixed.ValueAsString = "0.1, 0.3, 0.4, 0.9";
            LeftHandDetectionScaleOnFixed.RangedValue.Minimum = 4;
            LeftHandDetectionScaleOnFixed.RangedValue.Maximum = 25;
            LeftHandDetectionScaleOnFixed.RangedValue.Value = 15;
            IsToFixScreenMappedAreas.Value = false;
            RightHandScreenMappedAreaOnFixed.ValueAsString = "0.65, 0.5, 0.85, 0.8";
            LeftHandScreenMappedAreaOnFixed.ValueAsString = "0.15, 0.5, 0.35, 0.8";
            CursorSpeedAndPrecisionMode.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            FastMovingHandsGestureMode.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
            ScreenMappedAreaResolutionSize.Width = 7540;
            ScreenMappedAreaResolutionSize.Height = 7540;
            TouchTargetScreenSize.Width = (int)System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            TouchTargetScreenSize.Height = (int)System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            StatusLedBrightnessMaximum.Value = 255;
        }
    }
}

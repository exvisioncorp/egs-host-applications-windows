namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDevice
    {
        public HidAccessPropertyString DeviceSerialNumber { get; private set; }
        public HardwareTypeOptions HardwareType { get; private set; }
        public HidAccessPropertyInt32Array FirmwareVersion { get; private set; }
        public HidAccessPropertySingle TemperatureInCelsius { get; private set; }
        public HidAccessPropertySingle TemperatureInFahrenheit { get; private set; }

        void CreateProperties()
        {
            DeviceSerialNumber = new HidAccessPropertyString() { DescriptionKey = nameof(Resources.EgsDevice_DeviceSerialNumber_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x10, ValueTypeOnDevice = "byte", DataLength = 12, IsReadOnly = true, NameOfProperty = "DeviceSerialNumber", AvailableFirmwareVersion = new Version("1.0") };
            HardwareType = new HardwareTypeOptions() { DescriptionKey = nameof(Resources.EgsDevice_HardwareType_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x1A, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = true, NameOfProperty = "HardwareType", AvailableFirmwareVersion = new Version("1.0") };
            FirmwareVersion = new HidAccessPropertyInt32Array() { DescriptionKey = nameof(Resources.EgsDevice_FirmwareVersion_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x20, ValueTypeOnDevice = "int", DataLength = 4, IsReadOnly = true, NameOfProperty = "FirmwareVersion", AvailableFirmwareVersion = new Version("1.0") };
            TemperatureInCelsius = new HidAccessPropertySingle() { DescriptionKey = nameof(Resources.EgsDevice_TemperatureInCelsius_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x25, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "TemperatureInCelsius", AvailableFirmwareVersion = new Version("1.1") };
            TemperatureInFahrenheit = new HidAccessPropertySingle() { DescriptionKey = nameof(Resources.EgsDevice_TemperatureInFahrenheit_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x27, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "TemperatureInFahrenheit", AvailableFirmwareVersion = new Version("1.1") };
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
        public ValueWithDescription<bool> IsToMonitorTemperature { get; private set; }
        [DataMember]
        public PowerModeOptions PowerMode { get; private set; }
        public HostMachineOperatingSystemOptions HostMachineOperatingSystem { get; private set; }
        [DataMember]
        public TouchInterfaceKindOptions TouchInterfaceKind { get; private set; }
        [DataMember]
        public DeviceRotationAngleInClockwiseOptions DeviceRotationAngleInClockwise { get; private set; }
        [DataMember]
        public DeviceUsageOptions DeviceUsage { get; private set; }
        [DataMember]
        public ValueWithDescription<bool> IsToDetectFaces { get; private set; }
        [DataMember]
        public FaceDetectionMethodOptions FaceDetectionMethod { get; private set; }
        public HidAccessPropertyBoolean IsToDetectFacesOnDevice { get; private set; }
        [DataMember]
        public ValueWithDescription<bool> IsToDetectHands { get; private set; }
        public HidAccessPropertyBoolean IsToDetectHandsOnDevice { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToSendTouchScreenHidReport { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToSendHoveringStateOnTouchScreenHidReport { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToSendEgsGestureHidReport { get; private set; }
        public HidAccessPropertyByte TrackableHandsCount { get; private set; }
        [DataMember]
        public CaptureBinningOptions CaptureBinning { get; private set; }
        public HidAccessPropertySize CaptureImageSize { get; private set; }
        [DataMember]
        public CaptureFpsOptions CaptureFps { get; private set; }
        [DataMember]
        public CaptureExposureModeOptions CaptureExposureMode { get; private set; }
        [DataMember]
        public HidAccessPropertySingleArray CaptureManualExposureParameters { get; private set; }
        public HidAccessPropertySingle LensEquivalentFocalLengthInMillimeters { get; private set; }
        public HidAccessPropertySingle SensorOnePixelSideLengthInMillimeters { get; private set; }
        public HidAccessPropertySingle LensFNumber { get; private set; }
        public HidAccessPropertySingle SensorExposureTimeInMilliseconds { get; private set; }
        [DataMember]
        public CameraViewImageSourceBitmapSizeOptions CameraViewImageSourceBitmapSize { get; private set; }
        public HidAccessPropertyRect CameraViewImageSourceRectInCaptureImage { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToDrawBordersOnCameraViewImageByDevice { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToFixFaceDetectionRegion { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect FaceDetectionAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyRangedIntRange FaceDetectionScaleOnFixed { get; private set; }
        [DataMember]
        public FaceSelectionOnDeviceMethodOptions FaceSelectionOnDeviceMethod { get; private set; }
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
        public HidAccessPropertyBoolean IsToFixHandTrackingAreas { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect RightHandTrackingAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect LeftHandTrackingAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyBoolean IsToFixScreenMappedAreas { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect RightHandScreenMappedAreaOnFixed { get; private set; }
        [DataMember]
        public HidAccessPropertyRatioRect LeftHandScreenMappedAreaOnFixed { get; private set; }
        [DataMember]
        public CursorSpeedAndPrecisionModeOptions CursorSpeedAndPrecisionMode { get; private set; }
        [DataMember]
        public FastMovingHandsGestureModeOptions FastMovingHandsGestureMode { get; private set; }
        public HidAccessPropertySize ScreenMappedAreaResolutionSize { get; private set; }
        public HidAccessPropertySize TouchTargetScreenSize { get; private set; }
        [DataMember]
        public HidAccessPropertyByte StatusLedBrightnessMaximum { get; private set; }

        void CreateProperties()
        {
            UsbProtocolRevision = new HidAccessPropertyInt32() { DescriptionKey = nameof(Resources.EgsDeviceSettings_UsbProtocolRevision_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x00, ValueTypeOnDevice = "int", DataLength = 1, IsReadOnly = false, NameOfProperty = "UsbProtocolRevision", AvailableFirmwareVersion = new Version("1.0") };
            IsToMonitorTemperature = new ValueWithDescription<bool>() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToMonitorTemperature_Description) };
            PowerMode = new PowerModeOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_PowerMode_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x2A, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "PowerMode", AvailableFirmwareVersion = new Version("1.1.7907") };
            HostMachineOperatingSystem = new HostMachineOperatingSystemOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_HostMachineOperatingSystem_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x30, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "HostMachineOperatingSystem", AvailableFirmwareVersion = new Version("1.0") };
            TouchInterfaceKind = new TouchInterfaceKindOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_TouchInterfaceKind_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x40, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "TouchInterfaceKind", AvailableFirmwareVersion = new Version("1.0") };
            DeviceRotationAngleInClockwise = new DeviceRotationAngleInClockwiseOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceRotationAngleInClockwise_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x50, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "DeviceRotationAngleInClockwise", AvailableFirmwareVersion = new Version("1.0") };
            DeviceUsage = new DeviceUsageOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_DeviceUsage_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0x90, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "DeviceUsage", AvailableFirmwareVersion = new Version("1.1.9015") };
            IsToDetectFaces = new ValueWithDescription<bool>() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToDetectFaces_Description) };
            FaceDetectionMethod = new FaceDetectionMethodOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceDetectionMethod_Description) };
            IsToDetectFacesOnDevice = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToDetectFacesOnDevice_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xA0, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToDetectFacesOnDevice", AvailableFirmwareVersion = new Version("1.0") };
            IsToDetectHands = new ValueWithDescription<bool>() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToDetectHands_Description) };
            IsToDetectHandsOnDevice = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToDetectHandsOnDevice_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xA8, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToDetectHandsOnDevice", AvailableFirmwareVersion = new Version("1.1") };
            IsToSendTouchScreenHidReport = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToSendTouchScreenHidReport_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xB0, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToSendTouchScreenHidReport", AvailableFirmwareVersion = new Version("1.0") };
            IsToSendHoveringStateOnTouchScreenHidReport = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToSendHoveringStateOnTouchScreenHidReport_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xB1, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToSendHoveringStateOnTouchScreenHidReport", AvailableFirmwareVersion = new Version("1.0") };
            IsToSendEgsGestureHidReport = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToSendEgsGestureHidReport_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x00, PropertyId = 0xC0, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToSendEgsGestureHidReport", AvailableFirmwareVersion = new Version("1.0") };
            TrackableHandsCount = new HidAccessPropertyByte() { DescriptionKey = nameof(Resources.EgsDeviceSettings_TrackableHandsCount_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x10, PropertyId = 0x60, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = true, NameOfProperty = "TrackableHandsCount", AvailableFirmwareVersion = new Version("1.0") };
            CaptureBinning = new CaptureBinningOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureBinning_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x10, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureBinning", AvailableFirmwareVersion = new Version("1.0") };
            CaptureImageSize = new HidAccessPropertySize() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureImageSize_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x20, ValueTypeOnDevice = "int", DataLength = 2, IsReadOnly = true, NameOfProperty = "CaptureImageSize", AvailableFirmwareVersion = new Version("1.0") };
            CaptureFps = new CaptureFpsOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureFps_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x30, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureFps", AvailableFirmwareVersion = new Version("1.0") };
            CaptureExposureMode = new CaptureExposureModeOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureExposureMode_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x60, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureExposureMode", AvailableFirmwareVersion = new Version("1.1") };
            CaptureManualExposureParameters = new HidAccessPropertySingleArray() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CaptureManualExposureParameters_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0x61, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = false, NameOfProperty = "CaptureManualExposureParameters", AvailableFirmwareVersion = new Version("1.1") };
            LensEquivalentFocalLengthInMillimeters = new HidAccessPropertySingle() { DescriptionKey = nameof(Resources.EgsDeviceSettings_LensEquivalentFocalLengthInMillimeters_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA0, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "LensEquivalentFocalLengthInMillimeters", AvailableFirmwareVersion = new Version("1.0") };
            SensorOnePixelSideLengthInMillimeters = new HidAccessPropertySingle() { DescriptionKey = nameof(Resources.EgsDeviceSettings_SensorOnePixelSideLengthInMillimeters_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA1, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "SensorOnePixelSideLengthInMillimeters", AvailableFirmwareVersion = new Version("1.0") };
            LensFNumber = new HidAccessPropertySingle() { DescriptionKey = nameof(Resources.EgsDeviceSettings_LensFNumber_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA2, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "LensFNumber", AvailableFirmwareVersion = new Version("1.0") };
            SensorExposureTimeInMilliseconds = new HidAccessPropertySingle() { DescriptionKey = nameof(Resources.EgsDeviceSettings_SensorExposureTimeInMilliseconds_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x20, PropertyId = 0xA3, ValueTypeOnDevice = "float", DataLength = 1, IsReadOnly = true, NameOfProperty = "SensorExposureTimeInMilliseconds", AvailableFirmwareVersion = new Version("1.0") };
            CameraViewImageSourceBitmapSize = new CameraViewImageSourceBitmapSizeOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CameraViewImageSourceBitmapSize_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x30, PropertyId = 0x00, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CameraViewImageSourceBitmapSize", AvailableFirmwareVersion = new Version("1.0") };
            CameraViewImageSourceRectInCaptureImage = new HidAccessPropertyRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CameraViewImageSourceRectInCaptureImage_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x30, PropertyId = 0x01, ValueTypeOnDevice = "int", DataLength = 4, IsReadOnly = true, NameOfProperty = "CameraViewImageSourceRectInCaptureImage", AvailableFirmwareVersion = new Version("1.0") };
            IsToDrawBordersOnCameraViewImageByDevice = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToDrawBordersOnCameraViewImageByDevice_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x30, PropertyId = 0x02, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToDrawBordersOnCameraViewImageByDevice", AvailableFirmwareVersion = new Version("1.0") };
            IsToFixFaceDetectionRegion = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToFixFaceDetectionRegion_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x40, PropertyId = 0x10, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToFixFaceDetectionRegion", AvailableFirmwareVersion = new Version("1.1.8120") };
            FaceDetectionAreaOnFixed = new HidAccessPropertyRatioRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceDetectionAreaOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x40, PropertyId = 0x20, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "FaceDetectionAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8120") };
            FaceDetectionScaleOnFixed = new HidAccessPropertyRangedIntRange() { DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceDetectionScaleOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x40, PropertyId = 0x21, ValueTypeOnDevice = "int", DataLength = 2, IsReadOnly = false, NameOfProperty = "FaceDetectionScaleOnFixed", AvailableFirmwareVersion = new Version("1.1.8120") };
            FaceSelectionOnDeviceMethod = new FaceSelectionOnDeviceMethodOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_FaceSelectionOnDeviceMethod_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x40, PropertyId = 0xA0, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "FaceSelectionOnDeviceMethod", AvailableFirmwareVersion = new Version("1.0") };
            IsToFixHandDetectionRegions = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToFixHandDetectionRegions_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x10, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToFixHandDetectionRegions", AvailableFirmwareVersion = new Version("1.1.8109") };
            RightHandDetectionAreaOnFixed = new HidAccessPropertyRatioRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_RightHandDetectionAreaOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x20, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "RightHandDetectionAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109") };
            RightHandDetectionScaleOnFixed = new HidAccessPropertyRangedInt() { DescriptionKey = nameof(Resources.EgsDeviceSettings_RightHandDetectionScaleOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x21, ValueTypeOnDevice = "int", DataLength = 1, IsReadOnly = false, NameOfProperty = "RightHandDetectionScaleOnFixed", AvailableFirmwareVersion = new Version("1.1.8109") };
            LeftHandDetectionAreaOnFixed = new HidAccessPropertyRatioRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_LeftHandDetectionAreaOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x30, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "LeftHandDetectionAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109") };
            LeftHandDetectionScaleOnFixed = new HidAccessPropertyRangedInt() { DescriptionKey = nameof(Resources.EgsDeviceSettings_LeftHandDetectionScaleOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x50, PropertyId = 0x31, ValueTypeOnDevice = "int", DataLength = 1, IsReadOnly = false, NameOfProperty = "LeftHandDetectionScaleOnFixed", AvailableFirmwareVersion = new Version("1.1.8109") };
            IsToFixHandTrackingAreas = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToFixHandTrackingAreas_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x70, PropertyId = 0x10, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToFixHandTrackingAreas", AvailableFirmwareVersion = new Version("1.1.8120") };
            RightHandTrackingAreaOnFixed = new HidAccessPropertyRatioRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_RightHandTrackingAreaOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x70, PropertyId = 0x20, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "RightHandTrackingAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8120") };
            LeftHandTrackingAreaOnFixed = new HidAccessPropertyRatioRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_LeftHandTrackingAreaOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x70, PropertyId = 0x30, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "LeftHandTrackingAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8120") };
            IsToFixScreenMappedAreas = new HidAccessPropertyBoolean() { DescriptionKey = nameof(Resources.EgsDeviceSettings_IsToFixScreenMappedAreas_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x80, PropertyId = 0x10, ValueTypeOnDevice = "bool", DataLength = 1, IsReadOnly = false, NameOfProperty = "IsToFixScreenMappedAreas", AvailableFirmwareVersion = new Version("1.1.8109") };
            RightHandScreenMappedAreaOnFixed = new HidAccessPropertyRatioRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_RightHandScreenMappedAreaOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x80, PropertyId = 0x20, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "RightHandScreenMappedAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109") };
            LeftHandScreenMappedAreaOnFixed = new HidAccessPropertyRatioRect() { DescriptionKey = nameof(Resources.EgsDeviceSettings_LeftHandScreenMappedAreaOnFixed_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0x80, PropertyId = 0x30, ValueTypeOnDevice = "float", DataLength = 4, IsReadOnly = false, NameOfProperty = "LeftHandScreenMappedAreaOnFixed", AvailableFirmwareVersion = new Version("1.1.8109") };
            CursorSpeedAndPrecisionMode = new CursorSpeedAndPrecisionModeOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_CursorSpeedAndPrecisionMode_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xD0, PropertyId = 0x00, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "CursorSpeedAndPrecisionMode", AvailableFirmwareVersion = new Version("1.0") };
            FastMovingHandsGestureMode = new FastMovingHandsGestureModeOptions() { DescriptionKey = nameof(Resources.EgsDeviceSettings_FastMovingHandsGestureMode_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xD0, PropertyId = 0x10, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "FastMovingHandsGestureMode", AvailableFirmwareVersion = new Version("1.1.8005") };
            ScreenMappedAreaResolutionSize = new HidAccessPropertySize() { DescriptionKey = nameof(Resources.EgsDeviceSettings_ScreenMappedAreaResolutionSize_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xE0, PropertyId = 0x10, ValueTypeOnDevice = "int", DataLength = 2, IsReadOnly = true, NameOfProperty = "ScreenMappedAreaResolutionSize", AvailableFirmwareVersion = new Version("1.0") };
            TouchTargetScreenSize = new HidAccessPropertySize() { DescriptionKey = nameof(Resources.EgsDeviceSettings_TouchTargetScreenSize_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xE0, PropertyId = 0x25, ValueTypeOnDevice = "int", DataLength = 2, IsReadOnly = false, NameOfProperty = "TouchTargetScreenSize", AvailableFirmwareVersion = new Version("1.0") };
            StatusLedBrightnessMaximum = new HidAccessPropertyByte() { DescriptionKey = nameof(Resources.EgsDeviceSettings_StatusLedBrightnessMaximum_Description), ReportId = 0x0B, MessageId = 0x00, CategoryId = 0xE0, PropertyId = 0x50, ValueTypeOnDevice = "byte", DataLength = 1, IsReadOnly = false, NameOfProperty = "StatusLedBrightnessMaximum", AvailableFirmwareVersion = new Version("1.1.7916") };
        }

        void AddPropertiesToHidAccessPropertyList()
        {
            HidAccessPropertyList = new List<HidAccessPropertyBase>();
            HidAccessPropertyList.Add(UsbProtocolRevision);
            HidAccessPropertyList.Add(PowerMode);
            HidAccessPropertyList.Add(HostMachineOperatingSystem);
            HidAccessPropertyList.Add(TouchInterfaceKind);
            HidAccessPropertyList.Add(DeviceRotationAngleInClockwise);
            HidAccessPropertyList.Add(DeviceUsage);
            HidAccessPropertyList.Add(IsToDetectFacesOnDevice);
            HidAccessPropertyList.Add(IsToDetectHandsOnDevice);
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
            HidAccessPropertyList.Add(CameraViewImageSourceRectInCaptureImage);
            HidAccessPropertyList.Add(IsToDrawBordersOnCameraViewImageByDevice);
            HidAccessPropertyList.Add(IsToFixFaceDetectionRegion);
            HidAccessPropertyList.Add(FaceDetectionAreaOnFixed);
            HidAccessPropertyList.Add(FaceDetectionScaleOnFixed);
            HidAccessPropertyList.Add(FaceSelectionOnDeviceMethod);
            HidAccessPropertyList.Add(IsToFixHandDetectionRegions);
            HidAccessPropertyList.Add(RightHandDetectionAreaOnFixed);
            HidAccessPropertyList.Add(RightHandDetectionScaleOnFixed);
            HidAccessPropertyList.Add(LeftHandDetectionAreaOnFixed);
            HidAccessPropertyList.Add(LeftHandDetectionScaleOnFixed);
            HidAccessPropertyList.Add(IsToFixHandTrackingAreas);
            HidAccessPropertyList.Add(RightHandTrackingAreaOnFixed);
            HidAccessPropertyList.Add(LeftHandTrackingAreaOnFixed);
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
            IsToMonitorTemperature.Value = false;
            PowerMode.Value = 0;
            HostMachineOperatingSystem.Value = 0;
            TouchInterfaceKind.Value = 0;
            DeviceRotationAngleInClockwise.Value = 0;
            DeviceUsage.Value = 0;
            IsToDetectFaces.Value = true;
            IsToDetectFacesOnDevice.Value = false;
            IsToDetectHands.Value = true;
            IsToDetectHandsOnDevice.Value = false;
            IsToSendTouchScreenHidReport.Value = true;
            IsToSendHoveringStateOnTouchScreenHidReport.Value = false;
            IsToSendEgsGestureHidReport.Value = true;
            TrackableHandsCount.Value = 2;
            CaptureBinning.Value = CaptureBinnings.Binning2x2;
            CaptureFps.Value = 0;
            CaptureExposureMode.Value = 0;
            LensEquivalentFocalLengthInMillimeters.Value = 2.92f;
            SensorOnePixelSideLengthInMillimeters.Value = 0.0028f;
            LensFNumber.Value = 2.2f;
            SensorExposureTimeInMilliseconds.Value = 8.0f;
            CameraViewImageSourceBitmapSize.Value = CameraViewImageSourceBitmapSizes.Size_384x240;
            IsToDrawBordersOnCameraViewImageByDevice.Value = false;
            IsToFixFaceDetectionRegion.Value = false;
            FaceDetectionAreaOnFixed.ValueAsString = "0.3, 0.2, 0.7, 0.6";
            FaceDetectionScaleOnFixed.RangedRange.Minimum = 24;
            FaceDetectionScaleOnFixed.RangedRange.Maximum = 240;
            FaceDetectionScaleOnFixed.RangedRange.From = 24;
            FaceDetectionScaleOnFixed.RangedRange.To = 150;
            FaceSelectionOnDeviceMethod.Value = 0;
            IsToFixHandDetectionRegions.Value = false;
            RightHandDetectionAreaOnFixed.ValueAsString = "0.6, 0.3, 0.9, 0.9";
            RightHandDetectionScaleOnFixed.RangedValue.Minimum = 4;
            RightHandDetectionScaleOnFixed.RangedValue.Maximum = 25;
            RightHandDetectionScaleOnFixed.RangedValue.Value = 15;
            LeftHandDetectionAreaOnFixed.ValueAsString = "0.1, 0.3, 0.4, 0.9";
            LeftHandDetectionScaleOnFixed.RangedValue.Minimum = 4;
            LeftHandDetectionScaleOnFixed.RangedValue.Maximum = 25;
            LeftHandDetectionScaleOnFixed.RangedValue.Value = 15;
            IsToFixHandTrackingAreas.Value = false;
            RightHandTrackingAreaOnFixed.ValueAsString = "0.55, 0.3, 0.95, 0.85";
            LeftHandTrackingAreaOnFixed.ValueAsString = "0.05, 0.3, 0.45, 0.85";
            IsToFixScreenMappedAreas.Value = false;
            RightHandScreenMappedAreaOnFixed.ValueAsString = "0.65, 0.5, 0.85, 0.8";
            LeftHandScreenMappedAreaOnFixed.ValueAsString = "0.15, 0.5, 0.35, 0.8";
            CursorSpeedAndPrecisionMode.Value = 0;
            FastMovingHandsGestureMode.Value = 0;
            ScreenMappedAreaResolutionSize.Width = 7540;
            ScreenMappedAreaResolutionSize.Height = 7540;
            StatusLedBrightnessMaximum.Value = 255;
        }
    }
}

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
    using System.Globalization;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;

    public class HidAccessPropertyOptionalTypeParameterBase
    {
        // NOTE: Do not add implementation of IPropertyChanged to this class.  Use this only for Optional<T>.
        public byte Value { get; protected set; }
        public string DescriptionKey { get; protected set; }
        public virtual string Description { get { return Resources.ResourceManager.GetString(DescriptionKey, Resources.Culture); } }
        public override string ToString() { return Description; }
    }

    public class HidAccessPropertyOptionalTypeParameterSize : HidAccessPropertyOptionalTypeParameterBase
    {
        public int Width { get; protected set; }
        public int Height { get; protected set; }
        public System.Drawing.Size Size { get { return new System.Drawing.Size(Width, Height); } }

        public override string ToString() { return Width + "x" + Height; }
    }



    public partial class HardwareTypeDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<HardwareTypeDetail> GetDefaultList()
        {
            var ret = new List<HardwareTypeDetail>();
            ret.Add(new HardwareTypeDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.HardwareTypeDetail_Value0_Description) });
            ret.Add(new HardwareTypeDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.HardwareTypeDetail_Value1_Description) });
            ret.Add(new HardwareTypeDetail() { Value = 2, DescriptionKey = Name.Of(() => Resources.HardwareTypeDetail_Value2_Description) });
            ret.Add(new HardwareTypeDetail() { Value = 3, DescriptionKey = Name.Of(() => Resources.HardwareTypeDetail_Value3_Description) });
            return ret;
        }
    }

    public partial class PowerModeDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<PowerModeDetail> GetDefaultList()
        {
            var ret = new List<PowerModeDetail>();
            ret.Add(new PowerModeDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.PowerModeDetail_Value0_Description) });
            ret.Add(new PowerModeDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.PowerModeDetail_Value1_Description) });
            return ret;
        }
    }

    public partial class HostMachineOperatingSystemDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<HostMachineOperatingSystemDetail> GetDefaultList()
        {
            var ret = new List<HostMachineOperatingSystemDetail>();
            ret.Add(new HostMachineOperatingSystemDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.HostMachineOperatingSystemDetail_Value0_Description) });
            ret.Add(new HostMachineOperatingSystemDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.HostMachineOperatingSystemDetail_Value1_Description) });
            return ret;
        }
    }

    public enum EgsDeviceTouchInterfaceKind : byte
    {
        MultiTouch = 0,
        Mouse = 2,
        SingleTouch = 1,
    }

    public partial class TouchInterfaceKindDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public EgsDeviceTouchInterfaceKind EnumValue
        {
            get
            {
                var ret = (EgsDeviceTouchInterfaceKind)Value;
                return ret;
            }
        }

        public static List<TouchInterfaceKindDetail> GetDefaultList()
        {
            var ret = new List<TouchInterfaceKindDetail>();
            ret.Add(new TouchInterfaceKindDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.TouchInterfaceKindDetail_Value0_Description) });
            ret.Add(new TouchInterfaceKindDetail() { Value = 2, DescriptionKey = Name.Of(() => Resources.TouchInterfaceKindDetail_Value2_Description) });
            ret.Add(new TouchInterfaceKindDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.TouchInterfaceKindDetail_Value1_Description) });
            return ret;
        }
    }

    public partial class DeviceRotationAngleInClockwiseDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<DeviceRotationAngleInClockwiseDetail> GetDefaultList()
        {
            var ret = new List<DeviceRotationAngleInClockwiseDetail>();
            ret.Add(new DeviceRotationAngleInClockwiseDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.DeviceRotationAngleInClockwiseDetail_Value0_Description) });
            ret.Add(new DeviceRotationAngleInClockwiseDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.DeviceRotationAngleInClockwiseDetail_Value1_Description) });
            ret.Add(new DeviceRotationAngleInClockwiseDetail() { Value = 2, DescriptionKey = Name.Of(() => Resources.DeviceRotationAngleInClockwiseDetail_Value2_Description) });
            ret.Add(new DeviceRotationAngleInClockwiseDetail() { Value = 3, DescriptionKey = Name.Of(() => Resources.DeviceRotationAngleInClockwiseDetail_Value3_Description) });
            return ret;
        }
    }

#if false
    public partial class DeviceUsageDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<DeviceUsageDetail> GetDefaultList()
        {
            var ret = new List<DeviceUsageDetail>();
            ret.Add(new DeviceUsageDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.DeviceUsageDetail_Value0_Description) });
            ret.Add(new DeviceUsageDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.DeviceUsageDetail_Value1_Description) });
            return ret;
        }
    }

    public partial class ModelSetIdDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<ModelSetIdDetail> GetDefaultList()
        {
            var ret = new List<ModelSetIdDetail>();
            ret.Add(new ModelSetIdDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.ModelSetIdDetail_Value0_Description) });
            return ret;
        }
    }
#endif

    public partial class CaptureBinningDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<CaptureBinningDetail> GetDefaultList()
        {
            var ret = new List<CaptureBinningDetail>();
            ret.Add(new CaptureBinningDetail() { Value = 2, DescriptionKey = Name.Of(() => Resources.CaptureBinningDetail_Value2_Description) });
            ret.Add(new CaptureBinningDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.CaptureBinningDetail_Value1_Description) });
            if (ApplicationCommonSettings.IsInternalRelease)
            {
                ret.Add(new CaptureBinningDetail() { Value = 3, DescriptionKey = Name.Of(() => Resources.CaptureBinningDetail_Value3_Description) });
                ret.Add(new CaptureBinningDetail() { Value = 4, DescriptionKey = Name.Of(() => Resources.CaptureBinningDetail_Value4_Description) });
            }
            return ret;
        }
    }

    public partial class CaptureFpsDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<CaptureFpsDetail> GetDefaultList()
        {
            var ret = new List<CaptureFpsDetail>();
            ret.Add(new CaptureFpsDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.CaptureFpsDetail_Value0_Description) });
            ret.Add(new CaptureFpsDetail() { Value = 120, DescriptionKey = Name.Of(() => Resources.CaptureFpsDetail_Value120_Description) });
            ret.Add(new CaptureFpsDetail() { Value = 100, DescriptionKey = Name.Of(() => Resources.CaptureFpsDetail_Value100_Description) });
            return ret;
        }
    }

    public partial class CaptureExposureModeDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<CaptureExposureModeDetail> GetDefaultList()
        {
            var ret = new List<CaptureExposureModeDetail>();
            ret.Add(new CaptureExposureModeDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.CaptureExposureModeDetail_Value0_Description) });
            ret.Add(new CaptureExposureModeDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.CaptureExposureModeDetail_Value1_Description) });
            //ret.Add(new CaptureExposureModeDetail() { Value = 2, DescriptionKey = Name.Of(() => Resources.CaptureExposureModeDetail_Value2_Description) });
            return ret;
        }
    }

    public partial class CameraViewImageSourceBitmapSizeDetail : HidAccessPropertyOptionalTypeParameterSize
    {
        public override string Description { get { return string.Format(CultureInfo.InvariantCulture, "{0} x {1}", Width, Height); } }
        public static List<CameraViewImageSourceBitmapSizeDetail> GetDefaultList()
        {
            var ret = new List<CameraViewImageSourceBitmapSizeDetail>();
            ret.Add(new CameraViewImageSourceBitmapSizeDetail() { Value = 0, Width = 320, Height = 240 });
            ret.Add(new CameraViewImageSourceBitmapSizeDetail() { Value = 1, Width = 384, Height = 240 });
            ret.Add(new CameraViewImageSourceBitmapSizeDetail() { Value = 2, Width = 640, Height = 480 });
            return ret;
        }
    }

    public partial class FaceSelectionMethodKindDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<FaceSelectionMethodKindDetail> GetDefaultList()
        {
            var ret = new List<FaceSelectionMethodKindDetail>();
            ret.Add(new FaceSelectionMethodKindDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.FaceSelectionMethodKindDetail_Value0_Description) });
            ret.Add(new FaceSelectionMethodKindDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.FaceSelectionMethodKindDetail_Value1_Description) });
            ret.Add(new FaceSelectionMethodKindDetail() { Value = 2, DescriptionKey = Name.Of(() => Resources.FaceSelectionMethodKindDetail_Value2_Description) });
            return ret;
        }
    }

    public partial class CursorSpeedAndPrecisionModeDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<CursorSpeedAndPrecisionModeDetail> GetDefaultList()
        {
            var ret = new List<CursorSpeedAndPrecisionModeDetail>();
            ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.CursorSpeedAndPrecisionModeDetail_Value0_Description) });
            ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.CursorSpeedAndPrecisionModeDetail_Value1_Description) });
            ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 2, DescriptionKey = Name.Of(() => Resources.CursorSpeedAndPrecisionModeDetail_Value2_Description) });
            if (ApplicationCommonSettings.IsInternalRelease)
            {
                ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 3, DescriptionKey = "" });
                ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 4, DescriptionKey = "" });
                ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 5, DescriptionKey = "" });
                ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 6, DescriptionKey = "" });
                ret.Add(new CursorSpeedAndPrecisionModeDetail() { Value = 7, DescriptionKey = "" });
            }
            return ret;
        }
    }

    public partial class FastMovingHandsGestureModeDetail : HidAccessPropertyOptionalTypeParameterBase
    {
        public static List<FastMovingHandsGestureModeDetail> GetDefaultList()
        {
            var ret = new List<FastMovingHandsGestureModeDetail>();
            ret.Add(new FastMovingHandsGestureModeDetail() { Value = 0, DescriptionKey = Name.Of(() => Resources.FastMovingHandsGestureModeDetail_Value0_Description) });
            ret.Add(new FastMovingHandsGestureModeDetail() { Value = 1, DescriptionKey = Name.Of(() => Resources.FastMovingHandsGestureModeDetail_Value1_Description) });
            return ret;
        }
    }
}

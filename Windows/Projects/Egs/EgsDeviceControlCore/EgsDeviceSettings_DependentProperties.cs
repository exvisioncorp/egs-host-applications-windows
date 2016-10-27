namespace Egs
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
    using System.IO;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDeviceSettings
    {
        void SetEventHandlersAboutDependentProperties()
        {
            CaptureImageSize.ValueUpdated += delegate { OnPropertyChanged("CameraSpecificationValue"); };
            CaptureFps.ValueUpdated += delegate { OnPropertyChanged("CameraSpecificationValue"); };
            LensEquivalentFocalLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged("CameraSpecificationValue"); };
            SensorOnePixelSideLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged("CameraSpecificationValue"); };
            LensFNumber.ValueUpdated += delegate { OnPropertyChanged("CameraSpecificationValue"); };
            SensorExposureTimeInMilliseconds.ValueUpdated += delegate { OnPropertyChanged("CameraSpecificationValue"); };
        }

        public string CameraSpecificationValue
        {
            get
            {
                var ret = "";
                ret += "Capture Image Size: " + CaptureImageSize.Width + "x" + CaptureImageSize.Height + "    ";
                ret += "fps: " + CaptureFps.OptionalValue.SelectedItem.Description + "    ";
                ret += "Focal Length: " + LensEquivalentFocalLengthInMillimeters.Value + "mm    ";
                ret += "Pixel Size: " + SensorOnePixelSideLengthInMillimeters.Value * 1000 + "um    ";
                ret += "F Number: " + LensFNumber.Value + "    ";
                return ret;
            }
        }

        internal HidAccessPropertyRect GetCameraViewWindowRectByDefaultValue()
        {
            // System.Window.Rect is struct, so it is difficult to use it in Binding.
            if (CameraViewImageSourceBitmapSize == null) { Debugger.Break(); throw new EgsDeviceOperationException("CameraViewImageSourceBitmapSize == null"); }
            if (TouchTargetScreenSize == null) { Debugger.Break(); throw new EgsDeviceOperationException("TouchTargetScreenSize == null"); }

            var wVal = CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Width + 10;
            var wMin = TouchTargetScreenSize.Width / 10;
            var wMax = TouchTargetScreenSize.Width;
            var hVal = CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Height + 12;
            var hMin = TouchTargetScreenSize.Height / 10;
            var hMax = TouchTargetScreenSize.Height;
            // MUSTDO: test with changing DPI, because it can change the position of Camera View.
            var dpi = Dpi.DpiFromHdcForTheEntireScreen;
            var width = dpi.ScaledPrimaryScreenBounds.Width;
            var xVal = (int)(width - wVal - 50);
            var yVal = 100;
            return new HidAccessPropertyRect(
                new RangedInt(xVal, short.MinValue, short.MaxValue),
                new RangedInt(yVal, short.MinValue, short.MaxValue),
                new RangedInt(wVal, wMin, wMax),
                new RangedInt(hVal, hMin, hMax));
        }
    }
}

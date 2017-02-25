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
    using System.IO;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDeviceSettings
    {
        void CreatePropertiesAdditional()
        {
            _IsToUseDefaultFaceDetection = true;
            FaceDetectionIsProcessedBy = new OptionalValue<FaceDetectionIsProcessedByDetail>();
            FaceDetectionOnHost_RealFaceZMaximum = new RangedDouble(3.0, 1.0, 4.0, 0.1, 0.5, 0.1);
            FaceDetectionOnHost_Threshold = new RangedDouble(-0.4, -0.6, 0.0, 0.05, 0.1, 0.1);

            FaceDetectionIsProcessedBy.Options = FaceDetectionIsProcessedByDetail.GetDefaultList();
        }

        void InitializePropertiesByDefaultValueAdditional()
        {
            FaceDetectionOnHost_RealFaceZMaximum.Value = 3.0;
            FaceDetectionOnHost_Threshold.Value = -0.4;
            FaceDetectionIsProcessedBy.SelectSingleItemByPredicate(e => e.EnumValue == FaceDetectionIsProcessedByKind.HostApplication);
        }

        void AttachInternalEventHandlersAdditional()
        {
            FaceDetectionIsProcessedBy.SelectedItemChanged += delegate
            {
                switch (FaceDetectionIsProcessedBy.SelectedItem.EnumValue)
                {
                    case FaceDetectionIsProcessedByKind.Stopped:
                        IsToDetectFaces.Value = false;
                        break;
                    case FaceDetectionIsProcessedByKind.HostApplication:
                        IsToDetectFaces.Value = false;
                        IsToFixHandDetectionRegions.Value = true;
                        break;
                    case FaceDetectionIsProcessedByKind.Device:
                        IsToDetectFaces.Value = false;
                        IsToFixHandDetectionRegions.Value = false;
                        IsToDetectFaces.Value = true;
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException();
                }
            };
        }

        void SetEventHandlersAboutDependentProperties()
        {
            CaptureImageSize.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            CaptureFps.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            LensEquivalentFocalLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            SensorOnePixelSideLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            LensFNumber.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            SensorExposureTimeInMilliseconds.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };

            IsToMonitorTemperatureChanged += delegate { CurrentConnectedEgsDevice.IsMonitoringTemperature = IsToMonitorTemperature && CurrentConnectedEgsDevice.IsHidDeviceConnected; };
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
            var width = dpi.GetScaledRectangle(System.Windows.Forms.Screen.PrimaryScreen.Bounds).Width;
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

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
        }

        void InitializePropertiesByDefaultValueAdditional()
        {
            TouchTargetScreenSize.Width = (int)System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            TouchTargetScreenSize.Height = (int)System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            FaceDetectionMethod.Value = FaceDetectionMethods.DefaultProcessOnEgsDevice;
            CaptureExposureMode.Value = CaptureExposureModes.Auto;
        }

        void AttachInternalEventHandlersAdditional()
        {
            CaptureBinning.ValueUpdated += delegate { OnPixelSizeRelatedPropertiesUpdated(); };
            CaptureImageSize.ValueUpdated += delegate
            {
                CurrentConnectedEgsDevice.FaceDetectionOnHost.CaptureImageWidth = CaptureImageSize.Width;
                CurrentConnectedEgsDevice.FaceDetectionOnHost.CaptureImageHeight = CaptureImageSize.Height;
                OnPropertyChanged(nameof(CameraSpecificationValue));
            };
            CaptureFps.ValueUpdated += delegate { OnPropertyChanged(nameof(CameraSpecificationValue)); };
            LensEquivalentFocalLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged(nameof(CameraSpecificationValue)); };
            SensorOnePixelSideLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged(nameof(CameraSpecificationValue)); };
            LensFNumber.ValueUpdated += delegate { OnPropertyChanged(nameof(CameraSpecificationValue)); };
            SensorExposureTimeInMilliseconds.ValueUpdated += delegate { OnPropertyChanged(nameof(CameraSpecificationValue)); };

            IsToMonitorTemperature.ValueUpdated += delegate
            {
                if (CurrentConnectedEgsDevice == null) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } return; }
                CurrentConnectedEgsDevice.IsMonitoringTemperature = IsToMonitorTemperature.Value && CurrentConnectedEgsDevice.IsHidDeviceConnected;
            };
            FaceDetectionMethod.ValueUpdated += delegate
            {
                switch (FaceDetectionMethod.Value)
                {
                    case FaceDetectionMethods.DefaultProcessOnEgsDevice:
                        if (CameraViewImageSourceBitmapSize.Value != CameraViewImageSourceBitmapSizes.Size_384x240) { CameraViewImageSourceBitmapSize.Value = CameraViewImageSourceBitmapSizes.Size_384x240; }
                        break;
                    case FaceDetectionMethods.DefaultProcessOnEgsHostApplication:
                        if (CameraViewImageSourceBitmapSize.Value != CameraViewImageSourceBitmapSizes.Size_640x480) { CameraViewImageSourceBitmapSize.Value = CameraViewImageSourceBitmapSizes.Size_640x480; }
                        break;
                    case FaceDetectionMethods.SdkUserProcess:
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException();
                }
                OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged();
            };
            IsToDetectFaces.ValueUpdated += delegate { OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged(); };
            IsToDetectHands.ValueUpdated += delegate { OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged(); };

            CameraViewImageSourceBitmapSize.ValueUpdated += delegate
            {
                CurrentConnectedEgsDevice.FaceDetectionOnHost.CameraViewImageWidth = CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Width;
                CurrentConnectedEgsDevice.FaceDetectionOnHost.CameraViewImageHeight = CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Height;
                CurrentConnectedEgsDevice.FaceDetectionOnHost.SetCameraViewImageScale_DividedBy_CaptureImageScale_ToCameraViewImageHeight_DividedBy_CaptureImageheight();
            };
        }

        void OnPixelSizeRelatedPropertiesUpdated()
        {
            // TODO: get the actual binned pixel size from devices.
            var newSensorOnePixelSideLengthInMillimeters = 0.0014f * (int)CaptureBinning.Value;
            if (SensorOnePixelSideLengthInMillimeters.Value != newSensorOnePixelSideLengthInMillimeters)
            {
                SensorOnePixelSideLengthInMillimeters.Value = newSensorOnePixelSideLengthInMillimeters;
                OnPropertyChanged(nameof(CameraSpecificationValue));
            }

            if (CurrentConnectedEgsDevice == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            CurrentConnectedEgsDevice.FaceDetectionOnHost.CaptureImageBinnedPixelSize = SensorOnePixelSideLengthInMillimeters.Value;
            CurrentConnectedEgsDevice.FaceDetectionOnHost.SetCameraViewImageScale_DividedBy_CaptureImageScale_ToCameraViewImageHeight_DividedBy_CaptureImageheight();
        }

        void OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged()
        {
            if (CurrentConnectedEgsDevice == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            CurrentConnectedEgsDevice.On_FaceDetectionMethod_IsToDetectFaces_IsToDetectHands_IsHidDeviceConnected_Changed();
            CurrentConnectedEgsDevice.ResetHidReportObjects();
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
            // System.Window.Rectangle is struct, so it is difficult to use it in Binding.
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

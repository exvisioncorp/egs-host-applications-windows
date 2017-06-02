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

            if (ApplicationCommonSettings.HostApplicationName != "ZKOO")
            {
                FaceSelectionOnDeviceMethod.Value = FaceSelectionOnDeviceMethods.MostCenter;
            }
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
                // TODO: MUSTDO: This is temporary work-around.
                // After some troubles disable face detection in device settings, users may change this value or restart host application to enable face detection.
                // Currently most of EGS applications need face detection.
                IsToDetectFaces.Value = true;
                OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged();
            };
            IsToDetectFaces.ValueUpdated += delegate { OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged(); };
            IsToDetectHands.ValueUpdated += delegate { OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged(); };

            CameraViewImageSourceBitmapSize.ValueUpdated += delegate
            {
                CurrentConnectedEgsDevice.FaceDetectionOnHost.CameraViewImageWidth = CameraViewImageSourceBitmapSize.SelectedItem.Width;
                CurrentConnectedEgsDevice.FaceDetectionOnHost.CameraViewImageHeight = CameraViewImageSourceBitmapSize.SelectedItem.Height;
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
                ret += "fps: " + CaptureFps.SelectedItem.Description + "    ";
                ret += "Focal Length: " + LensEquivalentFocalLengthInMillimeters.Value + "mm    ";
                ret += "Pixel Size: " + SensorOnePixelSideLengthInMillimeters.Value * 1000 + "um    ";
                ret += "F Number: " + LensFNumber.Value + "    ";
                return ret;
            }
        }
    }
}

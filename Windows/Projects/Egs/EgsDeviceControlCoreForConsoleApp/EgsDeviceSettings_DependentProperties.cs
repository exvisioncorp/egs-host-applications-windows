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
        }

        void AttachInternalEventHandlersAdditional()
        {
            CaptureBinning.ValueUpdated += delegate { OnPixelSizeRelatedPropertiesUpdated(); };
            CaptureImageSize.ValueUpdated += delegate { OnPropertyChanged(nameof(CameraSpecificationValue)); };
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
            FaceDetectionMethod.ValueUpdated += delegate { OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged(); };
            IsToDetectFaces.ValueUpdated += delegate { OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged(); };
            IsToDetectHands.ValueUpdated += delegate { OnPropertiesRelatedToFaceDetectionAndIsToDetectHandsOnDeviceChanged(); };

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

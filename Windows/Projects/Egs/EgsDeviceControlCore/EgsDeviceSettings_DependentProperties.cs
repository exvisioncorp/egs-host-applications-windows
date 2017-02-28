﻿namespace Egs
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
            FaceDetectionMethod.Value = FaceDetectionMethodKind.DefaultProcessOnEgsDevice;

            // TODO: MUSTDO: fix the bug.
            CaptureExposureMode.OptionalValue.SelectSingleItemByPredicate(e => e.Value == 0);
        }

        void AttachInternalEventHandlersAdditional()
        {
            CaptureImageSize.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            CaptureFps.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            LensEquivalentFocalLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            SensorOnePixelSideLengthInMillimeters.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            LensFNumber.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };
            SensorExposureTimeInMilliseconds.ValueUpdated += delegate { OnPropertyChanged(Name.Of(() => CameraSpecificationValue)); };

            IsToMonitorTemperature.ValueUpdated += delegate
            {
                if (CurrentConnectedEgsDevice == null) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } return; }
                CurrentConnectedEgsDevice.IsMonitoringTemperature = IsToMonitorTemperature.Value && CurrentConnectedEgsDevice.IsHidDeviceConnected;
            };
            FaceDetectionMethod.ValueUpdated += delegate { OnFaceDetectionOnOffRelatedPropertiesUpdated(); };
            IsToDetectFaces.ValueUpdated += delegate { OnFaceDetectionOnOffRelatedPropertiesUpdated(); };

            CaptureBinning.ValueUpdated += delegate { OnPixelOneSideLengthRelatedPropertiesUpdated(); };
        }

        void OnPixelOneSideLengthRelatedPropertiesUpdated()
        {
            if (CaptureBinning.ValueOfSelectedItem <= 0)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new NotImplementedException();
            }

            // TODO: get the actual binned pixel size from devices.
            var newSensorOnePixelSideLengthInMillimeters = 0.0014f * CaptureBinning.ValueOfSelectedItem;
            if (SensorOnePixelSideLengthInMillimeters.Value != newSensorOnePixelSideLengthInMillimeters)
            {
                SensorOnePixelSideLengthInMillimeters.Value = newSensorOnePixelSideLengthInMillimeters;
                OnPropertyChanged(Name.Of(() => CameraSpecificationValue));
            }

            if (CurrentConnectedEgsDevice == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            CurrentConnectedEgsDevice.FaceDetectionOnHost.SensorImageBinnedPixelOneSideLength = SensorOnePixelSideLengthInMillimeters.Value;
        }

        void OnFaceDetectionOnOffRelatedPropertiesUpdated()
        {
            if (CurrentConnectedEgsDevice == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                return;
            }
            CurrentConnectedEgsDevice.UpdateFaceDetectionRelatedProperties();
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

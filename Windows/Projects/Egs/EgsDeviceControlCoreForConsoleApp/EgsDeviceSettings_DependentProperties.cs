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
    }
}

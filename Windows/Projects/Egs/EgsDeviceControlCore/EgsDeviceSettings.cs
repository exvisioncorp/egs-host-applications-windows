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
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    internal class HidAccessPropertyUpdatedEventArgs : EventArgs
    {
        internal protected HidAccessPropertyBase UpdatedProperty { get; private set; }
        internal protected HidAccessPropertyUpdatedEventArgs(HidAccessPropertyBase updatedProperty)
        {
            UpdatedProperty = updatedProperty;
        }
    }

    /// <summary>
    /// The settings set from host application to connected device, when device is (re)connected or host application is started.
    /// The information about capability and the status of "being connected" device, is not contained to this class.
    /// SDK users can change EGS settings through "Value" properties etc. of each property in this object.
    /// This also needs calling of "InitializeOnceAtStartup" method after construction.
    /// </summary>
    [DataContract]
    public partial class EgsDeviceSettings : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        internal IList<HidAccessPropertyBase> HidAccessPropertyList { get; private set; }
        internal event EventHandler<HidAccessPropertyUpdatedEventArgs> HidAccessPropertyUpdated;
        internal virtual void OnHidAccessPropertyUpdated(HidAccessPropertyUpdatedEventArgs e)
        {
            var t = HidAccessPropertyUpdated; if (t != null) { t(this, e); }
        }

        internal EgsDevice CurrentConnectedEgsDevice { get; set; }


        public EgsDeviceSettings()
        {
            CreateProperties();

            // After 960x540 device is connected, this size will be updated.
            CaptureImageSize.Value = new System.Drawing.Size(768, 480);

            // NOTE: If device receives too big area, the device restricts the value to Rect (0,0,1,1). 
            if (false)
            {
                RightHandDetectionAreaOnFixed.Value.XRange.Minimum = 0.05f;
                RightHandDetectionAreaOnFixed.Value.XRange.Maximum = 0.85f;
                RightHandDetectionAreaOnFixed.Value.YRange.Minimum = 0.05f;
                RightHandDetectionAreaOnFixed.Value.YRange.Maximum = 0.95f;
                LeftHandDetectionAreaOnFixed.Value.XRange.Minimum = 0.15f;
                LeftHandDetectionAreaOnFixed.Value.XRange.Maximum = 0.95f;
                LeftHandDetectionAreaOnFixed.Value.YRange.Minimum = 0.05f;
                LeftHandDetectionAreaOnFixed.Value.YRange.Maximum = 0.95f;
            }

            CreatePropertiesAdditional();

            Reset();
        }

        public void Reset()
        {
            InitializePropertiesByDefaultValue();
            InitializePropertiesByDefaultValueAdditional();

            OnImageSizeRelatedPropertiesUpdated();
        }

        public void InitializeOnceAtStartup()
        {
            AddPropertiesToHidAccessPropertyList();
            AttachInternalEventHandlers();
            AttachInternalEventHandlersAdditional();
        }

        public bool IsCaptureExposureModeManual
        {
            get { return CaptureExposureMode.OptionalValue.SelectedItem.Value == 1; }
        }
        public bool IsFaceDetectionMethodDefaultProcessOnEgsDevice
        {
            get { return FaceDetectionMethod.Value == FaceDetectionMethodKind.DefaultProcessOnEgsDevice; }
        }
        public bool IsFaceDetectionMethodDefaultProcessOnEgsHostApplication
        {
            get { return FaceDetectionMethod.Value == FaceDetectionMethodKind.DefaultProcessOnEgsHostApplication; }
        }

        internal void AttachInternalEventHandlers()
        {
            foreach (var item in HidAccessPropertyList)
            {
                item.ValueUpdated += delegate
                {
                    if (CurrentConnectedEgsDevice == null) { return; }
                    HidAccessPropertyUpdatedEventArgs e = new HidAccessPropertyUpdatedEventArgs(item);
                    OnHidAccessPropertyUpdated(e);
                };
            }

            CaptureExposureMode.OptionalValue.SelectedItemChanged += delegate
            {
                OnPropertyChanged(nameof(IsCaptureExposureModeManual));
            };
            FaceDetectionMethod.ValueUpdated += delegate
            {
                OnPropertyChanged(nameof(IsFaceDetectionMethodDefaultProcessOnEgsDevice));
                OnPropertyChanged(nameof(IsFaceDetectionMethodDefaultProcessOnEgsHostApplication));
            };

            TouchInterfaceKind.OptionalValue.SelectedItemChanged += (sender, e) =>
            {
                switch (TouchInterfaceKind.OptionalValue.SelectedItem.EnumValue)
                {
                    case EgsDeviceTouchInterfaceKind.MultiTouch:
                        TrackableHandsCount.Value = 2;
                        break;
                    case EgsDeviceTouchInterfaceKind.SingleTouch:
                        TrackableHandsCount.Value = 1;
                        break;
                    case EgsDeviceTouchInterfaceKind.Mouse:
                        TrackableHandsCount.Value = 1;
                        break;
                    default:
                        Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "TouchInterfaceKind: {0}", TouchInterfaceKind.OptionalValue.SelectedItem.EnumValue));
                        break;
                }
                if (CurrentConnectedEgsDevice != null)
                {
                    CurrentConnectedEgsDevice.ResetHidReportObjects();
                }
            };

            CursorSpeedAndPrecisionMode.OptionalValue.SelectedItemChanged += (sender, e) =>
            {
                // TODO: test
                switch (CursorSpeedAndPrecisionMode.OptionalValue.SelectedItem.Value)
                {
                    case 0:
                        this.FastMovingHandsGestureMode.OptionalValue.SelectSingleItemByPredicate(item => item.Value == 0);
                        break;
                    case 1:
                        this.FastMovingHandsGestureMode.OptionalValue.SelectSingleItemByPredicate(item => item.Value == 0);
                        break;
                    case 2:
                        this.FastMovingHandsGestureMode.OptionalValue.SelectSingleItemByPredicate(item => item.Value == 1);
                        break;
                    default:
                        Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "TouchInterfaceKind: {0}", TouchInterfaceKind.OptionalValue.SelectedItem.EnumValue));
                        break;
                }
                if (CurrentConnectedEgsDevice != null)
                {
                    CurrentConnectedEgsDevice.ResetHidReportObjects();
                }
            };

            CaptureImageSize.ValueUpdated += delegate { OnImageSizeRelatedPropertiesUpdated(); };
            CameraViewImageSourceBitmapSize.ValueUpdated += delegate { OnImageSizeRelatedPropertiesUpdated(); };
            CameraViewImageSourceRectInCaptureImage.ValueUpdated += delegate { OnImageSizeRelatedPropertiesUpdated(); };
            OnImageSizeRelatedPropertiesUpdated();
        }

        internal void OnImageSizeRelatedPropertiesUpdated()
        {
            // NOTE: It gets the value from device, but the initial value should be correct.  Some problems can be caused by DataBindings.
            var msg = "";
            msg += "CaptureImageSize: " + CaptureImageSize.Value.ToString() + Environment.NewLine;
            msg += "CameraViewImageSourceBitmapSize: " + CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.ToString() + Environment.NewLine;
            msg += "CameraViewImageSourceRectInCaptureImage: " + CameraViewImageSourceRectInCaptureImage.Value.ToString() + Environment.NewLine;
            Debug.WriteLine(msg);

            if (CurrentConnectedEgsDevice != null && CurrentConnectedEgsDevice.EgsGestureHidReport != null)
            {
                CurrentConnectedEgsDevice.EgsGestureHidReport.UpdateImageSizeRelatedProperties();
            }

            // TODO: MUSTDO: Debug the firmware.  So, do not return here.
            //return;
            Debug.WriteLine("\"On some PCs\", the host application cannot get the correct CameraViewImageSourceRectInCaptureImage from device for a while, after it changes CameraViewImageSourceBitmapSize.");

            // NOTE: If you update the value directly, infinite loop occurs.
            var correctCameraViewImageSourceRectInCaptureImage = new System.Drawing.Rectangle();
            if (CaptureImageSize.Value == new System.Drawing.Size(768, 480))
            {
                if (CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Size == new System.Drawing.Size(384, 240)) { correctCameraViewImageSourceRectInCaptureImage = new System.Drawing.Rectangle(8, 0, 752, 470); }
                else if (CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Size == new System.Drawing.Size(320, 240)) { correctCameraViewImageSourceRectInCaptureImage = new System.Drawing.Rectangle(71, 0, 625, 470); }
                else if (CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Size == new System.Drawing.Size(640, 480)) { correctCameraViewImageSourceRectInCaptureImage = new System.Drawing.Rectangle(71, 0, 625, 470); }
                else
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
                }
            }
            else if (CaptureImageSize.Value == new System.Drawing.Size(960, 540))
            {
                if (CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Size == new System.Drawing.Size(384, 240)) { correctCameraViewImageSourceRectInCaptureImage = new System.Drawing.Rectangle(56, 0, 848, 530); }
                else if (CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Size == new System.Drawing.Size(320, 240)) { correctCameraViewImageSourceRectInCaptureImage = new System.Drawing.Rectangle(126, 0, 707, 530); }
                else if (CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Size == new System.Drawing.Size(640, 480)) { correctCameraViewImageSourceRectInCaptureImage = new System.Drawing.Rectangle(126, 0, 707, 530); }
                else
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
                }

            }
            else
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new NotImplementedException();
            }

            if (CameraViewImageSourceRectInCaptureImage.Value != correctCameraViewImageSourceRectInCaptureImage)
            {
                CameraViewImageSourceRectInCaptureImage.Value = correctCameraViewImageSourceRectInCaptureImage;
            }
        }
    }
}

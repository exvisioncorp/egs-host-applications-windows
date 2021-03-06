﻿namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using System.Runtime.InteropServices;
    using System.Globalization;
    using Egs;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;

    public partial class EgsDevice
    {
        public string DeviceStatusString
        {
            get
            {
                if (IsHidDeviceConnected != CameraViewImageSourceBitmapCapture.IsCameraDeviceConnected)
                {
                    if (IsHidDeviceConnected == false) { return "Only Camera"; }
                    else { return "Only HID"; }
                }
                var ret = IsHidDeviceConnected ? Resources.CommonStrings_IsConnected : Resources.CommonStrings_IsNotConnected;
                return ret;
            }
        }

        public string DeviceSpecificationString
        {
            get
            {
                if (IsHidDeviceConnected == false) { return Resources.CommonStrings_IsNotConnected; }
                var ret = "";
                ret += Resources.EgsDevice_HardwareType_Description + ": " + HardwareTypeString;
                ret += "  " + Resources.EgsDevice_DeviceSerialNumber_Description + ": " + DeviceSerialNumberString;
                ret += "  " + Resources.EgsDevice_FirmwareVersion_Description + ": " + FirmwareVersionString;
                return ret;
            }
        }

        public string DeviceStatusDetailString
        {
            get
            {
                var ret = IsHidDeviceConnected ? (DeviceStatusString + " (" + DeviceSpecificationString + ")") : Resources.CommonStrings_IsNotConnected;
                return ret;
            }
        }

        public string DeviceSerialNumberString
        {
            get
            {
                var ret = IsHidDeviceConnected ? DeviceSerialNumber.Value : "";
                return ret;
            }
        }

        public string HardwareTypeString
        {
            get
            {
                var ret = IsHidDeviceConnected ? HardwareType.SelectedItem.Description : "";
                return ret;
            }
        }

        public string FirmwareVersionString
        {
            get
            {
                var ret = "";
                if (IsHidDeviceConnected)
                {
                    ret += FirmwareVersion.Value[0].ToString(CultureInfo.InvariantCulture);
                    ret += "." + FirmwareVersion.Value[1].ToString(CultureInfo.InvariantCulture);
                    ret += "." + FirmwareVersion.Value[2].ToString(CultureInfo.InvariantCulture);
                    ret += "." + FirmwareVersion.Value[3].ToString(CultureInfo.InvariantCulture);
                }
                return ret;
            }
        }

        public Version FirmwareVersionAsVersion
        {
            get
            {
                // TODO: fix spec.  If the device is not connected, return Empty Version object.
                if (string.IsNullOrEmpty(FirmwareVersionString)) { return new Version(); }
                try
                {
                    return new Version(FirmwareVersionString);
                }
                catch (Exception ex)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    Debug.WriteLine(ex.Message);
                    return new Version();
                }
            }
        }

        /// <summary>
        /// If the device can get temperature, this value will be update by EachDeviceStatusMonitoringTimer (System.Windows.Forms.Timer).  
        /// You can change the interval of the timer by EgsDevice.TemperatureMonitoringTimerIntervalTotalSeconds property.
        /// </summary>
        public string TemperatureInCelsiusString
        {
            get
            {
                if (IsHidDeviceConnected == false) { return Resources.CommonStrings_IsNotConnected; }
                var ret = TemperatureInCelsius.Value.ToString("F2", CultureInfo.CurrentCulture) + " [C]";
                return ret;
            }
        }

        public string TemperatureInFahrenheitString
        {
            get
            {
                if (IsHidDeviceConnected == false) { return Resources.CommonStrings_IsNotConnected; }
                var ret = TemperatureInFahrenheit.Value.ToString("F2", CultureInfo.CurrentCulture) + " [F]";
                return ret;
            }
        }

        internal void On_FaceDetectionMethod_IsToDetectFaces_IsToDetectHands_IsHidDeviceConnected_Changed()
        {
            switch (Settings.FaceDetectionMethod.Value)
            {
                case FaceDetectionMethods.DefaultProcessOnEgsDevice:
                    {
                        if (Settings.IsToDetectHandsOnDevice.Value != false) { Settings.IsToDetectHandsOnDevice.Value = false; }
                        {
                            if (Settings.IsToDetectFacesOnDevice.Value)
                            {
                                if (Settings.IsToDetectFacesOnDevice.Value != Settings.IsToDetectFaces) { Settings.IsToDetectFacesOnDevice.Value = Settings.IsToDetectFaces; }
                                if (Settings.IsToFixHandDetectionRegions.Value != false) { Settings.IsToFixHandDetectionRegions.Value = false; }
                            }
                            else
                            {
                                if (Settings.IsToFixHandDetectionRegions.Value != false) { Settings.IsToFixHandDetectionRegions.Value = false; }
                                if (Settings.IsToDetectFacesOnDevice.Value != Settings.IsToDetectFaces) { Settings.IsToDetectFacesOnDevice.Value = Settings.IsToDetectFaces; }
                            }
                        }

                        bool newIsToDetectHandsOnDevice = Settings.IsToDetectHands;
                        if (Settings.IsToDetectHandsOnDevice.Value != newIsToDetectHandsOnDevice) { Settings.IsToDetectHandsOnDevice.Value = newIsToDetectHandsOnDevice; }
                    }
                    break;
                case FaceDetectionMethods.DefaultProcessOnEgsHostApplication:
                    {
                        if (Settings.IsToDetectFacesOnDevice.Value != false) { Settings.IsToDetectFacesOnDevice.Value = false; }
                        if (Settings.IsToFixHandDetectionRegions.Value != true) { Settings.IsToFixHandDetectionRegions.Value = true; }
                    }
                    break;
                case FaceDetectionMethods.SdkUserProcess:
                    break;
                default:
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
            }
            if (Settings.IsToDetectFaces.Value == false) { ResetHidReportObjects(); }
            UpdateIsDetectingFaces();
            UpdateIsDetectingHands();
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsDetectingFaces = false;
        public event EventHandler IsDetectingFacesChanged;
        protected virtual void OnIsDetectingFacesChanged(EventArgs e)
        {
            var t = IsDetectingFacesChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsDetectingFaces));
        }
        public void UpdateIsDetectingFaces()
        {
            switch (Settings.FaceDetectionMethod.Value)
            {
                case FaceDetectionMethods.DefaultProcessOnEgsDevice:
                    IsDetectingFaces = IsHidDeviceConnected && (EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.DetectingFaces);
                    break;
                case FaceDetectionMethods.DefaultProcessOnEgsHostApplication:
                    IsDetectingFaces = IsConnected && Settings.IsToDetectFaces.Value && (IsTrackingOneOrMoreHands == false);
                    break;
                case FaceDetectionMethods.SdkUserProcess:
                    //IsDetectingFaces = _IsDetectingFaces;
                    break;
                default:
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    throw new NotImplementedException();
            }
        }
        public bool IsDetectingFaces
        {
            get { UpdateIsDetectingFaces(); return _IsDetectingFaces; }
            set { if (_IsDetectingFaces != value) { _IsDetectingFaces = value; OnIsDetectingFacesChanged(EventArgs.Empty); } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsDetectingHands = false;
        public event EventHandler IsDetectingHandsChanged;
        protected virtual void OnIsDetectingHandsChanged(EventArgs e)
        {
            var t = IsDetectingHandsChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsDetectingHands));
        }
        public void UpdateIsDetectingHands()
        {
            if (IsHidDeviceConnected == false) { IsDetectingFaces = false; }
            else
            {
                switch (Settings.FaceDetectionMethod.Value)
                {
                    case FaceDetectionMethods.DefaultProcessOnEgsDevice:
                        IsDetectingHands = (EgsGestureHidReport.MessageId == EgsGestureHidReportMessageIds.DetectingOrTrackingHands) && (IsTrackingOneOrMoreHands == false);
                        break;
                    case FaceDetectionMethods.DefaultProcessOnEgsHostApplication:
                        IsDetectingHands = Settings.IsToDetectFaces.Value && Settings.IsToDetectHands.Value && (IsTrackingOneOrMoreHands == false);
                        break;
                    case FaceDetectionMethods.SdkUserProcess:
                        //IsDetectingHands = _IsDetectingFaces;
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException();
                }
            }
        }
        public bool IsDetectingHands
        {
            get { UpdateIsDetectingHands(); return _IsDetectingHands; }
            set { if (_IsDetectingHands != value) { _IsDetectingHands = value; OnIsDetectingHandsChanged(EventArgs.Empty); } }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsTrackingOneOrMoreHands = false;
        public event EventHandler IsTrackingOneOrMoreHandsChanged;
        protected virtual void OnIsTrackingOneOrMoreHandsChanged(EventArgs e)
        {
            var t = IsTrackingOneOrMoreHandsChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsTrackingOneOrMoreHands));
        }
        public void UpdateIsTrackingOneOrMoreHands()
        {
            IsTrackingOneOrMoreHands = IsHidDeviceConnected && (EgsGestureHidReport.Hands[0].IsTracking || EgsGestureHidReport.Hands[1].IsTracking);
        }
        public bool IsTrackingOneOrMoreHands
        {
            get { UpdateIsTrackingOneOrMoreHands(); return _IsTrackingOneOrMoreHands; }
            set { if (_IsTrackingOneOrMoreHands != value) { _IsTrackingOneOrMoreHands = value; OnIsTrackingOneOrMoreHandsChanged(EventArgs.Empty); } }
        }

        void Update_IsDetectingFaces_IsDetectingHands_IsTracking()
        {
            UpdateIsDetectingFaces();
            UpdateIsDetectingHands();
        }

        void UpdateIsHidDeviceConnectedRelatedProperties(object sender, EventArgs e)
        {
            // NOTE: Does it need to allow accesses from EgsDeviceFirmwareUpdateModel?
            if (Settings == null) { Debugger.Break(); throw new EgsDeviceOperationException("Settings == null"); }

            On_FaceDetectionMethod_IsToDetectFaces_IsToDetectHands_IsHidDeviceConnected_Changed();

            IsSendingTouchScreenHidReport = Settings.IsToSendTouchScreenHidReport.Value && IsHidDeviceConnected;
            IsSendingHoveringStateOnTouchScreenHidReport = Settings.IsToSendHoveringStateOnTouchScreenHidReport.Value && IsHidDeviceConnected;
            IsSendingEgsGestureHidReport = Settings.IsToSendEgsGestureHidReport.Value && IsHidDeviceConnected;

            IsMonitoringTemperature = Settings.IsToMonitorTemperature.Value && IsHidDeviceConnected;

            // MUSTDO: FIX.  The next line can cause cross thread exceptions.
            if (IsHidDeviceConnected == false) { ResetHidReportObjects(); }

            OnPropertyChanged(nameof(DeviceStatusString));
            OnPropertyChanged(nameof(DeviceStatusDetailString));

            OnPropertyChanged(nameof(DeviceSerialNumberString));
            OnPropertyChanged(nameof(HardwareTypeString));
            OnPropertyChanged(nameof(FirmwareVersionString));
            OnPropertyChanged(nameof(FirmwareVersionAsVersion));
            OnPropertyChanged(nameof(DeviceSpecificationString));
            OnPropertyChanged(nameof(TemperatureInCelsiusString));
            OnPropertyChanged(nameof(TemperatureInFahrenheitString));
        }

        void UpdateIsConnected(object sender, EventArgs e)
        {
            IsConnected = CameraViewImageSourceBitmapCapture.IsCameraDeviceConnected && IsHidDeviceConnected;
            OnPropertyChanged(nameof(DeviceStatusString));
            OnPropertyChanged(nameof(DeviceStatusDetailString));
        }

        void EgsDeviceSettings_HidAccessPropertyUpdated(object sender, HidAccessPropertyUpdatedEventArgs e)
        {
            if (e.UpdatedProperty == Settings.IsToDetectFacesOnDevice)
            {
                Update_IsDetectingFaces_IsDetectingHands_IsTracking();
            }
            else if (e.UpdatedProperty == Settings.IsToDetectHandsOnDevice)
            {
                Update_IsDetectingFaces_IsDetectingHands_IsTracking();
            }
            else if (e.UpdatedProperty == Settings.IsToSendTouchScreenHidReport)
            {
                IsSendingTouchScreenHidReport = Settings.IsToSendTouchScreenHidReport.Value && IsHidDeviceConnected;
            }
            else if (e.UpdatedProperty == Settings.IsToSendHoveringStateOnTouchScreenHidReport)
            {
                IsSendingHoveringStateOnTouchScreenHidReport = Settings.IsToSendHoveringStateOnTouchScreenHidReport.Value && IsHidDeviceConnected;
            }
            else if (e.UpdatedProperty == Settings.IsToSendEgsGestureHidReport)
            {
                IsSendingEgsGestureHidReport = Settings.IsToSendEgsGestureHidReport.Value && IsHidDeviceConnected;
            }
            else if (e.UpdatedProperty == Settings.CameraViewImageSourceBitmapSize)
            {
                StopFaceDetectionAndRestartUvcAndRestartFaceDetection();
            }
            else if (e.UpdatedProperty == Settings.CaptureFps)
            {
                switch (Settings.CaptureFps.Value)
                {
                    case CaptureFpsKind.Auto:
                    case CaptureFpsKind.Fps100:
                        EgsGestureHidReport.FramesPerSecond = 100.0;
                        break;
                    case CaptureFpsKind.Fps120:
                        EgsGestureHidReport.FramesPerSecond = 120.0;
                        break;
                    default:
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        throw new NotImplementedException();
                }
            }

            bool hr = false;
            try
            {
                if (IsHidDeviceConnected && e.UpdatedProperty.IsReadOnly == false)
                {
                    hr = SetHidAccessPropertyBySetHidFeatureReport(e.UpdatedProperty);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
            }

#if ApplicationCommonSettings_CanChangeDeviceUsage
            // TODO: MUSTDO: test and debug
            if (e.UpdatedProperty == Settings.DeviceUsage)
            {
                // TODO: MUSTDO: think again about specification
                if (Settings.DeviceUsage.Value == DeviceUsages.RemoteTouch) { Settings.IsToDetectFaces.Value = true; }
                else if (Settings.DeviceUsage.Value == DeviceUsages.MotionControl) { Settings.IsToDetectFaces.Value = false; }
                ResetHidReportObjects();
            }
#endif
        }
    }
}

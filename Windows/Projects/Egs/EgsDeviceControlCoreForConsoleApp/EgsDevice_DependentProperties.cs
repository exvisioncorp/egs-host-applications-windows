namespace Egs
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
    using Egs.EgsDeviceControlCoreForConsoleApp.Properties;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;

    public partial class EgsDevice
    {
        public string DeviceStatusString
        {
            get
            {
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
                var ret = IsHidDeviceConnected ? HardwareType.OptionalValue.SelectedItem.Description : "";
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

        void UpdateIsHidDeviceConnectedRelatedProperties(object sender, EventArgs e)
        {
            // NOTE: Does it need to allow accesses from EgsDeviceFirmwareUpdateModel?
            if (Settings == null) { Debugger.Break(); throw new EgsDeviceOperationException("Settings == null"); }

            IsDetectingFaces = Settings.IsToDetectFaces.Value && IsHidDeviceConnected;
            IsDetectingHands = Settings.IsToDetectHands.Value && IsHidDeviceConnected;
            IsSendingTouchScreenHidReport = Settings.IsToSendTouchScreenHidReport.Value && IsHidDeviceConnected;
            IsSendingHoveringStateOnTouchScreenHidReport = Settings.IsToSendHoveringStateOnTouchScreenHidReport.Value && IsHidDeviceConnected;
            IsSendingEgsGestureHidReport = Settings.IsToSendEgsGestureHidReport.Value && IsHidDeviceConnected;

            IsMonitoringTemperature = IsToMonitorTemperature && IsHidDeviceConnected;

            // MUSTDO: FIX.  The next line can cause cross thread exceptions.
            if (IsHidDeviceConnected == false) { ResetHidReportObjects(); }

            OnPropertyChanged(Name.Of(() => DeviceStatusString));
            OnPropertyChanged(Name.Of(() => DeviceStatusDetailString));

            OnPropertyChanged(Name.Of(() => DeviceSerialNumberString));
            OnPropertyChanged(Name.Of(() => HardwareTypeString));
            OnPropertyChanged(Name.Of(() => FirmwareVersionString));
            OnPropertyChanged(Name.Of(() => FirmwareVersionAsVersion));
            OnPropertyChanged(Name.Of(() => DeviceSpecificationString));
            OnPropertyChanged(Name.Of(() => TemperatureInCelsiusString));
            OnPropertyChanged(Name.Of(() => TemperatureInFahrenheitString));
        }

        void UpdateIsConnected(object sender, EventArgs e)
        {
            IsConnected = IsHidDeviceConnected;
            OnPropertyChanged(Name.Of(() => DeviceStatusString));
            OnPropertyChanged(Name.Of(() => DeviceStatusDetailString));
        }

        void EgsDeviceSettings_HidAccessPropertyUpdated(object sender, HidAccessPropertyUpdatedEventArgs e)
        {
            var settings = (EgsDeviceSettings)sender;
            if (e.UpdatedProperty == settings.IsToDetectFaces) { IsDetectingFaces = Settings.IsToDetectFaces.Value && IsHidDeviceConnected; }
            else if (e.UpdatedProperty == settings.IsToDetectHands) { IsDetectingHands = Settings.IsToDetectHands.Value && IsHidDeviceConnected; }
            else if (e.UpdatedProperty == settings.IsToSendTouchScreenHidReport) { IsSendingTouchScreenHidReport = Settings.IsToSendTouchScreenHidReport.Value && IsHidDeviceConnected; }
            else if (e.UpdatedProperty == settings.IsToSendHoveringStateOnTouchScreenHidReport) { IsSendingHoveringStateOnTouchScreenHidReport = Settings.IsToSendHoveringStateOnTouchScreenHidReport.Value && IsHidDeviceConnected; }
            else if (e.UpdatedProperty == settings.IsToSendEgsGestureHidReport) { IsSendingEgsGestureHidReport = Settings.IsToSendEgsGestureHidReport.Value && IsHidDeviceConnected; }
            else if (e.UpdatedProperty == settings.CameraViewImageSourceBitmapSize)
            {
                OnPropertyChanged(NameOf_Settings_CameraViewImageSourceBitmapSize);
            }
            else if (e.UpdatedProperty == settings.CaptureFps)
            {
                switch (settings.CaptureFps.OptionalValue.SelectedItem.Value)
                {
                    case 0:
                    case 100:
                        EgsGestureHidReport.FramesPerSecond = 100.0;
                        break;
                    case 120:
                        EgsGestureHidReport.FramesPerSecond = 120.0;
                        break;
                    default:
                        throw new NotImplementedException();
                }
            }

            try
            {
                if (IsHidDeviceConnected && e.UpdatedProperty.IsReadOnly == false)
                {
                    if (CheckHidPropertyVersionAndCurrentFirmwareVersion(e.UpdatedProperty) == false) { return; }
                    SetHidFeatureReport(e.UpdatedProperty.ByteArrayData);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
            }
        }

        // TODO: MUSTDO: needs smarter implementation.
        internal const string NameOf_Settings_CameraViewImageSourceBitmapSize = "Settings.CameraViewImageSourceBitmapSize";
    }
}

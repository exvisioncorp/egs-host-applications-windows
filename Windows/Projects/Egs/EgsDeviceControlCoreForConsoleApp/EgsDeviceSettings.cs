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

        internal EgsDevice refToCurrentConnectedEgsDevice { get; set; }


        public EgsDeviceSettings()
        {
            CreateProperties();
            Reset();
        }

        public void Reset()
        {
            InitializePropertiesByDefaultValue();

            // TODO: MUSTDO: NOTE: keep the below lines, but certainly the firmware should be fixed!
            CaptureImageSize.Size = new int[2] { 768, 480 };
            CameraViewImageSourceRectInCapturedImage.Rect = new int[4] { 8, 0, 752, 470 };
        }

        public void InitializeOnceAtStartup()
        {
            AddPropertiesToHidAccessPropertyList();
            AttachInternalEventHandlers();
        }

        internal void AttachInternalEventHandlers()
        {
            foreach (var item in HidAccessPropertyList)
            {
                item.ValueUpdated += delegate
                {
                    if (refToCurrentConnectedEgsDevice == null) { return; }
                    HidAccessPropertyUpdatedEventArgs e = new HidAccessPropertyUpdatedEventArgs(item);
                    OnHidAccessPropertyUpdated(e);
                };
            }

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
                if (refToCurrentConnectedEgsDevice != null)
                {
                    refToCurrentConnectedEgsDevice.ResetHidReportObjects();
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
                if (refToCurrentConnectedEgsDevice != null)
                {
                    refToCurrentConnectedEgsDevice.ResetHidReportObjects();
                }
            };

            SetEventHandlersAboutDependentProperties();
        }
    }
}

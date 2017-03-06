namespace Egs
{
    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDevice
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        int _IndexInHidDevicePathList;
        public event EventHandler IndexInHidDevicePathListChanged;
        protected virtual void OnIndexInHidDevicePathListChanged(EventArgs e)
        {
            var t = IndexInHidDevicePathListChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IndexInHidDevicePathList));
        }
        public int IndexInHidDevicePathList
        {
            get { return _IndexInHidDevicePathList; }
            internal set
            {
                _IndexInHidDevicePathList = value; OnIndexInHidDevicePathListChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsConnected;
        public event EventHandler IsConnectedChanged;
        protected virtual void OnIsConnectedChanged(EventArgs e)
        {
            var t = IsConnectedChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsConnected));
        }
        public bool IsConnected
        {
            get { return _IsConnected; }
            internal set
            {
                _IsConnected = value; OnIsConnectedChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsUpdatingFirmware;
        internal event EventHandler IsUpdatingFirmwareChanged;
        protected internal virtual void OnIsUpdatingFirmwareChanged(EventArgs e)
        {
            var t = IsUpdatingFirmwareChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsUpdatingFirmware));
        }
        internal bool IsUpdatingFirmware
        {
            get { return _IsUpdatingFirmware; }
            set
            {
                _IsUpdatingFirmware = value; OnIsUpdatingFirmwareChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsSendingTouchScreenHidReport;
        public event EventHandler IsSendingTouchScreenHidReportChanged;
        protected virtual void OnIsSendingTouchScreenHidReportChanged(EventArgs e)
        {
            var t = IsSendingTouchScreenHidReportChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsSendingTouchScreenHidReport));
        }
        public bool IsSendingTouchScreenHidReport
        {
            get { return _IsSendingTouchScreenHidReport; }
            internal set
            {
                _IsSendingTouchScreenHidReport = value; OnIsSendingTouchScreenHidReportChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsSendingHoveringStateOnTouchScreenHidReport;
        public event EventHandler IsSendingHoveringStateOnTouchScreenHidReportChanged;
        protected virtual void OnIsSendingHoveringStateOnTouchScreenHidReportChanged(EventArgs e)
        {
            var t = IsSendingHoveringStateOnTouchScreenHidReportChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsSendingHoveringStateOnTouchScreenHidReport));
        }
        public bool IsSendingHoveringStateOnTouchScreenHidReport
        {
            get { return _IsSendingHoveringStateOnTouchScreenHidReport; }
            internal set
            {
                _IsSendingHoveringStateOnTouchScreenHidReport = value; OnIsSendingHoveringStateOnTouchScreenHidReportChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsSendingEgsGestureHidReport;
        public event EventHandler IsSendingEgsGestureHidReportChanged;
        protected virtual void OnIsSendingEgsGestureHidReportChanged(EventArgs e)
        {
            var t = IsSendingEgsGestureHidReportChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsSendingEgsGestureHidReport));
        }
        public bool IsSendingEgsGestureHidReport
        {
            get { return _IsSendingEgsGestureHidReport; }
            internal set
            {
                _IsSendingEgsGestureHidReport = value; OnIsSendingEgsGestureHidReportChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsMonitoringTemperature;
        public event EventHandler IsMonitoringTemperatureChanged;
        protected virtual void OnIsMonitoringTemperatureChanged(EventArgs e)
        {
            var t = IsMonitoringTemperatureChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsMonitoringTemperature));
        }
        public bool IsMonitoringTemperature
        {
            get { return _IsMonitoringTemperature; }
            internal set
            {
                _IsMonitoringTemperature = value; OnIsMonitoringTemperatureChanged(EventArgs.Empty);
            }
        }

    }
}


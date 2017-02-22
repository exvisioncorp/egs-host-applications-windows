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
            OnPropertyChanged("IndexInHidDevicePathList");
        }
        public int IndexInHidDevicePathList
        {
            get { return _IndexInHidDevicePathList; }
            private set
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
            OnPropertyChanged("IsConnected");
        }
        public bool IsConnected
        {
            get { return _IsConnected; }
            private set
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
            OnPropertyChanged("IsUpdatingFirmware");
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
        bool _IsDetectingFaces;
        public event EventHandler IsDetectingFacesChanged;
        protected virtual void OnIsDetectingFacesChanged(EventArgs e)
        {
            var t = IsDetectingFacesChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsDetectingFaces");
        }
        public bool IsDetectingFaces
        {
            get { return _IsDetectingFaces; }
            private set
            {
                _IsDetectingFaces = value; OnIsDetectingFacesChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsDetectingHands;
        public event EventHandler IsDetectingHandsChanged;
        protected virtual void OnIsDetectingHandsChanged(EventArgs e)
        {
            var t = IsDetectingHandsChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsDetectingHands");
        }
        public bool IsDetectingHands
        {
            get { return _IsDetectingHands; }
            private set
            {
                _IsDetectingHands = value; OnIsDetectingHandsChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsSendingTouchScreenHidReport;
        public event EventHandler IsSendingTouchScreenHidReportChanged;
        protected virtual void OnIsSendingTouchScreenHidReportChanged(EventArgs e)
        {
            var t = IsSendingTouchScreenHidReportChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsSendingTouchScreenHidReport");
        }
        public bool IsSendingTouchScreenHidReport
        {
            get { return _IsSendingTouchScreenHidReport; }
            private set
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
            OnPropertyChanged("IsSendingHoveringStateOnTouchScreenHidReport");
        }
        public bool IsSendingHoveringStateOnTouchScreenHidReport
        {
            get { return _IsSendingHoveringStateOnTouchScreenHidReport; }
            private set
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
            OnPropertyChanged("IsSendingEgsGestureHidReport");
        }
        public bool IsSendingEgsGestureHidReport
        {
            get { return _IsSendingEgsGestureHidReport; }
            private set
            {
                _IsSendingEgsGestureHidReport = value; OnIsSendingEgsGestureHidReportChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToUseDefaultFaceDetection;
        public event EventHandler IsToUseDefaultFaceDetectionChanged;
        protected virtual void OnIsToUseDefaultFaceDetectionChanged(EventArgs e)
        {
            var t = IsToUseDefaultFaceDetectionChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsToUseDefaultFaceDetection");
        }
        public bool IsToUseDefaultFaceDetection
        {
            get { return _IsToUseDefaultFaceDetection; }
            set
            {
                _IsToUseDefaultFaceDetection = value; OnIsToUseDefaultFaceDetectionChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToMonitorTemperature;
        public event EventHandler IsToMonitorTemperatureChanged;
        protected virtual void OnIsToMonitorTemperatureChanged(EventArgs e)
        {
            var t = IsToMonitorTemperatureChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsToMonitorTemperature");
        }
        public bool IsToMonitorTemperature
        {
            get { return _IsToMonitorTemperature; }
            set
            {
                _IsToMonitorTemperature = value; OnIsToMonitorTemperatureChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsMonitoringTemperature;
        public event EventHandler IsMonitoringTemperatureChanged;
        protected virtual void OnIsMonitoringTemperatureChanged(EventArgs e)
        {
            var t = IsMonitoringTemperatureChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsMonitoringTemperature");
        }
        public bool IsMonitoringTemperature
        {
            get { return _IsMonitoringTemperature; }
            private set
            {
                _IsMonitoringTemperature = value; OnIsMonitoringTemperatureChanged(EventArgs.Empty);
            }
        }

    }
}


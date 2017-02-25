namespace Egs
{
    using System;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDeviceSettings
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToMonitorTemperature;
        public event EventHandler IsToMonitorTemperatureChanged;
        protected virtual void OnIsToMonitorTemperatureChanged(EventArgs e)
        {
            var t = IsToMonitorTemperatureChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("IsToMonitorTemperature");
        }
        [DataMember]
        public bool IsToMonitorTemperature
        {
            get { return _IsToMonitorTemperature; }
            set
            {
                _IsToMonitorTemperature = value; OnIsToMonitorTemperatureChanged(EventArgs.Empty);
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
        [DataMember]
        public bool IsToUseDefaultFaceDetection
        {
            get { return _IsToUseDefaultFaceDetection; }
            set
            {
                _IsToUseDefaultFaceDetection = value; OnIsToUseDefaultFaceDetectionChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        OptionalValue<FaceDetectionIsProcessedByDetail> _FaceDetectionIsProcessedBy;
        public event EventHandler FaceDetectionIsProcessedByChanged;
        protected virtual void OnFaceDetectionIsProcessedByChanged(EventArgs e)
        {
            var t = FaceDetectionIsProcessedByChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("FaceDetectionIsProcessedBy");
        }
        [DataMember]
        public OptionalValue<FaceDetectionIsProcessedByDetail> FaceDetectionIsProcessedBy
        {
            get { return _FaceDetectionIsProcessedBy; }
            private set
            {
                _FaceDetectionIsProcessedBy = value; OnFaceDetectionIsProcessedByChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        RangedDouble _FaceDetectionOnHost_RealFaceZMaximum;
        public event EventHandler FaceDetectionOnHost_RealFaceZMaximumChanged;
        protected virtual void OnFaceDetectionOnHost_RealFaceZMaximumChanged(EventArgs e)
        {
            var t = FaceDetectionOnHost_RealFaceZMaximumChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("FaceDetectionOnHost_RealFaceZMaximum");
        }
        [DataMember]
        public RangedDouble FaceDetectionOnHost_RealFaceZMaximum
        {
            get { return _FaceDetectionOnHost_RealFaceZMaximum; }
            private set
            {
                _FaceDetectionOnHost_RealFaceZMaximum = value; OnFaceDetectionOnHost_RealFaceZMaximumChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        RangedDouble _FaceDetectionOnHost_Threshold;
        public event EventHandler FaceDetectionOnHost_ThresholdChanged;
        protected virtual void OnFaceDetectionOnHost_ThresholdChanged(EventArgs e)
        {
            var t = FaceDetectionOnHost_ThresholdChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("FaceDetectionOnHost_Threshold");
        }
        [DataMember]
        public RangedDouble FaceDetectionOnHost_Threshold
        {
            get { return _FaceDetectionOnHost_Threshold; }
            private set
            {
                _FaceDetectionOnHost_Threshold = value; OnFaceDetectionOnHost_ThresholdChanged(EventArgs.Empty);
            }
        }

    }
}


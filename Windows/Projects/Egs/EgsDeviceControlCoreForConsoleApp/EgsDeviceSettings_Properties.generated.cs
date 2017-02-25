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

    }
}


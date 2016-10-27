namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Media.Imaging;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using Egs;
    using Egs.Views;
    using Egs.DotNetUtility;

    [DataContract]
    class LauncherCursorSpeedAndPrecisionModeAndRecommendedAppsViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        CursorSpeedAndPrecisionModeButtonModel _ModeButtonModel;
        [DataMember]
        internal CursorSpeedAndPrecisionModeButtonModel ModeButtonModel
        {
            get { return _ModeButtonModel; }
            set { _ModeButtonModel = value; OnPropertyChanged("ModeButtonModel"); }
        }
        string _ModeDescriptionText;
        [DataMember]
        internal string ModeDescriptionText
        {
            get { return _ModeDescriptionText; }
            set { _ModeDescriptionText = value; OnPropertyChanged("ModeDescriptionText"); }
        }
        string _RecommendedAppsToPlayText;
        [DataMember]
        internal string RecommendedAppsToPlayText
        {
            get { return _RecommendedAppsToPlayText; }
            set { _RecommendedAppsToPlayText = value; OnPropertyChanged("RecommendedAppsToPlayText"); }
        }
        LauncherRecommendedAppViewModel _RecommendedAppLeft;
        [DataMember]
        internal LauncherRecommendedAppViewModel RecommendedAppLeft
        {
            get { return _RecommendedAppLeft; }
            set { _RecommendedAppLeft = value; OnPropertyChanged("RecommendedAppLeft"); }
        }
        LauncherRecommendedAppViewModel _RecommendedAppRight;
        [DataMember]
        internal LauncherRecommendedAppViewModel RecommendedAppRight
        {
            get { return _RecommendedAppRight; }
            set { _RecommendedAppRight = value; OnPropertyChanged("RecommendedAppRight"); }
        }
    }
}

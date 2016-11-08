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
    class LauncherRecommendedAppViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        string _AppTitle;
        [DataMember]
        public string AppTitle
        {
            get { return _AppTitle; }
            set { _AppTitle = value; OnPropertyChanged("AppTitle"); }
        }

        LaunchingOtherApplicationButtonModel _LaunchingOtherApplicationButtonModel;
        [DataMember]
        public LaunchingOtherApplicationButtonModel LaunchingOtherApplicationButtonModel
        {
            get { return _LaunchingOtherApplicationButtonModel; }
            set { _LaunchingOtherApplicationButtonModel = value; OnPropertyChanged("LaunchingOtherApplicationButtonModel"); }
        }

        string _AppIconImageSourceRelativeFolderPath;
        [DataMember]
        public string AppIconImageSourceRelativeFolderPath
        {
            get { return _AppIconImageSourceRelativeFolderPath; }
            set
            {
                _AppIconImageSourceRelativeFolderPath = value;
                UpdateAppIconImageSource();
                OnPropertyChanged("AppIconImageSourceRelativeFolderPath");
            }
        }
        string _AppIconImageSourceFileName;
        [DataMember]
        public string AppIconImageSourceFileName
        {
            get { return _AppIconImageSourceFileName; }
            set
            {
                _AppIconImageSourceFileName = value;
                UpdateAppIconImageSource();
                OnPropertyChanged("AppIconImageSourceFileName");
            }
        }
        public string AppIconImageSourceFilePath { get; private set; }
        public BitmapImage AppIconImageSource { get; private set; }
        void UpdateAppIconImageSource()
        {
            AppIconImageSourceFilePath = _AppIconImageSourceRelativeFolderPath + _AppIconImageSourceFileName;
            AppIconImageSource = BitmapImageUtility.LoadBitmapImageFromFile(AppIconImageSourceFilePath);
            OnPropertyChanged("AppIconImageSourceFilePath");
            OnPropertyChanged("AppIconImageSource");
        }
    }
}

namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.ComponentModel;

    class TutorialUpperSideMessageAreaViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        string _LeftTextBlockText;
        public string LeftTextBlockText
        {
            get { return _LeftTextBlockText; }
            set { _LeftTextBlockText = value; OnPropertyChanged(nameof(LeftTextBlockText)); }
        }
        string _RightTextBlockText;
        public string RightTextBlockText
        {
            get { return _RightTextBlockText; }
            set { _RightTextBlockText = value; OnPropertyChanged(nameof(RightTextBlockText)); }
        }
        Visibility _Visibility;
        public Visibility Visibility
        {
            get { return _Visibility; }
            set { _Visibility = value; OnPropertyChanged(nameof(Visibility)); }
        }

        public TutorialUpperSideMessageAreaViewModel()
        {
            _RightTextBlockText = "";
            _LeftTextBlockText = "";
            _Visibility = Visibility.Collapsed;
        }
    }

    class TutorialUpperSideMessageAreaViewModelExample : TutorialUpperSideMessageAreaViewModel
    {
        public TutorialUpperSideMessageAreaViewModelExample()
            : base()
        {
            // TODO: make localized string
            RightTextBlockText = $"{ApplicationCommonSettings.HostApplicationName} will be in the center of the screen.\n"
                + "Sit away 7ft.\n"
                + "Toward the front of the camera, in the middle.\n"
                + "To adjust the angle of the camera to see the Installation Guide.\n";
            LeftTextBlockText = "Well done.";
            Visibility = Visibility.Visible;
        }
    }
}

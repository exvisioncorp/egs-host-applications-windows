namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using Egs.DotNetUtility;

    [DataContract]
    partial class ImageButtonModel : ButtonModelBase
    {
        #region IsEnabled
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsEnabled;
        public event EventHandler IsEnabledChanged;
        protected virtual void OnIsEnabledChanged(EventArgs e)
        {
            var t = IsEnabledChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsEnabled));
        }
        public bool IsEnabled
        {
            get { return _IsEnabled; }
            set
            {
                _IsEnabled = value;
                UpdateImageSource();
                OnIsEnabledChanged(EventArgs.Empty);
            }
        }
        #endregion

        #region IsPressed
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsPressed;
        public event EventHandler IsPressedChanged;
        protected virtual void OnIsPressedChanged(EventArgs e)
        {
            var t = IsPressedChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsPressed));
        }
        public bool IsPressed
        {
            get { return _IsPressed; }
            set
            {
                if (_IsPressed == value) { return; }
                _IsPressed = value;
                UpdateImageSource();
                OnIsPressedChanged(EventArgs.Empty);
            }
        }
        #endregion

        #region IsHovered
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsHovered;
        public event EventHandler IsHoveredChanged;
        protected virtual void OnIsHoveredChanged(EventArgs e)
        {
            var t = IsHoveredChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsHovered));
        }
        public bool IsHovered
        {
            get { return _IsHovered; }
            set
            {
                if (_IsHovered == value) { return; }
                _IsHovered = value;
                UpdateImageSource();
                OnIsHoveredChanged(EventArgs.Empty);
            }
        }
        #endregion

        #region IsSelected
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsSelected;
        public event EventHandler IsSelectedChanged;
        protected virtual void OnIsSelectedChanged(EventArgs e)
        {
            var t = IsSelectedChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsSelected));
        }
        public bool IsSelected
        {
            get { return _IsSelected; }
            set
            {
                _IsSelected = value;
                UpdateImageSource();
                OnIsSelectedChanged(EventArgs.Empty);
            }
        }
        #endregion

        string _ImageSourceRelativeFolderPath;
        [DataMember]
        public string ImageSourceRelativeFolderPath
        {
            get { return _ImageSourceRelativeFolderPath; }
            set
            {
                _ImageSourceRelativeFolderPath = value;
                UpdateImageSourceDisabled();
                UpdateImageSourcePressed();
                UpdateImageSourceHovered();
                UpdateImageSourceSelected();
                UpdateImageSourceEnabled();
                UpdateImageSource();
                OnPropertyChanged(nameof(ImageSourceRelativeFolderPath));
            }
        }

        #region Disabled
        public BitmapImage ImageSourceDisabled { get; private set; }
        string _ImageSourceDisabledFileName;
        [DataMember]
        public string ImageSourceDisabledFileName
        {
            get { return _ImageSourceDisabledFileName; }
            set
            {
                _ImageSourceDisabledFileName = value;
                UpdateImageSourceDisabled();
                UpdateImageSource();
                OnPropertyChanged(nameof(ImageSourceDisabledFileName));
            }
        }
        void UpdateImageSourceDisabled()
        {
            var newImageSourceFilePath = _ImageSourceRelativeFolderPath + _ImageSourceDisabledFileName;
            ImageSourceDisabled = BitmapImageUtility.LoadBitmapImageFromFile(newImageSourceFilePath);
            OnPropertyChanged(nameof(ImageSourceDisabled));
        }
        #endregion

        #region Pressed
        public BitmapImage ImageSourcePressed { get; private set; }
        string _ImageSourcePressedFileName;
        [DataMember]
        public string ImageSourcePressedFileName
        {
            get { return _ImageSourcePressedFileName; }
            set
            {
                _ImageSourcePressedFileName = value;
                UpdateImageSourcePressed();
                UpdateImageSource();
                OnPropertyChanged(nameof(ImageSourcePressedFileName));
            }
        }
        void UpdateImageSourcePressed()
        {
            var newImageSourceFilePath = _ImageSourceRelativeFolderPath + _ImageSourcePressedFileName;
            ImageSourcePressed = BitmapImageUtility.LoadBitmapImageFromFile(newImageSourceFilePath);
            OnPropertyChanged(nameof(ImageSourcePressed));
        }
        #endregion

        #region Hovered
        public BitmapImage ImageSourceHovered { get; private set; }
        string _ImageSourceHoveredFileName;
        [DataMember]
        public string ImageSourceHoveredFileName
        {
            get { return _ImageSourceHoveredFileName; }
            set
            {
                _ImageSourceHoveredFileName = value;
                UpdateImageSourceHovered();
                UpdateImageSource();
                OnPropertyChanged(nameof(ImageSourceHoveredFileName));
            }
        }
        void UpdateImageSourceHovered()
        {
            var newImageSourceFilePath = _ImageSourceRelativeFolderPath + _ImageSourceHoveredFileName;
            ImageSourceHovered = BitmapImageUtility.LoadBitmapImageFromFile(newImageSourceFilePath);
            OnPropertyChanged(nameof(ImageSourceHovered));
        }
        #endregion

        #region Selected
        public BitmapImage ImageSourceSelected { get; private set; }
        string _ImageSourceSelectedFileName;
        [DataMember]
        public string ImageSourceSelectedFileName
        {
            get { return _ImageSourceSelectedFileName; }
            set
            {
                _ImageSourceSelectedFileName = value;
                UpdateImageSourceSelected();
                UpdateImageSource();
                OnPropertyChanged(nameof(ImageSourceSelectedFileName));
            }
        }
        void UpdateImageSourceSelected()
        {
            var newImageSourceFilePath = _ImageSourceRelativeFolderPath + _ImageSourceSelectedFileName;
            ImageSourceSelected = BitmapImageUtility.LoadBitmapImageFromFile(newImageSourceFilePath);
            OnPropertyChanged(nameof(ImageSourceSelected));
        }
        #endregion

        #region Enabled
        public BitmapImage ImageSourceEnabled { get; private set; }
        string _ImageSourceEnabledFileName;
        [DataMember]
        public string ImageSourceEnabledFileName
        {
            get { return _ImageSourceEnabledFileName; }
            set
            {
                _ImageSourceEnabledFileName = value;
                UpdateImageSourceEnabled();
                UpdateImageSource();
                OnPropertyChanged(nameof(ImageSourceEnabledFileName));
            }
        }
        void UpdateImageSourceEnabled()
        {
            var newImageSourceFilePath = _ImageSourceRelativeFolderPath + _ImageSourceEnabledFileName;
            ImageSourceEnabled = BitmapImageUtility.LoadBitmapImageFromFile(newImageSourceFilePath);
            OnPropertyChanged(nameof(ImageSourceEnabled));
        }
        #endregion

        public BitmapImage ImageSource { get; private set; }
        void UpdateImageSource()
        {
            BitmapImage newImageSource = null;
            const bool isToShowDebugMessage = false;
            if (IsEnabled == false) { newImageSource = ImageSourceDisabled; if (isToShowDebugMessage) { Debug.WriteLine("Disabled"); } }
            else if (IsPressed) { newImageSource = ImageSourcePressed; if (isToShowDebugMessage) { Debug.WriteLine("Pressed"); } }
            else if (IsHovered) { newImageSource = ImageSourceHovered; if (isToShowDebugMessage) { Debug.WriteLine("Hovered"); } }
            else if (IsSelected) { newImageSource = ImageSourceSelected; if (isToShowDebugMessage) { Debug.WriteLine("Selected"); } }
            else { newImageSource = ImageSourceEnabled; if (isToShowDebugMessage) { Debug.WriteLine("Enabled"); } }

            ImageSource = newImageSource;
            OnPropertyChanged(nameof(ImageSource));
        }

        public ImageButtonModel()
            : base()
        {
            _IsEnabled = true;
            _IsPressed = false;
            _IsHovered = false;
            _IsSelected = false;
        }
    }
}

namespace Egs
{
    using System;
    using System.Windows;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class CameraViewUserControlModel
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToDrawImageSet;
        public event EventHandler IsToDrawImageSetChanged;
        protected virtual void OnIsToDrawImageSetChanged(EventArgs e)
        {
            var t = IsToDrawImageSetChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsToDrawImageSet));
        }
        [DataMember]
        public bool IsToDrawImageSet
        {
            get { return _IsToDrawImageSet; }
            set
            {
                _IsToDrawImageSet = value; OnIsToDrawImageSetChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Windows.Media.Imaging.BitmapSource _CameraViewWpfBitmapSource;
        public event EventHandler CameraViewWpfBitmapSourceChanged;
        protected virtual void OnCameraViewWpfBitmapSourceChanged(EventArgs e)
        {
            var t = CameraViewWpfBitmapSourceChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewWpfBitmapSource));
        }
        public System.Windows.Media.Imaging.BitmapSource CameraViewWpfBitmapSource
        {
            get { return _CameraViewWpfBitmapSource; }
            private set
            {
                _CameraViewWpfBitmapSource = value; OnCameraViewWpfBitmapSourceChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _CameraViewWpfBitmapSourceWidth;
        public event EventHandler CameraViewWpfBitmapSourceWidthChanged;
        protected virtual void OnCameraViewWpfBitmapSourceWidthChanged(EventArgs e)
        {
            var t = CameraViewWpfBitmapSourceWidthChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewWpfBitmapSourceWidth));
        }
        public int CameraViewWpfBitmapSourceWidth
        {
            get { return _CameraViewWpfBitmapSourceWidth; }
            private set
            {
                _CameraViewWpfBitmapSourceWidth = value; OnCameraViewWpfBitmapSourceWidthChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        int _CameraViewWpfBitmapSourceHeight;
        public event EventHandler CameraViewWpfBitmapSourceHeightChanged;
        protected virtual void OnCameraViewWpfBitmapSourceHeightChanged(EventArgs e)
        {
            var t = CameraViewWpfBitmapSourceHeightChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(CameraViewWpfBitmapSourceHeight));
        }
        public int CameraViewWpfBitmapSourceHeight
        {
            get { return _CameraViewWpfBitmapSourceHeight; }
            private set
            {
                _CameraViewWpfBitmapSourceHeight = value; OnCameraViewWpfBitmapSourceHeightChanged(EventArgs.Empty);
            }
        }

    }
}


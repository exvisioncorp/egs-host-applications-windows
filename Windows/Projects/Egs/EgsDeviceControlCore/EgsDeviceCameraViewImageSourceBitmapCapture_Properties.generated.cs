namespace Egs
{
    using System;
    using System.Windows.Forms;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;

    public partial class EgsDeviceCameraViewImageSourceBitmapCapture
    {
        [EditorBrowsable(EditorBrowsableState.Never)]
        uint? _VideoCaptureDeviceIndex;
        public event EventHandler VideoCaptureDeviceIndexChanged;
        protected virtual void OnVideoCaptureDeviceIndexChanged(EventArgs e)
        {
            var t = VideoCaptureDeviceIndexChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("VideoCaptureDeviceIndex");
        }
        public uint? VideoCaptureDeviceIndex
        {
            get { return _VideoCaptureDeviceIndex; }
            internal set
            {
                _VideoCaptureDeviceIndex = value; OnVideoCaptureDeviceIndexChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string _VideoCaptureDeviceName;
        public event EventHandler VideoCaptureDeviceNameChanged;
        protected virtual void OnVideoCaptureDeviceNameChanged(EventArgs e)
        {
            var t = VideoCaptureDeviceNameChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("VideoCaptureDeviceName");
        }
        public string VideoCaptureDeviceName
        {
            get { return _VideoCaptureDeviceName; }
            internal set
            {
                _VideoCaptureDeviceName = value; OnVideoCaptureDeviceNameChanged(EventArgs.Empty);
            }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        string _VideoCaptureDeviceDevicePath;
        public event EventHandler VideoCaptureDeviceDevicePathChanged;
        protected virtual void OnVideoCaptureDeviceDevicePathChanged(EventArgs e)
        {
            var t = VideoCaptureDeviceDevicePathChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("VideoCaptureDeviceDevicePath");
        }
        public string VideoCaptureDeviceDevicePath
        {
            get { return _VideoCaptureDeviceDevicePath; }
            internal set
            {
                _VideoCaptureDeviceDevicePath = value; OnVideoCaptureDeviceDevicePathChanged(EventArgs.Empty);
            }
        }

    }
}


namespace FixedHandDetectionAreasExample01
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
    using System.Windows.Media.Animation;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Runtime.Serialization;
    using System.Collections.ObjectModel;
    using Egs;
    using Egs.Views;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;

    public class CameraViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        System.Windows.Media.Imaging.BitmapSource _CameraViewWpfBitmapSource;
        public event EventHandler CameraViewWpfBitmapSourceChanged;
        protected virtual void OnCameraViewWpfBitmapSourceChanged(EventArgs e)
        {
            var t = CameraViewWpfBitmapSourceChanged; if (t != null) { t(this, e); }
            OnPropertyChanged("CameraViewWpfBitmapSource");
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
            OnPropertyChanged("CameraViewWpfBitmapSourceWidth");
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
            OnPropertyChanged("CameraViewWpfBitmapSourceHeight");
        }
        public int CameraViewWpfBitmapSourceHeight
        {
            get { return _CameraViewWpfBitmapSourceHeight; }
            private set
            {
                _CameraViewWpfBitmapSourceHeight = value; OnCameraViewWpfBitmapSourceHeightChanged(EventArgs.Empty);
            }
        }

        public EgsDevice Device { get; private set; }
        System.Windows.Int32Rect CameraViewWpfBitmapSourceWritePixelsSourceRect { get; set; }

        public CameraViewModel()
        {

        }

        public void InitializeOnceAtStartup(EgsDevice device)
        {
            Trace.Assert(device != null);
            Trace.Assert(device.CameraViewImageSourceBitmapCapture != null);
            Device = device;
            Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapSizeOrPixelFormatChanged += delegate
            {
                CreateCameraViewWpfBitmapSource();
            };
            Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += delegate
            {
                UpdateCameraViewWpfBitmapSource();
            };
        }

        internal void CreateCameraViewWpfBitmapSource()
        {
            try
            {
                var capture = Device.CameraViewImageSourceBitmapCapture;
                var width = capture.CameraViewImageSourceBitmapSize.Width;
                var height = capture.CameraViewImageSourceBitmapSize.Height;
                Debug.Assert(width > 0 && height > 0);
                CameraViewWpfBitmapSource = new System.Windows.Media.Imaging.WriteableBitmap(width, height, 96, 96, System.Windows.Media.PixelFormats.Bgr24, null);
                CameraViewWpfBitmapSourceWidth = width;
                CameraViewWpfBitmapSourceHeight = height;
                CameraViewWpfBitmapSourceWritePixelsSourceRect = new System.Windows.Int32Rect(0, 0, width, height);
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Debug.WriteLine(ex.Message);
            }
        }

        internal void UpdateCameraViewWpfBitmapSource()
        {
            using (var ms = new System.IO.MemoryStream())
            {
                Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap.Save(ms, System.Drawing.Imaging.ImageFormat.Bmp);
                ms.Position = 0;
                var newImage = new BitmapImage();
                newImage.BeginInit();
                // The next line causes System.ObjectDisposedException.
                //newImage.StreamSource = ms;
                // new MemoryStream(ms.ToArray()) is necessary.
                newImage.StreamSource = new System.IO.MemoryStream(ms.ToArray());
                newImage.EndInit();
                newImage.Freeze();
                CameraViewWpfBitmapSource = newImage;
            }
        }
    }
}

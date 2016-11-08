namespace Egs
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
    using Egs.Views;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;
    using Egs.DotNetUtility.Views;

    /// <summary>
    /// ViewModel of CameraViewUserControl.  When you use CameraViewUserControl,
    /// (1) Create an object of this ViewModel
    /// (2) Initialize it by (EgsDevice device)
    /// (3) Send it to CameraViewUserControl.InitializeOnceAtStartup() as an argument
    /// </summary>
    public partial class CameraViewUserControlModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        System.Windows.Int32Rect CameraViewWpfBitmapSourceWritePixelsSourceRect { get; set; }

        /// <summary>Reference to an EgsDevice object.  Users have to set this as an argument of InitializeOnceAtStartup() method.</summary>
        public EgsDevice Device { get; private set; }
        public EgsDeviceEgsGestureHidReport Report { get { return Device.EgsGestureHidReport; } }
        public bool IsFaceDetecting { get { return Report.IsFaceDetecting; } }
        public Rect FaceDetectionArea { get { return Report.FaceDetectionArea.ToWpfRect(); } }
        public EgsDeviceEgsGestureHidReportFace Face0 { get { return Report.Faces[0]; } }
        public EgsDeviceEgsGestureHidReportFace Face1 { get { return Report.Faces[1]; } }
        public EgsDeviceEgsGestureHidReportFace Face2 { get { return Report.Faces[2]; } }
        public EgsDeviceEgsGestureHidReportFace Face3 { get { return Report.Faces[3]; } }
        public EgsDeviceEgsGestureHidReportFace Face4 { get { return Report.Faces[4]; } }
        public EgsDeviceEgsGestureHidReportHand Hand0 { get { return Report.Hands[0]; } }
        public EgsDeviceEgsGestureHidReportHand Hand1 { get { return Report.Hands[1]; } }

        internal void RaiseMultipleObjectsPropertyChanged()
        {
            // NOTE: The properties are not independent properties which have data fields, just references to model properties.
            OnPropertyChanged("CameraViewWpfBitmapSource");
            OnPropertyChanged("CameraViewWpfBitmapSourceWidth");
            OnPropertyChanged("CameraViewWpfBitmapSourceHeight");
            OnPropertyChanged("Device");
            OnPropertyChanged("Report");
            OnPropertyChanged("IsFaceDetecting");
            OnPropertyChanged("FaceDetectionArea");
            OnPropertyChanged("Face0");
            OnPropertyChanged("Face1");
            OnPropertyChanged("Face2");
            OnPropertyChanged("Face3");
            OnPropertyChanged("Face4");
            OnPropertyChanged("Hand0");
            OnPropertyChanged("Hand1");
        }

        public CameraViewUserControlModel()
        {
            _IsToDrawImageSet = true;
            _CameraViewWpfBitmapSourceWidth = 384;
            _CameraViewWpfBitmapSourceHeight = 240;
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
                RaiseMultipleObjectsPropertyChanged();
            };
        }

        public void Reset()
        {
            IsToDrawImageSet = true;
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

        internal static CameraViewUserControlModel CameraViewUserControlModelForXamlDesign
        {
            get
            {
                var ret = new CameraViewUserControlModel();
                var device = EgsDevice.CreateEgsDeviceForXamlDesign();
                if (device.Settings.CaptureImageSize.Width == 0 || device.Settings.CaptureImageSize.Height == 0)
                {
                    // NOTE: Currently the host app can get the value from device.
                    device.Settings.CaptureImageSize.Width = 768;
                    device.Settings.CaptureImageSize.Height = 480;
                    //device.Settings.CaptureImageSize.Width = 960;
                    //device.Settings.CaptureImageSize.Height = 540;
                }
                device.Settings.CameraViewImageSourceRectInCapturedImage.Rect = new System.Drawing.Rectangle(0, 0, 768, 480);
                device.Settings.OnImageSizeRelatedPropertiesUpdated();

                ret.InitializeOnceAtStartup(device);
                ret.CreateCameraViewWpfBitmapSource();

                device.EgsGestureHidReport.MessageId = EgsGestureHidReportMessageIds.DetectingFaces;
                device.EgsGestureHidReport.FaceDetectionArea = new System.Drawing.Rectangle(0, 50, 384, 140);
                device.EgsGestureHidReport.Faces[0].Area = new System.Drawing.Rectangle(70, 70, 50, 50);
                device.EgsGestureHidReport.Faces[1].Area = new System.Drawing.Rectangle(170, 70, 50, 50);
                device.EgsGestureHidReport.Faces[2].Area = new System.Drawing.Rectangle(70, 170, 50, 50);
                device.EgsGestureHidReport.Faces[3].Area = new System.Drawing.Rectangle(170, 170, 50, 50);
                device.EgsGestureHidReport.Faces[4].Area = new System.Drawing.Rectangle(270, 170, 50, 50);
                device.EgsGestureHidReport.Faces.Select(e => e.IsDetected = true);
                device.EgsGestureHidReport.Faces[3].IsSelected = true;

                device.EgsGestureHidReport.Hands[0].IsDetecting = true;
                device.EgsGestureHidReport.Hands[1].IsDetecting = true;
                device.EgsGestureHidReport.Hands[0].DetectionArea = new System.Drawing.Rectangle(100, 100, 150, 150);
                device.EgsGestureHidReport.Hands[1].DetectionArea = new System.Drawing.Rectangle(300, 100, 150, 150);
                device.EgsGestureHidReport.Hands[0].IsTracking = true;
                device.EgsGestureHidReport.Hands[1].IsTracking = true;
                device.EgsGestureHidReport.Hands[0].TrackingArea = new System.Drawing.Rectangle(120, 120, 100, 100);
                device.EgsGestureHidReport.Hands[1].TrackingArea = new System.Drawing.Rectangle(320, 120, 100, 100);
                device.EgsGestureHidReport.Hands[0].ScreenMappedArea = new System.Drawing.Rectangle(150, 150, 50, 50);
                device.EgsGestureHidReport.Hands[1].ScreenMappedArea = new System.Drawing.Rectangle(350, 150, 50, 50);

                device.EgsGestureHidReport.Hands[0].X = 175;
                device.EgsGestureHidReport.Hands[0].Y = 175;
                ret.Device = device;
                return ret;
            }
        }
    }
}

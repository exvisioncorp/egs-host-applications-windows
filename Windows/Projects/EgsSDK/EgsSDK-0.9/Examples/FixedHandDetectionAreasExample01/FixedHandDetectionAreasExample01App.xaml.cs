namespace FixedHandDetectionAreasExample01
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Linq;
    using System.Windows;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using System.ComponentModel;
    using Egs;
    using Egs.Views;
    using Egs.DotNetUtility;

    public partial class FixedHandDetectionAreasExample01App : Application
    {
        public EgsDeviceSettings DeviceSettings { get; private set; }
        public EgsDevice Device { get; private set; }
        public IList<CursorViewModel> CursorViewModels { get; private set; }
        public IList<CursorForm> CursorViews { get; private set; }
        public CameraViewModel CameraViewBackgroundWindowModel { get; private set; }
        public FixedHandDetectionAreasExample01MainWindow CameraViewBackgroundWindow { get; private set; }
        public EgsHostFaceDetectionModel FaceDetection { get; private set; }

        public FixedHandDetectionAreasExample01App()
            : base()
        {
            Egs.BindableResources.Current.ChangeCulture("");
            if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
            {
                Application.Current.Shutdown();
                return;
            }

            DeviceSettings = new EgsDeviceSettings();
            DeviceSettings.InitializeOnceAtStartup();

            DeviceSettings.IsToDrawBordersOnCameraViewImageByDevice.Value = true;
            DeviceSettings.IsToDetectFaces.Value = false;
            DeviceSettings.IsToDetectHands.Value = true;
            DeviceSettings.IsToFixHandDetectionRegions.Value = true;
            // 0: 320*240  1:384*240  2:640*480
            DeviceSettings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedIndex = 2;

            Device = EgsDevice.GetDefaultEgsDevice(DeviceSettings);


            {
                CursorViewModels = new CursorViewModel[Device.TrackableHandsCountMaximum];
                CursorViewModels[0] = new CursorViewModel();
                CursorViewModels[1] = new CursorViewModel();
                CursorViews = new CursorForm[Device.TrackableHandsCountMaximum];
                CursorViews[0] = new CursorForm();
                CursorViews[1] = new CursorForm();
                var imageSetListRight = new List<ImageInformationSet>();
                var imageSetListLeft = new List<ImageInformationSet>();
                imageSetListRight.Add(new ImageInformationSet() { ImageSetIndex = 0, FolderPath = "Resources\\", SampleImageFileRelativePath = "Resources\\Sample.png" });
                imageSetListRight[0].AddImage((int)CursorImageIndexLabels.OpenHand, "Right_OpenHand.png");
                imageSetListRight[0].AddImage((int)CursorImageIndexLabels.CloseHand, "Right_CloseHand.png");
                imageSetListLeft.Add(new ImageInformationSet() { ImageSetIndex = 0, FolderPath = "Resources\\", SampleImageFileRelativePath = "Resources\\Sample.png" });
                imageSetListLeft[0].AddImage((int)CursorImageIndexLabels.OpenHand, "Left_OpenHand.png");
                imageSetListLeft[0].AddImage((int)CursorImageIndexLabels.CloseHand, "Left_CloseHand.png");
                CursorViews[0].InitializeOnceAtStartup(CursorViewModels[0], imageSetListRight);
                CursorViews[1].InitializeOnceAtStartup(CursorViewModels[1], imageSetListLeft);
                Device.EgsGestureHidReport.ReportUpdated += EgsGestureHidReport_ReportUpdated;
            }


            {
                FaceDetection = new EgsHostFaceDetectionModel();

                // TODO: check the minimum value.  200[ms]?
                var cameraViewImageSize = DeviceSettings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem;
                FaceDetection.CameraViewImageWidth = cameraViewImageSize.Width;
                FaceDetection.CameraViewImageHeight = cameraViewImageSize.Height;

                Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;
                FaceDetection.FaceDetectionCompleted += FaceDetection_FaceDetectionCompleted;
            }


            this.Exit += delegate
            {
                // By detaching this event handler, Exception does not happen.
                FaceDetection.FaceDetectionCompleted -= FaceDetection_FaceDetectionCompleted;
                Device.EgsGestureHidReport.ReportUpdated -= EgsGestureHidReport_ReportUpdated;
                Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged -= CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;

                DeviceSettings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
                DeviceSettings.IsToDetectFaces.Value = false;
                DeviceSettings.IsToDetectHands.Value = false;
                DeviceSettings.IsToFixHandDetectionRegions.Value = false;
                DeviceSettings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedIndex = 1;
                // Call this method before CursorView.Close().
                EgsDevice.CloseDefaultEgsDevice();

                foreach (var cursorView in CursorViews) { cursorView.Close(); }
                DuplicatedProcessStartBlocking.ReleaseMutex();
            };


            {
                CameraViewBackgroundWindowModel = new CameraViewModel();
                CameraViewBackgroundWindow = new FixedHandDetectionAreasExample01MainWindow();
                CameraViewBackgroundWindowModel.InitializeOnceAtStartup(Device);
                Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += delegate
                {
                    // Unfortunately, DataBinding does not seem to be able to solve "different thread" InvalidOperationException.
                    CameraViewBackgroundWindow.CameraViewImage.Dispatcher.InvokeAsync(() =>
                    {
                        CameraViewBackgroundWindow.CameraViewImage.Source = CameraViewBackgroundWindowModel.CameraViewWpfBitmapSource;
                    });
                };
                CameraViewBackgroundWindow.KeyDown += (sender, e) =>
                {
                    switch (e.Key)
                    {
                        case System.Windows.Input.Key.Escape:
                            CameraViewBackgroundWindow.Close();
                            break;
                    }
                };
                base.MainWindow = CameraViewBackgroundWindow;
                CameraViewBackgroundWindow.Show();
            }
        }

        void CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged(object sender, EventArgs e)
        {
            var isTrackingNoHand = (CursorViewModels[0].IsTracking == false) && (CursorViewModels[1].IsTracking == false);
            if (isTrackingNoHand)
            {
                FaceDetection.DetectFaceRunWorkerAsync(Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap);
            }
            if (FaceDetection.SelectedFaceRect.HasValue)
            {
                using (var g = System.Drawing.Graphics.FromImage(Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap))
                {
                    var pen = new System.Drawing.Pen(System.Drawing.Brushes.Green, 5);
                    g.DrawRectangle(pen, FaceDetection.SelectedFaceRect.Value);
                }
            }
        }

        void FaceDetection_FaceDetectionCompleted(object sender, EventArgs e)
        {
            FaceDetection.SelectOneFaceRect();
            if (FaceDetection.IsFaceDetected)
            {
                FaceDetection.UpdateSensorImageHandDetectionAreas(FaceDetection.SelectedFaceRect.Value);
            }

            var imageWidth = FaceDetection.SensorImageWidth;
            var imageHeight = FaceDetection.SensorImageHeight;
            var right = FaceDetection.SensorImageRightHandDetectionArea;
            var left = FaceDetection.SensorImageLeftHandDetectionArea;
            Debug.WriteLine("FaceDetection.HandDetectionScaleForEgsDevice: " + FaceDetection.HandDetectionScaleForEgsDevice);

            var RightHandDetectionAreaRatioRect = new Egs.DotNetUtility.RatioRect();
            var LeftHandDetectionAreaRatioRect = new Egs.DotNetUtility.RatioRect();
            RightHandDetectionAreaRatioRect.XRange.From = (float)(right.X / imageWidth);
            RightHandDetectionAreaRatioRect.XRange.To = (float)((right.X + right.Width) / imageWidth);
            RightHandDetectionAreaRatioRect.YRange.From = (float)(right.Y / imageHeight);
            RightHandDetectionAreaRatioRect.YRange.To = (float)((right.Y + right.Height) / imageHeight);
            LeftHandDetectionAreaRatioRect.XRange.From = (float)(left.X / imageWidth);
            LeftHandDetectionAreaRatioRect.XRange.To = (float)((left.X + left.Width) / imageWidth);
            LeftHandDetectionAreaRatioRect.YRange.From = (float)(left.Y / imageHeight);
            LeftHandDetectionAreaRatioRect.YRange.To = (float)((left.Y + left.Height) / imageHeight);
            DeviceSettings.RightHandDetectionAreaOnFixed.Value = RightHandDetectionAreaRatioRect;
            DeviceSettings.RightHandDetectionScaleOnFixed.RangedValue.Value = FaceDetection.HandDetectionScaleForEgsDevice;
            DeviceSettings.LeftHandDetectionAreaOnFixed.Value = LeftHandDetectionAreaRatioRect;
            DeviceSettings.LeftHandDetectionScaleOnFixed.RangedValue.Value = FaceDetection.HandDetectionScaleForEgsDevice;
        }

        bool isDrawingCursors = false;
        void EgsGestureHidReport_ReportUpdated(object sender, EventArgs e)
        {
            for (int i = 0; i < Device.TrackableHandsCount; i++)
            {
                CursorViewModels[i].UpdateByEgsGestureHidReportHand(Device.EgsGestureHidReport.Hands[i]);
            }
            // If Task is not used, delay in moving cursors can accumulate.
            System.Threading.Tasks.Task.Run(() =>
            {
                if (isDrawingCursors) { return; }
                isDrawingCursors = true;
                for (int i = 0; i < Device.TrackableHandsCount; i++)
                {
                    CursorViews[i].UpdatePosition();
                }
                isDrawingCursors = false;
            });
        }
    }
}

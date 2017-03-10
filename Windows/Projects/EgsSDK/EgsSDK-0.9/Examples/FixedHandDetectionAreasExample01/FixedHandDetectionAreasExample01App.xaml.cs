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
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public partial class FixedHandDetectionAreasExample01App : Application
    {
        public EgsDevice Device { get; private set; }
        public IList<CursorViewModel> CursorViewModels { get; private set; }
        public IList<CursorForm> CursorViews { get; private set; }
        public CameraViewModel CameraViewBackgroundWindowModel { get; private set; }
        public FixedHandDetectionAreasExample01MainWindow CameraViewBackgroundWindow { get; private set; }

        public EgsDeviceFaceDetectionOnHost HandDetectionAreaDecision { get; private set; }
        public FaceDetectionExampleModel FaceDetectionExample { get; private set; }

        public FixedHandDetectionAreasExample01App()
            : base()
        {
            Egs.BindableResources.Current.ChangeCulture("");
            if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
            {
                Application.Current.Shutdown();
                return;
            }

            Device = EgsDevice.GetDefaultEgsDevice();
            Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = true;
            Device.Settings.IsToDetectFaces.Value = true;
            Device.Settings.IsToDetectHands.Value = true;
            Device.Settings.IsToFixHandDetectionRegions.Value = true;
            // 0: 320*240  1:384*240  2:640*480
            Device.Settings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedIndex = 2;

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

            InitializeFaceDetection();
        }

        bool isDrawingCursors = false;
        void EgsGestureHidReport_ReportUpdated(object sender, EventArgs e)
        {
            for (int i = 0; i < Device.TrackableHandsCount; i++)
            {
                CursorViewModels[i].UpdateByEgsGestureHidReportHand(Device.EgsGestureHidReport.Hands[i]);
            }
            // If Task is not used, delay in moving cursors can accumulate.
            Task.Run(() =>
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


        public void InitializeFaceDetection()
        {
            // NOTE: important
            Device.Settings.FaceDetectionMethod.Value = Egs.PropertyTypes.FaceDetectionMethods.SdkUserProcess;

            HandDetectionAreaDecision = new EgsDeviceFaceDetectionOnHost();

            FaceDetectionExample = new FaceDetectionExampleModel();
            FaceDetectionExample.FaceDetectionCompleted += FaceDetection_FaceDetectionCompleted;

            Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;


            this.Exit += delegate
            {
                // Exception happens if you do not detach these event handlers.
                Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged -= CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;
                FaceDetectionExample.FaceDetectionCompleted -= FaceDetection_FaceDetectionCompleted;
                Device.EgsGestureHidReport.ReportUpdated -= EgsGestureHidReport_ReportUpdated;

                HandDetectionAreaDecision.Dispose();

                Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
                Device.Settings.IsToDetectFaces.Value = false;
                Device.Settings.IsToDetectHands.Value = false;
                Device.Settings.IsToFixHandDetectionRegions.Value = false;
                Device.Settings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedIndex = 1;
                // Call this method before CursorView.Close().
                EgsDevice.CloseDefaultEgsDevice();

                foreach (var cursorView in CursorViews) { cursorView.Close(); }
                DuplicatedProcessStartBlocking.ReleaseMutex();
            };
        }

        void CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged(object sender, EventArgs e)
        {
            var isToDetectFacesOnHost = (Device.EgsGestureHidReport.Hands[0].IsTracking == false) && (Device.EgsGestureHidReport.Hands[1].IsTracking == false);
            if (isToDetectFacesOnHost)
            {
                FaceDetectionExample.DetectFaceAsync(Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap);
            }
        }

        void FaceDetection_FaceDetectionCompleted(object sender, EventArgs e)
        {
            FaceDetectionExample.SelectOneFaceRect();
            if (FaceDetectionExample.SelectedFaceRect.HasValue)
            {
                HandDetectionAreaDecision.CaptureImageWidth = Device.Settings.CaptureImageSize.Width;
                HandDetectionAreaDecision.CaptureImageHeight = Device.Settings.CaptureImageSize.Height;
                HandDetectionAreaDecision.CameraViewImageWidth = Device.Settings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Width;
                HandDetectionAreaDecision.CameraViewImageHeight = Device.Settings.CameraViewImageSourceBitmapSize.OptionalValue.SelectedItem.Height;
                HandDetectionAreaDecision.UpdateEgsDeviceSettingsHandDetectionAreas(FaceDetectionExample.SelectedFaceRect);
                HandDetectionAreaDecision.UpdateDeviceSettings(Device.Settings);
                FaceDetectionExample.ShowResultImage(HandDetectionAreaDecision.CameraViewImageRightHandDetectionArea, HandDetectionAreaDecision.CameraViewImageLeftHandDetectionArea);
            }
        }
    }
}

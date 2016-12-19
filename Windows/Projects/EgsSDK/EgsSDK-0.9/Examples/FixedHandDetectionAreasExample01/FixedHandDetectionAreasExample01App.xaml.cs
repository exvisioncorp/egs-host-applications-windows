﻿namespace FixedHandDetectionAreasExample01
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

        FaceDetectionModel FaceDetection { get; set; }

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


            Device = EgsDevice.GetDefaultEgsDevice(DeviceSettings);

            CursorViewModels = new CursorViewModel[Device.TrackableHandsCountMaximum];
            CursorViewModels[0] = new CursorViewModel();
            CursorViewModels[1] = new CursorViewModel();
            CursorViews = new CursorForm[Device.TrackableHandsCountMaximum];
            CursorViews[0] = new CursorForm();
            CursorViews[1] = new CursorForm();
            var imageSetListRight = new List<ImageInformationSet>();
            var imageSetListLeft = new List<ImageInformationSet>();
            imageSetListRight.Add(new ImageInformationSet() { ImageSetIndex = 0, FolderPath = "Resources\\", SampleImageFileRelativePath = "Resources\\Sample.Png" });
            imageSetListRight[0].AddImage((int)CursorImageIndexLabels.OpenHand, "Right_OpenHand.png");
            imageSetListRight[0].AddImage((int)CursorImageIndexLabels.CloseHand, "Right_CloseHand.png");
            imageSetListLeft.Add(new ImageInformationSet() { ImageSetIndex = 0, FolderPath = "Resources\\", SampleImageFileRelativePath = "Resources\\Sample.Png" });
            imageSetListLeft[0].AddImage((int)CursorImageIndexLabels.OpenHand, "Left_OpenHand.png");
            imageSetListLeft[0].AddImage((int)CursorImageIndexLabels.CloseHand, "Left_CloseHand.png");
            CursorViews[0].InitializeOnceAtStartup(CursorViewModels[0], imageSetListRight);
            CursorViews[1].InitializeOnceAtStartup(CursorViewModels[1], imageSetListLeft);
            Device.EgsGestureHidReport.ReportUpdated += (sender, e) =>
            {
                for (int i = 0; i < Device.TrackableHandsCount; i++)
                {
                    CursorViewModels[i].UpdateByEgsGestureHidReportHand(Device.EgsGestureHidReport.Hands[i]);
                    CursorViews[i].UpdatePosition();
                }
            };


            CameraViewBackgroundWindowModel = new CameraViewModel();
            CameraViewBackgroundWindow = new FixedHandDetectionAreasExample01MainWindow();
            base.MainWindow = CameraViewBackgroundWindow;

            CameraViewBackgroundWindowModel.InitializeOnceAtStartup(Device);
            // CameraViewBackgroundWindow.DataContext = CameraViewBackgroundWindowModel;

            CameraViewBackgroundWindow.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        CameraViewBackgroundWindow.Close();
                        break;
                }
            };
            CameraViewBackgroundWindow.Show();


            {
                FaceDetection = new FaceDetectionModel(Device);
                var FaceDetectionStopwatch = Stopwatch.StartNew();
                var FaceDetectionIntervalMilliseconds = 100;
                Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += delegate
                {
                    if (FaceDetectionStopwatch.ElapsedMilliseconds < FaceDetectionIntervalMilliseconds) { return; }
                    FaceDetectionStopwatch.Stop();
                    var isTrackingMoreThanOneHand = CursorViewModels[0].IsTracking || CursorViewModels[1].IsTracking;
                    if (isTrackingMoreThanOneHand == false) { FaceDetection.DetectFaces(); }
                    FaceDetectionStopwatch.Restart();
                };
                FaceDetection.FaceDetectionCompleted += delegate
                {
                    DeviceSettings.RightHandDetectionAreaOnFixed.Value = FaceDetection.RightHandDetectionAreaRatioRect;
                    DeviceSettings.RightHandDetectionScaleOnFixed.RangedValue.Value = FaceDetection.RightHandDetectionScale;
                    DeviceSettings.LeftHandDetectionAreaOnFixed.Value = FaceDetection.LeftHandDetectionAreaRatioRect;
                    DeviceSettings.LeftHandDetectionScaleOnFixed.RangedValue.Value = FaceDetection.LeftHandDetectionScale;
                };
            }


            this.Exit += delegate
            {
                DeviceSettings.IsToDetectFaces.Value = false;
                DeviceSettings.IsToDetectHands.Value = false;
                DeviceSettings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
                DeviceSettings.IsToFixHandDetectionRegions.Value = false;
                EgsDevice.CloseDefaultEgsDevice();

                foreach (var cursorView in CursorViews) { cursorView.Close(); }
                DuplicatedProcessStartBlocking.ReleaseMutex();
            };
        }
    }
}

namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.ComponentModel;
    using System.Diagnostics;
    using DotNetUtility;

    public partial class EgsDeviceFaceDetectionOnHost : IDisposable, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        /// <summary>The focal length near its optical axis [mm]</summary>
        public double CalibratedFocalLength { get; set; }
        public double SensorImageBinnedPixelOneSideLength { get; set; }

        // NOTE: EGS devices return various information on the coordinate of their sensor image.
        //       But captured image size is different from sensor image size.

        public double SensorImageWidth { get; set; }
        public double SensorImageHeight { get; set; }
        public double CameraViewImageWidth { get; set; }
        public double CameraViewImageHeight { get; set; }

        /// <summary>
        /// return CameraViewImageHeight / SensorImageHeight  (Based on EGS device specification currently.)
        /// </summary>
        public double CameraViewImageScale_DividedBy_SensorImageScale { get { Trace.Assert(SensorImageHeight > 0); return CameraViewImageHeight / SensorImageHeight; } }

        public RangedDouble RealFaceBreadth { get; private set; }
        public RangedDouble MaxPlayableDistanceInMeter { get; private set; }
        public double RealFaceZMaximum { get { return MaxPlayableDistanceInMeter * 1000.0; } }
        public RangedDouble RealShoulderBreadth { get; private set; }
        public RangedDouble RealPalmBreadth { get; private set; }
        public RangedDouble RealDetectionAreaWidth { get; private set; }
        public RangedDouble RealDetectionAreaHeight { get; private set; }

        public double RealDetectionAreaCenterXOffset { get; set; }
        public double RealDetectionAreaCenterYOffset { get; set; }
        public double RealDetectionAreaCenterZOffset { get; set; }

        // NOTE: DetectorImageDetectableFaceWidthMinimum is decided by detector's specification.  In some case, dectector can find smaller faces by enlarging input image.

        public int DetectorImageDetectableFaceWidthMinimum { get; set; }
        public double DetectorImageScale_DividedBy_CameraViewImageScale
        {
            get
            {
                // (SensorImageBinnedPixelOneSideLength * SensorImageFaceWidth) : CalibratedFocalLength == RealFaceBreadth : RealFaceZMaximum
                // SensorImageFaceWidth = (CalibratedFocalLength * RealFaceBreadth) / (SensorImageBinnedPixelOneSideLength * RealFaceZMaximum);

                // (DetectorImageBinnedPixelOneSideLength * DetectorImageDetectableFaceWidthMinimum) == (SensorImageBinnedPixelOneSideLength * SensorImageFaceWidth)
                // DetectorImageBinnedPixelOneSideLength == (SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale

                // ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale)
                //                                        * DetectorImageDetectableFaceWidthMinimum  == (SensorImageBinnedPixelOneSideLength * ((CalibratedFocalLength * RealFaceBreadth) / (SensorImageBinnedPixelOneSideLength * RealFaceZMaximum)))
                // ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale) * DetectorImageDetectableFaceWidthMinimum
                //                                                                                   == CalibratedFocalLength * RealFaceBreadth / RealFaceZMaximum
                // ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) * RealFaceZMaximum                                 ) * DetectorImageDetectableFaceWidthMinimum
                //                                                                                   == CalibratedFocalLength * RealFaceBreadth * DetectorImageScale_DividedBy_CameraViewImageScale

                // DetectorImageScale_DividedBy_CameraViewImageScale = ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) * RealFaceZMaximum) * DetectorImageDetectableFaceWidthMinimum / (CalibratedFocalLength * RealFaceBreadth);
                var ret = (SensorImageBinnedPixelOneSideLength * RealFaceZMaximum * DetectorImageDetectableFaceWidthMinimum) / (CameraViewImageScale_DividedBy_SensorImageScale * CalibratedFocalLength * RealFaceBreadth);
                return ret;
            }
        }


        public RangedInt SetCameraViewImageBitmapIntervalMilliseconds { get; private set; }
        /// <summary>
        /// -3 => High Sensitivity.  +3 => High Specificity
        /// </summary>
        public RangedInt SensitivityAndSpecificity { get; private set; }
        public double DlibHogSvmThreshold { get { return (SensitivityAndSpecificity - 4.0) / 10.0; } }

        Stopwatch SetCameraViewImageBitmapIntervalStopwatch { get; set; }
        System.ComponentModel.BackgroundWorker Worker { get; set; }

        bool IsDetecting { get; set; }

        DlibSharp.Array2dUchar DlibArray2dUcharImage { get; set; }
        DlibSharp.FrontalFaceDetector DlibHogSvm { get; set; }
        public IList<System.Drawing.Rectangle> DetectedFaceRectsInCameraViewImage { get; private set; }

        public bool IsFaceDetected { get { return (DetectedFaceRectsInCameraViewImage != null) && (DetectedFaceRectsInCameraViewImage.Count > 0); } }

        public System.Drawing.Rectangle CameraViewImageRightHandDetectionArea { get; private set; }
        public System.Drawing.Rectangle CameraViewImageLeftHandDetectionArea { get; private set; }
        public System.Drawing.Rectangle SensorImageRightHandDetectionArea { get; private set; }
        public System.Drawing.Rectangle SensorImageLeftHandDetectionArea { get; private set; }
        public int HandDetectionScaleForEgsDevice { get; private set; }

        internal EgsDevice Device { get; private set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }

        public EgsDeviceFaceDetectionOnHost()
        {
            // (Kickstarter Version)
            CalibratedFocalLength = 2.92;

            if (ApplicationCommonSettings.IsDebuggingInternal)
            {
                // (Wider lens for development in Exvision)
                CalibratedFocalLength = 2.39;
            }

            // pixelOneSideLength: 0.0028[mm] (2x2 binning).
            SensorImageBinnedPixelOneSideLength = 0.0028;

            SensorImageWidth = 960;
            SensorImageHeight = 540;
            CameraViewImageWidth = 384;
            CameraViewImageHeight = 240;


            // +X:Right  +Y:Bottom  +Z:Back (camera to user)
            // Parameters input by user
            // (140,200) Avg: M:162 F:156
            RealFaceBreadth = new RangedDouble(159, 140, 200, 1, 5, 1);
            // 3[m].  10 feet UI
            MaxPlayableDistanceInMeter = new RangedDouble(3.0, 1.0, 5.0, 0.1, 0.5, 0.1);
            // (310,440) Avg: M:397 F:361
            RealShoulderBreadth = new RangedDouble(379, 310, 440, 1, 10, 1);
            // ( 65, 95) Avg: M: 82 F: 74
            RealPalmBreadth = new RangedDouble(78, 65, 95, 1, 3, 1);
            RealDetectionAreaWidth = new RangedDouble(RealPalmBreadth * 4, 100, 500, 10, 50, 10);
            RealDetectionAreaHeight = new RangedDouble(RealPalmBreadth * 4, 100, 500, 10, 50, 10);

            RealDetectionAreaCenterXOffset = (RealShoulderBreadth / 2) * 1.2;
            RealDetectionAreaCenterYOffset = RealFaceBreadth * 0.7;
            RealDetectionAreaCenterZOffset = -RealShoulderBreadth / 2.1;

            SensitivityAndSpecificity = new RangedInt(0, -3, 3, 1, 1, 1);

            DetectorImageDetectableFaceWidthMinimum = 73;
            Debug.WriteLine("DetectorImageScale_DividedBy_CameraViewImageScale: " + DetectorImageScale_DividedBy_CameraViewImageScale);

            SetCameraViewImageBitmapIntervalMilliseconds = new RangedInt(200, 0, 2000, 10, 100, 10);
            SetCameraViewImageBitmapIntervalStopwatch = Stopwatch.StartNew();

            Worker = new System.ComponentModel.BackgroundWorker();
            Worker.DoWork += delegate { DetectFaces(); };
            Worker.RunWorkerCompleted += delegate { DetectFaces_RunWorkerCompleted(); };

            DlibHogSvm = new DlibSharp.FrontalFaceDetector();
            DlibArray2dUcharImage = new DlibSharp.Array2dUchar();
        }

        internal void InitializeOnceAtStartup(EgsDevice device)
        {
            if (device == null)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                throw new ArgumentNullException("device");
            }
            Device = device;
            Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += Device_CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;
        }

        void Device_CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged(object sender, EventArgs e)
        {
            if (false && Device.Settings.FaceDetectionMethod.Value == PropertyTypes.FaceDetectionMethodKind.DefaultProcessOnEgsHostApplication && ApplicationCommonSettings.IsDebugging)
            {
                // Draw the latest result before return;
                if (SelectedFaceRect.HasValue)
                {
                    using (var g = System.Drawing.Graphics.FromImage(Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap))
                    {
                        var pen = new System.Drawing.Pen(System.Drawing.Brushes.Green, 5);
                        g.DrawRectangle(pen, SelectedFaceRect.Value);
                    }
                }
            }

            if (SetCameraViewImageBitmapIntervalStopwatch.ElapsedMilliseconds < SetCameraViewImageBitmapIntervalMilliseconds) { return; }
            SetCameraViewImageBitmapIntervalStopwatch.Reset();
            SetCameraViewImageBitmapIntervalStopwatch.Start();

            var isToDetectFacesOnHost =
                (Device.Settings.FaceDetectionMethod.Value == PropertyTypes.FaceDetectionMethodKind.DefaultProcessOnEgsHostApplication)
                && (Device.IsDetectingFaces == true)
                && (Device.Settings.IsToDetectHands.Value == true)
                && (Device.EgsGestureHidReport.Hands[0].IsTracking == false)
                && (Device.EgsGestureHidReport.Hands[1].IsTracking == false);
            if (isToDetectFacesOnHost)
            {
                DetectFaceRunWorkerAsync(Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap);
            }
        }

        public void DetectFaceRunWorkerAsync(System.Drawing.Bitmap cameraViewImageBitmap)
        {
            if (IsDetecting) { return; }

            IsDetecting = true;

            // Access to Bitmap must be in the same thread.
            Debug.Assert(cameraViewImageBitmap != null);
            Debug.Assert(cameraViewImageBitmap.Size.IsEmpty == false);
            CameraViewImageWidth = cameraViewImageBitmap.Width;
            CameraViewImageHeight = cameraViewImageBitmap.Height;
            using (var clonedBmp = (System.Drawing.Bitmap)cameraViewImageBitmap.Clone())
            {
                DlibArray2dUcharImage.SetBitmap(clonedBmp);
            }
            Worker.RunWorkerAsync();
        }

        public void DetectFaces()
        {
            try
            {
                var scale = DetectorImageScale_DividedBy_CameraViewImageScale;
                var detectorImageWidth = (int)(CameraViewImageWidth * scale);
                var detectorImageHeight = (int)(CameraViewImageHeight * scale);
                Debug.WriteLine("DetectorImageWidth: " + detectorImageWidth);
                Debug.WriteLine("DetectorImageHeight: " + detectorImageHeight);
                DlibArray2dUcharImage.ResizeImage(detectorImageWidth, detectorImageHeight);
                DetectedFaceRectsInCameraViewImage = DlibHogSvm.DetectFaces(DlibArray2dUcharImage, DlibHogSvmThreshold)
                    .Select(e => new System.Drawing.Rectangle((int)(e.X / scale), (int)(e.Y / scale), (int)(e.Width / scale), (int)(e.Height / scale)))
                    .ToList();
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Console.WriteLine(ex.Message);
            }
        }

        void DetectFaces_RunWorkerCompleted()
        {
            if (IsFaceDetected == false || isDisposing)
            {
                // NOTE: must set IsDetectinfFaces to false, before return.
                IsDetecting = false;
                return;
            }


            // TODO: MUSTDO: implement
            // Implemented in EgsDeviceFaceDetectionOnHost_FaceSelection.cs
            SelectOneFaceRect();
            var list = this.DetectedFaceRectsInCameraViewImage.ToList();
            for (int i = 0; i < Device.EgsGestureHidReport.Faces.Count; i++)
            {
                if (i >= list.Count) { continue; }
                Device.EgsGestureHidReport.Faces[i] = new EgsDeviceEgsGestureHidReportFace() { IsDetected = true, Area = list[i], IsSelected = false, Score = 0 };
            }
            if (Device.EgsGestureHidReport.Faces.Count > 0) { Device.EgsGestureHidReport.Faces[0].IsSelected = true; }


            UpdateEgsDeviceSettingsHandDetectionAreas(SelectedFaceRect.Value, Device.Settings);

            // TODO: MUSTDO: Think the order of the next 2 line.
            OnFaceDetectionCompleted(EventArgs.Empty);

            IsDetecting = false;
        }

        /// <summary>
        /// You can set "Hand Detection Areas" by setting "cameraViewImageFaceRect" detected by your method.
        /// </summary>
        /// <param name="cameraViewImageFaceRect"></param>
        public void UpdateEgsDeviceSettingsHandDetectionAreas(System.Drawing.Rectangle cameraViewImageFaceRect, EgsDeviceSettings deviceSettings)
        {
            Trace.Assert(CameraViewImageScale_DividedBy_SensorImageScale > 0);
            Trace.Assert(cameraViewImageFaceRect.Width > 0);

            SensorImageWidth = deviceSettings.CaptureImageSize.Width;
            SensorImageHeight = deviceSettings.CaptureImageSize.Height;

            double CameraViewImagePixelOneSideLengthOnSensor = SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale;

            // (CameraViewImagePixelOneSideLengthOnSensor * cameraViewImageFaceRect.Width) : CalibratedFocalLength = RealFaceBreadth : RealFaceCenterZ
            double RealFaceBreadthOnSensor = CameraViewImagePixelOneSideLengthOnSensor * cameraViewImageFaceRect.Width;
            double RealFaceCenterZ = CalibratedFocalLength * (RealFaceBreadth / RealFaceBreadthOnSensor);

            // Positive == Right
            double NormalizedCameraViewImageFaceCenterX = cameraViewImageFaceRect.X + cameraViewImageFaceRect.Width / 2.0 - CameraViewImageWidth / 2.0;
            double RealFaceCenterX = (CameraViewImagePixelOneSideLengthOnSensor * NormalizedCameraViewImageFaceCenterX) * (RealFaceCenterZ / CalibratedFocalLength);
            // Positive == Bottom
            double NormalizedCameraViewImageFaceCenterY = cameraViewImageFaceRect.Y + cameraViewImageFaceRect.Height / 2.0 - CameraViewImageHeight / 2.0;
            double RealFaceCenterY = (CameraViewImagePixelOneSideLengthOnSensor * NormalizedCameraViewImageFaceCenterY) * (RealFaceCenterZ / CalibratedFocalLength);

            double RealRightDetectionAreaCenterX = RealFaceCenterX + RealDetectionAreaCenterXOffset;
            double RealRightDetectionAreaCenterY = RealFaceCenterY + RealDetectionAreaCenterYOffset;
            double RealRightDetectionAreaLeft = RealRightDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double RealRightDetectionAreaTop = RealRightDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;
            double RealLeftDetectionAreaCenterX = RealFaceCenterX - RealDetectionAreaCenterXOffset;
            double RealLeftDetectionAreaCenterY = RealFaceCenterY + RealDetectionAreaCenterYOffset;
            double RealLeftDetectionAreaLeft = RealLeftDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double RealLeftDetectionAreaTop = RealLeftDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;

            // (SensorImageBinnedPixelOneSideLength * SensorImageXY) : CalibratedFocalLength = RealXY : RealZ
            // SensorImageXY = (CalibratedFocalLength / (SensorImageBinnedPixelOneSideLength * RealZ)) * RealXY

            // You can define different Z values to right and left.  (for example 2 players play)
            double RealDetectionAreaCenterZ = RealFaceCenterZ + RealDetectionAreaCenterZOffset;

            {
                double ScaleRealToCameraViewImagePixels = CalibratedFocalLength / (CameraViewImagePixelOneSideLengthOnSensor * RealDetectionAreaCenterZ);
                double CameraViewImageRightDetectionAreaX = ScaleRealToCameraViewImagePixels * RealRightDetectionAreaLeft + CameraViewImageWidth / 2.0;
                double CameraViewImageRightDetectionAreaY = ScaleRealToCameraViewImagePixels * RealRightDetectionAreaTop + CameraViewImageHeight / 2.0;
                double CameraViewImageRightDetectionAreaWidth = ScaleRealToCameraViewImagePixels * RealDetectionAreaWidth;
                double CameraViewImageRightDetectionAreaHeight = ScaleRealToCameraViewImagePixels * RealDetectionAreaHeight;
                double CameraViewImageLeftDetectionAreaX = ScaleRealToCameraViewImagePixels * RealLeftDetectionAreaLeft + CameraViewImageWidth / 2.0;
                double CameraViewImageLeftDetectionAreaY = ScaleRealToCameraViewImagePixels * RealLeftDetectionAreaTop + CameraViewImageHeight / 2.0;
                double CameraViewImageLeftDetectionAreaWidth = ScaleRealToCameraViewImagePixels * RealDetectionAreaWidth;
                double CameraViewImageLeftDetectionAreaHeight = ScaleRealToCameraViewImagePixels * RealDetectionAreaHeight;
                CameraViewImageRightHandDetectionArea = new System.Drawing.Rectangle(
                    (int)CameraViewImageRightDetectionAreaX,
                    (int)CameraViewImageRightDetectionAreaY,
                    (int)CameraViewImageRightDetectionAreaWidth,
                    (int)CameraViewImageRightDetectionAreaHeight);
                CameraViewImageLeftHandDetectionArea = new System.Drawing.Rectangle(
                    (int)CameraViewImageLeftDetectionAreaX,
                    (int)CameraViewImageLeftDetectionAreaY,
                    (int)CameraViewImageLeftDetectionAreaWidth,
                    (int)CameraViewImageLeftDetectionAreaHeight);
            }

            double ScaleRealToSensorImagePixels = CalibratedFocalLength / (SensorImageBinnedPixelOneSideLength * RealDetectionAreaCenterZ);
            double SensorImageRightDetectionAreaX = ScaleRealToSensorImagePixels * RealRightDetectionAreaLeft + SensorImageWidth / 2.0;
            double SensorImageRightDetectionAreaY = ScaleRealToSensorImagePixels * RealRightDetectionAreaTop + SensorImageHeight / 2.0;
            double SensorImageRightDetectionAreaWidth = ScaleRealToSensorImagePixels * RealDetectionAreaWidth;
            double SensorImageRightDetectionAreaHeight = ScaleRealToSensorImagePixels * RealDetectionAreaHeight;
            double SensorImageLeftDetectionAreaX = ScaleRealToSensorImagePixels * RealLeftDetectionAreaLeft + SensorImageWidth / 2.0;
            double SensorImageLeftDetectionAreaY = ScaleRealToSensorImagePixels * RealLeftDetectionAreaTop + SensorImageHeight / 2.0;
            double SensorImageLeftDetectionAreaWidth = ScaleRealToSensorImagePixels * RealDetectionAreaWidth;
            double SensorImageLeftDetectionAreaHeight = ScaleRealToSensorImagePixels * RealDetectionAreaHeight;
            SensorImageRightHandDetectionArea = new System.Drawing.Rectangle(
                (int)SensorImageRightDetectionAreaX,
                (int)SensorImageRightDetectionAreaY,
                (int)SensorImageRightDetectionAreaWidth,
                (int)SensorImageRightDetectionAreaHeight);
            SensorImageLeftHandDetectionArea = new System.Drawing.Rectangle(
                (int)SensorImageLeftDetectionAreaX,
                (int)SensorImageLeftDetectionAreaY,
                (int)SensorImageLeftDetectionAreaWidth,
                (int)SensorImageLeftDetectionAreaHeight);

            double SensorImagePalmImageWidth = ScaleRealToSensorImagePixels * RealPalmBreadth;
            // When SensorImagePalmBreadth is about 30, HandDetectionScaleForEgsDevice is 8.
            HandDetectionScaleForEgsDevice = (int)((8.0 / 30.0) * SensorImagePalmImageWidth);
            Debug.WriteLine("HandDetectionScaleForEgsDevice: " + HandDetectionScaleForEgsDevice);

            var RightHandDetectionAreaRatioRect = new Egs.DotNetUtility.RatioRect();
            var LeftHandDetectionAreaRatioRect = new Egs.DotNetUtility.RatioRect();
            RightHandDetectionAreaRatioRect.XRange.From = (float)(SensorImageRightHandDetectionArea.X / SensorImageWidth);
            RightHandDetectionAreaRatioRect.XRange.To = (float)((SensorImageRightHandDetectionArea.X + SensorImageRightHandDetectionArea.Width) / SensorImageWidth);
            RightHandDetectionAreaRatioRect.YRange.From = (float)(SensorImageRightHandDetectionArea.Y / SensorImageHeight);
            RightHandDetectionAreaRatioRect.YRange.To = (float)((SensorImageRightHandDetectionArea.Y + SensorImageRightHandDetectionArea.Height) / SensorImageHeight);
            LeftHandDetectionAreaRatioRect.XRange.From = (float)(SensorImageLeftHandDetectionArea.X / SensorImageWidth);
            LeftHandDetectionAreaRatioRect.XRange.To = (float)((SensorImageLeftHandDetectionArea.X + SensorImageLeftHandDetectionArea.Width) / SensorImageWidth);
            LeftHandDetectionAreaRatioRect.YRange.From = (float)(SensorImageLeftHandDetectionArea.Y / SensorImageHeight);
            LeftHandDetectionAreaRatioRect.YRange.To = (float)((SensorImageLeftHandDetectionArea.Y + SensorImageLeftHandDetectionArea.Height) / SensorImageHeight);

            deviceSettings.RightHandDetectionAreaOnFixed.Value = RightHandDetectionAreaRatioRect;
            deviceSettings.RightHandDetectionScaleOnFixed.RangedValue.Value = HandDetectionScaleForEgsDevice;
            deviceSettings.LeftHandDetectionAreaOnFixed.Value = LeftHandDetectionAreaRatioRect;
            deviceSettings.LeftHandDetectionScaleOnFixed.RangedValue.Value = HandDetectionScaleForEgsDevice;
        }

        #region IDisposable
        private bool disposed = false;
        private bool isDisposing = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }
            isDisposing = true;
            if (disposing)
            {
                // dispose managed objects, and dispose objects that implement IDisposable
                if (Device != null)
                {
                    // When InitializeOnceAtStartup is not called.
                    Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged -= Device_CameraViewImageSourceBitmapCapture_CameraViewImageSourceBitmapChanged;
                }
                SetCameraViewImageBitmapIntervalStopwatch.Reset();
                SetCameraViewImageBitmapIntervalStopwatch.Start();
                while (IsDetecting)
                {
                    System.Threading.Thread.Sleep(50);
                    if (SetCameraViewImageBitmapIntervalStopwatch.ElapsedMilliseconds > 2000)
                    {
                        if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                        Console.WriteLine("FaceDetection BackgroundWorkder did not completed.");
                        break;
                    }
                }
                if (DlibArray2dUcharImage != null) { DlibArray2dUcharImage.Dispose(); DlibArray2dUcharImage = null; }
                if (DlibHogSvm != null) { DlibHogSvm.Dispose(); DlibHogSvm = null; }
            }
            // release any unmanaged objects and set the object references to null
            disposed = true;
        }
        ~EgsDeviceFaceDetectionOnHost() { Dispose(false); }
        #endregion
    }
}

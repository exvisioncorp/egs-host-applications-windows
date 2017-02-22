namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    public partial class EgsDeviceFaceDetectionOnHost : IDisposable
    {
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

        public double RealFaceBreadth { get; set; }
        public double RealFaceZMaximum { get; set; }
        public double RealShoulderBreadth { get; set; }
        public double RealPalmBreadth { get; set; }

        public double RealDetectionAreaCenterXOffset { get; set; }
        public double RealDetectionAreaCenterYOffset { get; set; }
        public double RealDetectionAreaCenterZOffset { get; set; }

        public double RealDetectionAreaWidth { get; set; }
        public double RealDetectionAreaHeight { get; set; }


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


        public int IntervalMilliseconds { get; set; }
        Stopwatch IntervalStopwatch { get; set; }
        bool IsDetectingFaces { get; set; }
        System.ComponentModel.BackgroundWorker Worker { get; set; }

        DlibSharp.Array2dUchar DlibArray2dUcharImage { get; set; }
        DlibSharp.FrontalFaceDetector DlibHogSvm { get; set; }
        public IList<System.Drawing.Rectangle> DetectedFaceRects { get; private set; }

        public bool IsFaceDetected { get { return (DetectedFaceRects != null) && (DetectedFaceRects.Count > 0); } }

        public System.Drawing.Rectangle SensorImageRightHandDetectionArea { get; private set; }
        public System.Drawing.Rectangle SensorImageLeftHandDetectionArea { get; private set; }
        public int HandDetectionScaleForEgsDevice { get; private set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }

        public EgsDeviceFaceDetectionOnHost()
        {
            // (Kickstarter Version)
            CalibratedFocalLength = 2.92;
#if DEBUG
            // (Wider lens for development in Exvision)
            CalibratedFocalLength = 2.39;
#endif

            // pixelOneSideLength: 0.0028[mm] (2x2 binning).
            SensorImageBinnedPixelOneSideLength = 0.0028;

            SensorImageWidth = 768.0;
            SensorImageHeight = 480.0;
            CameraViewImageWidth = 384.0;
            CameraViewImageHeight = 240.0;


            // +X:Right  +Y:Bottom  +Z:Back (camera to user)
            // Parameters input by user
            RealFaceBreadth = 159.0;     // (140,200) Avg: M:162 F:156
            RealFaceZMaximum = 3000.0;   // 3[m].  10 feet UI
            RealShoulderBreadth = 379.0; // (310,440) Avg: M:397 F:361
            RealPalmBreadth = 78.0;      // ( 65, 95) Avg: M: 82 F: 74
            RealDetectionAreaCenterXOffset = (RealShoulderBreadth / 2) * 1.2;
            RealDetectionAreaCenterYOffset = RealFaceBreadth * 0.7;
            RealDetectionAreaCenterZOffset = -RealShoulderBreadth / 2.1;
            RealDetectionAreaWidth = RealPalmBreadth * 4;
            RealDetectionAreaHeight = RealPalmBreadth * 4;


            DetectorImageDetectableFaceWidthMinimum = 73;
            Debug.WriteLine("DetectorImageScale_DividedBy_CameraViewImageScale: " + DetectorImageScale_DividedBy_CameraViewImageScale);

            IntervalMilliseconds = 200;
            IntervalStopwatch = Stopwatch.StartNew();

            DlibHogSvm = new DlibSharp.FrontalFaceDetector();
            DlibArray2dUcharImage = new DlibSharp.Array2dUchar();

            Worker = new System.ComponentModel.BackgroundWorker();
            Worker.DoWork += delegate
            {
                try
                {
                    if (IntervalStopwatch.ElapsedMilliseconds < IntervalMilliseconds) { return; }
                    IntervalStopwatch.Reset();
                    IntervalStopwatch.Start();

                    var scale = DetectorImageScale_DividedBy_CameraViewImageScale;
                    var detectorImageWidth = (int)(CameraViewImageWidth * scale);
                    var detectorImageHeight = (int)(CameraViewImageHeight * scale);
                    Debug.WriteLine("DetectorImageWidth: " + detectorImageWidth);
                    Debug.WriteLine("DetectorImageHeight: " + detectorImageHeight);
                    DlibArray2dUcharImage.ResizeImage(detectorImageWidth, detectorImageHeight);
                    DetectedFaceRects = DlibHogSvm.DetectFaces(DlibArray2dUcharImage, -0.5)
                        .Select(e => new System.Drawing.Rectangle((int)(e.X / scale), (int)(e.Y / scale), (int)(e.Width / scale), (int)(e.Height / scale)))
                        .ToList();

                    OnFaceDetectionCompleted(EventArgs.Empty);
                }
                catch (Exception ex)
                {
#if DEBUG
                    Debugger.Break();
#endif
                    Console.WriteLine(ex.Message);
                }
            };
            Worker.RunWorkerCompleted += (sender, e) => { IsDetectingFaces = false; };
        }

        public void DetectFaceRunWorkerAsync(System.Drawing.Bitmap cameraViewImageBitmap)
        {
            if (IsDetectingFaces) { return; }
            IsDetectingFaces = true;
            // Access to Bitmap must be in the same thread.
            Debug.Assert(cameraViewImageBitmap != null);
            Debug.Assert(cameraViewImageBitmap.Size.IsEmpty == false);
            Debug.Assert(CameraViewImageWidth == cameraViewImageBitmap.Width);
            Debug.Assert(CameraViewImageHeight == cameraViewImageBitmap.Height);
            using (var clonedBmp = (System.Drawing.Bitmap)cameraViewImageBitmap.Clone())
            {
                DlibArray2dUcharImage.SetBitmap(clonedBmp);
            }
            Worker.RunWorkerAsync();
        }

        public void UpdateSensorImageHandDetectionAreas(System.Drawing.Rectangle cameraViewImageFaceRect)
        {
            Trace.Assert(CameraViewImageScale_DividedBy_SensorImageScale > 0);
            Trace.Assert(cameraViewImageFaceRect.Width > 0);

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
        }

        #region IDisposable
        private bool disposed = false;
        public void Dispose() { Dispose(true); GC.SuppressFinalize(this); }
        protected virtual void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                // dispose managed objects, and dispose objects that implement IDisposable
                IntervalStopwatch.Reset();
                IntervalStopwatch.Start();
                while (IsDetectingFaces)
                {
                    System.Threading.Thread.Sleep(50);
                    if (IntervalStopwatch.ElapsedMilliseconds > 2000)
                    {
#if DEBUG
                        Debugger.Break();
#endif
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

namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    public class FaceDetectionModel
    {
        public double SensorImageBinnedPixelOneSideLength { get; set; }
        /// <summary>The focal length near its optical axis</summary>
        public double SensorImageCalibratedFocalLength { get; set; }

        // NOTE: EGS devices return various information on the coordinate of their sensor image.

        public double SensorImageWidth { get; set; }
        public double SensorImageHeight { get; set; }
        public double CameraViewImageWidth { get; set; }
        public double CameraViewImageHeight { get; set; }

        public double CameraViewImageScale_DividedBy_SensorImageScale { get { Trace.Assert(SensorImageHeight > 0); return CameraViewImageHeight / SensorImageHeight; } }
        public double CameraViewImageCalibratedFocalLength { get { return SensorImageCalibratedFocalLength * CameraViewImageScale_DividedBy_SensorImageScale; } }

        public double RealFaceBreadth { get; set; }
        public double RealFaceZMaximum { get; set; }
        public double RealShoulderBreadth { get; set; }
        public double RealPalmBreadth { get; set; }

        // NOTE: DetectorImageDetectableFaceWidthMinimum is decided by detector's specification.  In some case, dectector can find smaller faces by enlarging input image.

        public int DetectorImageDetectableFaceWidthMinimum { get; set; }
        public double DetectorImageScale_DividedBy_CameraViewImageScale
        {
            get
            {
                // (SensorImageBinnedPixelOneSideLength * SensorImageFaceWidth) : SensorImageCalibratedFocalLength == RealFaceBreadth : RealFaceZMaximum
                // SensorImageFaceWidth = (SensorImageCalibratedFocalLength * RealFaceBreadth) / (SensorImageBinnedPixelOneSideLength * RealFaceZMaximum);

                // (DetectorImageBinnedPixelOneSideLength * DetectorImageDetectableFaceWidthMinimum) == (SensorImageBinnedPixelOneSideLength * SensorImageFaceWidth)
                // DetectorImageBinnedPixelOneSideLength == (SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale

                // ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale)
                //                                        * DetectorImageDetectableFaceWidthMinimum  == (SensorImageBinnedPixelOneSideLength * ((SensorImageCalibratedFocalLength * RealFaceBreadth) / (SensorImageBinnedPixelOneSideLength * RealFaceZMaximum)))
                // ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) / DetectorImageScale_DividedBy_CameraViewImageScale) * DetectorImageDetectableFaceWidthMinimum
                //                                                                                   == SensorImageCalibratedFocalLength * RealFaceBreadth / RealFaceZMaximum
                // ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) * RealFaceZMaximum                                 ) * DetectorImageDetectableFaceWidthMinimum
                //                                                                                   == SensorImageCalibratedFocalLength * RealFaceBreadth * DetectorImageScale_DividedBy_CameraViewImageScale

                // DetectorImageScale_DividedBy_CameraViewImageScale = ((SensorImageBinnedPixelOneSideLength / CameraViewImageScale_DividedBy_SensorImageScale) * RealFaceZMaximum) * DetectorImageDetectableFaceWidthMinimum / (SensorImageCalibratedFocalLength * RealFaceBreadth);
                var ret = (SensorImageBinnedPixelOneSideLength * RealFaceZMaximum * DetectorImageDetectableFaceWidthMinimum) / (CameraViewImageScale_DividedBy_SensorImageScale * SensorImageCalibratedFocalLength * RealFaceBreadth);
                return ret;
            }
        }

        public double RealDetectionAreaCenterXOffset { get; set; }
        public double RealDetectionAreaCenterYOffset { get; set; }
        public double RealDetectionAreaCenterZOffset { get; set; }

        public double RealDetectionAreaWidth { get; set; }
        public double RealDetectionAreaHeight { get; set; }

        public bool IsToDetectFaces { get; set; }

        DlibSharp.Array2dUchar DlibArray2dUcharImage { get; set; }
        DlibSharp.FrontalFaceDetector DlibHogSvm { get; set; }
        System.ComponentModel.BackgroundWorker Worker { get; set; }
        System.Drawing.Bitmap InputBitmap { get; set; }

        public bool IsDetectingFaces { get; private set; }
        public IList<System.Drawing.Rectangle> DetectedFaceRects { get; private set; }
        public System.Drawing.Rectangle SelectedFaceRect { get; private set; }
        public bool IsFaceDetected { get { return (DetectedFaceRects != null) && (DetectedFaceRects.Count > 0); } }
        public System.Drawing.Rectangle RightHandDetectionArea { get; private set; }
        public System.Drawing.Rectangle LeftHandDetectionArea { get; private set; }
        public int HandDetectionScaleForEgsDevice { get; private set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }

        public FaceDetectionModel()
        {
            DlibHogSvm = new DlibSharp.FrontalFaceDetector();
            DlibArray2dUcharImage = new DlibSharp.Array2dUchar();
            Worker = new System.ComponentModel.BackgroundWorker();

            // pixelOneSideLength: 0.0028[mm] (2x2 binning).
            SensorImageBinnedPixelOneSideLength = 0.0028;
            // (Wide lens for development in Exvision)
            //CalibratedFocalLengthOnSensorImage = 2.39;
            // (Kickstarter Version)
            SensorImageCalibratedFocalLength = 2.92;
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

            DetectorImageDetectableFaceWidthMinimum = 40;
            Debug.WriteLine("DetectorImageScale_DividedBy_CameraViewImageScale: " + DetectorImageScale_DividedBy_CameraViewImageScale);

            RealDetectionAreaCenterXOffset = (RealShoulderBreadth / 2) * 1.2;
            RealDetectionAreaCenterYOffset = RealFaceBreadth * 0.7;
            RealDetectionAreaCenterZOffset = -RealShoulderBreadth / 2.1;
            RealDetectionAreaWidth = RealPalmBreadth * 4;
            RealDetectionAreaHeight = RealPalmBreadth * 4;

            IsToDetectFaces = true;
            IsDetectingFaces = false;
        }

        public void SetBitmap(System.Drawing.Bitmap bmp)
        {
            InputBitmap = (System.Drawing.Bitmap)bmp.Clone();
        }

        public void Update()
        {
            if (IsDetectingFaces) { return; }

            IsDetectingFaces = true;
            Trace.Assert(InputBitmap != null);
            // Access to Bitmap must be in the same thread.
            Trace.Assert(InputBitmap.Width > 0 && InputBitmap.Height > 0);
            Debug.Assert(CameraViewImageWidth == InputBitmap.Width);
            Debug.Assert(CameraViewImageHeight == InputBitmap.Height);
            Trace.Assert(DlibArray2dUcharImage != null);

            DlibArray2dUcharImage.SetBitmap(InputBitmap);

            var scale = DetectorImageScale_DividedBy_CameraViewImageScale;
            var detectorImageWidth = (int)(CameraViewImageWidth * scale);
            var detectorImageHeight = (int)(CameraViewImageHeight * scale);
            Debug.WriteLine("DetectorImageWidth: " + detectorImageWidth);
            Debug.WriteLine("DetectorImageHeight: " + detectorImageHeight);
            DlibArray2dUcharImage.ResizeImage(detectorImageWidth, detectorImageHeight);

            DetectedFaceRects = DlibHogSvm.DetectFaces(DlibArray2dUcharImage, -0.5)
                .Select(e => new System.Drawing.Rectangle((int)(e.X / scale), (int)(e.Y / scale), (int)(e.Width / scale), (int)(e.Height / scale)))
                .ToList();

            // Heavy tasks must run in the other thread.
            if (IsFaceDetected)
            {
                SelectOneFaceRect();
                UpdateHandDetectionAreas(SelectedFaceRect);
                OnFaceDetectionCompleted(EventArgs.Empty);
            }
            IsDetectingFaces = false;
        }

        void SelectOneFaceRect()
        {
            SelectedFaceRect = DetectedFaceRects[0];
            Func<System.Drawing.Rectangle, double> predictor = e => e.Width + e.Height;
            var faceSizeMax = predictor(SelectedFaceRect);
            for (int i = 1; i < DetectedFaceRects.Count; i++)
            {
                var size = predictor(DetectedFaceRects[i]);
                if (size > faceSizeMax)
                {
                    faceSizeMax = size;
                    SelectedFaceRect = DetectedFaceRects[i];
                }
            }
        }

        void UpdateHandDetectionAreas(System.Drawing.Rectangle imageFaceRect)
        {
            // (PixelOneSideLength * ImageFaceWidth) : CalibratedFocalLength = RealFaceWidth : RealFaceCenterZ
            double RealFaceCenterZ = (CameraViewImageCalibratedFocalLength * RealFaceBreadth) / (SensorImageBinnedPixelOneSideLength * imageFaceRect.Width);
            // Positive == Right
            double ImageFaceCenterX = imageFaceRect.X + imageFaceRect.Width / 2.0 - CameraViewImageWidth / 2.0;
            // Positive == Bottom
            double ImageFaceCenterY = imageFaceRect.Y + imageFaceRect.Height / 2.0 - CameraViewImageHeight / 2.0;
            // (PixelOneSideLength * ImageFaceCenterXY) : CalibratedFocalLength = RealFaceCenterXY : RealFaceCenterZ
            // RealFaceCenterXY = ((PixelOneSideLength * RealFaceCenterZ) / CalibratedFocalLength) * ImageFaceCenterXY
            double RealFaceCenterX = ((SensorImageBinnedPixelOneSideLength * RealFaceCenterZ) / CameraViewImageCalibratedFocalLength) * ImageFaceCenterX;
            double RealFaceCenterY = ((SensorImageBinnedPixelOneSideLength * RealFaceCenterZ) / CameraViewImageCalibratedFocalLength) * ImageFaceCenterY;

            double RealRightDetectionAreaCenterX = RealFaceCenterX + RealDetectionAreaCenterXOffset;
            double RealRightDetectionAreaCenterY = RealFaceCenterY + RealDetectionAreaCenterYOffset;
            double RealRightDetectionAreaLeft = RealRightDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double RealRightDetectionAreaTop = RealRightDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;
            double RealLeftDetectionAreaCenterX = RealFaceCenterX - RealDetectionAreaCenterXOffset;
            double RealLeftDetectionAreaCenterY = RealFaceCenterY + RealDetectionAreaCenterYOffset;
            double RealLeftDetectionAreaLeft = RealLeftDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double RealLeftDetectionAreaTop = RealLeftDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;
            // You can define different Z values to right and left.  (for example 2 players play)
            double RealDetectionAreaCenterZ = RealFaceCenterZ + RealDetectionAreaCenterZOffset;

            double ScaleRealToImage = (CameraViewImageCalibratedFocalLength / (SensorImageBinnedPixelOneSideLength * RealDetectionAreaCenterZ));

            // (PixelOneSideLength * ImageXY) : CalibratedFocalLength = RealXY : RealZ
            // ImageXY = (CalibratedFocalLength / (PixelOneSideLength * RealZ)) * RealXY
            double ImageRightDetectionAreaX = ScaleRealToImage * RealRightDetectionAreaLeft + CameraViewImageWidth / 2.0;
            double ImageRightDetectionAreaY = ScaleRealToImage * RealRightDetectionAreaTop + CameraViewImageHeight / 2.0;
            double ImageRightDetectionAreaWidth = ScaleRealToImage * RealDetectionAreaWidth;
            double ImageRightDetectionAreaHeight = ScaleRealToImage * RealDetectionAreaHeight;
            double ImageLeftDetectionAreaX = ScaleRealToImage * RealLeftDetectionAreaLeft + CameraViewImageWidth / 2.0;
            double ImageLeftDetectionAreaY = ScaleRealToImage * RealLeftDetectionAreaTop + CameraViewImageHeight / 2.0;
            double ImageLeftDetectionAreaWidth = ScaleRealToImage * RealDetectionAreaWidth;
            double ImageLeftDetectionAreaHeight = ScaleRealToImage * RealDetectionAreaHeight;

            RightHandDetectionArea = new System.Drawing.Rectangle((int)ImageRightDetectionAreaX, (int)ImageRightDetectionAreaY, (int)ImageRightDetectionAreaWidth, (int)ImageRightDetectionAreaHeight);
            LeftHandDetectionArea = new System.Drawing.Rectangle((int)ImageLeftDetectionAreaX, (int)ImageLeftDetectionAreaY, (int)ImageLeftDetectionAreaWidth, (int)ImageLeftDetectionAreaHeight);

            double CameraViewImagePalmBreadth = ScaleRealToImage * RealPalmBreadth;
            double SensorImagePalmBreadth = CameraViewImagePalmBreadth / CameraViewImageScale_DividedBy_SensorImageScale;
            // when SensorImagePalmBreadth is about 30, scale factor is 8.
            HandDetectionScaleForEgsDevice = (int)((8.0 / 30.0) * SensorImagePalmBreadth);
        }
    }
}

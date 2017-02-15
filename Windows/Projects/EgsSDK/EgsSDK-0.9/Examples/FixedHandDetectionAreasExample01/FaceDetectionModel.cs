namespace FixedHandDetectionAreasExample01
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public class FaceDetectionModel
    {
        public double BinnedPixelOneSideLength { get; set; }
        public double CalibratedFocalLengthOnSensorImage { get; set; }
        public double SensorImageWidth { get; set; }
        public double SensorImageHeight { get; set; }
        public double CameraViewImageWidth { get; set; }
        public double CameraViewImageHeight { get; set; }
        public double RatioOfCameraViewImageHeightToSensorImageHeight { get; set; }
        public double CalibratedFocalLengthOnCameraViewImage { get; set; }

        public void UpdateCalibratedFocalLengthOnCameraViewImage(double cameraViewImageHeight)
        {
            Trace.Assert(SensorImageHeight > 0);
            Trace.Assert(cameraViewImageHeight > 0);
            CameraViewImageHeight = cameraViewImageHeight;
            RatioOfCameraViewImageHeightToSensorImageHeight = CameraViewImageHeight / SensorImageHeight;
            CalibratedFocalLengthOnCameraViewImage = CalibratedFocalLengthOnSensorImage * RatioOfCameraViewImageHeightToSensorImageHeight;
        }

        public double RealFaceBreadth { get; set; }
        public double RealShoulderBreadth { get; set; }
        public double RealPalmBreadth { get; set; }
        public double RealDetectionAreaCenterXOffset { get; set; }
        public double RealDetectionAreaCenterYOffset { get; set; }
        public double RealDetectionAreaCenterZOffset { get; set; }
        public double RealDetectionAreaWidth { get; set; }
        public double RealDetectionAreaHeight { get; set; }

        public bool IsToDetectFaces { get; set; }
        public Mat CameraViewImageMat { get; private set; }
        public DlibSharp.Array2dUchar DlibArray2dUcharImage { get; private set; }
        public bool IsDetectingFaces { get; private set; }
        DlibSharp.FrontalFaceDetector DlibHogSvm { get; set; }
        public IList<Rect> DetectedFaceRects { get; private set; }
        public Rect SelectedFaceRect { get; private set; }
        public bool IsFaceDetected { get { return (DetectedFaceRects != null) && (DetectedFaceRects.Count > 0); } }
        public Rectf RightHandDetectionArea { get; private set; }
        public Rectf LeftHandDetectionArea { get; private set; }
        public int HandDetectionScaleForEgsDevice { get; private set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }

        Window resultWnd = null;
        Window debug1Wnd = null;
        Window debug2Wnd = null;
        bool _IsToShowResultImage = false;
        public bool IsToShowResultImage
        {
            get { return _IsToShowResultImage; }
            set
            {
                _IsToShowResultImage = value;
                if (_IsToShowResultImage)
                {
                    if (resultWnd != null) { resultWnd.Dispose(); }
                    // Construction of Window in ShowResultImage() does not seem to show any image.
                    resultWnd = new Window("Result");
                }
                else
                {
                    if (resultWnd != null) { resultWnd.Dispose(); resultWnd = null; }
                }
            }
        }

        public FaceDetectionModel()
        {
            DlibHogSvm = new DlibSharp.FrontalFaceDetector();
            DlibArray2dUcharImage = new DlibSharp.Array2dUchar();

            // pixelOneSideLength: 0.0028[mm] (2x2 binning).
            BinnedPixelOneSideLength = 0.0028;
            // (Wide lens for development in Exvision)
            //CalibratedFocalLengthOnSensorImage = 2.39;
            // (Kickstarter Version)
            CalibratedFocalLengthOnSensorImage = 2.92;
            SensorImageWidth = 768.0;
            SensorImageHeight = 480.0;
            CameraViewImageWidth = 384.0;
            CameraViewImageHeight = 240.0;

            // You can set RatioOfCameraViewImageHeightToSensorImageHeight and CalibratedFocalLengthOnCameraViewImage without this method.
            UpdateCalibratedFocalLengthOnCameraViewImage(CameraViewImageHeight);

            // +X:Right  +Y:Bottom  +Z:Back (camera to user)
            // Parameters input by user
            RealFaceBreadth = 159.0;     // (140,200) Avg: M:162 F:156
            RealShoulderBreadth = 379.0; // (310,440) Avg: M:397 F:361
            RealPalmBreadth = 78.0;      // ( 65, 95) Avg: M: 82 F: 74
            RealDetectionAreaCenterXOffset = (RealShoulderBreadth / 2) * 1.2;
            RealDetectionAreaCenterYOffset = RealFaceBreadth * 0.7;
            RealDetectionAreaCenterZOffset = -RealShoulderBreadth / 2.1;
            RealDetectionAreaWidth = RealPalmBreadth * 4;
            RealDetectionAreaHeight = RealPalmBreadth * 4;

            IsToDetectFaces = true;
            IsToShowResultImage = false;
            CameraViewImageMat = new Mat();
        }

        public async void UpdateAsync(System.Drawing.Bitmap bmp)
        {
            if (IsDetectingFaces) { return; }
            IsDetectingFaces = true;
            Trace.Assert(bmp != null);
            // Access to Bitmap must be in the same thread.
            Trace.Assert(bmp.Width > 0 && bmp.Height > 0);
            Debug.Assert(CameraViewImageWidth == bmp.Width);
            Debug.Assert(CameraViewImageHeight == bmp.Height);
            Trace.Assert(DlibArray2dUcharImage != null);

            if (CameraViewImageMat != null)
            {
                CameraViewImageMat.Dispose();
                CameraViewImageMat = bmp.ToMat();
            }

            DlibArray2dUcharImage.SetBitmap(bmp);
            
            // Heavy tasks must run in the other thread.
            await System.Threading.Tasks.Task.Run(() =>
            {
                if (IsToDetectFaces)
                {
                    DetectedFaceRects = DlibHogSvm.DetectFaces(DlibArray2dUcharImage, -0.5)
                        .Select(e => new OpenCvSharp.Rect(e.X, e.Y, e.Width, e.Height)).ToList();
                    if (IsFaceDetected)
                    {
                        SelectOneFaceRect();
                        UpdateHandDetectionAreas(SelectedFaceRect);
                    }
                }
                if (IsToShowResultImage) { ShowResultImage(); }
                OnFaceDetectionCompleted(EventArgs.Empty);
                IsDetectingFaces = false;
            });
        }

        void SelectOneFaceRect()
        {
            SelectedFaceRect = DetectedFaceRects[0];
            Func<OpenCvSharp.Rect, double> predictor = e => e.Width + e.Height;
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

        void UpdateHandDetectionAreas(Rect imageFaceRect)
        {
            // (PixelOneSideLength * ImageFaceWidth) : CalibratedFocalLength = RealFaceWidth : RealFaceCenterZ
            double RealFaceCenterZ = (CalibratedFocalLengthOnCameraViewImage * RealFaceBreadth) / (BinnedPixelOneSideLength * imageFaceRect.Width);
            // Positive == Right
            double ImageFaceCenterX = imageFaceRect.X + imageFaceRect.Width / 2.0 - CameraViewImageWidth / 2.0;
            // Positive == Bottom
            double ImageFaceCenterY = imageFaceRect.Y + imageFaceRect.Height / 2.0 - CameraViewImageHeight / 2.0;
            // (PixelOneSideLength * ImageFaceCenterXY) : CalibratedFocalLength = RealFaceCenterXY : RealFaceCenterZ
            // RealFaceCenterXY = ((PixelOneSideLength * RealFaceCenterZ) / CalibratedFocalLength) * ImageFaceCenterXY
            double RealFaceCenterX = ((BinnedPixelOneSideLength * RealFaceCenterZ) / CalibratedFocalLengthOnCameraViewImage) * ImageFaceCenterX;
            double RealFaceCenterY = ((BinnedPixelOneSideLength * RealFaceCenterZ) / CalibratedFocalLengthOnCameraViewImage) * ImageFaceCenterY;

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

            double ScaleRealToImage = (CalibratedFocalLengthOnCameraViewImage / (BinnedPixelOneSideLength * RealDetectionAreaCenterZ));

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

            RightHandDetectionArea = new Rectf((float)ImageRightDetectionAreaX, (float)ImageRightDetectionAreaY, (float)ImageRightDetectionAreaWidth, (float)ImageRightDetectionAreaHeight);
            LeftHandDetectionArea = new Rectf((float)ImageLeftDetectionAreaX, (float)ImageLeftDetectionAreaY, (float)ImageLeftDetectionAreaWidth, (float)ImageLeftDetectionAreaHeight);

            double CameraViewImagePalmBreadth = ScaleRealToImage * RealPalmBreadth;
            double SensorImagePalmBreadth = CameraViewImagePalmBreadth / RatioOfCameraViewImageHeightToSensorImageHeight;
            // when SensorImagePalmBreadth is about 30, scale factor is 8.
            HandDetectionScaleForEgsDevice = (int)((8.0 / 30.0) * SensorImagePalmBreadth);
        }

        public void ShowResultImage()
        {
            Trace.Assert(resultWnd != null);
            if (DetectedFaceRects == null) { return; }
            if (CameraViewImageMat == null) { return; }
            foreach (var face in DetectedFaceRects)
            {
                var center = new Point
                {
                    X = (int)(face.X + face.Width * 0.5),
                    Y = (int)(face.Y + face.Height * 0.5)
                };
                var axes = new Size
                {
                    Width = (int)(face.Width * 0.5),
                    Height = (int)(face.Height * 0.5)
                };
                Cv2.Ellipse(CameraViewImageMat, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
            }
            if (IsFaceDetected)
            {
                Cv2.Rectangle(CameraViewImageMat, GetRectFromRectf(RightHandDetectionArea), new Scalar(255, 0, 0), 4);
                Cv2.Rectangle(CameraViewImageMat, GetRectFromRectf(LeftHandDetectionArea), new Scalar(0, 0, 255), 4);
            }
            resultWnd.ShowImage(CameraViewImageMat);
        }

        static Rect GetRectFromRectf(Rectf src)
        {
            return new Rect((int)src.X, (int)src.Y, (int)src.Width, (int)src.Height);
        }
    }
}

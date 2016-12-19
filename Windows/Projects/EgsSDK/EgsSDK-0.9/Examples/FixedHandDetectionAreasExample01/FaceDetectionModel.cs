namespace FixedHandDetectionAreasExample01
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public class FaceDetectionModel
    {
        public Egs.EgsDevice Device { get; private set; }

        public double CalibratedFocalLength { get; set; }
        public double RealFaceWidth { get; set; }
        public double RealShoulderWidth { get; set; }
        public double RealShoulderHeightOffset { get; set; }
        public double RealDetectionAreaCenterZOffset { get; set; }
        public double RealDetectionAreaWidth { get; set; }
        public double RealDetectionAreaHeight { get; set; }

        public bool IsDetectingFaces { get; private set; }
        public Mat CameraViewImageMat { get; private set; }
        CascadeClassifier haarCascade { get; set; }
        public IList<Rect> DetectedFaceRects { get; private set; }
        public Rect SelectedFaceRect { get; private set; }
        public bool IsFaceDetected { get; private set; }
        public Rect RightHandDetectionArea { get; private set; }
        public Rect LeftHandDetectionArea { get; private set; }
        public Egs.DotNetUtility.RatioRect RightHandDetectionAreaRatioRect { get; private set; }
        public int RightHandDetectionScale { get; private set; }
        public Egs.DotNetUtility.RatioRect LeftHandDetectionAreaRatioRect { get; private set; }
        public int LeftHandDetectionScale { get; private set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }

        bool isToShowDebugImages;
        Window srcWnd;
        Window debug1Wnd;
        Window debug2Wnd;

        public FaceDetectionModel(Egs.EgsDevice device)
        {
            Trace.Assert(device != null);
            Device = device;
            haarCascade = new CascadeClassifier("DataFromOpenCv/haarcascade_frontalface_alt.xml");

            // Decided by equivalent focal length of the device lens
            // (Wide lens for development in Exvision)
            CalibratedFocalLength = 1.0;
            // (Kickstarter Version)
            //CalibratedFocalLength = 1.5;

            // Paramters input by user
            // Face Width: Avg:160[mm] Range:(140[mm],200[mm])
            RealFaceWidth = 160;
            RealShoulderWidth = 500;
            // +X:Right  +Y:Bottom  +Z:Back
            RealShoulderHeightOffset = 110;
            RealDetectionAreaCenterZOffset = -200;
            RealDetectionAreaWidth = 200;
            RealDetectionAreaHeight = 200;

            RightHandDetectionAreaRatioRect = new Egs.DotNetUtility.RatioRect();
            LeftHandDetectionAreaRatioRect = new Egs.DotNetUtility.RatioRect();

            isToShowDebugImages = false;
#if DEBUG
            isToShowDebugImages = true;
#endif
            if (isToShowDebugImages)
            {
                srcWnd = new Window("srcWnd");
                if (false) { debug1Wnd = new Window("debug1Wnd"); }
                if (false) { debug2Wnd = new Window("debug2Wnd"); }
            }
        }

        public void DetectFaces()
        {
            if (IsDetectingFaces) { return; }
            IsDetectingFaces = true;
            using (var bmp = (System.Drawing.Bitmap)Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap.Clone())
            using (CameraViewImageMat = bmp.ToMat())
            {
                using (var srcGray = CameraViewImageMat.CvtColor(ColorConversionCodes.BGR2GRAY))
                {
                    if (false) { using (var srcGrayCanny = srcGray.Canny(50, 200)) { debug1Wnd.ShowImage(srcGrayCanny); } }
                    if (false) { using (var srcGrayGaussianBlur = srcGray.GaussianBlur(new Size(), 10, 10)) { debug2Wnd.ShowImage(srcGrayGaussianBlur); } }

                    DetectedFaceRects = haarCascade.DetectMultiScale(srcGray, 1.08, 2, HaarDetectionType.ScaleImage, new Size(25, 25));
                }

                IsFaceDetected = DetectedFaceRects.Count > 0;
                if (IsFaceDetected)
                {
                    SelectOneFaceRect();
                    UpdateHandDetectionAreas();
                    if (isToShowDebugImages) { ShowDebugImages(); }
                }

                OnFaceDetectionCompleted(EventArgs.Empty);
            }
            IsDetectingFaces = false;
        }

        void SelectOneFaceRect()
        {
            SelectedFaceRect = DetectedFaceRects[0];
            Func<OpenCvSharp.Rect, double> predictor = e => e.Width + e.Height;
            var faceSizeMax = predictor(SelectedFaceRect);
            for (int i = 1; i < DetectedFaceRects.Count; i++)
            {
                var size = predictor(DetectedFaceRects[i]);
                if (size > faceSizeMax) { faceSizeMax = size; SelectedFaceRect = DetectedFaceRects[i]; }
            }
        }

        void UpdateHandDetectionAreas()
        {
            var imgSize = Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapSize;

            // pixelOneSideLength: 0.0028[mm] (2x2 binning).
            double pixelOneSideLength = 0.0028;
            // (pixelOneSideLength * imageFaceWidth) : calibratedFocalLength = realFaceWidth : realFaceZ
            double realFaceZ = (CalibratedFocalLength * RealFaceWidth) / (pixelOneSideLength * SelectedFaceRect.Width);
            // Positive == Right
            double imageFaceCenterX = SelectedFaceRect.X + SelectedFaceRect.Width / 2.0 - imgSize.Width / 2.0;
            // Positive == Bottom
            double imageFaceCenterY = SelectedFaceRect.Y + SelectedFaceRect.Height / 2.0 - imgSize.Height / 2.0;
            // (pixelOneSideLength * imageFaceXY) : calibratedFocalLength = realFaceXY : realFaceZ
            // realFaceXY = ((pixelOneSideLength * realFaceZ) / calibratedFocalLength) * imageFaceXY
            double realFaceX = ((pixelOneSideLength * realFaceZ) / CalibratedFocalLength) * imageFaceCenterX;
            double realFaceY = ((pixelOneSideLength * realFaceZ) / CalibratedFocalLength) * imageFaceCenterY;

            double realRightDetectionAreaCenterX = realFaceX + RealShoulderWidth / 2.0;
            double realRightDetectionAreaCenterY = realFaceY + RealShoulderHeightOffset;
            double realRightDetectionAreaX = realRightDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double realRightDetectionAreaY = realRightDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;
            double realRightDetectionAreaCenterZ = realFaceZ + RealDetectionAreaCenterZOffset;
            double realLeftDetectionAreaCenterX = realFaceX - RealShoulderWidth / 2.0;
            double realLeftDetectionAreaCenterY = realFaceY + RealShoulderHeightOffset;
            double realLeftDetectionAreaX = realLeftDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double realLeftDetectionAreaY = realLeftDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;
            double realLeftDetectionAreaCenterZ = realFaceZ + RealDetectionAreaCenterZOffset;

            // (pixelOneSideLength * imageXY) : calibratedFocalLength = realXY : realZ
            // imageXY = (calibratedFocalLength / (pixelOneSideLength * realZ)) * realXY
            double imageRightDetectionAreaX = (CalibratedFocalLength / (pixelOneSideLength * realRightDetectionAreaCenterZ)) * realRightDetectionAreaX + imgSize.Width / 2.0;
            double imageRightDetectionAreaY = (CalibratedFocalLength / (pixelOneSideLength * realRightDetectionAreaCenterZ)) * realRightDetectionAreaY + imgSize.Height / 2.0;
            double imageRightDetectionAreaWidth = (CalibratedFocalLength / (pixelOneSideLength * realRightDetectionAreaCenterZ)) * RealDetectionAreaWidth;
            double imageRightDetectionAreaHeight = (CalibratedFocalLength / (pixelOneSideLength * realRightDetectionAreaCenterZ)) * RealDetectionAreaHeight;
            double imageLeftDetectionAreaX = (CalibratedFocalLength / (pixelOneSideLength * realLeftDetectionAreaCenterZ)) * realLeftDetectionAreaX + imgSize.Width / 2.0;
            double imageLeftDetectionAreaY = (CalibratedFocalLength / (pixelOneSideLength * realLeftDetectionAreaCenterZ)) * realLeftDetectionAreaY + imgSize.Height / 2.0;
            double imageLeftDetectionAreaWidth = (CalibratedFocalLength / (pixelOneSideLength * realLeftDetectionAreaCenterZ)) * RealDetectionAreaWidth;
            double imageLeftDetectionAreaHeight = (CalibratedFocalLength / (pixelOneSideLength * realLeftDetectionAreaCenterZ)) * RealDetectionAreaHeight;

            RightHandDetectionArea = new Rect((int)imageRightDetectionAreaX, (int)imageRightDetectionAreaY, (int)imageRightDetectionAreaWidth, (int)imageRightDetectionAreaHeight);
            LeftHandDetectionArea = new Rect((int)imageLeftDetectionAreaX, (int)imageLeftDetectionAreaY, (int)imageLeftDetectionAreaWidth, (int)imageLeftDetectionAreaHeight);
            RightHandDetectionAreaRatioRect.XRange.From = (float)imageRightDetectionAreaX / imgSize.Width;
            RightHandDetectionAreaRatioRect.XRange.To = (float)(imageRightDetectionAreaX + imageRightDetectionAreaWidth) / imgSize.Width;
            RightHandDetectionAreaRatioRect.YRange.From = (float)imageRightDetectionAreaY / imgSize.Height;
            RightHandDetectionAreaRatioRect.YRange.To = (float)(imageRightDetectionAreaY + imageRightDetectionAreaHeight) / imgSize.Height;
            LeftHandDetectionAreaRatioRect.XRange.From = (float)imageLeftDetectionAreaX / imgSize.Width;
            LeftHandDetectionAreaRatioRect.XRange.To = (float)(imageLeftDetectionAreaX + imageLeftDetectionAreaWidth) / imgSize.Width;
            LeftHandDetectionAreaRatioRect.YRange.From = (float)imageLeftDetectionAreaY / imgSize.Height;
            LeftHandDetectionAreaRatioRect.YRange.To = (float)(imageLeftDetectionAreaY + imageLeftDetectionAreaHeight) / imgSize.Height;

            // TODO: fix
            RightHandDetectionScale = (int)(imageRightDetectionAreaWidth / 8.0) + 4;
            LeftHandDetectionScale = (int)(imageLeftDetectionAreaWidth / 8.0) + 4;
            Debug.WriteLine("HandDetectionScale: {0}, {1}", RightHandDetectionScale, LeftHandDetectionScale);
        }

        void ShowDebugImages()
        {
            foreach (Rect face in DetectedFaceRects)
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
            Cv2.Rectangle(CameraViewImageMat, RightHandDetectionArea, new Scalar(255, 0, 0), 4);
            Cv2.Rectangle(CameraViewImageMat, LeftHandDetectionArea, new Scalar(0, 0, 255), 4);
            srcWnd.ShowImage(CameraViewImageMat);
        }
    }
}

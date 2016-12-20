namespace FixedHandDetectionAreasExample01
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public class FaceDetectionModel
    {
        public double CalibratedFocalLength { get; set; }
        public double PixelOneSideLength { get; set; }

        public double RealFaceWidth { get; set; }
        public double RealShoulderWidth { get; set; }
        public double RealShoulderHeightOffset { get; set; }
        public double RealDetectionAreaCenterZOffset { get; set; }
        public double RealDetectionAreaWidth { get; set; }
        public double RealDetectionAreaHeight { get; set; }

        public bool IsDetectingFaces { get; private set; }
        public Mat CameraViewImageMat { get; private set; }

        public float ImageWidth { get; private set; }
        public float ImageHeight { get; private set; }
        CascadeClassifier haarCascade { get; set; }
        public IList<Rect> DetectedFaceRects { get; private set; }
        public Rect SelectedFaceRect { get; private set; }
        public bool IsFaceDetected { get; private set; }
        public Rectf RightHandDetectionArea { get; private set; }
        public Rectf LeftHandDetectionArea { get; private set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }

        bool isToShowResultImage;
        Window srcWnd;
        Window debug1Wnd;
        Window debug2Wnd;

        public FaceDetectionModel()
        {
            haarCascade = new CascadeClassifier("DataFromOpenCv/haarcascade_frontalface_alt.xml");

            // (Wide lens for development in Exvision)
            //CalibratedFocalLength = 1.0;
            // (Kickstarter Version)
            CalibratedFocalLength = 1.5;
            // pixelOneSideLength: 0.0028[mm] (2x2 binning).
            PixelOneSideLength = 0.0028;

            // Parameters input by user
            // Face Width: Avg:160[mm] (Range:140-200[mm])
            RealFaceWidth = 160;
            RealShoulderWidth = 500;
            // +X:Right  +Y:Bottom  +Z:Back (camera to user)
            RealShoulderHeightOffset = 110;
            RealDetectionAreaCenterZOffset = -200;
            RealDetectionAreaWidth = 200;
            RealDetectionAreaHeight = 200;

            isToShowResultImage = false;
#if DEBUG
            isToShowResultImage = true;
#endif
            if (isToShowResultImage)
            {
                srcWnd = new Window("srcWnd");
                if (false) { debug1Wnd = new Window("debug1Wnd"); }
                if (false) { debug2Wnd = new Window("debug2Wnd"); }
            }
        }

        public void DetectFaces(System.Drawing.Bitmap bmp)
        {
            if (IsDetectingFaces) { return; }
            IsDetectingFaces = true;
            Trace.Assert(bmp != null);
            Trace.Assert(bmp.Width > 0 && bmp.Height > 0);
            ImageWidth = bmp.Width;
            ImageHeight = bmp.Height;
            using (CameraViewImageMat = bmp.ToMat())
            {
                using (var srcGray = CameraViewImageMat.CvtColor(ColorConversionCodes.BGR2GRAY))
                {
                    DetectedFaceRects = haarCascade.DetectMultiScale(srcGray, 1.08, 2, HaarDetectionType.ScaleImage, new Size(25, 25));
                }

                IsFaceDetected = DetectedFaceRects.Count > 0;
                if (IsFaceDetected)
                {
                    SelectOneFaceRect();
                    UpdateHandDetectionAreas(SelectedFaceRect);
                    if (isToShowResultImage) { ShowResultImage(); }
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
            double RealFaceCenterZ = (CalibratedFocalLength * RealFaceWidth) / (PixelOneSideLength * imageFaceRect.Width);
            // Positive == Right
            double ImageFaceCenterX = imageFaceRect.X + imageFaceRect.Width / 2.0 - ImageWidth / 2.0;
            // Positive == Bottom
            double ImageFaceCenterY = imageFaceRect.Y + imageFaceRect.Height / 2.0 - ImageHeight / 2.0;
            // (PixelOneSideLength * ImageFaceCenterXY) : CalibratedFocalLength = RealFaceCenterXY : RealFaceCenterZ
            // RealFaceCenterXY = ((PixelOneSideLength * RealFaceCenterZ) / CalibratedFocalLength) * ImageFaceCenterXY
            double RealFaceCenterX = ((PixelOneSideLength * RealFaceCenterZ) / CalibratedFocalLength) * ImageFaceCenterX;
            double RealFaceCenterY = ((PixelOneSideLength * RealFaceCenterZ) / CalibratedFocalLength) * ImageFaceCenterY;

            double RealRightDetectionAreaCenterX = RealFaceCenterX + RealShoulderWidth / 2.0;
            double RealRightDetectionAreaCenterY = RealFaceCenterY + RealShoulderHeightOffset;
            double RealRightDetectionAreaLeft = RealRightDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double RealRightDetectionAreaTop = RealRightDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;
            double RealRightDetectionAreaCenterZ = RealFaceCenterZ + RealDetectionAreaCenterZOffset;
            double RealLeftDetectionAreaCenterX = RealFaceCenterX - RealShoulderWidth / 2.0;
            double RealLeftDetectionAreaCenterY = RealFaceCenterY + RealShoulderHeightOffset;
            double RealLeftDetectionAreaLeft = RealLeftDetectionAreaCenterX - RealDetectionAreaWidth / 2.0;
            double RealLeftDetectionAreaTop = RealLeftDetectionAreaCenterY - RealDetectionAreaHeight / 2.0;
            double RealLeftDetectionAreaCenterZ = RealFaceCenterZ + RealDetectionAreaCenterZOffset;

            // (PixelOneSideLength * ImageXY) : CalibratedFocalLength = RealXY : RealZ
            // ImageXY = (CalibratedFocalLength / (PixelOneSideLength * RealZ)) * RealXY
            double ImageRightDetectionAreaX = (CalibratedFocalLength / (PixelOneSideLength * RealRightDetectionAreaCenterZ)) * RealRightDetectionAreaLeft + ImageWidth / 2.0;
            double ImageRightDetectionAreaY = (CalibratedFocalLength / (PixelOneSideLength * RealRightDetectionAreaCenterZ)) * RealRightDetectionAreaTop + ImageHeight / 2.0;
            double ImageRightDetectionAreaWidth = (CalibratedFocalLength / (PixelOneSideLength * RealRightDetectionAreaCenterZ)) * RealDetectionAreaWidth;
            double ImageRightDetectionAreaHeight = (CalibratedFocalLength / (PixelOneSideLength * RealRightDetectionAreaCenterZ)) * RealDetectionAreaHeight;
            double ImageLeftDetectionAreaX = (CalibratedFocalLength / (PixelOneSideLength * RealLeftDetectionAreaCenterZ)) * RealLeftDetectionAreaLeft + ImageWidth / 2.0;
            double ImageLeftDetectionAreaY = (CalibratedFocalLength / (PixelOneSideLength * RealLeftDetectionAreaCenterZ)) * RealLeftDetectionAreaTop + ImageHeight / 2.0;
            double ImageLeftDetectionAreaWidth = (CalibratedFocalLength / (PixelOneSideLength * RealLeftDetectionAreaCenterZ)) * RealDetectionAreaWidth;
            double ImageLeftDetectionAreaHeight = (CalibratedFocalLength / (PixelOneSideLength * RealLeftDetectionAreaCenterZ)) * RealDetectionAreaHeight;

            RightHandDetectionArea = new Rectf((float)ImageRightDetectionAreaX, (float)ImageRightDetectionAreaY, (float)ImageRightDetectionAreaWidth, (float)ImageRightDetectionAreaHeight);
            LeftHandDetectionArea = new Rectf((float)ImageLeftDetectionAreaX, (float)ImageLeftDetectionAreaY, (float)ImageLeftDetectionAreaWidth, (float)ImageLeftDetectionAreaHeight);
        }

        void ShowResultImage()
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
            Cv2.Rectangle(CameraViewImageMat, GetRectFromRectf(RightHandDetectionArea), new Scalar(255, 0, 0), 4);
            Cv2.Rectangle(CameraViewImageMat, GetRectFromRectf(LeftHandDetectionArea), new Scalar(0, 0, 255), 4);
            srcWnd.ShowImage(CameraViewImageMat);
        }

        static Rect GetRectFromRectf(Rectf src)
        {
            return new Rect((int)src.X, (int)src.Y, (int)src.Width, (int)src.Height);
        }
    }
}

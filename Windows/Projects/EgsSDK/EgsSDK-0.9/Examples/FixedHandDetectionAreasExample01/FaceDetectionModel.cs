namespace FixedHandDetectionAreasExample01
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Diagnostics;
    using OpenCvSharp;
    using OpenCvSharp.Extensions;

    public class FaceDetectionExampleModel
    {
        Mat cameraViewImageMat { get; set; }
        CascadeClassifier haarCascade { get; set; }
        bool IsDetectingFaces { get; set; }

        public IList<System.Drawing.Rectangle> DetectedFaceRects { get; private set; }
        public Nullable<System.Drawing.Rectangle> SelectedFaceRect { get; private set; }
        Window resultWnd { get; set; }

        public event EventHandler FaceDetectionCompleted;
        protected virtual void OnFaceDetectionCompleted(EventArgs e)
        {
            var t = FaceDetectionCompleted; if (t != null) { t(this, e); }
        }

        public FaceDetectionExampleModel()
        {
            haarCascade = new CascadeClassifier("DataFromOpenCv/haarcascade_frontalface_alt.xml");
            resultWnd = new Window("Result");
        }

        public async void DetectFaceAsync(System.Drawing.Bitmap bmp)
        {
            if (IsDetectingFaces) { return; }
            IsDetectingFaces = true;
            Trace.Assert(bmp != null);
            // Access to Bitmap must be in the same thread.
            Trace.Assert(bmp.Width > 0 && bmp.Height > 0);
            if (cameraViewImageMat != null) { cameraViewImageMat.Dispose(); }
            cameraViewImageMat = bmp.ToMat();
            // Heavy tasks must run in the other thread.
            await System.Threading.Tasks.Task.Run(() =>
            {
                using (var srcGray = cameraViewImageMat.CvtColor(ColorConversionCodes.BGR2GRAY))
                {
                    DetectedFaceRects = haarCascade.DetectMultiScale(srcGray, 1.08, 2, HaarDetectionType.ScaleImage, new Size(25, 25))
                        .Select(e => new System.Drawing.Rectangle(e.X, e.Y, e.Width, e.Height)).ToList();
                }
                OnFaceDetectionCompleted(EventArgs.Empty);
                IsDetectingFaces = false;
            });
        }

        public void SelectOneFaceRect()
        {
            if (DetectedFaceRects == null || DetectedFaceRects.Count == 0)
            {
                SelectedFaceRect = null;
                return;
            }
            SelectedFaceRect = DetectedFaceRects[0];
            Func<System.Drawing.Rectangle, double> predictor = e => e.Width + e.Height;
            var faceSizeMax = predictor(SelectedFaceRect.Value);
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

        public void ShowResultImage(System.Drawing.Rectangle rightRect, System.Drawing.Rectangle leftRect)
        {
            Trace.Assert(resultWnd != null);
            if (DetectedFaceRects == null) { return; }
            if (cameraViewImageMat == null) { return; }
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
                Cv2.Ellipse(cameraViewImageMat, center, axes, 0, 0, 360, new Scalar(255, 0, 255), 4);
            }
            if (SelectedFaceRect.HasValue)
            {
                Cv2.Rectangle(cameraViewImageMat, GetRectFromRectangle(rightRect), new Scalar(255, 0, 0), 4);
                Cv2.Rectangle(cameraViewImageMat, GetRectFromRectangle(leftRect), new Scalar(0, 0, 255), 4);
            }
            resultWnd.ShowImage(cameraViewImageMat);
        }

        static Rect GetRectFromRectangle(System.Drawing.Rectangle src)
        {
            return new Rect((int)src.X, (int)src.Y, (int)src.Width, (int)src.Height);
        }
    }
}

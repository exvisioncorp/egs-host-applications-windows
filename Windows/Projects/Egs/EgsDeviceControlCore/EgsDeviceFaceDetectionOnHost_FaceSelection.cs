namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using DotNetUtility;

    public partial class EgsDeviceFaceDetectionOnHost
    {
        // TODO: MUSTDO: implement

        public Nullable<System.Drawing.Rectangle> SelectedFaceRect { get; private set; }

        public void UpdateSelectedFaceRect()
        {
            if (IsFaceDetected)
            {
                SelectMostCenter();
            }
            else
            {
                SelectedFaceRect = null;
            }
        }

        void SelectMostCenter()
        {
            Func<System.Drawing.Rectangle, double> predictor = e =>
            {
                var faceX = e.Left + e.Width / 2.0;
                var faceY = e.Top + e.Height / 2.0;
                var imageX = CameraViewImageWidth / 2.0;
                var imageY = CameraViewImageHeight / 2.0;
                var dx = faceX - imageX;
                var dy = faceY - imageY;
                var ret = Math.Sqrt(dx * dx + dy * dy);
                return ret;
            };
            SelectedFaceRect = DetectedFaceRectsInCameraViewImage.OrderBy(predictor).First();
        }

        void SelectBiggest()
        {
            Func<System.Drawing.Rectangle, double> predictor = e => e.Width + e.Height;
            SelectedFaceRect = DetectedFaceRectsInCameraViewImage.OrderByDescending(predictor).First();
        }
    }
}

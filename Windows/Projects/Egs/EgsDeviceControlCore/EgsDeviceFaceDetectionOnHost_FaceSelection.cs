namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    public partial class EgsDeviceFaceDetectionOnHost
    {
        public Nullable<System.Drawing.Rectangle> SelectedFaceRect { get; private set; }

        public void SelectOneFaceRect()
        {
            if (DetectedFaceRectsInCameraViewImage == null || DetectedFaceRectsInCameraViewImage.Count == 0)
            {
                SelectedFaceRect = null;
            }
            else
            {
                SelectMostCenter();
            }
            OnPropertyChanged("SelectedFaceRect");
        }

        void SelectMostCenter()
        {
            Func<System.Drawing.Rectangle, double> predictor = e =>
            {
                var faceCenterX = e.Left + e.Width / 2;
                var cameraViewImageCenterX = CameraViewImageWidth / 2;
                return Math.Abs(faceCenterX - cameraViewImageCenterX);
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

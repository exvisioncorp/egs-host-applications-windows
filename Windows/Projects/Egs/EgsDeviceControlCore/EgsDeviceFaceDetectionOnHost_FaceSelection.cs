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
                return;
            }
            SelectMostCenter();
        }

        void SelectMostCenter()
        {
            Func<System.Drawing.Rectangle, double> predictor = e =>
            {
                var faceCenterX = e.Left + e.Width / 2;
                var cameraViewImageCenterX = CameraViewImageWidth / 2;
                return Math.Abs(faceCenterX - cameraViewImageCenterX);
            };

            SelectedFaceRect = DetectedFaceRectsInCameraViewImage[0];
            var distanceMinimum = predictor(SelectedFaceRect.Value);

            for (int i = 1; i < DetectedFaceRectsInCameraViewImage.Count; i++)
            {
                var distance = predictor(DetectedFaceRectsInCameraViewImage[i]);
                if (distance < distanceMinimum)
                {
                    distanceMinimum = distance;
                    SelectedFaceRect = DetectedFaceRectsInCameraViewImage[i];
                }
            }
        }

        void SelectBiggest()
        {
            Func<System.Drawing.Rectangle, double> predictor = e => e.Width + e.Height;

            SelectedFaceRect = DetectedFaceRectsInCameraViewImage[0];
            var faceSizeMax = predictor(SelectedFaceRect.Value);

            for (int i = 1; i < DetectedFaceRectsInCameraViewImage.Count; i++)
            {
                var size = predictor(DetectedFaceRectsInCameraViewImage[i]);
                if (size > faceSizeMax)
                {
                    faceSizeMax = size;
                    SelectedFaceRect = DetectedFaceRectsInCameraViewImage[i];
                }
            }
        }
    }
}

namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;

    public class FaceSelectionModel
    {
        public FaceDetectionModel FaceDetection { get; private set; }
        public Nullable<System.Drawing.Rectangle> SelectedFaceRect { get; private set; }

        public FaceSelectionModel(FaceDetectionModel faceDetection)
        {
            Trace.Assert(faceDetection != null);
            FaceDetection = faceDetection;
        }

        public void SelectOneFaceRect()
        {
            if (FaceDetection.DetectedFaceRects == null || FaceDetection.DetectedFaceRects.Count == 0)
            {
                SelectedFaceRect = null;
                return;
            }
            SelectedFaceRect = FaceDetection.DetectedFaceRects[0];
            Func<System.Drawing.Rectangle, double> predictor = e => e.Width + e.Height;
            var faceSizeMax = predictor(SelectedFaceRect.Value);
            for (int i = 1; i < FaceDetection.DetectedFaceRects.Count; i++)
            {
                var size = predictor(FaceDetection.DetectedFaceRects[i]);
                if (size > faceSizeMax)
                {
                    faceSizeMax = size;
                    SelectedFaceRect = FaceDetection.DetectedFaceRects[i];
                }
            }
        }
    }
}

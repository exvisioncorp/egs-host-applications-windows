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
    }
}

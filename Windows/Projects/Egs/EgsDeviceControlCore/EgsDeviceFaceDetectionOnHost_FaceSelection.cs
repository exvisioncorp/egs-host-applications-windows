namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using DotNetUtility;

    // TODO: MUSTDO: implement

    public partial class EgsDeviceFaceDetectionOnHost
    {
        double DistanceFromCameraViewImageCenter(System.Drawing.Rectangle rect)
        {
            var faceX = rect.Left + rect.Width / 2.0;
            var faceY = rect.Top + rect.Height / 2.0;
            var imageX = CameraViewImageWidth / 2.0;
            var imageY = CameraViewImageHeight / 2.0;
            var dx = faceX - imageX;
            var dy = faceY - imageY;
            var ret = Math.Sqrt(dx * dx + dy * dy);
            return ret;
        }
    }
}

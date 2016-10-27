namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Controls.Primitives;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;

    public class OpaqueClickableImage : Image
    {
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            var source = (BitmapSource)Source;

            // Get the pixel of the source that was hit
            var x = (int)(hitTestParameters.HitPoint.X * (source.PixelWidth - 1) / ActualWidth);
            var y = (int)(hitTestParameters.HitPoint.Y * (source.PixelHeight - 1) / ActualHeight);

            // Copy the single pixel into a new byte array representing RGBA
            var pixel = new byte[4];
            source.CopyPixels(new Int32Rect(x, y, 1, 1), pixel, 4, 0);

            if (false) { Debug.WriteLine("x={0}, y={1}, pixel={{{2},{3},{4},{5}}}, Format={6}", x, y, pixel[0], pixel[1], pixel[2], pixel[3], source.Format); }

            if (source.Format == PixelFormats.Bgra32 && pixel[3] == 0) { return null; }
            else if (source.Format == PixelFormats.Indexed8 && pixel[0] == 0) { return null; }

            return new PointHitTestResult(this, hitTestParameters.HitPoint);
        }
    }
}

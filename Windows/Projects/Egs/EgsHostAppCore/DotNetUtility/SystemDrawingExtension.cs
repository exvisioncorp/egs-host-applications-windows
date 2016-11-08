namespace Egs.DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;

    [EditorBrowsable(EditorBrowsableState.Never)]
    static class SystemDrawingExtension
    {
        public static System.Windows.Rect ToWpfRect(this System.Drawing.Rectangle src)
        {
            return new System.Windows.Rect((double)src.X, (double)src.Y, (double)src.Width, (double)src.Height);
        }
        public static System.Windows.Point ToWpfPoint(this System.Drawing.Point src)
        {
            return new System.Windows.Point((double)src.X, (double)src.Y);
        }
        public static System.Windows.Size ToWpfSize(this System.Drawing.Size src)
        {
            return new System.Windows.Size((double)src.Width, (double)src.Height);
        }
    }
}

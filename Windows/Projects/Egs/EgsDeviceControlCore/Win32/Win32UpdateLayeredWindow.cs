namespace Egs.Win32
{
    using System;
    using System.Runtime.InteropServices;

    // NOTE: We referred various documents on the Web.  Maybe some codes have their origin in some documents written by Rui Godinho Lopes.  For example, a code in CodeProject
    // http://www.codeproject.com/script/Content/ViewAssociatedFile.aspx?rzp=%2FKB%2Fgraphics%2FalphaBG%2Falphabg_src.zip&zep=Alpha+Blended+Form%2FPerPixelAlphaForm.cs&obid=20758&obtid=2&ovid=1

    //[System.Security.SuppressUnmanagedCodeSecurityAttribute]
    internal static partial class NativeMethods
    {
        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Point
        {
            public int x;
            public int y;
            public Win32Point(int x, int y) { this.x = x; this.y = y; }
        }

        [StructLayout(LayoutKind.Sequential)]
        internal struct Win32Size
        {
            public int cx;
            public int cy;
            public Win32Size(int cx, int cy) { this.cx = cx; this.cy = cy; }
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct ARGB
        {
            public byte Blue;
            public byte Green;
            public byte Red;
            public byte Alpha;
        }

        [StructLayout(LayoutKind.Sequential, Pack = 1)]
        internal struct BLENDFUNCTION
        {
            public byte BlendOp;
            public byte BlendFlags;
            public byte SourceConstantAlpha;
            public byte AlphaFormat;
        }


        internal const Int32 ULW_COLORKEY = 0x00000001;
        internal const Int32 ULW_ALPHA = 0x00000002;
        internal const Int32 ULW_OPAQUE = 0x00000004;

        internal const byte AC_SRC_OVER = 0x00;
        internal const byte AC_SRC_ALPHA = 0x01;

        internal static int GWL_EXSTYLE = -20;
        internal static int WS_EX_LAYERED = 0x80000;
        internal static int LWA_ALPHA = 0x2;

        [DllImport("kernel32.dll", EntryPoint = "CopyMemory")]
        extern internal static void CopyMemory(IntPtr Destination, IntPtr Source, uint Length);

        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool UpdateLayeredWindow(IntPtr hwnd, IntPtr hdcDst, ref Win32Point pptDst, ref Win32Size psize, IntPtr hdcSrc, ref Win32Point pprSrc, Int32 crKey, ref BLENDFUNCTION pblend, int dwFlags);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        extern internal static IntPtr CreateCompatibleDC(IntPtr hDC);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool DeleteDC(IntPtr hdc);

        [DllImport("gdi32.dll", ExactSpelling = true)]
        extern internal static IntPtr SelectObject(IntPtr hDC, IntPtr hObject);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool DeleteObject(IntPtr hObject);

        [DllImport("user32")]
        extern internal static int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32")]
        extern internal static int GetWindowLong(IntPtr hWnd, int nIndex);
    }

    internal static partial class NativeMethods
    {
        internal static void CallWin32UpdateLayeredWindow(System.Windows.Forms.Form form, System.Drawing.Bitmap bitmap, IntPtr hBitmap, byte opacity, int x, int y)
        {
            // NOTE: opacity is normally 255.
            if (bitmap.PixelFormat != System.Drawing.Imaging.PixelFormat.Format32bppArgb)
            {
                throw new ArgumentException("The bitmap must be 32bpp with alpha-channel.", "bitmap");
            }

            var hdcForTheEntireScreen = NativeMethods.GetDC(IntPtr.Zero);
            var compatibleDcForTheEntireScreen = NativeMethods.CreateCompatibleDC(hdcForTheEntireScreen);
            var hBitmapBeingReplaced = IntPtr.Zero;

            try
            {
                // The SelectObject function selects an object into the specified device context (DC).  The new object replaces the previous object of the same type.
                // Return value: If the selected object is not a region and the function succeeds, the return value is a handle to the object being replaced.  If the selected object is a region and the function succeeds, the return value is one of the following values.
                // https://msdn.microsoft.com/en-us/library/windows/desktop/dd162957(v=vs.85).aspx
                hBitmapBeingReplaced = NativeMethods.SelectObject(compatibleDcForTheEntireScreen, hBitmap);

                var bitmapSize = new NativeMethods.Win32Size(bitmap.Width, bitmap.Height);
                var sourcePoint = new NativeMethods.Win32Point(0, 0);
                var leftTopPoint = new NativeMethods.Win32Point(x, y);
                var blend = new NativeMethods.BLENDFUNCTION();
                blend.BlendOp = NativeMethods.AC_SRC_OVER;
                blend.BlendFlags = 0;
                blend.SourceConstantAlpha = opacity;
                blend.AlphaFormat = NativeMethods.AC_SRC_ALPHA;

                NativeMethods.SetWindowLong(form.Handle, NativeMethods.GWL_EXSTYLE, NativeMethods.GetWindowLong(form.Handle, NativeMethods.GWL_EXSTYLE) | NativeMethods.WS_EX_LAYERED);
                NativeMethods.UpdateLayeredWindow(form.Handle, hdcForTheEntireScreen, ref leftTopPoint, ref bitmapSize, compatibleDcForTheEntireScreen, ref sourcePoint, 0, ref blend, NativeMethods.ULW_ALPHA);
            }
            finally
            {
                NativeMethods.ReleaseDC(IntPtr.Zero, hdcForTheEntireScreen);
                if (hBitmap != IntPtr.Zero)
                {
                    NativeMethods.SelectObject(compatibleDcForTheEntireScreen, hBitmapBeingReplaced);
                }
                NativeMethods.DeleteDC(compatibleDcForTheEntireScreen);
            }
        }
    }
}

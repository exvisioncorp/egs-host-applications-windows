﻿namespace Egs.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using System.IO;
    using System.ComponentModel;
    using Egs.PropertyTypes;

    /// <summary>
    /// This class shows "Gesture Cursor".  This class must be initialized by InitializeOnceAtStartup() method.
    /// </summary>
    public partial class CursorForm : Form
    {
        sealed class CursorImageWindowsFormsBitmap : IDisposable
        {
            internal Bitmap CursorBitmap { get; set; }
            internal IntPtr CursorHBitmapForUiAccessTrue { get; set; }

            private bool disposed = false;
            internal void ReleaseObjects()
            {
                if (CursorBitmap != null) { CursorBitmap.Dispose(); CursorBitmap = null; }
                if (CursorHBitmapForUiAccessTrue != IntPtr.Zero) { Egs.Win32.NativeMethods.DeleteObject(CursorHBitmapForUiAccessTrue); CursorHBitmapForUiAccessTrue = IntPtr.Zero; }
            }
            public void Dispose()
            {
                if (disposed) { return; }
                ReleaseObjects();
                disposed = true;
                GC.SuppressFinalize(this);
            }
            ~CursorImageWindowsFormsBitmap() { Dispose(); }
        }

        // TODO: MUSTDO: If you set this to true, various problems happens.  I doubt that it is caused by set "TOPMOST" every time.  I set this to false for now.
        const bool isToUsePictureBox = false;

        internal CursorViewModel refToCursorViewModel { get; private set; }

        /// <summary>
        /// List of ImageInformationSet.  One ImageInformationSet has "open hand" and "closed hand" images and so on.  And this list has the list of the image set, i.e. "00_Defalut", "01_Blue", "02_Blue" and so on.
        /// </summary>
        public IList<ImageInformationSet> CursorImageInformationSetList { get; private set; }

        internal int ActualWindowLeft { get; private set; }
        internal int ActualWindowTop { get; private set; }

        Dictionary<ImageInformation, CursorImageWindowsFormsBitmap> imagesDict { get; set; }
        PictureBox currentPictureBox { get; set; }
        bool hasToRedrawCursor { get; set; }
        Win32SetWindowPosition setWindowPosition { get; set; }

        public CursorForm()
        {
            InitializeComponent();

            hasToRedrawCursor = false;
        }

        /// <summary>
        /// This method must be called after CursorViewModel and List of ImageInformationSet are constructed.
        /// </summary>
        public void InitializeOnceAtStartup(CursorViewModel cursorViewModel, IList<ImageInformationSet> cursorImageInformationSetList)
        {
            Trace.Assert(cursorViewModel != null);
            Trace.Assert(cursorImageInformationSetList != null);

            refToCursorViewModel = cursorViewModel;
            CursorImageInformationSetList = cursorImageInformationSetList;

            try
            {
                imagesDict = new Dictionary<ImageInformation, CursorImageWindowsFormsBitmap>();
                foreach (var cursorImageInformationSet in CursorImageInformationSetList)
                {
                    foreach (var cursorImageInformation in cursorImageInformationSet.ImageInformationList)
                    {
                        imagesDict[cursorImageInformation] = new CursorImageWindowsFormsBitmap();
                        var fullPath = Path.Combine(cursorImageInformationSet.FolderPath, cursorImageInformation.FileRelativePath);
                        var bmp = (Bitmap)Bitmap.FromFile(fullPath);
                        imagesDict[cursorImageInformation].CursorBitmap = bmp;
                        imagesDict[cursorImageInformation].CursorHBitmapForUiAccessTrue = bmp.GetHbitmap(Color.FromArgb(0));
                    }
                }
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging)
                {
                    Debugger.Break();
                    MessageBox.Show(ex.Message);
                    MessageBox.Show("Application cannot find some image resources.");
                }
                throw;
            }

            if (isToUsePictureBox)
            {
                currentPictureBox = new PictureBox() { Dock = DockStyle.Fill };
                this.Controls.Add(currentPictureBox);
                this.AllowTransparency = true;
                this.TransparencyKey = Color.FromArgb(240, 240, 240); ;
            }
            else
            {
                setWindowPosition = new Win32SetWindowPosition(this.Handle);
            }

            refToCursorViewModel.IsVisibleChanged += refToCursorViewModel_IsVisibleChanged;
            refToCursorViewModel.StateUpdated += refToCursorViewModel_StateUpdated;

            // TODO: MUSTDO: Update these values when image set is updated.
            Width = 256;
            Height = 256;
        }

        void refToCursorViewModel_IsVisibleChanged(object sender, EventArgs e)
        {
            if (false) { Console.WriteLine("IsVisible Changed: " + refToCursorViewModel.IsVisible); }
            if (refToCursorViewModel.IsVisible)
            {
                if (isToUsePictureBox)
                {
                    this.TopMost = true;
                }
                else
                {
                    setWindowPosition.BringToTop();
                }
                hasToRedrawCursor = true;
            }
            else
            {
                this.Hide();
            }
        }

        void refToCursorViewModel_StateUpdated(object sender, EventArgs e)
        {
            int imageIndex = refToCursorViewModel.CurrentImageIndex;
            if (imageIndex < 0) { return; }
            ActualWindowLeft = (int)(refToCursorViewModel.PositionX - this.Width / 2.0);
            ActualWindowTop = (int)(refToCursorViewModel.PositionY - this.Height / 2.0);
            hasToRedrawCursor = true;
        }

        [EditorBrowsable(EditorBrowsableState.Never)]
        // NOTE: This is an override member, so cannot be changed to internal.
        protected override CreateParams CreateParams
        {
            [System.Security.Permissions.SecurityPermission(System.Security.Permissions.SecurityAction.LinkDemand, Flags = System.Security.Permissions.SecurityPermissionFlag.UnmanagedCode)]
            get
            {
                if (isToUsePictureBox)
                {
                    return base.CreateParams;
                }
                else
                {
                    CreateParams cp = base.CreateParams;
                    const int WS_EX_LAYERED = 0x80000;
                    const int WS_EX_NOACTIVATE = 0x08000000;
                    const int WS_EX_TOOLWINDOW = 0x00000080;
                    const int WS_EX_TRANSPARENT = 0x00000020;
                    const int WS_EX_TOPMOST = 0x00000008;
                    cp.ExStyle |= WS_EX_NOACTIVATE | WS_EX_LAYERED | WS_EX_TOOLWINDOW | WS_EX_TRANSPARENT | WS_EX_TOPMOST;
                    return cp;
                }
            }
        }

        /// <summary>
        /// By calling this method, the Gesture Cursor position will be updated.  Typically, this method is called after updating CursorViewModel object in "EgsDevice.EgsGestureHidReport.ReportUpdated" event's event handler.
        /// </summary>
        public void UpdatePosition()
        {
            if (hasToRedrawCursor == false) { return; }

            // TODO: MUSTDO: Performance stress test, when the app runs on lower spec PC.
            if (false && ApplicationCommonSettings.IsDebuggingInternal) { System.Threading.Thread.Sleep(3); }

            int cursorImageIndex = refToCursorViewModel.CurrentImageIndex;

            if ((refToCursorViewModel.IsVisible == false) || (cursorImageIndex < 0))
            {
                // MUSTDO: FIX: this code is called too many times when left hand operation in mouse mode.
                //Debug.WriteLine("refToCursorViewModel.IsVisible: " + refToCursorViewModel.IsVisible);
                //Debug.WriteLine("cursorImageIndex: " + cursorImageIndex);
                if (true)
                {
                    this.BeginInvoke(new Action(() => { this.Visible = false; }));
                }
                else
                {
                    // MUSTDO: FIX: In tutorial, cross thread exception occurs.
                    this.Visible = false;
                }
                return;
            }

            ActualWindowLeft = (int)(refToCursorViewModel.PositionX - this.Width / 2.0);
            ActualWindowTop = (int)(refToCursorViewModel.PositionY - this.Height / 2.0);

            var imageSetIndex = refToCursorViewModel.CurrentCursorImageSetIndex;

            if (isToUsePictureBox)
            {
                currentPictureBox.Image = imagesDict[CursorImageInformationSetList[imageSetIndex].ImageInformationList[cursorImageIndex]].CursorBitmap;
                this.TopMost = true;
                this.Left = ActualWindowLeft;
                this.Top = ActualWindowTop;
            }
            else
            {
                var bmp = imagesDict[CursorImageInformationSetList[imageSetIndex].ImageInformationList[cursorImageIndex]];
                Win32.NativeMethods.CallWin32UpdateLayeredWindow(this, bmp.CursorBitmap, bmp.CursorHBitmapForUiAccessTrue, 255, ActualWindowLeft, ActualWindowTop);
                setWindowPosition.SetWindowPosition(ActualWindowLeft, ActualWindowTop);
            }
            hasToRedrawCursor = false;
        }
    }
}

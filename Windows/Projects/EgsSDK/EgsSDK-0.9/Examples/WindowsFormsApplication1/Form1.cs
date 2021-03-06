﻿namespace WindowsFormsApplication1
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Data;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using Egs;
    using Egs.Views;

    public partial class Form1 : Form
    {
        // Basic objects are EgsDeviceSettings and EgsDevice.
        // An instance of EgsDevice creates and holds
        //  * EgsDeviceCameraViewImageSourceBitmapCapture  CameraViewImageSourceBitmapCapture;    // Bitmap capture with AForge.NET
        //  * EgsDeviceEgsGestureHidReport      EgsGestureHidReport;        // HID report as vendor-specific
        //  * EgsDeviceTouchScreenHidReport     TouchScreenHidReport;       // HID report for OS
        public EgsDevice Device { get; private set; }

        // CursorViewModel can receive EgsGestureHidReport objects and get more useful information about tracking hands.
        public IList<CursorViewModel> CursorViewModels { get; private set; }
        // CursorForm objects can draw "Gesture Cursor"s.
        public IList<CursorForm> CursorViews { get; private set; }

        // CameraViewUserControl and CameraViewWindow are not used in this example program.
        public PictureBox CameraViewImagePictureBox { get; private set; }
        public Bitmap LatestWinFormsBitmap;

        public Form1()
        {
            InitializeComponent();

            // It shows some debugging information.
            ApplicationCommonSettings.IsDebugging = true;


            CameraViewImagePictureBox = new PictureBox() { Dock = DockStyle.Fill, SizeMode = PictureBoxSizeMode.CenterImage };
            this.Controls.Add(CameraViewImagePictureBox);
            this.Size = new Size(800, 600);


            Device = EgsDevice.GetDefaultEgsDevice();
            Device.Settings.FaceDetectionMethod.Value = Egs.PropertyTypes.FaceDetectionMethods.DefaultProcessOnEgsDevice;
            Device.Settings.IsToDetectFaces.Value = true;
            Device.Settings.IsToDetectHands.Value = true;
#if DEBUG
            Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = true;
            Device.Settings.CameraViewImageSourceBitmapSize.Value = Egs.PropertyTypes.CameraViewImageSourceBitmapSizes.Size_640x480;
#else
            Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
            Device.Settings.CameraViewImageSourceBitmapSize.Value = Egs.PropertyTypes.CameraViewImageSourceBitmapSizes.Size_384x240;
#endif


            // CursorForm objects observe CursorViewModel objects in the OnePersonBothHandsViewModel object, and draw the "Gesture Cursor" on desktop.
            {
                CursorViewModels = new CursorViewModel[Device.TrackableHandsCountMaximum];
                CursorViewModels[0] = new CursorViewModel();
                CursorViewModels[1] = new CursorViewModel();
                CursorViews = new CursorForm[Device.TrackableHandsCountMaximum];
                CursorViews[0] = new CursorForm();
                CursorViews[1] = new CursorForm();
                var imageSetListRight = new List<ImageInformationSet>();
                var imageSetListLeft = new List<ImageInformationSet>();
                imageSetListRight.Add(new ImageInformationSet() { ImageSetIndex = 0, FolderPath = "Resources\\", SampleImageFileRelativePath = "Resources\\Sample.png" });
                imageSetListRight[0].AddImage((int)CursorImageIndexLabels.OpenHand, "Right_OpenHand.png");
                imageSetListRight[0].AddImage((int)CursorImageIndexLabels.CloseHand, "Right_CloseHand.png");
                imageSetListLeft.Add(new ImageInformationSet() { ImageSetIndex = 0, FolderPath = "Resources\\", SampleImageFileRelativePath = "Resources\\Sample.png" });
                imageSetListLeft[0].AddImage((int)CursorImageIndexLabels.OpenHand, "Left_OpenHand.png");
                imageSetListLeft[0].AddImage((int)CursorImageIndexLabels.CloseHand, "Left_CloseHand.png");
                CursorViews[0].InitializeOnceAtStartup(CursorViewModels[0], imageSetListRight);
                CursorViews[1].InitializeOnceAtStartup(CursorViewModels[1], imageSetListLeft);
                Device.EgsGestureHidReport.ReportUpdated += EgsGestureHidReport_ReportUpdated;
            }

            // You can make your original camera view.
            Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += delegate
            {
                if (CameraViewImagePictureBox.Image != null) { CameraViewImagePictureBox.Image.Dispose(); }
                // Bitmap.Clone() is shallow copy method.
                var bmp = (System.Drawing.Bitmap)Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmap.Clone();
                using (var g = Graphics.FromImage(bmp))
                {
                    if (Device.EgsGestureHidReport.IsStandingBy) { }
                    else if (Device.EgsGestureHidReport.IsFaceDetecting)
                    {
                        g.DrawRectangle(Pens.LightCyan, Device.EgsGestureHidReport.FaceDetectionArea);
                        for (int i = 0; i < Device.EgsGestureHidReport.DetectedFacesCount; i++)
                        {
                            g.DrawRectangle(Pens.LightGreen, Device.EgsGestureHidReport.Faces[i].Area);
                        }
                        if (Device.EgsGestureHidReport.SelectedFaceIndex >= 0)
                        {
                            g.DrawRectangle(Pens.LightYellow, Device.EgsGestureHidReport.Faces[Device.EgsGestureHidReport.SelectedFaceIndex].Area);
                        }
                        foreach (var hand in Device.EgsGestureHidReport.Hands)
                        {
                            g.DrawRectangle(Pens.OrangeRed, hand.DetectionArea);
                        }
                    }
                    else
                    {
                        g.DrawRectangle(Pens.LightYellow, Device.EgsGestureHidReport.Faces[Device.EgsGestureHidReport.SelectedFaceIndex].Area);
                        foreach (var hand in Device.EgsGestureHidReport.Hands)
                        {
                            g.DrawRectangle(Pens.OrangeRed, hand.ScreenMappedArea);
                            int ellipseRadius = hand.IsTouching ? 5 : 10;
                            g.DrawEllipse(Pens.OrangeRed, (int)hand.XInCameraViewImage - ellipseRadius, (int)hand.YInCameraViewImage - ellipseRadius, ellipseRadius * 2, ellipseRadius * 2);
                        }
                    }
                }
                CameraViewImagePictureBox.Image = bmp;
            };

            this.FormClosed += (sender, e) =>
            {
                // If you do not detach this event handler, exceptions can happen.
                Device.EgsGestureHidReport.ReportUpdated -= EgsGestureHidReport_ReportUpdated;

                // When the application quits, please stop face detection and hand detection.
                Device.Settings.IsToDetectFaces.Value = false;
                Device.Settings.IsToDetectHands.Value = false;
                Device.Settings.IsToDrawBordersOnCameraViewImageByDevice.Value = false;
                EgsDevice.CloseDefaultEgsDevice();

                CameraViewImagePictureBox.Dispose();
                foreach (var cursorView in CursorViews) { cursorView.Close(); }
            };
        }

        bool isDrawingCursors = false;
        void EgsGestureHidReport_ReportUpdated(object sender, EventArgs e)
        {
            for (int i = 0; i < Device.TrackableHandsCount; i++)
            {
                CursorViewModels[i].UpdateByEgsGestureHidReportHand(Device.EgsGestureHidReport.Hands[i]);
            }
            // If Task is not used, delay in moving cursors can accumulate.
            System.Threading.Tasks.Task.Run(() =>
            {
                if (isDrawingCursors) { return; }
                isDrawingCursors = true;
                for (int i = 0; i < Device.TrackableHandsCount; i++)
                {
                    CursorViews[i].UpdatePosition();
                }
                isDrawingCursors = false;
            });
        }
    }
}

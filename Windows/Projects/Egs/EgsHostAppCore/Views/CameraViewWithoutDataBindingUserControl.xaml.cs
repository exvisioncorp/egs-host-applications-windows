namespace Egs.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;
    using Egs.DotNetUtility;

    public partial class CameraViewWithoutDataBindingUserControl : UserControl
    {
        CameraViewUserControlModel cameraViewUserControlModel { get; set; }

        public Thickness MarginBetweenUserControlAndContent
        {
            get
            {
                return new Thickness(
                    cameraViewBorder.BorderThickness.Left + cameraViewBorder.Margin.Left,
                    cameraViewBorder.BorderThickness.Top + cameraViewBorder.Margin.Top,
                    cameraViewBorder.BorderThickness.Right + cameraViewBorder.Margin.Right,
                    cameraViewBorder.BorderThickness.Bottom + cameraViewBorder.Margin.Bottom);
            }
        }

        List<Grid> FaceGrids { get; set; }
        List<TranslateTransform> FaceLocations { get; set; }
        List<ContentControl> FaceSizes { get; set; }
        List<Grid> HandDetectionAreaGrids { get; set; }
        List<TranslateTransform> HandDetectionAreaLocations { get; set; }
        List<ContentControl> HandDetectionAreaSizes { get; set; }
        List<Grid> HandScreenMappedAreaGrids { get; set; }
        List<TranslateTransform> HandScreenMappedAreaLocations { get; set; }
        List<ContentControl> HandScreenMappedAreaSizes { get; set; }
        List<Grid> HandPointerGrids { get; set; }
        List<TranslateTransform> HandPointerPositions { get; set; }
        List<ContentControl> HandPointerHoveringContentControls { get; set; }
        List<ContentControl> HandPointerTouchingContentControls { get; set; }

        public CameraViewWithoutDataBindingUserControl()
        {
            InitializeComponent();

            cameraDeviceIsDisconnectedMessageGrid.Visibility = Visibility.Collapsed;
            cameraViewImage.Visibility = Visibility.Collapsed;
            cameraViewImageBordersAndPointersGrid.Visibility = Visibility.Collapsed;

            FaceGrids = new List<Grid>() { Face0Grid, Face1Grid, Face2Grid, Face3Grid, Face4Grid };
            FaceLocations = new List<TranslateTransform>() { Face0Location, Face1Location, Face2Location, Face3Location, Face4Location };
            FaceSizes = new List<ContentControl>() { Face0Size, Face1Size, Face2Size, Face3Size, Face4Size };
            HandDetectionAreaGrids = new List<Grid>() { Hand0DetectionAreaGrid, Hand1DetectionAreaGrid };
            HandDetectionAreaLocations = new List<TranslateTransform>() { Hand0DetectionAreaLocation, Hand1DetectionAreaLocation };
            HandDetectionAreaSizes = new List<ContentControl>() { Hand0DetectionAreaSize, Hand1DetectionAreaSize };
            HandScreenMappedAreaGrids = new List<Grid>() { Hand0ScreenMappedAreaGrid, Hand1ScreenMappedAreaGrid };
            HandScreenMappedAreaLocations = new List<TranslateTransform>() { Hand0ScreenMappedAreaLocation, Hand1ScreenMappedAreaLocation };
            HandScreenMappedAreaSizes = new List<ContentControl>() { Hand0ScreenMappedAreaSize, Hand1ScreenMappedAreaSize };
            HandPointerGrids = new List<Grid>() { Hand0PointerGrid, Hand1PointerGrid };
            HandPointerPositions = new List<TranslateTransform>() { Hand0PointerPosition, Hand1PointerPosition };
            HandPointerHoveringContentControls = new List<ContentControl>() { Hand0PointerHoveringContentControl, Hand1PointerHoveringContentControl };
            HandPointerTouchingContentControls = new List<ContentControl>() { Hand0PointerTouchingContentControl, Hand1PointerTouchingContentControl };

            if (ApplicationCommonSettings.IsDebugging)
            {
                cameraDeviceIsDisconnectedMessageGrid.IsVisibleChanged += delegate
                {
                    Debug.WriteLine("cameraDeviceIsDisconnectedMessageGrid.IsVisible: " + cameraDeviceIsDisconnectedMessageGrid.IsVisible);
                };
            }
        }

        public void InitializeOnceAtStartup(CameraViewUserControlModel viewModel)
        {
            Trace.Assert(viewModel != null);
            Trace.Assert(viewModel.Device != null && viewModel.Device.CameraViewImageSourceBitmapCapture != null);
            if (cameraViewUserControlModel != null)
            {
                Trace.Assert(cameraViewUserControlModel.Device != null && cameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture != null);
                cameraViewUserControlModel.CameraViewWpfBitmapSourceWidthChanged -= cameraViewUserControlModel_CameraViewWpfBitmapSourceWidthChanged;
                cameraViewUserControlModel.CameraViewWpfBitmapSourceHeightChanged -= cameraViewUserControlModel_CameraViewWpfBitmapSourceHeightChanged;
                cameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture.IsCameraDeviceConnectedChanged -= capture_IsCameraDeviceConnectedChanged;
                cameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged -= capture_CameraViewImageSourceBitmapChanged;
                cameraViewUserControlModel = null;
            }
            this.DataContext = null;
            cameraViewUserControlModel = viewModel;
            cameraViewUserControlModel.CameraViewWpfBitmapSourceWidthChanged += cameraViewUserControlModel_CameraViewWpfBitmapSourceWidthChanged;
            cameraViewUserControlModel.CameraViewWpfBitmapSourceHeightChanged += cameraViewUserControlModel_CameraViewWpfBitmapSourceHeightChanged;
            cameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture.IsCameraDeviceConnectedChanged += capture_IsCameraDeviceConnectedChanged;
            cameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapChanged += capture_CameraViewImageSourceBitmapChanged;
        }

        void cameraViewUserControlModel_CameraViewWpfBitmapSourceWidthChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                mainGrid.Width = cameraViewUserControlModel.CameraViewWpfBitmapSourceWidth;
                cameraViewImageBordersAndPointersGrid.Width = cameraViewUserControlModel.CameraViewWpfBitmapSourceWidth;
            }));
        }

        void cameraViewUserControlModel_CameraViewWpfBitmapSourceHeightChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                mainGrid.Height = cameraViewUserControlModel.CameraViewWpfBitmapSourceHeight;
                cameraViewImageBordersAndPointersGrid.Height = cameraViewUserControlModel.CameraViewWpfBitmapSourceHeight;
            }));
        }

        void capture_IsCameraDeviceConnectedChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() =>
            {
                mainGrid.Width = cameraViewUserControlModel.CameraViewWpfBitmapSourceWidth;
                mainGrid.Height = cameraViewUserControlModel.CameraViewWpfBitmapSourceHeight;
                cameraViewImageBordersAndPointersGrid.Width = cameraViewUserControlModel.CameraViewWpfBitmapSourceWidth;
                cameraViewImageBordersAndPointersGrid.Height = cameraViewUserControlModel.CameraViewWpfBitmapSourceHeight;
            }));
        }

        void capture_CameraViewImageSourceBitmapChanged(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(new Action(() => { UpdateViewsByModel(); }));
        }

        void UpdateViewsByModel()
        {
            var device = cameraViewUserControlModel.Device;
            var report = cameraViewUserControlModel.Device.EgsGestureHidReport;
            var capture = cameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture;

            if (device == null) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } return; }
            if (report == null) { if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); } return; }
            if (capture == null)
            {
                cameraDeviceIsDisconnectedMessageGrid.Visibility = Visibility.Collapsed;
                cameraViewImage.Visibility = Visibility.Collapsed;
                cameraViewImageBordersAndPointersGrid.Visibility = Visibility.Collapsed;
                return;
            }

            var newCameraDeviceIsDisconnectedMessageGridVisibility = capture.IsCameraDeviceConnected ? Visibility.Collapsed : Visibility.Visible;
            if (cameraDeviceIsDisconnectedMessageGrid.Visibility != newCameraDeviceIsDisconnectedMessageGridVisibility)
            {
                cameraDeviceIsDisconnectedMessageGrid.Visibility = newCameraDeviceIsDisconnectedMessageGridVisibility;
            }

            var newCameraViewImageVisibility = (capture.IsCameraDeviceConnected && capture.IsUpdatingImageSource) ? Visibility.Visible : Visibility.Collapsed;
            if (cameraViewImage.Visibility != newCameraViewImageVisibility)
            {
                cameraViewImage.Visibility = newCameraViewImageVisibility;
            }
            if (capture.IsUpdatingImageSource)
            {
                cameraViewImage.Source = cameraViewUserControlModel.CameraViewWpfBitmapSource;
            }

            if (capture.IsCameraDeviceConnected == false || cameraViewUserControlModel.IsToDrawImageSet == false || report.IsStandingBy)
            {
                cameraViewImageBordersAndPointersGrid.Visibility = Visibility.Collapsed;
                return;
            }
            if (cameraViewImageBordersAndPointersGrid.Visibility == Visibility.Collapsed)
            {
                cameraViewImageBordersAndPointersGrid.Visibility = Visibility.Visible;
            }

            if (report.IsFaceDetecting)
            {
                for (int i = 0; i < report.DetectedFacesCount; i++)
                {
                    FaceGrids[i].Visibility = Visibility.Visible;
                    FaceGrids[i].Opacity = (i == report.SelectedFaceIndex) ? 1.0 : 0.5;
                    FaceLocations[i].X = report.Faces[i].Area.X;
                    FaceLocations[i].Y = report.Faces[i].Area.Y;
                    FaceSizes[i].Width = report.Faces[i].Area.Width;
                    FaceSizes[i].Height = report.Faces[i].Area.Height;
                }
                for (int i = report.DetectedFacesCount; i < 5; i++)
                {
                    FaceGrids[i].Visibility = Visibility.Collapsed;
                }

                for (int i = 0; i < 2; i++)
                {
                    HandDetectionAreaGrids[i].Visibility = report.Hands[i].IsDetecting ? Visibility.Visible : Visibility.Collapsed;
                    HandDetectionAreaLocations[i].X = report.Hands[i].DetectionArea.X;
                    HandDetectionAreaLocations[i].Y = report.Hands[i].DetectionArea.Y;
                    HandDetectionAreaSizes[i].Width = report.Hands[i].DetectionArea.Width;
                    HandDetectionAreaSizes[i].Height = report.Hands[i].DetectionArea.Height;
                }
            }
            else
            {
                // is detecting (1 or 2) hands or tracking (1 or 2) hands
                for (int i = 0; i < 5; i++)
                {
                    FaceGrids[i].Visibility = (i == report.SelectedFaceIndex) ? Visibility.Visible : Visibility.Collapsed;
                }
                {
                    var i = report.SelectedFaceIndex;
                    FaceLocations[i].X = report.Faces[i].Area.X;
                    FaceLocations[i].Y = report.Faces[i].Area.Y;
                    FaceSizes[i].Width = report.Faces[i].Area.Width;
                    FaceSizes[i].Height = report.Faces[i].Area.Height;
                }

                for (int i = 0; i < 2; i++)
                {
                    HandDetectionAreaGrids[i].Visibility = report.Hands[i].IsDetecting ? Visibility.Visible : Visibility.Collapsed;
                    if (report.Hands[i].IsDetecting)
                    {
                        HandDetectionAreaLocations[i].X = report.Hands[i].DetectionArea.X;
                        HandDetectionAreaLocations[i].Y = report.Hands[i].DetectionArea.Y;
                        HandDetectionAreaSizes[i].Width = report.Hands[i].DetectionArea.Width;
                        HandDetectionAreaSizes[i].Height = report.Hands[i].DetectionArea.Height;
                    }
                    HandScreenMappedAreaGrids[i].Visibility = report.Hands[i].IsTracking ? Visibility.Visible : Visibility.Collapsed;
                    HandPointerGrids[i].Visibility = report.Hands[i].IsTracking ? Visibility.Visible : Visibility.Collapsed;
                    if (report.Hands[i].IsTracking)
                    {
                        HandScreenMappedAreaLocations[i].X = report.Hands[i].ScreenMappedArea.X;
                        HandScreenMappedAreaLocations[i].Y = report.Hands[i].ScreenMappedArea.Y;
                        HandScreenMappedAreaSizes[i].Width = report.Hands[i].ScreenMappedArea.Width;
                        HandScreenMappedAreaSizes[i].Height = report.Hands[i].ScreenMappedArea.Height;
                        HandPointerPositions[i].X = report.Hands[i].XInCameraViewImage;
                        HandPointerPositions[i].Y = report.Hands[i].YInCameraViewImage;
                        HandPointerHoveringContentControls[i].Visibility = report.Hands[i].IsTouching ? Visibility.Collapsed : Visibility.Visible;
                        HandPointerTouchingContentControls[i].Visibility = report.Hands[i].IsTouching ? Visibility.Visible : Visibility.Collapsed;
                    }
                }
            }
        }
    }
}

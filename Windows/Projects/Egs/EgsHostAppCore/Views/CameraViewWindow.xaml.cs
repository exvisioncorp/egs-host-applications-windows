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

    using System.Windows.Media.Animation;
    using System.Windows.Controls.Primitives;
    using System.ComponentModel;
    using System.Diagnostics;
    using Egs.PropertyTypes;
    using Egs.DotNetUtility;
    using Egs.DotNetUtility.Views;

    public partial class CameraViewWindow : Window
    {
        CameraViewWindowModel cameraViewWindowModel { get; set; }
        CameraViewUserControlModel cameraViewUserControlModel { get; set; }
        System.Windows.Threading.DispatcherTimer windowsFormsCursorPositionMonitoringTimer { get; set; }
        bool isBeingResizedByMouse { get; set; }

        double DragStartLength { get; set; }
        Point mouseLeftButtonDownPointToThis { get; set; }

        public UniformGrid MainMenuItemsPanel { get { return mainMenuItemsPanel; } }
        public Button MinimizeButton { get { return minimizeButton; } }
        public Button SettingsButton { get { return settingsButton; } }
        public Button ExitButton { get { return exitButton; } }
        internal AspectRatioKeepingWindowResize Resizing { get; private set; }
        Egs.DotNetUtility.Dpi CurrentDpi { get; set; }

        static bool isToShowDebugMessages = false;
        static void DebugWrite(string debugMessage) { if (isToShowDebugMessages) { Debug.Write(debugMessage); } }
        static void DebugWriteLine(string debugMessage) { if (isToShowDebugMessages) { Debug.WriteLine(debugMessage); } }

        public CameraViewWindow()
        {
            InitializeComponent();

            this.MinWidth = 192;
            this.MinHeight = 120;

            try { this.Icon = Egs.DotNetUtility.BitmapImageUtility.LoadBitmapImageFromFile("Resources/CameraViewWindowIcon.png"); }
            catch { }
            this.Title = "ZKOO " + Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_CameraView;

            mainMenuItemsPanel.Visibility = Visibility.Collapsed;

            DragStartLength = 10.0;
            mouseLeftButtonDownPointToThis = new Point();

            Resizing = new AspectRatioKeepingWindowResize(this);
            Resizing.MarginBetweenWindowAndContent = cameraViewUserControl.MarginBetweenUserControlAndContent;
            Resizing.ContentAspectRatio = 384.0 / 240.0;
            Resizing.GripEdgeThickness = new Thickness(50);


            windowsFormsCursorPositionMonitoringTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) };
            windowsFormsCursorPositionMonitoringTimer.Tick += delegate
            {
                if (isBeingResizedByMouse && Mouse.LeftButton == MouseButtonState.Released) { Resizing.DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                var scaledCursorPosition = CurrentDpi.GetScaledPosition(System.Windows.Forms.Cursor.Position);
                Resizing.ResizeWithKeepingContentAspectRatio(scaledCursorPosition.X, scaledCursorPosition.Y);
            };
            Resizing.IsResizingChanged += (sender, e) =>
            {
                windowsFormsCursorPositionMonitoringTimer.IsEnabled = Resizing.IsResizing && cameraViewWindowModel.CanResize;
                if (Resizing.IsResizing == false) { isBeingResizedByMouse = false; }
            };


            this.MouseEnter += (sender, e) => WhenTouchEnterAndMouseEnter();
            this.MouseLeftButtonDown += (sender, e) => { DebugWrite("[Mouse] "); WhenTouchDownAndMouseLeftButtonDown(e.GetPosition(this)); };
            this.MouseMove += (sender, e) => { DebugWrite("[Mouse] "); WhenMouseMove((e.LeftButton == MouseButtonState.Pressed), e.GetPosition(this)); };
            this.MouseLeftButtonUp += (sender, e) => { DebugWrite("[Mouse] "); WhenTouchUpAndMouseLeftButtonUp(e.GetPosition(this)); };
            this.MouseRightButtonUp += (sender, e) => { DebugWrite("[Mouse] "); ShowMenu(); };
            this.MouseLeave += (sender, e) => { DebugWrite("[Mouse] "); WhenTouchLeaveAndMouseLeave(); };

#if false
            if (false)
            {
                this.TouchEnter += (sender, e) => WhenTouchEnterAndMouseEnter();
                this.TouchDown += (sender, e) => { DebugWrite("[Touch] "); WhenTouchDownAndMouseLeftButtonDown(e.GetTouchPoint(this).Position); };
                this.TouchUp += (sender, e) => { DebugWrite("[Touch] "); WhenTouchUpAndMouseLeftButtonUp(e.GetTouchPoint(this).Position); };
                this.TouchLeave += (sender, e) => { DebugWrite("[Mouse] "); WhenTouchLeaveAndMouseLeave(); };
                if (false)
                {
                    this.TouchMove += (sender, e) =>
                    {
                        // NOTE: This is only MOVE.
                        DebugWriteLine("[TouchMove] e.GetTouchPoint(this).Action: " + e.GetTouchPoint(this).Action);
                        DebugWrite("[Touch] ");
                        // NOTE: If you call the next method with "isPressed = true", exceptions can occur in calling DragMove().
                        WhenMouseMove(true, e.GetTouchPoint(this).Position);
                    };
                }
            }
#endif

            this.Visibility = Visibility.Visible;
        }
        void WhenTouchEnterAndMouseEnter()
        {
            DebugWriteLine("WhenTouchEnterAndMouseEnter()");
            if (cameraViewWindowModel == null) { return; }
            cameraViewWindowModel.IsMouseHovered = true;
        }
        void WhenTouchDownAndMouseLeftButtonDown(Point getPositionToThis)
        {
            DebugWriteLine("WhenTouchDownAndMouseLeftButtonDown(" + getPositionToThis + ")");
            if (cameraViewWindowModel == null) { return; }
            mouseLeftButtonDownPointToThis = getPositionToThis;
            cameraViewWindowModel.IsDragging = false;
            CurrentDpi = Egs.DotNetUtility.Dpi.DpiFromHdcForTheEntireScreen;
            var scaledCursorPosition = CurrentDpi.GetScaledPosition(System.Windows.Forms.Cursor.Position);
            Resizing.OnMouseDownOrTouchOnWindow(scaledCursorPosition.X, scaledCursorPosition.Y);
            isBeingResizedByMouse = true;
        }
        void WhenMouseMove(bool isPressed, Point getPositionToThis)
        {
            DebugWriteLine("WhenMouseMove(" + isPressed + ", " + getPositionToThis + ")");
            if (Resizing.IsResizing) { return; }
            if (cameraViewWindowModel == null) { return; }
            if (cameraViewWindowModel.IsDragging == false && isPressed)
            {
                if (cameraViewWindowModel == null) { return; }
                if (cameraViewWindowModel.CanDragMove)
                {
                    if (Resizing.DraggingRegion == AspectRatioKeepingWindowDraggingRegions.Center)
                    {
                        var mouseLeftButtonPointToThis = getPositionToThis;
                        var move = mouseLeftButtonPointToThis - mouseLeftButtonDownPointToThis;
                        if (move.Length >= DragStartLength)
                        {
                            this.DragMove();
                            cameraViewWindowModel.IsDragging = true;
                        }
                    }
                }
            }
        }
        void WhenTouchUpAndMouseLeftButtonUp(Point getPositionToThis)
        {
            DebugWriteLine("WhenTouchUpAndMouseLeftButtonUp(" + getPositionToThis + ")");
            Resizing.DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None;
            var move = getPositionToThis - mouseLeftButtonDownPointToThis;
            if (cameraViewWindowModel.CanShowMenu && move.Length < DragStartLength) { ShowMenu(); }
            cameraViewWindowModel.IsDragging = false;
        }
        void ShowMenu()
        {
            DebugWriteLine("ShowMenu()");
            if (cameraViewWindowModel == null) { return; }
            if (mainMenuItemsPanel.Visibility == Visibility.Collapsed)
            {
                (FindResource("SetVisibilityToVisibleWithOpacityIncreaseStoryboardKey") as Storyboard).Begin(mainMenuItemsPanel);
                mainMenuItemsPanel.Visibility = Visibility.Visible;
            }
        }
        void WhenTouchLeaveAndMouseLeave()
        {
            DebugWriteLine("WhenTouchLeaveAndMouseLeave()");
            if (cameraViewWindowModel == null) { return; }
            if (mainMenuItemsPanel.Visibility == Visibility.Visible)
            {
                (FindResource("SetVisibilityToCollapsedWithOpacityDecreaseStoryboardKey") as Storyboard).Begin(mainMenuItemsPanel);
                mainMenuItemsPanel.Visibility = Visibility.Collapsed;
            }
            cameraViewWindowModel.IsDragging = false;
            cameraViewWindowModel.IsMouseHovered = false;
        }

        public void InitializeOnceAtStartup(CameraViewWindowModel newCameraViewWindowModel, CameraViewUserControlModel newCameraViewUserControlModel)
        {
            Trace.Assert(newCameraViewWindowModel != null);
            Trace.Assert(newCameraViewUserControlModel != null);
            cameraViewUserControlModel = newCameraViewUserControlModel;
            cameraViewWindowModel = newCameraViewWindowModel;

            cameraViewUserControl.InitializeOnceAtStartup(newCameraViewUserControlModel);
            cameraViewWindowModel.IsDragging = false;
            cameraViewWindowModel.IsMouseHovered = false;

            DataContext = cameraViewWindowModel;

            cameraViewWindowModel.WindowStateChanged += delegate
            {
                // TODO: MUSTDO: find the better way
                this.Dispatcher.Invoke(() =>
                {
                    if (WindowState == WindowState.Maximized)
                    {
                        WindowState = WindowState.Normal;
                        return;
                    }
                    if (cameraViewWindowModel.IsNormalOrElseMinimized)
                    {
                        (FindResource("SetWindowStateToNormalWithOpacityIncreaseStoryboardKey") as Storyboard).Begin(this);
                    }
                    else
                    {
                        (FindResource("SetWindowStateToMinimizedWithOpacityDecreaseStoryboardKey") as Storyboard).Begin(this);
                    }
                });
            };

            newCameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapSizeOrPixelFormatChanged += (sender, e) =>
            {
                try
                {
                    var size = newCameraViewUserControlModel.Device.CameraViewImageSourceBitmapCapture.CameraViewImageSourceBitmapSize;
                    Resizing.ContentAspectRatio = (double)size.Width / (double)size.Height;
                    // TODO: MUSTDO: the next return is necessary, currently.
                    return;
                    var centerX = (Resizing.Left + Resizing.Right) / 2;
                    // NOTE: cannot use Application.Current.Dispatcher.Invoke
                    Resizing.Left = centerX - size.Width / 2;
                    Resizing.Right = centerX + size.Width / 2;
                }
                catch (Exception ex)
                {
                    if (ApplicationCommonSettings.IsDebuggingInternal) { Debugger.Break(); }
                    Console.WriteLine(ex.Message);
                }
            };

            this.Activated += (Sender, e) =>
            {
                DebugWriteLine("CameraView is activated.\r\n");
            };
        }

        internal void ReloadDataContext()
        {
            var currentDataContextBackup = this.DataContext;
            this.DataContext = null;
            this.DataContext = currentDataContextBackup;
        }
    }
}

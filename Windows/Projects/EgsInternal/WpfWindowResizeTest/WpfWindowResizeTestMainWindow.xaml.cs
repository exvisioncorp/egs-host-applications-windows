namespace WpfWindowResizeTest
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
    using System.Windows.Interop;
    using System.Diagnostics;
    using System.Runtime.InteropServices;
    using Egs.DotNetUtility;
    using DotNetUtility.Views;

    public partial class WpfWindowResizeTestMainWindow : Window
    {
        public AspectRatioKeepingWindowResize Resizing { get; private set; }
        System.Windows.Threading.DispatcherTimer windowsFormsCursorPositionMonitoringTimer { get; set; }
        bool isBeingResizedByMouse { get; set; }


        StateMonitoringWindow StateMonitorWindow { get; set; }
        Dpi CurrentDpi { get; set; }

        public WpfWindowResizeTestMainWindow()
        {
            InitializeComponent();

            CurrentDpi = Dpi.DpiFromHdcForTheEntireScreen;

            StateMonitorWindow = new StateMonitoringWindow();
            StateMonitorWindow.Initialize(this);
            StateMonitorWindow.Show();


            this.MinWidth = 192;
            this.MinHeight = 120;
            this.KeyDown += (sender, e) => { if (e.Key == Key.Escape) { this.Close(); } };


            Resizing = new AspectRatioKeepingWindowResize(this);
            Resizing.MarginBetweenWindowAndContent = new Thickness(
                windowBorder.BorderThickness.Left + windowBorder.Margin.Left,
                windowBorder.BorderThickness.Top + windowBorder.Margin.Top,
                windowBorder.BorderThickness.Right + windowBorder.Margin.Right,
                windowBorder.BorderThickness.Bottom + windowBorder.Margin.Bottom);
            Resizing.ContentAspectRatio = 384.0 / 240.0;
            Resizing.GripEdgeThickness = new Thickness(50);

            windowsFormsCursorPositionMonitoringTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(1000.0 / 60.0) };
            windowsFormsCursorPositionMonitoringTimer.Tick += delegate
            {
                if (isBeingResizedByMouse && Mouse.LeftButton == MouseButtonState.Released) { Resizing.DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
#if false
                // NOTE: App.configに、下記の記述を行わない限り、DPIが100%以外だとまともな値にならない！！！！！
                // <appSettings><add key="EnableWindowsFormsHighDpiAutoResizing" value="true" /></appSettings>
                var pos = System.Windows.Forms.Cursor.Position;
                var x = pos.X;
                var y = pos.Y;
#elif false
                // NOTE: これもダメ！
                var pos = Mouse.GetPosition(this);
                var x = this.Left + pos.X;
                var y = this.Top + pos.Y;
#else
                var pos = CurrentDpi.GetScaledPosition(System.Windows.Forms.Cursor.Position);
                var x = pos.X;
                var y = pos.Y;
#endif
                // NOTE: 渡された値は内部でDebug.Writelineで表示される。
                Resizing.ResizeWithKeepingContentAspectRatio(x, y);
            };
            Resizing.IsResizingChanged += (sender, e) =>
            {
                windowsFormsCursorPositionMonitoringTimer.IsEnabled = Resizing.IsResizing;
                if (Resizing.IsResizing == false) { isBeingResizedByMouse = false; }
            };
            Resizing.DraggingRegionChanged += (sender, e) =>
            {
                if (Resizing.DraggingRegion == AspectRatioKeepingWindowDraggingRegions.Center) { this.DragMove(); }
            };

            this.MouseLeftButtonDown += (sender, e) =>
            {
                CurrentDpi = Dpi.DpiFromHdcForTheEntireScreen;
                Debug.WriteLine("CurrentDpi: {0:G5}, {1:G5}", CurrentDpi.X, CurrentDpi.Y);
                var scaledCursorPosition = CurrentDpi.GetScaledPosition(System.Windows.Forms.Cursor.Position);
                Resizing.OnMouseDownOrTouchOnWindow(scaledCursorPosition.X, scaledCursorPosition.Y);
                isBeingResizedByMouse = true;
            };
            this.MouseLeftButtonUp += (sender, e) =>
            {
                Resizing.DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None;
            };
        }

#if false
        #region Old Code

        void Initialize02()
        {
            this.AllowsTransparency = false;
            this.WindowStyle = System.Windows.WindowStyle.None;
            this.ResizeMode = System.Windows.ResizeMode.CanResize;
            this.Left = 1;
            this.SizeChanged += (sender, e) =>
            {
                var widthDelta = e.NewSize.Width - e.PreviousSize.Width;
                var HeightDelta = e.NewSize.Height - e.PreviousSize.Height;
                var widthDeltaNormalized = Math.Abs(widthDelta / ContentAspectRatio);
                var heightDeltaNormalized = Math.Abs(HeightDelta);
                if (widthDeltaNormalized > heightDeltaNormalized)
                {
                    var contentWidth = e.NewSize.Width - (MarginBetweenWindowAndContent.Left + MarginBetweenWindowAndContent.Right);
                    var contentHeight = contentWidth / ContentAspectRatio;
                    isRenderSuspended = true;
                    this.Height = contentHeight + (MarginBetweenWindowAndContent.Top + MarginBetweenWindowAndContent.Bottom);
                    isRenderSuspended = false;
                    this.InvalidateVisual();
                }
                else
                {
                    var contentHeight = e.NewSize.Height - (MarginBetweenWindowAndContent.Top + MarginBetweenWindowAndContent.Bottom);
                    var contentWidth = contentHeight * ContentAspectRatio;
                    isRenderSuspended = true;
                    this.Width = contentWidth + (MarginBetweenWindowAndContent.Left + MarginBetweenWindowAndContent.Right);
                    isRenderSuspended = false;
                    this.InvalidateVisual();
                }
            };
        }

        bool isInSizeChangedEvent = false;
        Size previousWindowSize = new Size();
        double aspectRatio = 1.0;
        System.Windows.Threading.DispatcherTimer resizeDelayTimer { get; set; }

        void OldCodeInConstuctor()
        {
            this.Width = 384;
            this.SizeToContent = SizeToContent.Height;

            aspectRatio = (Width - marginOfTargetUIElementFromWindow) / (Height - marginOfTargetUIElementFromWindow);
            aspectRatio = Width / Height;

            this.MouseLeftButtonDown += (sender, e) => { DragMove(); };

            if (false)
            {
                // NG
                if (false) { userFeedbackImageUserControl.SizeChanged += (sender, e) => { this.SizeToContent = SizeToContent.WidthAndHeight; }; }
            }
            else if (false)
            {
                resizeDelayTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromSeconds(1.0) };
                resizeDelayTimer.Tick += delegate { UserFeedbackImageWindowResizeEventDelayTimer_01(); };
                this.SizeChanged += delegate { resizeDelayTimer.Start(); };
            }
            else
            {
                this.SizeToContent = SizeToContent.Manual;
                this._horizontalMargin = this.ActualWidth - targetWindowSizeUIElement.Width;
                this._verticalMargin = this.ActualHeight - targetWindowSizeUIElement.Height;
                this._fixRate = targetWindowSizeUIElement.Width / targetWindowSizeUIElement.Height;
                targetWindowSizeUIElement.Width = double.NaN;
                targetWindowSizeUIElement.Height = double.NaN;
            }
        }

        void MainWindow_SizeChanged_03(object sender, SizeChangedEventArgs e)
        {
            if (isInSizeChangedEvent) { e.Handled = true; isInSizeChangedEvent = false; return; }
            Size inside = new Size();
            Size aspectKeeping = new Size();
            Point delta = new Point();
            aspectRatio = userFeedbackImageUserControl.ActualWidth / userFeedbackImageUserControl.ActualHeight;
            inside.Width = Math.Max(1, Width - marginOfTargetUIElementFromWindow);
            inside.Height = Math.Max(1, Height - marginOfTargetUIElementFromWindow);
            aspectKeeping.Width = inside.Height * aspectRatio + marginOfTargetUIElementFromWindow;
            aspectKeeping.Height = inside.Width / aspectRatio + marginOfTargetUIElementFromWindow;
            delta.X = Width - userFeedbackImageUserControl.ActualWidth;
            delta.Y = Height - userFeedbackImageUserControl.ActualHeight;
            delta.X = e.NewSize.Width - e.PreviousSize.Width;
            delta.Y = e.NewSize.Height - e.PreviousSize.Height;
            if (delta.X != 0 && delta.Y != 0)
            {
                if (Width > aspectKeeping.Width) { isInSizeChangedEvent = true; Width = aspectKeeping.Width; }
                else if (Height > aspectKeeping.Height) { isInSizeChangedEvent = true; Height = aspectKeeping.Height; }
            }
            else if (delta.X != 0) { isInSizeChangedEvent = true; Height = aspectKeeping.Height; }
            else if (delta.Y != 0) { isInSizeChangedEvent = true; Width = aspectKeeping.Width; }
        }

        void UserFeedbackImageWindowResizeEventDelayTimer_01()
        {
            resizeDelayTimer.Stop();
            if (isInSizeChangedEvent) { isInSizeChangedEvent = false; return; }
            Size inside = new Size();
            Size aspectKeeping = new Size();
            Point delta = new Point();

            aspectRatio = userFeedbackImageUserControl.ActualWidth / userFeedbackImageUserControl.ActualHeight;
            inside.Width = Width - marginOfTargetUIElementFromWindow;
            inside.Height = Height - marginOfTargetUIElementFromWindow;
            aspectKeeping.Width = inside.Height * aspectRatio + marginOfTargetUIElementFromWindow;
            aspectKeeping.Height = inside.Width / aspectRatio + marginOfTargetUIElementFromWindow;

            if (false)
            {
                delta.X = Width - userFeedbackImageUserControl.ActualWidth;
                delta.Y = Height - userFeedbackImageUserControl.ActualHeight;
            }
            else
            {
                delta.X = Width - previousWindowSize.Width;
                delta.Y = Height - previousWindowSize.Height;
            }
            if (delta.X != 0 && delta.Y != 0)
            {
                if (Width > aspectKeeping.Width) { isInSizeChangedEvent = true; Width = aspectKeeping.Width; }
                else if (Height > aspectKeeping.Height) { isInSizeChangedEvent = true; Height = aspectKeeping.Height; }
            }
            else if (delta.X != 0) { isInSizeChangedEvent = true; Height = aspectKeeping.Height; }
            else if (delta.Y != 0) { isInSizeChangedEvent = true; Width = aspectKeeping.Width; }
            previousWindowSize.Width = Width;
            previousWindowSize.Height = Height;
        }

        void MainWindow_SizeChanged_02(object sender, SizeChangedEventArgs e)
        {
            // 自前コードと似たような挙動。
            if (e.WidthChanged && e.HeightChanged)
                this.SizeToContent = SizeToContent.Manual;
            else if (e.WidthChanged)
                this.SizeToContent = SizeToContent.Height;
            else if (e.HeightChanged)
                this.SizeToContent = SizeToContent.Width;
        }

        void MainWindow_SizeChanged_01(object sender, SizeChangedEventArgs e)
        {
            if (isInSizeChangedEvent) { isInSizeChangedEvent = false; return; }
            this.SizeToContent = SizeToContent.WidthAndHeight;
            isInSizeChangedEvent = true;
            this.SizeToContent = SizeToContent.Manual;
        }

        void MainWindow_SizeChanged_00(object sender, SizeChangedEventArgs e)
        {
            // 結局これもダメ。
            //if (isInSizeChangedEvent) { isInSizeChangedEvent = false; e.Handled = true; return; }
            double insideWidth = Width - marginOfTargetUIElementFromWindow;
            double insideHeight = Height - marginOfTargetUIElementFromWindow;
            //double aspectRatio = insideWidth / insideHeight;
            double aspectKeepingWindowWidth = insideHeight * aspectRatio + marginOfTargetUIElementFromWindow;
            double aspectKeepingWindowHeight = insideWidth / aspectRatio + marginOfTargetUIElementFromWindow;
            double deltaWidth = e.NewSize.Width - e.PreviousSize.Width;
            double deltaHeight = e.NewSize.Height - e.PreviousSize.Height;
            double aspectKeptDeltaWidth = deltaHeight * aspectRatio;
            double aspectKeptDeltaHeight = deltaWidth / aspectRatio;
            if (e.WidthChanged && e.HeightChanged)
            {
                // NOTE: (deltaWidth > 0) || (deltaHeight > 0)のとき、ユーザーはEnlargeしようとしていると考える。
                if (deltaWidth > 0 && deltaHeight > 0)
                {
                    Debug.WriteLine("deltaWidth > 0 && deltaHeight > 0");
                    if (deltaWidth > aspectKeptDeltaWidth) { isInSizeChangedEvent = true; Height = aspectKeepingWindowHeight; }
                    else if (deltaHeight > aspectKeptDeltaHeight) { isInSizeChangedEvent = true; Width = aspectKeepingWindowWidth; }
                }
                else if (deltaWidth > 0 && deltaHeight < 0)
                {
                    Debug.WriteLine("deltaWidth > 0 && deltaHeight < 0");
                    if (Height < aspectKeepingWindowHeight)
                    {
                        isInSizeChangedEvent = true; Height = aspectKeepingWindowHeight;
                        Debug.WriteLine("New Height=" + aspectKeepingWindowHeight);
                    }
                }
                else if (deltaWidth < 0 && deltaHeight > 0)
                {
                    Debug.WriteLine("deltaWidth < 0 && deltaHeight > 0");
                    if (Width < aspectKeepingWindowWidth)
                    {
                        isInSizeChangedEvent = true; Width = aspectKeepingWindowWidth;
                        Debug.WriteLine("New Width=" + aspectKeepingWindowWidth);
                    }
                }
                else if (deltaWidth < 0 && deltaHeight < 0)
                {
                    Debug.WriteLine("deltaWidth < 0 && deltaHeight < 0");
                    if (-deltaWidth > -aspectKeptDeltaWidth) { isInSizeChangedEvent = true; Height = aspectKeepingWindowHeight; }
                    else if (-deltaHeight > -aspectKeptDeltaHeight) { isInSizeChangedEvent = true; Width = aspectKeepingWindowWidth; }
                }
            }
            else if (e.WidthChanged)
            {
                Debug.WriteLine(string.Format("e.WidthChanged: Height={0}, aspectKeepingWindowHeight={1}", Height, aspectKeepingWindowHeight));
                isInSizeChangedEvent = true; Height = aspectKeepingWindowHeight;
                Debug.WriteLine("New Height=" + aspectKeepingWindowHeight);
                if (Height > aspectKeepingWindowHeight) { }
            }
            else if (e.HeightChanged)
            {
                Debug.WriteLine(string.Format("e.HeightChanged: Width={0}, aspectKeepingWindowWidth={1}", Width, aspectKeepingWindowWidth));
                isInSizeChangedEvent = true; Width = aspectKeepingWindowWidth;
                Debug.WriteLine("New Width=" + aspectKeepingWindowWidth);
                if (Width < aspectKeepingWindowWidth) { }
                //e.Handled = true;
            }
            //if (e.PreviousSize.IsEmpty == false) { e.Handled = isInSizeChangedEvent; }
        }


        #region failed
        double _fixRate;
        double _horizontalMargin;
        double _verticalMargin;

        protected override void OnSourceInitialized(EventArgs e)
        {
            var hwndSource = (HwndSource)HwndSource.FromVisual(this);
            hwndSource.AddHook(WndHookProc);
        }

        public void SetContent(FrameworkElement content)
        {
            targetWindowSizeUIElement.Child = content;
            targetWindowSizeUIElement.Width = content.Width;
            targetWindowSizeUIElement.Height = content.Height;
        }

        IntPtr WndHookProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            const int WM_SIZING = 0x214;
            const int WMSZ_LEFT = 1;
            const int WMSZ_RIGHT = 2;
            const int WMSZ_TOP = 3;
            const int WMSZ_TOPLEFT = 4;
            const int WMSZ_TOPRIGHT = 5;
            const int WMSZ_BOTTOM = 6;
            const int WMSZ_BOTTOMLEFT = 7;
            const int WMSZ_BOTTOMRIGHT = 8;

            if (msg == WM_SIZING)
            {
                var rect = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                var w = rect.right - rect.left - this._horizontalMargin;
                var h = rect.bottom - rect.top - this._verticalMargin;

                switch (wParam.ToInt32())
                {
                    case WMSZ_LEFT:
                    case WMSZ_RIGHT:
                        rect.bottom = (int)(rect.top + w / this._fixRate + this._verticalMargin);
                        break;
                    case WMSZ_TOP:
                    case WMSZ_BOTTOM:
                        rect.right = (int)(rect.left + h * this._fixRate + this._horizontalMargin);
                        break;
                    case WMSZ_TOPLEFT:
                        if (w / h > this._fixRate)
                        {
                            rect.top = (int)(rect.bottom - w / this._fixRate - this._verticalMargin);
                        }
                        else
                        {
                            rect.left = (int)(rect.right - h * this._fixRate - this._horizontalMargin);
                        }
                        break;
                    case WMSZ_TOPRIGHT:
                        if (w / h > this._fixRate)
                        {
                            rect.top = (int)(rect.bottom - w / this._fixRate - this._verticalMargin);
                        }
                        else
                        {
                            rect.right = (int)(rect.left + h * this._fixRate + this._horizontalMargin);
                        }
                        break;
                    case WMSZ_BOTTOMLEFT:
                        if (w / h > this._fixRate)
                        {
                            rect.bottom = (int)(rect.top + w / this._fixRate + this._verticalMargin);
                        }
                        else
                        {
                            rect.left = (int)(rect.right - h * this._fixRate - this._horizontalMargin);
                        }
                        break;
                    case WMSZ_BOTTOMRIGHT:
                        if (w / h > this._fixRate)
                        {
                            rect.bottom = (int)(rect.top + w / this._fixRate + this._verticalMargin);
                        }
                        else
                        {
                            rect.right = (int)(rect.left + h * this._fixRate + this._horizontalMargin);
                        }
                        break;
                    default:
                        break;
                }
                Marshal.StructureToPtr(rect, lParam, true);
            }
            return IntPtr.Zero;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct RECT
        {
            public int left;
            public int top;
            public int right;
            public int bottom;
        }
        #endregion

        #endregion // Old Code
#endif
    }
}

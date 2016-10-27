namespace DotNetUtility.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Input;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    public enum AspectRatioKeepingWindowDraggingRegions
    {
        None,
        Left,
        Right,
        Top,
        Bottom,
        LeftTop,
        RightTop,
        LeftBottom,
        RightBottom,
        Center,
    }

    public class AspectRatioKeepingWindowResize
    {
        public Thickness MarginBetweenWindowAndContent { get; set; }
        double _ContentAspectRatio;
        public double ContentAspectRatio
        {
            get { return _ContentAspectRatio; }
            set
            {
                if (value <= 0) { throw new ArgumentOutOfRangeException("value", value, "ContentAspectRatio must be larger than 0."); }
                _ContentAspectRatio = value;
            }
        }
        public Thickness GripEdgeThickness { get; set; }

        public Window AssociatedObject { get; private set; }

        bool _IsResizing;
        public event EventHandler IsResizingChanged;
        protected virtual void OnIsResizingChanged(EventArgs e) { var t = IsResizingChanged; if (t != null) { t(this, e); } }
        public bool IsResizing
        {
            get { return _IsResizing; }
            private set
            {
                if (_IsResizing != value)
                {
                    _IsResizing = value;
                    OnIsResizingChanged(EventArgs.Empty);
                }
            }
        }
        AspectRatioKeepingWindowDraggingRegions _DraggingRegion;
        public event EventHandler DraggingRegionChanged;
        protected virtual void OnDraggingRegionChanged(EventArgs e) { var t = DraggingRegionChanged; if (t != null) { t(this, e); } }
        public AspectRatioKeepingWindowDraggingRegions DraggingRegion
        {
            get { return _DraggingRegion; }
            set
            {
                _DraggingRegion = value;
                IsResizing = (_DraggingRegion != AspectRatioKeepingWindowDraggingRegions.None) && (_DraggingRegion != AspectRatioKeepingWindowDraggingRegions.Center);
                OnDraggingRegionChanged(EventArgs.Empty);
            }
        }
        Rect windowRectWhenResizeStarted { get; set; }
        Thickness cursorPositionMarginInWindowWhenResizeStarted { get; set; }
        public bool IsRenderSuspendedInUpdatingWindowRect { get; private set; }

        static bool isToShowDebugMessages = false;
        static void DebugWriteLine(string debugMessage) { if (isToShowDebugMessages) { Debug.WriteLine(debugMessage); } }



        public AspectRatioKeepingWindowResize(Window associatedObject)
        {
            Trace.Assert(associatedObject != null);

            AssociatedObject = associatedObject;

            MarginBetweenWindowAndContent = new Thickness();
            ContentAspectRatio = 1.0;
            GripEdgeThickness = new Thickness();

            _IsResizing = false;
            _DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None;
            IsRenderSuspendedInUpdatingWindowRect = false;
        }

        public void OnMouseDownOrTouchOnWindow(int dpiScaledCursorX, int dpiScaledCursorY)
        {
            var resizeStartedCursorPositionInWindow = new Point(dpiScaledCursorX - AssociatedObject.Left, dpiScaledCursorY - AssociatedObject.Top);

            var isLeft = resizeStartedCursorPositionInWindow.X < GripEdgeThickness.Left;
            var isTop = resizeStartedCursorPositionInWindow.Y < GripEdgeThickness.Top;
            var isRight = resizeStartedCursorPositionInWindow.X > (AssociatedObject.Width - GripEdgeThickness.Right);
            var isBottom = resizeStartedCursorPositionInWindow.Y > (AssociatedObject.Height - GripEdgeThickness.Bottom);
            if (isLeft)
            {
                if (isBottom) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.LeftBottom; }
                else if (isTop) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.LeftTop; }
                else { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.Left; }
            }
            else if (isRight)
            {
                if (isBottom) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.RightBottom; }
                else if (isTop) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.RightTop; }
                else { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.Right; }
            }
            else if (isTop) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.Top; }
            else if (isBottom) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.Bottom; }
            else { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.Center; }

            DebugWriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "X: {0:G6}  Y:{1:G6}  XDraggingRegion: {2}", resizeStartedCursorPositionInWindow.X, resizeStartedCursorPositionInWindow.Y, DraggingRegion));
            if (DraggingRegion != AspectRatioKeepingWindowDraggingRegions.Center)
            {
                windowRectWhenResizeStarted = new Rect(AssociatedObject.Left, AssociatedObject.Top, AssociatedObject.Width, AssociatedObject.Height);
                cursorPositionMarginInWindowWhenResizeStarted = new Thickness(resizeStartedCursorPositionInWindow.X, resizeStartedCursorPositionInWindow.Y, AssociatedObject.Width - resizeStartedCursorPositionInWindow.X, AssociatedObject.Height - resizeStartedCursorPositionInWindow.Y);
                DebugWriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "  windowRectWhenResizeStarted: {0}", windowRectWhenResizeStarted));
                DebugWriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "  cursorPositionMarginInWindowWhenResizeStarted: {0}", cursorPositionMarginInWindowWhenResizeStarted));
            }
        }

        public void ResizeWithKeepingContentAspectRatio(int dpiScaledCursorX, int dpiScaledCursorY)
        {
            if (IsResizing == false) { return; }
            DebugWriteLine("dpiScaledCursor: " + dpiScaledCursorX + ", " + dpiScaledCursorY);
            switch (DraggingRegion)
            {
                case AspectRatioKeepingWindowDraggingRegions.Left:
                    Left = dpiScaledCursorX - cursorPositionMarginInWindowWhenResizeStarted.Left;
                    break;
                case AspectRatioKeepingWindowDraggingRegions.Top:
                    Top = dpiScaledCursorY - cursorPositionMarginInWindowWhenResizeStarted.Top;
                    break;
                case AspectRatioKeepingWindowDraggingRegions.Right:
                    Right = dpiScaledCursorX + cursorPositionMarginInWindowWhenResizeStarted.Right;
                    break;
                case AspectRatioKeepingWindowDraggingRegions.Bottom:
                    Bottom = dpiScaledCursorY + cursorPositionMarginInWindowWhenResizeStarted.Bottom;
                    break;
                case AspectRatioKeepingWindowDraggingRegions.LeftTop:
                    LeftTop = new Point(dpiScaledCursorX - cursorPositionMarginInWindowWhenResizeStarted.Left, dpiScaledCursorY - cursorPositionMarginInWindowWhenResizeStarted.Top);
                    break;
                case AspectRatioKeepingWindowDraggingRegions.RightTop:
                    RightTop = new Point(dpiScaledCursorX + cursorPositionMarginInWindowWhenResizeStarted.Right, dpiScaledCursorY - cursorPositionMarginInWindowWhenResizeStarted.Top);
                    break;
                case AspectRatioKeepingWindowDraggingRegions.LeftBottom:
                    LeftBottom = new Point(dpiScaledCursorX - cursorPositionMarginInWindowWhenResizeStarted.Left, dpiScaledCursorY + cursorPositionMarginInWindowWhenResizeStarted.Bottom);
                    break;
                case AspectRatioKeepingWindowDraggingRegions.RightBottom:
                    RightBottom = new Point(dpiScaledCursorX + cursorPositionMarginInWindowWhenResizeStarted.Right, dpiScaledCursorY + cursorPositionMarginInWindowWhenResizeStarted.Bottom);
                    break;
                case AspectRatioKeepingWindowDraggingRegions.None:
                case AspectRatioKeepingWindowDraggingRegions.Center:
                default:
#if DEBUG
                    Debugger.Break();
#endif
                    throw new NotImplementedException("Invalid DraggingRegion.");
            }
        }

        // TODO: より良い仕様はあるとは思われるが、現状はこれでOKとする。
        public System.Drawing.Rectangle CurrentWorkingArea
        {
            get
            {
                var hWnd = new System.Windows.Interop.WindowInteropHelper(AssociatedObject).Handle;
                return System.Windows.Forms.Screen.FromHandle(hWnd).WorkingArea;
            }
        }
        public bool ValidateWindowLeft(double value)
        {
            var cwa = CurrentWorkingArea;
            if (value < cwa.Left) { return false; }
            if (value > cwa.Right) { return false; }
            if (value > Right - (GripEdgeThickness.Left + GripEdgeThickness.Right)) { return false; }
            return true;
        }
        public bool ValidateWindowTop(double value)
        {
            var cwa = CurrentWorkingArea;
            if (value < cwa.Top) { return false; }
            if (value > cwa.Bottom) { return false; }
            if (value > Bottom - (GripEdgeThickness.Top + GripEdgeThickness.Bottom)) { return false; }
            return true;
        }
        public bool ValidateWindowRight(double value)
        {
            var cwa = CurrentWorkingArea;
            if (value < cwa.Left) { return false; }
            if (value > cwa.Right) { return false; }
            if (value < Left + (GripEdgeThickness.Left + GripEdgeThickness.Right)) { return false; }
            return true;
        }
        public bool ValidateWindowBottom(double value)
        {
            var cwa = CurrentWorkingArea;
            if (value < cwa.Top) { return false; }
            if (value > cwa.Bottom) { return false; }
            if (value < Top + (GripEdgeThickness.Top + GripEdgeThickness.Bottom)) { return false; }
            return true;
        }
        bool ValidateWindowWidth(double value)
        {
            if (value < GripEdgeThickness.Left + GripEdgeThickness.Right) { return false; }
            if (value < AssociatedObject.MinWidth) { return false; }
            if (value > AssociatedObject.MaxWidth) { return false; }
            if (value > CurrentWorkingArea.Width) { return false; }
            return true;
        }
        bool ValidateWindowHeight(double value)
        {
            if (value < GripEdgeThickness.Top + GripEdgeThickness.Bottom) { return false; }
            if (value < AssociatedObject.MinHeight) { return false; }
            if (value > AssociatedObject.MaxHeight) { return false; }
            if (value > CurrentWorkingArea.Height) { return false; }
            return true;
        }
        double GetNewWindowHeight(double newWindowWidth)
        {
            var contentWidth = newWindowWidth - (MarginBetweenWindowAndContent.Left + MarginBetweenWindowAndContent.Right);
            var contentHeight = contentWidth / ContentAspectRatio;
            var ret = contentHeight + (MarginBetweenWindowAndContent.Top + MarginBetweenWindowAndContent.Bottom);
            return ret;
        }
        double GetNewWindowWidth(double newWindowHeight)
        {
            var contentHeight = newWindowHeight - (MarginBetweenWindowAndContent.Top + MarginBetweenWindowAndContent.Bottom);
            var contentWidth = contentHeight * ContentAspectRatio;
            var ret = contentWidth + (MarginBetweenWindowAndContent.Left + MarginBetweenWindowAndContent.Right);
            return ret;
        }
        double GetNewWindowTop(double newWindowTop, double newWindowHeight)
        {
            var ret = newWindowTop;
            var cwa = CurrentWorkingArea;
            ret = Math.Max(cwa.Top, ret);
            var newWindowBottom = ret + newWindowHeight;
            if (newWindowBottom > cwa.Bottom) { ret -= newWindowBottom - cwa.Bottom; }
            return ret;
        }
        double GetNewWindowLeft(double newWindowLeft, double newWindowWidth)
        {
            var ret = newWindowLeft;
            var cwa = CurrentWorkingArea;
            ret = Math.Max(cwa.Left, ret);
            var newWindowRight = ret + newWindowWidth;
            if (newWindowRight > cwa.Right) { ret -= newWindowRight - cwa.Right; }
            return ret;
        }
        bool UpdateWindowTopBottomAfterWindowLeftOrRightUpdated(double newWindowLeft, double newWindowWidth)
        {
            if (ValidateWindowWidth(newWindowWidth) == false) { return false; }
            var newWindowHeight = GetNewWindowHeight(newWindowWidth);
            if (ValidateWindowHeight(newWindowHeight) == false) { return false; }
            var newWindowTop = GetNewWindowTop(CenterY - newWindowHeight / 2.0, newWindowHeight);
            UpdateAssociatedObjectRect(newWindowLeft, newWindowTop, newWindowWidth, newWindowHeight);
            return true;
        }
        bool UpdateWindowLeftRightAfterWindowTopOrBottomUpdated(double newWindowTop, double newWindowHeight)
        {
            if (ValidateWindowHeight(newWindowHeight) == false) { return false; }
            var newWindowWidth = GetNewWindowWidth(newWindowHeight);
            if (ValidateWindowWidth(newWindowWidth) == false) { return false; }
            var newWindowLeft = GetNewWindowLeft(CenterX - newWindowWidth / 2.0, newWindowWidth);
            UpdateAssociatedObjectRect(newWindowLeft, newWindowTop, newWindowWidth, newWindowHeight);
            return true;
        }

        string GetAssociatedObjectRectString()
        {
            var ret = string.Format(System.Globalization.CultureInfo.InvariantCulture, "Left:{0,4}  Top:{1,4}  Width:{2,4}  Height:{3,4}", (int)AssociatedObject.Left, (int)AssociatedObject.Top, (int)AssociatedObject.Width, (int)AssociatedObject.Height);
            return ret;
        }

        // MUSTDO: 要修正！DPIが100以外の場合に、まともに動作しない！
        void UpdateAssociatedObjectRect(double left, double top, double width, double height)
        {
            if (IsRenderSuspendedInUpdatingWindowRect) { return; }

            // NOTE: MoveWindowを使うようになってから、単にエッジをクリックだけをしたときでも、TopとLeftが1ピクセルずつ移動してしまう問題が起こった。それを回避する。
            int diffAbsSum = Math.Abs((int)AssociatedObject.Left - (int)left) + Math.Abs((int)AssociatedObject.Top - (int)top) + Math.Abs((int)AssociatedObject.Width - (int)width) + Math.Abs((int)AssociatedObject.Height - (int)height);
            if (diffAbsSum <= 1) { return; }
            DebugWriteLine("[Before] " + GetAssociatedObjectRectString());
            var associatedObjectHWnd = new System.Windows.Interop.WindowInteropHelper(AssociatedObject).Handle;

            // NOTE: 物理ピクセルにもとづいて値が変わってしまう！！
            if (false) { Win32.NativeMethods.MoveWindow(associatedObjectHWnd, (int)left, (int)top, (int)width, (int)height, true); }
            // NOTE: Windowの状態が変わってしまうおそれがある。
            if (false) { Win32.NativeMethods.SetWindowPos(associatedObjectHWnd, IntPtr.Zero, (int)left, (int)top, (int)width, (int)height, 0); }
            // NOTE: オーバーヘッドが大きいのか、ちらつきが目立つが、DPIでスケールされた論理ピクセルに基づいて値が設定できるし、これで十分です！！
            IsRenderSuspendedInUpdatingWindowRect = true;
            AssociatedObject.Dispatcher.Invoke(new Action(() =>
            {
                AssociatedObject.Left = (int)left;
                AssociatedObject.Top = (int)top;
                AssociatedObject.Width = (int)width;
                AssociatedObject.Height = (int)height;
                AssociatedObject.InvalidateVisual();
            }));
            IsRenderSuspendedInUpdatingWindowRect = false;
            DebugWriteLine("[ After] " + GetAssociatedObjectRectString());
        }

        public double CenterX
        {
            get { return IsResizing ? (windowRectWhenResizeStarted.Left + windowRectWhenResizeStarted.Width / 2.0) : (AssociatedObject.Left + AssociatedObject.Width / 2.0); }
        }
        public double CenterY
        {
            get { return IsResizing ? (windowRectWhenResizeStarted.Top + windowRectWhenResizeStarted.Height / 2.0) : (AssociatedObject.Top + AssociatedObject.Height / 2.0); }
        }
        public double Left
        {
            get { return IsResizing ? windowRectWhenResizeStarted.Left : AssociatedObject.Left; }
            set
            {
                if (ValidateWindowLeft(value) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                var newWindowWidth = Right - value;
                if (UpdateWindowTopBottomAfterWindowLeftOrRightUpdated(value, newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
            }
        }
        public double Top
        {
            get { return IsResizing ? windowRectWhenResizeStarted.Top : AssociatedObject.Top; }
            set
            {
                if (ValidateWindowTop(value) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                var newWindowHeight = Bottom - value;
                if (UpdateWindowLeftRightAfterWindowTopOrBottomUpdated(value, newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
            }
        }
        public double Right
        {
            get { return IsResizing ? windowRectWhenResizeStarted.Right : (AssociatedObject.Left + AssociatedObject.Width); }
            set
            {
                if (ValidateWindowRight(value) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                var newWindowWidth = value - Left;
                if (UpdateWindowTopBottomAfterWindowLeftOrRightUpdated(Left, newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
            }
        }
        public double Bottom
        {
            get { return IsResizing ? windowRectWhenResizeStarted.Bottom : (AssociatedObject.Top + AssociatedObject.Height); }
            set
            {
                if (ValidateWindowBottom(value) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                var newWindowHeight = value - Top;
                if (UpdateWindowLeftRightAfterWindowTopOrBottomUpdated(Top, newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
            }
        }

        public double ContentLeft { get { return Left + MarginBetweenWindowAndContent.Left; } }
        public double ContentTop { get { return Top + MarginBetweenWindowAndContent.Top; } }
        public double ContentRight { get { return Right - MarginBetweenWindowAndContent.Right; } }
        public double ContentBottom { get { return Bottom - MarginBetweenWindowAndContent.Bottom; } }
        public Point LeftTop
        {
            get { return new Point(Left, Top); }
            set
            {
                if (((value.Y + MarginBetweenWindowAndContent.Top) - ContentBottom)
                    >= (1.0 / ContentAspectRatio) * ((value.X + MarginBetweenWindowAndContent.Left) - ContentRight))
                {
                    // point is in left area
                    if (ValidateWindowLeft(value.X) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = Right - value.X;
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = GetNewWindowHeight(newWindowWidth);
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowTop = GetNewWindowTop(Bottom - newWindowHeight, newWindowHeight);
                    UpdateAssociatedObjectRect(value.X, newWindowTop, newWindowWidth, newWindowHeight);
                }
                else
                {
                    // point is in top area
                    if (ValidateWindowTop(value.Y) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = Bottom - value.Y;
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = GetNewWindowWidth(newWindowHeight);
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowLeft = GetNewWindowLeft(Right - newWindowWidth, newWindowWidth);
                    UpdateAssociatedObjectRect(newWindowLeft, value.Y, newWindowWidth, newWindowHeight);
                }
            }
        }
        public Point RightTop
        {
            get { return new Point(Right, Top); }
            set
            {
                if (((value.Y + MarginBetweenWindowAndContent.Top) - ContentBottom)
                    >= -(1.0 / ContentAspectRatio) * ((value.X - MarginBetweenWindowAndContent.Right) - ContentLeft))
                {
                    // point is in right area
                    if (ValidateWindowRight(value.X) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = value.X - Left;
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = GetNewWindowHeight(newWindowWidth);
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowTop = GetNewWindowTop(Bottom - newWindowHeight, newWindowHeight);
                    UpdateAssociatedObjectRect(value.X - newWindowWidth, newWindowTop, newWindowWidth, newWindowHeight);
                }
                else
                {
                    // point is in top area
                    if (ValidateWindowTop(value.Y) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = Bottom - value.Y;
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = GetNewWindowWidth(newWindowHeight);
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowLeft = GetNewWindowLeft(Left, newWindowWidth);
                    UpdateAssociatedObjectRect(newWindowLeft, value.Y, newWindowWidth, newWindowHeight);
                }
            }
        }
        public Point LeftBottom
        {
            get { return new Point(Left, Bottom); }
            set
            {
                if (((value.Y - MarginBetweenWindowAndContent.Bottom) - ContentTop)
                    >= -(1.0 / ContentAspectRatio) * ((value.X + MarginBetweenWindowAndContent.Left) - ContentRight))
                {
                    // point is in bottom area
                    if (ValidateWindowBottom(value.Y) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = value.Y - Top;
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = GetNewWindowWidth(newWindowHeight);
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowLeft = GetNewWindowLeft(Right - newWindowWidth, newWindowWidth);
                    UpdateAssociatedObjectRect(newWindowLeft, value.Y - newWindowHeight, newWindowWidth, newWindowHeight);
                }
                else
                {
                    // point is in left area
                    if (ValidateWindowLeft(value.X) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = Right - value.X;
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = GetNewWindowHeight(newWindowWidth);
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowTop = GetNewWindowTop(Top, newWindowHeight);
                    UpdateAssociatedObjectRect(value.X, newWindowTop, newWindowWidth, newWindowHeight);
                }
            }
        }
        public Point RightBottom
        {
            get { return new Point(Right, Bottom); }
            set
            {
                if (((value.Y - MarginBetweenWindowAndContent.Bottom) - ContentTop)
                    >= (1.0 / ContentAspectRatio) * ((value.X - MarginBetweenWindowAndContent.Right) - ContentLeft))
                {
                    // cursor is in bottom area
                    if (ValidateWindowBottom(value.Y) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = value.Y - Top;
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = GetNewWindowWidth(newWindowHeight);
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowLeft = GetNewWindowLeft(Left, newWindowWidth);
                    UpdateAssociatedObjectRect(newWindowLeft, Top, newWindowWidth, newWindowHeight);
                }
                else
                {
                    // cursor is in right area
                    if (ValidateWindowRight(value.X) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowWidth = value.X - Left;
                    if (ValidateWindowWidth(newWindowWidth) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowHeight = GetNewWindowHeight(newWindowWidth);
                    if (ValidateWindowHeight(newWindowHeight) == false) { DraggingRegion = AspectRatioKeepingWindowDraggingRegions.None; return; }
                    var newWindowTop = GetNewWindowTop(Top, newWindowHeight);
                    UpdateAssociatedObjectRect(Left, newWindowTop, newWindowWidth, newWindowHeight);
                }
            }
        }
    }
}

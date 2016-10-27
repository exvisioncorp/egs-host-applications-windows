// NOTE: impossible: include file = "../../DotNetUtility/" + "DuplicatedProcessStartBlocking.cs"
namespace Egs
{
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

namespace DotNetUtility.Views
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Data;
    using System.Globalization;

    public class BooleanConverter<T> : IValueConverter
    {
        public BooleanConverter(T trueValue, T falseValue)
        {
            True = trueValue;
            False = falseValue;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool && ((bool)value) ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T && EqualityComparer<T>.Default.Equals((T)value, True);
        }
    }

    /// <summary>
    /// Convert true to Visibility.Collapsed and false to Visibility.Visible
    /// </summary>
    public sealed class TrueToCollapsedFalseToVisibleConverter : BooleanConverter<Visibility>
    {
        public TrueToCollapsedFalseToVisibleConverter() : base(Visibility.Collapsed, Visibility.Visible) { }
    }

    public class BoolToOppositeBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (targetType != typeof(bool)) { throw new InvalidOperationException("The target must be a bool"); }
            return !(bool)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }

    public class EnumToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null) { return System.Windows.DependencyProperty.UnsetValue; }
            if (Enum.IsDefined(value.GetType(), value) == false) { return System.Windows.DependencyProperty.UnsetValue; }
            object parameterValue = Enum.Parse(value.GetType(), parameterString);
            return parameterValue.Equals(value);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null || value.Equals(false)) { return System.Windows.DependencyProperty.UnsetValue; }
            return Enum.Parse(targetType, parameterString);
        }
    }

    internal class EnumToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null) { return System.Windows.DependencyProperty.UnsetValue; }
            if (Enum.IsDefined(value.GetType(), value) == false) { return System.Windows.DependencyProperty.UnsetValue; }
            object parameterValue = Enum.Parse(value.GetType(), parameterString);
            return (parameterValue.Equals(value)) ? System.Windows.Visibility.Visible : System.Windows.Visibility.Collapsed;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            string parameterString = parameter as string;
            if (parameterString == null || value.Equals(System.Windows.Visibility.Collapsed) || value.Equals(System.Windows.Visibility.Hidden)) { return System.Windows.DependencyProperty.UnsetValue; }
            return Enum.Parse(targetType, parameterString);
        }
    }

    public class Int32IndexToBooleanConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            int integer = (int)value;
            if (integer == int.Parse(parameter.ToString(), System.Globalization.CultureInfo.InvariantCulture))
                return true;
            else
                return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return parameter;
        }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Net;
    using System.Net.Cache;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Windows;

    public class ApplicationInstallerInformation
    {
        public string Title { get; set; }
        public string Version { get; set; }
        public System.Uri DownloadSiteUrl { get; set; }
        public System.Uri InstallerUrl { get; set; }
        public System.Uri ChangeLogUrl { get; set; }
    }

    public sealed class ApplicationUpdateModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }

        public Uri InformationFileUri { get; private set; }
        public ApplicationInstallerInformation LatestInstallerInformation { get; private set; }
        public int InstallerFileDownloadingProgressPercentage { get; private set; }
        public string DownloadedInstallerFileFullPath { get; private set; }
        public WebClient DownloadWebClient { get; private set; }

        //public SimpleDelegateCommand CancelCommand { get; private set; }
        //public event EventHandler DownloadCanceled;

        public ApplicationUpdateModel(Uri informationFileUri)
        {
            InformationFileUri = informationFileUri;
            //CancelCommand = new SimpleDelegateCommand();
            //CancelCommand.CanPerform = false;
            //CancelCommand.PerformEventHandler += delegate { if (DownloadWebClient != null) { DownloadWebClient.CancelAsync(); } };
        }

        static string GetFileName(Uri uri, string httpWebRequestMethod = "HEAD")
        {
            try
            {
                var fileName = string.Empty;
                if (uri.Scheme.Equals(Uri.UriSchemeHttp) || uri.Scheme.Equals(Uri.UriSchemeHttps))
                {
                    var httpWebRequest = (HttpWebRequest)WebRequest.Create(uri);
                    httpWebRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                    httpWebRequest.Method = httpWebRequestMethod;
                    httpWebRequest.AllowAutoRedirect = false;
                    var httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
                    if (httpWebResponse.StatusCode.Equals(HttpStatusCode.Redirect)
                        || httpWebResponse.StatusCode.Equals(HttpStatusCode.Moved)
                        || httpWebResponse.StatusCode.Equals(HttpStatusCode.MovedPermanently))
                    {
                        if (httpWebResponse.Headers["Location"] != null)
                        {
                            var location = httpWebResponse.Headers["Location"];
                            fileName = GetFileName(new Uri(location));
                            return fileName;
                        }
                    }
                    var contentDisposition = httpWebResponse.Headers["content-disposition"];
                    if (string.IsNullOrEmpty(contentDisposition) == false)
                    {
                        const string lookForFileName = "filename=";
                        var index = contentDisposition.IndexOf(lookForFileName, StringComparison.CurrentCultureIgnoreCase);
                        if (index >= 0)
                        {
                            fileName = contentDisposition.Substring(index + lookForFileName.Length);
                        }
                        if (fileName.StartsWith("\""))
                        {
                            fileName = fileName.Substring(1, fileName.Length - 2);
                        }
                        fileName = fileName.Split('\"')[0];
                    }
                }
                if (string.IsNullOrEmpty(fileName))
                {
                    fileName = System.IO.Path.GetFileName(uri.LocalPath);
                }
                return fileName;
            }
            catch (WebException)
            {
                return GetFileName(uri, "GET");
            }
        }

        public bool CheckInformationFile()
        {
            if (InformationFileUri == null) { return false; }

            try
            {
                var webRequest = WebRequest.Create(InformationFileUri);
                webRequest.CachePolicy = new HttpRequestCachePolicy(HttpRequestCacheLevel.NoCacheNoStore);
                var webResponse = webRequest.GetResponse();
                var appCastStream = webResponse.GetResponseStream();
                using (var reader = new System.IO.StreamReader(appCastStream))
                {
                    string jsonString = reader.ReadToEnd();
                    LatestInstallerInformation = Newtonsoft.Json.JsonConvert.DeserializeObject<ApplicationInstallerInformation>(jsonString);
                }
                OnPropertyChanged("LatestInstallerInformation");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public bool DownloadInstaller()
        {
            if (LatestInstallerInformation == null) { return false; }
            if (LatestInstallerInformation.InstallerUrl == null) { return false; }

            try
            {
                var tempFolderFullPath = System.IO.Path.GetTempPath();
                var downloadedFileName = GetFileName(LatestInstallerInformation.InstallerUrl);
                DownloadedInstallerFileFullPath = System.IO.Path.Combine(tempFolderFullPath, downloadedFileName);
                OnPropertyChanged("DownloadedInstallerFileFullPath");
                DownloadWebClient = new WebClient();
                DownloadWebClient.DownloadProgressChanged += (sender, e) =>
                {
                    InstallerFileDownloadingProgressPercentage = e.ProgressPercentage;
                    OnPropertyChanged("InstallerFileDownloadingProgressPercentage");
                };
                DownloadWebClient.DownloadFileCompleted += (sender, e) =>
                {
                    if (e.Cancelled)
                    {
                        //CancelCommand.CanPerform = false;
                        //var t = DownloadCanceled; if (t != null) { t(this, EventArgs.Empty); }
                        return;
                    }
                    StartSetup();
                };
                DownloadWebClient.DownloadFileAsync(LatestInstallerInformation.InstallerUrl, DownloadedInstallerFileFullPath);
                //CancelCommand.CanPerform = true;
                return true;
            }
            catch (Exception ex)
            {
                //CancelCommand.CanPerform = false;
                Debugger.Break();
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        void StartSetup()
        {
            Trace.Assert(string.IsNullOrEmpty(DownloadedInstallerFileFullPath) == false);

            var processStartInfo = new ProcessStartInfo { FileName = DownloadedInstallerFileFullPath, UseShellExecute = true };
            Process.Start(processStartInfo);

            // NOTE: 強制終了。
            // TODO: この方法で良いかの検討。
            Environment.Exit(0);
        }

        public bool OpenDownloadWebSite()
        {
            if (LatestInstallerInformation == null) { return false; }
            if (LatestInstallerInformation.DownloadSiteUrl == null) { return false; }
            if (string.IsNullOrEmpty(LatestInstallerInformation.DownloadSiteUrl.ToString())) { return false; }

            Process.Start("explorer.exe", "\"" + LatestInstallerInformation.DownloadSiteUrl + "\"");
            return true;
        }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Diagnostics;
    using System.IO;
    using System.Net;
    using System.ComponentModel;
    using System.Runtime.Serialization;

    public static class BitmapImageUtility
    {
        /// <summary>
        /// Load a bitmap image from a file more safely
        /// </summary>
        public static BitmapImage LoadBitmapImageFromFile(string filePath)
        {
            // NOTE) 
            // You have to be careful in loading some bitmap images from files in your WPF application.  This method provides the way.
            // In some conditions, These ways show the images on XAML editor, but the images do not appear in the runtime application!
            //  * <Image Source=""Images/MyImageFile.png"" />
            //  * <Image Source=""{Binding MyBitmapImage}"" /> and DataBinding
            //  * <BitmapImage x:Key=""MyBitmapImageKey"" UriSource=""Images/MyImageFile.png"" /> and <Image Source=""{StaticResource MyBitmapImageKey}"" />
            //  * <Image x:Name=""MyImage"" /> ... "MyImage.Source = new BitmapImage(new Uri("Images/MyImageFile.png", , UriKind.Relative));" in code-behind.
            if (File.Exists(filePath) == false) { return null; }
            var ret = new BitmapImage();
            using (var stream = File.OpenRead(filePath))
            {
                ret.BeginInit();
                ret.CacheOption = BitmapCacheOption.OnLoad;
                ret.StreamSource = stream;
                ret.EndInit();
            }
            return ret;
        }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    using System.IO;
    using System.Windows;
    using System.Windows.Input;
    using System.ComponentModel;
    using System.Diagnostics;

    public class SimpleDelegateCommand : ICommand//, INotifyPropertyChanged
    {
        //public event EventHandler CanExecuteChanged;
        public event EventHandler CanExecuteChanged
        {
            add { CommandManager.RequerySuggested += value; }
            remove { CommandManager.RequerySuggested -= value; }
        }
        public event EventHandler PerformEventHandler;
        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            var t = PropertyChanged;
            if (t != null) { t(this, new PropertyChangedEventArgs(propertyName)); }
        }
        bool _CanPerform = true;
        public bool CanPerform
        {
            get { return _CanPerform && (_IsPerforming == false); }
            set
            {
                _CanPerform = value;
                CommandManager.InvalidateRequerySuggested();
                //var t = CanExecuteChanged; if (t != null) { t(this, EventArgs.Empty); }
                //OnPropertyChanged("CanPerform");
                //OnPropertyChanged("CanExecute");
            }
        }
        bool _IsPerforming = false;
        public bool IsPerforming
        {
            get { return _IsPerforming; }
            protected set
            {
                _IsPerforming = value;
                CommandManager.InvalidateRequerySuggested();
                //var t = CanExecuteChanged; if (t != null) { t(this, EventArgs.Empty); }
                //OnPropertyChanged("IsPerforming");
                //OnPropertyChanged("CanPerform");
                //OnPropertyChanged("CanExecute");
            }
        }

        public bool CanExecute(object parameter)
        {
            return CanPerform;
        }
        public void Execute(object parameter)
        {
            IsPerforming = true;
            var t = PerformEventHandler;
            if (t != null) { t(this, EventArgs.Empty); }
            IsPerforming = false;
        }

        public SimpleDelegateCommand()
        {
        }

        public SimpleDelegateCommand(Action action)
        {
            PerformEventHandler += (sender, e) => action();
        }
    }
}

}

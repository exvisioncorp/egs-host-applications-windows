namespace Win32
{
    using System;
    using System.ComponentModel;
    using System.Runtime.InteropServices;

    /// <summary>
    /// Identifies dots per inch (dpi) type.  Refer to https://msdn.microsoft.com/en-us/library/windows/desktop/dn280511(v=vs.85).aspx
    /// </summary>
    public enum MonitorDpiType
    {
        /// <summary>
        /// MDT_Effective_DPI
        /// <para>Effective DPI that incorporates accessibility overrides and matches what Desktop Window Manage (DWM) uses to scale desktop applications.</para>
        /// </summary>
        EffectiveDpi = 0,
        /// <summary>
        /// MDT_Angular_DPI
        /// <para>DPI that ensures rendering at a compliant angular resolution on the screen, without incorporating accessibility overrides.</para>
        /// </summary>
        AngularDpi = 1,
        /// <summary>
        /// MDT_Raw_DPI
        /// <para>Linear DPI of the screen as measures on the screen itself.</para>
        /// </summary>
        RawDpi = 2,
        /// <summary>
        /// MDT_Default
        /// </summary>
        Default = EffectiveDpi,
    }

    internal static partial class NativeMethods
    {
        /// <summary>
        /// The GetDC function retrieves a handle to a device context (DC) for the client area of a specified window or for the entire screen.  
        /// You can use the returned handle in subsequent GDI functions to draw in the DC.  
        /// The device context is an opaque data structure, whose values are used internally by GDI.  
        /// The GetDCEx function is an extension to GetDC, which gives an application more control over how and whether clipping occurs in the client area.
        /// </summary>
        /// <param name="hWnd">
        /// hWnd [in] A handle to the window whose DC is to be retrieved.  If this value is NULL, GetDC retrieves the DC for the entire screen.
        /// </param>
        /// <returns>
        /// If the function succeeds, the return value is a handle to the DC for the specified window's client area.  
        /// If the function fails, the return value is NULL.
        /// </returns>
        [DllImport("user32.dll", ExactSpelling = true, SetLastError = true)]
        extern internal static IntPtr GetDC(IntPtr hWnd);

        [DllImport("gdi32.dll", ExactSpelling = true, SetLastError = true)]
        extern internal static int GetDeviceCaps(IntPtr hdc, int nIndex);

        [DllImport("user32.dll", ExactSpelling = true)]
        extern internal static int ReleaseDC(IntPtr hWnd, IntPtr hDC);

        internal enum GetWindowCmd : uint
        {
            GW_HWNDFIRST = 0,
            GW_HWNDLAST = 1,
            GW_HWNDNEXT = 2,
            GW_HWNDPREV = 3,
            GW_OWNER = 4,
            GW_CHILD = 5,
            GW_ENABLEDPOPUP = 6
        }
        [DllImport("user32.dll")]
        extern internal static IntPtr GetWindow(IntPtr hWnd, GetWindowCmd uCmd);

        /// <summary>
        /// Refer to https://msdn.microsoft.com/ja-jp/library/cc411206.aspx
        /// </summary>
        [Flags]
        internal enum SetWindowPosFlags : uint
        {
            // NOTE: not completed
            SWP_NOSIZE = 0x0001,
            SWP_NOMOVE = 0x0002,
            SWP_NOZORDER = 0x0004,
            SWP_NOREDRAW = 0x0008,
            SWP_NOACTIVATE = 0x0010,
            SWP_FRAMECHANGED = 0x0020,
            SWP_SHOWWINDOW = 0x0040,
            SWP_HIDEWINDOW = 0x0080,
            SWP_NOCOPYBITS = 0x0100,
            SWP_NOOWNERZORDER = 0x0200,
            SWP_NOSENDCHANGING = 0x0400,

            IgnoreResize = 0x0001,
            IgnoreMove = 0x0002,
            IgnoreZOrder = 0x0004,
            DoNotRedraw = 0x0008,
            DoNotActivate = 0x0010,
            DrawFrame = 0x0020,
            FrameChanged = 0x0020,
            ShowWindow = 0x0040,
            HideWindow = 0x0080,
            DoNotCopyBits = 0x0100,
            DoNotChangeOwnerZOrder = 0x0200,
            DoNotReposition = 0x0200,
            DoNotSendChangingEvent = 0x0400,
            DeferErase = 0x2000,
            SynchronousWindowPosition = 0x4000,
        }
        /// <summary>
        ///     Changes the size, position, and Z order of a child, pop-up, or top-level window. These windows are ordered
        ///     according to their appearance on the screen. The topmost window receives the highest rank and is the first window
        ///     in the Z order.
        ///     <para>See https://msdn.microsoft.com/en-us/library/windows/desktop/ms633545%28v=vs.85%29.aspx for more information.</para>
        /// </summary>
        /// <param name="hWnd">C++ ( hWnd [in]. Type: HWND )<br />A handle to the window.</param>
        /// <param name="hWndInsertAfter">
        ///     C++ ( hWndInsertAfter [in, optional]. Type: HWND )<br />A handle to the window to precede the positioned window in
        ///     the Z order. This parameter must be a window handle or one of the following values.
        ///     <list type="table">
        ///     <itemheader>
        ///         <term>HWND placement</term><description>Window to precede placement</description>
        ///     </itemheader>
        ///     <item>
        ///         <term>HWND_BOTTOM ((HWND)1)</term>
        ///         <description>
        ///         Places the window at the bottom of the Z order. If the hWnd parameter identifies a topmost
        ///         window, the window loses its topmost status and is placed at the bottom of all other windows.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>HWND_NOTOPMOST ((HWND)-2)</term>
        ///         <description>
        ///         Places the window above all non-topmost windows (that is, behind all topmost windows). This
        ///         flag has no effect if the window is already a non-topmost window.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>HWND_TOP ((HWND)0)</term><description>Places the window at the top of the Z order.</description>
        ///     </item>
        ///     <item>
        ///         <term>HWND_TOPMOST ((HWND)-1)</term>
        ///         <description>
        ///         Places the window above all non-topmost windows. The window maintains its topmost position
        ///         even when it is deactivated.
        ///         </description>
        ///     </item>
        ///     </list>
        ///     <para>For more information about how this parameter is used, see the following Remarks section.</para>
        /// </param>
        /// <param name="x">C++ ( X [in]. Type: int )<br />The new position of the left side of the window, in client coordinates.</param>
        /// <param name="y">C++ ( Y [in]. Type: int )<br />The new position of the top of the window, in client coordinates.</param>
        /// <param name="cx">C++ ( cx [in]. Type: int )<br />The new width of the window, in pixels.</param>
        /// <param name="cy">C++ ( cy [in]. Type: int )<br />The new height of the window, in pixels.</param>
        /// <param name="uFlags">
        ///     C++ ( uFlags [in]. Type: UINT )<br />The window sizing and positioning flags. This parameter can be a combination
        ///     of the following values.
        ///     <list type="table">
        ///     <itemheader>
        ///         <term>HWND sizing and positioning flags</term>
        ///         <description>Where to place and size window. Can be a combination of any</description>
        ///     </itemheader>
        ///     <item>
        ///         <term>SWP_ASYNCWINDOWPOS (0x4000)</term>
        ///         <description>
        ///         If the calling thread and the thread that owns the window are attached to different input
        ///         queues, the system posts the request to the thread that owns the window. This prevents the calling
        ///         thread from blocking its execution while other threads process the request.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_DEFERERASE (0x2000)</term>
        ///         <description>Prevents generation of the WM_SYNCPAINT message. </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_DRAWFRAME (0x0020)</term>
        ///         <description>Draws a frame (defined in the window's class description) around the window.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_FRAMECHANGED (0x0020)</term>
        ///         <description>
        ///         Applies new frame styles set using the SetWindowLong function. Sends a WM_NCCALCSIZE message
        ///         to the window, even if the window's size is not being changed. If this flag is not specified,
        ///         WM_NCCALCSIZE is sent only when the window's size is being changed
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_HIDEWINDOW (0x0080)</term><description>Hides the window.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOACTIVATE (0x0010)</term>
        ///         <description>
        ///         Does not activate the window. If this flag is not set, the window is activated and moved to
        ///         the top of either the topmost or non-topmost group (depending on the setting of the hWndInsertAfter
        ///         parameter).
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOCOPYBITS (0x0100)</term>
        ///         <description>
        ///         Discards the entire contents of the client area. If this flag is not specified, the valid
        ///         contents of the client area are saved and copied back into the client area after the window is sized or
        ///         repositioned.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOMOVE (0x0002)</term>
        ///         <description>Retains the current position (ignores X and Y parameters).</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOOWNERZORDER (0x0200)</term>
        ///         <description>Does not change the owner window's position in the Z order.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOREDRAW (0x0008)</term>
        ///         <description>
        ///         Does not redraw changes. If this flag is set, no repainting of any kind occurs. This applies
        ///         to the client area, the nonclient area (including the title bar and scroll bars), and any part of the
        ///         parent window uncovered as a result of the window being moved. When this flag is set, the application
        ///         must explicitly invalidate or redraw any parts of the window and parent window that need redrawing.
        ///         </description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOREPOSITION (0x0200)</term><description>Same as the SWP_NOOWNERZORDER flag.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOSENDCHANGING (0x0400)</term>
        ///         <description>Prevents the window from receiving the WM_WINDOWPOSCHANGING message.</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOSIZE (0x0001)</term>
        ///         <description>Retains the current size (ignores the cx and cy parameters).</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_NOZORDER (0x0004)</term>
        ///         <description>Retains the current Z order (ignores the hWndInsertAfter parameter).</description>
        ///     </item>
        ///     <item>
        ///         <term>SWP_SHOWWINDOW (0x0040)</term><description>Displays the window.</description>
        ///     </item>
        ///     </list>
        /// </param>
        /// <returns><c>true</c> or nonzero if the function succeeds, <c>false</c> or zero otherwise or if function fails.</returns>
        /// <remarks>
        ///     <para>
        ///     As part of the Vista re-architecture, all services were moved off the interactive desktop into Session 0.
        ///     hwnd and window manager operations are only effective inside a session and cross-session attempts to manipulate
        ///     the hwnd will fail. For more information, see The Windows Vista Developer Story: Application Compatibility
        ///     Cookbook.
        ///     </para>
        ///     <para>
        ///     If you have changed certain window data using SetWindowLong, you must call SetWindowPos for the changes to
        ///     take effect. Use the following combination for uFlags: SWP_NOMOVE | SWP_NOSIZE | SWP_NOZORDER |
        ///     SWP_FRAMECHANGED.
        ///     </para>
        ///     <para>
        ///     A window can be made a topmost window either by setting the hWndInsertAfter parameter to HWND_TOPMOST and
        ///     ensuring that the SWP_NOZORDER flag is not set, or by setting a window's position in the Z order so that it is
        ///     above any existing topmost windows. When a non-topmost window is made topmost, its owned windows are also made
        ///     topmost. Its owners, however, are not changed.
        ///     </para>
        ///     <para>
        ///     If neither the SWP_NOACTIVATE nor SWP_NOZORDER flag is specified (that is, when the application requests that
        ///     a window be simultaneously activated and its position in the Z order changed), the value specified in
        ///     hWndInsertAfter is used only in the following circumstances.
        ///     </para>
        ///     <list type="bullet">
        ///     <item>Neither the HWND_TOPMOST nor HWND_NOTOPMOST flag is specified in hWndInsertAfter. </item>
        ///     <item>The window identified by hWnd is not the active window. </item>
        ///     </list>
        ///     <para>
        ///     An application cannot activate an inactive window without also bringing it to the top of the Z order.
        ///     Applications can change an activated window's position in the Z order without restrictions, or it can activate
        ///     a window and then move it to the top of the topmost or non-topmost windows.
        ///     </para>
        ///     <para>
        ///     If a topmost window is repositioned to the bottom (HWND_BOTTOM) of the Z order or after any non-topmost
        ///     window, it is no longer topmost. When a topmost window is made non-topmost, its owners and its owned windows
        ///     are also made non-topmost windows.
        ///     </para>
        ///     <para>
        ///     A non-topmost window can own a topmost window, but the reverse cannot occur. Any window (for example, a
        ///     dialog box) owned by a topmost window is itself made a topmost window, to ensure that all owned windows stay
        ///     above their owner.
        ///     </para>
        ///     <para>
        ///     If an application is not in the foreground, and should be in the foreground, it must call the
        ///     SetForegroundWindow function.
        ///     </para>
        ///     <para>
        ///     To use SetWindowPos to bring a window to the top, the process that owns the window must have
        ///     SetForegroundWindow permission.
        ///     </para>
        /// </remarks>
        [DllImport("user32.dll", SetLastError = true, EntryPoint = "SetWindowPos")]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int x, int y, int cx, int cy, SetWindowPosFlags uFlags);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        extern internal static bool MoveWindow(IntPtr hWnd, int X, int Y, int nWidth, int nHeight, [MarshalAs(UnmanagedType.Bool)]bool bRepaint);

        internal enum MonitorDefaultTo
        {
            MONITOR_DEFAULTTONULL = 0,
            MONITOR_DEFAULTTOPRIMARY = 1,
            MONITOR_DEFAULTTONEAREST = 2,
        }
        [DllImport("user32.dll", CharSet = CharSet.Unicode, SetLastError = true)]
        extern internal static IntPtr MonitorFromWindow(IntPtr hWnd, MonitorDefaultTo dwFlags);

        [DllImport("SHCore.dll", CharSet = CharSet.Unicode, PreserveSig = false)]
        extern internal static void GetDpiForMonitor(IntPtr hMonitor, MonitorDpiType dpiType, ref uint dpiX, ref uint dpiY);

        // Winuser.h
        internal const int WM_DPICHANGED = 0x02E0;
        internal const int WM_INPUT = 0x000000FF;
        internal const int WM_DEVICECHANGE = 0x0219;
        internal static ushort ToLoWord(this IntPtr dword) { return (ushort)((uint)dword & 0xffff); }
        internal static ushort ToHiWord(this IntPtr dword) { return (ushort)((uint)dword >> 16); }
    }
}

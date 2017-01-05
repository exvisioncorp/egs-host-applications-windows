namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;
    using System.Windows.Forms;
    using Egs.Win32;

    internal partial class EgsDevicesWindowMessageReceivingForm : Form
    {
        delegate IntPtr WndProcDelegate(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam);
        IntPtr hMainWindow;
        IntPtr oldWndProcPtr;
        IntPtr newWndProcPtr;
        WndProcDelegate newWndProc;

        // TODO: MUSTDO: I thought I should remove this, but it is not easy.
        EgsDevice _CurrentDevice = null;
        internal EgsDevice CurrentDevice
        {
            get { return _CurrentDevice; }
            set
            {
                _CurrentDevice = value;
#if USE_OLD_HID
                HidReportsUpdateByWin32RawInput.Device = value;
#endif
            }
        }

        internal event EventHandler DeviceConnected;
        internal event EventHandler DeviceDisconnected;

        internal bool IsToUseActiveWindowHWnd { get; private set; }
        Win32DeviceNotification deviceNotification { get; set; }
#if USE_OLD_HID
        internal EgsDeviceHidReportsUpdateByWin32RawInput HidReportsUpdateByWin32RawInput { get; private set; }
        Win32RegisterRawInputDevices touchRawInputDevices { get; set; }
#endif
        internal Win32SetupDiForEgsDevice SetupDi { get; private set; }

        internal EgsDevicesWindowMessageReceivingForm()
        {
            InitializeComponent();
            this.Visible = false;
            IsToUseActiveWindowHWnd = EgsDevice.IsToUseActiveWindowHWnd;

            IntPtr monitoringHWnd = IntPtr.Zero;
            if (IsToUseActiveWindowHWnd)
            {
                IntPtr hWnd = NativeMethods.GetActiveWindow();
                hMainWindow = NativeMethods.GetForegroundWindow();
                newWndProc = new WndProcDelegate(WndProc);
                newWndProcPtr = Marshal.GetFunctionPointerForDelegate(newWndProc);
                oldWndProcPtr = NativeMethods.SetWindowLong(hMainWindow, -4, newWndProcPtr);
                if (true)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Console.WriteLine("Use HWND of the result of GetForegroundWindow()"); }
                    monitoringHWnd = hMainWindow;
                }
                else
                {
                    if (ApplicationCommonSettings.IsDebugging) { Console.WriteLine("Use HWND of the result of GetActiveWindow()"); }
                    monitoringHWnd = hWnd;
                }
            }
            else
            {
                if (ApplicationCommonSettings.IsDebugging) { Console.WriteLine("Use HWND of this Form"); }
                monitoringHWnd = this.Handle;
            }

            deviceNotification = new Win32DeviceNotification(monitoringHWnd);
            SetupDi = new Win32SetupDiForEgsDevice();

            deviceNotification.RegisterForDeviceNotificationsByUsbGuid();
#if USE_OLD_HID
            HidReportsUpdateByWin32RawInput = new EgsDeviceHidReportsUpdateByWin32RawInput();
            touchRawInputDevices = new Win32RegisterRawInputDevices(monitoringHWnd);
            touchRawInputDevices.RegisterRawInputDevices();
#endif
        }

        protected override void WndProc(ref Message m)
        {
            WndProc(m.HWnd, (uint)m.Msg, m.WParam, m.LParam);
            base.WndProc(ref m);
        }

        internal IntPtr WndProc(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            switch (msg)
            {
                case NativeMethods.WM_DEVICECHANGE:
                    ProcessWindowMessageDeviceChange(hWnd, msg, wParam, lParam);
                    break;
#if USE_OLD_HID
                case NativeMethods.WM_INPUT:
                    // MUSTDO: TODO: Fix the spec and implementation.  It is difficult to use multiple devices by smart way in various situations.
                    if (CurrentDevice != null)
                    {
                        // NOTE (en): My idea was wrong, i.e. "m.LParam" cannot decide the EgsDevice object which should receive the messages.  So cast data by GetRawInputData(m.LParam) to a structure, and then rawInput.header.hDevice should be checked.  But the value can depend on the kind of Report, and the value cannot specify which device sends it, even though KeyboardDevice can (maybe) do it.
                        // NOTE (ja): m.LParamからメッセージを処理するべきEgsDeviceを決定できると思ったら根本的に間違いだった、m.LParamをGetRawInputDataに渡して得られるデータを構造体にキャストして、rawInput.header.hDeviceの値を見る必要がある。しかもその値はReportの種類ごとかもしれず、KeyboardDeviceのときのように、デバイスごとの値が来るわけではない。
                        if (false) { Debug.WriteLine(lParam); }
                        HidReportsUpdateByWin32RawInput.ProcessRawInputData(lParam);
                    }
                    break;
#endif
            }
            if (IsToUseActiveWindowHWnd)
            {
                return NativeMethods.CallWindowProc(oldWndProcPtr, hWnd, msg, wParam, lParam);
            }
            else
            {
                return IntPtr.Zero;
            }
        }

        void ProcessWindowMessageDeviceChange(IntPtr hWnd, uint msg, IntPtr wParam, IntPtr lParam)
        {
            try
            {
                // The OnDeviceChange routine processes WM_DEVICECHANGE messages.
                Trace.Assert(msg == NativeMethods.WM_DEVICECHANGE);
                bool isConnected = wParam.ToInt32() == NativeMethods.DBT_DEVICEARRIVAL;
                bool isDisconnected = wParam.ToInt32() == NativeMethods.DBT_DEVICEREMOVECOMPLETE;

                if (isConnected == false && isDisconnected == false) { return; }

                // Find out if it's the device we're communicating with.
                string devicePath = deviceNotification.CheckDeviceChangeMessageSenderIsTargetDeviceOrNot(lParam);

                bool isTargetDevice = SetupDi.CheckDevicePathContainsVendorIdAndProductId(devicePath, "");
                if (isTargetDevice == false) { return; }

                if (isConnected)
                {
                    // If WParam contains DBT_DEVICEARRIVAL, a device has been attached.
                    Debug.WriteLine("A device has been attached.");
                    var t = DeviceConnected; if (t != null) { t(this, EventArgs.Empty); }
                }
                else if (isDisconnected)
                {
                    // If WParam contains DBT_DEVICEREMOVAL, a device has been removed.
                    Debug.WriteLine("A device has been removed.");
                    var t = DeviceDisconnected; if (t != null) { t(this, EventArgs.Empty); }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                if (ApplicationCommonSettings.IsDebugging)
                {
                    Debugger.Break();
                    string message = "Exception: " + ex.Message + Environment.NewLine + "Type: " + ex.GetType().Name + Environment.NewLine + "Method: " + ex.TargetSite.Name;
                    MessageBox.Show(message);
                }
                throw;
            }
        }
    }
}

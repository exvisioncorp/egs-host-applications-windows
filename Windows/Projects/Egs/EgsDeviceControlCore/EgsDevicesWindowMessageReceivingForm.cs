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
        // TODO: MUSTDO: I thought I should remove this, but it is not easy.
        EgsDevice _CurrentDevice = null;
        internal EgsDevice CurrentDevice
        {
            get { return _CurrentDevice; }
            set
            {
                _CurrentDevice = value;
                HidReportsUpdateByWin32RawInput.Device = value;
            }
        }

        internal event EventHandler DeviceConnected;
        internal event EventHandler DeviceDisconnected;

        internal EgsDeviceHidReportsUpdateByWin32RawInput HidReportsUpdateByWin32RawInput { get; private set; }
        Win32DeviceNotification deviceNotification { get; set; }
        Win32RegisterRawInputDevices touchRawInputDevices { get; set; }
        internal Win32SetupDiForEgsDevice SetupDi { get; private set; }

        internal EgsDevicesWindowMessageReceivingForm()
        {
            InitializeComponent();
            this.Visible = false;

            HidReportsUpdateByWin32RawInput = new EgsDeviceHidReportsUpdateByWin32RawInput();
            deviceNotification = new Win32DeviceNotification(this.Handle);
            touchRawInputDevices = new Win32RegisterRawInputDevices(this.Handle);
            SetupDi = new Win32SetupDiForEgsDevice();

            deviceNotification.RegisterForDeviceNotificationsByUsbGuid();
            touchRawInputDevices.RegisterRawInputDevices();
        }

        protected override void WndProc(ref Message m)
        {
            switch (m.Msg)
            {
                case NativeMethods.WM_DEVICECHANGE:
                    ProcessWindowMessageDeviceChange(ref m);
                    break;
                case NativeMethods.WM_INPUT:
                    // MUSTDO: TODO: Fix the spec and implementation.  It is difficult to use multiple devices by smart way in various situations.
                    if (CurrentDevice != null)
                    {
                        // NOTE (en): My idea was wrong, i.e. "m.LParam" cannot decide the EgsDevice object which should receive the messages.  So cast data by GetRawInputData(m.LParam) to a structure, and then rawInput.header.hDevice should be checked.  But the value can depend on the kind of Report, and the value cannot specify which device sends it, even though KeyboardDevice can (maybe) do it.
                        // NOTE (ja): m.LParamからメッセージを処理するべきEgsDeviceを決定できると思ったら根本的に間違いだった、m.LParamをGetRawInputDataに渡して得られるデータを構造体にキャストして、rawInput.header.hDeviceの値を見る必要がある。しかもその値はReportの種類ごとかもしれず、KeyboardDeviceのときのように、デバイスごとの値が来るわけではない。
                        if (false) { Debug.WriteLine(m.LParam); }
                        HidReportsUpdateByWin32RawInput.ProcessRawInputData(m.LParam);
                    }
                    break;
            }
            base.WndProc(ref m);
        }

        void ProcessWindowMessageDeviceChange(ref Message m)
        {
            try
            {
                // The OnDeviceChange routine processes WM_DEVICECHANGE messages.
                Trace.Assert(m.Msg == NativeMethods.WM_DEVICECHANGE);
                bool isConnected = m.WParam.ToInt32() == NativeMethods.DBT_DEVICEARRIVAL;
                bool isDisconnected = m.WParam.ToInt32() == NativeMethods.DBT_DEVICEREMOVECOMPLETE;

                if (isConnected == false && isDisconnected == false) { return; }

                // Find out if it's the device we're communicating with.
                string devicePath = deviceNotification.CheckDeviceChangeMessageSenderIsTargetDeviceOrNot(m.LParam);

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

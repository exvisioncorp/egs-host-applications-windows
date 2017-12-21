namespace WpfWindowResizeTest
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Shapes;
    using DotNetUtility;
    using DotNetUtility.Views;

    public partial class StateMonitoringWindow : Window
    {
        public System.Windows.Threading.DispatcherTimer StateMonitorWindowUpdateTimer { get; private set; }
        public Window MonitoringTarget { get; private set; }

        public StateMonitoringWindow()
        {
            InitializeComponent();
            StateMonitorWindowUpdateTimer = new System.Windows.Threading.DispatcherTimer() { Interval = TimeSpan.FromMilliseconds(33) };
            StateMonitorWindowUpdateTimer.Tick += StateMonitorWindowUpdateTimer_Tick;
        }

        public void Initialize(WpfWindowResizeTestMainWindow monitoringTarget)
        {
            System.Diagnostics.Trace.Assert(monitoringTarget != null);
            MonitoringTarget = monitoringTarget;
            // 呼び出すと、Window上にマウスカーソルがないときの値がおかしくなる。使えねえ。
            //Mouse.Capture(this);

            StateMonitorWindowUpdateTimer.Start();
        }

        void StateMonitorWindowUpdateTimer_Tick(object sender, EventArgs e)
        {
            var Cursor_Position = System.Windows.Forms.Cursor.Position;
            var Control_MousePosition = System.Windows.Forms.Control.MousePosition;
            var Mouse_GetPosition_Null = Mouse.GetPosition(null);
            var Mouse_GetPosition_This = Mouse.GetPosition(this);
            var PointToScreen_Mouse_GetPosition_This = PointToScreen(Mouse.GetPosition(this));
            var windowLocation = new Rect(MonitoringTarget.Left, MonitoringTarget.Top, MonitoringTarget.Width, MonitoringTarget.Height);

            var DpiFromSystemParameters = DpiExtensions.DpiFromSystemParameters;
            var DpiFromGetDpiForMonitor = DpiExtensions.GetDpiFromGetDpiForMonitor(this, Win32.MonitorDpiType.RawDpi);
            var DpiFromCompositionTargetTransformToDevice = DpiExtensions.GetDpiFromCompositionTargetTransformToDevice(this);

            var DpiFromHdcForTheEntireScreen = DotNetUtility.Dpi.DpiFromHdcForTheEntireScreen;
            var DpiFromGetDpiForMonitorWithEffectiveDpi = DotNetUtility.Dpi.DpiFromGetDpiForMonitorOfPrimaryMonitorWithEffectiveDpi;
            var DpiFromGetDpiForMonitorWithAngularDpi = DotNetUtility.Dpi.DpiFromGetDpiForMonitorOfNearestMonitorWithAngularDpi;
            var DpiFromGetDpiForMonitorWithRawDpi = DotNetUtility.Dpi.DpiFromGetDpiForMonitorOfNearestMonitorWithRawDpi;
            var Cursor_Position_ScaledByDpiFromGetDpiForMonitor = DpiFromGetDpiForMonitor.GetScaledPosition(System.Windows.Forms.Cursor.Position);
            var Cursor_Position_ScaledByDpiFromHdc = DpiFromHdcForTheEntireScreen.GetScaledPosition(System.Windows.Forms.Cursor.Position);

            var msgText = "";
            msgText += "Cursor_Position: " + Cursor_Position + Environment.NewLine;
            msgText += "Control_MousePosition: " + Control_MousePosition + Environment.NewLine;
            msgText += "Mouse_GetPosition_Null: " + Mouse_GetPosition_Null + Environment.NewLine;
            msgText += "Mouse_GetPosition_This: " + Mouse_GetPosition_This + Environment.NewLine;
            msgText += "PointToScreen_Mouse_GetPosition_This: " + PointToScreen_Mouse_GetPosition_This + Environment.NewLine;
            msgText += "windowLocation: " + windowLocation + Environment.NewLine;
            msgText += Environment.NewLine;
            msgText += "DpiFromSystemParameters: " + DpiFromSystemParameters + Environment.NewLine;
            msgText += "DpiFromGetDpiForMonitor: " + DpiFromGetDpiForMonitor + Environment.NewLine;
            msgText += "DpiFromCompositionTargetTransformToDevice: " + DpiFromCompositionTargetTransformToDevice + Environment.NewLine;
            msgText += Environment.NewLine;
            msgText += "DpiFromHdcForTheEntireScreen: " + DpiFromHdcForTheEntireScreen + Environment.NewLine;
            msgText += "DpiFromGetDpiForMonitorOfPrimaryMonitorWithEffectiveDpi: " + DpiFromGetDpiForMonitorWithEffectiveDpi + Environment.NewLine;
            msgText += "DpiFromGetDpiForMonitorOfNearestMonitorWithAngularDpi: " + DpiFromGetDpiForMonitorWithAngularDpi + Environment.NewLine;
            msgText += "DpiFromGetDpiForMonitorOfNearestMonitorWithRawDpi: " + DpiFromGetDpiForMonitorWithRawDpi + Environment.NewLine;
            msgText += Environment.NewLine;
            msgText += "Cursor_Position_ScaledByDpiFromGetDpiForMonitor: " + Cursor_Position_ScaledByDpiFromGetDpiForMonitor + Environment.NewLine;
            msgText += "Cursor_Position_ScaledByDpiFromHdc: " + Cursor_Position_ScaledByDpiFromHdc + Environment.NewLine;

            stateTextBlock.Text = msgText;
        }
    }
}

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
    using System.ComponentModel;
    using System.Diagnostics;
    using Microsoft.Win32;
    using Egs;
    using Egs.DotNetUtility;

    internal partial class EgsDeviceFirmwareUpdateUserControl : UserControl
    {
        public EgsDeviceFirmwareUpdateModel Model { get; private set; }
        public EgsDevice Device { get; private set; }
        public bool IsToHideThisWhenDeviceDisconnected { get; set; }

        public EgsDeviceFirmwareUpdateUserControl(EgsDevice device, Version firmwareVersionInDevice, bool isToShowDevelopersInformation)
        {
            InitializeComponent();

            Trace.Assert(device != null);
            Trace.Assert(firmwareVersionInDevice != null);
            Device = device;
            IsToHideThisWhenDeviceDisconnected = false;

            Device.IsHidDeviceConnectedChanged += Device_IsHidDeviceConnectedChanged;

            DevicePathTextBoxPanel.Visibility = isToShowDevelopersInformation ? Visibility.Visible : Visibility.Collapsed;
            LogTextBoxPanel.Visibility = isToShowDevelopersInformation ? Visibility.Visible : Visibility.Collapsed;
            ButtonsPanel.Visibility = isToShowDevelopersInformation ? Visibility.Visible : Visibility.Collapsed;

            PleaseConnectTheDeviceImage.Source = BitmapImageUtility.LoadBitmapImageFromFile(@".\Resources\PleaseConnectTheDevice.png");
            PleaseDisconnectTheDeviceImage.Source = BitmapImageUtility.LoadBitmapImageFromFile(@".\Resources\PleaseDisconnectTheDevice.png");


            Model = new EgsDeviceFirmwareUpdateModel(Device, firmwareVersionInDevice);
            Model.ProgressReport.ProgressChanged += ProgressReport_ProgressChanged;
            this.DataContext = Model;

            this.Unloaded += (sender, e) =>
            {
                Model.CancelAsync();
                Device.IsHidDeviceConnectedChanged -= Device_IsHidDeviceConnectedChanged;
            };
        }

        void Device_IsHidDeviceConnectedChanged(object sender, EventArgs e)
        {
            var title = "";
            title += "Index: " + Device.IndexInHidDevicePathList;
            title += "  " + "HidDeviceDevicePath: " + Device.HidDeviceDevicePath;
            this.Dispatcher.Invoke(new Action(() =>
            {
                DevicePathTextBox.Text = title;
                if (IsToHideThisWhenDeviceDisconnected)
                {
                    LogTextBox.Text = "";
                    this.Visibility = Device.IsHidDeviceConnected ? Visibility.Visible : Visibility.Collapsed;
                }
            }));
        }

        void ProgressReport_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            if (e.UserState != null)
            {
                var report = e.UserState as EgsDeviceFirmwareUpdateStateReport;
                if (string.IsNullOrEmpty(report.Message) == false) { LogTextBox.AppendText(report.Message); }
                if (string.IsNullOrEmpty(report.UserNotificationMessage) == false) { MessageBox.Show(report.UserNotificationMessage, Egs.EgsDeviceControlCore.Properties.Resources.EgsDeviceFirmwareUpdateModel_DeviceFirmwareUpdate); }
                if (string.IsNullOrEmpty(report.MessageForDebug) == false) { Debug.WriteLine(report.MessageForDebug); }
            }
            //textBox2.Text = Model.PercentProgress.ToString("N2") + "%";
            //UpdatingProgressBar.Value = Model.PercentProgress;
        }
    }
}

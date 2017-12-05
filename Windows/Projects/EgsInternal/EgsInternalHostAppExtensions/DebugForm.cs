namespace Egs
{
    using System;
    using System.Diagnostics;
    using System.Windows.Forms;
    using Egs.PropertyTypes;
    using Egs.Win32;

    public partial class DebugForm : Form
    {
        EgsDevice ownerDevice;

        public DebugForm(EgsDevice device)
        {
            InitializeComponent();
            ownerDevice = device;

            ownerDevice.IsConnectedChanged += (sender, e) => { this.Visible = ownerDevice.IsConnected; };
            ownerDevice.EgsGestureHidReport.ReportUpdated += (sender, e) =>
            {
                string str = BitConverter.ToString(ownerDevice.HidReportsUpdate.reportAsByteArray);
                str += DateTime.Now.ToString(" HH:mm:ss.fff");
                this.InsertList(str);
            };
            copyToClipboardButton.Click += (sender, e) =>
            {
                string str = "";
                foreach (string item in logListBox.Items) { str += item; }
                // need this check
                if (string.IsNullOrEmpty(str) == false) { Clipboard.SetText(str); }
            };
            clearLogButton.Click += (sender, e) =>
            {
                logListBox.Items.Clear();
            };
            monitorMouseCheckBox.Click += (sender, e) =>
            {
                ApplicationCommonSettings.IsToEmulateReportByActualMouseRawInputToDebugViews = this.monitorMouseCheckBox.Checked;
                if (monitorMouseCheckBox.Checked)
                {
                    ownerDevice.Settings.TouchInterfaceKind.Value = TouchInterfaceKinds.Mouse;
                }
            };

            this.monitorMouseCheckBox.Checked = ApplicationCommonSettings.IsToEmulateReportByActualMouseRawInputToDebugViews;
        }

        void InsertList(string str)
        {
            //textBox1.AppendText(string.Format(System.Globalization.CultureInfo.InvariantCulture, "RAW HID DATA: {0}\r\n", RawCodeForDebugging));

            //if ((limitTo100CheckBox.Checked) && (listBox1.Items.Count > 100)) listBox1.Items.RemoveAt(100);
            //if (((hidReport[21] > 0) || showEmptyReportCheckBox.Checked) && loggingCheckBox.Checked)
            //{

            //    listBox1.Items.Insert(0, string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\r\n", RawCodeForDebugging));
            //    Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\r\n", RawCodeForDebugging));
            //}

            if ((limitTo100CheckBox.Checked) && (logListBox.Items.Count > 100)) logListBox.Items.RemoveAt(100);
            if (loggingCheckBox.Checked)
            {
                logListBox.Items.Insert(0, string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\r\n", str));
                Debug.WriteLine(string.Format(System.Globalization.CultureInfo.InvariantCulture, "{0}\r\n", str));
            }
        }
    }
}

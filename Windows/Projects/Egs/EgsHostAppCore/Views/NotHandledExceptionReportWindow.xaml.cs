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
    using System.Windows.Shapes;

    public partial class NotHandledExceptionReportWindow : Window
    {
        public NotHandledExceptionReportWindow()
        {
            InitializeComponent();
            this.Title = ApplicationCommonSettings.HostApplicationName;
            SendMailHyperlink.NavigateUri = new Uri(ApplicationCommonSettings.SellerSupportNavigateUriString);
            ExitButton.Click += delegate { this.Close(); };
        }

        public void Initialize(Exception ex)
        {
            // NOTE: It shows messages in English.  It is OK.
            var text = "";
            text += "OS version: " + Environment.OSVersion + Environment.NewLine;
            text += ApplicationCommonSettings.HostApplicationName + " Host Application Version: " + ApplicationCommonSettings.ZkooHostAppExeAssemblyVersionMajorMinorBuildRevisionString + Environment.NewLine;
            text += "EgsHostAppCore.dll version: " + ApplicationCommonSettings.HostAppCoreDllAssemblyVersionMajorMinorBuildRevisionString + Environment.NewLine;
            text += "Exception Message: " + Environment.NewLine;
            text += ex.ToString();
            ExceptionTextBox.Text = text;
        }

        void OnNavigate(object sender, System.Windows.Navigation.RequestNavigateEventArgs e)
        {
            System.Diagnostics.Process.Start(e.Uri.AbsoluteUri);
            e.Handled = true;
        }
    }
}

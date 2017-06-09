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
    using System.Diagnostics;

    public partial class SettingsUserControl : UserControl
    {
        public Visibility TutorialAppSettingsGroupBoxVisibility { get { return tutorialAppSettingsGroupBox.Visibility; } set { tutorialAppSettingsGroupBox.Visibility = value; } }

        public SettingsUserControl()
        {
            InitializeComponent();

            bool isZkooOrEgsSdk = ApplicationCommonSettings.HostApplicationName == "ZKOO" || ApplicationCommonSettings.IsDeveloperRelease;
            HardwareTypeGroupBox.Visibility = isZkooOrEgsSdk ? Visibility.Visible : Visibility.Collapsed;
            CheckForEgsHostAppCoreUpdateCommandButton.Visibility = isZkooOrEgsSdk ? Visibility.Visible : Visibility.Collapsed;

            DeveloperSettingsTabItem.Visibility = (ApplicationCommonSettings.IsDeveloperRelease || ApplicationCommonSettings.IsDebugging) ? Visibility.Visible : Visibility.Collapsed;
            DeviceUsageGroupBox.Visibility = ApplicationCommonSettings.CanChangeDeviceUsage ? Visibility.Visible : Visibility.Collapsed;
        }

        internal void ReloadDataContext()
        {
            var currentDataContextBackup = this.DataContext;
            this.DataContext = null;
            this.DataContext = currentDataContextBackup;
        }
    }
}

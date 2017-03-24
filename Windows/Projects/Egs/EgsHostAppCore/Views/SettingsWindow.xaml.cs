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

    public partial class SettingsWindow : Window
    {
        public SettingsUserControl SettingsUserControl { get { return settingsUserControl; } }
        bool isClosingToExitApplication { get; set; }

        public SettingsWindow()
        {
            InitializeComponent();

            try { this.Icon = Egs.DotNetUtility.BitmapImageUtility.LoadBitmapImageFromFile("Resources/SettingsWindowIcon.png"); }
            catch { }
            this.Title = EgsHostAppBaseComponents.EgsHostApplicationName + " " + Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_Settings;

            isClosingToExitApplication = false;

            // NOTE: DragMove() is not good way, I think.  Users can move the window by dragging the window's title bar.
            if (false)
            {
                this.MouseLeftButtonDown += (sender, e) =>
                {
                    if (e.ButtonState != MouseButtonState.Pressed) { return; }
                    this.DragMove();
                };
            }

            this.IsVisibleChanged += (sender, e) =>
            {
                if ((bool)e.NewValue == true)
                {
                    // TODO: Maybe it is OK.  I forgot why it uses Dispatcher.
                    this.Dispatcher.Invoke(() =>
                    {
                        this.WindowState = WindowState.Normal;
                        this.Activate();
                    });
                }
            };

            this.Closing += (sender, e) =>
            {
                if (isClosingToExitApplication == false)
                {
                    this.Visibility = Visibility.Hidden;
                    e.Cancel = true;
                    return;
                }
            };
        }

        public void CloseToExitApplication()
        {
            isClosingToExitApplication = true;

            // http://stackoverflow.com/questions/31362077/loadfromcontext-occurred/31760355#31760355
            // If "NotMarshalable" occurs in VS 2015 and so on,
            // please uncheck the following option:
            // Tools –> Options –> Debugging –> General –> Enable UI Debugging Tools for XAML
            base.Close();
        }

        public void ToggleVisibility()
        {
            var newVisibility = (this.Visibility == Visibility.Visible) ? Visibility.Hidden : Visibility.Visible;
            this.Visibility = newVisibility;
        }

        public void InitializeOnceAtStartup(EgsHostAppBaseComponents host)
        {
            // NOTE: The settings window set the settings of CameraViewWindowModel, so the type of the argument is not EgsHostOnUserControl but EgsHostAppBaseComponents.
            Trace.Assert(host != null);
            this.DataContext = host;
        }

        internal void ReloadDataContext()
        {
            var currentDataContextBackup = this.DataContext;
            this.DataContext = null;
            this.DataContext = currentDataContextBackup;
            settingsUserControl.ReloadDataContext();
        }
    }
}

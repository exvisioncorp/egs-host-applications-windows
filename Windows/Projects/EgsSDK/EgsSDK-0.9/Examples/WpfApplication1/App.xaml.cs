namespace WpfApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Windows;
    using System.Diagnostics;
    using Egs;
    using Egs.Views;
    using Egs.DotNetUtility;

    public partial class App : Application
    {
        public App()
            : base()
        {
            Egs.BindableResources.Current.CultureChanged += delegate
            {
                ApplicationCommonSettings.HostApplicationName = Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_GestureCamera;
                //ApplicationCommonSettings.HostApplicationName = "WpfApplication1";
            };

            // You can change the application CultureInfo to some cultures.
            // The next line lets it use OS culture
            Egs.BindableResources.Current.ChangeCulture("");
            //Egs.BindableResources.Current.ChangeCulture(ApplicationCommonSettings.DefaultCultureInfoName);
            //Egs.BindableResources.Current.ChangeCulture("en");
            //Egs.BindableResources.Current.ChangeCulture("ja");
            //Egs.BindableResources.Current.ChangeCulture("zh-Hans");

            if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
            {
                // Currently, the ZkooHostApp is not service, so only one instance can run.
                var msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_Application0IsAlreadyRunning, ApplicationCommonSettings.HostApplicationName);
                MessageBox.Show(msg, ApplicationCommonSettings.HostApplicationName);
                Application.Current.Shutdown();
                return;
            }

            // The next line shows "Developer" tab on SettingsWindow.
            ApplicationCommonSettings.IsDebugging = true;

            // This object has all Views and Models
            var hostAppComponents = new EgsHostAppBaseComponents();
            // A lot of classes in "Egs" namespace have "InitializeOnceAtStartup..." methods.  Please call them.
            hostAppComponents.InitializeOnceAtStartup();
            hostAppComponents.HasResetSettings += delegate
            {
                // You can modify the application default settings here.
                hostAppComponents.Device.Settings.CursorSpeedAndPrecisionMode.Value = Egs.PropertyTypes.CursorSpeedAndPrecisionModes.Standard;
            };

            hostAppComponents.CameraViewWindow.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        // NOTE: The specification changed.
                        // When you use EgsHostAppBaseComponents, it disposes itself when CameraViewWindow is closed.
                        // So you need not to attach "event handler which calls host.Dispose()" to "event CameraViewWindow.Closed".
                        hostAppComponents.CameraViewWindow.Close();
                        break;
                }
            };

            this.Exit += delegate
            {
                if (hostAppComponents != null) { hostAppComponents.Dispose(); hostAppComponents = null; }
                DuplicatedProcessStartBlocking.ReleaseMutex();
            };
        }
    }
}

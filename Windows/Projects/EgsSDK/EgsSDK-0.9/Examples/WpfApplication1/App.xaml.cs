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
                MessageBox.Show(EgsHostAppBaseComponents.MessageOfOnlyOneInstanceCanRun);
                Application.Current.Shutdown();
                return;
            }

            // The next line shows "Developer" tab on SettingsWindow.
            ApplicationCommonSettings.IsDebugging = true;

            // This object has all Views and Models
            var host = new EgsHostAppBaseComponents();
            // A lot of classes in "Egs" namespace have "InitializeOnceAtStartup..." methods.  Please call them.
            host.InitializeOnceAtStartup();

            host.CameraViewWindow.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        // NOTE: The specification changed.
                        // When you use EgsHostAppBaseComponents, it dispose itself when CameraViewWindow is closed.
                        // So you need not to attach an event handler to call host.Dispose() to CameraViewWindow.Closed.
                        host.CameraViewWindow.Close();
                        break;
                }
            };

            this.Exit += delegate
            {
                DuplicatedProcessStartBlocking.ReleaseMutex();
            };
        }
    }
}

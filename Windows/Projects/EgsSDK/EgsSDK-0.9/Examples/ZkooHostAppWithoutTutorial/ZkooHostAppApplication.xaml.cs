namespace ZkooHostAppWithoutTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Diagnostics;
    using System.ComponentModel;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.Views;

    public partial class ZkooHostAppApplication : Application
    {
        EgsHostAppBaseComponents hostAppComponents { get; set; }

        public ZkooHostAppApplication()
            : base()
        {
            hostAppComponents = null;

            try
            {
                Egs.BindableResources.Current.ChangeCulture(ApplicationCommonSettings.DefaultCultureInfoName);
                //Egs.BindableResources.Current.ChangeCulture("en");
                //Egs.BindableResources.Current.ChangeCulture("ja");
                //Egs.BindableResources.Current.ChangeCulture("zh-Hans");

                if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
                {
                    MessageBox.Show(EgsHostAppBaseComponents.MessageOfOnlyOneInstanceCanRun);
                    if (Application.Current != null) { Application.Current.Shutdown(); }
                    return;
                }

                hostAppComponents = new EgsHostAppBaseComponents();
                hostAppComponents.InitializeOnceAtStartup();
                if (SettingsSerialization.LoadSettingsJsonFile(hostAppComponents) == false) { hostAppComponents.Reset(); }

                EgsHostAppBaseComponents.EgsHostApplicationName = "ZKOO";
                hostAppComponents.AppTrayIconAndMenuItems.TextOfNotifyIconInTray = EgsHostAppBaseComponents.EgsHostApplicationName;


                // NOTE: Not Disposed but Disposing
                hostAppComponents.Disposing += delegate
                {
                    // NOTE: Save settings before Dispose().
                    SettingsSerialization.SaveSettingsJsonFile(hostAppComponents);
                };

                base.Exit += delegate
                {
                    if (hostAppComponents != null) { hostAppComponents.Dispose(); hostAppComponents = null; }
                    ReleaseMutexAndShutdown();
                };

                hostAppComponents.CheckIfDeviceFirmwareIsLatestOrNotAndExitApplicationIfFailed();
                // NOTE: If users exit the application by the button on Camera View while "Firmware Update" dialog, exception occurs.
                // MUSTDO: We will fix this.
                if (hostAppComponents.SettingsWindow == null) { return; }


                // TODO: The tutorial program will be separated into an independent application.
                hostAppComponents.SettingsWindow.SettingsUserControl.TutorialAppSettingsGroupBoxVisibility = Visibility.Collapsed;
                hostAppComponents.StartTutorialCommand.CanPerform = false;
            }
            catch (Exception ex)
            {
                ShutdownApplicationByException(ex);
            }
        }

        void ShutdownApplicationByException(Exception ex)
        {
            if (ex is EgsHostApplicationIsClosingException)
            {
                // NOTE: Assuming that this is the correct way to shutdown the application.
                MessageBox.Show(Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_ApplicationWillExit, EgsHostAppBaseComponents.EgsHostApplicationName, MessageBoxButton.OK);
                if (hostAppComponents != null) { hostAppComponents.Dispose(); hostAppComponents = null; }
            }
            else
            {
                // NOTE: This is not handled exceptions.  At first it saves safer settings, and then it shows "we're sorry" window.
                try
                {
                    // NOTE: But in some cases, application is already shutdown, so this code itself can occur exceptions
                    var window = new NotHandledExceptionReportWindow();
                    window.Initialize(ex);
                    window.ShowDialog();
                }
                catch (Exception ex2)
                {
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    MessageBox.Show(ex2.Message);
                }
            }
            ReleaseMutexAndShutdown();
        }

        void ReleaseMutexAndShutdown()
        {
            DuplicatedProcessStartBlocking.ReleaseMutex();
            // TODO: MUSTDO: confirm this way is right or not.
            if (Application.Current != null) { Application.Current.Shutdown(); }
        }
    }
}

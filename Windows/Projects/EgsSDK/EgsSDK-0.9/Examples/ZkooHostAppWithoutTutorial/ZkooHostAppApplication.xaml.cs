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

                try
                {
                    // Sorry, EgsHostSettings is no longer available.
                    // EgsHostAppBaseComponents creates and has the object of EgsDeviceSettings
                    hostAppComponents = new EgsHostAppBaseComponents();
                    hostAppComponents.InitializeOnceAtStartup();

                    ZkooHostAppWithoutTutorial.Properties.Settings.Default.Reload();
                    if (string.IsNullOrEmpty(ZkooHostAppWithoutTutorial.Properties.Settings.Default.WholeSettingsAsJsonString) == false)
                    {
                        Newtonsoft.Json.JsonConvert.PopulateObject(ZkooHostAppWithoutTutorial.Properties.Settings.Default.WholeSettingsAsJsonString, hostAppComponents);
                    }
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                    MessageBox.Show("Failed to load the last settings.", EgsHostAppBaseComponents.EgsHostApplicationName);
                    ShutdownApplicationByException(ex);
                    return;
                }


                EgsHostAppBaseComponents.EgsHostApplicationName = "ZKOO";
                hostAppComponents.AppTrayIconAndMenuItems.TextOfNotifyIconInTray = EgsHostAppBaseComponents.EgsHostApplicationName;


                // NOTE: Not Disposed but Disposing
                hostAppComponents.Disposing += delegate
                {
                    // NOTE: Save settings before Dispose().
                    // EgsDevice.Dispose() calls "Settings.IsToDetectFace.Value = false".
                    SaveEgsHostAppBaseComponentsToPropertiesSettings(hostAppComponents);
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

        void SaveEgsHostAppBaseComponentsToPropertiesSettings(EgsHostAppBaseComponents obj)
        {
            // NOTE: Sometimes, this serialization fails!! and if wrong settings is saved, it causes various problems.  So it checks with deserialization.
            try
            {
                var serializationResult = Newtonsoft.Json.JsonConvert.SerializeObject(hostAppComponents, Newtonsoft.Json.Formatting.Indented);
                var deserializationResult = Newtonsoft.Json.JsonConvert.DeserializeObject(serializationResult);
                if (deserializationResult == null) { throw new Exception("Save Settings failed."); }
                ZkooHostAppWithoutTutorial.Properties.Settings.Default.WholeSettingsAsJsonString = serializationResult;
                ZkooHostAppWithoutTutorial.Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging)
                {
                    Debugger.Break();
                    MessageBox.Show(ex.Message);
                }
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
                    // If settings object is broken, Settings.Default.[SomeMember].get and Reset() method cause exceptions, and Save() method does not save the updated settings without any exceptions!
                    //ZkooHostAppWithoutTutorial.Properties.Settings.Default.WholeSettingsAsJsonString = ""; // error
                    //ZkooHostAppWithoutTutorial.Properties.Settings.Default.Reset(); // error
                    //var defaultSettings = new ZkooHostAppWithoutTutorial.Properties.Settings();
                    //defaultSettings.WholeSettingsAsJsonString = ""; // error
                    //defaultSettings.Properties.Clear();
                    //defaultSettings.Save(); // do nothing without any exception!

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

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
                    if (ApplicationCommonSettings.IsDebugging)
                    {
                        Debugger.Break();
                        MessageBox.Show("Failed to load the last settings.", EgsHostAppBaseComponents.EgsHostApplicationName);
                    }

                    // It constructs the object again.
                    if (hostAppComponents != null) { hostAppComponents.Dispose(); hostAppComponents = null; }
                    hostAppComponents = new EgsHostAppBaseComponents();
                    hostAppComponents.InitializeOnceAtStartup();
                    // Save the safe settings.
                    ZkooHostAppWithoutTutorial.Properties.Settings.Default.WholeSettingsAsJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(hostAppComponents, Newtonsoft.Json.Formatting.Indented);
                    ZkooHostAppWithoutTutorial.Properties.Settings.Default.Save();
                }


                EgsHostAppBaseComponents.EgsHostApplicationName = "ZKOO";
                hostAppComponents.AppTrayIconAndMenuItems.TextOfNotifyIconInTray = EgsHostAppBaseComponents.EgsHostApplicationName;


                // NOTE: Not Disposed but Disposing
                hostAppComponents.Disposing += delegate
                {
                    // NOTE: Save settings before Dispose().
                    // EgsDevice.Dispose() calls "Settings.IsToDetectFace.Value = false".
                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(hostAppComponents, Newtonsoft.Json.Formatting.Indented);
                    ZkooHostAppWithoutTutorial.Properties.Settings.Default.WholeSettingsAsJsonString = str;
                    ZkooHostAppWithoutTutorial.Properties.Settings.Default.Save();
                    //if (Application.Current != null) { Application.Current.Shutdown(); }
                };

                base.Exit += delegate
                {
                    if (hostAppComponents != null) { hostAppComponents.Dispose(); hostAppComponents = null; }
                    DuplicatedProcessStartBlocking.ReleaseMutex();
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
                MessageBox.Show(Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_ApplicationWillExit, EgsHostAppBaseComponents.EgsHostApplicationName, MessageBoxButton.OK);
                if (hostAppComponents != null)
                {
                    hostAppComponents.Dispose();
                    hostAppComponents = null;
                }
                else
                {
                    if (Application.Current != null) { Application.Current.Shutdown(); }
                }
                return;
            }
            try
            {
                if (true)
                {
                    var window = new NotHandledExceptionReportWindow();
                    window.Initialize(ex);
                    window.ShowDialog();
                }
                using (var saferObj = new EgsHostAppBaseComponents())
                {
                    // NOTE: Save safer settings.
                    saferObj.InitializeOnceAtStartup();
                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(saferObj, Newtonsoft.Json.Formatting.Indented);
                    ZkooHostAppWithoutTutorial.Properties.Settings.Default.WholeSettingsAsJsonString = str;
                    ZkooHostAppWithoutTutorial.Properties.Settings.Default.Save();
                }
            }
            catch (Exception ex2)
            {
                Debug.WriteLine(ex2.Message);
            }
            if (Application.Current != null) { Application.Current.Shutdown(); }
        }
    }
}

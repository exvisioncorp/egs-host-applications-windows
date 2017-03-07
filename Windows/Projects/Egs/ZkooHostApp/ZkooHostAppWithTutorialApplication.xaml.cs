namespace Egs.ZkooHostApp
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
    using Egs.ZkooTutorial;

    public partial class ZkooHostAppWithTutorialApplication : Application
    {
        EgsHostAppBaseComponents hostAppComponents { get; set; }

        public ZkooHostAppWithTutorialApplication()
            : base()
        {
            ZkooTutorialModel zkooTutorialModel = null;
            MainNavigationWindow navigator = null;
            hostAppComponents = null;

            // TODO: Check the correct way to catch all exceptions and show it "We're sorry" dialog.  But the next way is still meaningless now, and sometimes this code can cause a problem that application cannot exit.
            //AppDomain.CurrentDomain.UnhandledException += (sender, e) => { ShutdownApplicationByException((Exception)e.ExceptionObject); };

            try
            {
                Egs.BindableResources.Current.ChangeCulture(ApplicationCommonSettings.DefaultCultureInfoName);
                Egs.ZkooTutorial.BindableResources.Current.ChangeCulture(ApplicationCommonSettings.DefaultCultureInfoName);

                if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
                {
                    MessageBox.Show(EgsHostAppBaseComponents.MessageOfOnlyOneInstanceCanRun);
                    if (Application.Current != null) { Application.Current.Shutdown(); }
                    return;
                }

                try
                {
                    hostAppComponents = new EgsHostAppBaseComponents();
                    hostAppComponents.InitializeOnceAtStartup();

                    ZkooHostApp.Properties.Settings.Default.Reload();
                    if (string.IsNullOrEmpty(ZkooHostApp.Properties.Settings.Default.WholeSettingsAsJsonString) == false)
                    {
#if false
                        // TODO: Fix a problem that ObservableCollection can be duplicated.
                        var jss = new Newtonsoft.Json.JsonSerializerSettings();
                        // NOTE: If it is set to "Error", just unknown parameters in Json file can occur exceptions.
                        // To absorb the difference of the version of the Settings object, let it cancel the exception.
                        // Save the settings by the latest format, when the application exits.
                        //jss.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error; // default == Ignore
                        // NOTE: When there is not the description of the value in Json or the value is null, settings in constructors are used.  It can solve problems in JSON file by the settings in constructor.
                        jss.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore; // default == Include
                        // NOTE: I don't understand how "Reuse" works, so I don't use it.
                        // TODO: Check the behavior.
                        //jss.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Reuse; // default == Auto
                        jss.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
#endif
                        // Now I use not Deserialize but PopulateObject.
                        Newtonsoft.Json.JsonConvert.PopulateObject(ZkooHostApp.Properties.Settings.Default.WholeSettingsAsJsonString, hostAppComponents);
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

                    // It construct the object again.
                    if (hostAppComponents != null) { hostAppComponents.Dispose(); hostAppComponents = null; }
                    hostAppComponents = new EgsHostAppBaseComponents();
                    hostAppComponents.InitializeOnceAtStartup();
                    // NOTE: In some PCs, the host application fails to start in Tutorial.  So the Tutorial exits unexpectedly, it does not start Tutorial from next time.
                    hostAppComponents.IsToStartTutorialWhenHostApplicationStart = false;
                    // NOTE: Save the default settings.
                    ZkooHostApp.Properties.Settings.Default.WholeSettingsAsJsonString = Newtonsoft.Json.JsonConvert.SerializeObject(hostAppComponents, Newtonsoft.Json.Formatting.Indented);
                    ZkooHostApp.Properties.Settings.Default.Save();
                }


                EgsHostAppBaseComponents.EgsHostApplicationName = "ZKOO";
                hostAppComponents.AppTrayIconAndMenuItems.TextOfNotifyIconInTray = EgsHostAppBaseComponents.EgsHostApplicationName;
                base.MainWindow = hostAppComponents.CameraViewWindow;

                hostAppComponents.Disposing += delegate
                {
                    if (navigator != null)
                    {
                        // detach static event
                        Microsoft.Win32.SystemEvents.DisplaySettingsChanged -= navigator.OnDisplaySettingsChanged;
                    }
                    // Save hostAppComponents before Dispose().
                    // EgsDevice.Dispose() calls "Settings.IsToDetectFace.Value = false".
                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(hostAppComponents, Newtonsoft.Json.Formatting.Indented);
                    ZkooHostApp.Properties.Settings.Default.WholeSettingsAsJsonString = str;
                    ZkooHostApp.Properties.Settings.Default.Save();
                    if (Application.Current != null) { Application.Current.Shutdown(); }
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


                // NOTE: Codes about "Tutorial" application.
                // TODO: Tutorial application should be independent from this host application, so I wrote it as an event handler.
                var hasZkooTutorialLaunched = false;
                var zkooTutorialLaunchAction = new Action(() =>
                {
                    if (hasZkooTutorialLaunched == false)
                    {
                        zkooTutorialModel = new ZkooTutorialModel(hostAppComponents);
                        navigator = new MainNavigationWindow();
                        // attach static event
                        Microsoft.Win32.SystemEvents.DisplaySettingsChanged += navigator.OnDisplaySettingsChanged;
                        // NOTE: TODO: When exceptions occur in Initialize, we have to distinguish whether from Settings files or from source code.
                        zkooTutorialModel.TutorialAppHeaderMenu.InitializeOnceAtStartup(navigator, zkooTutorialModel);
                        zkooTutorialModel.InitializeOnceAtStartup(hostAppComponents);
                        navigator.InitializeOnceAtStartup(zkooTutorialModel);
                        hasZkooTutorialLaunched = true;
                    }
                    navigator.StartTutorial();
                });

                hostAppComponents.SettingsWindow.SettingsUserControl.TutorialAppSettingsGroupBoxVisibility = Visibility.Visible;
                hostAppComponents.StartTutorialCommand.PerformEventHandler += delegate
                {
                    zkooTutorialLaunchAction.Invoke();
                };

                Egs.BindableResources.Current.CultureChanged += delegate
                {
                    Egs.ZkooTutorial.BindableResources.Current.ChangeCulture(Egs.EgsDeviceControlCore.Properties.Resources.Culture.Name);
                };
                hostAppComponents.IsStartingDeviceFirmwareUpdate += delegate { if (navigator != null) { navigator.ExitTutorial(); } };
                hostAppComponents.IsStartingHostApplicationUpdate += delegate { if (navigator != null) { navigator.ExitTutorial(); } };

                if (hostAppComponents.IsToStartTutorialWhenHostApplicationStart)
                {
                    zkooTutorialLaunchAction.Invoke();
                }
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
                // TODO: Should make a reporting system to Exvision
                if (true)
                {
                    var window = new NotHandledExceptionReportWindow();
                    window.Initialize(ex);
                    window.ShowDialog();
                }
                using (hostAppComponents = new EgsHostAppBaseComponents())
                {
                    hostAppComponents.InitializeOnceAtStartup();
                    // NOTE: If some unknown problems occur, it does not start the tutorial application from next time.
                    hostAppComponents.IsToStartTutorialWhenHostApplicationStart = false;

                    var str = Newtonsoft.Json.JsonConvert.SerializeObject(hostAppComponents, Newtonsoft.Json.Formatting.Indented);
                    ZkooHostApp.Properties.Settings.Default.WholeSettingsAsJsonString = str;
                    ZkooHostApp.Properties.Settings.Default.Save();
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

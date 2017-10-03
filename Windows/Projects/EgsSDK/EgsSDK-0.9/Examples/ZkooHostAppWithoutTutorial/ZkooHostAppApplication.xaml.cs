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
                Egs.BindableResources.Current.CultureChanged += delegate
                {
                    ApplicationCommonSettings.HostApplicationName = Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_GestureCamera;
                };

                Egs.BindableResources.Current.ChangeCulture(ApplicationCommonSettings.DefaultCultureInfoName);

                if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
                {
                    var msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_Application0IsAlreadyRunning, ApplicationCommonSettings.HostApplicationName);
                    MessageBox.Show(msg, ApplicationCommonSettings.HostApplicationName);
                    if (Application.Current != null) { Application.Current.Shutdown(); }
                    return;
                }

                hostAppComponents = new EgsHostAppBaseComponents();
                hostAppComponents.InitializeOnceAtStartup();
                hostAppComponents.HasResetSettings += delegate
                {
                    // You can modify the application default settings here.
                    hostAppComponents.Device.Settings.CursorSpeedAndPrecisionMode.Value = Egs.PropertyTypes.CursorSpeedAndPrecisionModes.Standard;
                    hostAppComponents.Device.Settings.FaceDetectionMethod.Value = Egs.PropertyTypes.FaceDetectionMethods.DefaultProcessOnEgsHostApplication;
                };
                if (SettingsSerialization.LoadSettingsJsonFile(hostAppComponents) == false) { hostAppComponents.Reset(); }

                hostAppComponents.CameraViewWindow.Closed += delegate { hostAppComponents.Dispose(); };

                hostAppComponents.Disposing += delegate
                {
                    // NOTE: Save settings before Dispose().  Target event is not Disposed but Disposing.
                    if (hostAppComponents.CanSaveSettingsJsonFileSafely) { SettingsSerialization.SaveSettingsJsonFile(hostAppComponents); }
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
                // NOTE: Assuming that this is the correct way to shutdown the application.
                MessageBox.Show(Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_ApplicationWillExit, ApplicationCommonSettings.HostApplicationName, MessageBoxButton.OK);
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
            if (hostAppComponents != null) { hostAppComponents.Dispose(); hostAppComponents = null; }
            if (Application.Current != null) { Application.Current.Shutdown(); }
            DuplicatedProcessStartBlocking.ReleaseMutex();
        }
    }
}

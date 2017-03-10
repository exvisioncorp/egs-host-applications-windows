namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Diagnostics;
    using System.Windows;
    using System.ComponentModel;
    using System.Globalization;
    using System.Runtime.Serialization;
    using Egs;
    using Egs.DotNetUtility;
    using Egs.Views;
    using Egs.PropertyTypes;
    using Egs.EgsDeviceControlCore.Properties;

    /// <summary>
    /// Base components of EGS host application.
    /// </summary>
    [DataContract]
    public class EgsHostAppBaseComponents : EgsHostOnUserControl
    {
        public static string EgsHostApplicationName { get; set; }
        static EgsHostAppBaseComponents()
        {
            EgsHostApplicationName = "ZKOO";
        }
        public static string MessageOfOnlyOneInstanceCanRun
        {
            get
            {
                var ret = string.Format(System.Globalization.CultureInfo.InvariantCulture, Resources.CommonStrings_Application0IsAlreadyRunning, EgsHostApplicationName);
                return ret;
            }
        }

        [DataMember]
        public CameraViewWindowModel CameraViewWindowModel { get; private set; }
        public CameraViewWindow CameraViewWindow { get; private set; }
        public SettingsWindow SettingsWindow { get; private set; }
        public AppTrayIconAndMenuItemsComponent AppTrayIconAndMenuItems { get; private set; }

        public SimpleDelegateCommand CheckForEgsHostAppCoreUpdateCommand { get; private set; }
        public SimpleDelegateCommand UpdateDeviceFirmwareCommand { get; private set; }

        #region Tutorial
        [EditorBrowsable(EditorBrowsableState.Never)]
        bool _IsToStartTutorialWhenHostApplicationStart;
        public event EventHandler IsToStartTutorialWhenHostApplicationStartChanged;
        protected virtual void OnIsToStartTutorialWhenHostApplicationStartChanged(EventArgs e)
        {
            var t = IsToStartTutorialWhenHostApplicationStartChanged; if (t != null) { t(this, e); }
            OnPropertyChanged(nameof(IsToStartTutorialWhenHostApplicationStart));
        }
        [DataMember]
        public bool IsToStartTutorialWhenHostApplicationStart
        {
            get { return _IsToStartTutorialWhenHostApplicationStart; }
            set
            {
                _IsToStartTutorialWhenHostApplicationStart = value;
                OnIsToStartTutorialWhenHostApplicationStartChanged(EventArgs.Empty);
            }
        }
        public SimpleDelegateCommand StartTutorialCommand { get; private set; }
        #endregion

        public event EventHandler IsStartingDeviceFirmwareUpdate;
        public event EventHandler IsStartingHostApplicationUpdate;

        enum DeviceFirmwareUpdateConditions
        {
            UpdateIfOldForEndUsers,
            DoNotUpdateToTestOldFirmwares,
            AlwaysUpdateForDebugging
        }
        DeviceFirmwareUpdateConditions DeviceFirmwareUpdateCondition { get; set; }

        internal override void RaiseMultipleObjectsPropertyChanged()
        {
            base.RaiseMultipleObjectsPropertyChanged();
            OnPropertyChanged(nameof(CameraViewWindowModel));
        }

        public EgsHostAppBaseComponents()
            : base()
        {
            _IsToStartTutorialWhenHostApplicationStart = true;

            CameraViewWindowModel = new CameraViewWindowModel();
            CameraViewWindow = new CameraViewWindow();
            SettingsWindow = new SettingsWindow();
            AppTrayIconAndMenuItems = new AppTrayIconAndMenuItemsComponent();

            StartTutorialCommand = new SimpleDelegateCommand();
            CheckForEgsHostAppCoreUpdateCommand = new SimpleDelegateCommand();
            UpdateDeviceFirmwareCommand = new SimpleDelegateCommand();

            CheckForEgsHostAppCoreUpdateCommand.PerformEventHandler += delegate { CheckForApplicationUpdate(); };
            UpdateDeviceFirmwareCommand.PerformEventHandler += delegate
            {
                if (ApplicationCommonSettings.IsInternalRelease)
                {
                    try { StartDeviceFirmwareUpdate(); }
                    catch (EgsHostApplicationIsClosingException) { return; }
                }
                else
                {
                    MessageBox.Show("This function is used in internal release.");
                }
            };

            CheckForEgsHostAppCoreUpdateCommand.CanPerform = true;
            UpdateDeviceFirmwareCommand.CanPerform = false;

            DeviceFirmwareUpdateCondition = DeviceFirmwareUpdateConditions.UpdateIfOldForEndUsers;
            if (ApplicationCommonSettings.IsInternalRelease)
            {
                DeviceFirmwareUpdateCondition = DeviceFirmwareUpdateConditions.DoNotUpdateToTestOldFirmwares;
            }
            if (false) { DeviceFirmwareUpdateCondition = DeviceFirmwareUpdateConditions.AlwaysUpdateForDebugging; }
        }

        void CheckForApplicationUpdate()
        {
            try
            {
                var uri = new Uri(@"http://exvision.co.jp/egs/zkoo/ZkooSetupInformation_" + ApplicationCommonSettings.DefaultCultureInfoName + @".json");
                var appUpdate = new ApplicationUpdateModel(uri);
                if (appUpdate.CheckInformationFile() == false) { return; }
                var installerOnWebVersion = new Version(appUpdate.LatestInstallerInformation.Version);
                var isLatestVersionInstalled = installerOnWebVersion <= new Version(ApplicationCommonSettings.HostAppCoreDllAssemblyVersionMajorMinorBuildRevisionString);
#if DEBUG
                // MUSTDO: When you debug the application update, enable the next line.
                isLatestVersionInstalled = false;
#endif
                if (isLatestVersionInstalled)
                {
                    // NOTE: The latest version is installed.
                    var msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, Resources.CommonStrings_Application0IsLatest, EgsHostAppBaseComponents.EgsHostApplicationName);
                    MessageBox.Show(msg, EgsHostAppBaseComponents.EgsHostApplicationName, MessageBoxButton.OK);
                }
                else
                {
                    // NOTE: A newer version exists.
                    var t = IsStartingHostApplicationUpdate; if (t != null) { t(this, EventArgs.Empty); }
                    var msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, Resources.CommonStrings_CanDownloadNewApplication0, EgsHostAppBaseComponents.EgsHostApplicationName);
                    if (MessageBox.Show(msg, EgsHostAppBaseComponents.EgsHostApplicationName, MessageBoxButton.OKCancel) == MessageBoxResult.OK)
                    {
                        var appUpdateProgressWindow = new ApplicationUpdateProgressWindow();
                        appUpdateProgressWindow.DataContext = appUpdate;
                        appUpdateProgressWindow.Closing += (sender, e) => { appUpdate.DownloadWebClient.CancelAsync(); };
                        appUpdate.DownloadInstaller();
                        appUpdateProgressWindow.ShowDialog();
                    }
                }
            }
            catch (Exception ex)
            {
                if (ApplicationCommonSettings.IsDebugging) { Debugger.Break(); }
                Console.WriteLine(ex.Message);
            }
        }

        public override void InitializeOnceAtStartup()
        {
            base.InitializeOnceAtStartup();

            CameraViewWindowModel.InitializeOnceAtStartup(base.Device);
            SettingsWindow.InitializeOnceAtStartup(this);
            CameraViewWindow.InitializeOnceAtStartup(CameraViewWindowModel, CameraViewUserControlModel);

            AppTrayIconAndMenuItems.InitializeOnceAtStartup(this);

            CameraViewWindowModel.SettingsCommand.PerformEventHandler += (sender, e) =>
            {
                SettingsWindow.ToggleVisibility();
            };
            CameraViewWindowModel.ExitCommand.PerformEventHandler += (sender, e) =>
            {
                if (false)
                {
                    var message = string.Format(CultureInfo.InvariantCulture, Resources.CommonStrings_WouldYouExitApplication0, EgsHostApplicationName);
                    if (MessageBox.Show(message, Resources.CommonStrings_Confirmation, MessageBoxButton.OKCancel) != MessageBoxResult.OK) { return; }
                }
                this.Dispose();
            };

            CameraViewWindowModel.CanDragMoveChanged += delegate
            {
                // do nothing
            };

            CameraViewWindowModel.CanResizeChanged += delegate { OnCanResizeCameraViewWindowChanged(); };
            OnCanResizeCameraViewWindowChanged();

            CameraViewWindowModel.WindowStateHostApplicationsControlMethod.ValueUpdated += delegate
            {
                CameraViewWindowModel.StartCheckingIsShowingCameraViewWindow();
            };
            CameraViewWindowModel.WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods;
            CameraViewWindowModel.WindowStateUsersControlMethod.ValueUpdated += delegate
            {
                CameraViewWindowModel.StartCheckingIsShowingCameraViewWindow();
            };
            CameraViewWindowModel.WindowStateUsersControlMethod.Value = CameraViewWindowStateUsersControlMethods.ManualOnOff;

            Device.IsHidDeviceConnectedChanged += Device_IsHidDeviceConnectedChanged;
            // NOTE: Update is necessary here, too.
            if (ApplicationCommonSettings.IsInternalRelease) { UpdateDeviceFirmwareCommand.CanPerform = Device.IsHidDeviceConnected ? true : false; }

            CultureInfoAndDescription.ValueUpdated += delegate
            {
                SettingsWindow.ReloadDataContext();
                CameraViewWindow.ReloadDataContext();
            };

            CameraViewWindow.Closed += delegate
            {
                if (IsToDisposeThisWhenCameraViewWindowClosed)
                {
                    CameraViewWindow = null;
                    this.Dispose();
                }
            };

            if (ApplicationCommonSettings.IsDebugging)
            {
                SettingsWindow.Show();
            }
            IsToDisposeThisWhenCameraViewWindowClosed = true;
        }

        internal bool IsToDisposeThisWhenCameraViewWindowClosed { get; private set; }

        public override void Reset()
        {
            base.Reset();
            CameraViewWindowModel.Reset();
            IsToStartTutorialWhenHostApplicationStart = true;
        }

        void Device_IsHidDeviceConnectedChanged(object sender, EventArgs e)
        {
            CheckIfDeviceFirmwareIsLatestOrNotAndExitApplicationIfFailed();
            CameraViewWindowModel.WindowState = Device.IsHidDeviceConnected ? WindowState.Normal : WindowState.Minimized;
            if (ApplicationCommonSettings.IsInternalRelease) { UpdateDeviceFirmwareCommand.CanPerform = Device.IsHidDeviceConnected ? true : false; }
        }

        public void CheckIfDeviceFirmwareIsLatestOrNotAndExitApplicationIfFailed()
        {
            Application.Current.Dispatcher.Invoke(new Action(() =>
            {
                var hasToUpdateDeviceFirmware = GetHasToUpdateDeviceFirmware();
                if (hasToUpdateDeviceFirmware == false) { return; }
                StartDeviceFirmwareUpdate();
            }));
        }

        internal bool GetHasToUpdateDeviceFirmware()
        {
            if (Device == null) { Debugger.Break(); throw new InvalidOperationException("Device == null"); }

            if (Device.IsHidDeviceConnected == false) { return false; }
            switch (DeviceFirmwareUpdateCondition)
            {
                case DeviceFirmwareUpdateConditions.UpdateIfOldForEndUsers:
                    {
                        var firmwareVersionInImageFile = new Version(ApplicationCommonSettings.FirmwareVersionInImageFileString);
                        var ret =
                            (firmwareVersionInImageFile > Device.FirmwareVersionAsVersion)
                            && (Device.Settings.TouchInterfaceKind.Value != TouchInterfaceKinds.Mouse);
                        return ret;
                    }
                    break;
                case DeviceFirmwareUpdateConditions.DoNotUpdateToTestOldFirmwares:
                    return false;
                    break;
                case DeviceFirmwareUpdateConditions.AlwaysUpdateForDebugging:
                    return true;
                    break;
                default:
                    throw new InvalidOperationException("(DeviceFirmwareUpdateConditions)DeviceFirmwareUpdateCondition: " + DeviceFirmwareUpdateCondition + " is not defined");
            }
        }

        internal void StartDeviceFirmwareUpdate()
        {
            if (Device == null) { Debugger.Break(); throw new InvalidOperationException("Device == null"); }

            // NOTE: To call a code to exit "Tutorial" application, it raises such an ad-hoc event.
            var t = IsStartingDeviceFirmwareUpdate; if (t != null) { t(this, EventArgs.Empty); }

            var dfuUserControl = new EgsDeviceFirmwareUpdateUserControl(Device, Device.FirmwareVersionAsVersion, ApplicationCommonSettings.IsInternalRelease);

            if (dfuUserControl.Model.ShowDialogToAskAboutUpdatingDeviceFirmware() == false)
            {
                if (disposed) { return; }
                throw new EgsHostApplicationIsClosingException("firmware update is canceled.");
            }

            dfuUserControl.Model.UpdateFileListByFirmwareUpdateProtocolRevisionInDevice();

            Device.IsHidDeviceConnectedChanged -= Device_IsHidDeviceConnectedChanged;
            IsToDisposeThisWhenCameraViewWindowClosed = false;
            SettingsWindow.Close();
            CameraViewWindowModel.WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.KeepMinimized;

            var dfuWindow = new Window();
            dfuWindow.Content = dfuUserControl;
            dfuWindow.Width = 800;
            dfuWindow.WindowStyle = WindowStyle.None;
            dfuWindow.SizeToContent = SizeToContent.Height;
            dfuWindow.WindowStartupLocation = WindowStartupLocation.CenterScreen;
            dfuWindow.ResizeMode = ResizeMode.NoResize;
            dfuWindow.Title = "ZKOO - " + Resources.EgsDeviceFirmwareUpdateModel_DeviceFirmwareUpdate;
            dfuWindow.MouseLeftButtonDown += (sender, e) => { dfuWindow.DragMove(); };

            dfuUserControl.Model.ProgressReport.RunWorkerCompleted += (sender, e) =>
            {
                MessageBox.Show(dfuUserControl.Model.LastResult.Message);
                dfuWindow.Close();
            };
            dfuUserControl.Model.PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == "MessageText")
                {
                    if (dfuWindow.IsVisible == false) { return; }
                    dfuWindow.Dispatcher.Invoke(new Action(() =>
                    {
                        dfuWindow.Show();
                        dfuWindow.WindowState = WindowState.Normal;
                        dfuWindow.Activate();
                        dfuWindow.Topmost = true;
                        dfuWindow.Topmost = false;
                        dfuWindow.Focus();
                    }));
                }
            };
            dfuWindow.Closing += (sender, e) =>
            {
                // NOTE: When it completes normally, it must close windows, but it is not canceled, so it uses "IsBusy" property.
                if (dfuUserControl.Model.IsBusy
                    // NOTE: When users try to exit this process by task-bar or task manager and so on, it call dfuWindow.Close() in ProgressReport.RunWorkerCompleted.
                    && dfuUserControl.Model.IsCanceled == false)
                {
                    dfuUserControl.Model.CancelAsync();
                    e.Cancel = true;
                    return;
                }
            };

            if (ApplicationCommonSettings.IsInternalRelease == false)
            {
                dfuUserControl.Model.StartAsync();
            }

            dfuWindow.ShowDialog();

            CameraViewWindowModel.WindowStateHostApplicationsControlMethod.Value = CameraViewWindowStateHostApplicationsControlMethods.UseUsersControlMethods;

            // NOTE: Whether it completes or fails, it exits the host application.
            throw new EgsHostApplicationIsClosingException(dfuUserControl.Model.LastResult.Message);
        }

        void OnCanResizeCameraViewWindowChanged()
        {
            var isEnabled = CameraViewWindowModel.CanResize;
            var newResizeMode = isEnabled ? ResizeMode.CanResize : ResizeMode.CanMinimize;
            if (newResizeMode != CameraViewWindow.ResizeMode) { CameraViewWindow.ResizeMode = newResizeMode; }
        }

        #region IDisposable
        private bool disposed = false;
        protected override void Dispose(bool disposing)
        {
            if (disposed) { return; }
            if (disposing)
            {
                if (hasOnDisposingCalled == false) { OnDisposing(EventArgs.Empty); hasOnDisposingCalled = true; }
                if (AppTrayIconAndMenuItems != null) { AppTrayIconAndMenuItems.Dispose(); AppTrayIconAndMenuItems = null; }
                if (SettingsWindow != null) { SettingsWindow.CloseToExitApplication(); SettingsWindow = null; }
                if (CameraViewWindow != null) { CameraViewWindow.Close(); CameraViewWindow = null; }
            }
            disposed = true;
            base.Dispose(disposing);
        }
        #endregion

        internal static EgsHostAppBaseComponents EgsHostAppBaseComponentsForXamlDesign
        {
            get
            {
                var ret = new EgsHostAppBaseComponents();
                ret.InitializeOnceAtStartup();
                return ret;
            }
        }
    }
}

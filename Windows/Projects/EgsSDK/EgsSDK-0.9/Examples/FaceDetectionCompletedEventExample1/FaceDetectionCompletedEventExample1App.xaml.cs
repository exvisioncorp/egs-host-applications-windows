namespace FaceDetectionCompletedEventExample1
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using Egs;
    using Egs.DotNetUtility;

    public partial class FaceDetectionCompletedEventExample1App : Application
    {
        public EgsHostAppBaseComponents Components { get; private set; }
        public FaceDetectionCompletedEventExample1MainWindow Window { get; private set; }

        public FaceDetectionCompletedEventExample1App()
            : base()
        {
            Egs.BindableResources.Current.CultureChanged += delegate
            {
                ApplicationCommonSettings.HostApplicationName = "FaceDetectionCompletedEventExample1";
            };

            Egs.BindableResources.Current.ChangeCulture("");

            if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
            {
                var msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_Application0IsAlreadyRunning, ApplicationCommonSettings.HostApplicationName);
                MessageBox.Show(msg, ApplicationCommonSettings.HostApplicationName);
                Application.Current.Shutdown();
                return;
            }
            ApplicationCommonSettings.IsDebugging = true;

            Components = new EgsHostAppBaseComponents();
            Components.InitializeOnceAtStartup();
            Components.Device.Settings.FaceDetectionMethod.Value = Egs.PropertyTypes.FaceDetectionMethods.DefaultProcessOnEgsHostApplication;
            Components.CameraViewWindow.KeyDown += (sender, e) =>
            {
                switch (e.Key)
                {
                    case System.Windows.Input.Key.Escape:
                        Components.CameraViewWindow.Close();
                        break;
                }
            };
            Components.Device.FaceDetectionOnHost.FaceDetectionCompleted += FaceDetectionOnHost_FaceDetectionCompleted;

            Window = new FaceDetectionCompletedEventExample1MainWindow();

            this.Exit += delegate
            {
                Window.Close();
                if (Components != null) { Components.Dispose(); Components = null; }
                DuplicatedProcessStartBlocking.ReleaseMutex();
            };
        }

        bool isWindowOpened = false;
        async void FaceDetectionOnHost_FaceDetectionCompleted(object sender, EventArgs e)
        {
            if (Components.Device.FaceDetectionOnHost.IsFaceDetected == false) { return; }
            if (isWindowOpened == false)
            {
                await Task.Run(() =>
                {
                    isWindowOpened = true;
                    MessageBox.Show("Face is detected");
                    isWindowOpened = false;
                });
            }
        }
    }
}

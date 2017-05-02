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
            Egs.BindableResources.Current.ChangeCulture("");
            //Egs.BindableResources.Current.ChangeCulture("en");
            //Egs.BindableResources.Current.ChangeCulture("ja");
            //Egs.BindableResources.Current.ChangeCulture("zh-Hans");

            if (DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
            {
                MessageBox.Show(EgsHostAppBaseComponents.MessageOfOnlyOneInstanceCanRun);
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

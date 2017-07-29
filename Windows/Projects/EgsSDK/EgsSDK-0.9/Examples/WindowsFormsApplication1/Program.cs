namespace WindowsFormsApplication1
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Windows.Forms;
    using Egs;

    static class Program
    {
        /// <summary>
        /// アプリケーションのメイン エントリ ポイントです。
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Egs.BindableResources.Current.CultureChanged += delegate
            {
                ApplicationCommonSettings.HostApplicationName = Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_GestureCamera;
            };

            Egs.BindableResources.Current.ChangeCulture(ApplicationCommonSettings.DefaultCultureInfoName);

            if (Egs.DotNetUtility.DuplicatedProcessStartBlocking.TryGetMutexOnTheBeginningOfApplicationConstructor() == false)
            {
                var msg = string.Format(System.Globalization.CultureInfo.InvariantCulture, Egs.EgsDeviceControlCore.Properties.Resources.CommonStrings_Application0IsAlreadyRunning, ApplicationCommonSettings.HostApplicationName);
                MessageBox.Show(msg, ApplicationCommonSettings.HostApplicationName);
                return;
            }

            Application.Run(new Form1());

            Egs.DotNetUtility.DuplicatedProcessStartBlocking.ReleaseMutex();
        }
    }
}

namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.Diagnostics;
    using System.IO;
    using System.Runtime.Serialization;
    using System.ComponentModel;
    using Egs;
    using Egs.DotNetUtility;

    [DataContract]
    class LaunchingOtherApplicationButtonModel : TextButtonModel
    {
        [DataMember]
        public string ProcessStartInfoFileName { get; set; }
        [DataMember]
        public string ProcessStartInfoWorkingDirectory { get; set; }
        [DataMember]
        public string ProcessStartInfoArguments { get; set; }

        public Process LatestProcess { get; private set; }

        public LaunchingOtherApplicationButtonModel() : base() { }

        public void StartProcess()
        {
            var startInfo = new ProcessStartInfo();
            startInfo.FileName = ProcessStartInfoFileName;
            startInfo.WorkingDirectory = ProcessStartInfoWorkingDirectory;
            startInfo.Arguments = ProcessStartInfoArguments;
            if (false)
            {
                startInfo.CreateNoWindow = true;
                startInfo.UseShellExecute = false;
            }
            try { LatestProcess = Process.Start(startInfo); }
            catch (Exception ex) { MessageBox.Show(ex.Message, "The launcher could not start the application."); }
        }
    }
}

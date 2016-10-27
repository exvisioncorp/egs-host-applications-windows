namespace Egs.EgsSourceCodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Data;
    using System.Windows.Documents;
    using System.Windows.Input;
    using System.Windows.Media;
    using System.Windows.Media.Imaging;
    using System.Windows.Navigation;
    using System.Windows.Shapes;
    using System.IO;
    using Microsoft.Win32;

    partial class EgsDeviceSettingsGenerationMainWindow : Window
    {
        EgsDeviceHidAccessPropertiesGeneration EgsDeviceHidAccessPropertiesGeneration = new EgsDeviceHidAccessPropertiesGeneration();
        ZkooTutorialNarrationInformationsGeneration ZkooTutorialNarrationInformationsGeneration = new ZkooTutorialNarrationInformationsGeneration();

        public EgsDeviceSettingsGenerationMainWindow()
        {
            InitializeComponent();

            GenerateHostAppSettingsFromCsvFileButton.Click += delegate
            {
                if (EgsDeviceHidAccessPropertiesGeneration.LoadXlsxFile() == false) { return; }
                EgsDeviceHidAccessPropertiesGeneration.SaveCSharpFile();
            };
            GenerateZkooTutorialNarrationInformationsButton.Click += delegate
            {
                ZkooTutorialNarrationInformationsGeneration.Convert();
            };
            GenerateDeviceControlCoreResourcesButton.Click += delegate
            {
                var obj = new ZkooHostAppLocalizedStringResourcesGeneration();
                obj.Convert();
            };
        }
    }
}

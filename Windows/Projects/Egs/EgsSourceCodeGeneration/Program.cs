namespace Egs.EgsSourceCodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;

    static class Program
    {
        [STAThread]
        static void Main()
        {
            CreateFiles();
        }

        static void CreateFiles()
        {
            EgsDeviceHidAccessPropertiesGeneration EgsDeviceHidAccessPropertiesGeneration = new EgsDeviceHidAccessPropertiesGeneration();
            ZkooTutorialNarrationInformationsGeneration ZkooTutorialNarrationInformationsGeneration = new ZkooTutorialNarrationInformationsGeneration();
            ZkooHostAppLocalizedStringResourcesGeneration ZkooHostAppLocalizedStringResourcesGeneration = new ZkooHostAppLocalizedStringResourcesGeneration();
            //EgsDeviceSettingsTypeIdsGeneration EgsDeviceSettingsTypeIdsGeneration = new EgsDeviceSettingsTypeIdsGeneration();

            bool hr = EgsDeviceHidAccessPropertiesGeneration.LoadXlsxFile();
            hr = hr && EgsDeviceHidAccessPropertiesGeneration.SaveCSharpFile();
            hr = hr && ZkooTutorialNarrationInformationsGeneration.Convert();
            hr = hr && ZkooHostAppLocalizedStringResourcesGeneration.Convert();
            if (hr) { Console.WriteLine("All conversion has been completed!"); }
        }
    }
}

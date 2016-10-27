namespace Egs.EgsSourceCodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Configuration;
    using System.Data;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;

    public partial class EgsSourceCodeGenerationApplication : Application
    {
        EgsDeviceHidAccessPropertiesGeneration EgsDeviceHidAccessPropertiesGeneration = new EgsDeviceHidAccessPropertiesGeneration();
        ZkooTutorialNarrationInformationsGeneration ZkooTutorialNarrationInformationsGeneration = new ZkooTutorialNarrationInformationsGeneration();
        ZkooHostAppLocalizedStringResourcesGeneration ZkooHostAppLocalizedStringResourcesGeneration = new ZkooHostAppLocalizedStringResourcesGeneration();
        //EgsDeviceSettingsTypeIdsGeneration EgsDeviceSettingsTypeIdsGeneration = new EgsDeviceSettingsTypeIdsGeneration();

        public EgsSourceCodeGenerationApplication()
        {
            bool hr = EgsDeviceHidAccessPropertiesGeneration.LoadXlsxFile();
            hr = hr && EgsDeviceHidAccessPropertiesGeneration.SaveCSharpFile();
            hr = hr && ZkooTutorialNarrationInformationsGeneration.Convert();
            hr = hr && ZkooHostAppLocalizedStringResourcesGeneration.Convert();
            if (hr) { Console.WriteLine("All conversion has been completed!"); }
            Application.Current.Shutdown(hr ? 0 : -1);
        }
    }
}

namespace DotNetUtility
{
    using System;
    using System.Diagnostics;

    /// <summary>
    /// Save [DataMember] properties with Json.NET
    /// </summary>
    public static class SettingsSerialization
    {
        public static string GetDefaultSettingsFolderPath()
        {
            var assemblyInfo = System.Reflection.Assembly.GetEntryAssembly();
            var assemblyInfoGetName = assemblyInfo.GetName();

            var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
            var companyName = ((System.Reflection.AssemblyCompanyAttribute)(Attribute.GetCustomAttribute(assemblyInfo, typeof(System.Reflection.AssemblyCompanyAttribute)))).Company;
            foreach (var c in System.IO.Path.GetInvalidPathChars()) { companyName = companyName.Replace(c.ToString(), ""); }
            companyName = companyName.Replace(" ", "_");
            var assemblyName = assemblyInfoGetName.Name;
            var version = assemblyInfoGetName.Version.ToString();
            var settingsFileFolderPath = "";
            settingsFileFolderPath = System.IO.Path.Combine(settingsFileFolderPath, appDataFolderPath);
            settingsFileFolderPath = System.IO.Path.Combine(settingsFileFolderPath, companyName);
            settingsFileFolderPath = System.IO.Path.Combine(settingsFileFolderPath, assemblyName);
            settingsFileFolderPath = System.IO.Path.Combine(settingsFileFolderPath, version);
            return settingsFileFolderPath;
        }

        public static string GetDefaultSettingsFileName()
        {
            var assemblyInfo = System.Reflection.Assembly.GetEntryAssembly();
            var assemblyInfoGetName = assemblyInfo.GetName();
            var assemblyName = assemblyInfoGetName.Name;
            var settingsFileName = assemblyName + "_Settings.json";
            return settingsFileName;
        }

        public static string GetDefaultSettingsFilePath()
        {
            var ret = "";
            ret = System.IO.Path.Combine(GetDefaultSettingsFolderPath(), GetDefaultSettingsFileName());
            return ret;
        }

        public static bool SaveSettingsJsonFile(object obj)
        {
            return SaveSettingsJsonFile(obj, GetDefaultSettingsFilePath());
        }

        public static bool SaveSettingsJsonFile(object obj, string path)
        {
            try
            {
                var contents = Newtonsoft.Json.JsonConvert.SerializeObject(obj, Newtonsoft.Json.Formatting.Indented);
                System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(path));
                System.IO.File.WriteAllText(path, contents);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool DeleteSettingsJsonFile()
        {
            try
            {
                var path = GetDefaultSettingsFilePath();
                System.IO.File.Delete(path);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public static bool LoadSettingsJsonFile(object obj)
        {
            try
            {
                var path = GetDefaultSettingsFilePath();
                if (System.IO.File.Exists(path) == false) { return false; }
                var contents = System.IO.File.ReadAllText(path);

                if (false)
                {
                    // TODO: Fix a problem that ObservableCollection can be duplicated.
                    var jss = new Newtonsoft.Json.JsonSerializerSettings();
                    // NOTE: If it is set to "Error", just unknown parameters in Json file can occur exceptions.
                    // To absorb the difference of the version of the Settings object, let it cancel the exception.
                    // Save the settings by the latest format, when the application exits.
                    //jss.MissingMemberHandling = Newtonsoft.Json.MissingMemberHandling.Error; // default == Ignore
                    // NOTE: When there is not the description of the value in Json or the value is null, settings in constructors are used.  It can solve problems in JSON file by the settings in constructor.
                    // default == Include
                    jss.NullValueHandling = Newtonsoft.Json.NullValueHandling.Ignore;
                    // NOTE: I don't understand how "Reuse" works, so I don't use it.
                    // TODO: Check the behavior.
                    //jss.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Reuse; // default == Auto
                    jss.ObjectCreationHandling = Newtonsoft.Json.ObjectCreationHandling.Replace;
                    // Now I use not Deserialize but PopulateObject.
                }

                Newtonsoft.Json.JsonConvert.PopulateObject(contents, obj);
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

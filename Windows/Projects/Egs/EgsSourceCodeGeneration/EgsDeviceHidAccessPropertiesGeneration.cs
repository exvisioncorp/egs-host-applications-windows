namespace Egs.EgsSourceCodeGeneration
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.IO;
    using Microsoft.Win32;
    using NPOI.XSSF.UserModel;
    using DotNetUtility;
    using Egs;

    public class EgsDeviceHidAccessPropertiesGeneration
    {
        // NOTE: Build this project with "Any CPU" configuration.
        static readonly string InputFilePath = Environment.CurrentDirectory + "\\..\\..\\..\\..\\..\\..\\Documents\\UsbProtocol_EgsDeviceSettingsHidReport_HostAppSettings_PropertyList.xlsx";
        static readonly string OutputFilePath = Environment.CurrentDirectory + "\\..\\..\\..\\..\\Egs\\EgsDeviceControlCore\\EgsDeviceHidAccessProperties.generated.cs";

        public List<EgsDeviceHidAccessPropertyOneRecord> InputRecordList { get; set; }
        public Dictionary<string, List<ResourcesResXInformationOneRecord>> Culture_Resources_Dict { get; set; }

        public EgsDeviceHidAccessPropertiesGeneration()
        {
        }

        public bool LoadXlsxFile()
        {
            try
            {
                var newInputRecordList = new List<EgsDeviceHidAccessPropertyOneRecord>();
                var newCulture_Resources_Dict = new Dictionary<string, List<ResourcesResXInformationOneRecord>>();

                var headerCellString_ColumnIndex_Dict = new Dictionary<string, int>();
                var cultureList = new List<string>();
                using (var inputStream = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
                {
                    var book = new XSSFWorkbook(inputStream);
                    var sheet = book.GetSheetAt(0);
                    var header = sheet.GetRow(0);
                    for (int colIndex = 0; colIndex < header.LastCellNum; colIndex++)
                    {
                        var headerCellString = header.GetCell(colIndex).ToString();
                        headerCellString_ColumnIndex_Dict[headerCellString] = colIndex;
                        if (headerCellString.Contains("Options_")) { cultureList.Add(headerCellString.Replace("Options_", "")); }
                    }

                    foreach (var culture in cultureList)
                    {
                        newCulture_Resources_Dict[culture] = new List<ResourcesResXInformationOneRecord>();
                    }

                    var h = new EgsDeviceHidAccessPropertyOneRecord();
                    for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                    {
                        var item = new EgsDeviceHidAccessPropertyOneRecord();
                        var row = sheet.GetRow(rowIndex);
                        var cells = row.Cells;
                        item.OwnerClass = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.OwnerClass));
                        item.ValueTypeOnHost = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.ValueTypeOnHost));
                        item.ValueNameOnHost = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.ValueNameOnHost));
                        item.ReportId = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.ReportId));
                        item.MessageId = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.MessageId));
                        item.CategoryId = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.CategoryId));
                        item.PropertyId = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.PropertyId));
                        item.ValueTypeOnDevice = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.ValueTypeOnDevice));
                        int tempDataLength = 0;
                        var hr = int.TryParse(row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.DataLength)), out tempDataLength);
                        System.Diagnostics.Debug.Assert(hr);
                        item.DataLength = tempDataLength;
                        item.IsDataMember = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.IsDataMember)) == "true";
                        item.AccessModifierInFuture = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.AccessModifierInFuture));
                        item.IsReadOnly = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.IsReadOnly)) == "true";
                        item.AvailableFirmwareVersion = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.AvailableFirmwareVersion));
                        item.AccessModifierInLatestSdkForWindows = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.AccessModifierInLatestSdkForWindows));
                        item.PropertyInitializationOnWindows = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.PropertyInitializationOnWindows));

                        item.Language_DescriptionAndOptions_Dict = new Dictionary<string, DescriptionAndOptionsInOneLanguage>();
                        foreach (var culture in cultureList)
                        {
                            var daoiol = new DescriptionAndOptionsInOneLanguage();
                            daoiol.PropertyDescription = row.GetCellString(headerCellString_ColumnIndex_Dict, "Description_" + culture);
                            if (item.IsHidAccessPropertyOptional || item.IsEnumValueWithDescription)
                            {
                                var optionsString = row.GetCellString(headerCellString_ColumnIndex_Dict, "Options_" + culture);
                                var optionStringArray = optionsString.Split('\n');
                                daoiol.OptionalByteValueAndDescriptionList = new List<OptionalByteValueAndDescription>();
                                foreach (var optionString in optionStringArray)
                                {
                                    var indexOfFirstColon = optionString.IndexOf(':');
                                    var option = new OptionalByteValueAndDescription();
                                    var byteValueString = optionString.Substring(0, indexOfFirstColon);
                                    option.Value = byte.Parse(byteValueString);
                                    option.Description = optionString.Substring(indexOfFirstColon + 1);
                                    daoiol.OptionalByteValueAndDescriptionList.Add(option);
                                }
                            }
                            item.Language_DescriptionAndOptions_Dict[culture] = daoiol;
                        }

                        foreach (var culture in cultureList)
                        {
                            var descriptionResourcesResXInformationOneRecord = new ResourcesResXInformationOneRecord();
                            descriptionResourcesResXInformationOneRecord.Key = item.DescriptionKey;
                            descriptionResourcesResXInformationOneRecord.Value = item.Language_DescriptionAndOptions_Dict[culture].PropertyDescription;
                            descriptionResourcesResXInformationOneRecord.Comment = item.Language_DescriptionAndOptions_Dict[""].PropertyDescription;
                            newCulture_Resources_Dict[culture].Add(descriptionResourcesResXInformationOneRecord);
                            foreach (var option in item.Language_DescriptionAndOptions_Dict[culture].OptionalByteValueAndDescriptionList)
                            {
                                var optionResourcesResXInformationOneRecord = new ResourcesResXInformationOneRecord();
                                optionResourcesResXInformationOneRecord.Key = item.ValueNameOnHost + "Detail_Value" + option.Value + "_Description";
                                optionResourcesResXInformationOneRecord.Value = option.Description;
                                optionResourcesResXInformationOneRecord.Comment = item.Language_DescriptionAndOptions_Dict[""].OptionalByteValueAndDescriptionList.Single(e => e.Value == option.Value).Description;
                                newCulture_Resources_Dict[culture].Add(optionResourcesResXInformationOneRecord);
                            }
                        }

                        newInputRecordList.Add(item);
                    }
                }
                InputRecordList = newInputRecordList;
                Culture_Resources_Dict = newCulture_Resources_Dict;
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
                Console.WriteLine(ex.Message);
                return false;
            }
        }

        public string OneClassCodeString(IEnumerable<EgsDeviceHidAccessPropertyOneRecord> propertyInformationList)
        {
            var ret = "";

            try
            {
                var ownerClassColumn = propertyInformationList.Select(e => e.OwnerClass).ToList();
                var distinct = ownerClassColumn.Distinct();
                var single = distinct.Single();
                ret += @"
    public partial class " + single + @"
    {
";
            }
            catch { System.Diagnostics.Debugger.Break(); throw; }

            // property definitions
            foreach (var item in propertyInformationList)
            {
                try { ret += item.GetCodeOfPropertyDefinition(); }
                catch { System.Diagnostics.Debugger.Break(); throw; }
            }

            // creating property object
            ret +=
        @"
        void CreateProperties()
        {
";
            foreach (var item in propertyInformationList)
            {
                try { ret += item.GetCodeOfCreatingPropertyObject(); }
                catch { System.Diagnostics.Debugger.Break(); throw; }
            }
            ret +=
        @"        }
";
            // add properties to DevicePropertyList
            ret += @"
        void AddPropertiesToHidAccessPropertyList()
        {
            HidAccessPropertyList = new List<HidAccessPropertyBase>();
";
            foreach (var item in propertyInformationList)
            {
                try { ret += item.GetCodeOfAddingPropertiesToPropertyList(); }
                catch { System.Diagnostics.Debugger.Break(); throw; }
            }
            ret +=
        @"        }
";

            // initialize by default value
            ret += @"
        internal void InitializePropertiesByDefaultValue()
        {
";
            foreach (var item in propertyInformationList)
            {
                try { ret += item.GetCodeOfInitializationByDefaultValue(); }
                catch { System.Diagnostics.Debugger.Break(); throw; }
            }
            ret +=
        @"        }
";

            // end of class definition
            ret +=
        @"    }
";
            return ret;
        }

        public bool SaveCSharpFile()
        {
            try
            {
                var firmwareVersion = new Version(ApplicationCommonSettings.FirmwareVersionInImageFileString);
                using (var writer = new System.IO.StreamWriter(OutputFilePath))
                {
                    var EgsDevicePropertiesAll = InputRecordList.Where(e => e.OwnerClass == "EgsDevice").Where(e => string.IsNullOrEmpty(e.AccessModifierInLatestSdkForWindows) == false);
                    var EgsDeviceSettingsPropertiesAll = InputRecordList.Where(e => e.OwnerClass == "EgsDeviceSettings").Where(e => string.IsNullOrEmpty(e.AccessModifierInLatestSdkForWindows) == false);
                    IEnumerable<EgsDeviceHidAccessPropertyOneRecord> EgsDeviceProperties = null;
                    IEnumerable<EgsDeviceHidAccessPropertyOneRecord> EgsDeviceSettingsProperties = null;
                    if (ApplicationCommonSettings.IsInternalRelease == false)
                    {
                        EgsDeviceProperties = EgsDevicePropertiesAll.Where(e => new Version(e.AvailableFirmwareVersion) <= firmwareVersion);
                        EgsDeviceSettingsProperties = EgsDeviceSettingsPropertiesAll.Where(e =>
                        {
                            try
                            {
                                var version = new Version(e.AvailableFirmwareVersion);
                                return version <= firmwareVersion;
                            }
                            catch (Exception ex)
                            {
                                System.Diagnostics.Debugger.Break();
                                Console.WriteLine(ex.Message);
                                throw;
                            }
                        });
                    }
                    else
                    {
                        EgsDeviceProperties = EgsDevicePropertiesAll;
                        EgsDeviceSettingsProperties = EgsDeviceSettingsPropertiesAll;
                    }
                    var EgsHostSettingsProperties = InputRecordList.Where(e => e.OwnerClass == "EgsHostSettings");

                    var code =
        @"namespace Egs
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Egs.EgsDeviceControlCore.Properties;
    using Egs.DotNetUtility;
    using Egs.PropertyTypes;
";
                    code += OneClassCodeString(EgsDeviceProperties);
                    code += OneClassCodeString(EgsDeviceSettingsProperties);
                    if (false) { code += OneClassCodeString(EgsHostSettingsProperties); }
                    code +=
        @"}
";
                    writer.Write(code);
                }
                Console.WriteLine("Completed!");
                return true;
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debugger.Break();
                Console.WriteLine(ex.Message);
                return false;
            }
        }
    }
}

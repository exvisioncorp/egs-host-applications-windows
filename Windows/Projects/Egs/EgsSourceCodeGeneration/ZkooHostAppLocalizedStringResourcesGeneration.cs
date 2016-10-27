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
    using System.Globalization;
    using System.Diagnostics;
    using System.IO;
    using Microsoft.Win32;
    using System.Windows.Markup;
    using NPOI.XSSF.UserModel;
    using DotNetUtility;

    public class HostApplicationsResourcesStringsOneRecord
    {
        public int Index { get; set; }
        public bool UseOnWindows { get; set; }
        public bool UseOnAndroid { get; set; }
        public string RelativeFolderPath { get; set; }
        public string Namespace { get; set; }
        public string OwnerClass { get; set; }
        public string TargetObjectIdentifier00 { get; set; }
        public string TargetObjectIdentifier01 { get; set; }
        public string Property { get; set; }
        public string ResourceKeyOnWindows { get; set; }
        public List<string> ValueList { get; set; }
        public HostApplicationsResourcesStringsOneRecord()
        {
            ValueList = new List<string>();
        }
    }

    public class PartialClassPropertyDescriptionInformation
    {
        public class OnePartialClass
        {
            public string ClassName { get; set; }
            public List<HostApplicationsResourcesStringsOneRecord> InformationForPropertyDefinitionList { get; set; }
        }
        public class OneNamespace
        {
            public string NamespaceName { get; set; }
            public List<OnePartialClass> PartialClassList { get; set; }
        }
        public string DefinitionCSharpFileFullPath { get; set; }
        public List<OneNamespace> NamespaceList { get; set; }
    }

    public class ZkooHostAppLocalizedStringResourcesGeneration
    {
        static readonly string InputFilePath = Environment.CurrentDirectory + "\\..\\..\\..\\..\\..\\..\\Documents\\HostApplications_Resources_Strings.xlsx";

        EgsDeviceHidAccessPropertiesGeneration EgsDeviceHidAccessPropertiesGeneration { get; set; }
        List<ResourcesResXInformation> ResourcesResXInformationFromHidAccessPropertiesList { get; set; }

        List<HostApplicationsResourcesStringsOneRecord> InputRecordList { get; set; }
        Dictionary<string, List<HostApplicationsResourcesStringsOneRecord>> RelativeFolderPath_GroupedInputList_Dict { get; set; }
        Dictionary<string, ResourcesResXInformation> ResourceFilePath_ResourcesResXInformation_Dict { get; set; }
        List<PartialClassPropertyDescriptionInformation> PartialClassPropertyDescriptionInformationList { get; set; }

        public bool Convert()
        {
            try
            {
                ResourceFilePath_ResourcesResXInformation_Dict = new Dictionary<string, ResourcesResXInformation>();
                CreateList();
                CreateListFromDeviceHidProperties();
                foreach (var item in ResourceFilePath_ResourcesResXInformation_Dict.Values) { item.OverwriteResourcesFile(); }
                //OverwriteCSharpFiles();
                Console.WriteLine("Completed!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void CreateList()
        {
            InputRecordList = new List<HostApplicationsResourcesStringsOneRecord>();
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
                    if (headerCellString.Contains("Value"))
                    {
                        cultureList.Add(headerCellString.Replace("Value_", ""));
                    }
                }

                var h = new HostApplicationsResourcesStringsOneRecord();
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var item = new HostApplicationsResourcesStringsOneRecord();
                    var row = sheet.GetRow(rowIndex);
                    var cells = row.Cells;
                    item.Index = int.Parse(row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.Index)));
                    item.UseOnWindows = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.UseOnWindows)) == "true";
                    item.UseOnAndroid = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.UseOnAndroid)) == "true";
                    item.RelativeFolderPath = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.RelativeFolderPath));
                    item.Namespace = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.Namespace));
                    item.OwnerClass = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.OwnerClass));
                    item.TargetObjectIdentifier00 = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.TargetObjectIdentifier00));
                    item.TargetObjectIdentifier01 = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.TargetObjectIdentifier01));
                    item.Property = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.Property));
                    item.ResourceKeyOnWindows = row.GetCellString(headerCellString_ColumnIndex_Dict, Name.Of(() => h.ResourceKeyOnWindows));
                    foreach (var culture in cultureList)
                    {
                        var value = row.GetCellString(headerCellString_ColumnIndex_Dict, "Value_" + culture);
                        item.ValueList.Add(value);
                    }
                    InputRecordList.Add(item);
                }
            }


            var query01 = InputRecordList.Where(e => e.UseOnWindows).GroupBy(e => e.RelativeFolderPath);
            RelativeFolderPath_GroupedInputList_Dict = query01.ToDictionary(e => e.Key, e => e.ToList());

            foreach (var RelativeFolderPath_GroupedInputList_Dict_KeyValue in RelativeFolderPath_GroupedInputList_Dict)
            {
                int cultureIndex = 0;
                foreach (var culture in cultureList)
                {
                    var path = System.IO.Path.GetDirectoryName(InputFilePath) + "/" + RelativeFolderPath_GroupedInputList_Dict_KeyValue.Key;
                    path += @"/Resources";
                    if (string.IsNullOrEmpty(culture) == false) { path += "." + culture; }
                    path += ".resx";

                    var resXInfo = new ResourcesResXInformation();
                    resXInfo.ResourceResXFileFullPath = System.IO.Path.GetFullPath(path);
                    resXInfo.ResourcesResXInformationOneRecordList = RelativeFolderPath_GroupedInputList_Dict_KeyValue.Value.Select(e => new ResourcesResXInformationOneRecord()
                    {
                        Key = e.ResourceKeyOnWindows,
                        Value = e.ValueList[cultureIndex],
                        Comment = e.ValueList[0]
                    }).ToList();

                    ResourceFilePath_ResourcesResXInformation_Dict[resXInfo.ResourceResXFileFullPath] = resXInfo;
                    cultureIndex++;
                }
            }

            PartialClassPropertyDescriptionInformationList = RelativeFolderPath_GroupedInputList_Dict.Select(e1 => new PartialClassPropertyDescriptionInformation()
            {
                DefinitionCSharpFileFullPath = System.IO.Path.GetFullPath(System.IO.Path.GetDirectoryName(InputFilePath) + "/" + e1.Key + @"/GetPropertyFromResources.generated.cs"),
                NamespaceList = e1.Value.GroupBy(e2 => e2.Namespace).Select(e3 => new PartialClassPropertyDescriptionInformation.OneNamespace()
                {
                    NamespaceName = e3.Key,
                    PartialClassList = e3.GroupBy(e4 => e4.OwnerClass).Select(e5 => new PartialClassPropertyDescriptionInformation.OnePartialClass()
                    {
                        ClassName = e5.Key,
                        InformationForPropertyDefinitionList = e5.ToList()
                    }).ToList()
                }).ToList()
            }).ToList();
        }

        public void CreateListFromDeviceHidProperties()
        {
            var query01 = ResourceFilePath_ResourcesResXInformation_Dict.Where(e => System.IO.Path.GetDirectoryName(e.Key).Contains("DeviceControlCore"));
            EgsDeviceHidAccessPropertiesGeneration = new EgsSourceCodeGeneration.EgsDeviceHidAccessPropertiesGeneration();
            EgsDeviceHidAccessPropertiesGeneration.LoadXlsxFile();
            foreach (var culture in EgsDeviceHidAccessPropertiesGeneration.Culture_Resources_Dict.Keys)
            {
                var addingResources = EgsDeviceHidAccessPropertiesGeneration.Culture_Resources_Dict[culture];
                var fileName = "Resources" + (string.IsNullOrEmpty(culture) ? "" : "." + culture) + ".resx";
                var targetResources = query01.Single(e => e.Key.Contains(fileName)).Value.ResourcesResXInformationOneRecordList;
                targetResources.AddRange(addingResources);
            }
        }

        public void OverwriteCSharpFiles()
        {
            foreach (var file in PartialClassPropertyDescriptionInformationList)
            {
                using (var writer = new StreamWriter(file.DefinitionCSharpFileFullPath, false, Encoding.UTF8))
                {
                    var code =
@"using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
";
                    foreach (var oneNamespace in file.NamespaceList)
                    {
                        code += @"
namespace " + oneNamespace.NamespaceName + @"
{";
                        foreach (var onePartialClass in oneNamespace.PartialClassList)
                        {
                            code += @"
    partial class " + onePartialClass.ClassName + @"
    {";
                            foreach (var oneProperty in onePartialClass.InformationForPropertyDefinitionList)
                            {
                                var propertyIdentifier = oneProperty.ResourceKeyOnWindows.Replace(oneProperty.OwnerClass + "_", "");
                                string str = string.Format(@"
        public string {0} {{ get {{ return Properties.Resources.{1}; }} }}", propertyIdentifier, oneProperty.ResourceKeyOnWindows);
                                code += str;
                            }
                            code += @"
    }";
                        }
                        code += @"
}";
                    }
                    writer.Write(code);
                }
            }
        }
    }
}

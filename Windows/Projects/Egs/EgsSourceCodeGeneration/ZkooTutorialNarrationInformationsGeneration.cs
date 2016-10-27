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
    using System.Windows.Markup;
    using NPOI.XSSF.UserModel;

    class ZkooTutorialNarrationInformationsGeneration
    {
        static readonly string InputFilePath = Environment.CurrentDirectory + "\\..\\..\\..\\..\\..\\..\\Documents\\ZkooTutorial\\ZkooTutorialMultilingualNarrationStringTable.xlsx";
        static readonly string ExportCsFilePath = Environment.CurrentDirectory + "\\..\\..\\..\\..\\Egs\\ZkooTutorial\\NarrationInformationList.generated.cs";
        static readonly string ExportResxFileBasePath = Environment.CurrentDirectory + "\\..\\..\\..\\..\\Egs\\ZkooTutorial\\Properties\\NarrationTexts";
        List<NarrationInformationOneRecord> NarrationInformationList;

        public ZkooTutorialNarrationInformationsGeneration()
        {
        }

        public bool Convert()
        {
            try
            {
                CreateList();
                SaveResxFiles();
                SaveCSharpFile();
                Console.WriteLine("Completed!");
                return true;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }
        }

        void CreateList()
        {
            NarrationInformationList = new List<NarrationInformationOneRecord>();
            using (var inputStream = new FileStream(InputFilePath, FileMode.Open, FileAccess.Read))
            {
                var book = new XSSFWorkbook(inputStream);
                var sheet = book.GetSheetAt(0);
                for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    var row = sheet.GetRow(rowIndex);
                    if (row.GetCell(0) == null || string.IsNullOrEmpty(row.GetCell(0).StringCellValue)) { continue; }

                    var interfaceOneAbstractProperty = new NarrationInformationOneRecord();
                    var item = new NarrationInformationOneRecord();

                    interfaceOneAbstractProperty.ViewIndex = int.Parse(row.GetCell(0).StringCellValue);
                    item.ViewIndex = interfaceOneAbstractProperty.ViewIndex;

                    interfaceOneAbstractProperty.MessageIndex = int.Parse(row.GetCell(2).StringCellValue);
                    item.MessageIndex = interfaceOneAbstractProperty.MessageIndex;

                    interfaceOneAbstractProperty.SubIndex = int.Parse(row.GetCell(3).StringCellValue);
                    item.SubIndex = interfaceOneAbstractProperty.SubIndex;

                    var playSound = row.GetCell(4);
                    var waitCompletion = row.GetCell(5);
                    var playSE = row.GetCell(6);
                    var description = row.GetCell(7);

                    var jaTextCell = row.GetCell(8);
                    interfaceOneAbstractProperty.Culture_Text_Dict[""] = (jaTextCell != null) ? jaTextCell.StringCellValue : "";
                    item.Culture_Text_Dict["ja"] = (jaTextCell != null) ? jaTextCell.StringCellValue : "";
                    var enTextCell = row.GetCell(9);
                    item.Culture_Text_Dict[""] = (enTextCell != null) ? enTextCell.StringCellValue : "";
                    var zhTextCell = row.GetCell(10);
                    item.Culture_Text_Dict["zh-Hans"] = (zhTextCell != null) ? zhTextCell.StringCellValue : "";

                    NarrationInformationList.Add(item);
                }
            }
        }

        void SaveResxFiles()
        {
            var cultures = NarrationInformationList.SelectMany(e => e.Culture_Text_Dict.Keys).Distinct();
            foreach (var culture in cultures)
            {
                var narrationResX = new ResourcesResXInformation();
                narrationResX.ResourceResXFileFullPath = ExportResxFileBasePath + ResourcesResXInformation.GetResXFileExtension(culture);
                narrationResX.ResourcesResXInformationOneRecordList = NarrationInformationList.Select(e => new ResourcesResXInformationOneRecord()
                {
                    Key = e.PropertyIdentifier + "_TextKey",
                    Value = e.Culture_Text_Dict[culture],
                    Comment = e.Culture_Text_Dict[""]
                }).ToList();
                narrationResX.OverwriteResourcesFile();
            }
        }

        void SaveCSharpFile()
        {
            using (var writer = new StreamWriter(ExportCsFilePath))
            {
                var code =
@"namespace Egs.ZkooTutorial
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.ComponentModel;
    using System.Collections.ObjectModel;
    using System.Runtime.Serialization;

    class NarrationInformationList
    {
";
                foreach (var item in NarrationInformationList) { code += item.GetCodeOfNarrationInformationProperty(); }
                code +=
@"        public NarrationInformationList()
        {
";
                foreach (var item in NarrationInformationList) { code += item.GetCodeOfNarrationInformationPropertyConstructor(); }
                code +=
@"        }
";
                code +=
@"    }
}
";
                writer.Write(code);
            }
        }
    }

    class NarrationInformationOneRecord
    {
        static string[] invalidPathCharStrings = System.IO.Path.GetInvalidPathChars().Select(e => e.ToString()).ToArray();

        public int ViewIndex { get; set; }
        public int MessageIndex { get; set; }
        public int SubIndex { get; set; }
        public Dictionary<string, string> Culture_Text_Dict { get; set; }
        public string PropertyIdentifier
        {
            get
            {
                var ret = "";
                ret += string.Format(@"View{0:D3}_Message{1:D3}{2:D2}", ViewIndex, MessageIndex, SubIndex);
                return ret;
            }
        }
        string escapedText { get { return Culture_Text_Dict[""].Replace("\"", "\"\""); } }

        public NarrationInformationOneRecord()
        {
            Culture_Text_Dict = new Dictionary<string, string>();
        }

        public string GetCodeOfNarrationInformationProperty()
        {
            var normalizedTextInJapanese = Culture_Text_Dict["ja"].Replace("&", "and");
            var normalizedTextInEnglish = Culture_Text_Dict[""].Replace("&", "and");

            var ret = @"        /// <summary>";
            ret += normalizedTextInJapanese;
            ret += " (" + normalizedTextInEnglish + ")";
            ret += "</summary>" + Environment.NewLine;
            ret += "        public NarrationInformation " + PropertyIdentifier + " { get; private set; }" + Environment.NewLine;
            return ret;
        }

        public string GetCodeOfNarrationInformationPropertyConstructor()
        {
            var ret = string.Format(@"            View{0:D3}_Message{1:D3}{2:D2} = new NarrationInformation() {{ ViewIndex = {0}, MessageIndex = {1}, SubIndex = {2}, TextKey = ""{3}"" }};", ViewIndex, MessageIndex, SubIndex, PropertyIdentifier + "_TextKey");
            ret += Environment.NewLine;
            return ret;
        }
    }
}

namespace ExcelXamlConverterForLocalization
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
    using System.Windows.Markup;
    using NPOI.HSSF.UserModel;
    using NPOI.SS.UserModel;

    public partial class ExcelXamlConverterForLocalizationMainWindow : Window
    {
        Dictionary<string, string> Table = new Dictionary<string, string>();
        ExcelXamlConverter Converter;

        public ExcelXamlConverterForLocalizationMainWindow()
        {
            InitializeComponent();

            CreateTestDataButton.Click += CreateTestDataButton_Click;
            CreateFromXamlFileButton.Click += CreateFromXamlFileButton_Click;
            CreateFromExcelFileButton.Click += CreateFromExcelFileButton_Click;
            SaveToXamlButton.Click += SaveToXamlButton_Click;
            SaveToExcelButton.Click += SaveToExcelButton_Click;
        }

        void CreateTestDataButton_Click(object sender, RoutedEventArgs e)
        {
            Table.Clear();
            Table["Page01Title"] = "Initial Gesture Training";
            Table["Page01Video"] = "Tutorial01StartGestureTrainingFirstStepVideo.avi";
            Converter = new ExcelXamlConverter(Table);
        }

        void SaveToXamlButton_Click(object sender, RoutedEventArgs e)
        {
            Converter.SaveToXaml("test.xaml");
        }

        void SaveToExcelButton_Click(object sender, RoutedEventArgs e)
        {
            Converter.SaveToExcel("test.xls");
        }

        void CreateFromXamlFileButton_Click(object sender, RoutedEventArgs e)
        {
            Converter = ExcelXamlConverter.CreateFromXamlFile("test.xaml");
            Console.WriteLine(Converter);
        }

        void CreateFromExcelFileButton_Click(object sender, RoutedEventArgs e)
        {
            Converter = ExcelXamlConverter.CreateFromXamlFile("test.xaml");
            Console.WriteLine(Converter);
        }
    }

    public class ExcelXamlConverter
    {
        // Reference
        // https://tocsworld.wordpress.com/2014/08/13/%E5%A4%9A%E8%A8%80%E8%AA%9E%E5%AF%BE%E5%BF%9Cc%E3%80%81wpf%E7%B7%A8/

        private const int targetColumnIndex = 1;
        private static readonly string sheetName = "sheet1";

        private ExcelXamlConverter() { }

        private Dictionary<string, string> stringTable = new Dictionary<string, string>();

        public ExcelXamlConverter(Dictionary<string, string> table)
        {
            stringTable = table;
        }

        public static ExcelXamlConverter CreateFromXamlFile(string xamlResourceFile)
        {
            Dictionary<string, string> table = new Dictionary<string, string>();
            using (FileStream inputStream = new FileStream(xamlResourceFile, FileMode.Open))
            {
                ResourceDictionary xamlTop = XamlReader.Load(inputStream) as ResourceDictionary;
                if (xamlTop == null)
                {
                    throw new InvalidDataException();
                }
                foreach (System.Collections.DictionaryEntry item in xamlTop)
                {
                    table.Add(item.Key as String, item.Value as String);
                }
            }
            return new ExcelXamlConverter(table);
        }

        public static ExcelXamlConverter CreateFromExcelFile(string excelFile)
        {
            Dictionary<string, string> table = new Dictionary<string, string>();
            using (FileStream inputStream = new FileStream(excelFile, FileMode.Open, FileAccess.Read))
            {
                HSSFWorkbook book = new HSSFWorkbook(inputStream);
                ISheet sheet = book.GetSheet(sheetName);
                for (int rowIndex = 0; rowIndex <= sheet.LastRowNum; rowIndex++)
                {
                    IRow row = sheet.GetRow(rowIndex);
                    table.Add(row.GetCell(0).StringCellValue, row.GetCell(1).StringCellValue);
                }
            }
            return new ExcelXamlConverter(table);
        }

        public void SaveToXaml(string xamlResourceFile)
        {
            var resourceDictionary = new ResourceDictionary();
            foreach (var item in stringTable)
            {
                resourceDictionary.Add(item.Key, item.Value);
            }
            var xaml = XamlWriter.Save(resourceDictionary);
            File.WriteAllText(xamlResourceFile, xaml);
        }

        public void SaveToExcel(string excelFile)
        {
            using (FileStream outputStream = new FileStream(excelFile, FileMode.OpenOrCreate))
            {
                // Export to Excel with NPOI
                var book = new HSSFWorkbook();
                var sheet = book.CreateSheet(sheetName);
                int rowIndex = 0;
                foreach (KeyValuePair<string, string> item in stringTable)
                {
                    var row = sheet.CreateRow(rowIndex++);
                    var cell1 = row.CreateCell(0, NPOI.SS.UserModel.CellType.String);
                    var cell2 = row.CreateCell(targetColumnIndex, NPOI.SS.UserModel.CellType.String);
                    cell1.SetCellValue(item.Key);
                    cell2.SetCellValue(item.Value);
                }
                book.Write(outputStream);
            }
        }
    }
}

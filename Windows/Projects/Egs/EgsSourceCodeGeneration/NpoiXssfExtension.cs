using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Egs.EgsSourceCodeGeneration
{
    public static class NpoiXssfExtension
    {
        public static string GetCellString(this NPOI.SS.UserModel.IRow row, Dictionary<string, int> headerCellString_ColumnIndex_Dict, string headerKey)
        {
            var index = headerCellString_ColumnIndex_Dict[headerKey];
            var cell = row.GetCell(index);
            if (cell == null) { return ""; }
            if (cell.CellType == NPOI.SS.UserModel.CellType.Blank) { return ""; }
            if (cell.CellType == NPOI.SS.UserModel.CellType.String) { return cell.StringCellValue; }
            if (cell.CellType == NPOI.SS.UserModel.CellType.Formula) { return cell.StringCellValue; }
            return cell.ToString();
        }
    }
}

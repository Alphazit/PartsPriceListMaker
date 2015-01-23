using System;
using System.Collections.Generic;
using Microsoft.Office.Interop.Excel;

namespace PartsPriceListMaker
{
    class DataExport
    {
        /// <summary>
        /// エクセルで開く
        /// </summary>
        internal bool OpenExcel(List<Buhin> list, out string msg)
        {
            bool ret = false;
            msg = string.Empty;
            //開いているエクセルを取得
            Microsoft.Office.Interop.Excel.Application exApp = null;
            try
            {
                exApp = (Microsoft.Office.Interop.Excel.Application)System.Runtime.InteropServices.Marshal.GetActiveObject("Excel.Application");
            }
            catch (Exception ex)
            {

            }
            if (exApp == null)
            {
                //開いてないなら新規作成
                exApp = new Microsoft.Office.Interop.Excel.Application();
            }
            exApp.Visible = true;
            Workbook wb = exApp.Workbooks.Add();
            Worksheet sheet = wb.Sheets[1];
            sheet.Select(Type.Missing);
            //見出し行
            Range range = sheet.Cells[1, 1]; range.Value2 = "分　類"; range.ColumnWidth = 11; 
            range = sheet.Cells[1, 2]; range.Value2 = "メーカー";  range.ColumnWidth = 7;
            range = sheet.Cells[1, 3]; range.Value2 = "型　式";    range.ColumnWidth = 35;
            range = sheet.Cells[1, 4]; range.Value2 = "数　量";    range.ColumnWidth = 6;
            range = sheet.Cells[1, 5]; range.Value2 = "単位";      range.ColumnWidth = 4;
            range = sheet.Cells[1, 6]; range.Value2 = "仕入単価";  range.ColumnWidth = 9;
            range = sheet.Cells[1, 7]; range.Value2 = "仕入金額";  range.ColumnWidth = 9;
            range = sheet.Cells[1, 8]; range.Value2 = "仕入先";    range.ColumnWidth = 7;
            //データ
            for (int i = 0; i < list.Count; i++)
            {
                int j = i + 2;
                range = sheet.Cells[j, 1]; range.Value2 = list[i].Bunrui;
                range = sheet.Cells[j, 2]; range.Value2 = list[i].Maker;
                range = sheet.Cells[j, 3]; range.Value2 = list[i].Katashiki;
                range = sheet.Cells[j, 4]; range.Value2 = "0";
                range = sheet.Cells[j, 5]; range.Value2 = list[i].Tani;
                range = sheet.Cells[j, 6]; range.Value2 = list[i].Price;
                range = sheet.Cells[j, 7]; range.Value2 = "=D" + j.ToString() + "*F" + j.ToString();
                range = sheet.Cells[j, 8]; range.Value2 = list[i].Supplier;
            }
            //体裁を整える
            range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[1, 8]]; 
            range.Interior.ColorIndex = 44;
            range = sheet.Range[sheet.Cells[2, 7], sheet.Cells[list.Count + 1, 7]]; 
            range.Interior.ColorIndex = 20;
            range = sheet.Range[sheet.Cells[1, 1], sheet.Cells[list.Count + 1, 8]];
            range.Borders.get_Item(XlBordersIndex.xlEdgeBottom).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeLeft).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeRight).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlEdgeTop).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlInsideHorizontal).LineStyle = XlLineStyle.xlContinuous;
            range.Borders.get_Item(XlBordersIndex.xlInsideVertical).LineStyle = XlLineStyle.xlContinuous;
            ret = true;
            return ret;
        }
    }
}

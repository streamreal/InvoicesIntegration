using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Excel = Microsoft.Office.Interop.Excel;
using Microsoft.CSharp;

namespace сSharpProject
{
    class ExcelCharts
    {
        public static void Process()
        {
            Excel.Application xlApp = new Excel.Application();
            Excel.Workbook xlWorkBook;
            Excel.Worksheet xlWorkSheet;

            xlWorkBook = xlApp.Workbooks.Add(@"\\10.10.0.28\alta\Robot\test.xlsx");
            xlWorkSheet = (Excel.Worksheet)xlWorkBook.Worksheets[1];
            
            
            Console.WriteLine(xlWorkSheet.Cells[1,1].Value);
            Console.ReadLine();
        }     
    }
}
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Forms;
using Excel = Microsoft.Office.Interop.Excel;

namespace MyTool
{
    class EXCEL
    {
        /// <summary>
        /// 从excel文件读取内容
        /// </summary>
        /// <param name="fileName">excel文件名</param>
        /// <returns>获取读取数据的表</returns>
        static public DataTable ImportFromExcel(string fileName)
        {

            Excel.Application excelApp = null;
            Excel.Workbooks wbks = null;
            Excel._Workbook wbk = null;
            try
            {
                excelApp = new Excel.Application();
                excelApp.Visible = false;//是打开不可见
                wbks = excelApp.Workbooks;
                wbk = wbks.Add(fileName);
                object Nothing = Missing.Value;
                Excel._Worksheet whs;
                whs = (Excel._Worksheet)wbk.Sheets[1];//获取第一张工作表
                whs.Activate();
                DataTable dt = new DataTable(whs.Name);
                //读取excel表格的列标题
                int col_count = whs.UsedRange.Columns.Count;
                for (int col = 1; col <= col_count; col++)
                {
                    dt.Columns.Add(((Excel.Range)whs.Cells[1, col]).Text.ToString());
                }
                //读取数据
                for (int row = 1; row <= whs.UsedRange.Rows.Count; row++)
                {
                    DataRow dr = dt.NewRow();
                    //for (int col = 1; col < col_count; col++)
                    //{
                    //    dr[col - 1] = ((Excel.Range)whs.Cells[row, col]).Text.ToString();
                    //}
                    for (int col = 0; col < col_count; col++)
                    {
                        dr[col] = ((Excel.Range)whs.Cells[row, col+1]).Text.ToString();
                    }
                    dt.Rows.Add(dr);
                }
                return dt;
            }
            catch (Exception e)
            {
                throw e;
            }
            finally
            {
                //wbks.Close();//关闭工作簿
                excelApp.Quit();//关闭excel应用程序
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);//释放excel进程
                excelApp = null;
            }
        }
        /// <summary>
        /// 导出数据到excel文件
        /// </summary>
        /// <param name="dt">要导出的数据集</param>
        /// <returns>生成的文件名</returns>
        static public string ExportToExcel(DataTable dt)
        {
            
            Excel.Application excelApp = null;
            Excel.Workbooks wbks = null;
            Excel._Workbook wbk = null;
            try
            {
                excelApp = new Excel.Application();
                excelApp.Visible = false;//是打开不可见
                wbks = excelApp.Workbooks;
                wbk = wbks.Add(true);
                System.Windows.Forms.SaveFileDialog saveFileDialog = new System.Windows.Forms.SaveFileDialog();
                saveFileDialog.Title = "保存为";
                saveFileDialog.Filter = "xls工作薄|*.xls|xlsx工作薄|*.xlsx";
                String version = excelApp.Version;//获取你使用的excel 的版本号
                int FormatNum;//保存excel文件的格式
                if (Convert.ToDouble(version) < 12)//You use Excel 97-2003
                {
                    FormatNum = -4143;
                }
                else//you use excel 2007 or later
                {
                    FormatNum = 56;
                }
                object Nothing = Missing.Value;
                Excel._Worksheet whs;
                whs = (Excel._Worksheet)wbk.Sheets[1];//获取第一张工作表
                whs.Activate();
                //写入标题行
                int rowIndex = 1;
                for (int col = 0; col < dt.Columns.Count; col++)
                {
                    whs.Cells[rowIndex, col + 1] = dt.Columns[col].Caption.ToString();
                }
                rowIndex++;
                //写入数据内容
                foreach (DataRow row in dt.Rows)
                {
                    for (int colIndex = 0; colIndex < dt.Columns.Count; colIndex++)
                    {
                        whs.Cells[rowIndex, colIndex + 1] = row[colIndex].ToString();
                    }
                    rowIndex++;
                }
                excelApp.DisplayAlerts = false;
                //保存excel文件
                //wbk.SaveCopyAs(@"D:\test.xls");
               
                string newFileName = string.Empty;
                if (saveFileDialog.ShowDialog() == DialogResult.OK && saveFileDialog.FileName.Trim() != "")
                {
                    newFileName = saveFileDialog.FileName;
                    wbk.SaveAs(newFileName, FormatNum);
                    System.Windows.Forms.MessageBox.Show("数据已经成功导入EXCEL文件" + newFileName, "数据导出", MessageBoxButtons.OK, MessageBoxIcon.Asterisk);
                }
                //关闭文件
                wbk.Close(false, Nothing, Nothing);
                return newFileName;
            }
            catch (Exception e)
            {
                System.Windows.Forms.MessageBox.Show(e.ToString());
                return "EXPORT ERROR";
            }
            finally
            {
                //wbks.Close();//关闭工作簿
                excelApp.Quit();//关闭excel应用程序
                System.Runtime.InteropServices.Marshal.ReleaseComObject(excelApp);//释放excel进程
                excelApp = null;
            }
        }

        

    }
}


using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Office.Core;
using Excel = Microsoft.Office.Interop.Excel;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace xjplc
{
    class ExcelObject
    {
        public string mFilename;
        public Excel.Application app;
        public Excel.Workbooks wbs;
        public Excel.Workbook wb;
        public Excel.Worksheet ws;
        public Excel.Worksheets wss;
        public ExcelObject()
        { }
        public void Open0(string FileName)//打开一个Excel文件
        {
            wbs = app.Workbooks;
            wb = wbs.Open(FileName, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlPlatform.xlWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
 
        }

        /// <summary>
        /// 执行宏
        /// </summary>
        /// <param name="oApp">Excel对象</param>
        /// <param name="oRunArgs">参数（第一个参数为指定宏名称，后面为指定宏的参数值）</param>
        /// <returns>宏返回值</returns>
        private object RunMacro(object oApp, object[] oRunArgs)
        {
            try
            {
                // 声明一个返回对象
                object objRtn;

                // 反射方式执行宏
                objRtn = oApp.GetType().InvokeMember(
                                                        "Run",
                                                        System.Reflection.BindingFlags.Default |
                                                        System.Reflection.BindingFlags.InvokeMethod,
                                                        null,
                                                        oApp,
                                                        oRunArgs
                                                     );

                // 返回值
                return objRtn;

            }
            catch (Exception ex)
            {
                if (ex.InnerException.Message.ToString().Length > 0)
                {
                    throw ex.InnerException;
                }
                else
                {
                    throw ex;
                }
            }


        }
        public void ExcelRunMacro()
        {
            // 根据参数组是否为空，准备参数组对象
            try
            {
                app.Run("cfjs");
            }
            catch
            {

            }
            finally
            {
               // Close0();
            }
           


            

           // RunMacro(app, paraObjects);
        }

        public void Open(string FileName)//打开一个Excel文件
        {
            if(app == null)
            app = new Excel.Application();
            //app.WindowState = Excel.XlWindowState.xlMinimized;
            //if (wb!=null) wb.Close();
            wb = null;
            wbs = null;
            wbs = app.Workbooks;                        
            wb = wbs.Open(FileName,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Excel.XlPlatform.xlWindows,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing,Type.Missing);           
            ws = GetSheet("单原料求解");
            ws.Unprotect();
            mFilename = FileName;
        }

        public void openWb()
        {
            wb = wbs.Open(mFilename, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Excel.XlPlatform.xlWindows, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing, Type.Missing);
        }
        public void Create()//创建一个Excel对象
        {
            app = new Excel.Application();
            wbs = app.Workbooks;
            wb = wbs.Add(true);
        }
        public int Rowcnt()
        {
            int i=0;
            while (GetStrCellValue(ws, i + 2, 1) != "")
            {
                i++;
            }
            return i;
        }
        public static Boolean KillProcess(string proName)
        {

            Process[] p = Process.GetProcessesByName(proName);
            if (p.Count() > 0)
            {
                foreach (Process p1 in p)
                {
                    p1.Kill();
                }
                return true;
            }
            else return false;

        }
        //判断程序是否在执行
        //检查第一行 各项数据
        public Boolean CheckExcelFile(string[] str)
        {
            for (int i = 0; i < str.Length;i++ )
            {
                if (GetStrCellValue(ws, 1, i + 1) != str[i]) return false;
            }
            return true;
        }
        public void SetCellValue(Excel.Worksheet ws, int x, int y, object value)
        //ws：要设值的工作表     X行Y列     value   值
        {
            ws.Unprotect();
            if (value != ws.Cells[x, y])
            {
                ws.Cells[x, y] = value;
            }
        }
        public int GetIntCellValue(Excel.Worksheet ws, int x, int y)
        //ws：要获取设值的工作表     X行Y列     value   值
        {
            int i=-1;
            string j= ws.Cells[x, y].Text;
            if (int.TryParse(j,out i))
            {
                return i;
            }
            else
            return -1;
        }
        public string GetStrCellValue(Excel.Worksheet ws, int x, int y)
        //ws：要获取设值的工作表     X行Y列     value   值
        {
            if (ws.Cells[x, y].Text!=null)
            {
                string str = ws.Cells[x, y].Text.ToString();
                return ws.Cells[x, y].Text.ToString();
            }
            else return "";

        }
        public void SetCellValue(string ws, int x, int y, object value)
        //ws：要设值的工作表的名称 X行Y列 value 值
        {

            GetSheet(ws).Cells[x, y] = value;
        }

        public Excel.Worksheet GetSheet(string SheetName)
        //获取一个工作表
        {
            Excel.Worksheet s = (Excel.Worksheet)wb.Worksheets[SheetName];
            return s;
        }

        public Excel.Worksheet GetDefaultSheet()
        //获取一个工作表
        {
            Excel.Worksheet s = (Excel.Worksheet)wb.Sheets[1];
                              
            return s;
        }

        public Excel.Worksheet AddSheet(string SheetName)
        //添加一个工作表
        {
            Excel.Worksheet s = (Excel.Worksheet)wb.Worksheets.Add(Type.Missing, Type.Missing, Type.Missing, Type.Missing);
            s.Name = SheetName;
            return s;
        }

        public void DelSheet(string SheetName)//删除一个工作表
        {
            ((Excel.Worksheet)wb.Worksheets[SheetName]).Delete();
        }
        public Excel.Worksheet ReNameSheet(string OldSheetName, string NewSheetName)//重命名一个工作表一
        {
            Excel.Worksheet s = (Excel.Worksheet)wb.Worksheets[OldSheetName];
            s.Name = NewSheetName;
            return s;
        }

        public Excel.Worksheet ReNameSheet(Excel.Worksheet Sheet, string NewSheetName)//重命名一个工作表二
        {

            Sheet.Name = NewSheetName;

            return Sheet;
        }

        public void SetCellProperty(Excel.Worksheet ws, int Startx, int Starty, int Endx, int Endy, int size, string name, Excel.Constants color, Excel.Constants HorizontalAlignment)
        //设置一个单元格的属性   字体，   大小，颜色   ，对齐方式
        {
            name = "宋体";
            size = 12;
            color = Excel.Constants.xlAutomatic;
            HorizontalAlignment = Excel.Constants.xlRight;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Name = name;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Size = size;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Color = color;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).HorizontalAlignment = HorizontalAlignment;
        }

        public void SetCellProperty(string wsn, int Startx, int Starty, int Endx, int Endy, int size, string name, Excel.Constants color, Excel.Constants HorizontalAlignment)
        {
            //name = "宋体";
            //size = 12;
            //color = Excel.Constants.xlAutomatic;
            //HorizontalAlignment = Excel.Constants.xlRight;

            Excel.Worksheet ws = GetSheet(wsn);
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Name = name;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Size = size;
            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).Font.Color = color;

            ws.get_Range(ws.Cells[Startx, Starty], ws.Cells[Endx, Endy]).HorizontalAlignment = HorizontalAlignment;
        }
        public void UniteCells(Excel.Worksheet ws, int x1, int y1, int x2, int y2)
        //合并单元格
        {
            ws.get_Range(ws.Cells[x1, y1], ws.Cells[x2, y2]).Merge(Type.Missing);
        }
        public void UniteCells(string ws, int x1, int y1, int x2, int y2)
        //合并单元格
        {
            GetSheet(ws).get_Range(GetSheet(ws).Cells[x1, y1], GetSheet(ws).Cells[x2, y2]).Merge(Type.Missing);

        }

        public bool Save()
        //保存文档
        {
            
            if (mFilename == "")
            {
                return false;
            }
            else
            {
                try
                {
                    wb.Save();
                    return true;
                }

                catch (Exception ex)
                {
                    return false;
                }
            }
        }
        [DllImport("User32.dll", CharSet = CharSet.Auto)]
        public static extern int GetWindowThreadProcessId(IntPtr hwnd,out int ID);
        public static void KillExcel(Microsoft.Office.Interop.Excel.Application excel)
        {
        IntPtr t = new IntPtr(excel.Hwnd); //得到这个句柄，具体作用是得到这块内存入口

        int k = 0;
        GetWindowThreadProcessId(t, out k); //得到本进程唯一标志k
        System.Diagnostics.Process p = System.Diagnostics.Process.GetProcessById(k); //得到对进程k的引用
        p.Kill(); //关闭进程k
        }

        public void Close0()
        {
            if (app != null)
            {
                if(wb != null)
                wb.Save();
                wb.Close();
                if (wbs != null)
                    wbs.Close();
                app.Quit();
                System.Runtime.InteropServices.Marshal.ReleaseComObject(ws);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wb);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(wbs);
                System.Runtime.InteropServices.Marshal.ReleaseComObject(app);
                wb = null;
                wbs = null;
                app = null;
                GC.Collect();
                GC.WaitForPendingFinalizers();
            }

        }
        public void Close()
        //关闭一个Excel对象，销毁对象
        {
            
            if (app != null)
            {
                wb.Save();
                wb.Close();
                wbs.Close();

                app.Quit();
                wb = null;
                wbs = null;
                app = null;
                GC.Collect();
            }
        }
    }
}

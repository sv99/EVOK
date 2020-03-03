using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using NPOI.HSSF.UserModel;
using System.Windows.Forms;
using System.Diagnostics;
using NPOI.POIFS.FileSystem;
using NPOI;
using NPOI.OpenXml4Net.OPC;
using System.Drawing;
using NPOI.SS.Util;
using System.Threading;

namespace xjplc
{
    public class ExcelNpoi
    {
        string fileName;
        public string FileName
        {
            get { return fileName; }
            set { fileName = value; }
        }
        public ExcelNpoi(string file)
        {
            FileName = file;
        }
        IWorkbook currentWk;
        public ExcelNpoi()
        {

        }



        private string GetCellValue(ICell cell)
        {
            if (cell == null)
                return string.Empty;
            switch (cell.CellType)
            {
                case CellType.Blank: //空数据类型 这里类型注意一下，不同版本NPOI大小写可能不一样,有的版本是Blank（首字母大写)
                    return string.Empty;
                case CellType.Boolean: //bool类型
                    return cell.BooleanCellValue.ToString();
                case CellType.Error:
                    return cell.ErrorCellValue.ToString();
                case CellType.Numeric: //数字类型
                    if (HSSFDateUtil.IsCellDateFormatted(cell))//日期类型
                    {
                        return cell.DateCellValue.ToString();
                    }
                    else //其它数字
                    {
                        return cell.NumericCellValue.ToString();
                    }
                case CellType.Unknown: //无法识别类型
                default: //默认类型
                    return cell.ToString();//
                case CellType.String: //string 类型
                    return cell.StringCellValue;
                case CellType.Formula: //带公式类型
                    try
                    {
                        HSSFFormulaEvaluator e = new HSSFFormulaEvaluator(cell.Sheet.Workbook);
                        e.EvaluateInCell(cell);
                        return cell.ToString();
                    }
                    catch
                    {
                        return cell.NumericCellValue.ToString();
                    }
            }
        }
        /// <summary>
        /// 获取当前单元格所在的合并单元格的位置
        /// </summary>
        /// <param name="sheet">sheet表单</param>
        /// <param name="rowIndex">行索引 0开始</param>
        /// <param name="colIndex">列索引 0开始</param>
        /// <param name="start">合并单元格左上角坐标</param>
        /// <param name="end">合并单元格右下角坐标</param>
        /// <returns>返回false表示非合并单元格</returns>
        private  bool IsMergeCell(ISheet sheet, int rowIndex, int colIndex, out Point start, out Point end)
        {
            bool result = false;
            start = new Point(0, 0);
            end = new Point(0, 0);
            if ((rowIndex < 0) || (colIndex < 0)) return result;
            int regionsCount = sheet.NumMergedRegions;
            for (int i = 0; i < regionsCount; i++)
            {
                CellRangeAddress range = sheet.GetMergedRegion(i);
                //sheet.IsMergedRegion(range); 
                if (rowIndex >= range.FirstRow && rowIndex <= range.LastRow && colIndex >= range.FirstColumn && colIndex <= range.LastColumn)
                {
                    start = new Point(range.FirstRow, range.FirstColumn);
                    end = new Point(range.LastRow, range.LastColumn);
                    result = true;
                    break;
                }
            }
            return result;
        }
        #region 合并单元格读取
        public void ChangeCellValue(string[] data,string key)
        {
            IWorkbook workbook = NPOIOpenExcel(FileName);
            if (workbook == null)
            {
                return;
            }
            int sheetNum = workbook.NumberOfSheets;

            bool IsExist = false;

            ISheet mysheet = workbook.GetSheetAt(0);
            for (int m=0; m <= mysheet.LastRowNum; m++)
            {
                IRow mySourceRow = mysheet.GetRow(m); //默认只有一个页
                string key0 = mySourceRow.GetCell(7).StringCellValue; //设备编号来区分
                if (key0.Equals(key))
                {
                    IsExist = true;
                    data[0] = "20180716fsdfd";
                    IrowFromString(mySourceRow, data);

                    WriteToFile(workbook, FileName);

                    break;                    
                }
            }
            if (!IsExist)
            {
                AddRow(data);
            }
            //获取原格式行                                

        }

        bool compareCurrentWk(List<DataTable> dtL)
        {
            List<string> sheetname = new List<string>();
            for (int i = 0; i < currentWk.NumberOfSheets;i++)
            {
                sheetname.Add(currentWk.GetSheetName(i));
            }
            foreach (DataTable dt in dtL)
            {
                if (!sheetname.Contains(dt.TableName))
                {
                    return false;
                }
            }

           return true;
        }

        /// <summary>
        /// 将多个DataTable数据导入到excel中
        /// </summary>
        /// <param name="dts">要导入的数据集合</param>
        /// <param name="strExcelFileName">定义Excel文件名</param>
        /// <param name="indexType">给个 1 就行</param>
        public bool GridToExcels(List<DataTable> dts, string strExcelFileName, int indexType)
        {
            bool BSave = false;
            try
            {
                IWorkbook
                workbook =null;
                //获取后缀名
                string extension = strExcelFileName.Substring(strExcelFileName.LastIndexOf(".")).ToString().ToLower();
                //判断是否是excel文件
                if (extension == ".xlsx" || extension == ".xls")
                {
                    //判断excel的版本
                    if (extension == ".xlsx")
                    {
                        workbook = new XSSFWorkbook();
                    }
                    else
                    {
                        workbook = new HSSFWorkbook();
                    }
                }
            
                DataSet set = new DataSet();
                foreach (DataTable dt in dts)
                {
                    ISheet sheet = workbook.CreateSheet(dt.TableName);
                    ICellStyle HeadercellStyle = workbook.CreateCellStyle();
                    HeadercellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    HeadercellStyle.Alignment = NPOI.SS.UserModel.HorizontalAlignment.Center;
                    //字体
                    NPOI.SS.UserModel.IFont headerfont = workbook.CreateFont();
                    headerfont.Boldweight = (short)FontBoldWeight.Bold;
                    HeadercellStyle.SetFont(headerfont);


                    //用column name 作为列名
                    int icolIndex = 0;
                    IRow headerRow = sheet.CreateRow(0);
                    foreach (DataColumn item in dt.Columns)
                    {
                        ICell cell = headerRow.CreateCell(icolIndex);
                        cell.SetCellValue(item.ColumnName);
                        cell.CellStyle = HeadercellStyle;
                        icolIndex++;
                    }


                    ICellStyle cellStyle = workbook.CreateCellStyle();
                    //为避免日期格式被Excel自动替换，所以设定 format 为 『@』 表示一率当成text来看
                    cellStyle.DataFormat = HSSFDataFormat.GetBuiltinFormat("@");
                    cellStyle.BorderBottom = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderLeft = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderRight = NPOI.SS.UserModel.BorderStyle.Thin;
                    cellStyle.BorderTop = NPOI.SS.UserModel.BorderStyle.Thin;
                    NPOI.SS.UserModel.IFont cellfont = workbook.CreateFont();
                    cellfont.Boldweight = (short)FontBoldWeight.Normal;
                    cellStyle.SetFont(cellfont);
                    if (indexType == 1)
                    {
                        //建立内容行
                        int iRowIndex = 1;
                        int iCellIndex = 0;
                        foreach (DataRow Rowitem in dt.Rows)
                        {
                            IRow DataRow = sheet.CreateRow(iRowIndex);
                            foreach (DataColumn Colitem in dt.Columns)
                            {


                                ICell cell = DataRow.CreateCell(iCellIndex);
                                cell.SetCellValue(Rowitem[Colitem].ToString());
                                cell.CellStyle = cellStyle;
                                iCellIndex++;
                            }
                            iCellIndex = 0;
                            iRowIndex++;
                        }
                        //自适应列宽
                        for (int i = 0; i < icolIndex; i++)
                        {
                            sheet.AutoSizeColumn(i);
                        }
                    }

                }
                FileStream fswrite = File.Open(FileName, FileMode.Create, FileAccess.ReadWrite);
                workbook.Write(fswrite);
                fswrite.Close();
                BSave = true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
            return BSave;
        }

        public void saveMultiSheet(List<DataTable> dtLst)
        {
            if (currentWk != null && currentWk.NumberOfSheets == dtLst.Count && compareCurrentWk(dtLst))
            {
                FileStream fswrite = File.Open(FileName,FileMode.Create,FileAccess.ReadWrite);
                currentWk.Write(fswrite);
                fswrite.Close();
            }
        }
        //修改内容
        public void ChangeCellValue(string data,string tablename,Point p)
        {

            try
            {

                if (currentWk != null)
                { 
                    ISheet mysheet = currentWk.GetSheet(tablename);
                    mysheet.GetRow(p.X+1).GetCell(p.Y).SetCellValue(data);                                                                                                                                               
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.ToString());
            }

        }

       
        #endregion
        private void IrowFromString(IRow irow,string[] data)
        {
            for (int m = 0; m < irow.LastCellNum; m++)
            {
                if(m<data.Length)
                irow.GetCell(m).SetCellValue(data[m]);
            }
        }
        /// <summary>
        /// 在第一行之前插入
        /// </summary>
        public void AddRow(string[] data)
        {
            try
            {
                IWorkbook workbook = NPOIOpenExcel(FileName);
                if (workbook == null)
                {
                    return;
                }
                int sheetNum = workbook.NumberOfSheets;

                for (int i = 0; i < sheetNum; i++)
                {
                    ISheet mysheet = workbook.GetSheetAt(i);
                    //获取原格式行
                    IRow mySourceRow = mysheet.GetRow(1);
                                     
                    InsertRow(mysheet, 1, 1, mySourceRow,data);
                }

                WriteToFile(workbook, fileName);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }        

        }   
        //插入
        private void InsertRow(ISheet sheet, int insertRowIndex, int insertRowCount, IRow formatRow, string[] insertdata)
        {
            sheet.ShiftRows(insertRowIndex, sheet.LastRowNum, insertRowCount, true, false);
            for (int i = insertRowIndex; i < insertRowIndex + insertRowCount; i++)
            {
                IRow targetRow = null;
                ICell sourceCell = null;
                ICell targetCell = null;

                targetRow = sheet.CreateRow(i);

                for (int m = formatRow.FirstCellNum; m < formatRow.LastCellNum; m++)
                {
                    sourceCell = formatRow.GetCell(m);
                    if (sourceCell == null)
                    {
                        continue;
                    }
                    targetCell = targetRow.CreateCell(m);
                    targetCell.CellStyle = sourceCell.CellStyle;
                    targetCell.SetCellType(sourceCell.CellType);

                }
            }

            for (int i = insertRowIndex; i < insertRowIndex + insertRowCount; i++)
            {
                IRow firstTargetRow = sheet.GetRow(i);
                ICell firstSourceCell = null;
                ICell firstTargetCell = null;

                for (int m = formatRow.FirstCellNum; m < formatRow.LastCellNum; m++)
                {
                    firstSourceCell = formatRow.GetCell(m, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    if (firstSourceCell == null)
                    {
                        continue;
                    }
                    firstTargetCell = firstTargetRow.GetCell(m, MissingCellPolicy.CREATE_NULL_AS_BLANK);
                    firstTargetCell.CellStyle = firstSourceCell.CellStyle;
                    firstTargetCell.SetCellType(firstSourceCell.CellType);

                    if (insertdata != null && insertdata.Length > 0 && m<insertdata.Length)
                    {
                        firstTargetCell.SetCellValue(insertdata[m]);
                    }
                   // firstTargetCell.SetCellValue("test");
                }
            }
        }
        public DataTable ImportExcel(string filePath,int id,ref List<DataTable> dtLst)
        {
            DataTable dt = new DataTable();
            FileName = filePath;
            using (FileStream fsRead = System.IO.File.OpenRead(filePath))
            {
                IWorkbook wk = null;
                //获取后缀名
                string extension = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
                //判断是否是excel文件
                if (extension == ".xlsx" || extension == ".xls")
                {
                    //判断excel的版本
                    if (extension == ".xlsx")
                    {
                        wk = new XSSFWorkbook(fsRead);
                    }
                    else
                    {
                        wk = new HSSFWorkbook(fsRead);
                    }

                    //获取第一个sheet
                    int count = wk.NumberOfSheets;
                    for (int k = 0; k < count; k++)
                    {                    

                        ISheet sheet = wk.GetSheetAt(k);
                        dt = new DataTable(sheet.SheetName);
                        //获取第一行
                        IRow headrow = sheet.GetRow(0);
                        #region 根据第一行创建抬头
                        for (int i = headrow.FirstCellNum; i < headrow.Cells.Count; i++)
                        {
                            if (!dt.Columns.Contains(headrow.GetCell(i).StringCellValue))
                            {
                                DataColumn datacolum = new DataColumn(headrow.GetCell(i).StringCellValue);
                                // DataColumn datacolum = new DataColumn("F" + (i + 1));
                                dt.Columns.Add(datacolum);
                            }
                        }
                        #endregion

                        #region 读取每一行
                        //读取每行,从第二行起
                        for (int r = 1; r <= sheet.LastRowNum; r++)
                        {
                            bool result = false;
                            DataRow dr = dt.NewRow();
                            //获取当前行
                            IRow row = sheet.GetRow(r);
                            if (row == null) continue;
                            //读取每列
                            for (int j = 0; j < headrow.Cells.Count; j++)
                            {

                                ICell cell = row.GetCell(j); //一个单元格

                                if (cell == null) continue;

                                if (cell != null && cell.IsMergedCell)
                                {

                                    Point start = new Point();
                                    Point end = new Point();
                                    if (!IsMergeCell(sheet, r, j, out start, out end)) continue;
                                    IRow rowMerge = sheet.GetRow(start.X);
                                    ICell cellMerge = rowMerge.GetCell(start.Y); //一个单元格
                                    if (cellMerge != null)
                                    {
                                        dr[j] = GetCellValue(cellMerge);
                                        result = true;
                                    }
                                }
                                else
                                {

                                    dr[j] = GetCellValue(cell); //获取单元格的值 //全为空则不取

                                    if (dr[j].ToString() != "")
                                    {
                                        result = true;
                                    }
                                }
                            }
                            if (result == true)
                            {
                                dt.Rows.Add(dr); //把每行追加到DataTable
                            }
                        }
                        #endregion

                        //保存当前列
                        if (dtLst == null) dtLst = new List<DataTable>();
                        dtLst.Add(dt);

                    }

                }
                currentWk = wk;
                
                fsRead.Close();
            }          
            return dt;
        }
        public DataTable ImportExcel(string filePath,string sheetname)
        {
            DataTable dt = new DataTable();
            using (FileStream fsRead = System.IO.File.OpenRead(filePath))
            {
                IWorkbook wk = null;
                //获取后缀名
                string extension = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
                //判断是否是excel文件
                if (extension == ".xlsx" || extension == ".xls")
                {
                    //判断excel的版本
                    if (extension == ".xlsx")
                    {
                        wk = new XSSFWorkbook(fsRead);
                    }
                    else
                    {
                        wk = new HSSFWorkbook(fsRead);
                    }
                    //搜索相应的sheet在不在
                    int count = wk.NumberOfSheets;
                    int getId = 0;
                    bool isSheetExist = false;
                    for (int i = 0; i < count; i++)
                    {
                        ISheet sheet0 = wk.GetSheetAt(i);

                        if (sheet0.SheetName.Contains(sheetname))
                        {
                            getId = i;
                            isSheetExist = true;
                            break;
                        }

                    }
                    if (!isSheetExist) return dt;

                    //获取第一个sheet
                    ISheet sheet = wk.GetSheetAt(getId);
                    //获取第一行
                    IRow headrow = sheet.GetRow(4);
                    //创建列
                    for (int i = headrow.FirstCellNum; i < headrow.Cells.Count; i++)
                    {

                        if (!dt.Columns.Contains(headrow.GetCell(i).StringCellValue))
                        {
                            DataColumn datacolum = new DataColumn(headrow.GetCell(i).StringCellValue);
                            // DataColumn datacolum = new DataColumn("F" + (i + 1));
                            dt.Columns.Add(datacolum);
                        }
                    }
                    //读取每行,从第二行起
                    for (int r = 5; r <= sheet.LastRowNum; r++)
                    {

                        Application.DoEvents();

                        bool result = false;
                        DataRow dr = dt.NewRow();
                        //获取当前行
                        IRow row = sheet.GetRow(r);
                        if (row == null) continue;
                        //读取每列
                        for (int j = 0; j < headrow.Cells.Count; j++)
                        {
                            Application.DoEvents();
                            ICell cell = row.GetCell(j); //一个单元格

                            if (cell == null) continue;

                            if (cell != null && cell.IsMergedCell)
                            {

                                Point start = new Point();
                                Point end = new Point();
                                if (!IsMergeCell(sheet, r, j, out start, out end)) continue;
                                IRow rowMerge = sheet.GetRow(start.X);
                                ICell cellMerge = rowMerge.GetCell(start.Y); //一个单元格
                                if (cellMerge != null)
                                {
                                    dr[j] = GetCellValue(cellMerge);
                                    result = true;
                                }
                            }
                            else
                            {

                                dr[j] = GetCellValue(cell); //获取单元格的值 //全为空则不取

                                if (dr[j].ToString() != "")
                                {
                                    result = true;
                                }
                            }
                        }
                        if (result == true)
                        {
                            dt.Rows.Add(dr); //把每行追加到DataTable
                        }
                    }
                }

            }
            return dt;
        }
        public DataTable ImportExcel(string filePath, string sheetname,int rowid) //第几行开始有效
        {
            DataTable dt = new DataTable();
            using (FileStream fsRead = System.IO.File.OpenRead(filePath))
            {
                IWorkbook wk = null;
                //获取后缀名
                string extension = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
                //判断是否是excel文件
                if (extension == ".xlsx" || extension == ".xls")
                {
                    //判断excel的版本
                    if (extension == ".xlsx")
                    {
                        wk = new XSSFWorkbook(fsRead);
                    }
                    else
                    {
                        wk = new HSSFWorkbook(fsRead);
                    }
                    //搜索相应的sheet在不在
                    int count = wk.NumberOfSheets;
                    int getId = 0;
                    bool isSheetExist = false;
                    for (int i = 0; i < count; i++)
                    {
                        ISheet sheet0 = wk.GetSheetAt(i);

                        if (sheet0.SheetName.Contains(sheetname))
                        {
                            getId = i;
                            isSheetExist = true;
                            break;
                        }

                    }
                    if (!isSheetExist) return dt;

                    //获取第一个sheet
                    ISheet sheet = wk.GetSheetAt(getId);
                    //获取第一行
                    IRow headrow = sheet.GetRow(rowid);
                    //创建列
                    for (int i = headrow.FirstCellNum; i < headrow.Cells.Count; i++)
                    {

                        string colstr = "";
                        if (headrow.GetCell(i).CellType == CellType.String)
                        {
                            colstr = headrow.GetCell(i).StringCellValue;
                            
                        }
                        if (headrow.GetCell(i).CellType == CellType.Numeric)
                        {
                           
                            colstr = headrow.GetCell(i).NumericCellValue.ToString();
                           
                        }


                        if (!dt.Columns.Contains(colstr))
                        {
                         
                            DataColumn datacolum = new DataColumn(colstr);
                            // DataColumn datacolum = new DataColumn("F" + (i + 1));
                            if (datacolum!=null)
                            dt.Columns.Add(datacolum);
                            
                        }
                    }
                    //读取每行,从第二行起
                    for (int r = rowid+1; r <= sheet.LastRowNum; r++)
                    {

                        Application.DoEvents();

                        bool result = false;
                        DataRow dr = dt.NewRow();
                        //获取当前行
                        IRow row = sheet.GetRow(r);
                        if (row == null) continue;
                        //读取每列
                        for (int j = 0; j < headrow.Cells.Count; j++)
                        {
                            Application.DoEvents();
                            ICell cell = row.GetCell(j); //一个单元格

                            if (cell == null) continue;

                            if (cell != null && cell.IsMergedCell)
                            {

                                Point start = new Point();
                                Point end = new Point();
                                if (!IsMergeCell(sheet, r, j, out start, out end)) continue;
                                IRow rowMerge = sheet.GetRow(start.X);
                                ICell cellMerge = rowMerge.GetCell(start.Y); //一个单元格
                                if (cellMerge != null)
                                {
                                    dr[j] = GetCellValue(cellMerge);
                                    result = true;
                                }
                            }
                            else
                            {

                                dr[j] = GetCellValue(cell); //获取单元格的值 //全为空则不取

                                if (dr[j].ToString() != "")
                                {
                                    result = true;
                                }
                            }
                        }
                        if (result == true)
                        {
                            dt.Rows.Add(dr); //把每行追加到DataTable
                        }
                    }
                }

            }
            return dt;
        }

        public DataTable ImportExcel(string filePath)
        {
            DataTable dt = new DataTable();
            using (FileStream fsRead = System.IO.File.OpenRead(filePath))
            {
                IWorkbook wk = null;
                //获取后缀名
                string extension = filePath.Substring(filePath.LastIndexOf(".")).ToString().ToLower();
                //判断是否是excel文件
                if (extension == ".xlsx" || extension == ".xls")
                {
                    //判断excel的版本
                    if (extension == ".xlsx")
                    {
                        wk = new XSSFWorkbook(fsRead);
                    }
                    else
                    {
                        wk = new HSSFWorkbook(fsRead);
                    }

                    //获取第一个sheet
                    ISheet sheet = wk.GetSheetAt(0);
                    //获取第一行
                    IRow headrow = sheet.GetRow(0);
                    //创建列
                    for (int i = headrow.FirstCellNum; i < headrow.Cells.Count; i++)
                    {
                        if (!dt.Columns.Contains(headrow.GetCell(i).StringCellValue))
                        {
                            DataColumn datacolum = new DataColumn(headrow.GetCell(i).StringCellValue);
                            // DataColumn datacolum = new DataColumn("F" + (i + 1));
                            dt.Columns.Add(datacolum);
                        }
                    }     
                    //读取每行,从第二行起
                    for (int r = 1; r <= sheet.LastRowNum; r++)
                    {

                        Application.DoEvents();
                    
                        bool result = false;
                        DataRow dr = dt.NewRow();
                        //获取当前行
                        IRow row = sheet.GetRow(r);
                        if (row == null) continue;
                        //读取每列
                        for (int j = 0; j < headrow.Cells.Count; j++)
                        {
                            Application.DoEvents();
                            ICell cell = row.GetCell(j); //一个单元格

                            if (cell == null) continue;

                            if (cell != null && cell.IsMergedCell)
                            {

                                Point start = new Point();
                                Point end = new Point();
                                if (!IsMergeCell(sheet, r, j, out start, out end)) continue;
                                IRow rowMerge = sheet.GetRow(start.X);
                                ICell cellMerge = rowMerge.GetCell(start.Y); //一个单元格
                                if (cellMerge != null)
                                {
                                    dr[j] = GetCellValue(cellMerge);
                                    result = true;
                                }
                            }
                            else
                            {

                                dr[j] = GetCellValue(cell); //获取单元格的值 //全为空则不取

                                if (dr[j].ToString() != "")
                                {
                                    result = true;
                                }
                            }
                        }
                        if (result == true)
                        {
                            dt.Rows.Add(dr); //把每行追加到DataTable
                        }
                    }
                }

            }
            return dt;
        }
        private Stream OpenResource(string filename)
        {
            FileStream fs = new FileStream(filename, FileMode.Open, FileAccess.Read);
            return fs;
        }
        private IWorkbook NPOIOpenExcel(string filename)
        {
            IWorkbook myworkBook;
            Stream excelStream = OpenResource(filename);
            if (POIFSFileSystem.HasPOIFSHeader(excelStream))
                return new HSSFWorkbook(excelStream);
            if (POIXMLDocument.HasOOXMLHeader(excelStream))
            {
                return new XSSFWorkbook(OPCPackage.Open(excelStream));
            }
            if (filename.EndsWith(".xlsx"))
            {
                return new XSSFWorkbook(excelStream);
            }
            if (filename.EndsWith(".xls"))
            {
                new HSSFWorkbook(excelStream);
            }
            throw new Exception("Your InputStream was neither an OLE2 stream, nor an OOXML stream");
        }
        public void WriteToFile(IWorkbook workbook, string filename)
        {
            if (File.Exists(filename))
            {
                File.Delete(filename);
            }
            using (FileStream fs = new FileStream(filename, FileMode.OpenOrCreate, FileAccess.Write))
            {
                workbook.Write(fs);
                fs.Close();
            }
        }

        public void ExportDataToExcelNoDialog(DataTable TableName, string FilePath, Label lblStatus, ProgressBar barStatus)
        {

            //获得文件路径 
            string localFilePath = FilePath;
            //string localFilePath = path;//
            string FileName = Path.GetFileName(FilePath);
            //数据初始化
            int TotalCount = 0;     //总行数
            int RowRead = 0;    //已读行数
            int Percent = 0;    //百分比
            TotalCount = TableName.Rows.Count;
            if (lblStatus != null && barStatus != null)
            {

                lblStatus.Text = "共有" + TotalCount + "条数据";
                lblStatus.Visible = true;
                barStatus.Visible = true;
            }

            //NPOI
            IWorkbook workbook;
            string FileExt = Path.GetExtension(localFilePath).ToLower();
            if (FileExt == ".xlsx")
            {
                workbook = new XSSFWorkbook();
            }
            else if (FileExt == ".xls")
            {
                workbook = new HSSFWorkbook();
            }
            else
            {
                workbook = null;
            }
            if (workbook == null)
            {
                return;
            }
            ISheet sheet = string.IsNullOrEmpty(FileName) ? workbook.CreateSheet("Sheet1") : workbook.CreateSheet(FileName);
            //秒钟
            Stopwatch timer = new Stopwatch();
            timer.Start();

            try
            {
                //读取标题  
                IRow rowHeader = sheet.CreateRow(0);
                for (int i = 0; i < TableName.Columns.Count; i++)
                {
                    ICell cell = rowHeader.CreateCell(i);
                    cell.SetCellValue(TableName.Columns[i].ColumnName);
                }

                //读取数据  
                for (int i = 0; i < TableName.Rows.Count; i++)
                {
                    IRow rowData = sheet.CreateRow(i + 1);
                    for (int j = 0; j < TableName.Columns.Count; j++)
                    {
                        ICell cell = rowData.CreateCell(j);
                        cell.SetCellValue(TableName.Rows[i][j].ToString());
                    }
                    //状态栏显示
                    RowRead++;
                    if (lblStatus != null && barStatus != null)
                    {
                        Percent = (int)(100 * RowRead / TotalCount);
                        barStatus.Maximum = TotalCount; 
                        barStatus.Value = RowRead;
                        lblStatus.Text = "共有" + TotalCount + "条数据，已读取" + Percent.ToString() + "%的数据。";
                    }
                    Application.DoEvents();
                }
                if (lblStatus != null)
                    //状态栏更改
                    lblStatus.Text = "正在生成Excel...";
                Application.DoEvents();

                //转为字节数组  
                MemoryStream stream = new MemoryStream();
                workbook.Write(stream);
                var buf = stream.ToArray();

                //保存为Excel文件  
                using (FileStream fs = new FileStream(localFilePath, FileMode.Create, FileAccess.Write))
                {
                    fs.Write(buf, 0, buf.Length);
                    fs.Flush();
                    fs.Close();
                }
                if (lblStatus != null)
                    //状态栏更改
                    lblStatus.Text = "生成Excel成功，共耗时" + timer.ElapsedMilliseconds + "毫秒。";
                Application.DoEvents();

                //关闭秒钟
                timer.Reset();
                timer.Stop();

                //MessageBox.Show("保存成功！");
                /***
                    //成功提示
                    if (MessageBox.Show("导出成功，是否立即打开？", "提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information) == DialogResult.Yes)
                    {
                        System.Diagnostics.Process.Start(localFilePath);
                    }
                    if (lblStatus != null && barStatus != null)
                    {
                        //赋初始值
                        lblStatus.Visible = false;
                        barStatus.Visible = false;
                    }
                 ****/
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "提示", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            finally
            {
                //关闭秒钟
                timer.Reset();
                timer.Stop();
                //赋初始值
                if (lblStatus != null && barStatus != null)
                {
                    //赋初始值
                    lblStatus.Visible = false;
                    barStatus.Visible = false;
                }
            }
        }


    }

}

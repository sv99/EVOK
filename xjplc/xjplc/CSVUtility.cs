using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
namespace xjplc
{
    public class CsvStreamReader
    {
        private ArrayList rowAL;         //行链表,CSV文件的每一行就是一个链
        private string fileName;        //文件名

        private Encoding encoding;        //编码

        public CsvStreamReader()
        {
            this.rowAL = new ArrayList();
            this.fileName = "";
            this.encoding = Encoding.Default;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        public CsvStreamReader(string fileName)
        {
            this.rowAL = new ArrayList();
            this.fileName = fileName;
            this.encoding = Encoding.Default;
            LoadCsvFile();
        }
        public Boolean CheckCSVFile0(string[] str)
        {
            string strcmp;
            for (int i = 0; i < str.Length; i++)
            {

                strcmp = this[1, i + 1];
                if (strcmp != str[i]) return false;
            }
            return true;

        }
       
        
        //北京版本 横着读数据
        public Boolean CheckCSVFile(string[] str)
        {
            string strcmp = "1";
            int k = this.ColCount;
            int c = str.Length;
            if (k == 0 || c==0) return false;
            int j = k > c ? c : k;  
            for (int i = 0; i < j; i++)
            {
                strcmp = this[1, i + 1];
                if (!strcmp .Equals( str[i])) return false;
            }
            return true;
 
        }
        /// <summary>
        ///
        /// </summary>
        /// <param name="fileName">文件名,包括文件路径</param>
        /// <param name="encoding">文件编码</param>
        public CsvStreamReader(string fileName, Encoding encoding)
        {
            this.rowAL = new ArrayList();
            this.fileName = fileName;
            this.encoding = encoding;
            LoadCsvFile();
        }
        public DataTable GetDgvToTable(DataGridView dgv)
        {
            DataTable dt = new DataTable();

            // 列强制转换
            for (int count = 0; count < dgv.Columns.Count; count++)
            {
                DataColumn dc = new DataColumn(dgv.Columns[count].Name.ToString());
                dt.Columns.Add(dc);
            }

            // 循环行
            for (int count = 0; count < dgv.Rows.Count; count++)
            {
                DataRow dr = dt.NewRow();
                for (int countsub = 0; countsub < dgv.Columns.Count; countsub++)
                {
                    dr[countsub] = Convert.ToString(dgv.Rows[count].Cells[countsub].Value);
                }
                dt.Rows.Add(dr);
            }
            return dt;
        }
        public void SaveCSV(DataTable dt, string fileName)
        {
            if (ConstantMethod.FileIsUsed(FileName)) return;
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ",";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ",";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            //MessageBox.Show("CSV文件保存成功！");
        }

        public void SaveCSV0(DataTable dt, string fileName)
        {
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Create, System.IO.FileAccess.Write);
            StreamWriter sw = new StreamWriter(fs, System.Text.Encoding.Default);
            string data = "";

            //写出列名称
            for (int i = 0; i < dt.Columns.Count; i++)
            {
                data += dt.Columns[i].ColumnName.ToString();
                if (i < dt.Columns.Count - 1)
                {
                    data += ";";
                }
            }
            sw.WriteLine(data);

            //写出各行数据
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                data = "";
                for (int j = 0; j < dt.Columns.Count; j++)
                {
                    data += dt.Rows[i][j].ToString();
                    if (j < dt.Columns.Count - 1)
                    {
                        data += ";";
                    }
                }
                sw.WriteLine(data);
            }

            sw.Close();
            fs.Close();
            //MessageBox.Show("CSV文件保存成功！");
        }

        /// <summary>
        /// 文件名,包括文件路径
        /// </summary>
        public string FileName
        {
            set
            {
                this.fileName = value;
                LoadCsvFile();
            }
            get 
            {
                return this.fileName;
            }
        }

        /// <summary>
        /// 文件编码
        /// </summary>

        public Encoding FileEncoding
        {
            set
            {
                this.encoding = value;
            }
        }

        /// <summary>
        /// 获取行数
        /// </summary>
        public int RowCount
        {
            get
            {
                return this.rowAL.Count;
            }
        }
        public int ColCountReturn()
        {
            int i = 1;
            if (this.ColCount < 1) return 0;
            while (this[1, i] != "")
            {
                i++;
                if (i > this.ColCount) break;
                if (this[1, i] == null)
                    break;
            }

            return (i - 2);

        }
        public int RowCountReturn()
        {
            int i = 1;
            if (this.rowAL.Count < 1) return 0;
            while (this[i, 1] != "")
            {
                i++;
                if (i > this.rowAL.Count) break;
                if (this[i, 1] == null)
                    break;
            }

            return (i-2); 
            
        }
        //清理数据
        public void DataClear()
        {
            this.rowAL.Clear();
        }
        /// <summary>
        /// 获取列数
        /// </summary>
        public int ColCount
        {
            get
            {
                int maxCol;

                maxCol = 0;
                for (int i = 0; i < this.rowAL.Count; i++)
                {
                    ArrayList colAL = (ArrayList)this.rowAL[i];

                    maxCol = (maxCol > colAL.Count) ? maxCol : colAL.Count;
                }

                return maxCol;
            }
        }


        /// <summary>
        /// 获取某行某列的数据

        /// row:行,row = 1代表第一行

        /// col:列,col = 1代表第一列  
        /// </summary>
        public string this[int row, int col]
        {
            get
            {
                //数据有效性验证

                CheckRowValid(row);
                CheckColValid(col);
                ArrayList colAL = (ArrayList)this.rowAL[row - 1];

                //如果请求列数据大于当前行的列时,返回空值

                if (colAL.Count < col)
                {
                    return "";
                }

                return colAL[col - 1].ToString();
            }
        }


        /// <summary>
        /// 根据最小行，最大行，最小列，最大列，来生成一个DataTable类型的数据

        /// 行等于1代表第一行

        /// 列等于1代表第一列

        /// maxrow: -1代表最大行
        /// maxcol: -1代表最大列
        /// </summary>
        public DataTable this[int minRow, int maxRow, int minCol, int maxCol]
        {
            get
            {
                //数据有效性验证

                CheckRowValid(minRow);
                CheckMaxRowValid(maxRow);
                CheckColValid(minCol);
                CheckMaxColValid(maxCol);
                if (maxRow == -1)
                {
                    maxRow = RowCount;
                }
                if (maxCol == -1)
                {
                    maxCol = ColCount;
                }
                if (maxRow < minRow)
                {
                    throw new Exception("最大行数不能小于最小行数");
                }
                if (maxCol < minCol)
                {
                    throw new Exception("最大列数不能小于最小列数");
                }
                DataTable csvDT = new DataTable();
                int i;
                int col;
                int row;

                //增加列

                for (i = minCol; i <= maxCol; i++)
                {
                    csvDT.Columns.Add(i.ToString());
                }
                for (row = minRow; row <= maxRow; row++)
                {
                    DataRow csvDR = csvDT.NewRow();

                    i = 0;
                    for (col = minCol; col <= maxCol; col++)
                    {
                        csvDR[i] = this[row, col];
                        i++;
                    }
                    csvDT.Rows.Add(csvDR);
                }

                return csvDT;
            }
        }


        /// <summary>
        /// 检查行数是否是有效的

        /// </summary>
        /// <param name="col"></param>  
        private void CheckRowValid(int row)
        {
            if (row <= 0)
            {
                throw new Exception("行数不能小于0");
            }
            if (row > RowCount)
            {
                throw new Exception("没有当前行的数据");
            }
        }

        /// <summary>
        /// 检查最大行数是否是有效的

        /// </summary>
        /// <param name="col"></param>  
        private void CheckMaxRowValid(int maxRow)
        {
            if (maxRow <= 0 && maxRow != -1)
            {
                throw new Exception("行数不能等于0或小于-1");
            }
            if (maxRow > RowCount)
            {
                throw new Exception("没有当前行的数据");
            }
        }

        /// <summary>
        /// 检查列数是否是有效的

        /// </summary>
        /// <param name="col"></param>  
        private void CheckColValid(int col)
        {
            if (col <= 0)
            {
                throw new Exception("列数不能小于0");
            }
            if (col > ColCount)
            {
                throw new Exception("没有当前列的数据");
            }
        }

        /// <summary>
        /// 检查检查最大列数是否是有效的

        /// </summary>
        /// <param name="col"></param>  
        private void CheckMaxColValid(int maxCol)
        {
            if (maxCol <= 0 && maxCol != -1)
            {
                throw new Exception("列数不能等于0或小于-1");
            }
            if (maxCol > ColCount)
            {
                throw new Exception("没有当前列的数据");
            }
        }
        /// 将CSV文件的数据读取到DataTable中
        ///
        /// CSV文件路径 
        /// 返回读取了CSV数据的DataTable
        public DataTable OpenCSV(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');

                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {

                    DataRow dr = dt.NewRow();
                    for (int j = 0; j < aryLine.Length; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);

                }
            }
                sr.Close();
                fs.Close();
                return dt;            
        }
        //分号
        public DataTable OpenCSV0(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(';');

                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn(aryLine[i]);
                        dt.Columns.Add(dc);
                    }
                }
                else
                {

                    DataRow dr = dt.NewRow();
                    int maxCol = 0;
                    if (aryLine.Length < dt.Columns.Count)
                    {
                        maxCol = aryLine.Length;
                    }else
                    maxCol = dt.Columns.Count;
                    for (int j = 0; j < maxCol; j++)
                    {
                        dr[j] = aryLine[j];
                    }
                    dt.Rows.Add(dr);

                }
            }
            sr.Close();
            fs.Close();
            return dt;
        }
        /// 将CSV文件的数据读取到DataTable中
        ///
        /// CSV文件路径 这个函数不对 是从索菲亚修改独立防漏件的时候用的 不使用
        /// 返回读取了CSV数据的DataTable
        public DataTable OpenCSVSophia(string fileName)
        {
            DataTable dt = new DataTable();
            FileStream fs = new FileStream(fileName, System.IO.FileMode.Open, System.IO.FileAccess.Read);
            StreamReader sr = new StreamReader(fs, System.Text.Encoding.Default);
            //记录每次读取的一行记录
            string strLine = "";
            //记录每行记录中的各字段内容
            string[] aryLine;
            //标示列数
            int columnCount = 0;
            //标示是否是读取的第一行
            bool IsFirst = true;

            for (int i = 0; i < 5; i++)
            {
                DataColumn dc = new DataColumn("Param" + i.ToString());
                dt.Columns.Add(dc);
            }

            //逐行读取CSV中的数据
            while ((strLine = sr.ReadLine()) != null)
            {
                aryLine = strLine.Split(',');
                /*****
                if (IsFirst == true)
                {
                    IsFirst = false;
                    columnCount = aryLine.Length;
                    //创建列
                    for (int i = 0; i < columnCount; i++)
                    {
                        DataColumn dc = new DataColumn("Param"+i.ToString());
                        dt.Columns.Add(dc);
                    }
                }
                else
                {
                ***/
                DataRow dr = dt.NewRow();
                for (int j = 0; j < aryLine.Length; j++)
                {
                    dr[j] = aryLine[j];
                }
                dt.Rows.Add(dr);
                // }
            }

            sr.Close();
            fs.Close();
            return dt;
        }
        /// <summary>
        /// 载入CSV文件
        /// </summary>
        private void LoadCsvFile()
        {
            //对数据的有效性进行验证

            if (this.fileName == null)
            {
                return;
            }
            else if (!File.Exists(this.fileName))
            {
                throw new Exception("指定的CSV文件不存在");
            }
            else
            {
            }
            if (this.encoding == null)
            {
                this.encoding = Encoding.Default;
            }

            StreamReader sr = new StreamReader(this.fileName, this.encoding);
            string csvDataLine;

            csvDataLine = "";
            while (true)
            {
                string fileDataLine;

                fileDataLine = sr.ReadLine();
                if (fileDataLine == null)
                {
                    break;
                }
                if (csvDataLine == "")
                {
                    csvDataLine = fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
                }
                else
                {
                    csvDataLine += "\r\n" + fileDataLine;//GetDeleteQuotaDataLine(fileDataLine);
                }
                //如果包含偶数个引号，说明该行数据中出现回车符或包含逗号
                if (!IfOddQuota(csvDataLine))
                {
                    AddNewDataLine(csvDataLine);
                    csvDataLine = "";
                }
            }
            sr.Close();
            //数据行出现奇数个引号
            if (csvDataLine.Length > 0)
            {
                throw new Exception("CSV文件的格式有错误");
            }
        }

        /// <summary>
        /// 获取两个连续引号变成单个引号的数据行
        /// </summary>
        /// <param name="fileDataLine">文件数据行</param>
        /// <returns></returns>
        private string GetDeleteQuotaDataLine(string fileDataLine)
        {
            return fileDataLine.Replace("\"\"", "\"");
        }
        public void SaveCompare(DataTable dt)
        {
            int rowMaxCount = dt.Rows.Count > RowCount ? RowCount : dt.Rows.Count;
            int colMaxCount = dt.Rows.Count > RowCount ? RowCount : dt.Rows.Count;
            for (int i = 0; i < rowMaxCount; i++)
            {
                for (int j = 0; j < colMaxCount; j++)
                {
                    if (!this[i, j].Equals(dt.Rows[i][j].ToString()))
                    {
                        ArrayList colAL = (ArrayList)this.rowAL[i-1];

                        colAL[j - 1] = dt.Rows[i][j].ToString();

                    }
                }
            }

        }
        /// <summary>
        /// 判断字符串是否包含奇数个引号
        /// </summary>
        /// <param name="dataLine">数据行</param>
        /// <returns>为奇数时，返回为真；否则返回为假</returns>
        private bool IfOddQuota(string dataLine)
        {
            int quotaCount;
            bool oddQuota;

            quotaCount = 0;
            for (int i = 0; i < dataLine.Length; i++)
            {
                if (dataLine[i] == '\"')
                {
                    quotaCount++;
                }
            }

            oddQuota = false;
            if (quotaCount % 2 == 1)
            {
                oddQuota = true;
            }

            return oddQuota;
        }

        /// <summary>
        /// 判断是否以奇数个引号开始

        /// </summary>
        /// <param name="dataCell"></param>
        /// <returns></returns>
        private bool IfOddStartQuota(string dataCell)
        {
            int quotaCount;
            bool oddQuota;

            quotaCount = 0;
            for (int i = 0; i < dataCell.Length; i++)
            {
                if (dataCell[i] == '\"')
                {
                    quotaCount++;
                }
                else
                {
                    break;
                }
            }

            oddQuota = false;
            if (quotaCount % 2 == 1)
            {
                oddQuota = true;
            }

            return oddQuota;
        }

        /// <summary>
        /// 判断是否以奇数个引号结尾
        /// </summary>
        /// <param name="dataCell"></param>
        /// <returns></returns>
        private bool IfOddEndQuota(string dataCell)
        {
            int quotaCount;
            bool oddQuota;

            quotaCount = 0;
            for (int i = dataCell.Length - 1; i >= 0; i--)
            {
                if (dataCell[i] == '\"')
                {
                    quotaCount++;
                }
                else
                {
                    break;
                }
            }

            oddQuota = false;
            if (quotaCount % 2 == 1)
            {
                oddQuota = true;
            }

            return oddQuota;
        }

        /// <summary>
        /// 加入新的数据行

        /// </summary>
        /// <param name="newDataLine">新的数据行</param>
        private void AddNewDataLine(string newDataLine)
        {
            //System.Diagnostics.Debug.WriteLine("NewLine:" + newDataLine);

            ////return;

            ArrayList colAL = new ArrayList();
            string[] dataArray = newDataLine.Split(',');
            bool oddStartQuota;        //是否以奇数个引号开始

            string cellData;

            oddStartQuota = false;
            cellData = "";
            for (int i = 0; i < dataArray.Length; i++)
            {
                if (oddStartQuota)
                {
                    //因为前面用逗号分割,所以要加上逗号
                    cellData += "," + dataArray[i];
                    //是否以奇数个引号结尾
                    if (IfOddEndQuota(dataArray[i]))
                    {
                        colAL.Add(GetHandleData(cellData));
                        oddStartQuota = false;
                        continue;
                    }
                }
                else
                {
                    //是否以奇数个引号开始

                    if (IfOddStartQuota(dataArray[i]))
                    {
                        //是否以奇数个引号结尾,不能是一个双引号,并且不是奇数个引号

                        if (IfOddEndQuota(dataArray[i]) && dataArray[i].Length > 2 && !IfOddQuota(dataArray[i]))
                        {
                            colAL.Add(GetHandleData(dataArray[i]));
                            oddStartQuota = false;
                            continue;
                        }
                        else
                        {

                            oddStartQuota = true;
                            cellData = dataArray[i];
                            continue;
                        }
                    }
                    else
                    {
                        colAL.Add(GetHandleData(dataArray[i]));
                    }
                }
            }
            if (oddStartQuota)
            {
                throw new Exception("数据格式有问题");
            }
            this.rowAL.Add(colAL);
        }
        //释放资源
        public void Dispose()
        {
           rowAL=null;         //行链表,CSV文件的每一行就是一个链
          fileName = null;        //文件名

          encoding = null;        //编码
            ////GC.Collect();
            ////GC.WaitForPendingFinalizers();

    }
        /// <summary>
        /// 去掉格子的首尾引号，把双引号变成单引号

        /// </summary>
        /// <param name="fileCellData"></param>
        /// <returns></returns>
        private string GetHandleData(string fileCellData)
        {
            if (fileCellData == "")
            {
                return "";
            }
            if (IfOddStartQuota(fileCellData))
            {
                if (IfOddEndQuota(fileCellData))
                {
                    return fileCellData.Substring(1, fileCellData.Length - 2).Replace("\"\"", "\""); //去掉首尾引号，然后把双引号变成单引号
                }
                else
                {
                    throw new Exception("数据引号无法匹配" + fileCellData);
                }
            }
            else
            {
                //考虑形如""    """"      """"""   
                if (fileCellData.Length > 2 && fileCellData[0] == '\"')
                {
                    fileCellData = fileCellData.Substring(1, fileCellData.Length - 2).Replace("\"\"", "\""); //去掉首尾引号，然后把双引号变成单引号
                }
            }

            return fileCellData;
        }
    }
}

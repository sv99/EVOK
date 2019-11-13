using FastReport;
using FastReport.Barcode;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;
namespace fileconvert
{
    public partial class FormFileConvert : Form
    {

        ExcelNpoi exop;
        CsvStreamReader csvop;

        DataTable UserDt;

        CsvStreamReader csvSaveDemo;

        ConfigFileManager paraFile;

        int userSelectIdx = 0;

        int userId=0;
        public int UserSelectIdx
        {
            get { return userSelectIdx; }
            set { userSelectIdx = value; }
        }
        bool isSaveConfig;
        public bool IsSaveConfig
        {
            get { return isSaveConfig; }
            set { isSaveConfig = value; }
        }

        List<int> valueCol = new List<int>();
        public FormFileConvert()
        {
            InitializeComponent();        
        }

        void Init()
        {
            exop = new ExcelNpoi();
            csvop = new CsvStreamReader();
            UserDt = new DataTable();

            DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DialogExcelDataLoad.Filter = "文件(*.xls,*.xlsx,*.csv)|*.xls;*.csv;*.xlsx";
            DialogExcelDataLoad.FileName = "请选择数据文件";

            demoLoadDialog.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            demoLoadDialog.Filter = "文件(*.csv)|*.csv;";
            demoLoadDialog.FileName = "请选择数据模板文件";

            //条码
            op1.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            op1.Filter = "文件(*.frx)|*.frx;";
            op1.FileName = "请选择条码模板文件";

            csvSaveDemo = new CsvStreamReader();

            

            if (File.Exists(Constant.barCodeDemo))
            {
                printReport.Load(Constant.barCodeDemo);
            }
            else
            {
                barCodeButton.Enabled = false;
                MessageBox.Show("条码文件不存在！");
            }


            //20181018读取参数 因为欧派需要根据特定的参数进行分类
            paraFile = new ConfigFileManager();
            if (File.Exists(Constant.ConfigParamFilePath))
            {
                paraFile.LoadFile(Constant.ConfigParamFilePath);
            }

            
            if (int.TryParse(paraFile.ReadConfig(Constant.userName), out userId))
            {
                //用户名获取成功
                if(userId==Constant.hdiaoId)
                {
                    menu.Enabled = false;
                    csv1.Enabled = false;
                    csv2.Enabled = false;

                }
            }

            paramStr = new List<string>();

            if (userId != Constant.hdiaoId)
            if (!ReadFileDemo(""))
            {
                ConstantMethod.AppExit();
            }
           
            int i = 1;
            string s="";            
            while (!string.IsNullOrWhiteSpace(s=paraFile.ReadConfig(Constant.strParam + i.ToString())))
            {
                string ss = s;
                paramStr.Add(ss);
                i++;
            }


            stopConfig();

        }

        List<string> paramStr;
        public void DeleteStr(ref string beizhu, string s)
        {
            if (s.Length > 0 && beizhu.Contains(s))
                beizhu = beizhu.Replace(s, "");
        }
        #region 华雕转换
        //华雕转换
        public void hdiao(string  PathStr)
        {
            if (UserDt.Rows.Count == 0)
            {
                ConstantMethod.ShowInfo(rtbResult, "无数据！");
                return;
            }
            //获取有效行号
            int rowStart= UserDt.Rows.Count - 1;
            int rowEnd  = UserDt.Rows.Count - 1;
            //再转换长宽为尺寸
            for(int i = 0; i < UserDt.Rows.Count; i++)
            {
                List<object> arr = new List<object>(UserDt.Rows[i].ItemArray);

                string[] data= arr.ConvertAll(c => { return c.ToString(); }).ToArray();
              
                if (ConstantMethod.compareString(data,Constant.stValueArray))
                {
                    rowStart = i;
                }
                if (UserDt.Rows[i][0].ToString().Equals(Constant.edValue))
                {
                    rowEnd = i;
                }

            }
     
            //获取数量
            if (rowStart > 0 && rowStart < rowEnd && UserDt.Rows.Count>8)
            {
                DataTable dtOutPutTmp = ConstantMethod.getDataTableByString(Constant.strformatZh);

                string customName = UserDt.Rows[0][0].ToString();

                //长度 宽度都拿来当尺寸 同时增加客户姓名 增加标记号
                for (int i = rowStart + 1; i < rowEnd; i++)
                {
                    DataRow dr = dtOutPutTmp.NewRow();

                    dr[0] = UserDt.Rows[i][1].ToString();
                    int cnt = 0;
                    if (int.TryParse(UserDt.Rows[i][5].ToString(), out cnt))
                    {
                        dr[1] = (cnt*2).ToString();
                    }
                    else
                    {
                        dr[1] = 1;
                    }
                    
                    dr[2] = "0";
                    dr[3] = UserDt.Rows[i][1].ToString();
                    dr[4] = UserDt.Rows[i][0].ToString();
                    dr[5] = UserDt.Rows[i][3].ToString();
                    dr[6] = UserDt.Rows[i][4].ToString();
                    dr[7] = UserDt.Rows[i][6].ToString();
                    dr[8] = UserDt.Rows[i][7].ToString();
                    dr[9] = UserDt.Rows[i][8].ToString();
                    dr[10] = customName;
                    dr[11] = i.ToString() + "--1";
                    dr[12] = UserDt.Rows[i][1].ToString();
                    dr[13] = UserDt.Rows[i][2].ToString();
                    DataRow dr0 = dtOutPutTmp.NewRow();

                    dr0.ItemArray = dr.ItemArray;
                    dr0[0] = UserDt.Rows[i][2].ToString();
                    dr0[3] = UserDt.Rows[i][2].ToString();
                    dr0[11] = i.ToString() + "--2";
                    dtOutPutTmp.Rows.Add(dr);
                    dtOutPutTmp.Rows.Add(dr0);
                }
                //存在则开始分割表格 把表格集合 第一给dtouput 用户可以查看条码
                List<DataTable> dt = new List<DataTable>();              
                getDataTableByParam(dt, dtOutPutTmp, paramStr);
                foreach (DataTable dttemp in dt)
                {
                    if(dttemp.Rows[1][8] !=null)
                    fileManager.
                    SaveFileWithDefault(dttemp, PathStr,dttemp.Rows[1][8].ToString());
                    else ConstantMethod.ShowInfo(rtbResult, "分类标志错误！");
                }
                
            }
            else
                ConstantMethod.ShowInfo(rtbResult,"数据格式错误！");

            ConstantMethod.ShowInfo(rtbResult, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "转换结束！");
        }

        #endregion
        //通用型转换
        public void fileConvertFun(string PathStr)
        {

            //首先判断下 参数名 是否在源表格列名中存在
            bool isParamExist = true;
            string[] strColumns = new string[UserDt.Columns.Count];
            for (int i = 0; i < UserDt.Columns.Count; i++)
            {
                strColumns[i] = UserDt.Columns[i].ColumnName;
            }
            for (int i = 0; i < paramStr.Count; i++)
            {
                if (!strColumns.Contains(paramStr[i]))
                {
                    isParamExist = false;
                }
            }
            //不存在就代表参数没用
            if (!isParamExist)
            {
                if (
                (dtOutPut = fileManager.
                saveDataTableToFile(rtbResult, PathStr, pBar1, UserDt, valueCol, true, null)) == null)
                {
                    ConstantMethod.ShowInfo(rtbResult, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "转换错误，请检查转换规则！");
                }
            }
            else
            {
                //存在则开始分割表格 把表格集合 第一给dtouput 用户可以查看条码
                List<DataTable> dt = new List<DataTable>();
                getDataTableByParam(dt, UserDt, paramStr);
                foreach (DataTable dttemp in dt)
                {
                    if (dtOutPut == null)
                    {
                        if((dtOutPut = fileManager.saveDataTableToFile(rtbResult, PathStr, pBar1, dttemp, valueCol, false,paramStr))==null)
                        {
                            ConstantMethod.ShowInfo(rtbResult, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "转换错误，请检查转换规则！");
                        }
                        
                    }
                    else
                    {
                        if (fileManager.saveDataTableToFile(rtbResult, PathStr, pBar1, dttemp, valueCol, false, paramStr) == null)
                        {
                            ConstantMethod.ShowInfo(rtbResult, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "转换错误，请检查转换规则！");
                        }
                    }
                }
                
            }

           ConstantMethod.ShowInfo(rtbResult, DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss") + "转换结束！");

        }
        //传入要取的列名 paramL 和要比较的值 
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dt"></param>  要查询的表格
        /// <param name="paramL"></param> 要取拿一列 其中的列名
        /// <param name="value"></param> 这一列的 第一行的值
        /// <returns></returns>
        bool jugeIsParamExist(DataTable dt,List<string> paramL,List<string> value)
        {
            List<string> checkStr = new List<string>();
            if (dt.Rows.Count > 0)
            {
                foreach (string s in paramL)
                {
                    checkStr.Add(dt.Rows[0][s].ToString());
                }
                return ConstantMethod.compareString(checkStr.ToArray(),value.ToArray());
            }          
            return false;
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dtLst"></param> 表格集合
        /// <param name="res"></param>源表格
        /// <param name="paramL"></param>根据哪几列 来分
        void getDataTableByParam(List<DataTable> dtLst,DataTable res,List<string> paramL)
        {
            if (paramL.Count == 0) return;
           
            //思路逐行扫描 扫到表格列表没有的样式 就新建表格
            foreach(DataRow dr in res.Rows)
            {
                //获取当前行的值
                List<string> paramStrLst = new List<string>();
                foreach (string param in paramL)
                {                 
                    paramStrLst.Add(dr[param].ToString().Trim());                   
                }

                //开始判断当前行是否在表格集合里是否存在
                bool isExit = false;
                //寻找表格
                foreach (DataTable dt in dtLst)
                {
                    if (jugeIsParamExist(dt, paramL, paramStrLst))
                     {
                        isExit = true;
                        DataRow dr0 = dt.NewRow();
                        dr0.ItemArray = dr.ItemArray;
                        dt.Rows.Add(dr0);
                        break;
                     }
                }
                //如果样式没有那就创建表格
                if (!isExit)
                {
                    DataTable dtTemp = res.Clone();
                    DataRow dr0 = dtTemp.NewRow();
                    dr0.ItemArray = dr.ItemArray;
                    dtTemp.Rows.Add(dr0);           
                    dtLst.Add(dtTemp);
                }                           
            }

        }

        DataTable dtOutPut ;
        private void button3_Click(object sender, EventArgs e)
        {
            switch (userId)
            {
                case Constant.hdiaoId:
                    {
                        hdiao(DialogExcelDataLoad.FileName);
                        break;
                    }
                default:
                    {
                        fileConvertFun(DialogExcelDataLoad.FileName);
                        break;
                    }
            }                     
                     
            if (File.Exists(Constant.barCodeDemo))
            {
                barCodeButton.Enabled = true;
            }
        }
        public void ShowBarCode(int rowindex)
        {
            List<string> valuestr = new List<string>();

            if (dtOutPut != null && dtOutPut.Rows.Count > 0)
            {
                DataRow dr = dtOutPut.Rows[rowindex];
                for (int j = 3; j < dtOutPut.Columns.Count; j++)
                {
                    valuestr.Add(dr[j].ToString());
                }           

                printBarcode(printReport, valuestr.ToArray());
            }
            else
            {
                MessageBox.Show("无数据，请先导出数据！");
            }
        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {

            if (!IsSaveConfig)
            {
                ShowBarCode(dgv.CurrentRow.Index);
                return;
            }

            //可以重复选择
            if (valueCol.Contains(e.ColumnIndex))
            {

              // MessageBox.Show("请勿重复选取!");
              //  return;
            }

            valueCol.Add(e.ColumnIndex);
           
            string colname = UserDt.Columns[e.ColumnIndex].ColumnName;
            
            listBox2.Items.Add(colname);

            int colid = e.ColumnIndex + 1;

            if (valueCol.Count == 1)           
               ConstantMethod.ShowInfo(rtbResult, "添加第" + colid.ToString() + "列:" + colname + "====>尺寸！");
            
            if (valueCol.Count == 2)
             
                ConstantMethod.ShowInfo(rtbResult, "添加第" + colid.ToString() + "列:"+ colname + "====>设定数量！");

            if (valueCol.Count == 3)
                ConstantMethod.ShowInfo(rtbResult, "添加第" + colid.ToString()  +"列:" + colname + "====>条码！");

            if (valueCol.Count > 3)
            {
                ConstantMethod.ShowInfo(rtbResult, "添加第" + colid.ToString() +  "列:" + colname + "====>参数:" + (valueCol.Count-3).ToString());
            }

        }
        public void startConfig()
        {
          
            button3.Enabled = false;
            valueCol.Clear();
            rtbResult.Clear();
            IsSaveConfig = true;
            InitSaveConfig();
        }

        public void stopConfig()
        {
            

            button3.Enabled = true;
            IsSaveConfig = false;
            listBox1.Visible = false;
            listBox2.Visible = false;
            button1.Visible = false;
        }

        public void SaveFileDemo()
        {
            DataTable dt = new DataTable();

            if (valueCol.Count > 0)
            {

                
                DataColumn dtcolSize = new DataColumn("尺寸");

                DataColumn dtcolCnt = new DataColumn("设定数量");


                DataColumn dtcolBarCode = new DataColumn("条码");


                dt.Columns.Add(dtcolSize);
                dt.Columns.Add(dtcolCnt);
                dt.Columns.Add(dtcolBarCode);

                //增加列
                for (int i = 0; i < (valueCol.Count - 3); i++)
                {
                    DataColumn dtcolParm = new DataColumn("参数" + (i + 1).ToString());
                    dt.Columns.Add(dtcolParm);
                }

                DataRow dr = dt.NewRow();

                for (int i = 0; i < valueCol.Count; i++)
                {
                    dr[i] = valueCol[i];
                }

                dt.Rows.Add(dr);
                string filename = Constant.SaveFileDemo;
                csvSaveDemo.SaveCSV(dt,filename);
            }

        }
        public bool ReadFileDemo(string filename)
        {
            DataTable dt = new DataTable();
            if (string.IsNullOrWhiteSpace(filename))
                dt = csvSaveDemo.OpenCSV(Constant.SaveFileDemo);
            else
            {
                dt = csvSaveDemo.OpenCSV(filename);
            }
            rtbResult.Clear();
            if (dt.Rows.Count == 1)
            {
                valueCol.Clear();

                foreach (DataRow dr in dt.Rows)
                {
                    for (int i = 0; i < dr.ItemArray.Length; i++)
                    {
                        int s = 0;
                        if (int.TryParse(dr.ItemArray[i].ToString(), out s))
                        {
                            valueCol.Add(s);                           
                        }
                        else
                        {
                            MessageBox.Show("指定列错误，错误列号：" + (i + 1).ToString());
                            return false;
                        }
                    }
                }
                
                ConstantMethod.ShowInfo(rtbResult,"尺寸====>第"+valueCol[0]+"列");
                ConstantMethod.ShowInfo(rtbResult, "设定数量====>第" + valueCol[1] + "列");
                ConstantMethod.ShowInfo(rtbResult, "条码====>第" + valueCol[2] + "列");

                for (int i = 3; i < valueCol.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbResult, "参数"+(i-2).ToString()+"====>第" + valueCol[i] + "列");
                }
            }
            else
            {
                MessageBox.Show("加载模板数据错误！");
                return false;
            }

            return true;          

        }

        public void showColName()
        {
            rtbResult.Clear();
            if (valueCol.Count > 0)
            {
                ConstantMethod.ShowInfo(rtbResult, "尺寸====>第" + valueCol[0] + "列:" + UserDt.Columns[valueCol[0]].ColumnName);
                ConstantMethod.ShowInfo(rtbResult, "设定数量====>第" + valueCol[1] + "列:" + UserDt.Columns[valueCol[1]].ColumnName);
                ConstantMethod.ShowInfo(rtbResult, "条码====>第" + valueCol[2] + "列:" + UserDt.Columns[valueCol[2]].ColumnName);

                for (int i = 3; i < valueCol.Count; i++)
                {
                    ConstantMethod.ShowInfo(rtbResult, "参数" + (i - 2).ToString() + "====>第" + valueCol[i] + "列:" + UserDt.Columns[valueCol[i]].ColumnName);
                }
            }

        }
        private void button5_Click(object sender, EventArgs e)
        {          
           

        }

        public void AddBlankSpace(DataTable userdt,int count)
        {
            for (int i = 0; i < count; i++)
            {
                DataColumn dcol = new DataColumn("空白"+i.ToString());
                userdt.Columns.Add(dcol);
            }
        }
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult drresult;
            if (isSaveConfig)
            {
                drresult = MessageBox.Show("还有数据未保存!", "是否保存?",
                             MessageBoxButtons.YesNo,
                             MessageBoxIcon.Information,
                             MessageBoxDefaultButton.Button2);
                if (drresult == DialogResult.Yes)
                {
                    IsSaveConfig = false;
                }
                else
                {
                    ConstantMethod.AppExit();
                }
            }

        }

        private void button6_Click(object sender, EventArgs e)
        {

        }       
        private void 导入CSV文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {
                ConstantMethod.SaveDirectoryByFileDialog(DialogExcelDataLoad);

                string localFilePath = DialogExcelDataLoad.FileName;

                string FileExt = Path.GetExtension(localFilePath).ToLower();
               
                if (FileExt.Equals(Constant.CSVFileEX))
                {
                    UserDt = null;
                                      
                    UserDt = csvop.OpenCSV1(DialogExcelDataLoad.FileName);

                    if (UserDt != null && UserDt.Columns.Count > 3)
                    {
                        dgv.DataSource = UserDt;
                        showColName();
                    }
                    else
                    {
                        MessageBox.Show("加载文件错误！");
                    }
                }
                else
                {
                    MessageBox.Show("文件格式错误！");
                }
            }
        }

        private void 导入CSVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {
                ConstantMethod.SaveDirectoryByFileDialog(DialogExcelDataLoad);

                string localFilePath = DialogExcelDataLoad.FileName;

                string FileExt = Path.GetExtension(localFilePath).ToLower();

                if (FileExt.Equals(Constant.CSVFileEX))
                {
                    UserDt = null;
                  
                    UserDt = csvop.OpenCSV0(DialogExcelDataLoad.FileName);


                    if (UserDt != null && UserDt.Columns.Count > 3)
                    {
                        dgv.DataSource = UserDt;
                        showColName();
                    }
                    else
                    {
                        MessageBox.Show("加载文件错误！");
                    }
                }
                else
                {
                    MessageBox.Show("文件格式错误！");
                }
            }
        }

        private void 保存配置文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            stopConfig();
            SaveFileDemo();
        }
        void InitSaveConfig()
        {
            listBox2.Items.Clear();
            listBox1.Visible = true;
            listBox2.Visible = true;

            button1.Visible = true;
        }

        private void 设置导出模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            startConfig();

            ConstantMethod.ShowInfo(rtbResult, "双击任意一个单元指定每一列的数据内容，请按照尺寸，设定数量，条码，参数1..参数2..方式进行选取！");

            this.Focus();

     


            while (IsSaveConfig)
            {
                Application.DoEvents();
            }

            stopConfig();
        }

        private void 加载文件模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (demoLoadDialog.ShowDialog() == DialogResult.OK)
            {
                ConstantMethod.SaveDirectoryByFileDialog(demoLoadDialog);
                if (ReadFileDemo(demoLoadDialog.FileName))
                {
                    
                }
            }
        }

        private void FormFileConvert_Load(object sender, EventArgs e)
        {
            Init();
        }

        private void 导入excel文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {
                ConstantMethod.SaveDirectoryByFileDialog(DialogExcelDataLoad);

                UserDt = null;

                string localFilePath = DialogExcelDataLoad.FileName;

                string FileExt = Path.GetExtension(localFilePath).ToLower();
                if (FileExt.Equals(Constant.ExcelFileEX0) || FileExt.Equals(Constant.ExcelFileEX1))
                {
                    UserDt = exop.ImportExcel(DialogExcelDataLoad.FileName);
                    if (UserDt != null && UserDt.Columns.Count > 3)
                    {
                        dgv.DataSource = UserDt;
                        showColName();
                    }
                    else
                    {
                        MessageBox.Show("加载文件错误！");
                    }
                }
                else
                {
                    MessageBox.Show("文件格式错误！");
                }


            }

        }
        public void printBarcode(Report rp1, object s2)
        {

            
            string[] s1 = (string[])s2;           
            if (s1 != null && printReport != null)
            {
                try
                {
                    //在遇到结巴的情况下 保存下当前打印模式
                    //OldPrintBarCodeMode = PrintBarCodeMode;         

                    Application.DoEvents();

                if (rp1.FindObject("Barcode1") != null)
                    (rp1.FindObject("Barcode1") as BarcodeObject).Text = s1[0];


                for (int i = 1; i < s1.Length; i++)
                {
                    if (rp1.FindObject("Text" + (i).ToString()) != null && string.IsNullOrWhiteSpace(s1[i]))
                    {
                        (rp1.FindObject("Text" + (i).ToString()) as TextObject).Text = "";

                        continue;
                    }                  
                    //其他参数另外选
                    
                    if (rp1.FindObject("Text" + (i).ToString()) != null && (!string.IsNullOrWhiteSpace(s1[i])))
                    {
                       /***
                        if (s1[i].Contains('['))
                        {
                            s1[i] = s1[i].Replace('[', ' ');
                        }
                        if (s1[i].Contains(']'))
                        {
                            s1[i] = s1[i].Replace(']', ' ');
                        }
                   ***/
                       s1[i] = ConstantMethod.ShiftString(s1[i]);
                        (rp1.FindObject("Text" + (i).ToString()) as TextObject).Text = s1[i];
                    }
                }
              
                    rp1.Prepare();
                    rp1.Show();
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
               
            }
        }


        private void 查看条码模板ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<string> valuestr = new List<string>();
            
            if (dtOutPut != null && dtOutPut.Rows.Count > 0)
            {


                //foreach (DataRow dr in dtOutPut.Rows)
                // {
                DataRow dr = dtOutPut.Rows[dgv.CurrentRow.Index];
                for (int j = 3; j < dtOutPut.Columns.Count; j++)
                {
                    valuestr.Add(dr[j].ToString());
                }

                //  break;
                //}

                printBarcode(printReport, valuestr.ToArray());
            }
            else
            {
                MessageBox.Show("无数据，请先导出数据！");
            }
            
        }
        OpenFileDialog op1 = new OpenFileDialog();

        private void 加载条码ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (op1.ShowDialog()==DialogResult.OK)
            {
                op1.InitialDirectory = Path.GetDirectoryName(op1.FileName);
                if (File.Exists(op1.FileName))
                {
                    printReport.Load(op1.FileName);
                }
                else
                {
                    barCodeButton.Enabled = false;
                    MessageBox.Show("条码文件不存在！");
                }
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (valueCol.Count > 0 && IsSaveConfig)
            {
           
                ConstantMethod.ShowInfo(rtbResult,"删除"+UserDt.Columns[valueCol[valueCol.Count - 1]].ColumnName);
                valueCol.RemoveAt(valueCol.Count - 1);
                listBox2.Items.RemoveAt(listBox2.Items.Count-1);
            }
        }
    }
}
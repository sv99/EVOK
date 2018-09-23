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

        int userSelectIdx = 0;
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

            if (!ReadFileDemo(""))
            {
                ConstantMethod.AppExit();
            }

            if (File.Exists(Constant.barCodeDemo))
            {
                printReport.Load(Constant.barCodeDemo);
            }
            else
            {
                barCodeButton.Enabled = false;
                MessageBox.Show("条码文件不存在！");
            }

            string beizhu = "014180830001032-PWGA0002 01|";
            DeleteStr(ref beizhu,"|");         

        }
        public void DeleteStr(ref string beizhu, string s)
        {
            if (s.Length > 0 && beizhu.Contains(s))
                beizhu = beizhu.Replace(s, "");
        }
        private void button1_Click(object sender, EventArgs e)
        {         
                          
        }
        private void button2_Click(object sender, EventArgs e)
        {
         
           
        }

        DataTable dtOutPut ;
        private void button3_Click(object sender, EventArgs e)
        {
            dtOutPut = null;
            dtOutPut = new DataTable("file");
            rtbResult.Clear();
            pBar1.Value = 0;
            pBar1.Minimum = 0;
            //收集数据 保存
            if (valueCol.Count > 3  && UserDt.Columns.Count >= valueCol.Count)
            {
                pBar1.Maximum = UserDt.Rows.Count;

                DataColumn dtcolSize = new DataColumn("尺寸");

                DataColumn dtcolCnt = new DataColumn("设定数量");

                DataColumn dtcolCntDone = new DataColumn("已切数量");

                DataColumn dtcolBarCode = new DataColumn("条码");

                dtOutPut.Columns.Add(dtcolSize);
                dtOutPut.Columns.Add(dtcolCnt);
                dtOutPut.Columns.Add(dtcolCntDone);
                dtOutPut.Columns.Add(dtcolBarCode);
                ConstantMethod.ShowInfo(rtbResult, UserDt.Columns[valueCol[0]].ColumnName + "=====>" + dtOutPut.Columns[0].ColumnName);
                ConstantMethod.ShowInfo(rtbResult, UserDt.Columns[valueCol[1]].ColumnName + "=====>" + dtOutPut.Columns[1].ColumnName);
                ConstantMethod.ShowInfo(rtbResult, UserDt.Columns[valueCol[2]].ColumnName + "=====>" + dtOutPut.Columns[3].ColumnName);

                //增加列  ConstantMethod.ShowInfo(rtbResult,"开始转换，转换规则如下");
                for (int i = 0; i < (valueCol.Count - 3); i++)
                {
                    DataColumn dtcolParm = new DataColumn("参数" + (i + 1).ToString());                   
                    dtOutPut.Columns.Add(dtcolParm);
                    ConstantMethod.ShowInfo(rtbResult, UserDt.Columns[valueCol[i+3]].ColumnName + "=====>" + dtOutPut.Columns[i+4].ColumnName);
                }
               
               
                
                //增加行
                foreach (DataRow row in UserDt.Rows)
                {
                    DataRow dr2 = dtOutPut.NewRow();

                    pBar1.Value = pBar1.Value + 1;

                    for (int i = 0; i < dr2.ItemArray.Length; i++)
                    {
                        if (i == 2)
                        {
                            dr2[i] = "0";
                        }
                        else
                        {
                            if (i < 2)
                            {
                                dr2[i] = row[valueCol[i]];
                            }
                            else
                            {
                                dr2[i] = row[valueCol[i - 1]];
                            }
                        }

                    }

                    dtOutPut.Rows.Add(dr2);
                }

            }

            string filename = DialogExcelDataLoad.FileName;

            string dir = Path.GetDirectoryName(filename);

            string filestr = Path.GetFileNameWithoutExtension(filename);

            filename = dir + "\\" + filestr + "Machine.csv";

            SaveFile(dtOutPut, filename);

            ConstantMethod.ShowInfo(rtbResult,DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss")+"转换结束！");

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
            int colid = e.ColumnIndex + 1;

            if (valueCol.Count==1)
            ConstantMethod.ShowInfo(rtbResult,"添加第"+ colid.ToString()+"列:"+ colname+ "====>尺寸！");

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
        }

        public void stopConfig()
        {
            

            button3.Enabled = true;
            IsSaveConfig = false;            

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
        private void button5_Click(object sender, EventArgs e)
        {          
           

        }

        private void SaveFile(DataTable dt,string filename)
        {
            csvop.SaveCSV(dt,filename);
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
                                      
                    UserDt = csvop.OpenCSV(DialogExcelDataLoad.FileName);
                   
                    if (UserDt != null && UserDt.Columns.Count > 3)
                        dgv.DataSource = UserDt;
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
                        dgv.DataSource = UserDt;
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
                        dgv.DataSource = UserDt;
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
    }
}
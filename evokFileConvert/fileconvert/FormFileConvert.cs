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
            Init();
        }

        void Init()
        {
            exop = new ExcelNpoi();
            csvop = new CsvStreamReader();
            UserDt = new DataTable();

            DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DialogExcelDataLoad.Filter = "文件(*.xls,*.xlsx,*.csv)|*.xls;*.csv;*.xlsx";
            DialogExcelDataLoad.FileName = "请选择数据文件";


            csvSaveDemo = new CsvStreamReader();
            if (!ReadFileDemo())
           {
                ConstantMethod.AppExit();
            }

        }

        private void button1_Click(object sender, EventArgs e)
        {         
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {

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
        private void button2_Click(object sender, EventArgs e)
        {

            
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {


                string localFilePath = DialogExcelDataLoad.FileName;

                string FileExt = Path.GetExtension(localFilePath).ToLower();

                if (FileExt.Equals(Constant.CSVFileEX))
                {
                    UserDt = null;

                    if (csvsplitCombox.Text.Equals(Constant.CsvSplitComma))
                    {
                        UserDt = csvop.OpenCSV(DialogExcelDataLoad.FileName);
                    }
                    else
                    {
                        if (csvsplitCombox.Text.Equals(Constant.CsvSplitSemiColon))
                        {
                            UserDt = csvop.OpenCSV0(DialogExcelDataLoad.FileName);
                        }
                    }

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
               
        private void button3_Click(object sender, EventArgs e)
        {
            DataTable dt = new DataTable("file");
            //收集数据 保存
            if (valueCol.Count > 3  && UserDt.Columns.Count >= valueCol.Count)
            {

                DataColumn dtcolSize = new DataColumn("尺寸");

                DataColumn dtcolCnt = new DataColumn("设定数量");

                DataColumn dtcolCntDone = new DataColumn("已切数量");

                DataColumn dtcolBarCode = new DataColumn("条码");

                dt.Columns.Add(dtcolSize);
                dt.Columns.Add(dtcolCnt);
                dt.Columns.Add(dtcolCntDone);
                dt.Columns.Add(dtcolBarCode);
                //增加列
                for (int i = 0; i < (valueCol.Count - 3); i++)
                {
                    DataColumn dtcolParm = new DataColumn("参数" + (i + 1).ToString());
                    dt.Columns.Add(dtcolParm);
                }
                //增加行
                foreach (DataRow row in UserDt.Rows)
                {
                    DataRow dr2 = dt.NewRow();

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

                    dt.Rows.Add(dr2);
                }

            }

            string filename = DialogExcelDataLoad.FileName;

            string dir = Path.GetDirectoryName(filename);

            string filestr = Path.GetFileNameWithoutExtension(filename);

            filename = dir + "\\" + filestr + "Machine.csv";

            SaveFile(dt, filename);


        }

        private void dgv_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (!IsSaveConfig) return;

            //可以重复选择
            if (valueCol.Contains(e.ColumnIndex))
            {

              // MessageBox.Show("请勿重复选取!");
              //  return;
            }

            valueCol.Add(e.ColumnIndex);

            int colid = e.ColumnIndex + 1;

            if (valueCol.Count==1)
            ConstantMethod.ShowInfo(rtbResult,"添加第"+ colid.ToString()+"列为尺寸！");

            if (valueCol.Count == 2)
                ConstantMethod.ShowInfo(rtbResult, "添加第" + colid.ToString() + "列为设定数量！");

            if (valueCol.Count == 3)
                ConstantMethod.ShowInfo(rtbResult, "添加第" + colid.ToString() + "列为条码！");

            if (valueCol.Count > 3)
            {
                ConstantMethod.ShowInfo(rtbResult, "添加第" + colid.ToString() + "列为参数:"+ (valueCol.Count-3).ToString());
            }
        }
        public void startConfig()
        {
            button1.Enabled = false;
            button2.Enabled = false;
            button3.Enabled = false;
            button4.Enabled = false;
            valueCol.Clear();
            rtbResult.Clear();
            IsSaveConfig = true;
        }

        public void stopConfig()
        {
            

            button1.Enabled = true;
            button2.Enabled = true;
            button3.Enabled = true;
            button4.Enabled = true;
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
        public bool ReadFileDemo()
        {
            DataTable dt = new DataTable();


            dt = csvSaveDemo.OpenCSV(Constant.SaveFileDemo);

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

            }
            else
            {
                MessageBox.Show("数据行错误！");
                return false;
            }

            return true;
           

        }
        private void button5_Click(object sender, EventArgs e)
        {          
            startConfig();

            ConstantMethod.ShowInfo(rtbResult,"双击任意一个单元指定每一列的数据内容，请按照尺寸，设定数量，条码，参数1..参数2..方式进行选取！");
            
            this.Focus();

            while (IsSaveConfig)
            {
                Application.DoEvents();              
            }
           
            stopConfig();

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
            stopConfig();
            SaveFileDemo();
        }
    }
}
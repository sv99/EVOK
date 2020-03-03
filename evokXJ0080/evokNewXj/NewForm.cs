using simiDataOpt;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;
using xjplc.simi;

namespace evokNewXJ
{
    public partial class NewForm : Form
    {
        EvokXJWork evokWork;
        ConfigFileManager userPara;
        RestMaterial restM;
        RestForm resF;
        OptParamSet optF;
      
        public NewForm()
        {
            InitializeComponent();
            Init();
        }

        void Init()
        {

            userPara = new ConfigFileManager(Constant.ConfigSimiUserDataFilePath);
            restM = new RestMaterial();           
            InitParam();
            InitSimiParam();

        }

        void InitTemp()
        {
            string[] str = new string[12];
            DataTable dt = ConstantMethod.getDataTableByString(str);
        }
        public void InitParam()
        {
            //datasource 改变会出发 selectindex 改变事件  这样就会打条码导致 模式被自动修改
            //所以早点设置好 然后在 那个selectindexchanged事件里增加 通讯正常判断
            // printcb.DataSource = Constant.printBarcodeModeStr;
           // printcb.Items.AddRange(Constant.printBarcodeModeStr);
            LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);
            // evokWork = new EvokXJWork(Constant.evokGetTcp);
            evokWork = ConstantMethod.GetWork();
            evokWork.DeviceName = Constant.simiDeivceName;
            evokWork.MainForm   = this;
            evokWork.SetUserDataGridView(UserData);
            //evokWork.getOptSize().DataShowCb = listBox1;
            // evokWork.SetRtbWork(rtbWork);
             evokWork.SetRtbResult(richTextBox1);
            evokWork.SetPrintReport(Constant.BarCode1);
            evokWork.ShowCutPictureBox = Main_CutShow_Pic;
            evokWork.ShowCurrentCutPictureBox = pictureBox2;
           //evokWork.InitDgvParam(dgvParam);
           //evokWork.InitDgvIO(dgvIO);
           //evokWork.SetOptParamShowCombox(comboBox2);
            evokWork.getOptSize().Simi_Split_Combox = comboBox1;
           //errorList = evokWork.ErrorList;
           //UpdateTimer.Enabled = true;
            restM.Dgv = restDgv;                  
            restM.updateDgv();

            evokWork.Rsm = restM;
        }

        public PictureBox getShowPic()
        {
            return Main_CutShow_Pic;
        }

        private void skinTreeView1_DoubleClick(object sender, EventArgs e)
        {
            MessageBox.Show("double click");
        }

        private void autoPage_Click(object sender, EventArgs e)
        {

        }

        private void skinButton8_Click(object sender, EventArgs e)
        {
            string path = "";
            path = textBox1.Text;

            FolderBrowserDialog op = new FolderBrowserDialog();

            if (!Directory.Exists(path))
            {
                if (op.ShowDialog() != DialogResult.OK)
                {
                    return;
                }
                else
                path = op.SelectedPath;
            }

                                 
                t1.Nodes.Clear();
                t1.Nodes.Add(
                ConstantMethod.
                getRootNode(path, Constant.ShowPathName));
            
            
        }

        private void skinButton33_Click(object sender, EventArgs e)
        {
            ConfigFileManager parafile = new ConfigFileManager(Constant.ConfigParamFilePath);

            FolderBrowserDialog op = new FolderBrowserDialog();
            string path = "";

            if (op.ShowDialog() == DialogResult.OK)
            {

                path = op.SelectedPath;
            }
            else
            return;

            textBox1.Text = path;

            if (Directory.Exists(textBox1.Text))
            {
                parafile.WriteConfig("simiDataDir", textBox1.Text);
                MessageBox.Show("写入成功！");
            }
            else MessageBox.Show("写入失败！");
        }


        //根据客户要求 在指定文件夹下进行 按照日期的文件搜索
        string[] GetFileList(string dirRoot)
        {
 
            List<string> fileStr = new List<string>();

            //当前日期转换为数数字
            if (!Directory.Exists(dirRoot)) return fileStr.ToArray();
                          
            string dataTime = DateTime.Now.ToString("yyymmdd");
            string[] fileLst=Directory.GetDirectories(dirRoot);
            int NowDateTimeStr =0;

            int.TryParse(dataTime, out NowDateTimeStr);

            foreach (string dir in fileLst)
            {
                int temp=0;
                string dirName = Path.GetFileName(dir);
                if (int.TryParse(dirName, out temp))
                {
                    fileStr.Add(dir);
                }
            }
          

            return fileStr.ToArray();
        }

        private void t1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {

            if (File.Exists(t1.SelectedNode.Tag.ToString()))
            {

                evokWork.SetUserDataGridView(UserData);

                evokWork.LoadSimiData(t1.SelectedNode.Tag.ToString());

                dtAuto = evokWork.getOptSize().DtData;

                UserData.DataSource = dtAuto;



            }
        }

        public EvokXJWork getWork()
        {
            
            return this.evokWork;
           
        }

        MSizeForm msf;
        private void UserConfirm_Material_Click(object sender, EventArgs e)
        {

            if (!evokWork.DeviceStatus)
            {
                MessageBox.Show("设备离线！");
                return;
            }
            if (msf == null)
            {
                msf = new MSizeForm();
                if (getWork() != null)
                {
                    msf.evokWork = getWork();
                    msf.ShowDialog();
                }
            }
        }

        private void skinButton18_Click(object sender, EventArgs e)
        {

        }

        private void UserConfirm_RestMaterial_Click(object sender, EventArgs e)
        {
            if (resF == null)
            {
                resF = new RestForm();
                restM.Dgv = resF.getDgv();
            }

            restM.updateDgv();
            resF.ShowDialog();
        }

        private void skinButton17_Click(object sender, EventArgs e)
        {

        }
        DataTable dtAuto;
        DataTable dtManual;
        DataTable dtBl;

        
        public void UserOpt(int id)
        {
            if (evokWork.IsMaterialExist())
            {
                switch (id)
                {
                    case 1:
                        {
                            evokWork.getOptSize().DtData = dtAuto;
                            break;
                        }
                    case 2:
                        {
                            evokWork.getOptSize().DtData = dtManual;
                            break;
                        }
                    case 3:
                        {
                            string upsize = "0";
                            string downsize = "0";
                            string usersize = "0";
                            string oppositeSize = "0";
                            string maxSize = "0";

                            foreach (DataRow dr in dtBl.Rows)
                            {


                                usersize = dr[Constant.strformatSimiBl[7]].ToString();

                                dr[Constant.strformatSimiBl[1]] = "1";
                                dr[Constant.strformatSimiBl[2]] = "0";
                                dr[Constant.strformatSimiBl[3]] = "补料";

                                double sized =
                                evokWork.getOptSize().
                                SimiM.calculateSize(
                                usersize.ToString(),                                
                                dr[Constant.strformatSimiBl[4]].ToString(),
                                dr[Constant.strformatSimiBl[5]].ToString(),
                                ref oppositeSize,
                                ref maxSize);

                                dr[Constant.strformatSimiBl[0]] = maxSize;//排版需要大尺寸
                                dr[Constant.strformatSimiBl[8]] = sized.ToString("0.00");
                                dr[Constant.strformatSimiBl[9]] = oppositeSize;
                                dr[Constant.strformatSimiBl[19]] = evokWork.getOptSize().SimiM.Width.ToString();

                            }                           

                            evokWork.getOptSize().DtData = dtBl;
                            break;
                        }
                }
                

                if(evokWork.getOptSize().MaterialId <= Constant.patternMaterialId)
                {

                    evokWork.optReadySimi(Constant.optNormal,id);

                    UpdateNowOpt(0);

                    //evokWork.SetUserDataGridView(UserData);

                    MessageBox.Show("优化完成！");
                }
                else
                    MessageBox.Show("花纹材料请直接启动程序！");

            }
            else MessageBox.Show("材料不存在，请设置材料库文件！");

        }
        void UpdateNowOpt(int id)
        {
            evokWork.Simi_Show(id);
            CountId = id;
            label4.Text = (CountId + 1).ToString()+"/"+ evokWork.getOptSize().ProdInfoLst.Count.ToString();
        }
        bool BlReason(DataTable dtbltemp)
        {
            if (dtbltemp != null && dtbltemp.Rows.Count > 0)
            {
                foreach (DataRow dr in dtbltemp.Rows)
                {
                    if (string.IsNullOrWhiteSpace(dr[3].ToString()))
                    {
                        MessageBox.Show("补料原因为空！");
                        return false;
                    }
                }
            }

            return true;
        }
        private void UserOpt_Click(object sender, EventArgs e)
        {
            if (tc1.SelectedIndex == 3)
            {
                if (!BlReason(dtBl)) return;
            } 
                 
            UserOpt(tc1.SelectedIndex);        
        }

        private void UserOptShow_Click(object sender, EventArgs e)
        {
            if (evokWork.getOptSize().ProdInfoLst.Count <= 0)
            {
                MessageBox.Show("请先排版！");
                return;
            }
            DrawSizeForm drForm = new DrawSizeForm();
            drForm.showdata(evokWork.getOptSize());
            drForm.Show();
        }

        private void UserOptParam_Click(object sender, EventArgs e)
        {

            optF = new OptParamSet();

            optF.evokWork = this.evokWork;
            optF.ShowDialog();


        }

        private void skinButton29_Click(object sender, EventArgs e)
        {
           
        }
        OpenFileDialog DialogExcelDataLoad;
        int ReadSimiData()
        {

           if(DialogExcelDataLoad==null)
           DialogExcelDataLoad = new OpenFileDialog();

           if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
           {

                //if(evokWork.showFilePathLabel == null) evokWork.showFilePathLabel = label8;
               // evokWork.SetDataShowCb(listBox2);
                //evokWork.SetDataShowLbl(label14);
                evokWork.SetUserDataGridView(manualDgv);
                ConstantMethod.SaveDirectoryByFileDialog(DialogExcelDataLoad);
                evokWork.LoadSimiData(DialogExcelDataLoad.FileName);

            }
            return 0;
        }

        private void skinButton19_Click(object sender, EventArgs e)
        {
            ReadSimiData();
            dtManual = evokWork.getOptSize().DtData;
            manualDgv.DataSource = dtManual;

        }

        private void skinButton16_Click(object sender, EventArgs e)
        {

        }

        private void skinButton20_Click(object sender, EventArgs e)
        {

        }

        private void watchPage_Enter(object sender, EventArgs e)
        {

            evokWork.getOptSize().Simi_Split_Combox = comboBox4;

            if (dtBl == null)
            { 
               dtBl = ConstantMethod.getDataTableByString(Constant.strformatSimiBl);
            }
            blDgv.DataSource = dtBl;

            evokWork.getOptSize().DtData = dtBl;

        }

        private void skinButton31_Click(object sender, EventArgs e)
        {

        }

        private void skinButton25_Click(object sender, EventArgs e)
        {
            evokNewXJ.userInputForm ud = new evokNewXJ.userInputForm();
            evokWork.getOptSize().DtData = dtBl;
            ud.Wk = evokWork;
            ud.ShowDialog();
        }

        private void skinButton23_Click(object sender, EventArgs e)
        {

        }

        private void skinButton22_Click(object sender, EventArgs e)
        {
            restM.DeleteMaterial((restDgv.CurrentCell.RowIndex));
        }

        private void skinButton26_Click(object sender, EventArgs e)
        {
            restM.DeleteAllMaterial();
        }

        private void skinButton32_Click(object sender, EventArgs e)
        {
            restM.updateDgv();
        }


        int CountId = 0;//用户当前

        
        private void skinButton5_Click(object sender, EventArgs e)
        {
            if (evokWork.getOptSize().ProdInfoLst.Count <= 0)
            {
                MessageBox.Show("无数据");
                return;
            }

            CountId++;
            if (CountId >= evokWork.getOptSize().ProdInfoLst.Count)
            {
                CountId = evokWork.getOptSize().ProdInfoLst.Count - 1;
                MessageBox.Show("已到最后一根");
            }
            UpdateNowOpt(CountId);          
        }

        private void skinButton4_Click(object sender, EventArgs e)
        {
            if (evokWork.getOptSize().ProdInfoLst.Count <= 0)
            {
                MessageBox.Show("无数据");
                return;
            }
                CountId--;
            if (CountId < 0)
            {
                CountId = 0;
                MessageBox.Show("已到第一根");
            }
            UpdateNowOpt(CountId);
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
           // if (comboBox1.SelectedItem != null)
          //  evokWork.getOptSize().Simi_SelectData(, int.Parse(comboBox1.SelectedItem.ToString()), false);

        }

        void ShiftComBox(int index,ComboBox cb)
        {

            ComboBox cbSource = evokWork.getOptSize().Simi_Split_Combox;

            if (index == 0)
            {
                if (cbSource != null && !cbSource.Equals(cb))
                {
                    string[] str=new string[cbSource.Items.Count];
                    evokWork.getOptSize().Simi_Split_Combox.Items.CopyTo(str, 0);
                    cb.Items.Clear();
                    cb.Items.AddRange(str);// evokWork.getOptSize().Simi_Split_Combox.DataSource;
                    if (cbSource.SelectedIndex >= 0)
                    {
                        cb.SelectedIndex = cbSource.SelectedIndex;
                    }
                }
            }



        }
        private void autoPage_Enter(object sender, EventArgs e)
        {
           
            evokWork.getOptSize().Simi_Split_Combox = comboBox1;
            evokWork.getOptSize().
            Simi_SelectData(evokWork.getOptSize().DtData.TableName, 0, true);

        }
        void ShiftDtData()
        {

        }
        private void User_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {  

            if (!((ComboBox)sender).Visible) return;    
            if (((ComboBox)sender).SelectedItem != null && !string.IsNullOrWhiteSpace(((ComboBox)sender).SelectedItem.ToString()))
            evokWork.getOptSize().Simi_SelectData(
                evokWork.getOptSize().DtData.TableName, 
                int.Parse(((ComboBox)sender).SelectedItem.ToString()), 
                false);

            if (evokWork.IsMaterialExist())
            {
                switch (tc1.SelectedIndex)
                {
                    case 1:
                        {
                            dtAuto = evokWork.getOptSize().DtData; ;
                            break;
                        }
                    case 2:
                        {
                            dtManual = evokWork.getOptSize().DtData;
                            break;
                        }

                }
            }
            if (tc1.SelectedIndex == 0 )
            {
                UserOpt(0);
                UpdataXuhao();
            }
        }

        void UpdataXuhao()
        {
            Main_CutId_ComBx.Items.Clear();

            for (int i = 0; i < evokWork.getOptSize().ProdInfoLst.Count; i++)
            {
                Main_CutId_ComBx.Items.Add((i+1).ToString());
            }
        }
        private void mainPage_Enter(object sender, EventArgs e)
        {
            //切换数据显示区域
            ShiftComBox(tc1.SelectedIndex,Main_CutDatatable_ComBx);

            //设置第一个datatable 第一个数据源
            evokWork.getOptSize().
            Simi_SelectData(evokWork.getOptSize().DtData.TableName, 0, false);

            //更新combox序号
            UpdataXuhao();

        }

        private void handPage_Enter(object sender, EventArgs e)
        {
                      
            evokWork.getOptSize().Simi_Split_Combox = comboBox3;

            evokWork.getOptSize().
            Simi_SelectData(evokWork.getOptSize().DtData.TableName, 0, true);
        }

        private void skinButton6_Click(object sender, EventArgs e)
        {
            if (evokWork.getOptSize().ProdInfoLst.Count <= 0)
            {
                MessageBox.Show("无数据");
                return;
            }
            evokWork.ShowBarCode(0);
        }

        void OffLine_Start()
        {
            if (evokWork.IsOffLineMode)
            {
                tmrOffLine.Enabled = true;
                startLbl.BackColor = Color.Green;
                daijiLabel.BackColor = Color.Gray;
                errorLabel.BackColor = Color.Gray;
            }
        }
        void OffLine_Stop()
        {
            if (evokWork.IsOffLineMode)
            {
                tmrOffLine.Enabled = false;
                startLbl.BackColor = Color.Gray;
            }
        }
        
        private void startBtn_Click(object sender, EventArgs e)
        {
            if (evokWork.getOptSize().ProdInfoLst.Count <= 0)
            {
                MessageBox.Show("无数据");
                return;
            }

            evokWork.SetCutProCnt(0);
            if (evokWork.IsOffLineMode)
            {
                OffLine_Start();                                           
            }
            if (evokWork.SimimaterialId < Constant.patternMaterialId)
            {
                evokWork.StartWithOutDevice(0);
            }
            else
            {
                evokWork.StartWithOutDeviceWithPattern(5);
            }

        }

        private void skinLabel1_Click(object sender, EventArgs e)
        {
           
        }

        private void skinButton1_Click(object sender, EventArgs e)
        {

        }

        private void skinButton3_Click(object sender, EventArgs e)
        {
            if (evokWork.IsOffLineMode)
            {
                tmrOffLine.Enabled = false;
                startLbl.BackColor = Color.Gray;
                daijiLabel.BackColor = Color.Green;
                errorLabel.BackColor = Color.Gray;
            }
        }

        private void skinButton29_Click_1(object sender, EventArgs e)
        {
            evokWork.Simi_Show(0, 1);
        }

        private void SingleStart_Btn_Click(object sender, EventArgs e)
        {
            if (evokWork.IsOffLineMode)
            {
                OffLine_Start();
            }
            if (evokWork.SimimaterialId < Constant.patternMaterialId)
            {
                evokWork.StartWithOutDevice(101);
            }
            else
            {
                evokWork.StartWithOutDeviceWithPattern(5);
            }
        }

        private void Main_CutId_ComBx_SelectedIndexChanged(object sender, EventArgs e)
        {
            int id = 0;
            if(int.TryParse(Main_CutId_ComBx.SelectedItem.ToString(),out id))
            evokWork.Simi_Show(id-1);
        }

        private void skinButton7_Click(object sender, EventArgs e)
        {
            if (evokWork.getOptSize().ProdInfoLst.Count <= 0)
            {
                MessageBox.Show("无数据");
                return;
            }

            if (evokWork.IsOffLineMode)
            {
                OffLine_Start();
            }
            if (evokWork.SimimaterialId < Constant.patternMaterialId)
            {
                evokWork.SetCutProCnt(CountId+1);
                evokWork.StartWithOutDevice(101);
            }
            else
            {
                evokWork.StartWithOutDeviceWithPattern(5);
            }
        }

        private void tc1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if ((tc1.SelectedIndex == 4 || tc1.SelectedIndex == 3 )&& !ConstantMethod.UserPassWd(Constant.PwdNoOffSet))
            {
                e.Cancel = true;
            }
            if( tc1.SelectedIndex == 5  && !evokWork.DeviceStatus)           
            {
               MessageBox.Show("设备离线！");
               e.Cancel = true;
            }
        }

        private void UserData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {

        }

        private void UserData_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            evokWork.PrintUnCuttable(e.RowIndex);
        }

        private void NewForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show(Constant.formCloseTips, Constant.formCloseTitle, MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
            if (dr == DialogResult.No)
            {
                e.Cancel = true;//就不退了
                return;
            }
            else
            {
                e.Cancel = false;//退了
            }
        }

        private void skinButton30_Click(object sender, EventArgs e)
        {
            
            SqlConnection
            lo_conn = new SqlConnection("Server=" + textBox2 + ";Database=" + textBox4 + ";uid=" + textBox3 + ";pwd=" + textBox5);
          
            try
            {
                lo_conn.Open();
                
                DataTable dt = lo_conn.GetSchema("Tables");

                ConfigFileManager parafile = new ConfigFileManager(Constant.ConfigParamFilePath);
                parafile.WriteConfig("SQL_ServerName", textBox2.Text);
                parafile.WriteConfig("SQL_DatabaseName", textBox4.Text);
                parafile.WriteConfig("SQL_UserName", textBox3.Text);
                parafile.WriteConfig("SQL_Passwd", textBox5.Text);
                parafile.WriteConfig("SQL_Tablename", textBox12.Text);

                MessageBox.Show("设置成功！");
            }
            catch (Exception ex)
            {
                 MessageBox.Show("设置成功失败！");
                MessageBox.Show("数据库登录失败，错误：" + ex.Message);
            }
            
            
        }
        void InitSimiParam()
        {
            ConfigFileManager parafile = new ConfigFileManager(Constant.ConfigParamFilePath);

            textBox1.Text=  parafile.ReadConfig("simiDataDir");
            textBox2.Text=parafile.ReadConfig("SQL_ServerName");

            textBox4.Text = parafile.ReadConfig("SQL_DatabaseName");
            textBox3.Text = parafile.ReadConfig("SQL_UserName");
            textBox5.Text = parafile.ReadConfig("SQL_Passwd");
            textBox12.Text = parafile.ReadConfig("SQL_Tablename");

            textBox9.Text = parafile.ReadConfig("WlNear0");
            textBox6.Text = parafile.ReadConfig("WlNear1");
            textBox7.Text = parafile.ReadConfig("WlNear2");
            textBox8.Text = parafile.ReadConfig("WlNear3");
            textBox10.Text = parafile.ReadConfig("WlNear4");
            textBox13.Text = parafile.ReadConfig("WlNear5");
            

        }
        private void skinButton31_Click_1(object sender, EventArgs e)
        {
            int id = 0;
            if (!int.TryParse(textBox6.Text, out id)
                || !int.TryParse(textBox6.Text, out id)
                || !int.TryParse(textBox8.Text, out id)
                || !int.TryParse(textBox9.Text, out id)
                || !int.TryParse(textBox10.Text, out id)
               
                )
            {
                MessageBox.Show("数据错误！");

            }

            ConfigFileManager parafile = new ConfigFileManager(Constant.ConfigParamFilePath);

            parafile.WriteConfig("WlNear0", textBox9.Text);
            parafile.WriteConfig("WlNear1", textBox6.Text);
            parafile.WriteConfig("WlNear2", textBox7.Text);
            parafile.WriteConfig("WlNear3", textBox8.Text);
            parafile.WriteConfig("WlNear4", textBox10.Text);
            parafile.WriteConfig("WlNear5", textBox13.Text);
            evokWork?.ReadSimiWlst();
            MessageBox.Show("设置成功！");
        }

        private void skinButton28_Click(object sender, EventArgs e)
        {

        }
    }
}

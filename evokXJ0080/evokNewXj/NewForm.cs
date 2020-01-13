using simiDataOpt;
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
            evokWork.ShowCutPictureBox = pictureBox1;
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
            return pictureBox1;
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

            FolderBrowserDialog op = new FolderBrowserDialog();
            if (op.ShowDialog() == DialogResult.OK)
            {
                t1.Nodes.Add(
                ConstantMethod.
                getRootNode(op.SelectedPath,Constant.ShowPathName));

                /**
                string[] nodes = GetFileList(op.SelectedPath);
                if (nodes.Count() > 0)
                {
                    foreach (string s in nodes)
                    {
                        
                        TreeNode tn = new TreeNode(Path.GetFileName(s));

                        string[] nodes0 = GetFileList(s);

                        tn.Tag = s;

                        foreach (string s0 in nodes)
                        {
                            tn.Nodes.Add(Path.GetFileName(s0));
                            TreeNode tn1 = new TreeNode(Path.GetFileName(s));
                            foreach (string s1 in nodes)
                            {

                            }

                        t1.Nodes.Add(tn);


                    }                                     
                }
                ***/
            }
            
        }

        private void skinButton33_Click(object sender, EventArgs e)
        {
            
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
            if (msf == null)
            {
                msf = new MSizeForm();
                if (getWork() != null)
                {
                    msf.evokWork = getWork();
                    msf.Show();
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
            resF.Show();
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

                    evokWork.optReady(Constant.optNormal);

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
            DrawSizeForm drForm = new DrawSizeForm();
            drForm.showdata(evokWork.getOptSize());
            drForm.Show();
        }

        private void UserOptParam_Click(object sender, EventArgs e)
        {

           // if (optF == null)
           // {
                optF = new OptParamSet();              
           // }

            optF.Show();


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

                //if (evokWork.showFilePathLabel == null) evokWork.showFilePathLabel = label8;
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

        private void User_ComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {     
                 
            if (((ComboBox)sender).SelectedItem != null && !string.IsNullOrWhiteSpace(((ComboBox)sender).SelectedItem.ToString()))
            evokWork.getOptSize().Simi_SelectData(
                evokWork.getOptSize().DtData.TableName, 
                int.Parse(((ComboBox)sender).SelectedItem.ToString()), 
                false);
            if (tc1.SelectedIndex == 0 )
            {
                UserOpt(0);
            }
        }

        private void mainPage_Enter(object sender, EventArgs e)
        {

            ShiftComBox(tc1.SelectedIndex,comboBox5);
            evokWork.getOptSize().
            Simi_SelectData(evokWork.getOptSize().DtData.TableName, 0, true);
            
        }

        private void handPage_Enter(object sender, EventArgs e)
        {
            evokWork.getOptSize().Simi_Split_Combox = comboBox3;
            evokWork.getOptSize().
            Simi_SelectData(evokWork.getOptSize().DtData.TableName, 0, true);
        }

        private void skinButton6_Click(object sender, EventArgs e)
        {           
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

            if (evokWork.IsOffLineMode)
            {
                OffLine_Start();                                           
            }
            if (evokWork.SimimaterialId < Constant.patternMaterialId)
            {
                evokWork.StartWithOutDevice();
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
            evokWork.
            Simi_Show(0, 1);
        }
    }
}

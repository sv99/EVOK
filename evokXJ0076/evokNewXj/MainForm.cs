using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Threading;
using System.Windows.Forms;
using xjplc;

namespace evokNew0076
{
    public partial class MainForm : Form
    {
        private static Queue<Control> allCtrls = new Queue<Control>();

        CsvStreamReader csvop;
        bool propertyA = true;
        bool propertyB = false;
        bool propertyC = false;
        OptSize opXiaLiao; 
        OptSize opDoorShell;
        OptSize opDoorBan;
        private EvokXJWork evokWork0;
        private EvokXJWork evokWork1;
        private EvokXJWork evokWork2;


        List<EvokXJWork> evokWorkLst = new List<EvokXJWork>();
        workManager workMan;
        doorTypeInfo doorLst;

        ConfigFileManager paraFile;
      
        public MainForm()
        {
            InitializeComponent();
     
        }
  
        public void Init()
        {
            PortParam p0 = new PortParam();
            PortParam p1 = new PortParam();
            PortParam p2 = new PortParam();
            p0 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);
            p1 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath1);
            p2 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath2);

            //初始化设备
            List<string> strDataFormPath0 = new List<string>();

            strDataFormPath0.Add(Constant.PlcDataFilePathAuto);
            strDataFormPath0.Add(Constant.PlcDataFilePathHand);
            strDataFormPath0.Add(Constant.PlcDataFilePathParam);
            strDataFormPath0.Add(Constant.PlcDataFilePathIO);

            List<string> strDataFormPath1 = new List<string>();

            strDataFormPath1.Add(Constant.PlcDataFilePathAuto1);
            strDataFormPath1.Add(Constant.PlcDataFilePathHand1);
            strDataFormPath1.Add(Constant.PlcDataFilePathParam1);
            strDataFormPath1.Add(Constant.PlcDataFilePathIO1);

            List<string> strDataFormPath2 = new List<string>();
            strDataFormPath2.Add(Constant.PlcDataFilePathAuto2);
            strDataFormPath2.Add(Constant.PlcDataFilePathHand2);
            strDataFormPath2.Add(Constant.PlcDataFilePathParam2);
            strDataFormPath2.Add(Constant.PlcDataFilePathIO2);
         
            csvop = new CsvStreamReader();
            opXiaLiao = new OptSize();
            opDoorShell = new OptSize();          
            opDoorBan = new OptSize();
            if (propertyA)
            {
                evokWork0 = new EvokXJWork(strDataFormPath0, p0);             
            }

            if (propertyB) evokWork1 = new EvokXJWork(strDataFormPath1, p1);
        
            if (propertyC) evokWork2 = new EvokXJWork(strDataFormPath2, p2);
         
            InitWork();

            UpdateTimer.Enabled = true;
          
            
             doorLst = new doorTypeInfo();

            workMan = new workManager();

            LoadData(dgSize,Constant.DoorSizeFile);
            LoadData(dgDoorBan, Constant.DoorBanFile);
            LoadData(dgDoorShell, Constant.DoorShellFile);

            startDevice = startClick;

            rtbResult.Text = "门芯板信息";
            richTextBox7.Text = "下料锯信息";
            richTextBox8.Text = "门皮信息";
            
        }
        int getNOByDoorId(int id,OptSize op)
        {

            int idResult = -1;

            if(op.ProdInfoLst.Count>0)
            foreach (ProdInfo p in op.ProdInfoLst)
            {
                if (p.Param10[0].Equals(id.ToString()))
                {
                    if(int.TryParse(p.Param2[0],out idResult))
                    break;
                }
            }

            return idResult;

        }
        void upDateButton()
        {
            //设备状态和控件关联
            if (evokWork0.isDownLoading(Constant.doorSizeId)
                || evokWork0.isDownLoading(Constant.doorBanId)
                || evokWork0.isDownLoading(Constant.doorShellId))
            {
                if (stbtn.Enabled)
                {
                    stbtn.Enabled = false;
                    loadDataBtn.Enabled = false;
                    button1.Enabled = false;
                    
                }
                timerDoorSize.Enabled = true;
            }
           

            if (evokWork0.isDownLoading(Constant.doorSizeId))
            {
                if (button5.Enabled)
                {
                    button5.Enabled = false;
                    button8.Enabled = false;
                    button21.Enabled = false;
                    stbtn.Enabled = false;
                    label8.BackColor = Color.Green;             
                }
            }
            else
            {
                if (!button5.Enabled)
                {
                    button5.Enabled = true;
                    button8.Enabled = true;
                    button21.Enabled = true;
                    label8.BackColor = Color.Gray;           
                }
            }

            if (evokWork0.isDownLoading(Constant.doorBanId))
            {
                if (button10.Enabled)
                {
                    button10.Enabled = false;
                    button23.Enabled = false;
                    button17.Enabled = false;
                    label11.BackColor = Color.Green;
                                        
                }
            }
            else
            {
                if (!button10.Enabled)
                {
                    button10.Enabled = true;
                    button23.Enabled = true;
                    button17.Enabled = true;
                    label11.BackColor = Color.Gray;
                    
                }
            }


            if (evokWork0.isDownLoading(Constant.doorShellId))
            {
                if (button9.Enabled)
                {
                    button9.Enabled = false;
                    button22.Enabled = false;
                    button20.Enabled = false;
                    label9.BackColor = Color.Green;
                   
                }
            }
            else
            {
              
                    button9.Enabled = true;
                    button22.Enabled = true;
                    button20.Enabled = true;
                    label9.BackColor = Color.Gray;
                    
                
            }
        }
        private void UpdataEvokWork(EvokXJWork work, ToolStripStatusLabel s)
        {

           
                      
            foreach (PlcInfoSimple simple in evokWork0.PsLstAuto)
            {
                int showValue = simple.ShowValue;
            }

        }
        public void SetControlInEvokWork()
        {
            ConstantMethod.
           CheckAllCtrls(this, allCtrls);
            foreach (Control control in allCtrls)
            {
                if (control.Tag != null)
                {
                    if ((control.Parent == tabPage2) || (control.Parent.Parent == tabPage2)||
                        (control.Parent == tabPage1) || (control.Parent.Parent == tabPage1)||
                        (control.Parent == tabPage3) || (control.Parent.Parent == tabPage3)
                        )
                    {
                        foreach (PlcInfoSimple simple in evokWork0.PsLstAuto)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple, control)) break;                          
                        }
                        continue;
                    }
                }
            }
        }

        public void InitWork()
        {

            SetControlInEvokWork();
            if (propertyA)
            {
               // evokWork0.InitDgvParam(dgvParam);
                //evokWork0.InitDgvIO(dgvIO);
                evokWork0.DeviceProperty = Constant.devicePropertyA;
               // evokWork0.SetRtbResult(richTextBox2);
               // evokWork0.SetRtbWork(richTextBox1);
                evokWork0.SetLblStatus(label8);
                evokWork0.SetOptSize(opXiaLiao);
                evokWork0.SetPrintReport();
                errorList = evokWork0.ErrorList;
            }
            if (propertyB)
            {
                //evokWork1.InitDgvParam(dgvParam1);
               // evokWork1.InitDgvIO(dgvIO1);
                evokWork1.DeviceProperty = Constant.devicePropertyB;
               // evokWork1.SetRtbResult(richTextBox3);
               // evokWork1.SetRtbWork(richTextBox4);
                evokWork1.SetLblStatus(label9);
                evokWork1.SetOptSize(opDoorShell);
            }
             if (propertyC)
            { 
                //evokWork2.InitDgvParam(dgvParam2);
                //evokWork2.InitDgvIO(dgvIO2);
                evokWork2.DeviceProperty = Constant.devicePropertyC;
               // evokWork2.SetRtbResult(richTextBox5);
               // evokWork2.SetRtbWork(richTextBox6);
                evokWork2.SetLblStatus(label11);
                evokWork2.SetOptSize(opDoorBan);
            }
            //读取设备名
            paraFile = new ConfigFileManager();
            paraFile.LoadFile(Constant.ConfigParamFilePath);
            if (propertyA)
            {
                evokWork0.DeviceName = paraFile.ReadConfig("work0");
                evokWorkLst.Add(evokWork0);
            }

            if (propertyB)
            {
                evokWork1.DeviceName = paraFile.ReadConfig("work1");
                evokWorkLst.Add(evokWork1);

            }
            if (propertyC)
            {
                evokWork2.DeviceName = paraFile.ReadConfig("work2");
                evokWorkLst.Add(evokWork2);
            }

                 

          

        }

       
      
        private void UpdataError(EvokXJWork evokWork,ToolStripStatusLabel statusLabel)
        {
            if (evokWork.DeviceStatus)
            {
                statusLabel.Text = Constant.MachineWorking;
                statusLabel.BackColor = Color.Green;
            }
            else
            {
                statusLabel.Text = Constant.ConnectMachineFail;
                statusLabel.BackColor = Color.Red;
                return;
            }

            foreach (PlcInfoSimple p in evokWork.PsLstAuto)
            {
                if (p.Name.Contains(Constant.Alarm) && p.ShowStr != null && p.ShowStr.Count > 0)
                {
                    for (int i = 0; i < p.ShowStr.Count; i++)
                    {
                        int index = evokWork.ErrorList.IndexOf(p.ShowStr[i]);
                        if (p.ShowValue == Constant.M_ON && index < 0)
                        {
                            evokWork.ErrorList.Add(p.ShowStr[i]);
                        }
                        if (p.ShowValue == Constant.M_OFF && index > -1 && index < evokWork.ErrorList.Count)
                        {
                            evokWork.ErrorList.RemoveAt(index);
                        }
                    }
                }
            }

            if (errorList.Count > 0)
            {
                if (errorId >= errorList.Count)
                {
                    errorId = 0;
                }

                infoLbl.Text = errorList[errorId];

                errorId++;

            }
            else infoLbl.Text = "";


        }
       
     
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            if (propertyA)
                UpdataEvokWork(evokWork0, statusLabel1);

            if (propertyB)
            {
                UpdataEvokWork(evokWork1, statusLabel2);
            }
            if (propertyC)
            {
                UpdataEvokWork(evokWork2, statusLabel3);
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("关闭软件前，请关闭各个设备，是否继续关闭程序？", "关闭提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
            
            if (dr == DialogResult.No)
            {
                e.Cancel = true;//就不退了
                return;
            }
            else
            {
                foreach (EvokXJWork work in evokWorkLst)
                {
                    Thread t1 = new Thread(new ThreadStart(delegate
                    {
                       // work.stop();
                        work.LineStop();

                    }));
                    t1.Start();
                }

                e.Cancel = false;//退了
            }
            UpdateTimer.Enabled = false;
            timerDoorSize.Enabled = false;
      

            //FileSaveTimer.Enabled = false;
            if (evokWork0 != null)
            {
                evokWork0.Dispose();
            }
            if (evokWork1 != null)
            {
                evokWork1.Dispose();
            }
            if (evokWork2 != null)
            {
                evokWork2.Dispose();
            }

            ConstantMethod.Delay(1000);
        }
             
        //每个标签页都按照这个来  
        private void tabControl3_Selecting(object sender, TabControlCancelEventArgs e)
        {
            int id = tabControl1.SelectedIndex - 1;
            int id1 = ((TabControl)sender).SelectedIndex;
            //这里要加1 因为页面少了一个 和序号对不上了
            if (id1 > 0) id1++;
            {

                if (!evokWorkLst[id].ShiftPage(id1))
                {
                    e.Cancel = true;
                }
            }       
        }

        private void loadDataBtn_Click(object sender, EventArgs e)
        {
            DialogResult dr3 = MessageBox.Show("是否继续加载，请核对数据？", "加载提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

            if (dr3 == DialogResult.No)
            {
                return;
            }
            comboBox1.DataSource = null;
            dgSize.DataSource = null;
            dgDoorBan.DataSource = null;
            dgDoorShell.DataSource = null;
            checkBox2.Checked = false; 

            if (workMan.LoadData())
            {
                LogManager.WriteProgramLog("数据拆分结束");
                workMan.ShowResult(listView1);  
                             
                workMan.showDoorTypeList(comboBox1, 0);

                checkBox2.Checked = true;

                richTextBox7.Text = "当前木方数据数量行数为：" + ((DataTable)dgSize.DataSource).Rows.Count.ToString();

                richTextBox8.Text = "当前门皮数据数量行数为：" + ((DataTable)dgDoorShell.DataSource).Rows.Count.ToString();

                rtbResult.   Text = "当前门板数据数量行数为：" + ((DataTable)dgDoorBan.DataSource).Rows.Count.ToString();

                dataCheck(dgDoorBan, 1, 
                    evokWork0.doorBanLen.ShowValue, evokWork0.doorBanWidth.ShowValue, "门板");
                dataCheck(dgDoorShell, 1, 
                    evokWork0.doorShellLen.ShowValue, evokWork0.doorShellWidth.ShowValue, "门皮");

            }


        }

        bool dataCheck(DataGridView dgv ,int id,int height,int width,string tableName)
        {
            bool errorExist = false;
            if (id == 0)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    double  sizeheight = 0;
                    if (dgv.Rows[i].Cells[0].Value == null) continue;
                    if (!double.TryParse(dgv.Rows[i].Cells[0].Value.ToString(),out sizeheight))
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        errorExist = true;
                        continue;
                    }
                    if (sizeheight * Constant.dataMultiple > height)
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        errorExist = true;
                        continue;
                    }

                }

            }
            if (id == 1)
            {
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    double sizeheight0 = 0;
                    double sizewidth0= 0;

                   if(dgv.Rows[i].Cells[0].Value==null) continue;

                    if (!double.TryParse(dgv.Rows[i].Cells[0].Value.ToString(), out sizeheight0))
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        errorExist = true;
                        continue;
                    }

                    if (sizeheight0*Constant.dataMultiple > height)
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        errorExist = true;
                        continue;
                    }

                    if (!double.TryParse(dgv.Rows[i].Cells[4].Value.ToString(), out sizewidth0))
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        errorExist = true;
                        continue;
                    }

                    if (sizewidth0 * Constant.dataMultiple > width)
                    {
                        dgv.Rows[i].DefaultCellStyle.BackColor = System.Drawing.Color.Red;
                        errorExist = true;
                        continue;
                    }                   
                }
            }

            if (errorExist)
            {
                MessageBox.Show(tableName+"数据错误，请检验后再次导入");
            }

            return errorExist;
        }
        //每个单机切换回来 都切换显示
        void shift()
        {
            if (propertyA)
            {
                //evokWork0.SetRtbWork(richTextBox7);
                evokWork0.shiftToLine();
            }

            if (propertyB)
            {
                evokWork1.SetRtbWork(richTextBox8);
                evokWork1.shiftToLine();
            }
            if (propertyC)
            {
                evokWork2.SetRtbWork(rtbResult);
                evokWork2.shiftToLine();
            }
         
        }
        void startLine()
        {
            opXiaLiao.DtData = (DataTable)dgSize.DataSource;
            opDoorShell.DtData = (DataTable)dgDoorShell.DataSource;
            opDoorBan.DtData = (DataTable)dgDoorBan.DataSource;

            
           StartWork(0);
           StartWork(1);
           StartWork(2);
            
        }
                                     
        private void stbtn_Click(object sender, EventArgs e)
        {
            if (dgDoorBan.DataSource == null || dgDoorBan.Rows.Count < 1)
            {

                MessageBox.Show("数据为空，请重新加载！");
                return;
            }

            DialogResult dr = MessageBox.Show("各个设备是否就绪？请检查废料和工位", "关闭提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

            if (dr == DialogResult.No)
            {

                return;
            }
            dr = MessageBox.Show("各个设备料长是否已切换为联机模式？", "关闭提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

            if (dr == DialogResult.No)
            {
                
                return;
            }

            dr = MessageBox.Show("各个设备料长是否设置？", "关闭提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示


            if (dr == DialogResult.No)
            {
                return;
            }

           
            foreach (EvokXJWork work in evokWorkLst)
            {
                if (work.IsInNoSafe)
                {
                    MessageBox.Show(work.DeviceName + "急停中！线启动失败！");
                    return;
                }
                if (work.DeviceMode == Constant.M_ON)
                {              
                    MessageBox.Show(work.DeviceName + "设备模式错误！线启动失败！");
                    return;
                }
                if (
                    work.deviceStatusId == Constant.constantStatusId[1] ||
                    work.deviceStatusId == Constant.constantStatusId[2] ||
                    work.deviceStatusId == Constant.constantStatusId[3] ||
                    work.deviceStatusId == Constant.constantStatusId[4] ||
                    work.deviceStatusId == Constant.constantStatusId[5] 
                   
                    )
                {
                    work.showWorkInfo();
                }

            }

            startLine();
        }

      
        #region  设备启动
        public void StartWork(int id)
        {
            
            Thread cutdoorStartThread;

            cutdoorStartThread = new Thread(new ParameterizedThreadStart(startCut));

            cutdoorStartThread.Start(id);
                    

        }

        private void startCut(object obj)
        {
            int id = (int)obj;

             //20181118增加状态 判断
            if( evokWorkLst[id].deviceStatusId == Constant.constantStatusId[1]
                || evokWorkLst[id].deviceStatusId == Constant.constantStatusId[2]
                || evokWorkLst[id].deviceStatusId == Constant.constantStatusId[3]
                || evokWorkLst[id].deviceStatusId == Constant.constantStatusId[4]
                || evokWorkLst[id].deviceStatusId == Constant.constantStatusId[5])
            {
                MessageBox.Show(evokWorkLst[id].DeviceName+Constant.constantStatusStr[evokWorkLst[id].deviceStatusId]);
                return;

            }
            //已运行
            if (evokWorkLst[id].RunFlag)
            {
                MessageBox.Show(Constant.alreadyStart);

            }
            switch (evokWorkLst[id].DeviceProperty)
            {
                case Constant.devicePropertyA: 
                    {                         
                        evokWorkLst[id].optReady(Constant.optNo);
                        evokWorkLst[id].CutStartNormal(Constant.CutNormalWithShuChiMode);
                        break;
                    }
                    //门皮
                case Constant.devicePropertyB:
                    {
                        evokWorkLst[id].optReady(Constant.optNo);
                        if (!evokWorkLst[id].DataJoin())
                        {
                            MessageBox.Show("数据排版错误！");
                        }

                        evokWorkLst[id].CutDoorStartNormal(Constant.CutNormalDoorShellMode);
                        break;
                    }
                //ban
                case Constant.devicePropertyC:
                    {

                        evokWorkLst[id].optReady(Constant.optNo);  
                                                            
                        evokWorkLst[id].CutDoorStartNormal(Constant.CutNormalDoorBanMode);

                        break;
                    }
                default:
                    {
                        evokWorkLst[id].optReady(Constant.optNormal);

                        evokWorkLst[id].CutDoorStartNormal(Constant.CutNormalMode);

                        break;
                    }
            }                    
        }
        private void button2_Click(object sender, EventArgs e)
        {
           
           opXiaLiao.DtData = (DataTable)dgSize.DataSource;
           StartWork(tabControl1.SelectedIndex-1);

        }
        #endregion
       
       
       
      
      
       

        private void MainForm_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            Init();
            Application.DoEvents();               
            this.Visible = true;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            evokWorkLst[tabControl1.SelectedIndex - 1].stop();  
             
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            evokWorkLst[tabControl1.SelectedIndex - 1].pause();
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            evokWorkLst[tabControl1.SelectedIndex - 1].reset(); 
        }
        List<string> errorList = new List<string>();
        int errorId = 0;
        void countDoorSizeUpdate()
        {
            if(evokWork0.isDownLoading(Constant.doorSizeId))
            if (currentDoorSizeId != evokWork0.xialiaoCurrentId)
            {
                evokWork0.currentIdUpdateBanAndShell(dgSize, currentDoorSizeId, Constant.doorSizeId,1, opXiaLiao);
                currentDoorSizeId = evokWork0.xialiaoCurrentId;
                xialiaoLblCuurentId.Text = getNOByDoorId(evokWork0.xialiaoCurrentDoorIdInPs.ShowValue, opXiaLiao).ToString();

                }
            //门号                         
            evokWork0.currentIdUpdate(dgSize, evokWork0.xialiaoCurrentId, Constant.doorSizeId, 199, opXiaLiao);

        }
        void countDoorBanUpdate()
        {
            if(evokWork0.isDownLoading(Constant.doorBanId))
            if (currentDoorBanId != evokWork0.doorbanCurrentId)
            {
                evokWork0.currentIdUpdateBanAndShell(dgDoorBan, currentDoorBanId, Constant.doorBanId, 1, opDoorBan);
             
                currentDoorBanId = evokWork0.doorbanCurrentId;
                doorBanLblCuurentId.Text =
               getNOByDoorId(evokWork0.doorbanCurrentDoorIdInPs.ShowValue, opDoorBan).ToString();

            }
        }
        void countDoorShellUpdate()
        {
            if (evokWork0.isDownLoading(Constant.doorShellId))
            if (currentDoorShellId != evokWork0.doorshellCurrentId)
            {
                evokWork0.currentIdUpdateBanAndShell(dgDoorShell, currentDoorShellId, Constant.doorShellId, 1, opDoorShell);
                currentDoorShellId = evokWork0.doorshellCurrentId;
                doorShellLblCuurentId.Text =
                getNOByDoorId(evokWork0.doorshellCurrentDoorIdInPs.ShowValue, opDoorShell).ToString();

                }
        }
                  
        private void errorTimer_Tick(object sender, EventArgs e)
        {
            if (evokWork0 != null)
            {
                errorTimer.Enabled = false;
                //保存数据
                saveData();
                updateInfo(Constant.doorSizeId);             
                updateInfo(Constant.doorShellId);
                updateInfo(Constant.doorBanId);
                upDateButton();
                UpdataError(evokWork0, statusLabel1);
                errorTimer.Enabled = true ;

            }
        }
        int currentDoorShellId;
        int currentDoorSizeId;
        int currentDoorBanId;
        void updateInfo(int id)
        {
                                   
            switch (id)
            {
                case Constant.doorSizeId:
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                                                 
                        //状态
                        label8.Text = evokWork0.getXialiaoJuStatus();
                        label2.Text = evokWork0.getXialiaoJuStatus();

                            

                        }));
                        break;
                    }
                case Constant.doorBanId:
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                        label11.Text = evokWork0.getDoorBanStatus();
                        label3.Text  = evokWork0.getDoorBanStatus();                          
                       
                        
                        }));
                        break;
                    }
                case Constant.doorShellId:
                    {
                        this.Invoke((EventHandler)(delegate
                        {
                            label9.Text =evokWork0.getDoorShellStatus();
                        label29.Text = evokWork0.getDoorShellStatus();
                            
                        
                        }));
                        break;
                    }
            }
          
        }

           
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            workMan.ShowDoor(comboBox1.SelectedItem.ToString(),dgSize, dgDoorBan, dgDoorShell);
        }

        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {
            if (checkBox2.Checked)
            {
                workMan.ShowDoor(Constant.doorShellId, dgDoorShell);
                workMan.ShowDoor(Constant.doorBanId, dgDoorBan);
                workMan.ShowDoor(Constant.doorSizeId, dgSize);
            }
            else
            {
                dgDoorShell.DataSource = null;
                dgDoorBan.DataSource = null;
                dgSize.DataSource = null;
            }
        }

        private void listView1_SelectedIndexChanged(object sender, EventArgs e)
        {
            int rowid = listView1.Items.IndexOf(listView1.FocusedItem);
            workMan.ShowDoor(rowid, dgSize, dgDoorBan, dgDoorShell);
            checkBox2.Checked = false;

        }

       
        void buttonOperation(bool value)
        {
            stbtn.Enabled = value;
            button11.Enabled = value;
            button12.Enabled = value;
            stopBtn.Enabled = value;
        }
        void waitEnd(List<Thread> thLst)
        {
            //等待结束才走 超时60秒退出
            bool loop = true;
            int start = Environment.TickCount;
            if (thLst.Count > 0)
                while (loop)
                {
                    loop = false;
                    Application.DoEvents();
                    
                    foreach (Thread th in thLst)
                    {
                        if (th.IsAlive) loop = true;
                    }                   
                    if(Math.Abs(Environment.TickCount - start) < 60000)
                    {
                        Application.DoEvents();
                    }
                }
        }
       
     

    

        private void Opposite_Click_Auto_2(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork0.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, evokWork0.PsLstAuto);
            }
        }
        Action<object> startDevice;
        private void ON2OFF_Click(object sender, EventArgs e)
        {
            //startDevice(sender);
            //门板
            if (((Button)sender).Equals(button17))
            {
                DialogResult dr1 = MessageBox.Show("是否继续启动，门芯板锯是否有余料？原料尺寸是否正确？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

                if (dr1 == DialogResult.No)
                {
                    return;
                }
                evokWork0.doorBanSingleStart(opDoorBan);
            }
            //门皮
            if (((Button)sender).Equals(button20))
            {
                DialogResult dr2 = MessageBox.Show("是否继续启动，门皮板锯原料尺寸是否正确？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

                if (dr2 == DialogResult.No)
                {
                    return;
                }
                evokWork0.doorShellSingleStart(opDoorShell);
            }
            //下料锯
            if (((Button)sender).Equals(button5))
            {
                DialogResult dr0 = MessageBox.Show("是否继续启动，下料锯原料位置是否正确？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

                if (dr0 == DialogResult.No)
                {
                    return;
                }
                evokWork0.xialiaoSingleStart(opXiaLiao);
              
            }

            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork0.SetMPsONToOFF(((Control)sender).Tag.ToString(), Constant.Write, 0);

            }

            if (((Button)sender).Equals(button6) || ((Button)sender).Equals(button7))
            {
                evokWork0.xialiaoSingleStop();
                opXiaLiao.OptdataClear();
                label30.Text = "0";
                currentDoorSizeId = 0;

            }

            if (((Button)sender).Equals(button16) || ((Button)sender).Equals(button15))
            {
                evokWork0.doorBanSingleStop();
                currentDoorBanId = 0;
                opDoorBan.OptdataClear();
                label31.Text = "0";
            }



            if (((Button)sender).Equals(button18) || ((Button)sender).Equals(button19))
            {
                evokWork0.doorShellSingleStop();
                currentDoorShellId = 0;
                opDoorShell.OptdataClear();
                label32.Text = "0";
            }

        }

        private void button13_Click(object sender, EventArgs e)
        {
            evokWork0.LineEmgStop();
            startBtn(true);
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            shift();
        }

      

       

        private void connectMachine_Click(object sender, EventArgs e)
        {

            if (tabControl1.SelectedIndex == 0)
            {
                UpdateTimer.Enabled = false;
                foreach (EvokXJWork evokWork in evokWorkLst)
                {
                    if (!evokWork.DeviceStatus)
                        if (evokWork.RestartDevice(0))
                    {
                        SetControlInEvokWork();
                        UpdateTimer.Enabled = true;
                    }
                    else
                    {
                        MessageBox.Show(evokWork.DeviceName + Constant.ConnectMachineFail);
                    }

                }
            }
            else MessageBox.Show("请先切换到主页面！");
           
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            if(checkBox2.Checked)
            {

                workMan.doorLstReverse();
                workMan.ShowDoor(Constant.doorShellId, dgDoorShell);
                workMan.ShowDoor(Constant.doorBanId, dgDoorBan);
                workMan.ShowDoor(Constant.doorSizeId, dgSize);

            }
        }
        OpenFileDialog opSingleLoad;
        //加载文件数据
        void LoadData(DataGridView dg1,string name)
        {
            if (opSingleLoad == null)
            {
                opSingleLoad = new OpenFileDialog();
                opSingleLoad.InitialDirectory = ConstantMethod.GetAppPath();
                opSingleLoad.Filter = "文件(csv文件)|*.csv";
            }
            if(File.Exists(name))
            {
                DataTable dt = csvop.OpenCSV(name);
                if (dt != null)
                    dg1.DataSource = dt;
            }
            else
            if (opSingleLoad.ShowDialog() == DialogResult.OK)
            {
                DataTable dt = csvop.OpenCSV(opSingleLoad.FileName);
                if (dt != null)
                    dg1.DataSource = dt;
            }
          
        }
        bool isInSaveData;
        //保存文件数据
        void saveData()
        {
            if (isInSaveData) return;
            isInSaveData = true;
            DataTable dt0 = (DataTable)dgDoorBan.DataSource;
            DataTable dt1 = (DataTable)dgDoorShell.DataSource;
            DataTable dt2 = (DataTable)dgSize.DataSource;
   
            csvop.SaveCSV(dt0, Constant.DoorBanFile);
            csvop.SaveCSV(dt1, Constant.DoorShellFile);
            csvop.SaveCSV(dt2, Constant.DoorSizeFile);
            isInSaveData = false;

        }
        //下料锯排版
        void xialiaojuPai()
        {
            opXiaLiao.OptdataClear();
            opXiaLiao.DtData = (DataTable)dgSize.DataSource;
            dgSize.DataSource = opXiaLiao.DtData;

            opXiaLiao.Len = 2440000;
            opXiaLiao.Dbc = 400;
            opXiaLiao.Ltbc = 400;
            opXiaLiao.Safe = 0;
            opXiaLiao.WlMiniValue = 100;

            ConstantMethod.ShowInfo(richTextBox7, opXiaLiao.OptNormal(richTextBox7, Constant.optTaTa));

            label30.Text = opXiaLiao.ProdInfoLst.Count().ToString();

        }

        void doorShellPai()
        {
            opDoorShell.OptdataClear();
            opDoorShell.DtData = (DataTable)dgDoorShell.DataSource;
            dgDoorShell.DataSource = opDoorShell.DtData;

            opDoorShell.Len = 2100000;
            opDoorShell.Dbc = 400;
            opDoorShell.Ltbc = 400;
            opDoorShell.Safe = 0;
            opDoorShell.WlMiniValue = 100;


            ConstantMethod.ShowInfo(richTextBox8, opDoorShell.OptNormal(richTextBox8, Constant.optTaTa));

            label32.Text = opDoorShell.ProdInfoLst.Count().ToString();
        }

        void doorBanPai()
        {
            opDoorBan.OptdataClear();
            opDoorBan.DtData = (DataTable)dgDoorBan.DataSource;
            dgDoorBan.DataSource = opDoorBan.DtData;
            opDoorBan.Len = 2100000;
            opDoorBan.Dbc = 400;
            opDoorBan.Ltbc = 400;
            opDoorBan.Safe = 0;
            opDoorBan.WlMiniValue = 100;


            ConstantMethod.ShowInfo(rtbResult, opDoorBan.OptNormal(rtbResult, Constant.optTaTa));
            label31.Text = opDoorBan.ProdInfoLst.Count().ToString();

        }
        void addCount(ComboBox cb1,int sum)
        {
            cb1.Items.Clear();
            for (int i = 1; i <= sum; i++)
            {
                cb1.Items.Add(i.ToString());
            }
            if(cb1.Items.Count>0)
            cb1.Text = cb1.Items[0].ToString();
        }
        void startBtn(bool v)
        {
            stbtn.Enabled = v;
            loadDataBtn.Enabled = v;
            button1.Enabled = v;
            timerDoorSize.Enabled = (!v);
        }
        void ClearAllData()
        {
            opDoorBan.OptdataClear();
            opDoorShell.OptdataClear();
            opXiaLiao.OptdataClear();

            label30.Text = "0";
            label31.Text = "0";
            label32.Text = "0";


        }
        private void button1_Click_1(object sender, EventArgs e)
        {

            if(dataCheck(dgDoorBan, 1,
                evokWork0.doorBanLen.ShowValue, evokWork0.doorBanWidth.ShowValue, "门板")
          ||  dataCheck(dgDoorShell, 1,
                evokWork0.doorShellLen.ShowValue, evokWork0.doorShellWidth.ShowValue, "门皮"))
            {
                return;
            }


            startBtn(false);
            xialiaojuPai();
            doorShellPai();
            doorBanPai();
            addCount(comboBox2,dgDoorBan.Rows.Count);
            addCount(comboBox3, dgDoorBan.Rows.Count);
            addCount(comboBox4, dgDoorBan.Rows.Count);
            LogManager.WriteProgramLog("数据排版结束");
            MessageBox.Show("排版完成，可切换至单机页面查看具体结果！");
            startBtn(true);
        }
              
        private void stbtn_Click_1(object sender, EventArgs e)
        {

            startBtn(false);

            DialogResult dr3 = MessageBox.Show("是否继续启动，设备是否已复位，气泵是够已开？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

            if (dr3 == DialogResult.No)
            {
                return;
            }

            DialogResult dr = MessageBox.Show("是否继续启动，数据是否已排版？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

            if (dr == DialogResult.No)
            {
                return;
            }                    
                     
            List<int> deviceIdLst = new List<int>();

            //下料锯
            if (button2.BackColor == Color.Red)
            {
                DialogResult dr0 = MessageBox.Show("是否继续启动，下料锯原料位置是否正确？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

                if (dr0 == DialogResult.No)
                {
                    return;
                }

                if (opXiaLiao.ProdInfoLst.Count < 1)
                {
                    MessageBox.Show("下料锯无数据！");
                    return;
                }
                deviceIdLst.Add(Constant.doorSizeId);
                
             
            }
            //门皮
            if (button3.BackColor == Color.Red)
            {
                DialogResult dr2 = MessageBox.Show("是否继续启动，门皮板锯原料尺寸是否正确？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

                if (dr2 == DialogResult.No)
                {
                    return;
                }

                if (opDoorShell.ProdInfoLst.Count < 1)
                {
                    MessageBox.Show("门皮无数据！");
                    return;
                }

                deviceIdLst.Add(Constant.doorShellId);
                
  
            }

            //门板
            if (button4.BackColor == Color.Red)
            {
                DialogResult dr1 = MessageBox.Show("是否继续启动，门芯板锯是否有余料？原料尺寸是否正确？", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示

                if (dr1 == DialogResult.No)
                {
                    return;
                }
                if (opDoorBan.ProdInfoLst.Count < 1)
                {
                    MessageBox.Show("门板无数据！");
                    return;
                }
                deviceIdLst.Add(Constant.doorBanId);
           

            }

            if (deviceIdLst.Count == 0)
            {
                startBtn(true);
                MessageBox.Show("请先使能设备！");
                return;
            }

            //判断数据是否已生成
            if (evokWork0.IsLineReady(deviceIdLst))
            {               
                evokWork0.downLoadTest(deviceIdLst,opXiaLiao, opDoorBan, opDoorShell);               
            }
            else
            {
                MessageBox.Show("设备未就绪！");
            }

            startBtn(true);

        }

        private void button11_Click_1(object sender, EventArgs e)
        {
            evokWork0.LinePause();
        }

        private void stopBtn_Click_1(object sender, EventArgs e)
        {
            evokWork0.LineStop();
            ClearAllData();
            currentDoorBanId = 0;
            currentDoorShellId = 0;
            currentDoorSizeId = 0;
            
            startBtn(true);
        }

        private void button12_Click(object sender, EventArgs e)
        {
            evokWork0.LineReset();
            ClearAllData();
        }

        private void textBox1_Enter(object sender, EventArgs e)
        {
            evokWork0.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork0.PsLstAuto);
        }

        private void textBox1_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork0.AutoParamTxt_KeyPress(sender, e))
              tabPage2.Focus();
        }

        private void textBox1_Leave(object sender, EventArgs e)
        {
            evokWork0.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork0.PsLstAuto);
        }

        private void button8_Click(object sender, EventArgs e)
        {
            LoadData(dgSize,"1");
        }

        private void button9_Click(object sender, EventArgs e)
        {
            LoadData(dgDoorShell, "1");
        }

        private void button10_Click_1(object sender, EventArgs e)
        {
            LoadData(dgDoorBan, "1");
        }

        private void button21_Click(object sender, EventArgs e)
        {
            richTextBox7.Clear();
            xialiaojuPai();
        }

        private void button22_Click(object sender, EventArgs e)
        {
            richTextBox8.Clear();
            doorShellPai();
        }

        private void button23_Click(object sender, EventArgs e)
        {
            rtbResult.Clear();
            doorBanPai();
        }

        void startClick(object sender)
        {
            

            
        }
        private void button5_KeyUp(object sender, KeyEventArgs e)
        {
            
        }
        private WatchForm wForm;
        private void 查看当前设备数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (!ConstantMethod.UserPassWd())
            {
                return;
            }
            if (tabControl1.SelectedIndex < evokWork0.DataFormCount)
            {
                wForm = new WatchForm();
                wForm.SetShowDataTable(evokWork0.GetDataForm(0));
                //wForm.SetShowDataTable(evokWork.GetDataForm(4));
                wForm.Show();
            }
        }

        private void timerDoorSize_Tick(object sender, EventArgs e)
        {
            timerDoorSize.Enabled = false;
            countDoorSizeUpdate();
            countDoorBanUpdate();
            countDoorShellUpdate();
            timerDoorSize.Enabled = true;
        }
        void setDoneData(int doorId, OptSize op,DataGridView dgv)
        {
            int cntColor=0;
            foreach (DataRow dr in op.DtData.Rows)
            {
                int dataid = 0;
                if (dr[13] != null && (int.TryParse(dr[13].ToString(), out dataid)))
                {

                    string setcnt = dr[2].ToString();
                    string cntdone = dr[1].ToString();
                    if (dataid < doorId && !setcnt.Equals(cntdone))
                    {
                        dr[2] = dr[1];
                        dgv.Rows[cntColor].DefaultCellStyle.BackColor = System.Drawing.Color.Green;
                    }
                    else
                    {
                        if (dataid >= doorId )
                        {
                            dr[2] = 0;
                        }
                    }                  
                }
                cntColor++;
            }

            op.OptdataClear();
            MessageBox.Show("数据已更改，请重新排版！");
        }
        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox2.SelectedIndex == 0) return;

            DialogResult dr1 = MessageBox.Show("下料锯是否从ID号为"+comboBox2.SelectedItem+"开始加工", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
           
            if (dr1 == DialogResult.No)
            {
                return;
            }

            setDoneData(int.Parse(comboBox2.SelectedItem.ToString()), opXiaLiao,dgSize);



        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox3.SelectedIndex == 0) return;

            DialogResult dr1 = MessageBox.Show("门芯板锯是否从ID号为" + comboBox3.SelectedItem + "开始加工", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
           
            if (dr1 == DialogResult.No)
            {
                return;
            }

            setDoneData(int.Parse(comboBox3.SelectedItem.ToString()),opDoorBan, dgDoorBan);
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (comboBox4.SelectedIndex == 0) return;

            DialogResult dr1 = MessageBox.Show("门皮锯是否从ID号为" + comboBox4.SelectedItem + "开始加工", "启动提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
            
            if (dr1 == DialogResult.No)
            {
                return;
            }

            setDoneData(int.Parse(comboBox4.SelectedItem.ToString()), opDoorShell,dgDoorShell);
        }
    }
}

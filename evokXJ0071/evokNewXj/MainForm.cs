using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;

namespace evokNew0071
{   
    public partial class MainForm : Form
    {
        private static Queue<Control> allCtrls = new Queue<Control>();

        //List<string> errorList = new List<string>();
        //int errorId = 0;
        CsvStreamReader csvop;
        OptSize optsize;

        
        OptSize op0;
        OptSize op1;
        OptSize op2;
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
            optsize = new OptSize();
            op0 = new OptSize();
            op1 = new OptSize();          
            op2 = new OptSize();

            evokWork0 = new EvokXJWork(strDataFormPath0, p0);

            evokWork1 = new EvokXJWork(strDataFormPath1, p1);          
         
            evokWork2 = new EvokXJWork(strDataFormPath2, p2);

            InitWork();

            UpdateTimer.Enabled = true;

            doorLst = new doorTypeInfo();

            workMan = new workManager();
     

        }
        private void UpdataEvokWork(EvokXJWork work, ToolStripStatusLabel s)
        {
            if (work == null) return;
            UpdataError(work, s);
           
            //自动页面
            if (work.CurrentPageId== Constant.AutoPage)
            {
                foreach (PlcInfoSimple simple in work.PsLstAuto)
                {
                    int showValue = simple.ShowValue;
                   
                }
            }

            //IO
            if (work.CurrentPageId == Constant.IOPage)
            {
                int valueId = 0;
                foreach (DataGridViewRow dr in work.DgvIO.Rows)
                {
                    if (dr.Cells[1].Value != null)
                    if (int.TryParse(dr.Cells[1].Value.ToString(), out valueId))
                        {
                            if (valueId == Constant.M_ON)
                            {
                                dr.DefaultCellStyle.BackColor = Color.Red;
                            }
                            else
                            {
                                dr.DefaultCellStyle.BackColor = dgvIO.RowsDefaultCellStyle.ForeColor;
                            }
                        }
                }
            }
            //参数
        }
        public void SetControlInEvokWork()
        {
            ConstantMethod.
            CheckAllCtrls(this, allCtrls);
            foreach (Control control in allCtrls)
            {
                if (control.Tag != null)
                {
                    if ((control.Parent == tabPage7))
                   {
                        foreach (PlcInfoSimple simple in evokWork0.PsLstAuto)
                        {
                            if (simple.Name.Contains(control.Tag.ToString()) && simple.Name.Contains(Constant.Read))
                            {
                                simple.ShowControl = control;
                                break;
                            }
                        }
                    }
                 
                    if ((control.Parent == tabPage11))
                    {
                        foreach (PlcInfoSimple simple in evokWork1.PsLstAuto)
                        {
                            if (simple.Name.Contains(control.Tag.ToString()) && simple.Name.Contains(Constant.Read))
                            {
                                simple.ShowControl = control;
                                break;
                            }
                        }
                    }
                  
                    if ((control.Parent == tabPage12))
                    {
                        foreach (PlcInfoSimple simple in evokWork2.PsLstAuto)
                        {
                            if (simple.Name.Contains(control.Tag.ToString()) && simple.Name.Contains(Constant.Read))
                            {
                                simple.ShowControl = control;
                                break;
                            }
                        }
                    }
          
                }
            }
        }

        public void InitWork()
        {

            SetControlInEvokWork();


            evokWork0.InitDgvParam(dgvParam);
            evokWork0.InitDgvIO(dgvIO);
            evokWork0.DeviceProperty = Constant.devicePropertyA;
            evokWork0.SetRtbResult(richTextBox2);
            evokWork0.SetRtbWork(richTextBox1);
            evokWork0.SetLblStatus(label8);
            evokWork0.SetOptSize(op0);
            evokWork0.SetPrintReport(report1);
    
            evokWork1.InitDgvParam(dgvParam1);
            evokWork1.InitDgvIO(dgvIO1);
            evokWork1.DeviceProperty = Constant.devicePropertyB;        
            evokWork1.SetRtbResult(richTextBox3);
            evokWork1.SetRtbWork(richTextBox4);
            evokWork1.SetLblStatus(label9);
            evokWork1.SetOptSize(op1);
          
            evokWork2.InitDgvParam(dgvParam2);
            evokWork2.InitDgvIO(dgvIO2);
            evokWork2.DeviceProperty = Constant.devicePropertyC;
            evokWork2.SetRtbResult(richTextBox5);
            evokWork2.SetRtbWork(richTextBox6);
            evokWork2.SetLblStatus(label11);
            evokWork2.SetOptSize(op2);
        
            //读取设备名
            paraFile = new ConfigFileManager();
            paraFile.LoadFile(Constant.ConfigParamFilePath);
            evokWork0.DeviceName = paraFile.ReadConfig("work0");
            evokWork1.DeviceName = paraFile.ReadConfig("work1");
            evokWork2.DeviceName = paraFile.ReadConfig("work2");


            evokWorkLst.Add(evokWork0);
            evokWorkLst.Add(evokWork1);
            evokWorkLst.Add(evokWork2);        

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            //  evokWork0.ShiftPage(Constant.AutoPage);
            evokWork0.SetRtbWork(richTextBox1);
            evokWork0.shiftToLine();
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            evokWork1.SetRtbWork(richTextBox4);
            evokWork1.shiftToLine();
            // evokWork1.ShiftPage(Constant.AutoPage);
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

            evokWork.ShowLblStatus();

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
        }
       
     
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {


            UpdataEvokWork(evokWork0, statusLabel1);
            UpdataEvokWork(evokWork1, statusLabel2);
            UpdataEvokWork(evokWork2, statusLabel3);
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
                        work.stop();
                    }));
                    t1.Start();
                }

                e.Cancel = false;//退了
            }

            UpdateTimer.Enabled = false;

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

            ConstantMethod.Delay(100);

            Environment.Exit(0);
        }

       
     
        private void work0IO_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (work0Tab.SelectedIndex == 1)
            {
                if (!evokWork0.ShiftPage(Constant.IOPage))
                {
                    e.Cancel = true;
                }                
            }

            if (work0Tab.SelectedIndex == 0)
            {
                if (!evokWork0.ShiftPage(Constant.AutoPage))
                {
                    e.Cancel = true;
                }

            }

            if (work0Tab.SelectedIndex == 2)
            {
                if (!evokWork0.ShiftPage(Constant.ParamPage))
                {
                    e.Cancel = true;
                }
                
            }
        }

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
            comboBox1.DataSource = null;
            dgSize.DataSource = null;
            dgDoorBan.DataSource = null;
            dgDoorShell.DataSource = null;
            if (workMan.LoadData())
            {
                workMan.ShowResult(listView1);               
                workMan.showDoorTypeList(comboBox1, 0);
            }                                         
        }
        void shift()
        {
            evokWork0.SetRtbWork(richTextBox7);
            evokWork0.shiftToLine();
           

            evokWork1.SetRtbWork(richTextBox8);
            evokWork1.shiftToLine();
         

            evokWork2.SetRtbWork(rtbResult);
            evokWork2.shiftToLine();
         
        }
        void startLine()
        {
            op0.DtData = (DataTable)dgSize.DataSource;
            op1.DtData = (DataTable)dgDoorShell.DataSource;
            op2.DtData = (DataTable)dgDoorBan.DataSource;

            
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
                if (work.IsInEmg)
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
            { MessageBox.Show(Constant.alreadyStart); }
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
           
           op0.DtData = (DataTable)dgSize.DataSource;
           StartWork(tabControl1.SelectedIndex-1);

        }
        #endregion
        public void writeData(object sender, KeyPressEventArgs e,EvokXJWork evokWork)
        {
            if (evokWork.lcTxt_KeyPress(sender, e))
                resetBtn.Focus();
        }
        private void lcTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            writeData(sender, e, evokWorkLst[tabControl1.SelectedIndex - 1]);
        }
        public void SetInEdit(object sender, EventArgs e, EvokXJWork evokWork)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);
        }
        private void lcTxt_Enter(object sender, EventArgs e)
        {
            SetInEdit(sender, e, evokWorkLst[tabControl1.SelectedIndex - 1]);
        }
        public void SetOutEdit(object sender, EventArgs e, EvokXJWork evokWork)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);
        }
        private void lcTxt_Leave(object sender, EventArgs e)
        {

            SetOutEdit(sender, e, evokWorkLst[tabControl1.SelectedIndex - 1]);
        }

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
        private void errorTimer_Tick(object sender, EventArgs e)
        {

            errorList.Clear();
            foreach (EvokXJWork evokwork in evokWorkLst)
            {
                if (evokwork != null)
                {
                    for (int i = 0; i < evokwork.ErrorList.Count; i++)
                    {
                        errorList.Add(evokwork.DeviceName+evokwork.ErrorList[i]);
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

        private void button6_Click(object sender, EventArgs e)
        {
            op1.DtData = (DataTable)dgDoorShell.DataSource;
           
            StartWork(tabControl1.SelectedIndex - 1);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            
            if (tabControl1.SelectedIndex>0)
            if(evokWorkLst==null||evokWorkLst.Count==0||evokWorkLst[tabControl1.SelectedIndex-1] ==null || !evokWorkLst[tabControl1.SelectedIndex - 1].DeviceStatus )
            {

                evokWorkLst[tabControl1.SelectedIndex - 1].ShiftPage(Constant.AutoPage);
                MessageBox.Show("设备错误！");
                e.Cancel = true;
            }
           
        }

        private void button10_Click(object sender, EventArgs e)
        { 
            op2.DtData = (DataTable)dgDoorBan.DataSource;


            StartWork(tabControl1.SelectedIndex - 1);       
              
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
        }

        private void button11_Click(object sender, EventArgs e)
        {
                       
            foreach (EvokXJWork work in evokWorkLst)
            {
                
                    Thread t1 = new Thread(new ThreadStart(delegate
                    {
                        work.pause();
                    }));
                    t1.Start();
                           
            }
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
        private void stopBtn_Click(object sender, EventArgs e)
        {
            buttonOperation(false);
            List<Thread> thLst = new List<Thread>();

            foreach (EvokXJWork work in evokWorkLst)
            {
                Thread t1 = new Thread(new ThreadStart(delegate
                {
                    work.stop();
                }));
                t1.Start();
                thLst.Add(t1);
            }

            waitEnd(thLst);


            buttonOperation(true);
        }

        private void button12_Click(object sender, EventArgs e)
        {

        }

        private void tabControl1_Enter(object sender, EventArgs e)
        {
            evokWork2.SetRtbWork(richTextBox6);
        }

        private void tabPage1_Leave(object sender, EventArgs e)
        {
            
        }

        private void tabControl1_Deselecting(object sender, TabControlCancelEventArgs e)
        {
            if (tabControl1.SelectedIndex == 0)
                //如果在运行中 报警中 暂停中
                foreach (EvokXJWork work in evokWorkLst)
                {
                    //如果在运行中 则不切换
                    if (
                         work.deviceStatusId  == Constant.constantStatusId[1]
                      || work.deviceStatusId == Constant.constantStatusId[2]
                      || work.deviceStatusId == Constant.constantStatusId[3]
                      || work.deviceStatusId == Constant.constantStatusId[5])
                    {
                        MessageBox.Show(work.DeviceName+Constant.constantStatusStr[work.deviceStatusId]);
                        e.Cancel = true;
                        return;

                    }  
                //如果已启动            
                if (work.RunFlag )
                {
                    MessageBox.Show(work.DeviceName + "设备运行中，请先停止！");
                    tabControl1.SelectedIndex = 0;
                    e.Cancel = true;
                    return;
                }
            }
        }

        private void button12_Click_1(object sender, EventArgs e)
        {
            buttonOperation(false);
            List<Thread> thLst = new List<Thread>();
            foreach (EvokXJWork work in evokWorkLst)
            {
                if (!work.RunFlag)
                {
                    Thread t1 = new Thread(new ThreadStart(delegate
                    {
                        work.reset();
                    }));
                    t1.Start();
                    thLst.Add(t1);
                }
                else MessageBox.Show(work.DeviceName+"请先发送停止信号！");
            }

            //等待结束才走
            waitEnd(thLst);

            buttonOperation(true);

        }
        private void Opposite_Click_Auto_2(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork2.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, evokWork2.PsLstAuto);
            }
        }
        private void ccBtn_Click(object sender, EventArgs e)
        {
            Opposite_Click_Auto_2(sender, e);
        }

        private void button13_Click(object sender, EventArgs e)
        {

            foreach (EvokXJWork work in evokWorkLst)
            {
                Thread t1 = new Thread(new ThreadStart(delegate
                {
                    work.emgStop();
                }));
                t1.Start();

            }          
            
        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            shift();
        }

        private void button14_Click(object sender, EventArgs e)
        {
            richTextBox7.Clear();
            richTextBox8.Clear();
            rtbResult.Clear();
        }

        private void tabPage4_Enter(object sender, EventArgs e)
        {
            evokWork2.SetRtbWork(richTextBox6);
            evokWork2.shiftToLine();
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
    }
}

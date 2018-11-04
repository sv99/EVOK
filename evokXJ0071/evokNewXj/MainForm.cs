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

        OptSize op1;
        OptSize op0;
        private EvokXJWork evokWork0;
        private EvokXJWork evokWork1;
        private EvokXJWork evokWork2;


        List<EvokXJWork> evokWorkLst = new List<EvokXJWork>();
        workManager workMan;
        //private OptSize optSize;
        //private WatchForm wForm;
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
            op1 = new OptSize();
            op0 = new OptSize();

            evokWork0 = new EvokXJWork(strDataFormPath0, p0);

            evokWork1 = new EvokXJWork(strDataFormPath1, p1);
           
         
            //evokWork2 = new EvokXJWork(strDataFormPath1, p2);

            InitWork();

            UpdateTimer.Enabled = true;

            doorLst = new doorTypeInfo();

            workMan = new workManager();

         

        }
        private void UpdataEvokWork(EvokXJWork work, ToolStripStatusLabel s)
        {

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
            evokWork0.SetOptSize(op0);


            evokWork1.InitDgvParam(dgvParam1);
            evokWork1.InitDgvIO(dgvIO1);
            evokWork1.DeviceProperty = Constant.devicePropertyB;        
            evokWork1.SetRtbResult(richTextBox3);
            evokWork1.SetRtbWork(richTextBox4);
            evokWork1.SetOptSize(op1);



            //读取设备名
            paraFile = new ConfigFileManager();
            paraFile.LoadFile(Constant.ConfigParamFilePath);
            evokWork0.DeviceName = paraFile.ReadConfig("work0");
            evokWork1.DeviceName = paraFile.ReadConfig("work1");
            //evokWork2.DeviceName = paraFile.ReadConfig("work2");


            evokWorkLst.Add(evokWork0);
            evokWorkLst.Add(evokWork1);
           // evokWorkLst.Add(evokWork2);        

        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            evokWork0.ShiftPage(Constant.AutoPage);
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            evokWork1.ShiftPage(Constant.AutoPage);
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
        }


        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdataEvokWork(evokWork0, statusLabel1);
            UpdataEvokWork(evokWork1, statusLabel2);

           //UpdataEvokWork(evokWork2, statusLabel1);
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            DialogResult dr = MessageBox.Show("是否继续关闭程序？", "关闭提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
            if (dr == DialogResult.No)
            {
                e.Cancel = true;//就不退了
                return;
            }
            else
            {
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

        private void tabPage7_Enter(object sender, EventArgs e)
        {
            evokWork0.ShiftPage(Constant.ParamPage);
        }

        private void tabPage8_Enter(object sender, EventArgs e)
        {
            evokWork1.ShiftPage(Constant.ParamPage);
        }

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            evokWork1.ShiftPage(Constant.IOPage);
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
            if (work1Tab.SelectedIndex == 0)
            {
                if (!evokWork1.ShiftPage(Constant.AutoPage))
                {
                    e.Cancel = true;
                }
                
            }
            if (work1Tab.SelectedIndex == 1)
            {
                if (!evokWork1.ShiftPage(Constant.IOPage))
                {
                    e.Cancel = true;
                }

            }
            if (work1Tab.SelectedIndex == 2)
            {
                if (!evokWork1.ShiftPage(Constant.ParamPage))
                {
                    e.Cancel = true;
                }
                
            }

        }

        private void loadDataBtn_Click(object sender, EventArgs e)
        {

            if (workMan.LoadData())
            {
                workMan.ShowResult(listBox1);
                listBox1.SelectedIndex = 0;
                workMan.
                ShowDoor(listBox1.SelectedIndex, dgSize, dgDoorBan, dgDoorShell);
               
            }          
        }



        private void stbtn_Click(object sender, EventArgs e)
        {
          
           
            //HandleTest();
           // evokWork1.optReady(Constant.optNormal);
            //确定设备处于电脑控制状态
            //发送启动信号
            //进行数据统计
          //  ConstantMethod.ShowInfo(rtbResult, op0.OptNormal(rtbResult));
            //ConstantMethod.ShowInfo(rtbResult, op1.OptNormal(rtbResult));

            
           // evokWork1.CutDoorStartNormal(Constant.CutNormalDoorMode);
           // evokWork0.CutStartNormal(Constant.CutNormalMode);
           // evokWork1.CutStartNormal(Constant.CutNormalMode);
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (listBox1.SelectedIndex > 0)
            {
                workMan.
                ShowDoor(listBox1.SelectedIndex, dgSize, dgDoorBan, dgDoorShell);
            }
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

          
            evokWorkLst[id].optReady(Constant.optNormal);

            switch(evokWorkLst[id].DeviceProperty)
            {
                case Constant.devicePropertyA:
                    {
                        evokWorkLst[id].CutDoorStartNormal(Constant.CutNormalMode);
                        break;
                    }
                case Constant.devicePropertyB:
                    {
                        evokWorkLst[id].CutDoorStartNormal(Constant.CutNormalDoorMode);
                        break;
                    }
                default:
                    {
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
            op1.DtData = (DataTable)dgDoorBan.DataSource;

            StartWork(tabControl1.SelectedIndex - 1);
        }

        private void tabControl1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if(tabControl1.SelectedIndex>0)
            if(evokWorkLst==null||evokWorkLst.Count==0||evokWorkLst[tabControl1.SelectedIndex-1] ==null || !evokWorkLst[tabControl1.SelectedIndex - 1].DeviceStatus )
            {
                MessageBox.Show("设备错误！");
                e.Cancel = true;
            }
        }
    }
}

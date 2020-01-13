using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using xjplc;
using System.IO;

namespace evokNew0081
{
    public partial class WorkForm : Form
    {
        //存储控件
        private static Queue<Control> allCtrls = new Queue<Control>();

        //错误信息
        List<string> errorList = new List<string>();

        //错误带有多种语言的时候
        int errorId = 0;
    
        private EvokXJWork evokWork;

        private WatchForm wForm;


        ConfigFileManager cfg;
        public WorkForm()
        {     
            //需要密码的时候     
          //ConstantMethod.InitPassWd();
            InitializeComponent();
        }
        #region 初始化
        public void InitLang()
        {
            //语言设置
            //设置提醒字符串
            Constant.InitStr(this);
            //evokWork.ShiftDgvParamLang(dgvParam, MultiLanguage.getLangId());
            //evokWork.updateColName(dgvParam);
            //evokWork.updateColName(dgvIO);
            //一些控件的库需要更换
            /***
            string[] s = Constant.cutMode.Split('/');
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(s);
            comboBox1.SelectedIndex = evokWork.CutSelMode;
            s = Constant.printMode.Split('/');
            ***/

        }
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            Constant.InitStr(this);
            InitParam();
            InitControl();
            InitView0();
            InitLang();
            Application.DoEvents();
            //evokWork.ListSmallSizePrinter(comboBox3);
            this.Visible = true;
        }

        void ReadUserData()
        {
            if (cfg == null) cfg = new ConfigFileManager(Constant.ConfigUserDataFilePath);
            textBox3.Text = cfg.ReadConfig("pos1");
            textBox2.Text = cfg.ReadConfig("pos2");
            textBox4.Text = cfg.ReadConfig("pos3");
            textBox5.Text = cfg.ReadConfig("pos4");
            textBox6.Text = cfg.ReadConfig("pos5");
            
        }
        void WriteUserData()
        {

            cfg.WriteConfig("pos1", textBox3.Text);
            cfg.WriteConfig("pos2", textBox2.Text);
            cfg.WriteConfig("pos3", textBox4.Text);
            cfg.WriteConfig("pos4", textBox5.Text);
            cfg.WriteConfig("pos5", textBox6.Text);
           
        }
        private void InitControl()
        {
            SetControlInEvokWork();
          //  printcb.SelectedIndex = evokWork.PrintBarCodeMode;
           // evokWork.ChangePrintMode(printcb.SelectedIndex);
        }
        public void InitParam()
        {
            //datasource 改变会出发 selectindex 改变事件  这样就会打条码导致 模式被自动修改
            //所以早点设置好 然后在 那个selectindexchanged事件里增加 通讯正常判断
            //printcb.DataSource = Constant.printBarcodeModeStr;
           //printcb.Items.AddRange(Constant.printBarcodeModeStr);
            LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);
            evokWork = ConstantMethod.GetWork();
            evokWork.MainForm = this;
            //evokWork.SetUserDataGridView(UserData);
            //evokWork.SetRtbWork(rtbWork);
            //evokWork.SetRtbResult(rtbResult);
            evokWork.SetPrintReport();
            //evokWork.InitDgvParam(dgvParam);
           // evokWork.InitDgvIO(dgvIO);
            evokWork.SetOptParamShowCombox(comboBox2);
            
            errorList = evokWork.ErrorList;
            UpdateTimer.Enabled = true;

        }
        private void InitView0()
        {
            DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DialogExcelDataLoad.Filter = "文件(*.xls,*.xlsx,*.csv)|*.xls;*.csv;*.xlsx";
            DialogExcelDataLoad.FileName = "请选择数据文件";

            logOPF.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log";
            logOPF.Filter = "文件(*.log)|*.log";
            logOPF.FileName = "请选择日志文件";
            comboBox1.SelectedIndex = evokWork.CutSelMode;
            errorTimer.Enabled = true;

            evokWork.ReadCSVDataDefault();

            ReadUserData();


        }
        #endregion
        #region 按钮和数据输入事件
        private void autoSLBtn_Click(object sender, EventArgs e)
        {
           
             evokWork.scarModeSelect();
        }
        private void lcTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork.AutoParamTxt_KeyPress(sender, e))
                tc1.Focus();
        }

        private void AutoTextBox_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);           
        }

        private void AutoTxt_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);           
        }
        private void lcTxt_KeyPress0(object sender, KeyPressEventArgs e)
        {
            if (evokWork.AutoParamTxt_KeyPressWithRatioWithId(sender, e, tc1.SelectedIndex))
                tc1.Focus();
        }


        private void TextBox_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
        }

        private void Txt_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
        }
        private void BtnM101_MouseDown(object sender, MouseEventArgs e)
        {
            evokWork.SetMPsOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
        }

      
       private void connectMachine_Click(object sender, EventArgs e)
        {

            UpdateTimer.Enabled = false;

            if (evokWork.RestartDevice(tc1.SelectedIndex))
            {
                InitControl();
                UpdateTimer.Enabled = true;
            }
            else
            {
                MessageBox.Show(Constant.ConnectMachineFail);
            }
        }
    
      
        private void HandOff2On_Click(object sender, EventArgs e)
        {

        }

        private void qClr_Click(object sender, EventArgs e)
        {
            evokWork.ProClr();
        }
   

        private void BtnM101_MouseUp(object sender, MouseEventArgs e)
        {
            evokWork.SetMPsOff(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
        }
        private void button5_Click(object sender, EventArgs e)
        {
            if (evokWork.lliao)
            {
                evokWork.lliaoOFF();
            }
            else
            {
                evokWork.lliaoON();
            }
        }

        private void button11_Click(object sender, EventArgs e)
        {
            evokWork.pressShift();
        }

      
        private void 语言切换CHToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }

        private void menuItemChinese_Click(object sender, EventArgs e)
        {

            MultiLanguage.setLangId(Constant.idChinese, evokWork.ParamFile);

            InitLang();
        }

        private void menuItemEnglish_Click(object sender, EventArgs e)
        {
            MultiLanguage.setLangId(Constant.idEnglish, evokWork.ParamFile);
            InitLang();
        }

        #endregion
        #region 定时更新页面信息
        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
            UpdataError();
            UpdataAuto();
            UpdataHand();
            UpdataParam();
            UpdataIO();
        }

        private void FileSave_Tick(object sender, EventArgs e)
        {
            if (evokWork != null)
                evokWork.SaveFile();
        }

        private void UpdataAuto()
        {
            if (tc1.SelectedIndex == 0)
            {
                
                foreach (PlcInfoSimple simple in evokWork.PsLstAuto)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }

        private void UpdataError()
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
            if (tc1.SelectedIndex == 0)
            {
                foreach (PlcInfoSimple p in evokWork.PsLstAuto)
                {
                    if (p.Name.Contains(Constant.Alarm) && p.ShowStr != null && p.ShowStr.Count > 0)
                    {
                        for (int i = 0; i < p.ShowStr.Count; i++)
                        {
                            int index = errorList.IndexOf(p.ShowStr[i]);
                            if (p.ShowValue == Constant.M_ON && index < 0)
                            {
                                errorList.Add(p.ShowStr[i]);
                            }
                            if (p.ShowValue == Constant.M_OFF && index > -1 && index < errorList.Count)
                            {
                                errorList.RemoveAt(index);
                            }
                        }

                    }
                }
            }
            if (tc1.SelectedIndex == 1)
            {
                foreach (PlcInfoSimple p in evokWork.PsLstHand)
                {
                    if (p.Name.Contains(Constant.Alarm) && p.ShowStr != null && p.ShowStr.Count > 0)
                    {
                        for (int i = 0; i < p.ShowStr.Count; i++)
                        {
                            int index = errorList.IndexOf(p.ShowStr[i]);
                            if (p.ShowValue == Constant.M_ON && index < 0)
                            {
                                errorList.Add(p.ShowStr[i]);
                            }
                            if (p.ShowValue == Constant.M_OFF && index > 0 && index < p.ShowStr.Count)
                            {
                                errorList.RemoveAt(index);
                            }
                        }

                    }
                }
            }


        }
        private void UpdataHand()
        {
            if (tc1.SelectedIndex == 1)
            {
                foreach (PlcInfoSimple simple in evokWork.PsLstHand)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }
        private void UpdataParam()
        {
            if (tc1.SelectedIndex == 2)
            {
                foreach (PlcInfoSimple simple in evokWork.PsLstParam)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }
        private void UpdataIO()
        {
            if (tc1.SelectedIndex == 3)
            {

                foreach (PlcInfoSimple simple in evokWork.PsLstIO)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }
        #endregion
                          
      
        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
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

            UpdateTimer.Enabled = false;
            FileSaveTimer.Enabled = false;
            if ( evokWork != null)
            {
                 evokWork.Dispose();
            }
            WriteUserData();
            ConstantMethod.Delay(100);
           
            Environment.Exit(0);
        }
       
    
        public void SetControlInEvokWork()
        {
            ConstantMethod.
            CheckAllCtrls(this, allCtrls);
            foreach (Control control in allCtrls)
            {
                if (control.Tag != null)
                {
                    if ((control.Parent == tabPage1) || (control.Parent.Parent == tabPage1))
                    {
                        foreach (PlcInfoSimple simple in evokWork.PsLstAuto)
                        {
                           
                            if (ConstantMethod.setControlInPlcSimple(simple, control)) break;
                            //增加了判断 只要匹配到 就自动跳出来 下一个 201904251220;
                        }
                        continue;
                    }

                    if ((control.Parent != null && control.Parent == tabPage2)
                        || (control.Parent.Parent != null && control.Parent.Parent == tabPage2)
                        || (control.Parent.Parent.Parent != null && control.Parent.Parent.Parent == tabPage2)
                        || (control.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent == tabPage2))
                        {

                        foreach (PlcInfoSimple simple2 in evokWork.PsLstHand)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple2, control)) break;
                        }
                        continue;
                    }
                    if ((control.Parent == tabPage3)
                        || (control.Parent.Parent == tabPage3)
                        || control.Parent.Parent.Parent == tabPage3
                        || control.Parent.Parent.Parent.Parent == tabPage3)
                    {
                        foreach (PlcInfoSimple simple3 in evokWork.PsLstParam)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple3, control)) break;
                        }
                        continue;
                    }
                    if ((control.Parent == tabPage4)
                       || (control.Parent.Parent == tabPage4)
                       || control.Parent.Parent.Parent == tabPage4
                       || control.Parent.Parent.Parent.Parent == tabPage4)
                    {
                        foreach (PlcInfoSimple simple4 in evokWork.PsLstIO)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple4, control)) break;
                        }
                        continue;
                    }
                }
            }
        }
     
      public void showWatchForm()
        {
            if (wForm != null && wForm.Visible)
            {
                wForm.SetShowDataTable(evokWork.GetDataForm(tc1.SelectedIndex));
            }
        }
        private void tc1_Selecting(object sender, TabControlCancelEventArgs e)
        {
          
            if ( evokWork.RunFlag)
            {
                MessageBox.Show(Constant.IsWorking);
                e.Cancel = true;               
            }
            else if (!evokWork.ShiftPage(tc1.SelectedIndex))
            {
                e.Cancel = true;
            }
            showWatchForm();
        }

        private void 监控当前页面数据ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            if (!ConstantMethod.UserPassWd())
            {
                return;
            }
            if (tc1.SelectedIndex < evokWork.DataFormCount)
            {
                wForm = new WatchForm();
                wForm.SetShowDataTable(evokWork.GetDataForm(tc1.SelectedIndex));
                //wForm.SetShowDataTable(evokWork.GetDataForm(4));
                wForm.Show();
            }


        }

        private void errorTimer_Tick(object sender, EventArgs e)
        {
            if (errorList.Count > 0)
            {
                if (errorId >= errorList.Count)
                {
                    errorId = 0;
                }
               int id = MultiLanguage.getLangId();
                string[] splitError = errorList[errorId].Split('/');
                if (splitError.Length > 1)
                {
                     infoLbl.Text = splitError[id];
                }
                else
                {
                    infoLbl.Text = errorList[errorId];
                }

                errorId++;

            }
            else infoLbl.Text = "";
        }
              
      

        private void tabPage4_Enter(object sender, EventArgs e)
        {
            //dgvIO.ClearSelection();
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            //dgvParam.ClearSelection();
        }

        

        private void 查看日志文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {            
            evokWork.ShowNowLog(LogManager.LogFileName);
        }

        private void 加载历史日志文件ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (logOPF.ShowDialog() == DialogResult.OK)
            {
             
                evokWork.ShowNowLog(logOPF.FileName);
            }
        }

        private void 设备ToolStripMenuItem_Click(object sender, EventArgs e)
        {
           

        }
                 
        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if(comboBox1.Focused)
            evokWork.SetMode((comboBox1.SelectedIndex));
            tc1.Focus();
        }


        private void Opposite_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
            }
        }
        private void OnToOff_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.SetMPsONToOFF(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
            }
        }
        private void On_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.SetMPsOn(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
            }
        }
        private void label18_Click(object sender, EventArgs e)
        {

        }
        
        private void button15_Click(object sender, EventArgs e)
        {
            if (!evokWork.SetPos(textBox3.Text))
            {
                MessageBox.Show("数据设置错误！");
            }
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            if (!evokWork.SetPos(textBox2.Text))
            {
                MessageBox.Show("数据设置错误！");
            }
            
        }

        private void button16_Click(object sender, EventArgs e)
        {
            if (!evokWork.SetPos(textBox4.Text))
            {
                MessageBox.Show("数据设置错误！");
            }
            
        }

        private void button17_Click(object sender, EventArgs e)
        {
            if (!evokWork.SetPos(textBox5.Text))
            {
                MessageBox.Show("数据设置错误！");
            }
        }

        private void button18_Click(object sender, EventArgs e)
        {
            if (!evokWork.SetPos(textBox6.Text))
            {
                MessageBox.Show("数据设置错误！");
            }
        }

        private void textBox14_TextChanged(object sender, EventArgs e)
        {

        }
    }   
}

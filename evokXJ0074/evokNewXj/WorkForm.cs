using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using xjplc;
using System.IO;
using xjplc.报警;

namespace evokNew0074
{
    public partial class WorkForm : Form
    {
        private static Queue<Control> allCtrls = new Queue<Control>();

        List<string> errorList = new List<string>();
        int errorId = 0;

        private EvokXJWork evokWork;

        private Alarm alarmForm;
        private WatchForm wForm;

        public WorkForm()
        {
            //addDataTable();
            //ConstantMethod.InitPassWd();
            InitializeComponent();
        }
        #region 参数页面
        private void ParamTextBox_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstParam);
        }
        private void ParamTextTxt_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstParam);
        }
        private void ParamTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork.ParamParamTxt_KeyPress(sender, e))
                button88.Focus();
        }
        #endregion
        #region 手动画面操作
       
        private void HandTextBox_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
        }
        private void HandTextTxt_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
        }
        private void HandTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork.HandParamTxt_KeyPress(sender, e))
                handTc.Focus();
        }

        #endregion
        private void autoSLBtn_Click(object sender, EventArgs e)
        {
            evokWork.scarModeSelect();
        }

        private void AutoTextBox_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);
        }

        private void AutoTxt_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);
        }

        private void BtnM101_MouseDown(object sender, MouseEventArgs e)
        {
            evokWork.SetMPsOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
        }

        private void button4_Click(object sender, EventArgs e)
        {
            /***单机启动
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {evokWork.lcOutInPs.ShowValue;
                optSize.Dbc = 0;//evokWork.dbcOutInPs.ShowValue;
                optSize.Ltbc = 0;/// evokWork.ltbcOutInPs.ShowValue;
                optSize.Safe = 0;// evokWork.safeOutInPs.ShowValue;
                optSize.LoadCsvData(DialogExcelDataLoad.FileName);
            } 
            ****/
            loadDataBtn.Enabled = false;
            ReadCSVData();
            loadDataBtn.Enabled = true;

        }

        private void ccBtn_Click(object sender, EventArgs e)
        {
            if (evokWork.AutoMes)
            {
                evokWork.autoMesOFF();
            }
              //  optSize.Len = 244000
            else
            {
                evokWork.autoMesON();
            }
        }

        private static void CheckAllCtrls(Control item)
        {
            for (int i = 0; i < item.Controls.Count; i++)
            {
                if (item.Controls[i].HasChildren)
                {
                    CheckAllCtrls(item.Controls[i]);
                }
                allCtrls.Enqueue(item.Controls[i]);
            }
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

        private void dgvParam_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int rowIndex = dgvParam.SelectedCells[0].RowIndex;
            evokWork.DgvInOutEdit(rowIndex, true);
        }

        private void dgvParam_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            evokWork.dgvParam_CellEndEdit(dgvParam, sender, e);
        }

        private void dgvParam_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            dgvParam.EndEdit();
        }

        private void dgvParam_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            InitParam();
            InitControl();
            InitView0();
            Application.DoEvents();
            this.Visible = true;
        }
    
        private void HandOff2On_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
            }
        }
        private void HandOff_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.SetMPsOff(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
            }
        }
        private void Opposite_Click_Auto(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
            }
        }
        private void Opposite_Click_Param(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstParam);
            }
        }
        private void InitControl()
        {
            SetControlInEvokWork();
            printcb.SelectedIndex = evokWork.PrintBarCodeMode;
            evokWork.ChangePrintMode(printcb.SelectedIndex);

        }
        public void InitParam()
        {
            //datasource 改变会出发 selectindex 改变事件  这样就会打条码导致 模式被自动修改
            //所以早点设置好 然后在 那个selectindexchanged事件里增加 通讯正常判断
            printcb.DataSource = Constant.printBarcodeModeStr;

            LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);
            evokWork = ConstantMethod.GetWork();
            //梳齿机需要增加数据
            InitShuChi();
            evokWork.SetUserDataGridView(UserData);
            evokWork.SetRtbWork(rtbWork);
            evokWork.SetRtbResult(rtbResult);
            evokWork.SetPrintReport();
            evokWork.InitDgvParam(dgvParam);
            evokWork.InitDgvIO(dgvIO);
            UpdateTimer.Enabled = true;


        }
        #region 梳齿机
        public void InitShuChi()
        {
            evokWork.DeviceName = Constant.scjDeivceName;
            evokWork.DeviceProperty = Constant.scjDeivceId;
            addDataTable();
        }
        public void addDataTable()
        {
            List<string> strLst = new List<string>();
            string handFile = "";
            for (int i = 1; i < Constant.constantHandName.Length; i++)
            {
                try
                {
                    handFile = "";
                    handFile = Constant.PlcDataFilePathHandSC + Constant.constantHandName[i] + ".csv";
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }

                if (File.Exists(handFile))
                {
                    strLst.Add(handFile);
                }
            }
            evokWork.addDataForm(strLst);
        }
        #endregion
        private void InitView0()
        {
            DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DialogExcelDataLoad.Filter = "文件(*.xls,*.xlsx,*.csv)|*.xls;*.csv;*.xlsx";
            DialogExcelDataLoad.FileName = "请选择数据文件";

            logOPF.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log";
            logOPF.Filter = "文件(*.log)|*.log";
            logOPF.FileName = "请选择日志文件";
            //comboBox1.SelectedIndex = 2;
            errorTimer.Enabled = true;

            evokWork.ReadCSVDataDefault();
            

        }
       
        private void lcTxt_KeyPress0(object sender, KeyPressEventArgs e)
        {
            if (evokWork.AutoParamTxt_KeyPressWithRatio(sender, e))
                resetBtn.Focus();
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
            FileSaveTimer.Enabled = false;
            if (evokWork != null)
            {
                evokWork.Dispose();
            }
            ConstantMethod.Delay(100);

            Environment.Exit(0);
        }

        private void optBtn_Click(object sender, EventArgs e)
        {
            optBtn.Enabled = false;
            optBtn.BackColor = Color.Red;
            rtbResult.Clear();
            rtbWork.Clear();
            startOptShow();         

            evokWork.optReady(Constant.optShuChi);

          
            stopOptShow();
            optBtn.BackColor = Color.Transparent;
            optBtn.Enabled = true;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            evokWork.pause();

        }

        private void qClr_Click(object sender, EventArgs e)
        {
                    
            evokWork.ProClr();
        }

        private int ReadCSVData()
        {
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {
                ConstantMethod.SaveDirectoryByFileDialog(DialogExcelDataLoad);
                int num = ConstantMethod.IsWhichFile(DialogExcelDataLoad.FileName);
                if (num == Constant.CsvFile)
                {
                    evokWork.LoadCsvData(DialogExcelDataLoad.FileName);
                }
                if (num == Constant.ExcelFile)
                {
                    evokWork.LoadExcelData(DialogExcelDataLoad.FileName);
                }
            }
            return 0;
        }

        private void resetBtn_Click(object sender, EventArgs e)
        {
            evokWork.reset();
        }

        /// <summary>
        /// 控件tag 名称和plcsimple 结合起来
        /// plcsimple name只要包含 就可以和这个控件联合起来了 
        /// </summary>
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
                 
                    if ((control.Parent!=null && control.Parent == tabPage2)                      
                        || (control.Parent.Parent != null && control.Parent.Parent == tabPage2)
                        || (control.Parent.Parent.Parent!= null &&control.Parent.Parent.Parent == tabPage2)
                        || (control.Parent.Parent.Parent.Parent != null&&control.Parent.Parent.Parent.Parent == tabPage2)                        
                        || (control.Parent.Parent.Parent.Parent.Parent != null&& control.Parent.Parent.Parent.Parent.Parent == tabPage2))
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
                    if ((control.Parent != null && control.Parent == tabPage4)
                        || (control.Parent.Parent != null && control.Parent.Parent == tabPage4)
                        || (control.Parent.Parent.Parent != null && control.Parent.Parent.Parent == tabPage4)
                        || (control.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent == tabPage4)
                        || (control.Parent.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent.Parent == tabPage4))
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

        private void startBtnShow()
        {
            stbtn.Enabled = false;
            loadDataBtn.Enabled = false;
            optBtn.Enabled = false;
            button10.Enabled = false;
            qClr.Enabled = false;
            autoSLBtn.Enabled = false;
            ccBtn.Enabled = false;
            UserData.ReadOnly = true;
            printcb.Enabled = false;
            设备ToolStripMenuItem.Enabled = false;
        }
        private void startOptShow()
        {
            stbtn.Enabled = false;
            loadDataBtn.Enabled = false;
            optBtn.Enabled = false;
            button10.Enabled = false;
            qClr.Enabled = false;
            autoSLBtn.Enabled = false;
            ccBtn.Enabled = false;

            stopBtn.Enabled = false;
            pauseBtn.Enabled = false;
            resetBtn.Enabled = false;
            printcb.Enabled = false;
            if (rtbResult != null) rtbResult.Clear();
            ConstantMethod.ShowInfo(rtbResult, Constant.InOPT);

            设备ToolStripMenuItem.Enabled = false;

        }
        private void stopOptShow()
        {
            stbtn.Enabled = true;
            loadDataBtn.Enabled = true;
            optBtn.Enabled = true;
            button10.Enabled = true;
            qClr.Enabled = true;
            autoSLBtn.Enabled = true;
            ccBtn.Enabled = true;
            stopBtn.Enabled = true;
            pauseBtn.Enabled = true;
            resetBtn.Enabled = true;
            printcb.Enabled = true;
            设备ToolStripMenuItem.Enabled = true;

        }
        private void stbtn_Click(object sender, EventArgs e)
        {

            startBtnShow();
            evokWork.CutStartNormal(Constant.CutNormalWithShuChiMode);           
            stopBtnShow();
    }

        private void stopBtn_Click(object sender, EventArgs e)
        {
             evokWork.stop();        

        }

        private void stopBtnShow()
        {
             stbtn.Enabled = true;
             loadDataBtn.Enabled = true;
             optBtn.Enabled = true;
             button10.Enabled = true;
             qClr.Enabled = true;
             autoSLBtn.Enabled = true;
             ccBtn.Enabled = true;
             UserData.ReadOnly = false;
             printcb.Enabled = true;
            设备ToolStripMenuItem.Enabled = true;

        }
        public void showWatchForm()
        {
            if (wForm!=null && wForm.Visible)
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
            if(evokWork!=null)
            evokWork.SaveFile();
        }

        private void UpdataAuto()
        {
            if ( tc1.SelectedIndex == 0)
            {                               
                foreach (PlcInfoSimple simple in  evokWork.PsLstAuto)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }

        private void UpdataError()
        {
            
            if ( evokWork.DeviceStatus)
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
                foreach(PlcInfoSimple p in evokWork.PsLstAuto)
                {
                    if (p.Name.Contains(Constant.Alarm)&& p.ShowStr != null && p.ShowStr.Count > 0)
                    {
                        
                        for (int i = 0; i < p.ShowStr.Count; i++)
                        {
                            int index = errorList.IndexOf(p.ShowStr[i]);
                            if (p.ShowValue == Constant.M_ON && index <0)
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
            if ( tc1.SelectedIndex == 1)
            {
                foreach (PlcInfoSimple simple in  evokWork.PsLstHand)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }

        private void UpdataParam()
        {
            if (tc1.SelectedIndex == Constant.ParamPage)
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
                int valueId = 0;
                foreach (DataGridViewRow dr in dgvIO.Rows)
                {
                    if(dr.Cells["value0"].Value !=null)
                    if (int.TryParse(dr.Cells["value0"].Value.ToString(),out valueId))
                    {
                        if (valueId == Constant.M_ON)
                        {
                            dr.DefaultCellStyle.BackColor = Color.Red;
                        }
                        else
                        {
                            dr.DefaultCellStyle.BackColor =dgvIO.RowsDefaultCellStyle.ForeColor;
                        }
                    }                 
                }
            }
        }
        #endregion
        private void UserData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
           //  optSize.DtData.Rows[e.RowIndex][e.ColumnIndex] =  UserData.SelectedCells[0].Value;
        }

        private void UserData_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            // UserData.EndEdit();
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
                wForm.Show();
            }
        }

        private void errorTimer_Tick(object sender, EventArgs e)
        {
            if (errorList.Count == 0 && alarmForm != null && alarmForm.Visible)
            {
                alarmForm.Close();
            }

            if (errorList.Count > 0 && (alarmForm == null || !alarmForm.Visible))
            {

                alarmForm = new Alarm();

                alarmForm.Show();
            }

            //报警显示
            if (alarmForm != null && alarmForm.Visible)
            {
                alarmForm.showAlram(errorList);
            }
            /****
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
            ***/
        }

        private void printBarCodeBtn_Click(object sender, EventArgs e)
        {
            if (evokWork.IsPrintBarCode)
            {
                evokWork.plcHandleBarCodeOFF();
            }
            else
            {
                evokWork.plcHandleBarCodeON();
            }
        }

        private void BtnM101_MouseUp(object sender, MouseEventArgs e)
        {
            evokWork.SetMPsOff(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
        }

        private void printcb_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (evokWork.DeviceStatus)
            {
                evokWork.ChangePrintMode(printcb.SelectedIndex);
            }
            
            optBtn.Focus();
        }

        private void tabPage4_Enter(object sender, EventArgs e)
        {
            dgvIO.ClearSelection();
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            dgvParam.ClearSelection();
        }

        private void button10_Click(object sender, EventArgs e)
        {
            if (UserData.RowCount>0 
                && UserData.CurrentRow.Index > -1  
                && UserData.CurrentRow !=null)
                evokWork.ShowBarCode(UserData.CurrentRow.Index);
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

        private void ScrollTimer_Tick(object sender, EventArgs e)
        {

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

        private void button1_Click(object sender, EventArgs e)
        {
            evokWork.emgStop();
        }

        private void textBox16_TextChanged(object sender, EventArgs e)
        {

        }
       
        private void handTc_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (handTc.SelectedIndex == 0)
            {
                if (!evokWork.ShiftPage(Constant.HandPage))
                {
                    e.Cancel = true;                   
                }
            }
            else
            if (!evokWork.ShiftPage(3+handTc.SelectedIndex))
            {
                e.Cancel = true;
              
            }
            showWatchForm();
        }

        private void label28_Click(object sender, EventArgs e)
        {

        }

        private void button53_Click(object sender, EventArgs e)
        {
            HandOff2On_Click(sender,e);
            HandOff_Click(sender, e);


        }

        private void 查看报警信息ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            /***
            if (errorList.Count == 0)
            {
                MessageBox.Show("无报警信息！");
            }
            if (errorList.Count > 0)
            {

                alarmForm = new Alarm();

                alarmForm.Show();
            }
            ***/
            
        }
        #region 手动页面加密
        private void 手动页面参数设置ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                if (tc1.SelectedIndex != Constant.HandPage)
                {
                    MessageBox.Show("请先打开手动页面！");
                    return;
                }

                if (ConstantMethod.UserPassWd(Constant.PwdNoOffSet))
                {
                    showHandParam();
                }
            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void showHandParam()
        {
            g1Param.Visible = true;
            g2Param.Visible = true;
            g3Param.Visible = true;
            g4Param.Visible = true;
            g5Param.Visible = true;
            g6Param.Visible = true;
            g7Param.Visible = true;
            g8Param.Visible = true;
            g9Param.Visible = true;
            g11Param.Visible = true;
            g12Param.Visible = true;
            g13Param.Visible = true;
            g14Param.Visible = true;
            g15Param.Visible = true;
            g16Param.Visible = true;
            g17Param.Visible = true;
            g18Param.Visible = true;


        }
        public void hideHandParam()
        {
            g1Param.Visible = false;
            g2Param.Visible = false;
            g3Param.Visible = false;
            g4Param.Visible = false;
            g5Param.Visible = false;
            g6Param.Visible = false;
            g7Param.Visible = false;
            g8Param.Visible = false;
            g9Param.Visible = false;
            g11Param.Visible = false;
            g12Param.Visible = false;
            g13Param.Visible = false;
            g14Param.Visible = false;
            g15Param.Visible = false;
            g16Param.Visible = false;
            g17Param.Visible = false;
            g18Param.Visible = false;
        }
        #endregion

        private void tabPage5_Leave(object sender, EventArgs e)
        {
          
        }

        private void tabPage2_Leave(object sender, EventArgs e)
        {
            hideHandParam();
        }

        private void tabPage2_Enter(object sender, EventArgs e)
        {
            handTc.SelectedIndex = 0;
        }

        private void label114_Click(object sender, EventArgs e)
        {

        }
    }
}

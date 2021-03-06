using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using System.IO;
using xjplc;

namespace evokNew0076
{
    public partial class WorkForm : Form
    {
        private static Queue<Control> allCtrls = new Queue<Control>();

        List<string> errorList = new List<string>();
        int errorId = 0;
    
        private EvokXJWork evokWork;
        private OptSize optSize;


        private WatchForm wForm;

        public WorkForm()
        {        
           //ConstantMethod.InitPassWd();
            InitializeComponent();
        }

        private void autoSLBtn_Click(object sender, EventArgs e)
        {
             //evokWork.a;
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
            {
                optSize.Len = 244000;//evokWork.lcOutInPs.ShowValue;
                optSize.Dbc = 0;//evokWork.dbcOutInPs.ShowValue;
                optSize.Ltbc = 0;/// evokWork.ltbcOutInPs.ShowValue;
                optSize.Safe = 0;// evokWork.safeOutInPs.ShowValue;
                optSize.LoadCsvData(DialogExcelDataLoad.FileName);
            } 
            ****/                     
            loadDataBtn.Enabled = false;
            ReadCSVData0();
            loadDataBtn.Enabled = true;
           
        }

        private void ccBtn_Click(object sender, EventArgs e)
        {
            if ( evokWork.AutoMes)
            {
                 evokWork.autoMesOFF();
            }
            else
            {
                 evokWork.autoMesON();
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
            int rowIndex =  dgvParam.SelectedCells[0].RowIndex;
            evokWork.DgvInOutEdit(rowIndex,true);            
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

        private PlcInfoSimple getPsFromPslLst(string tag0, string str0, List<PlcInfoSimple> pslLst)
        {
            foreach (PlcInfoSimple simple in pslLst)
            {
                if (simple.Name.ToString().Contains(tag0) && simple.Name.Contains(str0))
                {
                    return simple;
                }
            }
            return null;
        }

        private void HandOff2On_Click(object sender, EventArgs e)
        {
            evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(),Constant.Write,evokWork.PsLstHand);
        }

        private void InitControl()
        {        
            SetControlInEvokWork();
                      
            evokWork.ChangePrintMode(printcb.SelectedIndex);

        }

        public void InitParam()
        {
            //datasource 改变会出发 selectindex 改变事件  这样就会打条码导致 模式被自动修改
            //所以早点设置好 然后在 那个selectindexchanged事件里增加 通讯正常判断
            printcb.DataSource = Constant.printBarcodeModeStr;

            LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);
                        
             optSize = new OptSize(UserData);
             evokWork = new EvokXJWork();
             evokWork.SetOptSize(optSize);
             evokWork.SetRtbWork(rtbWork);
             evokWork.SetRtbResult(rtbResult);
             evokWork.SetPrintReport();
             evokWork.InitDgvParam(dgvParam);
             evokWork.InitDgvIO(dgvIO);
             UpdateTimer.Enabled = true;
        }

        private void InitView0()
        {         
            DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DialogExcelDataLoad.Filter = "文件(*.xls,*.xlsx,*.csv)|*.xls;*.csv;*.xlsx";
            DialogExcelDataLoad.FileName = "请选择数据文件";

            logOPF.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory+"Log";
            logOPF.Filter = "文件(*.log)|*.log";
            logOPF.FileName = "请选择日志文件";


            printcb.SelectedIndex = evokWork.PrintBarCodeMode;

            errorTimer.Enabled = true;

        }

        private void IsoptBtnShow(bool showvalue)
        {
            if (showvalue ||  evokWork.RunFlag)
            {
                 optBtn.Enabled = false;
            }
            else if (! evokWork.RunFlag)
            {
                 optBtn.Enabled = true;
            }
        }

        private void lcTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(evokWork.AutoParamTxt_KeyPress(sender,e))
            tc1.Focus();
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
            if ( evokWork != null)
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
            /****单机启动
            optSize.Len = 24400;//evokWork.lcOutInPs.ShowValue;
            optSize.Dbc = 0;//evokWork.dbcOutInPs.ShowValue;
            optSize.Ltbc = 0;/// evokWork.ltbcOutInPs.ShowValue;
            optSize.Safe = 0;// evokWork.safeOutInPs.ShowValue;
            ***/

            /***
           // if (!evokWork.AutoMes)
           // {
                ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult));
           // }
           ****/
           //现场改回测长 20180805 无锡欧派
            if (!evokWork.AutoMes)
            {
                optSize.Len = evokWork.lcOutInPs.ShowValue;
                optSize.Dbc = evokWork.dbcOutInPs.ShowValue;
                optSize.Ltbc = evokWork.ltbcOutInPs.ShowValue;
                optSize.Safe = evokWork.safeOutInPs.ShowValue;

                ConstantMethod.ShowInfo(rtbResult, optSize.OptNormal(rtbResult));          
            }

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
        private int ReadCSVData0()
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
        private int ReadCSVData()
        {
            if ( DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {
                int num = ConstantMethod.IsWhichFile( DialogExcelDataLoad.FileName);
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
                    if ((control.Parent ==  tabPage1) || (control.Parent ==  groupBox1))
                    {
                        foreach (PlcInfoSimple simple in  evokWork.PsLstAuto)
                        {
                            if (simple.Name.Contains(control.Tag.ToString()) && simple.Name.Contains(Constant.Read))
                            {
                                simple.ShowControl = control;
                                break;
                            }
                        }
                    }
                    if (control.Parent ==  tabPage2)
                    {
                        foreach (PlcInfoSimple simple2 in  evokWork.PsLstHand)
                        {
                            if (simple2.Name.Contains(control.Tag.ToString()) && simple2.Name.Contains(Constant.Read))
                            {
                                simple2.ShowControl = control;
                                break;
                            }
                        }
                    }
                    if (control.Parent ==  tabPage3)
                    {
                        foreach (PlcInfoSimple simple3 in  evokWork.PsLstParam)
                        {
                            if ((simple3.Name.Contains(control.Tag.ToString()) && simple3.Name.Contains(Constant.Read)) && (control.Parent ==  tabPage3))
                            {
                                simple3.ShowControl = control;
                                break;
                            }
                        }
                    }
                    if (control.Parent == tabPage4)
                    {
                        foreach (PlcInfoSimple simple4 in evokWork.PsLstIO)
                        {
                            if ((simple4.Name.Contains(control.Tag.ToString()) && simple4.Name.Contains(Constant.Read)) && (control.Parent == tabPage4))
                            {
                                simple4.ShowControl = control;
                                break;
                            }
                        }
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
            UserData.ReadOnly = true;
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
            UserData.ReadOnly = true;
            stopBtn.Enabled = true;
            pauseBtn.Enabled = true;
            resetBtn.Enabled = true;
            printcb.Enabled = true;
            设备ToolStripMenuItem.Enabled = true;

        }

        private void stbtn_Click(object sender, EventArgs e)
        {
            startBtnShow();
            if (evokWork.AutoMes)
            {
                MessageBox.Show("请运行测长版本！");
                // evokWork.CutStartMeasure(Constant.CutMeasureMode);
                //测试代码 后续回复弹窗
                /***
                qClr_Click(sender, e);
                stbtn_Click(sender, e);
              **/
            }
            else
            {
                evokWork.CutStartNormal(Constant.CutNormalMode);
                //测试代码 后续回复弹窗
                /***
                qClr_Click(sender, e);
                optBtn_Click(sender, e);
                stbtn_Click(sender, e);
                ***/
            }
            //测试代码 后续回复弹窗
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
             evokWork.SaveFile();
        }

        private void UpdataAuto()
        {
            if ( tc1.SelectedIndex == 0)
            {    
                //现场 改回 20180805 无锡欧派                          
                IsoptBtnShow(evokWork.AutoMes);
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
             optSize.DtData.Rows[e.RowIndex][e.ColumnIndex] =  UserData.SelectedCells[0].Value;
        }

        private void UserData_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
             UserData.EndEdit();
        }

        private void UserData_DataError(object sender, DataGridViewDataErrorEventArgs e)
        {
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
            //report1.Show();
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

        private void button6_Click(object sender, EventArgs e)
        {
            evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);
        }

    }
}

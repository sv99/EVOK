using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using xjplc;
using System.IO;

namespace evokNew0066
{
    public partial class WorkForm : Form
    {
        //存储控件
        private static Queue<Control> allCtrls = new Queue<Control>();

        //错误信息
        List<string> errorList = new List<string>();


        List<string> warnList = new List<string>();
        //错误带有多种语言的时候
        int errorId = 0;
        int warnId = 0;
        private EvokXJWork evokWork;

        private WatchForm wForm;

        public WorkForm()
        {     
            //需要密码的时候     
          //ConstantMethod.InitPassWd();
            InitializeComponent();
        }
        #region 初始化
        public void InitLang()
        {
        
            //设置提醒字符串
            Constant.InitStr(this);
            evokWork.ShiftDgvParamLang(dgvParam, MultiLanguage.getLangId());
            evokWork.updateColName(dgvParam);
            evokWork.updateColName(dgvIO);
            //一些控件的库需要更换
            string[] s = Constant.cutMode.Split('/');
            comboBox1.Items.Clear();
            comboBox1.Items.AddRange(s);
            comboBox1.SelectedIndex = evokWork.CutSelMode;
            s = Constant.printMode.Split('/');
            printcb.Items.Clear();
            printcb.Items.AddRange(s);
            printcb.SelectedIndex = evokWork.PrintBarCodeMode;

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
            evokWork.ListSmallSizePrinter(comboBox3);
            this.Visible = true;
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
            // printcb.DataSource = Constant.printBarcodeModeStr;
            printcb.Items.AddRange(Constant.printBarcodeModeStr);
            LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);
            evokWork = ConstantMethod.GetWork();
            evokWork.MainForm = this;
            evokWork.SetUserDataGridView(UserData);
            evokWork.SetRtbWork(rtbWork);
            if (evokWork.IsArrangeOpt)
            {
                optchk.Checked = true; 
            }                     
            evokWork.SetRtbResult(rtbResult);
            evokWork.SetPrintReport();
            evokWork.InitDgvParam(dgvParam);
            evokWork.InitDgvIO(dgvIO);    
            evokWork.SetShowCnt(comboBox2);

            errorList = evokWork.ErrorList;

            warnList = evokWork.WarningList;

            UpdateTimer.Enabled = true;

        }
        private void InitView0()
        {
            DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DialogExcelDataLoad.Filter = Constant.fileFilter;
            DialogExcelDataLoad.FileName = Constant.openFileTips;

            logOPF.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory + "Log";
            logOPF.Filter = Constant.fileLogFilter;
            logOPF.FileName = Constant.selectLogFileTips;
            comboBox1.SelectedIndex = evokWork.CutSelMode;

            errorTimer.Enabled = true;

            evokWork.ReadCSVDataDefault();
                    
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

            ReadCSVData();
         
            //ReadSimiData();

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
        private void optBtn_Click(object sender, EventArgs e)
        {
            optBtn.Enabled = false;
            optBtn.BackColor = Color.Red;
            rtbResult.Clear();
            rtbWork.Clear();
            startOptShow();
            if (optchk.Checked)
            {
                evokWork.optReady(Constant.optNo);
            }else
            evokWork.optReady(Constant.optNormal);
             
            stopOptShow();
            optBtn.BackColor = Color.Transparent;
            optBtn.Enabled = true;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
            evokWork.pause();

        }
        private void HandOff2On_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
            }
        }
        private void Off2On_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(), Constant.Write,tc1.SelectedIndex);
            }
        }
        private void qClr_Click(object sender, EventArgs e)
        {
            evokWork.ProClr();
        }
        private void resetBtn_Click(object sender, EventArgs e)
        {
            evokWork.reset();
            stopBtnShow();
        }
        private void stbtn_Click(object sender, EventArgs e)
        {

            startBtnShow();

            if (evokWork.AutoMes)
            {    //测长模式下有测长和带结巴模式 当结巴部分有数据时自动为结巴模式
                 //结巴有三种模式 尺寸切了后 后一刀切到结巴
                 //尺寸切了后 结巴尾料分离
                 //无尺寸就去除结疤
                 //优化系数是针对索菲亚的 只有在测长的时候会进行保存和设置 20181012

                 /****
                int value0 = 0;
                if (int.TryParse(comboBox2.Text, out value0))
                {
                    evokWork.SetOptSizeParam1(value0);
                }
                ****/

                switch (comboBox1.SelectedIndex)
                {

                    case Constant.SizeScarSplit:
                        {
                            //尺寸切了后 结巴尾料分离
                            evokWork.CutStartMeasure(true, Constant.CutMeasureMode);
                            break;
                        }
                    case Constant.SizeScarNoSplit: 
                        {
                            //尺寸切了后 结巴尾料不分离
                            evokWork.CutStartMeasure(false, Constant.CutMeasureMode);
                            break;
                        }
                    case Constant.ScarSplit:
                        {
                            //无尺寸 就去除结疤
                            evokWork.CutStartMeasure(true, Constant.CutMeasureWithScarSplitNoSize);
                            break;
                        }

                }

                //测试代码 后续回复弹窗
                /**
                qClr_Click(sender, e);
                stbtn_Click(sender, e);
              **/
            }
            else
            {
                if(evokWork.SetCutProCnt(int.Parse(comboBox2.Text)))
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

        private void button1_Click(object sender, EventArgs e)
        {
            evokWork.emgStop();
            stopBtnShow();
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
               IsoptBtnShow(evokWork.AutoMes);
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
                    
            evokWork.errorListUpdate(tc1.SelectedIndex);             
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
        }
        private void UpdataIO()
        {
            if (tc1.SelectedIndex == 3)
            {
                int valueId = 0;
                foreach (DataGridViewRow dr in dgvIO.Rows)
                {
                    if (dr.Cells["value0"].Value != null)
                        if (int.TryParse(dr.Cells["value0"].Value.ToString(), out valueId))
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
        }
        #endregion
        #region 参数表格更新
        private void UserData_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //  optSize.DtData.Rows[e.RowIndex][e.ColumnIndex] =  UserData.SelectedCells[0].Value;
        }

        private void UserData_CellLeave(object sender, DataGridViewCellEventArgs e)
        {
            // UserData.EndEdit();
        }

        #endregion

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
            ConstantMethod.Delay(100);
           
            Environment.Exit(0);
        }
       
        private int ReadCSVData()
        {
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {

                if (evokWork.showFilePathLabel == null) evokWork.showFilePathLabel = label8;
                evokWork.SetDataShowCb(listBox1);
               // evokWork.SetDataShowLbl(label9);
                ConstantMethod.SaveDirectoryByFileDialog(DialogExcelDataLoad);
                int num = ConstantMethod.IsWhichFile( DialogExcelDataLoad.FileName);
                if (num == Constant.CsvFile)
                {
                     evokWork.LoadCsvData(DialogExcelDataLoad.FileName);
                }
                if (num == Constant.ExcelFile)
                {
                     evokWork.LoadExcelData(DialogExcelDataLoad.FileName);
                }
                //最多加十次
                evokWork.ShenAo(10);



            }

            return 0;
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

                    if ((control.Parent != null && control.Parent == tabPage2)
                        || (control.Parent.Parent != null && control.Parent.Parent == tabPage2)
                        || (control.Parent.Parent.Parent != null && control.Parent.Parent.Parent == tabPage2)
                        || (control.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent == tabPage2)
                        || (control.Parent.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent.Parent == tabPage2))
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
                    if (control.Parent == tabPage4)
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
            deviceMenuItem.Enabled = false;
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

            deviceMenuItem.Enabled = false;

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
            deviceMenuItem.Enabled = true;

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
            deviceMenuItem.Enabled = true;

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
            int id = MultiLanguage.getLangId();

            if (errorList.Count > 0)
            {
                if (errorId >= errorList.Count)
                {
                    errorId = 0;
                }              
               string[] splitError = errorList[errorId].Split('/');
                if (splitError.Length > 1)
                {
                     infoLbl.Text = splitError[id];
                    LogManager.WriteProgramLog(infoLbl.Text);
                }
                else
                {
                    infoLbl.Text = errorList[errorId];
                    LogManager.WriteProgramLog(infoLbl.Text);
                }

                errorId++;
            }
            else infoLbl.Text = "";
          
            if (warnList.Count > 0)
            {
                if (warnId >= warnList.Count)
                {
                    warnId = 0;
                }
                string[] splitError = warnList[warnId].Split('/');
                if (splitError.Length > 1)
                {
                    warningLabel.Text = splitError[id];
                    LogManager.WriteProgramLog(warningLabel.Text);
                }
                else
                {
                    warningLabel.Text = warnList[warnId];
                    LogManager.WriteProgramLog(warningLabel.Text);
                }

                warnId++;
            }
            else warningLabel.Text = "";

           
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
                          
        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
            if(tc1.SelectedIndex ==2)
            evokWork.SetSmallSizePrinter(comboBox3.SelectedItem.ToString());
        }

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            evokWork.ChangeCutMode(comboBox1.SelectedIndex);
        }
    
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            evokWork.SetDataShow(listBox1.SelectedIndex);
        }

        private void listBox1_DrawItem(object sender, DrawItemEventArgs e)
        {
            
        }

        private void button12_Click(object sender, EventArgs e)
        {
            optBtn.Enabled = false;
            optBtn.BackColor = Color.Red;
            rtbResult.Clear();
            rtbWork.Clear();
            startOptShow();

            evokWork.optReady(Constant.optNo);

            stopOptShow();
            optBtn.BackColor = Color.Transparent;
            optBtn.Enabled = true;
        }

        private void optchk_CheckedChanged(object sender, EventArgs e)
        {
            if (optchk.Checked)
            {
                evokWork.SetIsArrangeOpt(true);
            }
            else
            {
                evokWork.SetIsArrangeOpt(false);
            }
        }
    }   
}

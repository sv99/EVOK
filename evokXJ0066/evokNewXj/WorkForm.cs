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
        private static Queue<Control> allCtrls = new Queue<Control>();

        List<string> errorList = new List<string>();
        int errorId = 0;
        private EvokXJDevice evokDevice;
        private EvokXJWork evokWork;
        private OptSize optSize;

        private List<string> strDataFormPath;

        private WatchForm wForm;

        public WorkForm()
        {
             InitializeComponent();
        }

        private void autoSLBtn_Click(object sender, EventArgs e)
        {
             evokWork.autoSL();
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
             loadDataBtn.Enabled = false;
             ReadCSVData();
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

        private void ClrData()
        {
            if (( optSize.DtData != null) && ( optSize.DtData.Rows.Count > 0))
            {
                foreach (DataRow row in  optSize.DtData.Rows)
                {
                    row["已切数量"] = 0;
                }
            }
        }

        private void connectMachine_Click(object sender, EventArgs e)
        {
             tc1.SelectedIndex = 0;
             UpdateTimer.Enabled = false;
             evokDevice.RestartConneect( evokDevice.DataFormLst[0]);
            if ( evokDevice.getDeviceData())
            {
                 UpdateTimer.Enabled = true;
            }
            else
            {
                MessageBox.Show(Constant.ConnectMachineFail);
            }
            GC.Collect();
            GC.WaitForPendingFinalizers();
        }

        private void dgvParam_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {
            int rowIndex =  dgvParam.SelectedCells[0].RowIndex;
            evokWork.DgvInOutEdit(rowIndex,true);            
        }

        private void dgvParam_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            int num3;
            string s =  dgvParam.SelectedCells[0].Value.ToString();

            int rowIndex = dgvParam.SelectedCells[0].RowIndex;
            try
            {
                if (int.TryParse(s, out num3))
                {
                    evokWork.DgvValueEdit(rowIndex, num3);
                }
            }
            catch { }
            finally{ evokWork.DgvInOutEdit(rowIndex, false); }
            
           
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
             InitParam();
             InitControl();
             InitView0();
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
            evokWork.InitControl();
            evokWork.ShiftPage(0);
            SetControlInEvokWork();
        }


        public void InitParam()
        {
            optSize = new OptSize( UserData);
            strDataFormPath = new List<string>();
            if (File.Exists(Constant.PlcDataFilePath))
            {
                 strDataFormPath.Add(Constant.PlcDataFilePath);
            }
            else
            {
                MessageBox.Show(Constant.ErrorPlcFile);
                Environment.Exit(0);
            }
            if (File.Exists(Constant.PlcDataFilePath0))
            {
                 strDataFormPath.Add(Constant.PlcDataFilePath0);
            }
            else
            {
                MessageBox.Show(Constant.ErrorPlcFile);
                Environment.Exit(0);
            }
            if (File.Exists(Constant.PlcDataFilePath1))
            {
                 strDataFormPath.Add(Constant.PlcDataFilePath1);
            }
            else
            {
                MessageBox.Show(Constant.ErrorPlcFile);
                Environment.Exit(0);
            }
            evokDevice = new EvokXJDevice( strDataFormPath);
            if (! evokDevice.getDeviceData())
            {
                MessageBox.Show(Constant.ConnectMachineFail);
                Environment.Exit(0);
            }
             UpdateTimer.Enabled = true;
             optSize = new OptSize( UserData);
             evokWork = new EvokXJWork();
             evokWork.SetEvokDevice( evokDevice);
             evokWork.SetOptSize( optSize);
             evokWork.SetRtbWork( rtbWork);
             evokWork.SetRtbResult( rtbResult);
             evokWork.SetPrintReport(report1);

        }

        private void InitView0()
        {
             evokWork.InitDgv(dgvParam);
             DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
             DialogExcelDataLoad.Filter = "文件(*.xls,*.xlsx,*.csv)|*.xls;*.csv;*.xlsx";
             DialogExcelDataLoad.FileName = "请选择数据文件";
             wForm = new WatchForm();
             wForm.Visible = false;
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
            if (e.KeyChar == '\r')
            {
                int num;
                if (int.TryParse(((TextBox)sender).Text, out num) && num >-1)
                {
                    evokWork.SetDValue(((TextBox)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto,num);
                }                            
                optBtn.Focus();
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
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
             rtbResult.Clear();
             rtbWork.Clear();
             optSize.Len =  evokWork.lcOutInPs.ShowValue;
             optSize.Dbc =  evokWork.dbcOutInPs.ShowValue;
             optSize.Ltbc =  evokWork.ltbcOutInPs.ShowValue;
             optSize.Safe =  evokWork.safeOutInPs.ShowValue;
            if (! evokWork.AutoMes)
            {
                ConstantMethod.ShowInfo( rtbResult,  optSize.OptNormal( rtbResult));
            }
             optBtn.Enabled = true;
        }

        private void pauseBtn_Click(object sender, EventArgs e)
        {
             evokWork.pause();
        }

        private void qClr_Click(object sender, EventArgs e)
        {
             optSize.prodClear();
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

        public void SetControlInEvokWork()
        {
            CheckAllCtrls(this);
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
        }

        private void stbtn_Click(object sender, EventArgs e)
        {
             startBtnShow();
            if ( evokWork.AutoMes)
            {
                 evokWork.CutStartMeasure();
            }
            else
            {
                 evokWork.CutStartNormal();
            }
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
        }

        private void tc1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if ( evokWork.RunFlag)
            {
                MessageBox.Show(Constant.IsWorking);
                e.Cancel = true;
            }
            else if (! evokWork.ShiftPage( tc1.SelectedIndex))
            {
                e.Cancel = true;
                MessageBox.Show(Constant.ConnectMachineFail);
            }
        }

        private void UpdateTimer_Tick(object sender, EventArgs e)
        {
             UpdataError();
             UpdataAuto();
             UpdataHand();
             UpdataParam();
        }


        private void FileSave_Tick(object sender, EventArgs e)
        {
             evokWork.SaveFile();
        }

        private void UpdataAuto()
        {
            if ( tc1.SelectedIndex == 0)
            {
                               
                IsoptBtnShow( evokWork.AutoMes);
                foreach (PlcInfoSimple simple in  evokWork.PsLstAuto)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }

        private void UpdataError()
        {
            if ( evokDevice.Status != -1)
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
                            if (p.ShowValue == Constant.M_OFF && index > 0 && index <p.ShowStr.Count)
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
             wForm.SetShowDataTable(evokDevice.DataFormLst[ tc1.SelectedIndex]);
             wForm.ShowDialog();
        }

        private void errorTimer_Tick(object sender, EventArgs e)
        {
            if (errorList.Count >0)
            {
                infolbl.Text = errorList[errorId];
                errorId++;
                if (errorId >= errorList.Count)
                {
                    errorId = 0;
                }
            }
        }

        private void printBarCodeBtn_Click(object sender, EventArgs e)
        {
            if (evokWork.IsPrintBarCode)
            {
                evokWork.printBarCodeOFF();
            }
            else
            {
                evokWork.printBarCodeON();
            }
        }

        private void BtnM101_MouseUp(object sender, MouseEventArgs e)
        {
            evokWork.SetMPsOff(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
        }
    }
}

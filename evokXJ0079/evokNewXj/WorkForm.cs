using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using xjplc;
using System.IO;
using evokNewXJ;

namespace evokNew0079
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

        HySkDevice hySk ;

        public WorkForm()
        {     
            //需要密码的时候     
          //ConstantMethod.InitPassWd();
            InitializeComponent();
        }
        #region 初始化
        private void Form1_Load(object sender, EventArgs e)
        {
            this.Visible = false;
            Constant.InitStr(this);
            InitParam();
            InitControl();
            InitView0();
            hySk = new HySkDevice();
            hySk.ShowUserData(dataGridView1);
            hySk.DownLoadData = evokWork.Sdj_DownLoadData;
            Application.DoEvents();         
            this.Visible = true;
        }
        private void InitControl()
        {
            SetControlInEvokWork();           
        }
        public void InitParam()
        {
            //datasource 改变会出发 selectindex 改变事件  这样就会打条码导致 模式被自动修改
            //所以早点设置好 然后在 那个selectindexchanged事件里增加 通讯正常判断
            // printcb.DataSource = Constant.printBarcodeModeStr;
          
            LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);
            evokWork = new EvokXJWork();
            //初始化双端锯
            InitDoubleSaw();
           
      
                  
                       
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
            errorTimer.Enabled = true;          

        }
        #endregion
        #region 按钮和数据输入事件
        private void lcTxt_KeyPress0(object sender, KeyPressEventArgs e)
        {
            if (evokWork.AutoParamTxt_KeyPressWithRatioWithId(sender, e,tc1.SelectedIndex))
                tc1.Focus();
        }
        private void lcTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork.AutoParamTxt_KeyPress(sender, e))
                resetBtn.Focus();
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
            if(((Control)sender).Tag !=null)
            evokWork.SetMPsOn(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
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
        private void Off2On_Click(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
            }
        }

        private void BtnM101_MouseUp(object sender, MouseEventArgs e)
        {
            evokWork.SetMPsOff(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
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
                foreach (PlcInfoSimple simple in evokWork.PsLstIO)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }
        #endregion
        
        #region 通过式双端锯
        public void InitDoubleSaw()
        {
            evokWork.DeviceName = Constant.sdjDeivceName;
            evokWork.DeviceProperty = Constant.sdjDeivceId;
            addDataTable();
        }
        public void addDataTable()
        {
            List<string> strLst = new List<string>();
            string handFile = "";
            for (int i = 0; i < Constant.constantParamDoubleSawName.Length; i++)
            {
                try
                {
                    handFile = "";
                    handFile = Constant.PlcDataFilePathParamDoubleSaw + Constant.constantParamDoubleSawName[i] + ".csv";
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
            evokWork.addDataForm(strLst,2);
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

            if (evokWork != null)
            {
                evokWork.Dispose();
            }





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
                    if ((control.Parent != null && control.Parent == tabPage3)
                        || (control.Parent.Parent != null && control.Parent.Parent == tabPage3)
                        || (control.Parent.Parent.Parent != null && control.Parent.Parent.Parent == tabPage3)
                        || (control.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent == tabPage3)
                        || (control.Parent.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent.Parent == tabPage3))
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
        public void showWatchForm()
        {
            if (wForm != null && wForm.Visible)
            {
                wForm.SetShowDataTable(evokWork.GetDataForm(tc1.SelectedIndex));
            }
        }
        private void tc1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (evokWork.RunFlag)
            {
                MessageBox.Show(Constant.IsWorking);
                e.Cancel = true;
            }
            else
            if (tc1.SelectedIndex == Constant.ParamPage)
            {
                evokWork.ShiftPage(tc1.SelectedIndex);
                
                if (paraTc.SelectedIndex == 0)
                {
                    if (!evokWork.ShiftPage(Constant.ParamPage))
                    {
                        e.Cancel = true;
                    }
                }
                else
                if (!evokWork.ShiftPage(3 + paraTc.SelectedIndex))
                {
                    e.Cancel = true;

                }
               
            }
            else
            if (!evokWork.ShiftPage(tc1.SelectedIndex))
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

        private void button5_Click(object sender, EventArgs e)
        {

        }

        private void oppositeClick(object sender, EventArgs e)
        {
            evokWork.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
        }

        private void label12_Click(object sender, EventArgs e)
        {

        }

        private void paraTc_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (paraTc.SelectedIndex == 0)
            {
                if (!evokWork.ShiftPage(Constant.ParamPage))
                {
                    e.Cancel = true;
                }
            }
            else
            if (!evokWork.ShiftPage(3 + paraTc.SelectedIndex))
            {
                e.Cancel = true;

            }
            showWatchForm();
        }

        private void button57_Click(object sender, EventArgs e)
        {
           
            button57.Enabled = false;
            
            if (hySk.DownLoadData != null)
            {

                if (!hySk.DataOK)
                {
                    DialogResult dr = MessageBox.Show("存在错误数据，是否下发？", "确认！",MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
                    if (dr == DialogResult.No)
                    {
                        button57.Enabled = true;
                        return;
                    }
                    
                }
                hySk.DownLoadData(hySk.HySkLst);

                MessageBox.Show("数据发送完毕");
            }
            button57.Enabled = true;
        }

        private void button57_Click_1(object sender, EventArgs e)
        {
            hySk.ReadData();
        }
    }   
}

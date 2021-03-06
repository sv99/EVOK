using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using xjplc;
using System.IO;
using System.Linq;
using System.Text;

namespace evokNew0053
{
    public partial class WorkForm : Form
    {
        private static Queue<Control> allCtrls = new Queue<Control>();

        List<string> errorList = new List<string>();
        int errorId = 0;

        private EvokDTTcpWork evokWork;

        ConfigFileManager configData;


        List<CheckBox> chekLst;
        List<TextBox> textLst;
        Dictionary<CheckBox, TextBox> txtChk;
        private WatchForm wForm;

        public WorkForm()
        {
           
          // ConstantMethod.InitPassWd();
            InitializeComponent();
        }

        public void chkProcess(CheckBox ch)
        {
            if(chekLst.Count>0&&ch.Checked)
            foreach (CheckBox chk in chekLst)
            {             
              if (!chk.Tag.ToString().Equals(ch.Tag.ToString())) chk.Checked = false;
            }
        }

        private void AutoTextBox_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Read, evokWork.PsLstAuto);           
        }

        private void AutoTxt_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Read, evokWork.PsLstAuto);           
        }

        private void BtnM101_MouseDown(object sender, MouseEventArgs e)
        {          
            evokWork.SetMPsOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
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
        public void InitXZJ()
        {
            if (Constant.scCutType.Count() == Constant.scCutTypeImage.Count())
            {

                imageLstSc.Images.Clear();

                foreach (string imageName in Constant.scCutTypeImage)
                {
                    imageLstSc.Images.Add(Image.FromFile(Constant.ConfigSource + imageName + ".png"));
                }
            }
            if (Constant.hyCutType.Count() == Constant.hyCutTypeImage.Count())
            {

                imageLstHy.Images.Clear();

                foreach (string imageName in Constant.hyCutTypeImage)
                {
                    imageLstHy.Images.Add(Image.FromFile(Constant.ConfigSource + imageName + ".png"));
                }
            }        
            
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
        private void AutoOff2On_Click(object sender, EventArgs e)
        {
            evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);
        }
        private void AutoOn2Off_Click(object sender, EventArgs e)
        {
            evokWork.SetMPsONToOFF(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstAuto);
        }
        private void Opossite_Click_AutoPage(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, Constant.AutoPage);
            }
        }
        private void Opossite_Click_Param1(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, Constant.Param1Page);
            }
        }

        private void Opossite_Click_HandPage(object sender, EventArgs e)
        {
            if (sender != null && ((Control)sender).Tag != null)
            {
                evokWork.oppositeBitClick(((Control)sender).Tag.ToString(), Constant.Write, Constant.HandPage);
            }
        }
        private void InitControl()
        {        
            SetControlInEvokWork();                  
        }

        public void ReadSize()
        {
            string size1 = configData.ReadConfig("size1");
            string size2 = configData.ReadConfig("size2");
            string size3 = configData.ReadConfig("size3");
            string size4 = configData.ReadConfig("size4");
            string size5 = configData.ReadConfig("size5");
            string size6 = configData.ReadConfig("size6");

            textBox18.Text = size1;
            textBox1.Text = size2;
            textBox3.Text = size3;
            textBox4.Text = size4;
            textBox5.Text = size5;
            textBox6.Text = size6;
        }
        public void SaveSize()
        {
            double i = 0;
            if (double.TryParse(textBox18.Text, out i))
                configData.WriteConfig("size1", textBox18.Text);

            if (double.TryParse(textBox1.Text, out i))
                configData.WriteConfig("size2", textBox1.Text);

            if (double.TryParse(textBox3.Text, out i))
                configData.WriteConfig("size3", textBox3.Text);

            if (double.TryParse(textBox4.Text, out i))
                configData.WriteConfig("size4", textBox4.Text);

            if (double.TryParse(textBox5.Text, out i))
                configData.WriteConfig("size5", textBox5.Text);

            if (double.TryParse(textBox6.Text, out i))
                configData.WriteConfig("size6", textBox6.Text);


        }
        public void InitParam()
        {
            //datasource 改变会出发 selectindex 改变事件  这样就会打条码导致 模式被自动修改
            //所以早点设置好 然后在 那个selectindexchanged事件里增加 通讯正常判断
            
             LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);

             evokWork = new EvokDTTcpWork(Constant.xzjDeivceId);     
             evokWork.SetPrintReport();
             evokWork.InitDgvParam(dgvParam);
             evokWork.InitDgvIO(dgvIO);
             UpdateTimer.Enabled = true;

            configData = ConstantMethod.configFileBak(Constant.ConfigParamFilePath);

            if (evokWork.DeviceId == Constant.xzjDeivceId)
            {
                ReadSize();
                txtChk = new Dictionary<CheckBox, TextBox>();
                chekLst = new List<CheckBox>();
                textLst = new List<TextBox>();
                chekLst.Add(checkBox1);
                chekLst.Add(checkBox2);
                chekLst.Add(checkBox3);
                chekLst.Add(checkBox4);
                chekLst.Add(checkBox5);
                chekLst.Add(checkBox6);

                txtChk.Add(checkBox1,textBox18);

                txtChk.Add(checkBox2, textBox1);

                txtChk.Add(checkBox3, textBox3);
                txtChk.Add(checkBox4, textBox4);
                txtChk.Add(checkBox5, textBox5);
                txtChk.Add(checkBox6, textBox6);

                //InitXZJ();
            }
        }

        private void InitView0()
        {        
             
            DialogExcelDataLoad.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
            DialogExcelDataLoad.Filter = "文件(*.xls,*.xlsx,*.csv)|*.xls;*.csv;*.xlsx";
            DialogExcelDataLoad.FileName = "请选择数据文件";

            logOPF.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory+"Log";
            logOPF.Filter = "文件(*.log)|*.log";
            logOPF.FileName = "请选择日志文件";
           
            errorTimer.Enabled = true;         

        }
        
        private void lcTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if(evokWork.KeyPress_AutoPage(sender,e))
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

        
       


        private int ReadCSVData()
        {
            if (DialogExcelDataLoad.ShowDialog() == DialogResult.OK)
            {
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
            }
            return 0;
        }
              

       
        /// <summary>
        /// 20181105 这里以后PSlst 最好用参数或者索引 进行收集 避免有hand auto等字样
        /// 控件tag 名称和plcsimple 结合起来
        /// plcsimple name只要包含 就可以和这个控件联合起来了 
        /// </summary>
        public void SetControlInEvokWork()
        {
            if (evokWork == null) return;
            ConstantMethod.
            CheckAllCtrls(this, allCtrls);
            foreach (Control control in allCtrls)
            {
                if (control.Tag != null)
                {
                    if ((control.Parent ==  tabPage1) || (control.Parent.Parent == tabPage1))
                    {
                                              
                        foreach (DTPlcInfoSimple simple in  evokWork.PsLstAuto)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple, control)) break;
                        }
                    }
                    if (control.Parent ==  tabPage2 || control.Parent.Parent == tabPage2)
                    {
                        foreach (DTPlcInfoSimple simple2 in evokWork.PsLstHand)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple2,control)) break;
                        }
                    }
                    if (control.Parent ==  tabPage3 || control.Parent.Parent == tabPage3)
                    {
                        foreach (DTPlcInfoSimple simple3 in  evokWork.PsLstParam)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple3, control)) break;
                        }
                    }
                    if (control.Parent == tabPage4|| control.Parent.Parent == tabPage3)
                    {
                        foreach (DTPlcInfoSimple simple4 in evokWork.PsLstIO)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple4, control)) break; 
                        }
                    }
                   
                }
            }
        }

        private void startBtnShow()
        {
             stbtn.Enabled = false;
            
            
             ccBtn.Enabled = false;
             
             
             设备ToolStripMenuItem.Enabled = false;
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
            SaveSize();
            // evokWork.SaveFile();
        }
        void upScHy(DataGridView dgv, int id)
        {
            try
            {
                if (dgv == null) return;
                for (int i = 0; i < dgv.Rows.Count; i++)
                {
                    DTPlcInfoSimple dtScLight = evokWork.getDtPlcSimple(id, Constant.scLight + (i + 1).ToString() + Constant.Read);
                    DTPlcInfoSimple dtScData = evokWork.getDtPlcSimple(id, Constant.scData + (i + 1).ToString() + Constant.Read);

                    if (dtScLight != null)
                    {
                        #region 锁槽工位显示
                        if (dtScLight.ShowValue == 1)
                        {
                            if (dgv.Rows[i].Cells[1].Style.BackColor != Color.Red)
                            {
                                dgv.Rows[i].Cells[1].Style.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            if (dgv.Rows[i].Cells[1].Style.BackColor != Color.LightGray)
                            {
                                dgv.Rows[i].Cells[1].Style.BackColor = Color.LightGray;

                            }
                        }
                    }

                    if (dtScData != null)
                    {
                        if (dtScData.ShowValue < Constant.schyStrLst.Length)
                        {

                            if (evokWork.DtScHyShow.Rows[i][1] != null && !evokWork.DtScHyShow.Rows[i][1].ToString().Equals(Constant.schyStrLst[dtScData.ShowValue]))
                            {
                                evokWork.DtScHyShow.Rows[i][1] = Constant.schyStrLst[dtScData.ShowValue];

                            }
                            else
                            {
                                if (evokWork.DtScHyShow.Rows[i][1] != null)
                                {
                                    evokWork.DtScHyShow.Rows[i][1] = Constant.schyStrLst[dtScData.ShowValue];
                                }
                            }
                        }
                    }

                    #endregion


                    #region 合页工位显示

                    DTPlcInfoSimple dtHyLight = evokWork.getDtPlcSimple(id, Constant.hyLight + (i + 1).ToString() + Constant.Read);
                    DTPlcInfoSimple dtHyData = evokWork.getDtPlcSimple(id, Constant.hyData + (i + 1).ToString() + Constant.Read);
                    if (dtHyLight != null)
                    {
                        if (dtHyLight.ShowValue == 1)
                        {
                            if (dgv.Rows[i].Cells[2].Style.BackColor != Color.Red)
                            {
                                dgv.Rows[i].Cells[2].Style.BackColor = Color.Red;
                            }
                        }
                        else
                        {
                            if (dgv.Rows[i].Cells[2].Style.BackColor != Color.LightGray)
                            {
                                dgv.Rows[i].Cells[2].Style.BackColor = Color.LightGray;
                            }
                        }
                    }
                    if (dtHyData != null)
                    {
                        if (dtHyData.ShowValue < Constant.schyStrLst.Length)
                        {
                            if (evokWork.DtScHyShow.Rows[i][2] != null && !evokWork.DtScHyShow.Rows[i][2].ToString().Equals(Constant.schyStrLst[dtHyData.ShowValue]))
                            {
                                evokWork.DtScHyShow.Rows[i][2] = Constant.schyStrLst[dtHyData.ShowValue];

                            }
                        }
                        else
                        {
                            if (evokWork.DtScHyShow.Rows[i][2] == null)
                            {
                                evokWork.DtScHyShow.Rows[i][2] = Constant.schyStrLst[dtHyData.ShowValue];

                            }
                        }
                    }

                    #endregion

                }
            }
            catch (Exception ex)
            {

            }
          
        }
        
        private void UpdataAuto()
        {
            if (tc1.SelectedIndex == Constant.AutoPage)
            {
                
                foreach (DTPlcInfoSimple simple in evokWork.PsLstAuto)
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
                foreach(DTPlcInfoSimple p in evokWork.PsLstAuto)
                {
                    if (p.Name.Contains(Constant.Alarm)&& p.ShowStr != null && p.ShowStr.Count > 0)
                    {
                        if (p.Addr == 1004)
                        {
                            p.Addr = 1004;
                        }
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
                foreach (DTPlcInfoSimple p in evokWork.PsLstHand)
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
            if (tc1.SelectedIndex == Constant.HandPage)
            {
                foreach (DTPlcInfoSimple simple in  evokWork.PsLstHand)
                {
                    int showValue = simple.ShowValue;
                }
            }
        }

        private void UpdataParam()
        {
            if (tc1.SelectedIndex == Constant.ParamPage)
            {
                foreach (DTPlcInfoSimple simple in evokWork.PsLstParam)
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
       
        private void stbtn_MouseDown_1(object sender, MouseEventArgs e)
        {
            evokWork.mouseDown(sender,e,Constant.AutoPage);
        }

        private void stbtn_MouseUp_1(object sender, MouseEventArgs e)
        {
            evokWork.mouseUp(sender, e, Constant.AutoPage);
        }

        private void button12_MouseDown(object sender, MouseEventArgs e)
        {
            evokWork.mouseDown(sender, e, Constant.HandPage);
        }

        private void button12_MouseUp(object sender, MouseEventArgs e)
        {
            evokWork.mouseUp(sender, e, Constant.HandPage);
        }

        private void textBox15_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork.KeyPress_Page(sender, e, tc1.SelectedIndex))
                ((TextBox)sender).Parent.Focus();
        }

        private void textBox15_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Read, evokWork.AllPlcSimpleLst[tc1.SelectedIndex]);
        }

        private void textBox15_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Read, evokWork.AllPlcSimpleLst[tc1.SelectedIndex]);
        }                

        private void button39_MouseDown(object sender, MouseEventArgs e)
        {
            evokWork.mouseDown(sender, e, Constant.Param1Page);
        }

        private void button39_MouseUp(object sender, MouseEventArgs e)
        {
            evokWork.mouseUp(sender, e, Constant.Param1Page);
        }

       

      
              

    
        

      

      

       

      

      
      private void button44_Click(object sender, EventArgs e)
        {

        }

       

       

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            
        }

       

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

      

       

        private void button12_Click(object sender, EventArgs e)
        {

            DialogResult dr = MessageBox.Show("是否继续执行操作？", "关闭提示", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
            if (dr == DialogResult.No)
            {
                return;
            }
            evokWork.SetMPsONToOFF(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
        }

        private void textBox31_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                string scanStr = textBox31.Text;
                string[] strSplit = scanStr.Split('/');

                double doorWidth = 0;
            
                double doorHeight = 0;

                double thickness = 0;

                int hymode = 0;
                int hyCount = 0;
                int scId = -1;
                int hyId = -1;
                if (! (strSplit.Count() == 10))
                {
                    MessageBox.Show("条码数据量错误！");
                    return;
                }
                if (!double.TryParse(strSplit[1], out doorHeight) 
                    || !double.TryParse(strSplit[2], out doorWidth)
                    || !double.TryParse(strSplit[4], out thickness))


                {
                    MessageBox.Show("门长/门宽/门厚数据错误！");
                    return;
                }

               
                if (!int.TryParse(strSplit[5], out scId)
                    || !int.TryParse(strSplit[8], out hyId)
                    || !int.TryParse(strSplit[9], out hyCount)
                    || !int.TryParse(strSplit[6], out hymode)
                    )
                {
                    MessageBox.Show("锁槽号/合页号/合页数/合页模式数据错误！");
                    return;
                }

                evokWork.
                setScanParam(strSplit[2], strSplit[1], strSplit[4], hymode, strSplit[9]);

                evokWork.openProgramBarCode(scId,hyId);

                //显示扫码信息
                /***
                0:编号
                1:长
                2:宽
                3:正反面
                4:门厚
                5:锁号
                6:模式
                7:左右开
                8:合页号
                9:合页数
                ***/
                StringBuilder sb = new StringBuilder();
                string[] stringParam = {
                                       "编号",
                                        "长",
                                        "宽",
                                        "正反面",
                                        "门厚",
                                        "锁号",
                                        "模式",
                                        "左右开",
                                        "合页号",
                                        "合页数",
                                      };
                if (stringParam.Count() == strSplit.Count())
                {
                    sb.Append(DateTime.Now.ToShortTimeString());
                    sb.Append("扫码信息：");
                    for (int i = 0; i < strSplit.Count(); i++)
                    {
                        sb.Append(stringParam[i]);
                        sb.Append(strSplit[i]);
                    }
                }



              //  t2.Text = sb.ToString();

                //MessageBox.Show("扫码结束！");

                textBox31.Text = "";

            }
        }

        private void button16_Click(object sender, EventArgs e)
        {
            evokWork.SetMPsONToOFF(((Control)sender).Tag.ToString(), Constant.Write, tc1.SelectedIndex);
        }

        private void textBox31_Enter(object sender, EventArgs e)
        {
            ((TextBox)sender).Text = "";
        }

        private void button59_Click(object sender, EventArgs e)
        {
            if (button59.Text == "扫码关")
            {
                button59.Text = "扫码开";
                button59.BackColor = Color.Green;
            }
            else
            {
                button59.Text = "扫码关";
                button59.BackColor = Color.Gray;
            }
        }

        private void WorkForm_KeyPress(object sender, KeyPressEventArgs e)
        {

        }

        private void stopBtn_Click_1(object sender, EventArgs e)
        {
            
            evokWork.
            SetMultiPleTest();
        }

        private void checkBox1_CheckedChanged(object sender, EventArgs e)
        {
            chkProcess((CheckBox)sender);
            
            evokWork.
            SetJgCd(txtChk[(CheckBox)sender].Text);
        }
        public void SetLiaoLength()
        {

        }
        private void checkBox2_CheckedChanged(object sender, EventArgs e)
        {

        }
    }

    #endregion
}

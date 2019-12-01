using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using xjplc;
using System.IO;
using System.Linq;
using System.Text;

namespace evokNew0055
{
    public partial class WorkForm : Form
    {
        private static Queue<Control> allCtrls = new Queue<Control>();

        List<string> errorList = new List<string>();
        int errorId = 0;
        private EvokDTTcpWork evokWork;

        private WatchForm wForm;

        public WorkForm()
        {

            // ConstantMethod.InitPassWd();
            InitializeComponent();
        }

        private void autoSLBtn_Click(object sender, EventArgs e)
        {
            evokWork.autoSL();
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
        public void InitMSJ()
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


            //禁止排序
            UserData0.DataSource = evokWork.DtScHyShow;
            UserData1.DataSource = evokWork.DtScHyShow;

            for (int i = 0; i < evokWork.DtScHyShow.Columns.Count; i++)
            {
                UserData0.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;

                UserData1.Columns[i].SortMode = DataGridViewColumnSortMode.Programmatic;
            }

            for (int i = 0; i < 20; i++)
            {
                  comboBox1.Items.Add((i).ToString());
                  comboBox2.Items.Add((i).ToString());
                comboBox7.Items.Add((i).ToString());
                comboBox3.Items.Add((i).ToString());
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
            evokWork.SetMPsOFFToOn(((Control)sender).Tag.ToString(), Constant.Write, evokWork.PsLstHand);
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
        public void InitParam()
        {
            //datasource 改变会出发 selectindex 改变事件  这样就会打条码导致 模式被自动修改
            //所以早点设置好 然后在 那个selectindexchanged事件里增加 通讯正常判断
            // printcb.DataSource = Constant.printBarcodeModeStr;

            LogManager.WriteProgramLog(Constant.ConnectMachineSuccess);

            evokWork = new EvokDTTcpWork(Constant.msjDeivceId);
            evokWork.SetUserDataGridView(UserData1);
            evokWork.SetPrintReport();
            evokWork.InitDgvParam(dgvParam);
            evokWork.InitDgvIO(dgvIO);
            UpdateTimer.Enabled = true;
            if (evokWork.DeviceId == Constant.msjDeivceId)
            {
                InitMSJ();
            }
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

            /****
            UserData1.Rows.AddCopies(0,11);
            UserData0.Rows.AddCopies(0, 11);
            for (int i = 0; i < UserData1.Rows.Count; i++)
            {
                UserData1.Rows[i].Cells[0].Value = (i + 1);
            }
            UserData1.ReadOnly = true;

            for (int i = 0; i < UserData0.Rows.Count; i++)
            {
                UserData0.Rows[i].Cells[0].Value = (i + 1);
            }

            UserData0.ReadOnly = true;
            ****/



            //evokWork.ReadCSVDataDefault();

        }


        private void lcTxt_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork.KeyPress_AutoPage(sender, e))
                label10.Focus();
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



        /// <summary>
        /// 20181105 这里以后PSlst 最好用参数或者索引 进行收集 避免有hand auto等字样
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

                        foreach (DTPlcInfoSimple simple in evokWork.PsLstAuto)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple, control)) break;
                        }
                    }
                    if (control.Parent == tabPage2 || control.Parent.Parent == tabPage2)
                    {
                        foreach (DTPlcInfoSimple simple2 in evokWork.PsLstHand)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple2, control)) break;
                        }
                    }
                    if (control.Parent == tabPage3 || control.Parent.Parent == tabPage3)
                    {
                        foreach (DTPlcInfoSimple simple3 in evokWork.PsLstParam)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple3, control)) break;
                        }
                    }
                    if (control.Parent == tabPage4 || control.Parent.Parent == tabPage3)
                    {
                        foreach (DTPlcInfoSimple simple4 in evokWork.PsLstIO)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple4, control)) break;
                        }
                    }
                    if ((control.Parent.Parent == tabPage5 || control.Parent == tabPage5) && evokWork.DeviceId == Constant.msjDeivceId)
                    {
                        foreach (DTPlcInfoSimple simple5 in evokWork.ProgramConfigPsLst)
                        {
                            if (ConstantMethod.setControlInPlcSimple(simple5, control)) break;
                        }
                    }
                }
            }
        }


        private void tc1_Selecting(object sender, TabControlCancelEventArgs e)
        {
            if (evokWork.RunFlag)
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
            UpdataParam1();
            UpdataIO();
        }

        private void FileSave_Tick(object sender, EventArgs e)
        {
            evokWork.SaveFile();
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
                if (evokWork.DeviceId == Constant.msjDeivceId)
                {
                    upScHy(UserData1, Constant.AutoPage);
                }
                foreach (DTPlcInfoSimple simple in evokWork.PsLstAuto)
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
                foreach (DTPlcInfoSimple p in evokWork.PsLstAuto)
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
                foreach (DTPlcInfoSimple simple in evokWork.PsLstHand)
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

        private void UpdataParam1()
        {
            if (tc1.SelectedIndex == 4)
            {
                if (evokWork.DeviceId == Constant.msjDeivceId)
                {
                    upScHy(UserData0, Constant.Param1Page);
                }
                foreach (DTPlcInfoSimple simple in evokWork.ProgramConfigPsLst)
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



        private void button11_Click(object sender, EventArgs e)
        {
            evokWork.ClearError();
        }



        private void button1_Click(object sender, EventArgs e)
        {

        }

        private void groupBox2_Enter(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }

        private void label8_Click(object sender, EventArgs e)
        {

        }

        private void groupBox4_Enter(object sender, EventArgs e)
        {

        }

        private void stbtn_MouseDown_1(object sender, MouseEventArgs e)
        {
            evokWork.mouseDown(sender, e, Constant.AutoPage);
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

        private void tabPage5_Enter(object sender, EventArgs e)
        {
            groupBox5.Visible = false;
            groupBox6.Visible = true;
        }

        public void setColor(DataGridView dgv, int row, int col)
        {
            for (int i = 0; i < dgv.RowCount; i++)
            {
                for (int j = 0; j < dgv.ColumnCount; j++)
                {
                    dgv.Rows[i].Cells[j].Style.BackColor = Color.Gray;
                }
            }
            dgv.Rows[row].Cells[col].Style.BackColor = Color.Red;

        }


        private void button37_Click(object sender, EventArgs e)
        {
            evokWork.SetMPsONToOFF(((Control)sender).Tag.ToString(), Constant.Write, evokWork.ProgramConfigPsLst);

            groupBox5.Visible = false;
            groupBox6.Visible = true;
            evokWork.ShiftShowPage(10);
            evokWork.selectGw(0);

        }

        private void comboBox5_SelectedIndexChanged(object sender, EventArgs e)
        {
            //锁槽归锁槽  合页归合页
            if (tc1.SelectedIndex == Constant.Param1Page && groupBox5.Visible == true)
            {
                string selectStr = comboBox5.Text;

                List<string> strLstSc = new List<string>();
                strLstSc.AddRange(Constant.scCutType);

                List<string> strLstHy = new List<string>();
                strLstHy.AddRange(Constant.hyCutType);

                if (strLstSc.Contains(selectStr))
                {
                    int id = strLstSc.IndexOf(selectStr);
                    pictureBox1.Image = imageLstSc.Images[id];
                    evokWork.ShiftShowPage(Constant.scCutTypeShowId[id]);
                    //隐藏一些不必要的显示项
                    showControlById(comboBox5.SelectedIndex);
                }
                else
                {
                    if (strLstHy.Contains(selectStr))
                    {
                        int id = strLstHy.IndexOf(selectStr);
                        pictureBox1.Image = imageLstHy.Images[id];
                        evokWork.ShiftShowPage(Constant.hyCutTypeShowId[id]);
                        //隐藏一些不必要的显示项
                        showControlById(comboBox5.SelectedIndex);
                    }
                    else
                    {
                        MessageBox.Show("选择错误！");
                    }
                }

            }
        }

        private void comboBox3_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.ProgramConfigPsLst);
        }

        private void comboBox3_SelectedIndexChanged(object sender, EventArgs e)
        {
            //evokWork.selectProgramNo(int.Parse(comboBox3.SelectedItem.ToString()));
            //evokWork.SetOutEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.ProgramConfigPsLst);          
            //button40.Focus();
        }

        private void button41_Click(object sender, EventArgs e)
        {
            evokWork.SetInEdit("程序号", Constant.Read, evokWork.ProgramConfigPsLst);
        }

        private void textBox30_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (evokWork.KeyPress_Page(sender, e, tc1.SelectedIndex))
                button40.Focus();
        }

        private void textBox30_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((TextBox)sender).Tag.ToString(), Constant.Read, Constant.ScarPage);
        }

        private void textBox30_Leave(object sender, EventArgs e)
        {
            evokWork.SetOutEdit(((TextBox)sender).Tag.ToString(), Constant.Read, Constant.ScarPage);
        }

        private void comboBox6_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.PsLstAuto);
        }

        private void comboBox6_SelectedValueChanged(object sender, EventArgs e)
        {
            // if (string.IsNullOrWhiteSpace(comboBox6.Text.ToString())) return;
            // evokWork.openProgram(int.Parse(comboBox6.Text.ToString()));
            try
            {
                //   evokWork.openProgram(int.Parse(comboBox6.Text.ToString()), int.Parse(comboBox2.Text.ToString()));
                evokWork.SetOutEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.PsLstAuto);
            }
            catch (Exception ex)
            {

            }
            stbtn.Focus();
        }

        private void comboBox4_SelectedIndexChanged(object sender, EventArgs e)
        {
            evokWork.selectKnife(int.Parse(comboBox4.SelectedIndex.ToString()));
            evokWork.SetOutEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.ProgramConfigPsLst);
            button40.Focus();
        }

        void InitSc(int selectId, string valueStr, ComboBox cb5)
        {
            int id = 0;
            switch (selectId)
            {
                case 0:
                    {

                        List<string> str = new List<string>();
                        str.AddRange(Constant.scCutType);

                        cb5.Items.Clear();
                        cb5.Items.AddRange(str.ToArray());
                        id = (str.IndexOf(valueStr));
                        if (id < 0) id = 0;
                        evokWork.ShiftShowPage(Constant.scCutTypeShowId[id]);
                        cb5.SelectedIndex = id;

                        pictureBox1.Image = imageLstSc.Images[cb5.SelectedIndex];

                        break;
                    }
                case 1:
                    {

                        List<string> str = new List<string>();
                        str.AddRange(Constant.hyCutType);

                        cb5.Items.Clear();
                        cb5.Items.AddRange(str.ToArray());


                        id = (str.IndexOf(valueStr));
                        if (id < 0) id = 0;
                        evokWork.ShiftShowPage(Constant.hyCutTypeShowId[id]);
                        cb5.SelectedIndex = id;

                        pictureBox1.Image = imageLstHy.Images[cb5.SelectedIndex];

                        break;
                    }
                default:
                    {
                        break;
                    }
            }



        }


        private void UserData0_CellClick(object sender, DataGridViewCellEventArgs e)
        {

            if (!(e.RowIndex > -1 && e.ColumnIndex > -1)) return;
            UserData0.Rows[e.RowIndex].Selected = false;
            string valueStr = UserData0.Rows[e.RowIndex].Cells[e.ColumnIndex].Value.ToString();



            int id = 0;
            int offset = 0;

            if (e.ColumnIndex == 1)
            {
                //锁槽
                InitSc(0, valueStr, comboBox5);
                evokWork.selectGw(e.RowIndex + 1);
            }
            else
            if (e.ColumnIndex == 2)
            {
                //合页
                InitSc(1, valueStr, comboBox5);
                offset = 4;
                evokWork.selectGw(e.RowIndex + 13);
            }

            showControlById(offset + id);
            groupBox5.Visible = true;
            groupBox6.Visible = false;

        }

        void showControlById(int id)
        {

            if ((comboBox5.Text.Equals(Constant.scCutType[1])) || comboBox5.Text.Equals(Constant.scCutType[3]))
            {
                textBox26.Visible = false;
                textBox24.Visible = false;
                label51.Visible = false;
                label47.Visible = false;
            }
            else
            {
                textBox26.Visible = true;
                textBox24.Visible = true;
                label51.Visible = true;
                label47.Visible = true;
            }

            if ((comboBox5.Text.Equals(Constant.scCutType[4])))
            {
                textBox24.Visible = false;
                label47.Visible = false;
            }
            else
            {
                textBox24.Visible = true;
                label47.Visible = true;
            }

            if ((comboBox5.Text.Equals(Constant.hyCutType[1]))
                || (comboBox5.Text.Equals(Constant.hyCutType[2]))
                )
            {
                textBox25.Visible = false;
                label48.Visible = false;
            }
            else
            {
                textBox25.Visible = true;
                label48.Visible = true;
            }
        }
        private void button44_Click(object sender, EventArgs e)
        {

        }

        private void button41_Click_1(object sender, EventArgs e)
        {
            evokWork.SetMPsONToOFF(((Control)sender).Tag.ToString(), Constant.Write, evokWork.ProgramConfigPsLst);
        }

        private void button38_Click(object sender, EventArgs e)
        {

        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {

        }

        private void button34_Click(object sender, EventArgs e)
        {
            DialogResult dr = MessageBox.Show("确定删除?", "清除程序", MessageBoxButtons.OKCancel);

            if (dr == DialogResult.OK)//如果点击“确定”按钮
            {
                Opossite_Click_Param1(sender, e);
                ConstantMethod.Delay(200);
                Opossite_Click_Param1(sender, e);
            }
        }

        private void groupBox1_Enter(object sender, EventArgs e)
        {

        }

        private void comboBox7_SelectedValueChanged(object sender, EventArgs e)
        {
            evokWork.selectSc(int.Parse(((ComboBox)sender).Text));
            evokWork.SetOutEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.ProgramConfigPsLst);
            button27.Focus();
        }

        private void comboBox3_SelectedValueChanged(object sender, EventArgs e)
        {
            evokWork.selectHy(int.Parse(((ComboBox)sender).Text));
            evokWork.SetOutEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.ProgramConfigPsLst);
            button27.Focus();
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
                if (!(strSplit.Count() == 10))
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

                evokWork.openProgramBarCode(scId, hyId);

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



                t2.Text = sb.ToString();

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
            // MessageBox.Show(tc1.SelectedIndex.ToString()+ button59.Text);
            if ((tc1.SelectedIndex == 0) && (button59.Text.Equals("扫码开")))
            {

                textBox31.Focus();

                int i = (int)e.KeyChar;

                if (
                    (i <= 57 && i >= 47)
                    ||
                    (i == 0)
                    ||
                    (i >= 65 && i <= 90)
                    )
                {
                    textBox31.AppendText(e.KeyChar.ToString());
                }

                if (e.KeyChar == 13)
                {
                    textBox31_KeyPress(sender, e);
                }

                e.Handled = true;

            }
        }

        private void stopBtn_Click(object sender, EventArgs e)
        {

        }

        private void button46_Click(object sender, EventArgs e)
        {

        }

        private void comboBox1_Enter(object sender, EventArgs e)
        {
            evokWork.SetInEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.PsLstAuto);
        }

       

        private void comboBox1_SelectedValueChanged(object sender, EventArgs e)
        {
            int i=0;
            if (int.TryParse(((ComboBox)sender).Text, out i))
            {
                evokWork.selectHy(int.Parse(((ComboBox)sender).Text));
                evokWork.SetOutEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.PsLstAuto);
                stbtn.Focus();
            }
        }

        private void comboBox2_SelectedValueChanged(object sender, EventArgs e)
        {
            int i = 0;
      
            if (int.TryParse(((ComboBox)sender).Text,out i))
            {
                evokWork.selectSc(int.Parse(((ComboBox)sender).Text));
                evokWork.SetOutEdit(((ComboBox)sender).Tag.ToString(), Constant.Read, evokWork.PsLstAuto);
                stbtn.Focus();
            }
        
           
        }

        private void button60_Click(object sender, EventArgs e)
        {
            evokWork.bitOnClick(((Control)sender).Tag.ToString(), Constant.Write,Constant.AutoPage );
        }

        private void 中文ToolStripMenuItem_Click(object sender, EventArgs e)
        {

            MultiLanguage.setLangId(Constant.idChinese, evokWork.ParamFile);

            InitLang();

        }

        public void InitLang()
        {
            //语言设置
            //设置提醒字符串
            Constant.InitStr(this);

            /***
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
            ****/

        }

        private void 英文ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MultiLanguage.setLangId(Constant.idEnglish, evokWork.ParamFile);

            InitLang();

        }

        private void 成型样式编辑ToolStripMenuItem_Click(object sender, EventArgs e)
        {

        }
    }
    #endregion
}


using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.Windows.Forms;
using xjplc;
using System.IO;
using System.Collections;
using System.Text.RegularExpressions;

namespace evokNew0077
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
            InitDeviceMode();
            InitParam();
            InitControl();
            InitView0();
           
            Application.DoEvents();         
            this.Visible = true;
        }
        const int putongJiaYou = 0;
        const int putongChiTiao = 1;
        void InitDeviceMode()
        {
            //设备有好几种 那就显示不同的UI
            ConfigFileManager cfm = new ConfigFileManager(Constant.ConfigDeviceModeFilePath);
            if (cfm.ReadConfig(Constant.deviceMode).Equals(putongJiaYou.ToString()))
            {
                //加油类型
                //手动 
                button15.Visible = false;
                button14.Visible = false;
                button13.Visible = false;
                groupBox9.Visible = false;

                //参数这里
                label76.Visible = false;
                label77.Visible = false;
                textBox44.Visible = false;
                textBox45.Visible = false;
                label79.Visible = false;
                textBox50.Visible = false;
                this.Text = "水平打孔机-左右拉槽改加油";

            }
            if (cfm.ReadConfig(Constant.deviceMode).Equals(putongChiTiao.ToString()))
            {
                //齿条类型
                // 手动 
                lblY13.Visible = false;
                this.Text = "水平打孔机-左右拉槽带检测铣槽";
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

            //dataLoad(Constant.userdata);

            if (button9.Text == "扫码开")
            {
                button9.BackColor = Color.Green;
            }
            else
            {
                button9.BackColor = Color.Gray;
            }

            lightLst.Add(label50);
            lightLst.Add(label51);
            lightLst.Add(label52);
            lightLst.Add(label53);


            PrevlightLst.Add(label81);
            PrevlightLst.Add(label89);
            PrevlightLst.Add(label90);
            PrevlightLst.Add(label91);

            evokWork.updateData = UpdateData;

        }
        #endregion
        #region 按钮和数据输入事件


        private void lcTxt_KeyPress0(object sender, KeyPressEventArgs e)
        {
            if (evokWork.AutoParamTxt_KeyPressWithRatioWithId(sender, e,tc1.SelectedIndex))
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
                evokWork.IsShow(label42);
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
        #region 水平打孔机
        DataTable dtData;
        public void InitDoubleSaw()
        {
            evokWork.DeviceName = Constant.dkjDeivceName;
            evokWork.DeviceProperty = Constant.dkjDeivceId;
        }

        void dataLoad(string fileName)
        {
            CsvStreamReader op = new CsvStreamReader();
            if (File.Exists(fileName))
            {
                dtData = op.OpenCSV(fileName);
                dgv.DataSource = dtData;
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
            ConstantMethod.Delay(100);
           
            Environment.Exit(0);
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
                        || (control.Parent.Parent.Parent.Parent != null && control.Parent.Parent.Parent.Parent == tabPage2))
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
                          
        private void label100_Click(object sender, EventArgs e)
        {

        }

        private void label34_Click(object sender, EventArgs e)
        {

        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (DialogExcelDataLoad.ShowDialog()==DialogResult.OK)
            {
                dataLoad(DialogExcelDataLoad.FileName);
            }
        }
        void DataDownLoad()
        {
            //槽长 槽深 木销孔选择
            List<int> dataSize = new List<int>();
            List<bool> byteSel = new List<bool>();
            foreach (DataRow dr in dtData.Rows)
            {
                double temp0 = 0;
                double temp1 = 0;
                bool ts = false;
                double.TryParse(dr[0].ToString(), out temp0);
                double.TryParse(dr[1].ToString(), out temp1);
                if (temp0 < 1) temp0 = 0;
                if (temp1 < 1) temp1 = 0;
                temp0 *= 100;
                 temp1 *= 100;
                dataSize.Add((int)temp0);
                dataSize.Add((int)temp1);

                if (dr[2].ToString().Equals("1"))
                    ts = true;

                byteSel.Add(ts);
            }
            if (dataSize.Count > 0)
            {
                //处理数据 位转字节
                BitArray br = new BitArray(byteSel.ToArray());
                dataSize.Insert(0, dataSize.Count / 2);
                evokWork.dataLoad(dataSize.ToArray(), ConstantMethod.BitToIntTwo(br));
            }
        }
        private void button3_Click(object sender, EventArgs e)
        {
            DataDownLoad();
        }

        private void textBox21_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == '\r')
            {
                int count = 0;
                if (int.TryParse(textBox21.Text, out count))
                {
                    if (count <= dgv.Rows.Count && count > 0)
                    {
                        lcTxt_KeyPress0(sender, e);
                    }
                    else MessageBox.Show("数据超出,大于0 且小于尺寸数量!");
                }
                else MessageBox.Show("非法数据！");
            }
        }

        private void WorkForm_KeyPress(object sender, KeyPressEventArgs e)
        {
            if ((tc1.SelectedIndex == 0) && ((button9.Text == "扫码开")))
            {
                textBox37.Focus();
                int i = (int)e.KeyChar;

              //  richTextBox1.AppendText(i.ToString()+"\r\n");  

                if ((i < 58) && (i >= 47) || (i == 0) || (i ==95) || Regex.IsMatch(e.KeyChar.ToString(), "[a-zA-Z]"))
                {
                    textBox37.AppendText(e.KeyChar.ToString());

                }
                if (e.KeyChar == 13)
                {
                    textBox37_KeyPress(sender, e);
                }
                e.Handled = true;

            }
        }
        List<Label> lightLst = new List<Label>();       //当前显示
        List<Label> PrevlightLst = new List<Label>(); //预期显示
        public void UpdateData(string name)
        {
          //  if (string.IsNullOrWhiteSpace(name)) evokWork.StopRunning();
            //更新灯光
            if(lightLst !=null && lightLst.Count>0)
            foreach (Label l in lightLst)
            {
                l.Invoke((EventHandler)(delegate
                {
                    if (!string.IsNullOrWhiteSpace(name)&&name.Contains(l.Tag.ToString()))
                    {
                        l.BackColor = Color.Green;
                    }
                    else
                    {
                        l.BackColor = Color.Gray;
                    }

                }));
            }
            //
            if (evokWork.HoleDataLst != null && evokWork.HoleDataLst.Count > 0)
            {       
               
                foreach (DataTable dt in evokWork.HoleDataLst)
                {
                    //当前有多少面
                    foreach (Label l in PrevlightLst)
                    {
                        if (dt.TableName.Contains(l.Tag.ToString()))
                        {
                            l.BackColor = Color.Green;
                        }                      
                    }
                    if (!string.IsNullOrWhiteSpace(name) && dt.TableName.Contains(name))
                    {
                        dgv.Invoke((EventHandler)(delegate
                        {
                            dtData = dt;

                            dgv.DataSource = dt;

                            DataDownLoad();
                        }));
                    }

                }
            }
            if (string.IsNullOrWhiteSpace(name))
            {
                dgv.DataSource = null;
                label94.Text = "NA";
                foreach (Label l in PrevlightLst)
                {
                   
                   l.BackColor = Color.Gray;
                    
                }
            }

            



        }
        private void textBox37_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13 && textBox37.Text != "")
            {
                string s = textBox37.Text;
                textBox37.Clear();
                label94.Text = s;
                evokWork.ScanCode(s);

              
            }
        }
        #region 扫码
              
        
        #endregion
        private void button12_Click(object sender, EventArgs e)
        {
            if (fb1.ShowDialog() == DialogResult.OK)
            {
                evokWork.SetBarCodeSourceFolder(fb1.SelectedPath);
            }
        }

        private void button9_Click(object sender, EventArgs e)
        {
            textBox37.Text = "";

            if (((Button)sender).Text == "扫码关")
            {
                ((Button)sender).Text = "扫码开";
                textBox37.Focus();
                ((Button)sender).BackColor = Color.Green;
            }
            else
            {
                ((Button)sender).Text = "扫码关";
                ((Button)sender).BackColor = Color.Gray;
            }
        }

        private void button10_Click(object sender, EventArgs e)
        {
            evokWork.StopRunning();
            textBox37.Text = "";
        }

        private void button5_Click_1(object sender, EventArgs e)
        {
            evokWork.StopRunning();
            textBox37.Text = "";
        }

        private void textBox37_Enter(object sender, EventArgs e)
        {
            textBox3.Clear();
        }

        private void button13_Click(object sender, EventArgs e)
        {

        }

        private void textBox51_TextChanged(object sender, EventArgs e)
        {

        }

        private void tabPage1_Click(object sender, EventArgs e)
        {

        }
    }   
}

using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;
using xjplc;
using System.Net;

namespace zlzk
{
    public partial class YBForm : Form
    {
        
        string deviceDataFileName = null;
        DataTable devicedt=null;
        SocServer socserver = null;
        DeltaPlcCommand plccmd = null;

        System.Timers.Timer commTimer = new System.Timers.Timer(10000);
        
        YBDTWorkManger ybdtWorkManger;

        List<string> deviceIpLstStr;

        SetShowForm sform ;


       
        Dictionary<string, Button> btnShowLst;
        public YBForm()
        {
            InitializeComponent();
        }

        private void label16_Click(object sender, EventArgs e)
        {

        }

        private void Form1_Load(object sender, EventArgs e)
        {


            //加载文件信息 默认为程序文件夹吧 
            ybdtWorkManger = new YBDTWorkManger();
            ybdtWorkManger.ydtdWorkChangedEvent += this.ydbtWorkChangedEvent;
            deviceIpLstStr = new List<string>();
            deviceLB.DataSource = deviceIpLstStr;
            timer1.Enabled = true;
            sform = new SetShowForm();
            sform.showItemChanged += showItemChanged;
            dataGridView1.DataSource =
            ybdtWorkManger.ShowDataTable;
            List<Button> btnLst;
            btnLst = new List<Button>();
            btnLst.Add(button10);
            btnLst.Add(button5);
            btnLst.Add(button1);
            btnLst.Add(button4);
            btnLst.Add(button6);
            btnLst.Add(button7);
            btnLst.Add(button8);
            btnLst.Add(button12);
            btnLst.Add(button13);
            btnLst.Add(button14);
            btnLst.Add(button15);
            btnLst.Add(button16);
            btnLst.Add(button17);
            btnLst.Add(button18);
            btnLst.Add(button19);
            btnLst.Add(button20);
            btnLst.Add(button21);
            btnLst.Add(button22);
            btnLst.Add(button23);



            btnShowLst = new Dictionary<string, Button>();
            for (int j = 0; j < ybdtWorkManger.YbdtWorkInfoLst.Count; j++)
            {
                btnShowLst.Add( ybdtWorkManger.YbdtWorkInfoLst[j].DeviceId,btnLst[j]);
            }

        }

        private void TimeEvent(object source, System.Timers.ElapsedEventArgs e)
        {
            /***
            if (socserver.IsDeviceOk)
            {
               
                //DataProcess.ShowInfo(recmsg,"设备连接成功！");
                if (socserver.IsSetDRead)
                {
                    DataProcess.ShowInfo(recmsg, "设备设置读取成功！");
                }
                else
                {
                    socserver.SendMsgByte(plccmd.deltaCmdSendByte(plccmd.byteDelta10));
                    commTimer.Enabled = false;                
                }
                return;
            }
            else
            {
                
                DataProcess.ShowInfo(recmsg, "连接设备~~~~~~~~~~~~~~！");
                socserver.SendMsgByte(plccmd.deltaCmdSendByte(plccmd.byteDeltaCmd));
                
            }
            ***/
        }

        private void button1_Click(object sender, EventArgs e)
        {
            NPOI.ExportDataToExcelNoDialog(devicedt, deviceDataFileName,null,null);
           //NPOI.ExportDataToExcel(devicedt, "321", null, null);
        }
        
        private void button3_Click(object sender, EventArgs e)
        {
          //  if(deviceLB.SelectedIndex >-1)
           // ybdtWorkManger.YbdtWorkLst[deviceLB.SelectedIndex].SaveData();           
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (op1.ShowDialog() == DialogResult.OK)
            {
                //注释部分 与NPOI 不能同时调用同一个文件
                //userex.Open(op1.FileName);
                // userex.ws = userex.GetDefaultSheet();

                devicedt = NPOI.ImportExcel(op1.FileName);

                //dgvDevice.DataSource = devicedt;

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (socserver == null)
            {
                socserver = new SocServer();
                //socserver.Setsenmsg(sendmsg);
               // socserver.SetRecRichBox(recmsg);
               // socserver.Setiptext(deviceLB);
                socserver.startServer();
            }
        }

        private void button6_Click(object sender, EventArgs e)
        {
            if (socserver != null)
            {
                socserver.SafeClose();
            }
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //socserver.SendMsgByte(plccmd.deltaCmdSendByte(plccmd.byteDelta10));

            //socserver.SendMsgByte(plccmd.deltaCmdSendByte(plccmd.byteDeltaCmd));

        }
        //临时打包命令 读取D区
        private void tempDPack()
        {
            List<int> addrLst = new List<int>();
            List<int> idLst = new List<int>();
            List<int> addrcount = new List<int>();
            List<int> breakPoint = new List<int>();

            addrLst.Add(0);
            //addrLst.Add(1);
            //addrLst.Add(5);
            //addrLst.Add(6);

            idLst.Add(DeltaPlcCommand.Delta_D_ID);
            //idLst.Add(DeltaPlcCommand.Delta_D_ID);
            //idLst.Add(DeltaPlcCommand.Delta_D_ID);
            //idLst.Add(DeltaPlcCommand.Delta_D_ID);

            plccmd.PackDelta10(addrLst, idLst);

            string str = DataProcess.byteToHexStr(plccmd.byteDelta10);

        }
        //临时打包命令 读取M区
        private void tempMPack()
        {

        }
        private void button8_Click(object sender, EventArgs e)
        {
            

            /***
            recmsg.AppendText(str);

            byte[] array = System.Text.Encoding.ASCII.GetBytes(str);
            List<byte> cmdbyte = new List<byte>();
            cmdbyte.Add(0x3A);
            cmdbyte.AddRange(array);
            cmdbyte.Add(0x0d);
            cmdbyte.Add(0x0a);
            str = DataProcess.byteToHexStr(cmdbyte.ToArray());
            **/
           // recmsg.AppendText(str);


        }

        private void sendmsg_TextChanged(object sender, EventArgs e)
        {

        }

        private void button2_Click(object sender, EventArgs e)
        {
            commTimer.Enabled = true;
        }

        private void label10_Click(object sender, EventArgs e)
        {

        }
        void ydbtWorkChangedEvent(object sender,  YBDTWork ybtdWork0)
        {
            if (ybdtWorkManger.YbdtWorkLst != null && ybdtWorkManger.YbdtWorkLst.Count > 0)
            {
                List<string> tempLst = new List<string>();
                foreach (YBDTWork ybw in ybdtWorkManger.YbdtWorkLst)
                {
                    string strIp = (ybw.YbtdDevice.SocClient.RemoteEndPoint as IPEndPoint).Address.ToString();
                    YBDTWork yw = ybdtWorkManger.GetYBDTWorkFromLst(ybdtWorkManger.YbdtWorkLst, strIp);
                    if (yw != null)
                    {
                        tempLst.Add(yw.YbdtWorkInfo.DeviceId);
                    }
                    else
                    {
                        tempLst.Add(Constant.NoIdDevice);
                    }
                }
                deviceIpLstStr = tempLst;
                this.Invoke((EventHandler)(delegate
                {
                    deviceLB.DataSource = deviceIpLstStr;
                    deviceLB.Refresh();
                }));
                
              
            }
            else
            {
                this.Invoke((EventHandler)(delegate
                {
                    deviceLB.DataSource = null;
                    deviceLB.Refresh();
                }));
            }
        }
        public void SetButtonStatus(Button b1,int s,string str)
        {
            b1.Text = str;
            switch (s)
            {
             
                case -1:
                    {
                        b1.BackColor = System.Drawing.Color.Red;
                       
                        break;
                    }
                case 0:
                    {
                        b1.BackColor = System.Drawing.Color.Gray;
                        break;
                    }
                case 1:
                    {
                        b1.BackColor = System.Drawing.Color.Green;
                        break;
                    }
                case -2:
                    {
                        b1.BackColor = System.Drawing.Color.Chocolate;
                        break;
                    }
            }
        }
        private void deviceLstTimer_Tick(object sender, EventArgs e)
        {
            int id = deviceLB.SelectedIndex;
            if (id>-1&&id < ybdtWorkManger.YbdtWorkLst.Count && ybdtWorkManger.YbdtWorkLst.Count>0)
            {
                UpdateWorkInfo(ybdtWorkManger.YbdtWorkLst[id]);
            }          
            
        }
        public void SetLabelText(Control label0,string s)
        {
            if (!s.Equals(label0.Text.ToString()))
            {
                label0.Text = s;
            }
        }
        private void UpdateWorkInfo(YBDTWork yw)
        {

            List<string> strLst = new List<string>();
            List<Control> labLst = new List<Control>();

            labLst.Add(label25);
            labLst.Add(label31);
            labLst.Add(label3);
            labLst.Add(label5);
            labLst.Add(label6);
            labLst.Add(label11);
            labLst.Add(label9);
            labLst.Add(label7);
            labLst.Add(label37);
            labLst.Add(label8);
            labLst.Add(label39);
            labLst.Add(label10);
            labLst.Add(label41);
            labLst.Add(label52);
            labLst.Add(label51);
            labLst.Add(label48);
            labLst.Add(label47);
            labLst.Add(label44);
            labLst.Add(label43);
            labLst.Add(deviceGroupBox);

            strLst.Add(yw.YbdtWorkInfo.Department);
            strLst.Add(yw.YbdtWorkInfo.DanHao);
            strLst.Add(yw.StartTime.ToLocalTime().ToString());
            strLst.Add(yw.EndNeedTime.ToLocalTime().ToString());
            strLst.Add(yw.EndRealTime.ToLocalTime().ToString());
            strLst.Add(yw.YbdtWorkInfo.Speed);
            strLst.Add(yw.ReadSpeed.ToString());
            strLst.Add(yw.YbdtWorkInfo.SetProdQuantity);
            strLst.Add(yw.ProdQuantity.ToString());
            strLst.Add(yw.YbdtWorkInfo.TuHao);
            strLst.Add(yw.YbdtWorkInfo.ProdName);
            strLst.Add(yw.YbdtWorkInfo.GongXu);
            strLst.Add(yw.YbdtWorkInfo.GyTx);
            strLst.Add(yw.YbdtWorkInfo.OperatorName);
            strLst.Add(yw.YbdtWorkInfo.DeviceId);
            strLst.Add(yw.YbdtWorkInfo.CadPath);
            strLst.Add(yw.YbdtWorkInfo.Ddsm);
            strLst.Add(yw.YbdtWorkInfo.Jshu);
            strLst.Add(yw.YbdtWorkInfo.Gmj);
            strLst.Add(yw.YbdtWorkInfo.DeviceIP);

            if (strLst.Count == labLst.Count)
            {
                for (int i = 0; i < strLst.Count; i++)
                {
                    SetLabelText(labLst[i], strLst[i]);
                }
            }
            /****
            label25.Text = yw.YbdtWorkInfo.Department;
            label31.Text = yw.YbdtWorkInfo.DanHao;
            label3.Text = yw.StartTime.ToLocalTime().ToString();
            label5.Text = yw.EndNeedTime.ToLocalTime().ToString();
            label6.Text = yw.EndRealTime.ToLocalTime().ToString();
            label11.Text = yw.YbdtWorkInfo.Speed;
            label9.Text = yw.ReadSpeed.ToString();
            label7.Text = yw.YbdtWorkInfo.SetProdQuantity;
            label37.Text = yw.ProdQuantity.ToString();
            label8.Text = yw.YbdtWorkInfo.TuHao;
            label39.Text = yw.YbdtWorkInfo.ProdName;
            label10.Text = yw.YbdtWorkInfo.GongXu;
            label41.Text = yw.YbdtWorkInfo.GyTx;
            label52.Text = yw.YbdtWorkInfo.OperatorName;
            label51.Text = yw.YbdtWorkInfo.DeviceId;
            label48.Text = yw.YbdtWorkInfo.CadPath;
            label47.Text = yw.YbdtWorkInfo.Ddsm;
            label41.Text = yw.YbdtWorkInfo.Jshu;
            label43.Text = yw.YbdtWorkInfo.Gmj;
            *****/

        }
        private void ShowWork(YBDTWork yw)
        {
            //MessageBox.Show(deviceLB.SelectedItem.ToString());
            ybdtWorkForm ydForm = new ybdtWorkForm();
            ydForm.SetYbtdWork(yw);
            ydForm.Show();
        }
        private void deviceLB_Click(object sender, EventArgs e)
        {
            
            //MessageBox.Show(deviceLB.SelectedItem.ToString());
        }

        private void deviceLB_DoubleClick(object sender, EventArgs e)
        {
            ShowWork(ybdtWorkManger.YbdtWorkLst[deviceLB.SelectedIndex]);
        }

        private void button1_Click_1(object sender, EventArgs e)
        {
            string sql = " SELECT * FROM deviceinfo";

            DataTable testDatatable = new DataTable();

            testDatatable = SqlHelper.ExecuteDataTable(sql);

        }

        private void dataGridView1_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }

        private void tabPage1_Enter(object sender, EventArgs e)
        {
            deviceLstTimer.Enabled =true;
            if(deviceLB.Items.Count>0)
                deviceLB.SetSelected(0, true);
        }

        private void tabPage1_Leave(object sender, EventArgs e)
        {
            deviceLstTimer.Enabled = false;
        }
        void showItemChanged(object s,ItemChangedArgs e)
        {
            //根据显示项目 进行显示
            ybdtWorkManger.ItemShowStr = e.ItemSelect;
            ybdtWorkManger.SqlShowDataTable.Rows.Clear();

        }

        private void 设置显示项目ToolStripMenuItem_Click(object sender, EventArgs e)
        {
       
            sform.Show();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            UpdateRation();
            label22.Text = DateTime.Now.ToString("yyyy/MM/dd HH:mm:ss");
            
            if(ybdtWorkManger.YbdtWorkLst !=null & ybdtWorkManger.YbdtWorkLst.Count>0)
            //目前只针对两台设备 后期进行多台设备的自动创建按钮显示
            for (int i = 0; i < ybdtWorkManger.YbdtWorkLst.Count; i++)
            {
                if(i<btnShowLst.Count && i<10)
                SetButtonStatus(btnShowLst[ybdtWorkManger.YbdtWorkLst[i].YbdtWorkInfo.DeviceId], ybdtWorkManger.YbdtWorkLst[i].status, ybdtWorkManger.YbdtWorkLst[i].YbdtWorkInfo.DeviceId);
                /****
                if (i == 0)
                {
                    SetButtonStatus(button10, ybdtWorkManger.YbdtWorkLst[i].status, ybdtWorkManger.YbdtWorkLst[i].YbdtWorkInfo.DeviceId);
                }
                if (i == 1)
                {
                    SetButtonStatus(button5, ybdtWorkManger.YbdtWorkLst[i].status, ybdtWorkManger.YbdtWorkLst[i].YbdtWorkInfo.DeviceId);
                }
               *****/
            }
            ybdtWorkManger.GetDataFromSql(dataGridView1); 
            
        }
        private void UpdateRation()
        {
            foreach (DataGridViewRow dr in dataGridView1.Rows)
            {
                int fenzi;
                int fenmu;
                try
                {                 
                   this.Invoke((EventHandler)(delegate
                   {                                                               
                    if (int.TryParse(dr.Cells["当前产量"].Value.ToString(), out fenzi) && (int.TryParse(dr.Cells["排产量"].Value.ToString(), out fenmu)))
                    {

                        if (fenzi <= fenmu && fenzi>-1)
                        {
                            dr.Cells["完成率"].Value = ((double)fenzi / fenmu)*100;
                        }
                    }
                  }));
                }
                catch (Exception ex)
                {

                }                       
           }
        }
        private void button1_Click_2(object sender, EventArgs e)
        {
            if (SqlHelper.IsDBExist("zlzk"))
            {
                MessageBox.Show("数据库存在");
            }
           
        }

        private void dataGridView1_CellBeginEdit(object sender, DataGridViewCellCancelEventArgs e)
        {

        }

        private void YBForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            //首先直接退出
            ConstantMethod.AppExit();
            if (ybdtWorkManger.YbdtWorkLst.Count>0)
            {
                for (int i = 0; i < ybdtWorkManger.YbdtWorkLst.Count; i++)
                {
                    if (ybdtWorkManger.YbdtWorkLst[i] != null)
                        ConstantMethod.Delay(500);
                    ybdtWorkManger.YbdtWorkLst[i].Dispose();
                   // if (ybdtWorkManger.YbdtWorkLst.Count > 0) continue;
                }
                /****
                foreach (YBDTWork ybwork in ybdtWorkManger.YbdtWorkLst)
                {
                    if(ybwork !=null)
                    ConstantMethod.Delay(1000);
                    ybwork.Dispose();
                    if (ybdtWorkManger.YbdtWorkLst.Count > 0) continue;
                }
                *****/
                }
            
        }

        private void tabPage3_Enter(object sender, EventArgs e)
        {
            ConstantMethod.Delay(200);
            if (ybdtWorkManger != null)
            {
                timer1.Enabled = true;
                tabPage3.Refresh();
                dataGridView1.Refresh();
                dataGridView1.DataSource =
                ybdtWorkManger.ShowDataTable;
            }
        }

        private void tabPage3_Leave(object sender, EventArgs e)
        {
            timer1.Enabled = false;                    
        }
    }
}

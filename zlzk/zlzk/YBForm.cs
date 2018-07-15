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

        System.Timers.Timer commTimer = new System.Timers.Timer(2000);

        YBDTWorkManger ybdtWorkManger;

        List<string> deviceIpLstStr;
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
           // deviceLstTimer.Enabled = true;
            /****
            deviceDataFileName = System.AppDomain.CurrentDomain.BaseDirectory + "device.xlsx";
            if (!File.Exists(deviceDataFileName))
            {
                MessageBox.Show("设备文件丢失!");
                Application.Exit();
            }
            devicedt = new DataTable();

            devicedt=NPOI.ImportExcel(deviceDataFileName);

            dgvDevice.DataSource = devicedt;

            plccmd = new DeltaPlcCommand();

            
         
            dpro = new dataProc(recmsg, socserver,plccmd,label12);
            commTimer.Elapsed += new System.Timers.ElapsedEventHandler(TimeEvent);
            commTimer.AutoReset = true;
            socserver.setDataProc(dpro);

            //打包命令 临时用 读取D0

            tempDPack();
            ***/

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
        private void deviceLstTimer_Tick(object sender, EventArgs e)
        {
            int id = deviceLB.SelectedIndex;
            if (id>-1&&id < ybdtWorkManger.YbdtWorkLst.Count && ybdtWorkManger.YbdtWorkLst.Count>0)
            {
                UpdateWorkInfo(ybdtWorkManger.YbdtWorkLst[id]);
            }
        }

        private void UpdateWorkInfo(YBDTWork yw)
        {
            label25.Text = yw.YbdtWorkInfo.Department;
            label31.Text = yw.YbdtWorkInfo.DanHao;
            label3.Text = yw.StartTime.Date.ToString(); ; 
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
            deviceGroupBox.Text = yw.YbdtWorkInfo.DeviceIP;
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
    }
}

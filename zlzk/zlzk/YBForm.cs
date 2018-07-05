using System;
using System.Text;
using System.Collections;
using System.IO;
using System.Data;
using System.Text.RegularExpressions;
using System.Diagnostics;
using System.Windows.Forms;
using System.Collections.Generic;

namespace zlzk
{
    public partial class YBForm : Form
    {
        CsvStreamReader CSVop = null;
        string deviceDataFileName = null;
        DataTable devicedt=null;
        SocServer socserver = null;
        DeltaPlcCommand plccmd = null;

        System.Timers.Timer commTimer = new System.Timers.Timer(2000);

        dataProc dpro =null;


        YBDTWork ybDtWork;

        public class dataProc : SocServer.DataProc
        {
            RichTextBox rstr;
            SocServer socserver ;
            DeltaPlcCommand plccmd;
            List<byte> mbuffer;
            Label cl;
            public  bool IsSend = false;
            public dataProc(RichTextBox rstr0, SocServer s1, DeltaPlcCommand plccmd0,Label c0)
            {
                rstr = rstr0;
                socserver = s1;
                plccmd = plccmd0;
                mbuffer = new List<byte>();
                cl = c0;

            }
            void SocServer.DataProc.dataProc(byte[] data)
            {                
                //设备连接ID                
                if ((data.Length > 3) && (data[0] == 0x01) && (data[1] == 0x05) && (data[2] == 0x0c))
                {
                    socserver.IsDeviceOk = true;
                }
                if ((data.Length > 3) && (data[0] == 0x01) && (data[1] == 0x10) && (data[2] == 0x40) && (data[3] == 0xDC))
                {
                    socserver.IsSetDRead = true;
                }
                if (socserver.IsSetDRead)
                {
                    if ((data[0] == 0x01) && (data[1] == 0x10))
                    {
                        IsSend = false;
                    }
                     if (IsSend)
                    {
                        socserver.SendMsgByte(plccmd.deltaCmdSendByte(plccmd.byteDeltaCmdSetD0));
                        return;
                    }
                     
                    //DataProcess.ShowInfo(rstr, DataProcess.byteToHexStr(data));
                    socserver.SendMsgByte(plccmd.deltaCmdSendByte(plccmd.byteDeltaCmdRead));
                    cl.Invoke((EventHandler)(delegate
                    {
                        cl.Text = plccmd.Pack4BytesToInt(data[4], data[5]).ToString();

                    }));                  

                }

            }
        }
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
            ybDtWork = new YBDTWork();


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

            

            if (socserver == null)
            {
                socserver = new SocServer();
                socserver.Setsenmsg(sendmsg);
                socserver.SetRecRichBox(recmsg);
                socserver.Setiptext(listBox1);
                socserver.startconn_Click();
                
            }

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

        }

        private void button1_Click(object sender, EventArgs e)
        {
            NPOI.ExportDataToExcelNoDialog(devicedt, deviceDataFileName,null,null);
           //NPOI.ExportDataToExcel(devicedt, "321", null, null);
        }

        private void button3_Click(object sender, EventArgs e)
        {
            // 01 10 10 00 00 01 02 00 00 DC

            dpro.IsSend = true;

            
        }

        private void button4_Click(object sender, EventArgs e)
        {
            if (op1.ShowDialog() == DialogResult.OK)
            {
                //注释部分 与NPOI 不能同时调用同一个文件
                //userex.Open(op1.FileName);
                // userex.ws = userex.GetDefaultSheet();

                devicedt = NPOI.ImportExcel(op1.FileName);

                dgvDevice.DataSource = devicedt;

            }

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (socserver == null)
            {
                socserver = new SocServer();
                socserver.Setsenmsg(sendmsg);
                socserver.SetRecRichBox(recmsg);
                socserver.Setiptext(listBox1);
                socserver.startconn_Click();
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

            socserver.SendMsgByte(plccmd.deltaCmdSendByte(plccmd.byteDeltaCmd));

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
    }
}

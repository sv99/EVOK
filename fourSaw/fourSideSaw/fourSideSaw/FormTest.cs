using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc;
using xjplc.delta;
using xjplc.delta.TCP;

namespace fourSideSaw
{
    public partial class FormTest : Form
    {
        Socket socketDt;
        bool iswrite = false;
        public FormTest()
        {
            InitializeComponent();
           
        }
        void Recive()
        {
            while (true)
            {
                try
                {
                    Application.DoEvents();
                    byte[] buffer = new byte[200];
                    int r = socketDt.Receive(buffer);                    
                    ConstantMethod.ShowInfo(richTextBox1,ConstantMethod.byteToHexStr(buffer));
                    if (!iswrite)
                    {

                        //Thread.Sleep(100);
                       
                    }
                    if (iswrite)
                    {
                        iswrite = false;
                    }

                    socketDt.Send(data);
                }
                catch(Exception ex)
                {
                    ConstantMethod.ShowInfo(richTextBox1, ex.Message);

                }
            }
        }

        public bool connectDevice()
        {
            try
            {
                //创建负责通信的Socket
                socketDt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse("192.168.1.1");
                IPEndPoint point = new IPEndPoint(ip, Convert.ToInt32("502"));
                //获得要连接的远程服务器应用程序的IP地址和端口号
                    
                socketDt.Connect(point);

                ConstantMethod.ShowInfo(richTextBox1, "连接成功");
               
                
        

            }
            catch(Exception ex)
            {
                MessageBox.Show(ex.Message);
                return false;
            }

            return true;

        }
        private void button1_Click(object sender, EventArgs e)
        {          
            
                socketDt.Send(PackWriteSingleBitCmd10008(0));
                ConstantMethod.Delay(500);
                socketDt.Send(PackWriteSingleBitCmd10008(1));
            iswrite = false;

        }

        private void button5_Click(object sender, EventArgs e)
        {
            if (connectDevice())
            {
                button5.Enabled = false;
            }
        }
        public byte[] PackWriteSingleBitCmd10000(int value)
        {
            //00 00 00 00 00 06 01 05 A0 00 FF 00
            List<byte> cmdLst = new List<byte>();
            cmdLst.AddRange(Constant.DTModbusTcpHeader);
            cmdLst.Add(0x01);//站号
            cmdLst.Add(0x06);//功能码

            cmdLst.Add(0x13);//首地址高
            cmdLst.Add(0x88);//首地址低


            if (value == 1)
            {
                cmdLst.Add(0x00);//功能码
                cmdLst.Add(0x01);//功能码
            }
            else
            {
                cmdLst.Add(0x00);//功能码
                cmdLst.Add(0x00);//功能码
            }

            cmdLst.Insert(5, (byte)(cmdLst.Count - 5));

            iswrite = true;
            return cmdLst.ToArray();
        }
        public byte[] PackWriteSingleBitCmd10008(int value)
        {
            //00 00 00 00 00 06 01 05 A0 00 FF 00
            List<byte> cmdLst = new List<byte>();
            cmdLst.AddRange(Constant.DTModbusTcpHeader);
            cmdLst.Add(0x01);//站号
            cmdLst.Add(0x06);//功能码

            cmdLst.Add(0x13);//首地址高
            cmdLst.Add(0x8c);//首地址低


            if (value == 1)
            {
                cmdLst.Add(0x00);//功能码
                cmdLst.Add(0x01);//功能码
            }
            else
            {
                cmdLst.Add(0x00);//功能码
                cmdLst.Add(0x00);//功能码
            }

            cmdLst.Insert(5, (byte)(cmdLst.Count - 5));
            iswrite = true;

            return cmdLst.ToArray();
        }
        public byte[] PackWriteMultipleCmd2500()
        {
            List<byte> cmdLst = new List<byte>();

            cmdLst.AddRange(Constant.DTModbusTcpHeader);
            cmdLst.Add(0x01);//站号
            cmdLst.Add(0x10);//功能码

            cmdLst.Add(0x09);//首地址高
            cmdLst.Add(0xC4);//首地址低

            cmdLst.Add(0x00);//功能码
            cmdLst.Add(0x04);//功能码

            cmdLst.Add(0x08);//功能码


            string width = textBox3.Text.Trim();
            string length = textBox4.Text.Trim();

            int lengthInt = 0;
            int widthInt = 0;



            if (int.TryParse(length, out lengthInt))
            {
                long vallue16 = (lengthInt & 0xFFFF0000) >> 16;



                int addr_high = (lengthInt & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high);

                int addr_low = lengthInt & 0xFF;
                cmdLst.Add((byte)addr_low);


                int addr_high0 = ((int)vallue16 & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high0);

                int addr_low0 = (int)vallue16 & 0xFF;
                cmdLst.Add((byte)addr_low0);

            }
            else return null;

            if (int.TryParse(width, out widthInt))
            {
                long vallue16 = (widthInt & 0xFFFF0000) >> 16;



                int addr_high = (widthInt & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high);
                int addr_low = widthInt & 0xFF;
                cmdLst.Add((byte)addr_low);

                int addr_high0 = ((int)vallue16 & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high0);

                int addr_low0 = (int)vallue16 & 0xFF;
                cmdLst.Add((byte)addr_low0);


            }
            else return null;

            cmdLst.Insert(5, (byte)(cmdLst.Count - 5));

            iswrite = true;
            return cmdLst.ToArray();
        }
        public byte[] PackWriteMultipleCmd2600()
        {
            List<byte> cmdLst = new List<byte>();

            cmdLst.AddRange(Constant.DTModbusTcpHeader);
            cmdLst.Add(0x01);//站号
            cmdLst.Add(0x10);//功能码

            cmdLst.Add(0x0A);//首地址高
            cmdLst.Add(0x28);//首地址低

            cmdLst.Add(0x00);//功能码
            cmdLst.Add(0x04);//功能码

            cmdLst.Add(0x08);//功能码


            string width = textBox2.Text.Trim();
            string length = textBox1.Text.Trim();
            int lengthInt = 0;
            int widthInt = 0;



            if (int.TryParse(length, out lengthInt))
            {
                long vallue16 = (lengthInt & 0xFFFF0000) >> 16;



                int addr_high = (lengthInt & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high);

                int addr_low = lengthInt & 0xFF;
                cmdLst.Add((byte)addr_low);


                int addr_high0 = ((int)vallue16 & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high0);

                int addr_low0 = (int)vallue16 & 0xFF;
                cmdLst.Add((byte)addr_low0);

            }
            else return null;

            if (int.TryParse(width, out widthInt))
            {
                long vallue16 = (widthInt & 0xFFFF0000) >> 16;



                int addr_high = (widthInt & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high);
                int addr_low = widthInt & 0xFF;
                cmdLst.Add((byte)addr_low);

                int addr_high0 = ((int)vallue16 & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high0);

                int addr_low0 = (int)vallue16 & 0xFF;
                cmdLst.Add((byte)addr_low0);


            }
            else return null;

            cmdLst.Insert(5, (byte)(cmdLst.Count - 5));

            iswrite = true;
            return cmdLst.ToArray();
        }
        public byte[] PackWriteMultipleCmd2504()
        {
            List<byte> cmdLst = new List<byte>();

            cmdLst.AddRange(Constant.DTModbusTcpHeaderFake);
            cmdLst.Add(0x01);//站号
            cmdLst.Add(0x10);//功能码

            cmdLst.Add(0x09);//首地址高
            cmdLst.Add(0xC8);//首地址低

            cmdLst.Add(0x00);//功能码
            cmdLst.Add(0x04);//功能码

            cmdLst.Add(0x08);//功能码


            string width = textBox2.Text.Trim();
            string length= textBox1.Text.Trim();
            int lengthInt = 0;
            int widthInt = 0;



            if (int.TryParse(length, out lengthInt))
            {
                long vallue16 = (lengthInt & 0xFFFF0000) >> 16;
             

                int addr_high = (lengthInt & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high);

                int addr_low = lengthInt & 0xFF;
                cmdLst.Add((byte)addr_low);


                int addr_high0 = ((int)vallue16 & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high0);

                int addr_low0 = (int)vallue16 & 0xFF;
                cmdLst.Add((byte)addr_low0);

            }
            else return null;

            if (int.TryParse(width, out widthInt))
            {
                long vallue16 = (widthInt & 0xFFFF0000) >> 16;

               

                int addr_high = (widthInt & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high);
                int addr_low = widthInt & 0xFF;
                cmdLst.Add((byte)addr_low);

                int addr_high0 = ((int)vallue16 & 0xFF00) >> 8;
                cmdLst.Add((byte)addr_high0);

                int addr_low0 = (int)vallue16 & 0xFF;
                cmdLst.Add((byte)addr_low0);


            }
            else return null;

            cmdLst.Insert(5, (byte)(cmdLst.Count-5));


            return cmdLst.ToArray();
        }
        private void button4_Click(object sender, EventArgs e)
        {

            try
            {
                socketDt.Send(PackWriteMultipleCmd2500());
            
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        
        }
        byte[] data = { 0x00, 0x00, 0x00, 0x00, 0x00, 0x06, 0x01, 0x03, 0x00, 0x00, 0x00, 0x02 };
        private void button6_Click(object sender, EventArgs e)
        {
                    
            try
            {
                socketDt.Send(PackWriteMultipleCmd2504());
                iswrite = true;
                byte[] buffer = new byte[200];
                int r = socketDt.Receive(buffer);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
           

        }

        private void button3_Click(object sender, EventArgs e)
        {
          

                socketDt.Send(PackWriteSingleBitCmd10000(0));
                ConstantMethod.Delay(500);
                socketDt.Send(PackWriteSingleBitCmd10000(1));
          

        }
        public void ReadDataTest()
        {
           //根据手册来
            int[] addr  = { 0, 1, 2 ,100};
            int[] id    = { Constant.MBAddrId, Constant.MBAddrId, Constant.MBAddrId, Constant.MBAddrId };
            int[] count = { 2, 2, 2, 4 };

            List<byte> cmdInLst = new List<byte>(); //反馈可变

            ConstantMethod.ShowInfo(richTextBox1, ConstantMethod.byteToHexStr(DTTcpCmdPackAndDataUnpack.PackReadByteCmd(addr, id, count, cmdInLst)));

            ConstantMethod.ShowInfo(richTextBox1, ConstantMethod.byteToHexStr(cmdInLst.ToArray()));

        }
        public void ReadBitTest()
        {
            //发送

            //  C7 97 00 00 00 19
            //01 43 01
            //00 12  以下是地址和 读取个数
            //01 C0 20 00 00 01
            //01 C0 20 05 00 01
            //01 C0 20 2D 00 01
            //73 86

            //回复：
            //AE 00 00 00 
            // 00 0E 
            // 01 43 01
            // 00 09      以下是值
            // 00 01 00 
            // 00 01 01 
            // 00 01 01
            int[] addr = {0,5,45};
            int[] id = {Constant.MXAddrId, Constant.MXAddrId, Constant.MXAddrId};
            int[] count = { 9, 1,1 };
            List<byte> cmdInLst = new List<byte>(); //反馈可变

            ConstantMethod.ShowInfo(richTextBox1,ConstantMethod.byteToHexStr(DTTcpCmdPackAndDataUnpack.PackReadBitCmd(addr, id, count, cmdInLst)));

            ConstantMethod.ShowInfo(richTextBox1, ConstantMethod.byteToHexStr(cmdInLst.ToArray()));

        }
        public void WriteByteTest()
        {
            List<byte[]> value = new List<byte[]>();
            byte[] value0 = { 100, 0 };
            byte[] value1 = { 200, 0 };
            byte[] value2 = { 0x2c, 0x01 };
            value.Add(value0);
            value.Add(value1);
            value.Add(value2);
            int[] addr = { 0, 5, 9 };
            int[] id = { Constant.MBAddrId, Constant.MBAddrId, Constant.MBAddrId };
            int[] count = { 2, 2, 2 };
            List<byte> cmdInLst = new List<byte>(); //反馈可变

            ConstantMethod.ShowInfo(richTextBox1, ConstantMethod.byteToHexStr(DTTcpCmdPackAndDataUnpack.PackWriteByteCmd(addr, id, count, value,cmdInLst)));

            ConstantMethod.ShowInfo(richTextBox1, ConstantMethod.byteToHexStr(cmdInLst.ToArray()));




        }

        public void WriteBitTest()
        {
            List<byte[]> value = new List<byte[]>();
            byte[] value0 = { 0x00,0x00};
            byte[] value1 = { 0 };
            byte[] value2 = { 0 };
            value.Add(value0);
            value.Add(value1);
            value.Add(value2);

            int[] addr = { 45, 20,0};
            int[] id = { Constant.MXAddrId, Constant.MXAddrId, Constant.MXAddrId };
            int[] count = { 15, 1, 1 };

            List<byte> cmdInLst = new List<byte>(); //反馈可变
            dtClient.CmdOut = DTTcpCmdPackAndDataUnpack.PackWriteBitCmd(addr, id, count, value, cmdInLst);
            dtClient.sendTestData();
           
           ConstantMethod.ShowInfo(richTextBox1, ConstantMethod.byteToHexStr(dtClient.CmdOut));

           ConstantMethod.ShowInfo(richTextBox1, ConstantMethod.byteToHexStr(cmdInLst.ToArray()));

         

        }
        EvokDTTcpWork dtwork;
        public void  dttcpWorkTest()
        {
            dtwork = new EvokDTTcpWork();                    
        }
        private void button2_Click(object sender, EventArgs e)
        {
            //List<int> ssTest = new List<int>();

            //ssTest.Add(1);
            //ssTest.Add(2);
            //ssTest.Add(2);
            //ssTest.Add(2);
            //ssTest.Add(2);
            //ssTest.ToArray()[0] = 90;
            //int[] dataArray= {0xcc,0x00,0x00,0x00,0x00};
            //dataArray = ssTest.ToArray();
            //dataArray[0] = 9;

            //dataArray.Reverse();
          //  int s =BitConverter.ToInt16(dataArray, 0);
            //dttcpWorkTest();
            //WriteBitTest();
            // ReadDataTest();
            // ReadBitTest();
            // WriteByteTest();
            //DTModbusTcpCmdPackAndDataUnpack.PackReadDCmd(0,Constant.MWAddrId,2);
            // DTModbusTcpCmdPackAndDataUnpack.PackWriteSingleDCmd(0, 0x0100,Constant.MWAddrId);
            // DTModbusTcpCmdPackAndDataUnpack.PackReadBitCmd(16, Constant.QXAddrId, 13);
            //int[] value = { 0x0100, 0x0200 };
            //DTModbusTcpCmdPackAndDataUnpack.PackSetMultipleDCmd(0, Constant.QWAddrId,value);
            //int[] addr = {0,1,2,100};
            //int[] id = {Constant.MBAddrId, Constant.MBAddrId, Constant.MBAddrId, Constant.MBAddrId };
            //int[] count = { 2, 2,2,1 };
            //// DTTcpCmdPackAndDataUnpack.PackReadBitCmd(addr, id, count);
            //DTTcpCmdPackAndDataUnpack.PackReadByteCmd(addr, id, count);
            /***测试写寄存器
            List<byte[]> value = new List<byte[]>();
            byte[] value0 = { 100, 0 };
            byte[] value1 = { 200, 0 };
            byte[] value2 = { 0x2c,0x01};
            value.Add(value0);
            value.Add(value1);
            value.Add(value2);
            int[] addr = { 0, 5,9 };
            int[] id = { Constant.MBAddrId, Constant.MBAddrId, Constant.MBAddrId };
            int[] count = { 2, 2, 2};

            DTTcpCmdPackAndDataUnpack.PackWriteByteCmd(addr, id, count, value);
            ***/
            /****
            List<byte[]> value = new List<byte[]>();
            byte[] value0 = {1};
            byte[] value1 = {1};
            byte[] value2 = {1};
            value.Add(value0);
            value.Add(value1);
            value.Add(value2);

            int[] addr = { 8, 45, 0XFFFF8 };
            int[] id = { Constant.MXAddrId, Constant.MXAddrId, Constant.MXAddrId };
            int[] count = { 1, 1, 1 };

            DTTcpCmdPackAndDataUnpack.PackWriteBitCmd(addr, id, count, value);
            ***/

        }

        private void richTextBox1_TextChanged(object sender, EventArgs e)
        {
            if (richTextBox1.Lines.Count() > 100) richTextBox1.Clear();
        }

        private void button7_Click(object sender, EventArgs e)
        {
            //开启一个新的线程不停的接收服务端发来的消息
            Thread th = new Thread(Recive);
            th.IsBackground = true;
            th.Start();
        }
        public void RestartDevice()
        {
            if (!dtwork.RestartDevice(Constant.AutoPage))
            {
                

                    MessageBox.Show(Constant.ConnectMachineFail);
                    Environment.Exit(0);
                
            }
        }
        DTTcpClientManager dtClient = new DTTcpClientManager(ConstantMethod.LoadServerParam(Constant.ConfigServerPortFilePath));
        private void button8_Click(object sender, EventArgs e)
        {

            RestartDevice();
        }

        private void timer1_Tick(object sender, EventArgs e)
        {
            if (dtwork == null) return;
            if (dtwork.DeviceStatus)
            {
                button2.BackColor = Color.Green;
            }
            else
            {
                button2.BackColor = Color.Red;
            }
        }
    }
}

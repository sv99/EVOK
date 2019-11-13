using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc.easyComm
{
    public class easycomMangager
    { 

        SerialPortListener m_SerialPort = null;
        public SerialPortListener SerialPort
        {
            get { return m_SerialPort; }
            set { m_SerialPort = value; }
        }

        //串口数据存储
        List<byte> m_buffer = null;

        //串口连接定时
        //定时器 更新 页面 检测通讯是否正常    
        System.Timers.Timer ErrorConnTimer = null;// new System.Timers.Timer(500);
        //串口连接超时次数
        int ErrorConnCount = 0;

        bool status;
        public CommEventArgs DataProcessEventArgs { get; private set; }
        private void openPort()
        {
            if (SerialPort.Start())
            {
                status = true;
            }
            else
            {
                status = false;
            }
        }

        public easycomMangager(PortParam portparam)
        {
            SerialPort = new SerialPortListener(portparam.m_portName, portparam.m_baudRate);
            SerialPort.PortName = portparam.m_portName;
            SerialPort.BaudRate = portparam.m_baudRate;
            SerialPort.DataBits = portparam.m_dataBits;
            SerialPort.StopBits = portparam.m_stopbits;
            SerialPort.Parity = portparam.m_parity;
            SerialPort.Handshake = portparam.m_handshake;
            SerialPort.ReceivedBytesThreshold = portparam.m_receivedBytesThreshold;
            SerialPort.ReceiveTimeout = portparam.m_receiveTimeout;
            SerialPort.SendInterval = portparam.m_sendInterval;
            SerialPort.ReadBufferSize = portparam.m_readBufferSize;

            SerialPort.OnSerialPortReceived += new OnReceivedData(XJPLC_SerialPort_Received);

            m_buffer = new List<byte>();

            ErrorConnTimer = new System.Timers.Timer(Constant.XJConnectTimeOut);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            ErrorConnTimer.Enabled = false;

            ErrorConnTimer.AutoReset = true;

            ErrorConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(ErrorConnTimerEvent);

            ErrorConnCount = 0;

            DataProcessEventArgs = new CommEventArgs();

            openPort();


        }
        //处理数据 委托
        public event commDataProcess EventDataProcess;//利用委托来声明事件

        private void ErrorConnTimerEvent(object sender, EventArgs e)
        {

        }

        public void WriteData(byte[] data)
        {
            //这里可以增加发送命令延时
            ConstantMethod.Delay(50);
            SerialPort.Send(data.ToArray());

        }

        void XJPLC_SerialPort_Received(object sender, SerialPortEvents e)
        {

            if (e.BufferData != null)
            {
                m_buffer.AddRange(e.BufferData);

                if (
                    m_buffer.Count > 3
                    && m_buffer[0] == CmdNewPack.header
                    && m_buffer[m_buffer.Count - 1] == CmdNewPack.tail[1]
                    && m_buffer[m_buffer.Count - 2] == CmdNewPack.tail[0]
                    )
                {

                    //返回命令统一都是0X01开头 如果不是则移除             
                    DataProcessEventArgs.Byte_buffer = m_buffer.ToArray();
                    EventDataProcess(this, DataProcessEventArgs);
                    e.BufferData = null;
                    m_buffer.Clear();
                }
            }
        }

        public class CmdNewPack
        {

            public static int modeAbs = 1;
            public static int modeRel = 0;
            public static byte[] tail = { 0x0d, 0x0a };
            public static byte header = 0x3A;
            public static int modeT = 1;
            public static int modeS = 2;

            public static int dirP = 1;
            public static int dirN = 0;

            public static byte controlAddress = 0x81;
            public static byte motorAddress = 0x01;

            public static byte[] zdqj = { 0x01, 0x01 };
            public static byte[] zdht = { 0x01, 0x02 };
            public static byte[] zdzz = { 0x01, 0x03 };
            public static byte[] zdyz = { 0x01, 0x04 };
            public static byte[] zdstop = { 0x01, 0x05 };

            public static byte[] cambbAdd = { 0x02, 0x01 };
            public static byte[] cambbDec = { 0x02, 0x02 };
            public static byte[] cambjAdd = { 0x02, 0x03 };
            public static byte[] cambjDec = { 0x02, 0x04 };
            public static byte[] camstop = { 0x02, 0x05 };


            public static byte[] ytUp = { 0x03, 0x01 };
            public static byte[] ytDown = { 0x03, 0x02 };
            public static byte[] ytLeft = { 0x03, 0x03 };
            public static byte[] ytRight = { 0x03, 0x04 };
            public static byte[] ytReset = { 0x03, 0x05 };
            public static byte[] ytstop = { 0x03, 0x06 };

            public static byte[] tgUp = { 0x04, 0x01 };
            public static byte[] tgDown = { 0x04, 0x02 };
            public static byte[] tgstop = { 0x04, 0x03 };

            public static byte[] cwOpen = { 0x05, 0x01 };
            public static byte[] cwClose = { 0x05, 0x02 };

            public static byte[] camSnap = { 0x06, 0x01 };

            public static byte[] lightYAdd = { 0x07, 0x01 };
            public static byte[] lightYDec = { 0x07, 0x02 };

            public static byte[] lightJAdd = { 0x08, 0x01 };
            public static byte[] lightJDec = { 0x08, 0x02 };

            public static byte[] speedAdd = { 0x09, 0x01 };
            public static byte[] speedDec = { 0x09, 0x02 };

            public static byte[] jxpClear = { 0x0a, 0x01 };

            public static byte[] shouxian = { 0x0b, 0x01 };

            public static byte[] heart = { 0xbb, 0xbb };
            public static byte[] connect = { 0xaa, 0xaa };


        }

    }

}

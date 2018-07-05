
/********************************************************************
	created:	2018/05/21
	created:	21:5:2018   11:19
	filename: 	e:\project\2018\中意木工\新版\plc测试\xjplc\xjplc\serialportlistener .cs
	file path:	e:\project\2018\中意木工\新版\plc测试\xjplc\xjplc
	file base:	serialportlistener 
	file ext:	cs
	author:		王均晖
	
	purpose: 负责与PLC 数据的发送和接收等管理	
*********************************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Threading;

namespace xjplc
{


    //委托处理结果
    public delegate void HandResult(object sender, SerialPortEvents e);

    //委托处理接收数据完成
    public delegate void OnReceivedData(object sender,SerialPortEvents e);

    //发送数据回调
    public delegate void OnSendData(object sender,SerialPortEvents e);
    public class SerialPortListener
    {

        #region 成员变量

        //定义接收区数据
        private List<byte> m_buffer = null;

        
        public event HandResult SerialPortResult;

        public event OnReceivedData OnSerialPortReceived;

        public event OnSendData OnSerialPortSend;

        delegate void AsyncCallResult(byte[] data);

        delegate void AsyncSend(byte[] data);

        bool m_isReceiving = false;

        //定义串口对象 

        SerialPort m_serialPort = null;
        public System.IO.Ports.SerialPort m_SerialPort
        {
            get { return m_serialPort; }
            set { m_serialPort = value; }
        }

        //结束符号
        char m_endChar = '\0';

        public char EndChar
        {
            get { return m_endChar; }
            set { m_endChar = value; }
        }

        //发送缓冲区大小

        int m_writeBufferSize = 4096;
        public int WriteBufferSize
        {
            get {return m_writeBufferSize; }
            set {m_writeBufferSize= value; }
        }

        //是否启用结束符
        bool m_useEndChar = false;
        public bool UseEndChar
        {
            get {return m_useEndChar; }
            set {m_useEndChar= value; }
        }

        //发送时间间隔
        int m_sendInterval = 100;

        public int SendInterval
        {
            get {return m_sendInterval; }
            set {m_sendInterval= value; }
        }

        //连续接收超时时间
        int m_receiveTimeout = 500;

        public int ReceiveTimeout
        {
            get { return m_receiveTimeout; }
            set { m_receiveTimeout = value; }
        }

        //最后接收时间
        DateTime m_lastReceiveTime;

        public DateTime LastReceiveTime
        {
            get {return m_lastReceiveTime; }
        }

        

        //串口名称
        private string m_portName = "COM1";
        public string PortName
        {
            get { return m_portName; }
            set { m_portName = value; }
        }

        //波特率
        private int m_baudRate = 9600;
        public int BaudRate
        {
            get { return m_baudRate; }
            set { m_baudRate = value; }
        }


        //停止位长度
        private StopBits m_stopBits = StopBits.None;
        public StopBits StopBits
        {
            get { return m_stopBits; }
            set { m_stopBits = value; }
        }

        //奇偶校验
        private Parity m_parity = Parity.None;
        public System.IO.Ports.Parity Parity
        {
            get { return m_parity; }
            set { m_parity = value; }
        }

        //控制协议
        private Handshake m_handshake = Handshake.None;
        public System.IO.Ports.Handshake Handshake
        {
            get { return m_handshake; }
            set { m_handshake = value; }
        }
        //数据长度
        private int  m_dataBits =8;
        public int DataBits
        {
            get { return m_dataBits; }
            set { m_dataBits = value; }
        }

        //缓冲区大小
        private int m_readBufferSize = 4;
        public int ReadBufferSize
        {
            get { return m_readBufferSize; }
            set { m_readBufferSize = value; }
        }

        //输入缓冲区的字节数
        private int m_receivedBytesThreshold = 1;
        public int ReceivedBytesThreshold
        {
            get { return m_receivedBytesThreshold; }
            set { m_receivedBytesThreshold = value; }
        }

        //获取当前是否打开
        public bool IsOpen { get { return m_SerialPort == null ? false : m_SerialPort.IsOpen; } }
        #endregion
        //构造函数

        #region 构造方法
        public SerialPortListener(string portName, int baudRate)
        {
            m_portName = portName;
            m_baudRate = baudRate;
            m_buffer = new List<byte>();
            m_SerialPort = new SerialPort(portName,BaudRate);
            m_SerialPort.DataReceived += new SerialDataReceivedEventHandler(SerialPort_DataReceived);
        }
        public SerialPortListener()
        {
            
           

        }
        #endregion

        #region 开启关闭串口
        public bool Start()
        {
            
            try
            {
                //如果之前端口没变 且端口打开的 那就不改了 不去停止
               
                    Stop();
                m_SerialPort.PortName = m_portName;
                m_SerialPort.BaudRate = m_baudRate;
                m_SerialPort.Parity = m_parity;
                m_SerialPort.StopBits = m_stopBits;
                m_SerialPort.Handshake = m_handshake;
                m_SerialPort.DataBits = m_dataBits;
                m_SerialPort.ReadBufferSize = m_readBufferSize;
                m_SerialPort.ReceivedBytesThreshold = m_receivedBytesThreshold;
                m_SerialPort.WriteBufferSize = m_writeBufferSize;
                    if (!m_SerialPort.IsOpen)
                    m_SerialPort.Open();
                
                //会出现超时现象 死循环 容易造成内存泄露
               //m_threadMonitor = new Thread(new ThreadStart(SerialPortMonitor));
               //m_threadMonitor.IsBackground = true;
               //m_threadMonitor.Start();
                return true;
            }

            catch
            {
                throw new SerialPortException(string.Format("无法打开串口:{0}", m_SerialPort.PortName));
            }
        }

        public void Stop()
        {
            //防止上次数据还在传输
            ConstantMethod.Delay(100);
            m_SerialPort.Close();
            m_buffer.Clear();
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        #endregion

        #region 接收数据
        //接收数据
        void SerialPort_DataReceived(object sender,EventArgs e)
        {
            StartReceive();      
            int availCount = m_SerialPort.BytesToRead;
            byte[] bs = new byte[availCount];
            bool bend = false;
            for (int i = 0; i < availCount;i++)
            {
                byte b = (byte)m_SerialPort.ReadByte();
                if (m_useEndChar && b == m_endChar)
                {
                   
                    bend = true;

                    break;
                }
                bs[i] = b;
            }
            lock (m_buffer) m_buffer.AddRange(bs);
          
            EndReceive(bs);
            if (bend) CallEndResult();
        }
       
        /// <summary>  
        /// 开始接收  
        /// </summary>  
        void StartReceive()
        {
            if (!m_isReceiving)
                m_isReceiving = true;
        }

        void EndReceive(byte[] bs)
        {
            //设置最后接收时间  
            m_lastReceiveTime = DateTime.Now;
            m_isReceiving = false;
            if (OnSerialPortReceived != null
                && bs != null
                && bs.Length != 0)
            {
                AsyncCallResult call = new AsyncCallResult(CalReceived);
                call.BeginInvoke(bs, null, null);
            }
            bs = null;
        }
        void CalReceived(byte[] data)
        {
            OnSerialPortReceived(this,new SerialPortEvents(data));
        }

 
        //处理接手数据完成
        void CallEndResult()
        {
            byte[] result = CopyBuffer(true);
            if (result != null && result.Length > 0)
            {
                if (SerialPortResult != null)
                {
                    //异步通知接收完成  
                    AsyncCallResult call = new AsyncCallResult(CalResult);
                    call.BeginInvoke(result, null, null);
                }
            }
            result = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }

        void CalResult(byte[] data)
        {
            SerialPortResult(this, new SerialPortEvents(data));
            data = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }

        byte[] CopyBuffer(bool isClean = false)
        {
            byte[] bs = null;
            lock (m_buffer)
            {
                bs = new byte[m_buffer.Count];
                m_buffer.CopyTo(bs);
                if (isClean)
                    m_buffer.Clear();
            }
            return bs;
        }

        #endregion

        #region 发送数据 
        public void Send(byte[] bs)
        {
            if (!m_SerialPort.IsOpen)
            {
                throw new SerialPortException("不能在串口关闭状态发送数据");
            }
            
            //异步发送  
            AsyncSend call = new AsyncSend(DoAsyncSend);
           
            call.BeginInvoke(bs, null, null);
           //bs = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }

        void DoAsyncSend(byte[] data)
        {
            int s = 0;

            if (data == null || data.Length == 0)
                return;
            if (m_useEndChar)
            {
                Array.Resize<byte>(ref data, data.Length + 1);
                data[data.Length - 1] = BitConverter.GetBytes(m_endChar)[0];
            }
            if (data.Length > m_writeBufferSize)
            {
                //分包发送  
                int sendCount = 0;
                while (sendCount < data.Length)
                {
                    byte[] sendBytes;
                    if (sendCount + m_writeBufferSize > data.Length)
                    {
                        sendBytes = new byte[data.Length - sendCount];
                        Array.Copy(data, sendCount == 0 ? 0 : sendCount - 1, sendBytes, 0, sendBytes.Length);
                        sendCount = data.Length;
                    }
                    else
                    {
                        sendBytes = new byte[m_writeBufferSize];
                        Array.Copy(data, sendCount == 0 ? 0 : sendCount - 1, sendBytes, 0, sendBytes.Length);
                        sendCount += m_writeBufferSize;
                    }
                    //发送数据  
                    m_SerialPort.Write(sendBytes, 0, sendBytes.Length);
                    s += sendBytes.Length;
                    if (OnSerialPortSend != null)
                        OnSerialPortSend(this, new SerialPortEvents(sendBytes));
                    Array.Resize<byte>(ref sendBytes, 0);
                    Thread.Sleep(m_sendInterval);
                }
            }
            else
            {
                //一次发送  
                m_SerialPort.Write(data, 0, data.Length);
            }
            if (OnSerialPortSend != null)
                OnSerialPortSend(this, new SerialPortEvents(data));
            //清理数据  
            Array.Clear(data, 0, data.Length);
            data = null;
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
        }
        #endregion
    }
    public class SerialPortException : Exception
    {
        public SerialPortException(string message)
            : base(message)
        {
        }
    }
    public class SerialPortEvents : EventArgs
    {
        /// <summary>  
        /// 传递数据  
        /// </summary>  
        private byte[] m_bufferData;
        public byte[] BufferData
        {
            get { return m_bufferData; }
            set { m_bufferData = value; }
        }

        public SerialPortEvents()
            : base()
        {
        }

        public SerialPortEvents(byte[] data)
            : base()
        {
            m_bufferData = data;
        }

    }
}

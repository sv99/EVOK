using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc.tanuo
{
    class easyCom
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

            public easyCom(PortParam portparam)
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

        }
    }

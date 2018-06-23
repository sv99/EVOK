/********************************************************************
	created:	2018/05/21
	created:	21:5:2018   11:04
	filename: 	E:\project\2018\中意木工\新版\plc测试\xjplc\xjplc\CommManager.cs
	file path:	E:\project\2018\中意木工\新版\plc测试\xjplc\xjplc
	file base:	CommManager
	file ext:	cs
	author:		王均晖
	
	purpose:	此类主要集中处理与PLC的通信 数据简单转化等
*********************************************************************/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO.Ports;
using System.Windows.Forms;


namespace xjplc
{
   
   
   public  class XJCommManager
    {
        //串口
        SerialPortListener m_SerialPort = null;
        //串口数据存储
        List<byte>         m_buffer = null;
        //串口连接定时
        //定时器 更新 页面 检测通讯是否正常    
        System.Timers.Timer ErrorConnTimer = null;// new System.Timers.Timer(500);
        //串口连接超时次数
        int ErrorConnCount=0;
        //这个参数只能在通讯过程中 去改动 其他地方改动的话 会影响错误事件的判断
        //信捷专用命令类
        XJPLCPackCmdAndDataUnpack XJPLCcmd = null;

        DTPLCPackCmdAndDataUnpack DTPLCcmd = null;
        //数据处理好之后 传递到device
        commEventArgs DataProcessEventArgs;

        bool isRePackCmdReadDMDataOut = false;
        bool IsRePackymp = false;
        public bool IsRePackCmdReadDMDataOut
        {
            get { return isRePackCmdReadDMDataOut; }
            set { isRePackCmdReadDMDataOut = value; }
        }
        /// <summary>
        /// 构造函数
        /// 串口配置 
        /// </summary>
        /// <param name="portparam"></param>
        public XJCommManager(PortParam portparam)
        {
            
            m_SerialPort = new SerialPortListener(portparam.m_portName, portparam.m_baudRate);

            m_SerialPort.PortName = portparam.m_portName;
            m_SerialPort.BaudRate = portparam.m_baudRate;
            m_SerialPort.DataBits = portparam.m_dataBits;
            m_SerialPort.StopBits = portparam.m_stopbits;
            m_SerialPort.Parity = portparam.m_parity;
            m_SerialPort.Handshake = portparam.m_handshake;
            m_SerialPort.ReceivedBytesThreshold = portparam.m_receivedBytesThreshold;
            m_SerialPort.ReceiveTimeout = portparam.m_receiveTimeout;
            m_SerialPort.SendInterval = portparam.m_sendInterval;
            m_SerialPort.ReadBufferSize = portparam.m_readBufferSize;

            m_SerialPort.OnSerialPortReceived += new OnReceivedData(XJPLC_SerialPort_Received);

            m_buffer = new List<byte>();     

            ErrorConnTimer = new System.Timers.Timer(100);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            ErrorConnTimer.Enabled = false;

            ErrorConnTimer.AutoReset = true;

            ErrorConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(ErrorConnTimerEvent);
           
            ErrorConnCount = 0;


            DataProcessEventArgs = new commEventArgs();

        } 
        /// <summary>
        /// 设置通讯类的 命令打包工具 应该是从上层调用的类中指定的
        /// </summary>
        /// <param name="xjplccmd"></param>
        public void SetXJPLCcmd(XJPLCPackCmdAndDataUnpack xjplccmd)
        {
            if(xjplccmd != null)
            XJPLCcmd = xjplccmd;

        }

        public void SetDTPLCcmd(DTPLCPackCmdAndDataUnpack xjplccmd)
        {
            if (xjplccmd != null)
                DTPLCcmd = xjplccmd;

        }
        //进行读取工作的标志 false后 通讯不再继续循环
        bool isGoToGetData = false;
        public bool IsGoToGetData
        {
            get { return isGoToGetData; }
            set { isGoToGetData = value; }
        }
        //设备是否准备好 发送了设备是否存在命令后 就可以判断了
        bool isDeviceReady;
        public bool IsDeviceReady
        {
            get { return isDeviceReady; }
            set { isDeviceReady = value; }
        }
        //port 是否正常
        bool status;
        public bool Status
        {
            get { return status; }
            set { status = value; }
        }

        bool isNoConnection;
        public bool IsNoConnection
        {
            get { return isNoConnection; }
            set { isNoConnection = value; }
        }
        //复位所有状态位和缓冲 
        //复位之后假定 硬件端口不会变化，那端口不会关闭
        //
        public void Reset()
        {
            ErrorConnCount = 0;
            ErrorConnTimer.Enabled = false;            
            isDeviceReady = false;
            isGoToGetData = false;
            isWriteCmd    = false;
            m_buffer.Clear();
            ConstantMethod.Delay(150);
            closePort();
            
           
        }
        
        //通讯错误引发的事件
        private void ErrorConnTimerEvent(object sender, EventArgs e)
        {
            if (isNoConnection)
                return;
            ErrorConnCount++;
            //通讯错误次数太多 就直接停了吧
            if (ErrorConnCount < Constant.ErrorConnCountMax && ErrorConnCount > 2)
            {
                GetData();
                return;
            }
            else if (ErrorConnCount > Constant.ErrorConnCountMax)
            {
                Reset();
            }
        }
        /// <summary>
        /// 就当你不小心中断了 去错误时间那里重新连一下
        /// </summary>
        /// <returns></returns>
        public void GetData()
        {
            if (IsGoToGetData)
            {
                if(XJPLCcmd.CmdOut!=null && XJPLCcmd.CmdOut.Count()>0)
                m_SerialPort.Send(XJPLCcmd.CmdOut.ToArray());
            }
        }
              
        /// <summary>
        /// 连接PLC 步骤： 打开串口 发送设备是否存在命令 
        /// </summary>
        /// <returns></returns>
        public bool ConnectMachine()
        {
            Reset();

            //目前只是打开窗口 后续要增加打开串口后 返回协议 是否正确
            openPort();

            //询问设备是否存在 收到的数据 变成 回复的长度
            for (int i = 0; i < 3; i++)
            {
                if (status)
                {
                    SetCmdOut(Constant.XJExistByteOut.ToArray(), Constant.XJExistByteIn.Count());
                    SetCmdIn(Constant.XJExistByteIn);
                    m_SerialPort.Send(XJPLCcmd.CmdOut);                    
                    //等待连接 约300秒         
                    ConstantMethod.Delay(Constant.ReadTimeOut, ref isDeviceReady);

                    if (isDeviceReady)
                    {
                        ErrorConnTimer.Enabled = true;
                        break;
                    }
                }
            }
                                
            

            m_buffer.Clear();

            return  isDeviceReady ;

        } 
        //
        /// <summary>
        /// 打开串口
        /// </summary>
        private void openPort()
        {
            if (m_SerialPort.Start())
            {
                status = true;
            }
            else
            {
                status = false;
            }
        }
        /// <summary>
        /// 关闭串口
        /// </summary>
        private void closePort()
        {
            //设立一个通讯标志位 主动停止 
            m_SerialPort.Stop();
            status = false;
        }


        #region 数据接收处理
        //信捷设备数据返回码长度
        int receivedByteCount = 1000;
        public int ReceivedByteCount
        {
            get { return receivedByteCount; }
            set { receivedByteCount = value; }
        }
        private void SetCmdIn(byte[] cmdbyte)
        {
            XJPLCcmd.CmdIn = cmdbyte;      
        }
        private void SetCmdOut(byte[] cmdbyte, int rececount)
        {
            XJPLCcmd.CmdOut = cmdbyte;
            receivedByteCount = rececount;

        }
        //将数据设置成 正常监控DM区域的命令 比较常用 写成函数
        private void SetCmdOutToCmdReadDMDataOut()
        {
            SetCmdOut(XJPLCcmd.CmdReadDMDataOut, XJPLCcmd.ReceivedDMDataCount);
        }
        /// <summary>
        /// 串口数据接收 针对信捷PLC 
        /// 数据没有结束符 只能通过校验来确认
        /// 信捷PLC 通讯模式 发送一条带DM 区域的指令 返回所有数据
        /// 数据错误需要超时机制来进行恢复 如果超时一定次数 则判断中断 机器不存在
        //  数据长度达到 就进行数据处理判断 同时发送下一组读取命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void XJPLC_SerialPort_Received(object sender, SerialPortEvents e)
        {
           
                if (e.BufferData != null)
                {                    
                    m_buffer.AddRange(e.BufferData);
                    //返回命令统一都是0X01开头 如果不是则移除
                    if (m_buffer[0] != 0x01) m_buffer.RemoveAt(0);         
                    if ((m_buffer.Count > receivedByteCount))               
                        m_buffer.Clear();
                    //收到数据就清空错误指示器，如果长时间通讯错误 肯定会一直往上加
                     ErrorConnCount = 0;
                    
                #region 数据长度合格了
                //数据接收超长了 也清空 数据刚好相等 头几个数据相同
                                
                if (m_buffer.Count == receivedByteCount)
                  {                                           
                        //符合校验规则
                        if (XJPLCPackCmdAndDataUnpack.IsEnd(m_buffer.ToArray()))
                        {
                        
                        if ((!isDeviceReady)
                            &&
                          (ConstantMethod.compareByteStrictly(m_buffer.ToArray(), XJPLCcmd.CmdIn)))
                          {
                            #region 设备未连接 
                            //设备通了 要开始 连接了哦 准备好了
                            //设置标志位 打开监控定时器 设置发送和 接收命令
                            isDeviceReady = true;
                            isGoToGetData = true;                            
                            SetCmdOutToCmdReadDMDataOut();
                            m_buffer.Clear();
                            #endregion
                        }
                        else
                          {
                            
                            //设备连接情况下 是写数据区域命令的反馈 还是读数据区域命令的反馈                           
                            if (isWriteCmd)
                            {
                                #region 设备连接了 发送设置DM区域数据
                                //操做XJPLCcmd.CmdOut 只能在一条主线上做 不能再好多地方不然会出错
                                //这样 写寄存器的数据就要先放在一个缓存寄存命令里
                                SetCmdOut(XJPLCcmd.CmdSetBDREGOut, XJPLCcmd.CmdSetBDREGIn.Count());
                                SetCmdIn(XJPLCcmd.CmdSetBDREGIn);

                                if (ConstantMethod.compareByte(m_buffer.ToArray(), XJPLCcmd.CmdIn))
                                {
                                    isWriteCmd = false;                                  
                                    SetCmdOutToCmdReadDMDataOut();
                                }
                                #endregion 
                            }
                            else
                            {
                                #region 数据处理
                                //device重新打包监控数据命令
                                if (IsRePackCmdReadDMDataOut)
                                {                                  
                                    SetCmdOutToCmdReadDMDataOut();
                                    IsRePackCmdReadDMDataOut = false; 
                                    IsRePackymp=true;
                                    m_buffer.Clear();
                                }  
                                                                                           
                                //处理函数不为空 然后 数据开头也对 那就处理数据呗
                                if ((EventDataProcess != null) )
                                {                                    
                                    if (ConstantMethod.compareByte(m_buffer.ToArray(),Constant.XJReadDataCmdCheck))
                                    {
                                        if (IsRePackymp)
                                        {
                                            IsRePackCmdReadDMDataOut = false;
                                        }
                                        DataProcessEventArgs.Byte_buffer = m_buffer.ToArray();
                                        EventDataProcess(this, DataProcessEventArgs);
                                    }
                                   
                                }
                                #endregion
                            }
                        }
                    }
                      
                    //在读的过程当中 如果遇到要写 那就先写 有没有在优化                  
                    if (IsGoToGetData && isNoConnection ==false)
                    {                      
                        m_SerialPort.Send(XJPLCcmd.CmdOut.ToArray());                       
                    }
                    m_buffer.Clear();
                    #endregion 数据长度合格了
                }
                else
                {                 
                    //收到字节数超出预计长度了   
                    if (m_buffer.Count > receivedByteCount) m_buffer.Clear();
                 }

            }

            ////GC.Collect();
            ////GC.WaitForPendingFinalizers();

        }
        //处理数据 委托
        public event commDataProcess EventDataProcess;//利用委托来声明事件

      
        
        #endregion 数据接收处理
      

        #region 发送数据
        //是否需要写数据
        bool isWriteCmd = false;
        /// <summary>
        /// 串口写数据 打包好 到接收处发送
        /// </summary>
        /// <param name="Addr"></param>
        /// <param name="count"></param>
        /// <param name="value"></param>
        /// <param name="Area"></param>
        /// <returns></returns>
        public bool SetMultipleDArea(int Addr, int count, int[] value, string Area)
        {
            if (isDeviceReady)
            {
                XJPLCcmd.PackCmdSetDREGOut(Addr, count, value, Area);
                //防止前面在写数据 
                while (isWriteCmd)
                {
                    Application.DoEvents();
                }

                isWriteCmd = true;               

                ConstantMethod.DelayWriteCmd(Constant.WriteTimeOut,ref isWriteCmd);

                
                return (!isWriteCmd);
            }
            return false;

        }
        public bool SetMultipleMArea(int Addr, int count, int[] value, string Area)
        {
            if (isDeviceReady)
            {
                XJPLCcmd.PackCmdSetBREGOut(Addr, count, value, Area);

                isWriteCmd = true;

                ConstantMethod.DelayWriteCmd(Constant.WriteTimeOut, ref isWriteCmd);

                return (!isWriteCmd);
            }
            return false;

        }
        #endregion 发送数据
    }
}

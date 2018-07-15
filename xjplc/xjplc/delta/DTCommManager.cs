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
   
   
    public class DTCommManager
    {
        //串口
        SerialPortListener m_SerialPortListener = null;
        //串口数据存储
        List<byte>         m_buffer = null;
        //串口连接定时
        //定时器 更新 页面 检测通讯是否正常    
        System.Timers.Timer ErrorConnTimer = null;// new System.Timers.Timer(500);
        //串口连接超时次数
        int ErrorConnCount=0;
        //这个参数只能在通讯过程中 去改动 其他地方改动的话 会影响错误事件的判断
        //台达专用命令类
        DTPLCPackCmdAndDataUnpack DTPLCcmd = null;
        //数据处理好之后 传递到device
        CommEventArgs DataProcessEventArgs;

        bool isRePackCmdReadDMDataOut = false;
        bool IsRePackymp = false;
        public bool IsRePackCmdReadDMDataOut
        {
            get { return isRePackCmdReadDMDataOut; }
            set { isRePackCmdReadDMDataOut = value; }
        }
        bool isSetReadMDataOut = false;
        public bool IsSetReadMDataOut
        {
            get { return isSetReadMDataOut; }
            set { isSetReadMDataOut = value; }
        }
        bool isSetReadDDataOut = false;
        public bool IsSetReadDDataOut
        {
            get { return isSetReadDDataOut; }
            set { isSetReadDDataOut = value; }
        }
        private bool isReadingD;
        public bool IsReadingD
        {
            get { return isReadingD; }
            set { isReadingD = value; }
        }
        private bool isReadingM;
        public bool IsReadingM
        {
            get { return isReadingM; }
            set { isReadingM = value; }
        }
        /// <summary>
        /// 构造函数
        /// 串口配置 
        /// </summary>
        /// <param name="portparam"></param>
        public DTCommManager(PortParam portparam)
        {
            
            m_SerialPortListener = new SerialPortListener(portparam.m_portName, portparam.m_baudRate);

            m_SerialPortListener.PortName = portparam.m_portName;
            m_SerialPortListener.BaudRate = portparam.m_baudRate;
            m_SerialPortListener.DataBits = portparam.m_dataBits;
            m_SerialPortListener.StopBits = portparam.m_stopbits;
            m_SerialPortListener.Parity = portparam.m_parity;
            m_SerialPortListener.Handshake = portparam.m_handshake;
            m_SerialPortListener.ReceivedBytesThreshold = portparam.m_receivedBytesThreshold;
            m_SerialPortListener.ReceiveTimeout = portparam.m_receiveTimeout;
            m_SerialPortListener.SendInterval = portparam.m_sendInterval;
            m_SerialPortListener.ReadBufferSize = portparam.m_readBufferSize;

            m_SerialPortListener.OnSerialPortReceived += new OnReceivedData(DTPLC_SerialPort_Received);

            m_buffer = new List<byte>();     

            ErrorConnTimer = new System.Timers.Timer(Constant.XJConnectTimeOut);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            ErrorConnTimer.Enabled = false;

            ErrorConnTimer.AutoReset = true;

            ErrorConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(ErrorConnTimerEvent);
           
            ErrorConnCount = 0;


            DataProcessEventArgs = new CommEventArgs();

        } 
        /// <summary>
        /// 设置通讯类的 命令打包工具 应该是从上层调用的类中指定的
        /// </summary>
        /// <param name="DTPLCcmd"></param>
        public void SetDTPLCcmd(DTPLCPackCmdAndDataUnpack dtplccmd)
        {
            if(dtplccmd != null)
            DTPLCcmd = dtplccmd;
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
            isSetReadDDataOut = false;
            isSetReadMDataOut = false;
            isReadingD = false;
            isReadingM = false;
            isNoConnection = false;
            
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
                if(DTPLCcmd.CmdOut!=null && DTPLCcmd.CmdOut.Count()>0)
                m_SerialPortListener.Send(DTPLCcmd.CmdOut.ToArray());
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
                    SetCmdOutIn(Constant.DTExistByteOutIn, Constant.DTExistByteOutIn);
                    
                    //这里要进行几个步骤的操作
                    //第一发送存在的命令码
                    //第二发送设置的命令码                  
                    //等待连接 约300秒 
                                      
                    m_SerialPortListener.Send(DTPLCcmd.CmdOut.ToArray());

                    ConstantMethod.Delay(Constant.ReadCommTimeOut, ref isDeviceReady);

                    if (isDeviceReady)
                    {
                        //设备准好后
                        IsSetReadDDataOut = true;
                        //测试先隐藏
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
            if (m_SerialPortListener.Start())
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
            m_SerialPortListener.Stop();
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
            DTPLCcmd.CmdIn = cmdbyte;      
        }
        private void SetCmdOut(byte[] cmdbyte, int rececount)
        {
                                  
            DTPLCcmd.CmdOut = ConstantMethod.DeltaCmdPro(cmdbyte.ToArray());      
            receivedByteCount = rececount;
        }
        /// <summary>
        /// 台达的 根据发送的命令 就知道了
        /// </summary>
        /// <param name="cmdbyte"></param>
        private void SetCmdDOut(byte[] cmdbyte)
        {
            int count = 0;
            DTPLCcmd.CmdOut = ConstantMethod.DeltaCmdPro(cmdbyte.ToArray());
            //20180703 写到这里
            if(cmdbyte.Length>6)
            count = ConstantMethod.Pack4BytesToInt(cmdbyte[5], cmdbyte[4]);
            receivedByteCount = count * 8+11; //根据回复的数据判断

        }
        private void SetCmdMOut(byte[] cmdbyte)
        {
            int count = 0;
            DTPLCcmd.CmdOut = ConstantMethod.DeltaCmdPro(cmdbyte.ToArray());
            //20180703 写到这里
            if (cmdbyte.Length > 6)
                count = ConstantMethod.Pack4BytesToInt(cmdbyte[5], cmdbyte[4]);
             //这里发送的数据如果为0002 代表有4个8位字节 就是2个16位数据
                         
           receivedByteCount = count * 4 + 11; //根据回复的数据判断
        }
        private void SetCmdOutIn(byte[] cmdbyteOut, byte[] cmdbyteIn)
        {

            DTPLCcmd.CmdOut = ConstantMethod.DeltaCmdPro(cmdbyteOut.ToArray());
            DTPLCcmd.CmdIn = ConstantMethod.DeltaCmdPro(cmdbyteIn.ToArray());
            receivedByteCount = DTPLCcmd.CmdIn.Length;
        }
        //将数据设置成 正常监控DM区域的命令 比较常用 写成函数
        private void SetCmdOutToCmdReadDMDataOut()
        {
            SetCmdOut(DTPLCcmd.CmdReadDDataOut, DTPLCcmd.ReceivedDDataCount);
        }
        /// <summary>
        /// 串口数据接收 针对信台达PLC 
        /// 数据没有结束符 只能通过校验来确认
        /// 信捷PLC 通讯模式 发送一条带DM 区域的指令 返回所有数据
        /// 数据错误需要超时机制来进行恢复 如果超时一定次数 则判断中断 机器不存在
        //  数据长度达到 就进行数据处理判断 同时发送下一组读取命令
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void DTPLC_SerialPort_Received(object sender, SerialPortEvents e)
        {
           
                if (e.BufferData != null)
                {              
                 m_buffer.AddRange(e.BufferData);              
                //返回命令统一都是0X3a开头 如果不是则移除
                if (m_buffer[0] != Constant.DTHeader) m_buffer.RemoveAt(0);         
                    if ((m_buffer.Count > receivedByteCount))               
                        m_buffer.Clear();
                    //收到数据就清空错误指示器，如果长时间通讯错误 肯定会一直往上加
                     ErrorConnCount = 0;
                    
                #region 数据长度合格了
                //数据接收超长了 也清空 数据刚好相等 头几个数据相同
                                
                if (m_buffer.Count == receivedByteCount)
                  {
                    //进行转换 去掉第一个0x3a
                    m_buffer = ConstantMethod.DeltaBufferPro(m_buffer);

                    //符合校验规则
                    if (DTPLCPackCmdAndDataUnpack.IsEnd(m_buffer.ToArray()))
                        {
                        if ((!isDeviceReady)
                            && 
                          (ConstantMethod.compareByteStrictly(m_buffer.ToArray(), ConstantMethod.DeltaBufferPro(DTPLCcmd.CmdIn))))
                          {
                            #region 设备未连接 
                            //设备通了 要开始 连接了哦 准备好了
                            //设置标志位 打开监控定时器 设置发送和 接收命令
                            isDeviceReady = true;
                            isGoToGetData = true; 
                            //台达这里还不能发送 因为要设置读取命令先                        
                            //SetCmdOutToCmdReadDMDataOut();
                            m_buffer.Clear();
                            #endregion
                           }
                        else
                          {
                            
                            //设备连接情况下 是写数据区域命令的反馈 还是读数据区域命令的反馈                           
                            if (isWriteCmd)
                            {
                                #region 设备连接了 发送设置DM区域数据
                                //操做DTPLCcmd.CmdOut 只能在一条主线上做 不能再好多地方不然会出错
                                //这样 写寄存器的数据就要先放在一个缓存寄存命令里
                                SetCmdOutIn(DTPLCcmd.CmdSetBDREGOut, DTPLCcmd.CmdSetBDREGIn);
                                if (ConstantMethod.compareByte(m_buffer.ToArray(), DTPLCcmd.CmdSetBDREGIn))
                                {
                                    isWriteCmd = false;                                                                    
                                   //SetCmdOutToCmdReadDMDataOut();
                                }
                                #endregion 
                            }
                            else
                            {
                                #region 设置DM区域读取 这里不同于信捷 因为设置 命令返回还是要检查的 信捷直接就修改命令后 就返回数据了 所以多了个ympIsRePackymp
                                if (IsSetReadDDataOut)
                                {
                                    if (DTPLCcmd.CmdSetReadDDataOut != null)
                                    {
                                        SetCmdOutIn(DTPLCcmd.CmdSetReadDDataOut.ToArray(), DTPLCcmd.CmdSetReadDDataIn.ToArray());

                                        if (ConstantMethod.compareByte(m_buffer.ToArray(), DTPLCcmd.CmdSetReadDDataIn))
                                        {
                                            SetCmdDOut(DTPLCcmd.CmdReadDDataOut.ToArray());
                                            IsSetReadDDataOut = false;
                                            IsSetReadMDataOut = true;
                                            IsReadingD = true;
                                            IsReadingM = false;
                                        }
                                    }
                                    else
                                    {
                                        IsSetReadDDataOut = false;
                                        IsSetReadMDataOut = true;
                                    }
                                }
                                else
                                if (IsSetReadMDataOut)
                                {
                                    if (DTPLCcmd.CmdSetReadMDataOut != null)
                                    {
                                        SetCmdOutIn(DTPLCcmd.CmdSetReadMDataOut.ToArray(), DTPLCcmd.CmdSetReadMDataIn.ToArray());

                                        if (ConstantMethod.compareByte(m_buffer.ToArray(), DTPLCcmd.CmdSetReadMDataIn))
                                        {
                                            SetCmdMOut(DTPLCcmd.CmdReadMDataOut.ToArray());
                                            IsSetReadMDataOut = false;
                                            IsReadingM = true;
                                            IsReadingD = false;
                                        }
                                    }else
                                    IsSetReadMDataOut = false;
                                }
                                else
                                #endregion                                
                                //20180703249写到这里
                                #region 数据处理                                                                                                                                                      
                                //处理函数不为空 然后 数据开头也对 那就处理数据呗
                                if ((EventDataProcess != null))
                                {                             

                                    if (ConstantMethod.compareByte(m_buffer.ToArray(), Constant.DTReadDataCmdCheck))
                                    {
                                        DataProcessEventArgs.Byte_buffer = m_buffer.ToArray();
                                        EventDataProcess(this, DataProcessEventArgs);
                                        ConstantMethod.Delay(50);

                                        if ((IsReadingM) && (DTPLCcmd.CmdReadDDataOut != null))
                                        {
                                            IsReadingD = true;
                                            IsReadingM = false;
                                        }
                                        else
                                        if ((IsReadingD) && (DTPLCcmd.CmdReadMDataOut != null))
                                        {
                                            IsReadingM = true;
                                            IsReadingD = false;
                                        }

                                    }



                                }
                                #endregion
                            }
                        }
                    }
                      
                    //在读的过程当中 如果遇到要写 那就先写 有没有在优化                  
                    if (IsGoToGetData && IsNoConnection ==false)
                    {
                        if (!IsSetReadDDataOut && !IsSetReadMDataOut && !isWriteCmd)
                        {
                            if ((IsReadingM) && (DTPLCcmd.CmdReadMDataOut != null))
                            {
                                SetCmdMOut(DTPLCcmd.CmdReadMDataOut);
                            }
                            else
                            if ((IsReadingD) && (DTPLCcmd.CmdReadDDataOut != null))
                            {
                                SetCmdDOut(DTPLCcmd.CmdReadDDataOut);
                            }
                        }

                        m_SerialPortListener.Send(DTPLCcmd.CmdOut.ToArray());                       
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
                DTPLCcmd.PackCmdSetDREGOut(Addr, count, value, Area);
                //防止前面在写数据 
                while (isWriteCmd)
                {
                    Application.DoEvents();
                }

                isWriteCmd = true;               

                ConstantMethod.DelayWriteCmd(Constant.WriteCommTimeOut,ref isWriteCmd);

                
                return (!isWriteCmd);
            }
            return false;

        }
        public bool SetMultipleMArea(int Addr, int count, int[] value, string Area)
        {
            if (isDeviceReady)
            {
                DTPLCcmd.PackCmdSetBREGOut(Addr, count, value, Area);

                isWriteCmd = true;

                ConstantMethod.DelayWriteCmd(Constant.WriteCommTimeOut, ref isWriteCmd);

                return (!isWriteCmd);
            }
            return false;

        }
        #endregion 发送数据
    }
}

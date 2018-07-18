using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Timers;

namespace xjplc
{
    public class SocServerManager
    {

        public SocServerManager(Socket socClient0)
        {
            SocClient = socClient0;
            DataProcessEventArgs = new SocEventArgs();
            ParameterizedThreadStart pts = new ParameterizedThreadStart(RecMsg);

            Thread trd = new Thread(pts);
            trd.IsBackground = true;

            trd.Start(SocClient);

            ErrorConnTimer = new System.Timers.Timer(Constant.XJConnectTimeOut);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            ErrorConnTimer.Enabled = false;

            ErrorConnTimer.AutoReset = true;

            ErrorConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(ErrorConnTimerEvent);

            ErrorConnCount = 0;
        }

        private void ErrorConnTimerEvent(object sender, ElapsedEventArgs e)
        {
           
            ErrorConnCount++;
            
            //通讯错误次数太多 就直接停了吧
            if (ErrorConnCount < Constant.ErrorConnCountMax && ErrorConnCount > 2)
            {
               //GetData();
                return;
            }
            else if (ErrorConnCount > Constant.ErrorConnCountMax)
            {
                Dispose();
            }
        }

        System.Timers.Timer ErrorConnTimer = null;// new System.Timers.Timer(500);

        YBDTWorkManger workManger;
        public xjplc.YBDTWorkManger WorkManger
        {
            get { return workManger; }
            set { workManger = value; }
        }
        public void Dispose()
        {
            LogManager.WriteProgramLog(Constant.ErrorSocConnection);
            //从通信套接字集合中删除被中断连接的通信套接字对象 
            ErrorConnTimer.Enabled = false;
            if(WorkManger !=null)
            foreach (YBDTWork y in WorkManger.YbdtWorkLst)
            {
                if (y.YbtdDevice.SocClient.RemoteEndPoint.ToString().Equals(this.SocClient.RemoteEndPoint.ToString()))
                {
                    WorkManger.YBDTWorkChanged(Constant.DelWork, y);
                    break;
                }
            }
            try
            {
                SocClient.Shutdown(SocketShutdown.Both);
            }
            catch
            {

            }
            try
            {
                SocClient.Close();
            }
            catch
            {
            }
        }
        bool status;
        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
        Socket socClient;
        public System.Net.Sockets.Socket SocClient
        {
            get { return socClient; }
            set { socClient = value; }
        }

        bool isSetReadMDataOut;
        public bool IsSetReadMDataOut
        {
            get { return isSetReadMDataOut; }
            set { isSetReadMDataOut = value; }
        }
        bool isSetReadDDataOut;
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
        bool isGoToGetData;
        public bool IsGoToGetData
        {
            get { return isGoToGetData; }
            set { isGoToGetData = value; }
        }
        bool isDeviceReady;
        public bool IsDeviceReady
        {
            get { return isDeviceReady; }
            set { isDeviceReady = value; }
        }
        bool isWriteCmd = false;
        public bool IsWriteCmd
        {
            get { return isWriteCmd; }
            set { isWriteCmd = value; }
        }

        public int ErrorConnCount { get; private set; }

        public DTPLCPackCmdAndDataUnpack DTPLCcmd = null;
        public void SetDTPLCcmd(DTPLCPackCmdAndDataUnpack dtplccmd)
        {
            if (dtplccmd != null)
                DTPLCcmd = dtplccmd;
        }

        public bool ConnectMachine()
        {

            for (int i = 0; i < 3; i++)
            {
                if (Status)
                {
                    SetCmdOutIn(Constant.DTExistByteOutIn, Constant.DTExistByteOutIn);
                    SendMsgByte(DTPLCcmd.CmdOut.ToArray());

                    ConstantMethod.Delay(Constant.ReadSocTimeOut, ref isDeviceReady);

                    if (IsDeviceReady)
                    {
                        ErrorConnTimer.Enabled = true;
                        //设备准好后
                        // IsSetReadDDataOut = true;
                        break;
                    }
                }
            }

            return IsDeviceReady;
        }

        private void SetCmdOutIn(byte[] cmdbyteOut, byte[] cmdbyteIn)
        {

            DTPLCcmd.CmdOut = ConstantMethod.DeltaCmdPro(cmdbyteOut.ToArray());
            DTPLCcmd.CmdIn = ConstantMethod.DeltaCmdPro(cmdbyteIn.ToArray());
            receivedByteCount = DTPLCcmd.CmdIn.Length;

        }


        public SocEventArgs DataProcessEventArgs;

        private int receivedByteCount=1000;
        #region 接收数据
        private void SetCmdDOut(byte[] cmdbyte)
        {
            int count = 0;
            DTPLCcmd.CmdOut = ConstantMethod.DeltaCmdPro(cmdbyte.ToArray());
            //20180703 写到这里
            if (cmdbyte.Length > 6)
                count = ConstantMethod.Pack4BytesToInt(cmdbyte[5], cmdbyte[4]);
            receivedByteCount = count * 8 + 11; //根据回复的数据判断

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
        public event socDataProcess EventDataProcess;//利用委托来声明事件
        private void RecMsg(object socketClientPara)
        {
            Socket socketRec = socketClientPara as Socket;

            List<byte> m_buffer = new List<byte>();
            

            while (true)
            {
                Application.DoEvents();

                byte[] arrRecMsg = new byte[1024];

                int length = -1;
                #region 判断数据读取是否正常 不正常就退出 删除自己在设备集合中的位置
                try
                {
                    length = socketRec.Receive(arrRecMsg);
                }
                catch (SocketException ex)
                {


                    LogManager.WriteProgramLog(Constant.ErrorSocConnection+ex.Message);
                    //从通信套接字集合中删除被中断连接的通信套接字对象                    
                    
                    break;
                }
                #endregion

                if (length > 0)
                {

                    

                    byte[] data = new byte[length];

                    Array.Copy(arrRecMsg, data, length);

                    m_buffer.AddRange(data);


                    if (m_buffer[0] == Constant.DTHeader && ((m_buffer[m_buffer.Count - 1] == 0x0a && m_buffer[m_buffer.Count - 2] == 0x0d)))
                    {
                        m_buffer = ConstantMethod.DeltaBufferPro(m_buffer);

                        ErrorConnCount = 0;

                        if ((!IsDeviceReady)
                            &&
                          (ConstantMethod.compareByteStrictly(m_buffer.ToArray(), ConstantMethod.DeltaBufferPro(DTPLCcmd.CmdIn))))
                        {
                            #region 设备未连接 
                            //设备通了 要开始 连接了哦 准备好了
                            //设置标志位 打开监控定时器 设置发送和 接收命令
                            IsDeviceReady = true;
                            IsGoToGetData = true;
                            //台达这里还不能发送 因为要设置读取命令先                        
                            //SetCmdOutToCmdReadDMDataOut();
                            m_buffer.Clear();
                            #endregion
                        }
                        else
                        {

                            //设备连接情况下 
                            //写数据区域命令的反馈 
                            //读数据区域命令的反馈
                            //设置读DM区域的命令反馈
                            if (IsWriteCmd)
                            {
                                #region 设备连接了 发送设置DM区域数据
                                //操做DTPLCcmd.CmdOut 只能在一条主线上做 不能再好多地方不然会出错
                                //这样 写寄存器的数据就要先放在一个缓存寄存命令里
                                SetCmdOutIn(DTPLCcmd.CmdSetBDREGOut, DTPLCcmd.CmdSetBDREGIn);
                                if (ConstantMethod.compareByte(m_buffer.ToArray(), DTPLCcmd.CmdSetBDREGIn))
                                {
                                    IsWriteCmd = false;
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
                                    }
                                    else
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
                        #region 继续发送下一组数据
                        if (IsGoToGetData)
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
                            m_buffer.Clear();
                           
                            SendMsgByte(DTPLCcmd.CmdOut.ToArray());
                        }

                        #endregion
                    }

                }

                Thread.Sleep(10);

                arrRecMsg = null;

            }
        }
        #endregion
        #region 发送数据

        public bool SendMsgByte(byte[] cmdbyte)
        {
            if (SocClient !=null)
            {
                SocClient.Send(cmdbyte);
                return true;
            }
            else return false;

        }   
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

                ConstantMethod.DelayWriteCmd(Constant.WriteCommTimeOut, ref isWriteCmd);


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

        #endregion
    }
    public class SocServer
    {

        Socket socketWatch = null;
        Thread threadWatch = null;

        string ip = ConstantMethod.GetLocalIP();//"192.168.100.3";  //修改为主动获取
         
        int port= Constant.ServerPort;                //暂定为8899                    


        public System.Collections.Generic.List<string> ClientLst
        {
            get { return ClientLst; }
            set { ClientLst = value; }
        }

        public event socketClientChanged newClientIn;//利用委托来声明事件

        public SocServer()
        {           

        }
        
        public void startServer()
        {
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse(ip.Trim());
            IPEndPoint endpoint = new IPEndPoint(ipaddress, port);
            socketWatch.Bind(endpoint);

            socketWatch.Listen(10);

            threadWatch = new Thread(WatchingConn);
            threadWatch.IsBackground = true;
            threadWatch.Start();           

        }

        List<YBDTWork> ybWorkLst;
        public List<xjplc.YBDTWork> YbWorkLst
        {
            get
            {
               return ybWorkLst;
            }
            set
            {
               ybWorkLst = value;
            }
        }

        //这里进行临时的程序搭建
        private void WatchingConn(object obj)
        {

            while (true)
            {
                Application.DoEvents();

                Socket socConn = socketWatch.Accept();

                if (newClientIn != null)
                {
                    newClientIn(this,socConn);
                }
                
                                             
                Thread.Sleep(10);                             
            }

        }
        public void SafeClose()
        {
            if (socketWatch == null)
                return;

            if (!socketWatch.Connected)
                return;

            try
            {
                socketWatch.Shutdown(SocketShutdown.Both);
            }
            catch
            {
            }
            try
            {
                socketWatch.Close();
            }
            catch
            {
            }
        }
                  
    }
}

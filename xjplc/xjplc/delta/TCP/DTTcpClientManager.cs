using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace xjplc.delta.TCP
{
    public class DTTcpClientManager
    {
       Socket socketDt;
        int deviceId=0;
        public int DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }

       ServerInfo serverParam;

       public byte[] CmdOut = Constant.IsDtTcpExitOut;
       public byte[] CmdIn = Constant.IsDtTcpExitIn;

       List<List<byte>> CmdOutLst;
       List<List<byte>> CmdInLst;

       public List<byte>  ReadDCmdOut ;
       public List<byte>  ReadDCmdIn  ;

       public List<byte>  ReadMCmdOut ;
       public List<byte>  ReadMCmdIn ;

       public List<List<byte>> ReadDCmdOutLst;
       public List<List<byte>> ReadDCmdInLst;

       public List<List<byte>> ReadMCmdOutLst;
       public List<List<byte>> ReadMCmdInLst;

       public List<byte>  SetDMCmdOut;
       public List<byte>  SetDMCmdIn;

       public List<byte>  SetMCmdOut;
       public List<byte>  SetMCmdIn;
        //数据存储
        List<byte> m_buffer = null;
        bool isRePackCmdReadDMDataOut = false;

        bool IsRePackymp = false;
        public bool IsRePackCmdReadDMDataOut
        {
            get { return isRePackCmdReadDMDataOut; }
            set { isRePackCmdReadDMDataOut = value; }
        }

        bool isRepackDone;
        public bool IsRepackDone
        {
            get { return isRepackDone; }
            set { isRepackDone = value; }
        }
        //port 是否正常
        bool status;
        public bool Status
        {
            get { return status; }
            set { status = value; }
        }
        RichTextBox showRichTextBox;
        public System.Windows.Forms.RichTextBox ShowRichTextBox
        {
            get { return showRichTextBox; }
            set { showRichTextBox = value; }
        }
        //串口连接定时
        //定时器 更新 页面 检测通讯是否正常    
        System.Timers.Timer ErrorConnTimer = null;// new System.Timers.Timer(500);

        //串口连接超时次数
        int ErrorConnCount = 0;

        //数据处理好之后 传递到device 
        SocEventArgs DataProcessEventArgs;

        //不去检测通讯错误
        bool isNoConnection;
        public bool IsNoConnection
        {
            get { return isNoConnection; }
            set { isNoConnection = value; }
        }

        //设备是否准备好 发送了设备是否存在命令后 就可以判断了
        bool isDeviceReady;
        public bool IsDeviceReady
        {
            get { return isDeviceReady; }
            set { isDeviceReady = value; }
        }    

        Thread recThread;
        public DTTcpClientManager(ServerInfo serverParam0)
        {

            m_buffer = new List<byte>();

            ErrorConnTimer = new System.Timers.Timer(Constant.XJConnectTimeOut);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            ErrorConnTimer.Enabled = false;

            ErrorConnTimer.AutoReset = true;

            ErrorConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(ErrorConnTimerEvent);

            ErrorConnCount = 0;

            DataProcessEventArgs = new SocEventArgs();

            serverParam = serverParam0;

            ReadDCmdOut = new List<byte>();
            ReadDCmdIn = new List<byte>();

            ReadMCmdOut = new List<byte>();
            ReadMCmdIn = new List<byte>();

            SetDMCmdOut = new List<byte>();
            SetDMCmdIn = new List<byte>();

            SetMCmdOut = new List<byte>();
            SetMCmdIn = new List<byte>();

            CmdOutLst = new List<List<byte>>();
            CmdInLst = new List<List<byte>>();

            ReadDCmdOutLst = new List<List<byte>>();;
            ReadDCmdInLst = new List<List<byte>>();;

           ReadMCmdOutLst = new List<List<byte>>();;
           ReadMCmdInLst = new List<List<byte>>();;


    }
        public void SetDeviceId(int id)
        {
            DeviceId = id;
            if (DeviceId == Constant.xzjDeivceId)
            {
                CmdOut = Constant.IsDtAsPlcTcpExitOut;
                CmdIn = Constant.IsDtAsPlcTcpExitIn;
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
              
                ErrorConnCount = 0;
                if (CmdOut != null && CmdOut.Count() > 0) socketDt .Send(CmdOut.ToArray());
            }
        }

        public void Start()
        {
            IsGoToGetData = true;
            status = true;
            ClearBufferCmdOut();
            if (ReadMCmdIn.Count > 0)
            {
                CmdOut = ReadMCmdOut.ToArray();
                CmdIn = ReadMCmdIn.ToArray();
            }
            else
            {
                CmdOut = ReadDCmdOut.ToArray(); 
                CmdIn = ReadDCmdIn.ToArray();
            }
            if (DeviceId == Constant.xzjDeivceId)
            {
                CmdOut = Constant.IsDtAsPlcTcpExitOut;
                CmdIn = Constant.IsDtAsPlcTcpExitIn;
            }
            GetData();


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
                SetReadCmd();
                GetData();
                return;
            }
            else if (ErrorConnCount > Constant.ErrorConnCountMax)
            {
              Reset();
            }
        }
        public void CloseTcpClient()
        {
            try { socketDt.Shutdown(SocketShutdown.Both); } catch { }

            try { socketDt.Close(); } catch { }

            ErrorConnTimer.Enabled = false;

        }
        public bool OpenTcpClient()
        {               
            try
            {
                //如果已经有了 关闭再开就重新开起来
                if (socketDt != null)
                {
                    CloseTcpClient();
                }
                //创建负责通信的Socket
                socketDt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(serverParam.server_Ip);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(serverParam.server_Port));
                //获得要连接的远程服务器应用程序的IP地址和端口号
                socketDt.Connect(point);

                //ConstantMethod.ShowInfo(richTextBox1, "连接成功");
                if (recThread!=null && recThread.IsAlive)
                {
                    recThread.Abort();
                }
                //开启一个新的线程不停的接收服务端发来的消息
                recThread = new Thread(DTPLC_TCPClient_Received);
                recThread.IsBackground = true;
                recThread.Start();
                //开启定时器
                ErrorConnTimer.Enabled = true;
              
                Start();
                return true;
            }
            catch (Exception ex)
            {
               // MessageBox.Show(ex.Message);
                return false;
            }
        }

        public void ClearBufferCmdOut()
        {
            CmdOutLst.Clear();
            CmdInLst.Clear();
            
        }
        private void SetWriteCmd()
        {
            if (SetDMCmdOut.Count > 0)
            {
                ClearBufferCmdOut();
                ClearCmd();
           
                CmdOutLst.Add(SetDMCmdOut);
                CmdInLst.Add(SetDMCmdIn);

                //设置写命令
                CmdOut = SetDMCmdOut.ToArray();
                CmdIn = SetDMCmdIn.ToArray();
                CmdOutLst.RemoveAt(0);
                CmdInLst.RemoveAt(0);
            }
        }
        private void SetReadCmd()
        {
            if (ReadDCmdOut.Count == 0 && 
                ReadMCmdOut.Count == 0 && 
                ReadDCmdOutLst.Count==0&&
                ReadMCmdOutLst.Count == 0
              )
            {

                ClearBufferCmdOut();

                if (DeviceId == Constant.xzjDeivceId)
                {
                    CmdOutLst.Add(Constant.IsDtAsPlcTcpExitOut.ToList<byte>());
                    CmdInLst.Add(Constant.IsDtAsPlcTcpExitIn.ToList<byte>());
                }
                else
                {
                    CmdOutLst.Add(Constant.IsDtTcpExitOut.ToList<byte>());
                    CmdInLst.Add(Constant.IsDtTcpExitIn.ToList<byte>());
                }
            }
            else
            {
                if (CmdOutLst.Count == 0)
                {
                    if (ReadDCmdOutLst.Count > 0)
                    {
                        CmdOutLst.AddRange(ReadDCmdOutLst);
                        CmdInLst.AddRange(ReadDCmdOutLst);
                    }

                    if (ReadMCmdOutLst.Count > 0)
                    {
                        CmdOutLst.AddRange(ReadMCmdOutLst);
                        CmdInLst.AddRange(ReadMCmdInLst);
                    }

                    if (ReadDCmdOut.Count > 0)
                    {
                        CmdOutLst.Add(ReadDCmdOut);
                        CmdInLst.Add(ReadDCmdIn);
                    }
                    if (ReadMCmdOut.Count > 0)
                    {
                        CmdOutLst.Add(ReadMCmdOut);
                        CmdInLst.Add(ReadMCmdIn);
                    }

                }           
            }

            CmdOut = CmdOutLst[0].ToArray();
            CmdIn = CmdInLst[0].ToArray();
            CmdOutLst.RemoveAt(0);
            CmdInLst.RemoveAt(0);

        }
        void DTPLC_TCPClient_Received()
        {
            while (true)
            {
                try
                {
                    Application.DoEvents();
                    byte[] buffer = new byte[1024];
                    int r = socketDt.Receive(buffer);
                    if (r > 0)
                    {                      
                        byte[] array_buffer = new byte[r];
                        Array.Copy(buffer,array_buffer,r);

                        Thread.Sleep(10);
                        if (IsRePackCmdReadDMDataOut)
                        {
                            if (IsRepackDone)
                            {
                                SetReadCmd();
                                //if(ConstantMethod.compareByte(array_buffer, SetDMCmdIn.ToArray())))
                                isRePackCmdReadDMDataOut = false;
                            }
                        }
                        else                     
                        if (isWriteCmd) //如果则写数据
                        {                

                            if (ConstantMethod.compareByte(array_buffer, SetDMCmdIn.ToArray()))
                            {
                                isWriteCmd = false;                           
                            }
                            else
                            {
                                CmdOut = SetDMCmdOut.ToArray();// CmdOutLst[0].ToArray();
                                CmdIn  = SetDMCmdIn.ToArray();// CmdInLst[0].ToArray();                                
                            }                                              
                        }
                        else
                        {
                            if ((EventDataProcess != null))
                            {

                                if (ConstantMethod.compareByte(array_buffer, Constant.IsDtAsPlcTcpExitIn))
                                {
                                   SetReadCmd();
                                }
                                else
                                //判断数据是否正确                             
                                if (array_buffer.Count() == CmdIn.Count())
                                {
                                    SetReadCmd();
                                    isDeviceReady = true;
                                    DataProcessEventArgs.Byte_buffer = array_buffer.ToArray();
                                    EventDataProcess(this, DataProcessEventArgs);
                                }else
                                SetReadCmd();
                            }
                        }
                        GetData();
                        //获取到数据 显示
                       // ConstantMethod.ShowInfo(showRichTextBox, ConstantMethod.byteToHexStr(array_buffer));  
                                                                                  
                    }
                    else
                    {
                        throw new Exception("Error Recevie Data");
                    }                                                                                                                
                }
                catch (Exception ex)
                {

                   // throw new Exception("Error Recevie Data0");
                    // ConstantMethod.ShowInfo(r1, ex.Message);
                }
            }
        }
        public void ClearCmd()
        {
            ReadDCmdIn.Clear();
            ReadMCmdIn.Clear();
            ReadMCmdOut.Clear();
            ReadDCmdOut.Clear();
        }
        private void SendTestData()
        {
            socketDt.Send(CmdOut);
        }
        //进行读取工作的标志 false后 通讯不再继续循环
        bool isGoToGetData = false;
        public bool IsGoToGetData
        {
            get { return isGoToGetData; }
            set { isGoToGetData = value; }
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
        public void Reset()
        {
            status = false;
            ErrorConnCount = 0;
            ErrorConnTimer.Enabled = false;
            isDeviceReady = false;
            isGoToGetData = false;
            IsWriteCmd = false;            
            isReadingD = false;
            isReadingM = false;
            isNoConnection = false;
            m_buffer.Clear();
            ConstantMethod.Delay(150);
            ClearBufferCmdOut();
            CloseTcpClient();
        }

        //是否需要写数据
        bool isWriteCmd = false;
        public bool IsWriteCmd
        {
            get { return isWriteCmd; }
            set { isWriteCmd = value; }
        }

        public void sendTestData()
        {
            isWriteCmd = true;
        }

        //处理数据 委托
        public event socDataProcess EventDataProcess;//利用委托来声明事件

        #region 数据写操作       

        public bool SetMultipleDMArea(int Addr, int count, List<byte[]> value, string Area)
        {
            if (isDeviceReady)
            {
              
                List<int> addr = new List<int>();
                List<int> addressid = new List<int>();
                List<int> count0 = new List<int>();
                int addrid = DTTcpCmdPackAndDataUnpack.GetIntAreaFromStr(Area);
                if (DeviceId == Constant.xzjDeivceId)
                {
                    addrid = XJPLCPackCmdAndDataUnpack.AreaGetFromStr(Area);
                    for (int i = 0; i < count; i++)
                    {
                        if(value.Count>0)
                        addr.Add(Addr + i*(value[0].Count()/2));
                        addressid.Add(addrid);
                        count0.Add(1);
                    }
                }
                else                          
                for (int i = 0; i < count; i++)
                {                    
                    addr.Add(Addr+i);
                    addressid.Add(addrid);
                    count0.Add(1);
                }

                SetDMCmdOut.Clear();
                SetDMCmdIn.Clear();
                if (DeviceId == Constant.xzjDeivceId)
                {
                    if (addrid < Constant.HSD_ID)
                        SetDMCmdOut.AddRange(DTTcpCmdPackAndDataUnpack.AsPlcPackWriteByteCmd(addr.ToArray(), addressid.ToArray(), count0.ToArray(), value, SetDMCmdIn));
                    else
                        SetDMCmdOut.AddRange(DTTcpCmdPackAndDataUnpack.AsPlcPackWriteBitCmd(addr.ToArray(), addressid.ToArray(), count0.ToArray(), value, SetDMCmdIn));

                }
                else
                {
                    if (addrid > Constant.MXAddrId)
                        SetDMCmdOut.AddRange(DTTcpCmdPackAndDataUnpack.PackWriteByteCmd(addr.ToArray(), addressid.ToArray(), count0.ToArray(), value, SetDMCmdIn));
                    else
                        SetDMCmdOut.AddRange(DTTcpCmdPackAndDataUnpack.PackWriteBitCmd(addr.ToArray(), addressid.ToArray(), count0.ToArray(), value, SetDMCmdIn));
                }
                ConstantMethod.Delay(200); //延时一下 防止数据发太快 监控上看不到
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
        


        #endregion

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

/****
 * 基础类解析
 * 基本属性： 设备ID ，设备状态（正常或者错误），端口信息，缓冲区
 *****/
namespace xjplc.TcpDevice
{
    public class TcpDevice
    {

        //缓冲区发送数据
        public byte[] CmdOut = Constant.IsDtTcpExitOut;

        public byte[] CmdIn = Constant.IsDtTcpExitIn;

        //缓冲区即将发送的数据
        public byte[] CmdOutReadyGo  ;

        public byte[] CmdInReadyCome  ;

        //发送队列的数据
        List<List<byte>> cmdOutLst;
        public System.Collections.Generic.List<System.Collections.Generic.List<byte>> CmdOutLst
        {
            get { return cmdOutLst; }
            set { cmdOutLst = value; }
        }
        //接收队列数据
        List<List<byte>> cmdInLst;
        public System.Collections.Generic.List<System.Collections.Generic.List<byte>> CmdInLst
        {
            get { return cmdInLst; }
            set { cmdInLst = value; }
        }
  
        System.Timers.Timer connectWatchTimer;

        Socket socketDt;

        ServerInfo serverParam;

        Thread recThread;
        //网口连接超时次数
        int ErrorConnCount = 0;
        //设备ID
        int deviceId = 0;
        public int DeviceId
        {
            get { return deviceId; }
            set { deviceId = value; }
        }

        string deviceName;
        public string DeviceName
        {
            get { return deviceName; }
            set { deviceName = value; }
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

        //数据处理好之后 传递到device 
        SocEventArgs DataProcessEventArgs;

        //进行读取工作的标志 false后 通讯不再继续循环
        bool isGoToGetData = false;
        public bool IsGoToGetData
        {
            get { return isGoToGetData; }
            set { isGoToGetData = value; }
        }
        public void Dispose()
        {
            status = false;
            ErrorConnCount = 0;
            connectWatchTimer.Enabled = false;
            isDeviceReady = false;
            isGoToGetData = false;

            ConstantMethod.Delay(150);

            ClearBufferCmdOut();

            CloseTcpClient();
          
        }
        public bool Reset()
        {
            status = false;
            ErrorConnCount = 0;
            connectWatchTimer.Enabled = false;
            isDeviceReady = false;
            isGoToGetData = false;
            
            ConstantMethod.Delay(10);

            ClearBufferCmdOut();

            CloseTcpClient();

            return  OpenTcpClient();

        }

        void ClearBufferCmdOut()
        {
            CmdInLst.Clear();
            CmdOutLst.Clear();
        }
        //设备是否准备好 发送了设备是否存在命令后 就可以判断了
        bool isDeviceReady;
        public bool IsDeviceReady
        {
            get { return isDeviceReady; }
            set { isDeviceReady = value; }
        }

        public  void SetReadCmd()
        {
            /**
            if (CmdOutLst.Count > 0 && CmdInLst.Count > 0)
            {
                CmdOut = CmdOutLst[0].ToArray();
                CmdIn = CmdInLst[0].ToArray();
                CmdOutLst.RemoveAt(0);
                CmdInLst.RemoveAt(0);
            }
            else
            {
         ***/
                CmdOut = CmdOutReadyGo;
                CmdIn = CmdInReadyCome;
              
            System.Diagnostics.Debug.WriteLine(ConstantMethod.byteToHexStr(CmdOutReadyGo));
            // }

        }
        public TcpDevice(ServerInfo p0)
        {

            connectWatchTimer = new System.Timers.Timer(500);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            connectWatchTimer.Enabled = false;

            connectWatchTimer.AutoReset = true;

            connectWatchTimer.Elapsed += new System.Timers.ElapsedEventHandler(ErrorConnTimerEvent);

            ErrorConnCount = 0;

            serverParam = p0;

            CmdOutLst = new List<List<byte>>();

            CmdInLst = new List<List<byte>>();

            DataProcessEventArgs = new SocEventArgs();

        }

        //通讯错误引发的事件
        private void ErrorConnTimerEvent(object sender, EventArgs e)
        {

            ErrorConnCount++;
            //通讯错误次数太多 就直接停了吧
            if (ErrorConnCount < Constant.ErrorConnCountMax && ErrorConnCount > 2)
            {
                string ss = DeviceName;
                GetData();
                return;
            }
            else if (ErrorConnCount > Constant.ErrorConnCountMax)
            {
                Reset();
            }
        }
        bool isOnLine = false;

        public void CloseTcpClient()
        {
            try { socketDt.Shutdown(SocketShutdown.Both); } catch { }

            try { socketDt.Close(); } catch { }

            connectWatchTimer.Enabled = false;
            Status = false;
            isOnLine = false;
        }
        void OnConnect(object sender,SocketAsyncEventArgs e)
        {
            isOnLine = true;
        }
        public bool OpenTcpClient()
        {
            try
            {
                if (socketDt != null)
                {
                    CloseTcpClient();
                }
                if (!ConstantMethod.IsNetWorkExist(serverParam.server_Ip)) return false;
                socketDt = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                IPAddress ip = IPAddress.Parse(serverParam.server_Ip);
                IPEndPoint point = new IPEndPoint(ip, int.Parse(serverParam.server_Port));
                //获得要连接的远程服务器应用程序的IP地址和端口号
              //  socketDt.Connect(point);

                //非阻塞连接
                SocketAsyncEventArgs connectArgs = new SocketAsyncEventArgs();
                connectArgs.UserToken = socketDt;
                connectArgs.RemoteEndPoint = point;
                connectArgs.Completed += new EventHandler<SocketAsyncEventArgs>(OnConnect);

                socketDt.ConnectAsync(connectArgs);

                ConstantMethod.Delay(200);

                if (recThread != null && recThread.IsAlive)
                {
                    recThread.Abort();
                }
                if (isOnLine)
                {

                    //开启一个新的线程不停的接收服务端发来的消息
                    recThread = new Thread(DataReceived);
                    recThread.IsBackground = true;
                    recThread.Start();
                    //开启定时器
                    connectWatchTimer.Enabled = true;
                    SetReadCmd();
                    Start();

                    Status = true;
                    
                    return Status;
                }
                return false;

            }
            catch (Exception ex)
            {
               // MessageBox.Show(DeviceName+"无法连接，请检查网络！");
                //ConstantMethod.AppExit();
                return false;
            }

        }
        public void GetData()
        {
            if (IsGoToGetData  && !string.IsNullOrWhiteSpace(serverParam.server_Ip))
            {
                if (CmdOut != null && CmdOut.Count() > 0) socketDt.Send(CmdOut.ToArray());
            }
        }
    

        public void Start()
        {
            IsGoToGetData = true;
            IsDeviceReady = true;           
            ClearBufferCmdOut();
            GetData();
        }
        #region 数据处理

        public event socDataProcess EventDataProcess;//利用委托来声明事件

        void DataReceived()
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
                        Status = true;

                        ErrorConnCount = 0;

                        byte[] array_buffer = new byte[r];
                        Array.Copy(buffer, array_buffer, r);

                        Thread.Sleep(100);

                        DataProcessEventArgs.Byte_buffer = array_buffer.ToArray();

                        if(EventDataProcess != null && DataProcessEventArgs !=null)
                        EventDataProcess(this, DataProcessEventArgs);

                        SetReadCmd();                                             
                        GetData();
                       

                    }
                }
                catch (Exception ex)
                {

                }
            }
        }

        #endregion
    }

    //服务器端 测试用
    public class TcpServerDevice
    {
        Thread threadWatch = null;

        Socket socketWatch = null;


    }
}

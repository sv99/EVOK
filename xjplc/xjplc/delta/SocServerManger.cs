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
namespace xjplc
{
    public class SocServerClient
    {
        public SocServerClient(Socket socClient0)
        {
            SocClient = socClient0;

            ParameterizedThreadStart pts = new ParameterizedThreadStart(RecMsg);

            Thread trd = new Thread(pts);
            trd.IsBackground = true;

            trd.Start(SocClient);
        }
        List<YBDTWork> ybtdWorkLst;
        public System.Collections.Generic.List<xjplc.YBDTWork> YbtdWorkLst
        {
            get { return ybtdWorkLst; }
            set { ybtdWorkLst = value; }
        }
    
        Socket socClient;
        public System.Net.Sockets.Socket SocClient
        {
            get { return socClient; }
            set { socClient = value; }
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

        public commEventArgs DataProcessEventArgs;
        #region 数据处理
        public event commDataProcess EventDataProcess;//利用委托来声明事件
        private void RecMsg(object socketClientPara)
        {
            Socket socketRec = socketClientPara as Socket;

            List<byte> m_buffer = new List<byte>();

            while (true)
            {
                Application.DoEvents();

                byte[] arrRecMsg = new byte[1024];

                int length = -1;

                try
                {
                    length = socketRec.Receive(arrRecMsg);
                }
                catch (SocketException ex)
                {
                    LogManager.WriteProgramLog(Constant.ErrorSocConnection+ex.Message);
                    //从通信套接字集合中删除被中断连接的通信套接字对象 
                    foreach (YBDTWork y in YbtdWorkLst)
                    {
                        if (y.YbtdDevice.SocClient.LocalEndPoint.ToString().Equals(this.SocClient.LocalEndPoint))
                        {
                            YbtdWorkLst.Remove(y);
                            break;
                        }
                    } 
                    

                    break;
                }
           

                if (length > 0)
                {

                    byte[] data = new byte[length];

                    Array.Copy(arrRecMsg, data, length);

                    m_buffer.AddRange(data);

                    if (m_buffer[0] == Constant.DTHeader && ((m_buffer[m_buffer.Count - 1] == 0x0a && m_buffer[m_buffer.Count - 2] == 0x0d)))
                    {
                        if ((EventDataProcess != null))
                        {
                            DataProcessEventArgs.Byte_buffer = m_buffer.ToArray();
                            EventDataProcess(this, DataProcessEventArgs);
                        }
                    }

                }

                Thread.Sleep(10);

                arrRecMsg = null;

            }
        }
        public bool SendMsgByte(string strClientKey, byte[] cmdbyte)
        {
            if (SocClient !=null)
            {
                SocClient.Send(cmdbyte);
                return true;
            }
            else return false;

        }

        #endregion
    }
    public class SocServer
    {

        Socket socketWatch = null;
        Thread threadWatch = null;

        string ip = ConstantMethod.GetLocalIP();//"192.168.100.3";  //修改为主动获取
         
        int port= Constant.ServerPort;                //暂定为8899                    

        List<string> clientLst;
        public System.Collections.Generic.List<string> ClientLst
        {
            get { return clientLst; }
            set { clientLst = value; }
        }


        public SocServer()
        {           

        }
        
        public void startconn_Click()
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
        public System.Collections.Generic.List<xjplc.YBDTWork> YbWorkLst
        {
            get { return ybWorkLst; }
            set { ybWorkLst = value; }
        }

  

        //这里进行临时的程序搭建
        private void WatchingConn(object obj)
        {

            while (true)
            {
                Application.DoEvents();

                Socket socConn = socketWatch.Accept();

                YBDTWork ybdtwork = new YBDTWork(socConn);

                if (YbWorkLst != null)
                {                 
                    ybdtwork.YbtdDevice.YbtdWorkLst = YbWorkLst;
                    YbWorkLst.Add(ybdtwork);
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

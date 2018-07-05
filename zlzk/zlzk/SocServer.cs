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

namespace zlzk
{
    public class SocServer
    {

        Socket socketWatch = null;
        Thread threadWatch = null;
        RichTextBox recmsg = null;
        ListBox iptext = null;
        string ip = DataProcess.GetLocalIP();//"192.168.100.3";  //修改为主动获取 
        string port="8899";                //暂定为8899
        TextBox sendmsg = null;       
        bool isDeviceOk = false;

        bool isSetDRead = false;

        
        public bool IsSetDRead
        {
            get { return isSetDRead; }
            set { isSetDRead = value; }
        }
        private DataProc datapro = null;
        public bool IsDeviceOk
        {
            get { return isDeviceOk; }
            set { isDeviceOk = value; }
        }
        public void setDataProc(DataProc d)
        {
            datapro = d;
        }
        public void SetRecRichBox(RichTextBox recmsg0)
        {
            recmsg = recmsg0;
        }
        public void Setiptext(ListBox iptext0)
        {
            iptext = iptext0;
        }
        public void Setsenmsg(TextBox sendmsg0)
        {
            sendmsg = sendmsg0;
        }
        public SocServer()
        {
           
        }
        
        public void startconn_Click()
        {
            socketWatch = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipaddress = IPAddress.Parse(ip.Trim());
            IPEndPoint endpoint = new IPEndPoint(ipaddress, int.Parse(port.Trim()));
            socketWatch.Bind(endpoint);


            socketWatch.Listen(10);

            threadWatch = new Thread(WatchingConn);
            threadWatch.IsBackground = true;
            threadWatch.Start();

            recmsg.Invoke(new Action(() =>
            {
                recmsg.AppendText("开始对客户端进行监听！" + "\r\n");
            }
           ));

        }


        //保存了服务器端所有负责和客户端通信发套接字  
        Dictionary<string, Socket> dictSocket = new Dictionary<string, Socket>();
        //保存了服务器端所有负责调用通信套接字.Receive方法的线程  
        Dictionary<string, Thread> dictThread = new Dictionary<string, Thread>();

        //这里进行临时的程序搭建
        private void WatchingConn(object obj)
        {

            while (true)
            {
                Socket socConn = socketWatch.Accept();
               
                iptext.Invoke(new Action(() =>
                {
                    //向下拉项中添加一个客户端的ip端口字符串，作为客户端的唯一标识  
                    iptext.Items.Add(socConn.RemoteEndPoint.ToString());
                }));

                ////将与客户端通信的套接字对象sokConn添加到键值对集合中，并以客户端IP端口作为键  
                dictSocket.Add(socConn.RemoteEndPoint.ToString(), socConn);            

                ParameterizedThreadStart pts = new ParameterizedThreadStart(RecMsg);
                Thread trd = new Thread(pts);
                trd.IsBackground = true;
                trd.Start(socConn);

                dictThread.Add(socConn.RemoteEndPoint.ToString(), trd);
                /****
                recmsg.Invoke(new Action(() =>
                {
                    recmsg.AppendText("客户端连接成功！" + "\r\n");
                }
                ));
                ***/
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
            recmsg.AppendText("关闭监听！");
        }
        
        public interface DataProc
        {
            void dataProc(byte[] data);
        }
        private void RecMsg(object socketClientPara)
        {
            Socket socketRec = socketClientPara as Socket;
            List<byte> m_buffer = new List<byte>();
            while (true)
            {
                byte[] arrRecMsg = new byte[1024];

                //int length = socketClient.Receive(arrRecMsg);//......wrong......  

                int length = -1;
                try
                {
                    length = socketRec.Receive(arrRecMsg);
                }
                catch (SocketException ex)
                {
                    MessageBox.Show("异常：" + ex.Message);
                    //从通信套接字集合中删除被中断连接的通信套接字对象  
                    dictSocket.Remove(socketRec.RemoteEndPoint.ToString());
                    //从通信线程结合中删除被终端连接的通信线程对象  
                    dictThread.Remove(socketRec.RemoteEndPoint.ToString());
                    //从列表中移除被中断的连接 ip:Port  
                    iptext.Items.Remove(socketRec.RemoteEndPoint.ToString());
                   

                    break;
                }

                catch (Exception ex)
                {
                    MessageBox.Show("异常：" + ex.Message);
                    break;
                }
                

                // string str = Encoding.UTF8.GetString(arrRecMsg, 0, length);
                if (length > 0)
                {

                    byte[] data = new byte[length];
                    Array.Copy(arrRecMsg, data, length);
                    
                    if (data[0] == 0x3a)
                    {                       
                        string str = Encoding.ASCII.GetString((data));
                        //str = str.Remove(0,1);
                        str = str.Trim();
                        str = str.Remove(0, 1);

                        m_buffer.AddRange(data);

                        if (m_buffer[m_buffer.Count - 1] == 0x0a && m_buffer[m_buffer.Count - 2] == 0x0d)
                        {

                            byte[] byteArray = DataProcess.StrToHexByte(str);
                            

                            datapro.dataProc(byteArray);
                            m_buffer.Clear();
                        }
                                           

                    }
                    if (m_buffer.Count > 0 && m_buffer[0] == 0x3a)
                    {
                        m_buffer.AddRange(data);
                    }


                }        
                /****
                recmsg.Invoke(new Action(() =>
                {
                    recmsg.AppendText(iptext.Items[0].ToString() + ":\r\n"  + str + "\r\n");
                }));
                ***/

                Thread.Sleep(10);

                arrRecMsg = null;
               
                GC.Collect();
                GC.WaitForPendingFinalizers();

            }
        }
        private DateTime GetTime()
        {
            DateTime getTime = new DateTime();
            getTime = DateTime.Now;
            return getTime;
        }
                 

        private void SendMsg(string sendMsg)
        {
            if (iptext.Items.Count < 1) return;
            if (string.IsNullOrEmpty(iptext.Items[0].ToString()))
            {
                MessageBox.Show("请选择通信IP！");
            }

            else
            {
                byte[] strSendMsg = Encoding.UTF8.GetBytes(sendMsg);
                string strClientKey = iptext.Items[0].ToString();//通过Key匹配对应ip地址的客户端  

                dictSocket[strClientKey].Send(strSendMsg);
                /****
                recmsg.Invoke(new Action(() =>
               {
                   recmsg.AppendText("服务器:" + "\r\n" + GetTime() + "\r\n" + sendMsg + "\r\n");

               }));
               ***/
               sendmsg.Text = null;

            }
        }
        public void SendMsgByte(byte[] cmdbyte)
        {
            if (iptext.Items.Count < 1) return;
            if (string.IsNullOrEmpty(iptext.Items[0].ToString()))
            {
                MessageBox.Show("请选择通信IP！");
            }

            else
            {
                               
                string strClientKey = iptext.Items[0].ToString();//通过Key匹配对应ip地址的客户端  

                dictSocket[strClientKey].Send(cmdbyte);
                /***
                recmsg.Invoke(new Action(() =>
                {
                    recmsg.AppendText("服务器:" + cmdbyte + "\r\n");

                }));
                
                sendmsg.Text = null;
                ***/

            }
        }

        public void sendok_Click()
        {
           // SendMsg(sendmsg.Text.Trim());

        }

    }
}

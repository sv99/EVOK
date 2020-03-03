
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Text;
using System.Windows.Forms;
using SDK_HANDLE = System.Int32;
namespace xm
{
    public struct DEV_INFO
    {
        public int nListNum;		   //position in the list
        public SDK_HANDLE lLoginID;			//login handle
        public int lID;				//device ID
        public string szDevName;		//device name
        public string szIpaddress;		//device IP
        public string szUserName;		//user name
        public string szPsw;			//pass word
        public int nPort;				//port number
        public int nTotalChannel;		//total channel
        public SDK_CONFIG_NET_COMMON_V2 NetComm;                  // net config
        public H264_DVR_DEVICEINFO NetDeviceInfo;
        public byte bSerialID;//be SerialNumber login
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 64)]
        public string szSerIP;//server ip
        public int nSerPort;           //server port
        [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 16)]
        public string szSerialInfo;  //SerialNumber
        public bool bStartAlarm;
    }

    public struct CHANNEL_INFO
    {
        string szChnnelName;			// 通道名称.
        public int nChnnID;							// 用于地图节点管理
        public int nChannelNo;							// 通道号.
        public int bUserRight;							// 用户权限(使能).
        public int PreViewChannel;						// 预览通道
        public int nStreamType;						// 码流类型
        public DEV_INFO DeviceInfo;							// 设备信息.
        public int nCombinType;						// 组合编码模式
        public int dwTreeItem;							//记录设备树中的节点句柄，可以节省查找事件
        public int nFlag;								//1为选择为录像 0 为没有被选择 2 为正在录像
        public int nWndIndex;
    };

    public class xmCam
    {
        [DllImport("NetSdk.dll")]
        public static extern int H264_DVR_SearchDevice(IntPtr ptr, int nBufLen, ref int nRetLength, int nSearchTime);

        DEV_INFO devInfo;

        int status = -1;
        int m_iPlayhandle;
        Control showCC;
        public bool Status
        {
            get { return status == 1; }
        }
        public Control owner = new Control();
        private NetSDK.fDisConnect disCallback;
        public xmCam()
        {
            devInfo = new DEV_INFO();
            InitSDK();
        }
        void DisConnectBackCallFunc(SDK_HANDLE lLoginID, string pchDVRIP, int nDVRPort, IntPtr dwUser)
        {

            NetSDK.H264_DVR_Logout(lLoginID);           
        }

        public int InitSDK()
        {
            //initialize
            NetSDK.g_config.nSDKType = NetSDK.SDK_TYPE.SDK_TYPE_GENERAL;

            disCallback = new NetSDK.fDisConnect(DisConnectBackCallFunc);
            GC.KeepAlive(disCallback);
            int bResult = NetSDK.H264_DVR_Init(disCallback, owner.Handle);
            NetSDK.H264_DVR_SetConnectTime(3000, 3);
            return bResult;
        }
        public bool ExitSDK()
        {
            return NetSDK.H264_DVR_Cleanup();
        }
  
        public bool OpenCamxm(Control cc)
        {
            if (status < 0) return false;

            H264_DVR_CLIENTINFO playstru = new H264_DVR_CLIENTINFO();

            playstru.nChannel = 0;
            playstru.nStream = 0;
            playstru.nMode = 0;
            playstru.hWnd = cc.Handle;
            m_iPlayhandle = NetSDK.H264_DVR_RealPlay(devInfo.lLoginID, ref playstru);

            m_iPlayhandle = NetSDK.H264_DVR_RealPlay(devInfo.lLoginID, ref playstru);
            showCC = cc;
            if (m_iPlayhandle <= 0)
            {
                Int32 dwErr = NetSDK.H264_DVR_GetLastError();
                string info = string.Format("打开{0}摄像头失败,错误码：{1}", devInfo.szDevName, dwErr.ToString());
                MessageBox.Show(info);

                return false;
            }
            else
            {

                NetSDK.H264_DVR_MakeKeyFrame(devInfo.lLoginID, 0, 0);
                return true;
            }

            
        }
        public bool Init(string devName,string username,string pwd,string ip,string port)
        {

            H264_DVR_DEVICEINFO dvrdevInfo = new H264_DVR_DEVICEINFO();
            dvrdevInfo.Init();
            int nError;
            SDK_HANDLE nLoginID = NetSDK.H264_DVR_Login(
                ip.Trim(),
                ushort.Parse(port.Trim()),
                username, pwd,
                ref dvrdevInfo,
                out nError,
                SocketStyle.TCPSOCKET);

            if (nLoginID > 0)
            {
                devInfo.szDevName = devName;
                devInfo.lLoginID = nLoginID;
                devInfo.nPort = Int32.Parse(port);
                devInfo.szIpaddress = ip;
                devInfo.szUserName = username;
                devInfo.szPsw = pwd;
                devInfo.NetDeviceInfo = dvrdevInfo;
                status = 1;
                return true; 
            }                     

            return false;
           
        }

        public void dispose()
        {
            if (this == null) return;
            if (m_iPlayhandle > 0)
            {
                NetSDK.H264_DVR_StopRealPlay(m_iPlayhandle, (uint)showCC.Handle);
                m_iPlayhandle = -1;

            }

            if (devInfo.lLoginID > 0)
            {
                NetSDK.H264_DVR_Logout(devInfo.lLoginID);
                                             
            }
            ExitSDK();
        }
    }
}

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows.Forms;
using xjplc;
using static xjplc.CHCNetSDK;

namespace xjplc
{
    public class Camera
    {

        const int adjustId_Up = 1;
        const int adjustId_Down = 2;
        const int adjustId_Left = 3;
        const int adjustId_Right = 4;
        private uint iLastErr = 0;
        private Int32 m_lUserID = -1;
        private bool m_bInitSDK = false;
        private bool m_bRecord = false;
        private bool m_bTalk = false;
        private Int32 m_lRealHandle = -1;
        private int lVoiceComHandle = -1;

        int  m_lChannel=1;
        private string str;

        private  string[] StatusStrip= {"未知", "连接成功", "预览中", "空闲", "录像中" };
        private  int[] statusStatusip= { 0, 1, 2, 3,4};


        CHCNetSDK.REALDATACALLBACK RealData = null;
        public CHCNetSDK.NET_DVR_PTZPOS m_struPtzCfg;

        int status = -1;

        System.Timers.Timer errorTimer = new System.Timers.Timer(); //初始化定时器
       
        private string recordFileName;
        public string RecordFileName
        {
            get { return recordFileName; }
            set { recordFileName = value; }
        }

        public int Status
        {
            get { return status; }
            set { status = value; }
        }

       
        
        string devName;
        public string DevName
        {
            get { return devName; }
            set { devName = value; }
        }
        public Camera()
        {
            m_bInitSDK = CHCNetSDK.NET_DVR_Init();

            if (m_bInitSDK == false)
            {
                MessageBox.Show("摄像头初始化失败！");
                ConstantMethod.AppExit();
                return ;
            }
            else
            {
                //保存SDK日志 To save the SDK log
              //  CHCNetSDK.NET_DVR_SetLogToFile(3, "C:\\SdkLog\\", true);
                
            }

       
            
        }
        #region 错误报警
        void InitError()
        {
            errorTimer.Interval = 500;//配置时间500ms
            errorTimer.Elapsed += new System.Timers.ElapsedEventHandler(OnTimedEvent);
            errorTimer.AutoReset = true;//每到指定时间Elapsed事件是到时间就触发
            errorTimer.Enabled = true; //指示 Timer 是否应引发 Elapsed 事件。
            errorCount = 0;
        }
        void closeErrorTimer()
        {
            errorTimer.Enabled = false;
            errorCount = 3;
        }
        public bool IsOnLine
        {
            get { return errorCount < 2; }
        }
        int errorCount = 100;
        private void OnTimedEvent(Object source, ElapsedEventArgs e)
        {
            errorCount++;
        }

        public void RealDataCallBack(Int32 lRealHandle, UInt32 dwDataType, IntPtr pBuffer, UInt32 dwBufSize, IntPtr pUser)
        {
            if (dwBufSize > 0)
            {
                errorCount = 0;
            }
        }

        #endregion
        public bool CheckOut()
        {
            //注销登录 Logout the device
            if (m_lRealHandle >= 0)
            {
                //MessageBox.Show("请先停止显示");
                StopView();
                ConstantMethod.Delay(500);
               //return false;
            }
            if(m_lUserID>0)
            if (!CHCNetSDK.NET_DVR_Logout(m_lUserID))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "退出失败，错误号= " + iLastErr;
                MessageBox.Show(str);
                return false;
            }

            m_lUserID = -1;

            Status = -1;

            return true;

        }

      
        //public static extern bool NET_DVR_MatrixGetDisplayCfg(int lUserID, uint dwDispChanNum, ref NET_DVR_VGA_DISP_CHAN_CFG lpDisplayCfg);

        //public static extern bool NET_DVR_MatrixSetDisplayCfg(int lUserID, uint dwDispChanNum, ref NET_DVR_VGA_DISP_CHAN_CFG lpDisplayCfg);

        public void getDispLay()
        {
            NET_DVR_VGA_DISP_CHAN_CFG lpDisplayCfg= new NET_DVR_VGA_DISP_CHAN_CFG();
            uint dwDispChanNum=1;
            bool okget = false;
            okget= NET_DVR_MatrixGetDisplayCfg(m_lUserID, dwDispChanNum, ref lpDisplayCfg);
            
        }
        public bool Login(string ip, int port, string userName, string pwd)
        {
            string DVRIPAddress = ip; //设备IP地址或者域名
            int DVRPortNumber = port;//设备服务端口号
            string DVRUserName = userName;//设备登录用户名
            string DVRPassword = pwd;//设备登录密码
          
            CHCNetSDK.NET_DVR_DEVICEINFO_V30 DeviceInfo = new CHCNetSDK.NET_DVR_DEVICEINFO_V30();

            //登录设备 Login the device
            m_lUserID = CHCNetSDK.NET_DVR_Login_V30(DVRIPAddress, DVRPortNumber, DVRUserName, DVRPassword, ref DeviceInfo);
            if (m_lUserID < 0)
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string str = "摄像头打开失败，错误号= " + iLastErr; //登录失败，输出错误号

               //MessageBox.Show(str);
            }
            else
            {

                Status = 1;
                InitShowStr();
                InitError();
                //登录成功  //MessageBox.Show("Login Success!");
                return true;
            }
            Status = 0;
            return false;
        }
        public bool ListView(Control RealPlayWnd)
        {

            if (m_lUserID < 0)
            {
                MessageBox.Show("请先登录摄像头！");
                return false;
            }

            if (m_lRealHandle < 0)
            {
                CHCNetSDK.NET_DVR_PREVIEWINFO lpPreviewInfo = new CHCNetSDK.NET_DVR_PREVIEWINFO();
                lpPreviewInfo.hPlayWnd = RealPlayWnd.Handle;//预览窗口
                lpPreviewInfo.lChannel = 1;// Int16.Parse(1);//预te览的设备通道 默认为1
                lpPreviewInfo.dwStreamType = 0;//码流类型：0-主码流，1-子码流，2-码流3，3-码流4，以此类推
                lpPreviewInfo.dwLinkMode = 0;//连接方式：0- TCP方式，1- UDP方式，2- 多播方式，3- RTP方式，4-RTP/RTSP，5-RSTP/HTTP 
                lpPreviewInfo.bBlocked = true; //0- 非阻塞取流，1- 阻塞取流
                lpPreviewInfo.dwDisplayBufNum = 1; //播放库播放缓冲区最大缓冲帧数
                lpPreviewInfo.byProtoType = 0;
                lpPreviewInfo.byPreviewMode = 0;

                /***
                if (textBoxID.Text != "")
                {
                    lpPreviewInfo.lChannel = -1;
                    byte[] byStreamID = System.Text.Encoding.Default.GetBytes(textBoxID.Text);
                    lpPreviewInfo.byStreamID = new byte[32];
                    byStreamID.CopyTo(lpPreviewInfo.byStreamID, 0);
                }
                ***/

                if (RealData == null)
                {
                    RealData = new CHCNetSDK.REALDATACALLBACK(RealDataCallBack);//预览实时流回调函数
                }

                IntPtr pUser = new IntPtr();//用户数据

                //打开预览 Start live view 
                m_lRealHandle = CHCNetSDK.NET_DVR_RealPlay_V40(m_lUserID, ref lpPreviewInfo, RealData, pUser);
                if (m_lRealHandle < 0)
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "预览错误，错误号= " + iLastErr; //预览失败，输出错误号
                    MessageBox.Show(str);
                    return false;
                }
                else
                {
                    Status = 2;
                    //预览成功
                    return true;
                }
            }
            Status = 0;
            return false;           
        }
        
        public bool Capture(string filename)
        {


            string sJpegPicFileName;
            //图片保存路径和文件名 the path and file name to save
            sJpegPicFileName = filename;

            int lChannel = 1; //通道号 Channel number

            CHCNetSDK.NET_DVR_JPEGPARA lpJpegPara = new CHCNetSDK.NET_DVR_JPEGPARA();
            lpJpegPara.wPicQuality = 0; //图像质量 Image quality
            lpJpegPara.wPicSize = 0xff; //抓图分辨率 Picture size: 2- 4CIF，0xff- Auto(使用当前码流分辨率)，抓图分辨率需要设备支持，更多取值请参考SDK文档

            //JPEG抓图 Capture a JPEG picture
            if (!CHCNetSDK.NET_DVR_CaptureJPEGPicture(m_lUserID, lChannel, ref lpJpegPara, sJpegPicFileName))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                //str = "NET_DVR_CaptureJPEGPicture failed, error code= " + iLastErr;
                MessageBox.Show("截图失败！错误号："+ iLastErr);
                return false;
            }           
            return true;;
        }
        public bool StopView()
        {
            if (m_lRealHandle >= 0)
            {
                //停止预览 Stop live view 
                if (!CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle))
                {
                    iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                    str = "停止预览失败，错误号= " + iLastErr;
                    MessageBox.Show(str);
                    return false;
                }
                m_lRealHandle = -1;
                Status = 3;
                closeErrorTimer();
                return true;
            }
            Status = 0;
            return false;
        }
    
       
        public void Zoom_In_Start()
        {
            if (Status==2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.ZOOM_IN, 0);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1,CHCNetSDK.ZOOM_IN, 0);
            }
        }   
        public void Zoom_In_Stop()
        {
            if (Status == 2)
            {

                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.ZOOM_IN, 1);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1, CHCNetSDK.ZOOM_IN, 1);
            }
        }
        public void OneKeyFocus()
        {
           
        }
        public void Zoom_Out_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.ZOOM_OUT, 0);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1, CHCNetSDK.ZOOM_OUT, 0);
            }
        }
        public void Zoom_Out_Stop()
        {
            if (Status == 2)
            {
                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.ZOOM_OUT, 1);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1, CHCNetSDK.ZOOM_OUT, 1);
            }
        }
        public void Focus_Near_Stop()
        {
            if (Status == 2)
            {
                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.FOCUS_NEAR, 1);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1, CHCNetSDK.FOCUS_NEAR, 1);
            }
        }
        public void Focus_Near_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.FOCUS_NEAR, 0);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1,CHCNetSDK.FOCUS_NEAR, 0);
            }
        }

        public void Focus_Far_Stop()
        {
            if (Status == 2)
            {
                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.FOCUS_FAR, 1);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1, CHCNetSDK.FOCUS_FAR, 1);
            }
        }
        private CHCNetSDK.NET_DVR_SHOWSTRING_V30 m_struShowStrCfg;
        public CHCNetSDK.NET_DVR_DEVICEINFO_V30 m_struDeviceInfo;
        private CHCNetSDK.NET_DVR_PICCFG_V30 m_struPicCfgV30;


        public bool InitShowStr()
        {
            UInt32 dwReturn = 0;
            Int32 nSize = Marshal.SizeOf(m_struShowStrCfg);
            IntPtr ptrShowStrCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struShowStrCfg, ptrShowStrCfg, false);

            if (!CHCNetSDK.NET_DVR_GetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_GET_SHOWSTRING_V30, m_lChannel, ptrShowStrCfg, (UInt32)nSize, ref dwReturn))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string strErr = "NET_DVR_GET_SHOWSTRING_V30 failed, error code= " + iLastErr;
                //获取字符叠加参数失败，输出错误号 Failed to get overlay parameters and output the error code
                MessageBox.Show(strErr);
          
            }
            else
            {
                m_struShowStrCfg = (CHCNetSDK.NET_DVR_SHOWSTRING_V30)Marshal.PtrToStructure(ptrShowStrCfg, typeof(CHCNetSDK.NET_DVR_SHOWSTRING_V30));                
            }

            Marshal.FreeHGlobal(ptrShowStrCfg);

            return true;

        }
       
        public bool StringAddTest(int id,string ss,int posx ,int posy)
        {

            if (id > 3 || id < 0) return false;
            m_struShowStrCfg.struStringInfo[id].wShowString = 1;                  

            m_struShowStrCfg.struStringInfo[id].sString = ss;

            int chineseCount = ConstantMethod.GetHanNumFromString(ss);
            int totalcount = chineseCount * 2 + ss.Length - chineseCount;
            m_struShowStrCfg.struStringInfo[id].wStringSize = (ushort)(totalcount);
            m_struShowStrCfg.struStringInfo[id].wShowStringTopLeftX = (ushort)posx;
            m_struShowStrCfg.struStringInfo[id].wShowStringTopLeftY = (ushort)posy;
         
            int nSize = Marshal.SizeOf(m_struShowStrCfg);
            IntPtr ptrShowStrCfg = Marshal.AllocHGlobal(nSize);
            Marshal.StructureToPtr(m_struShowStrCfg, ptrShowStrCfg, false);

            if (!CHCNetSDK.NET_DVR_SetDVRConfig(m_lUserID, CHCNetSDK.NET_DVR_SET_SHOWSTRING_V30, m_lChannel, ptrShowStrCfg, (UInt32)nSize))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                string strErr = "NET_DVR_SET_SHOWSTRING_V30 failed, error code= " + iLastErr;
                //设置字符叠加参数失败，输出错误号 Failed to set overlay parameters and output the error code
                MessageBox.Show(strErr);
            }
            else
            {
               // MessageBox.Show("设置图像参数成功！");
            }



            Marshal.FreeHGlobal(ptrShowStrCfg);

            return true;
        }
        int adjustId = 0;
        bool IsAdjusitng;
        public void StartAdjust(int id)
        {
            switch (id)
            {
                case 1:
                    {

                        if (adjustId != id)
                        {
                            if (adjustId > 0) StopAdjust();
                            Focus_Far_Start();
                        }
                        adjustId = id;
                        break;
                    }
                case 2:
                    {
                        if (adjustId != id)
                        {
                            if (adjustId > 0) StopAdjust();
                            Focus_Near_Start();
                        }
                        adjustId = id;
                        break;
                    }
                case 3:
                    {
                        if (adjustId != id)
                        {
                            if (adjustId > 0) StopAdjust();
                            Zoom_Out_Start();
                        }                       
                        adjustId = id;
                        break;
                    }
                case 4:
                    {
                        if (adjustId != id)
                        {
                            if (adjustId > 0) StopAdjust();
                            Zoom_In_Start();
                        }                       
                        adjustId = id;
                        break;
                    }
                default:
                    {
                        StopAdjust();
                        break;
                    }
            }
        }

        public void StopAdjust()
        {
            if (!IsAdjusitng) return;

            IsAdjusitng = false;
            switch (adjustId)
            {
                case 1:
                    {
                       
                        Focus_Far_Stop();
                        adjustId = 0;
                        break;
                    }
                case 2:
                    {
                        Focus_Near_Stop();
                        adjustId = 0;
                        break;
                    }
                case 3:
                    {
                        Zoom_Out_Stop();
                        adjustId = 0;
                        break;
                    }
                case 4:
                    {
                        Zoom_In_Stop();
                        adjustId = 0;
                        break;
                    }
                default :
                    {

                        adjustId = 0;
                        break;
                    }
            }
        }
        public void Focus_Far_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                CHCNetSDK.NET_DVR_PTZControl(m_lRealHandle, CHCNetSDK.FOCUS_FAR, 0);
            }
            else
            {
                CHCNetSDK.NET_DVR_PTZControl_Other(m_lRealHandle, 1, CHCNetSDK.FOCUS_FAR, 0);
            }
        }
        public bool StopRecord(string filename)
        {
            //停止录像 Stop recording
            if (!CHCNetSDK.NET_DVR_StopSaveRealData(m_lRealHandle))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "录像失败, 错误码 = " + iLastErr;
                MessageBox.Show(str);
                return false;
            }
            else
            {
                str = "录像已保存！" + filename;
                MessageBox.Show(str);
                Status = 2;

            }

            return true;
        }

        //录像
        public bool StartRecord(string filename )
        {
            if (Status != 2) return false;
            int idChannel = 1;
            string sVideoFileName=filename;
            if (File.Exists(filename))
            {

                MessageBox.Show("文件名已存在！");
                return false;

            }
            int lChannel = idChannel; //通道号 Channel number
            CHCNetSDK.NET_DVR_MakeKeyFrame(m_lUserID, lChannel);

            //开始录像 Start recording
            if (!CHCNetSDK.NET_DVR_SaveRealData(m_lRealHandle, sVideoFileName))
            {
                iLastErr = CHCNetSDK.NET_DVR_GetLastError();
                str = "NET_DVR_SaveRealData failed, error code= " + iLastErr;
                return false;
            }
            else
            {
                Status = 4;              
            }


            return true;
        }
        public void dispose()
        {
            if (this == null) return;
            if (status == 4)
            {
                StopRecord(RecordFileName);
            }
            if (m_lRealHandle >= 0)
            {
                CHCNetSDK.NET_DVR_StopRealPlay(m_lRealHandle);
            }
            if (m_lUserID >= 0)
            {
                CHCNetSDK.NET_DVR_Logout(m_lUserID);
            }
            if (m_bInitSDK == true)
            {
                CHCNetSDK.NET_DVR_Cleanup();
            }
        }
    }
}

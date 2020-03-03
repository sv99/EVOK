
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static xjplc.HankVisionSDK;
using static xjplc.HDVPLAYSDK;

namespace xjplc.tanuo
{


    public class HankCamera
    {

        private string[] StatusStrip = { "未知", "连接成功", "预览中", "空闲", "录像中" };
        private int[] statusStatusip = { 0, 1, 2, 3, 4 };

        int status;
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        IntPtr showhWnd; //显示窗口句柄
        public System.IntPtr ShowhWnd
        {
            get { return showhWnd; }
            set { showhWnd = value; }
        }
        int g_lUserID = -1;  //摄像头 ID
        int m_lPlayHandle = -1; //摄像头播放句柄
        int m_lRealDataID = -1;  //实时流句柄

        ServerInfo deviceInfo;
        public Action<int> CamShow;
        public void SetDeviceInfo(ServerInfo ser)
        {
            this.deviceInfo = ser;
        }


        public HankCamera()
        {
            for (int i = 0; i < 5; i++)
            {
                OsdText s = new OsdText();
                s.Id = (i).ToString();
                osdLst.Add(s);
            }

        }

        #region //打开摄像头
        public bool IsOnLine()
        {
            return Status == 2;
        }
        public bool OpenDevice(ServerInfo ser)
        {

            if (!HankVisionSDK.IPCNET_Init())
            {

                return false;
            }
            //登录
            g_lUserID = HankVisionSDK.IPCNET_Login(ser.server_Ip, uint.Parse(ser.server_Port), ser.userName, ser.userPwd);
            //视频浏览
            if (g_lUserID < 0) return false;
            Status = 2;

            //开始预览
            IntPtr puser = new IntPtr();

            tagEncodeType m_EncodeType = tagEncodeType.ENCODE_H264;

            tagRealDataInfo sRealInfo = new tagRealDataInfo();

            sRealInfo.eEncodeType = m_EncodeType;
            sRealInfo.lChannel = 0;
            sRealInfo.lStreamMode = 0;  // 子码流

            SetDeviceInfo(ser);

            if (RealData == null)
            {
                RealData = new HankVisionSDK.CBRealData(RealData0);
            }
            if (cbne == null)
            {
                cbne = new CBNetException(CBN);
            }
            /***
            if (cbdf == null)
            {
                cbdf = new CBDrawFun(CBDrawFun);
            }
            ***/
            m_lRealDataID = IPCNET_StartRealData(
                g_lUserID,
                ref sRealInfo,
                RealData,
                puser);

            if (m_lRealDataID < 0) return false;


            IPCNET_SetNetExceptionCallBack(cbne, puser);
          
            HDVPLAY_SetDrawFunCallBack(m_lPlayHandle, cbdf, puser);

           

            return true;
        }


        #endregion
        #region //关闭摄像头
        public void dispose()
        {


            if (RecordCmd)
            {
                RecordCmd = false;
            }
         
            if (status == 4)
            {
                StopRecord();
            }
                    
            if (m_lRealDataID >= 0)
            {
                if (!IPCNET_StopRealData(m_lRealDataID))
                {
                    //AfxMessageBox("实时预览停止失败！");
                    return;
                }
                m_lRealDataID = -1;
            }

            if (m_lPlayHandle >= 0)
            {
                if (!HDVPLAY_Stop(m_lPlayHandle))
                {
                    //TRACE("HDVPLAY_Stop Faild!\n");
                }
                if (!HDVPLAY_CloseStream(m_lPlayHandle))
                {
                    // TRACE("HDVPLAY_CloseStream Faild!\n");
                }
                m_lPlayHandle = -1;
            }

            if (g_lUserID > 0)
            {
                HankVisionSDK.IPCNET_Logout(g_lUserID);
            }
            g_lUserID = -1;


            Status = -1;

        }
        #endregion
        #region 录像

        FileStream fs;

        bool RecordCmd = false;

        string recordFileName;
        public void Record()
        {


            if (RecordCmd)
            {
                RecordCmd = false;
            }
            else
            {

                RecordCmd = true;
                if (string.IsNullOrWhiteSpace(recordFileName))
                {
                    string
                    fileName =
                        Directory.GetCurrentDirectory() + "\\" +
                    DateTime.Now.ToString("yyyyMMddhhmmss") + ".avi";
                    fs = new FileStream(fileName, FileMode.Create);
                }

            }

        }

        #endregion

        //拍照
        public bool TakePhoto(string filename)
        {
            if (Status != 2) return false;
            if (!HDVPLAY_CapturePicture(m_lPlayHandle, filename, tagPictype.PIC_JPEG))
            {
                MessageBox.Show("拍照失败" + HDVPLAY_GetLastError().ToString());

                return false;
            }

            return true;

        }
        public bool StartRecord(string filename)
        {
            if (Status != 2) return false;

            if (File.Exists(filename))
            {

                MessageBox.Show("文件名已存在！");
                return false;

            }
            if (HDVPLAY_StartRecord(m_lPlayHandle, filename, tagRecordtype.RECORD_AVI))
            {
                recordFileName = filename;
                Status = 4;
                return true;
            }
            else return false;
        }
        public bool StopRecord()
        {
            if (HDVPLAY_StopRecord(m_lPlayHandle))
            {
                Status = 2;
                MessageBox.Show("录像已保存！" + recordFileName);
                return true;
            }
            else return false;
        }
        //复位

        #region 焦距放大
        int adjustId = 0;
        bool IsAdjusitng;
        public void StartAdjust(int id)
        {
            if (g_lUserID > 0)
            {
                if (CamShow != null) CamShow(id);
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
        }
        bool StartPtzControl()
        {
            return HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.FOCUS_FAR, 6, 6, false);
        }
        bool StopPtzControl()
        {
            return HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.FOCUS_FAR, 6, 6, true);
        }
        public void Focus_Far_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                if (!StartPtzControl())
                {
                    MessageBox.Show("ERROR");
                }
            }
        }
        public void Focus_Far_Stop()
        {
           
            StopPtzControl();
            
        }

        public void Focus_Near_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.FOCUS_NEAR, 6, 6, false);
            }
       }
        public void Focus_Near_Stop()
        {
           
            HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.FOCUS_NEAR, 6, 6, true);
            IsAdjusitng = false;
        }

        public  void Zoom_Out_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_TELE, 1, 1, false);
            }
        }
        public void Zoom_Out_Stop()
        {         
            HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_TELE, 1, 1,true);
            IsAdjusitng = false;
        }

        public void Zoom_In_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_WIDE, 1, 1, false);
            }
        }
        public void Zoom_In_Stop()
        {
           
            HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_WIDE, 1, 1, true);
            IsAdjusitng = false;
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
                default:
                    {

                        adjustId = 0;
                        break;
                    }
            }
        }

        #endregion
        //焦距放大
        //看门狗

        #region 功能函数

        HankVisionSDK.CBRealData RealData = null;

        CBNetException cbne;

        CBDrawFun      cbdf;    

        void RealData0(
            Int32 lRealHandle,
             tagRealDataType eDataType,
             IntPtr pBuffer,
             UInt32 lBufSize,
             IntPtr pUserData)
        {

            switch (eDataType)
            {
                case tagRealDataType.REALDATA_HEAD:

                    m_lPlayHandle = 
                    HDVPLAYSDK.
                    HDVPLAY_OpenStream(pBuffer, lBufSize);

                    if (m_lPlayHandle < 0)
                    {
                        // MessageBox.Show(m_lPlayHandle.ToString());
                        return;
                    }
                    if (!HDVPLAYSDK.HDVPLAY_Play(m_lPlayHandle, ShowhWnd, true))
                    {
                        //MessageBox.Show("播放失败！");
                        return;
                    }

                    //   HDVPLAY_SetDecodeCallBack(m_lPlayHandle, _CBDecodeFun, pUserData);

                    break;
                case tagRealDataType.REALDATA_VIDEO:

                    if (RecordCmd)
                    {
                        byte[] buf = new byte[lBufSize];                        
                        Marshal.Copy(pBuffer, buf, 0, (int)lBufSize);                   
                        //将byte数组写入文件中 
                        // fs.Position = fs.Length;
                        fs.Write(buf, 0, (int)lBufSize);
                        fs.Flush();


                    }
                    else
                    {
                        if (fs != null)
                        {
                            fs.Close();
                            fs = null;
                        }
                    }

                    /*实时视频流数据*/ // 处理码流数据
                    if (!HDVPLAY_InputData(m_lPlayHandle, pBuffer, lBufSize))
                    {
                        // MessageBox.Show("视频输入失败！");
                        return;
                    }


                    break;

            }
        }

        //设备错误
        public void CBN(int lLoginID,
                         int lRealHandle,
                         tagNetExceptionType eNetMsgType,
                        IntPtr pUserData)
        {
           // showInfo("错误" + eNetMsgType);
        }



        #endregion


        #region 添加字符回调函数 //在hdvplay中



        List<OsdText> osdLst = new List<OsdText>();
        string OsdHeader
        {
            get
            {
                return "http://" +deviceInfo.server_Ip+ "/cgi/image_set?Channel=1&Group=OSDInfo";
            }
        }
        string CompactOsdstr(int id)
        {
            string ts = OsdHeader;
            
            if (id >= 0 && id < osdLst.Count)
            {
                if (id == 0)
                {
                    ts +=

                   ("&" +
                  osdLst[id].status + "&" +
                  osdLst[id].Title + "&" +
                  osdLst[id].PX + "&" +
                  osdLst[id].PY
                  );
                }
                else
                    ts +=

                        ("&" +
                       osdLst[id].status + "&" +
                       osdLst[id].Title + "&" +
                       osdLst[id].PX + "&" +
                       osdLst[id].PY + "&" +
                       osdLst[id].FontSize
                       );
                return ts;
            }
            else
                foreach (OsdText os in osdLst)
                {

                    ts +=

                        ("&" +
                       os.status + "&" +
                       os.Title + "&" +
                       os.PX + "&" +
                       os.PY + "&" +
                       os.FontSize
                       );
                }

            return ts;

        }

        bool SetOsdText(int id, string str, int x, int y, int showId = 0, int fsize = 24)
        {
            if (id >= osdLst.Count || id < 0)
            {
                return false;
            }

            osdLst[id].StatusValue = showId;
            osdLst[id].TitleValue = str;
            osdLst[id].PXValue = x;
            osdLst[id].PYValue = y;
            osdLst[id].FontSizeValue = fsize;

            return true;

        }


        void CBDrawFun(
                       UInt32 lPlayHandle,
                       IntPtr hdc,
                       uint nWidth,
                       uint nHeight,
                       IntPtr pUserData
                       )
        {
            Graphics gr = Graphics.FromHdc(hdc);
            Font f = new Font("宋体",24);
            Brush b = Brushes.Blue;
            gr.DrawString("312312321",f, b,100, 100);

        }

        public bool HideString(int id)
        {
            SetOsdText(id, "", 0, 0, ShowOff, 5);

            string sUrl = CompactOsdstr(id);

            byte[] ss = new byte[3000];

            return false;
           // return CGISDK.IPCCGI_SetValue(ref sUrl, ref ss[0], deviceInfo.userName, deviceInfo.userPwd);

        }

        public bool SetString(int id,string s,int x,int y,int fontsize)
        {

            SetOsdText(id, s,x, y,ShowOn, fontsize);
            
            string sUrl = CompactOsdstr(id);

            byte[] ss = new byte[3000];
            byte[] byteArray = Encoding.UTF8.GetBytes(sUrl);


            return CGISDK.IPCCGI_SetValue(ref byteArray[0], ref ss[0], deviceInfo.userName, deviceInfo.userPwd);
            

        }

        #endregion

    }
}


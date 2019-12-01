
using System;
using System.Collections.Generic;
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

        public void SetDeviceInfo(ServerInfo ser)
        {
            this.deviceInfo = ser;
        }


        public HankCamera()
        {

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


            //开始预览
            IntPtr puser = new IntPtr();

            tagEncodeType m_EncodeType = tagEncodeType.ENCODE_H264;

            tagRealDataInfo sRealInfo = new tagRealDataInfo();

            sRealInfo.eEncodeType = m_EncodeType;
            sRealInfo.lChannel = 0;
            sRealInfo.lStreamMode = 0;  // 子码流


            if (RealData == null)
            {
                RealData = new HankVisionSDK.CBRealData(RealData0);
            }
            if (cbne == null)
            {
                cbne = new CBNetException(CBN);
            }


            m_lRealDataID = IPCNET_StartRealData(
                g_lUserID,
                ref sRealInfo,
                RealData,
                puser);

            if (m_lRealDataID < 0) return false;


            IPCNET_SetNetExceptionCallBack(cbne, puser);

            Status = 2;

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

        //复位

        #region 焦距放大
        int adjustId = 0;
        bool IsAdjusitng;
        public void StartAdjust(int id)
        {
            if (g_lUserID > 0)
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
            
        }

        public  void Zoom_Out_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_WIDE, 6, 6, false);
            }
        }
        public void Zoom_Out_Stop()
        {
            
                HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_WIDE, 6, 6,true);
            
        }

        public void Zoom_In_Start()
        {
            if (Status == 2)
            {
                if (IsAdjusitng) return;
                IsAdjusitng = true;
                HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_TELE, 6, 6, false);
            }
        }
        public void Zoom_In_Stop()
        {
           
                HankVisionSDK.IPCNET_PTZControl(g_lUserID, 0, tagPtzCommand.ZOOM_TELE, 6, 6, true);
            
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
        #endregion

    }
}


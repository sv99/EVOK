using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
namespace xjplc
{
   public  class HankVisionSDK
    {

        public HankVisionSDK()
        {
             
        }

        public const int SERIALNO_LEN = 48;
        public const int MAX_NAME_LEN_32 = 32;
        public const int MAX_VERSION_LEN = 16;
        public const int MAX_URL_LEN = 128;
        public const int MAX_PARKING_PLACE_NUM = 3;
        public const int DEVICE_TYPE_IPC = 0;
        public const int DEVICE_TYPE_NVR = 1;
        public const int ERR_NET_SUCCEED = 0; /**< 执行成功 */
        public const int ERR_NET_FAIL = 1; /**< 执行失败 */
        public const int ERR_NET_INVALIDPARAM = 2;  /**< 输入参数非法 */
        public const int ERR_NET_NOMEMORY = 3; /**< 内存分配失败 */
        public const int ERR_NET_SYSFAIL = 4; /**< 系统通用错误 */
        public const int ERR_NET_USERNAME = 5; /**< 用户名错误 */
        public const int ERR_NET_PASSWORD = 6;  /**< 密码错误 */
        public const int ERR_NET_NOINIT = 7;  /**< 没有初始化 */
        public const int ERR_NET_INVALIDCHANNEL = 8;  /**< 通道号错误 */
        public const int ERR_NET_NOLOGIN = 9;  /**< 没有登录 */
        //网络错误
        public const int ERR_NET_OPENSOCKET = 10;  /**< 创建SOCKET错误 */
        public const int ERR_NET_SEND = 11;  /**< 向设备发送网络数据失败 */
        public const int ERR_NET_RECV = 12;  /**< 从设备接收网络数据失败 */
        public const int ERR_NET_CONNNECT = 13;  /**< 连接设备失败，设备不在线、设备忙或网络原因引起的连接超时等 */
        public const int ERR_NET_AUDIOFULL = 14;  /**< 设备的音频连接已满 */

        public const int ERR_NET_DEVTYPE_PTZSURPPORT = 15;  /**< 不支持PTZ控制,设备类型错误 */
        public const int ERR_NET_NODEVTYPE = 16;  /**< 获取设备类型失败 */
        public const int ERR_NET_NODEVPARAMTYPE = 17;  /**< 不支持该设备类型,没有配置对象 */
        public const int ERR_NET_IPOFFLINE = 18;  /**< IP不在线 */
        public const int ERR_NET_RET_INVALIDPARAM = 19;  /**< 设备返回参数错误 */

        public const int ERR_NET_CONNNECT_NBIO = 20;  /**< 连接设备，设置非阻塞模式失败 */
        public const int ERR_NET_CONNNECT_SELECT = 21;  /**< 连接设备，设置select模式超时时间失败 */
        public const int ERR_NET_CONNNECT_ISSET = 22;  /**< 连接设备，设置select模式置位失败 */
        public const int ERR_NET_CONNNECT_BIO = 23; /**< 连接设备，设置阻塞模式失败 */

        /******************************************************************************				
        解码器错误码定义,范围1000-1499.				
        *******************************************************************************/

        public const int ERR_DEC_SUCCEED = 1000;     /**< 执行成功 */
        public const int ERR_DEC_SYSTEM = 1003; /**< 系统内部错误 */
        public const int ERR_DEC_UNKNOWN = 1004;     /**< 未知错误 */
        public const int ERR_DEC_IPADDR_CONFLICT = 1005;    /**< IP地址冲突 */
        public const int ERR_DEC_DEVICE_NOTYPE = 1008;   /**< 不支持该设备类型，不是解码器 */
        public const int ERR_DEC_NETWORK_FAIL_CONNECT = 1009;   /**< 连接设备失败。设备不在线或网络原因引起的连接超时等 */
        public const int ERR_DEC_NETWORK_SEND = 1010;   /**< 向设备发送失败 */
        public const int ERR_DEC_NETWORK_RECV = 1011;     /**< 从设备接收数据失败 */
        public const int ERR_DEC_NETWORK_RECV_TIMEOUT = 1012;    /**< 从设备接收数据超时 */
        public const int ERR_DEC_NETWORK_INVALIDATE = 1013;  /**< ip地址,子网掩码,网关不匹配 */
        public const int ERR_DEC_INVALIDATE_URL = 1014;  /**< 非法的URL */
        public const int ERR_DEC_NOMONITOR = 1015;    /**< 指定显示器不存在 */
        public const int ERR_DEC_NOWINDOW = 1016;     /**< 指定窗口不存在 */
        public const int ERR_DEC_PICTURE_NONUM = 1017;    /**< 不支持该画面数 */
        public const int ERR_DEC_PICTURE_SHIFT_FISRT = 1018;      /**< 画面数导航切换，已经达最早状态 */
        public const int ERR_DEC_PICTURE_SHIFT_LAST = 1019;   /**< 画面数导航切换，已经达最后状态 */


        [StructLayoutAttribute(LayoutKind.Sequential)]
        //区域坐标
        public struct tagRECT
        {
            int left;
            int top;
            int right;
            int bottom;
        }
        /******************************************************************************
                               SDKNET通用数据结构定义
        *******************************************************************************/

        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagPOINT
        {
            int x;
            int y;
        }
        /**
        * @struct tagNetExceptionType
        * @brief 网络异常信息类型参数
        * @attention**/
      
        public  enum tagNetExceptionType
        {
            NETEXCEPTION_RELEASE = 0, /**< 异常信息已解除，即恢复正常 */
            NETEXCEPTION_FULLCONNECT, /**< 设备端视频连接已超过最大值 */
            NETEXCEPTION_RECONNECT,   /**< 当前连接已经断开，将与设备进行重连 */
            NETEXCEPTION_STOPED,      /**< 与设备的连接完全断开，不再重连 */
            NETEXCEPTION_LOGINFAILED, /**< 用户名/密码错误导致的登录设备失败，将会自动进行重连 */
        }

        /**
        * @struct tagPlayParam
        * @brief 编码类型参数
        * @attention
        */
        public enum tagEncodeType
        {
            ENCODE_MPEG4 = 1,   /**< MPEG4编码 */
            ENCODE_H264,        /**< H264编码 */
            ENCODE_H264_Hi3510, /**< H264 3510编码 */
            ENCODE_MJPEG,       /**< MJPEG编码，暂无用 */
            ENCODE_H265,        /**< H265编码 */
        }

        /**
       * @struct tagRealDataInfo
       * @brief 实时数据流参数
       * @attention
       */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagRealDataInfo
        {
           public  uint lChannel;    /**< 通道号，从0开始 */
            public uint lStreamMode; /**< 码流类型，0-主码流，1-子码流 */
            public tagEncodeType eEncodeType; /**< 编码类型*/
        }

        /**
        * @enum tagRealDataType
        * @brief 回调实时流的数据类型
        * @attention 无
        */
        public enum tagRealDataType
        {
           
            REALDATA_HEAD,   /**< 实时流的头数据 */
            REALDATA_VIDEO,  /**< 实时视频流数据（包括复合流和音视频分开的视频流数据） */
            REALDATA_AUDIO,  /**< 实时音频流数据 */
        }

        /**
        * @struct tagTalkParam
        * @brief 语音对讲的参数
        * @attention
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagTalkParam
        {
            uint nAudioEncode;    /**< 预留，音频编码类型 */
            uint nSamplesPerSec;  /**< 采样频率，取值为：8000，11025，22050，44100 */
            uint nBitsPerSample;  /**< 预留，采样位数，如：8，16 */
            uint nSampleFrmSize;  /**< 采样单位帧缓冲（发送）大小， 512或640（编码）*/
        }

        /**
        * @struct tagAlarmerInfo
        * @brief 报警源设备信息
        * @attention 无
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagAlarmerInfo
        {
            byte[] sDeviceIP;      /**< 报警源设备的IP地址 */
            ushort wLinkPort; /**< 报警源设备的通讯端口 */
        }

        /**
        * @enum tagRealDataType
        * @brief 报警类型
        * @attention 无
        */
        public enum tagAlarmType
        {
            ALARM_UNKNOWN = 0,/**< 未知类型报警 */
            ALARM_INPUT,      /**< 继电器输入报警 */
            ALARM_MOTION,     /**< 移动侦测报警 */
            ALARM_SHELTER,    /**< 视频遮挡报警 */
            ALARM_VIDEOLOST,  /**< 视频丢失报警 */
            ALARM_DEVICEERR,  /**< 预留，设备异常报警 */
        }
        /**
        * @struct tagAlarmerDeviceInfo
        * @brief 报警信息
        * @attention 无
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagAlarmInfo
        {
            tagAlarmType eAlarmType;    /**< 报警类型 */
            uint nAlarmID;      /**< 报警通道号，从1开始，通常表示设备通道号，继电器输入报警时表示继电器输入号；设备异常类型时表示：1-硬盘满，2-硬盘出错，3-网络断开，4-非法访问，5-网络冲突 */
            byte cAlarmStatus; /**< 报警状态，0-无报警，1-有报警。继电器输入报警时， 0-报警取消，1-报警触发，2-报警持续 */
            byte cAlarmArea;   /**< 报警区域号，移动侦测和视频遮挡有效，区域号代表哪个区域发生报警 */
        }

        /**
        * @enum tagPtzCommand
        * @brief 云台控制命令
        * @attention 同时描述了IPCNET_PTZControl接口中2个参数对应的含义和设置，p1表示参数iParam1，p2表示参数iParam2
        */
        public enum tagPtzCommand
        {
            //基本命令
            ZOOM_TELE,      /**< 焦距变大(倍率变大,视野缩小,目标放大),p1速度 */
            ZOOM_WIDE,      /**< 焦距变小(倍率变小,视野放大,目标缩小),p1速度 */
            FOCUS_NEAR,     /**< 焦点前调(目标靠近),p1速度 */
            FOCUS_FAR,      /**< 焦点后调(目标远离),p1速度 */
            IRIS_OPEN,      /**< 光圈扩大,p1速度 */
            IRIS_CLOSE,     /**< 光圈缩小,p1速度 */
            UP,             /**< 上转,p1水平速度,p2垂直速度 */
            DOWN,           /**< 下转,p1水平速度,p2垂直速度 */
            LEFT,           /**< 左转,p1水平速度,p2垂直速度 */
            RIGHT,          /**< 右转,p1水平速度,p2垂直速度 */
            UP_LEFT,        /**< 左上,p1水平速度,p2垂直速度 */
            UP_RIGHT,       /**< 右上,p1水平速度,p2垂直速度 */
            DOWN_LEFT,      /**< 左下,p1水平速度,p2垂直速度 */
            DOWN_RIGHT,     /**< 右下,p1水平速度,p2垂直速度 */

            //预置位操作
            SET_PRESET,     /**< 设置预置点,p1预置点的序号(1-255) */
            GOTO_PRESET,    /**< 转到预置点,p1预置点的序号  */

            //花样扫描
            START_CRUISE,   /**< 开始花样扫描,p1花样扫描的序号(1-4) */
            STOP_CRUISE,    /**< 停止花样扫描,p1花样扫描的序号 */
            RUN_CRUISE,     /**< 运行花样扫描,p1花样扫描的序号 */

            //自动水平运行
            START_AUTO_PAN, /**< 开始自动水平运行,p1自动水平运行的序号(1-4) */
            STOP_AUTO_PAN,  /**< 停止自动水平运行,p1自动水平运行的序号 */
            RUN_AUTO_PAN,   /**< 运行自动水平运行,p1自动水平运行的序号 */

            AUTO_SCAN,      /**< 自动扫描 */
            FLIP,           /**< 翻转 */
            STOP,           /**< 停止 */
            ENTER_MENU,     /**< 进入菜单 */

            //辅助开关/继电器
            AUX_PWRON,      /**< 打开辅助设备开关,p1辅助开关号(1-雨刷,2-灯光,3-加热器) */
            AUX_PWROFF,     /**< 关闭辅助设备开关,p1辅助开关号(1-雨刷,2-灯光,3-加热器) */

            //自动老化模式命令Automatic aging model
            AUTO_AGING_PTZ_STOP,//停止命令，在停止缩放和聚焦时使用
            AUTO_AGING_FOCUS_NEAR,//放大命令
            AUTO_AGING_FOCUS_FAR,//缩小命令
            AUTO_AGING_ZOOM_TELE,//聚焦+ 命令
            AUTO_AGING_ZOOM_WIDE,//聚焦- 命令
            AUTO_AGING_LOCK_FOCAL1,//锁焦1命令
            AUTO_AGING_LOCK_FOCAL2,//锁焦2命令
            AUTO_AGING_MANUAL_CUTTING_COLOR,//手动切彩色模式命令
            AUTO_AGING_MANUAL_CUTTING_BLACK_WHITE,//手动切黑白模式命令
            AUTO_AGING_MANUAL_CUTTING_ICUT,//自动切换ICUT模式命令
            AUTO_AGING_OLD_MODE,//自动老化模式命令
            AUTO_AGING_NORMAL_MODE,//正常模式命令，在停止自动切换模式和自动老化模式时使用
            AUTO_AGING_SET_TIMES,//设置机芯倍数命令
            AUTO_AGING_SET_FOCUS_LEVEL,//设置机芯聚焦等级命令
            AUTO_AGING_RESET,//复位机芯命令
        }


        /******************************************************************************
           解码器数据结构定义
        *******************************************************************************/

        /**
        * @struct tagIPC_DEVICEINFO
        * @brief 登录后返回的设备信息
        * @attention 目前返回的只有lDevTypeNumber
*/
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagIPC_DEVICEINFO
        {
            uint lDevTypeNumber;                 /**设备编号,主类型+次类型;如0x00010002为主类型1+次类型2 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = SERIALNO_LEN, ArraySubType = UnmanagedType.U4)]
            byte[] pszDevSerialNO;   /**设备序列号 */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_NAME_LEN_32, ArraySubType = UnmanagedType.U4)]
            byte[] pszDevName;    /**设备名称 */
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_VERSION_LEN, ArraySubType = UnmanagedType.U4)]
            byte[] pszDevSoftVer; /**设备版本号 */
	        byte  cAlarmInPortNum;                /**报警输入个数 */
	        byte  cAlarmOutPortNum;               /**报警输出个数 */
	        byte  cChanNum;                       /**通道个数,模拟的 */
	        byte  cIPChanNum;                     /**最大数字通道个数,保留 */
	        byte  cZeroChanNum;                   /**零通道编码个数,DVR需要 */
	        byte  cDiskNum;				       /**硬盘个数,DVR需要 */
	        byte  cAudioChanNum;				   /**语音通道个数,DVR需要 */
	        byte  cMainProto;					   /**主码流协议类型 */
	        byte  cSubProto;					   /**子码流协议类型 */
	        byte  cSupport;                    /**能力,位与结果为0表示不支持,1表示支持*/
            /**cSupport & 0x1,表示是否支持搜索
               cSupport & 0x2,表示是否支持回放
               cSupport & 0x4,表示是否支持能力集获取 */


            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.U4)]
            byte[] cReserve;                      /**预留 */
        }

        /**
        * @enum tagCamType
        * @brief 前端设备(camera)类型
        * @attention 0表示停止播放
        */
        public enum tagCamType
        {
            CAM_STOP = 0,      /**停止播放 */
            CAM_SERIAL_M,      /**M系列 */
            CAM_SERIAL_N,      /**N系列 */
            CAM_SERIAL_K,      /**K系列 */
            CAM_SERIAL_G,      /**G系列 */
            CAM_SERIAL_T,      /**T系列 */
            CAM_SERIAL_END     /*结束标记（无效） */
        }
     

        /**
        * @enum tagStreamType
        * @brief 前端设备(camera)码流类型
        * @attention 无
        */
        public enum tagStreamType
        {
            ENCODE_MPEG4_MAJOR = 1,   /**mpeg4主码流 */
            ENCODE_MPEG4_MINOR,       /**mpeg4副码流 */
            ENCODE_MOTION_JPEG,       /**MJPEG */
            ENCODE_H264_MAJOR,        /**h.264主码流 */
            ENCODE_H264_MINOR,        /**h.264副码流 */
            ENCODE_HI3510_H264_MAJOR,/**Hi3510_h.264主码流 */
            ENCODE_HI3510_H264_MINOR, /**Hi3510_h.264副码流 */
            ENCODE_END                /**结束标记 */
        }
      

            /**
            * @enum tagPictureNumType
            * @brief 画面数
            * @attention 目前只支持4画面,以后可以扩充
            */
            public enum tagPictureNumType
            {
                PICTURE_NUM_ONE = 1,   /**1画面 */
                PICTURE_NUM_FORE = 4,  /**4画面 */
                PICTURE_NUM_NINE = 9,  /**9画面 */
                PICTURE_NUM_SIXTEEN = 16,/**16画面*/
                PICTURE_NUM_END        /**结束,扩展在此前添加 */
            }


        /**
        * @struct tagVIDEO_SHIFT_INFO
        * @brief 视频切换信息
        * @attention 无
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagVIDEO_SHIFT_INFO
        {
            tagCamType eDevType;         /**前端设备类型号,为0是表示停止解码 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_URL_LEN, ArraySubType = UnmanagedType.U4)]

            byte[] pszURL;   /**视频源地址
										 IPC://<IP address>/Port/Channel/Codec/UserName/PassWord/
										 采用默认用户名密码时可以不填UserName和PassWord.
										 Port:采用TCP协议,90端口.
										 Channel:通道编号:以0为计数基点,取非负数.
										 Codec: 如上的E_STREAM_TYPE. 
										 例：IPC://192.168.1.100/90/0/1 */
            char cMonitorID;            /**显示器编号 */
            char cWindowID;         /**窗口编号 */
            ushort sExtLength;          /**扩展位 */
        }

        /**
        * @struct tagPICTURE_SHIFT_INFO
        * @brief 画面切换信息
        * @attention 无
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public struct tagPICTURE_SHIFT_INFO
        {
            char cShiftDir;          /**切换方向,默认0正常切换,其他暂不支持 */
            char cMonitorID;          /**显示器编号 */
            char cPicNum;             /**画面数量,目前支持1画面和4画面之间的切换 */
            char cFirstPic;       /**首窗口显示画面编号,可指定对第几个画面进行单屏显示 */
        }


        /**
         * @struct tagNET_INFO
         * @brief 网络参数信息
         * @attention 无
       */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public  struct tagNET_INFO
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[]  pszIPAddress;  /**IP地址 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[] pszNetMask;    /**子网掩码 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[] pszNetGate;    /**网关 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[] pszDNS;        /**DNS服务器 */

            byte cAutoGet;         /**自动获取 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            byte[] pszExtLength;  /**扩展位 */
        }


        /**
        * @struct tagDEV_PARAM_ALL
        * @brief 设备参数信息
        * @attention 无
        */
        [StructLayoutAttribute(LayoutKind.Sequential)]
        public  struct tagDEV_PARAM_ALL
        {
            byte cNetNum;       /**psNetInfo结构体个数,固定为1 */
            byte cMonitorNum;   /**psPictureInfo结构体个数 */
            byte cVideoNum;      /**psVideoInfo结构体个数 */
            byte cFill;          /**填充字节 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 1, ArraySubType = UnmanagedType.Struct)]
            tagNET_INFO[]  psNetInfo;  //**网络信息结构体 */tagNET_INFO;

            IntPtr  psPictureInfo;//**画面切换信息 */tagPICTURE_SHIFT_INFO
            IntPtr  psVideoInfo;  //**视频切换信息 */tagPICTURE_SHIFT_INFO
            byte cPictureNum;   /**可支持的画面切换种类 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 31, ArraySubType = UnmanagedType.I1)]
            byte[] cPictureValue;/**可支持的画面切换个数 */
        }

        /**
        * @enum tagGetParamCmd
        * @brief 获取设备参数操作类型
        * @attention 目前只支持解码器
        */
        public  enum tagGetParamCmd
        {
            /***********解码器部分********/
            PARAM_GET_DECODER_V2524All = 0,      /**解码器V2524所有参数 */
            /***********结束**************/

            PARAM_GET_END
        }

        /**
        * @enum tagSetParamCmd
        * @brief 设置设备参数操作类型
        * @attention 目前只支持解码器
        */
        public  enum tagSetParamCmd
        {
            /*************解码器部分************/
            PARAM_SET_DECODER_VIDEOSHIFT = 0,      /**解码器视频切换 */
            PARAM_SET_DECODER_PICTURESHIFT,        /**画面切换 */
            PARAM_SET_DECODER_NETWORKSET,          /**网络设置 */
            PARAM_SET_DECODER_SETOPTION,           /**解码器启动选项设置 */
            /*************结束*****************/

            PARAM_SET_END
        }

        //停车场车位车牌识别
        /**
        * @struct tagIceDevInfo
        * @brief 车位智能识别设备信息
        * @attention 无
        */
       public  struct tagIceDevInfo
        {
            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 128, ArraySubType = UnmanagedType.I1)]

            byte[] sDeviceIP;      /**< 车位智能识别设备的IP地址 */
            ushort wLinkPort; /**< 车位智能识别设备的通讯端口 */
        }

        /**
        * @struct tagIpcIce_ParkingPlaceState
        * @brief 当前车位状态
        * @attention 无
        */
        public  struct tagIpcIce_ParkingPlaceState
        {
            int state;//0时不合法，1为当前车位上的车牌号，2时当前车位是空，

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 8, ArraySubType = UnmanagedType.I1)]
            byte[] placeno;//车位编号

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]
            byte[] plate;//state1时有效
            int plate_color;//车牌颜色
            int plate_type;//车牌类型

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 64, ArraySubType = UnmanagedType.I1)]
            byte[] reserved;
        }

        /**
        * @struct tagIpcIce_Recognition_Result
        * @brief 车位智能算法识别结果
        * @attention 无
        */
        public  struct tagIpcIce_Recognition_Result
        {
            int changed;//代表当前状态有无变化，有变化1，无变化0，60秒未收到设备发来的包则说明设备断线
            int place_number;//车位数量，不能超过MAX_PARKING_PLACE_NUM
            int jpeg_length;//车牌照片长度

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = MAX_PARKING_PLACE_NUM, ArraySubType = UnmanagedType.Struct)]
            tagIpcIce_ParkingPlaceState[] PlateResult;

        }


        //停车场卡口车牌识别
        /**
        * @struct tagIpcVLPR_Recognition_Result
        * @brief 车牌智能算法识别结果
        * @attention 无
        */
        public  struct tagHankIce_VPLR_Result
        {
            int alarm_type;             //代表当前报警类型VLPR_ALARM_T
            int alarm_ip;               //代表设备IP地址
            int cur_time;               //当前时间(从1970-1-1至今过去的秒数)
            int jpeg_length;            //图片长度,若为0则无图片
            int jpeg_width;             //图片分辨率的宽度
            int jpeg_height;            //图片分辨率的高度
            int has_plate;              //有无车牌,若为0则其后成员无效

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 16, ArraySubType = UnmanagedType.I1)]

            byte[] plate_number;      //车牌号
            int plate_color;            //车牌颜色,ICE_PLATECOLOR_E
            int plate_top;              //车牌矩形上边缘坐标,0~100
            int plate_bottom;           //车牌矩形下边缘坐标,0~100
            int plate_left;             //车牌矩形左边缘坐标,0~100
            int plate_right;            //车牌矩形右边缘坐标,0~100
            int past_result_fromTFcard; //标示当前结果是否是过去的TF卡中保存的结果

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 60, ArraySubType = UnmanagedType.I1)]
            byte[] reserved;
        }

        //3D指令
        public  struct tag3D_TMP_INFO
        {
            int midx;////坐标中心的x值。
            int midy;//坐标中心的y值。
            byte actionType;//1:不放大;2:放大4倍;3:放大;4:缩小
        }


        /**
        * @struct tagHeartbeatInfo
        * @brief 心跳信息
        * @attention
        */
        public  struct tagHeartbeatInfo
        {
            char cLive;         /**< 是否有心跳，0-有，1-无 */

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 19, ArraySubType = UnmanagedType.I1)]
            byte[] cReserved; /**< 预留 */
        }

        /**
        * @struct tagLanSearchInfo
        * @brief 局域网设备搜索反馈信息
        * @attention 无
        */
        public  struct tagLanSearchInfo
        {
            int nDevType;//设备类型 若该值为DEVICE_TYPE_IPC则为网络摄像机，若该值为DEVICE_TYPE_NVR则为网络硬盘录像机

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[] ucSoftWareVersion;//设备的软件版本

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[] ucDeviceIP;//设备IP地址

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[] ucSubnetIP;//设备子网地址

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 4, ArraySubType = UnmanagedType.I1)]
            byte[] ucGateway;//设备网关

            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 6, ArraySubType = UnmanagedType.I1)]
            byte[] ucDevMac;//设备MAC地址

            int nHttpPort;//HTTP端口号

            int nVideoPort;//视频端口号

            int nRtspPort;//RTSP端口号

            byte ucReserved; // 保留字节，未使用

        }


        /**
        * @enum tagSerialReqType
        * @brief 透明通道命令类型
        * @attention 无
        */
        public enum tagSerialReqType
        {
            SERIAL_SWITCH_TYPE = 0, //0 透明通道开关切换
            SERIAL_CONNECT_TYPE,    //1 设置透明通道连接方式
            SERIAL_SET_TYPE,        //2 设置透明通道参数
            SERIAL_SEND_TYPE        //3 透明通道发送数据
        }
                                                                                                                                  

        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool IPCNET_Init();


        //登录信息 IP 端口 用户名 密码 等
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern Int32 IPCNET_Login(
                                     string sDevIP,
                                     uint nDevPort,
                                     string sUserName,
                                     string sPassword,
                                     bool bIsValidation=false);




        /**
        * 接收网络连接异常信息的回调函数
        * @param [IN] lLoginID    登陆的ID，IPCNET_Login的返回值 
        * @param [IN] lRealHandle 当前实时数据获取的句柄
        * @param [IN] eNetMsgType 网络连接异常信息的类型
        * @param [IN] pUserData   用户自定义的数据
        * @return 无
        * @note 无
        */

        public delegate void CBNetException(int lLoginID,
                                        int lRealHandle,
                                        tagNetExceptionType eNetMsgType,                                       
                                        IntPtr pUserData);

        /**
        * 设置接收网络连接异常信息的回调函数
        * @param [IN] fNetExceptionMsg 接收异常信息的回调函数，为NULL表示不接收异常信息
        * @param [IN] pUserData        用户自定义的数据，回调函数原值返回
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用IPCNET_GetLastError
        * @note 无
        */
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool IPCNET_SetNetExceptionCallBack(CBNetException fNetExceptionMsg,
                                                               IntPtr pUserData );


        /**
        * 释放SDK资源，在结束之前最后调用
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用IPCNET_GetLastError
        * @note 无
        */
        [DllImport(@"IPCSDK_NET.dll")]
        public static extern bool IPCNET_Cleanup();


        /**
       * 获取错误码
       * @return 返回值为错误码
       * @note 无
       */
        [DllImport(@"IPCSDK_NET.dll")]
        public static extern long IPCNET_GetLastError();

     

        //注销
        [DllImport(@"IPCSDK_NET.dll")]
        public static extern bool  IPCNET_Logout(Int32 lLoginID);

        //
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool HDVPLAY_Init();


            /******************************************************************************
    解码器用户登陆接口
    *******************************************************************************/
        /**
        * 用户登陆
        * @param [IN]   sDevIP    设备IP地址
        * @param [IN]   nDevPort  设备端口号
        * @param [IN]   sUserName 登录的用户名,最大长度为32字节
        * @param [IN]   sPassword 用户密码,最大长度为32字节
        * @param [OUT]  psDeviceInfo 用户登录后设备返回的信息
        * @return 返回如下结果:
        * - 失败:-1
        * - 其他值:表示返回的用户ID值.该用户ID具有唯一性,后续对设备的操作都需要通过此ID实现
        * - 获取错误码调用IPCNET_GetLastError
        * @note 无
        */
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern Int32   IPCNET_LoginDec(string sDevIP,

                                     uint nDevPort,
                                     string  sUserName,
                                     string sPassword,
                                     tagIPC_DEVICEINFO psDeviceInfo);


        /******************************************************************************
              //解码器用户注销接口
       *******************************************************************************/
        /**
        * 用户注销
        * @param [IN]   lLoginID 用户ID号,IPCNET_Login的返回值
        * @return 返回如下结果:
        * - 成功:true
        * - 失败:false
        * - 获取错误码调用IPCNET_GetLastError
        * @note 只用于解码器,与IPCNET_LoginDec配合使用
        */
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool  IPCNET_LogoutDec(Int32 lLoginID);

        /******************************************************************************
        SDKNET监听上传报警接口
        *******************************************************************************/
        /**
        * 接收设备主动上传报警回调函数的指针类型
        * @param [IN]   pAlarmer    报警源设备的信息
        * @param [IN]   pAlarmInfo  报警信息 
        * @param [IN]   pUserData   用户自定义的数据
        * @return 无
        * @note 无
*/
        public delegate void  CBAlarmMsg
            (
            ref tagAlarmerInfo pAlarmer,
            ref tagAlarmInfo  pAlarmInfo,
            IntPtr pUserData
            );
        /******************************************************************************
        SDKNET实时流获取接口
        *******************************************************************************/
        /**
        * 实时码流数据回调函数的指针类型
        * @param [IN]   lRealHandle 当前的实时数据的句柄
        * @param [IN]   eDataType   回调的数据类型
        * @param [IN]   pBuffer     存放数据的缓冲区指针
        * @param [IN]   lBufSize    存放数据的缓冲区大小
        * @param [IN]   pUserData   用户数据，调用IPCNET_StartRealData时用户输入的值
        * @return 无
        * @note 无
*/
        public delegate void  CBRealData(
                                    Int32 lRealHandle,
                                   tagRealDataType eDataType,
                                   IntPtr  pBuffer,
                                   uint lBufSize,
                                   IntPtr pUserData);


            /**
            * 开始实时数据获取
            * @param [IN]   lLoginID      登陆的ID，IPCNET_Login的返回值
            * @param [IN]   sRealDataInfo 实时数据流的参数结构体
            * @param [IN]   fRealData     码流数据回调函数
            * @param [IN]   pUserData     用户自定义的数据，回调函数原值返回
            * @return 返回如下结果：
            * - 失败：-1
            * - 其他值：作为IPCNET_StopRealData等函数的句柄参数
            * - 获取错误码调用IPCNET_GetLastError
            * @note 无
            */
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern  Int32 IPCNET_StartRealData(
            Int32 lLoginID,
            ref tagRealDataInfo pRealDataInfo,
            CBRealData fRealData,
           IntPtr pUserData );
                /**
        * 停止实时数据获取
        * @param [IN]   lRealHandle 实时数据的句柄，IPCNET_StartRealData的返回值
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用IPCNET_GetLastError
        * @note 无
        */
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool IPCNET_StopRealData(Int32 lRealHandle);



                /******************************************************************************
        SDKNET标准数据流获取接口
        *******************************************************************************/
        /**
        * 标准数据的媒体信息，每次回调都会附带此信息。
        **/
        public  struct tagAV_INFO
        {
            byte m_AVType;     //音视频类型，1--视频，2--音频
            byte m_EncoderType;   //编码类型，  1--H264，2--MPEG4，3--G711_U（音频），4--H265
            byte m_FrameType;     //帧类型，    1--I帧， 2--P帧，		如果是音频帧，则为0
            byte m_FrameRate;      //帧率，	如果是音频帧，则为0
            ushort m_VideoWidth;    //视频宽度，如果是音频帧，则为0
            ushort m_VideoHeight;   //视频高度，如果是音频帧，则为0


            [MarshalAsAttribute(UnmanagedType.ByValArray, SizeConst = 3, ArraySubType = UnmanagedType.I1)]
            byte[] m_Reserved;    //保留
            byte m_Channels;       //通道类型，  1--单声道，2--双声道，如果是视频帧，则为0
            ushort m_Samples;       //采样率，  如果是视频帧，则为0
            ushort m_BitCount;      //采样位数，如果是视频帧，则为0
            UInt32 m_TimeStamp;     //时间戳
        }

                /**
        * 标准码流数据回调函数的指针类型
        * @param [IN]   lRawHandle     当前的原始数据的句柄
        * @param [IN]   pAVInfo		   数据所对应的媒体信息，可针对此信息对音视频做不同处理
        * @param [IN]   pRawBuffer     存放数据的缓冲区指针
        * @param [IN]   lRawBufSize    存放数据的缓冲区大小
        * @param [IN]   pUserData      用户数据，调用IPCNET_StartRawData时用户输入的值
        * @return 无
        * @note 
        */
        public  delegate void CBRawData
            (
                                    Int32 lRawHandle,
                                    ref tagAV_INFO  pAVInfo,
								   IntPtr pRawBuffer,
                                   UInt32 lRawBufSize,
                                   IntPtr pUserData
            );


                /**
        * 开始获取标准数据
        * @param [IN]   lLoginID      登陆的ID，IPCNET_Login的返回值
        * @param [IN]   sRawDataInfo  实时数据流的参数结构体
        * @param [IN]   fRawData      标准码流数据回调函数
        * @param [IN]   pUserData     用户自定义的数据，回调函数原值返回
        * @return 返回如下结果：
        * - 失败：-1
        * - 其他值：作为IPCNET_StopRawData等函数的句柄参数
        * - 获取错误码调用IPCNET_GetLastError
        * @note 与实时流获取是相互独立的，即可在不开启实时预览功能时直接获取标准数据
        */
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern  Int32  IPCNET_StartRawData(Int32 lLoginID,
                                                    tagRealDataInfo pRawDataInfo,
                                                    CBRawData fRawData,
                                                    IntPtr pUserData );


                /**
        * 停止获取标准数据
        * @param [IN]   lRawHandle    IPCNET_StartRawData的返回值
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用IPCNET_GetLastError
        * @note 无
        */
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool  IPCNET_StopRawData(Int32 lRawHandle);


        /******************************************************************************
SDKNET云台控制接口
*******************************************************************************/
        /**
        * 云台控制接口，不用启动预览时也可以使用
        * @param [IN]   lLoginID    登陆的ID，IPCNET_Login的返回值
        * @param [IN]   nChannel    设备通道号， 从0开始
        * @param [IN]   ePTZCommand 云台控制命令
        * @param [IN]   iParam1     参数1，具体内容跟控制命令有关，详见E_PTZ_COMMAND
        * @param [IN]   iParam2     参数2，同上
        * @param [IN]   bStop       是否停止，对云台八方向操作及镜头操作命令有效，进行其他操作时，本参数应填充false
        * @return 返回如下结果：
        * - 成功：true
        * - 失败：false
        * - 获取错误码调用IPCNET_GetLastError
        * @note 当iParam1表示速度时，范围是1~8
*/
        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool  IPCNET_PTZControl(long lLoginID,
                                                  uint nChannel,
                                                  tagPtzCommand ePTZCommand,
                                                  int iParam1 = 6,
                                                  int iParam2 = 6,
                                                  bool bStop = false);

                    /**
            * PTZ区域选择放大接口
            * @param [IN] lLoginID   登陆的ID，IPCNET_Login的返回值
            * @param [IN] nChannel   设备通道号，从0开始
            * @param [IN] rcSelWnd   选择要放大的客户区域坐标
            * @param [IN] rcVideoWnd 视频显示框的客户区域坐标
            * @return 返回如下结果：
            * - 成功：true
            * - 失败：false
            * - 获取错误码调用IPCNET_GetLastError
            */


        [DllImport(@"IPCSDK_Net.dll")]
        public static extern bool IPCNET_PTZSelZoomIn(Int32 lLoginID,
                                                   uint nChannel,
                                                   ref tagRECT rcSelWnd,
                                                   ref tagRECT rcVideoWnd,
                                                   ref tag3D_TMP_INFO tmp3DInfo);


    

    }
}

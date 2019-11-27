
using handGameControl;
using prjInfoSetting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc.tanuo;
using xjplc.TcpDevice;
using xm;

namespace xjplc
{


    //工程信息
    public class  projInfo
    {
        ConfigFileManager cfg;

        string prjName;
        public string PrjName
        {
            get { return prjName; }
            set { prjName = value; }
        }
        string prjOperator;
        public string PrjOperator
        {
            get { return prjOperator; }
            set { prjOperator = value; }
        }
        string prjDevice;
        public string PrjDevice
        {
            get { return prjDevice; }
            set { prjDevice = value; }
        }
        string prjCompany;
        public string PrjCompany
        {
            get { return prjCompany; }
            set { prjCompany = value; }
        }
        string prjDirection;
        public string PrjDirection
        {
            get { return prjDirection; }
            set { prjDirection = value; }
        }
        string prjAddress;
        public string PrjAddress
        {
            get { return prjAddress; }
            set { prjAddress = value; }
        }
        string prjInfo;
        public string PrjInfo
        {
            get { return prjInfo; }
            set { prjInfo = value; }
        }
        //检测井号
        string insQdJingHao;
        public string InsQdJingHao
        {
            get { return insQdJingHao; }
            set { insQdJingHao = value; }
        }
        string insZdJingHao;
        public string InsZdJingHao
        {
            get { return insZdJingHao; }
            set { insZdJingHao = value; }
        }
        string insJingKouGaoCheng;
        public string InsJingKouGaoCheng
        {
            get { return insJingKouGaoCheng; }
            set { insJingKouGaoCheng = value; }
        }
        string insJingKouJingdu;
        public string InsJingKouJingdu
        {
            get { return insJingKouJingdu; }
            set { insJingKouJingdu = value; }
        }
        string insJingKouJWeidu;
        public string InsJingKouJWeidu
        {
            get { return insJingKouJWeidu; }
            set { insJingKouJWeidu = value; }
        }
        string insJingHao;
        public string InsJingHao
        {
            get { return insJingHao; }
            set { insJingHao = value; }
        }
        //管道信息
        string insGuanDuanTypeID;
        public string InsGuanDuanTypeID
        {
            get { return insGuanDuanTypeID; }
            set { insGuanDuanTypeID = value; }
        }
        string insGuanDuanMaterialID;
        public string InsGuanDuanMaterialID
        {
            get { return insGuanDuanMaterialID; }
            set { insGuanDuanMaterialID = value; }
        }
        string insGuanDuanDiameter;
        public string InsGuanDuanDiameter
        {
            get { return insGuanDuanDiameter; }
            set { insGuanDuanDiameter = value; }
        }
        string insGuanDuanLength;
        public string InsGuanDuanLength
        {
            get { return insGuanDuanLength; }
            set { insGuanDuanLength = value; }
        }
        string insGuanDuanZgLength;
        public string InsGuanDuanZgLength
        {
            get { return insGuanDuanZgLength; }
            set { insGuanDuanZgLength = value; }
        }
        //建设信息
        string pipeBelongCompany;
        public string PipeBelongCompany
        {
            get { return pipeBelongCompany; }
            set { pipeBelongCompany = value; }
        }
        string designSlope;
        public string DesignSlope
        {
            get { return designSlope; }
            set { designSlope = value; }
        }
        string designDepth;
        public string DesignDepth
        {
            get { return designDepth; }
            set { designDepth = value; }
        }
        string buildDateTime;
        public string BuildDateTime
        {
            get { return buildDateTime; }
            set { buildDateTime = value; }
        }

        //评估标准
        string evalType;
        public string EvalType
        {
            get { return evalType; }
            set { evalType = value; }
        }
        string areaImportance;
        public string AreaImportance
        {
            get { return areaImportance; }
            set { areaImportance = value; }
        }
        string soil;
        public string Soil
        {
            get { return soil; }
            set { soil = value; }
        }
        string payLoad;
        public string PayLoad
        {
            get { return payLoad; }
            set { payLoad = value; }
        }

        string strExplain;
        public string StrExplain
        {
            get { return strExplain;  }
            set { strExplain = value; }
        }

        List<string> strLst;
        public projInfo()
        {
            strLst = new List<string>();
            cfg = new ConfigFileManager(Constant.cfgPrjFile);       
        }

        public delegate void showPrjdata(string[] str);

        public showPrjdata sData;
        public void  ReadData()
        {
            string[] s2 = cfg.ReadConfig(prjCfg.demoStr, prjCfg.ListStr).Split(prjCfg.splitChar);
            setData(s2);
            sData(s2);
        }
        public void setData(string[] s)
        {
            if (s.Length != 25) return;

            for (int i = 0; i < s.Count(); i++)
            {
                s[i] = s[i].Replace("\r\n", "");
            }
           
            PrjName      = s[0];
            PrjOperator  = s[1];
            PrjDevice    = s[2];
            PrjCompany   = s[3];
            PrjDirection = s[4];
            PrjAddress   = s[5];
            InsQdJingHao = s[6];
            InsZdJingHao = s[7];
            InsJingKouGaoCheng = s[8];
            InsJingKouJingdu = s[9];
            InsJingKouJWeidu = s[10];
            InsGuanDuanTypeID = s[11];
            InsGuanDuanMaterialID = s[12];
            InsGuanDuanDiameter = s[13];
            InsGuanDuanLength = s[14];
            InsGuanDuanZgLength = s[15];
            PipeBelongCompany = s[16];
            DesignSlope = s[17];
            DesignDepth = s[18];
            BuildDateTime = s[19];
            EvalType = s[20];
            AreaImportance = s[21];
            Soil = s[22];
            PayLoad = s[23];
            StrExplain = s[24];

            string strLst="";

            for (int i = 0; i < s.Length; i++)
            {
                if(i != s.Length-1)
                strLst = strLst + s[i] + prjCfg.splitChar;
                else strLst = strLst + s[i] ;
            }
            if (strLst.Length > 0)
            {              
               cfg.WriteConfig(prjCfg.demoStr, prjCfg.ListStr, strLst);
               //MessageBox.Show("参数已保存！");
            }        
        }

    }

    //设备管理类
    public class DeviceManageer
    {

        #region 字符常量


        const string configStr = "config";

        const string devName1 = "卷线盘";
        const string devName2 = "小车";
        const string devName3 = "后视";
        const string devName4 = "前视";


        const string ParamStr = "Param";
        #endregion
        #region 全局信息
        //图片 视频保存文件夹
        string tkxs;
        string videoPath;
        string imgPath;
        string imgType;
        string carLen;
        string stLen;
        string carIndex;

        public bool Status
        {
            get
            {
                if (this != null
                    && handCam != null
                    && Zd != null
                    && Zd.Status
                    //&& Jxp!=null
                   // && Jxp.Status
                    )
                    return true;

                return false;

            }
        }
        //全局复位
        public void reset()
        {
            LogManager.WriteProgramLog("手动初始化！");
            LoginInCam();
            ZdReset();
            JxpReset();

        }
        ConfigFileManager config;
               
        void Init()
        {

            config = new ConfigFileManager(Constant.ConfigParamFilePath);

            // InitHKCamera();

            InitHankCam();

            InitZd();

            InitPrjInfo();

            InitSysInfo();

            InitJxp();

            InitHand();

            InitJsControl();

            //InitXMCam();        

        }
        public void Dispose()
        {
            if (HKCam != null)
                HKCam.dispose();
            if (camxm != null)
                camxm.dispose();

            if (handCam != null)
            {
                handCam.dispose();
            }
        }
        public DeviceManageer(Action<string> s, PictureBox phk, PictureBox pxm)
        {
            ShowConnAction = s;
            hkCamRealPlayWnd = phk;
            xmCamRealPlayWnd = pxm;
            Init();

        }
        //在连接画面上显示字符
        public Action<string> ShowConnAction;

        //在主页面显示冒泡字符
        public Action<string> ShowTipsAction ;

        void ShowTips(string str)
        {
            ShowTipsAction?.Invoke(str);
        }
        void ShowConn(string str)
        {
            ShowConnAction?.Invoke(str);
        }

        public void ShowParamCfgForm()
        {
            sysForm.Visible = true;
        }
        #endregion
        #region 系统页面参数
        //显示参数代理
        public delegate void showParamFomr(Dictionary<string, string> s);
        public showParamFomr showpara;
        //工程信息
        projInfo prj;
        //系统页面
        sysInfo sysForm;

        //工程配置页面
        prjCfg vsForm;
        //系统页面初始化 
        void InitSysInfo()
        {
            sysForm = new sysInfo();
            sysForm.Hide();
            sysForm.setParam = setParam;
            showpara = sysForm.ShowSysParam;
            showParam();
            sysForm.SaveLoadParam();
        }

        void showParam()
        {

            Dictionary<string, string> strLst = new Dictionary<string, string>();

            foreach (string key in sysInfo.param)
            {
                strLst.Add(key, config.ReadConfig(ParamStr, configStr, key));
            }
            tkxs = strLst[sysInfo.param[0]];
            videoPath = strLst[sysInfo.param[1]];
            imgPath = strLst[sysInfo.param[2]];
            imgType = strLst[sysInfo.param[3]];
            carLen = strLst[sysInfo.param[16]];
            stLen = strLst[sysInfo.param[17]];
            carIndex = strLst[sysInfo.param[18]];

            if (showpara != null)
                showpara(strLst);

        }

        #endregion                                          
        #region 终端
        ZdDevice zd;
        public xjplc.TcpDevice.ZdDevice Zd
        {
            get { return zd; }
            set { zd = value; }
        }
        void InitZd()
        {

            ServerInfo p0 = new ServerInfo();

            p0.server_Ip = config.ReadConfig(ParamStr, configStr, sysInfo.param[14]);

            p0.server_Port = config.ReadConfig(ParamStr, configStr, sysInfo.param[15]);

            zd = new ZdDevice(p0);
            zd.DeviceName = devName2;
            zd.DeviceId = Constant.devicePropertyB;
            //尺寸运动初始化
            ContinueMonitorTimer = new System.Timers.Timer();
            ContinueMonitorTimer.Elapsed += ContinueMonitorTimerEvent;
            ContinueMonitorTimer.Interval = 1000;
            ContinueEnableCount = 0;

            ShowConn("初始化小车");
            if (!zd.OpenTcpClient())
            {
                ShowConn("小车网络连接失败");
            }
        }
        //倾斜角度 翻滚角度当前值 如果变化了 就改变角度显示
        int qxjd = 0;
        int fgjd = 0;
        public void ShowQxFgPic(PictureBox qxP, PictureBox fgP, Label qx, Label fg, Image image2)
        {
            qx.Text = Zd.CurrentQxValue.ToString();
            fg.Text = Zd.CurrentFgValue.ToString();
            Bitmap image = new Bitmap(image2);
            //如果当前值变了
            if (Zd.CurrentQxValue != qxjd)
            {
                qxP.Image = ConstantMethod.RotateImg(image, (-1) * Zd.CurrentQxValue);
                qxjd = Zd.CurrentQxValue;
            }

            //如果当前值变了
            if (Zd.CurrentFgValue != fgjd)
            {
                fgP.Image = ConstantMethod.RotateImg(image, (-1) * Zd.CurrentFgValue);
                fgjd = Zd.CurrentFgValue;
            }


        }
        void ZdReset()
        {          
            if (Zd != null && !Zd.Status)
            {
                ShowConn("初始化小车");
                if (!Zd.Reset())
                {
                    ShowConn("小车网络连接失败");
                }
            }
        }
        //云台运动
        public void CamPTStart(int id)
        {           
            if (Zd == null) return;
            Zd. CamPTStart(id);
        }

        public void   PTRst()
        {
            if(!Zd.CamPTStart(5)) ShowTips("云台复位失败，请检查云台状态!");       
        }

        string qxAlarm = "车体倾斜，危险！";      
        int QXangleAlarmMin = 15;
        int QXangleAlarmMax = -15;

        public void ShowQxAlarm(PictureBox qxP, PictureBox fgP, Label qx, Label fg, Image image2)
        {
            if (Zd != null && Zd.Status)
            {
                
                if (Zd.CurrentQxValue < QXangleAlarmMax || Zd.CurrentQxValue > QXangleAlarmMin) ShowTips(qxAlarm);
                ShowQxFgPic(qxP, fgP, qx, fg, image2);
            }
        }

        public void CwOpposite()
        {
            if (Zd != null && Zd.Status)
            {
                Zd.CwOpposite();
            }
        }

        public void showCWStatus(CheckBox  cwchk)
        {
            if (Zd.CwEnable > 0)
            {
                cwchk.Checked = true;
            }
            else
            {
                cwchk.Checked = false;
            }
        }
        public void CwOnOff(bool value)
        {
            if (Zd != null && Zd.Status)
            {
                Zd.CwOnOff(value);
            }
        }
        //收线的时候 小车一起往后退
        public void StartGetLineBackZd()
        {
            if (Zd == null) return;
            Zd.CarStart(5);
        }

        //收线的时候 小车停止一起往后退
        public void StopGetLineBackZd()
        {
            Zd.CarStart(0);
        }

        
        //代表后退按键按下后 的标志 按键抬起就停止持续运动
        bool ReadyToQuitContinueMoveFlag = false;
        public void CarStart(int id)
        {
            if (Zd == null) return;

            //上下左右 其中上要跟踪进行
            switch (id)
            {
                case 1: //上
                    {

                        //持续运动检测器开启
                        OpenContinueMonitorTimer();
                        break;
                    }
                 
                case 2: //
                    {

                        if (ContinueMoveFlag)
                        {
                            ReadyToQuitContinueMoveFlag = true;
                            return;
                        }
                        break;
                    }
                case 0: //
                    {
                        if (ContinueMoveFlag && ReadyToQuitContinueMoveFlag)
                        {
                            StopContinueMove();
                           // ShowTips("持续运动结束！");
                            return;
                        }else 
                        if (ContinueMoveFlag)
                        {
                            ShowTips("持续运动中,请后退解锁！");
                            return;
                        }
                        
                        break;
                    }
                default:
                    {
                        if (ContinueMoveFlag)
                        {
                            //ShowTips("持续运动中,请后退解锁！");
                            return;
                        }
                        break;
                    }
            }

            Zd.CarStart(id);
        }      
         
        public void TaiGanStart(int id)
        {
            Zd.StartGan(id);           
        }

        void ZdParamAdjust(int id)
        {
            if (Zd == null) return;

             switch (id)
            {
                case 1:
                    Zd.incFarLight();
                    if (FarLightChange != null)
                        FarLightChange(Zd.Farlight);
                    break;
                case 2:
                    Zd.decFarLight();
                    if (FarLightChange != null)
                        FarLightChange(Zd.Farlight);
                    break;
                case 3:
                    Zd.incNearLight();
                    if (NearLightChange != null)
                    {
                        NearLightChange(Zd.Nearlight);
                    }
                    break;
                case 4:
                    Zd.decNearLight();
                    if (NearLightChange != null)
                    {
                        NearLightChange(Zd.Nearlight);
                    }
                    break;
                case 5:
                    Zd.incSpeed();
                    if (ZdSpeedChange != null)
                    {
                        ZdSpeedChange(Zd.Speed);
                    }
                    break;
                case 6:
                    Zd.decSpeed();
                    if (ZdSpeedChange != null)
                    {
                        ZdSpeedChange(Zd.Speed);
                    }
                    break;
            }
        }

        public void SetZdSpeed(int v)
        {
            if (Zd != null && Zd.Status)
            {
                if(v<=10 && v>0)
                Zd.SetSpeed(v);
            }
        }
        public void SetLight(int farLight, int nearLight)
        {
            if (Zd != null && Zd.Status)
            {
                Zd.SetLightCTRL(farLight, nearLight);
            }
        }

        #region 持续运动模块
        //持续运动计数 手势保持时间=timerGestureCount*定时器
        int ContinueEnableCount;
        int MaxGestureCount = 3;
        public bool ContinueMoveFlag = false;
        System.Timers.Timer ContinueMonitorTimer;

        private void ContinueMonitorTimerEvent(object sender, EventArgs e)
        {
            ContinueEnableCount++;
            if (ContinueEnableCount >= MaxGestureCount)
            {
                //启动持续运动
                StartContinueMove();

            }

        }
        void OpenContinueMonitorTimer()
        {
            if (!ContinueMoveFlag && !ContinueMonitorTimer.Enabled )
            ContinueMonitorTimer.Enabled = true;
        }
        void StartContinueMove()
        {
            if (!Status) return;
       
           // ShowTips("持续运动中！");
            ContinueMoveFlag = true;
            ContinueMonitorTimer.Enabled = false;
            ContinueEnableCount = 0;

        }
        public void StopContinueMove()
        {
            if (!Status) return;
            if (ContinueMoveFlag)
            {
               // ShowTips("持续运动结束！");
                ContinueMoveFlag = false;
                ContinueEnableCount = 0;
                ContinueMonitorTimer.Enabled = false;
                zd.CarStart(0);
                ReadyToQuitContinueMoveFlag = false;
            }         

        }

        #endregion




        #endregion
        #region 海康摄像头
        Camera camHK;
        public Camera HKCam
        {
            get { return camHK; }
            set { camHK = value; }
        }
        PictureBox hkCamRealPlayWnd;

        bool startRecordId;
        public bool LoginInCam()
        {
            if (config != null && !camHK.IsOnLine)
            {
                ShowConn("初始化相机");

                string ip = config.ReadConfig(ParamStr, configStr, sysInfo.param[4]);
                string port = config.ReadConfig(ParamStr, configStr, sysInfo.param[5]);
                string userName = config.ReadConfig(ParamStr, configStr, sysInfo.param[6]);
                string pwd = config.ReadConfig(ParamStr, configStr, sysInfo.param[7]);

                int portInt = -1;
                if (!string.IsNullOrWhiteSpace(ip)
                    && !string.IsNullOrWhiteSpace(port)
                    && !string.IsNullOrWhiteSpace(userName)
                    
                    && int.TryParse(port, out portInt)
                    && HKCam != null
                    )
                {
                    LoginOutCam();

                    if (HKCam.Login(ip, portInt, userName, pwd))
                    {
                        if (!ListHKCamView(hkCamRealPlayWnd))
                        {
                            ShowConn("前置摄像头预览失败！");
                        }
                        ShowConn("前置摄像头连接成功！");
                    }
                    else
                    {
                        ShowConn("前置摄像头网络连接失败！");
                    }
                }

            }else ShowConn("相机配置文件或者参数错误！");
            return false;
        }

        void setRealPlayWnd(PictureBox p12)
        {
            hkCamRealPlayWnd = p12;
        }
        public bool startRecord()
        {
            string filename = GetCurrentVideoFile();
            if (filename == null)
            {
                MessageBox.Show("视频保存路径错误！");
                return false;
            }

            if (HKCam != null && startRecordId == false)
            {
                startRecordId = HKCam.StartRecord(filename);
                return true;
            }
            return false;

        }
        public void stopRecord()
        {
            string filename = GetCurrentVideoFile();
            if (startRecordId)
            {
                if (HKCam.StopRecord(filename))
                    startRecordId = false;
                return;
            }
        }
        string GetCurrentImageFile()
        {

            string dateTime = DateTime.Now.ToString("yyyyMMdd_HHmmss");

            if (Directory.Exists(imgPath))
            {
                string folder = Path.Combine(imgPath, prj.PrjName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                //return imgPath + "\\" + prj.PrjName + "\\" + dateTime + imgType;
                //return imgPath + "\\"+dateTime + imgType;
                return Path.Combine(imgPath, prj.PrjName, dateTime + imgType); //imgPath + prj.PrjName + dateTime + imgType;
            }

            return null;

        }
        string GetCurrentVideoFile()
        {

            string dateTime = DateTime.Now.ToString("HHmmss");

            if (Directory.Exists(videoPath))
            {
                string folder = Path.Combine(videoPath, prj.PrjName);
                if (!Directory.Exists(folder))
                {
                    Directory.CreateDirectory(folder);
                }
                // return videoPath + prj.PrjName + dateTime + ".mp4";
                //  string str= Path.Combine(folder, prj.InsQdJingHao + "-" + prj.InsZdJingHao + "-", dateTime + ".mp4");

                return Path.Combine(folder, prj.InsQdJingHao + "-" + prj.InsZdJingHao + "-" + dateTime + ".mp4");


            }


            return null;


        }
        public string takePic()
        {
            string filePath = GetCurrentImageFile();
            if (filePath == null)
            {
                return "截图路径错误！";

            }
            if (filePath != null && HKCam != null)
            {
                HKCam.Capture(filePath);

                return filePath;
            }
            return "截图失败！";
        }

        public void InitHKCamera()
        {
            
            HKCam = new Camera();
            HKCam.DevName = devName4;
            LoginInCam();
        }
        public bool LoginOutCam()
        {
            return HKCam.CheckOut();
        }

        public bool ListHKCamView(Control c1)
        {
            if(c1!=null)
            (c1 as PictureBox).SizeMode = PictureBoxSizeMode.Normal;
            return HKCam.ListView(c1);
        }

        string PrjName = "";
        string InsQdJingHao = "";
        string InsGuanDuanDiameter = "";
        string InsZdJingHao = "";
        
        public void ShowHKInfo()
        {
            if (HKCam == null) return;

            if (PrjName != prj.PrjName)
            {
                HKCam.StringAddTest(1, prj.PrjName, 10, 15);
                PrjName = prj.PrjName;
            }

            if (InsQdJingHao != prj.InsQdJingHao || InsZdJingHao != prj.InsZdJingHao)
            {
                HKCam.StringAddTest(2, prj.InsQdJingHao + "---" + prj.InsZdJingHao, 10, 50);
                InsQdJingHao = prj.InsQdJingHao;
                InsZdJingHao = prj.InsZdJingHao;
            }

            if (InsGuanDuanDiameter != prj.InsGuanDuanDiameter)
            {
                HKCam.StringAddTest(3, "DN" + prj.InsGuanDuanDiameter + "mm", 10, 95);
                InsGuanDuanDiameter = prj.InsGuanDuanDiameter;
            }


        }

        public void CamStart(int id)
        {
            if (HKCam != null && HKCam.Status !=0)
                HKCam.StartAdjust(id);
        }

        #endregion
        #region hankVIsion
        HankCamera handCam;

        //初始化
        void InitHankCam()
        {
            handCam = new HankCamera();

            string ip = config.ReadConfig(ParamStr, configStr, sysInfo.param[4]);
            string port = config.ReadConfig(ParamStr, configStr, sysInfo.param[5]);
            string userName = config.ReadConfig(ParamStr, configStr, sysInfo.param[6]);
            string pwd = config.ReadConfig(ParamStr, configStr, sysInfo.param[7]);

            ServerInfo sr = new ServerInfo();

            sr.server_Ip = ip;
            sr.server_Port = port;
            sr.userName = userName;
            sr.userPwd = pwd;
            handCam.ShowhWnd =hkCamRealPlayWnd.Handle;
            handCam.OpenDevice(sr);


        }

        #endregion
        #region 卷线盘
        string oldCodedata = "";
        const string xcAlarm = "线长超限！";
        int xcMax = 145;
        double parambb = 0;
        double paramB = 0;
        //收线中
        bool IsInGetLineFlag=false;

        //强制收线标志
        bool ForceToGetLine = false;

        JxpDevice jxp;
        public xjplc.TcpDevice.JxpDevice Jxp
        {
            get { return jxp; }
            set { jxp = value; }
        }
        void InitJxp()
        {

            ServerInfo p0 = new ServerInfo();

            p0.server_Ip = config.ReadConfig(ParamStr, configStr, sysInfo.param[12]);

            p0.server_Port = config.ReadConfig(ParamStr, configStr, sysInfo.param[13]);

            Jxp = new JxpDevice(p0);
            Jxp.DeviceName = devName1;
            Jxp.DeviceId = Constant.devicePropertyA;
            ShowConn("初始化卷线盘");
            if (!Jxp.OpenTcpClient())
            {
                ShowConn("卷线盘网络连接失败！");
            }

        }
        void JxpReset()
        {
            if (jxp != null && !jxp.Status)
            {
                ShowConn("初始化卷线盘");
                if (!jxp.Reset())
                {
                    ShowConn("卷线盘网络连接失败");
                }
            }
        }

        public double ParamB
        {
            get { return paramB; }
            set { paramB = value; }
        }
        //设发送上来数据为X,显示数据为Y, 201804281009 
        // Y＝X*1.27(0＜x＜3)
        // Y＝X＋0.8(x＞＝3)
        //Y＝X(X＜＝0)
        public double JxpRealLen
        {
            get
            {
                double carLenDouble = 0;
                double stLenDouble = 0;
                double carIndexDouble = 0;
                double resultDouble = 0;
                if (double.TryParse(carLen, out carLenDouble)
                    && double.TryParse(stLen, out stLenDouble)
                    && double.TryParse(carIndex, out carIndexDouble))
                {

                    if (Jxp.EncoderData <= 0) return ParamB + Jxp.EncoderData;

                    if (Jxp.EncoderData <= stLenDouble)
                    {
                        resultDouble = ParamB + Jxp.EncoderData * carIndexDouble;

                    }
                    else
                    {
                        resultDouble = ParamB + Jxp.EncoderData + carLenDouble;
                    }

                    return resultDouble;
                }
                else
                {
                    return jxp.EncoderData;
                }
            }
        }

        public void ShowDistance()
        {
            if (HKCam == null || HKCam.Status == 0) return;
            //获取卷线盘运算距离 
            string s = JxpRealLen.ToString("0.00");
            if (Jxp != null && !oldCodedata.Equals(s))
            {
                HKCam.StringAddTest(0, "距离:" + s + "M", 350, 600);
                oldCodedata = s;
            }
            else
            {
                if (Jxp == null && oldCodedata == "")
                {
                    HKCam.StringAddTest(0, "距离:0M", 450, 600);
                    oldCodedata = "0";
                }
            }

        }      
        public void ShowEncoderDataAlarm()
        {
            if (Jxp.EncoderData > xcMax)
                ShowTips(xcAlarm);
        }
                
        string shouxianAlarm = "线长小于2M";
        public void StopAutoGetLine()
        {
                       
            if (Jxp.EncoderData < 2 && !ForceToGetLine&& Jxp.IsGetingLine)
            {
                ShowTipsAction(shouxianAlarm);
                JxpStopGetLine();
                StopGetLineBackZd();
            }

        }
        public bool JxpIsInGetLine { get { return Jxp.IsGetingLine; } }
        bool JxpStartGetLine()
        {

            if (Jxp.Status && Jxp.EncoderData < 3)
            {
                DialogResult dr = MessageBox.Show("是否进入强制收线模式？", "确认", MessageBoxButtons.YesNo, MessageBoxIcon.Information);//触发事件进行提示
                if (dr == DialogResult.Yes)
                {
                    ForceToGetLine = true;
                }
                else
                {
                    ForceToGetLine = false;
                    return false;
                }

            }

            if (Jxp != null && Jxp.Status)
            {
                Jxp.PackGetLineCmd();
                IsInGetLineFlag = true;
                
            }

            return true;
        }
        void JxpStopGetLine()
        {
            if (Jxp != null && Jxp.Status)
            {                
                Jxp.PackStopGetLineCmd();
                IsInGetLineFlag = false;
            }
        }
        public void AutoStopGetLine()
        {
            JxpStopGetLine();
            StopGetLineBackZd();
        }

        public void AutoStartGetLine()
        {
            if (JxpStartGetLine())
            {
                StartGetLineBackZd();
            }
        }
        public void ManualGetLine()
        {
            if (IsInGetLineFlag)
            {
                JxpStopGetLine();
            }
            else
            {
                JxpStartGetLine();
            }
        }
        //自动收线 小车跟着一起动
        public void AutoGetLine()
        {
            if (Jxp == null || !Jxp.Status) return;

            if (IsInGetLineFlag)
            {
                AutoStopGetLine();
            }
            else
            {
                AutoStartGetLine();
            }

        }


        public void CleatMeter()
        {
            if (Jxp != null && Jxp.Status)
                Jxp.ClrMeter();
        }

        #endregion
        #region 参数设置


        void setParam(Dictionary<string, string> strLst)
        {

            foreach (string key in strLst.Keys)
            {
                config.WriteConfig(ParamStr, configStr, key, strLst[key]);
            }
            tkxs = strLst[sysInfo.param[0]];
            videoPath = strLst[sysInfo.param[1]];
            imgPath = strLst[sysInfo.param[2]];
            imgType = strLst[sysInfo.param[3]];

            if (double.TryParse(strLst[sysInfo.param[19]], out parambb))
            {

            }

        }
        //清除偏移
        public bool clrParambb()
        {
            ParamB = 0;
            return true;
        }
        public bool setParambb()
        {
            if (parambb > 0)
            {
                ParamB = parambb;
                MessageBox.Show("置位成功！");
                return true;
            }
            else
            {
                MessageBox.Show("置位失败！");
                return false;
            }

        }
        #endregion        
        #region 工程信息


        void InitPrjInfo()
        {
            prj = new projInfo();
            vsForm = new prjCfg();
            vsForm.Visible = false;
            prj.sData = vsForm.ShowData;
            prj.ReadData();
            vsForm.callBackDataUpLoad = SetPrjInfo;
        }

        public void ShowVideCfgForm()
        {
            if (vsForm == null)
            {

            }
            vsForm.ShowDialog();
        }

        public void SetPrjInfo(string[] s)
        {
            prj.setData(s);
        }
        #endregion
        #region 雄迈摄像头
        PictureBox
        xmCamRealPlayWnd;
        xmCam camxm;

        public void InitXMCam()
        {
            string ip = "192.168.0.12";
            string user = "admin";
            string port = "34567";
            string pwd = "";
            string devName = "后视";

            camxm = new xmCam();
            if (!camxm.Init(devName, user, pwd, ip, port))
            {
                MessageBox.Show("后视摄像头打开错误！");
            }
            camxm.OpenCamxm(xmCamRealPlayWnd);

        }
        public bool openXmcam(Control cc)
        {
            if (camxm != null)
                return camxm.OpenCamxm(cc);
            return false;

        }
        #endregion
        #region 北通手柄

        #endregion
        #region  自制串口手柄
        handControl handMan;
        public handControl HandMan
        {
            get { return handMan; }
            set { handMan = value; }
        }
        void InitHand()
        {
            HandMan = new handControl(handDataProcess);
        }
        void sendHeart()
        {
            HandMan.sendHeart();
        }
        public RichTextBox r2;
        public void showInfo(String S )
        {
            ConstantMethod.ShowInfo(r2, S.ToString());
        }
        public Action<int> FarLightChange;
        public Action<int> NearLightChange;
        public Action<int> ZdSpeedChange;
        void handDataProcess(byte[] s)
        {                     
            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdqj, s))
            {
                CarStart(1);
                showInfo("终端前进");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdht, s))
            {
                 CarStart(2);
                showInfo("终端后退");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdzz, s))
            {
                CarStart(3);
                showInfo("终端左转");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdyz, s))
            {
                CarStart(4);
                showInfo("终端右转");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdstop, s))
            {
                 CarStart(0);
                showInfo("终端停止");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambbAdd, s))
            {
                 CamStart(1);
                 showInfo("变倍+");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambbDec, s))
            {
                CamStart(2);

                showInfo("变倍-");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambjAdd, s))
            {
                CamStart(3);
                showInfo("变焦+");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambjDec, s))
            {
                CamStart(4);
               showInfo("变焦-");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.camstop, s))
            {
                CamStart(0);
                showInfo("摄像头动作停止");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytUp, s))
            {
                CamPTStart(1);
                showInfo("云台向上");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytDown, s))
            {
                CamPTStart(2);
                showInfo("云台向下");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytLeft, s))
            {
                CamPTStart(3);
                showInfo("云台向左");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytRight, s))
            {
                CamPTStart(4);
                showInfo("云台向右");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytReset, s))
            {

                CamPTStart(5);
                showInfo("云台复位");
                return;
            }


            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytstop, s))
            {
                CamPTStart(0);
                showInfo("云台停止");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.tgUp, s))
            {

                TaiGanStart(1);
                showInfo("抬杆向上");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.tgDown, s))
            {
                TaiGanStart(2);
                showInfo("抬杆向下");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.tgstop, s))
            {
                TaiGanStart(0);
                showInfo("抬杆停止动作");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cwOpen, s))
            {
                 CwOnOff(true) ;
                 showInfo("除雾开启");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cwClose, s))
            {
                CwOnOff(false);
                showInfo("除雾关闭");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.camSnap, s))
            {
                if (camHK != null)
                {
                    takePic();
                }
                showInfo("截屏");
                return;
            }


            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightYAdd, s))
            {
                ZdParamAdjust(1);
                showInfo("远光灯亮度+1");
                
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightYDec, s))
            {
                ZdParamAdjust(2);
                showInfo("远光灯亮度-1");
                              
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightJAdd, s))
            {
                ZdParamAdjust(3);
                
                showInfo("近光灯亮度+1");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightJDec, s))
            {
                ZdParamAdjust(4);
                showInfo("近光灯亮度-1");
                
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.speedAdd, s))
            {
                ZdParamAdjust(5);
                showInfo("速度+1");              
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.speedDec, s))
            {
                ZdParamAdjust(6);
                showInfo("速度-1");                
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.jxpClear, s))
            {
                clrParambb();
                CleatMeter();
                showInfo("卷线盘米数清零");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.shouxian, s))
            {
                AutoGetLine();
                showInfo("收线开关");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.heart, s))
            {
                sendHeart();
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.connect, s))
            {
                showInfo("连接指令");
                return;
            }
        }
        #endregion
        #region 北通手柄

        Joystick JsControl;

        void InitJsControl()
        {
            JsControl = new Joystick(JoystickAPI.JOYSTICKID1);
            JsControl.CommEvent += JsDataProcess;
            int i = JoystickAPI.joyGetNumDevs();
            if (i > 0)
            {

                JsControl.Capture();
               // JsControl.rtbResult = r2;
            }
        }


        void JsDataProcess(object sender, JoystickEventArgs e)
        {

            int x = (int)e.Buttons;
            switch (x)
            {
                case 1:
                    showInfo("变焦+");
                    CamStart(1);
                    break;
                case 2:
                    showInfo("变焦-");
                    CamStart(2);
                    break;
                case 3:
                    CamStart(3);
                    showInfo("变倍-");
                    break;
                case 4:
                    CamStart(4);
                    showInfo("变倍+");
                    break;
                case 5:
                    CarStart(1);
                    showInfo("车体前进");
                    break;
                case 6:
                    CarStart(2);
                    showInfo("车体后退");
                    break;
                case 7:
                    CarStart(3);
                    showInfo("车体左转");
                    break;
                case 8:
                    CarStart(4);
                    showInfo("车体右转");
                    break;
                case 9:
                    CamPTStart(5);
                    showInfo("云台复位");
                    break;
                case 10:
                    AutoGetLine();
                    showInfo("收线");                   
                    break;
                case 11:
                    CamPTStart(1);
                    showInfo("云台上");
                   
                    break;
                case 12:
                    CamPTStart(2);
                    showInfo("云台下");
                    
                    break;
                case 13:
                    CamPTStart(3);
                    showInfo("云台左");
                    
                    break;
                case 14:
                    CamPTStart(4);
                    showInfo("云台右");
                    break;
                case 15:
                    clrParambb();
                    CleatMeter();
                    showInfo("清零");
                    break;
                case 16:
                    TaiGanStart(1);
                    showInfo("抬杆升");
                    break;
                case 17:
                    CwOpposite();
                    showInfo("除雾开关");
                    break;
                case 18:
                    TaiGanStart(2);
                    showInfo("抬杆降");
                    break;
                case 19:
                    ZdParamAdjust(1);
                    showInfo("远光加");
                    break;
                case 20:
                    ZdParamAdjust(3);
                    showInfo("近光加");
                    break;
                case 21:
                    ZdParamAdjust(2);
                    showInfo("远光减");
                    break;
                case 22:
                    ZdParamAdjust(4);
                    showInfo("近光减");
                    break;
                case 23:
                    CamStart(0);
                    showInfo("摄像头停止调整");
                    break;
                case 24:
                    TaiGanStart(0);
                    showInfo("AY弹起");
                    break;
                case 25:
                    CarStart(0);
                    showInfo("左边遥感回归,车体停止运动");
                    break;
                case 26:
                    CamPTStart(0);
                    showInfo("右边遥感回归，云台停止运动");
                    break;
            }

           // ConstantMethod.ShowInfo(r2,x.ToString());          

        }
        #endregion

    }
}

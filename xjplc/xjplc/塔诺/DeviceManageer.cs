using prjInfoSetting;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{

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
            get { return strExplain; }
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

    public class DeviceManageer
    {

        public delegate void showParamFomr(Dictionary<string,string> s);

        public showParamFomr showpara;
        #region 字符常量

       
        const string configStr = "config";
        
        const string devName1 = "卷线盘";
        const string devName2 = "小车";
        const string devName3 = "后视";
        const string devName4 = "前视";
       

        const string ParamStr = "Param";
        #endregion

        projInfo prj;
        Camera cam;
        sysInfo sysForm;
        prjCfg vsForm;


        public demo.Camera HKCam
        {
            get { return cam; }
            set { cam = value; }
        }
        ConfigFileManager config;

        //图片 视频保存文件夹
        string tkxs;
        string videoPath;
        string imgPath;
        string imgType;
        string carLen;
        string stLen;
        string carIndex;
        #region 卷线盘
        JxpDevice jxp;
        public xjplc.TcpDevice.JxpDevice Jxp
        {
            get { return jxp; }
            set { jxp = value; }
        }
        void InitJxp()
        {


            ServerInfo p0 = new ServerInfo();

            p0.server_Ip= config.ReadConfig(ParamStr, configStr, sysInfo.param[12]);

            p0.server_Port = config.ReadConfig(ParamStr, configStr, sysInfo.param[13]);

            Jxp = new JxpDevice(p0);
            Jxp.DeviceName = devName1;
            Jxp.DeviceId = Constant.devicePropertyA;
            Jxp.OpenTcpClient();

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
            zd.OpenTcpClient();

        }

        int qxjd = 0;
        int fgjd=0;
        public void showQxFg(PictureBox qxP, PictureBox fgP,Label qx,Label fg,Image image2)
        {
            qx.Text = zd.CurrentQxValue.ToString();
            fg.Text = zd.CurrentFgValue.ToString();
            Bitmap image = new Bitmap(image2);
            if (zd.CurrentQxValue != qxjd)
            {

                
                qxP.Image = ConstantMethod.RotateImg(image, (-1)*zd.CurrentQxValue);

               // ConstantMethod.rotatePic(qxP, zd.CurrentQxValue - qxjd);
                qxjd = zd.CurrentQxValue;
            }
            if (zd.CurrentFgValue != fgjd)
            { 
                fgP.Image = ConstantMethod.RotateImg(image, (-1) * zd.CurrentFgValue);

               // ConstantMethod.rotatePic(fgP, zd.CurrentFgValue - fgjd);
                fgjd = zd.CurrentFgValue;
            }
        }
       
        #endregion

        #region 海康摄像头


        bool startRecordId;
        public bool  startRecord()
        {
            string filename = GetCurrentVideoFile();
            if (filename == null)
            {
                MessageBox.Show("视频保存路径错误！");
                return false;
            }

            if (HKCam != null && startRecordId ==false)
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
                return Path.Combine(imgPath, prj.PrjName, dateTime+imgType); //imgPath + prj.PrjName + dateTime + imgType;
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

                return Path.Combine(folder, prj.InsQdJingHao+"-"+prj.InsZdJingHao+"-"+dateTime + ".mp4");


            }


            return null;


        }
        public string takePic()
        {
            string filePath = GetCurrentImageFile();
            if(filePath ==null)
            {
                return "截图路径错误！";
                
            }
            if (filePath != null && HKCam !=null)
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
        }

        public bool LoginInCam()
        {
            if (config != null)
            {
                string ip = config.ReadConfig(ParamStr, configStr, sysInfo.param[4]);
                string port= config.ReadConfig(ParamStr, configStr, sysInfo.param[5]);
                string userName = config.ReadConfig(ParamStr, configStr, sysInfo.param[6]);
                string pwd = config.ReadConfig(ParamStr, configStr, sysInfo.param[7]);

                int portInt = -1;
                if (!string.IsNullOrWhiteSpace(ip)
                    && !string.IsNullOrWhiteSpace(port)
                    && !string.IsNullOrWhiteSpace(userName)
                    && !string.IsNullOrWhiteSpace(pwd)
                    && int.TryParse(port, out portInt)
                    && HKCam !=null
                    )
                {
                  return HKCam.Login(ip,portInt,userName,pwd);
                }

            }
            return false;
        }
        string PrjName = "";
        string InsQdJingHao= "";
        string InsGuanDuanDiameter = "";
        public void ShowHKInfo()
        {


            if (PrjName != prj.PrjName)
            {
                HKCam.StringAddTest(1, prj.PrjName, 10, 15);
                PrjName = prj.PrjName;
            }
            
            if (InsQdJingHao != prj.InsQdJingHao)
            {
                HKCam.StringAddTest(2, prj.InsQdJingHao+"---"+ prj.InsZdJingHao, 10, 50);
                InsQdJingHao = prj.InsQdJingHao;
            }
            
            if (InsGuanDuanDiameter != prj.InsGuanDuanDiameter)
            {
                HKCam.StringAddTest(3, "DN"+prj.InsGuanDuanDiameter+"mm", 10,95);
                InsGuanDuanDiameter = prj.InsGuanDuanDiameter;
            }
            

        }

        #region 卷线盘显示
        string oldCodedata = "";


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
                if (   double.TryParse(carLen, out carLenDouble)
                    && double.TryParse(stLen, out stLenDouble)
                    && double.TryParse(carIndex, out carIndexDouble))
                {

                    if (Jxp.EncoderData <= 0) return Jxp.EncoderData; 

                    if (Jxp.EncoderData <= stLenDouble)
                    {
                        resultDouble = Jxp.EncoderData * carIndexDouble;

                    }
                    else
                    {
                        resultDouble = Jxp.EncoderData + carLenDouble;
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
            //获取卷线盘运算距离 
            string s = JxpRealLen.ToString("0.00");
            if (Jxp!=null && !oldCodedata .Equals(s))
            {
                HKCam.StringAddTest(0, "距离:" + s + "M", 350, 600);
                oldCodedata = s;
            }
            else
            {
                if (Jxp == null && oldCodedata=="")
                {
                    HKCam.StringAddTest(0, "距离:0M", 450, 600);
                    oldCodedata = "0";
                }
            }

        }

        #endregion
        public bool LoginOutCam()
        {
            return HKCam.CheckOut();
        }

        public bool ListHKCamView(Control c1)
        {
           (c1 as PictureBox).SizeMode = PictureBoxSizeMode.Normal;
            return HKCam.ListView(c1);     
        }

        #endregion
        void Init()
        {
            
            config = new ConfigFileManager(Constant.ConfigParamFilePath);
           
            IPAddress[] ips = Dns.GetHostAddresses(""); //当参数为""时返回本机所有IP
            bool IsExist = false;
            foreach (IPAddress ip in ips)
            {
                if (Regex.IsMatch(ip.ToString(), "(192.168.0.)+"))               
                    {
                      IsExist = true;
                    }
                
            }

            if (!IsExist)
            {
                MessageBox.Show("网络地址错误,请检查网络连接！");
                ConstantMethod.AppExit();
            }
                      
            InitHKCamera();
            InitZd();
           //InitXMCam();
           InitPrjInfo();
           InitSysInfo();
           InitJxp();

        }
        #region 参数设置
        void InitSysInfo()
        {
            sysForm = new sysInfo();
            sysForm.Hide();
            sysForm.setParam = setParam;
            showpara = sysForm.ShowSysParam;
            showParam();
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

        void setParam(Dictionary<string,string> strLst)
        {

            foreach (string key in strLst.Keys)
            {
                config.WriteConfig(ParamStr, configStr, key, strLst[key]);
            }
            tkxs = strLst[sysInfo.param[0]];
            videoPath = strLst[sysInfo.param[1]];
            imgPath = strLst[sysInfo.param[2]];
            imgType = strLst[sysInfo.param[3]];
        }
        #endregion
        public void ShowParamCfgForm()
        {
            sysForm.Visible = true;
        }

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
        }
        public bool openXmcam(Control cc)
        {
            if(camxm !=null)
            return camxm.OpenCamxm(cc);
            return false;

        }
        #endregion
        public void Dispose()
        {
            if(HKCam !=null)
            HKCam.dispose();
            if(camxm != null)
            camxm.dispose();
        }
        public DeviceManageer()
        {
            Init();
        }
    }
}

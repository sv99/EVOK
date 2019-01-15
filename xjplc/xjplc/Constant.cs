using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace xjplc
{

    public struct ServerInfo
    {
        //服务器地址
        public string server_Ip;


        //服务器端口
        public string server_Port;
       
    }
    /// <summary>
    /// 串口参数类
    /// </summary>
    public struct PortParam
    {
        //串口名
        public string m_portName;
        //波特率
        public int m_baudRate;
        //奇偶校验
        public Parity m_parity;
        //停止位
        public StopBits m_stopbits;
        //流控制 传输协议
        public Handshake m_handshake;
        //数据位长度
        public int m_dataBits;
        //读取缓冲区大小
        public int m_readBufferSize;
        //读取缓冲区中断标准
        public int m_receivedBytesThreshold;
        //连续接收超时时间
        public int m_receiveTimeout;
        //发送时间间隔
        public int m_sendInterval;
    }
    //网络处理数据参数
    public class SocEventArgs : EventArgs
    {
        byte[] byte_buffer;
        public byte[] Byte_buffer
        {
            get { return byte_buffer; }
            set { byte_buffer = value; }
        }


        public SocEventArgs()
        {

        }
    }

    public class TcpEventArgs : EventArgs
    {
        byte[] byte_buffer;
        public byte[] Byte_buffer
        {
            get { return byte_buffer; }
            set { byte_buffer = value; }
        }

        public TcpEventArgs()
        {

        }
    }
    //委托事件传递类 这里用作传递数据接收后处理的参数
    public class CommEventArgs : EventArgs
    {
        byte[] byte_buffer;
        public byte[] Byte_buffer
        {
            get { return byte_buffer; }
            set { byte_buffer = value; }
        }
        
        public CommEventArgs()
        {

        }
    }

    public class ItemChangedArgs : EventArgs
    {
        List<string> itemAll;
        public System.Collections.Generic.List<string> ItemAll
        {
            get { return itemAll; }
            set { itemAll = value; }
        }

        List<string> itemSelect;
        public System.Collections.Generic.List<string> ItemSelect
        {
            get { return itemSelect; }
            set { itemSelect = value; }
        }
        public ItemChangedArgs()
        {

        }
    }

    //串口数据处理
    public delegate void commDataProcess(object s, CommEventArgs e);//声明自定义的事件委托，用来执行事件的声明，和处理方法的传递
    //网络数据处理
    public delegate void socDataProcess(object s, SocEventArgs e);//声明自定义的事件委托，用来执行事件的声明，和处理方法的传递
    //网络客户端进来
    public delegate void socketClientChanged(object s, Socket client);//声明自定义的事件委托，用来执行事件的声明，和处理方法的传递

    //远邦台技项目
    public delegate void userShowItemChanged(object s, ItemChangedArgs e);//声明自定义的事件委托，用来执行事件的声明，和处理方法的传递//
                                                                      //网络客户端进来
    public delegate void ydtdWorkChanged(object s, YBDTWork ybtdWork0);//声明自定义的事件委托，用来执行事件的声明，和处理方法的传递
    public class Constant
    {

        public static readonly string CsvSplitComma = ",";
        public static readonly string CsvSplitSemiColon = ";";
        public static readonly char CsvSplitCommaChar = ',';
        public static readonly string CsvSplitDoubleQuaotaion = "\"";
        public static readonly char CsvSplitSemiColonChar = ';';
        public static readonly string ConnectMachineSuccess = "设备连接成功！";
        public static readonly string ConnectMachineFail = "设备连接失败,请检查设备端口！";
        public static readonly string ReadPlcInfoFail = "读取设备数据文件失败，软件即将关闭！";
        public static readonly string CSVFileEX = ".csv";
        public static readonly string ExcelFileEX0 = ".xlsx";
        public static readonly string ExcelFileEX1 = ".xls";
        public static readonly string SaveFileDemo = ConstantMethod.GetAppPath() + "filedemo.csv";
        public static readonly string SaveFileDemoConfigPath = ConstantMethod.GetAppPath() + "config\\filedemo.csv";
        public static readonly string SingleMode = "单字";
        public static readonly string DoubleMode = "双字";
        public static readonly string BitMode = "位";
        public static readonly string MachineWorking = "设备运行中！";
        public static readonly string SetDataFail = "数据设置失败！";
        public static readonly string FileIsInUse = "文件使用中！";
        public static readonly int ExcelFile = 1;

        public const int evokGetDefaultMode = 0;
        public const int evokGetAutoMode = 1;
        public const int optNormal = 0;
        public const int optNormalWithParamCount = 1;
        public const int optNormalExcel = 2;
        public const int optNo = 3;
        //20190109 梳齿模式增加
        public const int optShuChi = 4;
        public const int optNormalMax = 1500;
        public static readonly int CsvFile = 2;
        public static readonly int ErrorFile = 0;
        public const int AutoPage = 0;
        public const int HandPage = 1;
        public const int ParamPage = 2;
        public const int IOPage = 3;
        public const int Param1Page = 4;
        public static readonly int AutoPageID = 2;
        public static readonly int HandPageID = 3;
        public static readonly string Alarm = "报警";
        public static readonly int DataRowWatchMax = 50; //监控太多不行 还是少监控一点吧
        public static readonly string[] plcDataFile = { "addr", "mode", "bin", "count", "value", "param1", "param2", "param3", "param4", "param5", "param6" };
        public static readonly string sqlChar10 = "char(10)";
        public static readonly string sqlChar20 = "char(20)";

        public const int GetScarSuccess = 0;
        public const int GetScarWrongScar = 1;
        public static readonly string GetScarWrongScarStr = "结疤数据错误！";
        public const int GetScarScarNoEven = 2;
        public static readonly string GetScarScarNoEvenStr = "结疤数据不是偶数！";
        public const int GetScarScarNoAscend = 4;
        public static readonly string GetScarScarNoAscendStr = "结疤数据排序错误！";
        public const int GetScarScarValueError = 5;
        public static readonly string GetScarScarValueErrorStr = "结疤数据错误值,请检查结疤数据和料长数据！";
        public static readonly int MaxScarCount = 40;
        public static readonly int ScarStartAddress = 6000;
        public static readonly int ScarPage = 4;
        public static readonly string ScarName = "结疤";
        public static readonly string GetScarError = "获取结疤数据错误！";

        public const int LTBCdefault = 700;
        public const int LTBCMax = 23000;
        public const int LTBCAddDbc = 1;
        public const int SizeScarSplit = 0;
        public const int SizeScarNoSplit = 1;
        public const int ScarSplit = 2;
        public const int doorId = 10;
        //线圈值常量
        public static readonly int M_ON = 1;
        public static readonly int M_OFF = 0;
        //PLC 要读取的数据的文件列名 CSV
        public static readonly string[] PLCstrCol = { "addr", "mode", "bin", "count", "value", "param1", "param2", "param3", "param4", "param5", "param6" };
        public static readonly string PlcDataFilePathAuto = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_auto.csv";
        public static readonly string PlcDataFilePathHand = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_hand.csv";
        public static readonly string PlcDataFilePathParam = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_param.csv";
        public static readonly string PlcDataFilePathIO = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_IO.csv";

        public static readonly string PlcDataFilePathAuto1 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_auto1.csv";
        public static readonly string PlcDataFilePathHand1 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_hand1.csv";
        public static readonly string PlcDataFilePathParam1 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_param1.csv";
        public static readonly string PlcDataFilePathIO1 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_IO1.csv";

        public static readonly string userdata = ConstantMethod.GetAppPath() + "user data.csv";

        public static readonly string PlcDataFilePathAuto2 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_auto2.csv";
        public static readonly string PlcDataFilePathHand2 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_hand2.csv";
        public static readonly string PlcDataFilePathParam2 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_param2.csv";
        public static readonly string PlcDataFilePathIO2 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_IO2.csv";
        public static readonly string PlcDataFilezhj = ConstantMethod.GetAppPath() + "Plc Data\\纵横锯数据样本.csv";
        public static readonly string PlcDataFilehqj = ConstantMethod.GetAppPath() + "Plc Data\\横切锯样本.csv";
        public static readonly string PlcDataFilePath3 = ConstantMethod.GetAppPath() + "Plc Data\\plc_data3.csv";
        public static readonly string PlcDataFilePathScar = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_scar.csv";

        public static readonly string ErrorMValue = "位值只能为1或者0！";

        //public static readonly string ConstantMethod.GetAppPath() = ConstantMethod.GetAppPath();// System.AppDomain.CurrentDomain.BaseDirectory; //Application.StartupPath + "\\";// Path.GetDirectoryName(Application.ExecutablePath)+"\\";//System.AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string ConfigSerialportFilePath = ConstantMethod.GetAppPath() + "config\\configSerialport.xml";
        public static readonly string ConfigSerialportFilePath_bak = ConstantMethod.GetAppPath() + "config\\configSerialport_bak.xml";
        public static readonly string ConfigSerialportFilePath1 = ConstantMethod.GetAppPath() + "config\\configSerialport1.xml";
        public static readonly string ConfigDevice1 = ConstantMethod.GetAppPath() + "Plc Data\\device1.csv";
        public static readonly string ConfigDevice2 = ConstantMethod.GetAppPath() + "Plc Data\\device2.csv";
        public static readonly string ConfigDevice3 = ConstantMethod.GetAppPath() + "Plc Data\\device3.csv";
        public static readonly string ConfigDevice4 = ConstantMethod.GetAppPath() + "Plc Data\\device4.csv";
        public static readonly string ConfigDevice5 = ConstantMethod.GetAppPath() + "Plc Data\\device5.csv";
        public static readonly string ConfigExcelOpt = ConstantMethod.GetAppPath() + "source\\1.xlsm";
        public static readonly string ConfigSerialportFilePath2 = ConstantMethod.GetAppPath() + "config\\configSerialport2.xml";
        public static readonly string ConfigParamFilePath = ConstantMethod.GetAppPath() + "config\\configParam.xml";
        public static readonly string ConfigParamFilePath_Bak = ConstantMethod.GetAppPath() + "config\\configParam_bak.xml";
        public static readonly string ConfigPassWdFilePath = ConstantMethod.GetAppPath() + "config\\configPwd.xml";
        public static readonly string ConfigPassWdFilePath_Bak = ConstantMethod.GetAppPath() + "config\\configPwd__Bak.xml";
        public static readonly string ErrorSerialportConfigFile = "端口配置文件不存在，请检查config文件夹，软件即将关闭！";
        public static readonly string ErrorPwdConfigFile = "密钥文件不存在，请检查config文件夹，软件即将关闭！";
        public static readonly string ErrorParamConfigFile = "参数配置文件不存在，请检查config文件夹，软件即将关闭！";
        public static readonly string ErrorPlcFile = "设备数据文件不存在，请检查plc data文件夹，软件即将关闭！";
        public static readonly string strValue = "value";
        public static readonly string strParam = "param";
        public static readonly string strParam1 = "param1";
        public static readonly string strParam2 = "param2";
        public static readonly string strParam3 = "param3";
        public static readonly string strParam4 = "param4";
        public static readonly string strParam5 = "param5";
        public static readonly string strParam6 = "param6";
  

        public static readonly string passwdTime = "passwdTime";
        public static readonly string passwd = "passwd";
        public static readonly string NoSerialPort = "端口错误或者不存在！";
        public static readonly string passwdCount = "passwdCount";
        public static readonly string PortName = "PortName";
        public static readonly string Bin = "bin";
        public static readonly string Read = "读";
        public static readonly string Write = "写";
        public static readonly string printBarcodeMode = "printBarcodeMode";
        public static readonly string optParam1 = "optParam1";
        public static readonly string IsSaveProdLog = "IsSaveProdLog";
        public static readonly int NoPrintBarCode = 0;
        public static readonly int AutoBarCode = 1;
        public static readonly int HandBarCode = 2;
        public const int doorSizeId = 0;
        public const int doorBanId = 1;  
        public const int doorShellId = 2;           //0       1          2        3        4         5        6           7
        public static string[] constantStatusStr = {"未知","报警中", "运行中", "复位中", "停止中", "暂停中","启动就绪","待机中" };
        public static int[]    constantStatusId  = {0,1,2,3,4,5,6,7};
        public static Color[] colorRgb = {
                                        Color.LightGray,
                                        Color.Red,
                                        Color.Green,
                                        Color.Blue,
                                        Color.Brown,
                                        Color.Pink,
                                        Color.Yellow,
                                        Color.Yellow,
                                        };
        public static readonly string[] printBarcodeModeStr = {"无条码","自动贴条码","手动贴条码" };

        public static readonly string[] strformatEh = { "size", "CountToCut", "cntdone", "barcode", "param1", "param2", "param3", "param4", "param5", "param6", "param7", "param8", "param9", "param10", "param11", "param12", "param13" };
        public static readonly string[] strformatZh = { "尺寸", "设定数量", "已切数量", "条码", "参数1" ,
            "参数2" , "参数3" , "参数4","参数5" , "参数6" , "参数7", "参数8" , "参数9" , "参数10" ,"参数11", "参数12" , "参数13" , "参数14" };
        public static readonly string pwdWrong = "密码错误！";
        public static readonly string pwdOk = "密码正确！";
        public static readonly int pwdCountMax = 6;
        public static readonly int PwdOffSet = 1000;
        public static readonly string prodResult = "生产结果";
        public static readonly int dataMultiple = 100;
        public static readonly int MaxShowCount = 2000;
        public static readonly string ScarId = "-1";
        public static readonly string barCodeDemo = ConstantMethod.GetAppPath() + "零部件打印.frx";
        public static readonly string SplitTypeFile = ConstantMethod.GetAppPath() + "config\\SplitType.xls";
        #region  优化数据
        //优化错误返回值
        public static readonly string prodLstNoData = "数据收集错误！";
        public static readonly string  optFail = "优化结果错误！";
        public static readonly string optResultNoData = "优化结果无数据！";
        public static readonly string optSuccess = "优化结束！";
        public static readonly string NextOpt = "下一根料准备！";
        public static readonly string ErrorMeasure = "测量错误！";
        public static readonly string InOPT = "优化中,请稍等。。。。。。。。";
        public static readonly int MeaSureMaxTime = 120000;

        public static readonly int StartWaitMaxTime = 100000;
        #endregion
        #region 开阳
        public static readonly string devSTHY = "双头合页机";
        public static readonly string devSCLS = "锁槽拉手机";
        public static readonly string devSC = "锁槽机";
        public static readonly string devSBJ = "四边锯";
        public static readonly string[] sthyDataCol = { "门长", "门厚", "门宽", "角度阀", "配方" };
        public static readonly string[] sclsDataCol = { "门长", "门厚", "门宽", "配方1" ,"配方2" };
        public static readonly string[] scDataCol = { "门长", "门厚", "门宽", "配方" };
        #endregion
        #region 运行数据
        public const  int devicePropertyA = 0;//设备类别 分为两种 一种是一维 
        public const  int devicePropertyB = 1;//门皮 一种是二维

        public const int devicePropertyC = 2;//门板 一种是二维
        public static readonly string emgStopTip = "急停或紧急退出中，请复位！";
        public static readonly string alreadyStart = "设备生产中!";
        public static readonly string noData = "无数据！";
        public static readonly string measureOutOfTime = "测量超时！";
        public static readonly string IsWorking = "设备生产中！";
        public static readonly string CutEnd = "生产结束！";
        public static readonly string Start = "软件启动！";
        public static readonly string Quit = "软件退出！";
        public static readonly string MachineAlarm = "设备报警！";
        public static readonly string CommManageError = "接收数据超时，重新连接！";
        public static readonly string DeviceConnectionError = "通讯超时，重置连接！";
        public static readonly string DeviceStartCut = "设备启动！";
        public static readonly string DevicePause = "设备暂停！";
        public static readonly string DeviceStop = "设备停止！";
        public static readonly string DeviceEmgStop = "设备急停！";
        public static readonly string DeviceStartFailed = "设备启动失败，请检查设备情况！";
        public static readonly string DeviceReset = "设备复位！";
        public static readonly string DeviceNoPrinter = "无打印机！";
        public static readonly string DeviceNoLogFile = "无日志文件！";
        public static readonly string PlcReadDataError= "设备读取数据超时！";
        public static readonly string LoadFileSt = "加载文件开始！";
        public static readonly string LoadFileEd = "加载文件结束！";
        public static readonly string MeasureSt = "开始测量！";
        public static readonly string MeasureEd = "结束测量！";
        public static readonly string DataDownLoad = "数据下发！";
        public static readonly string DataDownLoadSuccess = "数据下发成功！";
        public static readonly string DataDownLoadFail = "数据下发失败！";
        public static readonly string AutoMeasureMode = "自动测长模式！";
        public static readonly string NormalMode = "正常优化模式！";
        public static readonly string ShuChiMode = "梳齿无限料长模式！";
        public static readonly string DoorShell = "门皮模式！";
        public const  int CutMeasureRotateWithHoleMode = 2;
        public const   int CutNormalMode = 0;
        public const  int CutMeasureMode = 1;
        public const int  CutNormalWithHoleMode = 3;
        public const int CutMeasureWithScarSplitNoSize = 4;
        public const int CutNormalDoorShellMode = 5;
        public const int CutNormalWithShuChiMode = 6;
        public const int CutNormalDoorBanMode = 7;
        public const string CutMeasureTips0 = "请选择模式或者导入数据";
        #endregion
        #region 梳齿机
           public static readonly int scjDeivceId = 3;
           public static readonly string scjDeivceName = "梳齿机";
           public static readonly string PlcDataFilePathHandSC = ConstantMethod.GetAppPath() + "Plc Data\\plc_data_hand-";
           public static string[] constantHandName = 
            {
              "机械手", "铣槽","推料", "前送料", "同步接料", "送料切料", "打码拉料", "分料"
            };

        #endregion
        #region PLC
        public static readonly int schneiderPlc= 0;
           public static readonly int xinjiePlc = 1;
           public static readonly int deltaPlc = 2;
           public static readonly int mitsubishiPlc = 3;
           public static readonly int[] PlcDAddressOffset = { 040000, 0, 0, 0 };
        public static readonly int[] PlcMAddressOffset = { 040000, 0, 0, 0x2000 };

        #endregion
        #region 台达PLC专用
        public static readonly string ConfigSource = ConstantMethod.GetAppPath() + "source\\";

        public static readonly int sbjDeivceId = 2;
        #region 门锁机
        public static readonly int msjDeivceId = 1;

        #region 锁槽

        public static readonly string[] scCutType = {
                                                               "垂直方槽",
                                                                "垂直圆孔",
                                                                "水平方槽",
                                                                "水平圆孔",
                                                                 "椭圆槽"
                                                                // "合页槽",
                                                                // "上开口合页槽",
                                                                //"下开口合页槽" 
                                                        };

        public static readonly int[] scCutTypeShowId = {        16,
                                                                17,
                                                                18,
                                                                19,
                                                                20,
                                                                //23,
                                                                //24
                                                          };


        public static readonly string[] scCutTypeImage = {     "w1",
                                                                "w2",
                                                                "w3",
                                                                "w4",
                                                                "w8",
                                                                //"w5",
                                                                //"w6",
                                                                //"w7"

        };
        #endregion

        #region 合页

        public static readonly string[] hyCutType =           {                                                              
                                                                "合页槽",
                                                                "上开口合页槽",
                                                                "下开口合页槽" };

        public static readonly int[] hyCutTypeShowId = {      
                                                                22,
                                                                23,
                                                                24
                                                          };

        public static readonly string[] hyCutTypeImage = {     
                                                                "w5",
                                                                "w6",
                                                                "w7"
                                                          };

        #endregion 合页



        public static readonly string[] axisStr = {          "X1",
                                                                "Y1",
                                                                "Z1",
                                                                "X2",
                                                                "Y2",
                                                                "Z2",
                                                                "A" };

        public static readonly string[] schyStrLst = {" ",
                                                                "垂直方槽",
                                                                "垂直圆孔",
                                                                "水平方槽",
                                                                "水平圆孔",
                                                                "椭圆槽",
                                                                "右底边插销",
                                                                "合页槽" ,
                                                                "上开口合页槽",
                                                                "下开口合页槽",
                                                                "左底边圆孔",
                                                                "右底边圆孔"



        };
        
        public static readonly string errClrValue = "8";
        #endregion
        #region MODBUSTCP

        public static readonly string ConfigServerPortFilePath = ConstantMethod.GetAppPath() + "config\\configServerport.xml";
        public static readonly string ConfigServerPortFilePath1 = ConstantMethod.GetAppPath() + "config\\configServerport1.xml";
        public static readonly string ConfigServerPortFilePath2 = ConstantMethod.GetAppPath() + "config\\configServerport2.xml";
        public static readonly string ConfigServerPortFilePath3 = ConstantMethod.GetAppPath() + "config\\configServerport3.xml";
        public static readonly string ConfigServerPortFilePath4 = ConstantMethod.GetAppPath() + "config\\configServerport4.xml";
        public static readonly string ConfigServerPortFilePath5 = ConstantMethod.GetAppPath() + "config\\configServerport5.xml";

        public static readonly string ServerIp = "ServerIp"; 
        public static readonly string ServerIpPort = "Port";
        public const  int floatShow = 0;
        public const int intShow = 1;
        public static readonly string dataOutOfRange = "数据超范围！";
        public static readonly string scLight = "锁槽工位指示";
        public static readonly string scData = "锁槽工位数据";
        public static readonly string hyLight = "合页工位指示";
        public static readonly string hyData = "合页工位数据";
        public static readonly string dataConvertError = "数据转换错误！";
        public static readonly string[] tcpType = { "BOOL", "SINT", "USINT",
                                                        "INT", "UINT",
                                                        "DINT",  "UDINT",
                                                        "LINT" , "ULINT",
                                                        "REAL",
                                                        "LREAL",
                                                        "Time" , "Date", "TOD", "DT" };
        public static readonly int[] tcpTypeByteCount = { 1, 1, 1,
                                                    2, 2,
                                                    4,  4,
                                                    8 , 8,
                                                    4,
                                                    8,
                                                    8 , 8, 8, 8 };
        public static readonly string Bool = "BOOL";
        public static readonly string SINT = "SINT";
        public static readonly string USINT = "USINT";
        public static readonly string INT = "INT";
        public static readonly string UINT = "UINT";
        public static readonly string DINT = "DINT";
        public static readonly string UDINT = "UDINT";
        public static readonly string LINT =  "LINT";
        public static readonly string ULINT = "ULINT";
        public static readonly string REAL = "REAL";
        public static readonly string LREAL = "LREAL";
        public static readonly string Time = "Time";
        public static readonly string Date = "Date";
        public static readonly string TOD = "TOD";
        public static readonly string DT = "DT";

        public static readonly int BoolMemory = 1;
        public static readonly int SINTMemory = 1;
        public static readonly int USINTMemory = 1;
        public static readonly int INTMemory = 2;
        public static readonly int UINTMemory = 2;
        public static readonly int DINTMemory = 4;
        public static readonly int UDINTMemory = 4;
        public static readonly int LINTMemory = 8;
        public static readonly int ULINTMemory = 8;
        public static readonly int REALMemory = 4;
        public static readonly int LREALMemory = 8;
        public static readonly int TimeMemory = 8;
        public static readonly int DateMemory = 8;
        public static readonly int TODMemory = 8;
        public static readonly int DTMemory = 8;

        public static readonly string[] dataType = { "IX", "QX", "MX",
                                                      "IW", "QW", "MW",
                                                      "MB", "IB" , "QB",
                                              "MD", "ML" , "ID", "IL", "QD","QL" };

        //相对地址偏移
        public static readonly int[] dataTypeAddrOffset = { 1, 1, 1,
                                                      2, 2, 2,
                                                      1, 1 , 1,
                                              4, 8 , 4, 8,4,8 };

        public static readonly int[] dataTypeAddr = { 0X01C01800, 0X01C01C00, 0X01C02000,
                                                      0X00380300, 0X00380380, 0X00380400,
                                                      0X00380400, 0X00380300 , 0X00380380,
                0X00380400,0X00380400,0X00380300,0X00380300,0X00380380,0X00380380


        };

        public static readonly int[] dataTypeAreaInt = {0,1,2,5,3,4 ,6,7,14,12,13,8,9,10,11};

        public static readonly int IXAddr = 0X01C01800;
        public static readonly int QXAddr = 0X01C01C00;
        public static readonly int MXAddr = 0X01C02000;


        public static readonly int MBAddr = 0X00380400;
        public static readonly int MWAddr = 0X00380400;
        public static readonly int MDAddr = 0X00380400;
        public static readonly int MLAddr = 0X00380400;

        public static readonly int QBAddr = 0X00380380;
        public static readonly int QWAddr = 0X00380380;
        public static readonly int QDAddr = 0X00380380;
        public static readonly int QLAddr = 0X00380380;

        public static readonly int IBAddr = 0X00380300;
        public static readonly int IWAddr = 0X00380300;
        public static readonly int IDAddr = 0X00380300;
        public static readonly int ILAddr = 0X00380300;
        


        public static readonly byte[] DTTcpHeader = { 0xBB, 0x00, 0x00, 0x00 };
         
        public static readonly byte[] DTTcpFunctionReadBitCmd = { 0x43, 0x01};
        public static readonly byte[] DTTcpFunctionReadByteCmd = { 0x43, 0x02 };

        public static readonly byte[] DTTcpFunctionWriteBitCmd = { 0x43, 0x03 };
        public static readonly byte[] DTTcpFunctionWriteByteCmd = { 0x43, 0x04 };
        public static readonly int IXMODBUSAddr = 0X6000;
        public static readonly int QXMODBUSAddr = 0XA000;
        public static readonly int IWMODBUSAddr = 0X8000;
        public static readonly int QWMODBUSAddr = 0XA000;
        public static readonly int MWMODBUSAddr = 0X0000;

        public const   int IXAddrId = 0;
        public const   int QXAddrId = 1;
        public const   int MXAddrId = 2;
     
        public const   int QWAddrId = 3;
        public const   int MWAddrId = 4;
        public const   int IWAddrId = 5;
        public const   int MBAddrId = 6;
        public const   int IBAddrId = 7;
        public const int   IDAddrId = 8;
        public const int   ILAddrId = 9;

        public const int  QDAddrId = 10;
        public const int  QLAddrId = 11;
        public const int  MDAddrId = 12;
        public const int  MLAddrId = 13;
        public const int  QBAddrId = 14;

        public static byte[] IsDtTcpExitOut = { 0x06, 0x66, 0x00, 0x00, 0x00, 0x07, 0x01, 0x43, 0x01, 0x00, 0x00, 0x5D, 0x84 };

         public static byte[] IsDtTcpExitIn = { 0x06, 0x66, 0x00, 0x00, 0x00, 0x05, 0x01, 0x43, 0x01, 0x00, 0x00 };
                        
        public static readonly byte[] DTModbusTcpHeader = { 0x00, 0x00, 0x00, 0x00 };
        public static readonly byte[] DTModbusTcpHeaderFake = { 0x00, 0x00, 0x00, 0x00,0x00 };

        public static readonly byte[] DTModbusTcpWriteSingleDataDeviceId = { 0x01 };

        public static readonly string DeviceIp = "192.168.0.1";

        public static readonly int DevicePort = 502;
        #endregion
        //台达PLC 发送命令 判断是否存在 存在则回复以下

        public static readonly int DTTCPMAXAddr = 500000;
        public static readonly int DTTCPMAXDAddr = 7000;
        public static readonly int DTTCPMAXHDAddr = 7000;
        public static readonly int DTTCPMAXMAddr = 7000;
        public static readonly int DTTCPMAXHMAddr = 7000;
        public static readonly int DTTCPMAXXAddr = 7000;
        public static readonly int DTTCPMAXYAddr = 7000;
        // 3a 30 31 30 35 30 43 30 30 46 46 30 30 45 46 0d 0a
        public static readonly byte[] DTExistByteOutIn = 
            { 0x01,0x05,0x0C,0x00,0xFF,0x00,0xEF};
        public static readonly byte[] DTExistByteOutIn0 =
           { 0x3a,0x30 ,0x31 ,0x30 ,0x35 ,0x30 ,0x43 ,0x30 ,0x30 ,0x46 ,0x46 ,0x30 ,0x30 ,0x45 ,0x46 ,0x0d ,0x0a};
        public static readonly byte DTHeader = 0x3a;
        public static readonly byte[] DTReadDataCmdCheck = { 0x01, 0x03 };
        public static readonly byte[] DTEnd = { 0x0d, 0x0a };
        public static readonly byte ReadBitCmdFunctionCode = 0X02 ;
        public static readonly byte ReadDCmdFunctionCode =  0X03 ;
        public static readonly byte WriteSingleDCmdFunctionCode = 0X06;

        public static readonly byte WriteMultipleDCmdFunctionCode = 0X10;
        public static readonly byte[] DTDeviceId = { 0X01 };
        public static readonly int TaiDaConnectMode485 = 1;
        public static readonly int TaiDaConnectMode232 = 0;
        public static readonly byte[] DTCmdSetReadDDataOut232 = { 0x40, 0xdc };
        public static readonly byte[] DTCmdSetReadDDataOut485 = { 0x43, 0x98 };
        
        public static readonly byte[] DTCmdSetReadMDataOut232 = { 0x40, 0x00 };
        public static readonly byte[] DTCmdSetReadMDataOut485 = { 0x42, 0xbc };

        public static readonly byte[] DTCmdReadDDataOut232 = { 0x41, 0x90 };
        public static readonly byte[] DTCmdReadDDataOut485 = { 0x44, 0x4c };

        public static readonly byte[] DTCmdReadMDataOut232 = { 0x40, 0xC8 };
        public static readonly byte[] DTCmdReadMDataOut485 = { 0x43, 0x84 };

        public static readonly byte[] DTExistByteOutIn485 =
        { 0x3a, 0x30 , 0x31 , 0x30  , 0x35 , 0x30 , 0x43 , 0x33  , 0x38 , 0x46 , 0x46 , 0x30  , 0x30 , 0x42 , 0x37 , 0x0d , 0x0a};
        

        #endregion
        #region 信捷PLC 专用
        //固定头
        public static readonly int XJHeader = 0x01;
        //信捷PLC 发送命令 判断是否存在 存在则回复以下
        // 01 03 f5 c0 00 01 b7 fa
        public static readonly byte[] XJExistByteOut = { 0x01, 0x03, 0xf5, 0xc0, 0x00, 0x01, 0xb7, 0xfa };
        //信捷PLC 设备存在则会返回
        //01 03 02 00  02 39 85
        public static readonly byte[] XJExistByteIn = { 0x01, 0x03, 0x02, 0x00, 0x02, 0x39, 0x85 };
        //device类和设备重新握手的次数
        public static readonly int DeviceErrorConnCountMax = 200; //最大连接次数
        //通讯过程中 commmanager 重复通讯的最大次数
        public static readonly int ErrorConnCountMax =5;
        //信捷专用码 最大区域
        public static readonly int XJMaxAddr = 50000;
        public static readonly int XJMaxDAddr = 7000;
        public static readonly int XJMaxHDAddr = 7000;
        public static readonly int XJMaxMAddr = 7000;
        public static readonly int XJMaxHMAddr = 7000;
        public static readonly int XJMaxXAddr = 7000;
        public static readonly int XJMaxYAddr = 7000;
     
        public static readonly int XJINTSingleMode = 1;

        public static readonly int XJINTDoubleMode = 2;

        public static readonly int XJConnectMaxCount = 5;
        public static readonly int XJConnectTimeOut = 300;//300;
        public static readonly int XJRestartTimeOut = 2000;//2000;
        //读取超时 COMM超时 soc连接比较慢 所以要4秒 进行缓冲
        public static readonly int ReadCommTimeOut = 4000; 

        //写入超时 
        public static readonly int WriteCommTimeOut = 5000;

        //PLC 数据反馈 在切割的时候 数据发送 超时
        public static readonly int   PlcCountTimeOut = 90000;
        public static readonly byte[] XJReadDataCmdCheck = { 0x01, 0x19 };


        //信捷地址偏移常量
        public static readonly int M_addr = 0;
        public static readonly int D_addr = 0;
        public static readonly int X_addr = 0x5000;
        public static readonly int Y_addr = 0x6000;
        public static readonly int HD_addr = 0xA080;
        public static readonly int HSD_addr = 0xB880;
        public static readonly int HM_addr = 0xC100;

        //地址偏移常量
        public static readonly int Delta_D_addr = 0x1000;
        public static readonly int Delta_X_addr = 0x0400;
        public static readonly int Delta_Y_addr = 0x0500;
        public static readonly int Delta_M_addr = 0x0800;
        //
        //寄存器ID常量  实例化时需要定义
        public const int D_ID = 0;   //默认占两个通道
        public const int HD_ID = 1;
        public const int HSD_ID = 2;

        public const int M_ID = 3;
        public const int HM_ID = 4;
        public const int X_ID = 5;
        public const int Y_ID = 6;

        public const int Delta_D_ID = 7;

        // 监控模式和进制
        public static string[] mode = { "位", "浮点", "单字", "双字" };
        public static string[] bin = { "10进制", "2进制", "16进制", "无符号", "ASCII" };
        public static string[] strDMArea = { "D", "HD", "HSD", "M", "HM", "X", "Y" };

        #endregion 信捷PLC 专用

        #region 锯片旋转带打孔
        public static readonly char Angle45 = '/';
        public static readonly char Angle135 = '\\';
        public static readonly char Angle90 = '|';
        public static readonly  int Angle90Int = 90000;
        #endregion
        public const int DeviceNoConnection = -1;
        public const int DeviceConnected = 0;
        //
        public const int PLCXY = 7;
        //D区数据太多 要分开显示 那滚动条结束后 才能显示 不然会出错
        public const int ScrollTimerValue = 200;


        #region 远邦台技数据
        public static readonly string[] strformatYB = { "日计划单号", "日期", "车间", "图号", "名称", "工序", "工艺特性", "姓名", "人员特性", "设备大类", "设备编号", "设备地址", "设备特性", "图纸链接", "调度说明", "排产量", "节拍", "机数", "工模具" };
        public static readonly string[] strformatYBSave = { "计划单号", "日期", "车间", "图号", "名称", "工序", "姓名",  "设备编号", "实产量", "开始时间", "实际结束时间", "停机时间", "不正常原因"};
        public static readonly string[] strformatSql = { "日计划单号", "日期", "车间", "图号", "名称", "工序", "工艺特性", "姓名", "人员特性", "设备大类", "设备编号", "设备地址", "设备特性", "图纸链接", "调度说明", "排产量", "节拍", "机数", "工模具", "实际节拍", "当前产量", "开始时间", "理论结束时间", "实际结束时间" };
        public static readonly string[] strformatSqlType = { "System.String", "System.DateTime", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.String", "System.Int32", "System.Int32", "System.String", "System.String", "System.Int32", "System.Int32", "System.DateTime", "System.DateTime", "System.DateTime" };
        public static readonly int ServerPort = 8899;
        public static readonly string BeginToListen = "开始搜寻设备！";
        public static readonly string ErrorSocConnection = "网络连接错误！";
        public static readonly string NoIdDevice = "未知设备！";
        public static readonly int ReadSocTimeOut = 2000;
        public static readonly int WriteSocTimeOut = 2000;
        public static readonly string sqlDb = "zlzk";
        public static readonly string sqlDataTableName = "deviceInfo";
        public static readonly string sqlGetDataTable = "SELECT * FROM deviceinfo";
        public static readonly string sqlDeviceIp = "设备地址";
        public static readonly string showItemPath = ConstantMethod.GetAppPath() + "showItem.xlsx";
        public const  int AddWork  = 0;
        public const  int DelWork  = 1;
        public const  int sqlWrite = 1;
        public const  int sqlRead  = 0;
        #endregion
    }

}

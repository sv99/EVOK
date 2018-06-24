﻿using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc
{
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
    //委托事件传递类 这里用作传递数据接收后处理的参数
    public class commEventArgs : EventArgs
    {
        byte[] byte_buffer;
        public byte[] Byte_buffer
        {
            get { return byte_buffer; }
            set { byte_buffer = value; }
        }
        public commEventArgs()
        {

        }
    }
    public delegate void commDataProcess(object s, commEventArgs e);//声明自定义的事件委托，用来执行事件的声明，和处理方法的传递
    public class Constant
        {
        public static readonly string ConnectMachineSuccess = "设备连接成功！";
        public static readonly string ConnectMachineFail    = "设备连接失败,请检查设备端口！";
        public static readonly string ReadPlcInfoFail       = "读取设备数据文件失败，软件即将关闭！";
        public static readonly string CSVFiileEX = ".csv";
        public static readonly string SingleMode = "单字";
        public static readonly string DoubleMode = "双字";
        public static readonly string BitMode = "位";
        public static readonly string MachineWorking = "设备运行中！";
        public static readonly string SetDataFail = "数据设置失败！";
        public static readonly string FileIsInUse = "文件使用中！";
        public static readonly int ExcelFile = 1;
        public static readonly int CsvFile = 2;
        public static readonly int ErrorFile = 0;
        public static readonly string Alarm = "报警";
        //线圈值常量
        public static readonly int M_ON = 1;
        public static readonly int M_OFF = 0;
        //PLC 要读取的数据的文件列名 CSV
        public static readonly string[] PLCstrCol = {"addr","mode", "bin", "count","value","param1","param2","param3","param4","param5","param6"};
        public static readonly string PlcDataFilePathAuto = AppFilePath + "Plc Data\\plc_data_auto.csv";
        public static readonly string PlcDataFilePathHand = AppFilePath + "Plc Data\\plc_data_hand.csv";
        public static readonly string PlcDataFilePathParam = AppFilePath + "Plc Data\\plc_data_param.csv";
        public static readonly string PlcDataFilePathIO = AppFilePath + "Plc Data\\plc_data_IO.csv";
        public static readonly string PlcDataFilePath3 = AppFilePath + "Plc Data\\plc_data3.csv";
        public static readonly string ErrorMValue = "位值只能为1或者0！";
        public static readonly string AppFilePath = System.AppDomain.CurrentDomain.BaseDirectory;
        public static readonly string ConfigFilePath = AppFilePath + "config\\config.xml";
        public static readonly string ErrorConfigFile = "配置文件不存在，请检查config文件夹，软件即将关闭！";
        public static readonly string ErrorPlcFile = "设备数据文件不存在，请检查plc data文件夹，软件即将关闭！";
        public static readonly string strValue = "value";
        public static readonly string strParam1 = "param1";
        public static readonly string strParam2 = "param2";
        public static readonly string strParam3 = "param3";
        public static readonly string strParam4 = "param4";
        public static readonly string strParam5 = "param5";
        public static readonly string strParam6 = "param6";
        public static readonly string PortName = "PortName";
        public static readonly string Bin = "bin";
        public static readonly string Read= "读";
        public static readonly string Write = "写";

        public static readonly string[] strformatEh = { "size", "CountToCut", "cntdone", "barcode", "param1" };
        public static readonly string[] strformatZh = { "尺寸", "设定数量", "已切数量", "条码", "参数1" };

        #region  优化数据
        //优化错误返回值
        public static readonly string prodLstNoData = "数据收集错误！";
        public static readonly string  optFail = "优化结果错误！";
        public static readonly string optResultNoData = "优化结果无数据！";
        public static readonly string optSuccess = "优化结束！";
        public static readonly string NextOpt = "下一根料准备！";
        public static readonly string ErrorMeasure = "测量错误！";
        public static readonly int MeaSureMaxTime = 30000;
        #endregion

        #region 运行数据
        public static readonly string emgStopTip = "急停或紧急退出中，请复位！";
        public static readonly string noData = "无数据！";
        public static readonly string measureOutOfTime = "测量超时！";
        public static readonly string IsWorking = "设备生产中！";
        public static readonly string CutEnd = "生产结束！";
        #endregion
        #region 台达PLC专用
        //台达PLC 发送命令 判断是否存在 存在则回复以下
        // 3a 30 31 30 35 30 43 30 30 46 46 30 30 45 46 0d 0a
        public static readonly byte[] DTExistByteOutIn = 
            { 0x3a,0x30,0x31 ,0x30 ,0x35 ,0x30 ,0x43 ,0x30 ,0x30 ,0x46 ,0x46 ,0x30 ,0x30 ,0x45 ,0x46 ,0x0d ,0x0a };       
        #endregion
        #region 信捷PLC 专用
        //信捷PLC 发送命令 判断是否存在 存在则回复以下
        // 01 03 f5 c0 00 01 b7 fa
        public static readonly byte[] XJExistByteOut = { 0x01, 0x03, 0xf5, 0xc0, 0x00, 0x01, 0xb7, 0xfa };
        //信捷PLC 设备存在则会返回
        //01 03 02 00  02 39 85
        public static readonly byte[] XJExistByteIn = { 0x01, 0x03, 0x02, 0x00, 0x02, 0x39, 0x85 };
        //device类和设备重新握手的次数
        public static readonly int DeviceErrorConnCountMax = 2;
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
        //读取超时 
        public static readonly int ReadTimeOut = 200; //这里0.5 秒别改 工控机性能不行

        //写入超时 
        public static readonly int WriteTimeOut = 500;

        public static readonly byte[] XJReadDataCmdCheck = { 0x01, 0x19 };



        //地址偏移常量
        public static readonly int M_addr = 0;
        public static readonly int D_addr = 0;
        public static readonly int X_addr = 0x5000;
        public static readonly int Y_addr = 0x6000;
        public static readonly int HD_addr = 0xA080;
        public static readonly int HSD_addr = 0xB880;
        public static readonly int HM_addr = 0xC100;
        public static readonly int Delta_D_addr = 0x1000;
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
        public const int DeviceNoConnection = -1;
        public const int DeviceConnected = 0;
        //
        public const int PLCXY = 7;

    }

}

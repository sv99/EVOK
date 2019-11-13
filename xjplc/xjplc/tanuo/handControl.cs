using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc.tanuo
{
    public class handControl
    {

        public handControl(Action<byte[]>  datapro)
        {
            decodeData = datapro;
            Init();
        }

        public bool findHandControl()
        {
            return false;
        }

        easyCom eCom;

        bool isExist = false;

        bool findPort()
        {
            SerialPort m_serialPort = new SerialPort();

            string[] str = SerialPort.GetPortNames();

            List<string> portNameLst = new List<string>();
            PortParam portparam0 = new PortParam();


            portparam0 = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);
            ConstantMethod.fileCopy(Constant.ConfigSerialportFilePath, Constant.ConfigSerialportFilePath_bak);

            for (int i = 0; i < (str.Length + 1); i++)
            {
                try
                {
                    if (i == 0)
                        m_serialPort.PortName = portparam0.m_portName;
                    else
                    {
                        if (!str[i - 1].Equals(portparam0.m_portName))
                            m_serialPort.PortName = str[i - 1];
                        else continue;
                    }
                    m_serialPort.BaudRate = portparam0.m_baudRate;
                    m_serialPort.Parity = portparam0.m_parity;
                    m_serialPort.StopBits = portparam0.m_stopbits;
                    m_serialPort.Handshake = portparam0.m_handshake;
                    m_serialPort.DataBits = portparam0.m_dataBits;
                    m_serialPort.ReadBufferSize = 1024;
                    m_serialPort.WriteBufferSize = 1024;
                    m_serialPort.ReadTimeout = 100;
                    if (!m_serialPort.IsOpen)
                    {
                        m_serialPort.Open();
                    }
                    byte[] resultByte = new byte[Constant.XJExistByteIn.Count()];

                    m_serialPort.Write(Constant.XJExistByteOut, 0, Constant.XJExistByteOut.Length);

                    ConstantMethod.Delay(200);

                    m_serialPort.Read(resultByte, 0, Constant.XJExistByteIn.Count());

                    if (ConstantMethod.compareByteStrictly(resultByte, Constant.XJExistByteIn))
                    {
                        //rtbResult.AppendText("连接成功" + m_serialPort.PortName);
                        ConstantMethod.SetPortParam(Constant.ConfigSerialportFilePath, Constant.PortName, m_serialPort.PortName);
                        return true;
                    }
                }
                catch
                {
                    //rtbResult.AppendText("连接失败" + m_serialPort.PortName);
                    //throw new SerialPortException(
                    //string.Format("无法打开串口:{0}", m_serialPort.PortName));
                    //continue;

                }
                finally { m_serialPort.Close(); };

            }

            return false;


        }
        void Init()
        {          
            PortParam portParam = ConstantMethod.LoadPortParam(Constant.ConfigSerialportFilePath);
            eCom = new easyCom(portParam);
            eCom.EventDataProcess += new commDataProcess(Dataprocess);
            sendHeart();
        }
        void Dataprocess(object sender, CommEventArgs e)
        {
            byte[] s = (byte[])e.Byte_buffer.Clone();

            if (s.Count() != 9) return;
            byte[] value =
            s.Skip(3).Take(2).ToArray();

            exeCmd(value);


        }

        #region  协议交流
        public Action<Byte[]> decodeData;
        void exeCmd(byte[] s)
        {
            isExist = true;
            if (decodeData != null)
            {
               decodeData(s);
               return;
            }
            //测试用 后续不在这里解析
            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdqj, s))
            {
                //showInfo("终端前进");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdht, s))
            {
                //showInfo("终端后退");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdzz, s))
            {
                // showInfo("终端左转");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdyz, s))
            {
                //showInfo("终端右转");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.zdstop, s))
            {
                // showInfo("终端停止");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambbAdd, s))
            {
                // showInfo("变倍+");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambbDec, s))
            {
                //  showInfo("变倍-");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambjAdd, s))
            {
                // showInfo("变焦+");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.cambjDec, s))
            {
                // showInfo("变焦-");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.camstop, s))
            {
                //  showInfo("摄像头动作停止");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytUp, s))
            {
                // showInfo("云台向上");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytDown, s))
            {
                //  showInfo("云台向下");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytLeft, s))
            {
                //  showInfo("云台向左");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytRight, s))
            {
                // showInfo("云台向右");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytReset, s))
            {
                //  showInfo("云台复位");
                return;
            }


            if (ConstantMethod.compareByteStrictly(CmdNewPack.ytstop, s))
            {
                //  showInfo("云台停止");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.tgUp, s))
            {
                // showInfo("抬杆向上");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.tgDown, s))
            {
                // showInfo("抬杆向下");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.tgstop, s))
            {
                // showInfo("抬杆停止动作");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cwOpen, s))
            {
                // showInfo("除雾开启");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.cwClose, s))
            {
                //showInfo("除雾关闭");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.camSnap, s))
            {
                //showInfo("截屏");
                return;
            }


            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightYAdd, s))
            {
                // showInfo("远光灯亮度+1");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightYDec, s))
            {
                // showInfo("远光灯亮度-1");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightJAdd, s))
            {
                // showInfo("近光灯亮度+1");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.lightJDec, s))
            {
                //showInfo("近光灯亮度-1");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.speedAdd, s))
            {
                // showInfo("速度+1");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.speedDec, s))
            {
                // showInfo("速度-1");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.jxpClear, s))
            {
                //showInfo("卷线盘米数清零");
                return;
            }

            if (ConstantMethod.compareByteStrictly(CmdNewPack.shouxian, s))
            {
                // showInfo("收线开关");
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.heart, s))
            {
                //showInfo("心跳包");
                sendHeart();
                return;
            }
            if (ConstantMethod.compareByteStrictly(CmdNewPack.connect, s))
            {
                //showInfo("连接指令");
                return;
            }

        }

        public void sendConn()
        {
            List<byte> cmdInTemp = new List<byte>();

            cmdInTemp.Add(CmdNewPack.controlAddress);

            cmdInTemp.Add(CmdNewPack.motorAddress);

            cmdInTemp.Add(0xaa);
            cmdInTemp.Add(0xaa);

            cmdInTemp.AddRange(ConstantMethod.CheckSum(cmdInTemp.ToArray()));

            cmdInTemp.AddRange(CmdNewPack.tail);

            cmdInTemp.Insert(0, CmdNewPack.header);

            eCom.WriteData(cmdInTemp.ToArray());

        }

        public void sendHeart()
        {
            List<byte> cmdInTemp = new List<byte>();

            cmdInTemp.Add(CmdNewPack.controlAddress);

            cmdInTemp.Add(CmdNewPack.motorAddress);


            cmdInTemp.Add(0xbb);

            cmdInTemp.Add(0xbb);

            cmdInTemp.AddRange(ConstantMethod.CheckSum(cmdInTemp.ToArray()));

            cmdInTemp.AddRange(CmdNewPack.tail);

            cmdInTemp.Insert(0, CmdNewPack.header);

            eCom.WriteData(cmdInTemp.ToArray());


        }

        #endregion


    }

    public class CmdNewPack
    {

        public static int modeAbs = 1;
        public static int modeRel = 0;
        public static byte[] tail = { 0x0d, 0x0a };
        public static byte header = 0x3A;
        public static int modeT = 1;
        public static int modeS = 2;

        public static int dirP = 1;
        public static int dirN = 0;

        public static byte controlAddress = 0x81;
        public static byte motorAddress = 0x01;

        public static byte[] zdqj = { 0x01, 0x01 };
        public static byte[] zdht = { 0x01, 0x02 };
        public static byte[] zdzz = { 0x01, 0x03 };
        public static byte[] zdyz = { 0x01, 0x04 };
        public static byte[] zdstop = { 0x01, 0x05 };

        public static byte[] cambbAdd = { 0x02, 0x01 };
        public static byte[] cambbDec = { 0x02, 0x02 };
        public static byte[] cambjAdd = { 0x02, 0x03 };
        public static byte[] cambjDec = { 0x02, 0x04 };
        public static byte[] camstop = { 0x02, 0x05 };


        public static byte[] ytUp = { 0x03, 0x01 };
        public static byte[] ytDown = { 0x03, 0x02 };
        public static byte[] ytLeft = { 0x03, 0x03 };
        public static byte[] ytRight = { 0x03, 0x04 };
        public static byte[] ytReset = { 0x03, 0x05 };
        public static byte[] ytstop = { 0x03, 0x06 };

        public static byte[] tgUp = { 0x04, 0x01 };
        public static byte[] tgDown = { 0x04, 0x02 };
        public static byte[] tgstop = { 0x04, 0x03 };

        public static byte[] cwOpen = { 0x05, 0x01 };
        public static byte[] cwClose = { 0x05, 0x02 };

        public static byte[] camSnap = { 0x06, 0x01 };

        public static byte[] lightYAdd = { 0x07, 0x01 };
        public static byte[] lightYDec = { 0x07, 0x02 };

        public static byte[] lightJAdd = { 0x08, 0x01 };
        public static byte[] lightJDec = { 0x08, 0x02 };

        public static byte[] speedAdd = { 0x09, 0x01 };
        public static byte[] speedDec = { 0x09, 0x02 };

        public static byte[] jxpClear = { 0x0a, 0x01 };

        public static byte[] shouxian = { 0x0b, 0x01 };

        public static byte[] heart = { 0xbb, 0xbb };
        public static byte[] connect = { 0xaa, 0xaa };


    }

}

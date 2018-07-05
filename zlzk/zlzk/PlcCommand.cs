using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace zlzk
{

    public class PlcInfo
    {
        private int minValue;
        public int MinValue
        {
            get { return minValue; }
            set { minValue = value; }
        }
        private int maxValue;
        public int MaxValue
        {
            get { return maxValue; }
            set { maxValue = value; }
        }
        public bool IsValueNormal
        {
            get
            {
                return ((showvalue > (minValue + 1)) && (showvalue < (maxValue + 1))) ? true : false;
            }

        }
        public Byte[] byteValue;

        private int xuhao;  //对于M 是buffer的 第几个字节开始 对于d的话 没啥用 只要byte是两个数量就好了
        public int Xuhao
        {
            get { return xuhao; }
            set { xuhao = value; }
        }
        private int area;
        public int Area
        {
            set { area = value; }
            get { return area; }
        }

        private int addr;
        public int Addr
        {
            set { addr = value; }
            get { return addr; }
        }

        private int showMode;
        public int ShowMode
        {
            get { return showMode; }
            set { showMode = value; }
        }

        private int showvalue;
        public int ShowValue
        {
            get
            {
                if (area < 3)
                {
                    showvalue = ((int)(byteValue[0] << 8)) | byteValue[1];
                }
                else
                {
                    int duibi = 0;
                    duibi = (int)Math.Pow(2, Xuhao);
                    showvalue = (byteValue[0] & duibi) == duibi ? 1 : 0;
                }

                return showvalue;
            }
        }
        public PlcInfo(int addr, int area)
        {
            this.addr = addr;
            this.area = area;

        }
        public PlcInfo()
        {

        }

    }
    public class DeltaPlcCommand
    {
       
            //地址偏移常量
            public static int M_addr = 0;
            public static int D_addr = 0;
            public static int X_addr = 0x5000;
            public static int Y_addr = 0x6000;
            public static int HD_addr = 0xA080;
            public static int HSD_addr = 0xB880;
            public static int HM_addr = 0xC100;
        //台达专用
        public static int Delta_D_addr = 0x1000;

        //
        //寄存器ID常量  实例化时需要定义
        public const int D_ID = 0;   //默认占两个通道
            public const int HD_ID = 1;
            public const int HSD_ID = 2;

            public const int M_ID = 3;
            public const int HM_ID = 4;
            public const int X_ID = 5;
            public const int Y_ID = 6;
        //台达专用
            public const int Delta_D_ID = 7;

        //线圈值常量
            public static int M_ON = 1;
            public static int M_OFF = 0;

            // 监控模式和进制
            public string[] mode = { "位", "浮点", "单字", "双字" };
            public string[] bin = { "10进制", "2进制", "16进制", "无符号", "ASCII" };
            public string[] MADDR = { "M", "HM" };
            public string[] DADDR = { "D", "HD", "HSD" };
            public DeltaPlcCommand()
            {
                byte05Report[0] = 0X01;
                byte05Report[1] = 0X05;
                byte05Report[2] = 0X60;
                byte05Report[3] = 0X00;
                byte05Report[4] = 0XFF;
                byte05Report[5] = 0X00;
                byte05Report[6] = 0X92;
                byte05Report[7] = 0X3A;


                byte0FReport[0] = 0X01;
                byte0FReport[1] = 0X0F;
                byte0FReport[2] = 0X60;
                byte0FReport[3] = 0X00;
                byte0FReport[4] = 0X00;
                byte0FReport[5] = 0X10;
                byte0FReport[6] = 0X4A;
                byte0FReport[7] = 0X07;

                byte0FReport[0] = 0X01;
                byte0FReport[1] = 0X10;
                byte0FReport[2] = 0X00;
                byte0FReport[3] = 0X00;
                byte0FReport[4] = 0X00;
                byte0FReport[5] = 0X03;
                byte0FReport[6] = 0X80;
                byte0FReport[7] = 0X08;

               byteDeltaCmd[0] = 0X01;
               byteDeltaCmd[1] = 0X05;
               byteDeltaCmd[2] = 0X0C;
               byteDeltaCmd[3] = 0X00;
               byteDeltaCmd[4] = 0XFF;
               byteDeltaCmd[5] = 0X00;
               byteDeltaCmd[6] = 0XEF;

            byteDeltaCmdRead[0] = 0X01;
            byteDeltaCmdRead[1] = 0X03;
            byteDeltaCmdRead[2] = 0X41;
            byteDeltaCmdRead[3] = 0X90;
            byteDeltaCmdRead[4] = 0X00;
            byteDeltaCmdRead[5] = 0X01;
            byteDeltaCmdRead[6] = 0X2A;

            byteDeltaCmdSetD0[0] = 0X01;
            byteDeltaCmdSetD0[1] = 0X10;
            byteDeltaCmdSetD0[2] = 0X10;
            byteDeltaCmdSetD0[3] = 0X00;
            byteDeltaCmdSetD0[4] = 0X00;
            byteDeltaCmdSetD0[5] = 0X01;
            byteDeltaCmdSetD0[6] = 0X02;
            byteDeltaCmdSetD0[7] = 0X00;
            byteDeltaCmdSetD0[8] = 0X00;
            byteDeltaCmdSetD0[9] = 0XDC;

            // 01 10 10 00 00 01 02 00 00 DC

        }


        public byte[] byte01 = new byte[8];
            public byte[] byte02 = new byte[8];
            public byte[] byte03 = new byte[8];
            public byte[] byte04 = new byte[8];
            public byte[] byte05 = new byte[8];
            public byte[] byte06 = new byte[8];
            public byte[] byte0F;
            public byte[] byte10;
            public byte[] byte05Report = new byte[8];
            public byte[] byte0FReport = new byte[8];
            public byte[] byte10Report = new byte[8];
             //台达PLC 在发送读取签 先发条命令进行测试 是否正确
            public byte[] byteDeltaCmd = new byte[7];
            //台达PLC 读取已经设置好的D 区的命令
            public byte[] byteDeltaCmdRead = new byte[7];
           //台达PLC 读取已经设置好的D 区的命令
           public byte[] byteDeltaCmdSetD0 = new byte[10];

        //信捷专用的码
        public byte[] byte19 = null;
            public int UnPack19(byte[] m_buffer, List<PlcInfo> dplcInfoLst, List<PlcInfo> mplcInfoLst)
            {
                //完整性检查
                if (m_buffer.Count() < 4) return -2;
                if (!(m_buffer[0] == 0x01 && m_buffer[1] == 0x19)) return -3;
                int dCount = dplcInfoLst.Count;
                int mCount = mplcInfoLst.Count;
                //数据个数不对 退出
                if (m_buffer.Count() != (3 + dCount * 2 + 2 + (int)Math.Ceiling((double)mCount / 8))) return -4;
                mCount = (int)Math.Ceiling((double)mCount / 8);
                byte[] dArea_buffer = m_buffer.Skip(3).Take(dCount * 2).ToArray();
                byte[] mArea_buffer = m_buffer.Skip(m_buffer.Count() - mCount - 2).Take(mCount).ToArray();//b.Skip(b.Count() - 8).ToArray(); 
                                                                                                          //分类读取
                if (dCount > 0)
                    for (int i = 0; i < dCount; i++)
                    {
                        if (dplcInfoLst[i].Area < (M_ID))
                        {
                            dplcInfoLst[i].byteValue[0] = dArea_buffer[i * 2];
                            dplcInfoLst[i].byteValue[1] = dArea_buffer[i * 2 + 1];

                        }

                    }

                mCount = mplcInfoLst.Count;
                if (mCount > 0)
                {
                    for (int i = 0; i < (mCount); i++)
                    {
                        if (mplcInfoLst[i].Area > (HSD_ID))
                        {
                            mplcInfoLst[i].byteValue[0] = mArea_buffer[i / 8];
                            mplcInfoLst[i].Xuhao = i % 8;
                        }

                    }
                }

                return 0;

            }
        //台达专用码 一个是ASCI码的字节 一个是最后转换后要发的数据
        public byte[] byteDelta10 = null;
  
        public byte[] deltaCmdSendByte(byte[] byteDelta)
        {
            string str = DataProcess.byteToHexStr(byteDelta);
            byte[] array = System.Text.Encoding.ASCII.GetBytes(str);
            List<byte> cmdbyte = new List<byte>();
            cmdbyte.Add(0x3A);
            cmdbyte.AddRange(array);
            cmdbyte.Add(0x0d);
            cmdbyte.Add(0x0a);

            return cmdbyte.ToArray();
        }
       
        public int PackDelta10(List<int> addrLst, List<int> idLst)
        {
            // 验证数据
            if (addrLst.Count != idLst.Count ) return -1;

            int Dcount = 0;
            int addr = 0;
            List<byte> cmdByte = new List<byte>();

            for (int i = 0; i < addrLst.Count; i++)
            {
                #region 根据读取类型 确定地址
                addr = addrLst[i];

                switch (idLst[i])
                {
                    case X_ID:
                        {
                            addr = addr + X_addr;
                            break;
                        }
                    case Y_ID:
                        {
                            addr = addr + Y_addr;
                            break;
                        }
                    case M_ID:
                        {
                            addr = addr + M_addr;
                            break;
                        }
                    case HM_ID:
                        {
                            addr = addr + M_addr;
                            break;
                        }
                    case D_ID:
                        {
                            addr = addr + D_addr;
                            Dcount++;
                            break;
                        }
                    case HSD_ID:
                        {
                            addr = addr + HSD_addr;
                            Dcount++;
                            break;
                        }
                    case HD_ID:
                        {
                            addr = addr + HD_addr;
                            Dcount++;
                            break;
                        }
                    case Delta_D_ID:
                        {
                            addr = addr + Delta_D_addr;
                            Dcount++;
                            break;
                        }
                    default:
                        {
                            addr = addr + M_addr;
                            break;
                        }
                }
                int addr_high = (addr & 0xFF00) >> 8;
                int addr_low = addr & 0xFF;


                cmdByte.Add((byte)addr_high);
                cmdByte.Add((byte)addr_low);
                
                
     
            }
                #endregion


                //开始拼接
                byteDelta10 = new byte[10 + addrLst.Count * 2];

                byteDelta10[0] = 0x01;
                byteDelta10[1] = 0x10;
                byteDelta10[2] = 0x40;
                byteDelta10[3] = 0xDC;
                int addrCount = addrLst.Count+1;
                int addr_high0 = (addrCount & 0xFF00) >> 8;
                int addr_low0 = addrCount & 0x00FF;
            
                byteDelta10[4] = (byte)(addr_high0);
                byteDelta10[5] = (byte)(addr_low0);

                byteDelta10[6] = (byte)(addrLst.Count * 2 + 2); //对照指令来进行

                addrCount = addrLst.Count;
                addr_high0 = (addrCount & 0xFF00) >> 8;
                addr_low0 = addrCount & 0x00FF;
                byteDelta10[7] = (byte)(addr_high0);
                byteDelta10[8] = (byte)(addr_low0);

                Array.Copy(cmdByte.ToArray(), 0, byteDelta10, 9, cmdByte.Count);

                //crc
                byte[] byteLrcData = new byte[byteDelta10.Count() - 1];

                Array.Copy(byteDelta10, 0, byteLrcData, 0, byteDelta10.Count() - 1);
               
                byteDelta10[byteDelta10.Count() - 1] = LRC16_C(byteLrcData);



                

            return 0;

        }


            
      

        public int PackCmd10(int addr, int count, int[] value, string D)
        {
            int ByteLen = count * 2;

            byte10 = new byte[9 + ByteLen];
            if (D == "HD")
            {
                addr += HD_addr;
            }
            int addr_high = (addr & 0xFF00) >> 8;
            int addr_low = addr & 0xFF;
            int count_high = (count & 0xFF00) >> 8;
            int count_low = count & 0xFF;
            //站号 命令码
            byte10[0] = 0x01;
            byte10[1] = 0x10;
            //地址 
            byte10[2] = (byte)addr_high;
            byte10[3] = (byte)addr_low;
            //个数
            byte10[4] = (byte)count_high;
            byte10[5] = (byte)count_low;
            //字节数
            byte10[6] = (byte)ByteLen;
            //值 (低位在前 高位在后） 低位在前 高位在后
            for (int i = 0; i < ByteLen; i = i + 2)
            {
                byte10[8 + i] = (byte)(value[i / 2] & 0xFF);

                byte10[7 + i] = (byte)((value[i / 2] >> 8) & 0xFF);
            }

            //拷贝数组传入CRC校验
            byte[] byteCrcData = new byte[7 + ByteLen];
            Array.Copy(byte10, 0, byteCrcData, 0, 7 + ByteLen);

            //crc
            byte10[7 + ByteLen] = CRC16_C(byteCrcData)[1];
            byte10[8 + ByteLen] = CRC16_C(byteCrcData)[0];
            return 0;

        }
          public PlcInfo GetPlcInfo(int addr, string XYM)
            {
                PlcInfo tmpInfo = new PlcInfo();
                switch (XYM)
                {

                    case "X":
                        {
                            addr = addr + X_addr;
                            tmpInfo.Area = X_ID;
                            tmpInfo.byteValue = new byte[1];
                            break;
                        }
                    case "Y":
                        {
                            addr = addr + Y_addr;
                            tmpInfo.Area = Y_ID;
                            tmpInfo.byteValue = new byte[1];
                            break;
                        }
                    case "M":
                        {
                            addr = addr + M_addr;
                            tmpInfo.byteValue = new byte[1];
                            tmpInfo.Area = M_ID;
                            break;
                        }
                    case "HM":
                        {
                            addr = addr + HM_addr;
                            tmpInfo.byteValue = new byte[1];
                            tmpInfo.Area = HM_ID;
                            break;
                        }
                    case "HD":
                        {
                            addr = addr + HD_addr;
                            tmpInfo.byteValue = new byte[2];
                            tmpInfo.Area = HD_ID;
                            break;
                        }
                    case "D":
                        {
                            addr = addr + D_addr;
                            tmpInfo.byteValue = new byte[2];
                            tmpInfo.Area = D_ID;
                            break;
                        }
                    case "HSD":
                        {
                            addr = addr + HSD_addr;
                            tmpInfo.byteValue = new byte[2];
                            tmpInfo.Area = HSD_ID;
                            break;
                        }
                    default:
                        {
                            addr = addr + M_addr;
                            tmpInfo.byteValue = new byte[1];
                            tmpInfo.Area = M_ID;
                            break;
                        }

                }
                tmpInfo.Addr = addr;
                tmpInfo.Xuhao = -1;
                return tmpInfo;

            }
            public int PackCmd01(int addr, int count, string XYM)
            {

                switch (XYM)
                {

                    case "X":
                        {
                            addr = addr + X_addr;
                            break;
                        }
                    case "Y":
                        {
                            addr = addr + Y_addr;
                            break;
                        }
                    case "M":
                        {
                            addr = addr + M_addr;
                            break;
                        }
                    case "HM":
                        {
                            addr = addr + HM_addr;
                            break;
                        }
                    default:
                        {
                            addr = addr + M_addr;
                            break;
                        }

                }
                int addr_high = (addr & 0xFF00) >> 8;
                int addr_low = addr & 0xFF;
                int count_high = (count & 0xFF00) >> 8;
                int count_low = count & 0xFF;

                //站号 命令码
                byte01[0] = 0x01;
                byte01[1] = 0x01;
                //地址 
                byte01[2] = (byte)addr_high;
                byte01[3] = (byte)addr_low;
                //个数
                byte01[4] = (byte)count_high;
                byte01[5] = (byte)count_low;

                //拷贝数组传入CRC校验
                byte[] byteCrcData = new byte[6];
                Array.Copy(byte01, 0, byteCrcData, 0, 6);

                //crc
                byte01[6] = CRC16_C(byteCrcData)[1];
                byte01[7] = CRC16_C(byteCrcData)[0];

                return 0;


            }

            public int PackCmd03(int addr, int count, string D)
            {
                if (D == "HD")
                {
                    addr += HD_addr;
                }
                if (D == "HSD")
                {
                    addr += HSD_addr;
                }

                int addr_high = (addr & 0xFF00) >> 8;
                int addr_low = addr & 0xFF;
                int count_high = (count & 0xFF00) >> 8;
                int count_low = count & 0xFF;

                //站号 命令码
                byte03[0] = 0x01;
                byte03[1] = 0x03;
                //地址 
                byte03[2] = (byte)addr_high;
                byte03[3] = (byte)addr_low;
                //个数
                byte03[4] = (byte)count_high;
                byte03[5] = (byte)count_low;


                //拷贝数组传入CRC校验
                byte[] byteCrcData = new byte[6];
                Array.Copy(byte03, 0, byteCrcData, 0, 6);

                //crc
                byte03[6] = CRC16_C(byteCrcData)[1];
                byte03[7] = CRC16_C(byteCrcData)[0];

                return 0;
            }

            public int PackCmd05(int addr, int value, string XYM)
            {

                switch (XYM)
                {
                    case "X":
                        {
                            addr = addr + X_addr;
                            break;
                        }
                    case "Y":
                        {
                            addr = addr + Y_addr;
                            break;
                        }
                    case "M":
                        {
                            addr = addr + M_addr;
                            break;
                        }
                    case "HM":
                        {
                            addr = addr + HM_addr;
                            break;
                        }
                    default:
                        {
                            addr = addr + M_addr;
                            break;
                        }

                }
                int addr_high = (addr & 0xFF00) >> 8;
                int addr_low = addr & 0xFF;
                //站号 命令码
                byte05[0] = 0x01;
                byte05[1] = 0x05;
                //地址 
                byte05[2] = (byte)addr_high;
                byte05[3] = (byte)addr_low;
                //值

                byte05[4] = 0;
                byte05[5] = 0;
                if (value == 1)
                {
                    byte05[4] = 0xFF;
                }

                //拷贝数组传入CRC校验
                byte[] byteCrcData = new byte[6];
                Array.Copy(byte05, 0, byteCrcData, 0, 6);

                //crc
                byte05[6] = CRC16_C(byteCrcData)[1];
                byte05[7] = CRC16_C(byteCrcData)[0];
                return 0;
            }

            public int PackCmd0F(int addr, int count, int value, string XYM)
            {
                switch (XYM)
                {
                    case "X":
                        {
                            addr = addr + X_addr;
                            break;
                        }
                    case "Y":
                        {
                            addr = addr + Y_addr;
                            break;
                        }
                    case "M":
                        {
                            addr = addr + M_addr;
                            break;
                        }

                    default:
                        {
                            addr = addr + M_addr;
                            break;
                        }


                }
                //根据线圈个数来决定 数组个数 
                int ByteLenAdd = count % 8;
                int ByteLen = (count / 8);

                if (ByteLenAdd > 0)
                {
                    ByteLen = (count / 8) + 1;
                }

                byte0F = new byte[9 + ByteLen];
                int addr_high = (addr & 0xFF00) >> 8;
                int addr_low = addr & 0xFF;
                int count_high = (count & 0xFF00) >> 8;
                int count_low = count & 0xFF;
                //站号 命令码
                byte0F[0] = 0x01;
                byte0F[1] = 0x0F;
                //地址 
                byte0F[2] = (byte)addr_high;
                byte0F[3] = (byte)addr_low;
                //个数
                byte0F[4] = (byte)count_high;
                byte0F[5] = (byte)count_low;
                //字节数
                byte0F[6] = (byte)ByteLen;
                //值 (低位在前 高位在后） 先取低8位
                for (int i = 0; i < ByteLen; i++)
                {
                    byte0F[7 + i] = (byte)(value & 0xFF);
                    value = (value >> 8);
                }

                //拷贝数组传入CRC校验
                byte[] byteCrcData = new byte[7 + ByteLen];
                Array.Copy(byte0F, 0, byteCrcData, 0, 7 + ByteLen);

                //crc
                byte0F[7 + ByteLen] = CRC16_C(byteCrcData)[1];
                byte0F[8 + ByteLen] = CRC16_C(byteCrcData)[0];


                return 0;
            }

            public int PackCmd19(List<int> addrLst, List<int> idLst, List<int> addrcount)
            {
                //验证数据
                if (addrLst.Count != idLst.Count || addrLst.Count != addrcount.Count) return -1;

                int Dcount = 0, Mcount = 0;
                int addr = 0, count = 0;
                List<byte> cmdByte = new List<byte>();
                for (int i = 0; i < addrLst.Count; i++)
                {
                    #region 根据读取类型 确定地址
                    addr = addrLst[i];
                    count = addrcount[i];

                    switch (idLst[i])
                    {
                        case X_ID:
                            {
                                addr = addr + X_addr;
                                break;
                            }
                        case Y_ID:
                            {
                                addr = addr + Y_addr;
                                break;
                            }
                        case M_ID:
                            {
                                addr = addr + M_addr;
                                break;
                            }
                        case HM_ID:
                            {
                                addr = addr + M_addr;
                                break;
                            }
                        case D_ID:
                            {
                                addr = addr + D_addr;
                                Dcount++;
                                break;
                            }
                        case HSD_ID:
                            {
                                addr = addr + HSD_addr;
                                Dcount++;
                                break;
                            }
                        case HD_ID:
                            {
                                addr = addr + HD_addr;
                                Dcount++;
                                break;
                            }
                        default:
                            {
                                addr = addr + M_addr;
                                break;
                            }
                    }
                    int addr_high = (addr & 0xFF00) >> 8;
                    int addr_low = addr & 0xFF;
                    int count_high = (count & 0xFF00) >> 8;
                    int count_low = count & 0xFF;

                    cmdByte.Add((byte)addr_high);
                    cmdByte.Add((byte)addr_low);
                    cmdByte.Add((byte)count_high);
                    cmdByte.Add((byte)count_low);

                    #endregion


                    //开始拼接
                    byte19 = new byte[7 + addrLst.Count * 4];

                    byte19[0] = 0x01;
                    byte19[1] = 0x19;
                    byte19[2] = 0x00;
                    byte19[3] = (byte)addrLst.Count;
                    byte19[4] = (byte)(Dcount + 1);

                    Array.Copy(cmdByte.ToArray(), 0, byte19, 5, cmdByte.Count);

                    //crc
                    byte[] byteCrcData = new byte[byte19.Count() - 2];

                    Array.Copy(byte19, 0, byteCrcData, 0, byte19.Count() - 2);

                    byte19[byte19.Count() - 2] = CRC16_C(byteCrcData)[1];
                    byte19[byte19.Count() - 1] = CRC16_C(byteCrcData)[0];


                }


                return 0;
            }

            

            public int[] DataUnpack03(int addr, int cnt, Byte[] DataByte)
            {

                int[] valueR = new int[cnt];
                //int[] value = new int[Len];

                if (!(DataByte.Length > 3)) { return valueR; }
                int Len = DataByte[2] / 2;
                int[] value = new int[Len];

                if ((DataByte[2] % 2) == 0)
                {

                    for (int j = 0; j < Len; j++)
                    {
                        // value[j] = DataByte[4 + j * 2];
                        //value[j] = (value[j] <<8);
                        //value[j] = DataByte[3 + j * 2];
                        value[j] = ((int)(DataByte[3 + j * 2] << 8)) | DataByte[4 + j * 2];
                    }
                }
                Array.Copy(value, 0, valueR, 0, cnt);
                return valueR;

            }

            public int Pack4BytesToInt(int Low, int High)
            {
                int valueLow = Low;
                int valueHigh = High << 16;

                int valueLong = valueHigh | valueLow;
                return valueLong;
            }
            //判断传入的数据是否完整 根据CRC 校验判别
            public static Boolean IsEnd(Byte[] BufList)
            {

                if (BufList.Count() < 5) return false;


                byte[] buf = BufList;


                byte[] buf1 = new byte[buf.Count() - 2];//去掉末尾两个数据

                Array.Copy(buf, buf1, buf.Count() - 2);

                byte[] buf2 = new byte[2];
                buf2[0] = CRC16_C(buf1)[0];
                buf2[1] = CRC16_C(buf1)[1];

                if ((buf[(buf.Count() - 1)] == buf2[0]) && (buf[(buf.Count() - 2)] == buf2[1]))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        public static byte LRC16_C(byte[] data)
        {
            byte result = 0;
            int cnt=0;
            for (int i = 0; i < data.Length; i++)
            {
                cnt = cnt + data[i];

            }
            result = (byte)(256 - cnt % 256);
            return result;

        }
            public static byte[] CRC16_C(byte[] data)
            {
                byte CRC16Lo;
                byte CRC16Hi;   //CRC寄存器 
                byte CL; byte CH;       //多项式码&HA001 
                byte SaveHi; byte SaveLo;
                byte[] tmpData;

                int Flag;
                CRC16Lo = 0xFF;
                CRC16Hi = 0xFF;
                CL = 0x01;
                CH = 0xA0;
                tmpData = data;
                for (int i = 0; i < tmpData.Length; i++)
                {
                    CRC16Lo = (byte)(CRC16Lo ^ tmpData[i]); //每一个数据与CRC寄存器进行异或 
                    for (Flag = 0; Flag <= 7; Flag++)
                    {
                        SaveHi = CRC16Hi;
                        SaveLo = CRC16Lo;
                        CRC16Hi = (byte)(CRC16Hi >> 1);      //高位右移一位 
                        CRC16Lo = (byte)(CRC16Lo >> 1);      //低位右移一位 
                        if ((SaveHi & 0x01) == 0x01) //如果高位字节最后一位为1 
                        {
                            CRC16Lo = (byte)(CRC16Lo | 0x80);   //则低位字节右移后前面补1 
                        }             //否则自动补0 
                        if ((SaveLo & 0x01) == 0x01) //如果LSB为1，则与多项式码进行异或 
                        {
                            CRC16Hi = (byte)(CRC16Hi ^ CH);
                            CRC16Lo = (byte)(CRC16Lo ^ CL);
                        }
                    }
                }
                byte[] ReturnData = new byte[2];
                ReturnData[0] = CRC16Hi;       //CRC高位 
                ReturnData[1] = CRC16Lo;       //CRC低位 
                return ReturnData;
            }


        }
    
}

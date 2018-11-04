using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xjplc.delta
{

    public class DTTcpCmdPackAndDataUnpack
    {


        /// <summary>
        /// 获取相应的单元体数据
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="count"></param>
        /// <param name="area"></param>
        /// <param name="mode"></param>
        /// <returns></returns>
        public static DTTcpPlcInfo[] GetPlcInfo(int addr, int count, string area, string mode)
        {
            area = area.ToUpper();
            mode = mode.ToUpper();
            List<DTTcpPlcInfo> dtPlcInfoLst = new List<DTTcpPlcInfo>();
            Dictionary<string, int> typeAndByteCount = new Dictionary<string, int>();
            //数据表达类型
            for (int m = 0; m < Constant.tcpType.Count(); m++)
            {
                typeAndByteCount.Add(Constant.tcpType[m], Constant.tcpTypeByteCount[m]);
            }
                                  
            for (int i = 0; i < count; i++)
            {
                DTTcpPlcInfo dtInfo = new DTTcpPlcInfo(addr, area, mode);
                dtPlcInfoLst.Add(dtInfo);
            }           
           return dtPlcInfoLst.ToArray();
       }
        //传入需要读取的数据 返回 两个命令数组
        //目前D区 数量 就按照一个 一个读就可以了 所以count这里就1
        public static void PackReadDCmd(DTTcpPlcInfo[] plcInfoLst, List<byte> cmdOut,  List<byte> cmdIn0)
        {
            if (plcInfoLst==null || plcInfoLst.Count() < 1) return;
            List<int> addrLst = new List<int>();//连续地址的起始地址
            List<int> idLst = new List<int>();  //地址是D xy HSD
            List<int> addrcount = new List<int>(); //起始地址开始 读取几个寄存器

            Dictionary<string, int> addrCountByte = new Dictionary<string, int>();
            //读取数量的偏移int 2个字节  dint4个字节
            ConstantMethod.ArrayToDictionary(addrCountByte,Constant.tcpType,Constant.tcpTypeByteCount);
            ////ML MB  MW 这种针对的偏移都不同 相对地址不同
            //Dictionary<string, int> addrShift = new Dictionary<string, int>();
            ////读取数量的偏移int 2个字节  dint4个字节
            //ConstantMethod.ArrayToDictionary(addrShift, Constant.dataType, Constant.dataTypeAddrOffset);
            foreach (DTTcpPlcInfo d in plcInfoLst)
            {
                addrLst.Add(d.RelAddr);
                idLst.Add(d.IntArea);
                addrcount.Add(1*addrCountByte[d.ValueMode]);

            }
            cmdOut.Clear();
            cmdIn0.Clear();
            cmdOut.AddRange(PackReadByteCmd(addrLst.ToArray(), idLst.ToArray(), addrcount.ToArray(), cmdIn0));

        }
       
        public static void PackReadMCmd(DTTcpPlcInfo[] plcInfoLst, List<byte> cmdOut, List<byte> cmdIn0)
        {
            List<int> addrLst = new List<int>();//连续地址的起始地址
            List<int> idLst = new List<int>();  //地址是D xy HSD
            List<int> addrcount = new List<int>(); //起始地址开始 读取几个寄存器

            Dictionary<string, int> addrCountByte = new Dictionary<string, int>();
            //读取数量的偏移int 2个字节  dint4个字节
            ConstantMethod.ArrayToDictionary(addrCountByte, Constant.tcpType, Constant.tcpTypeByteCount);
            //ML MB  MW 这种针对的偏移都不同 相对地址不同
            Dictionary<string, int> addrShift = new Dictionary<string, int>();
            //读取数量的偏移int 2个字节  dint4个字节
            ConstantMethod.ArrayToDictionary(addrShift, Constant.dataType, Constant.dataTypeAddrOffset);
            foreach (DTTcpPlcInfo d in plcInfoLst)
            {
                addrLst.Add(d.RelAddr * addrShift[d.StrArea]);
                idLst.Add(d.IntArea);
                addrcount.Add(1 * addrCountByte[d.ValueMode]);

            }
            cmdOut.Clear();
            cmdIn0.Clear();
            cmdOut.AddRange(PackReadBitCmd(addrLst.ToArray(), idLst.ToArray(), addrcount.ToArray(), cmdIn0));

        }
      
        //根据字符 获取地址范围
        public static int GetIntAreaFromStr(string XYM)
        {

            Dictionary<string, int> addrAbsFromType = new Dictionary<string, int>();
            ConstantMethod.ArrayToDictionary(addrAbsFromType, Constant.dataType, Constant.dataTypeAreaInt);
            int addr = -1;
            if (addrAbsFromType.TryGetValue(XYM, out addr))
            {
                
            }

            return addr;
        }
        //根据字符获取绝对地址
        public static int GetAbsAddrFromStr(int maddr, string XYM)
        {

            Dictionary<string, int> addrAbsFromType = new Dictionary<string, int>();
            ConstantMethod.ArrayToDictionary(addrAbsFromType,Constant.dataType,Constant.dataTypeAddr);
            int addr = -1;
            Dictionary<string, int> addrShift = new Dictionary<string, int>();
            //读取数量的偏移int 2个字节  dint4个字节
            ConstantMethod.ArrayToDictionary(addrShift, Constant.dataType, Constant.dataTypeAddrOffset);

            if (addrAbsFromType.TryGetValue(XYM, out addr))
            {
                addr = maddr * addrShift[XYM] + addr;
            }

            return addr;
        }
        public static int GetAbsAddrFromInt(int maddr, int XYM)
        {

            Dictionary<int, int> addrAbsFromType = new Dictionary<int, int>();
            ConstantMethod.ArrayToDictionary(addrAbsFromType, Constant.dataTypeAreaInt, Constant.dataTypeAddr);

            Dictionary<int, int> addrShift = new Dictionary<int, int>();
            //读取数量的偏移int 2个字节  dint4个字节
            ConstantMethod.ArrayToDictionary(addrShift, Constant.dataTypeAreaInt, Constant.dataTypeAddrOffset);

            int addr = -1;
            if (addrAbsFromType.TryGetValue(XYM, out addr))
            {
                addr = maddr* addrShift[XYM] + addr;
            }
            return addr;
        }
        public static byte[] GetAbsAddr(int maddr, string XYM, int addrbytecount)
        {
            List<byte> cmdLst = new List<byte>();          
            cmdLst.AddRange(BitConverter.GetBytes(GetAbsAddrFromStr(maddr, XYM)));
            cmdLst.Reverse();
            return cmdLst.ToArray();

        }
        //bytecount 代表返回几个字节的地址
        public static byte[] GetAbsAddr(int addr, int addressid,int addrbytecount)
        {

            
            List<byte> cmdLst = new List<byte>();
            cmdLst.AddRange(BitConverter.GetBytes(GetAbsAddrFromInt(addr, addressid)));
            cmdLst.Reverse();

            return cmdLst.ToArray();

        }

        public static byte[] getRandom4Byte()
        {
            List<byte> result = new List<byte>();

            Random rd = new Random();
            int a = rd.Next(0, 300);

            result.AddRange(BitConverter.GetBytes(a));

            return result.ToArray();

        }
        /// <summary>
        /// 指定位置插入字节数
        /// </summary>
        /// <param name="cmdOutLst"></param>
        /// <param name="pos"></param>
        public static void InserCount(List<byte> cmdOutLst,int pos)
        {
            cmdOutLst.InsertRange(pos, ConstantMethod.getDataHighLowByte(cmdOutLst.Count));
        }

        //读取多个位 IX MX QX IB QB
        //设置多个位  IX MX QX 

        /// <summary>  
        //0E 77 00 00      协议号 发什么返回什么
        //00 37            后面的字节数
        //01               站号
        //43 01            功能码
        //00 30            后面字节数
        //01 C0 20 01 00 01  MW0.1  地址加读取个数
        //01 C0 20 02 00 01  MW0.2
        //01 C0 1C 01 00 01  QX0.1
        //01 C0 1C 02 00 01  QX0.2
        //01 C0 20 03 00 01  MX0.3
        //01 C0 20 05 00 01  MX0.5
        //01 C0 20 2D 00 01  MX5.5
        //01 C0 20 00 00 01  MX0.0 
        //A4 D4
        //回复：
        //B7 68 00 00     协议号 发什么返回什么
        //00 1D           后面的字节数
        //01              站号
        //43 01           功能码
        //00 18           后面的字节数
        //00 01 00        字节数00 01+值00--- MW0.1   false
        //00 01 01        字节数00 01+值00--- MW0.2   true
        //00 01 00       QX0.1
        //00 01 00       QX0.2
        //00 01 00       MX0.3
        //00 01 00       MX0.5
        //00 01 00       MX5.5
        //00 01 00       MX0.0
    
        /// </summary>
        /// 输入地址 地址属性 地址后面要读取的个数
        /// <param name="addr"></param>
        /// <param name="addressid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] PackReadBitCmd(int[] addr, int[] addressid, int[] count, List<byte> cmdInLst)
        {
            if (addressid.Count() != addr.Count() || count.Count() != addr.Count())
            {
                return null;
            }

            //协议号+后面的字节数+站号+功能码+后面字节数+地址加读取字节数+...+CRC校验
            List<byte> cmdOutLst = new List<byte>();  

            //地址变换
            for (int i=0;i<addr.Count();i++)
            {
                cmdOutLst.AddRange(GetAbsAddr(addr[i], addressid[i],4));
                cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(count[i]));
            }

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);

            cmdOutLst.InsertRange(0,Constant.DTTcpFunctionReadBitCmd);
            cmdOutLst.InsertRange(0, Constant.DTDeviceId);

            cmdOutLst.AddRange(ConstantMethod.CRC16_C(cmdOutLst.ToArray()));

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);

            cmdOutLst.InsertRange(0, Constant.DTTcpHeader);


            /***
             * 输入数据
           
            ****/           

            for (int i = 0; i < count.Count(); i++)
            {
                int cnt = (int)Math.Ceiling((double)count[i] /8 );
                cmdInLst.AddRange(ConstantMethod.getDataHighLowByte(cnt));
                for (int j = 0; j < cnt; j++)
                {
                    cmdInLst.Add(0x00);
                }

            }
            DTTcpCmdPackAndDataUnpack.InserCount(cmdInLst, 0);
            cmdInLst.InsertRange(0, Constant.DTTcpFunctionReadBitCmd);
            cmdInLst.InsertRange(0, Constant.DTDeviceId);
            DTTcpCmdPackAndDataUnpack.InserCount(cmdInLst, 0);

            cmdInLst.InsertRange(0, Constant.DTTcpHeader);


            return cmdOutLst.ToArray();

        }

        /// <summary>
        /// 报文结构：  
        //01 FA 00 00 
        //00 1F                后面字节数
        //01                   站号
        //43 02               功能码
        //00 18               后面字节数
        //00 38 04 00 00 02   地址+读取字节数 MB0 MB1 
        //00 38 04 01 00 02   地址+读取字节数 MB1 MB2
        //00 38 04 02 00 02   地址+读取字节数 MB2 MB3
        //00 38 04 64 00 01   地址+读取字节数 MB100 
        //47 2E               crc 校验码    
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="addressid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] PackReadByteCmd(int[] addr, int[] addressid, int[] count, List<byte> cmdInLst)
        {
            if (addressid.Count() != addr.Count() || count.Count() != addr.Count())
            {
                return null;
            }
            //协议号+后面的字节数+站号+功能码+后面字节数+地址加读取字节数+...+CRC校验
            List<byte> cmdOutLst = new List<byte>();
        
            //地址变换
            for (int i = 0; i < addr.Count(); i++)
            {
                cmdOutLst.AddRange(GetAbsAddr(addr[i], addressid[i], 4));
                cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(count[i]));
            }

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);

            cmdOutLst.InsertRange(0, Constant.DTTcpFunctionReadByteCmd);
            cmdOutLst.InsertRange(0, Constant.DTDeviceId);

            cmdOutLst.AddRange(ConstantMethod.CRC16_C(cmdOutLst.ToArray()));

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);

            cmdOutLst.InsertRange(0, Constant.DTTcpHeader);

            /***
             * 输入数据
           
            ****/

            for (int i = 0; i < count.Count(); i++)
            {               
                cmdInLst.AddRange(ConstantMethod.getDataHighLowByte(count[i]));
                for (int j = 0; j < count[i]; j++)
                {
                    cmdInLst.Add(0x00);
                }

            }
            DTTcpCmdPackAndDataUnpack.InserCount(cmdInLst, 0);
            cmdInLst.InsertRange(0, Constant.DTTcpFunctionReadByteCmd);
            cmdInLst.InsertRange(0, Constant.DTDeviceId);
            DTTcpCmdPackAndDataUnpack.InserCount(cmdInLst, 0);

            cmdInLst.InsertRange(0, Constant.DTTcpHeader);

         
            return cmdOutLst.ToArray();


        }

        /// <summary>
        /// 报文结构：    
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="addressid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] PackWriteByteCmd(int[] addr, int[] addressid, int[] count, List<byte[]> value, List<byte> cmdInLst)
        {


            if (addressid.Count() != addr.Count() || count.Count() != addr.Count() || value.Count()!=addr.Count())
            {
                return null;
            }
     
            //协议号+后面的字节数+站号+功能码+后面字节数+地址加设置字节数+...+CRC校验
            List<byte> cmdOutLst = new List<byte>();
            byte[] byteHeader = getRandom4Byte();

            //地址变换
            for (int i = 0; i < addr.Count(); i++)
            {
                cmdOutLst.AddRange(GetAbsAddr(addr[i], addressid[i], 4));
                cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(value[i].Count()));
                cmdOutLst.AddRange(value[i]);

            }

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);

            cmdOutLst.InsertRange(0, Constant.DTTcpFunctionWriteByteCmd);
            cmdOutLst.InsertRange(0, Constant.DTDeviceId);

            cmdOutLst.AddRange(ConstantMethod.CRC16_C(cmdOutLst.ToArray()));

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);


            
            cmdOutLst.InsertRange(0, byteHeader);
            //返回数据打包 

           // for (int i = 0; i < count.Count(); i++)
            //{
              //  for (int j = 0; j < count[i]; j++)
              //  {
                    cmdInLst.Add(0x00);
                    cmdInLst.Add(0x00);
            // }

            // }
            // DTTcpCmdPackAndDataUnpack.InserCount(cmdInLst, 0);
            cmdInLst.InsertRange(0, Constant.DTTcpFunctionWriteByteCmd);
            cmdInLst.InsertRange(0, Constant.DTDeviceId);
            DTTcpCmdPackAndDataUnpack.InserCount(cmdInLst, 0);

            cmdInLst.InsertRange(0, byteHeader);




            return cmdOutLst.ToArray();

        }
      
        /// <summary>
        /// 报文结构：    
        /// </summary>
        /// <param name="addr"></param>
        /// <param name="addressid"></param>
        /// <param name="count"></param>
        /// <returns></returns>
        public static byte[] PackWriteBitCmd(int[] addr, int[] addressid, int[] count, List<byte[]> value,List<byte> cmdInLst)
        {
            if (addressid.Count() != addr.Count() || count.Count() != addr.Count() || value.Count() != addr.Count())
            {
                return null;
            }

            //协议号+后面的字节数+站号+功能码+后面字节数+地址加设置字节数+...+CRC校验
            List<byte> cmdOutLst = new List<byte>();
            byte[] byteHeader = getRandom4Byte();

            //地址变换  位的话 个数 和 字节数 不一定相等 8个位才算一个字节
            for (int i = 0; i < addr.Count(); i++)
            {
                cmdOutLst.AddRange(GetAbsAddr(addr[i], addressid[i], 4));
                cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(count[i]));  //设置多少个位
                int cnt = (int)Math.Ceiling((double)count[i] / 8);
                if (value[i].Count() != cnt) return null;
                cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(value[i].Count()));  //后面有几个字节
                cmdOutLst.AddRange(value[i]);  

            }

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);

            cmdOutLst.InsertRange(0, Constant.DTTcpFunctionWriteBitCmd);
            cmdOutLst.InsertRange(0, Constant.DTDeviceId);

            cmdOutLst.AddRange(ConstantMethod.CRC16_C(cmdOutLst.ToArray()));

            DTTcpCmdPackAndDataUnpack.InserCount(cmdOutLst, 0);

            cmdOutLst.InsertRange(0, byteHeader);

            //返回的数据
            cmdInLst.Add(0x00);
            cmdInLst.Add(0x00);
            cmdInLst.InsertRange(0, Constant.DTTcpFunctionWriteBitCmd);
            cmdInLst.InsertRange(0, Constant.DTDeviceId);
            DTTcpCmdPackAndDataUnpack.InserCount(cmdInLst, 0);

            cmdInLst.InsertRange(0, byteHeader);

            return cmdOutLst.ToArray();

        }

    }
    public class DTModbusTcpCmdPackAndDataUnpack
    {
        
        /*****
        0x02 读位装置寄存器（一次最多可以读256 个 位（bit）的数据）的值。 %IX、%QX
        0x03 读单个或多个字装置寄存器（一次最多可读100 个字（Word）的数据）的值。%IW、%QW、%MW
        0x05 写单个位装置位寄存器的值。 % QX
        0x06 写单个字装置寄存器的值。 %QW、%MW
        0x0F 写多个位装置寄存器（一次最多可以写256 个位（bit）的数据）的值。% QX
        0x10 写多个字装置寄存器的值（一次最多可写100 个字（Word）的数据）。 %QW、%MW
        ******/
        
        //命令有很多种 但是 每次出去只能有一种 回来也只能有一种

        //02 读位装置寄存器 用于 %IX、%QX
        public static byte[] PackReadBitCmd(int addr, int addressid,int count)
        {
            List<byte> cmdOutLst = new List<byte>();

            List<byte> cmdInLst = new List<byte>(); //反馈可变


            cmdOutLst.AddRange(PackFunctionCode(Constant.ReadBitCmdFunctionCode,addr,addressid));

                     
            cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(count));

         
            cmdOutLst.InsertRange(4, ConstantMethod.getDataHighLowByte(cmdOutLst.Count - 4));


            //返回的数据框架填充
            cmdInLst.AddRange(PackFunctionCode(Constant.ReadBitCmdFunctionCode));
           
            int bytecount = (int)Math.Ceiling((double)count / 8);
            cmdInLst.Add((byte)bytecount);

            for (int i = 0; i < bytecount; i++)
            {
                cmdInLst.Add(0); 
            }

            cmdInLst.InsertRange(4, ConstantMethod.getDataHighLowByte(cmdInLst.Count - 4));




            return cmdOutLst.ToArray();



        }

        //03 读单个或多个字装置寄存器 %IW、%QW、%MW
        public static byte[] PackReadDCmd(int addr, int addressid,int count)
        {

            List<byte> cmdOutLst = new List<byte>();
            List<byte> cmdInLst = new List<byte>(); //反馈可变


            cmdOutLst.AddRange(PackFunctionCode(Constant.ReadDCmdFunctionCode,addr, addressid));         

            cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(count));
            cmdOutLst.InsertRange(4, ConstantMethod.getDataHighLowByte(cmdOutLst.Count - 4));

            //返回的数据框架填充
            cmdInLst.AddRange(PackFunctionCode(Constant.ReadDCmdFunctionCode));

            cmdInLst.Add((byte)(count*2));

            for (int i = 0; i < count; i++)
            {
                cmdInLst.Add(0);
                cmdInLst.Add(0);
            }
            cmdInLst.InsertRange(4, ConstantMethod.getDataHighLowByte(cmdInLst.Count - 4));

            return cmdOutLst.ToArray();

        }

        //0x05 %QX
        public static byte[] PackWriteSingleBitCmd(int address, int value)
        {
            List<byte> cmdLst = new List<byte>();         
         
            return cmdLst.ToArray();
        }
        public static byte[] PackFunctionCode(byte functionCode)
        {
            List<byte> cmdLst = new List<byte>();

            cmdLst.AddRange(Constant.DTModbusTcpHeader);

            cmdLst.AddRange(Constant.DTDeviceId);//站号

            cmdLst.Add(functionCode);//功能码 

            return cmdLst.ToArray();
        }

        public static byte[] GetAbsAddr(int addr, int addressid)
        {
            List<byte> cmdLst = new List<byte>();

            switch (addressid)
            {

                case Constant.IWAddrId:
                    {
                        addr = addr + Constant.IWMODBUSAddr;
                        break;
                    }
                case Constant.QWAddrId:
                    {
                        addr = addr + Constant.QWMODBUSAddr;
                        break;
                    }
                case Constant.MWAddrId:
                    {
                        addr = addr + Constant.MWMODBUSAddr;
                        break;
                    }
                case Constant.IXAddrId:
                    {
                        addr = addr + Constant.IXMODBUSAddr;
                        break;
                    }
                case Constant.QXAddrId:
                    {
                        addr = addr + Constant.QXMODBUSAddr;
                        break;
                    }

            }

            cmdLst.AddRange(ConstantMethod.getDataHighLowByte(addr));


            return cmdLst.ToArray();

        }
        public static byte[] PackFunctionCode(byte functionCode,int addr,int addressid)
        {
            List<byte> cmdLst = new List<byte>();

            cmdLst.AddRange(Constant.DTModbusTcpHeader);

            cmdLst.AddRange(Constant.DTDeviceId);//站号
            cmdLst.Add(functionCode);//功能码 

            

            cmdLst.AddRange(GetAbsAddr(addr, addressid));


            return cmdLst.ToArray();
        }
        //0x06  %QW、%MW
        public static byte[] PackWriteSingleDCmd(int addr, int value, int addressid)
        {
            List<byte> cmdOutLst = new List<byte>();
            List<byte> cmdInLst = new List<byte>(); //反馈固定
            if (addr < 0) return null;

            if (value < 0) return null;

            cmdOutLst.AddRange(PackFunctionCode(Constant.WriteSingleDCmdFunctionCode,addr,addressid));

          //  cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(addr));

            if(value>0)
            cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(value));
            else
            cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(0));

            cmdOutLst.InsertRange(4, ConstantMethod.getDataHighLowByte(cmdOutLst.Count - 4));

            cmdInLst = cmdOutLst;

            return cmdOutLst.ToArray();
        }

        //0x0F % QX
        public static byte[] PackSetMultipleBitCmd(int addr, int count)
        {
            List<byte> cmdLst = new List<byte>();

            return cmdLst.ToArray();
        }

        //0x10 %QW、%MW
        public static byte[] PackSetMultipleDCmd(int addr,int addressid ,int[] value)
        {
            List<byte> cmdOutLst = new List<byte>();

            List<byte> cmdInLst = new List<byte>(); //反馈固定

            if (addr < 0) return null;

            if (value.Count() == 0) return null;

            cmdOutLst.AddRange(PackFunctionCode(Constant.WriteMultipleDCmdFunctionCode, addr, addressid));

            cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(value.Count()));

            cmdOutLst.Add((byte)(value.Count()*2));

            foreach (int v in value)
            {
                cmdOutLst.AddRange(ConstantMethod.getDataHighLowByte(v));
            }

            cmdOutLst.InsertRange(4, ConstantMethod.getDataHighLowByte(cmdOutLst.Count - 4));

            
            //返回的数据框架填充
            cmdInLst.AddRange(PackFunctionCode(Constant.WriteMultipleDCmdFunctionCode));

            cmdInLst.AddRange(GetAbsAddr(addr,addressid));


            cmdInLst.AddRange(ConstantMethod.getDataHighLowByte(value.Length));

           
            cmdInLst.InsertRange(4, ConstantMethod.getDataHighLowByte(cmdInLst.Count - 4));

            return cmdOutLst.ToArray();

        }

    }
}

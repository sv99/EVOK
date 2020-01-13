using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
namespace xjplc
{
   
    public class DTPLCPackCmdAndDataUnpack
    {
         
        public DTPLCPackCmdAndDataUnpack()
        {        

        }
        int connectMode = 0;
        public int ConnectMode
        {
            get { return connectMode; }
            set { connectMode = value; }
        }
        //命令有很多种 但是 每次出去只能有一种 回来也只能有一种
        public byte[] CmdOut = null;
        public byte[] CmdIn = null;
        //在写命令时 回来的数据是可以固定判定的
        //信捷专用的码 要读取哪些数据 发这个命令 回复的命令 在打包函数打包好
        public byte[] CmdReadDMDataOut = null;

        //台达专用的码 要读取哪些数据 发这个命令 回复的命令 在打包函数打包好
        public byte[] CmdSetReadMDataOut = null;

        //台达专用的码 要读取哪些数据 发这个命令 回复的命令 在打包函数打包好
        public byte[] CmdSetReadDDataOut = null;

        //台达专用的码 设置命令后 返回这个
        public byte[] CmdSetReadMDataIn = null;

        //台达专用的码 设置命令后 返回这个
        public byte[] CmdSetReadDDataIn = null;

        //台达专用的码 要读取哪些数据 发这个命令 回复的命令 在打包函数打包好
        public byte[] CmdReadMDataOut = null;

        //台达专用的码 要读取哪些数据 发这个命令 回复的命令 在打包函数打包好
        public byte[] CmdReadDDataOut = null;

        //写B区 和D 区的命令 缓冲区 不然直接操作cmdout 会出错 矛盾
        public byte[] CmdSetBDREGOut = null;
        public byte[] CmdSetBDREGIn = null;
        //信捷专用的码 写B区 多个 从pack0f命令演变过来
        public byte[] CmdSetBREGOut = null;

        //信捷专用的码 写B区回复的码
        public byte[] CmSetBREGIn = new byte[7];

        //信捷专用的码 写D区 多个 从pack10命令演变过来
        public byte[] CmdSetDREGOut = null;

        //信捷专用的码 写D区回复的码
         public byte[] CmSetDREGIn = new byte[7];

        //发送读取命令后 需要返回数据的长度 名称需要改
      public int ReceivedDMDataCount = 0;

     //台达 D M区分开
      public int ReceivedMDataCount = 0;

      public int ReceivedDDataCount = 0;

      public int[] UnPackCmdReadDMDataIn(DataTable datform,byte[] m_buffer,List<DTPlcInfo> dplcInfoLst, List<List<DTPlcInfo>> mplcInfoLst)
      {

            List<int> UpDateRow = new List<int>();
            if (dplcInfoLst == null && mplcInfoLst == null)
            {
                UpDateRow.Add(-1);
                return UpDateRow.ToArray();
            }
            if (m_buffer.Count() < 1)
            {
                UpDateRow.Add(-2);
                return UpDateRow.ToArray();
            }

            //完整性检查
            if (m_buffer.Count() < 4)
            {
                UpDateRow.Add(-3);
                return UpDateRow.ToArray();
            }
            if (!(m_buffer[0] == 0x01 && m_buffer[1] == 0x19))
            {
                UpDateRow.Add(-4);
                return UpDateRow.ToArray();
            }
            int dCount = dplcInfoLst.Count;

            int mCount = 0;
            for (int i = 0; i < mplcInfoLst.Count; i++)
            {
                double cntdb = (double)mplcInfoLst[i].Count / 8;
                mCount = mCount +
                    (int)Math.Ceiling(cntdb);
            }
            byte[] dArea_buffer;
            byte[] mArea_buffer;
            if ((dCount * 2) < m_buffer.Count())
                dArea_buffer = m_buffer.Skip(3).Take(dCount * 2).ToArray();
            else
            {
                UpDateRow.Add(-5);
                return UpDateRow.ToArray();
            }
            if ((mCount ) < m_buffer.Count())
                mArea_buffer = m_buffer.Skip(m_buffer.Count()- mCount - 2).Take(mCount).ToArray();
            else
            {
                UpDateRow.Add(-6);
                return UpDateRow.ToArray();
            };//b.Skip(b.Count() - 8).ToArray(); 
            //分类读取 D区在前 按序号读取

            if (dCount>0)
            for (int i = 0; i < dCount; i++)
            {
                  if(dplcInfoLst[i].Row>= datform.Rows.Count) continue;

                   if (dplcInfoLst[i].IntArea < (Constant.M_ID))
                    {
                        if ((i * 2) < dArea_buffer.Count() && (i * 2 + 1) < dArea_buffer.Count())
                        {                       
                            dplcInfoLst[i].ByteValue[0] = dArea_buffer[i * 2];
                            dplcInfoLst[i].ByteValue[1] = dArea_buffer[i * 2 + 1];
                            //更新监控表格数据
                           
                            if (!datform.Rows[dplcInfoLst[i].Row]["value"].ToString().Equals(dplcInfoLst[i].PlcValue.ToString()) 
                                && !dplcInfoLst[i].IsInEdit )
                            {
                                if (datform.Rows[dplcInfoLst[i].Row]["addr"].ToString().Contains(dplcInfoLst[i].RelAddr.ToString())
                                    && datform.Rows[dplcInfoLst[i].Row]["addr"].ToString().Contains(dplcInfoLst[i].StrArea)
                                    )
                                {                                
                                    string s = dplcInfoLst[i].PlcValue.ToString();
                                    datform.Rows[dplcInfoLst[i].Row]["value"] = s;
                                    UpDateRow.Add(dplcInfoLst[i].Row);
                                                                       
                                }
                            }                         
                        }                   
                    }
             }          

            if ((mCount > 0) && (mCount == mArea_buffer.Count()))
            {
                int mArea_Buffer_Id = 0;
                int mArea_Buffer_Id_Old = 0;
                for (int i = 0; i < mCount; i++)
                {
                  
                    //这里解释下为什么要这样，
                    mArea_Buffer_Id_Old = mArea_Buffer_Id;
                    mArea_Buffer_Id = 0;
                    if (i< mplcInfoLst.Count)
                    for (int m = 0; m < mplcInfoLst[i].Count; m++)
                    {
                        if (mplcInfoLst[i][m].Row >= datform.Rows.Count) continue;

                            mArea_Buffer_Id = mArea_Buffer_Id_Old+ m / 8;
                        if (mArea_Buffer_Id < mArea_buffer.Count())
                        {
                                mplcInfoLst[i][m].ByteValue[0] = mArea_buffer[mArea_Buffer_Id];
                                mplcInfoLst[i][m].Xuhao = m % 8;
                        }
                      
                        if (!datform.Rows[mplcInfoLst[i][m].Row]["value"].ToString().Equals(mplcInfoLst[i][m].PlcValue.ToString())
                                && !mplcInfoLst[i][m].IsInEdit)
                        {
                                string m10addr=mplcInfoLst[i][m].RelAddr.ToString();
                                if (mplcInfoLst[i][m].IntArea > Constant.HM_ID)
                                m10addr = ConstantMethod.GetXYAddr10To8(mplcInfoLst[i][m].RelAddr).ToString();

                                if (datform.Rows[mplcInfoLst[i][m].Row]["addr"].ToString().Contains(m10addr)
                                    &&
                                    datform.Rows[mplcInfoLst[i][m].Row]["addr"].ToString().Contains(mplcInfoLst[i][m].StrArea)
                                    )
                                {
                                    datform.Rows[mplcInfoLst[i][m].Row]["value"] = mplcInfoLst[i][m].PlcValue.ToString();
                                    UpDateRow.Add(mplcInfoLst[i][m].Row);
                                }                              
                        }
                       
                        }
                    mArea_Buffer_Id++;
                }
                                              
            }
            
            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            return UpDateRow.ToArray();

        }
        public int[] UnPackCmdReadMDataIn(DataTable datform, byte[] m_buffer, List<List<DTPlcInfo>> mplcInfoLst)
        {

            List<int> UpDateRow = new List<int>();
            if (mplcInfoLst == null)
            {
                UpDateRow.Add(-1);
                return UpDateRow.ToArray();
            }
            if (m_buffer.Count() < 1)
            {
                UpDateRow.Add(-2);
                return UpDateRow.ToArray();
            }

            //完整性检查
            if (m_buffer.Count() < 4)
            {
                UpDateRow.Add(-3);
                return UpDateRow.ToArray();
            }
            //台达专用
            if (!(m_buffer[0] == 0x01 && m_buffer[1] == 0x03))
            {
                UpDateRow.Add(-4);
                return UpDateRow.ToArray();
            }
            //统计个数
            int mCount = 0;
            List<DTPlcInfo> mpLst = new List<DTPlcInfo>();
            for (int i = 0; i < mplcInfoLst.Count; i++)
            {         
                mpLst.AddRange(mplcInfoLst[i]);
            }
            mCount = mpLst.Count;
            mCount = (int)Math.Ceiling((double)mCount / 16);
            mCount = mCount * 2;
            byte[] mArea_buffer= new byte[mCount];
            
            Array.Copy(m_buffer,3, mArea_buffer,0, mCount);
           
            if ((mCount > 0) &&(mArea_buffer !=null)&& (mCount == mArea_buffer.Count()))
            {
                int s = 0;                                       
                        for (int m = 0; m < mpLst.Count; m++)
                        {
                          if (m > (8 * (s + 1)-1)) s++; //8个为一个字节                   
                           
                          mpLst[m].ByteValue[0] = mArea_buffer[s];
                          mpLst[m].Xuhao = m % 8;   // 8个一组 进行解析                                                  

                           if (mpLst[m].Row >= datform.Rows.Count) continue;
                            //更新表格数据 不相等的 就更新下
                            if (!datform.Rows[mpLst[m].Row]["value"].ToString().Equals(mpLst[m].PlcValue.ToString())
                                    && !mpLst[m].IsInEdit)
                            {
                                string m10addr = mpLst[m].RelAddr.ToString();
                                if (mpLst[m].IntArea > Constant.HM_ID)
                                    m10addr = ConstantMethod.GetXYAddr10To8(mpLst[m].RelAddr).ToString();

                                if (datform.Rows[mpLst[m].Row]["addr"].ToString().Contains(m10addr)
                                    &&
                                    datform.Rows[mpLst[m].Row]["addr"].ToString().Contains(mpLst[m].StrArea)
                                    )
                                {
                                    datform.Rows[mpLst[m].Row]["value"] = mpLst[m].PlcValue.ToString();
                                    UpDateRow.Add(mpLst[m].Row);
                                }
                            }

                        }
                

            }

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            return UpDateRow.ToArray();

        }
        public int[] UnPackCmdReadDDataIn(DataTable datform, byte[] m_buffer, List<DTPlcInfo> dplcInfoLst)
        {
           
            List<int> UpDateRow = new List<int>();
            if (dplcInfoLst == null )
            {
                UpDateRow.Add(-1);
                return UpDateRow.ToArray();
            }
            if (m_buffer.Count() < 1)
            {
                UpDateRow.Add(-2);
                return UpDateRow.ToArray();
            }

            //完整性检查
            if (m_buffer.Count() < 4)
            {
                UpDateRow.Add(-3);
                return UpDateRow.ToArray();
            }
            if (!(m_buffer[0] == 0x01 && m_buffer[1] == 0x03))
            {
                UpDateRow.Add(-4);
                return UpDateRow.ToArray();
            }
            int dCount = dplcInfoLst.Count;

       
           
            byte[] dArea_buffer;
           
            if ((dCount * 4) < m_buffer.Count())
                dArea_buffer = m_buffer.Skip(3).Take(dCount * 4).ToArray();
            else
            {
                UpDateRow.Add(-5);
                return UpDateRow.ToArray();
            }
            
            //分类读取 D区在前 按序号读取

            if (dCount > 0)
                for (int i = 0; i < dCount; i++)
                {

                    if (dplcInfoLst[i].Row >= datform.Rows.Count) continue;

                    if (dplcInfoLst[i].IntArea < (Constant.M_ID))
                    {
                        if (
                            (i * 4) < dArea_buffer.Count() && 
                            (i * 4 + 1) < dArea_buffer.Count() &&
                            (i * 4 + 2) < dArea_buffer.Count()&&
                            (i * 4 + 3) < dArea_buffer.Count()
                            )
                        {
                            dplcInfoLst[i].ByteValue[0] = dArea_buffer[i*4];
                            dplcInfoLst[i].ByteValue[1] = dArea_buffer[i*4 + 1];
                            dplcInfoLst[i].DoubleModeHigh.ByteValue[0] = dArea_buffer[i*4 +2];
                            dplcInfoLst[i].DoubleModeHigh.ByteValue[1] = dArea_buffer[i * 4 + 3];

                            //更新监控表格数据
                            if (!datform.Rows[dplcInfoLst[i].Row]["value"].ToString().Equals(dplcInfoLst[i].PlcValue.ToString())
                                && !dplcInfoLst[i].IsInEdit)
                            {
                                if (datform.Rows[dplcInfoLst[i].Row]["addr"].ToString().Contains(dplcInfoLst[i].RelAddr.ToString())
                                    && datform.Rows[dplcInfoLst[i].Row]["addr"].ToString().Contains(dplcInfoLst[i].StrArea)                                  
                                    )
                                {
                                    string s = dplcInfoLst[i].PlcValue.ToString();

                                    datform.Rows[dplcInfoLst[i].Row]["value"] = s;

                                    double valueDouble;

                                    if (double.TryParse(s, out valueDouble))
                                    {
                                        valueDouble = valueDouble / Constant.dataMultiple;

                                        datform.Rows[dplcInfoLst[i].Row]["param6"] = valueDouble.ToString();
                                    }

                                    UpDateRow.Add(dplcInfoLst[i].Row);

                                }
                            }
                        }
                    }
                }            

            //GC.Collect();
            //GC.WaitForPendingFinalizers();
            return UpDateRow.ToArray();

        }

        public int PackCmSetDREGIn(int Addr, int count, string Area)
        {
            int ByteLen = count;
           
            int addr_high = (Addr & 0xFF00) >> 8;
            int addr_low = Addr & 0xFF;
            int count_high = (count & 0xFF00) >> 8;
            int count_low = count & 0xFF;
            //站号 命令码
            CmSetDREGIn[0] = 0x01;
            CmSetDREGIn[1] = 0x10;
            //地址 
            CmSetDREGIn[2] = (byte)addr_high;
            CmSetDREGIn[3] = (byte)addr_low;
            //个数
            CmSetDREGIn[4] = (byte)count_high;
            CmSetDREGIn[5] = (byte)count_low;
            //字节数

            //拷贝数组传入CRC校验
            byte[] byteCrcData = new byte[6];
            Array.Copy(CmSetDREGIn, 0, byteCrcData, 0, 6);

            //crc
            CmSetDREGIn[6] = LRC16_C(byteCrcData);
           

            CmdSetBDREGIn = CmSetDREGIn;

            return 0;
        }
        public int PackCmSetBREGIn(int addr, int count, string Area)
        {
            int ByteLen = count;
            
            int addr_high = (addr & 0xFF00) >> 8;
            int addr_low = addr & 0xFF;
            int count_high = (count & 0xFF00) >> 8;
            int count_low = count & 0xFF;
            //站号 命令码
            CmSetBREGIn[0] = 0x01;
            CmSetBREGIn[1] = 0x0f;
            //地址 
            CmSetBREGIn[2] = (byte)addr_high;
            CmSetBREGIn[3] = (byte)addr_low;
            //个数
            CmSetBREGIn[4] = (byte)count_high;
            CmSetBREGIn[5] = (byte)count_low;
            //字节数

            //拷贝数组传入CRC校验
            byte[] byteCrcData = new byte[6];
            Array.Copy(CmSetBREGIn, 0, byteCrcData, 0, 6);

            //crc
            CmSetBREGIn[6] = LRC16_C(byteCrcData);
        

            CmdSetBDREGIn = CmSetBREGIn;

            return 0;
        }
        public int PackCmdSetBREGOut(int addr, int count, int[] value, string XYM)
        {
         
            switch (XYM)
            {
                case "X":
                    {
                        addr = ConstantMethod.GetXYAddr8To10(addr);
                        addr = addr + Constant.Delta_X_addr;                                            
                        break;
                    }
                case "Y":
                    {
                        addr = ConstantMethod.GetXYAddr8To10(addr);
                        addr = addr + Constant.Delta_Y_addr;
                        break;
                    }
                case "M":
                    {
                        addr = addr + Constant.Delta_M_addr;
                        break;
                    }               
                default:
                    {
                        addr = addr + Constant.Delta_M_addr;
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

            CmdSetBREGOut = new byte[8 + ByteLen];
            int addr_high = (addr & 0xFF00) >> 8;
            int addr_low = addr & 0xFF;
            int count_high = (count & 0xFF00) >> 8;
            int count_low = count & 0xFF;
            //站号 命令码
            CmdSetBREGOut[0] = 0x01;
            CmdSetBREGOut[1] = 0x0F;
            //地址 
            CmdSetBREGOut[2] = (byte)addr_high;
            CmdSetBREGOut[3] = (byte)addr_low;
            //个数
            CmdSetBREGOut[4] = (byte)count_high;
            CmdSetBREGOut[5] = (byte)count_low;
            //字节数
            CmdSetBREGOut[6] = (byte)ByteLen;
            //值 (低位在前 高位在后） 先取低8位
            for (int i = 0; i < ByteLen; i++)
            {
                CmdSetBREGOut[7 + i] = (byte)(value[i] & 0xFF);
            }

            //拷贝数组传入CRC校验
            byte[] byteCrcData = new byte[7 + ByteLen];
            Array.Copy(CmdSetBREGOut, 0, byteCrcData, 0, 7 + ByteLen);

            //crc
            CmdSetBREGOut[7 + ByteLen] = LRC16_C(byteCrcData);
            
            PackCmSetBREGIn(addr, count, XYM);

            CmdSetBDREGOut = CmdSetBREGOut;

            return 0;
        }
        
        public int PackCmdSetDREGOut(int Addr, int count, int[] value, string Area)
        {
            int ByteLen = count ;
            CmdSetDREGOut = new byte[8 + ByteLen*2];
            if (Area == "D")
            {
                Addr += Constant.Delta_D_addr;
            }
            int addr_high = (Addr & 0xFF00) >> 8;
            int addr_low = Addr & 0xFF;
            int count_high = (count & 0xFF00) >> 8;
            int count_low = count & 0xFF;
            //站号 命令码
            CmdSetDREGOut[0] = 0x01;
            CmdSetDREGOut[1] = 0x10;
            //地址 
            CmdSetDREGOut[2] = (byte)addr_high;
            CmdSetDREGOut[3] = (byte)addr_low;
            //个数
            CmdSetDREGOut[4] = (byte)count_high;
            CmdSetDREGOut[5] = (byte)count_low;
            //字节数
            CmdSetDREGOut[6] = (byte)(ByteLen*2);
            //值 (低位在前 高位在后） 低位在前 高位在后
            for (int i = 0; i < ByteLen*2; i = i + 2)
            {
                CmdSetDREGOut[8 + i] = (byte)(value[i / 2] & 0xFF);

                CmdSetDREGOut[7 + i] = (byte)((value[i / 2] >> 8) & 0xFF);
            }

            //拷贝数组传入CRC校验
            byte[] byteCrcData = new byte[7 + ByteLen*2];
            Array.Copy(CmdSetDREGOut, 0, byteCrcData, 0, 7 + ByteLen*2);

            //CRC
            CmdSetDREGOut[7 + ByteLen*2] = LRC16_C(byteCrcData);
            
            PackCmSetDREGIn(Addr, count, Area);

            CmdSetBDREGOut = CmdSetDREGOut;

            return 0;
        }    
       
        /// <summary>
        /// 通过字母获取寄存器所在区域ID
        /// </summary>
        /// <param name="XYM"></param>
        /// <returns></returns>
        public static int AreaGetFromStr(string XYM)
      {
            //去个最大值
            int addr = -1;
            switch (XYM)
            {

                case "X":
                    {
                        addr = Constant.X_ID;
                        
                        break;
                    }
                case "Y":
                    {
                        addr = Constant.Y_ID;
                        
                        break;
                    }
                case "M":
                    {
                        addr = Constant.M_ID;
                        
                        break;
                    }
                case "HM":
                    {
                        addr = Constant.HM_ID;

                        break;
                    }
                case "HD":
                    {
                        addr = Constant.HD_ID;

                        break;
                    }
                case "D":
                    {
                        addr = Constant.D_ID;

                        break;
                    }
                case "HSD":
                    {
                        addr = Constant.HSD_ID;

                        break;
                    }
                default:
                    {
                        addr = -1;
                        break;
                    }

            }
            return addr;
        }
        /// <summary>
        /// 通过字母获取寄存器所在区域绝对地址
        /// 传入相对地址
        /// </summary>
        /// <param name="XYM"></param>
        /// <returns></returns>
        public static int AreaGetFromStr(int maddr,string XYM)
        {
            //去个最大值
            int addr = maddr;
            switch (XYM)
            {

                case "X":
                    {                                                                 
                        addr = addr + Constant.Delta_X_addr;

                        break;
                    }
                case "Y":
                    {
                       
                        addr = addr + Constant.Delta_Y_addr;
              
                        break;
                    }
                case "M":
                    {
                        addr = addr + Constant.Delta_M_addr;
                      
                        break;
                    }               
                
                case "D":
                    {
                        addr = addr + Constant.Delta_D_addr;
                       
                        break;
                    }
                
                default:
                    {
                        addr = addr + Constant.Delta_M_addr;
                      
                        break;
                    }

            }
            return addr;
        }
        /// <summary>
        /// 通过区域整型数据 相对地址获取寄存器所在区域绝对地址
        /// </summary>
        /// <param name="XYM"></param>
        /// <returns></returns>
        public static int AreaGetFromStr(int maddr, int XYM)
        {
            //去个最大值
            int addr = maddr;
            switch (XYM)
            {

                case Constant.X_ID:
                    {
                        addr = addr + Constant.Delta_X_addr;

                        break;
                    }
                case Constant.Y_ID:
                    {
                        addr = addr + Constant.Delta_Y_addr;

                        break;
                    }
                case Constant.M_ID:
                    {
                        addr = addr + Constant.Delta_M_addr;

                        break;
                    }
               
                case Constant.D_ID:
                    {
                        addr = addr + Constant.Delta_D_addr;

                        break;
                    }
         
                default:
                    {
                        addr = addr + Constant.Delta_M_addr;

                        break;
                    }

            }
            return addr;
        }

        /// <summary>
        /// 通过绝对地址，获取寄存器所在区域相对
        /// </summary>
        /// <param name="XYM"></param>
        /// <returns></returns>
        public static int RelAbsGet(int maddr, int XYM)
        {
            //去个最大值
            int addr = maddr;
            switch (XYM)
            {

                case Constant.X_ID:
                    {
                        addr = addr - Constant.Delta_X_addr;

                        break;
                    }
                case Constant.Y_ID:
                    {
                        addr = addr - Constant.Delta_Y_addr;

                        break;
                    }
                case Constant.M_ID:
                    {
                        addr = addr - Constant.Delta_M_addr;

                        break;
                    }
            
                case Constant.D_ID:
                    {
                        addr = addr - Constant.Delta_D_addr;

                        break;
                    }
                
                default:
                    {
                        addr = addr + Constant.M_addr;

                        break;
                    }

            }
            return addr;
        }
        /// <summary>
        /// XY 地址 8进制转换成十进制 进行类建立
        /// </summary>
        /// <param name="addr"></param>
        /// <returns></returns>
     

        public DTPlcInfo[] GetPlcInfo(int addr, int count ,string XYM,string mode)
        {
            List<DTPlcInfo> plcinforlst = new List<DTPlcInfo>();
            int addrreal=addr;
            if (DTPLCPackCmdAndDataUnpack.AreaGetFromStr(XYM.Trim()) > Constant.HM_ID)
            {
                addrreal = ConstantMethod.GetXYAddr8To10(addr);
            }
            for (int i = 0; i < count; i++)
            {
                DTPlcInfo tmpInfo = new DTPlcInfo(addrreal, XYM.Trim(),mode);                          
                tmpInfo.Xuhao = -1;
                plcinforlst.Add(tmpInfo);
                addrreal++;

            }
            

            
            return plcinforlst.ToArray();

        }


        public int PackSetCmdReadMDataOut(List<List<DTPlcInfo>> addrLst)
        {
            List<DTPlcInfo> mplcLst = new List<DTPlcInfo>();
            byte[] DTCmdSetReadMDataOutTemp;
            byte[] DTCmdReadMDataOutTemp;
            DTCmdSetReadMDataOutTemp= Constant.DTCmdSetReadMDataOut232;
            DTCmdReadMDataOutTemp= Constant.DTCmdReadMDataOut232;

            if (ConnectMode == Constant.TaiDaConnectMode485)
            {
                DTCmdSetReadMDataOutTemp = Constant.DTCmdSetReadMDataOut485;
                DTCmdReadMDataOutTemp = Constant.DTCmdReadMDataOut485;
            }


            for (int i = 0; i < addrLst.Count; i++)
            {
                mplcLst.AddRange(addrLst[i]);
            }

            if (!(mplcLst.Count > 0))
            {
                CmdSetReadMDataOut = null;
                CmdSetReadMDataIn = null;
                CmdReadMDataOut = null;
    
                return -1;

            }

            List<byte> cmdByte = new List<byte>();
            for (int i = 0; i < mplcLst.Count; i++)
            {
                int addr_high = (mplcLst[i].AbsAddr & 0xFF00) >> 8;
                int addr_low = mplcLst[i].AbsAddr & 0xFF;
                cmdByte.Add((byte)addr_high);
                cmdByte.Add((byte)addr_low);
            }

            List<byte> byteLst = new List<byte>();
            List<byte> byteLstIn = new List<byte>();
            List<byte> byteLstOut = new List<byte>();
            byteLst.Add(0x01);
            byteLst.Add(0x10);

            byteLst.AddRange(DTCmdSetReadMDataOutTemp);

            byteLstIn.Add(0x01);
            byteLstIn.Add(0x10);
            byteLstIn.AddRange(DTCmdSetReadMDataOutTemp);

            byteLstOut.Add(0x01);
            byteLstOut.Add(0x03);

            byteLstOut.AddRange(DTCmdReadMDataOutTemp);


            int addrcount_high = ((mplcLst.Count + 1) & 0xFF00) >> 8;
            int addrcount_low = (mplcLst.Count + 1) & 0xFF;
            
            int c = (int)Math.Ceiling((double)mplcLst.Count/16); //多少个16位的数据 至少是16位 
            int addrcount_high0 = ((c) & 0xFF00) >> 8;
            int addrcount_low0 = (c) & 0xFF;

            int count = (mplcLst.Count + 1) * 2;

            byteLst.Add((byte)addrcount_high);
            byteLst.Add((byte)addrcount_low);
            byteLstIn.Add((byte)addrcount_high);
            byteLstIn.Add((byte)addrcount_low);

            byteLstOut.Add((byte)addrcount_high0);
            byteLstOut.Add((byte)addrcount_low0);
            byteLst.Add((byte)((mplcLst.Count + 1) * 2));

            addrcount_high = ((mplcLst.Count) & 0xFF00) >> 8;
            addrcount_low = (mplcLst.Count) & 0xFF;

            byteLst.Add((byte)addrcount_high);
            byteLst.Add((byte)addrcount_low);

            byteLst.AddRange(cmdByte);

            byteLst.Add(DTPLCPackCmdAndDataUnpack.LRC16_C(byteLst.ToArray()));
            byteLstIn.Add(DTPLCPackCmdAndDataUnpack.LRC16_C(byteLstIn.ToArray()));
            byteLstOut.Add(DTPLCPackCmdAndDataUnpack.LRC16_C(byteLstOut.ToArray()));
            //开始拼接 台达设置读取D区命令
            CmdSetReadMDataOut = byteLst.ToArray();

            CmdSetReadMDataIn = byteLstIn.ToArray();

            CmdReadMDataOut = byteLstOut.ToArray();
            return 0;
        }
        
        public int PackSetCmdReadDDataOut(List<DTPlcInfo> addrLst)
        {
            List<byte> byteLst = new List<byte>();
            List<byte> byteLstIn = new List<byte>();
            List<byte> byteLstOut = new List<byte>();

            byte[] DTCmdSetReadDDataOutTemp;
            byte[] DTCmdReadDDataOutTemp;

            DTCmdSetReadDDataOutTemp = Constant.DTCmdSetReadDDataOut232;
            DTCmdReadDDataOutTemp = Constant.DTCmdReadDDataOut232;
            if (ConnectMode == Constant.TaiDaConnectMode485)
            {
                DTCmdSetReadDDataOutTemp = Constant.DTCmdSetReadDDataOut485;
                DTCmdReadDDataOutTemp = Constant.DTCmdReadDDataOut485;
            }
            if (!(addrLst.Count > 0))
            {
                CmdSetReadDDataOut = null;
                CmdSetReadDDataIn  = null;
                CmdReadDDataOut    = null;
         
                return -1;
            }

            List<byte> cmdByte = new List<byte>();
            for (int i = 0; i < addrLst.Count; i++)
            {
                int addr_high = (addrLst[i].AbsAddr & 0xFF00) >> 8;
                int addr_low = addrLst[i].AbsAddr & 0xFF;

                cmdByte.Add((byte)addr_high);
                cmdByte.Add((byte)addr_low);
            }

            int addrcount_high = ((addrLst.Count+1) & 0xFF00) >> 8;
            int addrcount_low = (addrLst.Count+1) & 0xFF;

            int count = (addrLst.Count+1) * 2;
            //发送设置读取D区命令
            byteLst.Add(0x01);
            byteLst.Add(0x10);
            byteLst.AddRange(DTCmdSetReadDDataOutTemp);
          

            byteLstIn.Add(0x01);
            byteLstIn.Add(0x10);
            byteLstIn.AddRange(DTCmdSetReadDDataOutTemp);
           

            byteLstOut.Add(0x01);
            byteLstOut.Add(0x03); 
            byteLstOut.AddRange(DTCmdReadDDataOutTemp);          

            int addrcount_high0 = ((addrLst.Count ) & 0xFF00) >> 8;
            int addrcount_low0 = (addrLst.Count) & 0xFF;
            byteLstOut.Add((byte)addrcount_high0);
            byteLstOut.Add((byte)addrcount_low0);

            byteLst.Add((byte)addrcount_high);
            byteLst.Add((byte)addrcount_low);
            byteLstIn.Add((byte)addrcount_high);
            byteLstIn.Add((byte)addrcount_low);
            byteLst.Add((byte) ((addrLst.Count + 1) * 2));

            addrcount_high = ((addrLst.Count ) & 0xFF00) >> 8;
            addrcount_low = (addrLst.Count ) & 0xFF;

            byteLst.Add((byte)addrcount_high);
            byteLst.Add((byte)addrcount_low);
            byteLst.AddRange(cmdByte);


            //根据232 还是485来选择
            byteLst.Add(DTPLCPackCmdAndDataUnpack.LRC16_C(byteLst.ToArray()));
            byteLstIn.Add(DTPLCPackCmdAndDataUnpack.LRC16_C(byteLstIn.ToArray()));
            byteLstOut.Add(DTPLCPackCmdAndDataUnpack.LRC16_C(byteLstOut.ToArray()));
            //开始拼接 台达设置读取D区命令
            CmdSetReadDDataOut = byteLst.ToArray();

            CmdSetReadDDataIn = byteLstIn.ToArray();

            CmdReadDDataOut = byteLstOut.ToArray();

            return 0;


        }

        /// <summary>
        /// 1.在这里 前面已经打包过了 这里不需要再换算成绝对地址了
        /// 2.要读取哪些数据啊 就在这里打包了 
        /// 3.打包数据后一定要有返回多少的数据的个数
        /// </summary>
        /// <param name="addrLst"></param>
        /// <param name="idLst"></param>
        /// <param name="addrcount"></param>
        /// <returns></returns>
        public int PackCmdReadDMDataOut(List<int> addrLst, List<int> idLst, List<int> addrcount,int DMDataReceCount)
        {
            //验证数据
            if (addrLst.Count != idLst.Count || addrLst.Count != addrcount.Count) return -1;

            int Dcount = 0;
            int addr=0,count=0;
            List<byte> cmdByte = new List<byte>();
            for (int i = 0; i < addrLst.Count; i++)
            {

                #region 根据读取类型 确定地址
                addr = addrLst[i];
                count = addrcount[i];
                if (idLst[i] < Constant.M_ID) Dcount++;
                int addr_high = (addr & 0xFF00) >> 8;
                int addr_low = addr & 0xFF;
                int count_high = (count & 0xFF00) >> 8;
                int count_low = count & 0xFF;

                cmdByte.Add((byte)addr_high);
                cmdByte.Add((byte)addr_low);
                cmdByte.Add((byte)count_high);
                cmdByte.Add((byte)count_low);

                #endregion

            }
                //开始拼接 台达设置读取D区命令
                CmdReadDMDataOut = new byte[7 + addrLst.Count * 4];

                CmdReadDMDataOut[0] = 0x01;
                CmdReadDMDataOut[1] = 0x10;
                CmdReadDMDataOut[2] = 0x00;
                CmdReadDMDataOut[3] = (byte)addrLst.Count;
                CmdReadDMDataOut[4] = (byte)(Dcount+1);
               
                Array.Copy(cmdByte.ToArray(), 0, CmdReadDMDataOut, 5, cmdByte.Count);

                //crc
                byte[] byteCrcData = new byte[CmdReadDMDataOut.Count()-2];

                Array.Copy(CmdReadDMDataOut, 0, byteCrcData, 0, CmdReadDMDataOut.Count()-2);

                CmdReadDMDataOut[CmdReadDMDataOut.Count()-2]   = CRC16_C(byteCrcData)[1];
                CmdReadDMDataOut[CmdReadDMDataOut.Count() - 1] = CRC16_C(byteCrcData)[0];

                ReceivedDMDataCount = DMDataReceCount;


            return 0;
        }
    
            
      //判断传入的数据是否完整 根据LRC 校验判别
      public static Boolean IsEnd(Byte[] BufList)
      {

          if (BufList.Count() < 5 ) return false;

          byte[] buf = BufList;
          
          byte[] buf1 = new byte[buf.Count() - 1];//去掉末尾两个数据

          Array.Copy(buf, 0,buf1,0, buf.Count() - 1);

          byte[] buf2 = new byte[1];
          byte[] bufTest = {0x01,0x03,0x04,0x01,0x00,0x01 };
          buf2[0] = LRC16_C(buf1);
    
          if ((buf[(buf.Count() - 1)] == buf2[0]))
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
            byte sum = 0;
            foreach (byte b in data)
            {
                sum += b;
            }

            sum =(byte)(sum % 0x100);//模FF
            sum = (byte)(0x100 -sum);//取反+1

            return (byte)sum;
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

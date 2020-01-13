using Modbus.Device;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Modbus;

namespace xjplc.xj
{
    public class XjTcpManager
    {

        TcpClient client;
      
        Modbus.Device.ModbusIpMaster master;

        ServerInfo serCurrent;
        //数据区 直接进行更新
        public List<XJPlcInfo> DPlcInfo;
        //这里为什么要这样？ 因为M值不连续的分一组 再整理起来
        public List<List<XJPlcInfo>> MPlcInfoAll;

        public DataTable dataForm;

        bool isReadingData;
        public bool IsReadingData
        {
            get { return isReadingData; }
            set { isReadingData = value; }
        }
        int status = -1;
        public int Status
        {
            get { return status; }
            set { status = value; }
        }
        //定时器 更新 页面 检测通讯是否正常    
        System.Timers.Timer ErrorConnTimer = null;// new System.Timers.Timer(500);//串口连接超时次数

        //如果要切换表格说一声 就切换好
        bool isRePackCmdReadDMDataOut;
        public bool IsRePackCmdReadDMDataOut
        {
            get { return isRePackCmdReadDMDataOut; }
            set { isRePackCmdReadDMDataOut = value; }
        }
        //定时器 访问更新数据   
        System.Timers.Timer GetDataConnTimer = null;// new System.Timers.Timer(500);//串口连接超时次数

        int ErrorConnCount = 0;

        public XjTcpManager(ServerInfo ser)
        {
          
            ErrorConnTimer = new System.Timers.Timer(Constant.XJConnectTimeOut);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            ErrorConnTimer.Enabled = false;

            ErrorConnTimer.AutoReset = true;

            ErrorConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(ErrorConnTimerEvent);

            ErrorConnCount = 0;

            GetDataConnTimer = new System.Timers.Timer(Constant.XJConnectTcpTime);  //这里0.3 秒别改 加到常量里 工控机性能不行 

            GetDataConnTimer.Enabled = false;

            GetDataConnTimer.AutoReset = true;

            GetDataConnTimer.Elapsed += new System.Timers.ElapsedEventHandler(GetDataConnTimerEvent);

            serCurrent = ser;

            Reset();
        }
               
        private void ErrorConnTimerEvent(object sender, EventArgs e)
        {

            ErrorConnCount++;
            //通讯错误次数太多 就直接停了吧
            if (ErrorConnCount < Constant.ErrorConnCountMax && ErrorConnCount > 4)
            {          
                LogManager.WriteProgramLog(Constant.CommManageError);
                return;
            }
            else if (ErrorConnCount > Constant.ErrorConnCountMax)
            {
                DeviceClear();
               //清理标志位            
            }
        }

        public void DeviceClear()
        {

            ErrorConnTimer.Enabled = false;

            GetDataConnTimer.Enabled = false;

            master.Dispose();

            ErrorConnCount = 0;

            Status = Constant.DeviceNoConnection;

        }
        public void Reset()
        {

            //蓝屏了 居然 
            /***
           if(!ConstantMethod.CheckAddressPort(serCurrent))
           {
               MessageBox.Show(Constant.TcpIpError);
               return;
              //ConstantMethod.AppExit();
           }
           ***/
            try
            {

                client = new TcpClient(serCurrent.server_Ip, int.Parse(serCurrent.server_Port));

                master = ModbusIpMaster.CreateIp(client);

                Status = Constant.DeviceConnected;

                ConstantMethod.Delay(100);

                StartGetData();

            }
            catch (Exception ex)
            {
                Status = Constant.DeviceNoConnection;
                MessageBox.Show(Constant.ErrorSocConnection);
               // ConstantMethod.AppExit();
            }
        }
        #region 数据获取

        bool getDValueFromLst(List<XJPlcInfo> tDplcLst)
        {
            //这里dplcinfo 已经默认传进来 两个了
            try
            {
                /***
                if (tDplcLst[0].AbsAddr == 41674)
                {
                    int i = 0;
                }
                ***/
                ushort[] inputs = master.ReadHoldingRegisters((byte)Constant.DefaultUnitId, (ushort)tDplcLst[0].AbsAddr, (ushort)(tDplcLst.Count));
              //  ushort[] inputs = master.ReadHoldingRegisters((byte)Constant.DefaultUnitId,200,20);

                if (inputs != null && inputs.Count() == tDplcLst.Count)
                {
                    ErrorConnCount = 0;
                    for (int i = 0; i < tDplcLst.Count(); i++)
                    {
                        if (IsRePackCmdReadDMDataOut) return true;

                        byte[] a = BitConverter.GetBytes(inputs[i]);

                        tDplcLst[i].ByteValue[0] = a[1]; //低字节
                        tDplcLst[i].ByteValue[1] = a[0]; //低字节
                    }

                    for (int i = 0; i < tDplcLst.Count(); i++)
                    {
                        if (IsRePackCmdReadDMDataOut) return true; 
                                                                            

                        if (tDplcLst[i].Row >= dataForm.Rows.Count) continue;

                        string sValue = tDplcLst[i].PlcValue.ToString();

                        int iValue = tDplcLst[i].PlcValue;

                        if (!dataForm.Rows[tDplcLst[i].Row]["value"].ToString().Equals(sValue)
                        && !tDplcLst[i].IsInEdit && (tDplcLst[i].BelongToDT != null) )
                        {
                            if (dataForm.Rows[tDplcLst[i].Row]["addr"].ToString().Contains(tDplcLst[i].RelAddr.ToString())
                                && dataForm.Rows[tDplcLst[i].Row]["addr"].ToString().Contains(tDplcLst[i].StrArea)
                                )
                            {
                                
                                if (tDplcLst[i].BelongToDT != null)
                                {
                                    dataForm.Rows[tDplcLst[i].Row]["value"] = sValue;

                                    double ration = 1;

                                    if (double.TryParse(dataForm.Rows[tDplcLst[i].Row][Constant.strParam7].ToString(), out ration))
                                    {
                                        dataForm.Rows[tDplcLst[i].Row]["value"] = sValue;
                                        dataForm.Rows[tDplcLst[i].Row][Constant.strParam6] = (iValue / ration).ToString("0.00");
                                    }
                                }
                            }
                        }
                    }

                    return true;
                }

                return false;
            }
            catch (Exception ex)
            {
             
                return false;
            }
            
        }
       
        bool getMValueFromLst(List<XJPlcInfo> mPlst)
        {
            try
            {
                bool[] inputBools = master.ReadCoils((byte)Constant.DefaultUnitId, (ushort)mPlst[0].AbsAddr, (ushort)mPlst.Count);
              
                if (inputBools != null && inputBools.Count() == mPlst.Count)
                {
                    ErrorConnCount = 0;

                    for (int i = 0; i < mPlst.Count; i++)
                    {
                        if (IsRePackCmdReadDMDataOut) return true;

                        if (mPlst[i].Row >= dataForm.Rows.Count) continue;

                        if (mPlst[i].ByteValue != null && mPlst[i].ByteValue.Count() > 0)
                        {
                            mPlst[i].ByteValue[0] = Constant.tcpMValue;
                            mPlst[i].PlcValue = (inputBools[i] == true) ? 1 : 0;
                        }
                        else return false;

                        if (!dataForm.Rows[mPlst[i].Row]["value"].ToString().Equals(mPlst[i].PlcValue.ToString())
                        && !mPlst[i].IsInEdit)
                        {
                            string m10addr = mPlst[i].RelAddr.ToString();
                            if (mPlst[i].IntArea > Constant.HM_ID)
                                m10addr = ConstantMethod.GetXYAddr10To8(mPlst[i].RelAddr).ToString();

                            if (dataForm.Rows[mPlst[i].Row]["addr"].ToString().Contains(m10addr)
                                &&
                                dataForm.Rows[mPlst[i].Row]["addr"].ToString().Contains(mPlst[i].StrArea)
                                )
                            {
                                dataForm.Rows[mPlst[i].Row]["value"] = mPlst[i].PlcValue.ToString();
                                
                           }
                        }
                    }

                    return true;
                }

                return false;
               
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        void dataReading()
        {          
            #region  //处理D区
            if (DPlcInfo.Count > 0)
            {             
                    List<XJPlcInfo> tDplcLst = new List<XJPlcInfo>();
                    foreach (XJPlcInfo p in DPlcInfo)
                    {
                        if (IsRePackCmdReadDMDataOut) return;
                        if ((tDplcLst.Count > 0) && (
                            (tDplcLst[tDplcLst.Count - 1].IntArea != p.IntArea) ||
                            (p.AbsAddr - tDplcLst[tDplcLst.Count - 1].AbsAddr > 1) || DPlcInfo.IndexOf(p) == DPlcInfo.Count - 1))
                        {
                            //tDplcLst.Add(p);
                            getDValueFromLst(tDplcLst);
                            tDplcLst.Clear();
                            tDplcLst.Add(p);
                        }
                        else
                        {
                            tDplcLst.Add(p);
                        }
                    }

                   //这里考虑到 上面还遗留了最后一个 没有获取数据 所以这里要在进行操作一遍

                    if(tDplcLst.Count > 0)
                    {
                       getDValueFromLst(tDplcLst);
                    }
                    tDplcLst.Clear();
                    tDplcLst = null;
             }                           
            #endregion
            
            #region 处理M区 

            if (MPlcInfoAll.Count > 0)
            {
                foreach (List<XJPlcInfo> mPlst in MPlcInfoAll)
                {
                    if (IsRePackCmdReadDMDataOut) return;
                    getMValueFromLst(mPlst);
                  
                }              
            }
            #endregion

        
        }

        //设置两个定时器
        private void GetDataConnTimerEvent(object sender, EventArgs e)
        {
            GetDataConnTimer.Enabled = false;

            if (!IsRePackCmdReadDMDataOut)
            {
                IsReadingData = true;
                dataReading();
                IsReadingData = false;
            }
           
            GetDataConnTimer.Enabled = true;

        }
        void StartGetData()
        {
            if (Status == Constant.DeviceConnected)
            {
                GetDataConnTimer.Enabled = true;

                ErrorConnTimer.Enabled = true;
            }
        }
        #endregion

        #region 数据写入

        public bool SetMultipleDArea(int Addr, int count, int[] value, string Area)
        {
            if (Status == 0)
            {
                ushort[] uvalue;

                uvalue = Array.ConvertAll<int, ushort>(value, s => (ushort)s);
                try
                {
                    
                    master.WriteMultipleRegisters((byte)Constant.DefaultUnitId, 
                    (ushort)XJPLCPackCmdAndDataUnpack.AreaGetFromStr(Addr, Area), uvalue);
                    
                    return true;
                } 
                catch(Exception ex)
                {
                    return false;
                }
                               
            }
            else return false;
        }

        public bool SetMultipleMArea(int Addr, int count, int[] value, string Area)
        {
            if (Status == 0)
            {

                bool[] uvalue;

                uvalue = Array.ConvertAll<int, bool>(value, s => { if (s > 0) return true; else return false; });

                try
                {

                   
                    master.WriteSingleCoil((byte)Constant.DefaultUnitId,
                        (ushort)XJPLCPackCmdAndDataUnpack.AreaGetFromStr(Addr, Area),
                        uvalue[0]);
                    ConstantMethod.Delay(100);
                   
                }
                catch (Exception ex)
                {

                }

                return true;
            }
            else return false;
        }


        #endregion
    }
}

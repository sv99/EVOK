using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace xjplc
{   
    public class EvokXJDevice:XJDevice
    {

        public EvokXJDevice(List<string> strfile,PortParam p0) : base(strfile,p0)
        {

        }
        public EvokXJDevice(List<string> strfile, ServerInfo s0) : base(strfile, s0)
        {

        }
        public EvokXJDevice(List<string> strfile):base(strfile)
        {
             
        }
        public bool  SetDValue(PlcInfoSimple p,int value0)
        {
              
            if (p == null || p.BelongToDataform == null ||p.Area ==null || p.Mode ==null)
            {
               
                return false;
            }
            if (p != null && p.BelongToDataform !=null)
            {                           
                WriteSingleDData(p.Addr,value0,p.Area,p.Mode);
            }
            
            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0,ref p);
            if (p.Ration > 0)
                if (value0 == p.ShowValue * p.Ration) return true; else return false;
            else
            if (value0 == p.ShowValue * Constant.dataMultiple) return true;
            else return false;

        }

        public bool SetMValueON(PlcInfoSimple p)
        {

            if (p == null || p.BelongToDataform == null || p.Area == null || p.Mode == null)
            {
                return false;
            }
            int value0 = 1;
            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0, ref p);

            if (value0 == p.ShowValue) return true; else return false;
        }
        public bool SetMValueOFF(PlcInfoSimple p)
        {
            int value0 = 0;
            if (p != null && p.BelongToDataform != null) 
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0, ref p);

            if (value0 == p.ShowValue) return true; else return false;

        }
        public void SetMValueON2OFF(PlcInfoSimple p)
        {
            SetMValueON(p);
            ConstantMethod.Delay(200);
            SetMValueOFF(p);
        }
        public void SetMValueOFF2ON(PlcInfoSimple p)
        {
            SetMValueOFF(p);
            SetMValueON(p);
        }
        public void SetMValueOFF2ON2OFF(PlcInfoSimple p)
        {
            SetMValueON2OFF(p);         
        }

        public bool opposite(PlcInfoSimple p)
        {
            if (p.ShowValue > 0)
            {
                return SetMValueOFF(p);
            }
            else
            {
                return SetMValueON(p);
            }      
        }
        public bool SetMultiPleDValue(PlcInfoSimple stPlcInfoSimple, int[] value0)
        {
            if (stPlcInfoSimple != null && stPlcInfoSimple.BelongToDataform != null && value0.Count()>0)
            {
                bool writeok=
                WriteMultiPleDMData(
                    stPlcInfoSimple.Addr, 
                    value0, 
                    stPlcInfoSimple.Area, 
                    stPlcInfoSimple.Mode);
                //取消查看 在多次发送的时候会比较卡
              // LogManager.WriteProgramLog("数据下发地址："+ stPlcInfoSimple.Addr+"写入值："+ value0[0].ToString());
              ConstantMethod.DelayWriteCmdOk(300, ref value0[0], ref stPlcInfoSimple);
                //可能时间太久 要等下
                //LogManager.WriteProgramLog("数据下发地址：" + stPlcInfoSimple.Addr + "返回值：" + stPlcInfoSimple.ShowValue);

              if (stPlcInfoSimple.Ration > 0)
               {
                    int margin = Math.Abs(value0[0]) - Math.Abs((int)(stPlcInfoSimple.ShowValue * stPlcInfoSimple.Ration));
                    if (Math.Abs(margin)<1* stPlcInfoSimple.Ration)
                    { return true; }

               }

               if (value0[0] == stPlcInfoSimple.ShowValue) return true;
                
              return false;
                
                                                        
            }

            if(stPlcInfoSimple ==null) LogManager.WriteProgramLog(stPlcInfoSimple.Name+"数据下发地址：附属类为空！");
            if (stPlcInfoSimple != null && stPlcInfoSimple.BelongToDataform == null) LogManager.WriteProgramLog(stPlcInfoSimple.Name+"附属类所属表格为空！");
            if(value0.Count()==0 && stPlcInfoSimple !=null) LogManager.WriteProgramLog("数据下发地址：" + stPlcInfoSimple.Name+stPlcInfoSimple.Addr  + "写入数据数量错误：" + value0.Count().ToString());

            return false;
        }

    }
}

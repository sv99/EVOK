using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
namespace xjplc
{
    
    public class EvokDTDevice:DTDevice
    {
        public EvokDTDevice(List<string> strfile):base(strfile)
        {
             
        }
        public bool  SetDValue(DTPlcInfoSimple p,int value0)
        {

            if (p != null && p.BelongToDataform !=null)
            {                           
                WriteSingleDData(p.Addr,value0,p.Area,p.Mode);
            }
                      
            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0,ref p);

            if (value0 == p.ShowValue) return true; else return false;
        }

        public bool SetMValueON(DTPlcInfoSimple p)
        {
            int value0 = 1;
            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0, ref p);

            if (value0 == p.ShowValue) return true; else return false;
        }
        public bool SetMValueOFF(DTPlcInfoSimple p)
        {
            int value0 = 0;
            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(Constant.XJConnectTimeOut, ref value0, ref p);

            if (value0 == p.ShowValue) return true; else return false;
        }
        public void SetMValueON2OFF(DTPlcInfoSimple p)
        {
            SetMValueON(p);
            SetMValueOFF(p);
        }
        public void SetMValueOFF2ON(DTPlcInfoSimple p)
        {
            SetMValueOFF(p);
            SetMValueON(p);
        }
        public bool SetMultiPleDValue(DTPlcInfoSimple stDTPlcInfoSimple, int[] value0)
        {
            if (stDTPlcInfoSimple != null && stDTPlcInfoSimple.BelongToDataform != null)
            {
                WriteMultiPleDMData(
                    stDTPlcInfoSimple.Addr, 
                    value0, 
                    stDTPlcInfoSimple.Area, 
                    stDTPlcInfoSimple.Mode);
                ConstantMethod.DelayWriteCmdOk(Constant.WriteCommTimeOut, ref value0[0], ref stDTPlcInfoSimple);
                //可能时间太久 要等下 
                if (value0[0] == stDTPlcInfoSimple.ShowValue) return true; else return false;

            }
            return false;
        }

    }
}

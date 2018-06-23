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
        public EvokXJDevice(List<string> strfile):base(strfile)
        {
             
        }
        public void SetDValue(PlcInfoSimple p,int value0)
        {

            if (p != null && p.BelongToDataform !=null)
            {                           
                WriteSingleDData(p.Addr,value0,p.Area,p.Mode);
            }
                      
            ConstantMethod.DelayWriteCmdOk(300, ref value0,ref p);

        }

        public void SetMValueON(PlcInfoSimple p)
        {
            int value0 = 1;
            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }

            ConstantMethod.DelayWriteCmdOk(300, ref value0, ref p);
        }
        public void SetMValueOFF(PlcInfoSimple p)
        {
            int value0 = 0;
            if (p != null && p.BelongToDataform != null)
            {
                WriteSingleMData(p.Addr, value0, p.Area, p.Mode);
            }
        }
        public void SetMValueON2OFF(PlcInfoSimple p)
        {
            SetMValueON(p);
            SetMValueOFF(p);
        }
        public void SetMValueOFF2ON(PlcInfoSimple p)
        {
            SetMValueOFF(p);
            SetMValueON(p);
        }
        public void SetMultiPleDValue(PlcInfoSimple stPlcInfoSimple, int[] value0)
        {
            if (stPlcInfoSimple != null && stPlcInfoSimple.BelongToDataform != null)
            {
                WriteMultiPleDMData(
                    stPlcInfoSimple.Addr, 
                    value0, 
                    stPlcInfoSimple.Area, 
                    stPlcInfoSimple.Mode);
                ConstantMethod.DelayWriteCmdOk(300, ref value0[0], ref stPlcInfoSimple);

            }
        }

    }
}

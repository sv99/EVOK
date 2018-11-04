using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc
{
    //比较去重复PLCINFO
    public class ModelComparer : IEqualityComparer<XJPlcInfo>
    {
        public bool Equals(XJPlcInfo x, XJPlcInfo y)
        {
            return ((x.RelAddr == y.RelAddr) && (x.IntArea == y.IntArea));
        }
        public int GetHashCode(XJPlcInfo obj)
        {
            return obj.RelAddr.GetHashCode();
        }
    }

    public class ModelComparerDT : IEqualityComparer<DTPlcInfo>
    {
        public bool Equals(DTPlcInfo x, DTPlcInfo y)
        {
            return ((x.RelAddr == y.RelAddr) && (x.IntArea == y.IntArea));
        }
        public int GetHashCode(DTPlcInfo obj)
        {
            return obj.RelAddr.GetHashCode();
        }
    }
    public class ModelComparerDTTcpInfo : IEqualityComparer<DTTcpPlcInfo>
    {
        public bool Equals(DTTcpPlcInfo x, DTTcpPlcInfo y)
        {
            return ((x.RelAddr == y.RelAddr) && (x.IntArea == y.IntArea));
        }
        public int GetHashCode(DTTcpPlcInfo obj)
        {
            return obj.RelAddr.GetHashCode();
        }
    }

}

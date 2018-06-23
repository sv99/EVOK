using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc
{
    //比较去重复PLCINFO
    public class ModelComparer : IEqualityComparer<PlcInfo>
    {
        public bool Equals(PlcInfo x, PlcInfo y)
        {
            return ((x.RelAddr == y.RelAddr) && (x.IntArea == y.IntArea));
        }
        public int GetHashCode(PlcInfo obj)
        {
            return obj.RelAddr.GetHashCode();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xjplc
{
    public class evokWorkManager
    {
        List<EvokXJWork> evokWorkLst;
        public System.Collections.Generic.List<xjplc.EvokXJWork> EvokWorkLst
        {
            get { return evokWorkLst; }
            set { evokWorkLst = value; }
        }
        public evokWorkManager()
        {
            EvokWorkLst = new List<EvokXJWork>();
        }
    }
}

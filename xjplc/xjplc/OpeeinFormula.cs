using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc
{

   public  class OpeeinFormula
    {
        List<string> doorLst;
        public System.Collections.Generic.List<string> DoorLst
        {
            get { return doorLst; }
            set { doorLst = value; }
        }
        List<double> conditonRange;
        public System.Collections.Generic.List<double> ConditonRange
        {
            get { return conditonRange; }
            set { conditonRange = value; }
        }

        double width_offset;
        public double Width_offset
        {
            get { return width_offset; }
            set { width_offset = value; }
        }
        double height_offset;
        public double Height_offset
        {
            get { return height_offset; }
            set { height_offset = value; }
        }

        List<string> paramLst;
        public System.Collections.Generic.List<string> ParamLst
        {
            get { return paramLst; }
            set { paramLst = value; }
        }

        public OpeeinFormula(string fileName)
        {
            doorLst = new List<string>();
            conditonRange = new List<double>();
            paramLst = new List<string>();
        }

    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace xjplc
{
  
        public struct SimiPatternPoint
        {
            public PointDouble leftDown;
            public PointDouble leftUp;
            public PointDouble rightDown;
            public PointDouble rightUp;
        }



    public struct PointDouble
    {
        public double X;
        public double Y;
    }

}

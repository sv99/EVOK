using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace xjplc
{
    //一切从这里出发 socserver 负责修改这个worklst集合 传输到 socclient里 进行删除
    public class YBDTWorkManger
    {
        List<YBDTWork> ybdtWorkLst;
        SocServer socServer;
        public YBDTWorkManger()
        {
            ybdtWorkLst = new List<YBDTWork>();
            socServer = new SocServer();
            socServer.startconn_Click();
            socServer.YbWorkLst = ybdtWorkLst;

        }
    }
}

using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Sockets;
using System.Text;

namespace xjplc.delta
{
    //管理一堆设备的集合类  
   public class YBDTDevice : SocServerClient
    {
        DataTable dataForm;
        
        public YBDTDevice(Socket soc):base(soc)
        {
            dataForm = new DataTable();
            this.EventDataProcess += new commDataProcess(Dataprocess);
        }
        void Dataprocess(object sender, commEventArgs e)
        {

        }




    }
}

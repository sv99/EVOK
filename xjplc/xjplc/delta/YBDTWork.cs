using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using xjplc.delta;

namespace xjplc
{
    //每个work 有一个device 就是socclient啦
    public class YBDTWork
    {
        ExcelNpoi Excelop;
        DataTable UserDt;
        string UserDtFileName = string.Concat(
            Constant.AppFilePath,DateTime.Now.ToString("yyyMMdd"), ".xlsx");
        bool IsLoadData;

        YBDTDevice ybtdDevice;
        public xjplc.delta.YBDTDevice YbtdDevice
        {
            get { return ybtdDevice; }
            set { ybtdDevice = value; }
        }      
     
        public YBDTWork(Socket soc)
        {
            Excelop = new ExcelNpoi();
            UserDt = new DataTable();

            //加载数据如果不对 那就用户自己加载
            if (!LoadExcelData(UserDtFileName))
            {
                /***
                OpenFileDialog op = new OpenFileDialog();
                op.InitialDirectory = AppDomain.CurrentDomain.BaseDirectory;
                op.Filter = "文件(*.xls,*.xlsx)|*.xls;*.xlsx";
                op.FileName = "请选择数据文件";
                if (op.ShowDialog() == DialogResult.OK)
                {
                    if (!LoadExcelData(op.FileName))
                    {
                        MessageBox.Show(Constant.ErrorPlcFile);
                        ConstantMethod.AppExit();
                    }
                }   
                ***/           
            }

            YbtdDevice = new YBDTDevice(soc);                       

        }
        

        public bool LoadExcelData(string filename)
        {
        LogManager.WriteProgramLog(Constant.LoadFileSt + filename);
        if (!File.Exists(filename))
        {
            return false;
        }
         IsLoadData = true;
          UserDt = Excelop.ImportExcel(filename);

            if (UserDt.Rows.Count < 1) return false;

            string[] str = ConstantMethod.GetColumnsByDataTable(UserDt);

            if (str == null) return false;

            if (!ConstantMethod.compareString(str, Constant.strformatYB))
            {
                return false;
            }
          
            Excelop.FileName = filename;
          

            IsLoadData = false;

            LogManager.WriteProgramLog(Constant.LoadFileEd);
            return true;
        }

    }
}

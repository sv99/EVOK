using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using xjplc;
using System.Windows.Forms;
namespace zlzk
{
    class YBDTWork
    {
        ExcelNpoi Excelop;
        DataTable UserDt;
        string UserDtFileName = string.Concat(
            Constant.AppFilePath,DateTime.Now.ToString("yyyMMdd"), ".xlsx");
        bool IsLoadData;

        public YBDTWork()
        {
            Excelop = new ExcelNpoi();
            UserDt = new DataTable();

            //加载数据如果不对 那就用户自己加载
            if (!LoadExcelData(UserDtFileName))
            {
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
            }
        
     
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
